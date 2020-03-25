using System;
using System.Collections;
using System.Collections.Generic;
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
namespace AVEVA.CDMS.ZHEPC_Plugins
{
    public class Report
    {
        /// <summary>
        /// 获取创建信函表单的默认配置
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="ProjectKeyword"></param>
        /// <returns></returns>
        public static JObject GetFileCodeDefaultInfo(string sid, string ProjectKeyword)
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
                string documentType = m_prj.Code;

                Project prjProject = CommonFunction.getParentProjectByTempDefn(m_prj, "DESIGNPROJECT");

                if (prjProject == null)
                {
                    reJo.msg = "获取项目目录失败！";
                    return reJo.Value;
                }

                string curCrewCode = "";

                curCrewCode = prjProject.Code;
                if (string.IsNullOrEmpty(curCrewCode))
                {
                    reJo.msg = "获取项目来源失败！";
                    return reJo.Value;
                }

                JArray jaData = new JArray();

                ////获取机组列表
                //List<DictData> CrewList = GetCrewData(prjProject, "1", "");
                //获取单位列表
                List<DictData> UnitList = GetUnitData(prjProject, "1", "");
                ////获取专业列表
                //List<DictData> ProfessionList = GetProfessionData(prjProject, "1", "");
                ////获取部门列表
                //List<DictData> DepartmentList = GetDepartmentData(prjProject, "1", "");

                ////获取种类列表
                //List<DictData> KindList = GetKindData(prjProject, "1", "");

                JArray jaCrews = new JArray();
                //foreach (DictData data6 in CrewList)
                //{
                //    {
                //        JObject joCrews = new JObject(
                //            new JProperty("crewType", "机组"),
                //            new JProperty("crewId", data6.O_ID.ToString()),
                //            new JProperty("crewCode", data6.O_Code),
                //            new JProperty("crewDesc", data6.O_Desc)
                //            );
                //        jaCrews.Add(joCrews);
                //    }
                //}

                JArray jaUnits = new JArray();
                foreach (DictData data6 in UnitList)
                {
                    {
                        JObject joUnits = new JObject(
                            new JProperty("unitType", "参建单位"),
                            new JProperty("unitId", data6.O_ID.ToString()),
                            new JProperty("unitCode", data6.O_Code),
                            new JProperty("unitDesc", data6.O_Desc)
                            );
                        jaUnits.Add(joUnits);
                    }
                }

                JArray jaProfessions = new JArray();
                #region 当表式的专业选项只有一个的时候，就在初始化时返回一个选项
                if (documentType == "A.5" || documentType == "A.20" || documentType == "A.30")
                {
                    JObject joProfessions = new JObject(
                        new JProperty("professionType", "内容"),
                        new JProperty("professionId", "GC"),
                        new JProperty("professionCode", "GC"),
                        new JProperty("professionDesc", "工程")
                        );
                    jaProfessions.Add(joProfessions);
                }

                if (documentType == "A.16" || documentType == "A.17" || documentType == "A.19")
                {
                    JObject joProfessions = new JObject(
                        new JProperty("professionType", "专业"),
                        new JProperty("professionId", "JH"),
                        new JProperty("professionCode", "JH"),
                        new JProperty("professionDesc", "计划")
                        );
                    jaProfessions.Add(joProfessions);
                }

                if (documentType == "A.38" || documentType == "A.57" )
                {
                    JObject joProfessions = new JObject(
                        new JProperty("professionType", "专业"),
                        new JProperty("professionId", "AQ"),
                        new JProperty("professionCode", "AQ"),
                        new JProperty("professionDesc", "安全")
                        );
                    jaProfessions.Add(joProfessions);
                }

                if (documentType == "C.1" || documentType == "C.2")
                {
                    JObject joProfessions = new JObject(
                        new JProperty("professionType", "专业"),
                        new JProperty("professionId", "SJ"),
                        new JProperty("professionCode", "SJ"),
                        new JProperty("professionDesc", "设计")
                        );
                    jaProfessions.Add(joProfessions);
                } 
                #endregion

                //foreach (DictData data6 in ProfessionList)
                //{
                //    {
                //        JObject joProfessions = new JObject(
                //            new JProperty("professionType", "专业"),
                //            new JProperty("professionId", data6.O_ID.ToString()),
                //            new JProperty("professionCode", data6.O_Code),
                //            new JProperty("professionDesc", data6.O_Desc)
                //            );
                //        jaProfessions.Add(joProfessions);
                //    }
                //}
                //if (documentType == "D.3")
                //{
                //    foreach (DictData data6 in DepartmentList)
                //    {
                //        {
                //            JObject joDeaprtments = new JObject(
                //                new JProperty("professionType", "部门"),
                //                new JProperty("professionId", data6.O_ID.ToString()),
                //                new JProperty("professionCode", data6.O_Code),
                //                new JProperty("professionDesc", data6.O_Desc)
                //                );
                //            jaProfessions.Add(joDeaprtments);
                //        }
                //    }
                //}

                JArray jaKinds = new JArray();
                //foreach (DictData data6 in KindList)
                //{
                //    {
                //        JObject joKinds = new JObject(
                //            new JProperty("kindType", "种类"),
                //            new JProperty("kindId", data6.O_ID.ToString()),
                //            new JProperty("kindCode", data6.O_Code),
                //            new JProperty("kindDesc", data6.O_Desc)
                //            );
                //        jaKinds.Add(joKinds);
                //    }
                //}

                //获取从上级目录单位简码
                string curUnitCode = m_prj.GetValueByKeyWord("CONSTRUCTIONUNIT_CODE");
                if (!string.IsNullOrEmpty(curUnitCode))
                {

                }

                jaData.Add(new JObject(
                    new JProperty("CrewList", jaCrews),
                    new JProperty("UnitList", jaUnits),
                    new JProperty("ProfessionList", jaProfessions),
                    new JProperty("KindList", jaKinds),
                    new JProperty("CurUnitCode", curUnitCode)
                    ));

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

