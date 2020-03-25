using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using AVEVA.CDMS.Server;
using System.Threading;

namespace AVEVA.CDMS.WebApi
{
    //文件下载防盗链
    public class DaoLian : IHttpHandler
    {
        //当前下载文件统计
        private static int curDownloadFileCount=0;
        /// <summary>
        /// 总下载带宽默认是1024000=1000KB/秒，范围在102400 到 102400000
        /// </summary>
        public static long DownloadBpsTotal = 204800; //1024000; //

        public DaoLian()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        #region IHttpHandler 成员

        public bool IsReusable
        {
            get { return true; }
        }

        //响应IIS处理程序映射，实现文件下载防盗链功能
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                // 获取文件服务器端物理路径
                string FileName = context.Server.MapPath(context.Request.FilePath);
                string contentType = MimeMapping.GetMimeMapping(FileName);

                //预览pdf时，需要把pdf传过来的字符修改一遍
                string pPara = context.Request["p"];
                pPara = pPara.Replace(" ", "%2B").Replace("+", "%2B");

                string para = AVEVA.CDMS.WebApi.EnterPointController.ProcessRequest(pPara);

                //string para = AVEVA.CDMS.WebApi.EnterPointController.ProcessRequest(context.Request["p"]);

                 long startBytes = 0;

                //如果是断点续传下载文件，从Range里面获取已经下载的文件块大小
                //if (HttpContext.Current.Request.Headers["Range"] != null)
                 if (context.Request.Headers["Range"] != null)
                {
                    context.Response.StatusCode = 206;
                    string[] range = context.Request.Headers["Range"].Split(new char[] { '=', '-' });
                    startBytes = Convert.ToInt64(range[1]);
                }

                string[] sArray = Regex.Split(para, "___", RegexOptions.IgnoreCase);
                bool downRight = true;

                if (sArray.Length != 3)
                { downRight = false; }
                else
                {
                    string sid = sArray[0];
                    string DocKeyword = sArray[1];
                    string strdt = sArray[2];
                    DateTime dt = Convert.ToDateTime(strdt);

                    TimeSpan span = (TimeSpan)(DateTime.Now - dt);


                    //限制一小时内下载文件
                    if (span.TotalMilliseconds > 3600000)
                    {
                        downRight = false;
                    }
                    else
                    {
                        //判断用户是否已登录
                        User curUser = DBSourceController.GetCurrentUser(sid);
                        if (curUser == null)
                        {
                            downRight = false;
                        }
                        else
                        {
                            DBSource dbsource = curUser.dBSource;
                            
                            if (string.IsNullOrEmpty(DocKeyword))
                            {
                                downRight = false;
                            }
                            else
                            {
                                Doc ddoc = dbsource.GetDocByKeyWord(DocKeyword);
                                if (ddoc == null)
                                {
                                    downRight = false;
                                }
                                else
                                {
                                    Doc doc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;
                                    //如果下载的文件名对不上，也不允许下载，防止用同一个下载链接参数下载其他文件
                                    if (FileName.IndexOf(doc.O_filename)<0)
                                    {
                                        //pdf预览时，允许下载
                                        if (FileName.IndexOf(Path.GetFileNameWithoutExtension(doc.O_filename) + ".pdf") < 0) { 
                                            downRight = false;
                                        }
                                    }
                                    if (downRight == true)
                                    {
                                        //判断是否有读文件权限
                                        downRight = DocController.GetDocDFReadRight(doc, curUser);
                                    }
                                }
                            }
                        }
                    }
                }

