using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AVEVA.CDMS.Server;
using AVEVA.CDMS.Common;
using AVEVA.CDMS.WebApi;
using System.Runtime.Serialization;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
//using LinqToDB;

namespace AVEVA.CDMS.HXEPC_Plugins
{
    public class MeetMinutesCN
    {
        //GetDraftMeetMinutesCNDefault

        /// <summary>
        /// 获取创建会议纪要表单的默认配置
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="ProjectKeyword"></param>
        /// <returns></returns>
        public static JObject GetDraftMeetMinutesCNDefault(string sid, string ProjectKeyword, string DraftOnProject)
        {
            return DraftGetDefault.GetDefaultInfo(sid, ProjectKeyword, DraftOnProject);

            //ExReJObject reJo = new ExReJObject();
            //try
            //{
            //    User curUser = DBSourceController.GetCurrentUser(sid);
            //    if (curUser == null)
            //    {
            //        reJo.msg = "登录验证失败！请尝试重新登录！";
            //        return reJo.Value;
            //    }

            //    DBSource dbsource = curUser.dBSource;
            //    if (dbsource == null)
            //    {
            //        reJo.msg = "登录验证失败！请尝试重新登录！";
            //        return reJo.Value;
            //    }

            //    Project m_Project = dbsource.GetProjectByKeyWord(ProjectKeyword);

            //    if (m_Project == null)
            //    {
            //        reJo.msg = "参数错误！文件夹不存在！";
            //        return reJo.Value;
            //    }

            //    #region 初始化变量
            //    //获取发文单位列表
            //    JObject joSendCompany = new JObject();

            //    //获取收文单位列表
            //    JObject joRecCompany = new JObject();

            //    string DocNumber = "";// 设置编号

            //    string groupCode = "", groupKeyword = "", groupType = "", sourceCompany = "";

            //    //获取项目号
            //    string RootProjectCode = ""; string RootProjectDesc = ""; string ProjectDesc = "";

            //    //是否在项目起草
            //    bool bDraftOnProject = true;
            //    #endregion

            //    //获取目录的信息，包括项目所在目录的代码，描述，当前目录的描述等
            //    DraftGetDefault.GetProjectInfo(m_Project, ref ProjectDesc, ref RootProjectCode, ref RootProjectDesc, ref bDraftOnProject);


            //    //获取所有项目部门
            //    DraftGetDefault.GetDepartmentInfo(dbsource, ref joRecCompany, ref joSendCompany);

            //    if (bDraftOnProject)
            //    {
            //        //获取项目通信代码
            //        DraftGetDefault.GetProjectShoreInfo("NOT", m_Project,
            //             ref sourceCompany, ref joRecCompany, ref joSendCompany);

            //    }

            //    //获取登录用户的用户组，所在部门等信息
            //    DraftGetDefault.GetUserGroupInfo("NOT", dbsource, ref groupCode,
            //        ref groupKeyword, ref groupType, ref sourceCompany);

            //    string auditorList = "", auditorDesc = "";
            //    //从组织机构获取部长助理（副部长）
            //    DraftGetDefault.GetAuditUserInfo("NOT", dbsource, groupCode, "部长助理",
            //        ref auditorList, ref auditorDesc);


            //    JObject joData = new JObject(
            //        new JProperty("RootProjectCode", RootProjectCode),
            //        new JProperty("DocNumber", DocNumber),
            //        new JProperty("RecCompanyList", joRecCompany),
            //        new JProperty("SendCompanyList", joSendCompany),
            //        new JProperty("SourceCompany", sourceCompany),
            //         new JProperty("GroupKeyword", groupKeyword),
            //        new JProperty("GroupType", groupType),
            //        new JProperty("AuditorList", auditorList),
            //         new JProperty("AuditorDesc", auditorDesc),
            //         new JProperty("RootProjectDesc", RootProjectDesc),
            //          new JProperty("ProjectDesc", ProjectDesc)
            //        );


            //    reJo.data = new JArray(joData);
            //    reJo.success = true;
            //    return reJo.Value;
            //}
            //catch (Exception e)
            //{
            //    reJo.msg = e.Message;
            //    CommonController.WebWriteLog(reJo.msg);
            //}
            //return reJo.Value;
        }