        //获取机组列表
        public static JObject GetCrewList(string sid, string ProjectKeyword, string Filter)
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

                Project prjProject = CommonFunction.getParentProjectByTempDefn(m_prj, "DESIGNPROJECT");

                if (prjProject == null)
                {
                    reJo.msg = "获取项目目录失败！";
                    return reJo.Value;
                }

                string curCrewCode = "";

                curCrewCode = prjProject.Code;
                if (string.IsNullOrEmpty(curCrewCode))
                {
                    reJo.msg = "获取项目来源失败！";
                    return reJo.Value;
                }

                JArray jaData = new JArray();

                //获取机组列表
                List<DictData> CrewList = GetCrewData(prjProject, "1", Filter);

                JArray jaCrews = new JArray();
                foreach (DictData data6 in CrewList)
                {
                    {
                        JObject joCrews = new JObject(
                            new JProperty("crewType", "机组"),
                            new JProperty("crewId", data6.O_ID.ToString()),
                            new JProperty("crewCode", data6.O_Code),
                            new JProperty("crewDesc", data6.O_Desc)
                            );
                        jaCrews.Add(joCrews);
                    }
                }

                jaData.Add(new JObject(
                    new JProperty("CrewList", jaCrews)
                    ));

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

        //获取参建单位列表
        public static JObject GetUnitList(string sid, string ProjectKeyword, string Filter)
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

                Project prjProject = CommonFunction.getParentProjectByTempDefn(m_prj, "DESIGNPROJECT");

                if (prjProject == null)
                {
                    reJo.msg = "获取项目目录失败！";
                    return reJo.Value;
                }

                string curUnitCode = "";

                curUnitCode = prjProject.Code;
                if (string.IsNullOrEmpty(curUnitCode))
                {
                    reJo.msg = "获取项目来源失败！";
                    return reJo.Value;
                }

                JArray jaData = new JArray();

                //获取机组列表
                List<DictData> UnitList = GetUnitData(prjProject, "1", Filter);

                JArray jaUnits = new JArray();
                foreach (DictData data6 in UnitList)
                {
                    {
                        JObject joUnits = new JObject(
                            new JProperty("unitType", "参建单位"),
                            new JProperty("unitId", data6.O_ID.ToString()),
                            new JProperty("unitCode", data6.O_Code),
                            new JProperty("unitDesc", data6.O_Desc)
                            );
                        jaUnits.Add(joUnits);
                    }
                }

                jaData.Add(new JObject(
                    new JProperty("UnitList", jaUnits)
                    ));

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

        //获取专业列表
        public static JObject GetProfessionList(string sid, string ProjectKeyword, string Filter)
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
                string documentType = m_prj.Code;

                Project prjProject = CommonFunction.getParentProjectByTempDefn(m_prj, "DESIGNPROJECT");

                if (prjProject == null)
                {
                    reJo.msg = "获取项目目录失败！";
                    return reJo.Value;
                }

                string curProfessionCode = "";

                curProfessionCode = prjProject.Code;
                if (string.IsNullOrEmpty(curProfessionCode))
                {
                    reJo.msg = "获取项目来源失败！";
                    return reJo.Value;
                }

                JArray jaData = new JArray();

                JArray jaProfessions = new JArray();

                if (documentType == "A.7" || documentType == "A.18" || documentType == "A.29" ||
                    documentType == "A.31" || documentType == "A.34" || documentType == "A.37" ||
                    documentType == "C.4" || documentType == "D.3")
                {
                    //获取专业列表
                    List<DictData> ProfessionList = GetProfessionData(prjProject, "1", Filter);

                    foreach (DictData data6 in ProfessionList)
                    {
                        {
                            JObject joProfessions = new JObject(
                                new JProperty("professionType", "专业"),
                                new JProperty("professionId", data6.O_ID.ToString()),
                                new JProperty("professionCode", data6.O_Code),
                                new JProperty("professionDesc", data6.O_Desc)
                                );
                            jaProfessions.Add(joProfessions);
                        }
                    }
                }

                if (documentType == "D.3" || documentType == "D.4" || documentType == "A.33" ||
                    documentType == "A.41" || documentType == "A.53" || documentType == "A.54")
                {
                    //获取部门列表
                    List<DictData> DepartmentList = GetDepartmentData(prjProject, "1", Filter);

                    foreach (DictData data6 in DepartmentList)
                    {
                        {
                            JObject joDepartments = new JObject(
                                new JProperty("professionType", "部门"),
                                new JProperty("professionId", data6.O_ID.ToString()),
                                new JProperty("professionCode", data6.O_Code),
                                new JProperty("professionDesc", data6.O_Desc)
                                );
                            jaProfessions.Add(joDepartments);
                        }
                    }
                }

                #region 部分表式固定设置了专业选项
                if (documentType == "A.3")
                {
                    JObject joDepartments = new JObject(
                        new JProperty("professionType", "内容"),
                        new JProperty("professionId", "SZ"),
                        new JProperty("professionCode", "SZ"),
                        new JProperty("professionDesc", "施工组织")
                        );
                    jaProfessions.Add(joDepartments);

                    joDepartments = new JObject(
                        new JProperty("professionType", "内容"),
                        new JProperty("professionId", "GH"),
                        new JProperty("professionCode", "GH"),
                        new JProperty("professionDesc", "管理规划")
                        );
                    jaProfessions.Add(joDepartments);
                }

                if (documentType == "A.6")
                {
                    JObject joDepartments = new JObject(
                        new JProperty("professionType", "专业"),
                        new JProperty("professionId", "TJ"),
                        new JProperty("professionCode", "TJ"),
                        new JProperty("professionDesc", "土建")
                        );
                    jaProfessions.Add(joDepartments);

                    joDepartments = new JObject(
                        new JProperty("professionType", "专业"),
                        new JProperty("professionId", "AZ"),
                        new JProperty("professionCode", "AZ"),
                        new JProperty("professionDesc", "安装")
                        );
                    jaProfessions.Add(joDepartments);
                } 
                #endregion

