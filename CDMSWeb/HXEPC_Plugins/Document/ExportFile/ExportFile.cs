using AVEVA.CDMS.Server;
using AVEVA.CDMS.WebApi;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web;
using System.Web.UI;

namespace AVEVA.CDMS.HXEPC_Plugins
{
    /// <summary>
    /// 功能： 输出选择的树节点目录
    /// 思路：1.获取选择的节点，然后遍历目录。
    ///       2.在服务器上创建目录文件夹/文件，然后打包（压缩整个文件夹）
    ///       3.然后传文件压缩包给前端下载
    ///       4.删除第2步服务器上创建的 目录文件夹/文件
    /// </summary>
    public class ExportFile
    {
        public static JObject ExportFolder(string sid, string ProjectKeyword, string ConditionalAttrJson)
        {

            ExReJObject reJo = new ExReJObject();
            try
            {
                User curUser = DBSourceController.GetCurrentUser(sid);
                if (curUser == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

                DBSource dbsource = curUser.dBSource;
                if (dbsource == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }
                JObject reJoData = new JObject();

                var Parentproject = dbsource.GetProjectByKeyWord(ProjectKeyword);

                //存放用户选的条件信息
                Dictionary<string, string> paramDictionary = new Dictionary<string, string>();

                if (!string.IsNullOrEmpty(ConditionalAttrJson))//条件
                {
                    JArray jaAttr = (JArray)JsonConvert.DeserializeObject(ConditionalAttrJson);

                    foreach (JObject joAttr in jaAttr)
                    {
                        string strName = joAttr["name"].ToString();
                        string strValue = joAttr["value"].ToString();

                        //获取生成的起始日期
                        if (strName == "stardata") paramDictionary.Add("stardata", strValue);//stardata = strValue; 
                        //获取生成的终止日期
                        else if (strName == "enddata") paramDictionary.Add("enddata", strValue);//enddata = strValue;
                        //获取发送方
                        else if (strName == "sendCompany") paramDictionary.Add("sendCompany", strValue);//sendCompany = strValue;
                        //获取接收方
                        else if (strName == "recCompany") paramDictionary.Add("recCompany", strValue);//recCompany = strValue;
                        //是否回复
                        //else if (strName == "reply") { if (strValue == "1") { paramDictionary.Add("reply", "是"); } else { paramDictionary.Add("reply", "否"); } }
                        else if (strName == "reply") { if (strValue == "1") { paramDictionary.Add("reply", "是"); } else if (strValue == "0") { paramDictionary.Add("reply", "否"); } else  if (strValue == "") { paramDictionary.Add("reply", ""); } else { paramDictionary.Add("reply", "是或否"); } }
                    }
                }


                string currPath = System.Web.HttpContext.Current.Server.MapPath("~");//AppDomain.CurrentDomain.BaseDirectory; //

                string proname = Parentproject.ToString; // Parentproject.Description == "" ?  Parentproject.O_projectname : Parentproject.Code+"__"+ Parentproject.Description;

                //递归创建目录和 copy文件
                var result = CreateFile(Parentproject, currPath, paramDictionary);//在根目录下创建子目录


                var filePath = System.Web.HttpContext.Current.Server.MapPath("~") + proname; //主文件夹地址
                var zippath = filePath + ".zip";//压缩包放的地址
                //将创建的文件夹打包成压缩包
                ZipClass zip = new ZipClass();
                var resultzip = zip.Zip(filePath, proname);


                if (Directory.Exists(filePath))
                {
                    //true为递归删除子文件内容
                    Directory.Delete(filePath, true);//压缩完后删除原文件夹
                }
                #region 这种是 window.open(url, '_parent');//在父窗口打开链接 的下载方式。本导出没有用这种方法。用的是form表单正常提交，然后流下载的方法。（要删除临时压缩包之类的，好操作）
                var prepath = urlconvertor(zippath);
                reJo.data = new JArray(new JObject(new JProperty("prePath", prepath),//文件下载地址
                            new JProperty("filename", proname + ".zip")//文件名
                    //new JProperty("lastModified", lastModified),//最后修改时间
                    //new JProperty("fileMD5", fileMd5)
                            ));
                #endregion

                DownStream(zippath, proname);//流下载
                //reJo.success = true;
                return reJo.Value;
            }
            catch (Exception e)
            {
                reJo.success = false;
                reJo.msg = e.Message;
                CommonController.WebWriteLog(reJo.msg);

            }
            return reJo.Value;
        }



        //本地路径转换成URL相对路径
        private static string urlconvertor(string imagesurl1)
        {
            string tmpRootDir = System.Web.HttpContext.Current.Server.MapPath(System.Web.HttpContext.Current.Request.ApplicationPath.ToString());//获取程序根目录
            string imagesurl2 = imagesurl1.Replace(tmpRootDir, ""); //转换成相对路径
            imagesurl2 = imagesurl2.Replace(@"\", @"/");
            //imagesurl2 = imagesurl2.Replace(@"Aspx_Uc/", @"");
            return imagesurl2;
        }

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="serverPath">压缩文件物理地址</param>
        protected static void DownStream(string serverPath, string name)
        {
            string fileName = name + "_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".zip";//客户端保存的文件名
            string filePath = serverPath;//压缩文件物理路径


            ////以字符流的形式下载文件,流方式下载文件不能超过400M
            //FileStream fs = new FileStream(filePath, FileMode.Open);
            //byte[] bytes = new byte[(int)fs.Length];
            //fs.Read(bytes, 0, bytes.Length);
            //fs.Close();
            //HttpContext.Current.Response.ContentType = "application/octet-stream";
            ////通知浏览器下载文件而不是打开
            //HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8));
            //File.Delete(serverPath);//删除压缩后的文件
            //// HttpContext.Current.Response.Write("{success:true}");
            //HttpContext.Current.Response.BinaryWrite(bytes);
            //HttpContext.Current.Response.Flush();
            //HttpContext.Current.Response.End();

            //WriteFile实现下载
            FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.Exists == true)
            {
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ClearContent();
                HttpContext.Current.Response.ClearHeaders();
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8));
                HttpContext.Current.Response.AddHeader("Content-Length", fileInfo.Length.ToString());
                HttpContext.Current.Response.AddHeader("Content-Transfer-Encoding", "binary");
                HttpContext.Current.Response.ContentType = "application/octet-stream";
                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("gb2312");
                HttpContext.Current.Response.WriteFile(fileInfo.FullName);
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.End();
            }
            File.Delete(serverPath);//删除压缩后的文件
        }



