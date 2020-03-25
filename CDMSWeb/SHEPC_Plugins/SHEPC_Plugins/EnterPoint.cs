using System;
using System.Collections;
using System.Collections.Generic;
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

namespace AVEVA.CDMS.SHEPC_Plugins
{
    public class EnterPoint
    {
        public static string PluginName = "SHEPC_Plugins";
        public static void Init()
        {
            //if (WebWorkFlowEvent.BeforeWFSelectUsers1 == null)
            //{
            //    WebWorkFlowEvent.BeforeWFSelectUsers1 = (WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event1)Delegate.Combine(WebWorkFlowEvent.BeforeWFSelectUsers1, new WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event1(AVEVA.CDMS.SHEPC_Plugins.EnterPoint.BeforeWF));
            //}
            //else
            //{
            //    WebWorkFlowEvent.BeforeWFSelectUsers1 = new WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event1(AVEVA.CDMS.SHEPC_Plugins.EnterPoint.BeforeWF);
            //}

            //记录本插件的唯一标记
            //string PluginName = "SHEPC_Plugins";
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

                //WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event BeforeWFSelectUsers = new WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event(AVEVA.CDMS.SHEPC_Plugins.EnterPoint.BeforeWF);
                //WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event_Class Before_WorkFlow_SelectUsers_Event_Class = new WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event_Class();
                //Before_WorkFlow_SelectUsers_Event_Class.Event = BeforeWFSelectUsers;
                //Before_WorkFlow_SelectUsers_Event_Class.PluginName = PluginName;
                //WebWorkFlowEvent.ListBeforeWFSelectUsers.Add(Before_WorkFlow_SelectUsers_Event_Class);

                //添加流程按钮事件处理
                WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event BeforeWFSelectUsers = new WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event(AVEVA.CDMS.SHEPC_Plugins.EnterPoint.BeforeWF);
                WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event_Class Before_WorkFlow_SelectUsers_Event_Class = new WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event_Class();
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

                //if (WebWorkFlowEvent.BeforeWFSelectUsers1 == null)
                //{
                //    WebWorkFlowEvent.BeforeWFSelectUsers1 = (WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event1)Delegate.Combine(WebWorkFlowEvent.BeforeWFSelectUsers1, new WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event1(AVEVA.CDMS.SHEPC_Plugins.EnterPoint.BeforeWF));
                //}
                //else
                //{
                //    WebWorkFlowEvent.BeforeWFSelectUsers1 = new WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event1(AVEVA.CDMS.SHEPC_Plugins.EnterPoint.BeforeWF);
                //}

                //CreateProjectMenu menu2 = new CreateProjectMenu
                //{
                //    MenuId = "_CreateProject",
                //    MenuName = "创建工程",
                //    MenuType = enWebMenuType.Single,
                //    MenuPosition = enWebMenuPosition.TVProject,
                //    TVMenuPositon = enWebTVMenuPosition.TVDocument,
                //    //MenuState = enWebMenuState.Enabled,
                //};
                //list.Add(menu2);

                ContactListMenu hw = new ContactListMenu();
                hw.MenuName = "生成联系单";
                hw.MenuPosition = enWebMenuPosition.TVProject;
                hw.MenuType = enWebMenuType.Single;

                menuList.Add(hw);

                A1Menu A1 = new A1Menu();
                A1.MenuName = "生成A.1工程开工报审表";
                A1.MenuPosition = enWebMenuPosition.TVProject;
                A1.MenuType = enWebMenuType.Single;

                menuList.Add(A1);

                A2Menu A2 = new A2Menu();
                A2.MenuName = "生成A.2工程复工申请表";
                A2.MenuPosition = enWebMenuPosition.TVProject;
                A2.MenuType = enWebMenuType.Single;

                menuList.Add(A2);

                A3Menu A3 = new A3Menu();
                A3.MenuName = "生成A.3施工组织设计报审表";
                A3.MenuPosition = enWebMenuPosition.TVProject;
                A3.MenuType = enWebMenuType.Single;

                menuList.Add(A3);

                A4Menu A4 = new A4Menu();
                A4.MenuName = "生成A.4方案报审表";
                A4.MenuPosition = enWebMenuPosition.TVProject;
                A4.MenuType = enWebMenuType.Single;

                menuList.Add(A4);

                A5Menu A5 = new A5Menu();
                A5.MenuName = "生成A.5分包单位资格报审表";
                A5.MenuPosition = enWebMenuPosition.TVProject;
                A5.MenuType = enWebMenuType.Single;

                menuList.Add(A5);

                A6Menu A6 = new A6Menu();
                A6.MenuName = "生成A.6单位资质报审表";
                A6.MenuPosition = enWebMenuPosition.TVProject;
                A6.MenuType = enWebMenuType.Single;

                menuList.Add(A6);

                A7Menu A7 = new A7Menu();
                A7.MenuName = "生成A.7人员资质报审表";
                A7.MenuPosition = enWebMenuPosition.TVProject;
                A7.MenuType = enWebMenuType.Single;

                menuList.Add(A7);

                A8Menu A8 = new A8Menu();
                A8.MenuName = "生成A.8工程控制网测量／线路复测报审表";
                A8.MenuPosition = enWebMenuPosition.TVProject;
                A8.MenuType = enWebMenuType.Single;

                menuList.Add(A8);

                A9Menu A9 = new A9Menu();
                A9.MenuName = "生成A.9主要施工机械／工器具／安全用具报审表";
                A9.MenuPosition = enWebMenuPosition.TVProject;
                A9.MenuType = enWebMenuType.Single;

                menuList.Add(A9);

                A10Menu A10 = new A10Menu();
                A10.MenuName = "生成A.10主要测量计量器具／试验设备检验报审表";
                A10.MenuPosition = enWebMenuPosition.TVProject;
                A10.MenuType = enWebMenuType.Single;

                menuList.Add(A10);

                A11Menu A11 = new A11Menu();
                A11.MenuName = "生成A.11质量验收及评定项目划分报审表";
                A11.MenuPosition = enWebMenuPosition.TVProject;
                A11.MenuType = enWebMenuType.Single;

                menuList.Add(A11);

                A12Menu A12 = new A12Menu();
                A12.MenuName = "生成A.12工程材料／构配件／设备报审表";
                A12.MenuPosition = enWebMenuPosition.TVProject;
                A12.MenuType = enWebMenuType.Single;

                menuList.Add(A12);

                A13Menu A13 = new A13Menu();
                A13.MenuName = "生成A.13主要设备开箱申请表";
                A13.MenuPosition = enWebMenuPosition.TVProject;
                A13.MenuType = enWebMenuType.Single;

                menuList.Add(A13);

                A14Menu A14 = new A14Menu();
                A14.MenuName = "生成A.14验收申请表";
                A14.MenuPosition = enWebMenuPosition.TVProject;
                A14.MenuType = enWebMenuType.Single;

                menuList.Add(A14);

                A15Menu A15 = new A15Menu();
                A15.MenuName = "生成A.15中间交付验收交接表";
                A15.MenuPosition = enWebMenuPosition.TVProject;
                A15.MenuType = enWebMenuType.Single;

                menuList.Add(A15);

                A16Menu A16 = new A16Menu();
                A16.MenuName = "生成A.16计划／调整计划报审表";
                A16.MenuPosition = enWebMenuPosition.TVProject;
                A16.MenuType = enWebMenuType.Single;

                menuList.Add(A16);

                A17Menu A17 = new A17Menu();
                A17.MenuName = "生成A.17费用索赔申请表";
                A17.MenuPosition = enWebMenuPosition.TVProject;
                A17.MenuType = enWebMenuType.Single;

                menuList.Add(A17);

                A18Menu A18 = new A18Menu();
                A18.MenuName = "生成A.18监理工程师通知回复单";
                A18.MenuPosition = enWebMenuPosition.TVProject;
                A18.MenuType = enWebMenuType.Single;

                menuList.Add(A18);

                A19Menu A19 = new A19Menu();
                A19.MenuName = "生成A.19工程款支付申请表";
                A19.MenuPosition = enWebMenuPosition.TVProject;
                A19.MenuType = enWebMenuType.Single;

                menuList.Add(A19);

                A20Menu A20 = new A20Menu();
                A20.MenuName = "生成A.20工期变更报审表";
                A20.MenuPosition = enWebMenuPosition.TVProject;
                A20.MenuType = enWebMenuType.Single;

                menuList.Add(A20);

                A21Menu A21 = new A21Menu();
                A21.MenuName = "生成A.21设备／材料／构配件缺陷通知单";
                A21.MenuPosition = enWebMenuPosition.TVProject;
                A21.MenuType = enWebMenuType.Single;

                menuList.Add(A21);

                A22Menu A22 = new A22Menu();
                A22.MenuName = "生成A.22设备／材料／构配件缺陷处理报验表";
                A22.MenuPosition = enWebMenuPosition.TVProject;
                A22.MenuType = enWebMenuType.Single;

                menuList.Add(A22);

                A23Menu A23 = new A23Menu();
                A23.MenuName = "生成A.23单位工程验收申请表";
                A23.MenuPosition = enWebMenuPosition.TVProject;
                A23.MenuType = enWebMenuType.Single;

                menuList.Add(A23);

                A24Menu A24 = new A24Menu();
                A24.MenuName = "生成A.24工程竣工报验单";
                A24.MenuPosition = enWebMenuPosition.TVProject;
                A24.MenuType = enWebMenuType.Single;

                menuList.Add(A24);

                A25Menu A25 = new A25Menu();
                A25.MenuName = "生成A.25设计变更／变更设计执行情况反馈单";
                A25.MenuPosition = enWebMenuPosition.TVProject;
                A25.MenuType = enWebMenuType.Single;

                menuList.Add(A25);

                A26Menu A26 = new A26Menu();
                A26.MenuName = "生成A.26大中型施工机械进场／出场申报表";
                A26.MenuPosition = enWebMenuPosition.TVProject;
                A26.MenuType = enWebMenuType.Single;

                menuList.Add(A26);

                A27Menu A27 = new A27Menu();
                A27.MenuName = "生成A.27作业指导书报审表";
                A27.MenuPosition = enWebMenuPosition.TVProject;
                A27.MenuType = enWebMenuType.Single;

                menuList.Add(A27);

                A28Menu A28 = new A28Menu();
                A28.MenuName = "生成A.28工程沉降观测报审表";
                A28.MenuPosition = enWebMenuPosition.TVProject;
                A28.MenuType = enWebMenuType.Single;

                menuList.Add(A28);

                A29Menu A29 = new A29Menu();
                A29.MenuName = "生成A.29工程量签证单";
                A29.MenuPosition = enWebMenuPosition.TVProject;
                A29.MenuType = enWebMenuType.Single;

                menuList.Add(A29);

                A30Menu A30 = new A30Menu();
                A30.MenuName = "生成A.30总承包单位资质报审表";
                A30.MenuPosition = enWebMenuPosition.TVProject;
                A30.MenuType = enWebMenuType.Single;

                menuList.Add(A30);

                A31Menu A31 = new A31Menu();
                A31.MenuName = "生成A.31设计配合比报审表";
                A31.MenuPosition = enWebMenuPosition.TVProject;
                A31.MenuType = enWebMenuType.Single;

                menuList.Add(A31);

                A32Menu A32 = new A32Menu();
                A32.MenuName = "生成A.32强条实施计划／细则报审表";
                A32.MenuPosition = enWebMenuPosition.TVProject;
                A32.MenuType = enWebMenuType.Single;

                menuList.Add(A32);

                A33Menu A33 = new A33Menu();
                A33.MenuName = "生成A.33工程量签证单（业主合同）";
                A33.MenuPosition = enWebMenuPosition.TVProject;
                A33.MenuType = enWebMenuType.Single;

                menuList.Add(A33);

                A34Menu A34 = new A34Menu();
                A34.MenuName = "生成A.34工程量确认单（合同范围内）";
                A34.MenuPosition = enWebMenuPosition.TVProject;
                A34.MenuType = enWebMenuType.Single;

                menuList.Add(A34);

                A35Menu A35 = new A35Menu();
                A35.MenuName = "生成A.35承包单位资质报审表";
                A35.MenuPosition = enWebMenuPosition.TVProject;
                A35.MenuType = enWebMenuType.Single;

                menuList.Add(A35);

                B1Menu B1 = new B1Menu();
                B1.MenuName = "生成B.1监理工作联系单";
                B1.MenuPosition = enWebMenuPosition.TVProject;
                B1.MenuType = enWebMenuType.Single;

                menuList.Add(B1);

                B2Menu B2 = new B2Menu();
                B2.MenuName = "生成B.2监理工程师通知单";
                B2.MenuPosition = enWebMenuPosition.TVProject;
                B2.MenuType = enWebMenuType.Single;

                menuList.Add(B2);

                B3Menu B3 = new B3Menu();
                B3.MenuName = "生成B.3工程暂停令";
                B3.MenuPosition = enWebMenuPosition.TVProject;
                B3.MenuType = enWebMenuType.Single;

                menuList.Add(B3);

                C1Menu C1 = new C1Menu();
                C1.MenuName = "生成C.1图纸交付计划报审表";
                C1.MenuPosition = enWebMenuPosition.TVProject;
                C1.MenuType = enWebMenuType.Single;

                menuList.Add(C1);

                C2Menu C2 = new C2Menu();
                C2.MenuName = "生成C.2设计文件报检表";
                C2.MenuPosition = enWebMenuPosition.TVProject;
                C2.MenuType = enWebMenuType.Single;

                menuList.Add(C2);

                C3Menu C3 = new C3Menu();
                C3.MenuName = "生成C.3设计变更通知单报检表";
                C3.MenuPosition = enWebMenuPosition.TVProject;
                C3.MenuType = enWebMenuType.Single;

                menuList.Add(C3);

                D1Menu D1 = new D1Menu();
                D1.MenuName = "生成D.1工程联系单";
                D1.MenuPosition = enWebMenuPosition.TVProject;
                D1.MenuType = enWebMenuType.Single;

                menuList.Add(D1);

                D2Menu D2 = new D2Menu();
                D2.MenuName = "生成D.2工程变更申请单";
                D2.MenuPosition = enWebMenuPosition.TVProject;
                D2.MenuType = enWebMenuType.Single;

                menuList.Add(D2);

                D3Menu D3 = new D3Menu();
                D3.MenuName = "生成D.3工程联系单";
                D3.MenuPosition = enWebMenuPosition.TVProject;
                D3.MenuType = enWebMenuType.Single;

                menuList.Add(D3);

                SQUMenu SQU = new SQUMenu();
                SQU.MenuName = "生成SQU工程联系单";
                SQU.MenuPosition = enWebMenuPosition.TVProject;
                SQU.MenuType = enWebMenuType.Single;

                menuList.Add(SQU);

                return menuList;
            }
            catch { }
            return null;
        }