                jaData.Add(new JObject(
                    new JProperty("ProfessionList", jaProfessions)
                    ));

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

        //获取种类列表
        public static JObject GetKindList(string sid, string ProjectKeyword, string Filter)
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

                Project prjProject = CommonFunction.getParentProjectByTempDefn(m_prj, "DESIGNPROJECT");

                if (prjProject == null)
                {
                    reJo.msg = "获取项目目录失败！";
                    return reJo.Value;
                }

                string curKindCode = "";

                curKindCode = prjProject.Code;
                if (string.IsNullOrEmpty(curKindCode))
                {
                    reJo.msg = "获取项目来源失败！";
                    return reJo.Value;
                }

                JArray jaData = new JArray();

                //获取机组列表
                List<DictData> KindList = GetKindData(prjProject, "1", Filter);

                JArray jaKinds = new JArray();
                foreach (DictData data6 in KindList)
                {
                    {
                        JObject joKinds = new JObject(
                            new JProperty("kindType", "专业"),
                            new JProperty("kindId", data6.O_ID.ToString()),
                            new JProperty("kindCode", data6.O_Code),
                            new JProperty("kindDesc", data6.O_Desc)
                            );
                        jaKinds.Add(joKinds);
                    }
                }

                jaData.Add(new JObject(
                    new JProperty("KindList", jaKinds)
                    ));

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
        //获取机组类型
        public static List<DictData> GetCrewData(Project prjProject, string page, string Filter)
        {
            //ExReJObject reJo = new ExReJObject();
            List<DictData> reDDs = new List<DictData>();

            try
            {
                page = page ?? "1";
                string limit = "50";
                page = (Convert.ToInt32(page) - 1).ToString();
                int CurPage = Convert.ToInt32(page);



                Filter = Filter.Trim().ToLower();

                string curCrewCode = "";

                curCrewCode = prjProject.Code;
                if (string.IsNullOrEmpty(curCrewCode))
                {
                    //reJo.msg = "获取项目来源失败！";
                    return reDDs;
                }

                JArray jaData = new JArray();

                #region 添加机组
                //获取所有参建单位
                List<DictData> dictDataList = prjProject.dBSource.GetDictDataList("Crew");
                List<DictData> resultDDList = new List<DictData>();


                foreach (DictData data6 in dictDataList)
                {
                    //判断是否符合过滤条件
                    if (!string.IsNullOrEmpty(Filter) &&
                        data6.O_Code.ToLower().IndexOf(Filter) < 0 &&
                        data6.O_Desc.ToLower().IndexOf(Filter) < 0
                        //&& data6.O_sValue1.ToLower().IndexOf(Filter) < 0
                        )
                    {
                        continue;
                    }

                    if (data6.O_sValue1 != curCrewCode)
                    {
                        continue;
                    }
                    resultDDList.Add(data6);
                }

                #endregion

                int ShowNum = 50;

                reDDs = resultDDList.Skip(CurPage * ShowNum).Take(ShowNum).ToList();
                return reDDs;


            }
            catch (Exception e)
            {

            }

            return reDDs;
        }

        //获取公司列表
        public static List<DictData> GetUnitData(Project prjProject, string page, string Filter)
        {
            //ExReJObject reJo = new ExReJObject();
            List<DictData> reDDs = new List<DictData>();

            try
            {
                page = page ?? "1";
                string limit = "50";
                page = (Convert.ToInt32(page) - 1).ToString();
                int CurPage = Convert.ToInt32(page);



                Filter = Filter.Trim().ToLower();

                string curUnitCode = "";

                curUnitCode = prjProject.Code;
                if (string.IsNullOrEmpty(curUnitCode))
                {
                    //reJo.msg = "获取项目来源失败！";
                    return reDDs;
                }

                JArray jaData = new JArray();

                #region 添加机组
                //获取所有参建单位
                List<DictData> dictDataList = prjProject.dBSource.GetDictDataList("Unit");
                List<DictData> resultDDList = new List<DictData>();


                foreach (DictData data6 in dictDataList)
                {
                    //判断是否符合过滤条件
                    if (!string.IsNullOrEmpty(Filter) &&
                        data6.O_Code.ToLower().IndexOf(Filter) < 0 &&
                        data6.O_Desc.ToLower().IndexOf(Filter) < 0
                        //&&
                        //data6.O_sValue1.ToLower().IndexOf(Filter) < 0
                        )
                    {
                        continue;
                    }

                    if (data6.O_sValue1 != curUnitCode)
                    {
                        continue;
                    }
                    resultDDList.Add(data6);
                }

                #endregion

                int ShowNum = 50;

                reDDs = resultDDList.Skip(CurPage * ShowNum).Take(ShowNum).ToList();
                return reDDs;


            }
            catch (Exception e)
            {

            }

            return reDDs;
        }

