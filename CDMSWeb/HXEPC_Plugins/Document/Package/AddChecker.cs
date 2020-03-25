using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Linq;
using System.IO;
using AVEVA.CDMS.Server;
using AVEVA.CDMS.WebApi;
using System.Text.RegularExpressions;
using System.Threading;
//using LinqToDB;
using Newtonsoft.Json.Linq;

namespace AVEVA.CDMS.HXEPC_Plugins
{
    public class AddChecker
    {

        public static JObject AddCheckerAndGotoNextState(string sid, string DocKeyword, 
            string WorkStateBranchCode,string NextStateKeyword,string UserList, string IfAddUser)
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

                Doc ddoc = dbsource.GetDocByKeyWord(DocKeyword);

                if (ddoc == null)
                {
                    reJo.msg = "参数错误！流程文档不存在！";
                    return reJo.Value;
                }
                Doc m_doc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;

                WorkFlow wf = m_doc.WorkFlow;

                if (wf == null) {
                    reJo.msg = "参数错误！流程不存在！";
                    return reJo.Value;
                }

                WorkState nextWs = wf.WorkStateList.Find(ws => ws.KeyWord == NextStateKeyword);
                if (nextWs == null)
                {
                    reJo.msg = "参数错误！下一流程状态不存在！";
                    return reJo.Value;
                }

                WorkStateBranch branch = wf.CuWorkState.workStateBranchList.Find(wsb => wsb.defStateBrach.O_Code == WorkStateBranchCode);
                if (nextWs == null)
                {
                    reJo.msg = "参数错误！流程分支不存在！";
                    return reJo.Value;
                }

                #region 添加用户到下一流程状态
                if (IfAddUser == "true")
                {
                    string[] strArry = UserList.Split(new char[] { ',' });
                    Server.Group CheckerGroup = new Server.Group();

                    foreach (string op in strArry)
                    {
                        User CheckerUser = dbsource.GetUserByKeyWord(op);
                        if (CheckerUser != null)
                        {
                            CheckerGroup.AddUser(CheckerUser);
                            //不存在该用户才允许添加
                            bool IsExist = false;
                            foreach (WorkUser temp in nextWs.WorkUserList)
                            {
                                if (temp.User == CheckerUser)
                                {
                                    IsExist = true;
                                    break;
                                }
                            }
                            if (!IsExist)
                            {
                                nextWs.AfterAddUser(CheckerUser);
                            }
                        }
                    }

                }
                #endregion

                wf.CuWorkState.O_suser3 = "addCheckerPass";
                wf.Modify();

                ExReJObject GotoNextReJo = WebWorkFlowEvent.GotoNextStateAndSelectUser(branch);


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
