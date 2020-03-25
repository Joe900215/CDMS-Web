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
using Word = Microsoft.Office.Interop.Word;


//using System.Web.Script.Serialization;

namespace AVEVA.CDMS.HXEPC_Plugins
{
    public class EnterPoint
    {
        public static string PluginName = "HXEPC";

        //线程锁 
        internal static Mutex muxConsole = new Mutex();

        public static void Init()
        {
            //WebApi.WebExploreEvent

            //线程锁 
            muxConsole.WaitOne();
            try
            {
                //添加流程按钮事件处理
                //记录本插件的唯一标记

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

                if (isLoad == false)
                {
                    //拖拽文件后处理
                    //WebExploreEvent.OnAfterCreateNewObject += new WebExploreEvent.Explorer_AfterCreateNewObject(Document.OnAfterCreateNewObject);

                    //拖拽文件后处理
                    WebExploreEvent.Explorer_AfterCreateNewObject_Event AfterCreateNewObject = new WebExploreEvent.Explorer_AfterCreateNewObject_Event(Document.OnAfterCreateNewObject);
                    WebExploreEvent.Explorer_AfterCreateNewObject_Event_Class Explorer_AfterCreateNewObject_Event_Class = new WebExploreEvent.Explorer_AfterCreateNewObject_Event_Class();
                    Explorer_AfterCreateNewObject_Event_Class.Event = AfterCreateNewObject;
                    Explorer_AfterCreateNewObject_Event_Class.PluginName = PluginName;
                    WebExploreEvent.ListAfterCreateNewObject.Add(Explorer_AfterCreateNewObject_Event_Class);

                    //添加流程按钮事件处理
                    WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event BeforeWFSelectUsers = new WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event(AVEVA.CDMS.HXEPC_Plugins.EnterPoint.BeforeWF);
                    WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event_Class Before_WorkFlow_SelectUsers_Event_Class = new WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event_Class();
                    Before_WorkFlow_SelectUsers_Event_Class.Event = BeforeWFSelectUsers;
                    Before_WorkFlow_SelectUsers_Event_Class.PluginName = PluginName;
                    WebWorkFlowEvent.ListBeforeWFSelectUsers.Add(Before_WorkFlow_SelectUsers_Event_Class);

                    //添加流程文档创建者撤回流程事件处理
                    WebWorkFlowEvent.Before_Revoke_WorkFlow_Event BeforeRevokeWorkFlow = new WebWorkFlowEvent.Before_Revoke_WorkFlow_Event(AVEVA.CDMS.HXEPC_Plugins.EnterPoint.RevokeWorkFlow);
                    WebWorkFlowEvent.Before_Revoke_WorkFlow_Event_Class Before_Revoke_WorkFlow_Event_Class = new WebWorkFlowEvent.Before_Revoke_WorkFlow_Event_Class();
                    Before_Revoke_WorkFlow_Event_Class.Event = BeforeRevokeWorkFlow;
                    Before_Revoke_WorkFlow_Event_Class.PluginName = PluginName;
                    WebWorkFlowEvent.ListBeforeRevokeWorkFlow.Add(Before_Revoke_WorkFlow_Event_Class);

                    ////添加文档显示事件处理
                    WebDocEvent.Before_Get_Doc_List_Event BeforeGetDocs = new WebDocEvent.Before_Get_Doc_List_Event(BeforeGetDocList);
                    WebDocEvent.Before_Get_Doc_List_Event_Class Before_GetDocs_Event_Class = new WebDocEvent.Before_Get_Doc_List_Event_Class();
                    Before_GetDocs_Event_Class.Event = BeforeGetDocs;
                    Before_GetDocs_Event_Class.PluginName = PluginName;
                    WebDocEvent.ListBeforeGetDocs.Add(Before_GetDocs_Event_Class);

                    ////添加文件夹显示事件处理
                    WebProjectEvent.Before_Get_Project_List_Event BeforeGetProjects = new WebProjectEvent.Before_Get_Project_List_Event(BeforeGetProjectList);
                    WebProjectEvent.Before_Get_Project_List_Event_Class Before_GetProjects_Event_Class = new WebProjectEvent.Before_Get_Project_List_Event_Class();
                    Before_GetProjects_Event_Class.Event = BeforeGetProjects;
                    Before_GetProjects_Event_Class.PluginName = PluginName;
                    WebProjectEvent.ListBeforeGetProjects.Add(Before_GetProjects_Event_Class);

                    //添加获取文件夹图标事件处理
                    WebProjectEvent.After_Get_Project_Icon_Event AfterGetProjectIcon = new WebProjectEvent.After_Get_Project_Icon_Event(AfterGetProjectIconFun);
                    WebProjectEvent.After_Get_Project_Icon_Event_Class AfterGetProjectIcon_Event_Class = new WebProjectEvent.After_Get_Project_Icon_Event_Class();
                    AfterGetProjectIcon_Event_Class.Event = AfterGetProjectIconFun;
                    AfterGetProjectIcon_Event_Class.PluginName = PluginName;
                    WebProjectEvent.ListAfterGetProjectIconEvent.Add(AfterGetProjectIcon_Event_Class);

                    ////添加文档预览事件处理
                    WebDocEvent.Before_Preview_Doc_Event BeforePreviewDocEvent = new WebDocEvent.Before_Preview_Doc_Event(BeforePreviewDoc);
                    WebDocEvent.Before_Preview_Doc_Event_Class Before_Preview_Doc_Event_Class = new WebDocEvent.Before_Preview_Doc_Event_Class();
                    Before_Preview_Doc_Event_Class.Event = BeforePreviewDocEvent;
                    Before_Preview_Doc_Event_Class.PluginName = PluginName;
                    WebDocEvent.ListBeforePreviewDoc.Add(Before_Preview_Doc_Event_Class);

                    ////添加文档下载事件处理
                    WebDocEvent.Before_Download_File_Event BeforeDownloadFileEvent = new WebDocEvent.Before_Download_File_Event(BeforeDownloadFile);
                    WebDocEvent.Before_Download_File_Event_Class Before_Download_File_Event_Class = new WebDocEvent.Before_Download_File_Event_Class();
                    Before_Download_File_Event_Class.Event = BeforeDownloadFileEvent;
                    Before_Download_File_Event_Class.PluginName = PluginName;
                    WebDocEvent.ListBeforeDownloadFile.Add(Before_Download_File_Event_Class);

                    //添加选择用户事件处理
                    WebWorkFlowEvent.Before_Select_User_Event BeforeSelectUserEvent = new WebWorkFlowEvent.Before_Select_User_Event(BeforeSelectUser);
                    WebWorkFlowEvent.Before_Select_User_Event_Class Before_Select_User_Event_Class = new WebWorkFlowEvent.Before_Select_User_Event_Class();
                    Before_Select_User_Event_Class.Event = BeforeSelectUserEvent;
                    Before_Select_User_Event_Class.PluginName = PluginName;
                    WebWorkFlowEvent.ListBeforeSelectUser.Add(Before_Select_User_Event_Class);

                    //流程获取分支后事件处理
                    WebWorkFlowEvent.After_WorkFlow_GetBanch_Event AfterWorkFlowGetBanchEvent = new WebWorkFlowEvent.After_WorkFlow_GetBanch_Event(AfterWorkFlowGetBanch);
                    WebWorkFlowEvent.After_WorkFlow_GetBanch_Event_Class After_WorkFlow_GetBanch_Event_Class = new WebWorkFlowEvent.After_WorkFlow_GetBanch_Event_Class();
                    After_WorkFlow_GetBanch_Event_Class.Event = AfterWorkFlowGetBanchEvent;
                    After_WorkFlow_GetBanch_Event_Class.PluginName = PluginName;
                    WebWorkFlowEvent.ListAfterWorkFlowGetBanch.Add(After_WorkFlow_GetBanch_Event_Class);

                    //填写完Word属性项，保存Word前触发的事件
                    WebOfficeEvent.Before_Save_Word_Event BeforeSaveWordEvent = new WebOfficeEvent.Before_Save_Word_Event(BeforeSaveWord);
                    WebOfficeEvent.Before_Save_Word_Event_Class Before_Save_Word_Event_Class = new WebOfficeEvent.Before_Save_Word_Event_Class();
                    Before_Save_Word_Event_Class.Event = BeforeSaveWordEvent;
                    Before_Save_Word_Event_Class.PluginName = PluginName;
                    WebOfficeEvent.ListBeforeSaveWord.Add(Before_Save_Word_Event_Class);

                    ////添加用户获取系统子菜单前的事件
                    WebMenuEvent.Before_Get_Sys_Menu_Item_Event BeforeGetSysMenuItemEvent = new WebMenuEvent.Before_Get_Sys_Menu_Item_Event(BeforeGetSysMenuItem);
                    WebMenuEvent.Before_Get_Sys_Menu_Item_Event_Class Before_Get_Sys_Menu_Item_Event_Class = new WebMenuEvent.Before_Get_Sys_Menu_Item_Event_Class();
                    Before_Get_Sys_Menu_Item_Event_Class.Event = BeforeGetSysMenuItemEvent;
                    Before_Get_Sys_Menu_Item_Event_Class.PluginName = PluginName;
                    WebMenuEvent.ListGetSysMenuItem.Add(Before_Get_Sys_Menu_Item_Event_Class);

                }
            }
            catch { }
            finally
            {

                //解锁
                muxConsole.ReleaseMutex();
            }
        }

        public static List<ExWebMenu> CreateNewExMenu()
        {
            try
            {
                List<ExWebMenu> menuList = new List<ExWebMenu>();

                CreatePrjDocumentMenu createPrjDocumentMenu = new CreatePrjDocumentMenu
                {
                    MenuId = "HXEPC_CreatePrjDocument",
                    MenuName = "生成立项单...",
                    MenuType = enWebMenuType.Single,
                    MenuPosition = enWebMenuPosition.TVProject

                };
                menuList.Add(createPrjDocumentMenu);

                DraftDocumentMenu sendDocumentMenu = new DraftDocumentMenu
                {
                    MenuId = "HXEPC_SendDocument",
                    MenuName = "起草红头文...",
                    MenuType = enWebMenuType.Single,
                    MenuPosition = enWebMenuPosition.TVProject

                };
                menuList.Add(sendDocumentMenu);

                DraftMeetMinutesMenu draftMeetMinutesMenu = new DraftMeetMinutesMenu
                {
                    MenuId = "HXEPC_DraftMeetMinutes",
                    MenuName = "起草会议纪要...",
                    MenuType = enWebMenuType.Single,
                    MenuPosition = enWebMenuPosition.TVProject

                };
                menuList.Add(draftMeetMinutesMenu);


                DraftTransmittalCNMenu draftTransmittalCNMenu = new DraftTransmittalCNMenu
                {
                    MenuId = "HXEPC_DraftTransmittal",
                    MenuName = "起草文件传递单(中文)...",
                    MenuType = enWebMenuType.Single,
                    MenuPosition = enWebMenuPosition.TVProject

                };
                menuList.Add(draftTransmittalCNMenu);

                DraftLetterCNMenu draftLetterCNMenu = new DraftLetterCNMenu
                {
                    MenuId = "HXEPC_DraftLetterCN",
                    MenuName = "起草信函(中文)...",
                    MenuType = enWebMenuType.Single,
                    MenuPosition = enWebMenuPosition.TVProject

                };
                menuList.Add(draftLetterCNMenu);

                DraftRecognitionMenu draftRecognition = new DraftRecognitionMenu()
                {
                    MenuId = "HXEPC_DraftRecognition",
                    MenuName = "起草认质认价单...",
                    MenuType = enWebMenuType.Single,
                    MenuPosition = enWebMenuPosition.TVProject

                };

                menuList.Add(draftRecognition);

                DraftVisaMenu draftVisa = new DraftVisaMenu()
                {
                    MenuId = "HXEPC_DraftVisa",
                    MenuName = "起草签证...",
                    MenuType = enWebMenuType.Single,
                    MenuPosition = enWebMenuPosition.TVProject

                };

                menuList.Add(draftVisa);

                DraftReportMenu draftReport = new DraftReportMenu()
                {
                    MenuId = "HXEPC_DraftReport",
                    MenuName = "起草报告...",
                    MenuType = enWebMenuType.Single,
                    MenuPosition = enWebMenuPosition.TVProject

                };

                menuList.Add(draftReport);


                DraftNotifyMenu draftNotify = new DraftNotifyMenu()
                {
                    MenuId = "HXEPC_DraftNotify",
                    MenuName = "起草通知...",
                    MenuType = enWebMenuType.Single,
                    MenuPosition = enWebMenuPosition.TVProject

                };

                menuList.Add(draftNotify);


                DraftAskMenu draftAsk = new DraftAskMenu()
                {
                    MenuId = "HXEPC_DraftAsk",
                    MenuName = "起草请示...",
                    MenuType = enWebMenuType.Single,
                    MenuPosition = enWebMenuPosition.TVProject

                };

                menuList.Add(draftAsk);


                EditLetterTextMenu editLetterTextMenu = new EditLetterTextMenu
                {
                    MenuId = "HXEPC_EditLetterText",
                    MenuName = "编辑正文...",
                    MenuType = enWebMenuType.Single,
                    MenuPosition = enWebMenuPosition.LVDoc

                };
                menuList.Add(editLetterTextMenu);

                AddCompanyMenu companyMenu = new AddCompanyMenu
                {
                    MenuId = "HXEPC_AddCompany",
                    MenuName = "添加参建单位...",
                    MenuType = enWebMenuType.Single,
                    MenuPosition = enWebMenuPosition.TVProject

                };
                menuList.Add(companyMenu);

                EditCompanyMenu editCompanyMenu = new EditCompanyMenu
                {
                    MenuId = "HXEPC_EditCompany",
                    MenuName = "编辑参建单位...",
                    MenuType = enWebMenuType.Single,
                    MenuPosition = enWebMenuPosition.TVProject

                };
                menuList.Add(editCompanyMenu);

                EditDepartmentMenu editDepartmentMenu = new EditDepartmentMenu
                {
                    MenuId = "HXEPC_EditDepartment",
                    MenuName = "编辑项目部门...",
                    MenuType = enWebMenuType.Single,
                    MenuPosition = enWebMenuPosition.TVProject

                };
                menuList.Add(editDepartmentMenu);

                EditProjectGroupMenu editProjectGroupMenu = new EditProjectGroupMenu
                {
                    MenuId = "HXEPC_EditProjectGroup",
                    MenuName = "编辑项目组...",
                    MenuType = enWebMenuType.Single,
                    MenuPosition = enWebMenuPosition.TVProject

                };
                menuList.Add(editProjectGroupMenu);

                EditProjectInfoMenu editProjectInfoMenu = new EditProjectInfoMenu
                {
                    MenuId = "HXEPC_EditProjectInfo",
                    MenuName = "编辑项目资料...",
                    MenuType = enWebMenuType.Single,
                    MenuPosition = enWebMenuPosition.TVProject

                };
                menuList.Add(editProjectInfoMenu);


                ImportFileInfoMenu importFileInfoMenu = new ImportFileInfoMenu
               {
                   MenuId = "HXEPC_ImportFileInfo",
                   MenuName = "导入文件...",
                   MenuType = enWebMenuType.Single,
                   MenuPosition = enWebMenuPosition.TVProject

               };
                menuList.Add(importFileInfoMenu);

                EditFileAttrMenu editFileAttrMenu = new EditFileAttrMenu
                {
                    MenuId = "HXEPC_EditFileAttr",
                    MenuName = "修改文档...",
                    MenuType = enWebMenuType.Single,
                    MenuPosition = enWebMenuPosition.LVDoc

                };
                menuList.Add(editFileAttrMenu);

                UpgradeFileMenu upgradeFileMenu = new UpgradeFileMenu
                {
                    MenuId = "HXEPC_UpgradeFile",
                    MenuName = "文档升版...",
                    MenuType = enWebMenuType.Single,
                    MenuPosition = enWebMenuPosition.LVDoc

                };
                menuList.Add(upgradeFileMenu);

                //回复信函
                ReplyLetterMenu replyLetterMenu = new ReplyLetterMenu
                {
                    MenuId = "HXEPC_ReplyLetter",
                    MenuName = "回复信函...",
                    MenuType = enWebMenuType.Single,
                    MenuPosition = enWebMenuPosition.LVDoc

                };
                menuList.Add(replyLetterMenu);

                //回复信函
                GetSendCodeMenu getSendCodeMenu = new GetSendCodeMenu
                {
                    MenuId = "HXEPC_GetSendCode",
                    MenuName = "获取发文编号...",
                    MenuType = enWebMenuType.Single,
                    MenuPosition = enWebMenuPosition.LVDoc

                };
                menuList.Add(getSendCodeMenu);

                DraftSelfCheckMenu selfCheckMenu = new DraftSelfCheckMenu
                {
                    MenuId = "HXEPC_DraftSelfCheck",
                    MenuName = "自检",
                    MenuType = enWebMenuType.Single,
                    MenuPosition = enWebMenuPosition.LVDoc//文档栏

                };
                menuList.Add(selfCheckMenu);

                ExportFileMenu exportFile = new ExportFileMenu
                {
                    MenuId = "HXEPC_ExportFile",
                    MenuName = "导出文件...",
                    MenuType = enWebMenuType.Single,
                    MenuPosition = enWebMenuPosition.TVProject,//目录栏

                };
                menuList.Add(exportFile);

                ExportStatisProperMenu exportSFile = new ExportStatisProperMenu
                {
                    MenuId = "HXEPC_ExportStatisProper",
                    MenuName = "统计...",
                    MenuType = enWebMenuType.Single,
                    MenuPosition = enWebMenuPosition.TVProject,//树目录栏

                };

                menuList.Add(exportSFile);
                return menuList;
            }
            catch { }
            return null;
        }
        //显示文档列表前的事件，用于按条件筛选用户能看到的文档列表
        public static bool BeforeGetProjectList(string PlugName, ref List<Project> projectList)
        {
            if (PlugName != PluginName)
            {
                return false;
            }

            if (projectList.Count <= 0)
            {
                return false;
            }
            try
            {

                #region 函数逻辑
                //  1.获取当前登录用户

                // 2.获取当前项目的单位用户组和项目部用户组

                //  3.判断当前用户是否在单位用户组里面

                //   4.判读当前用户是否在项目部用户组里面

                //  5.1.如果当前用户在单位用户组里面,判断父目录是否是项目根目录

                // 5.2.如果父目录是项目根目录，去掉除通信类文件夹和存档管理文件夹目录外的所有文件夹 
                #endregion



                Project project = projectList[0];

                if (project == null) return false;



                Project rootProj = CommonFunction.getParentProjectByTempDefn(project, "HXNY_DOCUMENTSYSTEM");

                if (rootProj != null)
                {
                    DBSource dbsource = project.dBSource;

                    //1.获取当前登录用户
                    User curUser = project.dBSource.LoginUser;

                    // 2.获取当前项目的单位用户组和项目部用户组

                    Server.Group unitGroup = dbsource.GetGroupByName(rootProj.Code + "_ALLUnit");
                    Server.Group projGroup = dbsource.GetGroupByName(rootProj.Code + "_ProGroup");

                    //  3.判断当前用户是否在单位用户组里面
                    bool isUnitSec = false;
                    bool isProjUser = false;
                    if (unitGroup.UserList.Contains(curUser))
                    {
                        isUnitSec = true;
                    }
                    if (projGroup.UserList.Contains(curUser))
                    {
                        isProjUser = true;
                    }

                    //  5.1.如果当前用户在单位用户组里面,判断父目录是否是项目根目录
                    if (isUnitSec)
                    {
                        if (project.ParentProject.TempDefn.KeyWord == "HXNY_DOCUMENTSYSTEM")
                        {
                            // 5.2.如果父目录是项目根目录，去掉除通信类文件夹和存档管理文件夹目录外的所有文件夹 
                            List<Project> resultProjs = new List<Project>();
                            foreach (Project proj in projectList)
                            {
                                //if (proj.Description == "通信文件" || proj.Description == "通信管理" || proj.Description == "流程管理")
                                if (proj.TempDefn.KeyWord == "PRO_COMMUNICATION" || proj.TempDefn.KeyWord == "PRO_WORKFLOW")
                                {
                                    resultProjs.Add(proj);
                                }
                            }
                            projectList = resultProjs;
                        }
                    }
                }


            }
            catch { }
            return false;
        }