        //线程锁 
        internal static Mutex muxConsole = new Mutex();

        /// <summary>
        /// 起草会议纪要
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="ProjectKeyword"></param>
        /// <param name="DocAttrJson"></param>
        /// <returns></returns>
        public static JObject DraftMeetMinutesCN(string sid, string ProjectKeyword, string CataAttrJson, 
            string DocAttrJson, string FileListJson)
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
                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(DocAttrJson);

                Project m_Project = dbsource.GetProjectByKeyWord(ProjectKeyword);

                //定位到发文目录
                //m_Project = LocalProject(m_Project);

                if (m_Project == null)
                {
                    reJo.msg = "参数错误！文件夹不存在！";
                    return reJo.Value;
                }

                #region 获取项目参数项目

                //获取项目参数项目
                string fileCode = "", sendCode = "",//documentCode = "",
                    sendDate = "", totalPages = "",
                    mainFeeder = "", copyParty = "", title = "",
                    meetTime = "", meetPlace = "", hostUnit = "",
                    moderator = "", participants = "", content = "",
                    approvpath = "", nextStateUserList = "",fileId="";



                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();

                    //获取文件编码
                    if (strName == "fileCode") fileCode = strValue.Trim();

                    //获取发文编号
                    else if (strName == "sendCode") sendCode = strValue.Trim();

                    //获取发文日期
                    else if (strName == "sendDate") sendDate = strValue;
                    //获取页数
                    else if (strName == "totalPages") totalPages = strValue;

                    //获取主送
                    if (strName == "mainFeeder") mainFeeder = strValue.Trim();
                    //获取抄送
                    else if (strName == "copyParty") copyParty = strValue;

                    //获取会议主题
                    else if (strName == "title") title = strValue;
                    //获取会议时间
                    else if (strName == "meetTime") meetTime = strValue;
                    //获取会议地点
                    else if (strName == "meetPlace") meetPlace = strValue;

                    //获取主办单位
                    else if (strName == "hostUnit") hostUnit = strValue;
                    //获取主持人
                    else if (strName == "moderator") moderator = strValue;
                    //获取参会单位与人员
                    else if (strName == "participants") participants = strValue;

                    //获取会议内容
                    else if (strName == "content") content = strValue;

                    //获取审批路径
                    else if (strName == "approvpath") approvpath = strValue;

                    //获取下一状态人员
                    else if (strName == "nextStateUserList") nextStateUserList = strValue;

                    //获取文件ID
                    else if (strName == "fileId") fileId = strValue;

                }


                if (string.IsNullOrEmpty(fileCode))
                {
                    reJo.msg = "请填写文件编号！";
                    return reJo.Value;
                }

                else if (string.IsNullOrEmpty(sendCode))
                {
                    reJo.msg = "请填写发文编码！";
                    return reJo.Value;
                }
                else if (string.IsNullOrEmpty(title))
                {
                    reJo.msg = "请填写会议主题！";
                    return reJo.Value;
                }
                else if (string.IsNullOrEmpty(nextStateUserList))
                {
                    reJo.msg = "请选择校审人员！";
                    return reJo.Value;
                }
                else if (string.IsNullOrEmpty(meetTime))
                {
                    reJo.msg = "请填写会议时间！";
                    return reJo.Value;
                }
                else if (string.IsNullOrEmpty(meetPlace))
                {
                    reJo.msg = "请填写会议地点！";
                    return reJo.Value;
                }
                else if (string.IsNullOrEmpty(hostUnit))
                {
                    reJo.msg = "请填写主办单位！";
                    return reJo.Value;
                }
                else if (string.IsNullOrEmpty(moderator))
                {
                    reJo.msg = "请填写主持人！";
                    return reJo.Value;
                }
                else if (string.IsNullOrEmpty(participants))
                {
                    reJo.msg = "请填写参会单位与人员！";
                    return reJo.Value;
                }
                else if (string.IsNullOrEmpty(content))
                {
                    reJo.msg = "请填写会议内容！";
                    return reJo.Value;
                }
                if (string.IsNullOrEmpty(fileId))
                {
                    reJo.msg = "请填写文件ID！";
                    return reJo.Value;
                }
                #endregion


