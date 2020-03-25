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

namespace AVEVA.CDMS.HXPC_Plugins
{
    public class SelectUser
    {

        /// <summary>
        /// 主设人选择校审核人员时，获取默认值
        /// </summary>
        /// <param name="sid">连接密钥</param>
        /// <param name="WorkFlowKeyword">流程关键字</param>
        /// <returns></returns>
        public static JObject GetSelectUserExDefault(string sid, string WorkFlowKeyword)
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

                //加载数据

                //1. 当前用户
                //User cuUser = this.m_workflow.dBSource.LoginUser;
                //Doc mDoc = dbsource.GetDocByKeyWord(DocKeyword);
                //if (mDoc == null)
                //{
                //    reJo.msg = "文档不存在！";
                //    return reJo.Value;
                //}

                Object obj = dbsource.GetObjectByKeyWord(WorkFlowKeyword);
                if (!(obj is WorkFlow))
                {
                    reJo.msg = "流程不存在！";
                    return reJo.Value;
                }

                WorkFlow m_workflow = (WorkFlow)obj;
                //if (m_workflow == null)
                //{
                //    reJo.msg = "流程不存在！";
                //    return reJo.Value;
                //}

                //2. 找到所有需要主任分配的流程

                JArray jaWorkFlowList = new JArray();
                List<WorkState> wsList = new List<WorkState>();
                foreach (WorkFlow wf in curUser.ProcessWorkFlowList)
                {

                    //正在运行的流程，并且在主任分配人的状态
                    if (wf.CuWorkState.IsRuning && wf.CuWorkState.DefWorkState.O_Code == "DIRECTORSELECT")
                    {
                        WorkState ws = wf.CuWorkState;
                        wsList.Add(ws);

                        //项目名称
                        string projectName = wf.doc != null ? wf.doc.GetValueByKeyWord("DESIGNPROJECT_CODE") + "__" + wf.doc.GetValueByKeyWord("DESIGNPROJECT_DESC") : "";

                     ///判断流程是否在project
                        //if (string.IsNullOrEmpty(projectName))
                        //{
                        //    projectName = wf.Project != null ? wf.Project.GetValueByKeyWord("DESIGNPROJECT_CODE") + "__" + wf.Project.GetValueByKeyWord("DESIGNPROJECT_DESC") : "";
                        //}

                        if (!string.IsNullOrEmpty(projectName))
                        {
                            //任务名称
                            string docName = wf.doc != null ? wf.doc.ToString : "";

                         ///判断流程是否在project
                            //if (string.IsNullOrEmpty(docName))
                            //{
                            //    docName = wf.Project != null ? wf.Project.ToString : "";
                            //}

                            //流程名称
                            string workflowName = wf.DefWorkFlow.O_Description;
                            string workflowCode = wf.DefWorkFlow.O_Code;


                            if (workflowCode == "EXCHANGEDOC" || workflowCode == "SENDDOC" || workflowCode == "WORKTASK") //内部提资 和 发文 和 工作任务
                            {
                                JObject joWorkFlowList = new JObject(
                                    new JProperty("workflowKeyword", wf.KeyWord),
                                    new JProperty("projectName", projectName),
                                    new JProperty("docName", docName),
                                    new JProperty("workflowName", workflowName)

                                        );
                                jaWorkFlowList.Add(joWorkFlowList);
                                //DataGridViewRow item = new DataGridViewRow() { Tag = ws };
                                //DataGridViewTextBoxCell projectNamecell = new DataGridViewTextBoxCell();
                                //projectNamecell.Value = projectName;
                                //item.Cells.Add(projectNamecell);

                                //DataGridViewTextBoxCell docNamecell = new DataGridViewTextBoxCell();
                                //docNamecell.Value = docName;
                                //item.Cells.Add(docNamecell);

                                //DataGridViewTextBoxCell workflowNamecell = new DataGridViewTextBoxCell();
                                //workflowNamecell.Value = workflowName;
                                //item.Cells.Add(workflowNamecell);

                                //dgSelectUser.Rows.Add(item);

                            }
                        }
                    }


                }

