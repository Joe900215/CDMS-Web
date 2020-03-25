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
    public class WorkTask
    {
        /// <summary>
        /// 创建工作任务
        /// </summary>
        /// <param name="sid">sid</param>
        /// <param name="ProjectKeyword">选择的Project</param>
        /// <param name="docAttrJson">参数Json</param>
        /// <returns>
        /// <para>docAttrJson例子：</para>
        /// <code>
        /// [
        ///    { name: 'taskName', value: taskName },   //工作任务名称
        ///    { name: 'taskContent', value: taskContent }, //工作任务内容
        ///    { name: 'taskStartDate', value: taskStartDate }, //任务开始时间
        ///    { name: 'taskEndDate', value: taskEndDate }  //任务结束时间
        ///]
        ///</code>
        /// </returns>
        public static JObject CreateTask(string sid, string ProjectKeyword, string docAttrJson)
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

                Project m_Project = dbsource.GetProjectByKeyWord(ProjectKeyword);

                if (m_Project == null)
                {
                    reJo.msg = "参数错误！文件夹不存在！";
                    return reJo.Value;
                }

                string strTaskName = "", strTaskContent = "",
                    strTaskStartDate = "", strTaskEndDate = "";

                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(docAttrJson);

                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();

                    //获取工作任务名称
                    if (strName == "taskName") strTaskName = strValue;

                    //获取工作任务内容
                    else if (strName == "taskContent") strTaskContent = strValue;

                    //获取任务开始时间
                    else if (strName == "taskStartDate") strTaskStartDate = strValue;

                    //获取任务结束时间
                    else if (strName == "taskEndDate") strTaskEndDate = strValue;
                }

                if (string.IsNullOrEmpty(strTaskName))
                {
                    reJo.msg = "任务名称不能为空！";
                    return reJo.Value;
                }

                if (string.IsNullOrEmpty(strTaskContent))
                {
                    reJo.msg = "任务内容不能为空！";
                    return reJo.Value;
                }

                //获取工作任务内容模板
                List<TempDefn> tempDefnByCode = dbsource.GetTempDefnByCode("TASK");
                TempDefn mTempDefn = (tempDefnByCode != null) ? tempDefnByCode[0] : null;
                if (mTempDefn == null)
                {
                    reJo.msg = "工作任务模板不存在！";
                    return reJo.Value;
                }

                //如果目录存在  创建目录(指定存储空间和项目模板) 
                Project project = m_Project.NewProject(strTaskName, "", null, mTempDefn);
                if (project == null)
                {
                    reJo.msg = "创建工作任务目录失败！";
                    return reJo.Value;
                }


                //下面是将前面填写的数据，赋值到属性里，然后存进数据库
                AttrData attrDataByKeyWord = project.GetAttrDataByKeyWord("WORKTEST");
                if (attrDataByKeyWord != null)
                {
                    attrDataByKeyWord.SetCodeDesc(strTaskContent);

                }
                //attrDataByKeyWord = project.GetAttrDataByKeyWord("TASKOWNER");
                //if (attrDataByKeyWord != null)
                //{
                //    attrDataByKeyWord.SetCodeDesc(TC_TASKOWNER);
                //}
                attrDataByKeyWord = project.GetAttrDataByKeyWord("TASKPLANFINISHDATE");
                if (attrDataByKeyWord != null)
                {
                    attrDataByKeyWord.SetCodeDesc(strTaskEndDate);

                }
                attrDataByKeyWord = project.GetAttrDataByKeyWord("TASKPLANSTARTDATE");
                if (attrDataByKeyWord != null)
                {
                    attrDataByKeyWord.SetCodeDesc(strTaskStartDate);

                }

                //存进数据库
                project.AttrDataList.SaveData();

                //刷新数据源
                DBSourceController.RefreshDBSource(sid);

                reJo.data = new JArray(new JObject(new JProperty("ProjectKeyword", project.KeyWord)));

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
        /// 创建用户工作任务（在工日填报，即任务报告表单创建）
        /// </summary>
        /// <param name="sid">sid</param>
        /// <param name="UserKeyword">任务的责任人,默认是当前登录用户，如果是指派给其他用户的任务，就输入所指派用户的UserKeyword.</param>
        /// <param name="docAttrJson">参数Json，详见下面例子</param>
        /// <returns>
        /// <para>"docAttrJson例子：</para>
        /// <code>
        /// [
        ///    { name: 'taskName', value: taskName },   //工作任务名称
        ///    { name: 'taskContent', value: taskContent }, //工作任务内容
        ///    { name: 'taskStartDate', value: taskStartDate }, //任务开始时间
        ///    { name: 'taskEndDate', value: taskEndDate }  //任务结束时间
        /// ]
        ///</code>
        /// </returns>
        public static JObject CreateUserTask(string sid, string UserKeyword, string docAttrJson)
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

                User m_User = dbsource.GetUserByKeyWord(UserKeyword);

                if (m_User == null)
                {
                    m_User = curUser;
                }

                string strTaskName = "", strTaskContent = "",
                    strTaskStartDate = "", strTaskEndDate = "";

                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(docAttrJson);

                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();

                    //获取工作任务名称
                    if (strName == "taskName") strTaskName = strValue;

                    //获取工作任务内容
                    else if (strName == "taskContent") strTaskContent = strValue;

                    //获取任务开始时间
                    else if (strName == "taskStartDate") strTaskStartDate = strValue;

                    //获取任务结束时间
                    else if (strName == "taskEndDate") strTaskEndDate = strValue;
                }

                if (string.IsNullOrEmpty(strTaskName))
                {
                    reJo.msg = "任务名称不能为空！";
                    return reJo.Value;
                }

                if (string.IsNullOrEmpty(strTaskContent))
                {
                    reJo.msg = "任务内容不能为空！";
                    return reJo.Value;
                }
                
                DateTime startTime = Convert.ToDateTime(strTaskStartDate);
                DateTime endTime = Convert.ToDateTime(strTaskEndDate);

                AVEVA.CDMS.Server.Task newTask = dbsource.NewTask(enTaskLevel.Common, enTaskStatus.Runing, "任务", strTaskName, strTaskContent, m_User, null, startTime, endTime, 0, null);

                if (newTask == null)
                {
                    reJo.msg = "新建任务失败！";
                    return reJo.Value;
                }

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
        /// 获取任务报告表单默认参数
        /// </summary>
        /// <param name="sid">连接密钥</param>
        /// <returns></returns>
        public static JObject GetTaskReportDefault(string sid)
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

                #region 统计任务

                JArray jaTask = new JArray();

                List<AVEVA.CDMS.Server.Task> taskList = curUser.RuningTaskList;
                foreach (AVEVA.CDMS.Server.Task task in taskList)
                {
                    if (task.ProjectList != null && task.ProjectList.Count > 0)
                    {
                        if (task.ProjectList[0].WorkFlow == null) {
                            continue;
                        }

                        //当任务的流程在project的时候，如果流程已经完成，就不显示任务
                        WorkFlow taskWf=task.ProjectList[0].WorkFlow;
                        if (taskWf.O_WorkFlowStatus == enWorkFlowStatus.Finish)
                            continue;

                        //当任务的流程在project的时候，如果状态已经通过，也不显示任务
                        WorkState taskWfState =taskWf.WorkStateList.Find(ws => (ws.User == curUser && task.Description.IndexOf(ws.Description) >= 0));
                        if (taskWfState!=null && taskWfState.O_FinishDate.HasValue == true)
                        {
                            continue;
                        }
                    }
                    jaTask.Add(new JObject(new JProperty("taskKeyword", task.KeyWord),  //任务keyowrd
                            new JProperty("taskType", task.O_TaskType), //任务类型
                            new JProperty("taskName",task.O_Topics) //任务名称
                        ));

                }

                #endregion

                reJo.data = new JArray(new JObject(
                    new JProperty("taskList", jaTask)
                    ));

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
        /// 获取任务的报告列表
        /// </summary>
        /// <param name="sid">连接密钥</param>
        /// <param name="TaskKeyword">任务keyword</param>
        /// <returns></returns>
        public static JObject GetTaskReportList(string sid,string TaskKeyword)
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

                object obj=dbsource.GetObjectByKeyWord(TaskKeyword);
                if (obj == null)
                {
                    reJo.msg = "参数错误，获取任务失败！";
                    return reJo.Value;
                }

                if (!(obj is AVEVA.CDMS.Server.Task))
                {
                    reJo.msg = "获取任务失败！";
                    return reJo.Value;
                }

                AVEVA.CDMS.Server.Task mTask = (AVEVA.CDMS.Server.Task)obj;

                JArray jaReport = new JArray();
                if (mTask.TaskReportList != null && mTask.TaskReportList.Count > 0)
                {

                    foreach (TaskReport tp in mTask.TaskReportList)
                    {
                        jaReport.Add(new JObject(new JProperty("reportKeyword", tp.KeyWord),  //任务报告keyowrd
                            new JProperty("reportContent", tp.O_Content), //任务报告内容
                            new JProperty("reportDate", tp.O_Date.ToString()), //任务报告日期
                            new JProperty("schedule", tp.O_Schedule.ToString()) //任务报告进度
                        ));
                    }
                }

                reJo.data = new JArray(new JObject(
                    new JProperty("reportList", jaReport)
                    ));

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
        /// 获取任务报告内容
        /// </summary>
        /// <param name="sid">连接密钥</param>
        /// <param name="ReportKeyword">报告关键字</param>
        /// <returns></returns>
        public static JObject GetTaskReport(string sid, string ReportKeyword)
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

                object obj = dbsource.GetObjectByKeyWord(ReportKeyword);
                if (obj == null)
                {
                    reJo.msg = "参数错误，获取任务报告失败！";
                    return reJo.Value;
                }

                if (!(obj is AVEVA.CDMS.Server.TaskReport))
                {
                    reJo.msg = "获取任务报告失败！";
                    return reJo.Value;
                }

                AVEVA.CDMS.Server.TaskReport mTaskReport = (AVEVA.CDMS.Server.TaskReport)obj;


                JObject joReport = new JObject(
                    new JProperty("reportContent", mTaskReport.O_Content),//报告内容
                    new JProperty("reportProblom", mTaskReport.O_Problom),//存在问题
                    //new JProperty("reportDate", mTaskReport.O_Date.Value.ToString("U")),//报告日期
                    new JProperty("reportDate", mTaskReport.O_Date.Value.ToString("yyyy/MM/dd HH:mm:ss")),//报告日期
                    new JProperty("startDate", mTaskReport.O_StartDate.Value.ToString("yyyy/MM/dd HH:mm:ss")),//开始日期
                    new JProperty("schedule", mTaskReport.O_Schedule.ToString()),//进度
                    new JProperty("laborTime", mTaskReport.O_Time.ToString()) //实耗工时
                    );

                if (mTaskReport.O_EndDate != null)
                    joReport.Add(new JProperty("endDate", mTaskReport.O_EndDate.Value.ToString("yyyy/MM/dd HH:mm:ss")));

                reJo.data = new JArray(joReport);

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
        /// 创建任务报告
        /// </summary>
        /// <param name="sid">连接密钥</param>
        /// <param name="TaskKeyword">任务关键字</param>
        /// <param name="reportAttrJson">参数字符串，详见下面例子</param>
        /// <returns>
        /// <para>reportAttrJson例子：</para>
        /// <code>
        /// [
        ///    { name: 'content', value: strContent },  //任务报告内容
        ///    { name: 'problom', value: strProblem },  //任务报告存在问题
        ///    { name: 'schedule', value: strSchedule },    //任务报告进度
        ///    { name: 'laborTime', value: strLaborTime },  //实耗工时
        ///    { name: 'reportDate', value: strReportDate },    //任务报告时间
        ///    { name: 'startDate', value: strStartDate },  //任务报告开始时间
        ///    { name: 'endDate', value: strEndDate }   //任务报告结束时间
        ///]
        ///</code>
        /// </returns>
        public static JObject CreateTaskReport(string sid, string TaskKeyword,string reportAttrJson)
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

                object obj = dbsource.GetObjectByKeyWord(TaskKeyword);
                if (obj == null)
                {
                    reJo.msg = "参数错误，获取任务失败！";
                    return reJo.Value;
                }

                if (!(obj is AVEVA.CDMS.Server.Task))
                {
                    reJo.msg = "获取任务失败！";
                    return reJo.Value;
                }

                AVEVA.CDMS.Server.Task mTask = (AVEVA.CDMS.Server.Task)obj;

                //获取报告属性参数
                string strContent = "", strProblom = "",
                    strSchedule = "", strLaborTime = "",
                    strReportDate = "", strStartDate = "",
                    strEndDate = "";

                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(reportAttrJson);

                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();

                    //获取任务报告内容
                    if (strName == "content") strContent = strValue;

                    //获取任务报告存在问题
                    else if (strName == "problom") strProblom = strValue;

                    //获取任务报告进度
                    else if (strName == "schedule") strSchedule = strValue;

                    //获取实耗工时
                    else if (strName == "laborTime") strLaborTime = strValue;

                    //获取任务报告时间
                    else if (strName == "reportDate") strReportDate = strValue;

                    //获取任务报告开始时间
                    else if (strName == "startDate") strStartDate = strValue;

                    //获取任务报告结束时间
                    else if (strName == "endDate") strEndDate = strValue;
                }

                if (string.IsNullOrEmpty(strContent))
                {
                    reJo.msg = "请输入报告内容！";
                    return reJo.Value;
                }

                if (string.IsNullOrEmpty(strLaborTime))
                {
                    reJo.msg = "请输入实耗工时！";
                    return reJo.Value;
                }

                //int iTime = string.IsNullOrEmpty(strLaborTime) ? 0 : Convert.ToInt32(strLaborTime);
                float iTime = (float)(string.IsNullOrEmpty(strLaborTime) ? 0 : Convert.ToDecimal(strLaborTime));

                int iSchedule = string.IsNullOrEmpty(strSchedule) ? 0 : Convert.ToInt32(strSchedule);

                TaskReport newReport = mTask.NewTaskReport(DateTime.Now, iTime, iSchedule, strContent, strProblom);


                if (newReport == null)
                {
                    reJo.msg = "新增报告失败!" + mTask.dBSource.LastError;
                    return reJo.Value;
                }

                //修改开始时间和结束时间
                newReport.O_StartDate = Convert.ToDateTime(strStartDate);
                newReport.O_EndDate = Convert.ToDateTime(strEndDate);

                newReport.Modify();

                //更新任务报告列表
                //this.setTaskReportList();

                reJo.data = new JArray(new JObject(new JProperty("reportKeyword",newReport.KeyWord)));
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
        /// 修改任务报告
        /// </summary>
        /// <param name="sid">连接密钥</param>
        /// <param name="ReportKeyword">任务报告Keyword</param>
        /// <param name="reportAttrJson">参数字符串，详见下面例子</param>
        /// <returns>
        /// <para>reportAttrJson例子：</para>
        /// <code>
        ///  [
        ///    { name: 'content', value: strContent },  //任务报告内容
        ///    { name: 'problom', value: strProblem },  //任务报告存在问题
        ///    { name: 'schedule', value: strSchedule },    //任务报告进度
        ///    { name: 'laborTime', value: strLaborTime },  //实耗工时
        ///    { name: 'reportDate', value: strReportDate },    //任务报告时间
        ///    { name: 'startDate', value: strStartDate },  //任务报告开始时间
        ///    { name: 'endDate', value: strEndDate }   //任务报告结束时间
        ///]
        /// </code>
        /// </returns>
        public static JObject ModiTaskReport(string sid, string ReportKeyword,string reportAttrJson)
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

                object obj = dbsource.GetObjectByKeyWord(ReportKeyword);
                if (obj == null)
                {
                    reJo.msg = "参数错误，获取任务报告失败！";
                    return reJo.Value;
                }

                if (!(obj is AVEVA.CDMS.Server.TaskReport))
                {
                    reJo.msg = "获取任务报告失败！";
                    return reJo.Value;
                }

                AVEVA.CDMS.Server.TaskReport report = (AVEVA.CDMS.Server.TaskReport)obj;

                //获取报告属性参数
                string strContent = "", strProblom = "",
                    strSchedule = "", strLaborTime = "",
                    strReportDate="", strStartDate = "",
                    strEndDate = "";

                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(reportAttrJson);

                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();

                    //获取任务报告内容
                    if (strName == "content") strContent = strValue;

                    //获取任务报告存在问题
                    else if (strName == "problom") strProblom = strValue;

                    //获取任务报告进度
                    else if (strName == "schedule") strSchedule = strValue;

                    //获取实耗工时
                    else if (strName == "laborTime") strLaborTime = strValue;

                    //获取任务报告时间
                    else if (strName == "reportDate") strReportDate = strValue;

                    //获取任务报告开始时间
                    else if (strName == "startDate") strStartDate = strValue;

                    //获取任务报告结束时间
                    else if (strName == "endDate") strEndDate = strValue;
                }

                if (string.IsNullOrEmpty(strContent))
                {
                    reJo.msg = "请输入报告内容！";
                    return reJo.Value;
                }

                if (string.IsNullOrEmpty(strLaborTime))
                {
                    reJo.msg = "请输入实耗工时！";
                    return reJo.Value;
                }

                //修改报告

                report.O_Content = strContent;
                report.O_Problom = strProblom;
                //完成进度
                report.O_Schedule = string.IsNullOrEmpty(strSchedule) ? 0 : Convert.ToInt32(strSchedule);

                //float iTime = (float)(string.IsNullOrEmpty(strLaborTime) ? 0 : Convert.ToDecimal(strLaborTime));

                //实耗工时
                report.O_Time = (float)(string.IsNullOrEmpty(strLaborTime) ? 0 : Convert.ToDecimal(strLaborTime));
                //report.O_Time = (int)(string.IsNullOrEmpty(strLaborTime) ? 0 : Convert.ToInt32(strLaborTime));

                //报告生成时间
                report.O_Date = Convert.ToDateTime(strReportDate);

                //修改开始时间和结束时间
                report.O_StartDate = Convert.ToDateTime(strStartDate);
                report.O_EndDate = Convert.ToDateTime(strEndDate);

                report.Modify();


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
        /// 删除任务报告
        /// </summary>
        /// <param name="sid">连接密钥</param>
        /// <param name="ReportKeyword">任务报告Keyword</param>
        /// <returns></returns>
        public static JObject DeleteTaskReport(string sid, string ReportKeyword) {
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

                object obj = dbsource.GetObjectByKeyWord(ReportKeyword);
                if (obj == null)
                {
                    reJo.msg = "参数错误，获取任务报告失败！";
                    return reJo.Value;
                }

                if (!(obj is AVEVA.CDMS.Server.TaskReport))
                {
                    reJo.msg = "获取任务报告失败！";
                    return reJo.Value;
                }

                AVEVA.CDMS.Server.TaskReport report = (AVEVA.CDMS.Server.TaskReport)obj;

                if (report.Task.TaskUser != curUser)
                {
                    reJo.msg = "删除任务报告失败，您没有删除权限！";
                    return reJo.Value;
                }

                AVEVA.CDMS.Server.Task task = report.Task;

                report.Delete();

                task.Modify();

                //刷新数据源
                WebApi.DBSourceController.RefreshDBSource(sid);

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
