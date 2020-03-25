using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AVEVA.CDMS.Server;
using System.Collections;
using System.Reflection;
using System.IO;
using System.Threading;

namespace AVEVA.CDMS.WebApi
{
    /// <summary>  
    /// 流程操作类
    /// </summary>  
    public class WorkFlowController : Controller
    {

        //线程锁 
        private static Mutex muxConsole = new Mutex();

        /// <summary>
        /// 根据DOC或者Project或者WorkFlow的Keyword，返回一个对象的工作流各个状态的意见的JSON对象
        /// DOC，Project，WorkFlow的Keyword里面只选一个来处理
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="Keyword">DOC或者Project或者WorkFlow的关键字</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时，data包含多个JObject，JObject的值又分为多种类型：</para>
        /// <para>类型一：校审意见(WA),包含参数："UserName"，"WorkState"，"ProcAudit"，"DeProcAudit"，"ProcTime"，"KeyWord"，"Visible"值为("True")</para>
        /// <para>类型二：分支按钮(WSB),包含参数："BtnType"：值为"btn")，"Desc"，"KeyWord"，"Enabled","Visible"值为("False")</para>
        /// <para>类型三：返回修改按钮(RebackText),包含参数："BtnType"：值为("reback")，"Desc"，"KeyWord"：值为("")，"Visible"值为("False")</para>
        /// <para>类型四：默认校审意见列表("DefaultAuditList"),包含参数："BtnType"：值为("addAduit")，"Desc"，"KeyWord"：值为("")，"Visible"值为("False")</para>
        /// <para>类型五：返回流程号,包含参数："WfKeyword"，"Visible"值为("False")</para>
        /// <para>例子：</para>
        /// <code>
        /// {
        ///  "success": true,
        ///  "total": 0,
        ///  "msg": "",
        ///  "data": [
        ///    {
        ///      "BtnType": "reback",       //最右边的按钮
        ///      "Desc": "删除流程",        //按钮描述
        ///      "KeyWord": "",
        ///      "Visible": "False",        //是否显示在校审意见页
        ///      "Enabled": "T"             //按钮是否可用
        ///    },
        ///    {
        ///      "BtnType": "btn",          //普通按钮
        ///      "Desc": "选择校审人员",
        ///      "KeyWord": "W13132S59392D236",     //流程分支的模板的Keyword
        ///      "Visible": "False",
        ///      "Enabled": "T"
        ///    },
        ///    {    //校审意见列表下面那行字
        ///      "UserString": "1 设计:admin__administrator 2 主任:admin__administrator",   
        ///      "Visible": "False"
        ///    },
        ///    {
        ///      "WfKeyword": "GJEPCMSW13132",  //流程ID
        ///      "isFinish": "False",           //流程是否结束
        ///      "Visible": "False"
        ///    }
        ///  ]
        /// }
        /// </code>
        /// </returns>
        public static JObject GetWorkFlow(string sid, string Keyword)
        {
            ExReJObject reJo = new ExReJObject();

            try
            {
                //处理参数，判断是根据文档，目录还是流程Keyword参数来获取流程
                if (string.IsNullOrEmpty(Keyword) || string.IsNullOrEmpty(sid))
                {
                    reJo.msg = "错误的提交数据。";
                    return reJo.Value;
                }


                //登录用户
                User curUser = DBSourceController.GetCurrentUser(sid);
                if (curUser == null) return CommonController.SidError();

                //刷新数据源，保持和explorer客户端同步
                DBSource dbsource = curUser.dBSource;

                //获取目录对象
                object obj = dbsource.GetObjectByKeyWord(Keyword);
                WorkFlow workflow = new WorkFlow();
                if (obj is Doc)
                {
                    Doc ddoc = (Doc)obj;
                    Doc doc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;
                    workflow = doc.WorkFlow;
                }
                else if (obj is Project)
                {
                    Project proj = (Project)obj;
                    workflow = proj.WorkFlow;
                }
                else if (obj is WorkFlow)
                {
                    workflow = (WorkFlow)obj;
                }

                if (workflow == null) return null;
                //{
                //    reJo.msg = "错误的提交数据。";
                //    return reJo.Value;
                //}

                //线程锁 
                muxConsole.WaitOne();
                try
                {
                    //转换JSON字符串
                    JObject joMsg = WorkFlowController.GetWorkFlowJson(workflow);

                    JArray jaResult = new JArray();
                    JToken jtMsg = joMsg;
                    string RebackEnabled = jtMsg["RebackEnabled"].ToString();
                    string RetractEnabled = jtMsg["RetractEnabled"].ToString();
                    string strRbKeyword = jtMsg["RebackKeyword"].ToString();
                    string strRtKeyword = jtMsg["RetractKeyword"].ToString();
                    string userString = "";

                    //判断用户状态，此状态决定是否显示输入意见按钮
                    string UserStatus = jtMsg["UserStatus"].ToString();
                    foreach (JProperty jpMsg in jtMsg)
                    {
                        //添加返回流程的处理意见
                        if (jpMsg.Name == "WA" && !string.IsNullOrEmpty(jpMsg.Value.ToString()) && jpMsg.Value.ToString() != "null")
                        {
                            JArray ja5 = (JArray)(jpMsg.Value);
                            foreach (JObject jo in ja5)
                            {
                                jaResult.Add(new JObject {
                                new JProperty("UserName",jo["U"]),
                                new JProperty("UserKeyword",jo["UK"]),
                                new JProperty("WorkState",jo["W"]),
                                new JProperty("ProcAudit",jo["P"]),
                                new JProperty("DeProcAudit",jo["D"]),
                                new JProperty("ProcTime",jo["C"]),
                                new JProperty("KeyWord",jo["K"]),
                                new JProperty("AuditRight",jo["R"]),
                                new JProperty("FinishDate",jo["FD"]),
                                new JProperty("FinishUser",jo["FU"]),
                                new JProperty("FinishBrach",jo["FB"]),
                                //new JProperty("O_WorkStateNo",jo["O_WorkStateNo"]),
                                new JProperty("Visible","True")

                            });

                            }
                        }
                        else if (jpMsg.Name == "WSB" && !string.IsNullOrEmpty(jpMsg.Value.ToString()) && jpMsg.Value.ToString() != "null")
                        {
                            JArray jaWsb = (JArray)(jpMsg.Value);
                            foreach (JObject jo in jaWsb)
                            {
                                jaResult.Add(new JObject {
                                new JProperty("BtnType","btn"),
                                new JProperty("Desc",jo["D"]),
                                new JProperty("KeyWord",jo["K"]),
                                new JProperty("Visible","False"),
                                new JProperty("Enabled",jo["E"])
                            });
                            }
                        }
                        //如果撤销提交或删除流程按钮是禁用状态，就不显示
                        else if (jpMsg.Name == "RebackText" && !string.IsNullOrEmpty(jpMsg.Value.ToString()) && jpMsg.Value.ToString() != "null" && RebackEnabled == "true")
                        {
                            string strRb = (string)(jpMsg.Value);
                            jaResult.Add(new JObject {
                                new JProperty("BtnType","reback"),
                                new JProperty("Desc",strRb),
                                new JProperty("KeyWord",strRbKeyword),
                                new JProperty("Visible","False"),
                                new JProperty("Enabled",RebackEnabled=="true"?"T":"F")
                            });
                        }
                        else if (jpMsg.Name == "RetractText" && !string.IsNullOrEmpty(jpMsg.Value.ToString()) && jpMsg.Value.ToString() != "null" && RetractEnabled == "true")
                        {
                            string strRt = (string)(jpMsg.Value);
                            jaResult.Add(new JObject {
                                new JProperty("BtnType","reback"),
                                new JProperty("Desc",strRt),
                                new JProperty("KeyWord",strRtKeyword),
                                new JProperty("Visible","False"),
                                new JProperty("Enabled",RetractEnabled=="true"?"T":"F")
                            });
                        }
                        else if (jpMsg.Name == "DefaultAuditList" && !string.IsNullOrEmpty(jpMsg.Value.ToString()) && jpMsg.Value.ToString() != "null" && UserStatus == "Check")
                        {
                            string strRb = (string)(jpMsg.Value);
                            jaResult.Add(new JObject {
                                new JProperty("BtnType","addAduit"),
                                new JProperty("Desc",strRb),
                                new JProperty("KeyWord",""),
                                new JProperty("Visible","False")
                            });
                        }
                        else if (jpMsg.Name == "UserString" && !string.IsNullOrEmpty(jpMsg.Value.ToString()) && jpMsg.Value.ToString() != "null")
                        {
                            userString = (string)(jpMsg.Value);

                        }
                    }
                    string wfKeyword = workflow.KeyWord;
                    string isFinish = "False";
                    if (workflow.O_WorkFlowStatus == AVEVA.CDMS.Server.enWorkFlowStatus.Finish)
                    {
                        isFinish = "True";
                    }

                    //流程状态用户列表
                    jaResult.Add(new JObject {
                                new JProperty("UserString",userString),
                                new JProperty("Visible","False")
                            });

                    //流程Keyword
                    jaResult.Add(new JObject {
                                new JProperty("WfKeyword",wfKeyword),
                                new JProperty("isFinish",isFinish),
                                new JProperty("Visible","False")
                            });

                    reJo.data = jaResult;
                    reJo.success = true;
                }
                catch (Exception e)
                {
                    CommonController.WebWriteLog(e.Message);
                    reJo.msg = e.Message;
                }
                finally
                {
                    muxConsole.ReleaseMutex();
                }

            }
            catch (Exception ex)
            {
                CommonController.WebWriteLog(ex.Message);
                reJo.msg = ex.Message;
            }

            return reJo.Value;

        }

        /// <summary>
        /// 返回流程页信息
        /// </summary>  
        /// <param name="sid">连接秘钥</param>
        ///  <param name="Keyword">DOC或者Project或者WorkFlow的关键字</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时，data包含一个JObject，里面包含参数"state"；</para>
        /// <para>操作成功时，data包含多个JObject，JObject的值又分为多种类型：</para>
        /// <para>类型一：返回附件的文档列表,包含参数："code"，"desc"，"o_filename","o_size","o_credatetime","o_updatetime","o_status","attrKeyword","attrType"值为("Doc"）,"PageId"值为("1")</para>
        /// <para>类型二：返回附件的目录列表,包含参数："code"，"desc"，"o_credatetime","o_updatetime","o_status","attrKeyword","attrType"值为("Project"）,"PageId"值为("1")</para>
        /// <para>类型三：返回当前流程模板,包含参数："WorkState"：值为当前流程模板代码,"desc"值为("DefWorkFlowCode")</para>
        /// <para>类型四：返回当前流程描述,包含参数："WorkState"：值为当前流程描述,"desc"值为("DefWorkFlowDesc")，"PageId"值为("2")</para>
        /// <para>类型五：返回流程校审人员列表(当前流程状态时),包含参数："WorkState"：值为用户流程状态,"UserName"，"PageId"值为("2")</para>
        /// <para>类型六：返回流程校审人员列表(不是当前流程状态时),包含参数："UserName"，"PageId"值为("2")</para>
        /// <para>类型七：返回流程历史列表(不是当前流程状态时),包含参数："UserName"，"WorkState"值为流程操作，"SendDate"：日期，"PageId"值为("3")</para>
        /// <para>例子：</para>
        /// <code>
        /// {
        ///  "success": true,
        ///  "total": 0,
        ///  "msg": "",
        ///  "data": [
        ///    {               //流程附件 , 显示在流程--属性子页面
        ///      "code": "7.填报工时.txt",      //代码
        ///      "desc": "",                    //描述
        ///      "o_filename": "7.填报工时.txt",    //文件名
        ///      "o_size": "339",                   //文件大小
        ///      "o_credatetime": "2020/1/20 14:18:13",
        ///      "o_updatetime": "2020/1/20 14:18:35",
        ///      "o_status": "IN",          //对象状态，IN是检入状态
        ///      "attrKeyword": "GJEPCMSP2968D17",  //对象的关键字
        ///      "attrType": "Doc",     //对象类型，
        ///      "PageId": "1"          //当PageId为2时，显示在流程--校审意见子页面--里面的列表，即流程附件列表
        ///    },
        ///    {
        ///      "WorkState": "SENDDOC",        
        ///      "desc": "DefWorkFlowCode"      //当"desc"为"DefWorkFlowCode"时，上面的"WorkState"，传递的是流程模板的代码, 显示在流程--属性子页面
        ///    },
        ///    {
        ///      "WorkState": "发文工作流程",
        ///      "desc": "DefWorkFlowDesc",     //当"desc"为"DefWorkFlowDesc"时，上面的"WorkState"，传递的是流程模板的描述, 显示在流程--属性子页面
        ///      "PageId": "2"                  //PageId为2时，显示在流程--流程子页面--里面的列表
        ///    },
        ///    {
        ///      "WorkState": "DESIGN__设计",         //流程状态
        ///      "WorkStateKeyword": "GJEPCMSW13136S59400", //流程工作状态Keyword
        ///      "UserName": "admin__administrator",        //流程状态工作用户名
        ///      "UserKeyword": "GJEPCMSU1",                //流程状态工作用户Keyword
        ///      "FinishDate": "",                          //流程状态通过日期
        ///      "PageId": "2"                              //PageId为2时，显示在流程--流程子页面--里面的列表
        ///    },
        ///    {
        ///      "WorkState": "DIRECTORSELECT__主任 [←当前状态]",
        ///      "WorkStateKeyword": "GJEPCMSW13136S59401",
        ///      "UserName": "admin__administrator",
        ///      "UserKeyword": "GJEPCMSU1",
        ///      "FinishDate": "",
        ///      "PageId": "2"                  
        ///    },
        ///    {
        ///      "UserName": "admin__administrator",
        ///      "WorkState": "发起流程",
        ///      "SendDate": "2020/1/20 14:18:35",
        ///      "PageId": "3"                  //PageId为3时，显示在流程--历史--里面的列表
        ///    }
        ///  ]
        /// }
        /// </code>
        /// </returns>

