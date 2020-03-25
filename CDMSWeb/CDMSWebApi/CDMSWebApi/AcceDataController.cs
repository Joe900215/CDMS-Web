using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AVEVA.CDMS.Server;
using System.Runtime.Serialization;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;


namespace AVEVA.CDMS.WebApi
{
    //目录或者文档的权限
    public class AcceDataController : Controller
    {
        /// <summary>
        /// 获取目录的用户和用户组权限列表
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="ProjectKeyword">目录Keyword或者DocKeyword</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>例子：</para>
        /// <code>
        /// {
        ///  "success": true,
        ///  "total": 0,
        ///  "msg": "",
        ///  "data": []
        ///}
        ///</code>
        /// </returns>
        internal static JObject GetObjectRightList(string sid, string Keyword)
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
                object obj = dbsource.GetObjectByKeyWord(Keyword);
                if (obj == null)
                {
                    reJo.msg = "目录或文档不存在！";
                    return reJo.Value;
                }


                //权限对象
                Doc doc = null;
                Project project = null;
                if (obj is Doc)
                {
                    Doc ddoc = (Doc)obj;
                    doc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;
                }
                else if (obj is Project)
                {
                    project = (Project)obj;
                }
                List<Acce> acceDataList = new List<Acce>();
                if (project != null)
                {
                    if (project.acceData == null || project.acceData.Count <= 0)
                    {
                        acceDataList = project.ParentAcceData;
                    }
                    else
                    {
                        acceDataList = project.acceData;
                    }
                }
                else if (doc != null)
                {
                    if (doc.acceData == null || doc.acceData.Count <= 0)
                    {
                        acceDataList = doc.ParentAcceData;
                    }
                    else
                    {
                        acceDataList = doc.acceData;
                    }
                }


                JArray jaAcceDataList = new JArray();


                //组成权限组
                foreach (Acce acce in acceDataList)     //查找出所有的用户和用户组
                {
                    string iID = "";
                    string UserName = "";
                    bool bIsUser = false;
                    int iMask = 0;

                    if (acce.O_memtype == enUserGroupMeberType.User)   //判断用户
                    {
                        bIsUser = true;
                        iID = acce.user.KeyWord;
                        UserName = acce.user.ToString;
                        iMask = acce.O_mask;
                    }
                    else if (acce.O_memtype == enUserGroupMeberType.UserGroup) //判断用户组
                    {
                        bIsUser = false;
                        iID = acce.group.KeyWord;
                        UserName = acce.group.ToString;
                        iMask = acce.O_mask;
                    }

                    Right right = new Right(iMask);



                    //判断权限是否在当前文档获得
                    bool Enable = false;
                    if (doc != null && acce.O_objno2 == doc.O_itemno)
                        Enable = true;
                    if (project != null && acce.O_objno == project.O_projectno)
                        Enable = true;

                    //权限所在目录
                    string acceObj = "";
                    if (Enable == false)
                    {
                        acceObj = dbsource.GetProjectByID(acce.O_objno).ToString;
                    }
                    else {
                        if (acce.O_WorkFlowno != 0) {
                            WorkFlow wf= dbsource.GetWorkFlowByID(acce.O_WorkFlowno);
                            if (wf != null) {
                                acceObj = wf.DefWorkFlow.O_Description;
                            }
                        }
                    }

                    jaAcceDataList.Add(new JObject(new JProperty("IsUser", bIsUser), new JProperty("UserName", UserName), new JProperty("ObjectKeyWord", iID),
                        new JProperty("PFull", right.PFull), new JProperty("PCreate", right.PCreate),
                        new JProperty("PRead", right.PRead), new JProperty("PWrite", right.PWrite), new JProperty("PDelete", right.PDelete),
                        new JProperty("PCntrl", right.PCntrl), new JProperty("PNone", right.PNone),
                        new JProperty("DFull", right.DFull), new JProperty("DCreate", right.DCreate),
                        new JProperty("DRead", right.DRead), new JProperty("DWrite", right.DWrite), new JProperty("DDelete", right.DDelete),
                        new JProperty("DFRead", right.DFRead), new JProperty("DFWrite", right.DFWrite), new JProperty("DCntrl", right.DCntrl),
                        new JProperty("DNone", right.DNone), new JProperty("AcceObj", acceObj),
                        new JProperty("Enable", Enable), new JProperty("Visible", true)
                        ));
                }



