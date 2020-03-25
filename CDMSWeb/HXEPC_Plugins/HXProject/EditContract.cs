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

namespace AVEVA.CDMS.HXEPC_Plugins
{
    public class EditContract
    {
        /// <summary>
        /// 新建厂家资料目录时，获取默认值
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public static JObject GetEditContractDefault(string sid, string ProjectKeyword)
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

                Project prjProject = CommonFunction.getParentProjectByTempDefn(m_prj, "HXNY_DOCUMENTSYSTEM");

                if (prjProject == null)
                {
                    reJo.msg = "获取项目目录失败！";
                    return reJo.Value;
                }

                //获取项目代码
                string strProjCode = prjProject.Code;//.GetAttrDataByKeyWord("COMPANY").ToString;
                string strProjDesc = prjProject.Description;

                JArray jaContract = new JArray();
                JObject joContract = new JObject();

                List<DictData> dictDataList = dbsource.GetDictDataList("Contract");
                //[o_Code]:公司编码,[o_Desc]：公司描述,[o_sValue1]：项目代码

                foreach (DictData data6 in dictDataList)
                {
                    if (!string.IsNullOrEmpty(data6.O_sValue1) && data6.O_sValue1 == strProjCode)
                    {
                        joContract = new JObject(
                            new JProperty("contractId", data6.O_ID.ToString()),
                            new JProperty("contractCode", data6.O_Code),
                            new JProperty("contractDesc", data6.O_Desc)
                            ////,
                            ////new JProperty("contractEngDesc", data6.O_sValue1)
                            );
                        jaContract.Add(joContract);
                    }
                }


                reJo.data = new JArray(
                    new JObject(new JProperty("projectCode", strProjCode),
                    new JProperty("projectDesc", strProjDesc),
                    new JProperty("ContractList", jaContract)));
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
        /// 新建厂家资料目录
        /// </summary>
        public static JObject CreateContract(string sid, string ProjectKeyword, string contractAttrJson)
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
                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(contractAttrJson);

                string strContractCode = "", strContractDesc = "",
                    strContractEngDesc = "", strContractChinese = "",
                    strAddress = "", strProvince = "",
                    strPostCode = "", strEMail = "",
                    strReceiver = "", strFaxNo = "", strPhone = "";

                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();

                    switch (strName)
                    {
                        case "contractCode":
                            strContractCode = strValue;
                            break;
                        case "contractDesc":
                            strContractDesc = strValue;
                            break;
                        //case "contractEngDesc":
                        //    strContractEngDesc = strValue;
                        //    break;
                    }
                }

                if (string.IsNullOrEmpty(strContractCode))
                {
                    reJo.msg = "请输入项目编号！";
                    return reJo.Value;
                }
                if (string.IsNullOrEmpty(strContractDesc))
                {
                    reJo.msg = "请输入项目名称！";
                    return reJo.Value;
                }

                #endregion

                Project prjProject = CommonFunction.getParentProjectByTempDefn(m_prj, "HXNY_DOCUMENTSYSTEM");

                if (prjProject == null)
                {
                    reJo.msg = "获取项目目录失败！";
                    return reJo.Value;
                }

                //获取项目代码
                string strProjCode = prjProject.Code;

                JArray jaData = new JArray();
                JObject joContract = new JObject();

                List<DictData> dictDataList = dbsource.GetDictDataList("Contract");
                //[o_Code]:公司编码,[o_Desc]：公司描述,[o_sValue1]：项目代码