        //获取专业类型
        public static List<DictData> GetProfessionData(Project prjProject, string page, string Filter)
        {
            //ExReJObject reJo = new ExReJObject();
            List<DictData> reDDs = new List<DictData>();

            try
            {
                page = page ?? "1";
                string limit = "50";
                page = (Convert.ToInt32(page) - 1).ToString();
                int CurPage = Convert.ToInt32(page);



                Filter = Filter.Trim().ToLower();

                string curProfessionCode = "";

                curProfessionCode = prjProject.Code;
                if (string.IsNullOrEmpty(curProfessionCode))
                {
                    //reJo.msg = "获取项目来源失败！";
                    return reDDs;
                }

                JArray jaData = new JArray();

                #region 添加机组
                //获取所有参建单位
                List<DictData> dictDataList = prjProject.dBSource.GetDictDataList("Profession");
                List<DictData> resultDDList = new List<DictData>();


                foreach (DictData data6 in dictDataList)
                {
                    //判断是否符合过滤条件
                    if (!string.IsNullOrEmpty(Filter) &&
                        data6.O_Code.ToLower().IndexOf(Filter) < 0 &&
                        data6.O_Desc.ToLower().IndexOf(Filter) < 0
                        //&&
                        //data6.O_sValue1.ToLower().IndexOf(Filter) < 0
                        )
                    {
                        continue;
                    }

                    if (data6.O_sValue1 != curProfessionCode)
                    {
                        continue;
                    }
                    resultDDList.Add(data6);
                }

                #endregion

                int ShowNum = 50;

                reDDs = resultDDList.Skip(CurPage * ShowNum).Take(ShowNum).ToList();
                return reDDs;


            }
            catch (Exception e)
            {

            }

            return reDDs;
        }


        //获取部门类型
        public static List<DictData> GetDepartmentData(Project prjProject, string page, string Filter)
        {
            //ExReJObject reJo = new ExReJObject();
            List<DictData> reDDs = new List<DictData>();

            try
            {
                page = page ?? "1";
                string limit = "50";
                page = (Convert.ToInt32(page) - 1).ToString();
                int CurPage = Convert.ToInt32(page);



                Filter = Filter.Trim().ToLower();

                string curDepartmentCode = "";

                curDepartmentCode = prjProject.Code;
                if (string.IsNullOrEmpty(curDepartmentCode))
                {
                    //reJo.msg = "获取项目来源失败！";
                    return reDDs;
                }

                JArray jaData = new JArray();

                #region 添加机组
                //获取所有参建单位
                List<DictData> dictDataList = prjProject.dBSource.GetDictDataList("Department");
                List<DictData> resultDDList = new List<DictData>();


                foreach (DictData data6 in dictDataList)
                {
                    //判断是否符合过滤条件
                    if (!string.IsNullOrEmpty(Filter) &&
                        data6.O_Code.ToLower().IndexOf(Filter) < 0 &&
                        data6.O_Desc.ToLower().IndexOf(Filter) < 0
                        //&&
                        //data6.O_sValue1.ToLower().IndexOf(Filter) < 0
                        )
                    {
                        continue;
                    }

                    if (data6.O_sValue1 != curDepartmentCode)
                    {
                        continue;
                    }
                    resultDDList.Add(data6);
                }

                #endregion

                int ShowNum = 50;

                reDDs = resultDDList.Skip(CurPage * ShowNum).Take(ShowNum).ToList();
                return reDDs;


            }
            catch (Exception e)
            {

            }

            return reDDs;
        }
        //获取种类类型
        public static List<DictData> GetKindData(Project prjProject, string page, string Filter)
        {
            //ExReJObject reJo = new ExReJObject();
            List<DictData> reDDs = new List<DictData>();

            try
            {
                page = page ?? "1";
                string limit = "50";
                page = (Convert.ToInt32(page) - 1).ToString();
                int CurPage = Convert.ToInt32(page);



                Filter = Filter.Trim().ToLower();

                string curKindCode = "";

                curKindCode = prjProject.Code;
                if (string.IsNullOrEmpty(curKindCode))
                {
                    //reJo.msg = "获取项目来源失败！";
                    return reDDs;
                }

                JArray jaData = new JArray();

                #region 添加机组
                //获取所有参建单位
                List<DictData> dictDataList = prjProject.dBSource.GetDictDataList("Kind");
                List<DictData> resultDDList = new List<DictData>();


                foreach (DictData data6 in dictDataList)
                {
                    //判断是否符合过滤条件
                    if (!string.IsNullOrEmpty(Filter) &&
                        data6.O_Code.ToLower().IndexOf(Filter) < 0 &&
                        data6.O_Desc.ToLower().IndexOf(Filter) < 0
                        //&&
                        //data6.O_sValue1.ToLower().IndexOf(Filter) < 0
                        )
                    {
                        continue;
                    }

                    if (data6.O_sValue1 != curKindCode)
                    {
                        continue;
                    }
                    resultDDList.Add(data6);
                }

                #endregion

                int ShowNum = 50;

                reDDs = resultDDList.Skip(CurPage * ShowNum).Take(ShowNum).ToList();
                return reDDs;


            }
            catch (Exception e)
            {

            }

            return reDDs;
        }


        //获取文件著录编码流水号
        public static JObject GetFileCodeNumber(string sid, string FileCodePerfix)
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

                string runNum = "";

                string strSql = string.Format(
           "select lf.FILECODE from CDMS_Doc as cd inner join " +
             "(select Itemno, CD_FILECODE as FILECODE from User_Report where CD_FILECODE like '%{0}%')" +
             " as lf " +
             "on  cd.o_itemno = lf.Itemno " +
             " where cd.o_dmsstatus != 10 order by lf.FILECODE ",
       FileCodePerfix);

                string[] strArry = dbsource.DBExecuteSQL(strSql);