        //显示文档列表前的事件，用于按条件筛选用户能看到的文档列表
        public static bool BeforeGetDocList(string PlugName, Project project, ref List<Doc> docList, string filter)
        {
            if (PlugName != PluginName)
            {
                return false;
            }

            //搜索文档
            if (!string.IsNullOrEmpty(filter))
                return SearchDocList(project, ref docList, filter);



            if (docList.Count <= 0)
            {
                return false;
            }

            Project pproj = project.ParentProject;

            //if (pproj == null ||
            //    ((pproj.ParentProject == null || (pproj.ParentProject.Description != "通信管理" && pproj.ParentProject.Description != "通信文件"))
            //    && pproj.Description != "流程管理"))
            if (pproj == null ||
                ((pproj.ParentProject == null || (pproj.ParentProject.TempDefn.KeyWord != "PRO_COMMUNICATION" ))
                && pproj.TempDefn.KeyWord != "PRO_WORKFLOW"))
            {
                return false;
            }

            try
            {
                List<Doc> resultDocList = new List<Doc>();
                string projectTemp = "";
                if (project.TempDefn == null ||
                    ((projectTemp = project.TempDefn.KeyWord) != "COM_COMTYPE" &&
                       projectTemp != "RECOGNITION" &&
                       projectTemp != "NOTICE"))
                {
                    return true;
                }


                foreach (Doc d in project.DocList)
                {
                    try
                    {
                        string CuWsCode = "";

                        //排除文档实体发文目录，且流程状态是收文状态的文档
                        if ((d.ShortCutDoc == null &&
                                 (d.WorkFlow != null &&
                                 GetSourceDocReadRight(projectTemp, d.WorkFlow.CuWorkState.Code) &&
                                      d.WorkFlow.O_WorkFlowStatus != enWorkFlowStatus.Finish
                                      ))
                             || (d.ShortCutDoc != null &&
                               (d.ShortCutDoc.WorkFlow != null && d.ShortCutDoc.WorkFlow.O_WorkFlowStatus != enWorkFlowStatus.Finish
                                   ))
                                )
                        {
                            List<WorkUser> wus = d.ShortCutDoc == null ?
                                d.WorkFlow.CuWorkState.WorkUserList :
                                d.ShortCutDoc.WorkFlow.CuWorkState.WorkUserList;

                            foreach (WorkUser wu in wus)
                            {
                                if (wu.User != null && wu.User == project.dBSource.LoginUser)
                                {
                                    resultDocList.Add(d);
                                }
                            }
                        }

                    }
                    catch { }

                }


                docList = resultDocList;
            }
            catch (Exception ex)
            {
                WebApi.CommonController.WebWriteLog(DateTime.Now.ToString() + ":" + "获取文件列表错误," + ex.Message);

            }
            //docList = docList.Where(d=>(( d.WorkFlow==null && d.Creater==curUser) || d.WorkFlow != null)
            // || (d.ShortCutDoc != null && ((d.ShortCutDoc.WorkFlow == null && d.ShortCutDoc.Creater == curUser) || d.ShortCutDoc.WorkFlow != null))
            // //|| d.WorkFlow.O_WorkFlowStatus != enWorkFlowStatus.Finish  //流出已经走完的文档不显示
            // ).ToList<Doc>();

            return false;
        }

        /// <summary>
        /// 获取查看实体文件的权限
        /// </summary>
        /// <param name="ProjectTemp"></param>
        /// <param name="CuWsCode"></param>
        /// <returns></returns>
        private static bool GetSourceDocReadRight(string ProjectTemp, string CuWsCode)
        {
            try
            {
                //发文通信校审流程流程
                if ((ProjectTemp == "COM_COMTYPE" || ProjectTemp == "NOTICE") &&
                     (CuWsCode == "SECRETARIL" ||
                     CuWsCode == "DESIGN" || CuWsCode == "CHECK" || CuWsCode == "AUDIT" ||
                     CuWsCode == "APPROV" || CuWsCode == "AUDIT2" || CuWsCode == "SECRETARILMAN"))
                {

                    return true;
                }

                else if (ProjectTemp == "RECOGNITION" &&
                     (CuWsCode == "PROSECRETARIL" ||
                     CuWsCode == "PRODESIGN" || CuWsCode == "DIRCOSTCLERK" || CuWsCode == "PROAPPROV" ||
                     CuWsCode == "DIRCOSTCLERK2" || CuWsCode == "CONFIREMAN"))
                {

                    return true;
                }


            }
            catch { }
            return false;
        }



        /// <summary>
        /// 获取查看快捷方式文件的权限
        /// </summary>
        /// <param name="ProjectTemp"></param>
        /// <param name="CuWsCode"></param>
        /// <returns></returns>
        private static bool GetShortCutDocReadRight(string ProjectTemp, string CuWsCode)
        {
            try
            {
                //发文通信校审流程流程
                if (ProjectTemp == "COM_COMTYPE" &&
                     (CuWsCode == "RECUNIT" || CuWsCode == "DEPARTMENTCONTROL" ||
                     CuWsCode == "RESPONSIBLE" || CuWsCode == "MAINHANDLE" || CuWsCode == "RESPONSIBLE2"))
                {

                    return true;
                }

                else if (ProjectTemp == "RECOGNITION" &&
                     (CuWsCode == "DIRLEADER" ||
                     CuWsCode == "DIRECTOR" || CuWsCode == "FINE" || CuWsCode == "ZTBB"))
                {

                    return true;
                }


            }
            catch { }
            return false;
        }


        //显示文档列表前的事件，用于按条件筛选用户能看到的文档列表
        public static bool BeforeGetDocList2(Project project, ref List<Doc> docList, string filter)
        {
            //搜索文档
            //if (!string.IsNullOrEmpty(filter))
            //return SearchDocList(ref docList,   filter);

            //搜索文档附加属性

            #region 搜索文档附加属性
            if (!string.IsNullOrEmpty(filter))
            {
                List<Doc> newDocList = new List<Doc>();
                foreach (Doc docItem in docList)
                {
                    if (docItem.Code.IndexOf(filter) >= 0 || docItem.Description.IndexOf(filter) >= 0)
                    {
                        newDocList.Add(docItem);
                        continue;
                    }

                    if (docItem.ShortCutDoc != null)
                    {
                        AttrData mad = docItem.ShortCutDoc.AttrDataList.Find(ad => ad.ToString.IndexOf(filter) >= 0);
                        if (mad != null)
                        {
                            newDocList.Add(docItem);
                        }
                    }
                    else
                    {
                        AttrData mad = docItem.AttrDataList.Find(ad => ad.ToString.IndexOf(filter) >= 0);
                        if (mad != null)
                        {
                            newDocList.Add(docItem);
                        }
                    }
                }
                docList = newDocList;
                //return true;
            }
            #endregion

            if (docList.Count <= 0)
            {
                return false;
            }

            try
            {
                #region 判断是否是收发文目录，是否需要隐藏文件
                //判断是否是收发文目录，是否需要隐藏文件
                bool isRsProject = false;
                foreach (Doc doc in docList)
                {
                    try
                    {
                        string docTemp = "";
                        TempDefn doctempdefn = null;
                        if (doc.ShortCutDoc != null)
                        {
                            if (doc.ShortCutDoc.TempDefn == null)
                            {
                                continue;
                            }

                            doctempdefn = doc.ShortCutDoc.TempDefn;

                        }
                        else
                        {
                            doctempdefn = doc.TempDefn;
                        }
                        if (doctempdefn == null)
                        {
                            continue;
                        }
                        docTemp = doctempdefn.KeyWord;


                        if (docTemp == "CATALOGUING")
                        {
                            AttrData data;
                            //文文档模板
                            if ((data = doc.GetAttrDataByKeyWord("CA_ATTRTEMP")) != null)
                            {
                                string strData = data.ToString;
                                if (strData == "LETTERFILE" || strData == "FILETRANSMIT" ||
                                   strData == "MEETINGSUMMARY" || strData == "DOCUMENTFILE" ||
                                   strData == "RECOGNITIONFILE")
                                {
                                    isRsProject = true;
                                    break;
                                }
                            }
                        }
                    }
                    catch { }
                }

                Doc ddoc = docList.Find(d => d.Project != null);
                Project proj = ddoc.Project;

                if (proj == null)
                {
                    //防止在某些情况下文档找不到目录，把所有文档都显示出来
                    docList = new List<Doc>();
                    return false;
                }

                if (proj == null || proj.TempDefn == null) return false;

                Project commProj = CommonFunction.getParentProjectByTempDefn(proj, "PRO_COMMUNICATION");
                if (commProj == null)
                {
                    return false;
                }

                //if (ddoc.Project != null) {
                //    commProj = CommonFunction.getParentProjectByTempDefn(proj, "PRO_COMMUNICATION");
                //    if (commProj == null)
                //    {
                //        return false;
                //    }
                //}

                Project pproj = proj.ParentProject;
                DBSource dbsource = proj.dBSource;
                if (pproj == null) return false;

                if (!isRsProject || !(proj.TempDefn.KeyWord == "COM_COMTYPE" ||
                    pproj.Code == "收文" || pproj.Code == "发文" ||
                    pproj.Description == "收文" || pproj.Description == "发文"
                    ))
                {
                    return false;
                }
                #endregion

                User curUser = proj.dBSource.LoginUser;


                #region 筛选文档列表的步骤说明
                //1. 获取所有有流程（流程未完成），或者创建者是登录人的文档，缩小查询的范围

                //2 获取当前目录是收文目录还是发文目录

                //////////////////发文目录////////////////
                //2.1 如果是发文目录，就遍历文档

                //2.1.0 此步骤暂时忽略///////////////////////////////////////////////////////////////////////////////////////////////////////////////如果文档是快捷方式，就跳转到下一个文档（发文目录是实体文件 ，收文目录只是发文目录里的文档的快捷方式）

                //2.1.1获取文档的发文单位，并获取该发文单位的所有成员

                //2.2.2. 判断当前用户是否是发文单位的成员，不是发文单位的成员就跳到下一个文档

                //2.2.3 如果当前用户是当前文档的发文单位的成员，就把文档添加到文档列表

                //////////////////收文目录////////////////
                //2.2  如果是收文目录，就遍历文档

                //2.2.0 此步骤暂时忽略////////////////////////////////////////////////////////////////////////////////////////////////////////////// 如果文档不是快捷方式，就跳转到下一个文档

                //2.2.1 获取文档的收文单位和抄送单位

                //2.2.2 判断当前用户是否是收文单位或者抄送单位的成员，不是收文单位或者抄送单位的成员就跳到下一个文档

                //2.2.3 如果当前用户是当前文档的收文单位或者抄送单位的成员，就把文档添加到文档列表

                #endregion


                #region 1. 获取所有有流程（流程未完成），或者创建者是登录人的文档，缩小查询的范围
                //1. 获取所有有流程（流程未完成），或者创建者是登录人的文档，缩小查询的范围
                List<Doc> resultDocList = new List<Doc>();
                //if ( string.IsNullOrEmpty(filter) && 
//                if (pproj.ParentProject != null && (pproj.ParentProject.Description == "通信管理" || pproj.ParentProject.Description == "通信文件"))
                if (pproj.ParentProject != null && (pproj.ParentProject.TempDefn.KeyWord == "PRO_COMMUNICATION"))
                {
                    //如果是通信管理目录下的收发文文档列表，就要隐藏掉流程已经完成的文档
                    foreach (Doc d in docList)
                    {
                        try
                        {

                            //if (((d.WorkFlow == null && d.Creater == curUser) ||
                            //        (d.WorkFlow != null && d.WorkFlow.O_WorkFlowStatus != enWorkFlowStatus.Finish
                            //          && d.WorkFlow.CuWorkState.CuWorkUser.User == curUser))
                            // || (d.ShortCutDoc != null &&
                            //       (d.ShortCutDoc.WorkFlow != null && d.ShortCutDoc.WorkFlow.O_WorkFlowStatus != enWorkFlowStatus.Finish
                            //           && d.ShortCutDoc.WorkFlow.CuWorkState.CuWorkUser.User == curUser))
                            //    )
                            if ((d.ShortCutDoc == null &&
                                        ((d.WorkFlow == null && d.Creater == curUser) ||
                                        (d.WorkFlow != null && d.WorkFlow.O_WorkFlowStatus != enWorkFlowStatus.Finish
                                          )))
                                 || (d.ShortCutDoc != null &&
                                       (d.ShortCutDoc.WorkFlow != null && d.ShortCutDoc.WorkFlow.O_WorkFlowStatus != enWorkFlowStatus.Finish
                                           ))
                                    )
                            {
                                List<WorkUser> wus = d.ShortCutDoc == null ?
                                    d.WorkFlow.CuWorkState.WorkUserList :
                                    d.ShortCutDoc.WorkFlow.CuWorkState.WorkUserList;

                                foreach (WorkUser wu in wus)
                                {
                                    if (wu.User != null && wu.User == curUser)
                                    {
                                        resultDocList.Add(d);
                                    }
                                }
                            }

                        }
                        catch { }
                    }
                }
                else
                {
                    resultDocList = docList;
                }
                #endregion

                #region 2. 获取当前目录是收文目录还是发文目录
                // 2. 获取当前目录是收文目录还是发文目录
                Project rsProject = CommonFunction.getParentProjectByTempDefn(proj, "COM_SUBDOCUMENT");

                if (rsProject == null)
                {
                    WebApi.CommonController.WebWriteLog(DateTime.Now.ToString() + ":" + "没有查找到" + proj.KeyWord + "," + proj.Code + "的收发文目录。");
                    docList = new List<Doc>();
                    return false;
                }
                string projectType = "";
                if (rsProject.Code == "收文" || rsProject.Description == "收文")
                {
                    projectType = "收文";
                }
                else if (rsProject.Code == "发文" || rsProject.Description == "发文")
                {
                    projectType = "发文";
                }
                else
                {
                    WebApi.CommonController.WebWriteLog(DateTime.Now.ToString() + ":" + "没有查找到" + proj.KeyWord + "," + proj.Code + "的收发文目录。");
                    docList = new List<Doc>();
                    return false;
                }
                #endregion

                List<Doc> reDocList = new List<Doc>();

                #region 2.1 如果是发文目录，就遍历文档
                if (projectType == "发文")
                {
                    AttrData data;
                    foreach (Doc docItem in resultDocList)
                    {
                        try
                        {
                            if (reDocList.Contains(docItem)) continue;

                            Doc doc = docItem;
                            bool isShort = false;
                            if (doc.ShortCutDoc != null)
                            {
                                doc = doc.ShortCutDoc;
                                isShort = true;
                            }

                            if (doc.Creater == curUser && isShort == false)
                            {
                                //reDocList.Add(docItem);
                                //2.2.3 如果当前用户是当前文档的发文单位的成员，就把文档添加到文档列表
                                WorkFlow wf = CommonFunction.GetDocWorkFlow(docItem);
                                if (wf != null)
                                {
                                    foreach (Doc wfdocItem in wf.DocList)
                                    {
                                        reDocList.Add(wfdocItem);
                                    }
                                }
                                continue;
                            }

                            #region 判断流程用户里面有没有当前用户
                            //if (doc.WorkFlow != null && doc.WorkFlow.CuWorkState != null) {
                            //    bool isAdd = false;
                            //    foreach (WorkUser wu in doc.WorkFlow.CuWorkState.WorkUserList)
                            //    {
                            //        if (wu.User == curUser)
                            //        {
                            //            reDocList.Add(docItem);
                            //            isAdd = true;
                            //            break;
                            //        }
                            //    }
                            //    if (isAdd == true)
                            //    {
                            //        continue;
                            //    }
                            //}
                            //判断流程用户里面有没有当前用户
                            //if (doc.WorkFlow != null && doc.WorkFlowList != null)
                            //{
                            //    bool isAdd = false;
                            //    foreach (WorkState ws in doc.WorkFlow.WorkStateList)
                            //    {
                            //        foreach (WorkUser wu in ws.WorkUserList)
                            //        {
                            //            if (wu.User == curUser)
                            //            {
                            //                reDocList.Add(docItem);
                            //                isAdd = true;
                            //                break;
                            //            }
                            //        }
                            //        if (isAdd == true)
                            //        {
                            //            break;
                            //        }
                            //    }
                            //        if (isAdd == true)
                            //        {
                            //           continue;
                            //        }
                            //}

                            #endregion

                            string strAttrKeyword = "";

                            #region 如果文档没有模板，就找流程上其他有模板的文档
                            //如果文档没有模板，就找流程上其他有模板的文档
                            if (doc.TempDefn == null && doc.WorkFlow != null)
                            {
                                Doc twd = null;
                                foreach (Doc wd in doc.WorkFlow.DocList)
                                {
                                    if (wd.TempDefn != null)
                                    {
                                        twd = wd;
                                        break;
                                    }
                                }
                                if (twd != null)
                                {
                                    doc = twd;
                                }
                            }

                            #endregion

                            if (doc.TempDefn != null && doc.TempDefn.KeyWord == "CATALOGUING")
                            {
                                strAttrKeyword = "CA_SENDERCODE";
                            }

                            //2.1.1获取文档的发文单位，并获取该发文单位的所有成员
                            if ((data = doc.GetAttrDataByKeyWord(strAttrKeyword)) != null)
                            {
                                string senderCode = data.ToString;
                                AVEVA.CDMS.Server.Group group = dbsource.GetGroupByName(senderCode);
                                if (group == null)
                                {
                                    continue;
                                }

                                //2.2.2. 判断当前用户是否是发文单位的成员，不是发文单位的成员就跳到下一个文档
                                if (group.AllUserList.Contains(curUser))
                                {
                                    //foreach (Doc wfdocItem in docItem.WorkFlow.DocList)
                                    //{
                                    //    //2.2.3 如果当前用户是当前文档的发文单位的成员，就把文档添加到文档列表
                                    //    //reDocList.Add(docItem);
                                    //    reDocList.Add(wfdocItem);
                                    //}
                                    //2.2.3 如果当前用户是当前文档的发文单位的成员，就把文档添加到文档列表
                                    WorkFlow wf = CommonFunction.GetDocWorkFlow(docItem);
                                    if (wf != null)
                                    {
                                        foreach (Doc wfdocItem in wf.DocList)
                                        {
                                            reDocList.Add(wfdocItem);
                                        }
                                    }
                                }
                            }

                        }
                        catch { }
                    }
                    docList = reDocList;
                    return true;
                }
                #endregion

                //2.2  如果是收文目录，就遍历文档
                else if (projectType == "收文")
                {
                    AttrData data;
                    foreach (Doc docItem in resultDocList)
                    {
                        try
                        {
                            if (reDocList.Contains(docItem)) continue;

                            Doc doc = docItem;
                            bool isShort = false;
                            if (doc.ShortCutDoc != null)
                            {
                                doc = doc.ShortCutDoc;
                                isShort = true;
                            }

                            if (doc.Creater == curUser && isShort == false)
                            {
                                //reDocList.Add(docItem);
                                //foreach (Doc wfdocItem in docItem.WorkFlow.DocList)
                                //{
                                //    //2.2.3 如果当前用户是当前文档的发文单位的成员，就把文档添加到文档列表
                                //    //reDocList.Add(docItem);
                                //    reDocList.Add(wfdocItem);
                                //}

                                //2.2.3 如果当前用户是当前文档的发文单位的成员，就把文档添加到文档列表
                                WorkFlow wf = CommonFunction.GetDocWorkFlow(docItem);
                                if (wf != null)
                                {
                                    foreach (Doc wfdocItem in wf.DocList)
                                    {
                                        reDocList.Add(wfdocItem);
                                    }
                                }
                                continue;
                            }

                            #region 如果文档没有模板，就找流程上其他有模板的文档
                            //如果文档没有模板，就找流程上其他有模板的文档
                            if (doc.TempDefn == null && doc.WorkFlow != null)
                            {
                                Doc twd = null;
                                foreach (Doc wd in doc.WorkFlow.DocList)
                                {
                                    if (wd.TempDefn != null)
                                    {
                                        twd = wd;
                                        break;
                                    }
                                }
                                if (twd != null)
                                {
                                    doc = twd;
                                }
                            }

                            #endregion

                            string strRecAttrKeyword = "";
                            string strCopyAttrKeyword = "";
                            string strSenderAttrKeyword = "";
                            if (doc.TempDefn.KeyWord == "CATALOGUING")
                            {
                                strRecAttrKeyword = "CA_MAINFEEDERCODE";
                                strCopyAttrKeyword = "CA_COPYCODE";
                                strSenderAttrKeyword = "CA_SENDERCODE";
                            }
                            //2.2.0 只要不是发文单位的用户就添加文档
                            List<Server.Group> sendGroupList = new List<Server.Group>();
                            if ((data = doc.GetAttrDataByKeyWord(strSenderAttrKeyword)) != null)
                            {
                                string senderCode = data.ToString;
                                AVEVA.CDMS.Server.Group group = dbsource.GetGroupByName(senderCode);
                                if (group == null)
                                {
                                    continue;
                                }

                                // 2.2.2 判断当前用户是否是发文单位的成员，是发文单位的成员就跳到下一个文档
                                if (!group.AllUserList.Contains(curUser))
                                //2.2.3 如果当前用户是当前文档的发文单位的成员，不是就把文档添加到文档列表
                                // reDocList.Add(docItem);
                                {
                                    //foreach (Doc wfdocItem in docItem.WorkFlow.DocList)
                                    //{
                                    //    //2.2.3 如果当前用户是当前文档的发文单位的成员，就把文档添加到文档列表
                                    //    //reDocList.Add(docItem);
                                    //    reDocList.Add(wfdocItem);
                                    //}

                                    //2.2.3 如果当前用户是当前文档的发文单位的成员，就把文档添加到文档列表
                                    WorkFlow wf = CommonFunction.GetDocWorkFlow(docItem);
                                    if (wf != null)
                                    {
                                        foreach (Doc wfdocItem in wf.DocList)
                                        {
                                            reDocList.Add(wfdocItem);
                                        }
                                    }
                                }
                                continue;
                            }

                            //2.2.1 获取文档的收文单位和抄送单位
                            List<Server.Group> copyGroupList = new List<Server.Group>();
                            if ((data = doc.GetAttrDataByKeyWord(strRecAttrKeyword)) != null)
                            {
                                string senderCode = data.ToString;
                                AVEVA.CDMS.Server.Group group = dbsource.GetGroupByName(senderCode);
                                if (group == null)
                                {
                                    continue;
                                }

                                // 2.2.2 判断当前用户是否是收文单位或者抄送单位的成员，不是收文单位或者抄送单位的成员就跳到下一个文档
                                if (group.AllUserList.Contains(curUser))
                                //2.2.3 如果当前用户是当前文档的收文单位的成员，就把文档添加到文档列表
                                //  reDocList.Add(docItem);
                                {
                                    //foreach (Doc wfdocItem in docItem.WorkFlow.DocList)
                                    //{
                                    //    //2.2.3 如果当前用户是当前文档的发文单位的成员，就把文档添加到文档列表
                                    //    //reDocList.Add(docItem);
                                    //    reDocList.Add(wfdocItem);
                                    //}

                                    //2.2.3 如果当前用户是当前文档的发文单位的成员，就把文档添加到文档列表
                                    WorkFlow wf = CommonFunction.GetDocWorkFlow(docItem);
                                    if (wf != null)
                                    {
                                        foreach (Doc wfdocItem in wf.DocList)
                                        {
                                            reDocList.Add(wfdocItem);
                                        }
                                    }
                                }
                                continue;
                            }



                            if ((data = doc.GetAttrDataByKeyWord(strCopyAttrKeyword)) != null)
                            {
                                string senderCode = data.ToString;
                                string[] strArry = senderCode.Split(new char[] { ',' });
                                bool isAdd = false;
                                foreach (string strCopy in strArry)
                                {

                                    AVEVA.CDMS.Server.Group group = dbsource.GetGroupByName(strCopy);
                                    if (group == null)
                                    {
                                        continue;
                                    }

                                    // 2.2.2 判断当前用户是否是抄送单位的成员，不是收文单位或者抄送单位的成员就跳到下一个文档
                                    if (group.AllUserList.Contains(curUser))
                                    {
                                        //2.2.3 如果当前用户是当前文档的抄送单位的成员，就把文档添加到文档列表
                                        //reDocList.Add(docItem);
                                        //foreach (Doc wfdocItem in docItem.WorkFlow.DocList)
                                        //{
                                        //    //2.2.3 如果当前用户是当前文档的发文单位的成员，就把文档添加到文档列表
                                        //    //reDocList.Add(docItem);
                                        //    reDocList.Add(wfdocItem);
                                        //}

                                        //2.2.3 如果当前用户是当前文档的发文单位的成员，就把文档添加到文档列表
                                        WorkFlow wf = CommonFunction.GetDocWorkFlow(docItem);
                                        if (wf != null)
                                        {
                                            foreach (Doc wfdocItem in wf.DocList)
                                            {
                                                reDocList.Add(wfdocItem);
                                            }
                                        }
                                        isAdd = true;
                                        break;
                                    }

                                }
                            }
                        }
                        catch { }

                    }
                    docList = reDocList;
                    return true;
                }

                docList = resultDocList;
            }
            catch (Exception ex)
            {
                WebApi.CommonController.WebWriteLog(DateTime.Now.ToString() + ":" + "获取文件列表错误," + ex.Message);

            }
            //docList = docList.Where(d=>(( d.WorkFlow==null && d.Creater==curUser) || d.WorkFlow != null)
            // || (d.ShortCutDoc != null && ((d.ShortCutDoc.WorkFlow == null && d.ShortCutDoc.Creater == curUser) || d.ShortCutDoc.WorkFlow != null))
            // //|| d.WorkFlow.O_WorkFlowStatus != enWorkFlowStatus.Finish  //流出已经走完的文档不显示
            // ).ToList<Doc>();

            return false;
        }