        public static JObject GetWorkFlowPagesData(string sid, string Keyword)
        {
            ExReJObject reJo = new ExReJObject();
            try
            {
                Keyword = Keyword ?? "";
                if (string.IsNullOrEmpty(Keyword) || string.IsNullOrEmpty(sid))
                {
                    reJo.msg = "错误的提交数据。";
                    return reJo.Value;
                }


                //登录用户
                User curUser = DBSourceController.GetCurrentUser(sid);
                if (curUser == null) return CommonController.SidError();

                DBSource dbsource = curUser.dBSource;

                Object obj = null;

                obj = dbsource.GetObjectByKeyWord(Keyword);

                if (obj == null) return new JObject();
                WorkFlow curWorkflow;
                Message curMessage;
                Doc curDoc;
                Project curProject;
                if (obj is MsgUser)
                {
                    curMessage = ((MsgUser)obj).message;
                    curWorkflow = curMessage.workFlow;
                }
                else if (obj is Message)
                {
                    curMessage = (Message)obj;
                    curWorkflow = curMessage.workFlow;
                }
                else if (obj is Doc)
                {
                    Doc ddoc = (Doc)obj;
                    curDoc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;
                    curWorkflow = curDoc.WorkFlow;
                }
                else if (obj is Project)
                {
                    curProject = (Project)obj;
                    curWorkflow = curProject.WorkFlow;
                }
                else if (obj is WorkFlow)
                    curWorkflow = (WorkFlow)obj;
                else
                    return new JObject();

                if (curWorkflow == null) { return new JObject(); }

                #region 放置属性页面
                //放置属性页面
                JArray jaDocList = new JArray();
                JArray jaProjectList = new JArray();
                if ((curWorkflow.DocList != null) && (curWorkflow.DocList.Count > 0))
                {
                    foreach (Doc doc in curWorkflow.DocList)
                    {
                        jaDocList.Add(new JObject(new JProperty("code", doc.Code),
                                                  new JProperty("desc", doc.Description),
                                                  new JProperty("o_filename", doc.O_filename),
                                                  new JProperty("o_size", doc.O_size.ToString()),
                                                  new JProperty("o_credatetime", doc.O_credatetime.ToString()),
                                                  new JProperty("o_updatetime", doc.O_updatetime.HasValue ? doc.O_updatetime.Value.ToString() : ""),
                                                  new JProperty("o_dmsstatus", doc.O_dmsstatus.ToString()),
                                                  new JProperty("attrKeyword", doc.KeyWord),
                                                  new JProperty("attrType", "Doc")
                                                  ));
                    }
                }
                else if (curWorkflow.Project != null)
                {
                    jaProjectList.Add(new JObject(new JProperty("code", curWorkflow.Project.Code),
                          new JProperty("desc", curWorkflow.Project.Description),
                           new JProperty("o_credatetime", curWorkflow.Project.O_credatetime.ToString()),
                          new JProperty("o_updatetime", curWorkflow.Project.O_updatetime.HasValue ? curWorkflow.Project.O_updatetime.Value.ToString() : ""),
                          new JProperty("o_status", curWorkflow.Project.O_status.ToString()),
                          new JProperty("attrKeyword", curWorkflow.Project.KeyWord),
                          new JProperty("attrType", "Project")
                          ));
                }
                #endregion


                #region 放置流程状态
                //获取流程模板名称，添加流程模板节点


                JArray jaWorkStateList = new JArray();
                if ((curWorkflow.WorkStateList != null) && (curWorkflow.WorkStateList.Count > 0))
                {
                    int index = 0;

                    //放置流程状态

                    foreach (WorkState state in curWorkflow.WorkStateList)
                    {
                        index = index + 1;
                        if ((state.WorkUserList != null) && (state.WorkUserList.Count > 0))
                        {
                            JObject joWorkState = new JObject();
                            string strState = state.ToString;

                            //添加流程状态节点
                            //当状态是流程的当前状态，而且流程未结束时，标示为当前状态
                            if ((curWorkflow.CuWorkState == state) && !state.ToString.EndsWith("(流程结束)"))
                            {
                                strState = strState + " [←当前状态]";
                            }
                            else if (state.IsRuning)
                            {
                                strState = strState + " [运行]";
                            }
                            joWorkState.Add(new JProperty("State", strState));
                            joWorkState.Add(new JProperty("StateKeyword", state.KeyWord));

                            //在流程状态节点下添加用户子节点
                            //节点文本后面加上状态提示
                            JArray jaUserList = new JArray();

                            if ((state.WorkUserList != null) && (state.WorkUserList.Count > 0))
                            {
                                int indexUser = 0;
                                foreach (WorkUser user in state.WorkUserList)
                                {
                                    JObject joUserList = new JObject();
                                    indexUser = indexUser + 1;

                                    string str = "";
                                    string strUser = "";

                                    try
                                    {
                                        var wb = user.workStateBranch;
                                        //if (((user.workStateBranch != null) && (user.GetHistoryList() != null)) && (user.GetHistoryList().Count > 0))
                                        if (((wb != null) && (user.GetHistoryList() != null)) && (user.GetHistoryList().Count > 0))

                                        {
                                            //str = "[" + user.workStateBranch.defStateBrach.O_Description + "]";
                                            str = "[" + wb.defStateBrach.O_Description + "]";
                                        }

                                    }
                                    catch (Exception exGetBranch)
                                    {
                                        string strEx = exGetBranch.Message;
                                    }

                                    if (user.O_pass.HasValue && user.O_pass.Value)
                                    {
                                        str = str + "[通过]";
                                    }

                                    strUser = (user.User != null) ? string.Format("{0}{1}{2}", (user.O_indx > 0) ? (user.O_indx.ToString() + ".") : "", user.User.ToString, str) : "";


                                    //流程状态节点添加子节点，内容是用户和状态提示
                                    joUserList.Add(new JProperty("User", strUser));
                                    joUserList.Add(new JProperty("UserKeyword", user.User.KeyWord));
                                    jaUserList.Add(joUserList);
                                }
                            }

                            joWorkState.Add(new JProperty("UserList", jaUserList));
                            joWorkState.Add(new JProperty("FinishDate", state.O_FinishDate.ToString()));

                            jaWorkStateList.Add(joWorkState);

                        }
                    }


                }
                #endregion



                #region 放置流程历史ListView
                //放置流程历史ListView
                JArray jaWfHistory = new JArray();
                if (((curWorkflow.WorkStateList.Count > 0) && (curWorkflow.WorkStateList[0] != null)) && ((curWorkflow.WorkStateList[0].WorkUserList.Count > 0) && (curWorkflow.WorkStateList[0].WorkUserList[0].User != null)))
                {
                    JObject joWfHistory = new JObject();
                    joWfHistory.Add(new JProperty("User", curWorkflow.WorkStateList[0].WorkUserList[0].User.ToString));
                    joWfHistory.Add(new JProperty("Operation", "发起流程"));
                    joWfHistory.Add(new JProperty("SendDate", curWorkflow.O_CreateDate.ToString()));

                    jaWfHistory.Add(joWfHistory);
                }

                if ((curWorkflow.AllAuditList != null) && (curWorkflow.AllAuditList.Count > 0))
                {
                    foreach (Audit audit in curWorkflow.AllAuditList)
                    {
                        JObject joWfHistory = new JObject();
                        joWfHistory.Add(new JProperty("User", (audit.user != null) ? audit.user.ToString : ""));
                        joWfHistory.Add(new JProperty("Operation", audit.O_comments));
                        joWfHistory.Add(new JProperty("SendDate", audit.O_acttime.ToString()));
                        jaWfHistory.Add(joWfHistory);
                    }
                }
                #endregion

                JArray jaResult = new JArray();
                //返回流程属性页
                foreach (var ss in jaDocList)
                {
                    JObject joUserList = (JObject)ss;
                    jaResult.Add(new JObject {
                        new JProperty("code",joUserList["code"]),
                        new JProperty("desc",joUserList["desc"]),
                        new JProperty("o_filename",joUserList["o_filename"]),
                        new JProperty("o_size",joUserList["o_size"]),
                        new JProperty("o_credatetime",joUserList["o_credatetime"]),
                        new JProperty("o_updatetime",joUserList["o_updatetime"]),
                        new JProperty("o_status",joUserList["o_dmsstatus"]),
                        new JProperty("attrKeyword",joUserList["attrKeyword"]),
                        new JProperty("attrType",joUserList["attrType"]),
                        new JProperty("PageId","1")
                    });

                }

                foreach (var ss in jaProjectList)
                {
                    JObject joUserList = (JObject)ss;
                    jaResult.Add(new JObject {
                        new JProperty("code",joUserList["code"]),
                        new JProperty("desc",joUserList["desc"]),
                        new JProperty("o_credatetime",joUserList["o_credatetime"]),
                        new JProperty("o_updatetime",joUserList["o_updatetime"]),
                        new JProperty("o_status",joUserList["o_status"]),
                        new JProperty("attrKeyword",joUserList["attrKeyword"]),
                        new JProperty("attrType",joUserList["attrType"]),
                        new JProperty("PageId","1")
                    });
                }
                jaResult.Add(new JObject {
                        new JProperty("WorkState",curWorkflow.DefWorkFlow.O_Code),
                        new JProperty("desc","DefWorkFlowCode")
                    });

                #region 返回流程的流程页
                jaResult.Add(new JObject {
                        new JProperty("WorkState",curWorkflow.DefWorkFlow.O_Description),
                        new JProperty("desc","DefWorkFlowDesc"),
                        new JProperty("PageId","2")
                    });


                foreach (var ss in jaWorkStateList)
                {
                    JObject joWorkStateList = (JObject)ss;
                    JArray jaUserList = (JArray)joWorkStateList["UserList"];
                    int index = 1;
                    foreach (var ssUser in jaUserList)
                    {
                        JObject joUser = (JObject)ssUser;
                        if (index == 1)
                        {
                            jaResult.Add(new JObject {
                                new JProperty("WorkState",joWorkStateList["State"]),
                                new JProperty("WorkStateKeyword",joWorkStateList["StateKeyword"]),
                                new JProperty("UserName",joUser["User"]),
                                new JProperty("UserKeyword",joUser["UserKeyword"]),
                                new JProperty("FinishDate",joWorkStateList["FinishDate"]),
                                new JProperty("PageId","2")
                            });
                        }
                        else
                        {
                            jaResult.Add(new JObject {
                                new JProperty("WorkStateKeyword",joWorkStateList["StateKeyword"]),
                                new JProperty("UserName",joUser["User"]),
                                new JProperty("UserKeyword",joUser["UserKeyword"]),
                                new JProperty("PageId","2")
                            });
                        }

                        index = index + 1;
                    }
                }
                #endregion

                foreach (var ss in jaWfHistory)
                {
                    JObject joWfHistory = (JObject)ss;
                    jaResult.Add(new JObject {
                                new JProperty("UserName",joWfHistory["User"]),
                                new JProperty("WorkState",joWfHistory["Operation"]),
                                new JProperty("SendDate",joWfHistory["SendDate"]),
                                new JProperty("PageId","3")
                            });
                }

                reJo.data = jaResult;
                reJo.success = true;

            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
                reJo.msg = e.Message;
            }
            return reJo.Value;
        }

