namespace AVEVA.CDMS.WebApi
{
    using AVEVA.CDMS.Server;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class WebWorkFlowEvent
    {
        
        public static After_WorkFlow_RollBack_Event AfterWFRollBack;
        public static Before_WorkFlow_RollBack_Event BeforeWFRollBack;
        public static List<Before_WorkFlow_SelectUsers_Event_Class> ListBeforeWFSelectUsers=new List<Before_WorkFlow_SelectUsers_Event_Class> { };

        public static WorkFlow ThreadWorkFlow;

        #region 用户撤回流程事件
        public static List<Before_Revoke_WorkFlow_Event_Class> ListBeforeRevokeWorkFlow = new List<Before_Revoke_WorkFlow_Event_Class> { };

        //2018-6-30 小钱，按照华西能源要求，添加用户撤回流程事件
        public delegate ExReJObject Before_Revoke_WorkFlow_Event(string PluginName, WorkFlow workflow);

        public class Before_Revoke_WorkFlow_Event_Class
        {
            public Before_Revoke_WorkFlow_Event Event;
            public string PluginName;
        }
        #endregion

        #region 选择用户事件
        public static List<Before_Select_User_Event_Class> ListBeforeSelectUser = new List<Before_Select_User_Event_Class> { };

        //2018-6-30 小钱，按照华西能源要求，添加选择用户事件
        public delegate bool Before_Select_User_Event(string PluginName, WorkStateBranch wsBranch,ref string tabType, ref string tabPara);

        public class Before_Select_User_Event_Class
        {
            public Before_Select_User_Event Event;
            public string PluginName;
        }
        #endregion


        #region 获取流程分支后的事件
        public static List<After_WorkFlow_GetBanch_Event_Class> ListAfterWorkFlowGetBanch = new List<After_WorkFlow_GetBanch_Event_Class> { };

        //2018-6-30 小钱，按照华西能源要求，添加选择用户事件
        public delegate bool After_WorkFlow_GetBanch_Event(string PluginName, ref WorkStateBranch wsBranch);

        public class After_WorkFlow_GetBanch_Event_Class
        {
            public After_WorkFlow_GetBanch_Event Event;
            public string PluginName;
        }
        #endregion

        internal static bool AfterSetCurrent(WorkFlow workFlow)
        {
            bool flag = true;
            try
            {
                if (((workFlow == null) || (workFlow.CuWorkState == null)) || (workFlow.CuWorkState.DefWorkState == null))
                {
                    return flag;
                }
                DefWorkState defWorkState = workFlow.CuWorkState.DefWorkState;
                if (string.IsNullOrEmpty(defWorkState.O_IntoActiveDLL))
                {
                    return flag;
                }
                List<string> list = ParseDllString(defWorkState.O_LeavActiveHttp);
                if (list == null)
                {
                    return flag;
                }
                flag = RunUserPlugin(list,workFlow);
                //string str = list[0];
                //string str2 = list[1];
                //string str3 = list[2];
                //string name = list[3];
                //foreach (string str5 in new List<string> { str })
                //{
                //    try
                //    {
                //        if (!File.Exists(str5))
                //        {
                //            return flag;
                //        }
                //        if (!str5.ToLower().EndsWith(".dll"))
                //        {
                //            return flag;
                //        }
                //        Assembly assembly = Assembly.LoadFile(str5);
                //        if (assembly != null)
                //        {
                //            Type type = assembly.GetType(str2 + "." + str3);
                //            if (type != null)
                //            {
                //                MethodInfo method = type.GetMethod(name);
                //                if (method != null)
                //                {
                //                    object[] parameters = new object[] { workFlow };
                //                    method.Invoke(null, parameters);
                //                }
                //            }
                //        }
                //    }
                //    catch (Exception exception)
                //    {
                //        CDMS.Write(exception.ToString());
                //    }
                //}
            }
            catch (Exception exception2)
            {
                CDMS.Write(exception2.ToString());
            }
            return flag;
        }

        internal static bool AfterSetCurrent(WorkStateBranch wfBranch)
        {
            bool flag = true;
            try
            {
                if (((wfBranch == null) || (wfBranch.defStateBrach == null)) || (wfBranch.workState == null))
                {
                    return flag;
                }
                DefStateBranch defStateBrach = wfBranch.defStateBrach;
                if ((defStateBrach.ActiveList == null) || (defStateBrach.ActiveList.Count <= 0))
                {
                    return flag;
                }
                foreach (object obj2 in defStateBrach.ActiveList)
                {
                    if (obj2 is DefPlugin)
                    {
                        DefPlugin plugin = obj2 as DefPlugin;
                        if (!plugin.O_StartRun && !string.IsNullOrEmpty(plugin.O_Http))
                        {
                            List<string> list = ParseDllString(plugin.O_Http);
                            if (list == null)
                            {
                                return flag;
                            }

                            flag=RunUserPlugin(list,wfBranch.workState.WorkFlow);

                            //string strType = list[0];//类库名称
                            //string strMethod = list[1];//方法名称
                            //string strPara = list[2];//传送参数

                            //Type DefType=typeof(string);

                            //foreach (Type tp in EnterPointController.TypeList)
                            //{
                            //    if (tp.FullName == strType)
                            //    {
                            //        DefType = tp;
                            //        break;
                            //    }
                            //}
                            //if (DefType.Name != "string")
                            //{
                            //    System.Reflection.MethodInfo method = DefType.GetMethod(strMethod);//通过string类型的strMethod获得同名的方法“method”
                            //    if (method != null)
                            //    {
                            //        //添加参数
                            //        ParameterInfo[] pInfos = method.GetParameters();
                            //        List<object> objList = new List<object>();
                            //        foreach (ParameterInfo pInfo in pInfos)
                            //        {
                            //            //  objList.Add(Request[pInfo.Name] ?? "");
                            //        }
                            //        object[] paraObjs = objList.ToArray();
                            //        object obj3 = null;
                            //        obj3 = method.Invoke(null, paraObjs);
                            //        if (((obj3 != null) && (obj3 is PlugInRetValue)) && !(obj3 as PlugInRetValue).RetValue)
                            //        {
                            //            flag = false;
                            //        }
                            //    }
                            //}

                        }
                    }
                }
            }
            catch (Exception exception2)
            {
                CDMS.Write(exception2.ToString());
            }
            return flag;
        }

        /// <summary>
        /// 处理流程分支用户自定义插件（例如：签名等事件）
        /// </summary>
        /// <param name="list"></param>
        /// <param name="wf"></param>
        /// <returns></returns>
        internal static bool RunUserPlugin(List<string> list, WorkFlow wf)
        {
            bool flag = true;
            string strType = list[0];//类库名称
            string strMethod = list[1];//方法名称
            if (list.Count == 3)//如果客户自定义了参数
            {
                string strPara = list[2];//传送参数
            }

            Type DefType = typeof(string);

            foreach (Type tp in EnterPointController.TypeList)
            {
                if (tp.FullName == strType)
                {
                    DefType = tp;
                    break;
                }
            }
            if (DefType.Name != "string")
            {
                System.Reflection.MethodInfo method = DefType.GetMethod(strMethod);//通过string类型的strMethod获得同名的方法“method”
                if (method != null)
                {
                    //添加参数
                    ParameterInfo[] pInfos = method.GetParameters();
                    List<object> objList = new List<object>();
                    objList.Add(wf);
                    //添加用户自定义参数
                    foreach (ParameterInfo pInfo in pInfos)
                    {
                        //  objList.Add(Request[pInfo.Name] ?? "");
                    }
                    object[] paraObjs = objList.ToArray();
                    object obj3 = null;
                    obj3 = method.Invoke(null, paraObjs);
                    if (((obj3 != null) && (obj3 is PlugInRetValue)) && !(obj3 as PlugInRetValue).RetValue)
                    {
                        flag = false;
                    }
                }
            }
            return flag;
        }
        internal static bool BeforeSetCurrent(WorkFlow workFlow)
        {
            bool flag = true;
            try
            {
                if (((workFlow == null) || (workFlow.CuWorkState == null)) || (workFlow.CuWorkState.DefWorkState == null))
                {
                    return flag;
                }
                DefWorkState defWorkState = workFlow.CuWorkState.DefWorkState;
                if (string.IsNullOrEmpty(defWorkState.O_LeavActiveDLL))
                {
                    return flag;
                }
                List<string> list = ParseDllString(defWorkState.O_LeavActiveHttp);
                if (list == null)
                {
                    return flag;
                }
                flag = RunUserPlugin(list,workFlow);
                //string str = list[0];
                //string str2 = list[1];
                //string str3 = list[2];
                //string name = list[3];
                //foreach (string str5 in new List<string> { str })
                //{
                //    try
                //    {
                //        if (!File.Exists(str5))
                //        {
                //            return flag;
                //        }
                //        if (!str5.ToLower().EndsWith(".dll"))
                //        {
                //            return flag;
                //        }
                //        Assembly assembly = Assembly.LoadFile(str5);
                //        if (assembly != null)
                //        {
                //            Type type = assembly.GetType(str2 + "." + str3);
                //            if (type != null)
                //            {
                //                MethodInfo method = type.GetMethod(name);
                //                if (method != null)
                //                {
                //                    object[] parameters = new object[] { workFlow };
                //                    method.Invoke(null, parameters);
                //                }
                //            }
                //        }
                //    }
                //    catch (Exception exception)
                //    {
                //        CDMS.Write(exception.ToString());
                //    }
                //}
            }
            catch (Exception exception2)
            {
                CDMS.Write(exception2.ToString());
            }
            return flag;
        }

        internal static bool BeforeSetCurrent(WorkStateBranch wfBranch)
        {
            bool flag = true;
            try
            {
                if (((wfBranch == null) || (wfBranch.defStateBrach == null)) || (wfBranch.workState == null))
                {
                    return flag;
                }
                DefStateBranch defStateBrach = wfBranch.defStateBrach;
                if ((defStateBrach.ActiveList == null) || (defStateBrach.ActiveList.Count <= 0))
                {
                    return flag;
                }
                foreach (object obj2 in defStateBrach.ActiveList)
                {
                    if (obj2 is DefPlugin)
                    {
                        DefPlugin plugin = obj2 as DefPlugin;
                        if (plugin.O_StartRun && !string.IsNullOrEmpty(plugin.O_Http))
                        {
                            
                            List<string> list = ParseDllString(plugin.O_Http);
                            if (list == null)
                            {
                                return flag;
                            }
                            flag = RunUserPlugin(list,wfBranch.workState.WorkFlow);

                            //string str = list[0];
                            //string str2 = list[1];
                            //string str3 = list[2];
                            //string name = list[3];
                            //foreach (string str5 in new List<string> { str })
                            //{
                            //    try
                            //    {
                            //        string path = str5;
                            //        if (path.IndexOf(':') <= 0)
                            //        {
                            //            path = AppDomain.CurrentDomain.BaseDirectory + path;
                            //        }
                            //        if (File.Exists(path) && path.ToLower().EndsWith(".dll"))
                            //        {
                            //            Assembly assembly = Assembly.LoadFile(path);
                            //            if (assembly != null)
                            //            {
                            //                Type type = assembly.GetType(str2 + "." + str3);
                            //                if (type != null)
                            //                {
                            //                    MethodInfo method = type.GetMethod(name);
                            //                    if (method != null)
                            //                    {
                            //                        object[] parameters = new object[] { wfBranch.workState.WorkFlow };
                            //                        object obj3 = null;
                            //                        obj3 = method.Invoke(null, parameters);
                            //                        if ((obj3 is PlugInRetValue) && !(obj3 as PlugInRetValue).RetValue)
                            //                        {
                            //                            flag = false;
                            //                        }
                            //                    }
                            //                }
                            //            }
                            //        }
                            //    }
                            //    catch (Exception)
                            //    {
                            //    }
                            //}
                        }
                    }
                }
            }
            catch (Exception exception2)
            {
                CDMS.Write(exception2.ToString());
            }
            return flag;
        }

        internal static bool CanWorkFlowWithDraw(WorkFlow workFlow)
        {
            try
            {
                if (workFlow == null)
                {
                    return false;
                }
                if (workFlow.O_WorkFlowStatus == enWorkFlowStatus.Finish)
                {
                    if (((workFlow.CuWorkState != null) && (workFlow.CuWorkState.WorkUserList != null)) && (workFlow.CuWorkState.WorkUserList.Count > 0))
                    {
                        foreach (WorkUser user in workFlow.CuWorkState.WorkUserList)
                        {
                            if ((user.User != null) && (user.User == workFlow.dBSource.LoginUser))
                            {
                                return true;
                            }
                        }
                    }
                }
                else if ((workFlow.CuWorkState != null) && (workFlow.CuWorkState.PreWorkState != null))
                {
                    foreach (WorkUser user2 in workFlow.CuWorkState.WorkUserList)
                    {
                        if (user2.O_pass.HasValue && user2.O_pass.Value)
                        {
                            return false;
                        }
                    }
                    if ((workFlow.CuWorkState.WorkAuditList != null) && (workFlow.CuWorkState.WorkAuditList.Count > 0))
                    {
                        return false;
                    }
                    foreach (WorkUser user3 in workFlow.CuWorkState.PreWorkState.WorkUserList)
                    {
                        if (user3.O_pass.HasValue && (user3.User.ID == workFlow.dBSource.LoginUser.ID))
                        {
                            return true;
                        }
                    }
                    if ((workFlow.CuWorkState.PreWorkState.Code == "START") && (workFlow.WorkStateList[0].WorkUserList[0].User.ID == workFlow.dBSource.LoginUser.ID))
                    {
                        return true;
                    }
                    if ((workFlow.WorkStateList[0].WorkUserList[0].User.ID == workFlow.dBSource.LoginUser.ID) && (workFlow.CuWorkState.Code == workFlow.WorkStateList[0].Code))
                    {
                        return true;
                    }
                }
            }
            catch (Exception exception)
            {
                CDMS.Write(exception.ToString());
            }
            return false;
        }

        //转到流程下一状态，参数：wsBranch：本状态分支，userlist：用户列表
        public static ExReJObject GotoNextStateAndSelectUser(WorkStateBranch wsBranch)
        {
            ExReJObject reJo = GotoNextStateAndSelectUser(wsBranch, "");
            return reJo;
        }

        //转到流程下一状态，参数：wsBranch：本状态分支，userlist：用户列表
        public static ExReJObject GotoNextStateAndSelectUser(WorkStateBranch wsBranch, string userlist)
        {
            ExReJObject reJo = new ExReJObject();
            try
            {
                Group checkGroup = wsBranch.NextWorkState.CheckGroup;
                WorkFlow curWorkFlow = wsBranch.workState.WorkFlow;
                DBSource dbsource = wsBranch.dBSource;

                //流程转到下一状态前，调用客户插件，这里使用了多个委托事件，让不同的客户插件调用
                //但是这些事件需要启动菜单，把菜单的WebWorkFlowEvent.BeforeWFSelectUsers事件赋值后才能触发（服务端启动后直接按流程按钮是没反应的，因为此时事件还没有赋值）
                foreach (Before_WorkFlow_SelectUsers_Event_Class BeforeWFSelectUsers in ListBeforeWFSelectUsers)
                {
                    if (BeforeWFSelectUsers.Event != null)
                    {
                        ExReJObject BeforeWFReJo = BeforeWFSelectUsers.Event(BeforeWFSelectUsers.PluginName, wsBranch.workState.WorkFlow, wsBranch);
                        if (BeforeWFReJo.success == false)
                        {
                            BeforeWFReJo.success = true;
                            return BeforeWFReJo;
                        }
                    }
                }

                //判断下一状态是否有用户，没有就选择下一状态的人员
                if (((wsBranch.NextWorkState.DefWorkState != null) && (wsBranch.NextWorkState.DefWorkState.O_Code.ToUpper() != "END")) && (((wsBranch.NextWorkState.group == null) || (((wsBranch.NextWorkState.group.AllGroupList == null) || (wsBranch.NextWorkState.group.AllGroupList.Count <= 0)) && (wsBranch.NextWorkState.group.AllUserList == null))) || (wsBranch.NextWorkState.group.AllUserList.Count < 0)))
                {
                    //根据流程模板定义，选择下一状态的用户
                    //先自动选择人员
                    //checkGroup = wsBranch.NextWorkState.CheckGroup;

                    checkGroup = WebSelUserForNextState.SelectUser(curWorkFlow, wsBranch.NextWorkState);
                    if (checkGroup == null)
                    {
                        checkGroup = wsBranch.NextWorkState.CheckGroup;

                    }
                    if ((((checkGroup == null) || (checkGroup.UserList == null)) || (checkGroup.UserList.Count <= 0)) && (((checkGroup == null) || (checkGroup.GroupList == null)) || (checkGroup.GroupList.Count <= 0)))
                    {
                        if (userlist != null && userlist.Length > 0)
                        {
                            //List < User > userList = new List<User>();
                            Group userGroup = null;
                            userGroup = dbsource.NewGroup();
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
                            if (userGroup != null)
                            {
                                checkGroup = userGroup;
                            }
                            else
                            {
                                reJo.msg = "请必须选择下一状态的校审人员,否则流程无法进行下去!";
                                return reJo;
                            }
                        }
                        else
                        {
                            #region 选择用户触发的用户事件
                            string tabType = "", tabPara = "";
                            foreach (WebWorkFlowEvent.Before_Select_User_Event_Class BeforeSelectUserEvent in WebWorkFlowEvent.ListBeforeSelectUser)
                            {
                                if (BeforeSelectUserEvent.Event != null)
                                {
                                    //如果在用户事件中筛选过了，就跳出事件循环
                                    if (BeforeSelectUserEvent.Event(BeforeSelectUserEvent.PluginName, wsBranch,ref tabType, ref tabPara))
                                    {
                                        break;
                                    }
                                }
                            }
                            #endregion

                            //提示需要选择下一状态的校审人员
                            reJo.data = new JArray(new JObject(new JProperty("state", "selectUser"), 
                                new JProperty("wfKeyword", curWorkFlow.KeyWord),
                                new JProperty("tabType",tabType),
                                new JProperty("tabPara",tabPara)
                                ));
                            //reJo.success = true;
                            reJo.msg = "selectUser";
                            return reJo;
                        }
                    }
                }
                //添加用户组到下一状态
                wsBranch.NextStateAddGroup(checkGroup);


                //通过当前流程对象
                wsBranch.SetCurrentUserPass();
                //跳转前处理客户自定义函数(WorkFlow)
                BeforeSetCurrent(curWorkFlow);
                //跳转前处理客户自定义函数(WorkStateBranch)
                BeforeSetCurrent(wsBranch);
                if (((curWorkFlow.CuWorkState.DefWorkState != null) && (curWorkFlow.CuWorkState.DefWorkState.O_Code.ToUpper() == "START")) && (wsBranch != null))
                {
                    //转到下一流程对象（确定该分支为下一流程）
                    wsBranch.SetCurrent();
                }
                else
                {
                    //通过当前流程状态
                    curWorkFlow.CuWorkState.SetCuWorkStatePass();
                }
                //跳转后处理客户自定义函数(WorkFlow)
                AfterSetCurrent(curWorkFlow);
                //跳转后处理客户自定义函数(WorkStateBranch)
                AfterSetCurrent(wsBranch);

                //小钱2018-8-23 流程状态的O_SendDate改为当前时间
                curWorkFlow.CuWorkState.O_SendDate = DateTime.Now;
                curWorkFlow.CuWorkState.Modify();

                reJo.data = new JArray(new JObject(new JProperty("state", "Pass"), new JProperty("WorkFlowKeyword", curWorkFlow.KeyWord),
                    new JProperty("CuWorkStateCode", curWorkFlow.CuWorkState.Code)));
                reJo.success = true;
                return reJo;
            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(e.Message);
            }
            return reJo;
        }

        internal static List<string> ParseDllString(string dllString)
        {
            try
            {
                if (string.IsNullOrEmpty(dllString))
                {
                    return null;
                }
                List<string> list = new List<string>();
                char[] separator = new char[] { '<', '>' };
                string[] strArray = dllString.Split(separator);
                if ((strArray == null) || (strArray.Length < 2))//规定了传递两个参数
                {
                    return null;
                }
                foreach (string str in strArray)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        list.Add(str);
                    }
                }
                if (list.Count != 2)//规定了传递两个参数
                {
                    return null;
                }
                return list;
            }
            catch (Exception)
            {
            }
            return null;
        }

        /// <summary>
        /// 把流程中当前状态撤销,回到上一个状态
        /// 退回的过程中,要删除跳到当前状态时所发送的消息,所创建的快捷个人工作台 等等动作所生成的数据
        /// 而且必需只有直接提交到当前状态的人员,才能进行撤销
        /// </summary>
        /// <param name="workFlow"></param>
        /// <returns></returns>
        public static bool WorkFlowReback(WorkFlow wkFlow)
        {
            try
            {

                if (wkFlow == null || wkFlow.CuWorkState == null
                    || wkFlow.CuWorkState.PreWorkState == null
                    || wkFlow.dBSource == null || wkFlow.dBSource.LoginUser == null)
                    return false;


                if (!CanWorkFlowWithDraw(wkFlow))
                {
                    return false;
                }

                wkFlow.dBSource.ProgramRun = true;

                bool bEndWorkFlow = false;

                if (wkFlow.O_WorkFlowStatus == enWorkFlowStatus.Finish)
                    bEndWorkFlow = true;



                #region  非结束流程的撤销工作

                WorkState curWorkState = wkFlow.CuWorkState;
                WorkState preWorkState = bEndWorkFlow ? curWorkState : curWorkState.PreWorkState;



                if (WebWorkFlowEvent.BeforeWFRollBack != null)
                {
                    WebWorkFlowEvent.BeforeWFRollBack(wkFlow);
                }

                Project project = wkFlow.Project;
                List<Doc> DocList = wkFlow.DocList;

                //-------------------------------------------------------
                //删除其他人桌面的快捷方式
                bool mark = false;
                if (project != null)
                {
                    List<Project> shortProjectList = project.ShortProjectList;
                    if (shortProjectList != null)
                    {

                        //删除目录的快捷方式
                        foreach (Project p in shortProjectList)
                        {
                            if (p.O_type == enProjectType.UserCustom && (p.O_iuser1 == curWorkState.ID || (bEndWorkFlow && p.O_iuser1 == -wkFlow.O_WorkFlowno)))
                            {
                                p.DeleteLogicProject();
                                mark = true;
                            }
                        }

                    }
                }



                //目录没有快捷方式，则删除文档的快捷方式
                if (!mark && DocList != null && DocList.Count() > 0)
                {

                    //删除文档的快捷方式
                    foreach (Doc wfdoc in DocList)
                    {
                        List<Doc> shortDocList = wfdoc.ShortDocList;
                        if (shortDocList != null)
                        {
                            foreach (Doc shortDoc in shortDocList)
                            {

                                //判断父目录是否为个人工作台
                                if (shortDoc.Project.O_type == enProjectType.UserCustom && (shortDoc.O_iuser1 == curWorkState.ID || (bEndWorkFlow && shortDoc.O_iuser1 == -wkFlow.O_WorkFlowno)))
                                {

                                    //删除本对象
                                    shortDoc.Delete();




                                    //删除父目录
                                    shortDoc.Project.DeleteLogicProject();

                                }
                            }
                        }
                    }

                }



                //删除给同一批人发送的消息
                List<AVEVA.CDMS.Server.Message> mesList = null;
                if (bEndWorkFlow)
                {
                    String endKeyWord = "";
                    endKeyWord = wkFlow.GetENDWorkStateKeyword();
                    if (!String.IsNullOrEmpty(endKeyWord))
                        mesList = wkFlow.dBSource.DContext.Message.Where(m => m.Note == endKeyWord).ToList<AVEVA.CDMS.Server.Message>();

                }
                else
                    mesList = wkFlow.dBSource.DContext.Message.Where(m => m.Note == curWorkState.KeyWord).ToList<AVEVA.CDMS.Server.Message>();

                if (mesList != null)
                {
                    foreach (AVEVA.CDMS.Server.Message ms in mesList)
                    {
                        ms.dBSource = wkFlow.dBSource;

                        if (ms.UserList != null)
                        {


                            //逐个用户判断
                            int i = ms.UserList.Count();
                            foreach (MsgUser mu in ms.UserList)
                            {
                                mu.dBSource = wkFlow.dBSource;
                                if (mu.ReadDate == null)
                                {

                                    //删除用户
                                    mu.Delete();
                                    i--;
                                }
                            }


                            //如果用户都删除了，则删除消息
                            if (i == 0)
                            {
                                ms.Delete();

                                //删除消息对应的任务
                                if (ms.task != null)
                                {
                                    ms.task.dBSource = wkFlow.dBSource;
                                    ms.task.Delete();
                                }
                            }

                        }
                    }
                }
                //-------------------------------------------------------------------

                //wkFlow.CuWorkState = preWorkState; 


                //把撤回人的状态设为未通过状态
                if (preWorkState.WorkUserList != null)
                {
                    foreach (WorkUser wkUser in preWorkState.WorkUserList)
                    {
                        if (wkUser.User.O_userno == wkFlow.dBSource.LoginUser.O_userno)
                        {
                            wkUser.O_pass = false;
                            wkUser.Modify();
                            break;
                        }
                    }
                }




                //设置当前流程状态


                if (preWorkState.DefWorkState != null && preWorkState.DefWorkState.O_Code.ToUpper() == "START")
                {
                    //前一状态为Start状态,则可以删除流程了 

                    try
                    {
                        wkFlow.Delete();
                    }
                    catch (Exception subEx2) { AVEVA.CDMS.Server.CDMS.Write(subEx2.ToString()); }
                }
                else
                {
                    //wkFlow.Refresh();
                    wkFlow.CuWorkState = preWorkState;
                    wkFlow.MoveToState(preWorkState);

                    wkFlow.Modify();

                    //wkFlow.Refresh();
                    //wkFlow.dBSource.Refresh();
                }

                wkFlow.dBSource.ProgramRun = false;

                //TIM 2009-12-02 删除当前状态中,最后一个校审人的校审历史记录
                try
                {
                    //if (preWorkState.HistoryList != null || preWorkState.HistoryList.Count > 0)
                    //{


                    //    //撤销后,必需删除前一个状态的校审记录,撤销人员才能重新选择人员
                    //    WorkStateHistory needDelHistory = null;

                    //    List<WorkStateHistory> userHistories = null;
                    //    IEnumerable<WorkStateHistory> Histories = from h in preWorkState.HistoryList
                    //                                              where h.CheckUser == wkFlow.dBSource.LoginUser
                    //                                              orderby h.PassDate descending
                    //                                              select h;

                    //    if (Histories != null)
                    //        userHistories = Histories.ToList();

                    //    //取最新的一个提交记录
                    //    if (userHistories != null && userHistories.Count >= 0)
                    //    {
                    //        needDelHistory = userHistories[0];
                    //    }

                    //    if (needDelHistory != null)
                    //    {
                    //        //TODO 删除历史记录


                    //        try
                    //        {
                    //            Audit audit = wkFlow.dBSource.DContext.Audit.Single(a => a.O_actiontype == enActionType.WorkUserPass
                    //                                                                   && a.O_objno == needDelHistory.NextWorkState.O_stateno
                    //                                                                   && a.O_userno == needDelHistory.CheckUser.O_userno
                    //                                                                   && a.O_acttime == needDelHistory.PassDate

                    //                                                                   );



                    //            if (audit != null)
                    //            {
                    //                if (audit.dBSource == null)
                    //                    audit.dBSource = wkFlow.dBSource;

                    //                audit.Delete();

                    //            }
                    //        }
                    //        catch (Exception subEx3) { CDMS.Server.CDMS.Write(subEx3.ToString()); }
                    //    }

                    //}

                    //如果preWorkState中只有一个校审人,则可以删除curWorkState中的人员,使得preWorkState中的校审人
                    //即撤销人员可以重新选择人员; 
                    if (preWorkState.WorkUserList != null && preWorkState.WorkUserList.Count == 1 && !bEndWorkFlow)
                    {
                        if (curWorkState.WorkUserList != null && curWorkState.WorkUserList.Count > 0)
                        {
                            List<WorkUser> tempUserList = new List<WorkUser>();

                            foreach (WorkUser tempU in curWorkState.WorkUserList)
                            {
                                tempUserList.Add(tempU);
                            }

                            foreach (WorkUser tempU in tempUserList)
                            {
                                curWorkState.DeleteUser(tempU.User);
                            }
                        }
                    }


                    //写日志
                    preWorkState.dBSource.NewAudit(enActionType.WorkFlowStateChange, preWorkState.WorkFlow.O_WorkFlowno, 0, String.Format("由状态 {0} 撤回状态 {1}", bEndWorkFlow ? "END_结束" : curWorkState.DefWorkState.ToString, preWorkState.DefWorkState.ToString));

                    
                    wkFlow.Refresh();
                    //wkFlow.dBSource.Refresh();
                    
                }
                catch (Exception subEx4) { AVEVA.CDMS.Server.CDMS.Write(subEx4.ToString()); }


                if (AfterWFRollBack != null)
                {
                    AfterWFRollBack(wkFlow);
                }

                return true;


                #endregion





            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message , e.StackTrace); 
                AVEVA.CDMS.Server.CDMS.Write(e.ToString());
            }
            finally
            {
                if (wkFlow != null && wkFlow.dBSource != null)
                {
                    wkFlow.dBSource.ProgramRun = false;
                }
            }
            return false;
        }

        internal static bool WorkFlowReback2(WorkFlow wkFlow)
        {
            try
            {
                if (((wkFlow == null) || (wkFlow.CuWorkState == null)) || (((wkFlow.CuWorkState.PreWorkState == null) || (wkFlow.dBSource == null)) || (wkFlow.dBSource.LoginUser == null)))
                {
                    return false;
                }
                if (!CanWorkFlowWithDraw(wkFlow))
                {
                    return false;
                }
                if (((wkFlow != null) && (wkFlow.DocList != null)) && (wkFlow.DocList.Count > 0))
                {
                    foreach (Doc doc in wkFlow.DocList)
                    {
                        if (doc.OperateDocStatus == enDocStatus.OUT)
                        {
                            //MessageBox.Show("文件处于检出状态,不能撤销,如有正在打开的文档请先关闭.");
                            return false;
                        }
                    }
                }
                wkFlow.dBSource.ProgramRun = true;
                bool flag = false;
                if (wkFlow.O_WorkFlowStatus == enWorkFlowStatus.Finish)
                {
                    flag = true;
                }
                WorkState curWorkState = wkFlow.CuWorkState;
                WorkState workstat = flag ? curWorkState : curWorkState.PreWorkState;
                if (BeforeWFRollBack != null)
                {
                    BeforeWFRollBack(wkFlow);
                }
                Project project = wkFlow.Project;
                List<Doc> docList = wkFlow.DocList;
                bool flag2 = false;
                if (project != null)
                {
                    List<Project> shortProjectList = project.ShortProjectList;
                    if (shortProjectList != null)
                    {
                        foreach (Project projectItem in shortProjectList)
                        {
                            if (projectItem.O_type == enProjectType.UserCustom)
                            {
                                int? nullable = projectItem.O_iuser1;
                                int iD = curWorkState.ID;
                                if (!((nullable.GetValueOrDefault() == iD) && nullable.HasValue))
                                {
                                    if (!flag)
                                    {
                                        continue;
                                    }
                                    int? nullable2 = projectItem.O_iuser1;
                                    int num3 = -wkFlow.O_WorkFlowno;
                                    if (!((nullable2.GetValueOrDefault() == num3) && nullable2.HasValue))
                                    {
                                        continue;
                                    }
                                }
                                projectItem.DeleteLogicProject();
                                flag2 = true;
                            }
                        }
                    }
                }
                if ((!flag2 && (docList != null)) && (docList.Count<Doc>() > 0))
                {
                    foreach (Doc doc2 in docList)
                    {
                        List<Doc> shortDocList = doc2.ShortDocList;
                        if (shortDocList != null)
                        {
                            foreach (Doc doc3 in shortDocList)
                            {
                                if ((doc3.Project != null) && (doc3.Project.O_type == enProjectType.UserCustom))
                                {
                                    int? nullable3 = doc3.O_iuser1;
                                    int num4 = curWorkState.ID;
                                    if (!((nullable3.GetValueOrDefault() == num4) && nullable3.HasValue))
                                    {
                                        if (!flag)
                                        {
                                            continue;
                                        }
                                        int? nullable4 = doc3.O_iuser1;
                                        int num5 = -wkFlow.O_WorkFlowno;
                                        if (!((nullable4.GetValueOrDefault() == num5) && nullable4.HasValue))
                                        {
                                            continue;
                                        }
                                    }
                                    doc3.Delete();
                                    doc3.Project.DeleteLogicProject();
                                }
                            }
                        }
                    }
                }
                wkFlow.dBSource.MuxConsole.WaitOne();
                try
                {
                    List<AVEVA.CDMS.Server.Message> list4 = null;
                    if (flag)
                    {
                        string endKeyWord = "";
                        endKeyWord = wkFlow.GetENDWorkStateKeyword();
                        if (!string.IsNullOrEmpty(endKeyWord))
                        {
                            list4 = (from m in wkFlow.dBSource.DContext.Message
                                     where m.Note == endKeyWord
                                     select m).ToList<AVEVA.CDMS.Server.Message>();
                        }
                    }
                    else
                    {
                        list4 = (from m in wkFlow.dBSource.DContext.Message
                                 where m.Note == curWorkState.KeyWord
                                 select m).ToList<AVEVA.CDMS.Server.Message>();
                    }
                    if (list4 != null)
                    {
                        foreach (AVEVA.CDMS.Server.Message message in list4)
                        {
                            message.dBSource = wkFlow.dBSource;
                            if (message.UserList != null)
                            {
                                int num = message.UserList.Count<MsgUser>();
                                foreach (MsgUser user in message.UserList)
                                {
                                    user.dBSource = wkFlow.dBSource;
                                    if (!user.ReadDate.HasValue)
                                    {
                                        user.Delete();
                                        num--;
                                    }
                                }
                                if (num == 0)
                                {
                                    message.Delete();
                                    if (message.task != null)
                                    {
                                        message.task.dBSource = wkFlow.dBSource;
                                        message.task.Delete();
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
                finally
                {
                    wkFlow.dBSource.MuxConsole.ReleaseMutex();
                }
                if (workstat.WorkUserList != null)
                {
                    foreach (WorkUser user2 in workstat.WorkUserList)
                    {
                        if (user2.User.O_userno == wkFlow.dBSource.LoginUser.O_userno)
                        {
                            user2.O_pass = false;
                            user2.Modify();
                            break;
                        }
                    }
                }
                wkFlow.dBSource.ProgramRun = false;
                if ((workstat.DefWorkState != null) && (workstat.DefWorkState.O_Code.ToUpper() == "START"))
                {
                    try
                    {
                        wkFlow.Delete();
                        goto Label_062F;
                    }
                    catch (Exception exception)
                    {
                        CDMS.Write(exception.ToString());
                        goto Label_062F;
                    }
                }
                wkFlow.CuWorkState = workstat;
                wkFlow.MoveToState(workstat);
                wkFlow.Modify();
                wkFlow.Refresh();
                wkFlow.dBSource.Refresh();
            Label_062F:
                if (flag && wkFlow.DefWorkFlow.SetFileFinal)
                {
                    foreach (Doc doc4 in wkFlow.DocList)
                    {
                        if (doc4.O_dmsstatus == enDocStatus.IN_FINAL)
                        {
                            doc4.OperateDocStatus = enDocStatus.IN;
                        }
                    }
                }
                try
                {
                    if ((((workstat.WorkUserList != null) && (workstat.WorkUserList.Count == 1)) && (!flag && (curWorkState.WorkUserList != null))) && (curWorkState.WorkUserList.Count > 0))
                    {
                        List<WorkUser> list5 = new List<WorkUser>();
                        foreach (WorkUser user3 in curWorkState.WorkUserList)
                        {
                            list5.Add(user3);
                        }
                        foreach (WorkUser user4 in list5)
                        {
                            curWorkState.DeleteUser(user4.User);
                        }
                    }
                    workstat.dBSource.NewAudit(enActionType.WorkFlowStateChange, workstat.WorkFlow.O_WorkFlowno, 0, string.Format("由状态 {0} 撤回状态 {1}", flag ? "END_结束" : curWorkState.DefWorkState.ToString, workstat.DefWorkState.ToString));
                }
                catch (Exception exception2)
                {
                    CDMS.Write(exception2.ToString());
                }
                if (AfterWFRollBack != null)
                {
                    AfterWFRollBack(wkFlow);
                }
                return true;
            }
            catch (Exception exception3)
            {
                CDMS.Write(exception3.ToString());
            }
            finally
            {
                if ((wkFlow != null) && (wkFlow.dBSource != null))
                {
                    wkFlow.dBSource.ProgramRun = false;
                }
            }
            return false;
        }

        //public static void WorkFlowSetCuWorkStatePass(object obWorkFlow)
        //{
        //    try
        //    {
        //        if (obWorkFlow != null)
        //        {
        //            if (obWorkFlow is WorkFlow)
        //            {
        //                WorkFlow workFlow = obWorkFlow as WorkFlow;
        //                if ((workFlow != null) && (workFlow.CuWorkState != null))
        //                {
        //                    BeforeSetCurrent(workFlow);
        //                    workFlow.CuWorkState.SetCuWorkStatePass();
        //                    AfterSetCurrent(workFlow);
        //                }
        //            }
        //            else if (obWorkFlow is WorkFlowThreadParam)
        //            {
        //                WorkFlowThreadParam param = obWorkFlow as WorkFlowThreadParam;
        //                if ((param.WorkFlow != null) && (param.WorkFlow.CuWorkState != null))
        //                {
        //                    try
        //                    {
        //                        if (ExplorerManager.SetStatusBarMessage != null)
        //                        {
        //                            ExplorerManager.SetStatusBarMessage(string.Format("正在提交流程: \"{0}\" 分支:\"{1}\" 下一状态:\"{2}\" 附件:\"{3}...\" ", new object[] { param.WorkFlow.DefWorkFlow.ToString, param.WorkStateBranch.defStateBrach.ToString, param.WorkStateBranch.NextWorkState.DefWorkState.ToString, (param.WorkFlow.doc != null) ? param.WorkFlow.doc.ToString : "" }));
        //                        }
        //                    }
        //                    catch (Exception)
        //                    {
        //                        if (ExplorerManager.SetStatusBarMessage != null)
        //                        {
        //                            ExplorerManager.SetStatusBarMessage("正在提交流程...");
        //                        }
        //                    }
        //                    param.WorkStateBranch.SetCurrentUserPass();
        //                    if (WFPassThreadProgressForm != null)
        //                    {
        //                        WFPassThreadProgressForm.UnderAnotherForm = true;
        //                    }
        //                    BeforeSetCurrent(param.WorkFlow);
        //                    if (!BeforeSetCurrent(param.WorkStateBranch))
        //                    {
        //                        if (WFPassThreadProgressForm != null)
        //                        {
        //                            WFPassThreadProgressForm.NeedClosedNow = true;
        //                        }
        //                        param.IsPass = false;
        //                        if (ExplorerManager.SetStatusBarMessage != null)
        //                        {
        //                            ExplorerManager.SetStatusBarMessage("提交流程已撤销!");
        //                        }
        //                        if ((param.WorkFlow.CuWorkState != null) && (param.WorkFlow.CuWorkState.WorkUserList != null))
        //                        {
        //                            foreach (WorkUser user in param.WorkFlow.CuWorkState.WorkUserList)
        //                            {
        //                                if (user.User == param.WorkFlow.dBSource.LoginUser)
        //                                {
        //                                    user.O_pass = false;
        //                                    user.Modify();
        //                                    return;
        //                                }
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (((param.WorkFlow.CuWorkState.DefWorkState != null) && (param.WorkFlow.CuWorkState.DefWorkState.O_Code.ToUpper() == "START")) && (param.WorkStateBranch != null))
        //                        {
        //                            param.WorkStateBranch.SetCurrent();
        //                        }
        //                        else
        //                        {
        //                            param.WorkFlow.CuWorkState.SetCuWorkStatePass();
        //                        }
        //                        AfterSetCurrent(param.WorkFlow);
        //                        AfterSetCurrent(param.WorkStateBranch);
        //                        if (WFPassThreadProgressForm != null)
        //                        {
        //                            WFPassThreadProgressForm.UnderAnotherForm = false;
        //                        }
        //                        if (ExplorerManager.SetStatusBarMessage != null)
        //                        {
        //                            ExplorerManager.SetStatusBarMessage("提交流程完毕!");
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        if (ExplorerManager.SetStatusBarMessage != null)
        //        {
        //            ExplorerManager.SetStatusBarMessage("提交流程时出现异常: " + exception.ToString());
        //        }
        //        CDMS.Write(exception.ToString());
        //    }
        //    finally
        //    {
        //        if (WFPassThreadProgressForm != null)
        //        {
        //            WFPassThreadProgressForm.NeedClosedNow = true;
        //            WFPassThreadProgressForm = null;
        //        }
        //    }
        //}

        public delegate void After_WorkFlow_RollBack_Event(WorkFlow workFlow);

        public delegate void Before_WorkFlow_RollBack_Event(WorkFlow workFlow);

        public delegate ExReJObject Before_WorkFlow_SelectUsers_Event(string PlugName, WorkFlow wf, WorkStateBranch wsb);

     
        public class Before_WorkFlow_SelectUsers_Event_Class {
            public Before_WorkFlow_SelectUsers_Event Event;
            public string PluginName;
        }
    }
}

