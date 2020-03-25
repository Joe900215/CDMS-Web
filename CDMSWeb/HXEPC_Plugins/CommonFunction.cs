namespace AVEVA.CDMS.HXEPC_Plugins
{
    //using AVEVA.CDMS.Common;
    using AVEVA.CDMS.Server;
    //using Microsoft.Win32;
    //using SEAGULL;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Net.Mime;
    using System.Text;
    using System.Threading;
    using WebApi;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System.Text.RegularExpressions;

    //using System.Windows.Forms;


    public class CommonFunction
    {
        /// <summary>
        /// 获取项目对象
        /// </summary>
        /// <param name="obj">目录或者文档</param>
        /// <returns></returns>
        public static Project GetProject(object obj)
        {
            if (obj is Doc || obj is Project)
            {
                //或者对象
                Project p = null;
                if (obj is Doc)
                {
                    p = ((Doc)obj).Project;
                }
                else
                {
                    p = (Project)obj;
                }

                //查找
                while (p != null)
                {
                    if (p.TempDefn != null && p.TempDefn.KeyWord == "DESIGNPROJECT")
                    {
                        return p;
                    }
                    p = p.ParentProject;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取阶段对象
        /// </summary>
        /// <param name="obj">目录或者文档</param>
        /// <returns></returns>
        public static Project GetDesign(object obj)
        {
            if (obj is Doc || obj is Project)
            {
                //或者对象
                Project p_Design = null;
                if (obj is Doc)
                {
                    p_Design = ((Doc)obj).Project;
                }
                else
                {
                    p_Design = (Project)obj;
                }


                //查找
                while (p_Design != null)
                {
                    if (p_Design.TempDefn != null && p_Design.TempDefn.KeyWord == "DESIGNPHASE")
                    {
                        return p_Design;
                    }
                    p_Design = p_Design.ParentProject;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取专业对象
        /// </summary>
        /// <param name="obj">目录或者文档</param>
        /// <returns></returns>
        public static Project GetProfession(object obj)
        {
            if (obj is Doc || obj is Project)
            {
                //或者对象
                Project p_Pofession = null;
                if (obj is Doc)
                {
                    p_Pofession = ((Doc)obj).Project;
                }
                else
                {
                    p_Pofession = (Project)obj;
                }


                //查找
                while (p_Pofession != null)
                {
                    if (p_Pofession.TempDefn != null && p_Pofession.TempDefn.KeyWord == "PROFESSION")
                    {
                        return p_Pofession;
                    }
                    p_Pofession = p_Pofession.ParentProject;
                }
            }
            return null;
        }



        /// <summary>
        /// 查找模板
        /// </summary>
        /// <param name="obj">Doc， Project</param>
        /// <param name="keyword">模板关键字</param>
        /// <returns>TempDefn</returns>
        public static TempDefn GetTempDefn(object obj, string keyword)
        {
            if (obj is Doc || obj is Project)
            {
                //或者对象
                Project p = null;
                if (obj is Doc)
                {
                    p = ((Doc)obj).Project;
                }
                else
                {
                    p = (Project)obj;
                }


                //查找
                List<TempDefn> tdlist = p.dBSource.GetTempDefnByCode("keyword");
                if (tdlist != null && tdlist.Count > 0)
                {
                    return tdlist[0];
                }
            }

            return null;
        }


        //发文邮件
        /// <summary>
        /// 发邮件
        /// </summary>
        /// <param name="MailFrom">发件人邮箱地址</param>
        /// <param name="MailToList">收件人邮箱地址</param>
        /// <param name="MailTitle">主题</param>
        /// <param name="MailBody">邮件内容</param>
        /// <param name="FileList">附加文件</param>
        /// <param name="MailServer">服务器地址</param>
        /// <param name="UserName">邮箱登录账号</param>
        /// <param name="UserPw">邮箱登录密码</param>
        /// <returns></returns>
        private static bool SendEmail(string MailFrom, List<string> MailToList, string MailTitle, string MailBody, List<string> FileList, string MailServer, string UserName, string UserPw)
        {
            try
            {
                if ((MailToList == null) || (MailToList.Count < 1))
                {
                    return false;
                }
                MailMessage message = new MailMessage
                {
                    From = new MailAddress(MailFrom.Trim())
                };
                foreach (string str in MailToList)
                {
                    message.To.Add(str);
                }
                message.Subject = MailTitle;
                message.Body = MailBody;
                message.IsBodyHtml = true;
                message.BodyEncoding = Encoding.UTF8;
                message.Headers.Add("Disposition-Notification-To", MailFrom);
                if ((FileList != null) && (FileList.Count > 0))
                {
                    foreach (string str2 in FileList)
                    {
                        Attachment item = new Attachment(str2)
                        {
                            Name = Path.GetFileName(str2),
                            NameEncoding = Encoding.GetEncoding("gb2312"),
                            TransferEncoding = TransferEncoding.Base64
                        };
                        item.ContentDisposition.Inline = true;
                        item.ContentDisposition.DispositionType = "inline";
                        message.Attachments.Add(item);
                    }
                }
                new SmtpClient(MailServer) { Credentials = new NetworkCredential(UserName.Trim(), UserPw.Trim()) }.Send(message);
                return true;
            }
            catch (Exception exception)
            {
                //ErrorLog.WriteErrorLog(exception.ToString());
                WebApi.CommonController.WebWriteLog(exception.ToString());
                return false;
            }
        }

        public static Project GetProjectByDesc(Project curProject, string Desc)
        {
            if (curProject == null) return null;

            Project proj = curProject;
            foreach (Project prj in proj.ChildProjectList)
            {

                if (prj.Description == Desc)
                {
                    return prj;
                }
            }
            return null;
        }

        ///// <summary>
        ///// 流程里面发邮件
        ///// </summary>
        ///// <param name="wf"></param>
        //public static void SendMail(WorkFlow wf)
        //{
        //    try
        //    {
        //        Doc doc = wf.doc;
        //        string CompCode;
        //        string CompName;
        //        string CompEmail;
        //        string CompRecevier;
        //        string CompFaxNo;
        //        string CompEnclosure;

        //        string mailFrom = "CDMSAdmin@gedi.com.cn";
        //        List<string> mailToList = new List<string>();
        //        string mailTitle = doc.O_itemname;
        //        string mailBody = "";
        //        List<string> FileList = null;
        //        string mailServer = "smtp.gedi.com.cn";
        //        string userName = "CDMSAdmin";
        //        string userPw = "CDMS_12345";

        //        //查找设计阶段
        //        Project p = wf.doc.Project;
        //        while (p != null && p.ParentProject != null)
        //        {
        //            //找设计阶段
        //            if (p != null && p.TempDefn != null && p.TempDefn.KeyWord == "DESIGNPHASE")
        //            {
        //                break;
        //            }
        //            p = p.ParentProject;
        //        }

        //        //查找收文厂家
        //        Project profession = wf.CuWorkState.O_iuser4 != null ? p.dBSource.GetProjectByID((int)wf.CuWorkState.O_iuser4) : null;
        //        if (profession == null)
        //        {
        //            foreach (Project pp in p.ChildProjectList)
        //            {
        //                if (pp.Code == "收文")
        //                {
        //                    foreach (Project cj in pp.ChildProjectList)
        //                    {
        //                        //查找厂家
        //                        Project Comp = doc.Project.ParentProject;
        //                        if (cj.Code == Comp.Code)
        //                        {
        //                            //查找厂家数据
        //                            CompCode = cj.GetAttrDataByKeyWord("FC_COMPANYCODE").ToString;//厂家编码
        //                            CompName = cj.GetAttrDataByKeyWord("FC_COMPANYCHINESE").ToString;//厂家名称
        //                            CompEmail = cj.GetAttrDataByKeyWord("FC_EMAIL").ToString;//邮箱编号
        //                            CompRecevier = cj.GetAttrDataByKeyWord("FC_RECEIVER").ToString;//厂家收件人
        //                            CompFaxNo = cj.GetAttrDataByKeyWord("FC_FAXNO").ToString;//厂家传真号
        //                            CompEnclosure = doc.GetAttrDataByKeyWord("IF_SENDFILE").ToString;//文件附件

        //                            //分割附件
        //                            foreach (string enclosure in CompEnclosure.Split(new char[] { ';' }))
        //                            {
        //                                if (!string.IsNullOrEmpty(enclosure))
        //                                {
        //                                    mailToList.Add(enclosure);
        //                                }
        //                            }
        //                            if (DialogResult.Yes == MessageBox.Show("请核对邮件接收者地址,确认无误后点击确定进行发送(点取消请自行发送邮件)?\r\n接收方:" + CompName + "\r\n接收人:" + CompRecevier + "\r\n接收地址:" + CompEmail, "发送信息", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
        //                            {
        //                                try
        //                                {
        //                                    mailToList.Add(CompEmail);
        //                                    SendEmail(mailFrom, mailToList, mailTitle, mailBody, FileList, mailServer, userName, userPw);
        //                                    //MessageBox.Show(mailFrom + "\n\n发往\n\n" + CompEmail + "\n\n邮件发送成功   \r\n接口文件编号是：" + doc.Code);
        //                                    //ErrorLog.WriteErrorLog(mailFrom + "发往" + CompEmail + "邮件成功   接口文件编号是：" + doc.Code);
        //                                    WebApi.CommonController.WebWriteLog(mailFrom + "发往" + CompEmail + "邮件成功   接口文件编号是：" + doc.Code);
        //                                }
        //                                catch (Exception exception)
        //                                {
        //                                    //MessageBox.Show(mailFrom + "发往" + CompEmail + "邮件失败，请联系管理员。");
        //                                    //ErrorLog.WriteErrorLog(mailFrom + "发往" + CompEmail + "邮件失败" + exception.ToString() + "   接口文件编号是：" + doc.Code);
        //                                    WebApi.CommonController.WebWriteLog(mailFrom + "发往" + CompEmail + "邮件失败" + exception.ToString() + "   接口文件编号是：" + doc.Code);
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch { }
        //}


        /// <summary>
        /// 内部提资：专业主设人接收文件签名
        /// </summary>
        /// <param name="workflow"></param>
        public static void ExcSign(WorkFlow wf)
        {
            try
            {
                if (wf != null && wf.DocList != null && wf.DocList.Count > 0 && wf.O_WorkFlowStatus == enWorkFlowStatus.Finish)
                {

                    //取第一个文件
                    Doc doc = wf.DocList[0];

                    //文件全称
                    string str = doc.O_filename.ToUpper();

                    //流程状态不能为空
                    if ((wf.WorkStateList == null) || (wf.WorkStateList.Count <= 1))
                    {
                        return;
                    }

                    ////FTP对象
                    //FTPFactory fTP = null;
                    //if (doc.Storage.FTP != null)
                    //{
                    //    //文档存储位置
                    //    fTP = doc.Storage.FTP;
                    //}
                    //else
                    //{
                    //    fTP = new FTPFactory(doc.Storage);
                    //}
                    //if (fTP == null)
                    //{
                    //    return;
                    //}

                    //获取即将生成的互提单文件路径
                    string locFileName = doc.FullPathFile;

                    //文件路径不为空
                    if (!string.IsNullOrEmpty(locFileName))
                    {
                        if (System.IO.File.Exists(locFileName))
                        {
                            //文件位置
                            FileInfo info = new FileInfo(locFileName);

                            //文件是否只读
                            if (info.IsReadOnly)
                            {
                                info.IsReadOnly = false;
                                //info.Delete();
                            }
                        }

                        //读取
                        //fTP.download(doc.FullPathFile, doc.dBSource.LoginUser.WorkingPath + doc.O_filename, true);
                    }

                    //创建附加表单属性
                    Hashtable htUserKeyWord = new Hashtable();

                    //查找接收人
                    int indx = 1;
                    foreach (WorkState ws in wf.WorkStateList)
                    {
                        if (ws.DefWorkState.O_Code == "RECEIVE")
                        {
                            htUserKeyWord.Add("RECEIVER" + indx.ToString(), ws.WorkUserList[0].User.Code);            //接收用户
                            htUserKeyWord.Add("RENAME" + indx.ToString(), ws.O_suser3);                               //接收专业
                            htUserKeyWord.Add("RETIME" + indx.ToString(), DateTime.Now.ToString("yyyy-MM-dd"));       //接收时间
                            indx++;
                        }

                    }

                    //写表单
                    if (((htUserKeyWord.Count != 0) && str.EndsWith(".DOC")) || ((str.EndsWith(".DOCX") || str.EndsWith(".XLS")) || str.EndsWith(".XLSX")))
                    {
                        WebApi.CDMSWebOffice office = new WebApi.CDMSWebOffice
                        {
                            CloseApp = true,
                            VisibleApp = false
                        };

                        //释放文档
                        office.Release(true);
                        if ((doc.WorkFlow != null) && (doc.WorkFlow.O_WorkFlowStatus == enWorkFlowStatus.Finish))
                        {
                            office.IsFinial = true;
                        }

                        //写入文档
                        office.WriteDataToDocument(doc, locFileName, htUserKeyWord, htUserKeyWord);
                    }

                    ////上传文件
                    //if (fTP != null)
                    //{
                    //    fTP.upload(doc.dBSource.LoginUser.WorkingPath + doc.O_filename, doc.FullPathFile);
                    //    fTP.close();
                    //}
                }

            }
            catch (Exception ex)
            {
                //错误提示
                //AssistFun.PopUpPrompt("发生错误：" + ex.ToString());
                WebApi.CommonController.WebWriteLog("发生错误：" + ex.ToString());
            }
        }

        ////内部提资
        //public static bool InsertDocListAndOpenDoc(List<Doc> dl, Doc doc)
        //{
        //    try
        //    {
        //        if (ExMenu.callTheApp != null)
        //        {
        //            CallBackResult result2;
        //            CallBackParam param = new CallBackParam
        //            {
        //                callType = enCallBackType.UpdateDBSource,
        //                dbs = doc.dBSource
        //            };
        //            CallBackResult result = null;
        //            ExMenu.callTheApp(param, out result);
        //            CallBackParam param2 = new CallBackParam();
        //            if (doc == null)
        //            {
        //                return false;
        //            }
        //            param2.dList = new List<Doc>();
        //            param2.dList.Add(doc);
        //            param2.callType = enCallBackType.DocSimpleOpen;
        //            ExMenu.callTheApp(param2, out result2);
        //        }
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        //线程锁 
        internal static Mutex muxConsole = new Mutex();


        /// <summary>
        /// 发文通信校审流程签名
        /// </summary>
        /// <param name="wf"></param>
        /// 签名公式：<AVEVA.CDMS.HXEPC_Plugins.CommonFunction><HXSign>
        public static void HXSign(WorkFlow wf)
        {
            try
            {
                #region 发文流程
                if (wf.DefWorkFlow.O_Code == "COMMUNICATIONWORKFLOW")
                {
                    if (((wf != null) && (wf.DocList != null)) && (wf.DocList.Count > 0))
                    {
                        foreach (Doc doc in wf.DocList)
                        {
                            string str = doc.O_filename.ToUpper();
                            Hashtable htUserKeyWord = new Hashtable();
                            if ((wf.WorkStateList == null) || (wf.WorkStateList.Count <= 1))
                            {
                                continue;
                            }
                            int count = wf.WorkStateList.Count;

                            //获取即将函件的文件路径
                            string locFileName = doc.FullPathFile;

                            if (string.IsNullOrEmpty(locFileName))
                            {
                                return;
                            }
                            if (!System.IO.File.Exists(locFileName))
                            {
                                return;
                            }

                            //if (!string.IsNullOrEmpty(locFileName))
                            //{
                            //    if (System.IO.File.Exists(locFileName))
                            //    {
                            //        FileInfo info = new FileInfo(locFileName);
                            //        if (info.IsReadOnly)
                            //        {
                            //            info.IsReadOnly = false;
                            //            //info.Delete();
                            //        }
                            //    }
                            //    //fTP.download(doc.FullPathFile, doc.dBSource.LoginUser.WorkingPath + doc.O_filename, true);
                            //}

                            string code = wf.CuWorkState.Code;
                            if ((doc.WorkFlow != null) && (doc.WorkFlow.O_WorkFlowStatus == enWorkFlowStatus.Finish))
                            {
                                code = "END";
                            }

                            ////判断是否文控关闭
                            //if (code == "MAINHANDLE" ) {
                            //    return;
                            //}

                            switch (code)
                            {
                                //校核人
                                case "CHECK":
                                    //htUserKeyWord.Add("PREPAREDSIGN1", doc.dBSource.LoginUser.O_username);
                                    htUserKeyWord["CHECKPERSON"] = doc.WorkFlow.CuWorkState.WorkUserList[0].User.O_username;
                                    htUserKeyWord["CHECKTIME"] = DateTime.Now.ToString("yyyy.MM.dd");
                                    goto Label_04DA;

                                //审核人
                                case "AUDIT":
                                    htUserKeyWord["AUDITPERSON"] = doc.WorkFlow.CuWorkState.WorkUserList[0].User.O_username;
                                    htUserKeyWord["AUDITTIME"] = DateTime.Now.ToString("yyyy.MM.dd");
                                    goto Label_04DA;

                                //审定人
                                case "AUDIT2":
                                    htUserKeyWord["REVIEWER"] = doc.WorkFlow.CuWorkState.WorkUserList[0].User.O_username;
                                    htUserKeyWord["AUDITTIME2"] = DateTime.Now.ToString("yyyy.MM.dd");
                                    goto Label_04DA;

                                //批准人
                                case "APPROV":
                                    htUserKeyWord["APPROVER"] = doc.WorkFlow.CuWorkState.WorkUserList[0].User.O_username;
                                    htUserKeyWord["APPROVETIME"] = DateTime.Now.ToString("yyyy.MM.dd");

                                    //检查签名，如果没有签名的画斜线
                                    //校核人
                                    htUserKeyWord["CHECKPERSON"] = "slash";
                                    //审核人
                                    htUserKeyWord["AUDITPERSON"] = "slash";
                                    //审定人
                                    htUserKeyWord["REVIEWER"] = "slash";


                                    goto Label_04DA;


                                case "INTERFACE":
                                case "END":
                                    {
                                        string str3 = doc.dBSource.ParseExpression(doc, "$(PROJECTOWNER)");
                                        if (wf.DefWorkFlow.O_Code == "HTWORKFLOW")
                                        {
                                            //  htUserKeyWord["APPROVEPERSON"] = doc.dBSource.LoginUser.O_username;
                                        }
                                        else if (!string.IsNullOrEmpty(str3))
                                        {
                                            // htUserKeyWord["APPROVEPERSON"] = str3;
                                            // htUserKeyWord["APPROVETIME"] = DateTime.Now.ToString("yyyy.MM.dd");
                                        }
                                        goto Label_04DA;
                                    }
                                default:
                                    goto Label_04DA;
                            }
                            if (wf.CuWorkState.PreWorkState.Code == "CHECK")
                            {
                                htUserKeyWord["CHECKPERSON"] = doc.dBSource.LoginUser.O_username;
                            }
                            else
                            {
                                htUserKeyWord["AUDITPERSON"] = doc.dBSource.LoginUser.O_username;
                            }
                            Label_0385:
                            htUserKeyWord["AUDITTIME"] = DateTime.Now.ToString("yyyy.MM.dd");
                            Label_04DA:
                            if ((str.EndsWith(".DOC") || str.EndsWith(".DOCX")) || (str.EndsWith(".XLS") || str.EndsWith(".XLSX")))
                            {
                                //线程锁 
                                muxConsole.WaitOne();
                                try
                                {
                                    WebApi.CDMSWebOffice office = new WebApi.CDMSWebOffice
                                    {
                                        CloseApp = true,
                                        VisibleApp = false
                                    };
                                    office.Release(true);
                                    if (doc.WorkFlow != null)
                                    {
                                        enWorkFlowStatus status1 = doc.WorkFlow.O_WorkFlowStatus;
                                    }
                                    office.WriteDataToDocument(doc, locFileName, htUserKeyWord, htUserKeyWord);

                                    //WebApi.CDMSWebOffice.SetFilePagesInfo(doc,);
                                }
                                catch (Exception ExOffice)
                                {
                                    WebApi.CommonController.WebWriteLog(ExOffice.Message);
                                }
                                finally
                                {
                                    //解锁
                                    muxConsole.ReleaseMutex();
                                }
                                //if (((doc.WorkFlow != null) && (doc.WorkFlow.O_WorkFlowStatus == enWorkFlowStatus.Finish)) && (doc.GetValueByKeyWord("GEDI_INNERIFTYPE") != "提出资料"))
                                //{
                                //    if (wf.DefWorkFlow.O_Code == "INTERFACEWORKFLOW")
                                //    {
                                //        Doc doc2 = doc;
                                //        DateTime minValue = DateTime.MinValue;
                                //        if ((doc2.Project.DocList != null) && (doc2.Project.DocList.Count > 0))
                                //        {
                                //            doc2.Project.DocList.Sort(new Comparison<Doc>(CommonFunction.CompareByCode));
                                //            Doc doc3 = doc2.Project.DocList.Last<Doc>(dx => dx.TempDefn != null);
                                //            if ((doc3 != null) && ((doc3.TempDefn.Code == "GEDI_TRANSFERINGFORM") || (doc3.TempDefn.Code == "GEDI_VIEWFORM")))
                                //            {
                                //                if ((doc3.WorkFlow != null) && (doc3.WorkFlow.O_WorkFlowStatus == enWorkFlowStatus.Finish))
                                //                {
                                //                    minValue = doc3.WorkFlow.WorkStateList.Last<WorkState>().O_FinishDate.Value;
                                //                }
                                //            }
                                //            else if ((doc3 != null) && (doc3.TempDefn.Code == "IMPORTINTERFACEFILE"))
                                //            {
                                //                minValue = doc3.O_credatetime;
                                //            }
                                //        }
                                //    }
                                //}
                            }
                        }
                    }
                }
                #endregion
            }
            catch (Exception exception)
            {
                WebApi.CommonController.WebWriteLog(exception.ToString());
                //ErrorLog.WriteErrorLog(exception.ToString());
            }
        }


        /// <summary>
        /// 认质认价确认插件
        /// </summary>
        /// <param name="wf"></param>
        public static void ToConfireMan(WorkFlow wf)
        {
            return;
            try
            {
                #region 造价员提交到确认人员，把流程上的所有校审人员，添加到确认人员状态
                //造价员提交到确认人员，把流程上的所有校审人员，添加到确认人员状态
                if (wf.CuWorkState.DefWorkState.O_Code == "DIRCOSTCLERK2")
                {
                    //如果不存在确认人员状态，就获取流程上的所有人员，并放到确认人员状态
                    WorkState state = wf.WorkStateList.Find(ws => ws.DefWorkState.O_Code == "CONFIREMAN");
                    Server.Group gp = new Server.Group();

                    //添加项目专工
                    CommonFunction.AddWorkUserToGroup(wf, "PRODESIGN", ref gp);
                    //添加项目经理
                    CommonFunction.AddWorkUserToGroup(wf, "PROAPPROV", ref gp);
                    //添加成本控制部领导
                    CommonFunction.AddWorkUserToGroup(wf, "DIRLEADER", ref gp);
                    //添加成本控制员
                    CommonFunction.AddWorkUserToGroup(wf, "DIRECTOR", ref gp);
                    //添加财务部部长
                    CommonFunction.AddWorkUserToGroup(wf, "FINE", ref gp);
                    //添加招标部部长
                    CommonFunction.AddWorkUserToGroup(wf, "ZTBB", ref gp);

                    if (state == null)
                    {

                        //DefWorkState defWorkState = wf.DefWorkFlow.DefWorkStateList.Find(dwsx => dwsx.O_Code == "CONFIREMAN");
                        //state = wf.NewWorkState(defWorkState);
                        //state.SaveSelectUser(gp);

                        //state.IsRuning = false;

                        //state.PreWorkState = wsb.workState;
                        //state.O_iuser5 = new int?(wsb.workState.O_stateno);
                        //state.Modify();
                    }
                    else
                    {
                        //if (state.WorkUserList.Count <=1)
                        {
                            state.SaveSelectUser(gp);
                            //state.Modify();
                        }
                    }
                }
                #endregion
            }
            catch { }
        }

        /// <summary>
        /// 认质认价流程确认退回的时候删除所有确认的人员
        /// </summary>
        /// <param name="wf"></param>
        public static void HXBackDircostclerk(WorkFlow wf)
        {
            try
            {
                WorkState state = wf.WorkStateList.Find(ws => ws.DefWorkState.O_Code == "CONFIREMAN");
                //wf.WorkStateList.Remove(state);
                // wf.Modify();
                if (state != null) {
                    state.Delete();
                    //wf.Modify();
                //    state.WorkUserList[0].RemoveAll();
                //    state.Modify();
                }

                //重置造价员状态标志
                WorkState stateDir = wf.WorkStateList.Find(ws => ws.DefWorkState.O_Code == "DIRCOSTCLERK2");
                if (stateDir != null)
                {
                    stateDir.O_suser3 = null;
                    stateDir.Modify();
                }
            }
            catch { }
        }

        public static void DeleteRunning(WorkFlow wf)
        {
            try
            {
                if (wf.DefWorkFlow.O_Code == "COMMUNICATIONWORKFLOW")
                {
                    #region 代码逻辑
                    //代码逻辑
                    //1.获取部门文控：DEPARTMENTCONTROL 的有效的流程状态数量

                    //2.获取下一流程状态是文控：RECUNIT2 的流程状态的数量

                    //3.如果部门文控的有效的流程状态数量，等于下一流程状态是文控：RECUNIT2 的流程状态的数量，
                    //   就发送消息给文控：RECUNIT2；否则，就取消文控的运行状态 
                    #endregion

                    //1.获取部门文控：DEPARTMENTCONTROL 的有效的流程状态数量
                    List<WorkState> wsDCList = wf.WorkStateList.FindAll(ws => ws.DefWorkState.O_Code == "DEPARTMENTCONTROL");

                    //2.获取下一流程状态是文控：RECUNIT2 的流程状态的数量
                    List<WorkState> wsList = wf.WorkStateList.FindAll(ws => ws.NextWorkState != null && ws.NextWorkState.Code == "RECUNIT2");

                    //3.如果部门文控的有效的流程状态数量，等于下一流程状态是文控：RECUNIT2 的流程状态的数量，
                    //   就发送消息给文控：RECUNIT2
                    if (wsDCList.Count > 0 && wsList.Count > 0 && wsDCList.Count == wsList.Count)
                    {
                        List<User> mtoUser = new List<User>();
                       
                        //否则，就取消文控的运行状态
                        WorkState state = wf.WorkStateList.Find(wsx => wsx.Code == "RECUNIT2");
                        if (state != null)
                        {
                            foreach (WorkUser wu in state.WorkUserList)
                            {
                                if (wu.User != null)
                                {
                                    mtoUser.Add(wu.User);
                                }
                            }

                        }
                        if (mtoUser.Count > 0)
                        {

                            string str11 = wf.doc.Project.GetValueByKeyWord("HXNY_DOCUMENTSYSTEM_DESC");
                            if (string.IsNullOrEmpty(str11))
                                str11 = wf.doc.Project.GetValueByKeyWord("OPERATEADMIN_CODE");

                            string str12 = wf.doc.Project.GetValueByKeyWord("COM_SUBDOCUMENT_DESC");

                            string str13 = wf.doc.Project.Code;
                            string str14 = wf.doc.Code;
                            string mtitle = str11 + "_" + str12 + "_" + str13 + "的[" + str14 + "] 请关闭文件";


                            string mmessage = "※" + str11 + "_" + str12 + "_" + str13 + "的[" + str14 + "] ,  已处理该文件，请您关闭该文件,谢谢！";
                            wf.dBSource.SendMessage(wf.dBSource.LoginUser, mtoUser, null, mtitle, mmessage, null, wf.DocList, null, wf, null);
                        }
                    }

                    else
                    {
                        //否则，就取消文控的运行状态
                        WorkState state = wf.WorkStateList.Find(wsx => wsx.Code == "RECUNIT2");
                        if (state != null)
                        {
                            state.IsRuning = false;
                            state.Modify();
                        }
                    }

                    //if (((wf != null) && (wf.DocList != null)) && (wf.DocList.Count > 0))
                    //{

                    //    foreach (Doc doc in wf.DocList)
                    //    {
                    //        string str = doc.O_filename.ToUpper();
                    //        Hashtable htUserKeyWord = new Hashtable();
                    //        if ((wf.WorkStateList == null) || (wf.WorkStateList.Count <= 1))
                    //        {
                    //            continue;
                    //        }
                    //        int count = wf.WorkStateList.Count;

                    //        string code = wf.CuWorkState.Code;
                    //        if ((doc.WorkFlow != null) && (doc.WorkFlow.O_WorkFlowStatus == enWorkFlowStatus.Finish))
                    //        {
                    //            code = "END";
                    //        }

                    //        //判断是否文控关闭
                    //        if (code == "MAINHANDLE")
                    //        {
                    //            return;
                    //        }

                    //        WorkState state = wf.WorkStateList.Find(wsx => wsx.Code == "RECUNIT2");
                    //        state.IsRuning = false;
                    //        state.Modify();

                    //        break;
                    //    }
                    //}
                }
            }
            catch (Exception exception)
            {
                WebApi.CommonController.WebWriteLog(exception.ToString());
                //ErrorLog.WriteErrorLog(exception.ToString());
            }
        }


        //工作任务流程主任选择接收人时，自动创建用户任务
        public static void CreateUserTask(WorkFlow wf)
        {
            try
            {

                if (wf.CuWorkState.PreWorkState.Code == "DIRECTORSELECT" && (wf.DefWorkFlow.O_Code == "WORKTASK"))
                {
                    Project mProject = wf.Project;
                    if (mProject == null)
                    {
                        return;
                    }

                    ////添加用户任务
                    string strName = mProject.Code;//工作名称
                    string strWORKTEST = mProject.GetValueByKeyWord("WORKTEST");//工作内容
                    string strStartDate = mProject.GetValueByKeyWord("TASKPLANSTARTDATE");//开始时间
                    string strEndDate = mProject.GetValueByKeyWord("TASKPLANFINISHDATE"); //结束时间

                    DateTime dtStart = string.IsNullOrEmpty(strStartDate) ? DateTime.Now : Convert.ToDateTime(strStartDate);
                    DateTime dtEnd = string.IsNullOrEmpty(strEndDate) ? DateTime.Now : Convert.ToDateTime(strEndDate);

                    List<WorkUser> workUserList = wf.CuWorkState.WorkUserList;

                    foreach (WorkUser workUser in workUserList)
                    {
                        if (workUser != null && workUser.User != null)
                        {
                            User m_User = workUser.User;

                            if (m_User != null)
                            {
                                Task newTask = wf.dBSource.NewTask(enTaskLevel.Common, enTaskStatus.Runing, "任务", strName, strWORKTEST, m_User, null, dtStart, dtEnd, 0, null);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                WebApi.CommonController.WebWriteLog(exception.ToString());
                //ErrorLog.WriteErrorLog(exception.ToString());
            }
        }

        internal static WorkFlow GetDocWorkFlow(Doc doc)
        {
            try
            {
                if (doc.ShortCutDoc != null)
                {
                    return doc.ShortCutDoc.WorkFlow;
                }
                else
                {
                    return doc.WorkFlow;
                }

            }
            catch
            {

            }
            return null;
        }

        //internal static Server.Group GetDepartmentGroup(DBSource dbsource, string DepartmentCode, string GroupDesc) {
        //    //获取发文部门的部门领导
        //    AVEVA.CDMS.Server.Group group = dbsource.GetGroupByName(DepartmentCode);
        //    if (group != null)
        //    {
        //        AVEVA.CDMS.Server.Group ldGroup = group.AllGroupList.Find(g => g.Description == GroupDesc);
        //        if (ldGroup != null)
        //        {
        //            return ldGroup;
        //        }
        //    }
        //    return null;

        //}
        ////
        /// <summary>
        /// 把用户字符串转换成用户列表
        /// </summary>
        /// <param name="dbsource"></param>
        /// <param name="userlist">用户列表，格式："用户代码1__用户名1，用户代码2__用户名2。。。"</param>
        /// <returns></returns>
        public static Server.Group StrToGroup(DBSource dbsource, string userlist)
        {
            AVEVA.CDMS.Server.Group userGroup = new AVEVA.CDMS.Server.Group();
            try
            {
                string[] strArray = (string.IsNullOrEmpty(userlist) ? "" : userlist).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string struser in strArray)
                {
                    Object objuser = dbsource.GetObjectByKeyWord(struser);

                    if (objuser == null) break;
                    if (objuser is User)
                    {
                        userGroup.AddUser(objuser as User);
                        // userList.Add(user);
                    }

                }
            }
            catch { }
            return userGroup;
        }

        /// <summary>
        /// 转换用户字符串，获取只包含用户代码的字符串
        /// </summary>
        /// <param name="userlist">用户列表，格式："用户代码1__用户名1，用户代码2__用户名2。。。"</param>
        /// <returns>
        /// 返回格式："用户代码1，用户代码2。。。"
        /// </returns>
        public static string getUserCodelist(string userlist)
        {
            string strUserCodeList = "";

            string[] strArray = (string.IsNullOrEmpty(userlist) ? "" : userlist).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string struser in strArray)
            {
                string userCode = struser.Substring(0, struser.IndexOf("__"));
                strUserCodeList = strUserCodeList + userCode + ",";
            }

            if (strUserCodeList.EndsWith(","))
            {
                strUserCodeList = strUserCodeList.Remove(strUserCodeList.Length - 1);
            }

            return strUserCodeList;
        }
        public static int CompareByCode(Doc x, Doc y)
        {
            CaseInsensitiveComparer comparer = new CaseInsensitiveComparer();
            return comparer.Compare(x.O_credatetime, y.O_credatetime);
        }

        /// <summary>
        /// 根据模板获取父目录
        /// </summary>
        /// <param name="curProj"></param>
        /// <param name="TempDefnKeyWord"></param>
        /// <returns></returns>
        internal static Project getParentProjectByTempDefn(Project curProj, string TempDefnKeyWord)
        {
            //Project proj = null;

            #region 获取项目名称
            Project proj = curProj;
            Project rootProj = null;
            //string rootProjDesc = "";
            try
            {
                while (true)
                {
                    if (proj.TempDefn != null && proj.TempDefn.KeyWord == TempDefnKeyWord)
                    {
                        rootProj = proj;
                        //rootProjDesc = proj.Description;
                        break;
                    }
                    else
                    {
                        if (proj.ParentProject == null)
                        {
                            break;
                        }
                        else
                        {
                            proj = proj.ParentProject;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                WebApi.CommonController.WebWriteLog(DateTime.Now.ToString() + ":" + "根据模板获取项目错误," + ex.Message);

            }
            #endregion
            return rootProj;

        }

        /// <summary>
        /// 判断是否发送给项目
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        internal static bool GetIfSendToProject(Doc doc) {
            bool isToProj = false;
            //AttrData data=doc.GetAttrDataByKeyWord("CA_SENDCODE");
            //string sendcode = data.ToString;
            //if (data != null)
            ////判断是否发送给项目
            //{
            //    Regex regex = new Regex(@"^\w+\-\w+\-\w+\-\w+\-\w+$");
            //    if (regex.IsMatch(sendcode))
            //    {
            //        isToProj = true;
            //    }
            //}
            AttrData data = doc.GetAttrDataByKeyWord("CA_PROCODE");

            if (data != null) {
                string procode = data.ToString;
                if (!string.IsNullOrEmpty(procode))
                {
                    isToProj = true;
                }
            }
            return isToProj;
        }
        //internal static string GetSecrearilManByUnitCode(DBSource dbsource, string UnitCode)
        //{
        //    string secUserList = "";
        //    //从组织机构里面查找文控
        //    Server.Group gp = dbsource.GetGroupByName(UnitCode);
        //    if (gp == null)
        //    {
        //        return "";
        //    }
        //    foreach (Server.Group g in gp.AllGroupList)
        //    {
        //        if (g.Description == "文控")
        //        {

        //            foreach (User user in g.AllUserList)
        //            {
        //                secUserList = user.ToString + ",";

        //            }
        //            if (!string.IsNullOrWhiteSpace(secUserList))
        //            {
        //                secUserList = secUserList.Substring(0, secUserList.Length - 1);
        //            }
        //            break;
        //        }
        //    }
        //    return secUserList;
        //}

        /// <summary>
        /// 获取组织机构的文控组
        /// </summary>
        /// <param name="dbsource"></param>
        /// <param name="UnitCode"></param>
        /// <returns></returns>
        internal static Server.Group GetSecGroupByUnitCode(DBSource dbsource, string UnitCode)
        {
            string secUserList = "";
            //从组织机构里面查找文控
            Server.Group gp = dbsource.GetGroupByName(UnitCode);
            if (gp == null)
            {
                return null;
            }
            Server.Group resultGp = null;
            foreach (Server.Group g in gp.AllGroupList)
            {
                if (g.Description == "文控")
                {

                    resultGp = g;
                    break;
                }
            }

            #region 组织机构里面找不到文控，再到数据字典里面找
            if (resultGp == null)
            {
                //组织机构里面找不到文控，再到数据字典里面找
                List<DictData> dictDataList = dbsource.GetDictDataList("Communication");
                //[o_Code]:英文描述,[o_Desc]：中文描述,[o_sValue1]：通信代码
                //string str3 = m_Project.ExcuteDefnExpression("$(DESIGNPROJECT_CODE)")[0];
                foreach (DictData data6 in dictDataList)
                {
                    if (!string.IsNullOrEmpty(data6.O_sValue1) && data6.O_sValue1 == UnitCode)
                    {
                        secUserList = data6.O_sValue4;
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(secUserList))
                {
                    resultGp = GetGroupByFullName(dbsource, secUserList);
                }
                
            } 
            #endregion

            return resultGp;
        }

        //获取项目部门的文控
        //internal static User GetSecUserByDepartmentCode(DBSource dbsource, string DepartmentCode) {
        internal static string GetSecUserByDepartmentCode(DBSource dbsource, string DepartmentCode)
        {
            try
            {
                //先再组织机构里面获取文控组
                Server.Group gp = GetSecGroupByUnitCode(dbsource, DepartmentCode);
                if (gp != null)
                {
                    if (gp.AllUserList.Count > 0)
                    {
                        User user = gp.AllUserList[0];
                        return user.ToString;
                    }
                }
                else
                {
                    //如果组织机构里面没有，再从项目目录里面获取文控
                    Project m_RootProject = dbsource.RootLocalProjectList.Find(itemProj => itemProj.TempDefn.KeyWord == "PRODOCUMENTADMIN");
                    if (m_RootProject == null)
                    {
                        return null;
                    }

                    foreach (Project proj in m_RootProject.AllProjectList)
                    {
                        try
                        {
                            if (proj.TempDefn.KeyWord == "HXNY_DOCUMENTSYSTEM")
                            {//&& proj.Code== DepartmentCode) {
                                string strONSHORE = proj.GetAttrDataByKeyWord("RPO_ONSHORE").ToString;//通信代码
                                string strOFFSHORE = proj.GetAttrDataByKeyWord("RPO_OFFSHORE").ToString;//通信代码
                                if (strONSHORE == DepartmentCode || strOFFSHORE == DepartmentCode || proj.Code == DepartmentCode)
                                {
                                    string strUser = proj.GetAttrDataByKeyWord("SECRETARILMAN").ToString;//文件附件
                                    return strUser;
                                }
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { }
            return null;
        }

        internal static Server.Group GetUserRootOrgGroup(User curUser)
        {//获取组织机构用户组
            try
            {
                foreach (AVEVA.CDMS.Server.Group groupOrg in curUser.dBSource.AllGroupList)
                {
                    if ((groupOrg.ParentGroup == null) && (groupOrg.O_grouptype == enGroupType.Organization))
                    {
                        if (groupOrg.AllUserList.Contains(curUser))
                        {
                            return groupOrg;
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        /// <summary>
        /// 添加流程状态用户到用户组
        /// </summary>
        /// <param name="wf"></param>
        /// <param name="WorkStateCode"></param>
        /// <param name="gp"></param>
        internal static void AddWorkUserToGroup(WorkFlow wf,string WorkStateCode, ref Server.Group gp) {
            try
            {
                WorkState state = wf.WorkStateList.Find(ws => ws.DefWorkState.O_Code == WorkStateCode);
                if (state != null) {
                    foreach (WorkUser wu in state.WorkUserList)
                    {
                        if (!gp.UserList.Contains(wu.User))
                        {
                            gp.AddUser(wu.User);
                        }
                    }
                }
            }
            catch { }
            //return null;
        }

        internal static Doc GetWorkFlowDoc(WorkFlow wf)
        {
            Doc resultDoc = null;
            foreach (Doc doc in wf.DocList)
            {
                if (doc.TempDefn == null) continue;

                if (doc.TempDefn.KeyWord != "CATALOGUING") continue;

                AttrData data;
                if ((data = doc.GetAttrDataByKeyWord("CA_ATTRTEMP")) != null)
                {
                    string strData = data.ToString;
                    if (wf.DefWorkFlow.O_Code == "COMMUNICATIONWORKFLOW")
                    {
                        if (strData == "LETTERFILE" || strData == "FILETRANSMIT" ||
                           strData == "MEETINGSUMMARY" || strData == "DOCUMENTFILE")
                        {
                            resultDoc = doc;
                            return resultDoc;
                        }
                    }
                    else if (wf.DefWorkFlow.O_Code == "RECOGNITION")
                    {
                        if (strData == "RECOGNITIONFILE")
                        {
                            resultDoc = doc;
                            return resultDoc;
                        }
                    }
                }
            }
            return resultDoc;
        }

        internal static bool ConventDocToPdf(Doc cpdoc) {
            try
            {

                string sourceFileName = cpdoc.FullPathFile;
                string targetFileName = sourceFileName.Substring(0, sourceFileName.LastIndexOf(".") + 1) + "pdf";

                CDMSPdf.ConvertToPdf(cpdoc.FullPathFile, targetFileName);
                cpdoc.O_filename = cpdoc.O_filename.Substring(0, cpdoc.O_filename.LastIndexOf(".") + 1) + "pdf";

                cpdoc.Modify();
                return true;
            }
            catch { }
            return false;
        }

        /// <summary>
        /// 获取某个部门的指定用户组 
        /// </summary>
        /// <param name="dbsource"></param>
        /// <param name="UnitCode"></param>
        /// <param name="GroupDesc"></param>
        /// <returns></returns>
        internal static Server.Group GetGroupByDesc(DBSource dbsource,string UnitCode,string GroupDesc) {
          
            Server.Group resultGroup = new Server.Group();
            try {
                //从组织机构里面查找部长
                Server.Group gp = new Server.Group();
                string approvUserList = "";
            gp = dbsource.GetGroupByName(UnitCode);
            foreach (Server.Group g in gp.AllGroupList)
            {
                if (g.Description == GroupDesc)
                {

                    foreach (User user in g.AllUserList)
                    {
                        approvUserList = user.ToString + ",";

                    }
                    if (!string.IsNullOrWhiteSpace(approvUserList))
                    {
                        approvUserList = approvUserList.Substring(0, approvUserList.Length - 1);
                    }
                    break;
                }
            }

            string[] approvUserArray = (string.IsNullOrEmpty(approvUserList) ? "" : approvUserList).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

           


            foreach (string strObj in approvUserArray)
            {
                string strUser = strObj.IndexOf("__") >= 0 ? strObj.Substring(0, strObj.IndexOf("__")) : strObj;

                object obj = dbsource.GetUserByName(strUser);

                if (obj is User)
                {
                        //m_UserList.Add((User)obj);
                        resultGroup.AddUser((User)obj);
                }
            }
                return resultGroup;
            }
            catch
            {
            }
            return resultGroup;
        }

       /// <summary>
       /// 获取项目文控
       /// </summary>
       /// <param name="dbsource"></param>
       /// <param name="ProjectCode"></param>
       /// <returns></returns>
        internal static Server.Group GetProjectSecUser(DBSource dbsource,string ProjectCode) {
            //获取项目目录
            Server.Group recGroup = null;
            try
            {
                Project rProj = dbsource.RootLocalProjectList.Find(itemProj => itemProj.TempDefn.KeyWord == "PRODOCUMENTADMIN");
                Project pProj = rProj.ChildProjectList.Find(p => p.Code == ProjectCode);
                if (pProj != null)
                {
                    string strSecUser = pProj.GetValueByKeyWord("SECRETARILMAN");
                    string strUser = strSecUser.IndexOf("__") >= 0 ? strSecUser.Substring(0, strSecUser.IndexOf("__")) : strSecUser;
                    dbsource.GetUserByCode(strUser);
                    object obj = dbsource.GetUserByName(strUser);

                    if (obj is User)
                    {
                        recGroup = new Server.Group();
                        recGroup.AddUser((User)obj);
                    }
                }
            }
            catch { }
            return recGroup;
        }

        internal static User GetUserByFullName(DBSource dbsource, string UserFullName) {
            try
            {
                string strUser = UserFullName.IndexOf("__") >= 0 ? UserFullName.Substring(0, UserFullName.IndexOf("__")) : UserFullName;
                dbsource.GetUserByCode(strUser);
                object obj = dbsource.GetUserByName(strUser);

                if (obj is User)
                {
                    return ((User)obj);
                }
            }
            catch (Exception ex){

            }
            return null;
        }

        internal static Server.Group GetGroupByFullName(DBSource dbsource, string GroupFullName) {
            try
            {
                Server.Group result = new Server.Group();
                string[] strArray = GroupFullName.Split(new char[] { ',' });
                foreach (string UserFullName in strArray)
                {
                    string strUser = UserFullName.IndexOf("__") >= 0 ? UserFullName.Substring(0, UserFullName.IndexOf("__")) : UserFullName;
                    dbsource.GetUserByCode(strUser);
                    object obj = dbsource.GetUserByName(strUser);

                    if (obj is User)
                    {
                        result.AddUser((User)obj);
                    }
                }
                if (result.UserList.Count > 0)
                {
                    return result;
                }
                else {
                    return null;
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }
    }

    public class CataloguDoc
    {
        public Doc doc;
        public Project Project
        {
            get
            {
                if (doc == null) return null;
                return doc.Project;
            }
        }

        public WorkFlow WorkFlow
        {
            get
            {
                if (doc == null) return null;
                return doc.WorkFlow;
            }
        }

        public List<AttrData> AttrDataList
        {
            get
            {
                if (doc == null) return null;
                return doc.AttrDataList;
            }
        }

        /// <summary>
        /// 文档模板代码
        /// </summary>
        public string TempDefnCode
        {
            get
            {
                return "CATALOGUING";
            }
        }

        /// <summary>
        /// 信函模板关键字
        /// </summary>
        public string LetterAttrTemp
        {
            get
            {
                return "LETTERFILE";
            }
        }

        public string TransmittalAttrTemp
        {
            get
            {
                return "FILETRANSMIT";
            }
        }

        public bool isCataloguDoc {
            get
            {
                if (doc == null ) return false;

                TempDefn docTempDefn = doc.TempDefn;

                if(docTempDefn==null) return false;

                return docTempDefn.Code == TempDefnCode;
                //return "CATALOGUING";
            }
        }

        /// <summary>
        /// 收文单位代码
        /// </summary>
        public string CA_MAINFEEDERCODE
        {
            get
            {
                return GetAttrDataValue("CA_MAINFEEDERCODE");
            }
            set
            {
                SetAttrDataValue("CA_MAINFEEDERCODE", value);
            }
        }

        /// <summary>
        /// 文件标题
        /// </summary>
        public string CA_FILETITLE
        {
            get
            {
                return GetAttrDataValue("CA_FILETITLE");
            }
            set
            {
                SetAttrDataValue("CA_FILETITLE", value);
            }
        }

        //页数
        public string CA_PAGE
        {
            get
            {
                return GetAttrDataValue("CA_PAGE");
            }
            set
            {
                SetAttrDataValue("CA_PAGE", value);
            }
        }

        /// <summary>
        /// 主送方
        /// </summary>
        public string CA_MAINFEEDER
        {
            get
            {
                return GetAttrDataValue("CA_MAINFEEDER");
            }
            set
            {
                SetAttrDataValue("CA_MAINFEEDER", value);
            }
        }
        /// <summary>
        /// 发文单位代码
        /// </summary>
        public string CA_SENDERCODE
        {
            get
            {
                return GetAttrDataValue("CA_SENDERCODE");
            }
            set
            {
                SetAttrDataValue("CA_SENDERCODE", value);
            }
        }

        /// <summary>
        /// 发文单位描述
        /// </summary>
        public string CA_SENDUNIT
        {
            get
            {
                return GetAttrDataValue("CA_SENDUNIT");
            }
            set
            {
                SetAttrDataValue("CA_SENDUNIT", value);
            }
        }

        /// <summary>
        /// 收文单位描述
        /// </summary>
        public string CA_RECUNIT
        {
            get
            {
                return GetAttrDataValue("CA_RECUNIT");
            }
            set
            {
                SetAttrDataValue("CA_RECUNIT", value);
            }
        }

        /// <summary>
        /// 收文日期
        /// </summary>
        public string CA_RECDATE
        {
            get
            {
                return GetAttrDataValue("CA_RECDATE");
            }
            set
            {
                SetAttrDataValue("CA_RECDATE", value);
            }
        }

        /// <summary>
        /// 收文编码
        /// </summary>
        public string CA_RECEIPTCODE
        {
            get
            {
                return GetAttrDataValue("CA_RECEIPTCODE");
            }
            set
            {
                SetAttrDataValue("CA_RECEIPTCODE", value);
            }
        }

        /// <summary>
        /// 收文编码
        /// </summary>
        public string CA_SENDCODE
        {
            get
            {
                return GetAttrDataValue("CA_SENDCODE");
            }
            set
            {
                SetAttrDataValue("CA_SENDCODE", value);
            }
        }

        /// <summary>
        /// 份数
        /// </summary>
        public string CA_NUMBER
        {
            get
            {
                return GetAttrDataValue("CA_NUMBER");
            }
            set
            {
                SetAttrDataValue("CA_NUMBER", value);
            }
        }

        /// <summary>
        /// 文件ID
        /// </summary>
        public string CA_FILEID
        {
            get
            {
                return GetAttrDataValue("CA_FILEID");
            }
            set
            {
                SetAttrDataValue("CA_FILEID", value);
            }
        }

        public string CA_SENDER
        {
            get
            {
                return GetAttrDataValue("CA_SENDER");
            }
            set
            {
                SetAttrDataValue("CA_SENDER", value);
            }
        }

        public string CA_COPY
        {
            get
            {
                return GetAttrDataValue("CA_COPY");
            }
            set
            {
                SetAttrDataValue("CA_COPY", value);
            }
        }

        public string CA_COPYCODE
        {
            get
            {
                return GetAttrDataValue("CA_COPYCODE");
            }
            set
            {
                SetAttrDataValue("CA_COPYCODE", value);
            }
        }

        /// <summary>
        /// 发文单位类型
        /// </summary>
        public string CA_SENDERCLASS { get { return GetAttrDataValue("CA_SENDERCLASS"); } set { SetAttrDataValue("CA_SENDERCLASS", value); } }
        /// <summary>
        /// 收文单位类型
        /// </summary>
        public string CA_RECEIVERCLASS { get { return GetAttrDataValue("CA_RECEIVERCLASS"); } set { SetAttrDataValue("CA_RECEIVERCLASS", value); } }
        /// <summary>
        /// 发文日期
        /// </summary>
        public string CA_SENDDATE { get { return GetAttrDataValue("CA_SENDDATE"); } set { SetAttrDataValue("CA_SENDDATE", value); } }
        /// <summary>
        /// 密级
        /// </summary>
        public string CA_SECRETGRADE { get { return GetAttrDataValue("CA_SECRETGRADE"); } set { SetAttrDataValue("CA_SECRETGRADE", value); } }
        /// <summary>
        /// 保密期限
        /// </summary>
        public string CA_SECRETTERM { get { return GetAttrDataValue("CA_SECRETTERM"); } set { SetAttrDataValue("CA_SECRETTERM", value); } }
        /// <summary>
        /// 是否需要回复
        /// </summary>
        public string CA_IFREPLY { get { return GetAttrDataValue("CA_IFREPLY"); } set { SetAttrDataValue("CA_IFREPLY", value); } }
        /// <summary>
        /// 回复日期
        /// </summary>
        public string CA_REPLYDATE { get { return GetAttrDataValue("CA_REPLYDATE"); } set { SetAttrDataValue("CA_REPLYDATE", value); } }
        /// <summary>
        /// 审核级数
        /// </summary>
        public string CA_SERIES { get { return GetAttrDataValue("CA_SERIES"); } set { SetAttrDataValue("CA_SERIES", value); } }
        /// <summary>
        /// 摘要
        /// </summary>
        public string CA_ABSTRACT { get { return GetAttrDataValue("CA_ABSTRACT"); } set { SetAttrDataValue("CA_ABSTRACT", value); } }
        /// <summary>
        /// 附件
        /// </summary>
        public string CA_ENCLOSURE { get { return GetAttrDataValue("CA_ENCLOSURE"); } set { SetAttrDataValue("CA_ENCLOSURE", value); } }
        /// <summary>
        /// 紧急程度
        /// </summary>
        public string CA_URGENTDEGREE { get { return GetAttrDataValue("CA_URGENTDEGREE"); } set { SetAttrDataValue("CA_URGENTDEGREE", value); } }
        /// <summary>
        /// 文件编码
        /// </summary>
        public string CA_FILECODE { get { return GetAttrDataValue("CA_FILECODE"); } set { SetAttrDataValue("CA_FILECODE", value); } }
        /// <summary>
        /// 原文件编码
        /// </summary>
        public string CA_ORIFILECODE { get { return GetAttrDataValue("CA_ORIFILECODE"); } set { SetAttrDataValue("CA_ORIFILECODE", value); } }
        /// <summary>
        /// 项目代码
        /// </summary>
        public string CA_PROCODE { get { return GetAttrDataValue("CA_PROCODE"); } set { SetAttrDataValue("CA_PROCODE", value); } }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string CA_PRONAME { get { return GetAttrDataValue("CA_PRONAME"); } set { SetAttrDataValue("CA_PRONAME", value); } }
        /// <summary>
        /// 原文件名
        /// </summary>
        public string CA_ORIFILETITLE { get { return GetAttrDataValue("CA_ORIFILETITLE"); } set { SetAttrDataValue("CA_ORIFILETITLE", value); } }
        /// <summary>
        /// 主题
        /// </summary>
        public string CA_TITLE { get { return GetAttrDataValue("CA_TITLE"); } set { SetAttrDataValue("CA_TITLE", value); } }
        /// <summary>
        /// 正文内容
        /// </summary>
        public string CA_CONTENT { get { return GetAttrDataValue("CA_CONTENT"); } set { SetAttrDataValue("CA_CONTENT", value); } }
        /// <summary>
        /// 备注
        /// </summary>
        public string CA_NOTE { get { return GetAttrDataValue("CA_NOTE"); } set { SetAttrDataValue("CA_NOTE", value); } }
        /// <summary>
        /// 签发备注
        /// </summary>
        public string CA_DESNOTE { get { return GetAttrDataValue("CA_DESNOTE"); } set { SetAttrDataValue("CA_DESNOTE", value); } }
        /// <summary>
        /// 机组
        /// </summary>
        public string CA_CREW { get { return GetAttrDataValue("CA_CREW"); } set { SetAttrDataValue("CA_CREW", value); } }
        /// <summary>
        /// 厂房代码
        /// </summary>
        public string CA_FACTORY { get { return GetAttrDataValue("CA_FACTORY"); } set { SetAttrDataValue("CA_FACTORY", value); } }
        /// <summary>
        /// 系统
        /// </summary>
        public string CA_SYSTEM { get { return GetAttrDataValue("CA_SYSTEM"); } set { SetAttrDataValue("CA_SYSTEM", value); } }
        /// <summary>
        /// 来文类型
        /// </summary>
        public string CA_RECTYPE { get { return GetAttrDataValue("CA_RECTYPE"); } set { SetAttrDataValue("CA_RECTYPE", value); } }
        /// <summary>
        /// 工作类型
        /// </summary>
        public string CA_WORKTYPE { get { return GetAttrDataValue("CA_WORKTYPE"); } set { SetAttrDataValue("CA_WORKTYPE", value); } }
        /// <summary>
        /// 工作分项
        /// </summary>
        public string CA_WORKSUBTIEM { get { return GetAttrDataValue("CA_WORKSUBTIEM"); } set { SetAttrDataValue("CA_WORKSUBTIEM", value); } }
        /// <summary>
        /// 专业
        /// </summary>
        public string CA_MAJOR { get { return GetAttrDataValue("CA_MAJOR"); } set { SetAttrDataValue("CA_MAJOR", value); } }
        /// <summary>
        /// 文件分类
        /// </summary>
        public string CA_DOCSOURCE { get { return GetAttrDataValue("CA_DOCSOURCE"); } set { SetAttrDataValue("CA_DOCSOURCE", value); } }
        /// <summary>
        /// 档号
        /// </summary>
        public string CA_REFERENCE { get { return GetAttrDataValue("CA_REFERENCE"); } set { SetAttrDataValue("CA_REFERENCE", value); } }
        /// <summary>
        /// 语种
        /// </summary>
        public string CA_LANGUAGES { get { return GetAttrDataValue("CA_LANGUAGES"); } set { SetAttrDataValue("CA_LANGUAGES", value); } }
        /// <summary>
        /// 关联文件编码
        /// </summary>
        public string CA_RELATIONFILECODE { get { return GetAttrDataValue("CA_RELATIONFILECODE"); } set { SetAttrDataValue("CA_RELATIONFILECODE", value); } }
        /// <summary>
        /// 关联文件名称
        /// </summary>
        public string CA_RELATIONFILENAME { get { return GetAttrDataValue("CA_RELATIONFILENAME"); } set { SetAttrDataValue("CA_RELATIONFILENAME", value); } }
        /// <summary>
        /// 状态
        /// </summary>
        public string CA_STATE { get { return GetAttrDataValue("CA_STATE"); } set { SetAttrDataValue("CA_STATE", value); } }
        /// <summary>
        /// 电子文件
        /// </summary>
        public string CA_ELDOCUMENT { get { return GetAttrDataValue("CA_ELDOCUMENT"); } set { SetAttrDataValue("CA_ELDOCUMENT", value); } }
        /// <summary>
        /// 原件
        /// </summary>
        public string CA_ORIGINAL { get { return GetAttrDataValue("CA_ORIGINAL"); } set { SetAttrDataValue("CA_ORIGINAL", value); } }
        /// <summary>
        /// 复印件
        /// </summary>
        public string CA_COPYFILE { get { return GetAttrDataValue("CA_COPYFILE"); } set { SetAttrDataValue("CA_COPYFILE", value); } }
        /// <summary>
        /// 载体
        /// </summary>
        public string CA_CARRIER { get { return GetAttrDataValue("CA_CARRIER"); } set { SetAttrDataValue("CA_CARRIER", value); } }
        /// <summary>
        /// 扫描件
        /// </summary>
        public string CA_SCANS { get { return GetAttrDataValue("CA_SCANS"); } set { SetAttrDataValue("CA_SCANS", value); } }
        /// <summary>
        /// 归档文件清单编码
        /// </summary>
        public string CA_FILELISTCODE { get { return GetAttrDataValue("CA_FILELISTCODE"); } set { SetAttrDataValue("CA_FILELISTCODE", value); } }
        /// <summary>
        /// 传递方式
        /// </summary>
        public string CA_TRANSMITMETHOD { get { return GetAttrDataValue("CA_TRANSMITMETHOD"); } set { SetAttrDataValue("CA_TRANSMITMETHOD", value); } }
        /// <summary>
        /// 传递方式说明
        /// </summary>
        public string CA_TRANSMITMETHODSUPP { get { return GetAttrDataValue("CA_TRANSMITMETHODSUPP"); } set { SetAttrDataValue("CA_TRANSMITMETHODSUPP", value); } }
        /// <summary>
        /// 提交目的
        /// </summary>
        public string CA_SUBMISSIONOBJ { get { return GetAttrDataValue("CA_SUBMISSIONOBJ"); } set { SetAttrDataValue("CA_SUBMISSIONOBJ", value); } }
        /// <summary>
        /// 提交目的说明
        /// </summary>
        public string CA_SUBMISSIONOBJSUPP { get { return GetAttrDataValue("CA_SUBMISSIONOBJSUPP"); } set { SetAttrDataValue("CA_SUBMISSIONOBJSUPP", value); } }
        /// <summary>
        /// 会议时间
        /// </summary>
        public string CA_TIME { get { return GetAttrDataValue("CA_TIME"); } set { SetAttrDataValue("CA_TIME", value); } }
        /// <summary>
        /// 卷内序号
        /// </summary>
        public string CA_VOLUMENUMBER { get { return GetAttrDataValue("CA_VOLUMENUMBER"); } set { SetAttrDataValue("CA_VOLUMENUMBER", value); } }
        /// <summary>
        /// 厂房名称
        /// </summary>
        public string CA_FACTORYNAME { get { return GetAttrDataValue("CA_FACTORYNAME"); } set { SetAttrDataValue("CA_FACTORYNAME", value); } }
        /// <summary>
        /// 系统名称
        /// </summary>
        public string CA_SYSTEMNAME { get { return GetAttrDataValue("CA_SYSTEMNAME"); } set { SetAttrDataValue("CA_SYSTEMNAME", value); } }
        /// <summary>
        /// 归档日期
        /// </summary>
        public string CA_FILELISTTIME { get { return GetAttrDataValue("CA_FILELISTTIME"); } set { SetAttrDataValue("CA_FILELISTTIME", value); } }
        /// <summary>
        /// 联系人
        /// </summary>
        public string CA_CONTACTSMAN { get { return GetAttrDataValue("CA_CONTACTSMAN"); } set { SetAttrDataValue("CA_CONTACTSMAN", value); } }
        /// <summary>
        /// 责任人
        /// </summary>
        public string CA_RESPONSIBILITY { get { return GetAttrDataValue("CA_RESPONSIBILITY"); } set { SetAttrDataValue("CA_RESPONSIBILITY", value); } }
        /// <summary>
        /// 排架号
        /// </summary>
        public string CA_RACKNUMBER { get { return GetAttrDataValue("CA_RACKNUMBER"); } set { SetAttrDataValue("CA_RACKNUMBER", value); } }
        /// <summary>
        /// 介质
        /// </summary>
        public string CA_MEDIUM { get { return GetAttrDataValue("CA_MEDIUM"); } set { SetAttrDataValue("CA_MEDIUM", value); } }
        /// <summary>
        /// 案卷规格
        /// </summary>
        public string CA_FILESPEC { get { return GetAttrDataValue("CA_FILESPEC"); } set { SetAttrDataValue("CA_FILESPEC", value); } }
        /// <summary>
        /// 归档单位
        /// </summary>
        public string CA_FILEUNIT { get { return GetAttrDataValue("CA_FILEUNIT"); } set { SetAttrDataValue("CA_FILEUNIT", value); } }
        /// <summary>
        /// 版次
        /// </summary>
        public string CA_EDITION { get { return GetAttrDataValue("CA_EDITION"); } set { SetAttrDataValue("CA_EDITION", value); } }
        /// <summary>
        /// 会议编码
        /// </summary>
        public string CA_SUMMARYCODE { get { return GetAttrDataValue("CA_SUMMARYCODE"); } set { SetAttrDataValue("CA_SUMMARYCODE", value); } }
        /// <summary>
        /// 文件属性
        /// </summary>
        public string CA_ATTRTEMP { get { return GetAttrDataValue("CA_ATTRTEMP"); } set { SetAttrDataValue("CA_ATTRTEMP", value); } }
        /// <summary>
        /// 协办部门
        /// </summary>
        public string CA_ASSISTHANDLEUNIT { get { return GetAttrDataValue("CA_ASSISTHANDLEUNIT"); } set { SetAttrDataValue("CA_ASSISTHANDLEUNIT", value); } }

        /// <summary>
        /// 主办人
        /// </summary>
        public string CA_MAINHANDLE { get { return GetAttrDataValue("CA_MAINHANDLE"); } set { SetAttrDataValue("CA_MAINHANDLE", value); } }
        /// <summary>
        /// 主办人代码
        /// </summary>
        public string CA_MAINHANDLE_CODE { get { return GetAttrDataValue("CA_MAINHANDLE_CODE"); } set { SetAttrDataValue("CA_MAINHANDLE_CODE", value); } }
        /// <summary>
        /// 主办人名称
        /// </summary>
        public string CA_MAINHANDLE_DESC { get { return GetAttrDataValue("CA_MAINHANDLE_DESC"); } set { SetAttrDataValue("CA_MAINHANDLE_DESC", value); } }
        /// <summary>
        /// 保管期限
        /// </summary>
        public string CA_KEEPINGTIME { get { return GetAttrDataValue("CA_KEEPINGTIME"); } set { SetAttrDataValue("CA_KEEPINGTIME", value); } }
        /// <summary>
        /// 文件编码里面的来文类型
        /// </summary>
        public string CA_FILETYPE { get { return GetAttrDataValue("CA_FILETYPE"); } set { SetAttrDataValue("CA_FILETYPE", value); } }
        /// <summary>
        /// 文件编码里面的部门
        /// </summary>
        public string CA_UNIT { get { return GetAttrDataValue("CA_UNIT"); } set { SetAttrDataValue("CA_UNIT", value); } }
        /// <summary>
        /// 文件编码里面的流水号
        /// </summary>
        public string CA_FLOWNUMBER { get { return GetAttrDataValue("CA_FLOWNUMBER"); } set { SetAttrDataValue("CA_FLOWNUMBER", value); } }
        /// <summary>
        /// 收文ID
        /// </summary>
        public string CA_RECID { get { return GetAttrDataValue("CA_RECID"); } set { SetAttrDataValue("CA_RECID", value); } }
        /// <summary>
        /// 合同号
        /// </summary>
        public string CA_CONTRACTCODE { get { return GetAttrDataValue("CA_CONTRACTCODE"); } set { SetAttrDataValue("CA_CONTRACTCODE", value); } }
        /// <summary>
        /// 采购类型
        /// </summary>
        public string CA_PURCHASETYPE { get { return GetAttrDataValue("CA_PURCHASETYPE"); } set { SetAttrDataValue("CA_PURCHASETYPE", value); } }
        
        /// <summary>
        /// 回文编码
        /// </summary>
        public string CA_REPLYCODE { get { return GetAttrDataValue("CA_REPLYCODE"); } set { SetAttrDataValue("CA_REPLYCODE", value); } }

        /// <summary>
        /// 预发文编码
        /// </summary>
        public string CA_EXPSENDCODE { get { return GetAttrDataValue("CA_EXPSENDCODE"); } set { SetAttrDataValue("CA_EXPSENDCODE", value); } }

        /// <summary>
        /// 阅知部门代码
        /// </summary>
        public string CA_READUNIT { get { return GetAttrDataValue("CA_READUNIT"); } set { SetAttrDataValue("CA_READUNIT", value); } }

        /// <summary>
        /// 编制人
        /// </summary>
        public string CA_DESIGN { get { return GetAttrDataValue("CA_DESIGN"); } set { SetAttrDataValue("CA_DESIGN", value); } }

        /// <summary>
        /// 生效日期
        /// </summary>
        public string CA_APPROVTIME { get { return GetAttrDataValue("CA_APPROVTIME"); } set { SetAttrDataValue("CA_APPROVTIME", value); } }

        /// <summary>
        /// 回文日期
        /// </summary>
        public string CA_REPLYTIME { get { return GetAttrDataValue("CA_REPLYTIME"); } set { SetAttrDataValue("CA_REPLYTIME", value); } }

        private string GetAttrDataValue(string attrName)
        {
            string result = "";
            if (string.IsNullOrEmpty(attrName)) return result;
            if (doc != null)
            {
                AttrData data;
                //项目名称
                if ((data = doc.GetAttrDataByKeyWord(attrName)) != null)
                {
                    result = data.ToString;
                }
            }
            return result;
        }

        

        private void SetAttrDataValue(string attrName, string attrValue)
        {
            if (doc != null)
            {
                AttrData data;
                //项目名称
                if ((data = doc.GetAttrDataByKeyWord(attrName)) != null)
                {
                    data.SetCodeDesc(attrValue);
                    //doc.AttrDataList.SaveData();
                }
            }
        }

        /// <summary>
        /// 获取文档的模板分类的描述
        /// </summary>
        /// <returns></returns>
        public string GetAttrTempDesc()
        {
            string docType = "";
            try
            {
                AttrData data;
                if ((data = doc.GetAttrDataByKeyWord("CA_ATTRTEMP")) != null)
                {
                    if (data.ToString == "LETTERFILE")
                    {
                        docType = "信函";
                    }
                    else if (data.ToString == "FILETRANSMIT")
                    {
                        docType = "文件传递单";
                    }
                    else if (data.ToString == "MEETINGSUMMARY")
                    {
                        docType = "会议纪要";
                    }
                }
            }
            catch { }
            return docType;
        }

        /// <summary>
        /// 获取文档的模板分类的描述
        /// </summary>
        /// <returns></returns>
        public string GetAttrTempCode()
        {
            string docType = "";
            try
            {
                AttrData data;
                if ((data = doc.GetAttrDataByKeyWord("CA_ATTRTEMP")) != null)
                {
                    if (data.ToString == "LETTERFILE")
                    {
                        docType = "LET";
                    }
                    else if (data.ToString == "FILETRANSMIT")
                    {
                        docType = "TRA";
                    }
                    else if (data.ToString == "MEETINGSUMMARY")
                    {
                        docType = "MOM";
                    }
                }
            }
            catch { }
            return docType;
        }

        public void SaveAttrData()
        {
            try
            {
                if (doc != null)
                    doc.AttrDataList.SaveData();
            }
            catch { }
        }

        public void Modify()
        {
            try
            {
                if (doc != null)
                    doc.Modify();
            }
            catch { }
        }

        public string FullPathFile {
            get {
                if (doc != null)
                    return doc.FullPathFile;
                else
                    return "";
            }
        }

        public DBSource dbSource
        {
            get
            {
                if (doc != null)
                    return doc.dBSource;
                else
                    return null;
            }
        }

        public int? O_size {
            get
            {
                if (doc != null)
                    return doc.O_size;
                else
                    return 0;
            }
            set
            {
                doc.O_size = value;
            }
        }

        //获取著录属性
        public JObject GetCataloguAttrJson() {
            JObject result = new JObject();
            try
            {
                result = new JObject(
                    new JProperty("CA_REFERENCE", CA_REFERENCE),
                    new JProperty("CA_VOLUMENUMBER", CA_VOLUMENUMBER),
                    new JProperty("CA_FILECODE", CA_FILECODE),
                    new JProperty("CA_ORIFILECODE", CA_ORIFILECODE),
                    new JProperty("CA_RESPONSIBILITY", CA_RESPONSIBILITY),
                    new JProperty("CA_FILETITLE", CA_FILETITLE),
                    new JProperty("CA_PAGE", CA_PAGE),
                    new JProperty("CA_NUMBER", CA_NUMBER),
                     new JProperty("CA_MEDIUM", CA_MEDIUM),
                     new JProperty("CA_LANGUAGES", CA_LANGUAGES),
                     new JProperty("CA_PRONAME", CA_PRONAME),
                     new JProperty("CA_PROCODE", CA_PROCODE),
                     new JProperty("CA_MAJOR", CA_MAJOR),
                     new JProperty("CA_CREW", CA_CREW),
                     new JProperty("CA_FACTORY", CA_FACTORY),
                     new JProperty("CA_FACTORYNAME", CA_FACTORYNAME),
                     new JProperty("CA_SYSTEM", CA_SYSTEM),
                     new JProperty("CA_SYSTEMNAME", CA_SYSTEMNAME),
                     new JProperty("CA_RELATIONFILECODE", CA_RELATIONFILECODE),
                      new JProperty("CA_RELATIONFILENAME", CA_RELATIONFILENAME),
                       new JProperty("CA_FILESPEC", CA_FILESPEC),
                        new JProperty("CA_FILEUNIT", CA_FILEUNIT),
                         new JProperty("CA_SECRETGRADE", CA_SECRETGRADE),
                          new JProperty("CA_KEEPINGTIME", CA_KEEPINGTIME),
                    new JProperty("CA_FILELISTCODE", CA_FILELISTCODE),
                    new JProperty("CA_FILELISTTIME", CA_FILELISTTIME),
                    new JProperty("CA_RACKNUMBER", CA_RACKNUMBER),
                    new JProperty("CA_NOTE", CA_NOTE),
                    new JProperty("CA_WORKTYPE", CA_WORKTYPE),
                     new JProperty("CA_WORKSUBTIEM", CA_WORKSUBTIEM),
                      new JProperty("CA_EDITION", CA_EDITION),
                       new JProperty("CA_FILETYPE", CA_FILETYPE),
                       new JProperty("CA_UNIT", CA_UNIT), 
                       new JProperty("CA_FLOWNUMBER", CA_FLOWNUMBER)
                     );
            }
            catch
            {

            }

            return result;
        }


    }
}