                #region 获取文件列表
                List<LetterAttaFile> attaFiles = new List<LetterAttaFile>();
                if (!string.IsNullOrEmpty(FileListJson))
                {
                    int index = 0;
                    JArray jaFiles = (JArray)JsonConvert.DeserializeObject(FileListJson);

                    foreach (JObject joAttr in jaFiles)
                    {
                        string strFileName = joAttr["fn"].ToString();//文件名
                        string strFileCode = joAttr["fc"].ToString();//文件编码
                        string strFileDesc = joAttr["fd"].ToString();//文件题名
                        string strFilePage = joAttr["fp"].ToString();//页数
                        string strEdition = joAttr["fe"].ToString();//版次
                        string strSeculevel = joAttr["sl"].ToString();//密级
                        string strFileState = joAttr["fs"].ToString();//状态
                        string strRemark = joAttr["fr"].ToString();//备注

                        index++;
                        string strIndex = index.ToString();
                        LetterAttaFile afItem = new LetterAttaFile()
                        {
                            No = strIndex,
                            Name = strFileName,
                            Code = strFileCode,
                            Desc = strFileDesc,
                            Page = strFilePage,
                            Edition = strEdition,
                            Seculevel = strSeculevel,
                            Status = strFileState,
                            Remark = strRemark
                        };

                        attaFiles.Add(afItem);
                    }
                }
                #endregion

                #region 根据会议纪要模板，生成会议纪要文档

                //获取立项单文档所在的目录
                //Project m_Project = m_NewProject;

                List<TempDefn> docTempDefnByCode = m_Project.dBSource.GetTempDefnByCode("CATALOGUING");
                TempDefn docTempDefn = (docTempDefnByCode != null && docTempDefnByCode.Count > 0) ? docTempDefnByCode[0] : null;
                if (docTempDefn == null)
                {
                    reJo.msg = "没有与其相关的模板管理，创建无法正常完成";
                    return reJo.Value;
                }

                IEnumerable<string> source = from docx in m_Project.DocList select docx.Code;

                //会议纪要的文件命名是文件编码，与信函不一样
                string filename = fileCode + " " + title;
                if (source.Contains<string>(filename))
                {
                    for (int i = 1; i < 0x3e8; i++)
                    {
                        filename = fileCode + " " + title + i.ToString();
                        if (!source.Contains<string>(filename))
                        {
                            //reJo.msg = "新建会议纪要出错！";
                            //return reJo.Value;
                            break;
                        }
                    }
                }

                //文档名称
                Doc docItem = m_Project.NewDoc(filename + ".docx", filename, "", docTempDefn);
                if (docItem == null)
                {
                    reJo.msg = "新建会议纪要出错！";
                    return reJo.Value;
                }

                #endregion

                #region 获取著录表属性内容
                CataloguDoc caDoc = new CataloguDoc();
                caDoc.doc = docItem;

                DocumentCommon.SetGataloguDocAttr(caDoc, CataAttrJson);

                #endregion

                #region 设置文会议纪要文档附加属性

                //文档模板
                caDoc.CA_ATTRTEMP = "MEETINGSUMMARY";
                //函件编号
                caDoc.CA_FILECODE = fileCode;
                //会议主题
                caDoc.CA_TITLE = title;
                //会议时间
                caDoc.CA_TIME = meetTime;
                //函件编号
                caDoc.CA_SENDCODE = sendCode;
                //发文日期
                caDoc.CA_SENDDATE = sendDate;
                //主送
                caDoc.CA_MAINFEEDER = mainFeeder;
                //页数
                caDoc.CA_PAGE = totalPages;
                //抄送
                caDoc.CA_COPY = copyParty;
                //校审级数（审批路径）
                caDoc.CA_SERIES = approvpath;

                //文件ID
                caDoc.CA_FILEID = fileId;

                caDoc.SaveAttrData();
              //AttrData data;