        /// <summary>
        /// 启动新流程
        /// </summary>
        /// <param name="sid">登录关键字</param>
        /// <param name="DocList"> Doc列表，用','分割</param>
        /// <param name="WfKeyword">流程模板关键字</param>
        /// <param name="userlist">可选，用户关键字列表，用','分割</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时，data包含一个JObject，里面包含参数"state"</para>
        /// <para>当 "msg"的值为"selectDefWorkFlow"时，流程列表有多个流程，通知客户端弹出流程选择界面</para>
        /// <para>当 "msg"的值为"selectUser"时，提示需要选择下一状态的校审人员</para>
        /// <para>当 "state"的值为"Passr"时，表示新建流程成功</para>
        /// <para>例子：</para>
        /// </returns>
        [HttpPost]
        public static JObject StartNewWorkFlow(string sid, string DocList, string WfKeyword, string userlist)//注意这里的userlist是用户的Keyword,不是Desc
        {

            ExReJObject reJo = new ExReJObject();
            try
            {

                if (string.IsNullOrEmpty(sid) || string.IsNullOrEmpty(DocList))
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

                string strMsg = string.Empty;
                JArray jaGetList = new JArray();


                //登录用户
                User curUser = DBSourceController.GetCurrentUser(sid);
                if (curUser == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！"; return reJo.Value;
                }

                //获取目录或文档对象列表
                //List<Object> objList = new List<object>();
                string[] strArray = (string.IsNullOrEmpty(DocList) ? "" : DocList).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                DBSource dbsource = curUser.dBSource;

                //把流程模板关键字(Keyword)转换为流程模板的o_code
                DefWorkFlow defwf = dbsource.AllDefWorkFlowList.Find(d => d.KeyWord == WfKeyword);
                if (defwf!=null)
                {
                    WfKeyword = defwf.O_Code;
                }

                //转换为ProjectList
                List<Project> projectList = new List<Project>();

                //转换为DocList
                List<Doc> docList = new List<Doc>();

                //转换为对象列表
                //List<Object> objList = new List<Object>();

                foreach (string strObj in strArray)
                {
                    object obj = dbsource.GetObjectByKeyWord(strObj);
                    //objList.Add(obj);
                    if (obj is Doc)
                    {
                        Doc ddoc = (Doc)obj;
                        Doc doc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;
                        docList.Add((Doc)doc);
                    }
                    else if (obj is Project)
                    {
                        projectList.Add((Project)obj);
                    }
                }

                JObject joResult = new JObject();



                //文件为空
                if (docList.Count == 0 && projectList.Count == 0)
                {
                    reJo.msg = "请选择文档";
                    return reJo.Value;
                }


                //判断文件列表是否检入状态、最终状态或锁定状态
                if (projectList.Count > 0)
                {
                    //处理Project的流程
                    reJo.Value = ManageProjectWorkFlow(curUser, projectList, WfKeyword, userlist, true);

                    if (reJo.success == true && reJo.data.Count > 0)
                    {

                        joResult = (JObject)reJo.data[0];

                        //如果是PASS状态，就刷新数据源
                        if (joResult.Property("state") != null && ((string)joResult.Property("state")) == "Pass")
                        {
                            //刷新数据源，让文档的附加属性显示出来，流程才能获取最新的附加属性，然后根据附加属性来判断流程分支
                            DBSourceController.RefreshDBSource(sid);
                        }
                    }
                    else
                    {
                        return reJo.Value;
                        //reJo.msg="创建文件夹流程出错！";
                    }

                    return reJo.Value;
                }
                else
                {
                    foreach (Doc doc in docList)
                    {

                        if ((((doc != null) && (doc.OperateDocStatus != enDocStatus.IN)) && ((doc.OperateDocStatus != enDocStatus.IN_FINAL) && (doc.OperateDocStatus != enDocStatus.COMING_IN))) && (doc.OperateDocStatus != enDocStatus.GOING_OUT))
                        {
                            reJo.msg = doc.ToString + "不是检入状态、最终状态或锁定状态,不能提交流程分支\r\n";
                            return reJo.Value;
                        }




                        //选择定义流程
                        if (string.IsNullOrEmpty(WfKeyword))
                        {
                            List<DefWorkFlow> dwfList = getDefWorkFlowList(docList);
                            if (dwfList.Count == 1)
                            {
                                //如果流程列表只有一个流程，就启动流程
                                WfKeyword = dwfList[0].O_Code;
                            }
                            else
                            {
                                JArray dwfJa = new JArray();
                                foreach (DefWorkFlow dwf in dwfList)
                                {
                                    JObject joDwf = new JObject(new JProperty("id", dwf.KeyWord), new JProperty("text", dwf.O_Description), new JProperty("o_code", dwf.O_Code));
                                    dwfJa.Add(joDwf);
                                }

                                //reJo.success = true;
                                reJo.msg = "selectDefWorkFlow";
                                //如果流程列表有多个流程，就通知JS端弹出流程选择界面
                                reJo.data = new JArray(new JObject(new JProperty("state", "selectDefWorkFlow"), new JProperty("dwfList", dwfJa)));
                                return reJo.Value;
                            }
                        }



                        //启动流程
                        //定义新流程
                        WorkFlow newWF = dbsource.NewWorkFlow(docList, WfKeyword, true);


                        //创建流程失败
                        if (newWF == null || newWF.CuWorkState == null)
                        {
                            string mmsg = "建立流程出错,用户：" + dbsource.LoginUser.Code + ",文件名：" + docList[0].O_itemname + ",流程：" + WfKeyword + ",Error:" + dbsource.LastError;
                            AVEVA.CDMS.WebApi.CommonController.WebWriteLog(dbsource.LastError);

                            //如果文件残留了流程，就删除残留流程
                            if (docList[0].WorkFlow != null)
                            {
                                docList[0].WorkFlow.Delete();
                                docList[0].WorkFlow.Delete();
                            }

                            reJo.msg = mmsg;
                            return reJo.Value;
                        }


                        WorkStateBranch selWorkStateBranch = null;
                        if (newWF.CuWorkState == null || newWF.CuWorkState.workStateBranchList == null || newWF.CuWorkState.workStateBranchList.Count <= 0)
                        {
                            newWF.Delete();
                            newWF.Delete();
                            reJo.msg = "请必须选择下一状态的校审人员,否则流程无法进行下去!";
                            return reJo.Value;
                        }



                        //1.新建流程，2.选择流程分支，3.判断是否被锁定
                        //选择流程分支，如果是开始新的流程，选择第一个分支
                        selWorkStateBranch = SelectWorkStateBranch(newWF.CuWorkState.workStateBranchList);

                        if (selWorkStateBranch == null)
                        {
                            newWF.Delete();
                            newWF.Delete();
                            reJo.msg = "未选择合适分支,或者不存在符合当前对象的分支,流程启动取消!";
                            return reJo.Value;
                        }

                        //判断当前流程已被其他用户操作
                        if (dbsource.dBManager.IsExistUserData(selWorkStateBranch.KeyWord) || (newWF.UserStatus == enWKUserStatus.View))
                        {
                            //流程分支清除用户编辑状态
                            dbsource.dBManager.DeleteUserData(selWorkStateBranch.KeyWord);
                            bool flag = dbsource.dBManager.IsExistUserData(selWorkStateBranch.KeyWord);
                        }



                        //从流程模板中获取公式，定义流程下一状态的用户
                        //如果定义流程的定义状态中存在需要选择专业的定义状态,则需要提前选择专业及人员,并实例相应的状态
                        if (!SelProfessionOnWorkFlowStart(newWF, userlist))
                        {
                            newWF.Delete();
                            newWF.Delete();
                            reJo.msg = "分配校审人员失败,是否继续流程?";
                            return reJo.Value;
                        }

                        //获取下一状态的用户
                        if (selWorkStateBranch != null)
                        {
                            reJo = WebWorkFlowEvent.GotoNextStateAndSelectUser(selWorkStateBranch, userlist);

                            joResult = (JObject)reJo.data[0];

                            //如果不是PASS状态，就删除未建好的工作流
                            if (!(joResult.Property("state") != null && ((string)joResult.Property("state")) == "Pass"))
                            {
                                newWF.Delete();
                                newWF.Delete();
                            }


                            //刷新数据源，让文档的附加属性显示出来，流程才能获取最新的附加属性，然后根据附加属性来判断流程分支
                            DBSourceController.RefreshDBSource(sid);

                            reJo.success = true;
                            return reJo.Value;
                        }
                    }
                }

            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
                reJo.msg = e.Message;
            }
            return reJo.Value;
        }

        /// <summary>
        /// 处理Project的流程
        /// </summary>
        /// <param name="curUser"></param>
        /// <param name="projectList"></param>
        /// <param name="WfKeyword"></param>
        /// <param name="userlist"></param>
        /// <param name="bNewWorkFlow"></param>
        /// <returns></returns>
        internal static JObject ManageProjectWorkFlow(User curUser, List<Project> projectList, string WfKeyword, string userlist, bool bNewWorkFlow)
        {
            ExReJObject reJo = new ExReJObject();

            try
            {
                DBSource dbsource = curUser.dBSource;

                //选择定义流程
                if (string.IsNullOrEmpty(WfKeyword))
                {
                    List<DefWorkFlow> dwfList = getDefWorkFlowList(projectList);
                    if (dwfList.Count == 1)
                    {
                        //如果流程列表只有一个流程，就启动流程
                        WfKeyword = dwfList[0].O_Code;
                    }
                    else
                    {
                        JArray dwfJa = new JArray();
                        foreach (DefWorkFlow dwf in dwfList)
                        {
                            JObject joDwf = new JObject(new JProperty("id", dwf.KeyWord), new JProperty("text", dwf.O_Description), new JProperty("o_code", dwf.O_Code));
                            dwfJa.Add(joDwf);
                        }

                        //reJo.success = true;
                        reJo.msg = "selectDefWorkFlow";
                        //如果流程列表有多个流程，就通知JS端弹出流程选择界面
                        reJo.data = new JArray(new JObject(new JProperty("state", "selectDefWorkFlow"), new JProperty("dwfList", dwfJa)));
                        return reJo.Value;
                    }
                }

                //WorkFlow newWF = dbsource.NewWorkFlow(projectList[0], fm->selDefWorkFlow->O_Code, bAttaDoc);
                //启动流程
                //定义新流程
                WorkFlow newWF = dbsource.NewWorkFlow(projectList[0], WfKeyword, true);


                //创建流程失败
                if (newWF == null || newWF.CuWorkState == null)
                {
                    string mmsg = "建立流程出错,用户：" + dbsource.LoginUser.Code + ",文件名：" + projectList[0].ToString + ",流程：" + WfKeyword + ",Error:" + dbsource.LastError;
                    AVEVA.CDMS.WebApi.CommonController.WebWriteLog(dbsource.LastError);

                    //如果文件夹残留了流程，就删除残留流程
                    if (projectList[0].WorkFlow != null)
                    {
                        projectList[0].WorkFlow.Delete();
                        projectList[0].WorkFlow.Delete();
                    }

                    reJo.msg = mmsg;
                    return reJo.Value;
                }


                WorkStateBranch selWorkStateBranch = null;
                if (newWF.CuWorkState == null || newWF.CuWorkState.workStateBranchList == null || newWF.CuWorkState.workStateBranchList.Count <= 0)
                {
                    newWF.Delete();
                    newWF.Delete();
                    reJo.msg = "请必须选择下一状态的校审人员,否则流程无法进行下去!";
                    return reJo.Value;
                }



                //1.新建流程，2.选择流程分支，3.判断是否被锁定
                //选择流程分支，如果是开始新的流程，选择第一个分支
                selWorkStateBranch = SelectWorkStateBranch(newWF.CuWorkState.workStateBranchList);

                if (selWorkStateBranch == null)
                {
                    newWF.Delete();
                    newWF.Delete();
                    reJo.msg = "未选择合适分支,或者不存在符合当前对象的分支,流程启动取消!";
                    return reJo.Value;
                }

                //判断当前流程已被其他用户操作
                if (dbsource.dBManager.IsExistUserData(selWorkStateBranch.KeyWord) || (newWF.UserStatus == enWKUserStatus.View))
                {
                    //流程分支清除用户编辑状态
                    dbsource.dBManager.DeleteUserData(selWorkStateBranch.KeyWord);
                    bool flag = dbsource.dBManager.IsExistUserData(selWorkStateBranch.KeyWord);
                }



                //从流程模板中获取公式，定义流程下一状态的用户
                //如果定义流程的定义状态中存在需要选择专业的定义状态,则需要提前选择专业及人员,并实例相应的状态
                if (!SelProfessionOnWorkFlowStart(newWF, userlist))
                {
                    newWF.Delete();
                    newWF.Delete();
                    reJo.msg = "分配校审人员失败,是否继续流程?";
                    return reJo.Value;
                }

                JObject joResult = new JObject();

                //获取下一状态的用户
                if (selWorkStateBranch != null)
                {
                    reJo = WebWorkFlowEvent.GotoNextStateAndSelectUser(selWorkStateBranch, userlist);

                    joResult = (JObject)reJo.data[0];

                    //如果不是PASS状态，就删除未建好的工作流
                    if (!(joResult.Property("state") != null && ((string)joResult.Property("state")) == "Pass"))
                    {
                        newWF.Delete();
                        newWF.Delete();
                    }


                    //刷新数据源，让文档的附加属性显示出来，流程才能获取最新的附加属性，然后根据附加属性来判断流程分支
                    //DBSourceController.RefreshDBSource(sid);

                    reJo.success = true;
                    return reJo.Value;
                }

            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
                reJo.msg = e.Message;
            }

            return reJo.Value;
        }

        /// <summary>
        /// 获取可用流程模板列表
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="DocList">DOC列表,用","号分割</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时，data包含多个JObject，每个个JObject里面包含参数"id"：流程模板的KeyWord，"text"：流程模板的描述，"o_code":流程模板的代码</para>
        /// <para>例子：</para>
        /// </returns>
        public static JObject GetDefWorkFlowList(string sid, string DocList)
        {

            ExReJObject reJo = new ExReJObject();
            try
            {
                if (string.IsNullOrEmpty(sid) || string.IsNullOrEmpty(DocList))
                {
                    reJo.msg = "错误的提交数据。";
                    return reJo.Value;
                }


                //登录用户
                User curUser = DBSourceController.GetCurrentUser(sid);
                if (curUser == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！"; return reJo.Value;
                }

                DBSource dbsource = curUser.dBSource; //.NewDBSource();//DBSource dbsource = DBSourceController.dbManager.shareDBManger.DBSourceList[0].ShareLoaclDBSource;//
                if (dbsource == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }



                JArray reJa = new JArray();

                //获取目录或文档对象列表
                List<Object> objList = new List<object>();
                string[] strArray = (string.IsNullOrEmpty(DocList) ? "" : DocList).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string strObj in strArray)
                {
                    object obj = dbsource.GetObjectByKeyWord(strObj);

                    if (obj == null) return null;

                    //转换JSON字符串
                    objList.Add(obj);
                }
                List<DefWorkFlow> dwfList = getDefWorkFlowList(objList);


                foreach (DefWorkFlow dwf in dwfList)
                {
                    JObject joDwf = new JObject(new JProperty("id", dwf.KeyWord), new JProperty("text", dwf.O_Description), new JProperty("o_code", dwf.O_Code));
                    reJa.Add(joDwf);
                }
                reJo.data = reJa;

                reJo.success = true;
            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(e.Message);
            }
            return reJo.Value;

        }