        ////显示文档列表前的事件，用于按条件筛选用户能看到的文档列表
        //public static bool BeforeGetDocList(Project P, ref List<Doc> docList, string filter)
        //{
        //    if (P == null)
        //    {
        //        return false;
        //    }

        //    //搜索文档附加属性
        //    #region 搜索文档附加属性
        //    if (!string.IsNullOrEmpty(filter))
        //    {
        //        string sql = "select itemno from User_CATALOGUING where " +
        //            "CA_MAINFEEDERCODE like '%" + filter + "%' or " +
        //            "CA_SENDER like '%" + filter + "%' or " +
        //            "CA_SENDERCODE like '%" + filter + "%' or " +
        //            "CA_COPY like '%" + filter + "%' or " +
        //            "CA_COPYCODE like '%" + filter + "%' or " +
        //            "CA_SENDCODE like '%" + filter + "%' or " +
        //            "CA_RECEIPTCODE like '%" + filter + "%' or " +
        //            "CA_SENDERCLASS like '%" + filter + "%' or " +
        //            "CA_RECEIVERCLASS like '%" + filter + "%' or " +
        //            "CA_SENDUNIT like '%" + filter + "%' or " +
        //            "CA_RECUNIT like '%" + filter + "%' or " +
        //            "CA_PAGE like '%" + filter + "%' or " +
        //            "CA_SECRETGRADE like '%" + filter + "%' or " +
        //            "CA_SECRETTERM like '%" + filter + "%' or " +
        //            "CA_IFREPLY like '%" + filter + "%' or " +
        //            "CA_SERIES like '%" + filter + "%' or " +
        //            "CA_ABSTRACT like '%" + filter + "%' or " +
        //            "CA_ENCLOSURE like '%" + filter + "%' or " +
        //            "CA_URGENTDEGREE like '%" + filter + "%' or " +
        //            "CA_FILECODE like '%" + filter + "%' or " +
        //            "CA_PROCODE like '%" + filter + "%' or " +
        //            "CA_PRONAME like '%" + filter + "%' or " +
        //            "CA_FILETITLE like '%" + filter + "%' or " +
        //            "CA_ORIFILETITLE like '%" + filter + "%' or " +
        //            "CA_TITLE like '%" + filter + "%' or " +
        //            "CA_CONTENT like '%" + filter + "%' or " +
        //            "CA_NOTE like '%" + filter + "%' or " +
        //            "CA_DESNOTE like '%" + filter + "%' or " +
        //            "CA_CREW like '%" + filter + "%' or " +
        //            "CA_FACTORY like '%" + filter + "%' or " +
        //            "CA_SYSTEM like '%" + filter + "%' or " +
        //            "CA_RECTYPE like '%" + filter + "%' or " +
        //            "CA_WORKTYPE like '%" + filter + "%' or " +
        //            "CA_WORKSUBTIEM like '%" + filter + "%' or " +
        //            "CA_MAJOR like '%" + filter + "%' or " +
        //            "CA_FILEID like '%" + filter + "%' or " +
        //            "CA_DOCSOURCE like '%" + filter + "%' or " +
        //            "CA_REFERENCE like '%" + filter + "%' or " +
        //            "CA_LANGUAGES like '%" + filter + "%' or " +
        //            "CA_RELATIONFILECODE like '%" + filter + "%' or " +
        //            "CA_RELATIONFILENAME like '%" + filter + "%' or " +
        //            "CA_STATE like '%" + filter + "%' or " +
        //            "CA_ELDOCUMENT like '%" + filter + "%' or " +
        //            "CA_ORIGINAL like '%" + filter + "%' or " +
        //            "CA_NUMBER like '%" + filter + "%' or " +
        //            "CA_COPYFILE like '%" + filter + "%' or " +
        //            "CA_CARRIER like '%" + filter + "%' or " +
        //            "CA_SCANS like '%" + filter + "%' or " +
        //            "CA_FILELISTCODE like '%" + filter + "%' or " +
        //            "CA_TRANSMITMETHOD like '%" + filter + "%' or " +
        //            "CA_TRANSMITMETHODSUPP like '%" + filter + "%' or " +
        //            "CA_SUBMISSIONOBJ like '%" + filter + "%' or " +
        //            "CA_SUBMISSIONOBJSUPP like '%" + filter + "%' or " +
        //            "CA_VOLUMENUMBER like '%" + filter + "%' or " +
        //            "CA_FACTORYNAME like '%" + filter + "%' or " +
        //            "CA_SYSTEMNAME like '%" + filter + "%' or " +
        //            "CA_CONTACTSMAN like '%" + filter + "%' or " +
        //            "CA_RESPONSIBILITY like '%" + filter + "%' or " +
        //            "CA_RACKNUMBER like '%" + filter + "%' or " +
        //            "CA_MEDIUM like '%" + filter + "%' or " +
        //            "CA_FILESPEC like '%" + filter + "%' or " +
        //            "CA_FILEUNIT like '%" + filter + "%' or " +
        //            "CA_EDITION like '%" + filter + "%' or " +
        //            "CA_SUMMARYCODE like '%" + filter + "%' or " +
        //            "CA_ATTRTEMP like '%" + filter + "%' or " +
        //            "CA_ASSISTHANDLEUNIT like '%" + filter + "%' or " +
        //            "CA_MAINHANDLE_CODE like '%" + filter + "%' or " +
        //            "CA_MAINHANDLE_DESC like '%" + filter + "%' or " +
        //            "CA_RECDESIGN like '%" + filter + "%' or " +
        //            "CA_RECID like '%" + filter + "%' or " +
        //            "CA_SENDNUMBER like '%" + filter + "%' or " +
        //            "CA_RECNUMBBER like '%" + filter + "%' or " +
        //            "CA_ORIFILECODE like '%" + filter + "%' or " +
        //            "CA_READUNIT like '%" + filter + "%' or " +
        //            "CA_KEEPINGTIME like '%" + filter + "%' or " +
        //            "CA_SENDDESC like '%" + filter + "%' or " +
        //            "CA_RECEIPTDESC like '%" + filter + "%' or " +
        //            "CA_FILETYPE like '%" + filter + "%' or " +
        //            "CA_FLOWNUMBER like '%" + filter + "%' or " +
        //            "CA_UNIT like '%" + filter + "%'";


        //        //查询符合条件的文档
        //        List<Doc> dlist = P.dBSource.SelectDoc("select * from CDMS_Doc where o_itemno in (" + sql + ")");
        //        //docList.AddRange(dlist);

        //        docList = docList.Count <= 0 ? dlist : docList.Union(dlist).ToList<Doc>();          //剔除重复项

        //    }


        //    //加载目录下的文件，对于收发文目录下的文件华西能源要求：只显示个人工作台的文件，包括正在走流程的文件
        //    else
        //    {

        //        //判断是否为通讯目录下的文件
        //        if (P.ParentProject == null) return false;
        //        if (P.ParentProject.ParentProject == null) return false;
        //        if ((P.ParentProject.O_projectdesc == "收文" || P.ParentProject.O_projectdesc == "发文") && P.ParentProject.ParentProject.O_projectdesc == "通信管理")
        //        {

        //            //直接从流程里面查找流程中存在的用户
        //            string sql = "select * from CDMS_doc where o_itemno in (select c.o_itemno " +
        //                            "from CDMS_WorkUser a left join CDMS_WorkState b on a.o_stateno = b.O_stateno " +
        //                            "left join (select o_itemno, o_WorkFlowno, o_dmsstatus, o_projectno from CDMS_Doc where o_shortdocid=0 or o_shortDocID is null " +
        //                            "union all " +
        //                            "select c2.o_itemno, c2.o_WorkFlowno, c2.o_dmsstatus, c1.o_projectno " +
        //                            "from CDMS_Doc c1 " +
        //                            "left join CDMS_Doc c2 " +
        //                                "on (c2.o_shortdocid = 0 or c2.o_shortDocID is null) " +
        //                                "and c1.o_shortdocid = c2.o_itemno " +
        //                            "where c1.o_shortdocid > 0  " +
        //                                "and c2.o_itemno is not null) c  " +
        //                                    "on c.o_dmsstatus != 0  " +
        //                                    "and c.o_WorkFlowno = b.o_WorkFlowNo " +
        //                            "where c.o_projectno = " + P.O_projectno.ToString() +
        //                            "and a.o_userno = " + P.dBSource.LoginUser.O_userno.ToString() + ")";

        //            docList = P.dBSource.SelectDoc(sql);
        //            return true;

        //        }
        //    }
        //    #endregion


        //    return true;


        //    try
        //    {
        //        #region 判断是否是收发文目录，是否需要隐藏文件

        //        //判断是否是收发文目录，是否需要隐藏文件
        //        bool isRsProject = false;
        //        foreach (Doc doc in docList)
        //        {
        //            try
        //            {
        //                string docTemp = "";
        //                TempDefn doctempdefn = null;
        //                if (doc.ShortCutDoc != null)
        //                {
        //                    if (doc.ShortCutDoc.TempDefn == null)
        //                    {
        //                        continue;
        //                    }

        //                    doctempdefn = doc.ShortCutDoc.TempDefn;

        //                }
        //                else
        //                {
        //                    doctempdefn = doc.TempDefn;
        //                }
        //                if (doctempdefn == null)
        //                {
        //                    continue;
        //                }
        //                docTemp = doctempdefn.KeyWord;


        //                if (docTemp == "CATALOGUING" || docTemp == "FILETRANSMIT" ||
        //                    docTemp == "MEETINGSUMMARY" || docTemp == "DOCUMENTFILE")
        //                {
        //                    AttrData data;
        //                    //文文档模板
        //                    if ((data = doc.GetAttrDataByKeyWord("CA_ATTRTEMP")) != null)
        //                    {
        //                        string strData = data.ToString;
        //                        if (strData == "LETTERFILE" || strData == "FILETRANSMIT" ||
        //                           strData == "MEETINGSUMMARY" || strData == "DOCUMENTFILE")
        //                        {
        //                            isRsProject = true;
        //                            break;
        //                        }
        //                    }
        //                }
        //            }
        //            catch { }
        //        }

        //        Doc ddoc = docList.Find(d => d.Project != null);
        //        Project proj = ddoc.Project;

        //        if (proj == null)
        //        {
        //            //防止在某些情况下文档找不到目录，把所有文档都显示出来
        //            docList = new List<Doc>();
        //            return false;
        //        }