                if (strArry == null || strArry.Length == 0 ||
                    (strArry[strArry.Length - 1]).Length < FileCodePerfix.Length + 3
                    )
                {
                    runNum = "001";

                }
                else
                {

                    //5位数，不够位数补零
                    int tempNum = Convert.ToInt32((strArry[strArry.Length - 1]).Substring(FileCodePerfix.Length, 3));

                    runNum = (tempNum + 1).ToString("d3");
                }
                reJo.success = true;
                reJo.data = new JArray(new JObject(new JProperty("RunNum", runNum)));
                return reJo.Value;
            }
            catch (Exception exception)
            {
                WebApi.CommonController.WebWriteLog(exception.Message + "\r\n" + exception.Source + "\r\n" + exception.StackTrace);
                reJo.msg = "获取文件编号失败！" + exception.Message + "\r\n" + exception.Source + "\r\n" + exception.StackTrace;
            }
            return reJo.Value;
        }

        /// <summary>
        /// 创建报审单文档
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="ProjectKeyword">目录Keyword</param>
        /// <param name="documentType">文档表式</param>
        /// <param name="fileCode">文档编号</param>
        /// <param name="docAttrJson">文档属性</param>
        /// <returns></returns>
        public static JObject CreateDocument(string sid, string ProjectKeyword, string documentType, string fileCode, string title, string docAttrJson)
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

                #region 生成Doc文档
                string strDocDesc = getDocDesc(documentType, title); //获取文档描述


                List<TempDefn> TempDefns = dbsource.GetTempDefnByKeyWord("GEDIREPORT");
                if (TempDefns.Count <= 0)
                {
                    reJo.msg = "没有与其相关的文档模板，创建无法正常完成";
                    return reJo.Value;
                }

                TempDefn mTempDefn = TempDefns[0];

                if (mTempDefn == null)
                {
                    reJo.msg = "没有与其相关的文档模板，创建无法正常完成";
                    return reJo.Value;
                }

                Project projectByName = proj;

                bool hasRepeated = false;

                //查找源目录是否重复编码的接口
                foreach (Doc docItem in projectByName.DocList)
                {
                    if (docItem.TempDefn != null && docItem.Code.IndexOf(fileCode) >= 0)
                    {

                        hasRepeated = true;
                        break;
                    }
                }

                if (hasRepeated == true)
                {
                    reJo.msg = "已存在重复编码的函件 " + fileCode + " ！";
                    return reJo.Value;
                }

                //创建存储路径
                string destPath = projectByName.FullPath;
                if (!Directory.Exists(destPath))
                {
                    Directory.CreateDirectory(destPath);
                }

                string itemName = "";
                IEnumerable<string> source = from docx in projectByName.DocList select docx.Code;
                itemName = fileCode + " " + strDocDesc;
                if (source.Contains<string>(itemName))
                {
                    for (int i = 1; i < 0x3e8; i++)
                    {
                        itemName = fileCode + " " + strDocDesc + i.ToString();
                        if (!source.Contains<string>(itemName))
                        {
                            break;
                        }
                    }
                }

                Doc doc = projectByName.NewDoc(itemName + ".docx", itemName, "", mTempDefn);
                if (doc == null)
                {
                    reJo.msg = "新建" + strDocDesc + "出错！";
                    return reJo.Value;
                }

                doc.GetAttrDataByKeyWord("CD_FILECODE").SetCodeDesc(fileCode);
                doc.GetAttrDataByKeyWord("CD_TITLE").SetCodeDesc(title);//主题
                doc.GetAttrDataByKeyWord("CD_DOCTYPE").SetCodeDesc(documentType);//内容

                bool flag = doc.AttrDataList.SaveData();

                #endregion

                //下载Word模板并填写
                CreateWordDocument(doc, documentType, fileCode, title, docAttrJson);

                string strDocList = doc.KeyWord;

                reJo.success = true;
                reJo.data = new JArray(new JObject(new JProperty("ProjectKeyword", doc.Project.KeyWord),
                    new JProperty("DocKeyword", doc.KeyWord), new JProperty("DocList", strDocList),
                    new JProperty("DocCode", doc.Code)));
                return reJo.Value;
            }
            catch (Exception exception)
            {
                WebApi.CommonController.WebWriteLog(exception.Message + "\r\n" + exception.Source + "\r\n" + exception.StackTrace);
                reJo.msg = "创建文档失败！" + exception.Message + "\r\n" + exception.Source + "\r\n" + exception.StackTrace;
            }
            return reJo.Value;
        }

        /// <summary>
        /// 在关联文件夹创建快捷方式
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="ProjectKeyword"></param>
        /// <param name="documentType"></param>
        /// <param name="fileCode"></param>
        /// <param name="title"></param>
        /// <param name="docAttrJson"></param>
        /// <returns></returns>
        public static JObject CreateDocShortcut(string sid, string ProjectKeyword, string documentType, string DocList, string docAttrJson)
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

                //获取项目目录
                Project projectRoot = CommonFunction.getParentProjectByTempDefn(proj, "DESIGNPROJECT");

                //获取参建单位目录
                Project projUnit = CommonFunction.getParentProjectByTempDefn(proj, "CONSTRUCTIONUNIT");

                //获取关联目录
                Project projRa = projectRoot.ChildProjectList.Find(p => p.TempDefn!=null && p.TempDefn.KeyWord == "PROJECTRELATION");

                //获取关联目录下的参建单位目录
                Project projRaUnit = projRa.ChildProjectList.Find(p => p.Code == projUnit.Code);
                if (projRaUnit == null) {
                    reJo.msg = "关联参建单位文件夹不存在！";
                    return reJo.Value;
                }

                //获取关联目录下的表式目录
                Project projContactType = projRaUnit.ChildProjectList.Find(p => p.Code == proj.Code);
                if (projContactType == null)
                {
                    reJo.msg = "关联表式分类文件夹不存在！";
                    return reJo.Value;
                }

                string[] docArray = DocList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string docKey in docArray) {
                    Object obj = dbsource.GetObjectByKeyWord(docKey);
                    if (obj != null & obj is Doc) {
                        Doc docItem = (Doc)obj;
                        projContactType.NewDoc(docItem);
                    }

                }

                reJo.success = true;
                return reJo.Value;
            }
            catch (Exception exception)
            {
                WebApi.CommonController.WebWriteLog(exception.Message + "\r\n" + exception.Source + "\r\n" + exception.StackTrace);
                reJo.msg = "创建文档快捷方式失败！" + exception.Message + "\r\n" + exception.Source + "\r\n" + exception.StackTrace;
            }
            return reJo.Value;
        }

        public static JObject GetNavNewDocs(string sid, string page, string limit) { 
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

                JArray jaData = new JArray();

                //List<Doc> docList = dbsource.SelectDoc("select * from CDMS_Doc where o_dmsstatus !=10 and o_shortDocID is not null order by o_credatetime desc");
                List<Doc> docList = dbsource.SelectDoc("select * from CDMS_Doc where o_dmsstatus !=10 and o_shortDocID is not null order by o_credatetime desc");

                List<Doc> newDocList = new List<Doc>();

                foreach (Doc docItem in docList)
                {
                    //判断是否在关联文件夹下
                    string raProjName = docItem.Project.GetValueByKeyWord("PROJECTRELATION_CODE");

                    if (string.IsNullOrEmpty(raProjName)) continue;

                    Doc doc = docItem.ShortCutDoc;
                    if (doc == null) continue;

                    //只筛选出报审单
                    if (doc.TempDefn == null || doc.TempDefn.KeyWord != "GEDIREPORT")
                    {
                        continue;
                    }

                    bool hasRight = WebApi.DocController.GetDocDFReadRight(doc, curUser);

                    if (hasRight == false)
                    {
                        continue;
                    }

                    newDocList.Add(docItem);

                }

                List<Doc> AllDocList = new List<Doc>();

                int index = 0;
                foreach (Doc doc in newDocList)
                {
                    index = index + 1;
                    if (index > ShowNum * (CurPage - 1) && index <= ShowNum * CurPage)
                    {
                        AllDocList.Add(doc);
                    }
                    if (index >= ShowNum * CurPage)
                    {
                        break;
                    }
                }

                foreach (Doc docItem in AllDocList) {
                    Doc doc = docItem.ShortCutDoc;

                    string unitName = doc.GetValueByKeyWord("CONSTRUCTIONUNIT_DESC");
                    string isMount = doc.GetValueByKeyWord("CD_ISMOUNT");
                    string mountState = "未关闭";
                    if (isMount == "是") {
                        mountState = "已关闭";
                    } 
                    jaData.Add(new JObject(new JProperty("Keyword", docItem.KeyWord),
                        new JProperty("Title", doc.ToString),
                        new JProperty("Creater", doc.Creater.O_userdesc),
                        new JProperty("DocumentType", doc.Project.ToString),
                        new JProperty("Unit", unitName),
                        new JProperty("CreateTime", doc.O_credatetime.ToString("yyyy.MM.dd")),
                        new JProperty("MountState", mountState)));
                }

                reJo.total = newDocList.Count;
                reJo.data = jaData;

                reJo.success = true;
                return reJo.Value;
            }
            catch (Exception exception)
            {
                WebApi.CommonController.WebWriteLog(exception.Message + "\r\n" + exception.Source + "\r\n" + exception.StackTrace);
                reJo.msg = "获取最新文档失败！" + exception.Message + "\r\n" + exception.Source + "\r\n" + exception.StackTrace;
            }
            return reJo.Value;
        }

        //挂接已关闭文件后，修改文档的属性为已挂接 
        public static JObject AfterMountCloseFile(string sid, string DocKeyword) { 
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

                Doc sdoc = dbsource.GetDocByKeyWord(DocKeyword);

                if (sdoc == null) {
                    reJo.msg = "修改文档挂接属性失败！文档不存在！";
                    return reJo.Value;
                }

                Doc doc = sdoc.ShortCutDoc == null ? sdoc : sdoc.ShortCutDoc;

                doc.GetAttrDataByKeyWord("CD_ISMOUNT").SetCodeDesc("是");

                doc.AttrDataList.SaveData();

                reJo.success = true;
                return reJo.Value;
            }
            catch (Exception exception)
            {
                WebApi.CommonController.WebWriteLog(exception.Message + "\r\n" + exception.Source + "\r\n" + exception.StackTrace);
                reJo.msg = "修改文档挂接属性失败！" + exception.Message + "\r\n" + exception.Source + "\r\n" + exception.StackTrace;
            }
            return reJo.Value;

        }


        //取消文件的关闭状态，修改文档的属性为未挂接 
        public static JObject UnCloseFile(string sid, string DocKeyword)
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

                Doc sdoc = dbsource.GetDocByKeyWord(DocKeyword);

                if (sdoc == null)
                {
                    reJo.msg = "修改文档挂接属性失败！文档不存在！";
                    return reJo.Value;
                }

                Doc doc = sdoc.ShortCutDoc == null ? sdoc : sdoc.ShortCutDoc;

                doc.GetAttrDataByKeyWord("CD_ISMOUNT").SetCodeDesc("否");

                doc.AttrDataList.SaveData();

                reJo.success = true;
                return reJo.Value;
            }
            catch (Exception exception)
            {
                WebApi.CommonController.WebWriteLog(exception.Message + "\r\n" + exception.Source + "\r\n" + exception.StackTrace);
                reJo.msg = "修改文档挂接属性失败！" + exception.Message + "\r\n" + exception.Source + "\r\n" + exception.StackTrace;
            }
            return reJo.Value;

        }

        //获取文档描述
        internal static string getDocDesc(string docType, string title)
        {
            if (docType == "A.3")
            {
                return title + "施工组织设计报审表";
            }
            if (docType == "A.5")
            {
                return title + "分包单位资格报审表";
            }
            if (docType == "A.6")
            {
                return title + "单位资质报审表";
            }
            if (docType == "A.7")
            {
                return title + "人员资质报审表";
            }
            if (docType == "A.16")
            {
                return title + "计划、调整计划报审表";
            }
            if (docType == "A.17")
            {
                return title + "费用索赔申请表";
            }
            if (docType == "A.18")
            {
                return title + "监理工程师通知回复单";
            }
            if (docType == "A.19")
            {
                return title + "工程款支付申请表";
            }
            if (docType == "A.20")
            {
                return title + "工期变更报审表";
            }
            if (docType == "A.29")
            {
                return title + "工程量签证单";
            }
            if (docType == "A.30")
            {
                return title + "总承包单位资质报审表";
            }
            if (docType == "A.31")
            {
                return title + "强制性条文实施计划、细则报审表";
            }
            if (docType == "A.33")
            {
                return title + "工程变更费用报审表";
            }
            if (docType == "A.34")
            {
                return title + "工程变更签证单";
            }
            if (docType == "A.37")
            {
                return title + "工程竣工验收签证书";
            }
            if (docType == "A.38")
            {
                return title + "危险源辨识与风险评价报审表";
            }
            if (docType == "A.41")
            {
                return title + "年（月）度资金需求计划报审表";
            }
            if (docType == "A.53")
            {
                return title + "安全（质量)管理体系报审表";
            }
            if (docType == "A.54")
            {
                return title + "安全文明施工二次策划报审表";
            }
            if (docType == "A.57")
            {
                return title + "安全问题整改回复单";
            }
            if (docType == "C.1")
            {
                return title + "图纸交付计划报审表";
            }
            if (docType == "C.2")
            {
                return title + "设计文件报检表";
            }
            if (docType == "C.3")
            {
                return title + "设计变更通知单";
            }
            if (docType == "C.4")
            {
                return title + "设计交底纪要";
            }
            if (docType == "D.3")
            {
                return title + "工程联系单";
            }
            if (docType == "D.4")
            {
                return title + "标准、规程、规范清单报审表";
            }
            return null;
        }

        //获取ISO模板文件名
        internal static string getDocIsoType(string docType, string title)
        {
            if (docType == "A.3")
            {
                return "表A.3施工组织设计报审表";
            }
            if (docType == "A.5")
            {
                return "表A.5分包单位资格报审表";
            }
            if (docType == "A.6")
            {
                return "表A.6单位资质报审表";
            }
            if (docType == "A.7")
            {
                return "表A.7人员资质报审表";
            }
            if (docType == "A.16")
            {
                return "表A.16计划、调整计划报审表";
            }
            if (docType == "A.17")
            {
                return "表A.17费用索赔申请表";
            }
            if (docType == "A.18")
            {
                return "表A.18监理工程师通知回复单";
            }
            if (docType == "A.19")
            {
                return "表A.19工程款支付申请表";
            }
            if (docType == "A.20")
            {
                return "表A.20工期变更报审表";
            }
            if (docType == "A.29")
            {
                return "表A.29工程量签证单";
            }
            if (docType == "A.30")
            {
                return "表A.30总承包单位资质报审表";
            }
            if (docType == "A.31")
            {
                return "表A.31强制性条文实施计划、细则报审表";
            }
            if (docType == "A.33")
            {
                return "表A.33 工程变更费用报审表";
            }
            if (docType == "A.34")
            {
                return "表A.34工程变更签证单";
            }
            if (docType == "A.37")
            {
                return "表A.37工程竣工验收签证书";
            }
            if (docType == "A.38")
            {
                return "表A.38危险源辨识与风险评价报审表";
            }
            if (docType == "A.41")
            {
                return "表A.41 年（月）度资金需求计划报审表";
            }
            if (docType == "A.53")
            {
                return "表A.53 安全（质量)管理体系报审表";
            }
            if (docType == "A.54")
            {
                return "表A.54 安全文明施工二次策划报审表";
            }
            if (docType == "A.57")
            {
                return "表A.57安全问题整改回复单";
            }
            if (docType == "C.1")
            {
                return "表C.1图纸交付计划报审表";
            }
            if (docType == "C.2")
            {
                return "表C.2设计文件报检表";
            }
            if (docType == "C.3")
            {
                return "表C.3设计变更通知单";
            }
            if (docType == "C.4")
            {
                return "表C.4设计交底纪要";
            }
            if (docType == "D.3")
            {
                return "表D.3工程联系单";
            }
            if (docType == "D.4")
            {
                return "表D.4标准、规程、规范清单报审表";
            }
            return null;
        }

        /// <summary>
        /// 获取Word哈希表
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="docType"></param>
        /// <param name="fileCode"></param>
        /// <param name="title"></param>
        /// <param name="docAttrJson"></param>
        /// <returns></returns>
        internal static Hashtable GetHashtable(Doc doc, string docType, string fileCode, string title, string docAttrJson)
        {
            Hashtable htUserKeyWord = new Hashtable();
            if (docType == "A.3")
            {
                return A3.GetHashtable(doc, docType, fileCode, title, docAttrJson);
            }
            if (docType == "A.5")
            {
                return A5.GetHashtable(doc, docType, fileCode, title, docAttrJson);
            }
            if (docType == "A.6")
            {
                return A6.GetHashtable(doc, docType, fileCode, title, docAttrJson);
            }
            if (docType == "A.7")
            {
                return A7.GetHashtable(doc, docType, fileCode, title, docAttrJson);
            }
            if (docType == "A.16")
            {
                return A16.GetHashtable(doc, docType, fileCode, title, docAttrJson);
            }
            if (docType == "A.17")
            {
                return A17.GetHashtable(doc, docType, fileCode, title, docAttrJson);
            }
            if (docType == "A.18")
            {
                return A18.GetHashtable(doc, docType, fileCode, title, docAttrJson);
            }
            if (docType == "A.19")
            {
                return A19.GetHashtable(doc, docType, fileCode, title, docAttrJson);
            }
            if (docType == "A.20")
            {
                return A20.GetHashtable(doc, docType, fileCode, title, docAttrJson);
            }
            if (docType == "A.29")
            {
                return A29.GetHashtable(doc, docType, fileCode, title, docAttrJson);
            }
            if (docType == "A.30")
            {
                return A30.GetHashtable(doc, docType, fileCode, title, docAttrJson);
            }
            if (docType == "A.31")
            {
                return A31.GetHashtable(doc, docType, fileCode, title, docAttrJson);
            }
            if (docType == "A.33")
            {
                return A33.GetHashtable(doc, docType, fileCode, title, docAttrJson);
            }
            if (docType == "A.34")
            {
                return A34.GetHashtable(doc, docType, fileCode, title, docAttrJson);
            }
            if (docType == "A.37")
            {
                return A37.GetHashtable(doc, docType, fileCode, title, docAttrJson);
            }
            if (docType == "A.38")
            {
                return A38.GetHashtable(doc, docType, fileCode, title, docAttrJson);
            }
            if (docType == "A.41")
            {
                return A41.GetHashtable(doc, docType, fileCode, title, docAttrJson);
            }
            if (docType == "A.53")
            {
                return A53.GetHashtable(doc, docType, fileCode, title, docAttrJson);
            }
            if (docType == "A.54")
            {
                return A54.GetHashtable(doc, docType, fileCode, title, docAttrJson);
            }
            if (docType == "A.57")
            {
                return A57.GetHashtable(doc, docType, fileCode, title, docAttrJson);
            }
            if (docType == "C.1")
            {
                return C1.GetHashtable(doc, docType, fileCode, title, docAttrJson);
            }
            if (docType == "C.2")
            {
                return C2.GetHashtable(doc, docType, fileCode, title, docAttrJson);
            }
            if (docType == "C.3")
            {
                return C3.GetHashtable(doc, docType, fileCode, title, docAttrJson);
            }
            if (docType == "C.4")
            {
                return C4.GetHashtable(doc, docType, fileCode, title, docAttrJson);
            }
            if (docType == "D.3")
            {
                return D3.GetHashtable(doc, docType, fileCode, title, docAttrJson);
            }
            if (docType == "D.4")
            {
                return D4.GetHashtable(doc, docType, fileCode, title, docAttrJson);
            }
            return htUserKeyWord;
        }

        
        /// <summary>
        /// 获取Word表格哈希表
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="docType"></param>
        /// <param name="fileCode"></param>
        /// <param name="title"></param>
        /// <param name="docAttrJson"></param>
        /// <returns></returns>
        internal static Hashtable GetAuditHashTable(Doc doc, string docType, string fileCode, string title, string docAttrJson)
        {
            Hashtable htUserKeyWord = new Hashtable();
            if (docType == "A.5")
            {
                return A5.GetAuditHashtable(doc, docType, fileCode, title, docAttrJson);
            }
            if (docType == "A.7")
            {
                return A7.GetAuditHashtable(doc, docType, fileCode, title, docAttrJson);
            }
            if (docType == "A.30")
            {
                return A30.GetAuditHashtable(doc, docType, fileCode, title, docAttrJson);
            }
            return htUserKeyWord;
        }

        //线程锁 
        internal static Mutex muxConsole = new Mutex();

        internal static bool CreateWordDocument(Doc doc, string docType, string fileCode, string title, string docAttrJson)
        {

            Hashtable htUserKeyWord = GetHashtable(doc, docType, fileCode, title, docAttrJson);

            //用htAuditDataList传送附件列表到word
            Hashtable htAuditDataList = GetAuditHashTable(doc, docType, fileCode, title, docAttrJson);

            string docIsoType = getDocIsoType(docType, title);//获取ISO模板文件名

            string cPath = "/ISO/ZHEPC/";

            //获取从上级目录单位简码
            string curUnitCode = doc.GetValueByKeyWord("CONSTRUCTIONUNIT_CODE");
            if (!string.IsNullOrEmpty(curUnitCode))
            {
                //if (curUnitCode == "GEDI" || curUnitCode == "广东院") {
                    cPath = "/ISO/ZHEPC/EPC/";
                //}
            }

            //获取网站路径
            string sPath = System.Web.HttpContext.Current.Server.MapPath(cPath);


            //获取模板文件路径
            string modelFileName = sPath + docIsoType + ".docx";
            //获取即将生成的联系单文件路径
            string locFileName = doc.FullPathFile;

            if (System.IO.File.Exists(modelFileName))
            {
                //复制模板文件到存储目录，并覆盖同名文件
                System.IO.File.Copy(modelFileName, locFileName, true);


                //线程锁 
                muxConsole.WaitOne();
                try
                {
                    //把参数直接写进office
                    CDMSWebOffice office = new CDMSWebOffice
                    {
                        CloseApp = true,
                        VisibleApp = false
                    };
                    office.Release(true);
                    office.WriteDataToDocument(doc, locFileName, htUserKeyWord, htAuditDataList);
                }
                catch { }
                finally
                {

                    //解锁
                    muxConsole.ReleaseMutex();
                }

                ////把参数写进数据库
                ////参数格式：KEYWORD@@@VALUE@@@@KEYWORD1@@@VALUE1 
                //string strUserKeyWord = "";
                //int indexItem = 0;
                //foreach (DictionaryEntry UserKeyWordItem in htUserKeyWord)
                //{
                //    indexItem = indexItem + 1;
                //    strUserKeyWord = strUserKeyWord + UserKeyWordItem.Key + "@@@" + UserKeyWordItem.Value;
                //    if (indexItem < (htUserKeyWord.Count))
                //    {
                //        strUserKeyWord = strUserKeyWord + "@@@@";
                //    }
                //}
                //item.O_iuser4 = 1;
                //item.O_suser5 = strUserKeyWord;


                FileInfo info = new FileInfo(locFileName);
                int length = (int)info.Length;
                doc.O_size = new int?(length);
                doc.Modify();
                //if (string.IsNullOrEmpty(cp.strDoclist))
                //{
                //    cp.strDoclist = item.KeyWord;
                //}
                //else
                //{
                //    cp.strDoclist = item.KeyWord + "," + cp.strDoclist;
                //}

                //                reJo.success = true;
                //                reJo.reJo=new JObject(new JProperty("ProjectKeyword", cp.proj.KeyWord),
                //new JProperty("DocKeyword", doc.KeyWord), new JProperty("DocList", cp.strDoclist));
                return true;
            }
            return false;

        }

  
    }
}