              //  //文文档模板
              //  if ((data = docItem.GetAttrDataByKeyWord("CA_ATTRTEMP")) != null)
              //  {
              //      data.SetCodeDesc("MEETINGSUMMARY");
              //  }

              //  //函件编号
              //  if ((data = docItem.GetAttrDataByKeyWord("ME_FILECODE")) != null)
              //  {
              //      data.SetCodeDesc(fileCode);
              //  }

              //  //会议主题
              //  if ((data = docItem.GetAttrDataByKeyWord("ME_TITLE")) != null)
              //  {
              //      data.SetCodeDesc(title);
              //  }

              //  //会议时间
              //  if ((data = docItem.GetAttrDataByKeyWord("ME_TIME")) != null)
              //  {
              //      data.SetCodeDesc(meetTime);
              //  }

              //  //函件编号
              //  if ((data = docItem.GetAttrDataByKeyWord("ME_SENDCODE")) != null)
              //  {
              //      data.SetCodeDesc(sendCode);
              //  }

              //  //发文日期
              //  if ((data = docItem.GetAttrDataByKeyWord("ME_SENDDATE")) != null)
              //  {
              //      data.SetCodeDesc(sendDate);
              //  }

              //  //主送
              //  if ((data = docItem.GetAttrDataByKeyWord("ME_MAINSEND")) != null)
              //  {
              //      data.SetCodeDesc(mainFeeder);
              //  }

              //  //页数
              //  if ((data = docItem.GetAttrDataByKeyWord("ME_PAGE")) != null)
              //  {
              //      data.SetCodeDesc(totalPages);
              //  }

              //  //抄送
              //  if ((data = docItem.GetAttrDataByKeyWord("ME_COPY")) != null)
              //  {
              //      data.SetCodeDesc(copyParty);
              //  }


                ////保存项目属性，存进数据库
                //docItem.AttrDataList.SaveData();

                #endregion

                #region 录入数据进入word表单

                string strDocList = "";//获取附件

                //录入数据进入表单
                Hashtable htUserKeyWord = new Hashtable();


                //格式化日期
                DateTime senddate = Convert.ToDateTime(sendDate);

                string strSenddate = senddate.ToShortDateString().ToString().Replace("-", ".").Replace("/", ".");


                htUserKeyWord.Add("HEADERCODE", fileCode);//页眉里面的发文编码

                htUserKeyWord.Add("SENDDATE", strSenddate);//发送日期
                //htUserKeyWord.Add("PAGE", totalPages);//页数

                htUserKeyWord.Add("MAINFEEDER", mainFeeder);//主送
                //htUserKeyWord.Add("DOCUMENTCODE", documentCode);//发文编码

                htUserKeyWord.Add("COPY", copyParty);//抄送方

                htUserKeyWord.Add("TITLE", title);//标题
                htUserKeyWord.Add("MEETTIME", meetTime);//会议时间
                htUserKeyWord.Add("MEETPLACE", meetPlace);//会议地点

                htUserKeyWord.Add("HOSTUNIT", hostUnit);//主办单位
                htUserKeyWord.Add("MODERATOR", moderator);//主持人
                htUserKeyWord.Add("PARTICIPANTS", participants);//参会单位与人员

                htUserKeyWord.Add("CONTENT", content);//会议内容

                //电子签
                htUserKeyWord["PREPAREDSIGN"] = curUser.O_username;
                htUserKeyWord["DRAFTTIME"] = DateTime.Now.ToString("yyyy.MM.dd");

                #region 添加附件
                //添加附件
                List<string> list3 = new List<string>();
                foreach (LetterAttaFile file in attaFiles)
                {

                    list3.Add(file.No);
                    list3.Add(file.Code);
                    list3.Add(file.Desc);
                    list3.Add(file.Page);
                    list3.Add(file.Edition);
                    //list3.Add(file.Seculevel);
                    //list3.Add(file.Status);
                    list3.Add(file.Remark);
                }

                //用htAuditDataList传送附件列表到word
                Hashtable htAuditDataList = new Hashtable();
                //word里面表格关键字的设置公式(不需要加"$()") ：表格关键字+":"+已画好表格线的行数+":"+表格列数
                //例如关键字是"DRAWING",画了一行表格线，从第二行起画表格线,每行有6列，则公式是："DRAWING:1:6"
                htAuditDataList.Add("DRAWING", list3); 
                #endregion