        //        if (proj == null || proj.TempDefn == null) return false;

        //        Project commProj = CommonFunction.getParentProjectByTempDefn(proj, "PRO_COMMUNICATION");
        //        if (commProj == null)
        //        {
        //            return false;
        //        }


        //        Project pproj = proj.ParentProject;
        //        DBSource dbsource = proj.dBSource;
        //        if (pproj == null) return false;

        //        if (!isRsProject || !(proj.TempDefn.KeyWord == "COM_COMTYPE" ||
        //            pproj.Code == "收文" || pproj.Code == "发文" ||
        //            pproj.Description == "收文" || pproj.Description == "发文"
        //            ))
        //        {
        //            return false;
        //        }
        //        #endregion

        //        User curUser = proj.dBSource.LoginUser;


        //        #region 筛选文档列表的步骤说明
        //        //1. 获取所有有流程（流程未完成），或者创建者是登录人的文档，缩小查询的范围

        //        //2 获取当前目录是收文目录还是发文目录

        //        //////////////////发文目录////////////////
        //        //2.1 如果是发文目录，就遍历文档

        //        //2.1.0 此步骤暂时忽略///////////////////////////////////////////////////////////////////////////////////////////////////////////////如果文档是快捷方式，就跳转到下一个文档（发文目录是实体文件 ，收文目录只是发文目录里的文档的快捷方式）

        //        //2.1.1获取文档的发文单位，并获取该发文单位的所有成员

        //        //2.2.2. 判断当前用户是否是发文单位的成员，不是发文单位的成员就跳到下一个文档

        //        //2.2.3 如果当前用户是当前文档的发文单位的成员，就把文档添加到文档列表

        //        //////////////////收文目录////////////////
        //        //2.2  如果是收文目录，就遍历文档

        //        //2.2.0 此步骤暂时忽略////////////////////////////////////////////////////////////////////////////////////////////////////////////// 如果文档不是快捷方式，就跳转到下一个文档

        //        //2.2.1 获取文档的收文单位和抄送单位

        //        //2.2.2 判断当前用户是否是收文单位或者抄送单位的成员，不是收文单位或者抄送单位的成员就跳到下一个文档

        //        //2.2.3 如果当前用户是当前文档的收文单位或者抄送单位的成员，就把文档添加到文档列表

        //        #endregion


        //        #region 1. 获取所有有流程（流程未完成），或者创建者是登录人的文档，缩小查询的范围
        //        //1. 获取所有有流程（流程未完成），或者创建者是登录人的文档，缩小查询的范围
        //        List<Doc> resultDocList = new List<Doc>();
        //        if (pproj.ParentProject != null && pproj.ParentProject.Description == "通信管理")
        //        {
        //            //如果是通信管理目录下的收发文文档列表，就要隐藏掉流程已经完成的文档
        //            foreach (Doc d in docList)
        //            {
        //                try
        //                {

        //                    //if (((d.WorkFlow == null && d.Creater == curUser) ||
        //                    //        (d.WorkFlow != null && d.WorkFlow.O_WorkFlowStatus != enWorkFlowStatus.Finish
        //                    //          && d.WorkFlow.CuWorkState.CuWorkUser.User == curUser))
        //                    // || (d.ShortCutDoc != null &&
        //                    //       (d.ShortCutDoc.WorkFlow != null && d.ShortCutDoc.WorkFlow.O_WorkFlowStatus != enWorkFlowStatus.Finish
        //                    //           && d.ShortCutDoc.WorkFlow.CuWorkState.CuWorkUser.User == curUser))
        //                    //    )
        //                    if ((d.ShortCutDoc == null &&
        //                                ((d.WorkFlow == null && d.Creater == curUser) ||
        //                                (d.WorkFlow != null && d.WorkFlow.O_WorkFlowStatus != enWorkFlowStatus.Finish
        //                                  )))
        //                         || (d.ShortCutDoc != null &&
        //                               (d.ShortCutDoc.WorkFlow != null && d.ShortCutDoc.WorkFlow.O_WorkFlowStatus != enWorkFlowStatus.Finish
        //                                   ))
        //                            )
        //                    {
        //                        List<WorkUser> wus = d.ShortCutDoc == null ?
        //                            d.WorkFlow.CuWorkState.WorkUserList :
        //                            d.ShortCutDoc.WorkFlow.CuWorkState.WorkUserList;

        //                        foreach (WorkUser wu in wus)
        //                        {
        //                            if (wu.User != null && wu.User == curUser)
        //                            {
        //                                resultDocList.Add(d);
        //                            }
        //                        }
        //                    }

        //                }
        //                catch { }
        //            }
        //        }
        //        else
        //        {
        //            resultDocList = docList;
        //        }
        //        #endregion

        //        #region 2. 获取当前目录是收文目录还是发文目录
        //        // 2. 获取当前目录是收文目录还是发文目录
        //        Project rsProject = CommonFunction.getParentProjectByTempDefn(proj, "COM_SUBDOCUMENT");

        //        if (rsProject == null)
        //        {
        //            WebApi.CommonController.WebWriteLog(DateTime.Now.ToString() + ":" + "没有查找到" + proj.KeyWord + "," + proj.Code + "的收发文目录。");
        //            docList = new List<Doc>();
        //            return false;
        //        }
        //        string projectType = "";
        //        if (rsProject.Code == "收文" || rsProject.Description == "收文")
        //        {
        //            projectType = "收文";
        //        }
        //        else if (rsProject.Code == "发文" || rsProject.Description == "发文")
        //        {
        //            projectType = "发文";
        //        }
        //        else
        //        {
        //            WebApi.CommonController.WebWriteLog(DateTime.Now.ToString() + ":" + "没有查找到" + proj.KeyWord + "," + proj.Code + "的收发文目录。");
        //            docList = new List<Doc>();
        //            return false;
        //        }
        //        #endregion

        //        List<Doc> reDocList = new List<Doc>();

        //        #region 2.1 如果是发文目录，就遍历文档
        //        if (projectType == "发文")
        //        {
        //            AttrData data;
        //            foreach (Doc docItem in resultDocList)
        //            {
        //                try
        //                {
        //                    Doc doc = docItem;
        //                    bool isShort = false;
        //                    if (doc.ShortCutDoc != null)
        //                    {
        //                        doc = doc.ShortCutDoc;
        //                        isShort = true;
        //                    }

        //                    if (doc.Creater == curUser && isShort == false)
        //                    {
        //                        reDocList.Add(docItem);
        //                        continue;
        //                    }

        //                    #region 判断流程用户里面有没有当前用户
        //                    //if (doc.WorkFlow != null && doc.WorkFlow.CuWorkState != null) {
        //                    //    bool isAdd = false;
        //                    //    foreach (WorkUser wu in doc.WorkFlow.CuWorkState.WorkUserList)
        //                    //    {
        //                    //        if (wu.User == curUser)
        //                    //        {
        //                    //            reDocList.Add(docItem);
        //                    //            isAdd = true;
        //                    //            break;
        //                    //        }
        //                    //    }
        //                    //    if (isAdd == true)
        //                    //    {
        //                    //        continue;
        //                    //    }
        //                    //}
        //                    //判断流程用户里面有没有当前用户
        //                    //if (doc.WorkFlow != null && doc.WorkFlowList != null)
        //                    //{
        //                    //    bool isAdd = false;
        //                    //    foreach (WorkState ws in doc.WorkFlow.WorkStateList)
        //                    //    {
        //                    //        foreach (WorkUser wu in ws.WorkUserList)
        //                    //        {
        //                    //            if (wu.User == curUser)
        //                    //            {
        //                    //                reDocList.Add(docItem);
        //                    //                isAdd = true;
        //                    //                break;
        //                    //            }
        //                    //        }
        //                    //        if (isAdd == true)
        //                    //        {
        //                    //            break;
        //                    //        }
        //                    //    }
        //                    //        if (isAdd == true)
        //                    //        {
        //                    //           continue;
        //                    //        }
        //                    //}

        //                    #endregion

        //                    string strAttrKeyword = "";

        //                    #region 如果文档没有模板，就找流程上其他有模板的文档
        //                    //如果文档没有模板，就找流程上其他有模板的文档
        //                    if (doc.TempDefn == null && doc.WorkFlow != null)
        //                    {
        //                        Doc twd = null;
        //                        foreach (Doc wd in doc.WorkFlow.DocList)
        //                        {
        //                            if (wd.TempDefn != null)
        //                            {
        //                                twd = wd;
        //                                break;
        //                            }
        //                        }
        //                        if (twd != null)
        //                        {
        //                            doc = twd;
        //                        }
        //                    }

        //                    #endregion

        //                    if (doc.TempDefn != null && doc.TempDefn.KeyWord == "CATALOGUING")
        //                    {
        //                        strAttrKeyword = "CA_SENDERCODE";
        //                    }

        //                    //2.1.1获取文档的发文单位，并获取该发文单位的所有成员
        //                    if ((data = doc.GetAttrDataByKeyWord(strAttrKeyword)) != null)
        //                    {
        //                        string senderCode = data.ToString;
        //                        AVEVA.CDMS.Server.Group group = dbsource.GetGroupByName(senderCode);
        //                        if (group == null)
        //                        {
        //                            continue;
        //                        }

        //                        //2.2.2. 判断当前用户是否是发文单位的成员，不是发文单位的成员就跳到下一个文档
        //                        if (group.AllUserList.Contains(curUser))
        //                            //2.2.3 如果当前用户是当前文档的发文单位的成员，就把文档添加到文档列表
        //                            reDocList.Add(docItem);
        //                    }

        //                }
        //                catch { }
        //            }
        //            docList = reDocList;
        //            return true;
        //        }
        //        #endregion

        //        //2.2  如果是收文目录，就遍历文档
        //        else if (projectType == "收文")
        //        {
        //            AttrData data;
        //            foreach (Doc docItem in resultDocList)
        //            {
        //                try
        //                {
        //                    Doc doc = docItem;
        //                    bool isShort = false;
        //                    if (doc.ShortCutDoc != null)
        //                    {
        //                        doc = doc.ShortCutDoc;
        //                        isShort = true;
        //                    }

        //                    if (doc.Creater == curUser && isShort == false)
        //                    {
        //                        reDocList.Add(docItem);
        //                        continue;
        //                    }

        //                    #region 如果文档没有模板，就找流程上其他有模板的文档
        //                    //如果文档没有模板，就找流程上其他有模板的文档
        //                    if (doc.TempDefn == null && doc.WorkFlow != null)
        //                    {
        //                        Doc twd = null;
        //                        foreach (Doc wd in doc.WorkFlow.DocList)
        //                        {
        //                            if (wd.TempDefn != null)
        //                            {
        //                                twd = wd;
        //                                break;
        //                            }
        //                        }
        //                        if (twd != null)
        //                        {
        //                            doc = twd;
        //                        }
        //                    }

        //                    #endregion

        //                    string strRecAttrKeyword = "";
        //                    string strCopyAttrKeyword = "";
        //                    string strSenderAttrKeyword = "";
        //                    if (doc.TempDefn.KeyWord == "CATALOGUING")
        //                    {
        //                        strRecAttrKeyword = "CA_MAINFEEDERCODE";
        //                        strCopyAttrKeyword = "CA_COPYCODE";
        //                        strSenderAttrKeyword = "CA_SENDERCODE";
        //                    }
        //                    //2.2.0 只要不是发文单位的用户就添加文档
        //                    List<Server.Group> sendGroupList = new List<Server.Group>();
        //                    if ((data = doc.GetAttrDataByKeyWord(strSenderAttrKeyword)) != null)
        //                    {
        //                        string senderCode = data.ToString;
        //                        AVEVA.CDMS.Server.Group group = dbsource.GetGroupByName(senderCode);
        //                        if (group == null)
        //                        {
        //                            continue;
        //                        }

        //                        // 2.2.2 判断当前用户是否是发文单位的成员，是发文单位的成员就跳到下一个文档
        //                        if (!group.AllUserList.Contains(curUser))
        //                            //2.2.3 如果当前用户是当前文档的发文单位的成员，不是就把文档添加到文档列表
        //                            reDocList.Add(docItem);
        //                        continue;
        //                    }

        //                    //2.2.1 获取文档的收文单位和抄送单位
        //                    List<Server.Group> copyGroupList = new List<Server.Group>();
        //                    if ((data = doc.GetAttrDataByKeyWord(strRecAttrKeyword)) != null)
        //                    {
        //                        string senderCode = data.ToString;
        //                        AVEVA.CDMS.Server.Group group = dbsource.GetGroupByName(senderCode);
        //                        if (group == null)
        //                        {
        //                            continue;
        //                        }

        //                        // 2.2.2 判断当前用户是否是收文单位或者抄送单位的成员，不是收文单位或者抄送单位的成员就跳到下一个文档
        //                        if (group.AllUserList.Contains(curUser))
        //                            //2.2.3 如果当前用户是当前文档的收文单位的成员，就把文档添加到文档列表
        //                            reDocList.Add(docItem);
        //                        continue;
        //                    }



        //                    if ((data = doc.GetAttrDataByKeyWord(strCopyAttrKeyword)) != null)
        //                    {
        //                        string senderCode = data.ToString;
        //                        string[] strArry = senderCode.Split(new char[] { ',' });
        //                        bool isAdd = false;
        //                        foreach (string strCopy in strArry)
        //                        {

        //                            AVEVA.CDMS.Server.Group group = dbsource.GetGroupByName(strCopy);
        //                            if (group == null)
        //                            {
        //                                continue;
        //                            }

        //                            // 2.2.2 判断当前用户是否是抄送单位的成员，不是收文单位或者抄送单位的成员就跳到下一个文档
        //                            if (group.AllUserList.Contains(curUser))
        //                            {
        //                                //2.2.3 如果当前用户是当前文档的抄送单位的成员，就把文档添加到文档列表
        //                                reDocList.Add(docItem);
        //                                isAdd = true;
        //                                break;
        //                            }

        //                        }
        //                    }
        //                }
        //                catch { }

        //            }
        //            docList = reDocList;
        //            return true;
        //        }

        //        docList = resultDocList;
        //    }
        //    catch (Exception ex)
        //    {
        //        WebApi.CommonController.WebWriteLog(DateTime.Now.ToString() + ":" + "获取文件列表错误," + ex.Message);

        //    }
        //    //docList = docList.Where(d=>(( d.WorkFlow==null && d.Creater==curUser) || d.WorkFlow != null)
        //    // || (d.ShortCutDoc != null && ((d.ShortCutDoc.WorkFlow == null && d.ShortCutDoc.Creater == curUser) || d.ShortCutDoc.WorkFlow != null))
        //    // //|| d.WorkFlow.O_WorkFlowStatus != enWorkFlowStatus.Finish  //流出已经走完的文档不显示
        //    // ).ToList<Doc>();

        //    return false;
        //}

        //预览文件前处理的事件
        public static bool BeforePreviewDoc(string PlugName, Doc doc, ref bool PVRight)
        {
            if (PlugName != PluginName)
            {
                return false;
            }

            DBSource dbsource = doc.dBSource;
            User curUser = dbsource.LoginUser;
            if (curUser.IsAdmin)
            {
                return true;
            }
            #region 函数逻辑
            //1.获取文档密级属性

            //2.如果密级是公开的，就返回可以显示

            //3.如果密级是受限的（发文部门和收文部门可以查看），获取收文部门和发文部门的所有用户成员

            //3.1 如果当前用户在收文部门和发文部门的所有用户成员里面，就可以显示，否则不可以显示

            //4.如果密级是商业秘密的（流程里面的人和部门领导可以查看），获取流程里面的所有人员和部门领导

            //4.1 如果当前用户在流程里面的所有人员和部门领导里面的，就可以显示，否则不可以显示 
            #endregion

            #region 1.获取文档密级属性


            string secretgrade = "";
            AttrData data;
            //获取发送方代码
            if ((data = doc.GetAttrDataByKeyWord("CA_SECRETGRADE")) != null)
            {
                secretgrade = data.ToString;
            }
            if (string.IsNullOrEmpty(secretgrade))
            {
                return false;
            }
            #endregion

            //2.如果密级是公开的，就返回可以显示(不修改PVRight)
            if (secretgrade == "公开") return true;

            #region 3.如果密级是受限的（发文部门和收文部门可以查看），获取收文部门和发文部门的所有用户成员

            if (secretgrade == "受限")
            {
                //先把文档预览权限设置为否，再判断是否有文档预览权限
                PVRight = false;

                #region 判断流程用户里面有没有当前用户


                if (doc.WorkFlow != null && doc.WorkFlowList != null)
                {
                    bool isAdd = false;
                    foreach (WorkState ws in doc.WorkFlow.WorkStateList)
                    {
                        if (ws == null) continue;

                        foreach (WorkUser wu in ws.WorkUserList)
                        {
                            if (wu.User == curUser)
                            {
                                //如果在流程里面，就返回可以预览
                                PVRight = true;
                                return true;
                            }
                        }
                    }
                }

                #endregion

                #region 再查找文档模板里面定义里面的流程用户，是否有当前用户
                //再查找文档模板里面定义里面的流程用户，是否有当前用户
                //string senderCode = "";
                //编制人 
                if ((data = doc.GetAttrDataByKeyWord("LE_DESIGN")) != null)
                {
                    if (curUser.ToString == data.ToString) PVRight = true;
                }

                //校核人 
                if ((data = doc.GetAttrDataByKeyWord("LE_CHECK")) != null)
                {
                    if (curUser.ToString == data.ToString) PVRight = true;
                }

                //审核人
                if ((data = doc.GetAttrDataByKeyWord("LE_AUDIT")) != null)
                {
                    if (curUser.ToString == data.ToString) PVRight = true;
                }

                //审定人
                if ((data = doc.GetAttrDataByKeyWord("LE_AUDIT2")) != null)
                {
                    if (curUser.ToString == data.ToString) PVRight = true;
                }

                //批准人
                if ((data = doc.GetAttrDataByKeyWord("LE_APPROV")) != null)
                {
                    if (curUser.ToString == data.ToString) PVRight = true;
                }
                if (PVRight == true)
                {
                    return true;
                }
                #endregion

                #region 判断是否是发文部门的成员，如果是，就返回可以预览文件
                //获取发文部门
                string senderCode = "";
                if ((data = doc.GetAttrDataByKeyWord("CA_SENDERCODE")) != null)
                {
                    senderCode = data.ToString;
                }

                if (!string.IsNullOrEmpty(senderCode))
                {
                    //获取发文部门的所有人员
                    AVEVA.CDMS.Server.Group group = dbsource.GetGroupByName(senderCode);
                    if (group != null)
                    {

                        //3.1.1. 判断当前用户是否是发文单位的成员
                        if (group.AllUserList.Contains(curUser))
                        //3.1.2 如果当前用户是当前文档的发文单位的成员，就返回可以预览文档
                        {
                            PVRight = true;
                            return true;
                        }
                    }
                }
                #endregion

                #region 判断是否是收文部门的成员，如果是，就返回可以预览文件
                //获取发文部门
                string recverCode = "";
                if ((data = doc.GetAttrDataByKeyWord("CA_MAINFEEDERCODE")) != null)
                {
                    recverCode = data.ToString;
                }

                if (!string.IsNullOrEmpty(recverCode))
                {
                    //获取发文部门的所有人员
                    AVEVA.CDMS.Server.Group recGroup = dbsource.GetGroupByName(recverCode);
                    if (recGroup != null)
                    {

                        //3.1.1. 判断当前用户是否是发文单位的成员
                        if (recGroup.AllUserList.Contains(curUser))
                        //3.1.2 如果当前用户是当前文档的发文单位的成员，就返回可以预览文档
                        {
                            PVRight = true;
                            return true;
                        }
                    }
                }
                #endregion

                #region 判断是否是运营管理类的非通信目录，如果是，就判断是否是可以导入的部门的成员，如果是，就返回可以预览文档

                Project stoNoComProj = CommonFunction.getParentProjectByTempDefn(doc.Project, "STO_NOCOMTYPE");

                if (stoNoComProj == null && (doc.Project.TempDefn.KeyWord == "STO_NOCOMTYPE" || doc.Project.TempDefn.Code == "STO_NOCOMTYPE"))
                {
                    stoNoComProj = doc.Project;
                }

                if (stoNoComProj != null)
                {
                    string stoUnitCode = "";
                    if ((data = stoNoComProj.GetAttrDataByKeyWord("STO_UNIT")) != null)
                    {
                        stoUnitCode = data.ToString;
                    }

                    if (!string.IsNullOrEmpty(stoUnitCode))
                    {
                        string[] strArray = (string.IsNullOrEmpty(stoUnitCode) ? "" : stoUnitCode).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string strgroup in strArray)
                        {
                            //获取发文部门的所有人员
                            AVEVA.CDMS.Server.Group stoGroup = dbsource.GetGroupByName(strgroup);

                            if (stoGroup != null)
                            {

                                //3.1.1. 判断当前用户是否是发文单位的成员
                                if (stoGroup.AllUserList.Contains(curUser))
                                //3.1.2 如果当前用户是当前文档的发文单位的成员，就返回可以预览文档
                                {
                                    PVRight = true;
                                    return true;
                                }
                            }
                        }
                    }
                }
                #endregion


                #region 判断是否是项目管理类目录,如果是，就判断是否是当前项目组的用户，如果是，就返回可以预览文档
                //
                Project rootProj = CommonFunction.getParentProjectByTempDefn(doc.Project, "HXNY_DOCUMENTSYSTEM");

                if (rootProj != null)
                {
                    AVEVA.CDMS.Server.Group pro_group = dbsource.GetGroupByName(rootProj.Code + "_ALLgroup");
                    if (pro_group != null)
                    {
                        //3.1.1. 判断当前用户是否是当前项目组的用户的成员
                        if (pro_group.AllUserList.Contains(curUser))
                        //3.1.2 如果当前用户是当前项目组的用户的成员，就返回可以预览文档
                        {
                            PVRight = true;
                            return true;
                        }
                    }
                }
                #endregion


                //如果不在收文和发文部门里面，就返回不可以预览
                PVRight = false;
                return true;
            }

