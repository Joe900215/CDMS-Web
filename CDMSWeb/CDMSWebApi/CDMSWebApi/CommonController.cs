using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AVEVA.CDMS.Server;
using System.Runtime.Serialization;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace AVEVA.CDMS.WebApi
{
    public class CommonController : Controller
    {
        //
        private static string contentType = "application/x-www-form-urlencoded";
        private static string accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/x-silverlight, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, application/x-ms-application, application/x-ms-xbap, application/vnd.ms-xpsdocument, application/xaml+xml, application/x-silverlight-2-b1, */*";
        private static string userAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/5.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E; Zune 4.7; BOIE9;ZHCN)";
        public static string referer = "http://ui.ptlogin2.qq.com/cgi-bin/login?appid=1006102&s_url=http://id.qq.com/index.html";

        /// <summary>
        /// 获取HTML
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookieContainer"></param>
        /// <param name="method">GET or POST</param>
        /// <param name="postData">like "username=admin&password=123"</param>
        /// <returns></returns>
        //public static string GetHtml(string url, CookieContainer cookieContainer, string method = "GET", string postData = "")
        public static string GetHtml(string url, CookieContainer cookieContainer, string method = "GET", string postData = "")
        {
            HttpWebRequest httpWebRequest = null;
            HttpWebResponse httpWebResponse = null;
            try
            {
                httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                httpWebRequest.CookieContainer = cookieContainer;
                httpWebRequest.ContentType = contentType;
                httpWebRequest.Referer = referer;
                httpWebRequest.Accept = accept;
                httpWebRequest.UserAgent = userAgent;
                httpWebRequest.Method = method;
                httpWebRequest.ServicePoint.ConnectionLimit = int.MaxValue;

                if (method.ToUpper() == "POST")
                {
                    byte[] byteRequest = Encoding.Default.GetBytes(postData);
                    Stream stream = httpWebRequest.GetRequestStream();
                    stream.Write(byteRequest, 0, byteRequest.Length);
                    stream.Close();
                }

                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
                string html = streamReader.ReadToEnd();

                streamReader.Close();
                responseStream.Close();

                httpWebRequest.Abort();
                httpWebResponse.Close();

                return html;
            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
                return string.Empty;
            }
        }




        [HttpGet]
        /// <summary>
        /// 比较客户端和服务器上文件是否相同
        /// </summary>
        /// <param name="ServeFile">服务器端文件</param>
        /// <param name="CreateDate">客户端文件：创建时间</param>
        /// <param name="filesize">客户端文件：大小</param>
        /// <param name="lastmodify">客户端文件：最后编辑时间</param>
        /// <param name="md5">客户端文件：文件MD5</param>
        /// <returns>返回值：0_表示文件完全相同 -1 客户端文件不存在  1_表示服务器端文件不存在 2_表示客户端文件比服务器端文件小，表示服务器端文件需要备份 3_文件不相同</returns>
        private CompareFileType CompareFile(string ServeFile, string CreateDate, string filesize, string lastmodify, string md5)
        {
            try
            {
                string[] fileinfo = AVEVA.CDMS.Server.CDMS.getLocalFileMess(ServeFile);
                if (fileinfo[0] == CreateDate && fileinfo[1] == filesize && fileinfo[2] == lastmodify && fileinfo[3] == md5)
                {
                    return CompareFileType.Equal;
                }
                else if (int.Parse(filesize) < int.Parse(fileinfo[1]))
                    return CompareFileType.LocalSmall;
                else if (string.IsNullOrEmpty(fileinfo[0]))
                    return CompareFileType.ServerNotExist;
            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
            }


            return CompareFileType.Different;
        }

        /// <summary>
        /// 获取服务器上的文件属性:1.创建时间 2.大小 3.编辑时间 4.MD5
        /// </summary>
        /// <param name="serverfile">存储上的文件名称</param>
        /// <returns>返回 String[], 1.创建时间 2.大小 3.编辑时间 4.MD5</returns>
        private string[] GetServerFileInfo(string serverfile)
        {
            if (System.IO.File.Exists(serverfile))
                return AVEVA.CDMS.Server.CDMS.getLocalFileMess(serverfile);
            else
                return null;
        }



        /// <summary>
        /// 设置服务器上文件属性：创建时间和最后编辑时间
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="CreateTime">创建时间</param>
        /// <param name="ModifyTime">最后编辑时间</param>
        private void SetServerFileAttr(string fileName, string CreateTime, string ModifyTime)
        {
            try
            {
                System.IO.File.SetCreationTime(fileName, DateTime.Parse(CreateTime));
                System.IO.File.SetLastWriteTime(fileName, DateTime.Parse(ModifyTime));
            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
            }

        }

                   

        /// <summary>
        /// 设置服务器上文件的名称
        /// </summary>
        /// <param name="filename">文件名称（包含路径）</param>
        /// <param name="CreateDate">创建时间</param>
        /// <param name="ModiyDate">最后编辑时间</param>
        [HttpPost]
        private void SetFileAttr(string fileName, DateTime CreateDate, DateTime ModiyDate)
        {
            try
            {
                System.IO.File.SetCreationTime(fileName, CreateDate);
                System.IO.File.SetLastWriteTime(fileName, ModiyDate);
            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
            }

        }

        public static JArray NullToJson(string msg)//msg:查询出错消息
        {
            //JArray可以包含 JObject， JObject可以包含JProperty，JProperty可以包含JArray
            JArray retuJson = new JArray(
                new JObject { 
                        new JProperty("result","fail"),//添加查询失败标志
                    },
                new JObject { 
                        new JProperty("msg",msg),//添加查询出错消息
                    }
                    );
            //生成后格式:
            //[
            //  {"result":"fail"},
            //  {"msg":"出错消息代码"},
            //]
            return retuJson;
        }

        public static JObject NullToJObject(string msg)//msg:查询出错消息
        {
            //JArray可以包含 JObject， JObject可以包含JProperty，JProperty可以包含JArray

            JObject retuJson = new JObject { 
                        new JProperty("result","fail"),//添加查询失败标志
                        new JProperty("msg",msg),//添加查询出错消息
                    };
            //生成后格式:
            //[
            //  {"result":"fail"},
            //  {"msg":"出错消息代码"},
            //]
            return retuJson;
        }

        //public static JArray TranStringToJson(string json)
        public static JArray TranStringToJson(JArray ja)
        {

            //JArray可以包含 JObject， JObject可以包含JProperty，JProperty可以包含JArray
            JArray retuJson = new JArray(
                new JObject { 
                        new JProperty("result","success"),//添加查询成功标志
                    },
                new JObject { 
                        new JProperty("msg",ja),//把消息内容填入第二个JObject 
                    }
                    );
            //生成后格式:
            //[
            // {"result":"success"},
            // {"msg":[
            //          {"K":"ESPDBP133335SLocal","I":133335,"R":true,"D":"AP1000__公用文件","S":0},
            //          {"K":"ESPDBP134354SLocal","I":134354,"R":true,"D":"AP1000 LFCDMS__核电设计管理软件使用说明","S":0}
            //        ]
            //  }
            // ]
            //
            //在这里 "K":"ESPDBP133335SLocal" 和 "result":"success"都是 JProperty
            //JProperty 添加大括号后就是JObject ，{"result":"success"} 和  {"K":"ESPDBP133335SLocal","I":133335,"R":true,"D":"AP1000__公用文件","S":0} 都是 JObject
            //JObject 添加中括号[]后就是JArray

            return retuJson;
        }

    
        /// <summary>
        /// 加密关键字（用于自动登录）
        /// </summary>
        /// <returns>字符串</returns>
        private static string mRTStrs;

        internal static string mGetString_mStrs()
        {
            if (mRTStrs == null)
            {

                byte[] mStrs = new byte[52];
                mStrs[0] = 0x6f;
                mStrs[1] = 0x73;
                mStrs[2] = 0x6b;
                mStrs[3] = 0x6c;
                mStrs[4] = 0x65;
                mStrs[5] = 0x75;
                mStrs[6] = 0x30;
                mStrs[7] = 0x32;
                mStrs[8] = 0x37;
                mStrs[9] = 0x68;
                mStrs[10] = 0x33;
                mStrs[11] = 0x67;
                mStrs[12] = 0x73;
                mStrs[13] = 0x77;
                mStrs[14] = 0x6e;
                mStrs[15] = 0x38;
                mStrs[16] = 0x32;
                mStrs[17] = 0x39;
                mStrs[18] = 0x6e;
                mStrs[19] = 0x33;
                mStrs[20] = 0x67;
                mStrs[21] = 0x76;
                mStrs[22] = 0x33;
                mStrs[23] = 0x76;
                mStrs[24] = 0x73;
                mStrs[25] = 0x38;
                mStrs[26] = 0x73;
                mStrs[27] = 0x6a;
                mStrs[28] = 0x72;
                mStrs[29] = 0x69;
                mStrs[30] = 0x6f;
                mStrs[31] = 0x33;
                mStrs[32] = 0x6d;
                mStrs[33] = 0x71;
                mStrs[34] = 0x6e;
                mStrs[35] = 0x32;
                mStrs[36] = 0x37;
                mStrs[37] = 0x76;
                mStrs[38] = 0x31;
                mStrs[39] = 0x71;
                mStrs[40] = 0x31;
                mStrs[41] = 0x6d;
                mStrs[42] = 0x32;
                mStrs[43] = 0x62;
                mStrs[44] = 0x73;
                mStrs[45] = 0x34;
                mStrs[46] = 0x35;
                mStrs[47] = 0x61;
                mStrs[48] = 0x6e;
                mStrs[49] = 0x73;
                mStrs[50] = 0x61;
                mStrs[51] = 0x30;

                mRTStrs = System.Text.Encoding.UTF8.GetString(mStrs);
            }
            return mRTStrs;
        }

        internal  static JObject SidError()
        {
            return new JObject { 
                new JProperty("success",true),
                new JProperty("total",0),
                new JProperty("msg","sidError"),
                new JProperty("data",new JArray())
            };
        }

        //线程锁 
        private static Mutex muxConsole = new Mutex();
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="info">信息</param>
        /// <returns></returns>
        public static string WebWriteLog(string info)
        {
            //线程锁 
            muxConsole.WaitOne();
            try {
                string logPath = System.Web.HttpContext.Current.Server.MapPath("~/CDMSError.log");

                StreamWriter sw = new StreamWriter(logPath, true);
                sw.WriteLine(System.DateTime.Now + ": " + info);
                sw.Close();

                //if (!System.IO.File.Exists(@logPath))
                //{
                //    FileStream stream = System.IO.File.Create(@logPath);
                //    if (stream != null)
                //    {
                //        stream.Close();
                //    }
                //}
 
                ////取当前时间
                // string strCurTime = System.DateTime.Now.ToString();
                // System.IO.File.AppendAllText(@logPath, strCurTime + " : " + info + "\r\n", Encoding.Unicode);
            }
            catch{}
            //解锁
            muxConsole.ReleaseMutex();
            return "true";
        }

    }
}
