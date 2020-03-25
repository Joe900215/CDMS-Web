
namespace AVEVA.CDMS.HXPC_Common
{

    using System;
    using System.Collections.Generic;
    using AVEVA.CDMS.Server;
    using AVEVA.CDMS.Common;
    //using Newtonsoft.Json.Linq;


    public static class EnterPoint
    {

        public static string pluginType = "CS";

        //项目立项
        //public static JObject CreateRootProjectX(DBSource dbsource, JArray jaAttr)
        //{
        //    ExReJO reJo = new ExReJO();

        //    try
        //    {

        //        #region 获取项目参数项目

        //        //JArray jaAttr = (JArray)JsonConvert.DeserializeObject(projectAttrJson);

        //        //获取项目参数项目
        //        string projectCode = "", projectDesc = "", unintName = "", projectNo = "",
        //            unitMan = "", projectAmount = "", unitManPhone = "", buildArea = "",
        //            writeMan = "", contractTerms = "", designPhase = "", projectType = "",
        //            quality = "", releaseDate = "", planStartDate = "", planEndDate = "",
        //            projectTypeR = "", projectSize = "";


        //        int jaLen=jaAttr.Count;
                
        //        //foreach (JObject joAttr in jaAttr)
        //        for (int i=0;i<jaLen;i++)
        //        {
        //            JObject joAttr = (JObject)jaAttr[i];
        //            string strName = joAttr["name"].ToString();
        //            string strValue = joAttr["value"].ToString();

        //            //获取项目代码
        //            if (strName == "projectCode") projectCode = strValue;

        //            //获取项目名称
        //            else if (strName == "projectDesc") projectDesc = strValue;

        //            //获取建设单位
        //            else if (strName == "unintName") unintName = strValue;

        //            //获取合同号
        //            else if (strName == "projectNo") projectNo = strValue;

        //            //获取甲方联系人
        //            else if (strName == "unitMan") unitMan = strValue;

        //            //获取合同额
        //            else if (strName == "projectAmount") projectAmount = strValue;

        //            //获取甲方联系人电话
        //            else if (strName == "unitManPhone") unitManPhone = strValue;

        //            //获取建筑面积
        //            else if (strName == "buildArea") buildArea = strValue;

        //            //获取立项人
        //            else if (strName == "writeMan") writeMan = strValue;

        //            //获取合同主要条款
        //            else if (strName == "contractTerms") contractTerms = strValue;

        //            //获取设计阶段
        //            else if (strName == "designPhase") designPhase = strValue;

        //            //获取项目类型
        //            else if (strName == "projectType") projectType = strValue;

        //            //获取质量目标
        //            else if (strName == "quality") quality = strValue;

        //            //获取任务下达时间
        //            else if (strName == "releaseDate") releaseDate = strValue;

        //            //获取计划开始时间
        //            else if (strName == "planStartDate") planStartDate = strValue;

        //            //获取计划结束时间
        //            else if (strName == "planEndDate") planEndDate = strValue;

        //            //获取项目类型
        //            else if (strName == "projectTypeR") projectTypeR = strValue;

        //            //获取项目大小
        //            else if (strName == "projectSize") projectSize = strValue;

        //        }

        //        if (string.IsNullOrEmpty(projectCode))
        //        {
        //            reJo.msg = "请填写项目编号！";
        //            return reJo.Value;
        //        }
        //        else if (string.IsNullOrEmpty(projectDesc))
        //        {
        //            reJo.msg = "请填写项目名称！";
        //            return reJo.Value;
        //        }
        //        else if (string.IsNullOrEmpty(unintName))
        //        {
        //            reJo.msg = "请填写建设单位！";
        //            return reJo.Value;
        //        }
        //        else if (string.IsNullOrEmpty(designPhase))
        //        {
        //            reJo.msg = "请选择设计阶段！";
        //            return reJo.Value;
        //        }

        //        #endregion


        //        //  根据名称查找项目模板(根目录)对象
        //        List<TempDefn> tempDefnByCode = dbsource.GetTempDefnByCode("GEDI_INTERFACEPROJECT");
        //        TempDefn mTempDefn = (tempDefnByCode != null) ? tempDefnByCode[0] : null;
        //        if (mTempDefn == null)
        //        {
        //            reJo.msg = "欲创建的项目关联的模板不存在！不能完成创建";
        //            return reJo.Value;
        //        }
        //        else
        //        {

        //            //获取DBSource的虚拟Local目录
        //            Project m_Project = dbsource.NewProject(enProjectType.Local);

        //            //创建项目
        //            Project m_NewProject = m_Project.NewProject(projectCode, projectDesc, null, mTempDefn);


        //            if (m_NewProject == null)
        //            {
        //                reJo.msg = "新建项目失败";
        //                return reJo.Value;

        //            }
        //            else
        //            {

        //                //foreach (KeyValuePair<string, string> kvp in context)
        //                //{
        //                //    SetAttrData(pro, kvp.Key.ToUpper(), kvp.Value, "");

        //                //}
        //                //pro.GetAttrDataByKeyWord("unintName").SetCodeDesc(unintName);

        //                int jaLen2 = jaAttr.Count;

        //                //foreach (JObject joAttr in jaAttr)
        //                for (int i = 0; i < jaLen2; i++)
        //                {
        //                    JObject joAttr = (JObject)jaAttr[i];
        //                    string strName = joAttr["name"].ToString();
        //                    string strValue = joAttr["value"].ToString();

        //                    AttrData data;
        //                    if ((data = m_NewProject.GetAttrDataByKeyWord(strName)) != null)
        //                    {
        //                        data.SetCodeDesc(strValue);
        //                    }

        //                }

        //                ////存进数据库
        //                m_NewProject.AttrDataList.SaveData();

        //                AVEVA.CDMS.Server.Project project = null;

        //                List<TempDefn> tempDefnByKeyWord = m_NewProject.dBSource.GetTempDefnByKeyWord("GEDIHD_DESIGNPHASE");
        //                TempDefn defn2 = ((tempDefnByKeyWord != null) && (tempDefnByKeyWord.Count > 0)) ? tempDefnByKeyWord[0] : null;
        //                string str = designPhase;
        //                project = m_NewProject.GetProjectByName(str.Substring(0, str.IndexOf("__")));
        //                if (project == null)
        //                {
        //                    project = m_NewProject.NewProject(str.Substring(0, str.IndexOf("__")), str.Substring(str.IndexOf("__") + 2), m_NewProject.Storage, defn2);
        //                }
        //                if (project == null)
        //                {

        //                    reJo.msg = "创建/获取设计阶段失败，请联系管理员！";
        //                    return reJo.Value;
        //                }
        //                else
        //                {

        //                    //DBSourceController.RefreshDBSource(sid);
        //                    reJo.data = new JArray(new JObject(new JProperty("projectKeyword", project.KeyWord)));
        //                    reJo.success = true;
        //                    return reJo.Value;

        //                }
        //                //AVEVA.CDMS.WebApi.DBSourceController.RefreshDBSource(sid);
        //            }

        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        reJo.msg = e.Message;
        //        //CommonController.WebWriteLog(reJo.msg);
        //    }

        //    return reJo.Value;
        //}


    }
}

    