        //public static void BeforeWF(DBSource dbsource,WorkFlow wf, WorkStateBranch wsb)
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
                if ((((wf.DefWorkFlow.O_Code == "CONTACTFILE") || (wf.DefWorkFlow.O_Code == "ZCONTACTFILE") || (wf.DefWorkFlow.O_Code == "ZSENDFILE") ||
                    wf.DefWorkFlow.O_Code == "JSENDFILE" || wf.DefWorkFlow.O_Code == "ZRECEIVEFILE") 
                    && (wf.CuWorkState.Code == "START")) && !wf.CuWorkState.O_FinishDate.HasValue)
                {
                    DBSource dbsource = wf.dBSource;
                    User user = dbsource.LoginUser;
                    Doc doc = wf.DocList.Find(d => !d.Code.Contains("_附件"));
                    string format = "select DesignContent from User_GEDIUnitedWorkflowContent where ITEMNO={0} and UserName='{1}'";
                    string[] strArray = dbsource.DBExecuteSQL(string.Format(format, doc.ID, dbsource.LoginUser.Code));
                    //if (((strArray.Length == 1) && string.IsNullOrEmpty(strArray[0])) && (DialogResult.No == MessageBox.Show("您还未进行接口单的设计，请先设计接口单。是否仍继续提交流程？\r\n\r\n设计接口单，请通过文档右键-设计接口单...，如审查意见为“同意”也请在界面中进行操作。", "操作确认", MessageBoxButtons.YesNo)))
                    //{
                    //    throw new Exception("人为抛异常中止提交流程");
                    //}
                    User userByCode = null;
                    User user2 = null;
                    format = "select CheckName, AuditName from User_GEDIUnitedWorkflowContent where ITEMNO={0} and UserName='{1}'";
                    strArray = dbsource.DBExecuteSQL(string.Format(format, doc.ID, dbsource.LoginUser.Code));
                    if (strArray.Length == 2)
                    {
                        if (!string.IsNullOrEmpty(strArray[0]))
                        {
                            userByCode = dbsource.GetUserByCode(strArray[0]);
                        }
                        if (!string.IsNullOrEmpty(strArray[1]))
                        {
                            user2 = dbsource.GetUserByCode(strArray[1]);
                        }
                    }
                    if ((userByCode == null) || (user2 == null))
                    {
                        //fmSelectUser user3 = new fmSelectUser(wf)
                        //{
                        //    StartPosition = FormStartPosition.CenterScreen
                        //};
                        //if (user3.ShowDialog() == DialogResult.Abort)
                        //{
                        //    throw new Exception("TERMINATEWORKFLOW");
                        //}
                    }
                    else
                    {
                        AVEVA.CDMS.Server.Group group = new AVEVA.CDMS.Server.Group();
                        group.AddUser(userByCode);
                        if (wf.WorkStateList != null && wf.WorkStateList.Count > 0)
                        {
                            string strCheckStateCode = "";
                            string strAuditStateCode = "";
                            if (wf.DefWorkFlow.O_Code == "CONTACTFILE")
                            {
                                if (wf.WorkStateList[0].Description == "施工方校核" || wf.WorkStateList[0].Description == "施工方审核")
                                {
                                    strCheckStateCode = "CHECK";
                                    strAuditStateCode = "AUDIT";
                                }
                                else
                                {
                                    strCheckStateCode = "CHECK2";
                                    strAuditStateCode = "APPROV";
                                }
                            }
                            //SQU总包工程联系单
                            else if (wf.DefWorkFlow.O_Code == "ZCONTACTFILE")
                            {
                                string COMPANYTYPE = "";
                                if (wf.DocList[0].GetAttrDataByKeyWord("CD_COMPANYTYPE").ToString == "施工单位")
                                {
                                    strCheckStateCode = "CHECK";
                                    strAuditStateCode = "APPROV";
                                }
                                else {
                                    strCheckStateCode = "CHECK3";
                                    strAuditStateCode = "AUDIT3";
                                }
                            }
                            //总包发文流程
                            else if (wf.DefWorkFlow.O_Code == "ZSENDFILE")
                            { 
                                 strCheckStateCode = "CHECK";
                                 strAuditStateCode = "AUDIT";
                            }
                            //监理发文流程
                            else if (wf.DefWorkFlow.O_Code == "JSENDFILE")
                            { 
                                 strCheckStateCode = "CHECK";
                                 strAuditStateCode = "APPROV";
                            }
                            //总包收文流程
                            else if (wf.DefWorkFlow.O_Code == "ZRECEIVEFILE")
                            {
                                strCheckStateCode = "CHECK";
                                strAuditStateCode = "AUDIT";
                            }

                            WorkState state = wf.WorkStateList.Find(wsx => (wsx.Code == strCheckStateCode) && (wsx.CheckGroup.AllUserList.Count == 0));
                            if (state == null)
                            {
                                DefWorkState defWorkState = wf.DefWorkFlow.DefWorkStateList.Find(dwsx => dwsx.O_Code == strCheckStateCode);
                                state = wf.NewWorkState(defWorkState);
                                state.SaveSelectUser(group);
                                if (wf.DefWorkFlow.O_Code == "CONTACTFILE" )
                                {
                                    state.IsRuning = true;
                                }
                                else if (wf.DefWorkFlow.O_Code == "ZCONTACTFILE" || wf.DefWorkFlow.O_Code == "ZSENDFILE" || wf.DefWorkFlow.O_Code == "JSENDFILE" || wf.DefWorkFlow.O_Code == "ZRECEIVEFILE")
                                {
                                    state.IsRuning = false;
                                }
                                state.PreWorkState = wf.CuWorkState;
                                state.O_iuser5 = new int?(wf.CuWorkState.O_stateno);
                                state.Modify();
                            }
                            else
                            {
                                state.SaveSelectUser(group);
                                state.IsRuning = true;
                                state.PreWorkState = wf.CuWorkState;
                                state.O_iuser5 = new int?(wf.CuWorkState.O_stateno);
                                state.Modify();
                            }
                            AVEVA.CDMS.Server.Group group2 = new AVEVA.CDMS.Server.Group();
                            group2.AddUser(user2);
                            if (state != null)
                            {
                                DefWorkState state3 = wf.DefWorkFlow.DefWorkStateList.Find(dwsx => dwsx.O_Code == strAuditStateCode);
                                WorkState state4 = wf.NewWorkState(state3);
                                state4.SaveSelectUser(group2);
                                state4.PreWorkState = state;
                                state4.O_iuser5 = new int?(wf.CuWorkState.O_stateno);
                                state4.Modify();
                            }

                            ////如果是SQL总包联系单，把主送方文控找出来，并放到流程的SECRETARY角色里
                            //if (wf.DefWorkFlow.O_Code == "ZCONTACTFILE")
                            //{
                            //    AVEVA.CDMS.Server.Group groupSec = new AVEVA.CDMS.Server.Group();
                            //    groupSec.AddUser(user2);
                            //    if (state != null)
                            //    {
                            //        DefWorkState stateSecretary = wf.DefWorkFlow.DefWorkStateList.Find(dwsx => dwsx.O_Code == "SECRETARY");
                            //        WorkState wsSecretary = wf.NewWorkState(stateSecretary);
                            //        wsSecretary.SaveSelectUser(group2);
                            //        wsSecretary.PreWorkState = state;
                            //        wsSecretary.O_iuser5 = new int?(wf.CuWorkState.O_stateno);
                            //        wsSecretary.Modify();
                            //    }
                            //}
                        }
                    }
                }
            }
            catch { throw; }
            reJo.success = true;
            return reJo;
        }
        //获取创建联系单默认表单信息
        public JObject GetCreateWinDefault(string sid, string action, string ProjectKeyword, string projectCode,string Unit,string Send,string Receive, string profession,string type)
        //public JObject GetCreateWinDefault(System.Collections.Specialized.NameValueCollection nvc)
        {
            //string sid="",action="", ProjectKeyword="", projectCode="",Unit="",Send,profession="", type="";
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                if (!string.IsNullOrEmpty(sid))
                {
                    User curUser = DBSourceController.GetCurrentUser(sid);

                    //登录用户
                    DBSource dbsource = curUser.dBSource;
                    if (dbsource != null)//登录并获取dbsource成功
                    {
                        JObject reJo = new JObject();
                        if (true || (action == "CreateD3" || action == "CreateA1" || action == "CreateA10" || action == "CreateA11" || action == "CreateA12" || action == "CreateA15"))
                        {
                            Project proj = dbsource.GetProjectByKeyWord(ProjectKeyword);
                            if (proj != null)
                            {
                                //获取参建单位代码
                                string dispatchCode = proj.GetValueByKeyWord("CONSTRUCTIONUNIT");
                                //获取文档流水号
                                string runNum = getDocNum(dbsource, projectCode, Unit, dispatchCode, Receive,profession, type);
                                //获取默认经手人
                                string auditorText = "", auditorValue = "";
                                string strAuditor = proj.GetValueByKeyWord("DEFAULTAUDITOR");
                                if (!string.IsNullOrEmpty(strAuditor))
                                {
                                    User Auditor = dbsource.GetUserByCode(strAuditor);
                                    
                                    if (Auditor != null)
                                    {
                                        auditorText = Auditor.ToString;
                                        auditorValue = Auditor.KeyWord;
                                    }
                                }
                                success = true;
                                jaResult.Add(new JObject 
                                    { 
                                        new JProperty("DispatchCode", dispatchCode),
                                        new JProperty("RunNum", runNum),
                                        new JProperty("checkText",curUser.ToString),
                                        new JProperty("checkValue",curUser.KeyWord),
                                        new JProperty("auditorText",auditorText),
                                        new JProperty("auditorValue",auditorValue),
                                    });
                                        
                            }
                        }
                    }
                }
            }
            catch { }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建D1 DOC
        public static JObject CreateD1 (string sid, string action, string ProjectKeyword, string docNum, string title, string checkList,
    string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc = title+"工程联系单";
                string strDocType = "D.1工程联系单";//ISO模板文件名
                string strTempDefn = "D1";//定义目录模板
                //string strTempDefn = "CONTACTDOC";//定义目录模板

                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }else
                {
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", title);//主题
                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容


                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
                    cp.checkerList = code;
                    cp.auditorList = str11;

                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建D2 DOC
        public static JObject CreateD2(string sid, string action, string ProjectKeyword, string docNum, string title, string checkList,
    string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc = title+"_"+"工程变更申请单";
                string strDocType = "D.2工程变更申请单";//ISO模板文件名
                string strTempDefn = "D2";//定义目录模板
                //string strTempDefn = "CONTACTDOC";//定义目录模板

                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else
                {
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", title);//主题
                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //A3数据-----------------------------------------------------------------------------------------------------------------------------
                    item.GetAttrDataByKeyWord("CD_TITLE").SetCodeDesc(title);

                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
                    cp.checkerList = code;
                    cp.auditorList = str11;

                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        public static JObject CreateD3(string sid, string action, string ProjectKeyword, string docNum, string title, string checkList,
            string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc = title+"_"+"工程联系单";
                string strDocType = "";


                string strTempDefn = "D3";//定义目录模板


                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else {
                    string unittype = cp.unitType;//
                    if (unittype == "总承包单位")
                    { strDocType = "D.3总承包工程联系单"; }
                    else
                    {
                        strDocType = "D.3工程联系单";
                    }
                    cp.strDocType = strDocType;

                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    //string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", title);//主题
                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容

                     bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
                    cp.checkerList = code;
                    cp.auditorList = str11;

                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建SQU工程联系单
        public static JObject CreateSQU(string sid, string action, string ProjectKeyword, string docNum, string title, string checkList,//string aduitList,
            string needreply, string property, string request, string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc = title + "_" + "工程联系单";
                string strDocType = "";


                string strTempDefn = "SQU";//定义目录模板


                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else
                {
                    string unittype = cp.unitType;//

                    if (unittype == "总承包单位")
                    {
                        strDocType = "SQU总包工程联系单";
                    }
                    else
                    {
                        strDocType = "SQU工程联系单";
                    }

                    cp.strDocType = strDocType;

                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    //string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string receive = docNumArray[3];//收文单位（主送方）
                    string profession = docNumArray[4];//专业
                    string docType = docNumArray[5];//文档类型
                    string runNum = docNumArray[6];//流水号

                    string cd_sender = "";
                    string cd_sendto = "";

                    string sQL = "select o_code,o_desc ,o_svalue4 from cdms_dictdata where o_datatype = 2 and o_skey = 'GEDICompany' and o_svalue3 like '%{1}%' order by o_code";
                    sQL = sQL.Replace("{1}", "FA06241");
                    string[] strArray = dbsource.DBExecuteSQL(sQL);
                    if (((strArray != null) && (strArray.Length > 0)) && (strArray[0] != ""))
                    {
                        for (int i = 0; i < strArray.Length; i += 3)
                        {
                            try
                            {
                                if (send == strArray[i])
                                {
                                    cd_sender = strArray[i + 2] ;
                                }

                                if (receive == strArray[i])
                                {
                                    cd_sendto = strArray[i + 2];
                                }
                            }
                            catch
                            {
                            }
                        }
                    }



                    string cd_request = request;

                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", title);//主题
                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    htUserKeyWord.Add("SENDER", cd_sender);//内容
                    htUserKeyWord.Add("SENDTO", cd_sendto);//内容
                    htUserKeyWord.Add("REQUEST", cd_request);//

                    if (needreply == "需回复")
                    {
                        //htUserKeyWord["CHECKRESULT_NEEDREPLY"] = "√";
                        htUserKeyWord["CHECKRESULT_NEEDREPLY"] = " 需回复☑   不需回复□";
                    }
                    else
                    {
                        //htUserKeyWord["CHECKRESULT_NONEEDREPLY"] = "√";
                        htUserKeyWord["CHECKRESULT_NEEDREPLY"] = " 需回复□   不需回复☑";
                    }

                    if (property == "通知")
                    {
                        //htUserKeyWord["CHECKRESULT_NOTIFY"] = "√";
                        htUserKeyWord["CHECKRESULT_PROPERTY"] = " 通知☑   建议□   备忘□   联系□   其它□";
                    }
                    else if (property == "建议")
                    {
                        //htUserKeyWord["CHECKRESULT_SUGGEST"] = "√";
                        htUserKeyWord["CHECKRESULT_PROPERTY"] = " 通知□   建议☑   备忘□   联系□   其它□";
                    }
                    else if (property == "备忘")
                    {
                        //htUserKeyWord["CHECKRESULT_NEEDREPLY"] = "√";
                        htUserKeyWord["CHECKRESULT_PROPERTY"] = " 通知□   建议□   备忘☑   联系□   其它□";
                    }
                    else if (property == "联系")
                    {
                        //htUserKeyWord["CHECKRESULT_NEEDREPLY"] = "√";
                        htUserKeyWord["CHECKRESULT_PROPERTY"] = " 通知□   建议□   备忘□   联系☑   其它□";
                    }
                    else if (property == "其它")
                    {
                        //htUserKeyWord["CHECKRESULT_NEEDREPLY"] = "√";
                        htUserKeyWord["CHECKRESULT_PROPERTY"] = " 通知□   建议□   备忘□   联系□   其它☑";
                    }


                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //SQU数据-----------------------------------------------------------------------------------------------------------------------------
                    item.GetAttrDataByKeyWord("CD_SENDER").SetCodeDesc(cd_sender);
                    item.GetAttrDataByKeyWord("CD_SENDTO").SetCodeDesc(cd_sendto);
                    item.GetAttrDataByKeyWord("CD_REQUEST").SetCodeDesc(cd_request);
                    item.GetAttrDataByKeyWord("CD_PROPERTY").SetCodeDesc(property);//性质
                    item.GetAttrDataByKeyWord("CD_NEEDREPLY").SetCodeDesc(needreply);//是否需要回复

                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, receive, profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    //string strAduitor = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    //if (!string.IsNullOrEmpty(aduitList))
                    //{
                    //    User aduitor = dbsource.GetUserByKeyWord(aduitList);
                    //    strAduitor = aduitor.Code;
                    //}
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
                    cp.checkerList = code;
                    cp.auditorList = str11;

                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建A1 DOC
        public static JObject CreateA1(string sid, string action, string ProjectKeyword, string docNum, string title, string toDate,string checkList,
            string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc = title+"工程开工报审表";
                string strDocType = "A.1工程开工报审表";//ISO模板文件名
                string strTempDefn = "A1";//定义目录模板
                

                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn,ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else
                {
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string unittype = cp.unitType;//
                    string codeDescStr=cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    DateTime dt = Convert.ToDateTime(toDate);
                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", title);//主题
                    htUserKeyWord.Add("TITLE2", dt.ToString("yyyy年MM月dd日"));//日期参数
                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //A1数据-----------------------------------------------------------------------------------------------------------------------------
                    item.GetAttrDataByKeyWord("CD_TITLE1").SetCodeDesc(title);
                    item.GetAttrDataByKeyWord("CD_TITLE2").SetCodeDesc(dt.ToString("yyyy-MM-dd"));

                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);
                    
                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker= dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));

                    cp.checkerList = code;
                    cp.auditorList = str11;

                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建A2 DOC
        public static JObject CreateA2(string sid, string action, string ProjectKeyword, string docNum, string title, string title2, string checkList,
    string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc = title2+"工程复工申请表";
                string strDocType = "A.2工程复工申请表";//ISO模板文件名
                string strTempDefn = "A2";//定义目录模板
                //string strTempDefn = "CONTACTDOC";//定义目录模板

                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else
                {
                    string unittype = cp.unitType;//
                    if (unittype == "总承包单位")
                    { strDocType = "A.2总包工程复工申请表"; }
                    else
                    {
                        strDocType = "A.2工程复工申请表";
                    }

                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;

                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", title);//主题
                    htUserKeyWord.Add("TITLE2", title2);//主题
                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //A3数据-----------------------------------------------------------------------------------------------------------------------------
                    item.GetAttrDataByKeyWord("CD_TITLE1").SetCodeDesc(title);
                    item.GetAttrDataByKeyWord("CD_TITLE2").SetCodeDesc(title2);

                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
                    cp.checkerList = code;
                    cp.auditorList = str11;

                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建A3 DOC
        public static JObject CreateA3(string sid, string action, string ProjectKeyword, string docNum, string title, string checkList,
    string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc = title+"施工组织设计报审表";
                string strDocType = "";//"A.3施工组织设计报审表";//ISO模板文件名

                string strTempDefn = "A3";//定义目录模板


                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else
                {
                    string unittype = cp.unitType;
                    if (unittype == "总承包单位")
                    { strDocType = "A.3总包施工组织设计报审表"; }
                    else
                    {
                        strDocType = "A.3施工组织设计报审表";
                    }
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    //string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", title);//主题
                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //A3数据-----------------------------------------------------------------------------------------------------------------------------
                    item.GetAttrDataByKeyWord("CD_TITLE1").SetCodeDesc(title);

                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
                    cp.checkerList = code;
                    cp.auditorList = str11;

                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建A4 DOC
        public static JObject CreateA4(string sid, string action, string ProjectKeyword, string docNum, string title, string checkList,
    string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc = title+"方案报审表";
                string strDocType = "A.4方案报审表";//ISO模板文件名
                string strTempDefn = "A4";//定义目录模板


                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else
                {
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", title);//主题
                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //A3数据-----------------------------------------------------------------------------------------------------------------------------
                    item.GetAttrDataByKeyWord("CD_TITLE1").SetCodeDesc(title);

                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
                    cp.checkerList = code;
                    cp.auditorList = str11;

                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建A5 DOC
        public static JObject CreateA5(string sid, string action, string ProjectKeyword, string docNum, string title, string checkList,
    string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc = title+"分包单位资格报审表";
                string strDocType = "A.5分包单位资格报审表";//ISO模板文件名
                string strTempDefn = "A5";//定义目录模板
                //string strTempDefn = "CONTACTDOC";//定义目录模板

                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else
                {
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", title);//主题
                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //A3数据-----------------------------------------------------------------------------------------------------------------------------
                    item.GetAttrDataByKeyWord("CD_TITLE").SetCodeDesc(title);

                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
                    cp.checkerList = code;
                    cp.auditorList = str11;

                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建A6 DOC
        public static JObject CreateA6(string sid, string action, string ProjectKeyword, string docNum, string title,string title2, string checkList,
    string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc = title2+"单位资质报审表";
                string strDocType = "A.6单位资质报审表";//ISO模板文件名
                string strTempDefn = "A6";//定义目录模板


                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else
                {
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", title);//主题
                    htUserKeyWord.Add("TITLE2", title2);//主题2
                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //A3数据-----------------------------------------------------------------------------------------------------------------------------
                    item.GetAttrDataByKeyWord("CD_TITLE1").SetCodeDesc(title);
                    item.GetAttrDataByKeyWord("CD_TITLE2").SetCodeDesc(title2);

                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
                    cp.checkerList = code;
                    cp.auditorList = str11;

                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建A7 DOC
        public static JObject CreateA7(string sid, string action, string ProjectKeyword, string docNum, string title, string checkList,
    string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc = "人员资质报审表";
                string strDocType = "A.7人员资质报审表";//ISO模板文件名
                string strTempDefn = "A7";//定义目录模板
                //string strTempDefn = "CONTACTDOC";//定义目录模板

                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else
                {
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号


                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    //htUserKeyWord.Add("TITLE", title);//主题
                    string str = "";
                    int index = 0;
                    JArray ja = (JArray)JsonConvert.DeserializeObject(content);
                    foreach (JObject joCont in ja)
                    {
                        string strName = joCont["name"].ToString();//姓 名
                        string strJobs = joCont["jobs"].ToString();//岗位/工种
                        string strCertiName = joCont["certiName"].ToString();//证件名称
                        string strCertiSerial = joCont["certiSerial"].ToString();//证件编号
                        string strIssueUnit = joCont["issueUnit"].ToString();//发证单位
                        string strExpiryDate = joCont["expiryDate"].ToString();//有效期


                        if (index == 0)
                        {
                            htUserKeyWord.Add("TITLE", strName); htUserKeyWord.Add("TITLE2", strJobs); htUserKeyWord.Add("TITLE3", strCertiName);
                            htUserKeyWord.Add("TITLE4", strCertiSerial); htUserKeyWord.Add("TITLE5", strIssueUnit); htUserKeyWord.Add("TITLE6", strExpiryDate);
                        }
                        else if (index == 1)
                        {
                            htUserKeyWord.Add("TITLE7", strName); htUserKeyWord.Add("TITLE8", strJobs); htUserKeyWord.Add("TITLE9", strCertiName);
                            htUserKeyWord.Add("TITLE10", strCertiSerial); htUserKeyWord.Add("TITLE11", strIssueUnit); htUserKeyWord.Add("TITLE12", strExpiryDate);
                        }
                        else if (index == 2)
                        {
                            htUserKeyWord.Add("TITLE13", strName); htUserKeyWord.Add("TITLE14", strJobs); htUserKeyWord.Add("TITLE15", strCertiName);
                            htUserKeyWord.Add("TITLE16", strCertiSerial); htUserKeyWord.Add("TITLE17", strIssueUnit); htUserKeyWord.Add("TITLE18", strExpiryDate);
                        }
                        else if (index == 3)
                        {
                            htUserKeyWord.Add("TITLE19", strName); htUserKeyWord.Add("TITLE20", strJobs); htUserKeyWord.Add("TITLE21", strCertiName);
                            htUserKeyWord.Add("TITLE22", strCertiSerial); htUserKeyWord.Add("TITLE23", strIssueUnit); htUserKeyWord.Add("TITLE24", strExpiryDate);
                        }
                        else if (index == 4)
                        {
                            htUserKeyWord.Add("TITLE25", strName); htUserKeyWord.Add("TITLE26", strJobs); htUserKeyWord.Add("TITLE27", strCertiName);
                            htUserKeyWord.Add("TITLE28", strCertiSerial); htUserKeyWord.Add("TITLE29", strIssueUnit); htUserKeyWord.Add("TITLE30", strExpiryDate);
                        }
                        index = index + 1;
                    }

                    //htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //A7数据-----------------------------------------------------------------------------------------------------------------------------
                    //item.GetAttrDataByKeyWord("CD_TITLE").SetCodeDesc(title);

                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
                    cp.checkerList = code;
                    cp.auditorList = str11;

                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建A8 DOC
        public static JObject CreateA8(string sid, string action, string ProjectKeyword, string docNum, string title, string checkList,
    string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc = title+"工程控制网测量／线路复测报审表";
                string strDocType = "A.8工程控制网测量／线路复测报审表";//ISO模板文件名
                string strTempDefn = "A8";//定义目录模板
                //string strTempDefn = "CONTACTDOC";//定义目录模板

                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else
                {
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", title);//主题
                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //A3数据-----------------------------------------------------------------------------------------------------------------------------
                    item.GetAttrDataByKeyWord("CD_TITLE").SetCodeDesc(title);

                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));

                    cp.checkerList = code;
                    cp.auditorList = str11;

                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建A9 DOC
        public static JObject CreateA9(string sid, string action, string ProjectKeyword, string docNum, string title, string checkList,
    string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc = "主要施工机械／工器具／安全用具报审表";
                string strDocType = "A.9主要施工机械／工器具／安全用具报审表";//ISO模板文件名
                string strTempDefn = "A9";//定义目录模板
                //string strTempDefn = "CONTACTDOC";//定义目录模板

                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else
                {
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号


                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    //htUserKeyWord.Add("TITLE", title);//主题
                    string str = "";
                    int index = 0;
                    JArray ja = (JArray)JsonConvert.DeserializeObject(content);
                    foreach (JObject joCont in ja)
                    {
                        string strName = joCont["name"].ToString();
                        string strNumber = joCont["number"].ToString();
                        string strCheckNum = joCont["checkNum"].ToString();
                        string strCompany = joCont["company"].ToString();
                        string strCheckDate = joCont["checkDate"].ToString();
                        if (index == 0)
                        {
                            htUserKeyWord.Add("TITLE", strName); htUserKeyWord.Add("TITLE2", strNumber); htUserKeyWord.Add("TITLE3", strCheckNum);
                            htUserKeyWord.Add("TITLE4", strCompany); htUserKeyWord.Add("TITLE5", strCheckDate);
                        }
                        else if (index == 1)
                        {
                            htUserKeyWord.Add("TITLE6", strName); htUserKeyWord.Add("TITLE7", strNumber); htUserKeyWord.Add("TITLE8", strCheckNum);
                            htUserKeyWord.Add("TITLE9", strCompany); htUserKeyWord.Add("TITLE10", strCheckDate);
                        }
                        else if (index == 2)
                        {
                            htUserKeyWord.Add("TITLE11", strName); htUserKeyWord.Add("TITLE12", strNumber); htUserKeyWord.Add("TITLE13", strCheckNum);
                            htUserKeyWord.Add("TITLE14", strCompany); htUserKeyWord.Add("TITLE15", strCheckDate);
                        }
                        else if (index == 3)
                        {
                            htUserKeyWord.Add("TITLE16", strName); htUserKeyWord.Add("TITLE17", strNumber); htUserKeyWord.Add("TITLE18", strCheckNum);
                            htUserKeyWord.Add("TITLE19", strCompany); htUserKeyWord.Add("TITLE20", strCheckDate);
                        }
                        else if (index == 4)
                        {
                            htUserKeyWord.Add("TITLE21", strName); htUserKeyWord.Add("TITLE22", strNumber); htUserKeyWord.Add("TITLE23", strCheckNum);
                            htUserKeyWord.Add("TITLE24", strCompany); htUserKeyWord.Add("TITLE25", strCheckDate);
                        }
                        else if (index == 5)
                        {
                            htUserKeyWord.Add("TITLE26", strName); htUserKeyWord.Add("TITLE27", strNumber); htUserKeyWord.Add("TITLE28", strCheckNum);
                            htUserKeyWord.Add("TITLE29", strCompany); htUserKeyWord.Add("TITLE30", strCheckDate);
                        }
                        else if (index == 6)
                        {
                            htUserKeyWord.Add("TITLE31", strName); htUserKeyWord.Add("TITLE32", strNumber); htUserKeyWord.Add("TITLE33", strCheckNum);
                            htUserKeyWord.Add("TITLE34", strCompany); htUserKeyWord.Add("TITLE35", strCheckDate);
                        }
                        else if (index == 7)
                        {
                            htUserKeyWord.Add("TITLE36", strName); htUserKeyWord.Add("TITLE37", strNumber); htUserKeyWord.Add("TITLE38", strCheckNum);
                            htUserKeyWord.Add("TITLE39", strCompany); htUserKeyWord.Add("TITLE40", strCheckDate);
                        }
                        index = index + 1;
                    }

                    //htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //A9数据-----------------------------------------------------------------------------------------------------------------------------
                    //item.GetAttrDataByKeyWord("CD_TITLE").SetCodeDesc(title);

                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
                    cp.checkerList = code;
                    cp.auditorList = str11;

                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建A10 DOC
        public static JObject CreateA10(string sid, string action, string ProjectKeyword, string docNum, string title, string checkList,
    string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc = "主要测量计量器具／试验设备检验报审表";
                string strDocType = "A.10主要测量计量器具／试验设备检验报审表";//ISO模板文件名
                string strTempDefn = "A10";//定义目录模板
                //string strTempDefn = "CONTACTDOC";//定义目录模板

                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else
                {
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号
                    

                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    //htUserKeyWord.Add("TITLE", title);//主题
                    string str="";
                    int index = 0;
                    JArray ja = (JArray)JsonConvert.DeserializeObject(content);
                    foreach (JObject joCont in ja)
                    {
                        string strName=joCont["name"].ToString();
                        string strNumber=joCont["number"].ToString();
                        string strCheckNum = joCont["checkNum"].ToString();
                        string strCompany = joCont["company"].ToString();
                        string strCheckDate = joCont["checkDate"].ToString();
                        if (index == 0)
                        {
                            htUserKeyWord.Add("TITLE", strName); htUserKeyWord.Add("TITLE2", strNumber); htUserKeyWord.Add("TITLE3", strCheckNum);
                            htUserKeyWord.Add("TITLE4", strCompany); htUserKeyWord.Add("TITLE5", strCheckDate);
                        }
                        else if (index == 1)
                        {
                            htUserKeyWord.Add("TITLE6", strName); htUserKeyWord.Add("TITLE7", strNumber); htUserKeyWord.Add("TITLE8", strCheckNum);
                            htUserKeyWord.Add("TITLE9", strCompany); htUserKeyWord.Add("TITLE10", strCheckDate);
                        }
                        else if (index == 2)
                        {
                            htUserKeyWord.Add("TITLE11", strName); htUserKeyWord.Add("TITLE12", strNumber); htUserKeyWord.Add("TITLE13", strCheckNum);
                            htUserKeyWord.Add("TITLE14", strCompany); htUserKeyWord.Add("TITLE15", strCheckDate);
                        }
                        else if (index == 3)
                        {
                            htUserKeyWord.Add("TITLE16", strName); htUserKeyWord.Add("TITLE17", strNumber); htUserKeyWord.Add("TITLE18", strCheckNum);
                            htUserKeyWord.Add("TITLE19", strCompany); htUserKeyWord.Add("TITLE20", strCheckDate);
                        }
                        else if (index == 4)
                        {
                            htUserKeyWord.Add("TITLE21", strName); htUserKeyWord.Add("TITLE22", strNumber); htUserKeyWord.Add("TITLE23", strCheckNum);
                            htUserKeyWord.Add("TITLE24", strCompany); htUserKeyWord.Add("TITLE25", strCheckDate);
                        }
                        else if (index == 5)
                        {
                            htUserKeyWord.Add("TITLE26", strName); htUserKeyWord.Add("TITLE27", strNumber); htUserKeyWord.Add("TITLE28", strCheckNum);
                            htUserKeyWord.Add("TITLE29", strCompany); htUserKeyWord.Add("TITLE30", strCheckDate);
                        }
                        else if (index == 6)
                        {
                            htUserKeyWord.Add("TITLE31", strName); htUserKeyWord.Add("TITLE32", strNumber); htUserKeyWord.Add("TITLE33", strCheckNum);
                            htUserKeyWord.Add("TITLE34", strCompany); htUserKeyWord.Add("TITLE35", strCheckDate);
                        }
                        else if (index == 7)
                        {
                            htUserKeyWord.Add("TITLE36", strName); htUserKeyWord.Add("TITLE37", strNumber); htUserKeyWord.Add("TITLE38", strCheckNum);
                            htUserKeyWord.Add("TITLE39", strCompany); htUserKeyWord.Add("TITLE40", strCheckDate);
                        }
                        index = index + 1;
                    }

                    //htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //A10数据-----------------------------------------------------------------------------------------------------------------------------
                    //item.GetAttrDataByKeyWord("CD_TITLE").SetCodeDesc(title);

                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
                    cp.checkerList = code;
                    cp.auditorList = str11;

                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建A11 DOC
        public static JObject CreateA11(string sid, string action, string ProjectKeyword, string docNum, string title, string checkList,
    string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                //string strDocDesc = "施工组织设计报审表";
                //string strDocType = "A.3施工组织设计报审表";//ISO模板文件名
                string strDocDesc = title+"工程"+"质量验收及评定项目划分报审表";
                string strDocType = "A.11质量验收及评定项目划分报审表";
                string strTempDefn = "A11";//定义目录模板


                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else
                {
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", title);//主题
                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //A3数据-----------------------------------------------------------------------------------------------------------------------------
                    item.GetAttrDataByKeyWord("CD_TITLE1").SetCodeDesc(title);

                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
                    cp.checkerList = code;
                    cp.auditorList = str11;

                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建A12 DOC
        public static JObject CreateA12(string sid, string action, string ProjectKeyword, string docNum, string title,string toDate, string checkList,
    string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc = title+"工程材料／构配件／设备报审表";
                string strDocType = "A.12工程材料／构配件／设备报审表";
                string strTempDefn = "A12";//定义目录模板


                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else
                {
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    DateTime dt = Convert.ToDateTime(toDate);

                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", dt.ToString("yyyy年MM月dd日"));//日期参数
                    htUserKeyWord.Add("TITLE2", title);//主题
                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //A3数据-----------------------------------------------------------------------------------------------------------------------------
                    item.GetAttrDataByKeyWord("CD_TITLE1").SetCodeDesc(dt.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_TITLE2").SetCodeDesc(title);

                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
                    cp.checkerList = code;
                    cp.auditorList = str11;

                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建A13 DOC
        public static JObject CreateA13(string sid, string action, string ProjectKeyword, string docNum, string title, string title2, string checkList,
    string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                //string strDocDesc = "施工组织设计报审表";
                //string strDocType = "A.3施工组织设计报审表";//ISO模板文件名
                string strDocDesc = "主要设备开箱申请表";
                string strDocType = "A.13主要设备开箱申请表";
                string strTempDefn = "A13";//定义目录模板


                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else
                {
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", title);//主题
                    htUserKeyWord.Add("TITLE2", title2);//主题
                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //A13数据-----------------------------------------------------------------------------------------------------------------------------
                    item.GetAttrDataByKeyWord("CD_TITLE1").SetCodeDesc(title);
                    item.GetAttrDataByKeyWord("CD_TITLE2").SetCodeDesc(title2);

                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
                    cp.checkerList = code;
                    cp.auditorList = str11;

                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建A14 DOC
        public static JObject CreateA14(string sid, string action, string ProjectKeyword, string docNum, string title, string checkList,
    string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                //string strDocDesc = "施工组织设计报审表";
                //string strDocType = "A.3施工组织设计报审表";//ISO模板文件名
                string strDocDesc = title+"工程"+"验收申请表";
                string strDocType = "A.14验收申请表";
                string strTempDefn = "A14";//定义目录模板


                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else
                {
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", title);//主题

                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //A13数据-----------------------------------------------------------------------------------------------------------------------------
                    item.GetAttrDataByKeyWord("CD_TITLE1").SetCodeDesc(title);


                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
                    cp.checkerList = code;
                    cp.auditorList = str11;

                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建A15 DOC
        public static JObject CreateA15(string sid, string action, string ProjectKeyword, string docNum, string title, string title2, string checkList,
    string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc = title+"工程"+"中间交付验收交接表";
                string strDocType = "A.15中间交付验收交接表";
                string strTempDefn = "A15";//定义目录模板


                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else
                {
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", title);//主题
                    htUserKeyWord.Add("TITLE2", title2);//主题
                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //A3数据-----------------------------------------------------------------------------------------------------------------------------
                    item.GetAttrDataByKeyWord("CD_TITLE1").SetCodeDesc(title);
                    item.GetAttrDataByKeyWord("CD_TITLE2").SetCodeDesc(title2);

                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
                    cp.checkerList = code;
                    cp.auditorList = str11;

                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建A16 DOC
        public static JObject CreateA16(string sid, string action, string ProjectKeyword, string docNum, string title, string title2, string checkList,
    string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc = title2+"计划／调整计划报审表";
                string strDocType = "";

                string strTempDefn = "A16";//定义目录模板


                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else
                {

                    string unittype = cp.unitType;//
                    if (unittype == "总承包单位")
                    { strDocType = "A.16总承包计划／调整计划报审表"; }
                    else
                    {
                        strDocType = "A.16计划／调整计划报审表";
                    }
                    cp.strDocType = strDocType;

                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", title);//主题
                    htUserKeyWord.Add("TITLE2", title2);//主题
                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //A3数据-----------------------------------------------------------------------------------------------------------------------------
                    item.GetAttrDataByKeyWord("CD_TITLE1").SetCodeDesc(title);
                    item.GetAttrDataByKeyWord("CD_TITLE2").SetCodeDesc(title2);

                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
                    cp.checkerList = code;
                    cp.auditorList = str11;

                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建A17 DOC
        public static JObject CreateA17(string sid, string action, string ProjectKeyword, string docNum, string title, string title2, string title3, string checkList,
    string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                //string strDocDesc = "施工组织设计报审表";
                //string strDocType = "A.3施工组织设计报审表";//ISO模板文件名
                string strDocDesc = title2+"费用索赔申请表";
                string strDocType = "A.17费用索赔申请表";
                string strTempDefn = "A17";//定义目录模板


                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else
                {
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", title);//主题
                    htUserKeyWord.Add("TITLE2", title2);//主题
                    htUserKeyWord.Add("TITLE3", title3);//主题

                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //A13数据-----------------------------------------------------------------------------------------------------------------------------
                    item.GetAttrDataByKeyWord("CD_TITLE1").SetCodeDesc(title);
                    item.GetAttrDataByKeyWord("CD_TITLE2").SetCodeDesc(title2);
                    item.GetAttrDataByKeyWord("CD_TITLE3").SetCodeDesc(title3);

                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
                    cp.checkerList = code;
                    cp.auditorList = str11;

                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建A18 DOC
        public static JObject CreateA18(string sid, string action, string ProjectKeyword, string docNum, string title, string title2, string checkList,
    string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc = title+"监理工程师通知回复单";
                string strDocType = "A.18监理工程师通知回复单";
                string strTempDefn = "A18";//定义目录模板


                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else
                {
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", title);//主题
                    htUserKeyWord.Add("TITLE2", title2);//主题
                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //A3数据-----------------------------------------------------------------------------------------------------------------------------
                    item.GetAttrDataByKeyWord("CD_TITLE1").SetCodeDesc(title);
                    item.GetAttrDataByKeyWord("CD_TITLE2").SetCodeDesc(title2);

                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
                    cp.checkerList = code;
                    cp.auditorList = str11;

                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建A19 DOC
        public static JObject CreateA19(string sid, string action, string ProjectKeyword, string docNum, string title, string title2, string title3, 
            string title4, string fromDate, string toDate, string checkList,string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc = "工程款支付申请表";
                string strDocType = "A.19工程款支付申请表";//ISO模板文件名
                string strTempDefn = "A19";//定义目录模板


                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else
                {
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    DateTime todt = Convert.ToDateTime(toDate);
                    DateTime fromdt = Convert.ToDateTime(fromDate);
                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    //htUserKeyWord.Add("TITLE", title);//主题
                    htUserKeyWord.Add("TITLE", fromdt.ToString("yyyy年MM月dd日"));
                    htUserKeyWord.Add("TITLE2", todt.ToString("yyyy年MM月dd日"));
                    htUserKeyWord.Add("TITLE3", title);
                    htUserKeyWord.Add("TITLE4", title2);
                    htUserKeyWord.Add("TITLE5", title3);
                    htUserKeyWord.Add("TITLE6", title4);

                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //A19数据------------------------------------------------------------------------------------------------------
                    item.GetAttrDataByKeyWord("CD_TITLE1").SetCodeDesc(fromdt.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_TITLE2").SetCodeDesc(todt.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_TITLE3").SetCodeDesc(title);
                    item.GetAttrDataByKeyWord("CD_TITLE4").SetCodeDesc(title2);
                    item.GetAttrDataByKeyWord("CD_TITLE5").SetCodeDesc(title3);
                    item.GetAttrDataByKeyWord("CD_TITLE6").SetCodeDesc(title4);


                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
                    cp.checkerList = code;
                    cp.auditorList = str11;

                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建A20 DOC
        public static JObject CreateA20(string sid, string action, string ProjectKeyword, string docNum, string title, string title2, string toDate, 
            string toDate2, string checkList,string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc = title+"工期变更报审表";
                string strDocType = "A.20工期变更报审表";
                string strTempDefn = "A20";//定义目录模板


                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else
                {
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    DateTime dt = Convert.ToDateTime(toDate);
                    DateTime dt2 = Convert.ToDateTime(toDate2);

                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", title);//
                    htUserKeyWord.Add("TITLE2",dt.ToString("yyyy年MM月dd日"));//日期参数
                    htUserKeyWord.Add("TITLE3", title2);//
                    htUserKeyWord.Add("TITLE4", dt2.ToString("yyyy年MM月dd日"));//日期参数
                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //A3数据-----------------------------------------------------------------------------------------------------------------------------
                    item.GetAttrDataByKeyWord("CD_TITLE1").SetCodeDesc(title);
                    item.GetAttrDataByKeyWord("CD_TITLE2").SetCodeDesc(dt.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_TITLE3").SetCodeDesc(title2);
                    item.GetAttrDataByKeyWord("CD_TITLE4").SetCodeDesc(dt2.ToString("yyyy-MM-dd"));

                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
                    cp.checkerList = code;
                    cp.auditorList = str11;

                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建A21 DOC
        public static JObject CreateA21(string sid, string action, string ProjectKeyword, string docNum, string title, string title2, string checkList, 
            string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc = title2+"设备／材料／构配件缺陷通知单";
                string strDocType = "A.21设备／材料／构配件缺陷通知单";//ISO模板文件名
                string strTempDefn = "A21";//定义目录模板


                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else
                {
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", title);
                    htUserKeyWord.Add("TITLE2", title2);

                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //A21数据------------------------------------------------------------------------------------------------------
                    item.GetAttrDataByKeyWord("CD_TITLE1").SetCodeDesc(title);
                    item.GetAttrDataByKeyWord("CD_TITLE2").SetCodeDesc(title2);



                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
                    cp.checkerList = code;
                    cp.auditorList = str11;

                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建A22 DOC
        public static JObject CreateA22(string sid, string action, string ProjectKeyword, string docNum, string title, string checkList,
    string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc = "第"+title+"号"+"设备／材料／构配件缺陷处理报验表";
                string strDocType = "A.22设备／材料／构配件缺陷处理报验表";//ISO模板文件名
                string strTempDefn = "A22";//定义目录模板


                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else
                {
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", title);//主题
                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //A3数据-----------------------------------------------------------------------------------------------------------------------------
                    item.GetAttrDataByKeyWord("CD_TITLE1").SetCodeDesc(title);

                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
                    cp.checkerList = code;
                    cp.auditorList = str11;


                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }


        //创建A23 DOC
        public static JObject CreateA23(string sid, string action, string ProjectKeyword, string docNum, string title, string title2, string checkList,
    string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc = title+"单位工程验收申请表";
                string strDocType = "A.23单位工程验收申请表";//ISO模板文件名
                string strTempDefn = "A23";//定义目录模板
                //string strTempDefn = "CONTACTDOC";//定义目录模板

                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else
                {
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", title);//主题
                    htUserKeyWord.Add("TITLE2", title2);//主题
                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //A3数据-----------------------------------------------------------------------------------------------------------------------------
                    item.GetAttrDataByKeyWord("CD_TITLE").SetCodeDesc(title);
                    item.GetAttrDataByKeyWord("CD_TITLE2").SetCodeDesc(title2);

                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
                    cp.checkerList = code;
                    cp.auditorList = str11;

                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }


        //创建A24 DOC
        public static JObject CreateA24(string sid, string action, string ProjectKeyword, string docNum, string title, string checkList,
    string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc = title+"工程竣工报验单";
                string strDocType = "A.24工程竣工报验单";//ISO模板文件名
                //string strTempDefn = "A24";//定义目录模板
                string strTempDefn = "CONTACTDOC";//定义目录模板

                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else
                {
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", title);//主题
                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //A3数据-----------------------------------------------------------------------------------------------------------------------------
                    item.GetAttrDataByKeyWord("CD_TITLE").SetCodeDesc(title);

                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();

                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
                    cp.checkerList = code;
                    cp.auditorList = str11;

                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建A25 DOC
        public static JObject CreateA25(string sid, string action, string ProjectKeyword, string docNum, string title, string title2, string checkList,
    string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc =title2+ "设计变更／变更设计执行情况反馈单";
                string strDocType = "A.25设计变更／变更设计执行情况反馈单";//ISO模板文件名
                string strTempDefn = "A25";//定义目录模板

                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else
                {
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", title);//主题
                    htUserKeyWord.Add("TITLE2", title2);//主题
                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //A3数据-----------------------------------------------------------------------------------------------------------------------------
                    item.GetAttrDataByKeyWord("CD_TITLE1").SetCodeDesc(title);
                    item.GetAttrDataByKeyWord("CD_TITLE2").SetCodeDesc(title2);

                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
                    cp.checkerList = code;
                    cp.auditorList = str11;


                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建A26 DOC
        public static JObject CreateA26(string sid, string action, string ProjectKeyword, string docNum, string title, string title2, string checkList,
    string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc = title2+"大中型施工机械进场／出场申报表";
                string strDocType = "A.26大中型施工机械进场／出场申报表";//ISO模板文件名
                string strTempDefn = "A26";//定义目录模板

                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else
                {
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", title);//主题
                    htUserKeyWord.Add("TITLE2", title2);//主题
                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //A3数据-----------------------------------------------------------------------------------------------------------------------------
                    item.GetAttrDataByKeyWord("CD_TITLE1").SetCodeDesc(title);
                    item.GetAttrDataByKeyWord("CD_TITLE2").SetCodeDesc(title2);

                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
                    cp.checkerList = code;
                    cp.auditorList = str11;


                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }
        //创建A27 DOC
        public static JObject CreateA27(string sid, string action, string ProjectKeyword, string docNum, string title, string checkList,
    string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc = title+"作业指导书报审表";
                string strDocType = "A.27作业指导书报审表";//ISO模板文件名
                string strTempDefn = "A27";//定义目录模板

                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else
                {
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", title);//主题
                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //A3数据-----------------------------------------------------------------------------------------------------------------------------
                    item.GetAttrDataByKeyWord("CD_TITLE1").SetCodeDesc(title);

                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
                    cp.checkerList = code;
                    cp.auditorList = str11;


                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建A28 DOC
        public static JObject CreateA28(string sid, string action, string ProjectKeyword, string docNum, string title, string checkList,
    string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc = title+"工程沉降观测报审表";
                string strDocType = "A.28工程沉降观测报审表";//ISO模板文件名
                string strTempDefn = "A28";//定义目录模板
                //string strTempDefn = "CONTACTDOC";//定义目录模板

                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else
                {
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", title);//主题
                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //A3数据-----------------------------------------------------------------------------------------------------------------------------
                    item.GetAttrDataByKeyWord("CD_TITLE").SetCodeDesc(title);

                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
                    cp.checkerList = code;
                    cp.auditorList = str11;



                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建A29 DOC
        public static JObject CreateA29(string sid, string action, string ProjectKeyword, string docNum, string title, string title2, string title3, string title4,
            string checkList,string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc =title3+ "工程量签证单";
                string strDocType = "A.29工程量签证单";//ISO模板文件名
                string strTempDefn = "A29";//定义目录模板
                //string strTempDefn = "CONTACTDOC";//定义目录模板

                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else
                {
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", title);//主题
                    htUserKeyWord.Add("TITLE2", title2);
                    htUserKeyWord.Add("TITLE3", title3);
                    htUserKeyWord.Add("TITLE4", title4);
                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //A3数据-----------------------------------------------------------------------------------------------------------------------------
                    item.GetAttrDataByKeyWord("CD_TITLE1").SetCodeDesc(title);
                    item.GetAttrDataByKeyWord("CD_TITLE2").SetCodeDesc(title2);
                    item.GetAttrDataByKeyWord("CD_TITLE3").SetCodeDesc(title3);
                    item.GetAttrDataByKeyWord("CD_TITLE4").SetCodeDesc(title4);

                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "",code2="";
                    string str11 = "",str12="";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                        code2 = checker.ToString;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                        str12 = approver.ToString;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
                    cp.checkerList = code;
                    cp.auditorList = str11;


                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建A30 DOC
        public static JObject CreateA30(string sid, string action, string ProjectKeyword, string docNum, string title, string checkList,
    string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc = title+"总承包单位资质报审表";
                string strDocType = "A.30总承包单位资质报审表";//ISO模板文件名
                string strTempDefn = "A30";//定义目录模板
                //string strTempDefn = "CONTACTDOC";//定义目录模板

                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else
                {
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", title);//主题
                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //A3数据-----------------------------------------------------------------------------------------------------------------------------
                    item.GetAttrDataByKeyWord("CD_TITLE").SetCodeDesc(title);

                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
                    cp.checkerList = code;
                    cp.auditorList = str11;

                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建A31 DOC
        public static JObject CreateA31(string sid, string action, string ProjectKeyword, string docNum, string title, string checkList,
    string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc = title+"设计配合比报审表";
                string strDocType = "A.31设计配合比报审表";//ISO模板文件名
                string strTempDefn = "A31";//定义目录模板
                //string strTempDefn = "CONTACTDOC";//定义目录模板

                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else
                {
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", title);//主题
                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //A3数据-----------------------------------------------------------------------------------------------------------------------------
                    item.GetAttrDataByKeyWord("CD_TITLE").SetCodeDesc(title);

                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
                    cp.checkerList = code;
                    cp.auditorList = str11;

                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建A32 DOC
        public static JObject CreateA32(string sid, string action, string ProjectKeyword, string docNum, string title, string checkList,
    string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc = title+"强条实施计划／细则报审表";
                string strDocType = "A.32强条实施计划／细则报审表";//ISO模板文件名
                string strTempDefn = "A32";//定义目录模板
                //string strTempDefn = "CONTACTDOC";//定义目录模板

                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else
                {
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", title);//主题
                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //A3数据-----------------------------------------------------------------------------------------------------------------------------
                    item.GetAttrDataByKeyWord("CD_TITLE").SetCodeDesc(title);

                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
                    cp.checkerList = code;
                    cp.auditorList = str11;

                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建A33 DOC
        public static JObject CreateA33(string sid, string action, string ProjectKeyword, string docNum, string title, string title2, string title3, string title4,
            string checkList, string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc =title3+ "工程量签证单（业主合同）";
                string strDocType = "A.33工程量签证单（业主合同）";//ISO模板文件名
                string strTempDefn = "A33";//定义目录模板
                //string strTempDefn = "CONTACTDOC";//定义目录模板

                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else
                {
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", title);//主题
                    htUserKeyWord.Add("TITLE2", title2);
                    htUserKeyWord.Add("TITLE3", title3);
                    htUserKeyWord.Add("TITLE4", title4);
                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //A3数据-----------------------------------------------------------------------------------------------------------------------------
                    item.GetAttrDataByKeyWord("CD_TITLE1").SetCodeDesc(title);
                    item.GetAttrDataByKeyWord("CD_TITLE2").SetCodeDesc(title2);
                    item.GetAttrDataByKeyWord("CD_TITLE3").SetCodeDesc(title3);
                    item.GetAttrDataByKeyWord("CD_TITLE4").SetCodeDesc(title4);

                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
                    cp.checkerList = code;
                    cp.auditorList = str11;

                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建A34 DOC
        public static JObject CreateA34(string sid, string action, string ProjectKeyword, string docNum, string title, string title2, string title3, string title4,
            string checkList, string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc = title3+"工程量确认单（合同范围内）";
                string strDocType = "A.34工程量确认单（合同范围内）";//ISO模板文件名
                string strTempDefn = "A34";//定义目录模板
                //string strTempDefn = "CONTACTDOC";//定义目录模板

                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else
                {
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", title);//主题
                    htUserKeyWord.Add("TITLE2", title2);
                    htUserKeyWord.Add("TITLE3", title3);
                    htUserKeyWord.Add("TITLE4", title4);
                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //A3数据-----------------------------------------------------------------------------------------------------------------------------
                    item.GetAttrDataByKeyWord("CD_TITLE1").SetCodeDesc(title);
                    item.GetAttrDataByKeyWord("CD_TITLE2").SetCodeDesc(title2);
                    item.GetAttrDataByKeyWord("CD_TITLE3").SetCodeDesc(title3);
                    item.GetAttrDataByKeyWord("CD_TITLE4").SetCodeDesc(title4);

                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
                    cp.checkerList = code;
                    cp.auditorList = str11;

                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建A35 DOC
        public static JObject CreateA35(string sid, string action, string ProjectKeyword, string docNum, string title, string checkList,
    string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc = title+"承包单位资质报审表";
                string strDocType = "A.35承包单位资质报审表";//ISO模板文件名
                string strTempDefn = "A35";//定义目录模板
                //string strTempDefn = "CONTACTDOC";//定义目录模板

                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                else
                {
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", title);//主题
                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //A3数据-----------------------------------------------------------------------------------------------------------------------------
                    item.GetAttrDataByKeyWord("CD_TITLE").SetCodeDesc(title);

                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
                    cp.checkerList = code;
                    cp.auditorList = str11;

                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建B1 DOC
        public static JObject CreateB1(string sid, string action, string ProjectKeyword, string docNum, string title, string checkList, //string toDate,
            string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc = title + "监理工作联系单";
                string strDocType = "B.1监理工作联系单";//ISO模板文件名
                string strTempDefn = "B1";//定义目录模板


                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                {
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    //DateTime dt = Convert.ToDateTime(toDate);
                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", title);//主题
                   // htUserKeyWord.Add("TITLE2", dt.ToString("yyyy年MM月dd日"));//日期参数
                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //B1数据-----------------------------------------------------------------------------------------------------------------------------
                    //item.GetAttrDataByKeyWord("CD_TITLE1").SetCodeDesc(title);
                    //item.GetAttrDataByKeyWord("CD_TITLE2").SetCodeDesc(dt.ToString("yyyy-MM-dd"));

                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));

                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建B2 DOC
        public static JObject CreateB2(string sid, string action, string ProjectKeyword, string docNum, string title, string title2, string checkList, //string toDate,
            string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc = title + "监理工程师通知单";
                string strDocType = "B.2监理工程师通知单";//ISO模板文件名
                string strTempDefn = "B2";//定义目录模板


                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                {
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    //DateTime dt = Convert.ToDateTime(toDate);
                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", title);//主题
                    htUserKeyWord.Add("TITLE2", title2);//日期参数
                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //B2数据-----------------------------------------------------------------------------------------------------------------------------
                    item.GetAttrDataByKeyWord("CD_TITLE1").SetCodeDesc(title2);
                    //item.GetAttrDataByKeyWord("CD_TITLE2").SetCodeDesc(dt.ToString("yyyy-MM-dd"));

                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));

                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建B3 DOC
        public static JObject CreateB3(string sid, string action, string ProjectKeyword, string docNum, string title, string title2, string toDate, string checkList, //string toDate,
            string approvList, string content, string filelist, string IsUpEdition)
        {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                string strDocDesc = title + "工程暂停令";
                string strDocType = "B.3工程暂停令";//ISO模板文件名
                string strTempDefn = "B3";//定义目录模板


                CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, docNum, filelist, IsUpEdition);
                if (cp.success == false)
                {
                    msg = cp.msg;
                }
                {
                    cp.strDocType = strDocType;
                    Doc item = cp.itemDoc;//获取新建的文档
                    DBSource dbsource = item.dBSource;
                    string unittype = cp.unitType;//
                    string codeDescStr = cp.CodeDescStr;//附件描述属性

                    string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    //定义序号
                    //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
                    string projCode = docNumArray[0];//工程号
                    string unit = docNumArray[1];//机组
                    string send = docNumArray[2];//发文单位
                    string profession = docNumArray[3];//专业
                    string docType = docNumArray[4];//文档类型
                    string runNum = docNumArray[5];//流水号

                    DateTime dt = Convert.ToDateTime(toDate);
                    //向word文档传递参数
                    Hashtable htUserKeyWord = new Hashtable();
                    htUserKeyWord.Add("DOCNUMBER", docNum);
                    htUserKeyWord.Add("TITLE", title);//主题
                    htUserKeyWord.Add("TITLE2", dt.ToString("yyyy年MM月dd日"));//日期参数
                    htUserKeyWord.Add("TITLE3", title2);//日期参数

                    htUserKeyWord.Add("CONTENT", content);//内容
                    htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
                    //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

                    //设置工程联系单文档属性

                    item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
                    item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
                    item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
                    item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
                    item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
                    item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
                    item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
                    item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
                    //B3数据-----------------------------------------------------------------------------------------------------------------------------
                    item.GetAttrDataByKeyWord("CD_TITLE1").SetCodeDesc(title);
                    item.GetAttrDataByKeyWord("CD_TITLE2").SetCodeDesc(dt.ToString("yyyy-MM-dd"));
                    item.GetAttrDataByKeyWord("CD_TITLE3").SetCodeDesc(title2);

                    bool flag = item.AttrDataList.SaveData();
                    item.Modify();
                    List<Doc> m_DocList = new List<Doc>();
                    m_DocList.Add(item);

                    string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
                    format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
                    dbsource.DBExecuteSQL(format);

                    string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
                    string code = "";
                    string str11 = "";
                    if (!string.IsNullOrEmpty(checkList))
                    {
                        User checker = dbsource.GetUserByKeyWord(checkList);
                        code = checker.Code;
                    }
                    if (!string.IsNullOrEmpty(approvList))
                    {
                        User approver = dbsource.GetUserByKeyWord(approvList);
                        str11 = approver.Code;
                    }
                    dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));

                    saveDoc(cp, htUserKeyWord);
                    if (cp.success == true)
                    {
                        success = true;
                        jaResult.Add(cp.reJo);
                    }
                    else { msg = cp.msg; }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;

            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        //创建DOC
        public static CreatePara CreateContent(string sid, string action, string strDocDesc, string strTempDefn,
            string ProjectKeyword, string docNum, string filelist, string IsUpEdition)
        {

            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            Project projectByName = new Project();
            Doc item = new Doc();
            string unittype = "";//联系单的类型
            string codeDescStr = "";//获取附件属性描述
            string strDocList = "";//获取附件
            try
            {
                JObject reJo = new JObject();
                if (string.IsNullOrEmpty(sid))
                {
                    msg = "登录验证失败！请尝试重新登录！";
                    return null;
                }

                User curUser = DBSourceController.GetCurrentUser(sid);
                if (curUser == null)
                {
                    msg = "登录验证失败！请尝试重新登录！";
                    return null;
                }
                DBSource dbsource = curUser.dBSource;
                if (dbsource == null)//登录并获取dbsource成功
                {
                    msg = "登录验证失败！请尝试重新登录！";
                    return null;
                }



                Project proj = dbsource.GetProjectByKeyWord(ProjectKeyword);
                if (proj == null)
                {
                    msg = "错误的文档操作信息！指定的文档不存在！";
                }
                else
                {
                    string m_str5;
                    List<TempDefn> tempDefnByCode = dbsource.GetTempDefnByCode("EPCPROJECT");
                    TempDefn defn = (tempDefnByCode != null && tempDefnByCode.Count > 0) ? tempDefnByCode[0] : null;
                    TempDefn mTempDefn = null;
                    if (defn == null)
                    { msg = "项目目录模板错误！"; }
                    else
                    {

                        m_str5 = proj.GetValueByKeyWord("CONTACTTYPE");

                        if (strTempDefn != "")
                        {
                            mTempDefn = defn.GetTempDefnByCode(strTempDefn);
                        }
                        else
                        { mTempDefn = defn.GetTempDefnByCode("CONTACTDOC"); }

                        if (mTempDefn == null)
                        {
                            msg = "没有与其相关的模板管理，创建无法正常完成";
                        }
                        else
                        {
                            string mProjectName = docNum;

                            unittype = proj.GetAttrDataByKeyWord("CONSTRUCUNITTYPE").Code;

                            if (IsUpEdition != "true")//&& (action == "CreateD3"))
                            {
                                projectByName = proj.GetProjectByName(mProjectName);
                                bool hasRepeated = false;
                                if (projectByName == null)
                                {
                                    projectByName = proj.NewProject(mProjectName, strDocDesc);
                                }
                                else
                                {
                                    //查找源目录是否重复编码的接口
                                    foreach (Doc docItem in projectByName.DocList)
                                    {
                                        if (docItem.TempDefn != null && docItem.Code.IndexOf(docNum) >= 0)
                                        {
                                            
                                            hasRepeated = true;
                                            break;
                                        }
                                    }
                                }

                                if (hasRepeated == true)
                                {
                                    msg = "已存在重复编码的函件 " + docNum + " ！";
                                }
                                else
                                {
                                    if (projectByName == null)
                                    {
                                        msg = ("创建资料目录失败！");
                                    }
                                    else
                                    {
                                        //dbsource.ProgramRun = true;
                                        //m_Project = projectByName;
                                        //创建存储路径
                                        string destPath = projectByName.FullPath;
                                        if (!Directory.Exists(destPath))
                                        {
                                            Directory.CreateDirectory(destPath);
                                        }

                                        //添加文档附件
                                        //string[] strGetList = ProcessEnclosure(dbsource.LoginUser.Code, projectByName, mProjectName + " " + strDocDesc, filelist);
                                        //codeDescStr = strGetList[0];//获取附件属性描述
                                        //strDocList = strGetList[1];//文档keyword列表

                                        string itemName = "";
                                        IEnumerable<string> source = from docx in projectByName.DocList select docx.Code;
                                        itemName = mProjectName + " " + strDocDesc;
                                        if (source.Contains<string>(itemName))
                                        {
                                            for (int i = 1; i < 0x3e8; i++)
                                            {
                                                itemName = mProjectName + " " + strDocDesc + i.ToString();
                                                if (!source.Contains<string>(itemName))
                                                {
                                                    break;
                                                }
                                            }
                                        }

                                        item = projectByName.NewDoc(itemName + ".doc", itemName, "", mTempDefn);
                                        if (item == null)
                                        {
                                            msg = "新建" + strDocDesc + "出错！";
                                        }
                                        else
                                        {
                                            success = true;
                                        }

                                        //dbsource.ProgramRun = false;
                                    }

                                }
                            }
                        }
                    }


                }
            }

            catch (Exception e)
            {
                msg = e.Message;

            }
            CreatePara reCp = new CreatePara();
            reCp.success = success;//返回是否成功
            reCp.proj = projectByName;//返回新建的目录
            reCp.itemDoc = item;//返回新建的文档
            reCp.unitType = unittype;//获取联系单类型
            reCp.CodeDescStr = codeDescStr;////获取附件属性描述
            //reCp.strDocType = strDocType;//文档类型
            reCp.msg = msg;//返回错误消息
            reCp.strDoclist = strDocList;//获取附件
            //reCp.reJo = AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
            return reCp;
        }

        //线程锁 
        internal static Mutex muxConsole = new Mutex();
        public static CreatePara saveDoc(CreatePara cp, Hashtable htUserKeyWord)
        {
            Doc item = cp.itemDoc;
            cp.success = false;
            string str7 = cp.strDocType;

            //item.dBSource.ProgramRun = false;
            ////设置默认专工
            AttrData attrDataChecker = item.Project.ParentProject.GetAttrDataByKeyWord("DEFAULTCHECKER");
            if (attrDataChecker != null)
            {
                attrDataChecker.SetCodeDesc(cp.checkerList);
            }
            ////设置默认经手人
            AttrData attrDataAuditor = item.Project.ParentProject.GetAttrDataByKeyWord("DEFAULTAUDITOR");
            if (attrDataAuditor != null)
            {
                attrDataAuditor.SetCodeDesc(cp.auditorList);
            }

            item.Project.ParentProject.AttrDataList.SaveData();
            //item.dBSource.ProgramRun = false;

           //获取网站路径
            string sPath = System.Web.HttpContext.Current.Server.MapPath("/ISO/SHEPC/");


            //获取模板文件路径
            string modelFileName = sPath + cp.strDocType + ".doc";
            //获取即将生成的联系单文件路径
            string locFileName = item.FullPathFile;

            if (System.IO.File.Exists(modelFileName))
            {
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
                    office.WriteDataToDocument(item, locFileName, htUserKeyWord, htUserKeyWord);
                }
                catch { }
                finally
                {

                    //解锁
                    muxConsole.ReleaseMutex();
                }

                ////把参数写进数据库
                ////参数格式：KEYWORD@@@VALUE@@@@KEYWORD1@@@VALUE1 
                //string strUserKeyWord = "";
                //int indexItem = 0;
                //foreach (DictionaryEntry UserKeyWordItem in htUserKeyWord)
                //{
                //    indexItem = indexItem + 1;
                //    strUserKeyWord = strUserKeyWord + UserKeyWordItem.Key + "@@@" + UserKeyWordItem.Value;
                //    if (indexItem < (htUserKeyWord.Count))
                //    {
                //        strUserKeyWord = strUserKeyWord + "@@@@";
                //    }
                //}
                //item.O_iuser4 = 1;
                //item.O_suser5 = strUserKeyWord;
                

                FileInfo info = new FileInfo(locFileName);
                int length = (int)info.Length;
                item.O_size = new int?(length);
                item.Modify();
                if (string.IsNullOrEmpty(cp.strDoclist))
                {
                    cp.strDoclist = item.KeyWord;
                }
                else
                {
                    cp.strDoclist = item.KeyWord + "," + cp.strDoclist;
                }

                cp.success = true;
                cp.reJo=new JObject(new JProperty("ProjectKeyword", cp.proj.KeyWord),
new JProperty("DocKeyword", item.KeyWord), new JProperty("DocList", cp.strDoclist));


            }
            return cp;
        }


        public static JObject GetRunNum(string sid, string action, string ProjectCode, string Unit, string Send, string Receive, string Profession, string DocType)
        {            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                JObject reJo = new JObject();

                User curUser = DBSourceController.GetCurrentUser(sid);
                if (curUser == null)
                {
                    msg = "登录验证失败！请尝试重新登录！";
                    return null;
                }

                DBSource dbsource = curUser.dBSource;
                if (dbsource == null)
                {
                    msg = "登录验证失败！请尝试重新登录！";
                    return null;
                }
                if (string.IsNullOrEmpty(sid))
                { msg = "登录验证失败！请尝试重新登录！"; }
                else
                {

                    if (true || (action == "CreateD3" || action == "CreateA1" || action == "CreateA10" || action == "CreateA11" || action == "CreateA12" || action == "CreateA15"))
                    {
                        string runNum = getDocNum(dbsource, ProjectCode, Unit, Send, Receive, Profession, DocType);
                        if (string.IsNullOrEmpty(runNum)) runNum = "0001";
                        success = true;
                        jaResult.Add(new JObject(new JProperty("RunNum", runNum)));
                    }
                    else { msg = "错误的操作信息！"; }


                }
            }
            catch (Exception e)
            {
                msg = e.Message;
            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        /// <summary>
        /// 客户签名插件，调用方式：流程添加http客户插件，调用地址填：<AVEVA.CDMS.SHEPC_Plugins.EnterPoint><GEDIWebSign>
        /// </summary>
        /// <param name="wf"></param>
        public static void GEDIWebSign(WorkFlow wf) {
            try
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
                        //FTPFactory fTP = null;
                        //if (doc.Storage.FTP != null)
                        //{
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
                        //if (!string.IsNullOrEmpty(doc.FullPathFile))
                        //{
                        //    if (System.IO.File.Exists(wf.dBSource.LoginUser.WorkingPath + doc.O_filename))
                        //    {
                        //        FileInfo info = new FileInfo(wf.dBSource.LoginUser.WorkingPath + doc.O_filename);
                        //        if (info.IsReadOnly)
                        //        {
                        //            info.IsReadOnly = false;
                        //            info.Delete();
                        //        }
                        //    }
                        //    fTP.download(doc.FullPathFile, wf.dBSource.LoginUser.WorkingPath + doc.O_filename, true);
                        //}

                        //注意这里如果wf.CuWorkState不正确导致签不了名，需要查看插件的“跳转前执行”是否有打钩，有打钩的要去掉
                        string code = wf.CuWorkState.PreWorkState.Code;//获取当前流程对象（下一工作流对象的上一对象）
                        if ((doc.WorkFlow != null) && (doc.WorkFlow.O_WorkFlowStatus == enWorkFlowStatus.Finish))
                        {
                            code = "END";
                        }

                        switch (code)
                        {
                            //case "START":
                            //case "PREPAREDSIGN1":  //"CHECK":
                            case "CHECK":
                                //htUserKeyWord.Add("PREPAREDSIGN1", wf.dBSource.LoginUser.Code);
                                htUserKeyWord["PREPAREDSIGN1"] = PadName(wf.dBSource.LoginUser.Description);//wf.dBSource.LoginUser.Code;
                                htUserKeyWord["PREPAREDSIGNTIME"] = DateTime.Now.ToString("yyyy年MM月dd日");
                                goto Label_04DA;

                            //case "CHECK"://"AUDIT":
                            //    htUserKeyWord["CHECKPERSON"] = PadName(wf.dBSource.LoginUser.Description);// wf.dBSource.LoginUser.Code;
                            //    htUserKeyWord["CHECKTIME"] = DateTime.Now.ToString("yyyy年MM月dd日");
                            //    goto Label_04DA;

                            case "AUDIT"://"SECRETARY_P"://当流程是施工方审核发出
                                htUserKeyWord["AUDITPERSON"] = PadName(wf.dBSource.LoginUser.Description);// wf.dBSource.LoginUser.Code;
                                htUserKeyWord["AUDITTIME"] = DateTime.Now.ToString("yyyy年MM月dd日");

                                //填写意见
                                //GETWorkStateAudit(wf, "AUDIT");
                                goto Label_04DA;

                            case "CHECK2":  // "APPROV"://当流程是总包方校核
                                int i = 0;
                                foreach (WorkUser user2 in doc.WorkFlow.CuWorkState.PreWorkState.WorkUserList)
                                {
                                    i = i + 1;
                                    //if (wf.dBSource.LoginUser.Code == user2.User.Code)
                                    if (user2.User.Code == user2.User.Code)
                                    {
                                        htUserKeyWord["CHECK2PERSON" + i.ToString()] = user2.User.Description;// wf.dBSource.LoginUser.Code;
                                        htUserKeyWord["CHECK2TIME"] = DateTime.Now.ToString("yyyy年MM月dd日");

                                    }
                                }
                                //填写意见
                                //string aduitcheck2 = PadName(wf.dBSource.LoginUser.Description);// GETWorkStateAudit(wf, "CHECK2");
                                string aduitcheck2 = GETWorkStateAudit(wf, doc, "CHECK2");
                                htUserKeyWord["CHECK2AUDIT"] = aduitcheck2;

                                goto Label_04DA;

                            case "AUDIT2":  // "AUDIT2"://当流程是总包方审核
                                htUserKeyWord["AUDIT2PERSON"] = PadName(wf.dBSource.LoginUser.Description);// wf.dBSource.LoginUser.Code;
                                htUserKeyWord["AUDIT2TIME"] = DateTime.Now.ToString("yyyy年MM月dd日");
                                goto Label_04DA;

                            case "APPROV": //"SECRETARY_P2"://当流程是总包方批准
                                htUserKeyWord["APPROVPERSON"] = PadName(wf.dBSource.LoginUser.Description);// wf.dBSource.LoginUser.Code;
                                htUserKeyWord["APPROVTIME"] = DateTime.Now.ToString("yyyy年MM月dd日");

                                //填写意见
                                string aduit = GETWorkStateAudit(wf, doc, "APPROV");
                                if (aduit == "")
                                {
                                    aduit = GETWorkStateAudit(wf, doc, "CHECK2");
                                }
                                htUserKeyWord["APPROVAUDIT"] = aduit;

                                goto Label_04DA;

                            case "CHECK3": //"APPROV2"://当流程是监理方校核
                                i = 0;
                                foreach (WorkUser user2 in doc.WorkFlow.CuWorkState.PreWorkState.WorkUserList)
                                {
                                    i = i + 1;
                                    //if (wf.dBSource.LoginUser.Code == user2.User.Code)
                                    if (user2.User.Code == user2.User.Code)
                                    {
                                        htUserKeyWord["CHECK3PERSON" + i.ToString()] = user2.User.Description;// wf.dBSource.LoginUser.Code;
                                        htUserKeyWord["CHECK3TIME"] = DateTime.Now.ToString("yyyy年MM月dd日");

                                    }
                                }

                                //填写意见
                                string aduitcheck3 = GETWorkStateAudit(wf, doc, "CHECK3");
                                htUserKeyWord["CHECK3AUDIT"] = aduitcheck3;

                                goto Label_04DA;

                            case "AUDIT3": //"APPROV2"://当流程是监理方审核
                                htUserKeyWord["AUDIT3PERSON"] = PadName(wf.dBSource.LoginUser.Description);// wf.dBSource.LoginUser.Code;
                                htUserKeyWord["AUDIT3TIME"] = DateTime.Now.ToString("yyyy年MM月dd日");


                                //htUserKeyWord["Keyword1"] = PadName(wf.dBSource.LoginUser.Description);// wf.dBSource.LoginUser.Code;
                                //htUserKeyWord["Keyword2"] = PadName(wf.dBSource.LoginUser.Description);// wf.dBSource.LoginUser.Code;

                                //填写意见
                                string aduit3aduit = GETWorkStateAudit(wf, doc, "AUDIT3");
                                htUserKeyWord["AUDIT3AUDIT"] = aduit3aduit;

                                goto Label_04DA;

                            case "APPROV2": //"SECRETARY_P3"://当是监理方批准
                                htUserKeyWord["APPROV2PERSON"] = PadName(wf.dBSource.LoginUser.Description);// wf.dBSource.LoginUser.Code;
                                htUserKeyWord["APPROV2TIME"] = DateTime.Now.ToString("yyyy年MM月dd日");

                                //填写意见
                                string aduit2 = GETWorkStateAudit(wf, doc, "APPROV2");
                                if (aduit2 == "")
                                {
                                    aduit2 = GETWorkStateAudit(wf, doc, "CHECK3");
                                }
                                htUserKeyWord["APPROV2AUDIT"] = aduit2;

                                goto Label_04DA;

                            case "F_APPROV":
                                htUserKeyWord["F_APPROVPERSON"] = PadName(wf.dBSource.LoginUser.Description);// wf.dBSource.LoginUser.Code;
                                htUserKeyWord["F_APPROVTIME"] = DateTime.Now.ToString("yyyy年MM月dd日");

                                goto Label_04DA;

                            case "F_APPROV2":
                                 htUserKeyWord["F_APPROV2PERSON"] = PadName(wf.dBSource.LoginUser.Description);// wf.dBSource.LoginUser.Code;
                                 htUserKeyWord["F_APPROV2TIME"] = DateTime.Now.ToString("yyyy年MM月dd日");

                                goto Label_04DA;

                            case "AUDIT4":  // "APPROV3"://当流程是业主方审核
                                htUserKeyWord["AUDIT4PERSON"] = PadName(wf.dBSource.LoginUser.Description);// wf.dBSource.LoginUser.Code;
                                htUserKeyWord["AUDIT4TIME"] = DateTime.Now.ToString("yyyy年MM月dd日");
                                //填写意见
                                string aduit4 = GETWorkStateAudit(wf, doc, "AUDIT4");
                                htUserKeyWord["AUDIT4AUDIT"] = aduit4;
                                goto Label_04DA;

                            case "CHECK4": //"APPROV3"://当流程是业主方校核
                                i = 0;
                                foreach (WorkUser user2 in doc.WorkFlow.CuWorkState.PreWorkState.WorkUserList)
                                {
                                    i = i + 1;
                                    //if (wf.dBSource.LoginUser.Code == user2.User.Code)
                                    if (user2.User.Code == user2.User.Code)
                                    {
                                        htUserKeyWord["CHECK4PERSON" + i.ToString()] = user2.User.Description;// wf.dBSource.LoginUser.Code;
                                        htUserKeyWord["CHECK4TIME"] = DateTime.Now.ToString("yyyy年MM月dd日");

                                    }
                                }
                                //填写意见
                                string aduitcheck4 = GETWorkStateAudit(wf, doc, "CHECK4");
                                htUserKeyWord["CHECK4AUDIT"] = aduitcheck4;

                                goto Label_04DA;

                            case "APPROV3": //"SECRETARY_P4"://当流程是业主方批准
                                htUserKeyWord["APPROV3PERSON"] = PadName(wf.dBSource.LoginUser.Description);//wf.dBSource.LoginUser.Code;
                                htUserKeyWord["APPROV3TIME"] = DateTime.Now.ToString("yyyy年MM月dd日");

                                //如果是SQU总包联系单，总包发起且经过了副总经理再到总经理，就要把副总经理的名字签到第二个审核的位置
                                if (wf.DefWorkFlow.O_Code == "ZCONTACTFILE")
                                {
                                    WorkState state = wf.WorkStateList.Find(wsx => wsx.Code == "F_APPROV3");
                                    if (state != null)
                                    {
                                        User user = state.WorkUserList[0].User;
                                        htUserKeyWord["F_APPROV3PERSON_2"] = PadName(user.Description);
                                    }
                                    //wf.dBSource.LoginUser.Code;
                                }


                                //填写意见
                                string aduit3 = GETWorkStateAudit(wf, doc, "APPROV3");
                                if (aduit3 == "")
                                {
                                    aduit3 = GETWorkStateAudit(wf, doc, "AUDIT4");
                                }
                                htUserKeyWord["APPROV3AUDIT"] = aduit3;

                                goto Label_04DA;
                            case "APPROV4": 
                                htUserKeyWord["APPROV4PERSON"] = PadName(wf.dBSource.LoginUser.Description);
                                htUserKeyWord["APPROV4TIME"] = DateTime.Now.ToString("yyyy年MM月dd日");

                                //填写意见
                                string aduit6 = GETWorkStateAudit(wf, doc, "APPROV4");
                                htUserKeyWord["APPROV4AUDIT"] = aduit6;

                                goto Label_04DA;

                            case "F_APPROV3": //"SECRETARY_P4"://当流程是业主方批准
                                htUserKeyWord["F_APPROV3PERSON"] = PadName(wf.dBSource.LoginUser.Description);//wf.dBSource.LoginUser.Code;
                                htUserKeyWord["F_APPROV3TIME"] = DateTime.Now.ToString("yyyy年MM月dd日");


                                goto Label_04DA;
                            case "CHECK5": 
                                i = 0;
                                foreach (WorkUser user2 in doc.WorkFlow.CuWorkState.PreWorkState.WorkUserList)
                                {
                                    i = i + 1;
                                    //if (wf.dBSource.LoginUser.Code == user2.User.Code)
                                    if (user2.User.Code == user2.User.Code)
                                    {
                                        htUserKeyWord["CHECK5PERSON" + i.ToString()] = user2.User.Description;// wf.dBSource.LoginUser.Code;
                                        htUserKeyWord["CHECK5TIME"] = DateTime.Now.ToString("yyyy年MM月dd日");

                                    }
                                }
                                //填写意见
                                string aduitcheck5 = GETWorkStateAudit(wf, doc, "CHECK5");
                                htUserKeyWord["CHECK5AUDIT"] = aduitcheck5;

                                goto Label_04DA;

                            case "AUDIT5":  // "APPROV3"://当流程是业主方审核
                                htUserKeyWord["AUDIT5PERSON"] = PadName(wf.dBSource.LoginUser.Description);// wf.dBSource.LoginUser.Code;
                                htUserKeyWord["AUDIT5TIME"] = DateTime.Now.ToString("yyyy年MM月dd日");
                                //填写意见
                                string aduit5 = GETWorkStateAudit(wf, doc, "AUDIT5");
                                htUserKeyWord["AUDIT4AUDIT"] = aduit5;
                                goto Label_04DA;

                            case "CHECK6":
                                i = 0;
                                foreach (WorkUser user2 in doc.WorkFlow.CuWorkState.PreWorkState.WorkUserList)
                                {
                                    i = i + 1;
                                    //if (wf.dBSource.LoginUser.Code == user2.User.Code)
                                    if (user2.User.Code == user2.User.Code)
                                    {
                                        htUserKeyWord["CHECK6PERSON" + i.ToString()] = user2.User.Description;// wf.dBSource.LoginUser.Code;
                                        htUserKeyWord["CHECK6TIME"] = DateTime.Now.ToString("yyyy年MM月dd日");

                                    }
                                }
                                //填写意见
                                string aduitcheck6 = GETWorkStateAudit(wf, doc, "CHECK6");
                                htUserKeyWord["CHECK6AUDIT"] = aduitcheck6;

                                goto Label_04DA;

                            case "APPROV_":
                            case "SECTION":
                                {
                                    if ((doc.WorkFlow.CuWorkState.PreWorkState == null) || !(doc.WorkFlow.CuWorkState.PreWorkState.Code == "MORESIGN"))
                                    {
                                        break;
                                    }
                                    int num = 2;
                                    foreach (WorkUser user in doc.WorkFlow.CuWorkState.PreWorkState.WorkUserList)
                                    {
                                        htUserKeyWord["PREPAREDSIGN" + num] = user.User.Code;
                                        num++;
                                    }
                                    htUserKeyWord["MORESIGNIDEA"] = doc.WorkFlow.O_suser5;
                                    goto Label_0385;
                                }
                            //================================================================================
                            case "MORESIGN":

                                i = 0;
                                foreach (WorkUser user2 in doc.WorkFlow.CuWorkState.PreWorkState.WorkUserList)
                                {
                                    i = i + 1;
                                    //if (wf.dBSource.LoginUser.Code == user2.User.Code)
                                    if (user2.User.Code == user2.User.Code)
                                    {
                                        //htUserKeyWord["AUDITPERSON" + i.ToString()] = doc.WorkFlow.CuWorkState.PreWorkState.WorkUserList[0].User.Description;
                                        htUserKeyWord["AUDITPERSON" + i.ToString()] = user2.User.Description;
                                        htUserKeyWord["AUDITTIME" + i.ToString()] = DateTime.Now.ToString("yyyy.MM.dd");
                                    }
                                }
                                goto Label_04DA;

                            case "INTERFACE":
                            case "END":
                                {
                                    string str3 = doc.dBSource.ParseExpression(doc, "$(PROJECTOWNER)");
                                    if (wf.DefWorkFlow.O_Code == "HTWORKFLOW")
                                    {
                                        htUserKeyWord["APPROVEPERSON"] = wf.dBSource.LoginUser.Code;
                                    }
                                    else if (!string.IsNullOrEmpty(str3))
                                    {
                                        htUserKeyWord["APPROVEPERSON"] = str3;
                                        htUserKeyWord["APPROVETIME"] = DateTime.Now.ToString("yyyy.MM.dd");
                                    }
                                    goto Label_04DA;
                                }
                            default:
                                goto Label_04DA;
                        }
                        if (wf.CuWorkState.PreWorkState.Code == "CHECK")
                        {
                            htUserKeyWord["CHECKPERSON"] = wf.dBSource.LoginUser.Code;
                        }
                        else
                        {
                            htUserKeyWord["AUDITPERSON"] = wf.dBSource.LoginUser.Code;
                        }
                    Label_0385:
                        htUserKeyWord["AUDITTIME"] = DateTime.Now.ToString("yyyy.MM.dd");
                    Label_04DA:
                        if ((str.EndsWith(".DOC") || str.EndsWith(".DOCX")) || (str.EndsWith(".XLS") || str.EndsWith(".XLSX")))
                        {
                            CDMSWebOffice office = new CDMSWebOffice
                            {
                                CloseApp = true,
                                VisibleApp = false
                            };
                            office.Release(true);
                            if (doc.WorkFlow != null)
                            {
                                enWorkFlowStatus status1 = doc.WorkFlow.O_WorkFlowStatus;
                            }
                            string tmpFilePath="";
                            if (System.IO.File.Exists(doc.FullPathFile))
                            {
                                tmpFilePath = wf.dBSource.LoginUser.WorkingPath + doc.O_filename;
                                if (System.IO.File.Exists(tmpFilePath))
                                {
                                    System.IO.File.Delete(tmpFilePath);
                                }
                                System.IO.File.Copy(doc.FullPathFile, tmpFilePath);
                            }
                            office.WriteDataToDocument(doc, tmpFilePath, htUserKeyWord);
                            System.IO.File.Delete(doc.FullPathFile);
                            System.IO.File.Move(tmpFilePath, doc.FullPathFile);
                            //office.WriteDataToDocument(doc, doc.FullPathFile, htUserKeyWord);
                            if (((doc.WorkFlow != null) && (doc.WorkFlow.O_WorkFlowStatus == enWorkFlowStatus.Finish)) && (doc.GetValueByKeyWord("GEDI_INNERIFTYPE") != "提出资料"))
                            {
                                if (wf.DefWorkFlow.O_Code == "INTERFACEWORKFLOW")
                                {
                                    Doc doc2 = doc;
                                    DateTime minValue = DateTime.MinValue;
                                    if ((doc2.Project.DocList != null) && (doc2.Project.DocList.Count > 0))
                                    {
                                        doc2.Project.DocList.Sort(new Comparison<Doc>(CompareByCode));
                                        Doc doc3 = doc2.Project.DocList.Last<Doc>(dx => dx.TempDefn != null);
                                        if ((doc3 != null) && ((doc3.TempDefn.Code == "GEDI_TRANSFERINGFORM") || (doc3.TempDefn.Code == "GEDI_VIEWFORM")))
                                        {
                                            if ((doc3.WorkFlow != null) && (doc3.WorkFlow.O_WorkFlowStatus == enWorkFlowStatus.Finish))
                                            {
                                                minValue = doc3.WorkFlow.WorkStateList.Last<WorkState>().O_FinishDate.Value;
                                            }
                                        }
                                        else if ((doc3 != null) && (doc3.TempDefn.Code == "IMPORTINTERFACEFILE"))
                                        {
                                            minValue = doc3.O_credatetime;
                                        }
                                    }
                                    if ((doc2.Project.DocList == null) || (doc2.Project.DocList.Count == 0))
                                    {
                                        SetAttrData(doc2, "IFR_INTERVAL", "0");
                                    }
                                    else if (minValue == DateTime.MinValue)
                                    {
                                        SetAttrData(doc2, "IFR_INTERVAL", "");
                                    }
                                    else
                                    {
                                        TimeSpan span = (TimeSpan)(DateTime.Now.Date - minValue.Date);
                                        SetAttrData(doc2, "IFR_INTERVAL", span.Days.ToString());
                                    }
                                    doc.AttrDataList.SaveData();
                                }
                                //ErrorLog.WriteErrorLog("跳转到打包成pdf和分发邮件成功   接口文件编号是：" + doc.Code);
                                //RARWorkFlowAttachFile(doc);
                                if (wf.CuWorkState.Code == "INTERFACE")
                                {
                                    //WriteLog("接口工程师状态return");
                                    return;
                                }
                            }
                            //if (fTP != null)
                            //{
                            //    fTP.upload(wf.dBSource.LoginUser.WorkingPath + doc.O_filename, doc.FullPathFile);
                            //    fTP.close();
                            //}
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                CommonController.WebWriteLog(exception.Message);
                //ErrorLog.WriteErrorLog(exception.ToString());
                //AVEVA.CDMS.WebApi.EnterPointController.
            }
            //return true;

        }

        public static void CreatReceivedCTProjectZB(WorkFlow workflow)
        { CreatReceivedCTProject(workflow, "总承包单位"); }

        public static void CreatReceivedCTProjectJL(WorkFlow workflow)
        { CreatReceivedCTProject(workflow, "监理单位"); }

        public static void CreatReceivedCTProjectYZ(WorkFlow workflow)
        { CreatReceivedCTProject(workflow, "建设单位"); }

        public static void CreatReceivedCTProject(WorkFlow workflow, string unittype)
        {

            Project project = workflow.Project;
            workflow.dBSource.ProgramRun = false;
            try
            {

                Project projectByName2 = GetParentProjectByName(project, "FA06241");

                if (projectByName2 != null)
                {
                    ////根据属性关键字获取project
                    Project proj = GetProjectByAttrData(projectByName2, "CONSTRUCUNITTYPE", unittype);

                    CreateReceivedProject(workflow, proj);

                }

            }
            catch (Exception exception)
            {
                CommonController.WebWriteLog("生成收文资料相关目录出错，异常信息：" + exception.Message);
            }
            finally
            {
                workflow.dBSource.ProgramRun = false;
            }

        }

        //创建施工单位快捷方式
        public static void CreatReceivedCTProjectSG(WorkFlow workflow)
        {
            Project project = workflow.Project;

            User user=workflow.CuWorkState.WorkUserList[0].User;//.CuWorkUser.User;
            //获取项目根目录
            Project projectRoot = GetParentProjectByName(project, "FA06241");

            if (projectRoot != null)
            {
                //根据属性关键字获取project
                Project projectSG = projectRoot.GetProjectByName("SG");
                if (projectSG != null)
                {

                    foreach (Project proj in projectSG.ChildProjectList)
                    {
                        AttrData data6 = proj.GetAttrDataByKeyWord("PROJSECRETARY");
                        string userSecStr = data6.ToString;
                        if (userSecStr.IndexOf(user.Code) >= 0) {
                            CreateReceivedProject(workflow, proj);
                            break;
                        }
                    }
                }
            }
            
        }

        internal static bool CreateReceivedProject(WorkFlow workflow,Project proj) {
            Project sourceProject = workflow.Project;

            //获取联系单类别分类
            string contacttype = "";
            Project projContactType = CDMSFunction.GetParentProjectByTempDefn(sourceProject, "CONTACTTYPE");
            if (projContactType != null)
            {
                contacttype = projContactType.Code;
            }

            //获取发文单位代码
            string fromUnit = "";
            Project projUnit = CDMSFunction.GetParentProjectByTempDefn(sourceProject, "CONSTRUCTIONUNIT");
            if (projUnit != null)
            {
                fromUnit = projUnit.Code;
            }

            Project projByName = proj.GetProjectByName("S");
            if (projByName != null)
            {
                projByName = projByName.GetProjectByName("收发文");
                if (projByName != null)
                {
                    projByName = projByName.GetProjectByName("收文");
                    if (projByName != null)
                    {
                        Project projByNameFrom = null;
                        Project projByNameTo = null;
                        foreach (Project projItem in projByName.ChildProjectList)
                        {
                            string projcode = projItem.Code;
                            if (projcode.Substring(5, 4) == fromUnit)//发文单位代码"HBJK")
                            {
                                projByNameFrom = projItem.GetProjectByName(contacttype);//表格类别代码"DG01");
                                projByNameTo = projByNameFrom.NewProject(sourceProject.Code, sourceProject.Description);
                                break;
                            }
                        }
                        if (projByNameTo != null)
                        {
                            foreach (Doc doc in workflow.DocList)
                            {
                                projByNameTo.NewDoc(doc);
                            }
                        }
                    }

                }
            }
            return true;
        }

        //根据目录名查找父目录
        public static Project GetParentProjectByName(Project project, string code)
        {
            Project resultProject = project;
            try
            {
                while (resultProject.Code != code)
                {
                    resultProject = resultProject.ParentProject;
                }
            }
            catch (Exception exception)
            {
                resultProject = null;
            }
            return resultProject;
        }

        public static int CompareByCode(Doc x, Doc y)
        {
            CaseInsensitiveComparer comparer = new CaseInsensitiveComparer();
            return comparer.Compare(x.O_credatetime, y.O_credatetime);
        }

        //根据Project属性的关键字查找Project
        public static Project GetProjectByAttrData(Project project, string keyword, string code)
        {
            Project resultProject = null;
            try
            {
                if (project != null)
                {
                    foreach (Project proj in project.ChildProjectList)
                    {
                        AttrData data = proj.GetAttrDataByKeyWord(keyword);
                        if (data != null && data.Code == code)
                        {
                            resultProject = proj;
                            break;
                        }
                    }
                }
            }
            catch { }
            return resultProject;
        }

        //取流程意见（）
        public static String GETWorkStateAudit(WorkFlow wf, Doc doc, string code)
        {

            string audit = "";
            try
            {
                if (code == "CHECK2")
                {
                    audit = doc.GetAttrDataByKeyWord("CD_ZB_AUDIT").Code;
                }
                else if (code == "APPROV")
                {
                    audit = doc.GetAttrDataByKeyWord("CD_ZB_AUDIT2").Code;
                }
                else if (code == "CHECK3")
                {
                    audit = doc.GetAttrDataByKeyWord("CD_JL_AUDIT").Code;
                }
                else if (code == "APPROV2")
                {
                    audit = doc.GetAttrDataByKeyWord("CD_JL_AUDIT2").Code;
                }
                else if (code == "CHECK4")
                {
                    audit = doc.GetAttrDataByKeyWord("CD_YZ_AUDIT").Code;
                }
                else if (code == "APPROV3")
                {
                    audit = doc.GetAttrDataByKeyWord("CD_YZ_AUDIT2").Code;
                }

                if (audit == "")
                {
                    int? stateno = 0;
                    WorkState state = wf.WorkStateList.Find(wsx => wsx.Code == code);
                    //doc=wf.doc
                    stateno = state.O_stateno;//取工作状态ID
                    foreach (WorkAudit workaudit in wf.AllWorkAuditList)
                    {
                        int i = workaudit.O_WorkStateNo;
                        if (workaudit.O_WorkStateNo == stateno)
                        {
                            audit = workaudit.O_Problom;

                            if (code == "CHECK2")
                            {
                                doc.GetAttrDataByKeyWord("CD_ZB_AUDIT").SetCodeDesc(audit); doc.AttrDataList.SaveData();
                            }
                            else if (code == "APPROV")
                            {
                                doc.GetAttrDataByKeyWord("CD_ZB_AUDIT2").SetCodeDesc(audit); doc.AttrDataList.SaveData();
                            }
                            else if (code == "CHECK3")
                            {
                                doc.GetAttrDataByKeyWord("CD_JL_AUDIT").SetCodeDesc(audit); doc.AttrDataList.SaveData();
                            }
                            else if (code == "APPROV2")
                            {
                                doc.GetAttrDataByKeyWord("CD_JL_AUDIT2").SetCodeDesc(audit); doc.AttrDataList.SaveData();
                            }
                            else if (code == "CHECK4")
                            {
                                doc.GetAttrDataByKeyWord("CD_YZ_AUDIT").SetCodeDesc(audit); doc.AttrDataList.SaveData();
                            }
                            else if (code == "APPROV3")
                            {
                                doc.GetAttrDataByKeyWord("CD_YZ_AUDIT2").SetCodeDesc(audit); doc.AttrDataList.SaveData();
                            }
                        }
                    }
                }
            }
            catch { }
            return audit;

        }

        public static void SetAttrData(Doc doc, string attrKey, string value)
        {
            SetAttrData(doc, attrKey, value, "");
        }

        public static void SetAttrData(Project pro, string attrKey, string value)
        {
            SetAttrData(pro, attrKey, value, "");
        }

        public static void SetAttrData(Doc doc, string attrKey, string value, string desc)
        {
            try
            {
                if (doc != null)
                {
                    AttrData attrDataByKeyWord = doc.GetAttrDataByKeyWord(attrKey);
                    if (attrDataByKeyWord == null)
                    {
                        //AssistFun.PopUpPrompt("模板" + attrKey + "不存在!");
                    }
                    else
                    {
                        attrDataByKeyWord.SetCodeDesc(value, desc);
                    }
                }
            }
            catch (Exception exception)
            {
                //AssistFun.PopUpPrompt("At Yandingsoft.CDMS.Plugin:Class CommonFunction.SetAttrData(Project,string,string)\r\n" + exception.Message);
            }
        }

        public static void SetAttrData(Project pro, string attrKey, string value, string desc)
        {
            try
            {
                AttrData data;
                if ((pro != null) && ((data = pro.GetAttrDataByKeyWord(attrKey)) != null))
                {
                    data.SetCodeDesc(value, desc);
                }
            }
            catch (Exception exception)
            {
                //AssistFun.PopUpPrompt("At Yandingsoft.CDMS.Plugin:Class CommonFunction.SetAttrData(Project,string,string)\r\n" + exception.Message);
            }
        }

        public static JObject SendManualStartMsg(string sid, string action, string docKeyword) {
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                if (string.IsNullOrEmpty(sid))
                { msg = "登录验证失败！请尝试重新登录！"; }
                else
                {
                    User curUser = DBSourceController.GetCurrentUser(sid);

                    //登录用户
                    DBSource dbsource = curUser.dBSource;
                    if (dbsource == null)
                    { msg = "登录验证失败！请尝试重新登录！"; }
                    else//登录并获取dbsource成功
                    {
                        JObject reJo = new JObject();
                        if (action != "CreateD3")
                        { msg = "错误的操作信息！"; }
                        else
                        {
                            Doc doc = dbsource.GetDocByKeyWord(docKeyword);
                            if (doc == null)
                            { msg = "错误的参数，文档不存在！"; }
                            else
                            {
                                List<User> mtoUser = new List<User> {
                                    dbsource.LoginUser
                                };
                                dbsource.SendMessage(dbsource.GetUserByCode("admin"), mtoUser, null, "★★文档[" + doc.Code + "]需手动启动流程提醒", "您好，文档[" + doc.Code + "]的互提资料单流程您选择了手动启动，请知悉并及时手动启动流程。\r\n\r\n如果该文件已经启动流程，请忽略本消息并设为已读。在手动启动流程前，请保留本消息，作为提醒。\r\n\r\n温馨提示：您在手动启动流程时，请将主文件与附件全部选中再启动流程，以免流程结束没有附件！", null, new List<Doc> { doc }, null, null);
                                success = true;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;
            }
            return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }


        private static string[] ProcessEnclosure(string userCode, Project proj ,string fileNo,string filelist)
        {
            string str = "";
            string strKeywordList = "";
            try
            {
                if (proj == null)
                {
                    return new string[] { "", strKeywordList };
                }
                string[] fileArray = (string.IsNullOrEmpty(filelist) ? "" : filelist).Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (fileArray.Length > 0)
                {
                    //FTPFactory factory = this.m_Project.Storage.FTP ?? new FTPFactory(this.m_Project.Storage);
                    int num = 1;
                    string itemName = "";
                    //获取临时目录路径,保存临时上传，父project还没创建的文件
                    string temp = System.Environment.GetEnvironmentVariable("TEMP");
                    DirectoryInfo info = new DirectoryInfo(temp);
                    string sPath = info.FullName + "\\" + userCode + "\\";

                    //获取附件最终保存路径
                    string destPath = proj.FullPath;

                    foreach (string fileName in fileArray)
                    {
                        string sourcePath = sPath + fileName;//获取临时文件（源文件）路径
                        
                        str = str + fileName + "\r\n";
                        itemName = fileNo + "_附件";
                        IEnumerable<string> source = from docx in proj.DocList select docx.Code;
                        while (source.Contains<string>(itemName))
                        {
                            itemName= fileNo + "_附件" + ++num;
                        }
                        //string fileDestPath = destPath + itemName;//获取最终保存文件路径
                        Doc item = proj.NewDoc(itemName + fileName.Substring(fileName.LastIndexOf('.'), fileName.Length - fileName.LastIndexOf('.')), itemName, "", (TempDefn)null);
                        if (item != null)
                        {
                            //this.m_DocList.Add(item);
                            try
                            {
                                string fileDestPath = item.FullPathFile;
                                if (File.Exists(sourcePath))//如果存在已上传的附件，就添加附件
                                {
                                    FileInfo fileinfo = new FileInfo(sourcePath);
                                    item.O_size = new int?((int)fileinfo.Length);
                                    item.Modify();

                                    //删除原有同名文件
                                    if (System.IO.File.Exists(fileDestPath))
                                    {
                                        System.IO.File.Delete(fileDestPath);
                                    }

                                    //移动文件
                                    System.IO.File.Move(sourcePath, fileDestPath);

                                    if (string.IsNullOrEmpty(strKeywordList))
                                    {
                                        strKeywordList = item.KeyWord;
                                    }
                                    else
                                    {
                                        strKeywordList = strKeywordList + "," + item.KeyWord;
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }
                        item.TempDefn = null;
                        item.Modify();
                    }
                }
                if (str == string.Empty)
                {
                    str = "(无)";
                }
                return new string[] { str, strKeywordList };
            }
            catch (Exception exception)
            {
                return new string[] { str, strKeywordList };
            }
        }



        private static string PadName(string strName)
        {
            try
            {
                if (strName.Length == 2)
                {
                    strName = strName.Substring(0, 1) + "  " + strName.Substring(1, 1);
                }
            }
            catch
            {
            }
            return strName;
        }

        //获取文档流水号，参数：dbsource:数据源，projectCode:项目代码，Unit:机组，Send：发文单位，profession:专业，type:文档类型
        private static string getDocNum(DBSource dbsource, string projectCode, string Unit, string Send, string Receive, string profession, string type)
        {
            string result = "";

            string str = "";
            string format = "select top 1 FlowNo from User_AutoSendFlowNo where projectCode ='{0}' and Unit ='{1}' and Send ='{2}' and Receive ='{3}' and Profession = '{4}' and Type='{5}' and DocNo in (select o_itemno from CDMS_Doc where o_dmsstatus <>10) order by FlowNo DESC";
            format = string.Format(format, new object[] { projectCode, Unit, Send, Receive, profession, type });
            string[] strArray = dbsource.DBExecuteSQL(format);
            if ((strArray.Length > 0) && !string.IsNullOrEmpty(strArray[0]))
            {
                string str4 = (Convert.ToInt32(strArray[0]) + 1).ToString();
                int num = 4 - str4.Length;
                string str5 = "";
                for (int i = 0; i < num; i++)
                {
                    str5 = str5 + "0";
                }
                str = str5 + str4;
            }
            else
            {
                str = "0001";
            }
            //this.m_str6 = str;
            result = str;
            return result;
        }
    }

    public class CreatePara
    {

        private bool _success;//是否成功
        public bool success
        {
            get
            {
                return _success;
            }
            set
            {
                _success = value;
            }
        }

        private JObject _reJo;
        public JObject reJo
        {
            get
            {
                return _reJo;
            }
            set
            {
                _reJo = value;
            }
        }

        private Doc _itemDoc;//新创建的文档
        public Doc itemDoc
        {
            get
            {
                return _itemDoc;
            }
            set
            {
                _itemDoc = value;
            }
        }

        private Project _proj;//新创建的目录
        public Project proj
        {
            get
            {
                return _proj;
            }
            set
            {
                _proj = value;
            }
        }

        private string _msg;//返回的错误消息
        public string msg
        {
            get
            {
                return _msg;
            }
            set
            {
                _msg = value;
            }
        }

        private string _unitType;//联系单的类型
        public string unitType
        {
            get
            {
                return _unitType;
            }
            set
            {
                _unitType = value;
            }
        }

        private string _CodeDescStr;//获取附件属性描述
        public string CodeDescStr
        {
            get
            {
                return _CodeDescStr;
            }
            set
            {
                _CodeDescStr = value;
            }
        }


        private string _strDocType;//文档类型
        public string strDocType
        {
            get
            {
                return _strDocType;
            }
            set
            {
                _strDocType = value;
            }
        }


        private string _strDoclist;//附件列表
        public string strDoclist
        {
            get
            {
                return _strDoclist;
            }
            set
            {
                _strDoclist = value;
            }
        }

        private string _checkerList;//附件列表
        public string checkerList
        {
            get
            {
                return _checkerList;
            }
            set
            {
                _checkerList = value;
            }
        }

        private string _auditorList;//附件列表
        public string auditorList
        {
            get
            {
                return _auditorList;
            }
            set
            {
                _auditorList = value;
            }
        }

    }

}
