using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AVEVA.CDMS.Server;
using AVEVA.CDMS.Common;
using AVEVA.CDMS.WebApi;

namespace AVEVA.CDMS.TBSBIM_Plugins
{
    public class TBSProjectService
    {
        /// <summary>
        /// 检查项目Key
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="ProjectKey">项目Key，项目Key由项目目录ID和下载密码组成的字符串加密后生成</param>
        /// <returns></returns>
        public static JObject CheckProjectKey(string sid, string ProjectKey)
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


                string encrypKey = AVEVA.CDMS.WebApi.EnterPointController.ProcessRequest(ProjectKey);

                string[] arrayStr = encrypKey.Split("~~~".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                if (arrayStr == null || arrayStr.Length != 2)
                {
                    reJo.msg = "密钥解析错误！";
                    reJo.success = true;
                    reJo.data = new JArray(new JObject(new JProperty("KeyCorrect", "false")));
                    return reJo.Value;
                }

                if (string.IsNullOrEmpty(arrayStr[0]))
                {
                    reJo.msg = "项目ID解析错误！";
                    reJo.success = true;
                    reJo.data = new JArray(new JObject(new JProperty("KeyCorrect", "false")));
                    return reJo.Value;
                }

                int prjNo = Convert.ToInt32(arrayStr[0]);

                Project prj = dbsource.GetProjectByID(prjNo);
                if (prj == null)
                {
                    reJo.msg = "项目不存在！";
                    reJo.success = true;
                    reJo.data = new JArray(new JObject(new JProperty("KeyCorrect", "false")));
                    return reJo.Value;
                }

                string receiveKey = arrayStr[1];

                string downloadKey = prj.GetValueByKeyWord("TBS_DOWNLOADKEY").Replace("%2B", "+");

                if (string.IsNullOrEmpty(receiveKey) || string.IsNullOrEmpty(downloadKey) || downloadKey != ProjectKey)
                {
                    reJo.msg = "下载密钥不正确！";
                    reJo.success = true;
                    reJo.data = new JArray(new JObject(new JProperty("KeyCorrect", "false")));
                    return reJo.Value;
                }

                reJo.data = new JArray(new JObject(
                    new JProperty("ProjectKeyword", prj.KeyWord),
                    new JProperty("KeyCorrect", "true")));

                reJo.success = true;
                return reJo.Value;
            }
            catch (Exception ex)
            {

            }
            return reJo.Value;
        }

        /// <summary>
        /// 创建项目根目录
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="ProjectName"></param>
        /// <param name="DownloadPsw"></param>
        /// <returns></returns>
        public static JObject CreateTBSProject(string sid, string ProjectName, string DownloadPsw)
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

                if (string.IsNullOrEmpty(ProjectName))
                {
                    reJo.msg = "请输入项目名称！";
                    return reJo.Value;
                }

                if (string.IsNullOrEmpty(DownloadPsw))
                {
                    reJo.msg = "请输入下载密码！";
                    return reJo.Value;
                }

                Storage firstStorage = dbsource.AllStorageList[0];

                //JObject cJo = WebApi.ProjectController.CreateRootProject(sid, ProjectName, "", storage.Code, "TBS_DOCUMENTSYSTEM");

                string projectCode = ProjectName;
                string projectDesc = "";
                string storageName = firstStorage.Code;
                string TempDefnCode = "TBS_DOCUMENTSYSTEM";


                TempDefn mTempDefn = null;
                if (!string.IsNullOrEmpty(TempDefnCode))
                {
                    //  根据名称查找项目模板(根目录)对象
                    List<TempDefn> tempDefnByCode = dbsource.GetTempDefnByCode(TempDefnCode);
                    mTempDefn = (tempDefnByCode != null) ? tempDefnByCode[0] : null;
                    if (mTempDefn == null)
                    {
                        reJo.msg = "欲创建的文件夹关联的模板不存在！不能完成创建";
                        return reJo.Value;
                    }
                }

                Project pro = dbsource.NewProject(enProjectType.Local);

                //查找项目是否已经创建
                Project findProj = dbsource.RootLocalProjectList.Find(itemProj => itemProj.Code == projectCode);
                if (findProj != null)
                {
                    reJo.msg = "文件夹[" + projectCode + "]已存在！不能完成创建";
                    return reJo.Value;
                }

