using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Linq;
using System.IO;
using AVEVA.CDMS.Server;
using AVEVA.CDMS.WebApi;
using System.Text.RegularExpressions;
using System.Threading;
//using LinqToDB;

namespace AVEVA.CDMS.HXEPC_Plugins
{
    public class CloseDocument
    {
        public static bool Close(WorkFlow wf, WorkStateBranch wsb, bool bNeedGeneDistReceipt, bool bNeedFeedback)
        {
            try
            {

                CataloguDoc caDoc = new CataloguDoc();
                caDoc.doc = CommonFunction.GetWorkFlowDoc(wf);
                string replyDocCode = "";

                if (caDoc.CA_IFREPLY == "是")
                {
                    //回文编码占号
                    replyDocCode = SetPalindromeCod(caDoc);
                }

                if (bNeedGeneDistReceipt)
                {
                    //生成办文单
                    Doc fbDoc = GeneDistReceipt(wf, wsb, replyDocCode);

                    if (bNeedFeedback)
                    {
                        //发送反馈消息
                        sendFeedbackMsg(wf, wsb, fbDoc);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
            }
            return false;

        }

        //发送反馈消息

        internal static bool sendFeedbackMsg(WorkFlow wf, WorkStateBranch wsb, Doc fbDoc)
        {
            try
            {

                List<User> mtoUser = new List<User>();

                //添加发文流程发文部分的人员
                //文控
                AddToWSUserList(mtoUser, wf, "SECRETARIL");

                //编制人
                AddToWSUserList(mtoUser, wf, "DESIGN");

                //编制人
                AddToWSUserList(mtoUser, wf, "CHECK");

                //审核人
                AddToWSUserList(mtoUser, wf, "AUDIT");

                //审定人
                AddToWSUserList(mtoUser, wf, "AUDIT2");

                //批准人
                AddToWSUserList(mtoUser, wf, "APPROV");

                if (mtoUser.Count > 0)
                {

                    string str11 = wf.doc.Project.GetValueByKeyWord("HXNY_DOCUMENTSYSTEM_DESC");
                    if (string.IsNullOrEmpty(str11))
                        str11 = wf.doc.Project.GetValueByKeyWord("OPERATEADMIN_CODE");

                    string str12 = wf.doc.Project.GetValueByKeyWord("COM_SUBDOCUMENT_DESC");

                    string str13 = wf.doc.Project.Code;
                    string str14 = wf.doc.Code;
                    string mtitle = str11 + "_" + str12 + "_" + str13 + "的[" + str14 + "] 已处理完毕，请查看处理意见";

                    List<Doc> docList = new List<Doc>();
                    if (fbDoc != null) docList.Add(fbDoc);

                    string mmessage = "※" + str11 + "_" + str12 + "_" + str13 + "的[" + str14 + "] ,  已处理完毕，请查看处理意见,谢谢！";
                    wf.dBSource.SendMessage(wf.dBSource.LoginUser, mtoUser, null, mtitle, mmessage, null, docList, null, wf, null);
                }

                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }


        internal static List<User> AddToWSUserList(List<User> mtoUser, WorkFlow wf, string WSKeyword)
        {
            //编制人
            WorkState state = wf.WorkStateList.Find(wsx => wsx.Code == WSKeyword);
            if (state != null)
            {
                foreach (WorkUser wu in state.WorkUserList)
                {
                    if (wu.User != null && (!mtoUser.Contains(wu.User)))
                    {
                        mtoUser.Add(wu.User);
                    }
                }

            }

            return mtoUser;
        }
        //线程锁 
        internal static Mutex muxConsole = new Mutex();

        //发文流程分发收文，生成收文单
        internal static Doc GeneDistReceipt(WorkFlow wf, WorkStateBranch wsb, string replyDocCode)
        {
            try
            {
                DBSource dbsource = wf.dBSource;

                #region 代码逻辑
                // 1.从发文单获取文档属性

                //2.获取收文部门的存档管理的收文目录

                //3.收文目录新建收文单

                //4.把发文文档属性填入收文单属性

                //5.文档属性写入收文单文档 
                #endregion

                #region 1.从发文单获取文档属性
                CataloguDoc cdoc = new CataloguDoc();
                cdoc.doc = CommonFunction.GetWorkFlowDoc(wf);

                if (cdoc.doc == null) return null;


                //cdoc.doc = m_Doc;
                string strRecCode = cdoc.CA_SENDCODE;
                #endregion

                Project sendProj=null, recProj=null;

                #region 2.获取收文部门的存档管理的收文目录
                if (!GetProjectFilePath(cdoc, ref sendProj, ref recProj))
                {
                    return null;
                }
                #endregion




                #region 3.收文目录新建收文单,根据收文单模板，生成收文单文档

                //获取立项单文档所在的目录
                //Project m_Project = m_NewProject;

                List<TempDefn> docTempDefnByCode = dbsource.GetTempDefnByCode("CATALOGUING");
                TempDefn docTempDefn = (docTempDefnByCode != null && docTempDefnByCode.Count > 0) ? docTempDefnByCode[0] : null;
                if (docTempDefn == null)
                {
                    //reJo.msg = "没有与其相关的模板管理，创建无法正常完成";
                    //return reJo.Value;
                    return null;
                }

                IEnumerable<string> source = from docx in cdoc.Project.DocList select docx.Code;
                string filename = strRecCode + " 收文单";
                if (source.Contains<string>(filename))
                {

                    for (int i = 1; i < 0x3e8; i++)
                    {
                        filename = strRecCode + " 收文单" + i.ToString();
                        if (!source.Contains<string>(filename))
                        {
                            break;
                        }
                    }
                }

                //创建收文单
                CataloguDoc recdoc = new CataloguDoc();

                //文档名称
                recdoc.doc = recProj.NewDoc(filename + ".docx", filename, "", docTempDefn);

                if (recdoc.doc == null)
                {
                    //reJo.msg = "新建信函出错！";
                    //return reJo.Value;
                    return null;
                }



                #endregion

                #region 4.把发文文档属性填入收文单属性
                //发文单位代码
                string senderCode = "";

                string strRecverCode = ""; //string senderCode="";
                recdoc.CA_MAINFEEDERCODE = strRecverCode = cdoc.CA_MAINFEEDERCODE;
                recdoc.CA_SENDERCODE = senderCode = cdoc.CA_SENDERCODE;

                //发文单位
                string strCommUnit = recdoc.CA_SENDUNIT = cdoc.CA_SENDUNIT;

                //收文单的收文编码，等于发文单的发文编码
                string strSendCode = recdoc.CA_RECEIPTCODE = cdoc.CA_SENDCODE;
                //收文单的收文编号，等于发文单的文件ID
                string strRecNumber = recdoc.CA_RECID = cdoc.CA_FILEID;

                string strNeedReply = recdoc.CA_IFREPLY = cdoc.CA_IFREPLY;
                string replyDate = recdoc.CA_REPLYDATE = cdoc.CA_REPLYDATE;

                //收文日期
                string recDate = recdoc.CA_RECDATE = cdoc.CA_RECDATE;

                string strUrgency = recdoc.CA_URGENTDEGREE = cdoc.CA_URGENTDEGREE;
                string strRemark = recdoc.CA_NOTE = cdoc.CA_NOTE;

                //著录人
                // recdoc.CA_DESIGN = cdoc.CA_NOTE;

                string recType = cdoc.GetAttrTempCode();
                string recNumber = Document.getDocTempNumber(dbsource, "R", recType);
                //收文单的收文ID
                recdoc.CA_FILEID = recNumber;
                //文件标题
                string strTitle = recdoc.CA_FILETITLE = cdoc.CA_FILETITLE;
                string strPages = recdoc.CA_PAGE = cdoc.CA_PAGE;

                //回文编号
                recdoc.CA_REPLYCODE = replyDocCode;

                recdoc.SaveAttrData();
                // recdoc.doc.AttrDataList.SaveData(); 

                recdoc.Modify();
                #endregion


                #region 5. 录入数据进入word表单
                Hashtable htUserKeyWord = new Hashtable();


                #region 获取承办部门信息
                string recUnitDesc = "";
                //获取收文部门名称
                Server.Group gpRec = dbsource.GetGroupByName(strRecverCode);
                if (gpRec != null)
                {
                    recUnitDesc = gpRec.Description;
                    htUserKeyWord["HOSTUNIT"] = recUnitDesc;
                }

                //获取承办部分分发人
                WorkState wsSec = wf.WorkStateList.Find(ws => ws.DefWorkState.O_Code == "RECUNIT");
                if (wsSec != null && wsSec.WorkUserList != null && wsSec.WorkUserList.Count > 0)
                {
                    htUserKeyWord["HOSTUNITDISMAN"] = wsSec.WorkUserList[0].User.O_username;
                }


                //获取承办人
                string strHandle = cdoc.CA_MAINHANDLE;
                string mainHandle = strHandle.Substring(0, strHandle.IndexOf("__"));//.CA_MAINHANDLE_CODE;
                htUserKeyWord["HOSTUNITMAN"] = mainHandle;

                //获取承办人意见
                WorkAudit waMainHandle = wf.AllWorkAuditList.Find(wa => wa.workUser.User.O_username == mainHandle);
                if (waMainHandle != null)
                {
                    string mainHandleAudit = waMainHandle.O_Problom;
                    if (!string.IsNullOrEmpty(mainHandleAudit))
                    {
                        htUserKeyWord["HOSTUNITOPI"] = mainHandleAudit;
                    }
                }
                #endregion

                #region 获取协办部门信息
                string strCoUnits = cdoc.CA_ASSISTHANDLEUNIT;
                string[] strArryCoUnit = strCoUnits.Split(new char[] { ',' });
                int unitIndex = 0;
                foreach (string strCoUnit in strArryCoUnit)
                {
                    unitIndex = unitIndex + 1;
                    string strIndex = unitIndex.ToString();
                    string coUnitDesc = "";
                    //获取收文部门名称
                    Server.Group gpCo = dbsource.GetGroupByName(strCoUnit);
                    if (gpCo != null)
                    {
                        coUnitDesc = gpCo.Description;
                        htUserKeyWord["ASSUNIT" + strIndex] = coUnitDesc;


                        //获取协办部门分发人
                        List<WorkState> wsCoSecList = wf.WorkStateList.FindAll(ws => ws.DefWorkState.O_Code == "DEPARTMENTCONTROL");
                        foreach (WorkState wsCoSec in wsCoSecList)
                        {
                            if (wsCoSec != null && wsSec.WorkUserList != null && wsCoSec.WorkUserList.Count > 0)
                            {
                                User coUser = wsCoSec.WorkUserList[0].User;
                                if (gpCo.AllUserList.Contains(coUser))
                                {
                                    htUserKeyWord["ASSUNITDISMAN" + strIndex] = coUser.O_username;
                                    break;
                                }
                            }
                        }


                        //获取协办人
                        List<WorkState> wsCoManList = wf.WorkStateList.FindAll(ws => ws.DefWorkState.O_Code == "MAINHANDLE");
                        foreach (WorkState wsCoMan in wsCoManList)
                        {
                            if (wsCoMan != null && wsSec.WorkUserList != null && wsCoMan.WorkUserList.Count > 0)
                            {
                                User coUser = wsCoMan.WorkUserList[0].User;
                                if (gpCo.AllUserList.Contains(coUser))
                                {
                                    htUserKeyWord["ASSUNITMAN" + strIndex] = coUser.O_username;


                                    //获取协办人意见
                                    WorkAudit waCoHandle = wf.AllWorkAuditList.Find(wa => wa.workUser.User == coUser);
                                    if (waCoHandle != null)
                                    {
                                        string coHandleAudit = waCoHandle.O_Problom;
                                        if (!string.IsNullOrEmpty(coHandleAudit))
                                        {
                                            htUserKeyWord["ASSUNITOPI" + strIndex] = coHandleAudit;
                                        }
                                    }

                                    break;
                                }
                            }
                        }

                    }
                }

                #endregion

                #region 获取阅知部门信息
                string strReadUnits = cdoc.CA_READUNIT;
                string[] strArryReadUnit = strReadUnits.Split(new char[] { ',' });
                unitIndex = 0;
                foreach (string strReadUnit in strArryReadUnit)
                {
                    unitIndex = unitIndex + 1;
                    string strIndex = unitIndex.ToString();
                    string readUnitDesc = "";
                    //获取阅知部门名称
                    Server.Group gpRead = dbsource.GetGroupByName(strReadUnit);
                    if (gpRead != null)
                    {
                        readUnitDesc = gpRead.Description;
                        htUserKeyWord["READUNIT" + strIndex] = readUnitDesc;


                        //获取阅知部门分发人
                        List<WorkState> wsReadSecList = wf.WorkStateList.FindAll(ws => ws.DefWorkState.KeyWord == "DEPARTMENTCONTROL");
                        foreach (WorkState wsReadSec in wsReadSecList)
                        {
                            if (wsReadSec != null && wsSec.WorkUserList != null && wsReadSec.WorkUserList.Count > 0)
                            {
                                User readUser = wsReadSec.WorkUserList[0].User;
                                if (gpRead.AllUserList.Contains(readUser))
                                {
                                    htUserKeyWord["READUNITDISMAN" + strIndex] = readUser.O_username;
                                    break;
                                }
                            }
                        }


                        //获取阅知人
                        List<WorkState> wsReadManList = wf.WorkStateList.FindAll(ws => ws.DefWorkState.KeyWord == "MAINHANDLE");
                        foreach (WorkState wsReadMan in wsReadManList)
                        {
                            if (wsReadMan != null && wsSec.WorkUserList != null && wsReadMan.WorkUserList.Count > 0)
                            {
                                User readUser = wsReadMan.WorkUserList[0].User;
                                if (gpRead.AllUserList.Contains(readUser))
                                {
                                    htUserKeyWord["READUNITMAN" + strIndex] = readUser.O_username;


                                    //获取阅知人意见
                                    WorkAudit waReadHandle = wf.AllWorkAuditList.Find(wa => wa.workUser.User == readUser);
                                    if (waReadHandle != null)
                                    {
                                        string readHandleAudit = waReadHandle.O_Problom;
                                        if (!string.IsNullOrEmpty(readHandleAudit))
                                        {
                                            htUserKeyWord["ASSUNITOPI" + strIndex] = readHandleAudit;
                                        }
                                    }

                                    break;
                                }
                            }
                        }

                    }
                }

                #endregion

                ////格式化日期
                //DateTime RecDate = Convert.ToDateTime(strRecDate);
                //DateTime ReplyDate = Convert.ToDateTime(strReplyDate);
                //string recDate = RecDate.ToShortDateString().ToString().Replace("-", ".").Replace("/", ".");
                //string replyDate = ReplyDate.ToShortDateString().ToString().Replace("-", ".").Replace("/", ".");

                //htUserKeyWord.Add("PRONAME", strProjectDesc);//项目名称
                //htUserKeyWord.Add("PROCODE", strProjectCode);//项目代码
                htUserKeyWord.Add("SENDUNIT", strCommUnit);//来文单位
                htUserKeyWord.Add("RECDATE", recDate);//收文日期
                htUserKeyWord.Add("RECCODE", strRecCode);//收文编码
                htUserKeyWord.Add("RECNUMBER", strRecNumber);//收文编号
                htUserKeyWord.Add("FILECODE", strSendCode);//strFileCode);//文件编码
                htUserKeyWord.Add("FILETITLE", strTitle);//文件题名
                htUserKeyWord.Add("PAGE", strPages);//页数
                htUserKeyWord.Add("SENDCODE", replyDocCode);//发文编码
                htUserKeyWord.Add("IFREPLY", strNeedReply);//是否回文
                htUserKeyWord.Add("REPLYDATE", replyDate);//回文日期
                htUserKeyWord.Add("URGENTDEGREE", strUrgency);//紧急程度
                htUserKeyWord.Add("NOTE", strRemark);//备注
                htUserKeyWord.Add("DESIGN", dbsource.LoginUser.Description);//著录人


                string workingPath = dbsource.LoginUser.WorkingPath;


                try
                {
                    //上传下载文档
                    string exchangfilename = "收文单模板";

                    //获取网站路径
                    string sPath = System.Web.HttpContext.Current.Server.MapPath("/ISO/HXEPC/");

                    //获取模板文件路径
                    string modelFileName = sPath + exchangfilename + ".docx";

                    //获取即将生成的联系单文件路径
                    string locFileName = recdoc.FullPathFile;

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
                            office.WriteDataToDocument(recdoc.doc, locFileName, htUserKeyWord, htUserKeyWord);

                            //转换成PDF
                            CommonFunction.ConventDocToPdf(recdoc.doc);
                        }
                        catch { }
                        finally
                        {

                            //解锁
                            muxConsole.ReleaseMutex();
                        }
                    }


                    int length = (int)info.Length;
                    recdoc.O_size = new int?(length);
                    recdoc.Modify();

                }
                catch { }
                #endregion

                #region 6. 流程结束后文件存档

                FileArchive(wf, sendProj, recProj);

 
                #endregion

                return recdoc.doc;
            }
            catch (Exception mEx)
            {
                string msg = mEx.Message;
            }
            return null;
        }

        internal static string SetPalindromeCod(CataloguDoc caDoc)
        {
            string result = "";
            try
            {

                #region 获取回文编码
                //获取占号编码


                //获取项目代码
                string projectCode = caDoc.CA_PROCODE;

                //收文单位编码等于来文的发文单位编码
                string recCode = caDoc.CA_SENDERCODE;

                //发文单位编码等于来文的收文单位编码
                string senderCode = caDoc.CA_MAINFEEDERCODE;

                //回复的文件类型为信函
                string docType = "LET";

                string filecode = DocumentCommon.getDocSendCode(caDoc.dbSource, projectCode, docType, senderCode, recCode);

                #endregion


                #region 获取收文部门的发文目录
                //获取收文部门的发文目录
                //1.项目发给部门，自动生成的预编码文件，放在运营管理类的通信文件的发文目录下

                //2.部门发给部门，自动生成的预编码文件，放在运营管理类的通信文件的发文目录下

                //3.部门发给项目，自动生成的预编码文件，放在项目管理类的所属项目的通信文件的发文目录下

                //1.判断收文单位，是否是属于项目部门
                //记录收文单位的发文目录
                Project sendCommProj = null;
                AVEVA.CDMS.Server.Group senderGp=caDoc.dbSource.GetGroupByName(senderCode);

                if (senderGp != null)
                {

                    //运营管理类文件目录
                    Project rootProj = caDoc.dbSource.RootLocalProjectList.Find(itemProj => itemProj.TempDefn.KeyWord == "OPERATEADMIN");

                    //通信管理目录
                    Project commProj = rootProj.ChildProjectList.Find(itemProj => itemProj.TempDefn.KeyWord == "PRO_COMMUNICATION");

                    Project sendProj = CommonFunction.GetProjectByDesc(commProj, "发文");

                    sendCommProj = sendProj.GetProjectByName("信函");
                }
                else {
                    //获取项目里的通信类的收文目录
                    string projCode = caDoc.CA_PROCODE;

                    Project rProj = caDoc.dbSource.RootLocalProjectList.Find(itemProj => itemProj.TempDefn.KeyWord == "PRODOCUMENTADMIN");

                    //项目目录
                    Project pProj = rProj.ChildProjectList.Find(p => p.Code == projCode);

                    //通信文件目录
                    Project pcProject = pProj.ChildProjectList.Find(p => p.TempDefn.KeyWord == "PRO_COMMUNICATION");
                    //通信类目录
                    Project sendProj = CommonFunction.GetProjectByDesc(pcProject, "发文");

                    sendCommProj = sendProj.GetProjectByName("信函");

                }

                #endregion

                #region 在收文部门的发文目录创建占号文档
                //
                List<TempDefn> docTempDefnByCode = caDoc.dbSource.GetTempDefnByCode("CATALOGUING");
                TempDefn docTempDefn = (docTempDefnByCode != null && docTempDefnByCode.Count > 0) ? docTempDefnByCode[0] : null;

                CataloguDoc tCaDoc = new CataloguDoc();
                //tCaDoc.doc = caDoc.Project.NewDoc("", "PRE-" + filecode, "", docTempDefn);
                //tCaDoc.doc = caDoc.Project.NewDoc("", filecode, "", docTempDefn);
                tCaDoc.doc = sendCommProj.NewDoc("", filecode, "", docTempDefn);

                //收文编号，等于来文的发文编号
                string recFileCode = caDoc.CA_SENDCODE;

                tCaDoc.CA_ATTRTEMP = "PRECODE";
                tCaDoc.CA_SENDCODE = filecode;
                tCaDoc.CA_RECEIPTCODE = recFileCode;
                tCaDoc.SaveAttrData(); 
                #endregion

                //保存回文编码到发文的文档
                caDoc.CA_REPLYCODE = filecode;
                caDoc.SaveAttrData();

                return filecode;
            }
            catch (Exception mEx)
            {
                string msg = mEx.Message;
            }
            return result;
        }

        internal static bool GetProjectFilePath(CataloguDoc cdoc, ref Project sendProj, ref Project recProj)
        {

            //判断是否发送给项目

            bool isToProj = false;
            //判断当前文档是否发送给项目
            isToProj = CommonFunction.GetIfSendToProject(cdoc.doc);

            //Regex regex = new Regex(@"^\w+\-\w+\-\w+\-\w+\-\w+$");
            ////bool bhvt = Regex.IsMatch(email, @"^\w+@\w+\.\w+$");
            //if (regex.IsMatch(strRecCode))
            //{
            //    isToProj = true;
            //}


            //发文单位代码
            string senderCode = cdoc.CA_SENDERCODE;  // doc.GetAttrDataByKeyWord(senderKeyword).ToString;

            //收文单位代码
            string recerCode = cdoc.CA_MAINFEEDERCODE;

            string docType = cdoc.GetAttrTempDesc();

            Project rootProj, senderProj;
            //Project recProj;
            string projCode = cdoc.CA_PROCODE;

            #region  获取发文目录

            string recUnitCode = cdoc.CA_MAINFEEDERCODE;
            AVEVA.CDMS.Server.Group sendSecGroup = CommonFunction.GetSecGroupByUnitCode(cdoc.dbSource, senderCode);

            if (sendSecGroup==null)
            {

                #region 项目发到项目，或者项目发到部门



                Project rProj = cdoc.dbSource.RootLocalProjectList.Find(itemProj => itemProj.TempDefn.KeyWord == "PRODOCUMENTADMIN");
                //项目目录    
                Project pProj = rProj.ChildProjectList.Find(p => p.Code == projCode);
                //存档管理目录    
                Project psProject = pProj.ChildProjectList.Find(p => p.TempDefn.KeyWord == "PRO_STORAGE");
                //通信类目录
                Project commProject = psProject.ChildProjectList.Find(p => p.TempDefn.KeyWord == "STO_COMMUNICATION");


                //发文单位目录下的发文下的函件类型目录
                if (commProject == null)
                {
                    return false;
                }
                try
                {
                    sendProj = commProject.GetProjectByName("发文").GetProjectByName(docType);
                    if (sendProj == null)
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
                
                #endregion
            }
            else
            {
 
                #region 部门发到部门，或者部门发到项目
                //运营管理类文件目录部门发到部门，或者部门发到项目
                rootProj = CommonFunction.getParentProjectByTempDefn(cdoc.Project, "OPERATEADMIN");

                //存档管理目录
                Project storaProj = CommonFunction.GetProjectByDesc(rootProj, "存档管理");
                //通信类目录
                Project commProj = CommonFunction.GetProjectByDesc(storaProj, "通信类");

                ////发文单位目录
                senderProj = commProj.GetProjectByName(senderCode);


                //Project sendProj = null;

                //发文单位目录下的发文下的函件类型目录
                if (senderProj == null)
                {
                    return false;
                }
                try
                {
                    sendProj = senderProj.GetProjectByName("发文").GetProjectByName(docType);
                    if (sendProj == null)
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                } 
                #endregion
  
            }
            #endregion

            #region 获取收文目录
            AVEVA.CDMS.Server.Group recSecGroup = CommonFunction.GetSecGroupByUnitCode(cdoc.dbSource, recUnitCode);

            if (recSecGroup == null)

            {
                #region 项目发到项目，或者部门发到项目

                    //获取项目里的通信类的收文目录
                    string sendcode = cdoc.CA_SENDCODE;
                    //string projCode = sendcode.Substring(0, sendcode.IndexOf("-"));
                    Project rProj = cdoc.dbSource.RootLocalProjectList.Find(itemProj => itemProj.TempDefn.KeyWord == "PRODOCUMENTADMIN");

                    //项目目录
                    Project pProj = rProj.ChildProjectList.Find(p => p.Code == projCode);

                    //存档管理目录
                    Project psProject = pProj.ChildProjectList.Find(p => p.TempDefn.KeyWord == "PRO_STORAGE");
                    //通信类目录
                    Project scProject = psProject.ChildProjectList.Find(p => p.TempDefn.KeyWord == "STO_COMMUNICATION");

                    recProj = scProject.GetProjectByName("收文").GetProjectByName(docType);
                    if (recProj == null)
                    {
                        return false;
                    }

                #endregion

                ////发文单位目录,这里暂时没有分到发文单位目录，通信类目录直接到了收发文目录
                //senderProj = scProject;
            }
            else
            {
                //部门发部门，或者项目发到部门
                #region  获取运营管理类文件目录里的通信类的收文目录

                //运营管理类文件目录
                rootProj = cdoc.dbSource.RootLocalProjectList.Find(itemProj => itemProj.TempDefn.KeyWord == "OPERATEADMIN");

                //存档管理目录
                Project storaProj = CommonFunction.GetProjectByDesc(rootProj, "存档管理");
                //通信类目录
                Project commProj = CommonFunction.GetProjectByDesc(storaProj, "通信类");

                //收文单位目录
                Project recerProj = commProj.GetProjectByName(recerCode);
                //收文单位目录下的发文下的函件类型目录
                if (recerProj == null)
                {
                    return false;
                }


                recProj = recerProj.GetProjectByName("收文").GetProjectByName(docType);
                if (recProj == null)
                {
                    return false;
                }
                #endregion
            }
            
            #endregion

            return true;
        }
        /// <summary>
        /// 流程结束后文件存档
        /// </summary>
        /// <returns></returns>
        internal static bool FileArchive(WorkFlow wf, Project sendProj,Project recProj)
        {
            try
            {
                #region 6.0 复制文档到收文部门的收文目录和发文部门的发文目录
                Doc cpdoc = CommonFunction.GetWorkFlowDoc(wf);
                if (cpdoc != null)
                {
                    bool bConventToPdf = true;

                    #region 转换PDF到文件所在目录

                    if (bConventToPdf)
                    {

                        try
                        {

                            string sourceFileName = cpdoc.FullPathFile; string sFileName = cpdoc.O_filename;
                            string targetFileName = sourceFileName.Substring(0, sourceFileName.LastIndexOf(".") + 1) + "pdf";

                            CDMSPdf.ConvertToPdf(cpdoc.FullPathFile, targetFileName);
                            cpdoc.O_filename = cpdoc.O_filename.Substring(0, cpdoc.O_filename.LastIndexOf(".") + 1) + "pdf";

                            cpdoc.Modify();

                            #region 先把docx文件复制到存档管理目录
                            try
                            {
                                //创建目录
                                if (!Directory.Exists(sendProj.FullPath))
                                {
                                    Directory.CreateDirectory(sendProj.FullPath);
                                }

                                //拷贝文件
                                File.Move(sourceFileName, sendProj.FullPath + sFileName);
                            }
                            catch { }
                            #endregion
                        }
                        catch { }
                    }
                    #endregion

                }

                #endregion

                #region 6.1 流程结束后将文件存档到存档管理下的通信类目录
                foreach (Doc docItem in wf.DocList)
                {

                    //创建目录
                    if (!Directory.Exists(sendProj.FullPath))
                    {
                        Directory.CreateDirectory(sendProj.FullPath);
                    }

                    //拷贝文件
                    File.Move(docItem.FullPathFile, sendProj.FullPath + docItem.O_filename);

                    //改变文件父目录
                    sendProj.DocList.Add(docItem);
                    sendProj.Modify();

                    docItem.O_projectno = sendProj.O_projectno;
                    docItem.Modify();

                    //修改著录表里面的目录id
                    string format = "update User_CATALOGUING set ProjectNo={0} where Itemno={1}";

                    format = string.Format(format, new object[] {
                                                sendProj.O_projectno,docItem.O_itemno});

                    docItem.dBSource.DBExecuteSQL(format);

                }
                #endregion

                #region 6.2 复制附件到收文部门的收文目录和发文部门的发文目录
                foreach (Doc attadoc in wf.DocList)
                {
                    recProj.NewDoc(attadoc);
                }

                #endregion
            }
            catch {
                return false;
            }
            return true;
        }
    }
}
