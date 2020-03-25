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
//using LinqToDB;

namespace AVEVA.CDMS.HXEPC_Plugins
{
    public class DraftGetDefault
    {
        internal static JObject GetDefaultInfo(string sid, string ProjectKeyword, string DraftOnProject)
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

                //定位到发文目录
                //m_Project = LocalProject(m_Project);

                if (m_Project == null)
                {
                    reJo.msg = "参数错误！文件夹不存在！";
                    return reJo.Value;
                }


                #region 初始化变量
                //获取发文单位列表
                JObject joSendCompany = new JObject();

                //获取收文单位列表
                JObject joRecCompany = new JObject();

                string DocNumber = "";// 设置编号

                string groupCode = "", groupKeyword = "", groupType = "", sourceCompany = "", sourceCompanyDesc = "";

                //获取项目号
                string RootProjectCode = ""; string RootProjectDesc = ""; string ProjectDesc = "";

                string auditorList = "", auditorDesc = "";

                //是否在项目起草
                bool bDraftOnProject = true;
                #endregion

                //获取目录的信息，包括项目所在目录的代码，描述，当前目录的描述等
                DraftGetDefault.GetProjectInfo(m_Project, ref ProjectDesc, ref RootProjectCode, ref RootProjectDesc, ref bDraftOnProject);


                //获取所有项目部门
                DraftGetDefault.GetDepartmentInfo(dbsource, ref joRecCompany, ref joSendCompany);

                if (bDraftOnProject)
                {
                    //获取项目通信代码
                    DraftGetDefault.GetProjectShoreInfo("NOT", m_Project,
                         ref sourceCompany, ref sourceCompanyDesc, ref joRecCompany, ref joSendCompany);

                }
                else
                {
                    //获取登录用户的用户组，所在部门等信息
                    DraftGetDefault.GetUserGroupInfo("NOT", dbsource, ref groupCode,
                        ref groupKeyword, ref groupType, ref sourceCompany, ref sourceCompanyDesc);

                    //从组织机构获取部长助理（副部长）
                    DraftGetDefault.GetAuditUserInfo("NOT", dbsource, groupCode, "部长助理",
                        ref auditorList, ref auditorDesc);
                }

                JObject joData = new JObject(
                    new JProperty("RootProjectCode", RootProjectCode),
                    new JProperty("DocNumber", DocNumber),
                    new JProperty("RecCompanyList", joRecCompany),
                    new JProperty("SendCompanyList", joSendCompany),
                    new JProperty("SourceCompany", sourceCompany),
                    new JProperty("SourceCompanyDesc", sourceCompanyDesc),
                     new JProperty("GroupKeyword", groupKeyword),
                    new JProperty("GroupType", groupType),
                    new JProperty("AuditorList", auditorList),
                     new JProperty("AuditorDesc", auditorDesc),
                     new JProperty("RootProjectDesc", RootProjectDesc),
                      new JProperty("ProjectDesc", ProjectDesc)
                    );

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

        //获取目录的信息，包括项目所在目录的代码，描述，当前目录的描述等
        internal static bool GetProjectInfo(Project m_Project,ref string ProjectDesc, 
            ref string RootProjectCode, ref string RootProjectDesc,ref bool bDraftOnProject) {

            //获取项目号
            RootProjectCode = m_Project.GetValueByKeyWord("HXNY_DOCUMENTSYSTEM_CODE");
            if (RootProjectCode == null)
            {
                RootProjectCode = "";
                bDraftOnProject = false;
            }

            RootProjectDesc = m_Project.GetValueByKeyWord("HXNY_DOCUMENTSYSTEM_DESC");
            if (RootProjectDesc == null) RootProjectDesc = "";

            ProjectDesc = m_Project.Description;

            return true;
        }