                //获取存储空间
                Storage storage = string.IsNullOrEmpty(storageName) ? null : dbsource.GetStorageByName(storageName);

                Project NewProject = pro.NewProject(projectCode, projectDesc, storage, mTempDefn);

                if (NewProject == null)
                {
                    reJo.msg = "新建文件夹失败！";
                    return reJo.Value;
                }



                //NewProject
                string downloadKey = (NewProject.ID.ToString() + "~~~" + DownloadPsw);
                string dlKey = WebApi.EnterPointController.enProcessRequest(downloadKey);
                CommonFunction.SetAttrDataValue(NewProject, "TBS_DOWNLOADKEY", dlKey);

                string manUser = curUser.ToString;
                NewProject.GetAttrDataByKeyWord("TBS_PRJMANAGER").SetCodeDesc(manUser);    //项目管理人

                NewProject.AttrDataList.SaveData();
                
                DBSourceController.refreshDBSource(sid);

                reJo.data = new JArray(new JObject(new JProperty("DownloadKey", dlKey)));

                reJo.success = true;
                return reJo.Value;
            }
            catch (Exception ex)
            {

            }
            return reJo.Value;

        }

        /// <summary>
        /// 获取项目概况默认属性信息
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="ProjectKeyword"></param>
        /// <returns></returns>
        public static JObject GetPrjOverviewDefault(string sid, string ProjectKeyword)
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
                Project prj = dbsource.GetProjectByKeyWord(ProjectKeyword);
                if (prj == null)
                {
                    reJo.msg = "获取目录属性失败，目录不存在！";
                    return reJo.Value;
                }
                #region 读取属性值

                //项目概况页面
                string TBS_PROJNAME = prj.GetValueByKeyWord("TBS_PROJNAME");
                string TBS_CONSTRUCTIONUNIT = prj.GetValueByKeyWord("TBS_CONSTRUCTIONUNIT");
                string TBS_DESIGNUNIT = prj.GetValueByKeyWord("TBS_DESIGNUNIT");
                string TBS_SUPERVISORYUNIT = prj.GetValueByKeyWord("TBS_SUPERVISORYUNIT");
                string TBS_CONSTRUCTIONSITE = prj.GetValueByKeyWord("TBS_CONSTRUCTIONSITE");
                string TBS_ENGINEERINGUSE = prj.GetValueByKeyWord("TBS_ENGINEERINGUSE");
                string TBS_TOTALDURATION = prj.GetValueByKeyWord("TBS_TOTALDURATION");
                string TBS_STARTTIME = prj.GetValueByKeyWord("TBS_STARTTIME");
                string TBS_COMPLETIME = prj.GetValueByKeyWord("TBS_COMPLETIME");

                #endregion
                reJo.data = new JArray(new JObject(
                    //项目概况页面
                    new JProperty("TBS_PROJNAME", TBS_PROJNAME),
                    new JProperty("TBS_CONSTRUCTIONUNIT", TBS_CONSTRUCTIONUNIT),
                    new JProperty("TBS_DESIGNUNIT", TBS_DESIGNUNIT),
                    new JProperty("TBS_SUPERVISORYUNIT", TBS_SUPERVISORYUNIT),
                    new JProperty("TBS_CONSTRUCTIONSITE", TBS_CONSTRUCTIONSITE),
                    new JProperty("TBS_ENGINEERINGUSE", TBS_ENGINEERINGUSE),
                    new JProperty("TBS_TOTALDURATION", TBS_TOTALDURATION),
                    new JProperty("TBS_STARTTIME", TBS_STARTTIME),
                    new JProperty("TBS_COMPLETIME", TBS_COMPLETIME)
                    ));

                reJo.success = true;
                return reJo.Value;
            }
            catch (Exception ex)
            {

            }
            return reJo.Value;
        }

        /// <summary>
        /// 获取临建布置概况默认属性信息
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="ProjectKeyword"></param>
        /// <returns></returns>
        public static JObject GetArrangeOverviewDefault(string sid, string ProjectKeyword)
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
                Project prj = dbsource.GetProjectByKeyWord(ProjectKeyword);
                if (prj == null)
                {
                    reJo.msg = "获取目录属性失败，目录不存在！";
                    return reJo.Value;
                }
                #region 读取属性值

                string TBS_SITESTATUS = prj.GetValueByKeyWord("TBS_SITESTATUS");
                string TBS_PROPOSEDAREA = prj.GetValueByKeyWord("TBS_PROPOSEDAREA");
                string TBS_WORKERLIVEAREA = prj.GetValueByKeyWord("TBS_WORKERLIVEAREA");
                string TBS_MANAGERLIVEAREA = prj.GetValueByKeyWord("TBS_MANAGERLIVEAREA");
                string TBS_WORKAREA = prj.GetValueByKeyWord("TBS_WORKAREA");
                string TBS_OWNEROFFICE = prj.GetValueByKeyWord("TBS_OWNEROFFICE");
                string TBS_OWNERSTACKROOM = prj.GetValueByKeyWord("TBS_OWNERSTACKROOM");
                string TBS_OWNERMEETROOM = prj.GetValueByKeyWord("TBS_OWNERMEETROOM");
                string TBS_OWNEROFFICEAREA = prj.GetValueByKeyWord("TBS_OWNEROFFICEAREA");

                string TBS_SUPERVISORYOFFICE = prj.GetValueByKeyWord("TBS_SUPERVISORYOFFICE");
                string TBS_SUPERVISORYAREA = prj.GetValueByKeyWord("TBS_SUPERVISORYAREA");
                string TBS_CONSTRUCOFFICE = prj.GetValueByKeyWord("TBS_CONSTRUCOFFICE");
                string TBS_CONSTRUCMEETROOM = prj.GetValueByKeyWord("TBS_CONSTRUCMEETROOM");
                string TBS_CONSTRUCAREA = prj.GetValueByKeyWord("TBS_CONSTRUCAREA");
                string TBS_MANAGEDORM = prj.GetValueByKeyWord("TBS_MANAGEDORM");
                string TBS_PEAKWORKERS = prj.GetValueByKeyWord("TBS_PEAKWORKERS");
                string TBS_CAPACITY = prj.GetValueByKeyWord("TBS_CAPACITY");
                string TBS_ROOMCAPACITY = prj.GetValueByKeyWord("TBS_ROOMCAPACITY");
                string TBS_WORKERDORM = prj.GetValueByKeyWord("TBS_WORKERDORM");

                string TBS_PLANSTARTTIME = prj.GetValueByKeyWord("TBS_PLANSTARTTIME");
                string TBS_PLANENDTIME = prj.GetValueByKeyWord("TBS_PLANENDTIME");

                #endregion


                reJo.data = new JArray(new JObject(
                    new JProperty("TBS_SITESTATUS", TBS_SITESTATUS),
                    new JProperty("TBS_PROPOSEDAREA", TBS_PROPOSEDAREA),
                    new JProperty("TBS_WORKERLIVEAREA", TBS_WORKERLIVEAREA),
                    new JProperty("TBS_MANAGERLIVEAREA", TBS_MANAGERLIVEAREA),
                    new JProperty("TBS_WORKAREA", TBS_WORKAREA),
                    new JProperty("TBS_OWNEROFFICE", TBS_OWNEROFFICE),
                    new JProperty("TBS_OWNERSTACKROOM", TBS_OWNERSTACKROOM),
                    new JProperty("TBS_OWNERMEETROOM", TBS_OWNERMEETROOM),
                    new JProperty("TBS_OWNEROFFICEAREA", TBS_OWNEROFFICEAREA),

                    new JProperty("TBS_SUPERVISORYOFFICE", TBS_SUPERVISORYOFFICE),
                    new JProperty("TBS_SUPERVISORYAREA", TBS_SUPERVISORYAREA),
                    new JProperty("TBS_CONSTRUCOFFICE", TBS_CONSTRUCOFFICE),
                    new JProperty("TBS_CONSTRUCMEETROOM", TBS_CONSTRUCMEETROOM),
                    new JProperty("TBS_CONSTRUCAREA", TBS_CONSTRUCAREA),
                    new JProperty("TBS_MANAGEDORM", TBS_MANAGEDORM),
                    new JProperty("TBS_PEAKWORKERS", TBS_PEAKWORKERS),
                    new JProperty("TBS_CAPACITY", TBS_CAPACITY),
                    new JProperty("TBS_ROOMCAPACITY", TBS_ROOMCAPACITY),
                    new JProperty("TBS_WORKERDORM", TBS_WORKERDORM),

                    new JProperty("TBS_PLANSTARTTIME", TBS_PLANSTARTTIME),
                    new JProperty("TBS_PLANENDTIME", TBS_PLANENDTIME)
                    ));

                reJo.success = true;
                return reJo.Value;
            }
            catch (Exception ex)
            {

            }
            return reJo.Value;
        }

        /// <summary>
        /// 根据目录类型，获取项目的各个子目录
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="RootProjectKeyword"></param>
        /// <param name="ProjectType"></param>
        /// <returns></returns>
        public static JObject GetTBSProjectKeyword(string sid, string RootProjectKeyword, string ProjectType)
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

                Project rootPrj = dbsource.GetProjectByKeyWord(RootProjectKeyword);
                if (rootPrj == null)
                {
                    reJo.msg = "获取目录属性失败，目录不存在！";
                    return reJo.Value;
                }
                string prjKeyword = "";
                Project resultPrj;

                if (ProjectType == "TBS_CONSTRUCTIONPLAN" || ProjectType == "TBS_WORKORGPLAN" || ProjectType == "TBS_EQUIPMENTPLAN")
                {
                    Project ppPrj = rootPrj.ChildProjectList.Find(p => p.TempDefn.KeyWord == "TBS_PROJECTPLAN");
                    if (ppPrj == null)
                    {
                        reJo.msg = "获取目录属性失败，计划目录不存在！";
                        return reJo.Value;
                    }

                    resultPrj = ppPrj.ChildProjectList.Find(p => p.TempDefn.KeyWord == ProjectType);
                    if (resultPrj == null)
                    {
                        reJo.msg = "获取目录属性失败，目录不存在！";
                        return reJo.Value;
                    }
                }
                else if (ProjectType == "TBS_PROJECTFAMILYLIB" || ProjectType == "TBS_LAYOUTPLAN" || ProjectType == "TBS_CONSTRUPROPOSAL"
                    || ProjectType == "TBS_REVITMODEL" || ProjectType == "TBS_RENDERING" || ProjectType == "TBS_CALCULATIONLIST")
                {
                    resultPrj = rootPrj.ChildProjectList.Find(p => p.TempDefn.KeyWord == ProjectType);
                    if (resultPrj == null)
                    {
                        reJo.msg = "获取目录属性失败，目录不存在！";
                        return reJo.Value;
                    }
                }
                else
                {
                    reJo.msg = "获取目录属性失败，目录类型不正确！";
                    return reJo.Value;
                }
                string resultPrjKeyword = resultPrj.KeyWord;
                reJo.data = new JArray(new JObject(
                    new JProperty("ProjectKeyword", resultPrjKeyword)
                    ));

                reJo.success = true;
                return reJo.Value;
            }
            catch (Exception ex)
            {

            }
            return reJo.Value;
        }


        public static JObject GetPlanDocList(string sid, string ProjectKeyword) { 
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

                Project Prj = dbsource.GetProjectByKeyWord(ProjectKeyword);
                if (Prj == null)
                {
                    reJo.msg = "获取目录属性失败，目录不存在！";
                    return reJo.Value;
                }

                string docTempKey = "";
                //施工计划
                if (Prj.TempDefn.KeyWord == "TBS_CONSTRUCTIONPLAN")
                {
                    docTempKey = "CONSTRUCTIONPLAN";
                }
                //劳动力组织计划
                else if (Prj.TempDefn.KeyWord == "TBS_WORKORGPLAN")
                {
                    docTempKey = "WORKORGPLAN";
                }
                //机械设备计划
                else if (Prj.TempDefn.KeyWord == "TBS_EQUIPMENTPLAN")
                {
                    docTempKey = "EQUIPMENTPLAN";
                }

                JArray jaData=new JArray();

                foreach(Doc docItem in Prj.DocList){
                    string docTmpAttr = CommonFunction.GetAttrDataValue(docItem, "TBS_PLANTYPE");
                    if (docTmpAttr == docTempKey)
                    {
                        jaData.Add(new JObject(
                            new JProperty("DocKeyword", docItem.KeyWord),
                            new JProperty("Title", docItem.ToString)
                            ));
                    }
                }

                reJo.data = jaData;

                reJo.success = true;
                return reJo.Value;
            }
            catch (Exception ex)
            {

            }
            return reJo.Value;
        }
    }
}
