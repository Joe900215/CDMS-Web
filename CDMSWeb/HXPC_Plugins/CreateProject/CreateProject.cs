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
    public class CreateProject
    {
        /// <summary>
        /// 创建项目时，获取表单默认值，填充到combo
        /// </summary>
        /// <param name="sid">链接密钥</param>
        /// <returns></returns>
        public static JObject GetCreateRootProjectDefault(string sid)
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

                //测试代码，必须删除掉
                Doc doc = dbsource.GetDocByKeyWord("GJEPCMSP3434D589");
                Doc refDoc = dbsource.GetDocByKeyWord("GJEPCMSP702D12");

                if (doc != null && refDoc != null)
                {
                    if (!(doc.RefDocList.Contains(refDoc)))
                    {
                        doc.AddRefDoc(refDoc);
                        //doc.RefDocList.Add(refDoc);
                        doc.Modify();
                    }
                }
                //测试代码结束////////////////////

                int listIndex = 0;
                //设计阶段
                JObject joDesignPhase = new JObject();
                List<DictData> dictDataList = dbsource.GetDictDataList("DesignPhase");
                foreach (DictData d in dictDataList)
                {
                    //this.designPhase.Items.Add(d.O_Code + "__" + d.O_Desc);
                    listIndex = listIndex + 1;
                    joDesignPhase.Add(new JProperty("v" + listIndex.ToString(), d.O_Code + "__" + d.O_Desc));
                }

                //合同模式
                JObject joContractModel = new JObject();
                listIndex = 0;
                dictDataList = dbsource.GetDictDataList("ProjectContractModel");
                foreach (DictData d in dictDataList)
                {
                    //this.ContractModel.Items.Add(d.O_Code);
                    listIndex = listIndex + 1;
                    joContractModel.Add(new JProperty("v" + listIndex.ToString(), d.O_Code));
                }

                //开发阶段
                JObject joDevelopmentPhase = new JObject();
                listIndex = 0;
                dictDataList = dbsource.GetDictDataList("ProjectDevelopmentPhase");
                foreach (DictData d in dictDataList)
                {
                    //this.DevelopmentPhase.Items.Add(d.O_Code);
                    listIndex = listIndex + 1;
                    joDevelopmentPhase.Add(new JProperty("v" + listIndex.ToString(), d.O_Code));
                }

                //工程实施性质
                JObject joEngineeringProperties = new JObject();
                listIndex = 0;
                dictDataList = dbsource.GetDictDataList("EngineeringProperties");
                foreach (DictData d in dictDataList)
                {
                    //this.DesignDevelopment.Items.Add(d.O_Code);
                    listIndex = listIndex + 1;
                    joEngineeringProperties.Add(new JProperty("v" + listIndex.ToString(), d.O_Code));
                }

                //项目类型的选择
                JObject joProjectType = new JObject();
                listIndex = 0;
                dictDataList = dbsource.GetDictDataList("ProjectType");
                string key = "";
                foreach (DictData d in dictDataList)
                {

                    if (key.Contains(d.O_Code))
                    {
                        continue;
                    }

                    //this.PROJECTTYPE.Items.Add(d.O_Code);
                    listIndex = listIndex + 1;
                    joProjectType.Add(new JProperty("v" + listIndex.ToString(), d.O_Code));

                    key = d.O_Code;
                }

                reJo.data = new JArray(
                    new JObject(
                        new JProperty("DesignPhase", joDesignPhase),
                    new JProperty("ContractModel", joContractModel),
                    new JProperty("DevelopmentPhase", joDevelopmentPhase),
                    new JProperty("EngineeringProperties", joEngineeringProperties),
                    new JProperty("ProjectType", joProjectType)
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
        /// 获取二级项目类型
        /// </summary>
        /// <param name="sid">链接密钥</param>
        /// <param name="ProjectType">一级项目类型</param>
        /// <returns></returns>
        public static JObject GetProjectTypeII(string sid, string ProjectType)
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

                //根据一级项目类型获取二级项目类型
                JObject joData = new JObject();
                List<DictData> dictDataList = dbsource.GetDictDataList("ProjectType");

                int dicIndex = 0;
                foreach (DictData d in dictDataList)
                {
                    if (d.O_Code.Contains(ProjectType))
                    {
                        dicIndex = dicIndex + 1;
                        joData.Add(new JProperty("v" + dicIndex.ToString(), d.O_sValue1));
                    }
                }

                reJo.data = new JArray(joData);
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
        /// 项目立项Web接口
        /// </summary>
        /// <param name="sid">链接密钥</param>
        /// <param name="projectAttrJson">参数字符串，详见下面例子</param>
        /// <returns>
        /// <para>projectAttrJson例子： </para>
        /// <code>
        ///[
        ///    { name: 'projectCode', value: projectCode }, //项目代码
        ///    { name: 'projectDesc', value: projectDesc }, //项目名称
        ///    { name: 'unintName', value: unintName }, //建设单位
        ///    { name: 'PROJECTNOMBER', value: PROJECTNOMBER }, //合同号
        ///    { name: 'unitMan', value: unitMan }, //甲方联系人
        ///    { name: 'projectAmount', value: projectAmount }, //合同额
        ///    { name: 'unitManPhone', value: unitManPhone },   //甲方联系人电话
        ///    { name: 'buildArea', value: buildArea },  //建筑面积
        ///    { name: 'writeMan', value: writeMan },   //立项人
        ///    { name: 'contractTerms', value: contractTerms }, //合同主要条款
        ///    { name: 'designPhase', value: designPhase }, //设计阶段
        ///    { name: 'projectType', value: projectType }, //项目类型
        ///    { name: 'quality', value: quality }, //质量目标
        ///    { name: 'releaseDate', value: releaseDate }, //任务下达时间
        ///    { name: 'planStartDate', value: planStartDate }, //计划开始时间
        ///    { name: 'planEndDate', value: planEndDate }, //计划结束时间
        ///    { name: 'projectTypeR ', value: projectTypeR },  //项目类型
        ///    { name: 'projectSize', value: projectSize }, //项目大小
        ///    { name: 'Remarks', value: Remarks },     //备注
        ///    //下面的字段，由于版本的变更，暂时未能保存，可以先把字段传过来
        ///    { name: 'designBasis', value: designBasis },   //设计原则         
        ///    { name: 'TEXTEDITING', value: TEXTEDITING },   //文本编辑规定
        ///    { name: 'HUMAN', value: HUMAN },   //人力资源
        ///    { name: 'CONSTRUCTIONNATURE', value: CONSTRUCTIONNATURE },   //工程实施性质
        ///    { name: 'contractModel', value: contractModel }, //合同模式
        ///    { name: 'DESIGNPRINCIPLES', value: DESIGNPRINCIPLES },   //主要设计原则
        ///    { name: 'requirement', value: requirement },  //项目概况
        ///    { name: 'projectSummary', value: projectSummary },   //工程总结
        ///    { name: 'PROJECTTYPES', value: PROJECTTYPES },   //项目类型补充说明
        ///    { name: 'QUALITYTARGET', value: QUALITYTARGET }, //质量目标，也就是上面的quality字段
        ///    { name: 'developmentPhase', value: developmentPhase }    //项目开发阶段
        ///];
        /// </code>
        /// </returns>
        public static JObject CreateRootProject(string sid, string projectAttrJson)
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
                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(projectAttrJson);

                //reJo.Value = AVEVA.CDMS.HXPC_Common.EnterPoint.CreateRootProjectX(dbsource, jaAttr);
                reJo = CreateRootProjectX(dbsource, jaAttr, sid);


                //刷新数据源
                if (reJo.success == true || reJo.msg == "创建/获取设计阶段失败，请联系管理员！")
                {
                    DBSourceController.RefreshDBSource(sid);
                }

            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(reJo.msg);
            }

            return reJo.Value;
        }


        //项目立项
        public static ExReJObject CreateRootProjectX(DBSource dbsource, JArray jaAttr, string sid)
        {
            ExReJObject reJo = new ExReJObject();

            try
            {

                #region 获取项目参数项目

                //JArray jaAttr = (JArray)JsonConvert.DeserializeObject(projectAttrJson);

                //获取项目参数项目
                string projectCode = "", projectDesc = "", unintName = "", //projectNo = "",
                    unitMan = "", projectAmount = "", unitManPhone = "", buildArea = "",
                    writeMan = "", contractTerms = "", designPhase = "", projectType = "",
                    quality = "", releaseDate = "", planStartDate = "", planEndDate = "",
                    projectTypeR = "", projectSize = "";

                //合同号
                string projectNomber = "";
                //备注
                string Remarks = "";

                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"] == null ? "" : joAttr["name"].ToString();
                    string strValue = joAttr["value"] == null ? "" : joAttr["value"].ToString();

                    //获取项目代码
                    if (strName == "projectCode") projectCode = strValue.Trim();

                    //获取项目名称
                    else if (strName == "projectDesc") projectDesc = strValue;

                    //获取建设单位
                    else if (strName == "unintName") unintName = strValue;

                    //获取合同号
                    else if (strName == "projectNomber") projectNomber = strValue;

                    //获取甲方联系人
                    else if (strName == "unitMan") unitMan = strValue;

                    //获取合同额
                    else if (strName == "projectAmount") projectAmount = strValue;

                    //获取甲方联系人电话
                    else if (strName == "unitManPhone") unitManPhone = strValue;

                    //获取建筑面积
                    else if (strName == "buildArea") buildArea = strValue;

                    //获取立项人
                    else if (strName == "writeMan") writeMan = strValue;

                    //获取合同主要条款
                    else if (strName == "contractTerms") contractTerms = strValue;

                    //获取设计阶段
                    else if (strName == "designPhase") designPhase = strValue;

                    //获取项目类型
                    else if (strName == "projectType") projectType = strValue;

                    //获取质量目标
                    else if (strName == "quality") quality = strValue;

                    //获取任务下达时间
                    else if (strName == "releaseDate") releaseDate = strValue;

                    //获取计划开始时间
                    else if (strName == "planStartDate") planStartDate = strValue;

                    //获取计划结束时间
                    else if (strName == "planEndDate") planEndDate = strValue;

                    //获取项目类型
                    else if (strName == "projectTypeR") projectTypeR = strValue;

                    //获取项目大小
                    else if (strName == "projectSize") projectSize = strValue;

                    //备注
                    else if (strName == "Remarks") Remarks = strValue;

                }

                if (string.IsNullOrEmpty(projectCode))
                {
                    reJo.msg = "请填写项目编号！";
                    return reJo;
                }
                else if (string.IsNullOrEmpty(projectDesc))
                {
                    reJo.msg = "请填写项目名称！";
                    return reJo;
                }
                //else if (string.IsNullOrEmpty(unintName))
                //{
                //    reJo.msg = "请填写建设单位！";
                //    return reJo;
                //}
                else if (string.IsNullOrEmpty(designPhase))
                {
                    reJo.msg = "请选择设计阶段！";
                    return reJo;
                }

                #endregion


                //  根据名称查找项目模板(根目录)对象
                List<TempDefn> tempDefnByCode = dbsource.GetTempDefnByCode("DESIGNPROJECT");
                TempDefn mTempDefn = (tempDefnByCode != null) ? tempDefnByCode[0] : null;
                if (mTempDefn == null)
                {
                    reJo.msg = "欲创建的项目关联的模板不存在！不能完成创建";
                    return reJo;
                }
                else
                {

                    //获取DBSource的虚拟Local目录
                    Project m_Project = dbsource.NewProject(enProjectType.Local);

                    //查找项目是否已经创建
                    Project findProj=dbsource.RootLocalProjectList.Find(itemProj=>itemProj.Code== projectCode);
                    if (findProj != null )
                    {
                        reJo.msg = "项目[" + projectCode + "]已存在！不能完成创建";
                        return reJo;
                    }

                    //foreach (Project findProj in m_Project.AllProjectList)//.ChildProjectList)
                    //{
                    //    if (findProj != null && findProj.Code == projectCode)
                    //    {
                    //        reJo.msg = "项目[" + projectCode + "]已存在！不能完成创建";
                    //        return reJo;
                    //    }
                    //}

                    //创建项目
                    Project m_NewProject = m_Project.NewProject(projectCode, projectDesc, null, mTempDefn);


                    if (m_NewProject == null)
                    {
                        reJo.msg = "新建项目失败";
                        return reJo;

                    }
                    else
                    {

                        //foreach (KeyValuePair<string, string> kvp in context)
                        //{
                        //    SetAttrData(pro, kvp.Key.ToUpper(), kvp.Value, "");

                        //}
                        //pro.GetAttrDataByKeyWord("unintName").SetCodeDesc(unintName);

                        foreach (JObject joAttr in jaAttr)
                        {
                            string strName = joAttr["name"]==null?"":joAttr["name"].ToString();
                            string strValue = joAttr["value"] == null ? "" : joAttr["value"].ToString();

                            AttrData data;
                            if ((data = m_NewProject.GetAttrDataByKeyWord(strName)) != null)
                            {
                                data.SetCodeDesc(strValue);
                            }

                        }

                        ///存进数据库
                        m_NewProject.AttrDataList.SaveData();

                        //创建设计阶段
                        AVEVA.CDMS.Server.Project project = null;

                        List<TempDefn> tempDefnByKeyWord = m_NewProject.dBSource.GetTempDefnByKeyWord("DESIGNPHASE");
                        TempDefn defn2 = ((tempDefnByKeyWord != null) && (tempDefnByKeyWord.Count > 0)) ? tempDefnByKeyWord[0] : null;
                        string str = designPhase;
                        project = m_NewProject.GetProjectByName(str.Substring(0, str.IndexOf("__")));
                        if (project == null)
                        {
                            project = m_NewProject.NewProject(str.Substring(0, str.IndexOf("__")), str.Substring(str.IndexOf("__") + 2), m_NewProject.Storage, defn2);
                        }
                        if (project == null)
                        {

                            reJo.msg = "创建/获取设计阶段失败，请联系管理员！";
                            return reJo;
                        }



                        //增加领导组
                        AVEVA.CDMS.Server.Group gp = dbsource.GetGroupByName("Manager");
                        if (gp != null)
                        {
                            m_NewProject.groupList[0].AddGroup(gp);
                            m_NewProject.groupList[0].Modify();
                        }

                        reJo.data = new JArray(new JObject(new JProperty("projectKeyword", project.KeyWord)));
                        reJo.success = true;
                        return reJo;

                        //AVEVA.CDMS.WebApi.DBSourceController.RefreshDBSource(sid);

                    }
                }
            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(reJo.msg);
            }

            return reJo;
        }


        //项目立项备份
        public static ExReJObject CreateRootProjectX_bak(DBSource dbsource, JArray jaAttr)
        {
            ExReJObject reJo = new ExReJObject();

            try
            {

                #region 获取项目参数项目

                //JArray jaAttr = (JArray)JsonConvert.DeserializeObject(projectAttrJson);

                //获取项目参数项目
                string projectCode = "", projectDesc = "", unintName = "", projectNo = "",
                    unitMan = "", projectAmount = "", unitManPhone = "", buildArea = "",
                    writeMan = "", contractTerms = "", designPhase = "", projectType = "",
                    quality = "", releaseDate = "", planStartDate = "", planEndDate = "",
                    projectTypeR = "", projectSize = "";

                //合同号
                string projectNomber = "";
                //备注
                string Remarks = "";

                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();

                    //获取项目代码
                    if (strName == "projectCode") projectCode = strValue;

                    //获取项目名称
                    else if (strName == "projectDesc") projectDesc = strValue;

                    //获取建设单位
                    else if (strName == "unintName") unintName = strValue;

                    //获取合同号
                    else if (strName == "projectNo") projectNo = strValue;

                    //获取甲方联系人
                    else if (strName == "unitMan") unitMan = strValue;

                    //获取合同额
                    else if (strName == "projectAmount") projectAmount = strValue;

                    //获取甲方联系人电话
                    else if (strName == "unitManPhone") unitManPhone = strValue;

                    //获取建筑面积
                    else if (strName == "buildArea") buildArea = strValue;

                    //获取立项人
                    else if (strName == "writeMan") writeMan = strValue;

                    //获取合同主要条款
                    else if (strName == "contractTerms") contractTerms = strValue;

                    //获取设计阶段
                    else if (strName == "designPhase") designPhase = strValue;

                    //获取项目类型
                    else if (strName == "projectType") projectType = strValue;

                    //获取质量目标
                    else if (strName == "quality") quality = strValue;

                    //获取任务下达时间
                    else if (strName == "releaseDate") releaseDate = strValue;

                    //获取计划开始时间
                    else if (strName == "planStartDate") planStartDate = strValue;

                    //获取计划结束时间
                    else if (strName == "planEndDate") planEndDate = strValue;

                    //获取项目类型
                    else if (strName == "projectTypeR") projectTypeR = strValue;

                    //获取项目大小
                    else if (strName == "projectSize") projectSize = strValue;

                }

                if (string.IsNullOrEmpty(projectCode))
                {
                    reJo.msg = "请填写项目编号！";
                    return reJo;
                }
                else if (string.IsNullOrEmpty(projectDesc))
                {
                    reJo.msg = "请填写项目名称！";
                    return reJo;
                }
                else if (string.IsNullOrEmpty(unintName))
                {
                    reJo.msg = "请填写建设单位！";
                    return reJo;
                }
                else if (string.IsNullOrEmpty(designPhase))
                {
                    reJo.msg = "请选择设计阶段！";
                    return reJo;
                }

                #endregion


                //  根据名称查找项目模板(根目录)对象
                List<TempDefn> tempDefnByCode = dbsource.GetTempDefnByCode("GEDI_INTERFACEPROJECT");
                TempDefn mTempDefn = (tempDefnByCode != null) ? tempDefnByCode[0] : null;
                if (mTempDefn == null)
                {
                    reJo.msg = "欲创建的项目关联的模板不存在！不能完成创建";
                    return reJo;
                }
                else
                {

                    //获取DBSource的虚拟Local目录
                    Project m_Project = dbsource.NewProject(enProjectType.Local);

                    //创建项目
                    Project m_NewProject = m_Project.NewProject(projectCode, projectDesc, null, mTempDefn);


                    if (m_NewProject == null)
                    {
                        reJo.msg = "新建项目失败";
                        return reJo;

                    }
                    else
                    {

                        //foreach (KeyValuePair<string, string> kvp in context)
                        //{
                        //    SetAttrData(pro, kvp.Key.ToUpper(), kvp.Value, "");

                        //}
                        //pro.GetAttrDataByKeyWord("unintName").SetCodeDesc(unintName);

                        foreach (JObject joAttr in jaAttr)
                        {
                            string strName = joAttr["name"].ToString();
                            string strValue = joAttr["value"].ToString();

                            AttrData data;
                            if ((data = m_NewProject.GetAttrDataByKeyWord(strName)) != null)
                            {
                                data.SetCodeDesc(strValue);
                            }

                        }

                        ///存进数据库
                        m_NewProject.AttrDataList.SaveData();

                        AVEVA.CDMS.Server.Project project = null;

                        List<TempDefn> tempDefnByKeyWord = m_NewProject.dBSource.GetTempDefnByKeyWord("GEDIHD_DESIGNPHASE");
                        TempDefn defn2 = ((tempDefnByKeyWord != null) && (tempDefnByKeyWord.Count > 0)) ? tempDefnByKeyWord[0] : null;
                        string str = designPhase;
                        project = m_NewProject.GetProjectByName(str.Substring(0, str.IndexOf("__")));
                        if (project == null)
                        {
                            project = m_NewProject.NewProject(str.Substring(0, str.IndexOf("__")), str.Substring(str.IndexOf("__") + 2), m_NewProject.Storage, defn2);
                        }
                        if (project == null)
                        {

                            reJo.msg = "创建/获取设计阶段失败，请联系管理员！";
                            return reJo;
                        }
                        else
                        {

                            //DBSourceController.RefreshDBSource(sid);
                            reJo.data = new JArray(new JObject(new JProperty("projectKeyword", project.KeyWord)));
                            reJo.success = true;
                            return reJo;

                        }
                        //AVEVA.CDMS.WebApi.DBSourceController.RefreshDBSource(sid);
                    }

                }
            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(reJo.msg);
            }

            return reJo;
        }
    }
}