                //3. 找出本科室所有用户
                //3.1 筛选出科室主任是登录用户的用户组
                string cuUserCode = curUser.Code;
                IEnumerable<AVEVA.CDMS.Server.Group> filterIE = (from agl in m_workflow.dBSource.AllGroupList
                                                                 where (agl.Description == "主任" &&
                                                                     ((agl.AllUserList.Find(u => (u.Code == cuUserCode)) != null)))
                                                                 select agl);

                List<AVEVA.CDMS.Server.Group> groupList = filterIE.ToList();

                //3.2 筛选出科室的所有用户
                JArray jaUserList = new JArray();
                List<User> userList = new List<User>();
                foreach (AVEVA.CDMS.Server.Group group in groupList)
                {
                    foreach (User user in group.ParentGroup.AllUserList)
                    {
                        //用户名称
                        string userName = user.ToString;

                        //int index = this.dgUserList.Rows.Add();
                        //this.dgUserList.Rows[index].Tag = user;
                        //this.dgUserList.Rows[index].Cells[0].Value = userName;


                     ///统计任务
                        #region 统计任务
                        List<AVEVA.CDMS.Server.Task> taskList = user.RuningTaskList;
                        int ProjectOwnerCount = 0;//统计设总任务
                        int ProfessionOwnerCount = 0;//统计主设任务
                        int DesignerCount = 0;//统计设计任务
                        int SenderCount = 0;//统计发文任务
                        int CheckerCount = 0;//统计校核任务
                        int AuditorCount = 0;//统计校核任务
                        foreach (AVEVA.CDMS.Server.Task task in taskList)
                        {
                            string taskType = task.O_TaskType;
                            switch (taskType)
                            {
                                case "设总":
                                    ProjectOwnerCount = ProjectOwnerCount + 1;
                                    break;
                                case "主设":
                                    ProfessionOwnerCount = ProfessionOwnerCount + 1;
                                    break;
                                case "设计":
                                    DesignerCount = DesignerCount + 1;
                                    break;
                                case "发文":
                                    SenderCount = SenderCount + 1;
                                    break;
                                case "校核":
                                    CheckerCount = CheckerCount + 1;
                                    break;
                                case "审核":
                                    AuditorCount = AuditorCount + 1;
                                    break;
                            }
                        }
                        jaUserList.Add(new JObject(
                            new JProperty("userName", userName),
                            new JProperty("ProjectOwnerCount", ProjectOwnerCount == 0 ? "" : ProjectOwnerCount.ToString()),
                            new JProperty("ProfessionOwnerCount", ProfessionOwnerCount == 0 ? "" : ProfessionOwnerCount.ToString()),
                            new JProperty("DesignerCount", DesignerCount == 0 ? "" : DesignerCount.ToString()),
                            new JProperty("SenderCount", SenderCount == 0 ? "" : SenderCount.ToString()),
                            new JProperty("CheckerCount", CheckerCount == 0 ? "" : CheckerCount.ToString()),
                            new JProperty("AuditorCount", AuditorCount == 0 ? "" : AuditorCount.ToString())
                            ));
                        //if (ProjectOwnerCount != 0)
                        //{ }
                        //if (ProfessionOwnerCount != 0)
                        //{
                        //    this.dgUserList.Rows[index].Cells[1].Value = ProfessionOwnerCount.ToString();
                        //}
                        //if (DesignerCount != 0)
                        //{
                        //    this.dgUserList.Rows[index].Cells[2].Value = DesignerCount.ToString();
                        //}
                        //if (SenderCount != 0)
                        //{ }
                        //if (CheckerCount != 0)
                        //{
                        //    this.dgUserList.Rows[index].Cells[3].Value = CheckerCount.ToString();
                        //}
                        //if (AuditorCount != 0)
                        //{
                        //    this.dgUserList.Rows[index].Cells[4].Value = AuditorCount.ToString();
                        //}


                        #endregion

                    }
                }

                reJo.data = new JArray(new JObject(
                    new JProperty("WorkflowList", jaWorkFlowList),
                    new JProperty("UserList", jaUserList))
                    );
                reJo.success = true;
                return reJo.Value;

            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(reJo.msg);
            }