            #endregion

            //4.如果密级是商业秘密的（流程里面的人和部门领导可以查看），获取流程里面的所有人员和部门领导
            if (secretgrade == "商业秘密")
            {
                //先把文档预览权限设置为否，再判断是否有文档预览权限
                PVRight = false;

                #region 判断流程用户里面有没有当前用户


                if (doc.WorkFlow != null && doc.WorkFlowList != null)
                {
                    bool isAdd = false;
                    foreach (WorkState ws in doc.WorkFlow.WorkStateList)
                    {
                        if (ws == null) continue;

                        foreach (WorkUser wu in ws.WorkUserList)
                        {
                            if (wu.User == curUser)
                            {
                                //如果在流程里面，就返回可以预览
                                PVRight = true;
                                return true;
                            }
                        }
                    }
                }

                #endregion

                #region 再查找文档模板里面定义里面的流程用户，是否有当前用户
                //再查找文档模板里面定义里面的流程用户，是否有当前用户
                //string senderCode = "";
                //编制人 
                if ((data = doc.GetAttrDataByKeyWord("LE_DESIGN")) != null)
                {
                    if (curUser.ToString == data.ToString) PVRight = true;
                }

                //校核人 
                if ((data = doc.GetAttrDataByKeyWord("LE_CHECK")) != null)
                {
                    if (curUser.ToString == data.ToString) PVRight = true;
                }

                //审核人
                if ((data = doc.GetAttrDataByKeyWord("LE_AUDIT")) != null)
                {
                    if (curUser.ToString == data.ToString) PVRight = true;
                }

                //审定人
                if ((data = doc.GetAttrDataByKeyWord("LE_AUDIT2")) != null)
                {
                    if (curUser.ToString == data.ToString) PVRight = true;
                }

                //批准人
                if ((data = doc.GetAttrDataByKeyWord("LE_APPROV")) != null)
                {
                    if (curUser.ToString == data.ToString) PVRight = true;
                }
                if (PVRight == true)
                {
                    return true;
                }
                #endregion

                #region 判断是否发文部门的部门领导或文控，是就返回可以预览文档
                //判断是否发文部门的部门领导，是就返回可以预览文档
                string senderCode = "";
                if ((data = doc.GetAttrDataByKeyWord("CA_SENDERCODE")) != null)
                {
                    senderCode = data.ToString;
                }

                if (!string.IsNullOrEmpty(senderCode))
                {
                    //获取发文部门的部门领导
                    AVEVA.CDMS.Server.Group group = dbsource.GetGroupByName(senderCode);
                    if (group != null)
                    {
                        AVEVA.CDMS.Server.Group ldGroup = group.AllGroupList.Find(g => g.Description == "部长");
                        if (ldGroup != null)
                        {
                            if (ldGroup.AllUserList.Contains(curUser))
                            //3.1.2 如果当前用户是当前文档的发文单位的成员，就返回可以预览文档
                            {
                                PVRight = true;
                                return true;
                            }
                        }

                        AVEVA.CDMS.Server.Group secGroup = group.AllGroupList.Find(g => g.Description == "文控");
                        if (secGroup != null)
                        {
                            if (secGroup.AllUserList.Contains(curUser))
                            //3.1.2 如果当前用户是当前文档的发文单位的成员，就返回可以预览文档
                            {
                                PVRight = true;
                                return true;
                            }
                        }
                    }
                }
                #endregion

                #region 判断是否收文部门的部门领导或文控，是就返回可以预览文档
                //判断是否收文部门的部门领导，是就返回可以预览文档
                string recverCode = "";
                if ((data = doc.GetAttrDataByKeyWord("CA_MAINFEEDERCODE")) != null)
                {
                    recverCode = data.ToString;
                }

                if (!string.IsNullOrEmpty(recverCode))
                {
                    //获取收文部门的部门领导
                    AVEVA.CDMS.Server.Group group = dbsource.GetGroupByName(recverCode);
                    if (group != null)
                    {
                        AVEVA.CDMS.Server.Group ldGroup = group.AllGroupList.Find(g => g.Description == "部长");
                        if (ldGroup != null)
                        {
                            if (ldGroup.AllUserList.Contains(curUser))
                            //3.1.2 如果当前用户是当前文档的收文单位的成员，就返回可以预览文档
                            {
                                PVRight = true;
                                return true;
                            }
                        }

                        AVEVA.CDMS.Server.Group secGroup = group.AllGroupList.Find(g => g.Description == "文控");
                        if (secGroup != null)
                        {
                            if (secGroup.AllUserList.Contains(curUser))
                            //3.1.2 如果当前用户是当前文档的收文单位的成员，就返回可以预览文档
                            {
                                PVRight = true;
                                return true;
                            }
                        }
                    }
                }
                #endregion

                #region 判断是否是运营管理类的非通信目录，如果是，就判断是否是可以导入的部门的领导或文控，如果是，就返回可以预览文档

                Project stoNoComProj = CommonFunction.getParentProjectByTempDefn(doc.Project, "STO_NOCOMTYPE");

                if (stoNoComProj == null && (doc.Project.TempDefn.KeyWord == "STO_NOCOMTYPE" || doc.Project.TempDefn.Code == "STO_NOCOMTYPE"))
                {
                    stoNoComProj = doc.Project;
                }

                if (stoNoComProj != null)
                {
                    string stoUnitCode = "";
                    if ((data = stoNoComProj.GetAttrDataByKeyWord("STO_UNIT")) != null)
                    {
                        stoUnitCode = data.ToString;
                    }

                    if (!string.IsNullOrEmpty(stoUnitCode))
                    {
                        string[] strArray = (string.IsNullOrEmpty(stoUnitCode) ? "" : stoUnitCode).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string strgroup in strArray)
                        {
                            //获取发文部门的所有人员
                            AVEVA.CDMS.Server.Group group = dbsource.GetGroupByName(strgroup);

                            if (group == null) continue;

                            AVEVA.CDMS.Server.Group ldGroup = group.AllGroupList.Find(g => g.Description == "部长");
                            if (ldGroup != null)
                            {
                                if (ldGroup.AllUserList.Contains(curUser))
                                //3.1.2 如果当前用户是当前文档的收文单位的成员，就返回可以预览文档
                                {
                                    PVRight = true;
                                    return true;
                                }
                            }

                            AVEVA.CDMS.Server.Group secGroup = group.AllGroupList.Find(g => g.Description == "文控");
                            if (secGroup != null)
                            {
                                if (secGroup.AllUserList.Contains(curUser))
                                //3.1.2 如果当前用户是当前文档的收文单位的成员，就返回可以预览文档
                                {
                                    PVRight = true;
                                    return true;
                                }
                            }
                        }
                    }
                }
                #endregion

                #region 判断是否是项目管理类目录,如果是，就判断是否是当前项目组的领导或者文控，如果是，就返回可以预览文档
                //
                Project rootProj = CommonFunction.getParentProjectByTempDefn(doc.Project, "HXNY_DOCUMENTSYSTEM");

                if (rootProj != null)
                {
                    //获取项目组的领导
                    string strman = rootProj.GetAttrDataByKeyWord("DESIGNDIRECTOR").ToString;

                    string[] strArray = (string.IsNullOrEmpty(strman) ? "" : strman).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string struser in strArray)
                    {
                        string userCode = struser.Substring(0, struser.IndexOf("__"));
                        if (curUser.Code == userCode) {
                            PVRight = true;
                            return true;
                        }
                    }

                    //获取项目组的文控
                    strman = rootProj.GetAttrDataByKeyWord("SECRETARILMAN").ToString;

