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
    public class RevokeWorkflow
    {

        public static JObject DeleteWorkFlowAndDoc(string sid, string DocKeyword) {
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

                Doc ddoc = dbsource.GetDocByKeyWord(DocKeyword);
                if (ddoc == null)
                {
                    reJo.msg = "错误的文档操作信息！指定的文档不存在！";
                    return reJo.Value;
                }
                Doc doc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;
                WorkFlow flow;
                if ((flow = doc.WorkFlow) == null)
                {
                    reJo.msg = "错误的文档操作信息！指定的文档流程不存在！";
                    return reJo.Value;
                }

                bool RevokeRight = CheckRevokeRight(flow);
                if (!RevokeRight) {
                    reJo.msg = "错误的文档操作信息！用户没有回撤流程权限！";
                    return reJo.Value;
                }

                List<Doc> docList = new List<Doc>();
                foreach (Doc d in flow.DocList) {
                    docList.Add(d);
                }
                string wfKeyword = flow.KeyWord;

                flow.Delete();
                flow.Delete();
               
                DBSourceController.RefreshDBSource(sid);

                string ProjectKeyword = doc.Project.KeyWord;
                foreach (Doc docItem in docList)
                {

                    List<WorkFlow> wflist = docItem.WorkFlowList;
                    if (wflist == null || wflist.Count <= 0 || (wflist.Count==1 && wflist[0].KeyWord==wfKeyword))
                    {
                        DocController.DeleteDoc(sid, ProjectKeyword, docItem.KeyWord, "true");
                    }
                }

                reJo.success = true;
                return reJo.Value;
            }
            catch (Exception exception)
            {
                //WebApi.CommonController.WebWriteLog(exception.Message + "\r\n" + exception.Source + "\r\n" + exception.StackTrace);
                reJo.msg = "撤回流程失败！" + exception.Message;
            }
            return reJo.Value;
        }

        /// <summary>
        /// 判断回撤流程的权限
        /// </summary>
        /// <param name="wf"></param>
        /// <returns></returns>
        public static bool CheckRevokeRight(WorkFlow wf) {
            WorkState cuWorkState = wf.CuWorkState;
            User cuUser = wf.dBSource.LoginUser;

            #region 判断是否编制人
            WorkState wsDesign = wf.WorkStateList.Find(ws => ws.DefWorkState.O_Code == "DESIGN");

            bool isDesigner = false;
            foreach (WorkUser wu in wsDesign.WorkUserList)
            {
                if (wu.User == cuUser)
                {
                    isDesigner = true;
                }
            }

            if (!isDesigner)
            {
                return false;
            }

            #endregion

            string cuWsCode = cuWorkState.DefWorkState.O_Code;

            if (wf.DefWorkFlow.O_Code != "COMMUNICATIONWORKFLOW")
            {
                return false;
            }

            //判断流程是否已经通过批准
            if (cuWsCode != "SECRETARIL" && cuWsCode != "CHECK" && cuWsCode != "AUDIT" &&
                cuWsCode != "AUDIT2" && cuWsCode != "APPROV")
            {
                return false;
            }

            return true;
        }
    
    }
}
