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
//using System.Data.SQLite;
//using LinqToDB;


//using System.Web.Script.Serialization;

namespace AVEVA.CDMS.HXPC_Plugins
{
    public class EnterPoint
    {
        //记录本插件的唯一标记
        public static string PluginName = "HXPC";

        public static void Init()
        {
            //WebApi.WebExploreEvent


            //添加流程按钮事件处理

            //记录是否已加载
            bool isLoad = false;

            foreach (WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event_Class EventClass in WebWorkFlowEvent.ListBeforeWFSelectUsers)
            {
                if (EventClass.PluginName == PluginName)
                {
                    isLoad = true;
                    break;
                }
            }

            if (isLoad==false)
            {
                //拖拽文件后处理
                //WebExploreEvent.OnAfterCreateNewObject += new WebExploreEvent.Explorer_AfterCreateNewObject(Document.OnAfterCreateNewObject);

                //拖拽文件后处理
                WebExploreEvent.Explorer_AfterCreateNewObject_Event AfterCreateNewObject =new WebExploreEvent.Explorer_AfterCreateNewObject_Event(Document.OnAfterCreateNewObject);
                WebExploreEvent.Explorer_AfterCreateNewObject_Event_Class Explorer_AfterCreateNewObject_Event_Class = new WebExploreEvent.Explorer_AfterCreateNewObject_Event_Class();
                Explorer_AfterCreateNewObject_Event_Class.Event = AfterCreateNewObject;
                Explorer_AfterCreateNewObject_Event_Class.PluginName = PluginName;
                WebExploreEvent.ListAfterCreateNewObject.Add(Explorer_AfterCreateNewObject_Event_Class);


                //添加流程按钮事件处理
                WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event BeforeWFSelectUsers =new WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event(AVEVA.CDMS.HXPC_Plugins.EnterPoint.BeforeWF);
                WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event_Class Before_WorkFlow_SelectUsers_Event_Class  = new WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event_Class();
                Before_WorkFlow_SelectUsers_Event_Class.Event = BeforeWFSelectUsers;
                Before_WorkFlow_SelectUsers_Event_Class.PluginName = PluginName;
                WebWorkFlowEvent.ListBeforeWFSelectUsers.Add(Before_WorkFlow_SelectUsers_Event_Class);
            }
        }

        public static List<ExWebMenu> CreateNewExMenu()
        {
            try
            {
                List<ExWebMenu> menuList = new List<ExWebMenu>();

                //if (WebWorkFlowEvent.BeforeWFSelectUsers2 == null)
                //{
                //    WebWorkFlowEvent.BeforeWFSelectUsers2 = (WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event2)Delegate.Combine(WebWorkFlowEvent.BeforeWFSelectUsers2, new WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event2(AVEVA.CDMS.HXPC_Plugins.EnterPoint.BeforeWF));
                //}
                //else
                //{
                //    WebWorkFlowEvent.BeforeWFSelectUsers2 = new WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event2(AVEVA.CDMS.HXPC_Plugins.EnterPoint.BeforeWF);
                //}

                //SQUMenu SQU = new SQUMenu();
                //SQU.MenuName = "生成SQU工程联系单";
                //SQU.MenuPosition = enWebMenuPosition.TVProject;
                //SQU.MenuType = enWebMenuType.Single;

                //menuList.Add(SQU);

                BIMViewMenu BIMView = new BIMViewMenu();
                BIMView.MenuId = "HX_BIMView";
                BIMView.MenuName = "查看BIM模型";
                BIMView.MenuPosition = enWebMenuPosition.LVDoc;
                BIMView.MenuType = enWebMenuType.Single;

                menuList.Add(BIMView);

                SelectProfessionMenu selectProfessionMenu = new SelectProfessionMenu();
                selectProfessionMenu.MenuId = "HX_SelectProfession";
                selectProfessionMenu.MenuName = "创建设计阶段与选择专业...";
                selectProfessionMenu.MenuPosition = enWebMenuPosition.TVProject;
                selectProfessionMenu.MenuType = enWebMenuType.Single;

                menuList.Add(selectProfessionMenu);

                ExcangeDocMenu excangeDocMenu = new ExcangeDocMenu();
                excangeDocMenu.MenuId = "HX_ExcangeDoc";
                excangeDocMenu.MenuName = "生成提资单...";
                excangeDocMenu.MenuPosition = enWebMenuPosition.TVProject;
                excangeDocMenu.MenuType = enWebMenuType.Single;

                menuList.Add(excangeDocMenu);

                ExcangeDocMenu exchangeDocUpEditionMenu = new ExcangeDocMenu
                {
                    MenuId = "HX_ExchangeDocUpEdition",
                    MenuName = "提资升版...",
                    MenuType = enWebMenuType.Single,
                    MenuPosition = enWebMenuPosition.TVProject,
                };
                menuList.Add(exchangeDocUpEditionMenu);

                ExcangeDocMenu exchangeDocContinueMenu = new ExcangeDocMenu
                {
                    MenuId = "HX_ExchangeDocContinue",
                    MenuName = "继续提资...",
                    MenuType = enWebMenuType.Single,
                    MenuPosition = enWebMenuPosition.TVProject,
                };
                menuList.Add(exchangeDocContinueMenu);

                CompanyMenu companyMenu = new CompanyMenu
                {
                    MenuId = "HX_CreateCompany",
                    MenuName = "新建厂家资料...",
                    MenuType = enWebMenuType.Single,
                    MenuPosition = enWebMenuPosition.TVProject

                };
                menuList.Add(companyMenu);

                SendDocumentMenu sendDocumentMenu = new SendDocumentMenu
                {
                    MenuId = "HX_SendDocument",
                    MenuName = "创建发文...",
                    MenuType = enWebMenuType.Single,
                    MenuPosition = enWebMenuPosition.TVProject

                };
                menuList.Add(sendDocumentMenu);

                CreateProdectMenu createProdectMenu = new CreateProdectMenu
                {
                    MenuId = "HX_CreateProdect",
                    MenuName = "创建成品...",
                    MenuType = enWebMenuType.Single,
                    MenuPosition = enWebMenuPosition.TVProject

                };
                menuList.Add(createProdectMenu);

                WorkTaskMenu workTaskMenu = new WorkTaskMenu
                {
                    MenuId = "HX_CreateWorkTask",
                    MenuName = "创建任务...",
                    MenuType = enWebMenuType.Single,
                    MenuPosition = enWebMenuPosition.TVProject

                };
                menuList.Add(workTaskMenu);


                TaskReportMenu taskReportMenu = new TaskReportMenu
                {
                    MenuId = "HX_TaskReport",
                    MenuType = enWebMenuType.Single,
                    MenuName = "工日填报...",
                    //MenuToolBarBitmap = CDMSPlugin.Properties.Resources.TaskReport,
                    //MenuToolTipText = "工日填报..."

                };
                menuList.Add(taskReportMenu);

                ProjectInfoMenu projectInfoMenu = new ProjectInfoMenu
                {
                    MenuId = "HX_ProjectInfo",
                    MenuName = "批量立项...",
                    MenuType = enWebMenuType.Single,
                    MenuPosition = enWebMenuPosition.TVProject,
                    TVMenuPositon = enWebTVMenuPosition.TVDocument
                };
                menuList.Add(projectInfoMenu);

                ProjectExportMenu projectExportMenu = new ProjectExportMenu
                {
                    MenuId = "HX_ProjectExport",
                    MenuName = "导出项目...",
                    MenuType = enWebMenuType.Single,
                    MenuPosition = enWebMenuPosition.TVProject,
                    TVMenuPositon = enWebTVMenuPosition.TVDocument
                };
                menuList.Add(projectExportMenu);

                return menuList;
            }
            catch { }
            return null;
        }

