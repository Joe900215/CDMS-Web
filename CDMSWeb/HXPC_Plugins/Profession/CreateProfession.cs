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
    public class Profession
    {
        /// <summary>
        /// 获取专业列表
        /// </summary>
        /// <param name="sid">连接密钥</param>
        /// <returns></returns>
        public static JObject GetProfessionList(string sid)
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

                JObject joData = new JObject();
                //int dicIndex = 0;
                //从数据字典拿专业
                List<DictData> dictDataList = dbsource.GetDictDataList("PROFESSION");
                foreach (DictData data5 in dictDataList)
                {
                    //this.clbProfession.Items.Add(data5.O_Code + "__" + data5.O_Desc);
                    //dicIndex = dicIndex + 1;
                    joData.Add(new JProperty(data5.O_Code, data5.O_Code + "__" + data5.O_Desc));
                }

                //获取设计阶段列表
                JObject joDesignPhase = new JObject();
                dictDataList = dbsource.GetDictDataList("DesignPhase");
                foreach (DictData d in dictDataList)
                {
                    joDesignPhase.Add(new JProperty(d.O_Code, d.O_Code + "__" + d.O_Desc));
                }

                reJo.data = new JArray(joData, joDesignPhase);

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
        /// "创建设计阶段与选择专业..."功能，根据ProjectKeyword获取已创建的专业，以及项目阶段(选取项目阶段目录时触发)
        /// </summary>
        /// <param name="sid">连接密钥</param>
        /// <param name="ProjectKeyword">项目目录关键字</param>
        /// <returns></returns>
        public static JObject GetCreatedProfession(string sid, string ProjectKeyword)
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

                JObject joData = new JObject();

                Project m_prj = dbsource.GetProjectByKeyWord(ProjectKeyword);
                if (m_prj == null)
                {
                    reJo.msg = "获取已创建专业失败，目录不存在！";
                    return reJo.Value;
                }
                //设计阶段目录
                var p = CommonFunction.GetDesign(m_prj);
                if (p == null)
                {
                    m_prj = CommonFunction.GetProject(m_prj);
                }
                else
                {
                    m_prj = p;
                }
                if (m_prj != null && m_prj.TempDefn.Code == "DESIGNPHASE")
                {
                    joData.Add(new JProperty("DesignPhase", m_prj.ToString));

                    //获取已创建的专业
                    string sSelectedProf = "";
                    foreach (Project pp in m_prj.ChildProjectList)
                    {
                        sSelectedProf = sSelectedProf + pp.ToString + ",";
                    }
                    sSelectedProf = sSelectedProf.Substring(0, sSelectedProf.Length - 1);
                    joData.Add(new JProperty("CreatedProfession", sSelectedProf));
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
        /// "创建设计阶段与选择专业..."功能，获取项目某个阶段已创建的专业，以及项目阶段(选取项目根目录时触发)
        /// </summary>
        /// <param name="sid">连接密钥</param>
        /// <param name="ProjectKeyword">项目目录关键字</param>
        /// <param name="DesignPhaseCode">设计阶段代码</param>
        /// <returns></returns>
        public static JObject GetDesignPhaseProfession(string sid, string ProjectKeyword, string DesignPhaseCode)
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

                JObject joData = new JObject();

                //获取项目目录
                Project rootProj = dbsource.GetProjectByKeyWord(ProjectKeyword);
                if (rootProj == null)
                {
                    reJo.msg = "获取已创建专业失败，目录不存在！";
                    return reJo.Value;
                }

                //获取项目阶段目录
                Project m_prj = null;
                foreach (Project itemProj in rootProj.ChildProjectList)
                {
                    if (itemProj.ToString == DesignPhaseCode)
                    {
                        m_prj = itemProj;
                        break;
                    }
                }

                if (m_prj == null)
                {
                    reJo.msg = "获取已创建专业失败，目录不存在！";
                    return reJo.Value;
                }
                //设计阶段目录
                var p = CommonFunction.GetDesign(m_prj);
                if (p == null)
                {
                    m_prj = CommonFunction.GetProject(m_prj);
                }
                else
                {
                    m_prj = p;
                }
                if (m_prj != null && m_prj.TempDefn.Code == "DESIGNPHASE")
                {
                    joData.Add(new JProperty("DesignPhase", m_prj.ToString));

                    //获取已创建的专业
                    string sSelectedProf = "";
                    foreach (Project pp in m_prj.ChildProjectList)
                    {
                        sSelectedProf = sSelectedProf + pp.ToString + ",";
                    }
                    sSelectedProf = sSelectedProf.Substring(0, sSelectedProf.Length - 1);
                    joData.Add(new JProperty("CreatedProfession", sSelectedProf));

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
        /// 设置项目的相关专业
        /// </summary>
        /// <param name="sid">连接密钥</param>
        /// <param name="prjoectKeyword">目录关键字</param>
        /// <param name="designPhase">设计阶段</param>
        /// <param name="professionList">选取的相关专业列表，使用','分隔</param>
        /// <param name="startAtWfBtn">是否在流程按钮里面触发的本函数，可以是"true"或者"false"</param>
        /// <returns>
        /// <para>本函数在项目目录右键菜单“创建设计阶段与专业”，专业目录右键菜单“创建设计阶段与专业”，</para>
        /// <para>以及项目立项流程的设总“选择参与专业”流程按钮里面使用到</para>
        /// </returns>
        public static JObject SetProfession(string sid, string prjoectKeyword, string designPhase, string professionList,string startAtWfBtn)
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

                Project m_prj = dbsource.GetProjectByKeyWord(prjoectKeyword);

                if (m_prj == null)
                {
                    reJo.msg = "参数错误！文件夹不存在！";
                    return reJo.Value;
                }

                if (string.IsNullOrEmpty(designPhase))
                {
                    reJo.msg = "设计阶段不能为空，请选择阶段！";
                    return reJo.Value;
                }

                Project prj = null;
                if (m_prj.TempDefn.Code == "DESIGNPHASE")
                {
                    prj = m_prj;
                }
                else
                {
                    #region 创建新的设计阶段
                    //创建新的设计阶段
                    Project project = m_prj.ChildProjectList.Find(p => (p.TempDefn != null) && (p.TempDefn.Code == "DESIGNPHASE"));
                    List<TempDefn> list = dbsource.GetTempDefnByKeyWord("DESIGNPHASE");
                    TempDefn defn = ((list != null) && (list.Count > 0)) ? list[0] : null;
                    if (defn == null)
                    {
                        reJo.msg = "没有定义设计阶段模板！";
                        return reJo.Value;
                    }


                    string str = designPhase;// this.cbDesignPhase.SelectedItem.ToString();
                    bool flag = false;
                    prj = m_prj.GetProjectByName(str.Substring(0, str.IndexOf("__")));
                    if (prj == null)
                    {
                        prj = m_prj.NewProject(str.Substring(0, str.IndexOf("__")), str.Substring(str.IndexOf("__") + 2), m_prj.Storage, defn);
                    }
                    else
                    {
                        flag = true;
                    }
                    if (prj == null)
                    {
                        //MessageBox.Show("创建/获取设计阶段失败，请联系管理员！");
                        reJo.msg = "创建/获取设计阶段失败，请联系管理员！";
                        return reJo.Value;
                    }

                    //创建一个新的设计阶段时,是否把上阶段角色获取出来
                    //if (((project != null) && !flag) && (DialogResult.Yes == MessageBox.Show("您选择了一个新的设计阶段，是否从之前的设计阶段上获取设总、文秘等角色数据？", "操作确认", MessageBoxButtons.YesNo)))
                    if (((project != null) && !flag))
                    {
                        AttrData attrDataByKeyWord = prj.GetAttrDataByKeyWord("PROJECTOWNER");//设总
                        AttrData data = project.GetAttrDataByKeyWord("PROJECTOWNER");
                        if (attrDataByKeyWord != null)
                        {
                            attrDataByKeyWord.SetCodeDesc(data.ToString);
                        }

                        attrDataByKeyWord = prj.GetAttrDataByKeyWord("DOCUMENTMANAGER");//文控经理
                        data = project.GetAttrDataByKeyWord("DOCUMENTMANAGER");
                        if (attrDataByKeyWord != null)
                        {
                            attrDataByKeyWord.SetCodeDesc(data.ToString);
                        }

                        attrDataByKeyWord = prj.GetAttrDataByKeyWord("CONTROLMANAGER");//控制经理
                        data = project.GetAttrDataByKeyWord("CONTROLMANAGER");
                        if (attrDataByKeyWord != null)
                        {
                            attrDataByKeyWord.SetCodeDesc(data.ToString);
                        }

                        attrDataByKeyWord = prj.GetAttrDataByKeyWord("PURCHASMANAGER");//采购经理
                        data = project.GetAttrDataByKeyWord("PURCHASMANAGER");
                        if (attrDataByKeyWord != null)
                        {
                            attrDataByKeyWord.SetCodeDesc(data.ToString);
                        }

                        attrDataByKeyWord = prj.GetAttrDataByKeyWord("PROJECTMAINCHIEF");//项目总工程师
                        data = project.GetAttrDataByKeyWord("PROJECTMAINCHIEF");
                        if (attrDataByKeyWord != null)
                        {
                            attrDataByKeyWord.SetCodeDesc(data.ToString);
                        }

                        attrDataByKeyWord = prj.GetAttrDataByKeyWord("BUSINESSMANAGER");//商务经理
                        data = project.GetAttrDataByKeyWord("BUSINESSMANAGER");
                        if (attrDataByKeyWord != null)
                        {
                            attrDataByKeyWord.SetCodeDesc(data.ToString);
                        }

                        prj.AttrDataList.SaveData();

                    }
                    //选择一个目录, 创建一个流程对象
                    else
                    {
                        WorkFlow flow = m_prj.dBSource.NewWorkFlow(prj, "CREATEPROJECT");
                        //if (!WorkFlowEvent.GotoNextStateAndSelectUser(flow.CuWorkState.workStateBranchList[0]))
                        ExReJObject newWfReJo = WebWorkFlowEvent.GotoNextStateAndSelectUser(flow.CuWorkState.workStateBranchList[0]);//, "");
                        if (newWfReJo.success != true)
                        {
                            flow.Delete();
                            flow.Delete();
                        }
                    } 
                    #endregion
                }

                //创建收发文目录
                //if ((prj.GetProjectByName("发文") == null) && this.cbCreateFax.Checked)
                if (prj.GetProjectByName("发文") == null)
                {
                    Project project3 = prj.NewProject("发文", "", prj.Storage, null);
                }
                if (prj.GetProjectByName("收文") == null)
                {
                    Project project = prj.NewProject("收文", "", prj.Storage, null);
                }

                //查找模板
                List<TempDefn> tempDefnByKeyWord = dbsource.GetTempDefnByKeyWord("PROFESSION");
                TempDefn mTempDefn = ((tempDefnByKeyWord != null) && (tempDefnByKeyWord.Count > 0)) ? tempDefnByKeyWord[0] : null;
                if (mTempDefn == null)
                {
                    //MessageBox.Show("没有定义设计阶段模板！");
                    reJo.msg = "没有定义设计阶段模板！";
                    return reJo.Value;
                }

                //prj.dBSource.ProgramRun = true;
                string[] professionArray = (string.IsNullOrEmpty(professionList) ? "" : professionList).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                ////记录新建的Project
                //string newProjectList = "";

                JArray jaNewProjList = new JArray();
                foreach (string strProfession in professionArray)

                //foreach (object obj in this.clbProfession.CheckedItems)
                {
                    string str = strProfession;// obj.ToString();
                    string mProjectName = str.Substring(0, str.IndexOf("__"));
                    string mProjectdesc = str.Substring(str.IndexOf("__") + 2);


                    //属性为空的时候创建目录
                    if (prj.GetProjectByName(mProjectName) == null)
                    {

                        //创建专业
                        Project project = prj.NewProject(mProjectName, mProjectdesc, prj.Storage, mTempDefn);

                        //从数据字典及组织机构里面获取主任
                        User PROFESSIONOWNER = null; //主任
                        List<DictData> dictDataList = dbsource.GetDictDataList("PROFESSION");
                        foreach (DictData dd in dictDataList)
                        {
                            #region 创建单个专业
                            string profession = dd.O_sValue1.ToString();
                            if (dd.O_Code == mProjectName)
                            {
                                AVEVA.CDMS.Server.Group ProfGroup = project.dBSource.GetGroupByName(profession);
                                if (ProfGroup != null)
                                {
                                    foreach (AVEVA.CDMS.Server.Group gp in ProfGroup.ChildGroupList)
                                    {
                                        //主任
                                        if (gp.O_groupname.ToLower().EndsWith("director"))
                                        {
                                            if (gp.UserList == null || gp.UserList.Count <= 0)
                                                break;

                                            PROFESSIONOWNER = gp.UserList[0];  //取一个

                                            //启动流程
                                            WorkFlow flow = dbsource.NewWorkFlow(project, "SELECTPROFESSION");
                                            if (flow == null)
                                            {
                                                //AssistFun.PopUpPrompt("自动启动流程失败!请手动启动");
                                                reJo.msg = "自动启动流程失败!请手动启动";
                                                return reJo.Value;
                                            }
                                            else if ((flow != null) && (flow.CuWorkState != null))
                                            {
                                                if (((flow.CuWorkState == null) || (flow.CuWorkState.workStateBranchList == null)) || (flow.CuWorkState.workStateBranchList.Count <= 0))
                                                {
                                                    //MessageBox.Show("新建流程不存在下一状态,提交失败!");
                                                    //dbsource.ProgramRun = false;
                                                    flow.Delete();
                                                    flow.Delete();
                                                    reJo.msg = "新建流程不存在下一状态,提交失败!";
                                                    return reJo.Value;
                                                }
                                                WorkStateBranch wfBranch = flow.CuWorkState.workStateBranchList[0];
                                                if (wfBranch != null)
                                                {
                                                    AVEVA.CDMS.Server.Group group = new AVEVA.CDMS.Server.Group();
                                                    group.AddUser(PROFESSIONOWNER);
                                                    wfBranch.NextStateAddGroup(group);
                                                    ExReJObject reJoItem = WebWorkFlowEvent.GotoNextStateAndSelectUser(wfBranch, "");
                                                    if (reJoItem.success != true)
                                                    {
                                                        flow.Delete();
                                                        flow.Delete();
                                                    }
                                                    //if (!WorkFlowEvent.GotoNextStateAndSelectUser(wfBranch))
                                                    //{
                                                    //    this.m_dbs.ProgramRun = false;
                                                    //    flow.Delete();
                                                    //    flow.Delete();
                                                    //}
                                                }
                                            }
                                        }
                                    }
                                }

                                //保存附加属性
                                if (PROFESSIONOWNER != null) project.GetAttrDataByKeyWord("PROFESSIONMANAGER").SetCodeDesc(PROFESSIONOWNER.Code + "__" + PROFESSIONOWNER.Description);
                                project.AttrDataList.SaveData();
                            } 
                            #endregion
                        }
                    }
                }

                //当所选目录不为设计阶段模板，即新建设计阶段时，
                //或者是在流程按钮里面触发的本函数时，把流程提交到下一状态
                if ((m_prj.TempDefn.Code != "DESIGNPHASE"  || startAtWfBtn =="true") && m_prj.WorkFlow != null)
                {
                    m_prj.WorkFlow.O_suser3 = "pass";
                    m_prj.WorkFlow.Modify();
                    //把创建流程提交到下一流程状态
                    WorkStateBranch mainWfBranch = m_prj.WorkFlow.CuWorkState.workStateBranchList[0];
                    ExReJObject reJoItem = WebWorkFlowEvent.GotoNextStateAndSelectUser(mainWfBranch, "");
                }
                //刷新数据源
                AVEVA.CDMS.WebApi.DBSourceController.RefreshDBSource(sid);
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

        ///// <summary>
        ///// 创建设计阶段
        ///// </summary>
        ///// <param name="sid"></param>
        ///// <param name="PrjoectKeyword"></param>
        ///// <param name="designPhase"></param>
        ///// <returns></returns>
        //public static JObject CreateDesignPhase(string sid, string ProjectKeyword, string designPhase)
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
        //            reJo.msg = "参数错误！文件夹不存在！";
        //            return reJo.Value;
        //        }

        //        if (string.IsNullOrEmpty(designPhase))
        //        {
        //            reJo.msg = "设计阶段不能为空，请选择阶段！";
        //            return reJo.Value;
        //        }

        //        if (m_prj.TempDefn.Code == "DESIGNPROJECT")
        //        {

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
    }
}
