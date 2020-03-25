using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVEVA.CDMS.Server;
using AVEVA.CDMS.Common;
using AVEVA.CDMS.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace AVEVA.CDMS.WTMS_Plugins
{
    public class WorkTaskService
    {
        /// <summary>
        /// 创建报审单文档
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="ProjectKeyword">目录Keyword</param>
        /// <param name="documentType">文档表式</param>
        /// <param name="fileCode">文档编号</param>
        /// <param name="docAttrJson">文档属性</param>
        /// <returns></returns>
        public static JObject CreateWorkTask(string sid, string ProjectKeyword, string title, string docAttrJson)
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

                Project proj = dbsource.GetProjectByKeyWord(ProjectKeyword);
                if (proj == null)
                {
                    reJo.msg = "错误的操作信息！指定的文件夹不存在！";
                    return reJo.Value;
                }

                string TEXT1 = "", content = "", TEXT3 = "", TEXT4 = "";

                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(docAttrJson);

                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();

                    //获取文件编码 
                    if (strName == "TEXT1") TEXT1 = strValue.Trim();    //标题
                    if (strName == "TEXT2") content = strValue.Trim();    //内容
                    if (strName == "TEXT3") TEXT3 = strValue.Trim();    //要求回复日期
                    if (strName == "TEXT4") TEXT4 = strValue.Trim();    //收文单位

                }

                string[] strAry = TEXT4.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                #region 获取单位
                List<DictData> unitList = new List<DictData>();

                List<DictData> dictDataList = dbsource.GetDictDataList("Unit");
                foreach (string strUnitItem in strAry)
                {

                    //[o_Code]:英文描述,[o_Desc]：中文描述,[o_sValue1]：通信代码
                    //string str3 = m_Project.ExcuteDefnExpression("$(DESIGNPROJECT_CODE)")[0];
                    foreach (DictData data6 in dictDataList)
                    {
                        if (!string.IsNullOrEmpty(data6.O_Code) && (strUnitItem == data6.O_Code))
                        {
                            unitList.Add(data6);
                        }
                    }
                }
                string strUnitLst = "";
                foreach (DictData data6 in unitList)
                {

                    //strUnitLst = strUnitLst + data6.O_Code + "__" + data6.O_Desc + ",";
                    strUnitLst = strUnitLst + data6.O_Code + ",";
                }
                if (strUnitLst.Length > 1) strUnitLst = strUnitLst.Substring(0, strUnitLst.Length - 1);
                #endregion

                DateTime reqResDt = Convert.ToDateTime(TEXT3);

                #region 生成任务目录


                List<TempDefn> TempDefns = dbsource.GetTempDefnByKeyWord("WTMS_WORKTASKFOLDER");//获取工作任务目录模板
                if (TempDefns.Count <= 0)
                {
                    reJo.msg = "没有与其相关的文件夹模板，创建无法正常完成";
                    return reJo.Value;
                }

                TempDefn mTempDefn = TempDefns[0];

                if (mTempDefn == null)
                {
                    reJo.msg = "没有与其相关的文件夹模板，创建无法正常完成";
                    return reJo.Value;
                }

                List<TempDefn> unitTempDefns = dbsource.GetTempDefnByKeyWord("WTMS_UNITFOLDER");//获取工作任务目录模板
                if (unitTempDefns.Count <= 0)
                {
                    reJo.msg = "没有与其相关的文件夹模板，创建无法正常完成";
                    return reJo.Value;
                }

                TempDefn mUnitTempDefn = unitTempDefns[0];

                if (mUnitTempDefn == null)
                {
                    reJo.msg = "没有与其相关的文件夹模板，创建无法正常完成";
                    return reJo.Value;
                }

                Project projectByName = proj;

                bool hasRepeated = false;

                //查找源目录是否重复编码的接口
                foreach (Doc docItem in projectByName.DocList)
                {
                    if (docItem.TempDefn != null && docItem.Code.IndexOf(title) >= 0)
                    {

                        hasRepeated = true;
                        break;
                    }
                }

                if (hasRepeated == true)
                {
                    reJo.msg = "已存在重复编码的函件 " + title + " ！";
                    return reJo.Value;
                }

                //创建存储路径
                string destPath = projectByName.FullPath;
                if (!Directory.Exists(destPath))
                {
                    Directory.CreateDirectory(destPath);
                }

                string itemName = "";
                IEnumerable<string> source = from prj in projectByName.ChildProjectList select prj.Code;
                itemName = title;
                if (source.Contains<string>(itemName))
                {
                    for (int i = 1; i < 0x3e8; i++)
                    {
                        itemName = title + i.ToString();
                        if (!source.Contains<string>(itemName))
                        {
                            break;
                        }
                    }
                }

                Storage storage = projectByName.Storage;

                Project newProj = projectByName.NewProject(itemName, "", storage, mTempDefn);

                if (newProj == null)
                {
                    reJo.msg = "新建" + title + "出错！";
                    return reJo.Value;
                }

                newProj.GetAttrDataByKeyWord("TASK_DESC").SetCodeDesc(content); //任务描述
                newProj.GetAttrDataByKeyWord("TASK_REQUESRESTIME").SetCodeDesc(reqResDt.ToString("yyyy-MM-dd"));    //要求回复日期
                newProj.GetAttrDataByKeyWord("TASK_RECUNIT").SetCodeDesc(strUnitLst);    //收文单位
                newProj.GetAttrDataByKeyWord("TASK_RECUNITCOUNT").SetCodeDesc(unitList.Count.ToString());  //收文单位数量
                newProj.AttrDataList.SaveData();

                #endregion
                #region 生成任务目录下的单位目录

                foreach (DictData data6 in unitList)
                {
                    Project unitProj = newProj.NewProject(data6.O_Code, data6.O_Desc, storage, mUnitTempDefn);

                    if (unitProj == null)
                    {
                        continue;
                    }

                }

                bool flag = projectByName.AttrDataList.SaveData();

                #endregion

                reJo.success = true;
                reJo.data = new JArray(new JObject(new JProperty("ProjectKeyword", newProj.KeyWord)));
                //new JProperty("DocKeyword", doc.KeyWord), new JProperty("DocList", strDocList),
                //new JProperty("DocCode", doc.Code)));
                return reJo.Value;
            }
            catch (Exception exception)
            {
                WebApi.CommonController.WebWriteLog(exception.Message + "\r\n" + exception.Source + "\r\n" + exception.StackTrace);
                reJo.msg = "创建工作任务失败！" + exception.Message + "\r\n" + exception.Source + "\r\n" + exception.StackTrace;
            }
            return reJo.Value;
        }


        public static JObject ReplyWorkTask(string sid, string ProjectKeyword)
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

                Project proj = dbsource.GetProjectByKeyWord(ProjectKeyword);
                if (proj == null)
                {
                    reJo.msg = "错误的操作信息！指定的文件夹不存在！";
                    return reJo.Value;
                }

                Project rootProj = CommonFunction.getParentProjectByTempDefn(proj,"WTMS_DOCUMENTSYSTEM");
                if (rootProj == null)
                {
                    reJo.msg = "错误的操作信息！项目文件夹不存在！";
                    return reJo.Value;
                }

                 #region 获取回复工作任务目录模板
                List<TempDefn> TempDefns = dbsource.GetTempDefnByKeyWord("WTMS_RESPONSEFOLDER");//获取工作任务目录模板
                if (TempDefns.Count <= 0)
                {
                    reJo.msg = "没有与其相关的文件夹模板，创建无法正常完成";
                    return reJo.Value;
                }

                TempDefn mTempDefn = TempDefns[0];

                if (mTempDefn == null)
                {
                    reJo.msg = "没有与其相关的文件夹模板，创建无法正常完成";
                    return reJo.Value;
                } 
                #endregion

                
                #region 获取回复工作任务目录模板
                TempDefns = dbsource.GetTempDefnByKeyWord("WTMS_UNITFOLDER");//获取单位目录模板
                if (TempDefns.Count <= 0)
                {
                    reJo.msg = "没有与其相关的文件夹模板，创建无法正常完成";
                    return reJo.Value;
                }

                TempDefn mUnitTempDefn = TempDefns[0];

                if (mUnitTempDefn == null)
                {
                    reJo.msg = "没有与其相关的文件夹模板，创建无法正常完成";
                    return reJo.Value;
                } 
                #endregion

                //获取单位文控是当前用户的单位列表
                List<DictData> secUnitList = getSecDictList(curUser);
                if (secUnitList.Count <= 0) {
                    reJo.msg = "错误的操作信息！单位文控信息错误！";
                    return reJo.Value; 
                }

                string newPrjCode = proj.Code + "回复";

                Project cps= rootProj.ChildProjectList.Find(cp=>cp.Code==newPrjCode);
                if (cps!=null){
                    reJo.msg = "错误的操作信息！工作任务已经回复！";
                    return reJo.Value; 
                }

                Project newProj=new Project();

                string strunit = secUnitList[0].O_Code;

                bool isCreate = false;

                Project unitPrjItem=rootProj.ChildProjectList.Find(cp=>cp.Code == strunit || cp.Code == secUnitList[0].O_Desc);
                //新建单位目录
                if(unitPrjItem==null){
                    unitPrjItem= rootProj.NewProject(secUnitList[0].O_Code, secUnitList[0].O_Desc, proj.Storage, mUnitTempDefn);
                }

                if (unitPrjItem!=null)
                    newProj = unitPrjItem.NewProject(newPrjCode, "", proj.Storage, mTempDefn);


                reJo.success = true;
                reJo.data = new JArray(new JObject(new JProperty("ProjectKeyword", newProj.KeyWord)));

                return reJo.Value;
            }
            catch (Exception exception)
            {
                WebApi.CommonController.WebWriteLog(exception.Message + "\r\n" + exception.Source + "\r\n" + exception.StackTrace);
                reJo.msg = "回复工作任务失败！" + exception.Message + "\r\n" + exception.Source + "\r\n" + exception.StackTrace;
            }
            return reJo.Value;
        }

        /// <summary>
        /// 获取用户工作任务列表（包括需要回复的任务和已回复的任务）
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public static JObject GetUserWorkTaskList(string sid, string page, string limit)
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
                
                #region 初始化参数

                page = page ?? "1";
                limit = limit ?? "50";


                //分页参数
                int ShowNum = 50;  //每页显示行数
                int CurPage = 1;   //第几页
                try
                {
                    ShowNum = int.Parse(limit);
                }
                catch (Exception e)
                {
                    CommonController.WebWriteLog(e.Message);
                }
                try
                {
                    CurPage = int.Parse(page);
                }
                catch (Exception e)
                {
                    CommonController.WebWriteLog(e.Message);
                }
                // CurPage--;

                //获取用户单位代码
                //#region 获取单位文控是当前用户的单位列表
                //string strUser = curUser.ToString;

                ////获取单位文控是当前用户的单位列表
                //List<DictData> secUnitList = new List<DictData>();

                //List<DictData> dictDataList = dbsource.GetDictDataList("Unit");

                ////[o_Code]:英文描述,[o_Desc]：中文描述,[O_sValue3]：文控
                //foreach (DictData data6 in dictDataList)
                //{
                //    if (!string.IsNullOrEmpty(data6.O_sValue3) && (data6.O_sValue3.Contains(strUser)))
                //    {
                //        secUnitList.Add(data6);
                //    }
                //} 
                //#endregion

                //获取单位文控是当前用户的单位列表
                List<DictData> secUnitList = getSecDictList(curUser);

                //List<User> secUserList = CommonFunction.GetUserListByDesc(dbsource, strSecretarilman);

                #region 获取工作任务目录模板
                List<TempDefn> TempDefns = dbsource.GetTempDefnByKeyWord("WTMS_WORKTASKFOLDER");//获取工作任务目录模板
                if (TempDefns.Count <= 0)
                {
                    reJo.msg = "没有与其相关的文件夹模板，创建无法正常完成";
                    return reJo.Value;
                }

                TempDefn mTempDefn = TempDefns[0];

                if (mTempDefn == null)
                {
                    reJo.msg = "没有与其相关的文件夹模板，创建无法正常完成";
                    return reJo.Value;
                } 
                #endregion

                JArray jaData = new JArray(); 
                #endregion



                string strTdId = mTempDefn.ID.ToString();

                Dictionary<Project, string> projDict = new Dictionary<Project, string>();

                List<WTask> needReplyTasks = new List<WTask>();
                

                //需要回复工作任务
                List<Project> needReplyTaskList = new List<Project>();

                //已回复工作任务
                List<Project> repliedTaskList = new List<Project>();

                //获取工作任务目录
                List<Project> projList = dbsource.SelectProject("select * from CDMS_Project where o_DefnID=" + strTdId + " and o_type!=12");

                foreach(Project prj in projList){
                    Project prjProject = CommonFunction.getParentProjectByTempDefn(prj, "WTMS_DOCUMENTSYSTEM");
                    if (prjProject == null)
                    {
                        continue;
                    }

                    string recunit = prj.GetAttrDataByKeyWord("TASK_RECUNIT").ToString;
                    if (string.IsNullOrEmpty(recunit)) continue;

                    string[] strUnitAry = recunit.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    //获取单位代码相同，且项目代码相同的单位列表
                    List<DictData> sddList = secUnitList.Where(l => l.O_sValue1 == prjProject.Code && strUnitAry.Contains(l.O_Code)).ToList();
                    
                    if (sddList.Count > 0) {
                        needReplyTaskList.Add(prj);
                        WTask wt = new WTask();
                        wt.RootProject = prjProject;
                        wt.WorkTaskProject = prj;
                        needReplyTasks.Add(wt);

                        projDict.Add(prj, prjProject.Code);
                    }
                }

                //foreach (Project prjItem in needReplyTaskList)
                foreach (var wt in needReplyTasks)
                {
                    //Doc doc = prjItem.ShortCutDoc;

                    string reqResTime = wt.WorkTaskProject.GetValueByKeyWord("TASK_REQUESRESTIME");
                    string isMount = wt.RootProject.GetValueByKeyWord("CD_ISMOUNT");
                    string mountState = "未关闭";
                    if (isMount == "是")
                    {
                        mountState = "已关闭";
                    }
                    jaData.Add(new JObject(new JProperty("Keyword", wt.WorkTaskProject.KeyWord),
                        new JProperty("Title", wt.WorkTaskProject.ToString),   //工作任务
                        //new JProperty("Creater", kvp.Key.Creater.O_userdesc),
                        new JProperty("Creater", wt.WorkTaskProject.Creater.O_userdesc),
                        new JProperty("ProjectName", wt.RootProject.Code),    //项目名
                        new JProperty("ReqResTime", reqResTime),
                        new JProperty("CreateTime", wt.WorkTaskProject.O_credatetime.ToString("yyyy.MM.dd")),
                        new JProperty("MountState", mountState)));
                }

                reJo.total = needReplyTaskList.Count;
                reJo.data = jaData;

                reJo.success = true;
                return reJo.Value;
            }
            catch (Exception exception)
            {
                WebApi.CommonController.WebWriteLog(exception.Message + "\r\n" + exception.Source + "\r\n" + exception.StackTrace);
                reJo.msg = "获取工作任务列表失败！" + exception.Message + "\r\n" + exception.Source + "\r\n" + exception.StackTrace;
            }
            return reJo.Value;
        }

        public static JObject GetWorkTaskByKeyword(string sid, string ProjectKeyword) { 
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

                JArray jaData = new JArray();

                Project proj = dbsource.GetProjectByKeyWord(ProjectKeyword);
                if (proj == null)
                {
                    reJo.msg = "错误的操作信息！指定的文件夹不存在！";
                    return reJo.Value;
                }

                //要求回复时间
                string taskTitle = proj.Code;
                string taskDesc = proj.GetValueByKeyWord("TASK_DESC");
                string reqResTime = proj.GetValueByKeyWord("TASK_REQUESRESTIME");
                string recUnit = proj.GetValueByKeyWord("TASK_RECUNIT");

                string[] strAry = recUnit.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                #region 获取单位
                List<DictData> unitList = new List<DictData>();

                List<DictData> dictDataList = dbsource.GetDictDataList("Unit");
                foreach (string strUnitItem in strAry)
                {

                    //[o_Code]:英文描述,[o_Desc]：中文描述,[o_sValue1]：通信代码
                    //string str3 = m_Project.ExcuteDefnExpression("$(DESIGNPROJECT_CODE)")[0];
                    foreach (DictData data6 in dictDataList)
                    {
                        if (!string.IsNullOrEmpty(data6.O_Code) && (strUnitItem == data6.O_Code))
                        {
                            unitList.Add(data6);
                        }
                    }
                }
                string strUnitLst = "";
                foreach (DictData data6 in unitList)
                {

                    strUnitLst = strUnitLst + data6.O_Code + "__" + data6.O_Desc + ",";
                    //trUnitLst = strUnitLst + data6.O_Code + ",";
                }
                if (strUnitLst.Length > 1) strUnitLst = strUnitLst.Substring(0, strUnitLst.Length - 1);
                #endregion

                jaData.Add(new JObject(new JProperty("Title", taskTitle),
                    //new JProperty("ProjectName", wt.RootProject.Code),    //项目名
                    new JProperty("TaskDesc", taskDesc),
                    new JProperty("ReqResTime", reqResTime),
                    new JProperty("RecUnit", strUnitLst)
                    ));

                reJo.data = jaData;

                reJo.success = true;
                return reJo.Value;
            }
            catch (Exception exception)
            {
                WebApi.CommonController.WebWriteLog(exception.Message + "\r\n" + exception.Source + "\r\n" + exception.StackTrace);
                reJo.msg = "获取工作任务失败！" + exception.Message + "\r\n" + exception.Source + "\r\n" + exception.StackTrace;
            }
            return reJo.Value;
        }

        private static List<DictData> getSecDictList(User curUser)
        {

            #region 获取单位文控是当前用户的单位列表
            string strUser = curUser.ToString;

            //获取单位文控是当前用户的单位列表
            List<DictData> secUnitList = new List<DictData>();

            List<DictData> dictDataList = curUser.dBSource.GetDictDataList("Unit");

            //[o_Code]:英文描述,[o_Desc]：中文描述,[O_sValue3]：文控
            foreach (DictData data6 in dictDataList)
            {
                if (!string.IsNullOrEmpty(data6.O_sValue3) && (data6.O_sValue3.Contains(strUser)))
                {
                    secUnitList.Add(data6);
                }
            }
            return secUnitList;
            #endregion
        }
    }

    public class WTask {
        /// <summary>
        /// 工作任务目录
        /// </summary>
        public Project WorkTaskProject;
        /// <summary>
        /// 项目目录
        /// </summary>
        public Project RootProject;
    }
}
