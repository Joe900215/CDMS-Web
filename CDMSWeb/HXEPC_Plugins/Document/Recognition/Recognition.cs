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
    public class Recognition
    {
        /// <summary>
        /// 获取创建认质认价表单的默认配置
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="ProjectKeyword"></param>
        /// <returns></returns>
        public static JObject GetDraftRecognitionDefault(string sid, string ProjectKeyword, string DraftOnProject)
        {
            return DraftGetDefault.GetDefaultInfo(sid, ProjectKeyword, DraftOnProject);
          
        }

        //线程锁 
        internal static Mutex muxConsole = new Mutex();
        public static JObject DraftRecognition(string sid,string DocKeyword, string FileName,
            string ProjectKeyword, string DocAttrJson, string CataAttrJson, string FileListJson)
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


                if (m_Project == null)
                {
                    reJo.msg = "参数错误！文件夹不存在！";
                    return reJo.Value;
                }

                #region 获取信函参数内容

                //获取信函参数内容
                string fileCode = "", projectName = "", contractCode = "",
                   sendDate = "", sendCode = "", recCode = "",
                   recType = "", materialType = "", fileId = "";

                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(DocAttrJson);

                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();

                    ////获取函件编号
                    //if (strName == "documentCode") documentCode = strValue.Trim();

                    //获取文件编码 
                    if (strName == "fileCode") fileCode = strValue.Trim();

                    //获取项目名称 
                    else if (strName == "projectName") projectName = strValue;

                    //获取合同号 
                    else if (strName == "contractCode") contractCode = strValue;

                    //获取发文日期
                    else if (strName == "sendDate") sendDate = strValue;

                    //获取发文编码
                    else if (strName == "sendCode") sendCode = strValue;

                    //获取收文编码
                    else if (strName == "recCode") recCode = strValue;

                    //获取来文类型
                    else if (strName == "recType") recType = strValue;

                    //获取物资类型
                    else if (strName == "materialType") materialType = strValue;

                    //获取物资类型
                    else if (strName == "fileId") fileId = strValue;
                }


                if (string.IsNullOrEmpty(fileCode))
                {
                    reJo.msg = "请填写文件编号！";
                    return reJo.Value;
                }
                else if (string.IsNullOrEmpty(projectName))
                {
                    reJo.msg = "请填写合同名称！";
                    return reJo.Value;
                }


                #endregion

              

                #region 根据信函模板，生成信函文档

                //获取立项单文档所在的目录
                //Project m_Project = m_NewProject;

                List<TempDefn> docTempDefnByCode = m_Project.dBSource.GetTempDefnByCode("CATALOGUING");
                TempDefn docTempDefn = (docTempDefnByCode != null && docTempDefnByCode.Count>0) ? docTempDefnByCode[0] : null;
                if (docTempDefn == null)
                {
                    reJo.msg = "没有与其相关的模板管理，创建无法正常完成";
                    return reJo.Value;
                }

                
                string docCode = fileCode + " " + projectName+"认质认价报审单";


                //文档名称
                //Doc docItem = m_Project.NewDoc("", filename, "", docTempDefn);

                Doc ddoc = dbsource.GetDocByKeyWord(DocKeyword);

                //docItem. = filename;

                if (ddoc == null)
                {
                    reJo.msg = "新建认质认价单出错！";
                    return reJo.Value;
                }
                Doc docItem = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;

                docItem.TempDefn = docTempDefn;
                docItem.O_itemname  = docCode;
                docItem.O_filename = FileName;
                docItem.Modify();

                #endregion

                #region 获取著录表属性内容
                CataloguDoc caDoc = new CataloguDoc();
                caDoc.doc = docItem;

                DocumentCommon.SetGataloguDocAttr(caDoc, CataAttrJson);

                #endregion

                #region 设置信函文档附加属性
                //文件编码
                caDoc.CA_FILECODE = fileCode;
                //工程名称
                caDoc.CA_PRONAME = projectName;
                //合同号
                caDoc.CA_CONTRACTCODE = contractCode;
                //来文编号
                caDoc.CA_RECEIPTCODE = recCode;
                //来文类型
                caDoc.CA_RECTYPE = recType;
                //采购类型
                caDoc.CA_PURCHASETYPE= materialType;
                //文件模板
                caDoc.CA_ATTRTEMP = "RECOGNITIONFILE";

                //保存项目属性，存进数据库
                caDoc.SaveAttrData();



                #endregion

                string strDocList = "";//获取附件
                if (string.IsNullOrEmpty(strDocList))
                {
                    strDocList = docItem.KeyWord;
                }
                else
                {
                    strDocList = docItem.KeyWord + "," + strDocList;
                }

                //这里刷新数据源，否则创建流程的时候获取不了专业字符串
                DBSourceController.RefreshDBSource(sid);

                reJo.success = true;
                reJo.data = new JArray(new JObject(new JProperty("ProjectKeyword", docItem.Project.KeyWord),
                    new JProperty("DocKeyword", docItem.KeyWord), new JProperty("DocList", strDocList),
                    new JProperty("DocCode", docItem.Code)));

                //reJo.data = new JArray(new JObject(new JProperty("projectKeyword", m_Project.KeyWord)));
                reJo.success = true;
                return reJo.Value;

                //AVEVA.CDMS.WebApi.DBSourceController.RefreshDBSource(sid);


            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(reJo.msg);
            }

            return reJo.Value;
        }

        /// <summary>
        /// 创建发文单后，发起发文流程
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="docKeyword"></param>
        /// <param name="DocList"></param>
        /// <returns></returns>
        public static JObject RecognitionStartWorkFlow(string sid, string docKeyword, string DocList,  string UserList)
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

                Doc ddoc = dbsource.GetDocByKeyWord(docKeyword);
                if (ddoc == null)
                {
                    reJo.msg = "错误的文档操作信息！指定的文档不存在！";
                    return reJo.Value;
                }
                Doc doc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;

                #region 获取下一状态用户
                string[] userArray = (string.IsNullOrEmpty(UserList) ? "" : UserList).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                Server.Group group = new Server.Group();
                //启动工作流程
                //反转列表
                //m_UserList.Reverse();
                foreach (string strObj in userArray)
                {
                    object obj = dbsource.GetObjectByKeyWord(strObj);

                    if (obj is User)
                    {
                        //m_UserList.Add((User)obj);
                        group.AddUser((User)obj);
                    }
                }
                if (group.UserList.Count <= 0)
                {
                    reJo.msg = "获取下一流程状态用户错误，自动启动流程失败！请手动启动流程";
                    return reJo.Value;
                }
                #endregion


                {


                    #region 获取文档列表
                    string[] strArray = (string.IsNullOrEmpty(DocList) ? "" : DocList).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    List<Doc> m_DocList = new List<Doc>();
                    //启动工作流程
                    m_DocList.Reverse();
                    foreach (string strObj in strArray)
                    {
                        object obj = dbsource.GetObjectByKeyWord(strObj);

                        if (obj is Doc)
                        {
                            Doc mddoc = (Doc)obj;
                            Doc mdoc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;
                            m_DocList.Add(mdoc);
                        }
                    }
                    #endregion

                    WorkFlow flow = dbsource.NewWorkFlow(m_DocList, "RECOGNITION");
                    //if (flow == null || flow.CuWorkState == null || flow.CuWorkState.workStateBranchList == null || (flow.CuWorkState.workStateBranchList.Count <= 0))
                    if (flow == null)
                    {
                        reJo.msg = "自动启动流程失败!请手动启动";
                        return reJo.Value;
                    }



                    //放置项目经理
                    WorkState state = new WorkState();

                    Project rootProj = CommonFunction.getParentProjectByTempDefn(doc.Project, "HXNY_DOCUMENTSYSTEM");
                    string strman = rootProj.GetAttrDataByKeyWord("DESIGNDIRECTOR").ToString;

                    Server.Group gpMan = CommonFunction.GetGroupByFullName(dbsource,strman);
                    if (gpMan != null)
                    {
                        //放置项目经理
                        state = flow.WorkStateList.Find(wsx => (wsx.Code == "PROAPPROV") && (wsx.CheckGroup.AllUserList.Count == 0));
                        if (state == null)
                        {
                            DefWorkState defWorkState = flow.DefWorkFlow.DefWorkStateList.Find(dwsx => dwsx.O_Code == "PROAPPROV");
                            state = flow.NewWorkState(defWorkState);
                            state.SaveSelectUser(gpMan);

                            state.IsRuning = false;

                            state.PreWorkState = flow.CuWorkState;
                            state.O_iuser5 = new int?(flow.CuWorkState.O_stateno);
                            state.Modify();
                        }
                    }

                    #region 放置财务部部长
                    //获取财务部部长
                    Server.Group gpFine = CommonFunction.GetGroupByDesc(dbsource, "FINE", "部长");
                    //CommonFunction.GetDepartmentGroup(dbsource, "FINE", "部长");
                    if (gpFine != null)
                    {
                        //放置财务部部长
                        state = flow.WorkStateList.Find(wsx => (wsx.Code == "FINE") && (wsx.CheckGroup.AllUserList.Count == 0));
                        if (state == null)
                        {
                            DefWorkState defWorkState = flow.DefWorkFlow.DefWorkStateList.Find(dwsx => dwsx.O_Code == "FINE");
                            state = flow.NewWorkState(defWorkState);
                            state.SaveSelectUser(gpFine);

                            state.IsRuning = false;

                            state.PreWorkState = flow.CuWorkState;
                            state.O_iuser5 = new int?(flow.CuWorkState.O_stateno);
                            state.Modify();
                        }
                    }
                    #endregion


                    #region 放置成本控制部部长
                    //获取成本控制部部长
                    Server.Group gpCoce =  CommonFunction.GetGroupByDesc(dbsource, "COCE", "部长");
                        //CommonFunction.GetDepartmentGroup(dbsource, "COCE", "部长");
                    if (gpCoce != null)
                    {
                        //放置成本控制部部长
                        state = flow.WorkStateList.Find(wsx => (wsx.Code == "DIRLEADER") && (wsx.CheckGroup.AllUserList.Count == 0));
                        if (state == null)
                        {
                            DefWorkState defWorkState = flow.DefWorkFlow.DefWorkStateList.Find(dwsx => dwsx.O_Code == "DIRLEADER");
                            state = flow.NewWorkState(defWorkState);
                            state.SaveSelectUser(gpCoce);

                            state.IsRuning = false;

                            state.PreWorkState = flow.CuWorkState;
                            state.O_iuser5 = new int?(flow.CuWorkState.O_stateno);
                            state.Modify();
                        }
                    }
                    #endregion

                    #region 放置成本控制部部长
                    //获取招标部部长
                    User userZbb = dbsource.GetUserByName("yangmingyue");
                    Server.Group gpZbb = new Server.Group();
                    if (userZbb != null)
                    {
                        gpZbb.AddUser(userZbb);

                        //放置招标部部长
                        state = flow.WorkStateList.Find(wsx => (wsx.Code == "ZTBB") && (wsx.CheckGroup.AllUserList.Count == 0));
                        if (state == null)
                        {
                            DefWorkState defWorkState = flow.DefWorkFlow.DefWorkStateList.Find(dwsx => dwsx.O_Code == "ZTBB");
                            state = flow.NewWorkState(defWorkState);
                            state.SaveSelectUser(gpZbb);

                            state.IsRuning = false;

                            state.PreWorkState = flow.CuWorkState;
                            state.O_iuser5 = new int?(flow.CuWorkState.O_stateno);
                            state.Modify();
                        }
                    } 
                    #endregion

                    //获取下一状态

                    WorkState ws = new WorkState();

                    DefWorkState dws = flow.DefWorkFlow.DefWorkStateList.Find(s => s.KeyWord == "APPROV");
                    ws.DefWorkState = dws;


                    ////启动流程
                    WorkStateBranch branch = flow.CuWorkState.workStateBranchList[0];
                    branch.NextStateAddGroup(group);

                    ExReJObject GotoNextReJo = WebWorkFlowEvent.GotoNextStateAndSelectUser(flow.CuWorkState.workStateBranchList[0]);

                    if (!GotoNextReJo.success)
                    {
                        //  doc.dBSource.ProgramRun = false;
                        flow.Delete();
                        flow.Delete();

                        reJo.msg = "自动启动流程失败！请手动启动流程";
                        return reJo.Value;
                    }


                    DBSourceController.RefreshDBSource(sid);

                    return GotoNextReJo.Value;

                }

            }
            catch (Exception exception)
            {
                WebApi.CommonController.WebWriteLog(exception.Message + "\r\n" + exception.Source + "\r\n" + exception.StackTrace);
                reJo.msg = "启动流程失败！" + exception.Message + "\r\n" + exception.Source + "\r\n" + exception.StackTrace;
            }
            return reJo.Value;

        }

        /// <summary>
        /// 认质认价流程添加手写校审意见单附件到流程
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="DocKeyword"></param>
        /// <param name="AttaDocKeyword"></param>
        /// <returns></returns>
        public static JObject AddRecognitionAtta(string sid, string DocKeyword, string AttaDocKeyword) {
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

                Doc origDoc = dbsource.GetDocByKeyWord(DocKeyword);
                if (origDoc == null)
                {
                    reJo.msg = "错误的文档操作信息！指定的文档不存在！";
                    return reJo.Value;
                }

                Doc ddoc = dbsource.GetDocByKeyWord(AttaDocKeyword);
                if (ddoc == null)
                {
                    reJo.msg = "错误的文档操作信息！指定的文档不存在！";
                    return reJo.Value;
                }
                Doc doc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;

                origDoc.WorkFlow.DocList.Add(doc);
                origDoc.WorkFlow.CuWorkState.O_suser3 = "isAddAtta";
                origDoc.WorkFlow.Modify();

                doc.WorkFlow = origDoc.WorkFlow;
                doc.Modify();

                #region 复制到收文目录

                //管理员身份登录创建快捷方式
                DBSource adminDBSource = DBSourceController.dbManager.DBSourceList[0];

                //运营管理类文件目录
                Project rootProj = adminDBSource.RootLocalProjectList.Find(itemProj => itemProj.TempDefn.KeyWord == "OPERATEADMIN");

                //流程管理目录
                Project storaProj = CommonFunction.GetProjectByDesc(rootProj, "流程管理");
                //认质认价目录
                Project recProj = CommonFunction.GetProjectByDesc(storaProj, "认质认价");

                recProj.NewDoc(doc);

       
                #endregion

                //查找提交确认分支
                WorkStateBranch branch = origDoc.WorkFlow.CuWorkState.workStateBranchList.Find(wsb => wsb.defStateBrach.O_Code == "TOCONFIREMAN");

                ExReJObject GotoNextReJo2 = WebWorkFlowEvent.GotoNextStateAndSelectUser(branch);

                DBSourceController.RefreshDBSource(sid);

                reJo.success = true;
                return reJo.Value;
            }
            catch (Exception exception)
            {
                //WebApi.CommonController.WebWriteLog(exception.Message + "\r\n" + exception.Source + "\r\n" + exception.StackTrace);
                //reJo.msg = "启动流程失败！" + exception.Message + "\r\n" + exception.Source + "\r\n" + exception.StackTrace;
                reJo.msg = "添加手写校审意见单附件失败！";
            }
            return reJo.Value;
        }

            

        //定义信函附件结构体
        internal struct Material
        {
            // 文件序号
            public string No { get; set; }
            // 材料设备 名称
            public string MatName { get; set; }

            //规格
            public string Spec { get; set; }

            //计量单位 
            public string MeaUnit { get; set; }

            //设计图号 
            public string DesignNum { get; set; }

            //品牌
            public string Brand { get; set; }


            //报审数量
            public string Quantity { get; set; }

            //意见 
            public string Audit { get; set; }

            //报审价格
            public string Price { get; set; }
            //价格
            public string CostPrice { get; set; }
            //价格
            public string CenterPrice { get; set; }
            //价格
            public string TenderPrice { get; set; }

            //审核合价
            public string AuditPrice { get; set; }
            //备注
            public string Remark { get; set; }
        }
    }
}