                foreach (DictData data6 in dictDataList)
                {
                    if (!string.IsNullOrEmpty(data6.O_sValue1) && data6.O_sValue1 == strProjCode && data6.O_Code == strContractCode)
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
                //0,2,0,'Unit','"+strContractCode+"','"+strContractDesc+"','"+strProjCode+ "','','','','',0,0
                format = string.Format(format, new object[] {
                    0,2,0,"Contract",strContractCode,strContractDesc,strProjCode,"","","","",0,0
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
        /// 新建厂家资料目录
        /// </summary>
        public static JObject EDITContract(string sid, string ProjectKeyword, string contractAttrJson)
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
                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(contractAttrJson);

                string strContractId = "", strContractCode = "", strContractDesc = "",
                    strContractEngDesc = "", strContractChinese = "",
                    strAddress = "", strProvince = "",
                    strPostCode = "", strEMail = "",
                    strReceiver = "", strFaxNo = "", strPhone = "";

                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();

                    switch (strName)
                    {
                        case "contractId":
                            strContractId = strValue;
                            break;
                        case "contractCode":
                            strContractCode = strValue;
                            break;
                        case "contractDesc":
                            strContractDesc = strValue;
                            break;
                        //case "contractEngDesc":
                        //    strContractEngDesc = strValue;
                            break;
                    }
                }

                if (string.IsNullOrEmpty(strContractCode))
                {
                    reJo.msg = "请输入项目编号！";
                    return reJo.Value;
                }
                if (string.IsNullOrEmpty(strContractDesc))
                {
                    reJo.msg = "请输入项目名称！";
                    return reJo.Value;
                }
                #endregion

                Project prjProject = CommonFunction.getParentProjectByTempDefn(m_prj, "HXNY_DOCUMENTSYSTEM");

                if (prjProject == null)
                {
                    reJo.msg = "获取项目目录失败！";
                    return reJo.Value;
                }


                int contractId = Convert.ToInt32(strContractId);
                //获取项目代码
                string strProjCode = prjProject.Code;//.GetAttrDataByKeyWord("COMPANY").ToString;


                JArray jaData = new JArray();
                JObject joContract = new JObject();

                List<DictData> dictDataList = dbsource.GetDictDataList("Contract");
                //[o_Code]:公司编码,[o_Desc]：公司描述,[o_sValue1]：项目代码

                foreach (DictData data6 in dictDataList)
                {
                    if (!string.IsNullOrEmpty(data6.O_sValue1) && data6.O_sValue1 == strProjCode
                        && data6.O_Code == strContractCode && data6.O_ID != contractId)
                    {
                        reJo.msg = "已经存在相同的参建单位，请返回重试！";
                        return reJo.Value;
                    }
                }

                #region 添加到数据字典

                DictData dictData = null;

                foreach (DictData data6 in dictDataList)
                {
                    if (data6.O_ID == contractId)
                    {
                        dictData = data6;

                    }
                }

                if (dictData == null)
                {
                    reJo.msg = "参建单位ID不存在，请返回重试！";
                    return reJo.Value;

                }

                dictData.O_Code = strContractCode;
                dictData.O_Desc = strContractDesc;
                dictData.O_sValue1 = strProjCode;
               
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
        /// 删除机组
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="ProjectKeyword"></param>
        /// <param name="CrewCode"></param>
        /// <returns></returns>
        public static JObject DeleteContract(string sid, string ProjectKeyword, string ContractCode, string ContractId)
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

                Project prjProject = CommonFunction.getParentProjectByTempDefn(m_prj, "HXNY_DOCUMENTSYSTEM");

                if (prjProject == null)
                {
                    reJo.msg = "获取项目目录失败！";
                    return reJo.Value;
                }


                int contractId = Convert.ToInt32(ContractId);
                //获取项目代码
                string strProjCode = prjProject.Code;//.GetAttrDataByKeyWord("COMPANY").ToString;


                JArray jaData = new JArray();
                JObject joContract = new JObject();

                List<DictData> dictDataList = dbsource.GetDictDataList("Contract");
                //[o_Code]:公司编码,[o_Desc]：公司描述,[o_sValue1]：项目代码

                foreach (DictData data6 in dictDataList)
                {
                    if (!string.IsNullOrEmpty(data6.O_sValue1) && data6.O_sValue1 == strProjCode
                        && data6.O_Code == ContractCode && data6.O_ID == contractId)
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
    }
}