                if (downRight == false)
                {
                    //没有有文件下载权限，跳转到错误提示页
                    HttpResponse Response = context.Response;
                    Response.Redirect("~/errorDown.aspx");
                }
                else {
                    //确定有文件下载权限，把文件输出到客户端浏览器
                    context.Response.ContentType = contentType;
                    //context.Response.WriteFile(FileName);

                    //文件名转码
                    //FileName = HttpClient.EncodeUrl(FileName);
                    WriteStreamFile(context,FileName, startBytes);
                }
            }
            catch (Exception ex) {
                //CommonController.WebWriteLog("ProcessRequest:"+ex.Message);
            }
        }

        /// <summary>
        /// 使用OutputStream.Write分块下载文件，参数为文件绝对路径
        /// </summary>
        /// <param name="filePath"></param>
        //public static void DownLoadFile(HttpContext context,string filePath)
        public static void WriteStreamFile(HttpContext context, string filePath,long startBytes)
        {
            //string filePath = MapPathFile(FileName);
            //指定块大小
            long chunkSize = 204800;
            //建立一个200K的缓冲区
            byte[] buffer = new byte[chunkSize];
            //已读的字节数
            long dataToRead = 0;
            //控制下载流量带宽，设置文件下载延时
            long bpsTotal = 1024000;
            if (DownloadBpsTotal <= 102400000 && DownloadBpsTotal>=102400) {
                bpsTotal = DownloadBpsTotal;
            }

            FileStream stream = null;
            try
            {
                
                AddCurDownloadFileCount(1);

                //打开文件
                stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                //如果是断点续传下载文件,把文件定位到已经下载的文件块位置
                stream.Position = startBytes;

                //获取需要传送的文件块大小 = 文件总长度减去已传送的文件块大小
                dataToRead = stream.Length - startBytes;

                //添加Http头

                context.Response.ContentType = "application/octet-stream";


                //string UserAgent = context.Request.ServerVariables["http_user_agent"].ToLower();
                var uAgent= context.Request.ServerVariables["http_user_agent"];
                string UserAgent = uAgent == null ? "" : uAgent.ToLower();


                if (UserAgent.ToLower().IndexOf("msie") >= 0) //如果是IE浏览器  
                {
                    string encoFileName = HttpClient.EncodeUrl(Path.GetFileName(filePath));
                    context.Response.AddHeader("Content-Disposition", "attachement;filename=" + encoFileName);
                }
                else if (UserAgent.ToLower().IndexOf("firefox") >=0) //如果是火狐浏览器  
                {
                    context.Response.AddHeader("Content-Disposition", "attachement;filename*=utf8' '" + Path.GetFileName(filePath));
                }
                else
                {
                    context.Response.AddHeader("Content-Disposition", "attachement;filename=" + Path.GetFileName(filePath));
                }

                context.Response.AddHeader("Content-Length", dataToRead.ToString());

                int dtrIndex = 0;
                List<long> dtlList = new List<long>();
                int errCount = 0;

                while (dataToRead > 0)
                {
                    #region 防止用户下载大文件时按下取消
                    //防止用户下载大文件时按下取消
                    if (dtrIndex >= 3 && dataToRead == dtlList[dtrIndex - 2])
                    {
                        errCount = errCount + 1;
                        if (errCount > 10)
                        {
                            break;
                        }
                        System.Threading.Thread.Sleep(1000);
                    }
                    else {
                        errCount = 0;
                    } 
                    #endregion
                    
                    //HttpContext.Current表式当前HttpContext实例
                    //if (HttpContext.Current.Response.IsClientConnected)
                    if (context.Response.IsClientConnected)
                    {
                        int length = stream.Read(buffer, 0, Convert.ToInt32(chunkSize));

                        context.Response.OutputStream.Write(buffer, 0, length);
                        context.Response.Flush();
                        //context.Response.Clear();

                        buffer = new Byte[chunkSize];
                        dataToRead = dataToRead - length;
                        dtlList.Add(dataToRead);
                    }
                    else
                    {
                        //防止client失去连接
                        dataToRead = -1;
                    }
                    dtrIndex = dtrIndex + 1;

                    #region 添加延时以控制下载流量
                    //dtrIndex > 5这里不能设置太小，否则文件下载不了
                    if (dtrIndex > 5 && curDownloadFileCount > 0)
                    {
                        //添加延时以控制下载流量
                        //每个文件下载的流量
                        long bps = bpsTotal / curDownloadFileCount;
                        //每个文件每秒可以下载的流量除以块的大小，得到每秒可以写入多少个块
                        long chunk_bps = bps / chunkSize;
                        //1000毫秒除以每秒写入的块，得到延时时间
                        int chun_bps_int = Convert.ToInt32(chunk_bps);
                        //防止chun_bps_int等于零时无法所有用户下载文件问题
                        int curDelay = ((chun_bps_int == 0) ? 1000 : 1000 / chun_bps_int);
                        System.Threading.Thread.Sleep(curDelay);
                    }
                    #endregion

                }
            }
            catch (Exception ex)
            {
                
                //HttpContext.Current.Response.Write("Error:" + ex.Message);
                CommonController.WebWriteLog("WriteFile:"+ex.Message);
                throw ex;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
                context.Response.Close();
                context.Response.End();

                AddCurDownloadFileCount(-1);
                //if (curDownloadFileCount>=1)
                //    curDownloadFileCount = curDownloadFileCount - 1;
            }
        }

        //线程锁 
        private static Mutex muxConsole = new Mutex();

        private static void AddCurDownloadFileCount(int val) {


            //线程锁 
            muxConsole.WaitOne();
            if (val < 0)
            {
                if (curDownloadFileCount + val >= 0)
                    curDownloadFileCount = curDownloadFileCount + val;
            }
            else if (val > 0) {
                curDownloadFileCount = curDownloadFileCount + val;
            }
            //解锁
            muxConsole.ReleaseMutex();
        }

        #endregion
    }

}