            return reJo.Value;
        }

        /// <summary>
        /// 响应主设人选择校审核人员表单的确定按钮
        /// </summary>
        /// <param name="sid">连接密钥</param>
        /// <param name="workflowAttrJson">流程参数字符串,详见下面例子</param>
        /// <returns>
        /// <para>workflowAttrJson例子：</para>
        /// <code>
        /// [{
        ///        workflowKeyword: dataItem.workflowKeyword,   //流程关键字
        ///        checker: dataItem.checker,   //校核人
        ///        auditor: dataItem.auditor    //审核人
        ///    },
        ///  {
        ///        workflowKeyword: dataItem.workflowKeyword,
        ///        checker: dataItem.checker,
        ///        auditor: dataItem.auditor
        ///    }
        ///  ]
        ///  </code>
        /// </returns>
        public static JObject SelectUserEx(string sid, string workflowAttrJson)
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

                string projectKeyword = "";

                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(workflowAttrJson);


                foreach (JObject joAttr in jaAttr)
                {

                    string workflowKeyword = joAttr["workflowKeyword"].ToString();
                    string userCHECKCode = joAttr["checker"].ToString();//校核人
                    string userAUDITCode = joAttr["auditor"].ToString();//审核人

                    Object obj = dbsource.GetObjectByKeyWord(workflowKeyword);
                    if (obj == null || !(obj is WorkFlow))
                    {
                        continue;
                    }

                    WorkFlow workflow = (WorkFlow)obj;

                    if (workflow.doc != null)
                    {
                        projectKeyword = workflow.doc.Project.KeyWord;
                    }
                    else if (workflow.Project != null)
                    {
                        projectKeyword = workflow.Project.KeyWord;
                    }

                    if (!string.IsNullOrEmpty(userCHECKCode) && !string.IsNullOrEmpty(userAUDITCode))
                    {


                        //设定校核人
                        AVEVA.CDMS.Server.Group group = new AVEVA.CDMS.Server.Group();
                        User userByName = dbsource.GetUserByName(userCHECKCode.Substring(0, userCHECKCode.IndexOf("__")));
                        if (userByName != null)
                        {
                            group.AddUser(userByName);
                        }


                        //选择校核人
                        WorkState state = workflow.WorkStateList.Find(wsx => (wsx.Code == "CHECK"));
                        if (state == null)
                        {
                            DefWorkState defWorkState = workflow.DefWorkFlow.DefWorkStateList.Find(dwsx => dwsx.O_Code == "CHECK");
                            state = workflow.NewWorkState(defWorkState);

                        }


                        //如果已经选择了人员，就修改校核人
                        if (state.CheckGroup.AllUserList.Count != 0)
                        {
                            foreach (User userItem in state.CheckGroup.AllUserList)
                            {
                                state.DeleteUser(userItem);
                            }
                        }
                        state.SaveSelectUser(group);
                        state.Modify();
                        workflow.Modify();

                        state.IsRuning = true;
                        state.PreWorkState = workflow.CuWorkState;
                        state.O_iuser5 = new int?(workflow.CuWorkState.O_stateno);
                        state.Modify();




                        //设定审核人
                        AVEVA.CDMS.Server.Group groupAUDIT = new AVEVA.CDMS.Server.Group();
                        User userAUDIT = dbsource.GetUserByName(userAUDITCode.Substring(0, userAUDITCode.IndexOf("__")));
                        if (userAUDIT != null)
                        {
                            groupAUDIT.AddUser(userAUDIT);
                        }

                        WorkState stateAUDIT = workflow.WorkStateList.Find(wsx => (wsx.Code == "AUDIT"));
                        if (stateAUDIT == null)
                        {
                            DefWorkState defWorkState = workflow.DefWorkFlow.DefWorkStateList.Find(dwsx => dwsx.O_Code == "AUDIT");
                            stateAUDIT = workflow.NewWorkState(defWorkState);
                        }

                        //如果已经选择了人员，就修改校核人
                        if (stateAUDIT.CheckGroup.AllUserList.Count != 0)
                        {
                            foreach (User userItem in stateAUDIT.CheckGroup.AllUserList)
                            {
                                stateAUDIT.DeleteUser(userItem);
                            }
                        }
                        stateAUDIT.SaveSelectUser(groupAUDIT);
                        stateAUDIT.Modify();
                        workflow.Modify();

                        stateAUDIT.IsRuning = false;
                        stateAUDIT.PreWorkState = state;
                        //stateAUDIT.O_iuser5 = new int?(state.O_stateno);
                        stateAUDIT.Modify();

                        //workflow.Modify();
                        //这里需要刷新数据源，否则当校核人与审核人是同一个人的时候，审核人没有了通过审核按钮
                        WebApi.DBSourceController.RefreshDBSource(sid);

                        //提交下一状态
                        WorkStateBranch wsb = workflow.CuWorkState.workStateBranchList[0];
                        ExReJObject wfReJo=WebWorkFlowEvent.GotoNextStateAndSelectUser(wsb);

                    }
                }

                //所有流程都选择了校审核人员后，再刷新数据源
                WebApi.DBSourceController.RefreshDBSource(sid);

                reJo.data = new JArray(new JObject(new JProperty("projectKeyword",projectKeyword)));
                reJo.success = true;
                return reJo.Value;

            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(reJo.msg);
            }

            return reJo.Value;
        }

        /// <summary>
        /// 主设人选择校审核人员时，获取默认值
        /// </summary>
        /// <param name="sid">连接密钥</param>
        /// <param name="WorkFlowKeyword">流程关键字</param>
        /// <returns></returns>
        public static JObject GetSelectUserApprovDefault(string sid, string WorkFlowKeyword)
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

                //加载数据

                //1. 当前用户
                //User cuUser = this.m_workflow.dBSource.LoginUser;
                //Doc mDoc = dbsource.GetDocByKeyWord(DocKeyword);
                //if (mDoc == null)
                //{
                //    reJo.msg = "文档不存在！";
                //    return reJo.Value;
                //}

                Object obj = dbsource.GetObjectByKeyWord(WorkFlowKeyword);
                if (!(obj is WorkFlow))
                {
                    reJo.msg = "流程不存在！";
                    return reJo.Value;
                }

                WorkFlow m_workflow = (WorkFlow)obj;
                //if (m_workflow == null)
                //{
                //    reJo.msg = "流程不存在！";
                //    return reJo.Value;
                //}

                //2. 找到所有需要主任分配的流程

                JArray jaWorkFlowList = new JArray();
                List<WorkState> wsList = new List<WorkState>();
                foreach (WorkFlow wf in curUser.ProcessWorkFlowList)
                {

                    //正在运行的流程，并且在主任分配人的状态
                    if (wf.CuWorkState.IsRuning && wf.CuWorkState.DefWorkState.O_Code == "DIRECTORSELECT")
                    {
                        WorkState ws = wf.CuWorkState;
                        wsList.Add(ws);

                        //项目名称
                        string projectName = wf.doc != null ? wf.doc.GetValueByKeyWord("DESIGNPROJECT_CODE") + "__" + wf.doc.GetValueByKeyWord("DESIGNPROJECT_DESC") : "";

                        if (!string.IsNullOrEmpty(projectName))
                        {
                            //任务名称
                            string docName = wf.doc != null ? wf.doc.ToString : "";


                            //流程名称
                            string workflowName = wf.DefWorkFlow.O_Description;
                            string workflowCode = wf.DefWorkFlow.O_Code;


                            //if (workflowCode == "EXCHANGEDOC" || workflowCode == "SENDDOC" || workflowCode == "WORKTASK") //内部提资 和 发文 和 工作任务
                            if (workflowCode == "PRODUCT" || workflowCode == "PUBLICPRODUCT")//专业校审 和 通用校审
                                {
                                JObject joWorkFlowList = new JObject(
                                    new JProperty("workflowKeyword", wf.KeyWord),
                                    new JProperty("projectName", projectName),
                                    new JProperty("docName", docName),
                                    new JProperty("workflowName", workflowName)

                                        );
                                jaWorkFlowList.Add(joWorkFlowList);
                                //DataGridViewRow item = new DataGridViewRow() { Tag = ws };
                                //DataGridViewTextBoxCell projectNamecell = new DataGridViewTextBoxCell();
                                //projectNamecell.Value = projectName;
                                //item.Cells.Add(projectNamecell);

                                //DataGridViewTextBoxCell docNamecell = new DataGridViewTextBoxCell();
                                //docNamecell.Value = docName;
                                //item.Cells.Add(docNamecell);

                                //DataGridViewTextBoxCell workflowNamecell = new DataGridViewTextBoxCell();
                                //workflowNamecell.Value = workflowName;
                                //item.Cells.Add(workflowNamecell);

                                //dgSelectUser.Rows.Add(item);

                            }
                        }
                    }


                }

                //3. 找出本科室所有用户
                //3.1 筛选出科室主任是登录用户的用户组
                string cuUserCode = curUser.Code;
                IEnumerable<AVEVA.CDMS.Server.Group> filterIE = (from agl in m_workflow.dBSource.AllGroupList
                                                                 where (agl.Description == "主任" &&
                                                                     ((agl.AllUserList.Find(u => (u.Code == cuUserCode)) != null)))
                                                                 select agl);

                List<AVEVA.CDMS.Server.Group> groupList = filterIE.ToList();

                //3.2 筛选出科室的所有用户
                JArray jaUserList = new JArray();
                List<User> userList = new List<User>();
                foreach (AVEVA.CDMS.Server.Group group in groupList)
                {
                    foreach (User user in group.ParentGroup.AllUserList)
                    {
                        //用户名称
                        string userName = user.ToString;

                        //int index = this.dgUserList.Rows.Add();
                        //this.dgUserList.Rows[index].Tag = user;
                        //this.dgUserList.Rows[index].Cells[0].Value = userName;


                     ///统计任务
                        #region 统计任务
                        List<AVEVA.CDMS.Server.Task> taskList = user.RuningTaskList;
                        int ProjectOwnerCount = 0;//统计设总任务
                        int ProfessionOwnerCount = 0;//统计主设任务
                        int DesignerCount = 0;//统计设计任务
                        int SenderCount = 0;//统计发文任务
                        int CheckerCount = 0;//统计校核任务
                        int AuditorCount = 0;//统计校核任务
                        foreach (AVEVA.CDMS.Server.Task task in taskList)
                        {
                            string taskType = task.O_TaskType;
                            switch (taskType)
                            {
                                case "设总":
                                    ProjectOwnerCount = ProjectOwnerCount + 1;
                                    break;
                                case "主设":
                                    ProfessionOwnerCount = ProfessionOwnerCount + 1;
                                    break;
                                case "设计":
                                    DesignerCount = DesignerCount + 1;
                                    break;
                                case "发文":
                                    SenderCount = SenderCount + 1;
                                    break;
                                case "校核":
                                    CheckerCount = CheckerCount + 1;
                                    break;
                                case "审核":
                                    AuditorCount = AuditorCount + 1;
                                    break;
                            }
                        }
                        jaUserList.Add(new JObject(
                            new JProperty("userName", userName),
                            new JProperty("ProjectOwnerCount", ProjectOwnerCount == 0 ? "" : ProjectOwnerCount.ToString()),
                            new JProperty("ProfessionOwnerCount", ProfessionOwnerCount == 0 ? "" : ProfessionOwnerCount.ToString()),
                            new JProperty("DesignerCount", DesignerCount == 0 ? "" : DesignerCount.ToString()),
                            new JProperty("SenderCount", SenderCount == 0 ? "" : SenderCount.ToString()),
                            new JProperty("CheckerCount", CheckerCount == 0 ? "" : CheckerCount.ToString()),
                            new JProperty("AuditorCount", AuditorCount == 0 ? "" : AuditorCount.ToString())
                            ));
                        //if (ProjectOwnerCount != 0)
                        //{ }
                        //if (ProfessionOwnerCount != 0)
                        //{
                        //    this.dgUserList.Rows[index].Cells[1].Value = ProfessionOwnerCount.ToString();
                        //}
                        //if (DesignerCount != 0)
                        //{
                        //    this.dgUserList.Rows[index].Cells[2].Value = DesignerCount.ToString();
                        //}
                        //if (SenderCount != 0)
                        //{ }
                        //if (CheckerCount != 0)
                        //{
                        //    this.dgUserList.Rows[index].Cells[3].Value = CheckerCount.ToString();
                        //}
                        //if (AuditorCount != 0)
                        //{
                        //    this.dgUserList.Rows[index].Cells[4].Value = AuditorCount.ToString();
                        //}


                        #endregion

                    }
                }

                reJo.data = new JArray(new JObject(
                    new JProperty("WorkflowList", jaWorkFlowList),
                    new JProperty("UserList", jaUserList))
                    );
                reJo.success = true;
                return reJo.Value;

            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(reJo.msg);
            }

            return reJo.Value;
        }

        /// <summary>
        /// 响应主设人选择校核审核批准人员表单的确定按钮
        /// </summary>
        /// <param name="sid">连接密钥</param>
        /// <param name="workflowAttrJson">流程参数字符串，详见下面例子</param>
        /// <returns>
        /// <para>workflowAttrJson例子：</para>
        /// <code>
        /// [{
        ///    workflowKeyword: dataItem.workflowKeyword,   //流程关键字
        ///    checker: dataItem.checker,   //校核人
        ///    auditor: dataItem.auditor,   //审核人
        ///    approver: dataItem.approver  //批准人
        ///},
        ///{
        ///    workflowKeyword: dataItem.workflowKeyword,
        ///    checker: dataItem.checker,
        ///    auditor: dataItem.auditor,
        ///    approver: dataItem.approver
        ///}]
        /// </code>
        /// </returns>
        public static JObject SelectUserApprov(string sid, string workflowAttrJson)
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

                string projectKeyword = "";

                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(workflowAttrJson);


                foreach (JObject joAttr in jaAttr)
                {

                    string workflowKeyword = joAttr["workflowKeyword"].ToString();
                    string userCHECKCode = joAttr["checker"].ToString();//校核人
                    string userAUDITCode = joAttr["auditor"].ToString();//审核人
                    string userAPPROVCode = joAttr["approver"].ToString();//审核人

                    Object obj = dbsource.GetObjectByKeyWord(workflowKeyword);
                    if (obj == null || !(obj is WorkFlow))
                    {
                        continue;
                    }

                    WorkFlow workflow = (WorkFlow)obj;

                    if (workflow.doc != null)
                    {
                        projectKeyword = workflow.doc.Project.KeyWord;
                    }
                    else if (workflow.Project != null)
                    {
                        projectKeyword = workflow.Project.KeyWord;
                    }

                    if (!string.IsNullOrEmpty(userCHECKCode) && !string.IsNullOrEmpty(userAUDITCode))
                    {


                        //设定校核人
                        AVEVA.CDMS.Server.Group group = new AVEVA.CDMS.Server.Group();
                        User userByName = dbsource.GetUserByName(userCHECKCode.Substring(0, userCHECKCode.IndexOf("__")));
                        if (userByName != null)
                        {
                            group.AddUser(userByName);
                        }


                        //选择校核人
                        #region MyRegion
                        WorkState state = workflow.WorkStateList.Find(wsx => (wsx.Code == "CHECK"));
                        if (state == null)
                        {
                            DefWorkState defWorkState = workflow.DefWorkFlow.DefWorkStateList.Find(dwsx => dwsx.O_Code == "CHECK");
                            state = workflow.NewWorkState(defWorkState);

                        }


                        //如果已经选择了人员，就修改校核人
                        if (state.CheckGroup.AllUserList.Count != 0)
                        {
                            foreach (User userItem in state.CheckGroup.AllUserList)
                            {
                                state.DeleteUser(userItem);
                            }
                        }
                        state.SaveSelectUser(group);

                        state.Modify();
                        workflow.Modify();

                        state.IsRuning = true;
                        state.PreWorkState = workflow.CuWorkState;
                        state.O_iuser5 = new int?(workflow.CuWorkState.O_stateno);
                        state.Modify();
                        #endregion




                        //设定审核人
                        #region MyRegion
                        AVEVA.CDMS.Server.Group groupAUDIT = new AVEVA.CDMS.Server.Group();
                        User userAUDIT = dbsource.GetUserByName(userAUDITCode.Substring(0, userAUDITCode.IndexOf("__")));
                        if (userAUDIT != null)
                        {
                            groupAUDIT.AddUser(userAUDIT);
                        }

                        WorkState stateAUDIT = workflow.WorkStateList.Find(wsx => (wsx.Code == "AUDIT"));
                        if (stateAUDIT == null)
                        {
                            DefWorkState defWorkState = workflow.DefWorkFlow.DefWorkStateList.Find(dwsx => dwsx.O_Code == "AUDIT");
                            stateAUDIT = workflow.NewWorkState(defWorkState);
                        }

                        //如果已经选择了人员，就修改校核人
                        if (stateAUDIT.CheckGroup.AllUserList.Count != 0)
                        {
                            foreach (User userItem in stateAUDIT.CheckGroup.AllUserList)
                            {
                                stateAUDIT.DeleteUser(userItem);
                            }
                        }
                        stateAUDIT.SaveSelectUser(groupAUDIT);

                        stateAUDIT.Modify();
                        workflow.Modify();

                        stateAUDIT.IsRuning = false;
                        stateAUDIT.PreWorkState = state;
                        //stateAUDIT.O_iuser5 = new int?(state.O_stateno);
                        stateAUDIT.Modify();
                        #endregion


                        #region 设定批准人
                        if (!string.IsNullOrEmpty(userAPPROVCode))
                        {

                            AVEVA.CDMS.Server.Group groupAPPROV = new AVEVA.CDMS.Server.Group();
                            User userAPPROV = dbsource.GetUserByName(userAPPROVCode.Substring(0, userAPPROVCode.IndexOf("__")));
                            if (userAPPROV != null)
                            {
                                groupAPPROV.AddUser(userAPPROV);
                            }

                            WorkState stateAPPROV = workflow.WorkStateList.Find(wsx => (wsx.Code == "APPROV"));
                            if (stateAPPROV == null)
                            {
                                DefWorkState defWorkState = workflow.DefWorkFlow.DefWorkStateList.Find(dwsx => dwsx.O_Code == "APPROV");
                                stateAPPROV = workflow.NewWorkState(defWorkState);
                            }

                            //如果已经选择了人员，就修改校核人
                            if (stateAPPROV.CheckGroup.AllUserList.Count != 0)
                            {
                                foreach (User userItem in stateAPPROV.CheckGroup.AllUserList)
                                {
                                    stateAPPROV.DeleteUser(userItem);
                                }
                            }
                            stateAPPROV.SaveSelectUser(groupAPPROV);
                            stateAPPROV.IsRuning = true;
                            stateAPPROV.PreWorkState = state;
                            stateAPPROV.O_iuser5 = new int?(state.O_stateno);
                            stateAPPROV.Modify();
                        }
                        #endregion
                        //workflow.Modify();
                        //这里需要刷新数据源，否则当校核人与审核人是同一个人的时候，审核人没有了通过审核按钮
                        WebApi.DBSourceController.RefreshDBSource(sid);

                        //提交下一状态
                        WorkStateBranch wsb = workflow.CuWorkState.workStateBranchList[0];
                        ExReJObject wfReJo = WebWorkFlowEvent.GotoNextStateAndSelectUser(wsb);

                    }
                }

                //所有流程都选择了校审核人员后，再刷新数据源
                WebApi.DBSourceController.RefreshDBSource(sid);

                reJo.data = new JArray(new JObject(new JProperty("projectKeyword", projectKeyword)));
                reJo.success = true;
                return reJo.Value;

            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(reJo.msg);
            }

            return reJo.Value;
        }
    }
}