        /// <summary>
        /// 判断一组目录或者文档能采用的流程
        /// </summary>
        /// <param name="docList">Doc List</param>
        /// <returns></returns>
        internal static List<DefWorkFlow> getDefWorkFlowList(List<Doc> docList)
        {
            List<Object> objList = new List<Object>();
            foreach (Doc doc in docList)
            {
                objList.Add(doc);
            }
            return getDefWorkFlowList(objList);
        }

        /// <summary>
        /// 判断一组目录或者文档能采用的流程
        /// </summary>
        /// <param name="docList">Doc List</param>
        /// <returns></returns>
        internal static List<DefWorkFlow> getDefWorkFlowList(List<Project> projectList)
        {
            List<Object> objList = new List<Object>();
            foreach (Project prjoect in projectList)
            {
                objList.Add(prjoect);
            }
            return getDefWorkFlowList(objList);
        }

        /// <summary>
        /// 判断一组目录或者文档能采用的流程
        /// </summary>
        /// <param name="objList">Doc Project List</param>
        /// <returns></returns>
        internal static List<DefWorkFlow> getDefWorkFlowList(List<Object> objList)
        {

            List<DefWorkFlow> resultList = new List<DefWorkFlow>();
            try
            {
                if (objList == null || objList.Count() == 0) return null;
                DBSource dbs = objList[0] is Doc ? ((Doc)objList[0]).dBSource : ((Project)objList[0]).dBSource;

                if (dbs == null || dbs.IsLogin == false || dbs.AllDefWorkFlowList == null || dbs.AllDefWorkFlowList.Count <= 0)
                    return null;

                foreach (DefWorkFlow dWF in dbs.AllDefWorkFlowList)
                {
                    //根据表达式，如果不需要显示则跳过
                    if (!IsDefnWorkFlowNeedShow(objList, dWF))
                        continue;

                    if (!resultList.Contains(dWF))
                        resultList.Add(dWF);
                }
                return resultList;
            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
            }

            return new List<DefWorkFlow>();
        }



