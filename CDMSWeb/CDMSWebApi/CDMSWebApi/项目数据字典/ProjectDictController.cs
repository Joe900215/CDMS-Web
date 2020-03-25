using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AVEVA.CDMS.Server;
using AVEVA.CDMS.Common;
using AVEVA.CDMS.WebApi;

namespace AVEVA.CDMS.WebApi
{

    public class ProjectDictController : Controller
    {
        #region 项目数据字典数据相关
        /// <summary>
        /// 新建数据字典数据时，获取默认值
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public static JObject GetEditDictDataDefault(string sid, string DictDataType, string ProjectKeyword,
            string RootProjectTempDefnKeyword)
        {
            ExReJObject reJo = new ExReJObject();

            try
            {
                //string RootProjectTempDefnKeyword = "HXNY_DOCUMENTSYSTEM";

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

                Project m_prj = dbsource.GetProjectByKeyWord(ProjectKeyword);
                if (m_prj == null)
                {
                    reJo.msg = "参数错误，目录不存在！";
                    return reJo.Value;
                }

                Project prjProject = GetParentProjectByTempDefn(m_prj, RootProjectTempDefnKeyword);

                if (prjProject == null)
                {
                    reJo.msg = "获取项目目录失败！";
                    return reJo.Value;
                }

                //获取项目代码
                int ProjId = prjProject.ID;
                string strProjCode = prjProject.Code;//.GetAttrDataByKeyWord("COMPANY").ToString;
                string strProjDesc = prjProject.Description;

                JArray jaDictData = new JArray();
                JObject joDictData = new JObject();

                List<DictData> dictDataList = dbsource.GetDictDataList(DictDataType); //"DictData");
                //[o_Code]:公司编码,[o_Desc]：公司描述,[o_sValue1]：项目代码

                foreach (DictData data6 in dictDataList)
                {
                    //if (!string.IsNullOrEmpty(data6.O_sValue2) && data6.O_sValue2 == strProjCode)
                    if (data6.O_iValue1 != null && data6.O_iValue1 == ProjId )
                    {
                        joDictData = new JObject(
                            new JProperty("dictDataId", data6.O_ID.ToString()),
                            new JProperty("dictDataCode", data6.O_Code),
                            new JProperty("dictDataDesc", data6.O_Desc),
                            new JProperty("O_sValue1", data6.O_sValue1),
                            new JProperty("O_sValue2", data6.O_sValue2),
                            new JProperty("O_sValue3", data6.O_sValue3),
                            new JProperty("O_sValue4", data6.O_sValue4),
                            new JProperty("O_sValue5", data6.O_sValue5)
                            );
                        jaDictData.Add(joDictData);
                    }
                }


                reJo.data = new JArray(
                    new JObject(new JProperty("projectCode", strProjCode),
                    new JProperty("projectDesc", strProjDesc),
                    new JProperty("dictDataList", jaDictData)));
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
        /// 新建数据字典数据
        /// </summary>
        public static JObject CreateDictData(string sid, string DictDataType, string ProjectKeyword,
            string RootProjectTempDefnKeyword, string dictDataAttrJson)
        {
            ExReJObject reJo = new ExReJObject();

            try
            {
                //string RootProjectTempDefnKeyword = "HXNY_DOCUMENTSYSTEM";
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

                Project m_prj = dbsource.GetProjectByKeyWord(ProjectKeyword);
                if (m_prj == null)
                {
                    reJo.msg = "参数错误，目录不存在！";
                    return reJo.Value;
                }


                #region 获取传递过来的属性参数
                //获取传递过来的属性参数
                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(dictDataAttrJson);

                string strDictDataCode = "", strDictDataDesc = "",
                    //strDictDataEngDesc = "", strDictDataChinese = "",
                    //strAddress = "", strProvince = "",
                    //strPostCode = "", strEMail = "",
                    //strReceiver = "", strFaxNo = "", strPhone = "",
                    strO_sValue1 = "", strO_sValue2 = "", strO_sValue3 = "", 
                    strO_sValue4 = "", strO_sValue5 = "";

                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();

                    switch (strName)
                    {
                        case "dictDataCode":
                            strDictDataCode = strValue;
                            break;
                        case "dictDataDesc":
                            strDictDataDesc = strValue;
                            break;
                        //case "dictDataEngDesc":
                        //    strDictDataEngDesc = strValue;
                            //break;
                        case "o_sValue1":
                            strO_sValue1 = strValue;
                            break;
                        case "o_sValue2":
                            strO_sValue2 = strValue;
                            break;
                        case "o_sValue3":
                            strO_sValue3 = strValue;
                            break;
                        case "o_sValue4":
                            strO_sValue4 = strValue;
                            break;
                        case "o_sValue5":
                            strO_sValue5 = strValue;
                            break;
                    }
                }

                if (string.IsNullOrEmpty(strDictDataCode))
                {
                    reJo.msg = "请输入项目编号！";
                    return reJo.Value;
                }
                if (string.IsNullOrEmpty(strDictDataDesc))
                {
                    reJo.msg = "请输入项目名称！";
                    return reJo.Value;
                }

                #endregion

                Project prjProject = GetParentProjectByTempDefn(m_prj, RootProjectTempDefnKeyword);

                if (prjProject == null)
                {
                    reJo.msg = "获取项目目录失败！";
                    return reJo.Value;
                }

                //获取项目代码
                int ProjId=prjProject.ID;
                //string strProjCode = prjProject.Code;//.GetAttrDataByKeyWord("COMPANY").ToString;

                JArray jaData = new JArray();
                JObject joDictData = new JObject();

                List<DictData> dictDataList = dbsource.GetDictDataList(DictDataType);//"DictData");
                //[o_Code]:公司编码,[o_Desc]：公司描述,[o_sValue1]：项目代码

                foreach (DictData data6 in dictDataList)
                {
                    //if (!string.IsNullOrEmpty(data6.O_sValue2) && data6.O_sValue2 == strProjCode && data6.O_Code == strDictDataCode)
                     if ((data6.O_iValue1!=null) && data6.O_iValue1 == ProjId && data6.O_Code == strDictDataCode)
                    {
                        reJo.msg = "已经存在相同的参建单位，请返回重试！";
                        return reJo.Value;
                    }
                }
                //dbsource.NewDictData
                #region 添加到数据字典
                //添加到数据字典

                string format = "insert CDMS_DictData (" +
                    "o_parentno,o_datatype,o_ikey,o_skey,o_Code,o_Desc,o_sValue1,o_sValue2,o_sValue3,o_sValue4,o_sValue5,o_iValue1 ,o_iValue2)" +
                    " values ({0},{1},{2},'{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}',{11},{12}" + ")";
                //0,2,0,'Unit','"+strDictDataCode+"','"+strDictDataDesc+"','"+strProjCode+ "','','','','',0,0
                format = string.Format(format, new object[] {
                    0,2,0,"DictData",strDictDataCode,strDictDataDesc,strO_sValue1,strO_sValue2,strO_sValue3,strO_sValue4,strO_sValue5,ProjId,0
                });
                dbsource.DBExecuteSQL(format);

                DBSourceController.refreshDBSource(sid);
                
                #endregion


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
        /// 修改数据字典数据
        /// </summary>
        public static JObject EditDictData(string sid, string DictDataType, string ProjectKeyword,
            string RootProjectTempDefnKeyword, string dictDataAttrJson)
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

                Project m_prj = dbsource.GetProjectByKeyWord(ProjectKeyword);
                if (m_prj == null)
                {
                    reJo.msg = "参数错误，目录不存在！";
                    return reJo.Value;
                }


                #region 获取传递过来的属性参数
                //获取传递过来的属性参数
                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(dictDataAttrJson);

                string strDictDataId = "", strDictDataCode = "", strDictDataDesc = "",
                    //strDictDataEngDesc = "", 
                    strO_sValue1 = "", strO_sValue2 = "", strO_sValue3 = "",
                    strO_sValue4 = "", strO_sValue5 = "";

                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();

                    switch (strName)
                    {
                        case "dictDataId":
                            strDictDataId = strValue;
                            break;
                        case "dictDataCode":
                            strDictDataCode = strValue;
                            break;
                        case "dictDataDesc":
                            strDictDataDesc = strValue;
                            break;
                        case "o_sValue1":
                            strO_sValue1 = strValue;
                            break;
                        case "o_sValue2":
                            strO_sValue2 = strValue;
                            break;
                        case "o_sValue3":
                            strO_sValue3 = strValue;
                            break;
                        case "o_sValue4":
                            strO_sValue4 = strValue;
                            break;
                        case "o_sValue5":
                            strO_sValue5 = strValue;
                            break;
                    }
                }

                if (string.IsNullOrEmpty(strDictDataCode))
                {
                    reJo.msg = "请输入项目编号！";
                    return reJo.Value;
                }
                if (string.IsNullOrEmpty(strDictDataDesc))
                {
                    reJo.msg = "请输入项目名称！";
                    return reJo.Value;
                }
                #endregion

                Project prjProject = GetParentProjectByTempDefn(m_prj, RootProjectTempDefnKeyword);//"HXNY_DOCUMENTSYSTEM");

                if (prjProject == null)
                {
                    reJo.msg = "获取项目目录失败！";
                    return reJo.Value;
                }


                int dictDataId = Convert.ToInt32(strDictDataId);
                //获取项目代码
                //string strProjCode = prjProject.Code;//.GetAttrDataByKeyWord("COMPANY").ToString;
                int ProjId = prjProject.ID;

                JArray jaData = new JArray();
                JObject joDictData = new JObject();

                List<DictData> dictDataList = dbsource.GetDictDataList(DictDataType);//"DictData");
                //[o_Code]:公司编码,[o_Desc]：公司描述,[o_sValue1]：项目代码

                foreach (DictData data6 in dictDataList)
                {
                    //if (!string.IsNullOrEmpty(data6.O_sValue2) && data6.O_sValue2 == strProjCode
                    //    && data6.O_Code == strDictDataCode && data6.O_ID != dictDataId)
                    if ((data6.O_iValue1!=null) && data6.O_iValue1 == ProjId
                        && data6.O_Code == strDictDataCode && data6.O_ID != dictDataId)
                    {
                        reJo.msg = "已经存在相同的参建单位，请返回重试！";
                        return reJo.Value;
                    }
                }

                #region 添加到数据字典

                DictData dictData = null;

                foreach (DictData data6 in dictDataList)
                {
                    if (data6.O_ID == dictDataId)
                    {
                        dictData = data6;

                    }
                }

                if (dictData == null)
                {
                    reJo.msg = "参建单位ID不存在，请返回重试！";
                    return reJo.Value;

                }

                dictData.O_Code = strDictDataCode;
                dictData.O_Desc = strDictDataDesc;
                //dictData.O_sValue2 = strProjCode;
                dictData.O_sValue1 = strO_sValue1;
                dictData.O_sValue2 = strO_sValue2;
                dictData.O_sValue3 = strO_sValue3;
                dictData.O_sValue4 = strO_sValue4;
                dictData.O_sValue5 = strO_sValue5;
                dictData.O_iValue1 = ProjId;
                dictData.Modify();

                DBSourceController.refreshDBSource(sid);

                #endregion

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
        /// 删除数据字典数据
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="ProjectKeyword"></param>
        /// <param name="DictDataCode"></param>
        /// <returns></returns>
        public static JObject DeleteDictData(string sid, string DictDataType, string ProjectKeyword,
            string RootProjectTempDefnKeyword, string DictDataCode, string DictDataId)
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

                Project m_prj = dbsource.GetProjectByKeyWord(ProjectKeyword);
                if (m_prj == null)
                {
                    reJo.msg = "参数错误，目录不存在！";
                    return reJo.Value;
                }

                Project prjProject = GetParentProjectByTempDefn(m_prj, RootProjectTempDefnKeyword);//"HXNY_DOCUMENTSYSTEM");

                if (prjProject == null)
                {
                    reJo.msg = "获取项目目录失败！";
                    return reJo.Value;
                }


                int dictDataId = Convert.ToInt32(DictDataId);
                //获取项目代码
                string strProjCode = prjProject.Code;//.GetAttrDataByKeyWord("COMPANY").ToString;
                int ProjId = prjProject.ID;

                JArray jaData = new JArray();
                JObject joDictData = new JObject();

                List<DictData> dictDataList = dbsource.GetDictDataList(DictDataType);//"DictData");
                //[o_Code]:公司编码,[o_Desc]：公司描述,[o_sValue1]：项目代码

                foreach (DictData data6 in dictDataList)
                {
                    //if (!string.IsNullOrEmpty(data6.O_sValue2) && data6.O_sValue2 == strProjCode
                    //    && data6.O_Code == DictDataCode && data6.O_ID == dictDataId)
                    if ((data6.O_iValue1!=null) && data6.O_iValue1 == ProjId
                        && data6.O_Code == DictDataCode && data6.O_ID == dictDataId)
                    {
                        data6.Delete();
                        break;
                    }
                }

                DBSourceController.refreshDBSource(sid);

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
        /// 获取数据字典数据列表
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="DictDataType"></param>
        /// <param name="ProjectKeyword"></param>
        /// <param name="Page"></param>
        /// <param name="Limit"></param>
        /// <param name="Filter"></param>
        /// <returns></returns>
        public static JObject GetDictDataList(string sid, string DictDataType,
            string ProjectKeyword, string  RootProjectTempDefnKeyword, 
            string Page, string Limit, string Filter)
        {
            ExReJObject reJo = new ExReJObject();

            try
            {
                string page = Page ?? "1";
                string limit = Limit ?? "50";
                //string limit = "50";
                page = (Convert.ToInt32(page) - 1).ToString();
                int CurPage = Convert.ToInt32(page);

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

                Project m_prj = dbsource.GetProjectByKeyWord(ProjectKeyword);
                if (m_prj == null)
                {
                    reJo.msg = "参数错误，目录不存在！";
                    return reJo.Value;
                }

                Project prjProject = GetParentProjectByTempDefn(m_prj, RootProjectTempDefnKeyword);//"HXNY_DOCUMENTSYSTEM");

                if (prjProject == null)
                {
                    reJo.msg = "获取项目目录失败！";
                    return reJo.Value;
                }

                Filter = Filter.Trim().ToLower();

                //string curDictDataCode = "";

                //curDictDataCode = prjProject.Code;//.GetValueByKeyWord("PRO_COMPANY");
                //if (string.IsNullOrEmpty(curDictDataCode))
                //{
                //    reJo.msg = "获取项目来源失败！";
                //    return reJo.Value;
                //}
                int curDictDataProjId = prjProject.ID;
                if (curDictDataProjId == null) {
                    reJo.msg = "获取项目来源失败！";
                    return reJo.Value;                
                }

                JArray jaData = new JArray();

                #region 添加机组
                //获取所有参建单位
                List<DictData> dictDataList = dbsource.GetDictDataList(DictDataType);//"DictData");
                List<DictData> resultDDList = new List<DictData>();
                ////按代码排序
                //dictDataList.Sort(delegate (DictData x, DictData y)
                //{
                //    return x.O_Code.CompareTo(y.O_Code);
                //});


                foreach (DictData data6 in dictDataList)
                {
                    //判断是否符合过滤条件
                    if (!string.IsNullOrEmpty(Filter) &&
                        data6.O_Code.ToLower().IndexOf(Filter) < 0 &&
                        data6.O_Desc.ToLower().IndexOf(Filter) < 0 &&
                        data6.O_sValue1.ToLower().IndexOf(Filter) < 0
                        )
                    {
                        continue;
                    }

                    //if (data6.O_sValue2 != curDictDataCode)
                    if (data6.O_iValue1 != curDictDataProjId)
                    {
                        continue;
                    }
                    resultDDList.Add(data6);
                }

                #endregion


                reJo.total = resultDDList.Count();
                int ShowNum = 50;

                //获取字典列表
                List<DictData> hdds = dbsource.GetHeadDictDataList();
                DictData dd = hdds.Find(d=>d.O_skey==DictDataType);
                string typeDesc = "";
                if (dd != null) {
                    typeDesc = dd.O_Desc;
                }

                List<DictData> resDDList = resultDDList.Skip(CurPage * ShowNum).Take(ShowNum).ToList();

                foreach (DictData data6 in resDDList)
                {
                    {
                        JObject joData = new JObject(
                            new JProperty("dictDataTypeDesc", typeDesc),// "机组"),
                            new JProperty("dictDataId", data6.O_ID.ToString()),
                            new JProperty("dictDataCode", data6.O_Code),
                            new JProperty("dictDataDesc", data6.O_Desc)
                            );
                        jaData.Add(joData);
                    }
                }


                reJo.data = jaData;
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
        /// 根据模板获取父目录
        /// </summary>
        /// <param name="curProj"></param>
        /// <param name="TempDefnKeyWord"></param>
        /// <returns></returns>
        internal static Project GetParentProjectByTempDefn(Project curProj, string TempDefnKeyWord)
        {
            //Project proj = null;

            #region 获取项目名称
            Project proj = curProj;
            Project rootProj = null;
            //string rootProjDesc = "";
            try
            {
                while (true)
                {
                    if (proj.TempDefn != null && proj.TempDefn.KeyWord == TempDefnKeyWord)
                    {
                        rootProj = proj;
                        //rootProjDesc = proj.Description;
                        break;
                    }
                    else
                    {
                        if (proj.ParentProject == null)
                        {
                            break;
                        }
                        else
                        {
                            proj = proj.ParentProject;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                //WebApi.CommonController.WebWriteLog(DateTime.Now.ToString() + ":" + "根据模板获取项目错误," + ex.Message);

            }
            #endregion
            return rootProj;

        }


        #endregion

        /// <summary>
        /// 获取项目数据字典列表
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="DictDataType"></param>
        /// <param name="ProjectKeyword"></param>
        /// <param name="RootProjectTempDefnKeyword"></param>
        /// <returns></returns>
        public static JObject GetProjectDictTableList(string sid, string ProjectKeyword,
            string RootProjectTempDefnKeyword)
        {
            ExReJObject reJo = new ExReJObject();

            try
            {
                //string RootProjectTempDefnKeyword = "HXNY_DOCUMENTSYSTEM";

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

                Project m_prj = dbsource.GetProjectByKeyWord(ProjectKeyword);
                if (m_prj == null)
                {
                    reJo.msg = "参数错误，目录不存在！";
                    return reJo.Value;
                }

                Project prjProject = GetParentProjectByTempDefn(m_prj, RootProjectTempDefnKeyword);

                if (prjProject == null)
                {
                    reJo.msg = "获取项目目录失败！";
                    return reJo.Value;
                }

                //获取项目代码
                string strProjCode = prjProject.Code;
                string strProjDesc = prjProject.Description;

                //获取字典列表
                var hdds = dbsource.GetHeadDictDataList();
                //dbsource.GetDictDataList();
                List<DictData> dictdataList= dbsource.GetHeadDictDataList();
                //foreach (DictData dict in dictdataList)
                //{
                //    var hDictCode = dict.ToString;
                //    //获取字典数据列表
                //    var dds = dbsource.GetDictDataList(hDictCode);
                //}
                
                List<DictData>  reDictDataList = dictdataList.FindAll(dd => dd.O_iValue1 == 1);

                JObject joData = new JObject();
                JArray jaData=new JArray();
                foreach (DictData dd in reDictDataList)
                {
                    jaData.Add(new JObject(
                        new JProperty("Name", dd.O_skey),
                        new JProperty("Desc", dd.O_Desc)));
                }

                reJo.data = jaData;
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
        /// 创建项目数据字典数据表
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="DictDataType"></param>
        /// <param name="ProjectKeyword"></param>
        /// <param name="RootProjectTempDefnKeyword"></param>
        /// <returns></returns>
        public static JObject CreateProjectDictTable(string sid, string TableName, string TableDesc)
            //, string ProjectKeyword,
            //string RootProjectTempDefnKeyword)
        {
            ExReJObject reJo = new ExReJObject();

            try
            {
                //string RootProjectTempDefnKeyword = "HXNY_DOCUMENTSYSTEM";

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

                if (!curUser.IsAdmin) {
                    reJo.msg = "用户没有创建项目数据表权限！";
                    return reJo.Value;
                }
                //Project m_prj = dbsource.GetProjectByKeyWord(ProjectKeyword);
                //if (m_prj == null)
                //{
                //    reJo.msg = "参数错误，目录不存在！";
                //    return reJo.Value;
                //}

                //Project prjProject = GetParentProjectByTempDefn(m_prj, RootProjectTempDefnKeyword);

                //if (prjProject == null)
                //{
                //    reJo.msg = "获取项目目录失败！";
                //    return reJo.Value;
                //}

                #region 验证表名
                if (string.IsNullOrEmpty(TableName))
                {
                    reJo.msg = "表名不能为空！";
                    return reJo.Value;
                }

                if (string.IsNullOrEmpty(TableDesc)) {
                    reJo.msg = "表描述不能为空！";
                    return reJo.Value;
                }

                //获取字典列表
                var hdds = dbsource.GetHeadDictDataList();
                foreach (DictData dict in dbsource.GetHeadDictDataList())
                {
                    var hDictCode = dict.ToString;
                    if (hDictCode == TableName) {
                        reJo.msg = "已经存在数据表【" + TableName + "】！";
                        return reJo.Value;
                    }
                } 
                #endregion

                //新建前先验证数据
                //if (PassVaildate(null))
                //{
                    DictData dictdata = new DictData();
                    dictdata.StatusNew = true;
                    dictdata.O_skey = TableName;
                    dictdata.O_datatype = (int)enDictDataType.TableHead;

                    dictdata.O_Code = "Code";
                    dictdata.O_Desc = TableDesc;
                    dictdata.O_sValue1 = "sValue1";
                    dictdata.O_sValue2 = "sValue2";
                    dictdata.O_sValue3 = "sValue3";
                    dictdata.O_sValue4 = "sValue4";
                    dictdata.O_sValue5 = "sValue5";
                    dictdata.O_iValue1 = 1;//1代表是项目数据表


                    //通过以上的dictdata新建一个DictData
                    DictData newDictData = CreateNewDictData(dbsource, dictdata, enDictDataType.TableHead);

                    if (!newDictData.Write())
                    {
                        //MessageBox.Show("创建失败!", "提示", MessageBoxButtons.OK);
                        reJo.msg = "创建失败!";
                        return reJo.Value;
                    }

                    //DBSourceController.refreshDBSource(sid,"1");

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

        public static JObject DeleteProjectDictTable(string sid, string TableName)
        {
            ExReJObject reJo = new ExReJObject();

            try
            {
                //string RootProjectTempDefnKeyword = "HXNY_DOCUMENTSYSTEM";

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

                if (!curUser.IsAdmin) {
                    reJo.msg = "用户没有删除项目数据表权限！";
                    return reJo.Value;
                }

                #region 删除数据字典的数据
                
                //获取字典数据列表
                var dds = dbsource.GetDictDataList(TableName);
                if (dds != null)
                {
                    foreach (DictData dd in dds)
                    {
                        if (dd.Delete())
                        {
                            dds.Remove(dd);
                        }
                    }
                }
                #endregion
                #region 删除数据字典
                //DictData selDictdata = selObject as DictData;
                //List<DictData> dictdataList = dbsource.GetDictDataList(enDictDataType.TableHead);
                List<DictData> dictdataList =  dbsource.GetHeadDictDataList();
                DictData selDictdata = dictdataList.Find(sd => sd.O_skey == TableName);
                if (selDictdata == null)
                {
                    reJo.msg = "数据表【"+TableName+"】不存在！";
                    return reJo.Value;
                }
              
                    //从数据库中删除
                    if (selDictdata.Delete())
                    {
                        dictdataList.Remove(selDictdata);
                        //DelNodeAndItem(selDictdata);
                        //DBSourceController.refreshDBSource(sid, "1");
                    }
                    else
                    {
                        //fmErrorDialog fmError = new fmErrorDialog() { Error = "删除项与系统中的其他项有关联，不能删除!!", DetailError = selDictdata.dBSource.LastError };
                        //fmError.ShowDialog();
                        reJo.msg = "删除项与系统中的其他项有关联，不能删除!!";
                        return reJo.Value;
                    }
               
                
                #endregion
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

        //public static JObject GetProjectDictTableList(string sid)
        //{ 
        //            ExReJObject reJo = new ExReJObject();

        //    try
        //    {
        //        //string RootProjectTempDefnKeyword = "HXNY_DOCUMENTSYSTEM";

        //        User curUser = DBSourceController.GetCurrentUser(sid);
        //        if (curUser == null)
        //        {
        //            reJo.msg = "登录验证失败！请尝试重新登录！";
        //            return reJo.Value;
        //        }

        //        DBSource dbsource = curUser.dBSource;
        //        if (dbsource == null)
        //        {
        //            reJo.msg = "登录验证失败！请尝试重新登录！";
        //            return reJo.Value;
        //        }

        //        List<DictData> reDictDataList = new List<DictData>();
        //        List<DictData> dictdataList = dbsource.GetHeadDictDataList();
        //        reDictDataList = dictdataList.FindAll(dd => dd.O_iValue1 == 1);

        //        JObject joData = new JObject();
        //        JArray jaData=new JArray();
        //        foreach(DictData dd in reDictDataList){
        //            jaData.Add(new JObject(new JProperty("Name",dd.O_skey),new JProperty));
        //        }

        //        reJo.success = true;
        //        return reJo.Value;

        //    }
        //    catch (Exception e)
        //    {
        //        reJo.msg = e.Message;
        //        CommonController.WebWriteLog(reJo.msg);
        //    }

        //    return reJo.Value;
        //}

        /// <summary>
        /// 创建DictData
        /// </summary>
        /// <param name="dictdata"></param>
        /// <returns></returns>
        private static DictData CreateNewDictData(DBSource dbsource ,DictData dictdata, enDictDataType enType)
        {
            //对象的各个属性
            int iKey = 0;
            if (dictdata.O_ikey != 0)
            {
                iKey = dictdata.O_ikey;
            }

            int iValue1 = 0;
            if (dictdata.O_iValue1 != null)
            {
                iValue1 = (int)dictdata.O_iValue1;
            }

            int iValue2 = 0;
            if (dictdata.O_iValue2 != null)
            {
                iValue2 = (int)dictdata.O_iValue2;
            }

            string sKey = dictdata.O_skey == null ? "" : dictdata.O_skey;
            string sCode = dictdata.O_Code == null ? "" : dictdata.O_Code;
            string sDesc = dictdata.O_Desc == null ? "" : dictdata.O_Desc;
            string sValue1 = dictdata.O_sValue1 == null ? "" : dictdata.O_sValue1;
            string sValue2 = dictdata.O_sValue2 == null ? "" : dictdata.O_sValue2;
            string sValue3 = dictdata.O_sValue3 == null ? "" : dictdata.O_sValue3;
            string sValue4 = dictdata.O_sValue4 == null ? "" : dictdata.O_sValue4;
            string sValue5 = dictdata.O_sValue5 == null ? "" : dictdata.O_sValue5;

            //通过以上的dictdata新建一个DictData
            DictData newDictData = dbsource.NewDictData(enType,
                                                        iKey,
                                                        sKey,
                                                        sCode,
                                                        sDesc,
                                                        sValue1,
                                                        sValue2,
                                                        sValue3,
                                                        sValue4,
                                                        sValue5,
                                                        iValue1,
                                                        iValue2);
            return newDictData;
        }

    }
}