                //登录用户是否有权限编辑
                bool hasPCntrlRight = false;
                if (project != null)
                {
                    hasPCntrlRight = ProjectController.GetProjectPCntrlRight(project, curUser);
                }
                else
                {
                    hasPCntrlRight = DocController.GetDocDCntrlRight(doc, curUser);
                }

                jaAcceDataList.Add(new JObject(new JProperty("IsUser", true), new JProperty("UserName", "LoginUser"), new JProperty("UserKeyWord", curUser.KeyWord),
                    new JProperty("PCntrl", hasPCntrlRight), new JProperty("Visible", false)
                    ));


                reJo.data = jaAcceDataList;
                reJo.success = true;

            }
            catch (Exception ex)
            {
                reJo.msg = "运行错误！错误信息：" + ex.Message;
                CommonController.WebWriteLog(ex.Message);
            }
            return reJo.Value;
        }



        internal static JObject SetObjectRightList(string sid, string ObjectKeyword, string rightAttrJson)
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
                object curobj = dbsource.GetObjectByKeyWord(ObjectKeyword);
                if (curobj == null)
                {
                    reJo.msg = "目录或文档不存在！";
                    return reJo.Value;
                }


                //权限对象
                //获取所有需要修改权限的目录和文档
                List<GDialogItemData> GDialogItemDataList = new List<GDialogItemData>();

                //把所选的文档或目录对象，添加到需要修改权限的文档或目录对象列表

                Doc curdoc = null;
                Project curproject = null;
                if (curobj is Doc)
                {
                    Doc ddoc = (Doc)curobj;
                    curdoc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;
                    GDialogItemData curgdid = new GDialogItemData();
                    curgdid.bIsProject = false;
                    curgdid.Doc = curdoc;
                    GDialogItemDataList.Add(curgdid);
                }
                else if (curobj is Project)
                {
                    curproject = (Project)curobj;
                    GDialogItemData curgdid = new GDialogItemData();
                    curgdid.bIsProject = true;
                    curgdid.Project = curproject;
                    GDialogItemDataList.Add(curgdid);
                }
                else
                {
                    reJo.msg = "目录或文档不存在！";
                    return reJo.Value;
                }

                List<acceItem> acceList = new List<acceItem>();
                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(rightAttrJson);


                foreach (JObject joAttr in jaAttr)
                {

                    acceItem ai = new acceItem();
                    ai.ObjectKeyWord = joAttr["ObjectKeyword"].ToString();

                    ai.PFull = joAttr["PFull"].ToString();
                    ai.PCreate = joAttr["PCreate"].ToString();
                    ai.PRead = joAttr["PRead"].ToString();
                    ai.PWrite = joAttr["PWrite"].ToString();
                    ai.PDelete = joAttr["PDelete"].ToString();
                    ai.PCntrl = joAttr["PCntrl"].ToString();
                    ai.PNone = joAttr["PNone"].ToString();

                    ai.DFull = joAttr["DFull"].ToString();
                    ai.DCreate = joAttr["DCreate"].ToString();
                    ai.DRead = joAttr["DRead"].ToString();
                    ai.DWrite = joAttr["DWrite"].ToString();
                    ai.DDelete = joAttr["DDelete"].ToString();
                    ai.DFRead = joAttr["DFRead"].ToString();
                    ai.DFWrite = joAttr["DFWrite"].ToString();
                    ai.DCntrl = joAttr["DCntrl"].ToString();
                    ai.DNone = joAttr["DNone"].ToString();

                    Server.Group agroup = null;
                    User auser = null;

                    object acceobj = dbsource.GetObjectByKeyWord(ai.ObjectKeyWord);
                    if (acceobj == null)
                    {
                        continue;
                    }
                    if (acceobj is User)
                    {
                        auser = (User)acceobj;
                        ai.IsUser = true;
                        ai.User = auser;
                    }
                    else if (acceobj is Server.Group)
                    {
                        agroup = (Server.Group)acceobj;
                        ai.IsUser = false;
                        ai.Group = agroup;
                    }
                    acceList.Add(ai);
                }




                //获取所有要修改的用户或用户组的权限
                List<GRightPageItemData> gRightPageItemDataList = new List<GRightPageItemData>();

                foreach (acceItem ai in acceList) {
                    GRightPageItemData grpid = new GRightPageItemData();
                    grpid.bIsUser = ai.IsUser;
                    //grpid.iID = ai.ObjectKeyWord;
                    if (ai.IsUser)
                    {
                        grpid.User = ai.User;
                    }
                    else {
                        grpid.Group = ai.Group;
                    }
                    //用户或用户组的权限
                    //grpid.iMask
                    grpid.iMask= GetRightMask(ai);
                    gRightPageItemDataList.Add(grpid);
                }

                //小黎 2011-5-12 删除全部的用户和用户组
                //for (int j = 0; j < pDlg->m_listObjects.GetListCtrl().GetItemCount(); j++)
                foreach (GDialogItemData gItemData in GDialogItemDataList)
                {
                    //GDialogItemData gItemData = (GDialogItemData*)pDlg->m_listObjects.GetListCtrl().GetItemData(j);
                    List<Acce> AcceDataList = new List<Acce>();
                    if (gItemData.bIsProject)
                   
                    {
                        //Project ^ proj = GDialogData::dbs->GetProjectByID(gItemData->iProjectNo);
                        Project proj = gItemData.Project;
                        if (!proj.acceData.Right.PCntrl)
                        {
                            reJo.msg = "你没有修改目录的权限！";
                            return reJo.Value;
                        }


                        foreach (Acce acce in proj.acceData)
                        {
                            AcceDataList.Add(acce);
                        }

                        foreach (Acce acce in AcceDataList)
                        {
                            if (proj.acceData != null)
                            {
                                proj.acceData.DelAcce(acce);
                            }
                        }
                        if (!proj.acceData.Save())
                        {
                            reJo.msg = "设置过程出现错误！";
                            return reJo.Value;
                        }
                    }
                    else
                    {
                        //Doc  doc = GDialogData::dbs->GetDocByID(gItemData->iDocNo);
                        Doc doc = gItemData.Doc;

                        if (!doc.acceData.Right.DCntrl)
                        {
                            reJo.msg = "你没有修改文档的权限！";
                            return reJo.Value;
                        }

                        foreach (Acce acce in doc.acceData)

                        {
                            AcceDataList.Add(acce);
                        }
                        foreach (Acce acce in AcceDataList)

                        {
                            if (doc.acceData != null)
                            {
                                doc.acceData.DelAcce(acce);
                            }
                        }
                        if (!doc.acceData.Save())
                        {
                            reJo.msg = "设置过程出现错误！";
                            return reJo.Value;
                        }
                    }
                }


                //for (int index = 0; index < this->m_userListCtrl.GetItemCount(); index++)
                //设置用户或用户组权限到文档或目录
                foreach (GRightPageItemData rItemData in gRightPageItemDataList)
                {
                    //GRightPageItemData* rItemData = (GRightPageItemData*)this->m_userListCtrl.GetItemData(index);

                   // if (rItemData==null)
                   //     continue;

                    //获取所有对象
                    //if (pDlg->m_listObjects.GetListCtrl().GetItemCount() <= 0)
                    //{
                    //    this->EndWaitCursor();
                    //    return;
                    // }

                    Server.Group group=null;
                    User  user=null;

                    if (rItemData.bIsUser)
                    {
                        //用户
                        //    user = GDialogData::dbs->GetUserByID(rItemData->iID);
                        user = rItemData.User;
                    }
                    else
                    {
                        //用户组
                        //    group = GDialogData::dbs->GetGroupByID(rItemData->iID);
                        group = rItemData.Group;
                    }


                    //for (int j = 0; j < pDlg->m_listObjects.GetListCtrl().GetItemCount(); j++)
                    foreach (GDialogItemData gItemData in GDialogItemDataList)
                    {
                        //GDialogItemData* gItemData = (GDialogItemData*)pDlg->m_listObjects.GetListCtrl().GetItemData(j);

                        //if (gItemData==null)
                       //     continue;


                        if (gItemData.bIsProject)
                        {
                            //目录
                            //Project ^ project = GDialogData::dbs->GetProjectByID(gItemData->iProjectNo);
                            Project project = gItemData.Project;
                            if (project==null)
                                continue;

                            AcceData  nullAcceData = new AcceData();

                            //取用户或者用户组
                            if (project.acceData!=null)
                            {
                                if (user!=null)
                                {
                                    project.acceData.AddAcce(user, rItemData.iMask); //rItemData->iMask
                                }

                                if (group!=null)
                                {
                                    project.acceData.AddAcce(group, rItemData.iMask); //rItemData->iMask
                                }

                                if (!project.acceData.Save())
                                {
                                    reJo.msg = "设置过程出现错误！";
                                    return reJo.Value;
                                }
                            }

                        }
                        else
                        {
                            //文档
                            //Doc  doc = GDialogData::dbs->GetDocByID(gItemData->iProjectNo, gItemData->iDocNo);
                            Doc doc = gItemData.Doc;
                            if (doc==null)
                                continue;

                            //取用户或者用户组
                            if (doc.acceData!=null)
                            {
                                if (user!=null)
                                {
                                    doc.acceData.AddAcce(user, rItemData.iMask);
                                }

                                if (group!=null)
                                {
                                    doc.acceData.AddAcce(group, rItemData.iMask);
                                }

                                if (!doc.acceData.Save())
                                {
                                    reJo.msg = "设置过程出现错误！";
                                    return reJo.Value;
                                }
                            }
                        }
                    }
                    //if (user !=null)
                    //{
                    //    delete user;
                    //}
                    //if (group!=null)
                    //{
                    //    delete group;
                    //}

                }


                DBSourceController.RefreshDBSource(sid);

                reJo.msg = "设置成功！";
                reJo.success = true;
                return reJo.Value;
            }
            catch (Exception ex)
            {
                reJo.msg = "运行错误！错误信息：" + ex.Message;
                CommonController.WebWriteLog(ex.Message);
            }
            return reJo.Value;
        }

        private static int GetRightMask(acceItem ai)
        {
            int mask = 0;
            Right right = new Right(mask);

            //right.DCntrl = (ai.DCntrl == "True") ? true : false;

            //获取当前点击的节点对应的权限
            //if (hItem == this.m_hPCntrl)
            if (ai.PCntrl == "True")
            {
                //if (isChecked)
                //{
                // this.m_rightTreeCtrl.SetRightCheck(this.m_hPNone, 0);
                // this.m_rightTreeCtrl.SetRightCheck(this.m_hPRead, 1);

                right.PCntrl = true;
                right.PNone = false;
                right.PRead = true;
            }
            else
            {
                //  //  this.m_rightTreeCtrl.SetRightCheck(this.m_hPFull, 0);

                right.PCntrl = false;
                right.PFull = false;

                //    //if (right.PNone == true)
                //    //    this.m_rightTreeCtrl.SetRightCheck(this.m_hPNone, 1);
                //}


            }
            //if (hItem == this.m_hPCreate)
            if (ai.PCreate == "True")
            {
                ///*right.PCreate = true ; */
                //if (isChecked)
                //{
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hPNone, 0);
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hPRead, 1);
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hPWrite, 1);

                right.PNone = false;
                right.PRead = true;
                right.PWrite = true;
                right.PCreate = true;
            }
            else
            {
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hPFull, 0);
                right.PCreate = false;
                right.PFull = false;

                //    if (right.PNone == true)
                //        this.m_rightTreeCtrl.SetRightCheck(this.m_hPNone, 1);


                //}
            }
            //if (hItem == this.m_hPDelete)
            if (ai.PDelete == "True")
            {
                ///*right.PDelete = true ; */
                //if (isChecked)
                //{
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hPNone, 0);
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hPRead, 1);
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hPWrite, 1);

                right.PNone = false;
                right.PRead = true;
                right.PWrite = true;
                right.PDelete = true;


            }
            else
            {
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hPFull, 0);

                right.PDelete = false;
                right.PFull = false;

                //    if (right.PNone == true)
                //        this.m_rightTreeCtrl.SetRightCheck(this.m_hPNone, 1);

                //}
            }

            //if (hItem == this.m_hPRead)
            if (ai.PRead == "True")
            {
                //  /*right.PRead = true ; */
                // if (isChecked)
                //  {
                //     this.m_rightTreeCtrl.SetRightCheck(this.m_hPRead, 1);
                //      this.m_rightTreeCtrl.SetRightCheck(this.m_hPNone, 0);

                right.PRead = true;
                right.PNone = false;
            }
            else
            {
                //   this.m_rightTreeCtrl.SetRightCheck(this.m_hPFull, 0);
                //   this.m_rightTreeCtrl.SetRightCheck(this.m_hPWrite, 0);

                right.PFull = false;
                right.PRead = false;
                right.PWrite = false;

                // if (right.PNone == true)
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hPNone, 1);
                //}

            }
            //if (hItem == this.m_hPWrite)
            if (ai.PWrite == "True")
            {
                ///*right.PWrite= true ; */
                //if (isChecked)
                //{
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hPRead, 1);
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hPNone, 0);

                right.PWrite = true;
                right.PRead = true;
                right.PNone = false;
            }
            else
            {
                //     this.m_rightTreeCtrl.SetRightCheck(this.m_hPFull, 0);

                right.PWrite = false;
                right.PFull = false;

                //   if (right.PNone == true)
                //        this.m_rightTreeCtrl.SetRightCheck(this.m_hPNone, 1);

                //  }

            }
            //if (hItem == this.m_hPNone)
            if (ai.PNone == "True")
            {
                ///*right.PNone = true ; */
                //if (isChecked)
                //{
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hPRead, 0);
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hPWrite, 0);
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hPCntrl, 0);
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hPCreate, 0);
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hPDelete, 0);
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hPFull, 0);

                right.PRead = false;
                right.PWrite = false;
                right.PCntrl = false;
                right.PCreate = false;
                right.PDelete = false;
                right.PNone = true;
            }
            else
            {
                //    if (right.PNone == true)
                //    {
                //        this.m_rightTreeCtrl.SetRightCheck(this.m_hPNone, 1);
                //    }
                //    else
                right.PNone = false;
                //}




            }

            //if (hItem == this.m_hPFull)
            if (ai.PFull == "True")
            {
                ///*right.PFull = true ; */
                //if (isChecked)
                //{
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hPNone, 0);
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hPRead, 1);
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hPWrite, 1);
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hPCntrl, 1);
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hPCreate, 1);
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hPDelete, 1);


                right.PNone = false;
                right.PRead = true;
                right.PWrite = true;
                right.PCntrl = true;
                right.PCreate = true;
                right.PDelete = true;
                right.PFull = true;
            }
            else
            {
                right.PFull = false;

                //    if (right.PNone == true)
                //        this.m_rightTreeCtrl.SetRightCheck(this.m_hPNone, 1);

                //}
            }

            //if (hItem == this.m_hDCntrl)
            if (ai.DCntrl == "True")
            {
                /*right.DCntrl = true ; */
                // if (isChecked)
                // {
                //   this.m_rightTreeCtrl.SetRightCheck(this.m_hDNone, 0);

                //TIM 2009-07-27
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hDRead, 1);

                right.DCntrl = true;
                right.DNone = false;
            }
            else
            {
                //  this.m_rightTreeCtrl.SetRightCheck(this.m_hDFull, 0);

                right.DCntrl = false;
                right.DFull = false;

                //  if (right.DNone == true)
                //        this.m_rightTreeCtrl.SetRightCheck(this.m_hDNone, 1);
                //  }
            }
            //if (hItem == this.m_hDCreate)
                if (ai.DCreate == "True")
                {
                ///*right.DCreate = true ; */
                //if (isChecked)
                //{
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hDNone, 0);
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hDRead, 1);
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hDWrite, 1);

                    right.DCreate = true;
                    right.DNone = false;
                    right.DRead = true;
                    right.DWrite = true;
                }
                else
                {
                    //this.m_rightTreeCtrl.SetRightCheck(this.m_hDFull, 0);

                    right.DCreate = false;
                    right.DFull = false;

                //    if (right.DNone == true)
                //        this.m_rightTreeCtrl.SetRightCheck(this.m_hDNone, 1);
                //}
            }
            //if (hItem == this.m_hDDelete)
                if (ai.DDelete == "True")
                {
                ///*right.DDelete = true ; */
                //if (isChecked)
                //{
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hDNone, 0);
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hDRead, 1);
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hDWrite, 1);

                    right.DDelete = true;
                    right.DNone = false;
                    right.DRead = true;
                    right.DWrite = true;
                }
                else
                {
                    //this.m_rightTreeCtrl.SetRightCheck(this.m_hDFull, 0);

                    right.DDelete = false;
                    right.DFull = false;

                //    if (right.DNone == true)
                //        this.m_rightTreeCtrl.SetRightCheck(this.m_hDNone, 1);
                //}
            }
            //if (hItem == this.m_hDFread)
                if (ai.DFRead == "True")
                {
                ///*right.DFRead= true ; */
                //if (isChecked)
                //{
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hDNone, 0);
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hDRead, 1);

                    right.DNone = false;
                    right.DRead = true;
                    right.DFRead = true;
                }
                else
                {
                    //this.m_rightTreeCtrl.SetRightCheck(this.m_hDFull, 0);
                    //this.m_rightTreeCtrl.SetRightCheck(this.m_hDFWrite, 0);

                    right.DFRead = false;
                    right.DFull = false;
                    right.DFWrite = false;

                //    if (right.DNone == true)
                //        this.m_rightTreeCtrl.SetRightCheck(this.m_hDNone, 1);
                //}
            }

            //if (hItem == this.m_hDFWrite)
            if (ai.DFWrite == "True")
            {
                ///*right.DFWrite = true ; */
                //if (isChecked)
                //{
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hDNone, 0);
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hDRead, 1);
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hDWrite, 1);
                //    //				this.m_rightTreeCtrl.SetRightCheck(this.m_hDDelete , 1);
                //this.m_rightTreeCtrl.SetRightCheck(this.m_hDFread, 1);

                right.DNone = false;
                right.DRead = true;
                right.DWrite = true;
                right.DFRead = true;
                right.DFWrite = true;
            }
            else
            {
                //this.m_rightTreeCtrl.SetRightCheck(this.m_hDFull, 0);
                right.DFull = false;
                right.DFWrite = false;

                //    if (right.DNone == true)
                //        this.m_rightTreeCtrl.SetRightCheck(this.m_hDNone, 1);
                //}
            }
            //if (hItem == this.m_hDRead)
            if (ai.DRead == "True")
            {
                ///*right.DRead= true ; */
                //if (isChecked)
                //{
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hDNone, 0);

                right.DNone = false;
                right.DRead = true;
            }
            else
            {
                //this.m_rightTreeCtrl.SetRightCheck(this.m_hDFull, 0);
                //this.m_rightTreeCtrl.SetRightCheck(this.m_hDWrite, 0);
                //this.m_rightTreeCtrl.SetRightCheck(this.m_hDFread, 0);
                //this.m_rightTreeCtrl.SetRightCheck(this.m_hDFWrite, 0);


                right.Full = false;
                right.DWrite = false;
                right.DFRead = false;
                right.DFWrite = false;

                //    if (right.DNone == true)
                //        this.m_rightTreeCtrl.SetRightCheck(this.m_hDNone, 1);
                //}
            }
            //if (hItem == this.m_hDWrite)
                if (ai.DWrite == "True")
                {
                ///*right.DWrite= true ; */
                //if (isChecked)
                //{
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hDRead, 1);
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hDNone, 0);

                    right.DRead = true;
                    right.DWrite = true;
                    right.DNone = false;
                }
                else
                {
                    //this.m_rightTreeCtrl.SetRightCheck(this.m_hDFull, 0);

                    right.DWrite = false;
                    right.DFull = false;

                //    if (right.DNone == true)
                //        this.m_rightTreeCtrl.SetRightCheck(this.m_hDNone, 1);
                //}
            }
            //if (hItem == this.m_hDNone)
            if (ai.DNone == "True")
            {
                ///*right.DNone = true ; */
                //if (isChecked)
                //{
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hDRead, 0);
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hDWrite, 0);
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hDCntrl, 0);
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hDCreate, 0);
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hDDelete, 0);
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hDFWrite, 0);
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hDFread, 0);
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hDFull, 0);


                right.DCreate = false;
                right.DNone = true;
                right.DRead = false;
                right.DWrite = false;
                right.DCntrl = false;
                right.DDelete = false;
                right.DFRead = false;
                right.DFull = false;
                right.DFWrite = false;
            }
            else
            {

                //    if (right.DNone == true)
                //        this.m_rightTreeCtrl.SetRightCheck(this.m_hDNone, 1);

                //}

            }

            //if (hItem == this.m_hDFull)
            if (ai.DFull == "True")
            {
                ///*right.DFull = true ; */
                //if (isChecked)
                //{
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hDNone, 0);
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hDRead, 1);
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hDWrite, 1);
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hDCntrl, 1);
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hDCreate, 1);
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hDDelete, 1);
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hDFWrite, 1);
                //    this.m_rightTreeCtrl.SetRightCheck(this.m_hDFread, 1);


                right.DCreate = true;
                right.DNone = false;
                right.DRead = true;
                right.DWrite = true;
                right.DCntrl = true;
                right.DDelete = true;
                right.DFRead = true;
                right.DFull = true;
                right.DFWrite = true;
            }
            else
            {
                right.DFull = false;

                //    if (right.DNone == true)
                //        this.m_rightTreeCtrl.SetRightCheck(this.m_hDNone, 1);
                //}
            }


            return right.mask;
        }
    }


    /// <summary>
    /// 定义权限对象
    /// </summary>
    struct acceItem {
        public string ObjectKeyWord { get; set; }

        public string PFull { get; set; }
        public string PCreate { get; set; }
        public string PRead { get; set; }
        public string PWrite { get; set; }
        public string PDelete { get; set; }
        public string PCntrl { get; set; }
        public string PNone { get; set; }

        public string DFull { get; set; }
        public string DCreate { get; set; }
        public string DRead { get; set; }
        public string DWrite { get; set; }
        public string DDelete { get; set; }
        public string DFRead { get; set; }
        public string DFWrite { get; set; }
        public string DCntrl { get; set; }
        public string DNone { get; set; }

        public bool IsUser { get; set; }
        public User User { get; set; }
        public Server.Group Group { get; set; }
    }


    /// <summary>
    /// 定义要修改的用户或用户组对象
    /// </summary>
    struct GRightPageItemData
    {
        public bool bIsUser { get; set; }
        public int iID { get; set; }
        public int iMask { get; set; }
        public User User { get; set; }
        public Server.Group Group { get; set; }
    };

    /// <summary>
    /// 定义要修改的目录或文档对象
    /// </summary>
    struct GDialogItemData
    {
        public bool bIsProject { get; set; }
        public int iProjectNo { get; set; }
        public int iDocNo { get; set; }
        public string sDesc { get; set; }
        public Doc Doc { get; set; }
        public Project Project { get; set; }
    };

}