        /// <summary>
        /// 判断流程模板是否需要显示
        /// </summary>
        /// <param name="objList">doc project List</param>
        /// <param name="defWorkFlow">DefWorkFlow</param>
        /// <returns>DefWorkFlow</returns>
        private static bool IsDefnWorkFlowNeedShow(List<Object> objList, DefWorkFlow defWorkFlow)
        {
            try
            {
                if (objList == null || objList.Count <= 0)
                    return true;

                if (defWorkFlow == null)
                    return false;

                if (String.IsNullOrEmpty(defWorkFlow.O_Condtion))
                    return true;

                Project project = null;
                Doc doc = null;

                //所选择的对象只要有一个对象符合流程条件，就返回真
                foreach (Object obj in objList)
                {
                    if (obj is Project)
                    {
                        project = obj as Project;
                        if (project != null)
                            return IsObjectMathCondition(project, defWorkFlow.O_Condtion);
                    }
                    else if (obj is Doc)
                    {
                        doc = obj as Doc;
                        if (doc != null)
                        {
                            if (IsObjectMathCondition(doc, defWorkFlow.O_Condtion))
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;

                //if (project == null && doc == null)
                //    return true;


                //if (project != null)
                //    return IsObjectMathCondition(project, defWorkFlow.O_Condtion);
                //else if (doc != null)
                //    return IsObjectMathCondition(doc, defWorkFlow.O_Condtion);

            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
            }
            return true;
        }


        /// <summary>
        /// 根据新建的目录或者文档中模板的条件表达式,判断新建的目录或者文档对象是否符合条件
        /// </summary>
        /// <param name="projectOrDoc">Project或者Doc对象</param>
        /// <param name="express">表达式</param>
        /// <returns></returns>
        private static bool IsObjectMathCondition(Object projectOrDoc, String express)
        {
            try
            {
                if (projectOrDoc == null)
                    return false;

                if (string.IsNullOrEmpty(express))
                    return true;



                Project project = null;
                Doc doc = null;

                if (projectOrDoc is Project)
                {
                    project = (Project)projectOrDoc;
                }
                else if (projectOrDoc is Doc)
                {
                    doc = (Doc)projectOrDoc;
                }

                //判断新建对象是否符合条件表达式
                String[] conCode = null;
                String[] conDesc = null;

                if (project != null)
                {

                    conCode = project.ExcuteDefnExpression(express);
                    conDesc = project.ExcuteDefnExpression(express);

                }
                else if (doc != null)
                {

                    conCode = doc.ExcuteDefnExpression(express);
                    conDesc = doc.ExcuteDefnExpression(express);
                }

                if (conCode != null && conCode.Length > 0 && !String.IsNullOrEmpty(conCode[0].Trim()))
                {
                    if (AVEVA.CDMS.Server.CDMS.EnvalExpression(conCode[0]).ToUpper() != "TRUE")
                    {
                        return false;
                    }
                }
                if (conDesc != null && conDesc.Length > 0 && !String.IsNullOrEmpty(conDesc[0].Trim()))
                {
                    if (AVEVA.CDMS.Server.CDMS.EnvalExpression(conDesc[0]).ToUpper() != "TRUE")
                    {
                        return false;
                    }
                }

            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
                return false;
            }
            return true;
        }



        /// <summary>
        /// 添加流程意见
        /// 参数ProcAudit:流程意见
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="Keyword">流程关键字</param>
        /// <param name="ProcAudit">流程意见</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时，data包含一个空的JObject</para>
        /// <para>例子：</para>
        /// </returns>
        //[HttpGet]
        public static JObject AddWorkflowAudit(string sid, string Keyword, string ProcAudit)//, string DeProcAudit, string CheckerName, string WorkStateDesc, string CheckTime)
        {
            /// <param name="DeProcAudit">修改意见</param>
            /// <param name="CheckerName">签署人名字</param>
            /// <param name="WorkStateDesc">流程状态描述</param>
            /// <param name="CheckTime">意见签署时间</param>

            ExReJObject reJo = new ExReJObject();

            try
            {
                if (string.IsNullOrEmpty(sid) || string.IsNullOrEmpty(Keyword))
                {
                    reJo.msg = "错误的提交数据,流程不存在。";
                    return reJo.Value;
                }

                if (string.IsNullOrEmpty(ProcAudit))
                {
                    reJo.msg = "请填写意见！";
                    return reJo.Value;
                }


                //登录用户
                User curUser = DBSourceController.GetCurrentUser(sid);
                if (curUser == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

                DBSource dbsource = curUser.dBSource;


                //获取流程对象
                object obj = dbsource.GetObjectByKeyWord(Keyword);
                WorkFlow workflow = null;
                if (obj is Doc)
                {
                    Doc ddoc = (Doc)obj;
                    Doc doc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;
                    workflow = doc.WorkFlow;
                }
                else if (obj is Project)
                {
                    Project proj = (Project)obj;
                    workflow = proj.WorkFlow;
                }
                else if (obj is WorkFlow)
                {
                    workflow = (WorkFlow)obj;
                }
                if (workflow == null)
                {
                    reJo.msg = "提交数据错误,流程不存在。"; return reJo.Value;
                }


                if (workflow.UserStatus != enWKUserStatus.Check)
                {
                    reJo.msg = "流程用户状态不需要填写意见！";
                    return reJo.Value;
                }

                WorkAudit newAudit = workflow.NewAudit(ProcAudit, 0);

                if (newAudit == null)
                {
                    reJo.msg = "新建校审意见失败！";
                    return reJo.Value;
                }


                if (!workflow.AllWorkAuditList.Contains(newAudit))
                {
                    workflow.AllWorkAuditList.Add(newAudit);
                }
                reJo.success = true;
                return reJo.Value;

            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(e.Message);
            }
            return reJo.Value;
        }

        /// <summary>
        /// 编辑流程意见
        /// 参数ProcAudit:流程意见
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="WorkStateKeyword">流程状态关键字</param>
        /// <param name="ProcAudit">流程意见</param>
        /// <param name="DeProcAudit">修改意见</param>
        /// <param name="CheckerKeyword">签署人关键字</param>
        /// <param name="CheckDate">意见签署时间</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时，data包含一个空的JObject</para>
        /// <para>例子：</para>
        /// </returns>
        //[HttpGet]
        public static JObject ModiWorkflowAudit(string sid, string WorkStateKeyword, string CheckerKeyword, string CheckDate, string ProcAudit, string DeProcAudit)
        {
            //注意: 这里需要有流程状态关键字，校审人关键字，校审日期三个参数才能够定位到某条校审意见
            //因为同一个流程状态可能有多个校审人，同一个校审状态下的同一个校审人可能有多条校审意见
            ExReJObject reJo = new ExReJObject();

            try
            {
                if (string.IsNullOrEmpty(sid) || string.IsNullOrEmpty(WorkStateKeyword))
                {
                    reJo.msg = "填写意见失败,流程状态不存在。";
                    return reJo.Value;
                }

                if (string.IsNullOrEmpty(ProcAudit))
                {
                    reJo.msg = "请填写意见！";
                    return reJo.Value;
                }



                //登录用户
                User curUser = DBSourceController.GetCurrentUser(sid);
                if (curUser == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

                DBSource dbsource = curUser.dBSource;


                ////获取流程状态对象
                object obj = dbsource.GetObjectByKeyWord(WorkStateKeyword);
                WorkState workstate = null;
                if (obj is WorkFlow)
                {
                    //防止用流程状态关键字获取到的是流程的BUG

                    #region 尝试获取流程状态ID
                    string strCut = WorkStateKeyword.Substring(WorkStateKeyword.LastIndexOf("W") + 1);
                    strCut = WorkStateKeyword.Substring(WorkStateKeyword.LastIndexOf("S") + 1);

                    int WorkStateIdCut = Convert.ToInt32(strCut);
                    if (WorkStateIdCut <= 0)
                    {
                        reJo.msg = "填写意见失败,流程状态不存在。"; return reJo.Value;
                    }

                    WorkFlow wf = (WorkFlow)obj;
                    foreach (WorkState ws in wf.WorkStateList)
                    {
                        if (ws.ID == WorkStateIdCut)
                        {
                            workstate = ws;
                            break;
                        }
                    }
                    if (workstate == null)
                    {
                        reJo.msg = "填写意见失败,流程状态不存在。"; return reJo.Value;
                    }
                    #endregion
                }
                else if (obj is WorkState)
                {
                    workstate = (WorkState)obj;
                }
                else
                {
                    reJo.msg = "填写意见失败,流程状态不存在。"; return reJo.Value;
                }

                if (workstate == null)
                {
                    reJo.msg = "填写意见失败,流程状态不存在。"; return reJo.Value;
                }

                WorkFlow workflow = workstate.WorkFlow;

                User Checker = dbsource.GetUserByKeyWord(CheckerKeyword);
                if (Checker == null)
                {
                    reJo.msg = "填写意见失败,校审用户不存在！"; return reJo.Value;
                }


                List<WorkAudit> workAuditList = workflow.AllWorkAuditList;

                //校正时间格式
                DateTime dtCheckTime = Convert.ToDateTime(CheckDate);
                CheckDate = dtCheckTime.ToString();

                //定位到所在的意见
                //注意: 这里需要有流程状态关键字，校审人关键字，校审日期三个参数才能够定位到某条校审意见
                //因为同一个流程状态可能有多个校审人，同一个校审状态下的同一个校审人可能有多条校审意见
                WorkAudit workAudit = workflow.AllWorkAuditList.Find(wa =>
                    (wa.workUser.workState.KeyWord == WorkStateKeyword && wa.workUser.User.KeyWord == CheckerKeyword && wa.O_CheckDate.ToString() == CheckDate));

                if (workAudit == null)
                {
                    reJo.msg = "填写意见失败,校审意见不存在！"; return reJo.Value;
                }


                //校审人填写校审意见
                if (workflow.UserStatus != enWKUserStatus.View && workflow.CuWorkState.ID == workAudit.O_WorkStateNo)
                {
                    //校审意见
                    workAudit.O_Problom = ProcAudit;
                    workAudit.Modify();

                    reJo.success = true;
                    return reJo.Value;
                }
                //设计人填写修改意见
                else if (workflow.UserStatus == enWKUserStatus.Modify && workflow.CuWorkState.PreWorkState.ID == workAudit.O_WorkStateNo)
                {
                    //修改意见
                    workAudit.O_DesignerCom = DeProcAudit;
                    workAudit.O_ModifyDate = DateTime.Now;

                    workAudit.Modify();
                    reJo.success = true;
                    return reJo.Value;
                }
                else
                {
                    reJo.msg = "填写意见失败,不能编辑不是当前流程状态的意见！";
                    return reJo.Value;
                }



            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(e.Message);
            }
            return reJo.Value;
        }

        /// <summary>
        /// 跳转到下一流程状态
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="BrachKeyword">流程分支的关键字，可以是代码或描述</param>
        /// <param name="WfKeyword">流程关键字</param>
        /// <param name="userlist">下一状态用户列表,用","号分割</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时，data包含一个JObject，里面包含参数"state"；</para>
        /// <para>当 "msg"的值为"selectUser"时，提示需要选择下一状态的校审人员</para>
        /// <para>当 "state"的值为"Passr"时，表示新建流程成功</para>
        /// <para>例子：</para>
        /// </returns>
        public static JObject GotoNextWfState(string sid, string WfKeyword, string BrachKeyword, string userlist)
        {
            ExReJObject reJo = new ExReJObject();
            try
            {

                //登录用户
                User curUser = DBSourceController.GetCurrentUser(sid);
                if (curUser == null)
                {
                    reJo.msg = "登录失败！错误的账号密码";
                    return reJo.Value;
                }

                //获取数据源
                DBSource dbsource = curUser.dBSource;


                //获取目录对象
                Object obj = null;
                obj = dbsource.GetObjectByKeyWord(WfKeyword);


                if (obj == null)
                {
                    reJo.msg = "提交数据错误，对象不存在！";
                    return reJo.Value;
                }

                //规范反斜杠字符
                //btnText = btnText.Replace("\\",@"\");

                WorkFlow curWorkFlow = new WorkFlow();
                if (obj is WorkFlow)
                    curWorkFlow = (WorkFlow)obj;


                //定义流程分支
                WorkStateBranch wsBranch = null;


                if (wsBranch == null)
                {
                    //如果流程分支是空，就根据流程分支关键字获取流程分支
                    wsBranch = curWorkFlow.CuWorkState.workStateBranchList.Find(wsb => wsb.KeyWord == BrachKeyword);
                    //flow.CuWorkState.workStateBranchList
                }

                if (wsBranch == null)
                {
                    //如果流程分支是空，再根据按钮名字获取流程分支
                    wsBranch = curWorkFlow.CuWorkState.workStateBranchList.Find(wsb => wsb.defStateBrach.O_Description == BrachKeyword);
                    //flow.CuWorkState.workStateBranchList
                }

                if (wsBranch != null)
                {

                    #region 获取流程分支后的事件
                    foreach (WebWorkFlowEvent.After_WorkFlow_GetBanch_Event_Class AfterWorkFlowGetBanchEvent in WebWorkFlowEvent.ListAfterWorkFlowGetBanch)
                    {
                        if (AfterWorkFlowGetBanchEvent.Event != null)
                        {
                            //如果在用户事件中筛选过了，就跳出事件循环
                            if (AfterWorkFlowGetBanchEvent.Event(AfterWorkFlowGetBanchEvent.PluginName, ref wsBranch))
                            {
                                break;
                            }
                        }
                    }
                    #endregion

                    //获取流程dbManager
                    DBManager dBManager = curWorkFlow.dBSource.dBManager;

                    //先自动选择人员
                    Group group = wsBranch.NextWorkState.CheckGroup;

                    string caption = string.Empty;

                    //如果流程的文件列表大于零
                    if (((curWorkFlow != null) && (curWorkFlow.DocList != null)) && (curWorkFlow.DocList.Count > 0))
                    {

                        curWorkFlow.Refresh();

                        //判断当前流程已被其他用户操作
                        if (dBManager.IsExistUserData(wsBranch.KeyWord) || (curWorkFlow.UserStatus == enWKUserStatus.View))
                        {
                            //流程分支清除用户编辑状态
                            dBManager.DeleteUserData(wsBranch.KeyWord);
                            bool flag = dBManager.IsExistUserData(wsBranch.KeyWord);
                        }


                        //判断文件列表是否检入状态、最终状态或锁定状态
                        foreach (Doc doc in curWorkFlow.DocList)
                        {
                            if ((((doc != null) && (doc.OperateDocStatus != enDocStatus.IN)) && ((doc.OperateDocStatus != enDocStatus.IN_FINAL) && (doc.OperateDocStatus != enDocStatus.COMING_IN))) && (doc.OperateDocStatus != enDocStatus.GOING_OUT))
                            {
                                caption = caption + doc.ToString + "不是检入状态、最终状态或锁定状态,不能提交流程分支\r\n";
                            }
                        }
                    }
                    else
                    {
                        caption = "";
                    }


                    //定义提示信息
                    if (!string.IsNullOrEmpty(caption))
                    {
                        reJo.msg = caption;
                        return reJo.Value;
                        //return (new JObject(new JProperty("errorMsg", caption)));
                    }
                    else if ((wsBranch.defStateBrach.DefualtAudit.Contains("必须填写意见") && (curWorkFlow.CuWorkState.CuWorkUser != null)) && ((curWorkFlow.CuWorkState.CuWorkUser.WorkAuditList != null) && (curWorkFlow.CuWorkState.CuWorkUser.WorkAuditList.Count<WorkAudit>() == 0)))
                    {

                        caption = "用户没有填写校审意见!";
                        //return (new JObject(new JProperty("errorMsg", caption)));
                        reJo.msg = caption;
                        return reJo.Value;

                    }
                    else
                    {

                        //判断流程当前状态以及流程状态对象是否为空
                        if (((curWorkFlow != null) && (curWorkFlow.CuWorkState != null)) && (curWorkFlow.CuWorkState.DefWorkState != null))
                        {
                            WorkStateBranch branch2 = wsBranch;

                            if ((branch2 == null) || (branch2.NextWorkState == null))
                            {
                                //如果不存在下一状态，显示错误信息
                                //return (new JObject(new JProperty("errorMsg", "跳转失败,不存在下一状态!")));
                                reJo.msg = "跳转失败,不存在下一状态!";
                                return reJo.Value;
                            }

                        }
                    }

                    //如果当前流程状态对象的描述包含"ES"，并且当前用户以前没有通过当前流程状态
                    if (curWorkFlow.CuWorkState.DefWorkState.O_Description.ToUpper().Contains("(ES)") && !IsUserOncePassWorkState(curWorkFlow.CuWorkState, curWorkFlow.dBSource.LoginUser))
                    {
                        //获取当前流程状态的下一流程状态的用户组

                        //先自动选择人员
                        group = wsBranch.NextWorkState.CheckGroup;

                        //选人
                        if (group == null) group = WebSelUserForNextState.SelectUser(curWorkFlow, wsBranch.NextWorkState);
                        if (group != null)
                        {
                            //添加下一流程状态用户组到当前流程状态
                            wsBranch.NextStateAddGroup(group);
                        }
                    }

                    reJo = WebWorkFlowEvent.GotoNextStateAndSelectUser(wsBranch, userlist);


                    //刷新流程
                    //wsBranch.workState.WorkFlow.Refresh();
                    //刷新数据源，让文档的附加属性显示出来，流程才能获取最新的附加属性，然后根据附加属性来判断流程分支
                    DBSourceController.RefreshDBSource(sid);
                    return reJo.Value;
                }
            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(e.Message);
            }
            return reJo.Value;
        }

        /// <summary>
        /// 撤销提交流程
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="Keyword">流程，文档或目录的关键字</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时，data包含一个JObject，里面包含参数"state"；</para>
        /// <para>当 "msg"的值为"selectUser"时，提示需要选择下一状态的校审人员</para>
        /// <para>当 "state"的值为"Passr"时，表示新建流程成功</para>
        /// <para>例子：</para>
        /// </returns>
        public static JObject RebackWorkFlow(string sid, string Keyword)
        {
            ExReJObject reJo = new ExReJObject();
            try
            {
                //登录用户
                User curUser = DBSourceController.GetCurrentUser(sid);
                if (curUser == null)
                {
                    reJo.msg = "登录失败！错误的账号密码";
                    return reJo.Value;
                }


                //获取数据源
                DBSource dbsource = curUser.dBSource;

                //获取流程对象
                object obj = dbsource.GetObjectByKeyWord(Keyword);
                WorkFlow workflow = null;// new WorkFlow();
                if (obj is Doc)
                {
                    Doc ddoc = (Doc)obj;
                    Doc doc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;
                    workflow = doc.WorkFlow;
                }
                else if (obj is Project)
                {
                    Project proj = (Project)obj;
                    workflow = proj.WorkFlow;
                }
                else if (obj is WorkFlow)
                {
                    workflow = (WorkFlow)obj;
                }
                if (workflow == null)
                {
                    reJo.msg = "提交数据错误,流程不存在。"; return reJo.Value;
                }
                if (!WebWorkFlowEvent.WorkFlowReback(workflow))
                {
                    reJo.msg = "撤销提交失败!";
                    return reJo.Value;
                }
                else
                {
                    DBSourceController.RefreshDBSource(sid);
                    reJo.success = true;
                }

            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(e.Message);
            }
            return reJo.Value;
        }

        /// <summary>
        /// 删除流程
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="Keyword">流程，文档或目录的关键字</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时，data包含一个JObject，里面包含参数"state"；</para>
        /// <para>当 "msg"的值为"selectUser"时，提示需要选择下一状态的校审人员</para>
        /// <para>当 "state"的值为"Passr"时，表示新建流程成功</para>
        /// <para>例子：</para>
        /// </returns>
        public static JObject DeleteWorkFlow(string sid, string Keyword)
        {
            ExReJObject reJo = new ExReJObject();
            try
            {
                //登录用户
                User curUser = DBSourceController.GetCurrentUser(sid);
                if (curUser == null)
                {
                    reJo.msg = "登录失败！错误的账号密码";
                    return reJo.Value;
                }

                //获取数据源
                DBSource dbsource = curUser.dBSource;

                //获取流程对象
                object obj = dbsource.GetObjectByKeyWord(Keyword);
                WorkFlow workflow = null;// new WorkFlow();
                if (obj is Doc)
                {
                    Doc ddoc = (Doc)obj;
                    Doc doc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;
                    workflow = doc.WorkFlow;
                }
                else if (obj is Project)
                {
                    Project proj = (Project)obj;
                    workflow = proj.WorkFlow;
                }
                else if (obj is WorkFlow)
                {
                    workflow = (WorkFlow)obj;
                }
                if (workflow == null)
                {
                    reJo.msg = "提交数据错误,流程不存在。"; return reJo.Value;
                }

                //管理员才有权限删除流程
                if (dbsource.LoginUser.O_usertype == enUserType.DefaultAdmin || dbsource.LoginUser.O_usertype == enUserType.WindowsAdmin)
                {

                    workflow.Delete();
                    workflow.Delete();

                    reJo.success = true;
                }
                else
                {
                    reJo.Value = RebackWorkFlow(sid, Keyword);
                }
            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(e.Message);
            }
            return reJo.Value;
        }


        /// <summary>
        /// 流程文档创建人撤回流程
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="Keyword">流程，文档或目录的关键字</param>
        /// <returns></returns>
        public static JObject RevokeWorkFlow(string sid, string Keyword) {
            ExReJObject reJo = new ExReJObject();
            try
            {
                //登录用户
                User curUser = DBSourceController.GetCurrentUser(sid);
                if (curUser == null)
                {
                    reJo.msg = "登录失败！错误的账号密码";
                    return reJo.Value;
                }


                //获取数据源
                DBSource dbsource = curUser.dBSource;

                //获取流程对象
                object obj = dbsource.GetObjectByKeyWord(Keyword);
                WorkFlow workflow = null;// new WorkFlow();
                if (obj is Doc)
                {
                    Doc ddoc = (Doc)obj;
                    Doc doc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;
                    workflow = doc.WorkFlow;
                }
                else if (obj is Project)
                {
                    Project proj = (Project)obj;
                    workflow = proj.WorkFlow;
                }
                else if (obj is WorkFlow)
                {
                    workflow = (WorkFlow)obj;
                }
                if (workflow == null)
                {
                    reJo.msg = "提交数据错误,流程不存在。"; return reJo.Value;
                }

                //小钱 2018-7-15 增加流程文档的创建者可以撤回流程
                //if (!(workflow.O_WorkFlowStatus != enWorkFlowStatus.Finish &&
                //    (workflow.UserStatus == enWKUserStatus.Modify || workflow.UserStatus == enWKUserStatus.Check) &&
                //    workflow.doc != null && workflow.doc.Creater == workflow.dBSource.LoginUser))
                if (!(workflow.O_WorkFlowStatus != enWorkFlowStatus.Finish &&
                    workflow.doc != null && workflow.doc.Creater == workflow.dBSource.LoginUser))
                {
                    reJo.msg = "提交数据错误,流程撤回失败。";
                    return reJo.Value;
                }


                #region 撤回流程触发的用户事件
                foreach (WebWorkFlowEvent.Before_Revoke_WorkFlow_Event_Class BeforeRevokeWorkFlowEvent in WebWorkFlowEvent.ListBeforeRevokeWorkFlow)
                {
                    if (BeforeRevokeWorkFlowEvent.Event != null)
                    {
                        //如果在用户事件中筛选过了，就跳出事件循环
                       // if (BeforeRevokeWorkFlowEvent.Event(BeforeRevokeWorkFlowEvent.PluginName, workflow))
                        ExReJObject BeforeRevokeWorkFlowReJo = BeforeRevokeWorkFlowEvent.Event(BeforeRevokeWorkFlowEvent.PluginName, workflow);
                        if (BeforeRevokeWorkFlowReJo.success == false)
                        {
                            BeforeRevokeWorkFlowReJo.success = true;
                            return BeforeRevokeWorkFlowReJo.Value;
                            //break;
                        }
                    }
                }
               
                #endregion

                reJo.success = true;

            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(e.Message);
            }
            return reJo.Value;
        }

        /// <summary>
        /// 添加流程状态人员
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="ObjectKeyword">流程，文档或目录的关键字</param>
        /// <param name="WorkStateKeyword">流程状态的关键字</param>
        /// <param name="UserList">用户列表,用","号分割</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时，data包含一个JObject，里面包含参数"state"；</para>
        /// <para>当 "msg"的值为"selectUser"时，提示需要选择下一状态的校审人员</para>
        /// <para>当 "state"的值为"Passr"时，表示新建流程成功</para>
        /// <para>例子：</para>
        /// </returns>
        public static JObject AddWorkUser(string sid, string ObjectKeyword, string WorkStateKeyword,string UserList)
        {
            ExReJObject reJo = new ExReJObject();
            try
            {
                //登录用户
                User curUser = DBSourceController.GetCurrentUser(sid);
                if (curUser == null)
                {
                    reJo.msg = "登录失败！错误的账号密码";
                    return reJo.Value;
                }

                //获取数据源
                DBSource dbsource = curUser.dBSource;

                //获取流程对象
                object obj = dbsource.GetObjectByKeyWord(ObjectKeyword);
                WorkFlow workflow = null;// new WorkFlow();
                if (obj is Doc)
                {
                    Doc ddoc = (Doc)obj;
                    Doc doc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;
                    workflow = doc.WorkFlow;
                }
                else if (obj is Project)
                {
                    Project proj = (Project)obj;
                    workflow = proj.WorkFlow;
                }
                else if (obj is WorkFlow)
                {
                    workflow = (WorkFlow)obj;
                }
                if (workflow == null)
                {
                    reJo.msg = "提交数据错误,流程不存在。";
                    return reJo.Value;
                }

                //获取流程状态
                WorkState ws = null;
                foreach (WorkState wsItem in workflow.WorkStateList)
                {
                    if (wsItem.KeyWord == WorkStateKeyword)
                    {
                        ws = wsItem;
                    }
                }

                if (ws == null)
                {
                    reJo.msg = "提交数据错误,流程状态不存在。";
                    return reJo.Value;
                }

                if (!(curUser.IsAdmin || workflow.UserStatus == enWKUserStatus.Check))
                {
                    reJo.msg = "当前用户没有权限添加人员";
                    return reJo.Value;
                }

                if (string.IsNullOrEmpty(UserList))
                {
                    //提示需要选择添加的校审人员
                    reJo.data = new JArray(new JObject(new JProperty("state", "selectUser"), new JProperty("wfKeyword", workflow.KeyWord)));
                    reJo.msg = "selectUser";
                    return reJo.Value;
                }

                JArray JaDate = new JArray();

                string[] strArray = (string.IsNullOrEmpty(UserList) ? "" : UserList).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string struser in strArray)
                {

                    //fmSelUserOrGroup SelectUsers = new fmSelUserOrGroup(this.workFlow.dBSource, enCallerName.User, true, false, null);
                    //if (SelectUsers.ShowDialog() == DialogResult.OK)
                    //{
                    //    foreach (object obj in SelectUsers.ListObject)
                    //    {
                    //        User user = (User)obj;
                    //获取要添加的用户对象
                    object userobj = dbsource.GetObjectByKeyWord(struser);
                    User user= null;// new WorkFlow();
                    if (userobj is User)
                    {
                        user = (User)userobj;
                    }

                    if (user == null)
                    {
                        continue;
                    }

                            //不存在该用户才允许添加
                            bool IsExist = false;
                            foreach (WorkUser temp in ws.WorkUserList)
                            {
                                if (temp.User == user)
                                {
                            IsExist = true;
                            break;
                        }
                    }
                    if (!IsExist)
                    {
                        ws.AfterAddUser(user);
                        JaDate.Add(new JObject(new JProperty("WorkStateKeyword", WorkStateKeyword),
                            new JProperty("UserName", user.ToString),
                            new JProperty("UserKeyword", user.KeyWord),
                            new JProperty("PageId", "2")));

                    }
                            else
                            {
                        JaDate.Add(new JObject(new JProperty("WorkStateKeyword", ""), new JProperty("UserName",user.ToString), 
                            new JProperty("UserKeyword",user.KeyWord),
                            new JProperty("PageId", "2"),
                            new JProperty("errmsg", "已存在" + user.ToString + ",添加人员失败")));
                    }

                }
                reJo.data = JaDate;
                reJo.success = true;

            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(e.Message);
            }
            return reJo.Value;
        }

        //DeleteWorkUser
        /// <summary>
        /// 删除流程状态的用户
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="ObjectKeyword">流程，文档或目录的关键字</param>
        /// <param name="WorkStateKeyword">流程状态的关键字</param>
        /// <param name="UserKeyword">用户关键字</param>
        /// <returns></returns>
        public static JObject DeleteWorkUser(string sid, string ObjectKeyword, string WorkStateKeyword, string UserKeyword)
        {
            ExReJObject reJo = new ExReJObject();
            try
            {
                //登录用户
                User curUser = DBSourceController.GetCurrentUser(sid);
                if (curUser == null)
                {
                    reJo.msg = "登录失败！错误的账号密码";
                    return reJo.Value;
                }

                //获取数据源
                DBSource dbsource = curUser.dBSource;

                //获取流程对象
                object obj = dbsource.GetObjectByKeyWord(ObjectKeyword);
                WorkFlow workflow = null;// new WorkFlow();
                if (obj is Doc)
                {
                    Doc ddoc = (Doc)obj;
                    Doc doc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;
                    workflow = doc.WorkFlow;
                }
                else if (obj is Project)
                {
                    Project proj = (Project)obj;
                    workflow = proj.WorkFlow;
                }
                else if (obj is WorkFlow)
                {
                    workflow = (WorkFlow)obj;
                }
                if (workflow == null)
                {
                    reJo.msg = "提交数据错误,流程不存在。";
                    return reJo.Value;
                }

                //获取流程状态
                WorkState ws = null;
                foreach (WorkState wsItem in workflow.WorkStateList)
                {
                    if (wsItem.KeyWord == WorkStateKeyword)
                    {
                        ws = wsItem;
                    }
                }


                if (ws == null)
                {
                    reJo.msg = "提交数据错误,流程状态不存在。";
                    return reJo.Value;
                }

                if (!(curUser.IsAdmin || workflow.UserStatus == enWKUserStatus.Check))
                {
                    reJo.msg = "当前用户没有权限添加人员";
                    return reJo.Value;
                }

                WorkUser wu = null;
                foreach (WorkUser wuItem in ws.WorkUserList) {
                    if (wuItem.User.KeyWord == UserKeyword) {
                        wu = wuItem;
                    }
                }
                if (wu != null)
                {
                    //如果流程状态只有一个工作用户，就不允许删除
                    if (ws.WorkUserList.Count <= 1) {
                        reJo.msg = "不能删除流程状态里唯一的人员";
                        return reJo.Value;
                    }

                    wu.workState.DeleteUser(wu.User);

                    reJo.data = new JArray(new JObject(new JProperty("WorkStateKeyword", WorkStateKeyword),
                    new JProperty("UserKeyword", UserKeyword),
                    new JProperty("PageId", "2")));
                    reJo.success = true;
                }


            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(e.Message);
            }
            return reJo.Value;
        }

        /// <summary>
        /// 获取WorkflowPlace流程类别对象下子WorkflowPlace树节点列表
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="node">Project节点关键字</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时，data包含多个JObject，每个JObject里面包含参数"id"：节点关键字，"text"：节点文本，"parentId"：父节点关键字
        /// "leaf":是否有子节点("true","false"),"iconCls":设置图标</para>
        /// <para>例子：</para>
        /// </returns>

        public static JObject GetWorkFlowPlaceTree(string sid, string node)
        {
            ExReJObject reJo = new ExReJObject();
            try
            {
                string path = node ?? "/";

                string keyword = path;

                if (sid == null) return CommonController.SidError();

                string strMsg = string.Empty;

                JArray jaGetList = new JArray();



                //登录用户
                User curUser = DBSourceController.GetCurrentUser(sid);
                if (curUser == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

                //登录用户
                DBSource dbsource = curUser.dBSource;
                if (dbsource == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

                //如果是访问根流程树，就刷新一下数据源（按下F5自动刷新数据源）
                DBSourceController.refreshDBSource(sid, "1");

                string strProcessWorkFlow = curUser.ProcessWorkFlowList.Count.ToString();//待处理流程
                string strPlanWorkFlow = curUser.PlanWorkFlowList.Count.ToString();//参与流程
                string strFinishWorkFlow = curUser.FinishWorkFlowList.Count.ToString();//完成流程
                string strErrorWorkFlow = curUser.ErrorWorkFlowList.Count.ToString();//异常工作流

                JArray jaResult = new JArray();


                jaResult.Add(new JObject{                        
                            new JProperty("id",keyword+"_5"),//节点关键字
                            new JProperty("text","待处理流程"+"("+strProcessWorkFlow+")"),//节点文本
                            new JProperty("parentId",keyword),//父节点关键字
                            new JProperty("leaf",true),//没有子节点
                            new JProperty("iconCls","myfolder")//设置图标
                        });

                jaResult.Add(new JObject{                        
                            new JProperty("id",keyword+"_6"),//节点关键字
                            new JProperty("text","参与流程"+"("+strPlanWorkFlow+")"),//节点文本
                            new JProperty("parentId",keyword),//父节点关键字
                            new JProperty("leaf",true),//没有子节点
                            new JProperty("iconCls","myfolder")//设置图标
                        });
                jaResult.Add(new JObject{                        
                            new JProperty("id",keyword+"_7"),//节点关键字
                            new JProperty("text","完成流程"+"("+strFinishWorkFlow+")"),//节点文本
                            new JProperty("parentId",keyword),//父节点关键字
                            new JProperty("leaf",true),//没有子节点
                            new JProperty("iconCls","myfolder")//设置图标
                        });
                jaResult.Add(new JObject{                        
                            new JProperty("id",keyword+"_8"),//节点关键字
                            new JProperty("text","异常工作流"+"("+strErrorWorkFlow+")"),//节点文本
                            new JProperty("parentId",keyword),//父节点关键字
                            new JProperty("leaf",true),//没有子节点
                            new JProperty("iconCls","myfolder")//设置图标
                        });

                reJo.data = jaResult;
                reJo.total = jaResult.Count;
                reJo.success = true;
            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(e.Message);
            }
            return reJo.Value;
        }

        /// <summary>
        /// 获取用户的流程工作台列表
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="WorkflowType">流程的类型，值可以是:"正在处理流程_5","参与工作流_6","完成工作流_7","异常工作流_8"</param>
        /// <param name="page">页数，默认值1</param>
        /// <param name="limit">每页数量，默认值50</param>
        /// <returns></returns>
        public static JObject GetWorkFlowPlaceList(string sid, string WorkflowType, string page, string limit)
        {
           // return MessageController.GetMessageList(sid, WorkflowType, page, limit);


            ExReJObject reJo = new ExReJObject();
            JArray jaGetList = new JArray();

            try
            {
                WorkflowType = WorkflowType ?? "";
                page = string.IsNullOrEmpty(page) ? "1" : page;
                limit = string.IsNullOrEmpty(limit) ? "50" : limit;

                if (string.IsNullOrEmpty(WorkflowType))
                {
                    reJo.msg = "错误的提交数据。";
                    return reJo.Value;
                }
                else
                {
                    string strType = WorkflowType.Split('_')[1];

                    string strMsg = string.Empty;
                    page = (Convert.ToInt32(page) - 1).ToString();

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

                    JArray jaMsgList = new JArray();
                    JArray jaResult = new JArray();

                    //需要处理的工作流
                    List<WorkFlow> wfList = null;

                    enWorkFlowPlaceType wfpType = (enWorkFlowPlaceType)(Convert.ToInt32(strType));
                    if (wfpType == enWorkFlowPlaceType.ProcessWorkFlow)//待处理流程
                    {
                        wfList = curUser.ProcessWorkFlowList;
                    }
                    else if (wfpType == enWorkFlowPlaceType.PlanWorkFlow)//参与流程
                    {
                        wfList = curUser.PlanWorkFlowList;
                    }
                    else if (wfpType == enWorkFlowPlaceType.FinishWorkFlow)//完成流程
                    {
                        wfList = curUser.FinishWorkFlowList;
                    }
                    else if (wfpType == enWorkFlowPlaceType.ErrorWorkFlow)//异常工作流
                    {
                        wfList = curUser.ErrorWorkFlowList;
                    }


                    //处理流程
                    if (wfList != null && wfList.Count > 0)
                    {
                        int index = 0;

                        int iCount = wfList.Count;
                        
                        dbsource.Refresh();

                        for (int j = iCount - 1; j >= 0; j--)
                        {
                            index = index + 1;
                            int pageInt = Convert.ToInt32(page);
                            int limitInt = Convert.ToInt32(limit);

                            if (!(index > pageInt * limitInt && index <= (pageInt + 1) * limitInt))
                            {
                                continue;
                            }

                            WorkFlow mW = wfList[j];

                            if (mW == null)
                                continue;

                            //mW.Refresh();
                            if (mW.O_WorkFlowStatus == enWorkFlowStatus.Delete)
                                continue;

                            //文件名称
                            //修改查询文件名称的方法，提高了查询速度
                            string file = "";
                            string sql = "select o_itemname,o_itemdesc from cdms_doc where o_dmsstatus!=10 and o_WorkFlowno=" + mW.ID.ToString();
                            string[] docs = dbsource.DBExecuteSQL(sql);
                            if (docs.Length > 1)
                                file = string.IsNullOrEmpty(docs[1]) ? docs[0] : docs[0] + "__" + docs[1];
                            else if (mW.Project != null)
                                file = mW.Project.ToString;
                            //if (mW.doc != null)
                            //    file = mW.doc.ToString;
                            //else if (mW.Project != null)
                            //    file = mW.Project.ToString;


                            //流程名称
                            string DefWorkFlowName = "";
                            if (mW.DefWorkFlow != null)
                                DefWorkFlowName = mW.DefWorkFlow.ToString;


                            ////流程发起人
                            string Creater = "";
                            if (mW.WorkStateList != null && mW.WorkStateList.Count() > 0)
                                Creater = mW.WorkStateList[0].User.ToString;


                            ////发起时间
                            string CreateDate = "";
                            if (mW.O_CreateDate != null)
                                CreateDate = mW.O_CreateDate.ToString();

                            DateTime dt = Convert.ToDateTime(mW.O_CreateDate.ToString());
                            long timeStamp = (long)((DateTime.Now - dt).TotalMilliseconds); // 相差毫秒数

                            jaResult.Add(new JObject {
                                new JProperty("Keyword",mW.KeyWord),
                                new JProperty("File",file),
                                new JProperty("DefWorkFlow",DefWorkFlowName),
                                new JProperty("Creater",Creater),//时间
                                new JProperty("CreateDate",CreateDate),
                                new JProperty("post_time",timeStamp.ToString())// "1305823292"),
                            });
                        }
                    }


                    reJo.total = wfList.Count;

                    reJo.data = jaResult;
                    reJo.success = true;
                }
            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(e.Message);
            }
            return reJo.Value;
        }
 

        /// <summary>
        /// 判断用户是否通过了流程状态
        /// </summary>
        /// <param name="workState">状态</param>
        /// <param name="user">用户</param>
        /// <returns>是否通过流程</returns>
        private static bool IsUserOncePassWorkState(WorkState workState, User user)
        {
            try
            {
                if (((workState == null) || (workState.HistoryList == null)) || ((workState.HistoryList.Count <= 0) || (user == null)))
                {
                    return false;
                }
                foreach (WorkStateHistory history in workState.HistoryList)
                {
                    if (history.CheckUser == user)
                    {
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
            }
            return false;
        }

        /// <summary>
        /// 获取流程Json
        /// </summary>
        /// <param name="workFlow">WorkFlow对象</param>
        /// <returns></returns>
        private static JObject GetWorkFlowJson(WorkFlow workFlow)
        {
            try
            {
                if (workFlow == null)
                    return new JObject();//JArray();

                #region 设置标题
                string sTitle = "";
                if (workFlow != null && workFlow.CuWorkState != null)
                {
                    String workStateStr = workFlow.CuWorkState.ToString;

                    if (workFlow.CuWorkState.DefWorkState != null && (workFlow.CuWorkState.DefWorkState.O_Code.ToUpper() == "END" || workFlow.O_WorkFlowStatus == enWorkFlowStatus.Finish))
                        workStateStr = "流程结束";

                    switch (workFlow.UserStatus)
                    {
                        case enWKUserStatus.Check:
                            sTitle = "(当前状态 : " + workStateStr + " 当前用户状态: 填写校审意见)";
                            break;
                        case enWKUserStatus.Modify:
                            sTitle = "(当前流程状态 : " + workStateStr + " 当前用户状态: 设计修改)";
                            break;
                        case enWKUserStatus.View:
                            sTitle = "(当前流程状态 : " + workStateStr + " 当前用户状态: 浏览)";
                            break;
                        default:
                            break;
                    }
                }
                #endregion

                #region 下面那串
                string sUserString = GetWorkFlowUserString(workFlow);
                #endregion

                #region UserStatus
                String UserStatus = workFlow.UserStatus.ToString();
                #endregion

                #region 默认校审意见
                String DefaultAuditList = CurrentWSDefaultAuditList(workFlow);
                #endregion

                #region 分支按钮
                //K是wsb的Keyword，D是wsb的描述
                JArray jaWSB = new JArray();
                string enabled = (((workFlow.UserStatus != enWKUserStatus.Check) && (workFlow.UserStatus != enWKUserStatus.Modify)) ? "F" : "T");


                foreach (WorkStateBranch branch in workFlow.CuWorkState.workStateBranchList)
                {

                    JObject joWSB = new JObject(new JProperty("K", branch.KeyWord), new JProperty("D", branch.defStateBrach.O_Description), new JProperty("E", enabled));
                    jaWSB.Add(joWSB);
                }
                #endregion

                #region 所有校审意见
                //W是这个wa所属的ws的描述，U是用户，P是校审意见，D是修改意见，C是校审时间
                //String WA = "";



              
                List<WorkState> isAddWaWs = new List<WorkState>();
                JArray jaWA = new JArray();

                #region 添加发起流程的状态
                User createUser = workFlow.WorkStateList[0].WorkUserList[0].User;
                JObject cu_joWA = new JObject(new JProperty("W", "发起流程"), 
                    new JProperty("U", createUser.ToString),
                    new JProperty("UK", ""), new JProperty("P", ""),
                    new JProperty("D", ""), new JProperty("C", ""),
                    new JProperty("K", ""), new JProperty("R", ""),
                    new JProperty("FD", workFlow.O_CreateDate.ToString()),
                    new JProperty("FU", createUser.ToString),
                    new JProperty("FB", ""));
                jaWA.Add(cu_joWA); 
                #endregion

                int WAIndex = 0;
                foreach (WorkAudit wa in workFlow.AllWorkAuditList)
                {
                    String sWSDesc = "";
                    String sWSKeyword = "";
                    String sWSFinishDate = "";
                    String sWSFinishUser = "";
                    String sWSFinishStateBrach = "";

                    #region 查找没有填写意见，而且意见通过的流程状态
                    //查找没有填写意见，而且意见通过的流程状态

  
                    int wsIndex = (int)wa.workUser.workState.O_Index;
                    workFlow.WorkStateList.ForEach(ws =>
                    {
                        if (!(isAddWaWs.Contains(ws)))
                        {
                            if ((int)ws.O_Index <= wsIndex)
                            {
                                if ((workFlow.CuWorkState == ws) && !ws.ToString.EndsWith("(流程结束)")
                                    && workFlow.O_WorkFlowStatus != AVEVA.CDMS.Server.enWorkFlowStatus.Finish)
                                {
                                    sWSFinishStateBrach = " [←当前状态]";
               
                                    sWSFinishDate = "";
                                    foreach (WorkUser wu in ws.WorkUserList)
                                    {
                                        sWSFinishUser = sWSFinishUser + wu.User.ToString + ";";
                                    }
                                    if (sWSFinishUser.EndsWith(";"))
                                    {
                                        sWSFinishUser = sWSFinishUser.Substring(0, sWSFinishUser.Length - 1);
                                    }
                                    JObject p_joWA = new JObject(new JProperty("W", ws.Description), new JProperty("U", sWSFinishUser),
                                        new JProperty("UK", ""), new JProperty("P", ""),
                                        new JProperty("D", ""), new JProperty("C", ""),
                                        new JProperty("K", ""), new JProperty("R", ""),
                                        new JProperty("FD", sWSFinishDate), new JProperty("FU", sWSFinishUser),
                                        new JProperty("FB", sWSFinishStateBrach));//,new JProperty("K", WAIndex.ToString()));
                                    jaWA.Add(p_joWA);
                                    isAddWaWs.Add(ws);
                                }
                                else if (ws.O_FinishDate != null)
                                {
                                    String p_sWSFinishDate = "";
                                    String p_sWSFinishUser = "";
                                    String p_sWSFinishStateBrach = "";

                                    p_sWSFinishDate = ws.O_FinishDate.ToString();
                                    //获取通过的人员
                                    WorkUser fwu = ws.WorkUserList.Find(wu => ((wu.O_pass.HasValue && wu.O_pass.Value)));
                                    if (fwu == null && ws.WorkUserList.Count == 1)
                                    {
                                        fwu = ws.WorkUserList[0];
                                    }
                                    if (fwu != null && (!isAddWaWs.Contains(ws)))
                                    {
                                        p_sWSFinishUser = fwu.User.ToString;
                                        p_sWSFinishStateBrach = fwu.workStateBranch.defStateBrach.O_Description;
                                        isAddWaWs.Add(ws);
                                    }

                                    JObject p_joWA = new JObject(new JProperty("W", ws.Description), new JProperty("U", p_sWSFinishUser),
                                    new JProperty("UK", ""), new JProperty("P", ""),
                                    new JProperty("D", ""), new JProperty("C", ""),
                                    new JProperty("K", ""), new JProperty("R", ""),
                                    new JProperty("FD", p_sWSFinishDate), new JProperty("FU", p_sWSFinishUser),
                                    new JProperty("FB", p_sWSFinishStateBrach));//,new JProperty("K", WAIndex.ToString()));
                                    jaWA.Add(p_joWA);
                                    isAddWaWs.Add(ws);
                                }
                            }
                        }
                    }); 
                    #endregion


                    workFlow.WorkStateList.ForEach(ws =>
                    {
                        if (ws.ID == wa.O_WorkStateNo)
                        {
                            sWSDesc = ws.Description; sWSKeyword = ws.KeyWord;

                            #region 获取通过的人员
                            if (!isAddWaWs.Contains(ws))
                            {
                                if (ws.O_FinishDate != null)
                                {
                                    //获取通过的人员
                                    WorkUser fwu = ws.WorkUserList.Find(wu => ((wu.O_pass.HasValue && wu.O_pass.Value)));
                                    if (fwu == null && ws.WorkUserList.Count == 1)
                                    {
                                        fwu = ws.WorkUserList[0];
                                    }
                                    if (fwu != null && (!isAddWaWs.Contains(ws)))
                                    {
                                        sWSFinishUser = fwu.User.ToString;
                                        if ((workFlow.CuWorkState == ws) && !ws.ToString.EndsWith("(流程结束)")
                                            && workFlow.O_WorkFlowStatus != AVEVA.CDMS.Server.enWorkFlowStatus.Finish)
                                        {
                                                                    sWSFinishStateBrach = " [←当前状态]";
                                      
                                            sWSFinishDate = "";
                                        }
                                        else
                                        {
                                            sWSFinishStateBrach = fwu.workStateBranch.defStateBrach.O_Description;
                                            sWSFinishDate = ws.O_FinishDate.ToString();
                                        }
                                        isAddWaWs.Add(ws);
                                    }
                                }
                                
                            }
                            #endregion
                        }
                    });

                    string waRight = "";
                    //是否有校审人填写校审意见的权限
                    if (workFlow.UserStatus != enWKUserStatus.View && workFlow.CuWorkState.ID == wa.O_WorkStateNo)
                    {
                        waRight = "1";// "ProcAudit";
                    }
                    //是否有设计人填写修改意见
                    else if (workFlow.UserStatus == enWKUserStatus.Modify && workFlow.CuWorkState.PreWorkState.ID == wa.O_WorkStateNo)
                    {
                        waRight = "2";// "DeProcAudit";
                    }

                    JObject joWA = new JObject(new JProperty("W", sWSDesc), new JProperty("U", wa.workUser.User.ToString),
                    new JProperty("UK", wa.workUser.User.KeyWord), new JProperty("P", wa.O_Problom),
                    new JProperty("D", wa.O_DesignerCom), new JProperty("C", wa.O_CheckDate.ToString()),
                    new JProperty("K", sWSKeyword), new JProperty("R", waRight),
                    new JProperty("FD", ""), new JProperty("FU", ""),
                    new JProperty("FB", ""));//,new JProperty("K", WAIndex.ToString()));
                    jaWA.Add(joWA);
                    WAIndex++;
                }

                #region 再次查找没有填写意见，而且意见通过的流程状态
                //再次查找没有填写意见，而且意见通过的流程状态

                workFlow.WorkStateList.ForEach(ws =>
                {
                    if (!(isAddWaWs.Contains(ws)))
                    {
                        String p_sWSFinishDate = "";
                        String p_sWSFinishUser = "";
                        String p_sWSFinishStateBrach = "";

                        if (ws.O_FinishDate != null)
                        {


                            p_sWSFinishDate = ws.O_FinishDate.ToString();
                            //获取通过的人员
                            WorkUser fwu = ws.WorkUserList.Find(wu => ((wu.O_pass.HasValue && wu.O_pass.Value)));
                            if (fwu == null && ws.WorkUserList.Count == 1)
                            {
                                fwu = ws.WorkUserList[0];
                            }
                            if (fwu != null && (!isAddWaWs.Contains(ws)))
                            {
                                p_sWSFinishUser = fwu.User.ToString;
                                p_sWSFinishStateBrach = fwu.workStateBranch.defStateBrach.O_Description;
                                isAddWaWs.Add(ws);
                            }

                            JObject p_joWA = new JObject(new JProperty("W", ws.Description), new JProperty("U", p_sWSFinishUser),
                            new JProperty("UK", ""), new JProperty("P", ""),
                            new JProperty("D", ""), new JProperty("C", ""),
                            new JProperty("K", ""), new JProperty("R", ""),
                            new JProperty("FD", p_sWSFinishDate), new JProperty("FU", p_sWSFinishUser),
                            new JProperty("FB", p_sWSFinishStateBrach));//,new JProperty("K", WAIndex.ToString()));
                            jaWA.Add(p_joWA);
                            isAddWaWs.Add(ws);
                        }
                        else if ((workFlow.CuWorkState == ws) && !ws.ToString.EndsWith("(流程结束)")
                            && workFlow.O_WorkFlowStatus != AVEVA.CDMS.Server.enWorkFlowStatus.Finish)
                        {

                           p_sWSFinishStateBrach = " [←当前状态]";
           
                            p_sWSFinishDate = "";
                            foreach (WorkUser wu in ws.WorkUserList) {
                                p_sWSFinishUser = p_sWSFinishUser + wu.User.ToString + ";";
                            }
                            if (p_sWSFinishUser.EndsWith(";")) {
                                p_sWSFinishUser = p_sWSFinishUser.Substring(0, p_sWSFinishUser.Length - 1);
                            }
                            JObject p_joWA = new JObject(new JProperty("W", ws.Description), new JProperty("U", p_sWSFinishUser),
                                new JProperty("UK", ""), new JProperty("P", ""),
                                new JProperty("D", ""), new JProperty("C", ""),
                                new JProperty("K", ""), new JProperty("R", ""),
                                new JProperty("FD", p_sWSFinishDate), new JProperty("FU", p_sWSFinishUser),
                                new JProperty("FB", p_sWSFinishStateBrach));//,new JProperty("K", WAIndex.ToString()));
                            jaWA.Add(p_joWA);
                            isAddWaWs.Add(ws);

                        }

                    }
                });
                #endregion
                #endregion

                #region 删除撤销按钮部分
                bool bVisible = false;
                bool bEnabled = false;

                bool bRetractVisible = false;
                bool bRetractEnabled = false;
                String sRebackText = "";
                String sRetractText = "";
                String sRebackKeyword = "";
                String sRetractKeyword = "";

                if (CanWorkFlowWithDraw(workFlow))
                    bVisible = bEnabled = true;

                //小黎 2013-2-1 增加判断第一个状态时显示删除流程
                //小黎 2011-12-21 设计修改状态的时候要做的不是撤销提交，而是删除流程
                //老陈 2013-11-8 由于去掉起始状态，只有两个状态的流程，两个状态的上一状态都为start，由此增加对DefWorkState流水号的判断
                if (workFlow.UserStatus == enWKUserStatus.Modify || (workFlow.CuWorkState != null && workFlow.CuWorkState.PreWorkState != null
                    && workFlow.CuWorkState.PreWorkState.Code.ToLower() == "start" && workFlow.CuWorkState.O_Index == 1))
                {
                    sRebackText = "删除流程";
                    sRebackKeyword = "MS_DeleteWf";
                    //根据数据字典做成可配置
                    string sDontShowButton = workFlow.dBSource.GetCDMSUserConfig("DONOTShowDeleteWF");
                    if (sDontShowButton == "1")
                        bEnabled = false;
                }
                else
                {
                    sRebackText = "撤销提交";
                    sRebackKeyword = "MS_RebackWf";
                    //根据数据字典做成可配置
                    string sDontShowButton = workFlow.dBSource.GetCDMSUserConfig("DONOTShowRebackWF");
                    if (sDontShowButton == "1")
                        bEnabled = false;
                }

                //小黎 2012-1-29 管理员登录的时候都是删除流程
                if (workFlow.dBSource.LoginUser.IsAdmin)
                {
                    bool bOK = false;
                    string sAdminShowReback = workFlow.dBSource.GetCDMSUserConfig("AdminShowReback");
                    //LPEC特别配置的话，就必须要求当前人是admin才显示删除流程
                    //没配置的话，就按原来逻辑
                    if (sAdminShowReback == "1")
                        bOK = (workFlow.dBSource.LoginUser.Code == "admin");
                    else
                        bOK = true;

                    if (bOK)
                    {
                        bEnabled = true;
                        bVisible = true;
                        sRebackText = "删除流程";
                        sRebackKeyword = "MS_DeleteWf";
                    }
                }

                //小钱 2018-7-15 增加流程文档的创建者可以撤回流程
                //if (workFlow.O_WorkFlowStatus != enWorkFlowStatus.Finish &&
                //    (workFlow.UserStatus == enWKUserStatus.Modify || workFlow.UserStatus == enWKUserStatus.Check) &&
                //    workFlow.doc != null && workFlow.doc.Creater == workFlow.dBSource.LoginUser)
                if (workFlow.O_WorkFlowStatus != enWorkFlowStatus.Finish &&
                   workFlow.doc != null && workFlow.doc.Creater == workFlow.dBSource.LoginUser)
                {
                    sRetractText = "撤回流程";
                    sRetractKeyword = "MS_RetractWf";
                    //根据数据字典做成可配置
                    string sDontShowButton = workFlow.dBSource.GetCDMSUserConfig("DONOTShowRebackWF");
                    if (sDontShowButton == "1")
                    {
                        bRetractVisible = false;
                        bRetractEnabled = false;
                    }
                    else
                    {
                        bRetractVisible = true;
                        bRetractEnabled = true;
                    }
                }

                #endregion


                JObject jo = new JObject(
                       new JProperty("KeyWord", workFlow.KeyWord),
                       new JProperty("Enabled", ((workFlow.UserStatus != enWKUserStatus.Check) && (workFlow.UserStatus != enWKUserStatus.Modify)) ? "F" : "T"),
                       new JProperty("Title", sTitle), new JProperty("UserString", sUserString),
                       new JProperty("DefaultAuditList", DefaultAuditList),
                       new JProperty("UserStatus", UserStatus),
                       new JProperty("RebackVisible", bVisible.ToString().ToLower()),
                       new JProperty("RebackEnabled", bEnabled.ToString().ToLower()),
                       new JProperty("RebackText", sRebackText), new JProperty("RebackKeyword", sRebackKeyword),
                       new JProperty("RetractVisible", bRetractVisible.ToString().ToLower()),
                       new JProperty("RetractEnabled", bRetractEnabled.ToString().ToLower()),
                       new JProperty("RetractText", sRetractText), new JProperty("RetractKeyword", sRetractKeyword),
                       new JProperty("HasOtherWorkflow", HasOtherWorkflows(workFlow) ? "true" : "false")

                       );
                jo.Add(new JProperty("WSB", jaWSB));
                jo.Add(new JProperty("WA", jaWA));

                return jo;
            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
                return new JObject();
            }
        }


        /// <summary>
        /// 获取流程用户信息
        /// </summary>
        /// <param name="wf">WorkFlow对象</param>
        /// <returns></returns>
        private static String GetWorkFlowUserString(WorkFlow wf)
        {
            try
            {
                if (wf == null)
                    return "";
                if (wf.WorkStateList == null && wf.WorkStateList.Count <= 0)
                    return "异常...";

                String value = "";
                int iSeq = 0;
                foreach (WorkState ws in wf.WorkStateList)
                {
                    if (ws.WorkUserList == null || ws.WorkUserList.Count <= 0)
                        continue;

                    iSeq++;

                    String strWS = iSeq.ToString() + " " + ws.Description;

                    if (ws.WorkUserList != null && ws.WorkUserList.Count > 0)
                    {
                        String sU = "";
                        foreach (WorkUser wU in ws.WorkUserList)
                        {
                            if (String.IsNullOrEmpty(sU))
                                sU = (wU.User != null ? wU.User.ToString : "");
                            else
                                sU = sU + "," + (wU.User != null ? wU.User.ToString : "");
                        }

                        if (!String.IsNullOrEmpty(sU))
                            strWS += ":" + sU;
                    }

                    if (String.IsNullOrEmpty(value))
                        value = strWS;
                    else
                        value = value + " " + strWS;
                }
                return value;
            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
                return "异常...";
            }
        }

        /// <summary>
        /// 获取缺省意见表
        /// </summary>
        /// <param name="wf"></param>
        /// <returns></returns>
        private static String CurrentWSDefaultAuditList(WorkFlow wf)
        {

            if (wf != null && wf.CuWorkState != null && wf.CuWorkState.DefWorkState != null && !String.IsNullOrEmpty(wf.CuWorkState.DefWorkState.DefualtAuditList) && wf.CuWorkState.DefWorkState.DefualtAuditList != null)
                return wf.CuWorkState.DefWorkState.DefualtAuditList;
            else
                return "";
        }

        /// <summary>
        /// 判断当前登录人员是否允许撤销流程,退回前一状态
        /// </summary>
        /// <param name="workFlow"></param>
        /// <returns></returns>
        private static bool CanWorkFlowWithDraw(WorkFlow workFlow)
        {
            //TIM 2009-11-19 添加一个撤销按钮,实现上一个提交人员可以从当前状态退回前一个状态
            try
            {
                if (workFlow == null)
                    return false;

                if (workFlow.O_WorkFlowStatus == enWorkFlowStatus.Finish)
                {
                    // 当前状态为 End 状态的前一状态 , 而且撤销,也仅仅是撤回当前状态,只需设定当前用户不通过,并清除消息等即可
                    if (workFlow.CuWorkState != null && workFlow.CuWorkState.WorkUserList != null && workFlow.CuWorkState.WorkUserList.Count > 0)
                    {
                        foreach (WorkUser tempWu in workFlow.CuWorkState.WorkUserList)
                        {
                            if (tempWu.User != null && tempWu.User == workFlow.dBSource.LoginUser)
                            {
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    if (workFlow.CuWorkState != null && workFlow.CuWorkState.PreWorkState != null)
                    {
                        //当前状态，只要有人点了通过，就不允许撤销
                        foreach (WorkUser wU in workFlow.CuWorkState.WorkUserList)
                            if (wU.O_pass.HasValue && wU.O_pass.Value)
                                return false;

                        //当前状态已经有人填了校审意见，不允许撤销
                        if (workFlow.CuWorkState.WorkAuditList != null && workFlow.CuWorkState.WorkAuditList.Count > 0)
                            return false;

                        //当前登录者，是上一状态的WorkUser，并且点过，则允许撤销 
                        foreach (WorkUser wU in workFlow.CuWorkState.PreWorkState.WorkUserList)
                            if (wU.O_pass.HasValue && wU.User.ID == workFlow.dBSource.LoginUser.ID)
                                return true;

                        //刚发起到第二个状态，允许删除流程
                        if (workFlow.CuWorkState.PreWorkState.Code == "START")
                            if (workFlow.WorkStateList[0].WorkUserList[0].User.ID == workFlow.dBSource.LoginUser.ID)
                                return true;

                        //退回到发起状态，允许删除流程
                        if (workFlow.WorkStateList[0].WorkUserList[0].User.ID == workFlow.dBSource.LoginUser.ID)
                            if (workFlow.CuWorkState.Code == workFlow.WorkStateList[0].Code)
                                return true;
                    }

                }
            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
            }

            return false;
        }

        /// <summary>
        /// 是否有关联工作流对象存在
        /// </summary>
        /// <param name="wf">工作流对象</param>
        /// <returns></returns>
        private static bool HasOtherWorkflows(WorkFlow wf)
        {
            return (OtherWorkflowsList(wf) != null && OtherWorkflowsList(wf).Count > 0);
        }

        private static List<WorkFlow> historyWorkflowList;

        /// <summary>
        /// 获取相关工作流对象
        /// </summary>
        /// <param name="wf">工作流对象</param>
        /// <returns>相关工作流对象列表</returns>
        private static List<WorkFlow> OtherWorkflowsList(WorkFlow wf)
        {
            //get
            {
                if (historyWorkflowList == null)
                {

                    //返回值
                    historyWorkflowList = new List<WorkFlow>();

                    //当前已经加载的wf的.O_WorkFlowno
                    List<int> lstExistWfNumber = new List<int>();

                    if (wf.DocList != null && wf.DocList.Count > 0)
                    {
                        foreach (Doc d in wf.DocList)
                        {

                            if (d.WorkFlowList == null || d.WorkFlowList.Count <= 0)
                                continue;

                            foreach (WorkFlow wf0 in d.WorkFlowList)
                            {
                                if (wf0.O_WorkFlowno == wf.O_WorkFlowno)
                                    continue;

                                if (lstExistWfNumber.Contains(wf0.O_WorkFlowno))
                                    continue;
                                lstExistWfNumber.Add(wf0.O_WorkFlowno);

                                historyWorkflowList.Add(wf0);
                            }
                        }
                    }
                }

                return historyWorkflowList;
            }
        }


        /// <summary>
        /// 选择流程分支
        /// </summary>
        /// <param name="branchList"></param>
        /// <returns></returns>
        private static WorkStateBranch SelectWorkStateBranch(List<WorkStateBranch> branchList)
        {

            try
            {
                if (branchList == null || branchList.Count <= 0)
                    return null;

                //选择流程分支，如果下一级分支列表只有一个分支，选择第一个分支
                if (branchList.Count == 1)
                    return branchList[0];

                if (branchList.Count > 1)
                    return branchList[0];
            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
            }

            return null;
        }


        /// <summary>
        /// 如果定义流程的定义状态中存在需要选择专业的定义状态,则需要提前选择专业及人员,并实例相应的状态
        /// </summary>
        /// <param name="workFlow">工作流对象</param>
        /// <param name="userlist">用户列表</param>
        /// <returns></returns>
        private static bool SelProfessionOnWorkFlowStart(WorkFlow workFlow, string userlist)
        {
            try
            {
                if (workFlow == null
                    || workFlow.DefWorkFlow == null || workFlow.DefWorkFlow.DefWorkStateList == null
                    || workFlow.DefWorkFlow.DefWorkStateList.Count <= 0)
                    return false;

                //遍历所有的DefWorkState,找出其中需要在启动时选择人员的定义状态,分别实例这些并提示选择人员
                foreach (DefWorkState defWorkState in workFlow.DefWorkFlow.DefWorkStateList)
                {
                    if (defWorkState.StartSelectUser)
                    {

                        WorkState newWorkState = null;
                        bool bExisted = false;

                        //如果已经存在状态是用这个模板,则跳过
                        if (workFlow.WorkStateList != null && workFlow.WorkStateList.Count > 0)
                        {
                            foreach (WorkState wState in workFlow.WorkStateList)
                            {
                                if (wState.DefWorkState != null && wState.DefWorkState.O_No == defWorkState.O_No)
                                {
                                    bExisted = true;
                                    newWorkState = wState;
                                    break;
                                }
                            }
                        }

                        //该状态需要提前选择人员,并实例状态
                        if (!bExisted)
                        {
                            newWorkState = workFlow.NewWorkState(defWorkState);

                        }

                        if (newWorkState == null || newWorkState.DefWorkState == null)
                            continue;

                        AVEVA.CDMS.Server.Group group = WebSelUserForNextState.SelectUser(workFlow, newWorkState);

                        if (group == null) group = newWorkState.CheckGroup;

                        //在状态中增加用户
                        if (group != null)
                        {
                            if (!(newWorkState.SaveSelectUser(group)))
                            {
                                //MessageBox.Show("保存 " + + " 失败!");
                                return false;
                            }
                        }
                        else return false;

                    }
                }

                return true;
            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
            }
            return false;
        }

    }

}
