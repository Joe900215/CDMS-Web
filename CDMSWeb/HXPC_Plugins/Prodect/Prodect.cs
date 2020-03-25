using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class Prodect
    {
        /// <summary>
        /// 创建成品目录
        /// </summary>
        /// <param name="sid">连接密钥</param>
        /// <param name="ProjectKeyword">父目录关键字</param>
        /// <param name="projectAttrJson">参数关键字，详见下面例子</param>
        /// <returns>
        /// <para>projectAttrJson参数例子：</para>
        /// <code>
        /// [
        ///    { name: 'projectCode', value: projectCode }, //成品编码
        ///    { name: 'projectDesc', value: projectDesc }  //成品描述
        /// ];
        /// </code>
        /// </returns>
        public static JObject CreateProdect(string sid, string ProjectKeyword, string projectAttrJson)
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

                //获取项目参数项目
                string projectCode = "", projectDesc = "";

                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();

                    //获取成品目录代码
                    if (strName == "projectCode") projectCode = strValue;

                    //获取成品目录描述
                    else if (strName == "projectDesc") projectDesc = strValue;
                }

                if (string.IsNullOrEmpty(projectCode))
                {
                    reJo.msg = "请填成品目录代码！";
                    return reJo.Value;
                }
                else if (string.IsNullOrEmpty(projectDesc))
                {
                    reJo.msg = "请填写成品目录描述！";
                    return reJo.Value;
                }

                Project m_prj = dbsource.GetProjectByKeyWord(ProjectKeyword);
                if (m_prj == null)
                {
                    reJo.msg = "参数错误，目录不存在！";
                    return reJo.Value;
                }

                if (m_prj.ChildProjectList.Find(px => px.Code == projectCode) != null)
                {
                    //MessageBox.Show("已经存在相同版本的目录，请返回重试！");
                    reJo.msg = "已经存在相同版本的目录，请返回重试！";
                    return reJo.Value;
                }
                else
                {
                    TempDefn mTempDefn = CommonFunction.GetTempDefn(m_prj, "PRODUCTVOLUMN");
                    Project project = m_prj.NewProject(projectCode, projectDesc, m_prj.Storage, mTempDefn);
                    if (project == null)
                    {
                        //MessageBox.Show("新建版本目录失败，请联系管理员！");
                        reJo.msg = "新建版本目录失败，请联系管理员！";
                        return reJo.Value;
                    }

                    reJo.data = new JArray(new JObject(new JProperty("ProjectKeyword", project.KeyWord)));
                    reJo.success = true;
                    return reJo.Value;

                    //project.AttrDataList.SaveData();                 

                    //base.Close();
                    //if (ExMenu.callTheApp != null)
                    //{
                    //    CallBackParam param = new CallBackParam
                    //    {
                    //        callType = enCallBackType.UpdateDBSource,
                    //        dbs = this.m_dbs
                    //    };
                    //    CallBackResult result = null;
                    //    ExMenu.callTheApp(param, out result);
                    //}

                }
            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(reJo.msg);
            }

            return reJo.Value;
        }

        /// <summary>
        /// 获取“成品校审会签专业”表单默认参数
        /// </summary>
        /// <param name="sid">连接密钥</param>
        /// <param name="DocKeyword">成品文档关键字</param>
        /// <returns></returns>
        public static JObject GetProductSignDefault(string sid, string DocKeyword) {
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

                Doc doc = dbsource.GetDocByKeyWord(DocKeyword);
                if (doc==null)
                {
                    reJo.msg = "文档不存在！";
                    return reJo.Value;
                }

                Project proj = doc.Project;

                //获取设计阶段目录
                Project projDesignPhase = GetProjectByTempDefn(proj, "DESIGNPHASE");

                //获取本专业目录
                Project projProfession = GetProjectByTempDefn(proj, "PROFESSION");

                ///找设计阶段
                //Project p = doc.Project;
                //while (p != null && p.ParentProject != null)
                //{
                //    if (p != null && p.TempDefn != null && p.TempDefn.KeyWord == "DESIGNPHASE")
                //    {
                //        break;
                //    }
                //    p = p.ParentProject;
                //}

                JObject joData = new JObject();
                JArray jaPressionList = new JArray();

                //查找专业列表
                foreach (Project pp in projDesignPhase.ChildProjectList)
                {
                    AttrData attrDataByKeyWord = pp.GetAttrDataByKeyWord("PROFESSIONOWNER");
                    if (pp.TempDefn != null && pp.TempDefn.KeyWord == "PROFESSION" && pp != projProfession && attrDataByKeyWord != null)
                    {
                        jaPressionList.Add(new JObject(
                            new JProperty("ProjectKeyword", pp.KeyWord),//目录关键字
                            new JProperty("Pression", pp.ToString),//添加专业
                            new JProperty("OwnerUser", attrDataByKeyWord.ToString)//添加主设人
                            ));
                        //pList.Add(pp);
                        //this.clbProfession.Items.Add(pp.ToString);
                        //joData.Add(new JProperty(pp.KeyWord, pp.ToString));
                    }
                }

                joData.Add(new JProperty("PressionList", jaPressionList));

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
        /// 设置成品会签专业
        /// </summary>
        /// <param name="sid">连接密钥</param>
        /// <param name="DocKeyword">成品文档关键字</param>
        /// <param name="Receiver">会签人员关键字列表，使用','分隔</param>
        /// <returns></returns>
        public static JObject SetProdectSignProfession(string sid, string DocKeyword, string Receiver)
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

                Doc doc = dbsource.GetDocByKeyWord(DocKeyword);
                if (doc == null)
                {
                    reJo.msg = "文档不存在！";
                    return reJo.Value;
                }

                AVEVA.CDMS.Server.Group g_gCounterSignGroup = new AVEVA.CDMS.Server.Group();

                //如果有会签人员，就会签，否则就是不需要会签
                if (!string.IsNullOrEmpty(Receiver))
                {
                    foreach (string str in Receiver.Split(new char[] { ',' }))
                    {
                        User user = dbsource.GetUserByCode(str.Substring(0, str.IndexOf("_")));
                        if ((user != null) && !g_gCounterSignGroup.ContainUser(user))
                        {
                            g_gCounterSignGroup.AddUser(user);
                        }
                    }


                    AttrData ad = doc.GetAttrDataByKeyWord("MORESIGNER");
                    if ((ad != null) && (g_gCounterSignGroup.AllUserList.Count > 0))
                    {
                        ad.SetCodeDesc(g_gCounterSignGroup.UserListToString);
                    }

                    AVEVA.CDMS.Server.Group userGroup = new AVEVA.CDMS.Server.Group();
                    userGroup.AddUser(dbsource.LoginUser);
                    ad = doc.GetAttrDataByKeyWord("PROMANAGER");
                    if ((ad != null) && (userGroup.AllUserList.Count > 0))
                    {
                        ad.SetCodeDesc(userGroup.UserListToString);
                    }

                    doc.AttrDataList.SaveData();

                }

                //创建流程对象
                WorkFlow flow = dbsource.NewWorkFlow(new List<Doc> { doc }, "PRODUCT");

                //设置流程为已经选择了专业
                flow.O_suser3 = "pass";
                flow.Modify();

                //if (!WorkFlowEvent.GotoNextStateAndSelectUser(flow.CuWorkState.workStateBranchList[0]))
                ExReJObject newWfReJo = WebWorkFlowEvent.GotoNextStateAndSelectUser(flow.CuWorkState.workStateBranchList[0]);//, "");
                if (newWfReJo.success != true)
                {
                    flow.Delete();
                    flow.Delete();

                    reJo.msg = "启动流程失败！";
                    return reJo.Value;

                }
                else
                {

                    if (!string.IsNullOrEmpty(Receiver))
                    {
                        //建立会签流程状态并放置用户
                        DefWorkState stateSign = flow.DefWorkFlow.DefWorkStateList.Find(dwsx => dwsx.O_Code == "MORESIGN");
                        flow.NewWorkState(stateSign).SaveSelectUser(g_gCounterSignGroup);
                        flow.Modify();
                    }
                    //刷新数据源
                    AVEVA.CDMS.WebApi.DBSourceController.RefreshDBSource(sid);
                    reJo.success = true;
                    reJo.data = new JArray(new JObject(new JProperty("ProjectKeyword", doc.Project.KeyWord),
                        new JProperty("DocKeyword", doc.KeyWord)));
                    return reJo.Value;
                }
                //base.Close();


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
        /// 根据模板查找文件夹
        /// </summary>
        /// <param name="project"></param>
        /// <param name="tempDefnCode"></param>
        /// <returns></returns>
        private static Project GetProjectByTempDefn(Project project, string tempDefnCode)
        {
            Project pProj = null;
            pProj = project;
            while (true)
            {

                //把本级目录也查询是否对应模板
                if (pProj.TempDefn.Code == tempDefnCode)
                {
                    return pProj;
                }

                if (pProj.ParentProject == null)
                {
                    return null;
                }

                pProj = pProj.ParentProject;

            }

            return pProj;
        }
    }
}