                    strArray = (string.IsNullOrEmpty(strman) ? "" : strman).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string struser in strArray)
                    {
                        string userCode = struser.Substring(0, struser.IndexOf("__"));
                        if (curUser.Code == userCode)
                        {
                            PVRight = true;
                            return true;
                        }
                    }
                }
                #endregion
                return true;
            }

            return false;
        }

        //下载文件前处理的事件
        public static bool BeforeDownloadFile(string PlugName, Doc doc, ref bool DLRight)
        {
            if (PlugName != PluginName)
            {
                return false;
            }
            DLRight = false;
            User curUser = doc.dBSource.LoginUser;
            if (curUser.IsAdmin)
            {
                DLRight = true;
                return true;
            }

            return false;
        }

        public static bool SearchDocList(Project project, ref List<Doc> docList, string filter)
        {

            //搜索文档附加属性

            #region 搜索文档附加属性
            if (!string.IsNullOrEmpty(filter))
            {
                List<Doc> newDocList = new List<Doc>();
                foreach (Doc docItem in docList)
                {
                    if (docItem.Code.IndexOf(filter) >= 0 || docItem.Description.IndexOf(filter) >= 0)
                    {
                        newDocList.Add(docItem);
                        continue;
                    }

                    if (docItem.ShortCutDoc != null)
                    {
                        AttrData mad = docItem.ShortCutDoc.AttrDataList.Find(ad => ad.ToString.IndexOf(filter) >= 0);
                        if (mad != null)
                        {
                            newDocList.Add(docItem);
                        }
                    }
                    else
                    {
                        AttrData mad = docItem.AttrDataList.Find(ad => ad.ToString.IndexOf(filter) >= 0);
                        if (mad != null)
                        {
                            newDocList.Add(docItem);
                        }
                    }
                }
                docList = newDocList;
                //return true;
            }
            #endregion

            #region 遍历文档列表，返回有权限查看的文档（当前用户等于文档的流程里的用户）

            List<Doc> resultDocList = new List<Doc>();
            string projectTemp = "";
            if (project.TempDefn == null ||
                ((projectTemp = project.TempDefn.KeyWord) != "COM_COMTYPE" && projectTemp != "RECOGNITION"))
            {
                return true;
            }

            User curUser = project.dBSource.LoginUser;

            foreach (Doc d in project.DocList)
            {
                try
                {
                    string CuWsCode = "";

                    WorkFlow workFlow = null;
                    if (d.ShortCutDoc == null && d.WorkFlow != null)
                    {
                        workFlow = d.WorkFlow;
                    }
                    else if (d.ShortCutDoc != null && d.ShortCutDoc.WorkFlow != null)
                    {
                        workFlow = d.ShortCutDoc.WorkFlow;
                    }

                    if (workFlow == null) continue;

                    List<WorkState> workStateList = workFlow.WorkStateList.FindAll(ws => ws.WorkUserList.Find(wu => wu.User == curUser) != null);

                    foreach (WorkState workstate in workStateList)
                    {
                        //排除文档实体发文目录，且流程状态是收文状态的文档
                        if ((d.ShortCutDoc == null &&
                                 (d.WorkFlow != null &&
                                 GetSourceDocReadRight(projectTemp, workstate.Code)
                                      ))
                             || (d.ShortCutDoc != null &&
                               (d.ShortCutDoc.WorkFlow != null &&
                                 GetShortCutDocReadRight(projectTemp, workstate.Code)
                                   ))
                                )
                        {
                            if (!resultDocList.Contains(d))
                            {
                                resultDocList.Add(d);
                            }

                        }
                    }

                }
                catch { }

            }


            docList = resultDocList;

            #endregion
            return false;
        }

        //显示文件夹图标的事件，，用于按条件获取文件夹图标
        public static bool AfterGetProjectIconFun(string PlugName, Project project, ref string iconClass)//ref List<Doc> docList)
        {
            if (PlugName != PluginName)
            {
                return false;
            }
            //return true;
            User curUser = project.dBSource.LoginUser;

            List<Project> projectList = project.AllProjectList;
            projectList.Add(project);

            string projectTemp = "";

            foreach (Project proj in projectList)
            {
                try
                {


                    if (proj.TempDefn == null ||
                       ((projectTemp = proj.TempDefn.KeyWord) != "COM_COMTYPE" &&
                       projectTemp != "RECOGNITION" &&
                       projectTemp != "NOTICE"))
                    {
                        continue;
                    }
                    if (proj.Description == "通知") {
                        var sss = "1";
                    }
                    Project pproj = proj.ParentProject;
                    //if (pproj == null ||
                    //    ((pproj.ParentProject == null || (pproj.ParentProject.Description != "通信管理" && pproj.ParentProject.Description != "通信文件"))
                    //    && pproj.Description != "流程管理"))
                    if (pproj == null ||
                        ((pproj.ParentProject == null || (pproj.ParentProject.TempDefn.KeyWord != "PRO_COMMUNICATION"))
                        && pproj.TempDefn.KeyWord != "PRO_WORKFLOW"))
                    {
                        continue;
                    }
                    //if (proj.TempDefn != null && proj.TempDefn.KeyWord == "COM_COMTYPE")
                    {


                        foreach (Doc d in proj.DocList)
                        {
                            try
                            {
                                string CuWsCode = "";
                                //排除文档实体发文目录，且流程状态是收文状态的文档
                                if ((d.ShortCutDoc == null &&
                                         (d.WorkFlow != null &&
                                         GetSourceDocReadRight(projectTemp, d.WorkFlow.CuWorkState.Code) &&
                                              d.WorkFlow.O_WorkFlowStatus != enWorkFlowStatus.Finish
                                              ))
                                     || (d.ShortCutDoc != null &&
                                       (d.ShortCutDoc.WorkFlow != null && d.ShortCutDoc.WorkFlow.O_WorkFlowStatus != enWorkFlowStatus.Finish
                                           ))
                                        )
                                {
                                    List<WorkUser> wus = d.ShortCutDoc == null ?
                                        d.WorkFlow.CuWorkState.WorkUserList :
                                        d.ShortCutDoc.WorkFlow.CuWorkState.WorkUserList;

                                    foreach (WorkUser wu in wus)
                                    {
                                        if (wu.User != null && wu.User == curUser)
                                        {
                                            //resultDocList.Add(d);
                                            iconClass = "final2";
                                            return true;
                                        }
                                    }
                                }

                            }
                            catch { }

                        }
                    }
                }
                catch { }
            }
            return true;
        }

        public static ExReJObject BeforeWF(string PlugName, WorkFlow wf, WorkStateBranch wsb)
        {
            ExReJObject reJo = new ExReJObject();
            if (PlugName != PluginName)
            {
                //当reJo的成功状态返回为真时，继续流转到下一流程状态的操作
                reJo.success = true;
                return reJo;
            }
            try
            {
                #region 有默认校审意见的状态，必须填写意见
                WorkState workstate = wsb.workState;
                if (!string.IsNullOrEmpty(workstate.DefWorkState.DefualtAuditList))
                {
                    bool isPass = false;
                    User curUser = wf.dBSource.LoginUser;
                    WorkAudit audit = wsb.workState.WorkAuditList.Find(wa => wa.workUser.User == curUser && wa.O_CheckDate > wsb.workState.O_SendDate);
                    if (audit != null) isPass = true;


                    if (isPass != true)
                    {

                        #region 通知客户端弹出填写流程意见的窗口
                        reJo.data = new JArray(new JObject(
                            new JProperty("state", "RunFunc"),
                            new JProperty("plugins", "HXEPC_Plugins"),
                            new JProperty("DefWorkFlow", wf.DefWorkFlow.O_Code),
                            new JProperty("CuWorkState", wf.CuWorkState.Code),
                            new JProperty("FuncName", "fillRecAudit"),
                            new JProperty("DocKeyword", wf.doc.KeyWord),
                             new JProperty("ProjectKeyword", wf.doc.Project.KeyWord),
                            new JProperty("WorkStateKeyword", wsb.workState.KeyWord),
                            new JProperty("CheckerKeyword", wf.dBSource.LoginUser.KeyWord),
                             new JProperty("WorkflowKeyword", wf.KeyWord),
                             new JProperty("WorkStateBranchCode", wsb.defStateBrach.O_Code)
                         ));

                        //当reJo的成功状态返回为假时，中断流转到下一流程状态的操作CheckerKeyword
                        reJo.success = false;
                        return reJo;
                        #endregion
                    }
                }
                #endregion

                #region 如果是退回，弹出是否添加下一流程状态人员对话框
                //获取当前流程状态的Code
                string cuWsCode = wf.CuWorkState.DefWorkState.O_Code;
                //获取下一流程状态的Code
                string nextWsCode = wsb.NextWorkState.DefWorkState.O_Code;
                //获取前一流程状态的Code
                string preWsCode = wf.CuWorkState.PreWorkState.DefWorkState.O_Code;

                if (wf.DefWorkFlow.O_Code == "COMMUNICATIONWORKFLOW" &&
                    //校核人退回编制人，编制人再重新提交
                      ((cuWsCode == "DESIGN" && nextWsCode == "CHECK" &&
                            preWsCode == "CHECK") ||
                    //审定人退回审核人，审核人再重新提交
                       (cuWsCode == "AUDIT" && nextWsCode == "AUDIT2" &&
                            preWsCode == "AUDIT2") ||
                    //办理人退回审核人，部门负责人再重新提交
                       (cuWsCode == "RESPONSIBLE2" && nextWsCode == "MAINHANDLE" &&
                            preWsCode == "MAINHANDLE") ||
                       (cuWsCode == "DEPARTMENTCONTROL" && nextWsCode == "MAINHANDLE" &&
                            preWsCode == "MAINHANDLE")
                      )
                    )
                {
                    if (wsb.workState.O_suser3 == null || wsb.workState.O_suser3 != "addCheckerPass")
                    {
                        #region 通知客户端弹出是否添加下一流程状态人员对话框
                        string nextStateDesc = "";
                        if (nextWsCode == "CHECK")
                        {
                            nextStateDesc = "校核人";
                        }
                        else if (nextWsCode == "AUDIT2")
                        {
                            nextStateDesc = "审定人";
                        }
                        else if (nextWsCode == "MAINHANDLE")
                        {
                            nextStateDesc = "办理人";
                        }

                        reJo.data = new JArray(new JObject(
                            new JProperty("state", "RunFunc"),
                            new JProperty("plugins", "HXEPC_Plugins"),
                            new JProperty("DefWorkFlow", wf.DefWorkFlow.O_Code),
                            new JProperty("CuWorkState", wf.CuWorkState.Code),
                            new JProperty("FuncName", "ifAddChecker"),
                            new JProperty("DocKeyword", wf.doc.KeyWord),
                            new JProperty("ProjectKeyword", wf.doc.Project.KeyWord),
                            new JProperty("WorkStateKeyword", wsb.workState.KeyWord),
                            new JProperty("CheckerKeyword", wf.dBSource.LoginUser.KeyWord),
                            new JProperty("WorkflowKeyword", wf.KeyWord),
                            new JProperty("WorkStateBranchCode", wsb.defStateBrach.O_Code),
                            new JProperty("NextStateKeyword", wsb.NextWorkState.KeyWord),
                            new JProperty("NextStateDesc", nextStateDesc)
                         ));

                        //当reJo的成功状态返回为假时，中断流转到下一流程状态的操作CheckerKeyword
                        reJo.success = false;
                        return reJo;
                        #endregion
                    }
                    else if (wsb.workState.O_suser3 == "addCheckerPass")
                    {
                        //重置意见编写状态
                        wsb.workState.O_suser3 = null;
                        wsb.workState.Modify();
                    }
                }
                #endregion

                #region 项目立项流程
                if (wf.DefWorkFlow.O_Code == "CREATEPROJECT")
                {
                    if (wsb.defStateBrach.O_Description == "同意" || wsb.defStateBrach.O_Description == "结束")
                    {
                        //如果是填写好表单后再来提交下一流程，就流转到下一流程状态
                        if (!string.IsNullOrEmpty(wf.O_suser3) && wf.O_suser3 == "pass")
                        {
                            //当reJo的成功状态返回为真时，继续流转到下一流程状态的操作
                            reJo.success = true;
                            return reJo;
                        }

                        DBSource dbsource = wf.dBSource;

                        Doc m_Doc = wf.doc;
                        if (m_Doc == null)
                        {
                            reJo.success = true;
                            return reJo;
                        }
                        //获取项目代码
                        string projectCode = m_Doc.GetAttrDataByKeyWord("PROCODE").ToString;
                        //获取项目中文名称
                        string projectDescCN = m_Doc.GetAttrDataByKeyWord("PRONAME").ToString;
                        //获取项目英文名称
                        string projectDescEN = m_Doc.GetAttrDataByKeyWord("PROENGLISH").ToString;
                        //获取项目来源
                        string proSource = m_Doc.GetAttrDataByKeyWord("PROSOURCE").ToString;
                        //获取项目来源代码
                        string proSourceCode = m_Doc.GetAttrDataByKeyWord("PROSOURCECODE").ToString;
                        //获取项目地址 
                        string proAddress = m_Doc.GetAttrDataByKeyWord("PROADDRESS").ToString;
                        //获取项目邮箱
                        string eMail = m_Doc.GetAttrDataByKeyWord("EMAIL").ToString;
                        //获取国内通信代码
                        string onShoreCode = m_Doc.GetAttrDataByKeyWord("DOMCOMMUNICATIONCODE").ToString;
                        //获取国外通信代码
                        string offShoreCode = m_Doc.GetAttrDataByKeyWord("INTCOMMUNICATIONCODE").ToString;
                        //获取项目区域
                        string proFrom = "国内项目";
                        if (!string.IsNullOrEmpty(offShoreCode))
                        {
                            proFrom = "国际项目";
                        }
                        //获取项目通信名称
                        string proComName = m_Doc.GetAttrDataByKeyWord("PRONAME").ToString + "部";

                        string sourceUnite = "";
                        if (string.IsNullOrEmpty(sourceUnite)) sourceUnite = "";

                        JArray jaAttr = new JArray(
                            new JObject(
                                new JProperty("name", "projectCode"), new JProperty("value", projectCode)
                                ),
                            new JObject(
                                new JProperty("name", "projectDescCN"), new JProperty("value", projectDescCN)
                                ),
                            new JObject(
                                new JProperty("name", "projectDescEN"), new JProperty("value", projectDescEN)
                                ),
                            new JObject(
                                new JProperty("name", "proSource"), new JProperty("value", proSource)
                                ),
                            new JObject(
                                new JProperty("name", "proSourceCode"), new JProperty("value", proSourceCode)
                                ),
                            new JObject(
                                new JProperty("name", "proAddress"), new JProperty("value", proAddress)
                                ),
                            new JObject(
                                new JProperty("name", "eMail"), new JProperty("value", eMail)
                                ),
                            new JObject(
                                new JProperty("name", "onShoreCode"), new JProperty("value", onShoreCode)
                                ),
                            new JObject(
                                new JProperty("name", "offShoreCode"), new JProperty("value", offShoreCode)
                                ),
                            new JObject(
                                new JProperty("name", "proFrom"), new JProperty("value", proFrom)
                                ),
                            new JObject(
                                new JProperty("name", "proComName"), new JProperty("value", proComName)
                                )
                                );

                        ExReJObject reJo2 = CreateProject.CreateRootProjectX(dbsource, jaAttr);
                    }



                }
                #endregion

                #region 发文流程
                //发文流程
                if (wf.DefWorkFlow.O_Code == "COMMUNICATIONWORKFLOW")
                {

                    #region 批准人盖章
                    if (wf.CuWorkState.Code == "APPROV" && wsb.defStateBrach.O_Code == "TOSECRE" && wf.dBSource.LoginUser.O_userno == wf.CuWorkState.CuWorkUser.O_userno)
                    {
                        //如果是填写好表单后再来提交下一流程，就流转到下一流程状态
                        if (!string.IsNullOrEmpty(wf.O_suser3) && wf.O_suser3 == "approvpass")
                        {
                            //当reJo的成功状态返回为真时，继续流转到下一流程状态的操作
                            reJo.success = true;
                            return reJo;
                        }



                        //弹出重新设置发文编号窗口
                        reJo.data = new JArray(new JObject(
                            new JProperty("state", "RunFunc"),
                                new JProperty("plugins", "HXEPC_Plugins"),
                                new JProperty("DefWorkFlow", wf.DefWorkFlow.O_Code),
                                new JProperty("CuWorkState", wf.CuWorkState.Code),
                                 new JProperty("FuncName", "documenteSeal"),
                                new JProperty("DocKeyword", wf.doc.KeyWord),
                                new JProperty("ProjectKeyword", wf.doc.Project.KeyWord)
                                 ));
                        //当reJo的成功状态返回为假时，中断流转到下一流程状态的操作
                        reJo.success = false;
                        return reJo;
                    }
                    #endregion

                    #region 文控取号发出
                    //发文流程，文秘发出后，将发出的公文放置在共有发文目录下
                    if (wf.CuWorkState.Code == "SECRETARILMAN" && wsb.defStateBrach.O_Code == "TORECUNIT" && wf.dBSource.LoginUser.O_userno == wf.CuWorkState.CuWorkUser.O_userno)
                    {
                        //获取流程主文件

                        CataloguDoc ccadoc = new CataloguDoc();
                        ccadoc.doc = CommonFunction.GetWorkFlowDoc(wf);
                        string sendcode = ccadoc.CA_SENDCODE;

                        //如果有收文编码，就无需获取发文编号
                        string recCode = ccadoc.CA_RECEIPTCODE;

                        Project proj = new Project();

                        //判断是否发送给项目
                        bool isToProj = false;


                        //判断当前文档是否发送给项目
                        isToProj = CommonFunction.GetIfSendToProject(ccadoc.doc);

                        //Regex regex = new Regex(@"^\w+\-\w+\-\w+\-\w+\-\w+$");
                        ////bool bhvt = Regex.IsMatch(email, @"^\w+@\w+\.\w+$");
                        //if (regex.IsMatch(sendcode))
                        //{
                        //    isToProj = true;
                        //}

                        //如果有收文编码，就无需获取发文编号
                        //如果是填写好表单后再来提交下一流程，就流转到下一流程状态
                        if (!string.IsNullOrEmpty(recCode)||
                            (!string.IsNullOrEmpty(wf.CuWorkState.O_suser3) && wf.CuWorkState.O_suser3 == "pass"))
                        {
                            //复制快捷方式到收文目录
                            #region 复制快捷方式到收文目录
                            Project rsProject;
                            proj = wf.DocList[0].Project;
                            if (isToProj)
                            {
                                #region 项目发到项目，或者部门发到项目，或者项目发到部门
                                //复制快捷方式到收文目录
                                //项目发到项目，或者部门发到项目，或者项目发到部门
                                //1. 判断是否是项目发到部门
                                string recUnitCode = ccadoc.CA_MAINFEEDERCODE;
                                AVEVA.CDMS.Server.Group recSecGroup = CommonFunction.GetSecGroupByUnitCode(ccadoc.dbSource, recUnitCode);

                                //bool isSendToDepartment = false;
                                if (recSecGroup != null)
                                {
                                    #region 项目发送到部门
                                    //项目发送到部门
                                    //isSendToDepartment = true; 

                                    //运营管理类文件目录
                                    Project rootProj = wf.dBSource.RootLocalProjectList.Find(itemProj => itemProj.TempDefn.KeyWord == "OPERATEADMIN");

                                    //通信管理目录
                                    Project commProj = rootProj.ChildProjectList.Find(itemProj => itemProj.TempDefn.KeyWord == "PRO_COMMUNICATION");
                                    
                                    //Project commProj = CommonFunction.GetProjectByDesc(rootProj, "通信管理");

                                    Project recProj = CommonFunction.GetProjectByDesc(commProj, "收文");

                                    Project recCommProj = recProj.GetProjectByName(proj.Code);

                                    foreach (Doc doc in wf.DocList)
                                    {
                                        recCommProj.NewDoc(doc);
                                    }

                                    //当reJo的成功状态返回为真时，继续流转到下一流程状态的操作
                                    reJo.success = true;
                                    return reJo;

                                    #endregion
                                }
                                else
                                {
                                    #region 项目发到项目，或者部门发到项目
                                    //项目发到项目，或者部门发到项目

                                    string projCode = sendcode.Substring(0, sendcode.IndexOf("-"));
                                    Project rootProj = ccadoc.dbSource.RootLocalProjectList.Find(itemProj => itemProj.TempDefn.KeyWord == "PRODOCUMENTADMIN");
                                    Project pProj = rootProj.ChildProjectList.Find(p => p.Code == projCode);
                                    rsProject = pProj.ChildProjectList.Find(p => p.TempDefn.KeyWord == "PRO_COMMUNICATION");

                                    Project recProj = CommonFunction.GetProjectByDesc(rsProject, "收文");

                                    Project recCommProj = recProj.GetProjectByName(proj.Code);

                                    foreach (Doc doc in wf.DocList)
                                    {
                                        recCommProj.NewDoc(doc);
                                    }

                                    //当reJo的成功状态返回为真时，继续流转到下一流程状态的操作
                                    reJo.success = true;
                                    return reJo;

                                    #endregion
                                } 
                                #endregion

                            }
                            else
                            {
                                #region 部门发送到部门
                                //复制快捷方式到收文目录
                                //部门发送到部门

                                rsProject = CommonFunction.getParentProjectByTempDefn(proj, "PRO_COMMUNICATION");

                                Project recProj = CommonFunction.GetProjectByDesc(rsProject, "收文");

                                Project recCommProj = recProj.GetProjectByName(proj.Code);

                                foreach (Doc doc in wf.DocList)
                                {
                                    recCommProj.NewDoc(doc);
                                }

                                //当reJo的成功状态返回为真时，继续流转到下一流程状态的操作
                                reJo.success = true;
                                return reJo; 
                                #endregion
                            } 
                            #endregion




                        }

                        #region 获取文档发文编号
                        string docType = "";
                        string FileCode = "";
                        Doc wfDoc = null;// new Doc();
                        foreach (Doc doc in wf.DocList)
                        {
                            if (doc.TempDefn.KeyWord == "CATALOGUING")
                            {
                                docType = "信函";
                                string strDocType = "";
                                AttrData data;
                                //文文档模板
                                if ((data = doc.GetAttrDataByKeyWord("CA_ATTRTEMP")) != null)
                                {
                                    if (data.ToString == "LETTERFILE")
                                    {
                                        strDocType = "LET";
                                    }
                                    else if (data.ToString == "FILETRANSMIT")
                                    {
                                        strDocType = "TRA";
                                    }
                                    else if (data.ToString == "MEETINGSUMMARY")
                                    {
                                        strDocType = "MOM";
                                    }
                                }
                                wfDoc = doc;
                                string senderCode = "", recerCode = "";

                                if ((data = doc.GetAttrDataByKeyWord("CA_SENDERCODE")) != null)
                                {
                                    senderCode = data.ToString;
                                    FileCode = FileCode + data.ToString + "-";

                                }
                                if ((data = doc.GetAttrDataByKeyWord("CA_MAINFEEDERCODE")) != null)
                                {
                                    recerCode = data.ToString;
                                    FileCode = FileCode + data.ToString + "-";
                                }
                                if (isToProj)
                                {
                                    string projCode = sendcode.Substring(0, sendcode.IndexOf("-"));

                                    string docnum = DocumentCommon.getDocNumber(wf.dBSource, projCode, strDocType, senderCode, recerCode);
                                    FileCode = projCode + "-" + FileCode + strDocType + "-" + docnum;
                                }
                                else
                                {
                                    string docnum = DocumentCommon.getDocNumber(wf.dBSource, "", strDocType, senderCode, recerCode);
                                    FileCode = FileCode + strDocType + "-" + docnum;
                                }
                                break;
                            }
                        }
                        #endregion

                        //if (wfDoc == null) {
                        //    wfDoc = wf.doc;
                        //}

                        //wf.O_suser3 = "pass";
                        //wf.Modify();

                        //Project proj = wf.doc.Project;

                        //if (proj.Description == "信函") {

                        //    FileCode=""
                        //}
                        //弹出重新设置发文编号窗口
                        reJo.data = new JArray(new JObject(
                            new JProperty("state", "RunFunc"),
                                new JProperty("plugins", "HXEPC_Plugins"),
                                new JProperty("DefWorkFlow", wf.DefWorkFlow.O_Code),
                                new JProperty("CuWorkState", wf.CuWorkState.Code),
                            // new JProperty("FuncName", "letterCNFillInfo"),
                                new JProperty("FuncName", "resetFileCode"),
                                new JProperty("DocKeyword", wf.doc.KeyWord),
                                new JProperty("FileCode", FileCode)
                                ));
                        //当reJo的成功状态返回为假时，中断流转到下一流程状态的操作
                        reJo.success = false;
                        return reJo;
                    }
                    #endregion

                    #region 收文部门存档
                    if (wf.CuWorkState.Code == "RECUNIT" && wsb.defStateBrach.O_Code == "TOEND" && wf.dBSource.LoginUser.O_userno == wf.CuWorkState.CuWorkUser.O_userno)
                    {

                        try
                        {
                            #region 1.从发文单获取文档属性
                            CataloguDoc cdoc = new CataloguDoc();
                            cdoc.doc = CommonFunction.GetWorkFlowDoc(wf);

                            if (cdoc.doc == null)
                            {
                                reJo.msg = "存档失败，文档不存在！";
                                reJo.success = false;
                                return reJo;
                            }

                            Project sendProj = null, recProj = null;

                            #endregion

                            #region 2.获取收文部门的存档管理的收文目录，以及发文目录
                            if (!CloseDocument.GetProjectFilePath(cdoc, ref sendProj, ref recProj))
                            {
                                reJo.msg = "存档失败，存档目录不存在！";
                                reJo.success = false;
                                return reJo;
                            }
                            #endregion


                            #region 6. 流程结束后文件存档

                            if (!CloseDocument.FileArchive(wf, sendProj, recProj)) {
                                reJo.msg = "存档失败！";
                                reJo.success = false;
                                return reJo;
                            }

                            //生成回复收文编码
                            bool needGeneDistReceipt = false;//是否生成办文单
                            bool needFeedback = false;//是否发送反馈意见消息
                            CloseDocument.Close(wf, wsb, needGeneDistReceipt, needFeedback);

                            reJo.success = true;
                            return reJo;

                            #endregion
                            //Project proj = wf.doc.Project;
                            //Doc doc = null;
                            //string docType = "", senderKeyword = "", recerKeyword = "";
                            //foreach (Doc docItem in wf.DocList)
                            //{
                            //    if (docItem.TempDefn.KeyWord == "CATALOGUING")
                            //    {
                            //        doc = docItem;
                            //        AttrData data;
                            //        //文文档模板
                            //        if ((data = doc.GetAttrDataByKeyWord("CA_ATTRTEMP")) != null)
                            //        {
                            //            if (data.ToString == "LETTERFILE")
                            //            {
                            //                docType = "信函";
                            //            }
                            //            else if (data.ToString == "FILETRANSMIT")
                            //            {
                            //                docType = "文件传递单";
                            //            }
                            //            else if (data.ToString == "MEETINGSUMMARY")
                            //            {
                            //                docType = "会议纪要";
                            //            }
                            //        }
                            //        senderKeyword = "CA_SENDERCODE";
                            //        recerKeyword = "CA_MAINFEEDERCODE";
                            //        break;
                            //    }
                            //}

                            //if (doc == null)
                            //{
                            //    reJo.success = true;
                            //    return reJo;
                            //}

                            ////发文单位代码
                            //string senderCode = doc.GetAttrDataByKeyWord(senderKeyword).ToString;
                            ////收文单位代码
                            //string recerCode = doc.GetAttrDataByKeyWord(recerKeyword).ToString;

                            ////运营管理类文件目录
                            //Project rootProj = CommonFunction.getParentProjectByTempDefn(proj, "OPERATEADMIN");
                            ////存档管理目录
                            //Project storaProj = CommonFunction.GetProjectByDesc(rootProj, "存档管理");
                            ////通信类目录
                            //Project commProj = CommonFunction.GetProjectByDesc(storaProj, "通信类");

                            ////发文单位目录
                            //Project senderProj = commProj.GetProjectByName(senderCode);
                            //Project sendProj = null;
                            ////发文单位目录下的发文下的函件类型目录
                            //if (senderProj == null)
                            //{
                            //    reJo.msg = "归档失败，发文目录不存在！";
                            //    reJo.success = false;
                            //    return reJo;

                            //}
                            //try
                            //{
                            //    sendProj = senderProj.GetProjectByName("发文").GetProjectByName(docType);
                            //    if (sendProj == null)
                            //    {
                            //        reJo.msg = "归档失败，发文目录不存在！";
                            //        reJo.success = false;
                            //        return reJo;
                            //    }
                            //}
                            //catch
                            //{
                            //    reJo.msg = "归档失败，发文目录不存在！";
                            //    reJo.success = false;
                            //    return reJo;
                            //}


                            ////收文单位目录
                            //Project recerProj = commProj.GetProjectByName(recerCode);
                            ////收文单位目录下的发文下的函件类型目录
                            //if (recerProj == null)
                            //{
                            //    reJo.msg = "归档失败，收文目录不存在！";
                            //    reJo.success = false;
                            //    return reJo;
                            //}

                            //try
                            //{
                            //    Project recProj = recerProj.GetProjectByName("收文").GetProjectByName(docType);
                            //    if (recProj == null)
                            //    {
                            //        reJo.msg = "归档失败，收文目录不存在！！";
                            //        reJo.success = false;
                            //        return reJo;
                            //    }

                            //    #region 转换成pdf
                            //    try
                            //    {
                            //         Doc cpdoc = CommonFunction.GetWorkFlowDoc(wf);

                            //        string sourceFileName = cpdoc.FullPathFile; string sFileName = cpdoc.O_filename;
                            //        string targetFileName = sourceFileName.Substring(0, sourceFileName.LastIndexOf(".") + 1) + "pdf";

                            //        CDMSPdf.ConvertToPdf(cpdoc.FullPathFile, targetFileName);
                            //        cpdoc.O_filename = cpdoc.O_filename.Substring(0, cpdoc.O_filename.LastIndexOf(".") + 1) + "pdf";

                            //        cpdoc.Modify();

                            //        #region 先把docx文件复制到存档管理目录
                            //        try
                            //        {
                            //            //创建目录
                            //            if (!Directory.Exists(sendProj.FullPath))
                            //            {
                            //                Directory.CreateDirectory(sendProj.FullPath);
                            //            }

                            //            //拷贝文件
                            //            File.Move(sourceFileName, sendProj.FullPath + sFileName);
                            //        }
                            //        catch { }
                            //        #endregion
                            //    }
                            //    catch { } 
                            //    #endregion

                            //    if (wf.DocList.Count() > 0)
                            //    {
                            //        //流程结束后将文件归档
                            //        foreach (Doc cdoc in wf.DocList)
                            //        {

                            //            //创建目录
                            //            if (!Directory.Exists(sendProj.FullPath))
                            //            {
                            //                Directory.CreateDirectory(sendProj.FullPath);
                            //            }

                            //            //拷贝文件
                            //            File.Move(cdoc.FullPathFile, sendProj.FullPath + cdoc.O_filename);

                            //            //改变文件父目录
                            //            sendProj.DocList.Add(cdoc);
                            //            sendProj.Modify();

                            //            cdoc.O_projectno = sendProj.O_projectno;
                            //            cdoc.Modify();

                            //            //修改著录表里面的目录id
                            //            string format = "update User_CATALOGUING set ProjectNo={0} where Itemno={1}";

                            //            format = string.Format(format, new object[] {
                            //                    sendProj.O_projectno,cdoc.O_itemno});

                            //            cdoc.dBSource.DBExecuteSQL(format);

                            //        }


                            //        //转换PPDF
                            //        CommonFunction.ConventDocToPdf(wf.DocList[0]);

                            //        //在接收单位的目录下建立快捷方式
                            //        recProj.NewDoc(wf.DocList[0]);

                            //    }

                            //}
                            //catch (Exception e)
                            //{
                            //    reJo.msg = "归档失败，收文目录不存在！" + e.Message;
                            //    reJo.success = false;
                            //    return reJo;
                            //}


                            //reJo.success = true;
                            //return reJo;

                        }
                        catch
                        {
                            reJo.success = true;
                            return reJo;
                        }
                    }
                    #endregion

                    #region 收文部门分发办理

                    //if (wsb.NextWorkState.Code == "RECUNIT2") {
                    if (wf.CuWorkState.DefWorkState.O_Code == "RECUNIT" &&
                    (wsb.defStateBrach.O_Code == "TORESPONSIBLE" || wsb.defStateBrach.O_Code == "TOCONTROL1"))
                    {
                        SetRecUnit2(wf, wsb);
                    }

                    #region 分发办理

                    if (wf.CuWorkState.Code == "RECUNIT" && wsb.defStateBrach.O_Code == "TOCONTROL1" && wf.dBSource.LoginUser.O_userno == wf.CuWorkState.CuWorkUser.O_userno)
                    {
                        //如果是填写好表单后再来提交下一流程，就流转到下一流程状态
                        if (!string.IsNullOrEmpty(wf.O_suser3) && wf.O_suser3 == "tocontrol1pass")
                        {
                            //当reJo的成功状态返回为真时，继续流转到下一流程状态的操作
                            reJo.success = true;
                            return reJo;
                        }

                        try
                        {
                            string gpKeyword = "";
                            Server.Group gp = CommonFunction.GetUserRootOrgGroup(wf.dBSource.LoginUser);
                            if (gp != null)
                            {
                                gpKeyword = gp.KeyWord;
                            }

                            //弹出重新设置发文编号窗口DistriProcess
                            reJo.data = new JArray(new JObject(
                                new JProperty("state", "RunFunc"),
                                    new JProperty("plugins", "HXEPC_Plugins"),
                                    new JProperty("DefWorkFlow", wf.DefWorkFlow.O_Code),
                                    new JProperty("CuWorkState", wf.CuWorkState.Code),
                                    new JProperty("FuncName", "distriProcess"),
                                    new JProperty("DocKeyword", wf.doc.KeyWord),
                                    new JProperty("ProjectKeyword", wf.doc.Project.KeyWord),
                                    new JProperty("GroupType", "org"),
                                    new JProperty("GroupKeyword", gpKeyword)
                                    ));
                            //当reJo的成功状态返回为假时，中断流转到下一流程状态的操作
                            reJo.success = false;
                            return reJo;
                        }
                        catch
                        {
                            reJo.msg = "分发办理失败！";
                            reJo.success = false;
                            return reJo;
                        }
                    }
                    #endregion
                    #endregion


                    #region 办理人或设计人，提交最后一个文控的人员
                    //if ((wf.CuWorkState.Code == "MAINHANDLE" && wsb.defStateBrach.O_Code == "TOOPINION1") || //&& wf.dBSource.LoginUser.O_userno == wf.CuWorkState.CuWorkUser.O_userno)
                    //                    wf.CuWorkState.Code == "PROFESSIONAL")
                    //{
                    //    if (!string.IsNullOrEmpty(wf.O_suser3))
                    //    {

                    //     //   string[] strArry = wf.O_suser3.Split(new char[] { '~' });
                    //        bool isPass = false;
                    //        User curUser = wf.dBSource.LoginUser;
                    //        WorkAudit audit = wsb.workState.WorkAuditList.Find(wa => wa.workUser.User == curUser);
                    //        if (audit != null) isPass = true;


                    //        #region 如果成功通过了流程状态(成功填写了意见)，就跳转到下一流程状态（文控）
                    //        if (isPass == true)
                    //        {


                    //            //当reJo的成功状态返回为真时，继续流转到下一流程状态的操作
                    //            reJo.success = true;
                    //            return reJo;
                    //        }
                    //        #endregion


                    //        //  }

                    //        #region 通知客户端弹出填写流程意见的窗口
                    //        reJo.data = new JArray(new JObject(
                    //            new JProperty("state", "RunFunc"),
                    //            new JProperty("plugins", "HXEPC_Plugins"),
                    //            new JProperty("DefWorkFlow", wf.DefWorkFlow.O_Code),
                    //            new JProperty("CuWorkState", wf.CuWorkState.Code),
                    //            new JProperty("FuncName", "fillRecAudit"),
                    //            new JProperty("DocKeyword", wf.doc.KeyWord),
                    //             new JProperty("ProjectKeyword", wf.doc.Project.KeyWord),
                    //            new JProperty("WorkStateKeyword", wsb.workState.KeyWord),
                    //            new JProperty("CheckerKeyword", wf.dBSource.LoginUser.KeyWord),
                    //             new JProperty("WorkflowKeyword", wf.KeyWord),
                    //             new JProperty("WorkStateBranchCode", wsb.defStateBrach.O_Code)
                    //         ));

                    //        //当reJo的成功状态返回为假时，中断流转到下一流程状态的操作CheckerKeyword
                    //        reJo.success = false;
                    //        return reJo;
                    //        #endregion
                    //    }
                    //}
                    #endregion


                    #region 分发完成后，文控关闭文档
                    if (wf.CuWorkState.Code == "RECUNIT2" && wsb.defStateBrach.O_Code == "TOEND2")
                    {
                        //如果是填写好表单后再来提交下一流程，就流转到下一流程状态
                        string strSuser3 = string.IsNullOrEmpty(wf.O_suser3) ? "" : wf.O_suser3;
                        string[] strArry = strSuser3.Split(new char[] { '~' });
                        bool isPass = false, needFeedback = false;

                        //先判断是否要回复, 要回复的情况下，无需发送反馈消息
                        CataloguDoc caDoc = new CataloguDoc();
                        caDoc.doc = CommonFunction.GetWorkFlowDoc(wf);


                        //如果是填写好表单后再来提交下一流程，就流转到下一流程状态
                        if (caDoc.CA_IFREPLY == "是" ||
                            (!string.IsNullOrEmpty(strArry[0]) && strArry[0] == "recunit2pass"))
                        {
                            if (strArry.Length >= 2 &&
                                (!string.IsNullOrEmpty(strArry[1]) && strArry[1] == "true"))
                            {
                                //发送反馈消息
                                needFeedback = true;

                            }
                            bool needGeneDistReceipt = true;
                            CloseDocument.Close(wf, wsb, needGeneDistReceipt, needFeedback);
                            //生成办文单
                            //GeneDistReceipt(wf, wsb);

                            //当reJo的成功状态返回为真时，继续流转到下一流程状态的操作
                            reJo.success = true;
                            return reJo;
                        }

                        //弹出重新设置发文编号窗口
                        reJo.data = new JArray(new JObject(
                            new JProperty("state", "RunFunc"),
                                new JProperty("plugins", "HXEPC_Plugins"),
                                new JProperty("DefWorkFlow", wf.DefWorkFlow.O_Code),
                                new JProperty("CuWorkState", wf.CuWorkState.Code),
                                new JProperty("FuncName", "documenteFeedback"),
                                new JProperty("DocKeyword", wf.doc.KeyWord)
                         ));
                        //当reJo的成功状态返回为假时，中断流转到下一流程状态的操作
                        reJo.success = false;
                        return reJo;

                    }
                    #endregion

                }
                #endregion

                #region 收文流程
                if (wf.DefWorkFlow.O_Code == "RECEIVED")
                {
                    AttrData attrDataByKeyWord;
                    //if ((wf.CuWorkState.Code == "DESIGNMAN") && ((wf.DocList != null) && (wf.DocList.Count > 0)))
                    //{
                    //    attrDataByKeyWord = wf.DocList[0].GetAttrDataByKeyWord("DRAFTMAN");
                    //    if (attrDataByKeyWord != null)
                    //    {
                    //        attrDataByKeyWord.SetCodeDesc(wf.CuWorkState.CuWorkUser.User.ToString);
                    //        wf.DocList[0].AttrDataList.SaveData();
                    //    }
                    //}
                    if (wsb.defStateBrach.O_Description == "回复")
                    {
                        //如果是填写好表单后再来提交下一流程，就流转到下一流程状态
                        if (!string.IsNullOrEmpty(wf.O_suser3) && wf.O_suser3 == "pass")
                        {
                            //当reJo的成功状态返回为真时，继续流转到下一流程状态的操作
                            reJo.success = true;
                            return reJo;
                        }

                        //弹出填写收发文单位，发文编号窗口
                        reJo.data = new JArray(new JObject(
                            new JProperty("state", "RunFunc"),
                                new JProperty("plugins", "HXEPC_Plugins"),
                                new JProperty("DefWorkFlow", wf.DefWorkFlow.O_Code),
                                new JProperty("CuWorkState", wf.CuWorkState.Code),
                                new JProperty("FuncName", "replyLetterCN"),
                                new JProperty("DocKeyword", wf.doc.KeyWord)
                                ));
                        //当reJo的成功状态返回为假时，中断流转到下一流程状态的操作
                        reJo.success = false;
                        return reJo;

                    }
                }
                #endregion


                #region 认质认价流程

                if (wf.DefWorkFlow.O_Code == "RECOGNITION")
                {

                    #region 检查意见
                    ////检查意见
                    //if ((wsb.workState.DefWorkState.O_Code == "PRODESIGN" && wsb.defStateBrach.O_Code == "TOPROCOSTCLERK")
                    //|| (wsb.workState.DefWorkState.O_Code == "PROCOSTCLERK" && wsb.defStateBrach.O_Code == "TOPROAPPROV")
                    //|| (wsb.workState.DefWorkState.O_Code == "PROAPPROV" && wsb.defStateBrach.O_Code == "TODIRLEADER")
                    //|| (wsb.workState.DefWorkState.O_Code == "DIRECTOR" && wsb.defStateBrach.O_Code == "TODIRCOSTCLERK")
                    //|| (wsb.workState.DefWorkState.O_Code == "DIRCOSTCLERK" && wsb.defStateBrach.O_Code == "TOENGINEER")
                    //|| (wsb.workState.DefWorkState.O_Code == "ENGINEER" && wsb.defStateBrach.O_Code == "TOEND2")
                    //    )
                    //{
                    //    bool isPass = false;
                    //    User curUser = wf.dBSource.LoginUser;
                    //    WorkAudit audit = wsb.workState.WorkAuditList.Find(wa => wa.workUser.User == curUser);
                    //    if (audit != null) isPass = true;
                    //    // #region 如果成功通过了流程状态(成功填写了意见)，就跳转到下一流程状态（文控）
                    //    if (isPass != true)
                    //    {


                    //        #region 通知客户端弹出填写流程意见的窗口
                    //        reJo.data = new JArray(new JObject(
                    //            new JProperty("state", "RunFunc"),
                    //            new JProperty("plugins", "HXEPC_Plugins"),
                    //            new JProperty("DefWorkFlow", wf.DefWorkFlow.O_Code),
                    //            new JProperty("CuWorkState", wf.CuWorkState.Code),
                    //            new JProperty("FuncName", "fillRecAudit"),
                    //            new JProperty("DocKeyword", wf.doc.KeyWord),
                    //             new JProperty("ProjectKeyword", wf.doc.Project.KeyWord),
                    //            new JProperty("WorkStateKeyword", wsb.workState.KeyWord),
                    //            new JProperty("CheckerKeyword", wf.dBSource.LoginUser.KeyWord),
                    //             new JProperty("WorkflowKeyword", wf.KeyWord),
                    //             new JProperty("WorkStateBranchCode", wsb.defStateBrach.O_Code)
                    //         ));

                    //        //当reJo的成功状态返回为假时，中断流转到下一流程状态的操作CheckerKeyword
                    //        reJo.success = false;
                    //        return reJo;
                    //        #endregion
                    //    }


                    //} 
                    #endregion

                    #region 提交成本控制部领导分支 ，在运营管理目录下创建快捷方式
                    if (wsb.workState.DefWorkState.O_Code == "PROAPPROV" && wsb.defStateBrach.O_Code == "TODIRLEADER")
                    {
                        //获取运营管理类目录
                        Project m_RootProject = wf.dBSource.RootLocalProjectList.Find(itemProj => itemProj.TempDefn.KeyWord == "OPERATEADMIN");
                        if (m_RootProject == null)
                        {
                            reJo.success = true;
                            return reJo;
                        }

                        //获取文档所在目录
                        Project docProject = wf.doc.Project;
                        string strDocType = docProject.Description;

                        //获取流程管理目录
                        Project flowProj = m_RootProject.ChildProjectList.Find(p => p.Description == "流程管理");
                        if (flowProj == null)
                        {
                            reJo.success = true;
                            return reJo;
                        }

                        Project qProj = flowProj.ChildProjectList.Find(p => p.Description == strDocType);//"认质认价");
                        if (qProj == null)
                        {
                            reJo.success = true;
                            return reJo;
                        }

                        foreach (Doc doc in wf.DocList)
                        {
                            qProj.NewDoc(doc);

                        }

                        reJo.success = true;
                        return reJo;

                    }
                    #endregion


                    #region 第一个造价员状态提交到提交经理，自动把登陆用户添加到第二个造价员状态
                    if (wsb.workState.DefWorkState.O_Code == "DIRCOSTCLERK" && wsb.defStateBrach.O_Code == "TOPROAPPROV")
                    {
                        WorkState state = wf.WorkStateList.Find(ws => ws.DefWorkState.O_Code == "DIRCOSTCLERK2");
                        Server.Group gp = new Server.Group();
                        gp.AddUser(wf.dBSource.LoginUser);

                        if (state == null)
                        {
                            DefWorkState defWorkState = wf.DefWorkFlow.DefWorkStateList.Find(dwsx => dwsx.O_Code == "DIRCOSTCLERK2");
                            state = wf.NewWorkState(defWorkState);
                            state.SaveSelectUser(gp);

                            state.IsRuning = false;

                            state.PreWorkState = wsb.workState;
                            state.O_iuser5 = new int?(wsb.workState.O_stateno);
                            state.Modify();
                        }

                    }
                    #endregion

                    #region 造价员提交到确认人员，把流程上的所有校审人员，添加到确认人员状态
                    //造价员提交到确认人员，把流程上的所有校审人员，添加到确认人员状态
                    if (wsb.workState.DefWorkState.O_Code == "DIRCOSTCLERK2" && wsb.defStateBrach.O_Code == "TOCONFIREMAN")
                    {
                        #region 上传手写意见校审单
                        if (wsb.workState.O_suser3 == null || wsb.workState.O_suser3 != "isAddAtta")
                        {


                            CataloguDoc caDoc = new CataloguDoc();
                            caDoc.doc = CommonFunction.GetWorkFlowDoc(wf);
                            string fileCode = caDoc.CA_FILECODE;

                            fileCode = fileCode + " 手写校审意见单";

                            //删除之前的手写校审意见单
                            foreach (Doc doc in caDoc.Project.DocList)
                            {
                                if (doc.Code == fileCode)
                                {
                                    if (wf.DocList.Contains(doc))
                                    {
                                        wf.DocList.Remove(doc);
                                        wf.Modify();
                                        doc.WorkFlow = null;
                                        doc.Modify();
                                    }
                                    doc.Delete();
                                    break;
                                }
                            }

                            #region 通知客户端弹出添加流程附件的窗口
                            reJo.data = new JArray(new JObject(
                                new JProperty("state", "RunFunc"),
                                new JProperty("plugins", "HXEPC_Plugins"),
                                new JProperty("DefWorkFlow", wf.DefWorkFlow.O_Code),
                                new JProperty("CuWorkState", wf.CuWorkState.Code),
                                new JProperty("FuncName", "addRecognitionAtta"),
                                new JProperty("DocKeyword", wf.doc.KeyWord),
                                new JProperty("ProjectKeyword", wf.doc.Project.KeyWord),
                                new JProperty("WorkStateKeyword", wsb.workState.KeyWord),
                                new JProperty("CheckerKeyword", wf.dBSource.LoginUser.KeyWord),
                                new JProperty("WorkflowKeyword", wf.KeyWord),
                                new JProperty("WorkStateBranchCode", wsb.defStateBrach.O_Code),
                                new JProperty("FileCode", fileCode)
                             ));

                            //当reJo的成功状态返回为假时，中断流转到下一流程状态的操作CheckerKeyword
                            reJo.success = false;
                            return reJo;
                            #endregion
                        }
                        #endregion

                        #region 如果流程里面没有校审意见单，就生成校审意见单
                        CloseRecognition.CreateAuditSheet(wf, wsb);
                        #endregion

                        #region 如果不存在确认人员状态，就获取流程上的所有人员，并放到确认人员状态
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
                        #endregion
                    }
                    #endregion

                    #region 确认人员状态，所有确认人员全部确认后，关闭 并生成校审意见单
                    if (wsb.workState.DefWorkState.O_Code == "CONFIREMAN" && wsb.defStateBrach.O_Code == "TOEND3")
                    {
                        CloseRecognition.Confire(wf, wsb);
                    }
                    #endregion

                    //当reJo的成功状态返回为真时，继续流转到下一流程状态的操作
                    reJo.success = true;
                    return reJo;
                }
                #endregion


                #region 检查文件流程
                if (wf.DefWorkFlow.O_Code == "CHECKFILE")
                {
                    if (wsb.workState.DefWorkState.O_Code == "CHECK" && wsb.defStateBrach.O_Code == "DELETE")
                    {
                        //如果是删除，就删除文件
                        Doc doc = wf.doc;
                        if (doc != null)
                        {
                            if (!doc.Delete())
                            {
                                reJo.msg = "删除 " + doc.O_itemname + " 失败!";
                                reJo.success = false;
                                return reJo;
                            }
                        }
                    }
                    else if (wsb.workState.DefWorkState.O_Code == "CHECK" && wsb.defStateBrach.O_Code == "CONFIRM")
                    {
                        //如果是确认，就删除流程
                        wf.Delete();
                        wf.Delete();
                    }
                }
                #endregion



            }
            catch (Exception ex)
            { //throw; 
                var msg = ex.Message;
            }
            //当reJo的成功状态返回为真时，继续流转到下一流程状态的操作
            reJo.success = true;
            return reJo;
        }

        private static void SetRecUnit2(WorkFlow wf, WorkStateBranch wsb)
        {

            #region 添加最后一个文控的流程状态，并添加用户
            //if (wf.CuWorkState.Code == "RECUNIT" && (wsb.defStateBrach.O_Code == "TOCONTROL1" || wsb.defStateBrach.O_Code == "TORESPONSIBLE"))
            {
                #region 获取收文部门文控用户组
                Server.Group gp = new Server.Group();

                WorkState stateRec = wf.WorkStateList.Find(ws => ws.DefWorkState.O_Code == "RECUNIT");
                if (stateRec != null)
                {
                    foreach (WorkUser wu in stateRec.WorkUserList)
                    {
                        if (wu.User != null)
                        {
                            gp.AddUser(wu.User);
                        }
                    }
                }
                #endregion

                #region 添加最后一个文控的流程状态，并添加用户
                if (gp.UserList.Count > 0)
                {

                    WorkState state = wf.WorkStateList.Find(ws => ws.DefWorkState.O_Code == "RECUNIT2");
                    if (state == null)
                    {

                        //设置最后一个文控的人员
                        DefWorkState defWorkState = wf.DefWorkFlow.DefWorkStateList.Find(dwsx => dwsx.O_Code == "RECUNIT2");
                        state = wf.NewWorkState(defWorkState);
                        state.SaveSelectUser(gp);

                        state.IsRuning = false;

                        state.PreWorkState = wsb.workState;
                        state.O_iuser5 = new int?(wsb.workState.O_stateno);
                        state.Modify();

                    }
                    else
                    {
                        //如果文控状态存在，并且用户组没有人员，就添加用户
                        if (state.WorkUserList.Count <= 0)
                        {
                            state.SaveSelectUser(gp);

                            state.IsRuning = false;

                            state.PreWorkState = wsb.workState;
                            state.O_iuser5 = new int?(wsb.workState.O_stateno);

                            state.Modify();

                        }
                    }
                }

                #endregion
            }
            #endregion
        }

        /// <summary>
        /// 文档创建人撤回流程
        /// </summary>
        /// <param name="PlugName"></param>
        /// <param name="wf"></param>
        /// <returns></returns>
        public static ExReJObject RevokeWorkFlow(string PlugName, WorkFlow wf)
        {
            ExReJObject reJo = new ExReJObject();
            try
            {
                if (PlugName != PluginName)
                {
                    //当reJo的成功状态返回为真时，继续流转到下一流程状态的操作
                    reJo.success = true;
                    return reJo;
                }

                #region 代码逻辑

                //0. 判断流程状态，是否在批准之前（包括批准）
                //1. 弹出起草界面，修改各个属性
                //2  重新修改文档
                //3. 获取流程里面的文档
                //4. 获取流程各个状态的用户
                //5. 删除流程
                //6. 新建流程，把文档添加到流程
                //7. 把各个流程状态用户填入流程内 

                #endregion


                #region 0.判断流程状态，是否在批准之前（包括批准）
                bool RevokeRight = RevokeWorkflow.CheckRevokeRight(wf);
                if (!RevokeRight)
                {
                    reJo.msg = "错误的文档操作信息！用户没有回撤流程权限！";
                    //当reJo的成功状态返回为真时，继续流转到下一流程状态的操作
                    reJo.success = true;
                    return reJo;
                }
                //WorkState cuWorkState = wf.CuWorkState;
                //User cuUser = wf.dBSource.LoginUser;

                //#region 判断是否编制人
                //WorkState wsDesign = wf.WorkStateList.Find(ws => ws.DefWorkState.O_Code == "DESIGN");

                //bool isDesigner = false;
                //foreach (WorkUser wu in wsDesign.WorkUserList)
                //{
                //    if (wu.User == cuUser)
                //    {
                //        isDesigner = true;
                //    }
                //}

                //if (!isDesigner)
                //{
                //    //当reJo的成功状态返回为真时，继续流转到下一流程状态的操作
                //    reJo.success = true;
                //    return reJo;
                //}

                //#endregion

                //string cuWsCode = cuWorkState.DefWorkState.O_Code;

                //if (wf.DefWorkFlow.O_Code != "COMMUNICATIONWORKFLOW")
                //{
                //    //当reJo的成功状态返回为真时，继续流转到下一流程状态的操作
                //    reJo.success = true;
                //    return reJo;
                //}

                ////判断流程是否已经通过批准
                //if (cuWsCode != "SECRETARIL" && cuWsCode != "CHECK" && cuWsCode != "AUDIT" &&
                //    cuWsCode != "AUDIT2" && cuWsCode != "APPROV")
                //{
                //    //当reJo的成功状态返回为真时，继续流转到下一流程状态的操作
                //    reJo.success = true;
                //    return reJo;
                //}
                #endregion

                CataloguDoc caDoc = new CataloguDoc();
                caDoc.doc = CommonFunction.GetWorkFlowDoc(wf);
                string docType = "";
                string docTemp = caDoc.CA_ATTRTEMP;
                if (docTemp == "LETTERFILE")
                {
                    docType = "LET";
                }
                else if (docTemp == "FILETRANSMIT")
                {
                    docType = "TRA";
                }
                else if (docTemp == "MEETINGSUMMARY")
                {
                    docType = "MOM";
                }

                //弹出重新设置发文编号窗口
                reJo.data = new JArray(new JObject(
                new JProperty("state", "RunFunc"),
                    new JProperty("plugins", "HXEPC_Plugins"),
                    new JProperty("DefWorkFlow", wf.DefWorkFlow.O_Code),
                    new JProperty("CuWorkState", wf.CuWorkState.Code),
                    new JProperty("FuncName", "revokeWorkFlow"),
                    new JProperty("ProjectKeyword", wf.doc.Project.KeyWord),
                    new JProperty("DocKeyword", wf.doc.KeyWord),
                    new JProperty("DocType", docType)
             ));
                //当reJo的成功状态返回为假时，中断流转到下一流程状态的操作
                reJo.success = false;
                return reJo;

            }
            catch
            { //throw; 
            }
            //当reJo的成功状态返回为真时，继续流转到下一流程状态的操作
            reJo.success = true;
            return reJo;
        }

        public static bool BeforeSelectUser(string PlugName, WorkStateBranch wsBranch, ref string tabType, ref string tabPara)
        {
            if (PlugName != PluginName)
            {
                return false;
            }
            DBSource dbsource = wsBranch.dBSource;
            User curUser = dbsource.LoginUser;

            string groupCode = "";
            string groupKeyword = "";
            string groupType = "";

            //获取组织机构用户组
            foreach (AVEVA.CDMS.Server.Group groupOrg in dbsource.AllGroupList)
            {
                if ((groupOrg.ParentGroup == null) && (groupOrg.O_grouptype == enGroupType.Organization))
                {
                    if (groupOrg.AllUserList.Contains(curUser))
                    {

                        groupCode = groupOrg.Code;
                        groupKeyword = groupOrg.KeyWord;
                        groupType = "org";
                        break;
                    }
                }
            }

            tabType = groupType;
            tabPara = groupKeyword;

            return true;
        }


        //AfterWorkFlowGetBanch
        public static bool AfterWorkFlowGetBanch(string PlugName, ref WorkStateBranch wsBranch)
        {
            if (PlugName != PluginName)
            {
                return false;
            }
            WorkFlow wf = wsBranch.workState.WorkFlow;
            #region 发文流程
            //发文流程

            #region 代码逻辑
            //功能：自动创建下一流程状态，区别于其他分支的下一流程状态

            //1.判断流程是否发文流程

            //2.判断WorkState

            //3.如果WorkState等于部门文控，且分支为提交处理人，就获取下一状态的前一状态的ID，
            //  比较此ID是否等于当前WorkState的id

            //3.1 如果等于，就返回

            //3.2 如果不等于，就新建下一流程状态，新建流程分支，把当前流程分支，替换为新建的流程分支，并返回

            //4.如果WorkState等于办理人，且分支为填写意见，就获取下一状态的前一状态的ID，
            //  比较此ID是否等于当前WorkState的id

            //4.1 如果等于，就返回

            //4.2 如果不等于，就新建下一流程状态，新建流程分支，把当前流程分支，替换为新建的流程分支，并返回

            //5.如果WorkState等于设计人，且分支为填写意见，就获取下一状态的前一状态的ID，
            //  比较此ID是否等于当前WorkState的id

            //5.1 如果等于，就返回

            //5.2 如果不等于，就新建下一流程状态，新建流程分支，把当前流程分支，替换为新建的流程分支，并返回 
            #endregion

            if (wf.DefWorkFlow.O_Code == "COMMUNICATIONWORKFLOW")
            {
                //2.判断WorkState
                WorkState curWs = wf.CuWorkState;

                #region 部门文控提交流程事件
                //3.如果WorkState等于部门文控，且分支为提交处理人，就获取下一状态的前一状态的ID，
                //  比较此ID是否等于当前WorkState的id
                if ((curWs.DefWorkState.O_Code == "DEPARTMENTCONTROL" &&
                        (wsBranch.defStateBrach.O_Code == "MAINHANDLE2" || wsBranch.defStateBrach.O_Code == "TORESPONSIBLE2")) ||
                     (curWs.DefWorkState.O_Code == "RESPONSIBLE2" && wsBranch.defStateBrach.O_Code == "MAINHANDLE3") ||
                     (curWs.DefWorkState.O_Code == "MAINHANDLE" && wsBranch.defStateBrach.O_Code == "TOPROFESSIONAL")
                    )
                {
                    //if (wsBranch.defStateBrach.O_Code == "MAINHANDLE2")
                    {

                        //if (wsBranch.NextWorkState.PreWorkState == null) {
                        //    return true;
                        //}

                        //获取下一状态的前一状态的ID
                        int perId = wsBranch.NextWorkState.PreWorkState.ID;

                        //3.1 如果等于，就返回
                        if (perId == wsBranch.workState.ID)
                        {
                            return true;
                        }
                        else
                        {
                            //3.2 如果不等于，就新建下一流程状态，新建流程分支，
                            //把当前流程分支，替换为新建的流程分支，并返回
                            //DefWorkState defWorkState = wf.DefWorkFlow.DefWorkStateList.Find(dwsx => dwsx.O_Code == "MAINHANDLE");
                            DefWorkState defWorkState = wsBranch.NextWorkState.DefWorkState;
                            WorkState state = wf.NewWorkState(defWorkState);
                            //state.SaveSelectUser(secGroup);

                            state.IsRuning = false;

                            state.PreWorkState = wsBranch.workState;
                            state.O_iuser5 = new int?(wsBranch.workState.O_stateno);
                            state.Modify();

                            wsBranch.NextWorkState = state;
                            wsBranch.Modify();
                            return true;
                        }
                    }
                }
                #endregion

                //#region 办理人提交流程事件
                ////4.如果WorkState等于办理人，且分支为填写意见，就获取下一状态的前一状态的ID，
                ////  比较此ID是否等于当前WorkState的id
                //if (curWs.DefWorkState.O_Code == "MAINHANDLE")
                //{
                //    if (wsBranch.defStateBrach.O_Code == "MAINHANDLE2")
                //    {
                //        //获取下一状态的前一状态的ID
                //        int perId = wsBranch.NextWorkState.PreWorkState.ID;

                //        //4.1 如果等于，就返回
                //        if (perId == wsBranch.workState.ID)
                //        {
                //            return true;
                //        }
                //        else
                //        {
                //            //4.2 如果不等于，就新建下一流程状态，新建流程分支，
                //            //把当前流程分支，替换为新建的流程分支，并返回
                //            DefWorkState defWorkState = wf.DefWorkFlow.DefWorkStateList.Find(dwsx => dwsx.O_Code == "MAINHANDLE");
                //            WorkState state = wf.NewWorkState(defWorkState);
                //            //state.SaveSelectUser(secGroup);

                //            state.IsRuning = false;

                //            state.PreWorkState = wsBranch.workState;
                //            state.O_iuser5 = new int?(wsBranch.workState.O_stateno);
                //            state.Modify();

                //            wsBranch.NextWorkState = state;
                //            wsBranch.Modify();
                //            return true;
                //        }
                //    }
                //}
                //#endregion
            }
            #endregion
            return false;
        }

        /// <summary>
        /// 填充word属性，保存word前触发的事件
        /// </summary>
        /// <param name="PlugName"></param>
        /// <param name="WordApp"></param>
        /// <param name="oProjectOrDoc"></param>
        /// <returns></returns>
        public static bool BeforeSaveWord(string PlugName, Word.Application WordApp, object oProjectOrDoc, ref bool needReWrite, ref Hashtable htReWrite)
        {
            if (PlugName != PluginName)
            {
                return false;
            }
            if (oProjectOrDoc is Doc)
            {
                CataloguDoc caDoc = new CataloguDoc();
                caDoc.doc = (Doc)oProjectOrDoc;


                if (!caDoc.isCataloguDoc) return true;

                if (caDoc.doc.WorkFlow != null) return true;

                //获取附加属性里面原来的页数 
                string prePages = caDoc.CA_PAGE;

                Word.Document m_wdDoc = WordApp.ActiveDocument;

                object oMissing = System.Reflection.Missing.Value;

                //获取WORD文档的页数
                int pages = m_wdDoc.ComputeStatistics(Word.WdStatistic.wdStatisticPages, ref oMissing);

                if (pages > 0)
                {
                    string strPages = Convert.ToString(pages);
                    //保存到文档附加属性
                    string totalPages = caDoc.CA_PAGE = string.IsNullOrEmpty(prePages) ? strPages : strPages + "+" + prePages;
                    caDoc.SaveAttrData();

                    //把页数填充到word文档
                    needReWrite = true;
                    htReWrite.Add("PAGE", totalPages);//页数
                }

            }
            return true;
        }

        /// <summary>
        /// 用户获取系统子菜单前的事件
        /// </summary>
        /// <param name="PlugName"></param>
        /// <param name="User"></param>
        /// <param name="MenuName"></param>
        /// <param name="MenuState"></param>
        /// <returns></returns>
        public static bool BeforeGetSysMenuItem(string PlugName, User User, string MenuName, ref enWebMenuState MenuState)
        {
            if (PlugName != PluginName)
            {
                return false;
            }
            if (!User.IsAdmin)
            {
                if (MenuName != "预览文件")
                {
                    MenuState = enWebMenuState.Hide;
                    return true;
                }
            }

            return false;

        }


    }
}