        /// <summary>
        /// 递归在服务器上创建临时用的树目录（文件夹）。（后续打包压缩后删除文件夹）
        /// </summary>
        /// <param name="Parentproject"></param>
        /// <param name="currPath"></param>
        /// <returns></returns>
        public static int CreateFile(Project Parentproject, string currPath, Dictionary<string, string> paramDictionary)
        {
            if (Parentproject != null)
            {
                int docTotal = Parentproject.DocList.Count;// DocCount;
                var subPath = "";
                //检查是否存在文件夹
                string proname = Parentproject.ToString;
                subPath = currPath + "/" + proname + "/";
                if (false == System.IO.Directory.Exists(subPath))
                {
                    //创建文件夹
                    System.IO.Directory.CreateDirectory(subPath);
                }
                if (docTotal > 0)//如果包含文件，copy到新建的文件夹来
                {
                    DateTime? starDateTime = null;
                    DateTime? endDateTime = null;

                    if (!string.IsNullOrEmpty(paramDictionary["stardata"]))
                    {
                        starDateTime = DateTime.ParseExact(paramDictionary["stardata"], "yyyy年MM月dd日", System.Globalization.CultureInfo.CurrentCulture);//00时00分00秒
                        if (!string.IsNullOrEmpty(paramDictionary["enddata"]))
                        {
                            endDateTime = DateTime.ParseExact(paramDictionary["enddata"], "yyyy年MM月dd日", System.Globalization.CultureInfo.CurrentCulture);
                            endDateTime = new DateTime(endDateTime.Value.Year, endDateTime.Value.Month, endDateTime.Value.Day, 23, 59, 59);//23时59分59秒
                        }
                    }
                    List<Doc> projDocList = new List<Doc>();
                    var tempprojDocList = Parentproject.DocList;


                    if (Parentproject.DocList != null && Parentproject.DocList.Count > 0 && starDateTime != null && endDateTime != null)
                    {
                        //时间筛选
                        tempprojDocList = Parentproject.DocList.FindAll(t => ((DateTime)t.O_credatetime >= starDateTime && (DateTime)t.O_credatetime <= endDateTime));
                    }

                    if (tempprojDocList != null && tempprojDocList.Count > 0)
                    {
                        //著录属性筛选
                        var filterlist = tempprojDocList;
                        if (!String.IsNullOrEmpty(paramDictionary["sendCompany"]))
                        {
                            if (filterlist != null && filterlist.Count > 0)
                                filterlist = filterlist.FindAll(t => (t.GetAttrDataByKeyWord("CA_SENDERCODE") == null ? "" : t.GetAttrDataByKeyWord("CA_SENDERCODE").ToString) == paramDictionary["sendCompany"]);
                        }

                        if (!String.IsNullOrEmpty(paramDictionary["recCompany"]))
                        {
                            if (filterlist != null && filterlist.Count > 0)
                                filterlist = filterlist.FindAll(t => (t.GetAttrDataByKeyWord("CA_MAINFEEDERCODE") == null ? "" : t.GetAttrDataByKeyWord("CA_MAINFEEDERCODE").ToString) == paramDictionary["recCompany"]);
                        }
                        if (!String.IsNullOrEmpty(paramDictionary["reply"]))
                        {
                            //if (filterlist != null && filterlist.Count > 0)
                            //    filterlist = filterlist.FindAll(t => (t.GetAttrDataByKeyWord("CA_IFREPLY") == null ? "" : t.GetAttrDataByKeyWord("CA_IFREPLY").ToString) == paramDictionary["reply"]);
                            if (paramDictionary["reply"] == "是或否")
                            {
                                if (filterlist != null && filterlist.Count > 0)
                                    filterlist = filterlist.FindAll(t => (t.GetAttrDataByKeyWord("CA_IFREPLY") == null ? "" : t.GetAttrDataByKeyWord("CA_IFREPLY").ToString) == "是" || (t.GetAttrDataByKeyWord("CA_IFREPLY") == null ? "" : t.GetAttrDataByKeyWord("CA_IFREPLY").ToString) == "否");
                            }
                            else
                            {
                                if (filterlist != null && filterlist.Count > 0)
                                    filterlist = filterlist.FindAll(t => (t.GetAttrDataByKeyWord("CA_IFREPLY") == null ? "" : t.GetAttrDataByKeyWord("CA_IFREPLY").ToString) == paramDictionary["reply"]);
                            }

                        }

                        if (filterlist != null && filterlist.Count > 0)
                        {
                            projDocList.AddRange(filterlist);
                        }
                    }

                    foreach (var doc in projDocList)
                    {
                        //文件全称
                        string str = doc.O_filename.ToUpper();

                        //获取文件路径
                        string locFileName = doc.FullPathFile;

                        //文件路径不为空
                        if (!string.IsNullOrEmpty(locFileName))
                        {
                            FileInfo file = new FileInfo(locFileName);
                            if (file.Exists)
                            {
                                //文件位置
                                file.CopyTo(subPath + str, true);
                            }
                        }
                    }
                }
                foreach (var childproject in Parentproject.ChildProjectList)
                {
                    if (childproject != null)
                    {
                        var result = CreateFile(childproject, subPath, paramDictionary);
                    }
                }
            }
            return 1;
        }


    }
}