        public static ExReJObject BeforeWF(string PlugName, WorkFlow wf, WorkStateBranch wsb)
        {
            ExReJObject reJo = new ExReJObject();
            reJo.success = true;
            if (PlugName != EnterPoint.PluginName)
            {
                //当reJo的成功状态返回为真时，继续流转到下一流程状态的操作
                reJo.success = true;
                return reJo;
            }

            try
            {
                #region 火电项目立项流程
                if (wf.DefWorkFlow.O_Code == "CREATEPROJECT")
                {
                    if (wf.CuWorkState.Code == "DESIGNDIRECTOR")
                    {

                        Project project = wf.Project;
                        if (project != null)
                        {
                            AttrData ad = project.GetAttrDataByKeyWord("PROJECTOWNER");
                            if (ad != null)
                            {
                                ad.SetCodeDesc(wf.dBSource.LoginUser.ToString);
                                project.AttrDataList.SaveData();
                            }
                        }

                        if (!string.IsNullOrEmpty(wf.O_suser3) && wf.O_suser3 == "pass")
                        {
                            //当reJo的成功状态返回为真时，继续流转到下一流程状态的操作
                            reJo.success = true;
                            return reJo;
                        }

                        //提示需要选择下一状态的校审人员
                        reJo.data = new JArray(new JObject(
                                new JProperty("state", "RunFunc"),
                                new JProperty("plugins", "HXPC_Plugins"),
                                new JProperty("DefWorkFlow", wf.DefWorkFlow.O_Code),
                                new JProperty("CuWorkState", wf.CuWorkState.Code),
                                new JProperty("FuncName", "selectProfession"),
                                new JProperty("projectKeyword", wf.Project.KeyWord),
                                new JProperty("projectDesc", wf.Project.ToString)
                                ));
                        //当reJo的成功状态返回为假时，中断流转到下一流程状态的操作
                        reJo.success = false;
                        return reJo;

                    }
                }
                #endregion

                #region 项目选择参与专业流程
                //项目选择参与专业流程
                if (wf.DefWorkFlow.O_Code == "SELECTPROFESSION")
                {
                    //当前流程状态为主设人
                    if (wf.CuWorkState.Code == "DIRECTORMAN")
                    {
                        //判断主设人角色模板不为空
                        AttrData ad = wf.Project.GetAttrDataByKeyWord("PROFESSIONOWNER");
                        if (ad != null)
                        {
                            //主设人确定时添加当前登录用户为主设人角色
                            ad.SetCodeDesc(wf.dBSource.LoginUser.ToString);
                            wf.Project.AttrDataList.SaveData();

                            //当reJo的成功状态返回为真时，继续流转到下一流程状态的操作
                            reJo.success = true;
                            return reJo;
                        }
                        //主设人模板为空时，显示错误
                        else
                        {
                            //MessageBox.Show("添加主设人角色失败，请联系管理员！");
                            //当reJo的成功状态返回为假时，中断流转到下一流程状态的操作
                            reJo.msg= "添加主设人角色失败，请联系管理员！";
                            reJo.success = false;
                            return reJo;
                        }
                    }
                }
                #endregion

                #region 发文流程
                //发文流程
                if (wf.DefWorkFlow.O_Code == "SENDDOC")
                {
                    //发文流程，文秘发出后，将发出的公文放置在共有发文目录下
                    if (wf.CuWorkState.Code == "SECRETARILMAN" && wsb.defStateBrach.O_Code == "TOEND" && wf.dBSource.LoginUser.O_userno == wf.CuWorkState.CuWorkUser.O_userno)
                    {

                        //查找设计阶段
                        Project p = CommonFunction.GetDesign(wf.doc.Project);
                        if (p != null)
                        {
                            //查找发文
                            foreach (Project pp in p.ChildProjectList)
                            {
                                if (pp.Code == "发文")
                                {
                                    //创建厂家和文档
                                    Project cp = pp.NewProject(wf.doc.Project.O_projectname, wf.doc.Project.O_projectdesc);

                                    //创建发文的快捷方式
                                    if (cp != null)
                                    {
                                        foreach (Doc doc in wf.DocList)
                                        {
                                            cp.NewDoc(doc);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                #region 收文流程
                if (wf.DefWorkFlow.O_Code == "RECEIVEDOC")
                {
                    //1.设总选择专业：收文分发流程
                    if (wf.CuWorkState.Code == "PROJECTOWNER" && wf.dBSource.LoginUser.O_userno == wf.CuWorkState.CuWorkUser.O_userno)
                    {
                        //new fmDocSelectProfession(wf.doc) { StartPosition = FormStartPosition.CenterScreen }.ShowDialog();

                        //提示弹出选择需要分发的专业
                        reJo.data = new JArray(new JObject(
                                new JProperty("state", "RunFunc"),
                                new JProperty("plugins", "HXPC_Plugins"),
                                new JProperty("DefWorkFlow", wf.DefWorkFlow.O_Code),
                                new JProperty("CuWorkState", wf.CuWorkState.Code),
                                new JProperty("FuncName", "receiveDocSelectProfession"),
                                new JProperty("WfKeyword", wf.KeyWord)
                                ));
                        //当reJo的成功状态返回为假时，中断流转到下一流程状态的操作
                        reJo.success = false;
                        return reJo;
                    }


                    //收文流程，专业接收后，将接收后的公文放置在专业收文目录下
                    if (wf.CuWorkState.Code == "PROFESSION" && wsb.defStateBrach.O_Code == "TOEND" && wf.dBSource.LoginUser.O_userno == wf.CuWorkState.CuWorkUser.O_userno)
                    {
                        //查找设计阶段
                        Project p = CommonFunction.GetDesign(wf.doc.Project);


                        //查找专业
                        Project profession = wf.CuWorkState.O_iuser4 != null ? p.dBSource.GetProjectByID((int)wf.CuWorkState.O_iuser4) : null;
                        if (profession == null)
                        {
                            foreach (Project pp in p.ChildProjectList)
                            {
                                if (pp.TempDefn != null && pp.TempDefn.KeyWord == "PROFESSION")
                                {
                                    AttrData ad = pp.GetAttrDataByKeyWord("PROFESSIONOWNER");
                                    if (ad != null && ad.group != null && ad.group.UserList.Count > 0 && ad.group.UserList[0].O_userno == wsb.dBSource.LoginUser.O_userno)
                                    {
                                        profession = pp;
                                        break;
                                    }
                                }
                                if (profession != null)
                                    break;
                            }
                        }


                        //找到专业后，在专业下创建收文快捷方式
                        if (profession != null)
                        {
                            Project docProject = profession.NewProject("收发文", "");
                            if (docProject != null)
                            {
                                Project fp = docProject.NewProject("收文", "");

                                //创建厂家
                                if (fp != null)
                                {
                                    Project cp = fp.NewProject(wf.doc.Project.O_projectname, wf.doc.Project.O_projectdesc);

                                    //创建文件快捷方式
                                    if (cp != null)
                                    {
                                        foreach (Doc dd in wf.DocList)
                                        {
                                            cp.NewDoc(dd);
                                        }
                                    }
                                }
                            }
                        }

                        //当reJo的成功状态返回为真时，继续流转到下一流程状态的操作
                        reJo.success = true;
                        return reJo;
                    }
                }
                #endregion

                #region 主任选择校核人和审核人时弹出选择窗口
                //主任选择校核人和审核人时弹出选择窗口
                string workflowCode = wf.DefWorkFlow.O_Code;
                if (wsb.defStateBrach.O_Code == "DISPATCH2" && wf.CuWorkState.Code == "DIRECTORSELECT" && (workflowCode == "EXCHANGEDOC" || workflowCode == "SENDDOC" || workflowCode == "WORKTASK"))
                {

                    //判断是否已经选择了人
                    WorkState stateCHECK = wf.WorkStateList.Find(wsx => (wsx.Code == "CHECK"));
                    WorkState stateAUDIT = wf.WorkStateList.Find(wsx => (wsx.Code == "AUDIT"));
                    if ((stateCHECK.group != null && stateCHECK.group.AllUserList.Count != 0) && (stateAUDIT.group != null && stateAUDIT.group.AllUserList.Count != 0))
                    {
                        //reJo.msg = "用户组信息错误！";
                        //当reJo的成功状态返回为真时，继续流转到下一流程状态的操作
                        //当在主任选人表单选择了校审核人员的时候，把流程流转到校核状态
                        reJo.success = true;
                        return reJo;
                    }

                    ////内部提资：主任选择校审人员
                    //fmSelectUser fmSelectUser = new fmSelectUser(wf)
                    //{
                    //    StartPosition = FormStartPosition.CenterScreen
                    //};
                    //if (fmSelectUser.ShowDialog() == DialogResult.Cancel)
                    //{
                    //    throw new Exception("TERMINATEWORKFLOW");
                    //}

                    //向客户端发送提示消息：内部提资：主任选择校审人员
                    reJo.data = new JArray(new JObject(
                            new JProperty("state", "RunFunc"),
                            new JProperty("plugins", "HXPC_Plugins"),
                            new JProperty("DefWorkFlow", wf.DefWorkFlow.O_Code),
                            new JProperty("CuWorkState", wf.CuWorkState.Code),
                            new JProperty("FuncName", "selectUserEx"),
                            new JProperty("WfKeyword", wf.KeyWord)
                            ));
                    //当reJo的成功状态返回为假时，中断流转到下一流程状态的操作
                    reJo.success = false;
                    return reJo;
                }
                #endregion

                #region 主任选择校核人和审核人和批准人时弹出选择窗口
                //主任选择校核人和审核人时弹出选择窗口
                if (wsb.defStateBrach.O_Code == "DISPATCH2" && wf.CuWorkState.Code == "DIRECTORSELECT" && (workflowCode == "PRODUCT" || workflowCode == "PUBLICPRODUCT"))
                {

                    //判断是否已经选择了人
                    WorkState stateCHECK = wf.WorkStateList.Find(wsx => (wsx.Code == "CHECK"));
                    WorkState stateAUDIT = wf.WorkStateList.Find(wsx => (wsx.Code == "AUDIT"));
                    WorkState stateAPPROV = wf.WorkStateList.Find(wsx => (wsx.Code == "APPROV"));
                    if ((stateCHECK.group != null && stateCHECK.group.AllUserList.Count != 0) && (stateAUDIT.group != null && stateAUDIT.group.AllUserList.Count != 0) && (stateAPPROV.group != null && stateAPPROV.group.AllUserList.Count != 0))
                    {
                        //当在主任选人表单选择了校审核人员的时候，把流程流转到校核状态
                        reJo.success = true;
                        return reJo;
                    }

                    ////内部提资：主任选择校审人员
                    //fmSelectUserApprov fmSelectUser = new fmSelectUserApprov(wf)
                    //{
                    //    StartPosition = FormStartPosition.CenterScreen
                    //};
                    //if (fmSelectUser.ShowDialog() == DialogResult.Cancel)
                    //{
                    //    throw new Exception("TERMINATEWORKFLOW");
                    //}

                    //向客户端发送提示消息：内部提资：主任选择校审人员
                    reJo.data = new JArray(new JObject(
                            new JProperty("state", "RunFunc"),
                            new JProperty("plugins", "HXPC_Plugins"),
                            new JProperty("DefWorkFlow", wf.DefWorkFlow.O_Code),
                            new JProperty("CuWorkState", wf.CuWorkState.Code),
                            new JProperty("FuncName", "selectUserApprov"),
                            new JProperty("WfKeyword", wf.KeyWord)
                            ));
                    //当reJo的成功状态返回为假时，中断流转到下一流程状态的操作
                    reJo.success = false;
                    return reJo;
                }
                #endregion

                #region 内部流程：专业接收人员创建接收目录和快捷方式
                if (wf.DefWorkFlow.O_Code == "EXCHANGEDOC")
                {
                    if ((wf.CuWorkState.Code == "RECEIVE") && (wsb.defStateBrach.O_Code == "TOEND"))
                    {
                        //查找流程下文档是否为空
                        Doc doc = wf.DocList.Find(d => (d.TempDefn != null) && (d.TempDefn.Code == "EXCHANGEDOC"));
                        if (doc != null)
                        {
                            //查找签收专业
                            AttrData attrDataByKeyWord = doc.GetAttrDataByKeyWord("ED_PROFESSION");
                            if ((attrDataByKeyWord != null) && !string.IsNullOrEmpty(attrDataByKeyWord.ToString))
                            {

                                //获取数据字典的专业
                                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                                List<DictData> dictDataList = doc.Project.dBSource.GetDictDataList("Profession");
                                foreach (DictData dd in dictDataList)
                                {
                                    dictionary.Add(dd.O_Code, dd.O_sValue1);
                                }


                                //查找出设计阶段模板
                                //查找设计阶段
                                Project project = CommonFunction.GetDesign(doc.Project);
                                if (project == null)
                                {
                                    //当reJo的成功状态返回为真时，继续流转到下一流程状态的操作
                                    reJo.success = true;
                                    return reJo;
                                }


                                //查找专业
                                string[] strArray = attrDataByKeyWord.ToString.Split(new char[] { ',' });
                                for (int i = 0; i < strArray.Length; i++)
                                {
                                    Project receiveProfession = project.GetProjectByName(strArray[i].Substring(0, strArray[i].IndexOf("__")));
                                    if (receiveProfession == null)
                                    {
                                        continue;
                                    }
                                    if (receiveProfession.GetValueByKeyWord("PROFESSIONOWNER") == wf.dBSource.LoginUser.Code)
                                    {

                                        //查找内部接口
                                        Project tproject = receiveProfession.ChildProjectList.Find(p => p.Code == "内部接口");
                                        if (tproject != null)
                                        {
                                            Project iproject = tproject.ChildProjectList.Find(p => p.Code == "接收资料");
                                            if (iproject != null)
                                            {
                                                try
                                                {
                                                    //创建接收资料目录快捷方式
                                                    iproject.NewProject(doc.Project);
                                                }
                                                catch { }
                                            }
                                        }
                                        else
                                        {
                                            //MessageBox.Show("在内部接口目录下找不到" + strArray[i] + "专业");
                                            ////当reJo的成功状态返回为真时，继续流转到下一流程状态的操作
                                            //reJo.success = true;
                                            //return reJo;
                                        }
                                    }
                                }
                            }
                        }

                    }

                }
                #endregion

                #region 专业成品校审流程
                //专业成品校审流程
                if (wf.DefWorkFlow.O_Code == "PRODUCT")
                {
                    //当前流程状态为开始
                    if (wf.CuWorkState.Code == "START")
                    {
                        ////流程开始的时候，选择会签专业
                        //fmSelSignProfession fmSelSignProfession = new fmSelSignProfession(wf)
                        //{
                        //    StartPosition = FormStartPosition.CenterScreen
                        //};
                        //if (fmSelSignProfession.ShowDialog() == DialogResult.Cancel)
                        //{
                        //    //throw new Exception("TERMINATEWORKFLOW");
                        //}

                        //如果是选择专业后再来启动流程，就流转到下一流程状态
                        if (!string.IsNullOrEmpty(wf.O_suser3) && wf.O_suser3 == "pass")
                        {
                            //当reJo的成功状态返回为真时，继续流转到下一流程状态的操作
                            reJo.success = true;
                            return reJo;
                        }

                        //提示需要选择下一状态的校审人员
                        reJo.data = new JArray(new JObject(
                            new JProperty("state", "RunFunc"),
                                new JProperty("plugins", "HXPC_Plugins"),
                                new JProperty("DefWorkFlow", wf.DefWorkFlow.O_Code),
                                new JProperty("CuWorkState", wf.CuWorkState.Code),
                                new JProperty("FuncName", "productSelectProfession"),
                                new JProperty("DocKeyword", wf.doc.KeyWord)
                                ));
                        //当reJo的成功状态返回为假时，中断流转到下一流程状态的操作
                        reJo.success = false;
                        return reJo;
                    }
                }
                #endregion
            }
            catch
            { //throw; 
            }
            //当reJo的成功状态返回为真时，继续流转到下一流程状态的操作
            reJo.success = true;
            return reJo;
        }


        /// <summary>
        /// 设置项目设总
        /// </summary>
        /// <param name="sid">连接密钥</param>
        /// <param name="prjoectKeyword">目录关键字</param>
        /// <param name="userlist">设总的用户列表</param>
        /// <returns></returns>
        public static JObject SetProjectOwner(string sid, string prjoectKeyword, string userlist)
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

                Project project = dbsource.GetProjectByKeyWord(prjoectKeyword);

                //Project project = this.m_wf.Project;
                if (project == null)
                {
                    reJo.msg = "参数错误！文件夹不存在！";
                    return reJo.Value;
                }

                if (string.IsNullOrEmpty(userlist))
                {
                    reJo.msg = "参数错误！请选择设总！";
                    return reJo.Value;
                }

                User checker = dbsource.GetUserByKeyWord(userlist);
                if (checker == null)
                {
                    reJo.msg = "参数错误！选择的用户不存在！";
                    return reJo.Value;
                }

                string codeDescStr = checker.ToString;

                WorkFlow wf = project.WorkFlow;

                if (wf == null)
                {
                    reJo.msg = "参数错误！立项流程不存在！";
                    return reJo.Value;
                }

                User userByCode = wf.dBSource.GetUserByCode(codeDescStr.Substring(0, 5));
                AVEVA.CDMS.Server.Group group = new AVEVA.CDMS.Server.Group();
                group.AddUser(userByCode);
                DefWorkState defWorkState = wf.DefWorkFlow.DefWorkStateList.Find(dwsx => dwsx.O_Code == "DESIGNDIRECTOR");
                wf.NewWorkState(defWorkState).SaveSelectUser(group);

                AttrData attrdata = project.GetAttrDataByKeyWord("DESIGNPROJECT_OWNER");
                attrdata.SetCodeDesc(codeDescStr);

                project.AttrDataList.SaveData();

                AVEVA.CDMS.WebApi.DBSourceController.RefreshDBSource(sid);

                reJo.Value = AVEVA.CDMS.WebApi.WorkFlowController.GotoNextWfState(sid, wf.KeyWord, "选择项目设总", "");
                //提示需要选择下一状态的校审人员
                //reJo.data = new JArray(new JObject(new JProperty("FuncName", "selectUser"),new JProperty("wfKeyword",wf.KeyWord)));
                //reJo.success = true;

            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(reJo.msg);
            }
            return reJo.Value;
        }

        /// <summary>
        /// 设置项目文秘和进度工程师
        /// </summary>
        /// <param name="sid">连接密钥</param>
        /// <param name="prjoectKeyword">目录关键字</param>
        /// <param name="checkerList">文秘的用户列表</param>
        /// <param name="auditorList">进度工程师的用户列表</param>
        /// <returns></returns>
        public static JObject SetDirector(string sid, string prjoectKeyword, string checkerList, string auditorList)
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

                Project project = dbsource.GetProjectByKeyWord(prjoectKeyword);

                //Project project = this.m_wf.Project;
                if (project == null)
                {
                    reJo.msg = "参数错误！文件夹不存在！";
                    return reJo.Value;
                }

                if (string.IsNullOrEmpty(checkerList))
                {
                    reJo.msg = "参数错误！请选择文秘！";
                    return reJo.Value;
                }

                if (string.IsNullOrEmpty(auditorList))
                {
                    reJo.msg = "参数错误！请选择进度工程师！";
                    return reJo.Value;
                }

                User checker = dbsource.GetUserByKeyWord(checkerList);
                if (checker == null)
                {
                    reJo.msg = "参数错误！文秘角色所选择的用户不存在！";
                    return reJo.Value;
                }

                User auditor = dbsource.GetUserByKeyWord(auditorList);
                if (auditor == null)
                {
                    reJo.msg = "参数错误！进度工程师角色所选择的用户不存在！";
                    return reJo.Value;
                }

                string checkerDescStr = checker.ToString;
                string auditorDescStr = auditor.ToString;

                WorkFlow wf = project.WorkFlow;

                //User userChecker = wf.dBSource.GetUserByCode(checkerDescStr.Substring(0, 5));

                if (wf == null)
                {
                    reJo.msg = "参数错误！立项流程不存在！";
                    return reJo.Value;
                }

                //设置项目文秘和进度工程师
                project.GetAttrDataByKeyWord("INTERFACESECRETARY").SetCodeDesc(checkerDescStr);
                project.GetAttrDataByKeyWord("INTERFACECONTROLMAN").SetCodeDesc(auditorDescStr);

                project.AttrDataList.SaveData();

                AVEVA.CDMS.WebApi.DBSourceController.RefreshDBSource(sid);

                //流程流转到下一流程状态
                reJo.Value = AVEVA.CDMS.WebApi.WorkFlowController.GotoNextWfState(sid, wf.KeyWord, "选择秘书、进度工程师", "");
            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(reJo.msg);
            }
            return reJo.Value;
        }



 
        ////获取BIM模型SQL数据库文件路径，例如："\\127.0.0.1\storage\CDMSxxxxxx\xxx.sqlite"
        //public static JObject GetBIMSQLFilePath(string sid, string docKeyword)
        //{
        //    ExReJObject reJo = new ExReJObject();
        //    try
        //    {
        //        User curUser = DBSourceController.GetCurrentUser(sid);
        //        if (curUser == null)
        //        {
        //            reJo.msg = "登录验证失败！请尝试重新登录！";
        //            return reJo.Value;
        //        }

        //        DBSource dbsource = curUser.dBSource;
        //        if (dbsource == null)
        //        {
        //            reJo.msg = "登录验证失败！请尝试重新登录！";
        //            return reJo.Value;
        //        }

        //        Doc doc = dbsource.GetDocByKeyWord(docKeyword);
        //        if (doc == null)
        //        {
        //            reJo.msg = "错误的提交数据,文档不存在。";
        //            return reJo.Value;
        //        }

        //        //获取DOC文档存储路径
        //        JObject joPath = WebApi.DocController.GetDocFilePath(sid, docKeyword);
        //        if ((bool)joPath["success"] == false)
        //        {
        //            reJo.msg = "错误的提交数据,文档不存在。";
        //            return reJo.Value;
        //        }

        //        //数据库文件名
        //        string sqlFilename = doc.O_filename.Replace(".thm", ".sqlite");

        //        //文件路径必须使用文件的绝对路径,否则数据库文件打不开,这里不能直接使用存储里面的路径
        //        var path = joPath["msg"].ToString().Replace(".thm", ".sqlite");

        //        if (!System.IO.File.Exists(path))
        //        {
        //            reJo.msg = "数据库文档不存在！";
        //            return reJo.Value;
        //        }

        //        string tmpFilePath = "";
        //        if (System.IO.File.Exists(path))
        //        {
        //            //创建一个用户临时目录
        //            if (!Directory.Exists(dbsource.LoginUser.WorkingPath + curUser.KeyWord))
        //                Directory.CreateDirectory(dbsource.LoginUser.WorkingPath + curUser.KeyWord);

        //            //把数据库文件复制到临时目录
        //            tmpFilePath = dbsource.LoginUser.WorkingPath + curUser.KeyWord + @"\" + sqlFilename;
        //            if (System.IO.File.Exists(tmpFilePath))
        //            {
        //                System.IO.File.Delete(tmpFilePath);
        //            }
        //            System.IO.File.Copy(path, tmpFilePath);

        //            reJo.msg = tmpFilePath;
        //            reJo.success = true;
        //            return reJo.Value;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        reJo.msg = e.Message;
        //        CommonController.WebWriteLog(reJo.msg);
        //    }
        //    return reJo.Value;
        //}

        ///// <summary>
        ///// 获取THM文件的网络路径，此路径可在浏览器打开，例如：/Storage/CDMSxxxxxx/xxxxx.thm
        ///// </summary>
        ///// <param name="sid"></param>
        ///// <param name="docKeyword"></param>
        ///// <returns></returns>
        //public static JObject GetBIMTHMFilePath(string sid, string docKeyword) {
        //    ExReJObject reJo = new ExReJObject();
        //    try
        //    {
        //        User curUser = DBSourceController.GetCurrentUser(sid);
        //        if (curUser == null)
        //        {
        //            reJo.msg = "登录验证失败！请尝试重新登录！";
        //            return reJo.Value;
        //        }

        //        DBSource dbsource = curUser.dBSource;
        //        if (dbsource == null)
        //        {
        //            reJo.msg = "登录验证失败！请尝试重新登录！";
        //            return reJo.Value;
        //        }

        //        Doc curDoc = dbsource.GetDocByKeyWord(docKeyword);
        //        if (curDoc == null)
        //        {
        //            reJo.msg = "错误的提交数据,文档不存在。";
        //            return reJo.Value;
        //        }
        //        string tmpFilePath="/" + curDoc.Storage.O_storname + "/" + curDoc.Project.O_projectcode + "/" + HttpClient.EncodeUrl(curDoc.O_filename);//添加BIM模型路径

        //        reJo.msg = tmpFilePath;
        //        reJo.success = true;
        //        return reJo.Value;
        //    }
        //    catch (Exception e)
        //    {
        //        reJo.msg = e.Message;
        //        CommonController.WebWriteLog(reJo.msg);
        //    }
        //    return reJo.Value;
        //    }
        //public static JObject GetBIMTreeListJson(string sid, string sqlFilePath, string node, string type)
        //{
        //    ExReJObject reJo = new ExReJObject();
        //    try
        //    {

        //        //获取变量
        //        string keyword = node ?? "Root";
        //        //string Type = type ?? "1";


        //        User curUser = DBSourceController.GetCurrentUser(sid);
        //        if (curUser == null)
        //        {
        //            reJo.msg = "登录验证失败！请尝试重新登录！";
        //            return reJo.Value;
        //        }

        //        DBSource dbsource = curUser.dBSource;
        //        if (dbsource == null)
        //        {
        //            reJo.msg = "登录验证失败！请尝试重新登录！";
        //            return reJo.Value;
        //        }

        //        //获取根目录levels的List 
        //        if (keyword == "Root")
        //        {
        //            JObject joData = GetBIMRootLevels(sqlFilePath);
        //            return joData;
        //            //reJo.data = new JArray(joData);
        //            //reJo.success = true;
        //        }

        //        //当选取的节点类型是一级目录（floor）时
        //        if (type == "floor")
        //        {
        //            JObject joData = GetBIMLevelCategorys(sqlFilePath,node);
        //            return joData;
        //            //reJo.data = new JArray(joData);
        //            //reJo.success = true;
        //        }

        //        //当选取的节点类型是二级目录（category）时
        //        if (type == "category")
        //        {
        //            JObject joData = GetBIMCategoryNodes(sqlFilePath, node);
        //            return joData;
        //            //reJo.data = new JArray(joData);
        //            //reJo.success = true;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        reJo.msg = e.Message;
        //        CommonController.WebWriteLog(reJo.msg);
        //    }
        //    return reJo.Value;
        //}

        ///// <summary>
        ///// 获取根目录levels的List （一级目录列表）
        ///// </summary>
        ///// <param name="sqlFilePath"></param>
        ///// <returns></returns>
        //internal static JObject GetBIMRootLevels( string sqlFilePath)//string sid,
        //{
        //    ExReJObject reJo = new ExReJObject();
        //    try
        //    {

        //        if (!System.IO.File.Exists(sqlFilePath))
        //        {
        //            reJo.msg = "数据库文档不存在！";
        //            return reJo.Value;
        //        }

        //        var conn = string.Format("Data Source={0};pooling=false", sqlFilePath);

        //        using (var sqliteDb = new PDSVWeb.DataModels.PDSVModelDB("SQLite", conn))
        //        {
        //            var NodeTrees = sqliteDb.NodeTrees;//Common.ReadProjectDataBIMNodeTree(conn);
        //            var Nodes = sqliteDb.Nodes;//Common.ReadProjectDataBIMNode(conn);
        //            var listObjs = new List<object>();
        //            //创建树结构
        //            var levels = NodeTrees.ToLookup(t => t.Level, t => t);
        //            var levlFathers = new List<object>();
        //            JArray jaLevlFather = new JArray();
        //            foreach (var tree in levels)
        //            {
        //                var categorys = new List<object>();
        //                JArray jaCategorys = new JArray();

        //                JObject joLevlFatherItem = new JObject
        //                            {
        //                                new JProperty("id",tree.Key),//节点关键字
        //                                new JProperty("text",tree.Key),//节点文本
        //                                new JProperty("leaf", false),//没有子节点
        //                                new JProperty("type", "floor")//,
        //                                //new JProperty("children", jaCategorys)
        //                                //new JProperty("iconCls",folderType)//设置图标
        //                            };
        //                jaLevlFather.Add(joLevlFatherItem);
        //            }

        //            reJo.data = jaLevlFather;
        //            reJo.success = true;
        //            return reJo.Value;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        reJo.msg = e.Message;
        //        CommonController.WebWriteLog(reJo.msg);
        //    }
        //    return reJo.Value;
        //}

        ///// <summary>
        ///// 获取一级目录level的Category节点列表 
        ///// </summary>
        ///// <param name="sqlFilePath"></param>
        ///// <param name="level"></param>
        ///// <returns></returns>
        //internal static JObject GetBIMLevelCategorys(string sqlFilePath,string levelId)//string sid,
        //{
        //    ExReJObject reJo = new ExReJObject();
        //    try
        //    {
        //        if (!System.IO.File.Exists(sqlFilePath))
        //        {
        //            reJo.msg = "数据库文档不存在！";
        //            return reJo.Value;
        //        }

        //        var conn = string.Format("Data Source={0};pooling=false", sqlFilePath);

        //        using (var sqliteDb = new PDSVWeb.DataModels.PDSVModelDB("SQLite", conn))
        //        {
        //            var NodeTrees = sqliteDb.NodeTrees;//Common.ReadProjectDataBIMNodeTree(conn);
        //            //var Nodes = sqliteDb.Nodes;//Common.ReadProjectDataBIMNode(conn);

        //            var categorys = NodeTrees.Where(t => t.Level == levelId);

        //            JArray jaCategorys = new JArray();

        //            foreach (var titem in categorys)
        //            {
        //                JObject joCategoryItem = new JObject
        //                            {
        //                                new JProperty("id",titem.NodeId),//节点关键字
        //                                new JProperty("ParentId",levelId),
        //                                new JProperty("text",titem.Category),//节点文本
        //                                new JProperty("leaf", false),//没有子节点
        //                                new JProperty("type", "category")
        //                                //new JProperty("children", jaNodes)
        //                                //new JProperty("iconCls",folderType)//设置图标
        //                            };
        //                jaCategorys.Add(joCategoryItem);
        //            }

        //            reJo.data = jaCategorys;
        //            reJo.success = true;
        //            return reJo.Value;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        reJo.msg = e.Message;
        //        CommonController.WebWriteLog(reJo.msg);
        //    }
        //    return reJo.Value;
        //}

        ///// <summary>
        ///// 获取二级目录Category的node节点列表 
        ///// </summary>
        ///// <param name="sqlFilePath"></param>
        ///// <param name="categoryId"></param>
        ///// <returns></returns>
        //internal static JObject GetBIMCategoryNodes(string sqlFilePath, string categoryId)//string sid,
        //{
        //    ExReJObject reJo = new ExReJObject();
        //    try
        //    {
        //        if (!System.IO.File.Exists(sqlFilePath))
        //        {
        //            reJo.msg = "数据库文档不存在！";
        //            return reJo.Value;
        //        }

        //        var conn = string.Format("Data Source={0};pooling=false", sqlFilePath);

        //        using (var sqliteDb = new PDSVWeb.DataModels.PDSVModelDB("SQLite", conn))
        //        {
        //            //var NodeTrees = sqliteDb.NodeTrees;//Common.ReadProjectDataBIMNodeTree(conn);
        //            var Nodes = sqliteDb.Nodes;//Common.ReadProjectDataBIMNode(conn);
                    
        //            var nodes = Nodes.Where(t => t.NodeId == Convert.ToInt32(categoryId));

        //            JArray jaNodes  = new JArray();

        //            foreach (var child in nodes)
        //            {
        //                JObject joNodeItem = new JObject
        //                            {
        //                                new JProperty("id",child.ElementId),//节点关键字
        //                                new JProperty("ParentId",categoryId),
        //                                new JProperty("text",child.Name),//节点文本
        //                                new JProperty("leaf", true),//没有子节点
        //                                new JProperty("type", "node")
        //                                //new JProperty("children", jaNodes)
        //                                //new JProperty("iconCls",folderType)//设置图标
        //                            };
        //                jaNodes.Add(joNodeItem);
        //            }

        //            reJo.data = jaNodes;
        //            reJo.success = true;
        //            return reJo.Value;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        reJo.msg = e.Message;
        //        CommonController.WebWriteLog(reJo.msg);
        //    }
        //    return reJo.Value;
        //}

        //public static JObject GetBIMNodeParams(string sid, string sqlFilePath, string nodeId)
        //{
        //    ExReJObject reJo = new ExReJObject();
        //    try
        //    {
        //        User curUser = DBSourceController.GetCurrentUser(sid);
        //        if (curUser == null)
        //        {
        //            reJo.msg = "登录验证失败！请尝试重新登录！";
        //            return reJo.Value;
        //        }

        //        DBSource dbsource = curUser.dBSource;
        //        if (dbsource == null)
        //        {
        //            reJo.msg = "登录验证失败！请尝试重新登录！";
        //            return reJo.Value;
        //        }

        //        if (!System.IO.File.Exists(sqlFilePath))
        //        {
        //            reJo.msg = "数据库文档不存在！";
        //            return reJo.Value;
        //        }

        //        var conn = string.Format("Data Source={0};pooling=false", sqlFilePath);

        //        using (var sqliteDb = new PDSVWeb.DataModels.PDSVModelDB("SQLite", conn))
        //        {
        //            int eleId = Convert.ToInt32(nodeId);
        //            var Params = sqliteDb.Parametes.Where(t => t.ElementId == eleId); //Common.ReadProjectDataBIMParametes(conn, eleId);
        //            //var objects = new List<object>();
        //            JArray jaParam = new JArray();
        //            foreach (var param in Params)
        //            {
        //                //objects.Add(new { name = param.Param, value = param.Value, group = param.ParamGroup });
        //                JObject joParam = new JObject(new JProperty("name", param.Param),new JProperty("value", param.Value),new JProperty("group", param.ParamGroup));
        //                jaParam.Add(joParam);
        //            }
        //            reJo.data = jaParam;
        //            reJo.success = true;
        //            return reJo.Value;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        reJo.msg = e.Message;
        //        CommonController.WebWriteLog(reJo.msg);
        //    }
        //    return reJo.Value;
        //}

        ///// <summary>
        ///// 获取BIM模型目录树
        ///// </summary>
        ///// <returns></returns>
        //public static JObject GetBIMTree(string sid, string sqlFilePath )// string SqlitePath, string BIMProjectId,string Server_MapPath)
        //{
        //    ExReJObject reJo = new ExReJObject();
        //    try
        //    {
        //        User curUser = DBSourceController.GetCurrentUser(sid);
        //        if (curUser == null)
        //        {
        //            reJo.msg = "登录验证失败！请尝试重新登录！";
        //            return reJo.Value;
        //        }

        //        DBSource dbsource = curUser.dBSource;
        //        if (dbsource == null)
        //        {
        //            reJo.msg = "登录验证失败！请尝试重新登录！";
        //            return reJo.Value;
        //        }



        //        if (!System.IO.File.Exists(sqlFilePath))
        //        {
        //            reJo.msg = "数据库文档不存在！";
        //            return reJo.Value;
        //        }

        //        var conn = string.Format("Data Source={0};pooling=false", sqlFilePath);

        //        using (var sqliteDb = new PDSVWeb.DataModels.PDSVModelDB("SQLite", conn))
        //        {
        //            var NodeTrees = sqliteDb.NodeTrees;//Common.ReadProjectDataBIMNodeTree(conn);
        //            var Nodes = sqliteDb.Nodes;//Common.ReadProjectDataBIMNode(conn);
        //            var listObjs = new List<object>();
        //            //创建树结构
        //            var levels = NodeTrees.ToLookup(t => t.Level, t => t);
        //            var levlFathers = new List<object>();
        //            JArray jaLevlFather = new JArray();
        //            foreach (var tree in levels)
        //            {
        //                var categorys = new List<object>();
        //                JArray jaCategorys = new JArray();
        //                foreach (var item in tree.ToLookup(t => t.Category, t => t))
        //                {
        //                    var nodes = new List<object>();

        //                    foreach (var titem in item)
        //                    {
        //                        var children = Nodes.Where(t => t.NodeId == titem.NodeId);
        //                        JArray jaNodes = new JArray();
        //                        foreach (var child in children)
        //                        {
        //                          //  nodes.Add(new { id = child.ElementId, type = "node", ParentId = titem.NodeId, leaf = true, text = child.Name });
        //                            //组装js 字符串，把Jobject添加到返回数据Jarray
        //                            JObject joNodeItem = new JObject
        //                            {
        //                                new JProperty("id",child.ElementId),//节点关键字
        //                                new JProperty("text",child.Name),//节点文本
        //                                new JProperty("leaf", true),//没有子节点
        //                                new JProperty("type", "node")
        //                                //new JProperty("iconCls",folderType)//设置图标
        //                            };
        //                            jaNodes.Add(joNodeItem);
        //                        }
        //                        //categorys.Add(new { id = titem.NodeId, type = "category", text = item.Key, leaf=false, children = nodes });
        //                        JObject joCategoryItem = new JObject
        //                            {
        //                                new JProperty("id",titem.NodeId),//节点关键字
        //                                new JProperty("text",item.Key),//节点文本
        //                                new JProperty("leaf", false),//没有子节点
        //                                new JProperty("type", "category"),
        //                                new JProperty("children", jaNodes)
        //                                //new JProperty("iconCls",folderType)//设置图标
        //                            };
        //                        jaCategorys.Add(joCategoryItem);
        //                    }
        //                }
        //               // levlFathers.Add(new { id = tree.Key, text = tree.Key, type = "floor", state = "close", leaf = false, children = categorys });
        //                JObject joLevlFatherItem = new JObject
        //                            {
        //                                new JProperty("id",tree.Key),//节点关键字
        //                                new JProperty("text",tree.Key),//节点文本
        //                                new JProperty("leaf", false),//没有子节点
        //                                new JProperty("type", "floor"),
        //                                new JProperty("children", jaCategorys)
        //                                //new JProperty("iconCls",folderType)//设置图标
        //                            };
        //                jaLevlFather.Add(joLevlFatherItem);
        //            }
        //            //return levlFathers.Object2Json();
        //            //JavaScriptSerializer serializer = new JavaScriptSerializer();
        //            ////return serializer.Serialize(levlFathers);
        //            //string s=serializer.Serialize(levlFathers);
        //            //JArray joData = serializer.Deserialize(s);

        //            //string s=Object2Json(levlFathers);
        //            reJo.data = jaLevlFather;
        //            reJo.success = true;
        //            return reJo.Value;
        //        }
        //        //}

        //    }
        //    catch (Exception e)
        //    {
        //        reJo.msg = e.Message;
        //        CommonController.WebWriteLog(reJo.msg);
        //    }
        //    return reJo.Value;
        //}

        //public static string Object2Json(object obj)
        //{
        //    JavaScriptSerializer serializer = new JavaScriptSerializer();
        //    return serializer.Serialize(obj);
        //}

    }
}