                string workingPath = m_Project.dBSource.LoginUser.WorkingPath;


                try
                {
                    //上传下载文档
                    string exchangfilename = "会议纪要中文模板";

                    //获取网站路径
                    string sPath = System.Web.HttpContext.Current.Server.MapPath("/ISO/HXEPC/");

                    //获取模板文件路径
                    string modelFileName = sPath + exchangfilename + ".docx";

                    //获取即将生成的联系单文件路径
                    string locFileName = docItem.FullPathFile;

                    FileInfo info = new FileInfo(locFileName);

                    if (System.IO.File.Exists(modelFileName))
                    {
                        //如果存储子目录不存在，就创建目录
                        if (!Directory.Exists(info.Directory.FullName))
                        {
                            Directory.CreateDirectory(info.Directory.FullName);
                        }

                        //复制模板文件到存储目录，并覆盖同名文件
                        System.IO.File.Copy(modelFileName, locFileName, true);


                        //线程锁 
                        muxConsole.WaitOne();
                        try
                        {
                            //把参数直接写进office
                            CDMSWebOffice office = new CDMSWebOffice
                            {
                                CloseApp = true,
                                VisibleApp = false
                            };
                            office.Release(true);
                            office.WriteDataToDocument(docItem, locFileName, htUserKeyWord, htAuditDataList);
                        }
                        catch { }
                        finally
                        {

                            //解锁
                            muxConsole.ReleaseMutex();
                        }
                    }


                    int length = (int)info.Length;
                    docItem.O_size = new int?(length);
                    docItem.Modify();


                    if (string.IsNullOrEmpty(strDocList))
                    {
                        strDocList = docItem.KeyWord;
                    }
                    else
                    {
                        strDocList = docItem.KeyWord + "," + strDocList;
                    }

                    //这里刷新数据源，否则创建流程的时候获取不了专业字符串
                    DBSourceController.RefreshDBSource(sid);

                    reJo.success = true;
                    //reJo.data = new JArray(new JObject(new JProperty("ProjectKeyword", docItem.Project.KeyWord),
                    //    new JProperty("DocKeyword", docItem.KeyWord), new JProperty("DocList", strDocList)));
                    reJo.data = new JArray(new JObject(new JProperty("ProjectKeyword", docItem.Project.KeyWord),
                new JProperty("DocKeyword", docItem.KeyWord), new JProperty("DocList", strDocList),
                new JProperty("DocCode", docItem.Code)));
                    return reJo.Value;
                }
                catch { }
                #endregion

                reJo.data = new JArray(new JObject(new JProperty("projectKeyword", m_Project.KeyWord)));
                reJo.success = true;
                return reJo.Value;

                //AVEVA.CDMS.WebApi.DBSourceController.RefreshDBSource(sid);


            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(reJo.msg);
            }

            return reJo.Value;
        }

        public static JObject GetMeetMinutesCNNumber(string sid, string ProjectCode, string SendCompany, string RecCompany)
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


                //string runNum = getDocNumber(dbsource,ProjectCode, SendCompany, RecCompany);
                string runNum = Document.getDocTempNumber(dbsource, "S", "MOM"); //, SendCompany, RecCompany);
                if (string.IsNullOrEmpty(runNum)) runNum = "SMOM" + "00001";
                reJo.success = true;
                reJo.data = new JArray(new JObject(new JProperty("RunNum", runNum)));



            }
            catch (Exception exception)
            {
                WebApi.CommonController.WebWriteLog(exception.Message + "\r\n" + exception.Source + "\r\n" + exception.StackTrace);
                //AssistFun.PopUpPrompt(exception.Message + "\r\n" + exception.Source + "\r\n" + exception.StackTrace);
                reJo.msg = "获取会议纪要编号失败！" + exception.Message + "\r\n" + exception.Source + "\r\n" + exception.StackTrace;
            }
            return reJo.Value;
        }


    }
}