        //起草信函时,获取项目部门信息
        internal static bool GetDepartmentInfo(DBSource dbsource, ref JObject joRecCompany, ref JObject joSendCompany) {
            //获取所有项目部门
            List<DictData> dictDataList = dbsource.GetDictDataList("Communication");
            //[o_Code]:英文描述,[o_Desc]：中文描述,[o_sValue1]：通信代码
            foreach (DictData data6 in dictDataList)
            {
                if (!string.IsNullOrEmpty(data6.O_sValue1))
                {
                    joRecCompany.Add(new JProperty(data6.O_sValue1, data6.O_Desc));
                    joSendCompany.Add(new JProperty(data6.O_sValue1, data6.O_Desc));
                }
            }
            return true;
        }

        //起草信函时,获取项目通信代码信息
        internal static bool GetProjectShoreInfo(string docType,Project m_Project,
            ref string sourceCompany, ref string sourceCompanyDesc, ref JObject joRecCompany, ref JObject joSendCompany) {

            #region 获取项目的通信代码
            AttrData data;
            string commDesc = "";
            if ((data = m_Project.GetAttrDataByKeyWord("PRO_COMNAME")) != null)
            {
                commDesc = data.ToString;
                sourceCompanyDesc = data.ToString;
            }
            if ((data = m_Project.GetAttrDataByKeyWord("RPO_ONSHORE")) != null)
            {
                string strData = data.ToString;
                if (!string.IsNullOrEmpty(strData))
                {
                    sourceCompany = data.ToString;

                    try { 
                    joRecCompany.Add(new JProperty(data.ToString, commDesc));
                    joSendCompany.Add(new JProperty(data.ToString, commDesc));
                    }catch{ }
                }
            }
            if (string.IsNullOrEmpty(sourceCompany))
            {
                if ((data = m_Project.GetAttrDataByKeyWord("RPO_OFFSHORE")) != null)
                {
                    string strData = data.ToString;
                    if (!string.IsNullOrEmpty(strData))
                    {
                        sourceCompany = data.ToString;
                        try
                        {
                            joRecCompany.Add(new JProperty(data.ToString, commDesc));
                        joSendCompany.Add(new JProperty(data.ToString, commDesc));
                        }
                        catch { }
                    }
                }
            }
            #endregion
            return true;
        }

        //起草信函时,获取登录用户的用户组信息
        internal static bool GetUserGroupInfo(string docType, DBSource dbsource, ref string groupCode, ref string groupKeyword,
           ref string groupType, ref string sourceCompany, ref string sourceCompanyDesc)
        {
            //获取组织机构用户组
            foreach (AVEVA.CDMS.Server.Group groupOrg in dbsource.AllGroupList)
            {
                if ((groupOrg.ParentGroup == null) && (groupOrg.O_grouptype == enGroupType.Organization))
                {
                    if (groupOrg.AllUserList.Contains(dbsource.LoginUser))
                    {

                        groupCode = groupOrg.Code;
                        groupKeyword = groupOrg.KeyWord;
                        groupType = "org";
                        if (string.IsNullOrEmpty(sourceCompany))
                        {
                            sourceCompany = groupOrg.Code;
                            sourceCompanyDesc = groupOrg.Description;
                        }
                        break;
                    }
                }
            }
            return true;
        }

        internal static bool GetAuditUserInfo(string docType, DBSource dbsource, string groupCode,
           string position, ref string auditorList, ref string auditorDesc)
        {
            //获取部长助理（副部长）
            //Server.Group gp = CommonFunction.GetGroupByDesc(dbsource, groupCode, "部长助理");
            Server.Group gp = CommonFunction.GetGroupByDesc(dbsource, groupCode, position);
            if (gp.AllUserList == null || gp.AllUserList.Count <= 0) return false;

            foreach (User user in gp.AllUserList)
            {
                auditorList = auditorList + user.KeyWord + ",";
                auditorDesc = auditorDesc + user.ToString + ";";
            }
            if (!string.IsNullOrEmpty(auditorList))
            {
                auditorList = auditorList.Substring(0, auditorList.Length - 1);
                auditorDesc = auditorDesc.Substring(0, auditorDesc.Length - 1);
            }
            else
            {
                return false;
            }
            return true;
        }


    }
}
