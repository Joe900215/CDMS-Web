﻿using System;
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
    public class SelectProfession
    {
        public static JObject GetSelectProfessionList(string sid, string ProjectKeyword, string page, string Filter)
        {
            ExReJObject reJo = new ExReJObject();

            try
            {
                page = page ?? "1";
                string limit = "50";
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

                //Project prjProject = CommonFunction.getParentProjectByTempDefn(m_prj, "HXNY_DOCUMENTSYSTEM");

                //if (prjProject == null)
                //{
                //    reJo.msg = "获取项目目录失败！";
                //    return reJo.Value;
                //}

                Filter = Filter.Trim().ToLower();

                //string curProfessionCode = "";

                //curProfessionCode = prjProject.Code;//.GetValueByKeyWord("PRO_COMPANY");
                //if (string.IsNullOrEmpty(curProfessionCode))
                //{
                //    reJo.msg = "获取项目来源失败！";
                //    return reJo.Value;
                //}

                JArray jaData = new JArray();

                #region 添加机组
                //获取所有参建单位
                List<DictData> dictDataList = dbsource.GetDictDataList("Profession");
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

                    //if (data6.O_sValue2 != curProfessionCode)
                    //{
                    //    continue;
                    //}
                    resultDDList.Add(data6);
                }

                #endregion


                reJo.total = resultDDList.Count();
                int ShowNum = 50;

                List<DictData> resDDList = resultDDList.Skip(CurPage * ShowNum).Take(ShowNum).ToList();

                foreach (DictData data6 in resDDList)
                {
                    {
                        JObject joData = new JObject(
                            new JProperty("professionType", "专业"),
                            new JProperty("professionId", data6.O_ID.ToString()),
                            new JProperty("professionCode", data6.O_Code),
                            new JProperty("professionDesc", data6.O_Desc)
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

        //public static JObject GetSelectProfessionList(string sid, string ProjectKeyword, string Filter)
        //{
        //    ExReJObject reJo = new ExReJObject();

        //    try
        //    {

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

        //        Project m_prj = dbsource.GetProjectByKeyWord(ProjectKeyword);
        //        if (m_prj == null)
        //        {
        //            reJo.msg = "参数错误，目录不存在！";
        //            return reJo.Value;
        //        }


        //        Filter = Filter.Trim().ToLower();

        //        JArray jaData = new JArray();
        //        //获取所有参建单位
        //        List<DictData> dictDataList = dbsource.GetDictDataList("Profession");

        //        ////按代码排序
        //        //dictDataList.Sort(delegate (DictData x, DictData y)
        //        //{
        //        //    return x.O_Code.CompareTo(y.O_Code);
        //        //});

        //        foreach (DictData data6 in dictDataList)
        //        {
        //            //判断是否符合过滤条件
        //            if (!string.IsNullOrEmpty(Filter) && 
        //                data6.O_Code.ToLower().IndexOf(Filter)<0 && data6.O_Desc.ToLower().IndexOf(Filter) < 0) {
        //                continue;
        //            }

        //            //if (data6.O_sValue1 == curProfessionCode)
        //            {
        //                JObject joData = new JObject(
        //                    new JProperty("professionType", "专业"),
        //                    new JProperty("professionId", data6.O_ID.ToString()),
        //                    new JProperty("professionCode", data6.O_Code),
        //                    new JProperty("professionDesc", data6.O_Desc)
        //                    );
        //                jaData.Add(joData);
        //            }
        //        }

        //        reJo.data = jaData;
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
    }
}
