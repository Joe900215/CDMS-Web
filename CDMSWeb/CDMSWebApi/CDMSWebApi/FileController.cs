using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AVEVA.CDMS.Server;
using AVEVA.CDMS.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace AVEVA.CDMS.WebApi
{


    /// <summary>  
    /// DBSource操作类
    /// </summary>  
    public class FileController : Controller
    {
        //当前上传文件统计（统计正在上传文件块的文件）
        private static int curUploadFileCount = 0;
        /// <summary>
        /// 总上传带宽默认是1024000=1000KB/秒，范围在102400 到 102400000
        /// </summary>
        public static long UploadBpsTotal = 204800; //1024000;

        /// <summary>
        /// 比较时间是否相同
        /// </summary>
        /// <param name="datetime">字符串时间</param>
        /// <param name="dtime">时间类型</param>
        /// <returns>是否相同</returns>
        private static bool DateTimeEQ(string datetime, DateTime dtime)
        {
            DateTime sdtime = DateTime.Parse(datetime);
            if (sdtime.Equals(dtime))
                return true;
            else
                return false;
        }

        //public static JObject BeforeUploadFile(string sid, string DocKeyword, string ServerFileName, string CreateDate, string ModifyDate, Decimal FileSize, string MD5)
        //{
        //    return BeforeUploadFile(sid, DocKeyword, ServerFileName, CreateDate, ModifyDate, FileSize, MD5);
        //}

        /// <summary>
        /// 上传第一块文件块时触发，在上传文件，替换文件时使用
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="ObjectKeyword">对象关键字，可以是目录或者文档的关键字，可以为空则不判断权限</param>
        /// <param name="ServerFileName">服务器端文件相对路径加文件名称, 值由前面触发的CreateDoc函数获取，例如"upload\\abc.doc"</param>
        /// <param name="CreateDate">文件创建时间，网页端可以不传递，用于判断是否断点续传</param>
        /// <param name="FileSize">文件大小，必填，用于判断是否断点续传</param>
        /// <param name="ModifyDate">文件编辑时间，必填，用于判断是否断点续传</param>
        /// <param name="MD5">文件编辑时间，网页端可以不传递，用于判断是否断点续传</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功并且存储空间已经有同名文档时，data包含一个JObject，里面包含参数"state"(值为"seleSureReplace",提醒用户是否替换文件)，"fileName"(已有文件的文件名)，"fileSize"(已有文件的文件大小)，"updateTime"(已有文件的更新时间)</para>
        /// <para>操作成功并且上传成功时，success返回true,data包含一个空的JObject</para>
        /// <para>例子：</para>
        /// </returns>
        //public static JObject BeforeUploadFile(string sid, string DocKeyword, string ServerFileName, string CreateDate, string ModifyDate, string fileSize, string MD5)
        public static JObject BeforeUploadFile(string sid, string ObjectKeyword, string ServerFileName, string CreateDate, string ModifyDate, Decimal FileSize, string MD5)
        {
            JObject jo = new JObject();
            ExReJObject reJo = new ExReJObject();
            try
            {

                //0. 判断传入参数: SID, 服务器文件名称，最后编辑时间，文件大小 这几个参数不能为空
                if (string.IsNullOrEmpty(sid) ||
                    string.IsNullOrEmpty(ServerFileName) ||
                    FileSize <= 0)
                {
                    reJo.msg = "上传替换文件 " + ServerFileName + " 失败：传递参数有问题！";
                    reJo.success = false;
                    return reJo.Value;
                }



                //1.验证登录信息
                User curUser = DBSourceController.GetCurrentUser(sid);
                DBSource dbsource = curUser.dBSource;
                if (curUser == null || dbsource == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    reJo.success = false;
                    return reJo.Value;
                }



                //2.判断权限
                Doc doc = null;
                Project project = null;


                if (!string.IsNullOrEmpty(ObjectKeyword))
                {
                    object obj = dbsource.GetObjectByKeyWord(ObjectKeyword);

                    if (obj is Doc)
                    {
                        Doc ddoc = (Doc)obj;
                        doc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;

                        if (doc != null)
                        {
                            if (!DocController.GetDocDFWriteRight(doc, sid))
                            {
                                reJo.msg = "上传替换文件 " + ServerFileName + " 失败：用户没有替换文档权限！";
                                reJo.success = false;
                                return reJo.Value;
                            }

                            //如果是替换文档，并且没有替换文档的权限
                            if (doc.O_dmsstatus != (enDocStatus)2)
                            {
                                reJo.msg = "上传替换文件 " + ServerFileName + " 失败：文档不处于检入状态！";
                                reJo.success = false;
                                return reJo.Value;
                            }

                            project = doc.Project;
                        }
                    }
                    else if (obj is Project)
                    {
                        project = (Project)obj;

                        if (!ProjectController.GetProjectDCreateRight(project, sid))
                        {
                            reJo.msg = "上传文件 " + ServerFileName + " 失败：用户没有创建文档权限！";
                            reJo.success = false;
                            return reJo.Value;
                        }


                    }
                    else
                    {
                        reJo.msg = "上传文件 " + ServerFileName + " 失败：文档关键字参数错误！";
                        reJo.success = false;
                        return reJo.Value;
                    }
                }
                else
                {
                    //如果对象关键字为空，就判断是否上传数字签名，如果不是数字签名，就返回
                    if (ServerFileName.StartsWith("BMP/") && ServerFileName.ToLower().EndsWith(".jpg"))
                    {
                        //处理数字签名图片，这里无需处理，直接上传
                    }
                    else
                    {
                        reJo.msg = "上传文件 " + ServerFileName + " 失败：文档关键字参数错误！";
                        reJo.success = false;
                        return reJo.Value;
                    }
                }


                //组成服务器上全路径文件名称
                string FullFileName = "";
                if (ServerFileName.Contains(":") || ServerFileName.StartsWith("\\\\"))
                {
                    //用户指定文件
                    FullFileName = ServerFileName;
                }
                else if (project != null)
                {
                    //放置在存储下，每个Storage的另一个路径必须保存ISS的虚拟目录
                    FullFileName = project.Storage.O_protocol + (project.Storage.O_protocol.EndsWith("\\") || project.Storage.O_protocol.EndsWith("/") ? "" : "\\") + project.O_projectcode + "\\" + ServerFileName;
                }
                else
                {
                    //获取ISO文件
                    FullFileName = AppDomain.CurrentDomain.BaseDirectory + ServerFileName;
                }


                FullFileName = FullFileName.Replace("\\", "/");

                //创建目录
                try
                {
                    //取得要保存文件的路径
                    String FileDir = FullFileName.Substring(0, FullFileName.LastIndexOf("/"));

                    //查看路径是否存在,创建路径
                    if (!Directory.Exists(FileDir)) Directory.CreateDirectory(FileDir);
                }
                catch { }


                //3. 判断服务器上文件是否和客户端完全相同
                if (System.IO.File.Exists(FullFileName))
                {

                    //定义文件信息处理对象
                    FileInfo FileInfoMess = new FileInfo(FullFileName);

                    ////创建时间
                    //mess[0] = FileInfoMess.CreationTime.ToString();

                    ////文件大小信息
                    //mess[1] = FileInfoMess.Length.ToString();

                    ////文件修改时间
                    //mess[2] = FileInfoMess.LastWriteTime.ToString();


                    ////文件MD5码
                    //mess[3] = AVEVA.CDMS.Server.CDMS.getMD5Hash(fileName);


                    if (!string.IsNullOrEmpty(MD5))
                    {
                        //1.创建时间 2.大小 3.编辑时间 4.MD5</returns>
                        string[] fileattr = AVEVA.CDMS.Server.CDMS.getLocalFileMess(FullFileName);

                        //客户端文件的MD5与服务器上文件MD5相同，则不传输文件
                        if (MD5 == fileattr[3])
                        {
                            reJo.success = true;
                            reJo.data = new JArray(new JObject(new JProperty("resultValue", "-1"),//;//返回-1 表示服务器上存在，不需要传输文件
                            new JProperty("ServerFullFileName", FullFileName)));//给客户端返回服务端保存的完整文件路径,当存储空间已经有文件，
                                                                                //而DOC已经删除的情况下，返回服务器上的文件路径以便客户端更新文件信息
                            return reJo.Value;
                        }
                    }

                    //客户端没有获取到文件MD5，则比较创建时间，最后编辑时间和文件大小
                    else
                    {

                        //如果客户端文件的创建时间、编辑时间和大小与服务器上文件相同，则不传输
                        if (DateTimeEQ(ModifyDate, FileInfoMess.LastWriteTime) && FileInfoMess.Length == FileSize)
                        {
                            reJo.success = true;
                            reJo.data = new JArray(new JObject(new JProperty("resultValue", "-1"),//;//返回-1 表示服务器上存在，不需要传输文件
                            new JProperty("ServerFullFileName", FullFileName)));//给客户端返回服务端保存的完整文件路径,当存储空间已经有文件，
                                                                                //而DOC已经删除的情况下，返回服务器上的文件路径以便客户端更新文件信息
                            return reJo.Value;
                        }
                    }
                }


                //4. 如果传输文件存在，则判断是否断点续传 

                //记录客户端文件信息， 文件格式
                //创建时间
                //编辑时间
                //文件大小
                //MD5
                string AttributeFileName = FullFileName + ".Attr";

                //记录传输的数据, 每传输一块就在后面追加
                string DataFileName = FullFileName + ".Data";


                //中间文件存在
                if (System.IO.File.Exists(AttributeFileName) && System.IO.File.Exists(DataFileName))
                {

                    //读属性
                    string[] strs = System.IO.File.ReadAllLines(AttributeFileName);
                    if (strs[1] == ModifyDate && strs[2] == FileSize.ToString() && strs[3] == MD5)
                    {

                        //已经传输了一部分
                        reJo.success = true;

                        //定义文件信息处理对象
                        FileInfo FileInfoMess = new FileInfo(DataFileName);


                        //已经完成传输的文件需要是 512KB 整数，才断点续传
                        double number = FileInfoMess.Length / (512 * 1024);
                        if ((int)number == number)
                        {

                            reJo.data = new JArray(new JObject(
                                new JProperty("resultValue", FileInfoMess.Length.ToString()), //给客户端返回已传输的文件长度
                                new JProperty("ServerFullFileName", FullFileName)//给客户端返回服务端保存的完整文件路径
                                ));
                            return reJo.Value;
                        }

                    }
                }


                //删除临时文件
                try
                {
                    System.IO.File.Delete(AttributeFileName);
                    System.IO.File.Delete(DataFileName);
                }
                catch { }



                //5. 需要从头传输
                //记录客户端文件
                string[] Data = { CreateDate, ModifyDate, FileSize.ToString(), MD5 };
                System.IO.File.WriteAllLines(AttributeFileName, Data);

                //创建空文件
                FileStream NewFile = System.IO.File.Create(DataFileName);
                NewFile.Close();


                reJo.success = true;
                reJo.data = new JArray(new JObject(new JProperty("resultValue", "0"),                 //返回0, 表示全部重新传输文件
                                                   new JProperty("ServerFullFileName", FullFileName)  //给客户端返回服务端保存的完整文件路径
                                       ));
                return reJo.Value;


            }
            catch (Exception ex)
            {
                reJo.msg = "上传文件 " + ServerFileName + " 失败：" + ex.ToString();
                reJo.success = false;
                return reJo.Value;
            }

        }


        /// <summary>
        /// 继续上传文件块
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="ServerFullFileName">调用BeforeuploadFile函数后， 从BeforeuploadFile函数的返回里面获取到的， 创建的服务器上的文件名称</param>
        /// <param name="_HttpRequest">Http参数，系统自动生成，主要包含以下参数：Files:文件列表，Files里面只有一个文件, 文件大小512K, 客户端将大文件切块传输</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功并且上传成功时，success返回true,data包含一个空的JObject</para>
        /// </returns>
        public static JObject UploadFile(string sid, string ServerFullFileName, HttpRequestBase _HttpRequest)
        {
            JObject jo = new JObject(); ExReJObject reJo = new ExReJObject();
            try
            {

                curUploadFileCount = curUploadFileCount + 1;
                int chunkSize = 128 * 1024;
                //控制上传流量带宽，设置文件上传延时
                long bpsTotal = 1024000;
                if (UploadBpsTotal <= 102400000 && UploadBpsTotal >= 102400)
                {
                    bpsTotal = UploadBpsTotal;
                }

                //1. 检查参数
                if (_HttpRequest.Files.Count == 0)
                {
                    reJo.msg = "文档参数错误：文档数量为零！";
                    return reJo.Value;
                }
                HttpPostedFileBase file = _HttpRequest.Files[0];     //第一个文件对象
                if (file.ContentLength <= 0)
                {
                    reJo.msg = "文档参数错误：文件内容不能为空！";
                    return reJo.Value;
                }
                if (sid == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

                //登录用户
                User curUser = DBSourceController.GetCurrentUser(sid);
                if (curUser == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

                //2. 检查临时文件是否存在

                //记录客户端文件信息， 文件格式
                //创建时间
                //编辑时间
                //文件大小
                //MD5
                string AttributeFileName = ServerFullFileName + ".Attr";

                //记录传输的数据, 每传输一块就在后面追加
                string DataFileName = ServerFullFileName + ".Data";


                //临时文件存在
                if (System.IO.File.Exists(AttributeFileName) && System.IO.File.Exists(AttributeFileName))
                {

                    //读属性
                    string[] strs = System.IO.File.ReadAllLines(AttributeFileName);

                    //写文件内容
                    using (FileStream fsw = new FileStream(DataFileName, FileMode.Append, FileAccess.Write))
                    {
                        BinaryWriter bw = new BinaryWriter(fsw);

                        Stream reader = file.InputStream;//打开一个文件读取流信息，将其写入新文件

                        byte[] buffer = new byte[chunkSize];
                        int c = 0;

                        //文件合并 
                        while ((c = reader.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            bw.Write(buffer, 0, c);
                            bw.Flush();//清理缓冲区
                        }
                        reader.Close();
                        reader.Dispose();

                        bw.Close();
                        bw.Dispose();

                        fsw.Close();
                        fsw.Dispose();
                    }



                    //判断文件大小及MD5
                    FileInfo FileInfoMess = new FileInfo(DataFileName);//定义文件信息处理对象



                    if (long.Parse(strs[2]) == FileInfoMess.Length)
                    {
                        bool Finish = false;
                        if (!string.IsNullOrEmpty(strs[3]))
                        {
                            //1.创建时间 2.大小 3.编辑时间 4.MD5</returns>
                            string[] fileattr = AVEVA.CDMS.Server.CDMS.getLocalFileMess(DataFileName);

                            //MD5相同，则说明传输成功
                            if (fileattr[3] == strs[3])
                            {
                                Finish = true;
                            }
                        }
                        else
                        {

                            //客户端没有传MD5，则比较大小相同，则认为传输成功
                            Finish = true;
                        }


                        //传输成功
                        if (Finish)
                        {

                            //修改文件属性
                            try
                            {
                                if (!string.IsNullOrEmpty(strs[0])) System.IO.File.SetCreationTime(DataFileName, DateTime.Parse(strs[0]));
                                if (!string.IsNullOrEmpty(strs[1])) System.IO.File.SetLastWriteTime(DataFileName, DateTime.Parse(strs[1]));
                            }
                            catch { }

                            //如果原来的文件存在, 并且上传的文件比原文件小，需要备份原来的文件
                            if (System.IO.File.Exists(ServerFullFileName))
                            {
                                //去除原来的文件只读属性
                                FileAttributes attributes = System.IO.File.GetAttributes(ServerFullFileName);
                                if ((attributes & FileAttributes.ReadOnly).Equals(FileAttributes.ReadOnly))
                                {
                                    System.IO.File.SetAttributes(ServerFullFileName, attributes & ~FileAttributes.ReadOnly);
                                }


                                //判断文件大小
                                FileInfo oldFileInfoMess = new FileInfo(ServerFullFileName);//定义文件信息处理对象
                                if (oldFileInfoMess.Length > FileInfoMess.Length)
                                {
                                    //修改文件名称
                                    System.IO.File.Move(ServerFullFileName, ServerFullFileName + "(Delete)" + System.DateTime.Now.ToString("yyMMddHHmmss"));
                                }
                                else
                                {
                                    //删除保存传输信息的文件
                                    System.IO.File.Delete(ServerFullFileName);
                                }
                            }

                            //去除上传文件的只读属性
                            FileAttributes attributesUpFile = System.IO.File.GetAttributes(DataFileName);
                            if ((attributesUpFile & FileAttributes.ReadOnly).Equals(FileAttributes.ReadOnly))
                            {
                                System.IO.File.SetAttributes(DataFileName, attributesUpFile & ~FileAttributes.ReadOnly);
                            }

                            //修改文件名称
                            System.IO.File.Move(DataFileName, ServerFullFileName);



                            //删除保存传输信息的文件
                            System.IO.File.Delete(AttributeFileName);


                            //A. 文件传输完成
                            reJo.success = true;
                            reJo.msg = "FINISH"; //表示传输完成
                            return reJo.Value;
                        }
                    }

                    //// **********  正常状态一定要删除这段代码
                    ////调试代码：正常传输的时候要去掉,如果文件未传输完成，返回错误代码，提示客户端断点续传（这里加入测试代码）
                    //else if (long.Parse(strs[2]) > FileInfoMess.Length){

                    //    ////测试文件未传完时断点续传
                    //    ////当已传送文件大小大于1MB的时候，返回一个错误
                    //    //if (FileInfoMess.Length > 1024 * 1024)
                    //    //{
                    //    //    reJo.msg = "RESUMEFILE"; //表示重新开始断点续传
                    //    //    return reJo.Value;
                    //    //}

                    //    //测试重新上传文件
                    //    //当已传送文件大小大于1MB的时候，返回一个错误
                    //    if (FileInfoMess.Length == 1024 * 1024)
                    //    {

                    //        //删除临时文件重新传输
                    //        try
                    //        {
                    //            System.IO.File.Delete(AttributeFileName);
                    //            System.IO.File.Delete(DataFileName);
                    //        }
                    //        catch { }


                    //        //B. 传输错误，需要重新上传
                    //        reJo.msg = "REUPLOAD"; //表示传输完成
                    //        return reJo.Value;
                    //    }
                    //}

                    //如果传输的文件大于客户端文件，则说明传输出现错误，需要重新传输
                    else if (long.Parse(strs[2]) < FileInfoMess.Length)
                    {

                        //删除临时文件重新传输
                        try
                        {
                            System.IO.File.Delete(AttributeFileName);
                            System.IO.File.Delete(DataFileName);
                        }
                        catch { }


                        //B. 传输错误，需要重新上传
                        reJo.msg = "REUPLOAD"; //表示传输完成
                        return reJo.Value;

                    }


                    //C: 数据块传输成功
                    #region 添加延时以控制上传流量

                    //添加延时以控制上传流量
                    //每个文件上传的流量
                    long bps = bpsTotal / curUploadFileCount;
                    //每个文件每秒可以上传的流量除以块的大小，得到每秒可以写入多少个块
                    long chunk_bps = bps / chunkSize;
                    //1000毫秒除以每秒写入的块，得到延时时间
                    int curDelay = 1000 / Convert.ToInt32(chunk_bps);
                    System.Threading.Thread.Sleep(curDelay);
                    #endregion

                    reJo.success = true;
                    return reJo.Value;
                }
            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
                //reJo.msg = e.Message;
                reJo.msg = "RESUMEFILE"; //表示重新开始断点续传
            }
            finally
            {
                if (curUploadFileCount >= 1)
                    curUploadFileCount = curUploadFileCount - 1;
            }
            return reJo.Value;
        }

        /// <summary>
        /// 文件上传完毕后触发事件
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="DocKeyword">文档对象关键字</param>
        /// <param name="ServerFullFileName">调用BeforeuploadFile函数后， 创建的服务器上的文件名称</param>
        /// <returns></returns>
        public static JObject AfterUploadFile(string sid, string DocKeyword, string ServerFullFileName)
        {
            ExReJObject reJo = new ExReJObject();
            try 
            {

                //1.验证登录信息
                User curUser = DBSourceController.GetCurrentUser(sid);
                if (curUser == null )
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    reJo.success = false;
                    return reJo.Value;
                }

                DBSource dbsource = curUser.dBSource;
                if (dbsource == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    reJo.success = false;
                    return reJo.Value;
                }

                if (!string.IsNullOrEmpty(DocKeyword))
                {
                    Doc ddoc = dbsource.GetDocByKeyWord(DocKeyword);
                    Doc doc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;

                    ////获取文件大小
                    System.IO.FileInfo f = new FileInfo(ServerFullFileName);
                    if (f != null && doc != null)
                    {
                        doc.O_filename = f.Name;//替换文件时，修改文件名为已上传文件
                        doc.O_size = (int)f.Length;

                        //修改文件MD5
                        if (System.IO.File.Exists(ServerFullFileName))
                        {
                            string fileMd5 = FileMD5.GetMD5HashFromFile(ServerFullFileName);
                            if (!string.IsNullOrEmpty(fileMd5))
                            {
                                doc.O_suser2 = fileMd5;
                            }
                        }

                        doc.Modify();
                    }

                    #region 根据用户设置的条件，筛选需要显示的文档
                    //根据用户设置的条件，筛选需要显示的文档
                    foreach (WebFileEvent.After_Upload_File_Event_Class AfterUploadFileEventClass in WebFileEvent.ListAfterUploadFile)
                    {
                        if (AfterUploadFileEventClass.Event != null)
                        {
                            //如果在用户事件中筛选过了，就跳出事件循环
                            if (AfterUploadFileEventClass.Event(AfterUploadFileEventClass.PluginName, doc))
                            {
                                break;
                            }
                        }
                    }
                    #endregion
                }
                reJo.data = new JArray(new JObject(new JProperty("state", "uploadSuccess")));
                reJo.success = true;
            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
                reJo.msg = e.Message;
            }
            return reJo.Value;
        }
    }
}