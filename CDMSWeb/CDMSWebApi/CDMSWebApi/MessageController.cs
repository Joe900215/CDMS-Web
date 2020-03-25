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
    /// <summary>  
    /// 消息操作类
    /// </summary>  
    ///  
    public class MessageController : Controller
    {



        /// <summary>
        /// 获取Message消息类别对象下子Message树节点列表
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="node">Project节点关键字</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时，data包含多个JObject，每个JObject里面包含参数"id"：节点关键字，"text"：节点文本，"parentId"：父节点关键字
        /// "leaf":是否有子节点("true","false"),"iconCls":设置图标</para>
        /// <para>例子：</para>
        /// </returns>

        public static JObject GetMessageTree(string sid,string node)
        {
            ExReJObject reJo = new ExReJObject();
            try
            {
                string path = node ?? "/";

                string keyword = path;

                if (sid == null) return CommonController.SidError();

                string strMsg = string.Empty;

                JArray jaGetList = new JArray();



                //登录用户
                User curUser = DBSourceController.GetCurrentUser(sid);
                if (curUser == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }


                //登录用户
                DBSource dbsource = curUser.dBSource;
                if (dbsource == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

                //如果是访问根消息树，就刷新一下数据源（按下F5自动刷新数据源）
                DBSourceController.refreshDBSource(sid, "1");

                string strNoRead = curUser.NoReadMessageNumber.ToString() ;//未读消息
                string strRead = curUser.ReadedMessageNumber.ToString();//已读消息
                string strSend = curUser.SendMessageNumber.ToString();//已发送消息
                string strDraft = curUser.DraftMessageList.Count.ToString();//草稿

                //string strProcessWorkFlow = curUser.ProcessWorkFlowList.Count.ToString();//待处理流程
                //string strPlanWorkFlow = curUser.PlanWorkFlowList.Count.ToString();//参与流程
                //string strFinishWorkFlow = curUser.FinishWorkFlowList.Count.ToString();//完成流程
                //string strErrorWorkFlow = curUser.ErrorWorkFlowList.Count.ToString();//异常工作流


                JObject joMessage = new JObject(new JProperty("NoRead", strNoRead),
                                         new JProperty("Read", strRead),
                                         new JProperty("Send", strSend),
                                         new JProperty("Draft", strDraft));
                                         //,
                                         //new JProperty("ProcessWorkFlow", strProcessWorkFlow),
                                         //new JProperty("PlanWorkFlow", strPlanWorkFlow),
                                         //new JProperty("FinishWorkFlow", strFinishWorkFlow),
                                         //new JProperty("ErrorWorkFlow", strErrorWorkFlow));

                JArray jaResult = new JArray();
                jaResult.Add(new JObject{                        
                            new JProperty("id",keyword+"_1"),//节点关键字
                            new JProperty("text","新消息"+"("+joMessage["NoRead"]+")"),//节点文本
                            new JProperty("parentId",keyword),//父节点关键字
                            new JProperty("leaf",true),//没有子节点
                            new JProperty("iconCls","myfolder")//设置图标
                        });
                jaResult.Add(new JObject{                        
                            new JProperty("id",keyword+"_2"),//节点关键字
                            new JProperty("text","历史消息"+"("+joMessage["Read"]+")"),//节点文本
                            new JProperty("parentId",keyword),//父节点关键字
                            new JProperty("leaf",true),//没有子节点
                            new JProperty("iconCls","myfolder")//设置图标
                        });
                jaResult.Add(new JObject{                        
                            new JProperty("id",keyword+"_3"),//节点关键字
                            new JProperty("text","已发消息"+"("+joMessage["Send"]+")"),//节点文本
                            new JProperty("parentId",keyword),//父节点关键字
                            new JProperty("leaf",true),//没有子节点
                            new JProperty("iconCls","myfolder")//设置图标
                        });
                jaResult.Add(new JObject{                        
                            new JProperty("id",keyword+"_4"),//节点关键字
                            new JProperty("text","草稿"+"("+joMessage["Draft"]+")"),//节点文本
                            new JProperty("parentId",keyword),//父节点关键字
                            new JProperty("leaf",true),//没有子节点
                            new JProperty("iconCls","myfolder")//设置图标
                        });


                //下面暂时屏蔽
                if ("Visible" == "true")
                {
                    jaResult.Add(new JObject{                        
                            new JProperty("id",keyword+"_5"),//节点关键字
                            new JProperty("text","待处理流程"+"("+joMessage["ProcessWorkFlow"]+")"),//节点文本
                            new JProperty("parentId",keyword),//父节点关键字
                            new JProperty("leaf",true),//没有子节点
                            new JProperty("iconCls","myfolder")//设置图标
                        });

                    jaResult.Add(new JObject{                        
                            new JProperty("id",keyword+"_6"),//节点关键字
                            new JProperty("text","参与流程"+"("+joMessage["PlanWorkFlow"]+")"),//节点文本
                            new JProperty("parentId",keyword),//父节点关键字
                            new JProperty("leaf",true),//没有子节点
                            new JProperty("iconCls","myfolder")//设置图标
                        });
                    jaResult.Add(new JObject{                        
                            new JProperty("id",keyword+"_7"),//节点关键字
                            new JProperty("text","完成流程"+"("+joMessage["FinishWorkFlow"]+")"),//节点文本
                            new JProperty("parentId",keyword),//父节点关键字
                            new JProperty("leaf",true),//没有子节点
                            new JProperty("iconCls","myfolder")//设置图标
                        });
                    jaResult.Add(new JObject{                        
                            new JProperty("id",keyword+"_8"),//节点关键字
                            new JProperty("text","异常工作流"+"("+joMessage["ErrorWorkFlow"]+")"),//节点文本
                            new JProperty("parentId",keyword),//父节点关键字
                            new JProperty("leaf",true),//没有子节点
                            new JProperty("iconCls","myfolder")//设置图标
                        });
                }
                reJo.data = jaResult;
                reJo.total = jaResult.Count;
                reJo.success = true;
            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(e.Message);
            }
            return reJo.Value;
        }

 
                    
        /// <summary>
        /// 返回MessageList的JSON对象
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="MessageType">消息类型，值可以是："未读消息_1","已读消息_2","已发送消息_3","草稿_4","待处理流程_5","参与流程_6","完成流程_7","异常工作流_8"</param>
        /// <param name="page">要访问的页数</param>
        /// <param name="limit">每一页的记录数</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,total返回记录总数；操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时，data包含多个JObject，每个JObject里面包含参数"Keyword","Sender","Title","SendDatetime",
        /// "Workflow_DESC":流程名称,"SignificantType":重要程度,"DelayDay":延迟天数,"WorkflowId","post_time"</para>
        /// <para>例子：</para>
        /// </returns>
        public static JObject GetMessageList(string sid,string MessageType, string page, string limit)
        {

            ExReJObject reJo = new ExReJObject();
            JArray jaGetList = new JArray();

            try
            {
                MessageType = MessageType ?? "";
                page = string.IsNullOrEmpty(page)?"1" : page;
                limit = string.IsNullOrEmpty(limit) ? "50" : limit;

                if (string.IsNullOrEmpty(MessageType))
                {
                    reJo.msg = "错误的提交数据。";
                    return reJo.Value;
                }
                else
                {
                    string strType = MessageType.Split('_')[1];

                    string strMsg = string.Empty;
                    page = (Convert.ToInt32(page) - 1).ToString();

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

                    JArray jaMsgList = new JArray();
                    JArray jaResult = new JArray();

                    int DraftMessageType=4;
                    if (Convert.ToInt32(strType) == DraftMessageType)
                    {
                        List<Message> msgList = curUser.DraftMessageList;


                        foreach (var msg in msgList)  //查找某个字段与值
                        {
                            DateTime dt = (DateTime)msg.SendDate;
                            //DateTime dt = Convert.ToDateTime((((JObject)ss)["D"]).ToString());
                            long timeStamp = (long)((DateTime.Now - dt).TotalMilliseconds); // 相差毫秒数

                            User sender=dbsource.GetUserByID(msg.SenderID);
                            
                            string senderName=(sender==null)?"":sender.O_username;

                            jaResult.Add(new JObject {
                                new JProperty("Keyword",msg.KeyWord),
                                
                                new JProperty("Sender",senderName),
                                
                                new JProperty("Title",msg.Title),
                                new JProperty("SendDatetime",msg.SendDate),//时间
                                new JProperty("Workflow_DESC",""),//流程名称
                                new JProperty("SignificantType",""),//重要程度
                                
                                new JProperty("DelayDay",""),//延迟天数
                                
                                new JProperty("WorkflowId",""),//流程ID
                                new JProperty("post_time",timeStamp.ToString()),
                            });
                        }
                    }
                    else
                    {
                        //返回消息XML
                        jaMsgList = (JArray)JsonConvert.DeserializeObject(dbsource.GetMessageListJSON(curUser.O_userno, (enMessageType)(Convert.ToInt32(strType)), Convert.ToInt32(limit), Convert.ToInt32(page)));
                        
                        
                        foreach (var ss in jaMsgList)  //查找某个字段与值
                        {
                            DateTime dt = Convert.ToDateTime((((JObject)ss)["D"]).ToString());
                            long timeStamp = (long)((DateTime.Now - dt).TotalMilliseconds); // 相差毫秒数

                            jaResult.Add(new JObject {
                                new JProperty("Keyword",((JObject) ss)["KW"]),
                                new JProperty("Sender",((JObject) ss)["S"]),
                                new JProperty("Title",((JObject) ss)["T"]),
                                new JProperty("SendDatetime",((JObject) ss)["D"]),//时间
                                new JProperty("Workflow_DESC",((JObject) ss)["WF"]),//流程名称
                                new JProperty("SignificantType",((JObject) ss)["CU"]),//重要程度
                                new JProperty("DelayDay",((JObject) ss)["DD"]),//延迟天数
                                new JProperty("WorkflowId",((JObject) ss)["WID"]),//流程ID
                                new JProperty("post_time",timeStamp.ToString()),// "1305823292"),
                            });
                        }
                    }

                    if (strType == "1")
                    {
                        reJo.total = curUser.NoReadMessageNumber;//未读消息
                    }
                    else if (strType == "2")
                    {
                        reJo.total = curUser.ReadedMessageNumber;//已读消息
                    }
                    else if (strType == "3")
                    {
                        reJo.total = curUser.SendMessageNumber;//已发送消息
                    }
                    else if (strType == "4")
                    {
                        reJo.total = curUser.DraftMessageList.Count;//草稿
                    }
                    else if (strType == "5")
                    {
                        reJo.total = curUser.ProcessWorkFlowList.Count;//待处理流程
                    }
                    else if (strType == "6")
                    {
                        reJo.total = curUser.PlanWorkFlowList.Count;//参与流程
                    }
                    else if (strType == "7")
                    {
                        reJo.total = curUser.FinishWorkFlowList.Count;//完成流程
                    }
                    else if (strType == "8")
                    {
                        reJo.total = curUser.ErrorWorkFlowList.Count;//异常工作流
                    }

                    reJo.data = jaResult;
                    reJo.success = true;
                }
            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(e.Message);
            }
            return reJo.Value;
        }

        /// <summary>
        /// 返回用户未读消息列表,用于显示左下方消息提示窗口
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,total返回记录总数；操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时，data包含多个JObject，每个JObject里面包含参数"Keyword","Sender","Title","SendDatetime","Workflow_DESC":流程名称,"SignificantType":重要程度,"DelayDay":延迟天数,"WorkflowId","post_time"</para>
        /// <para>例子：</para>
        /// </returns>
        public static JObject GetUserNoReadMessageList(string sid)
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
            

                //**********************************************************
                //登录后判断是否有新消息,有这提示用户
                if (curUser != null && curUser.NoReadMessageNumber > 0)
                {
                    JArray jaResult = new JArray();

                    //for (int i = 1; i <= 5; i++)
                    //{
                    MsgUser frontMsg = curUser.NoReadMessageList[curUser.NoReadMessageList.Count-1];
                        jaResult.Add(new JObject { 
                            new JProperty("Title",frontMsg.message.Title),
                            new JProperty("Content",frontMsg.message.Messages),
                        });
                    //}
                    reJo.data = jaResult;
                    reJo.total = curUser.NoReadMessageNumber;
                    reJo.success = true;

                }
            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(e.Message);
            }
            return reJo.Value;
        }

         /// <summary>
        /// 返回Message的JSON对象
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="MessageKeyword">消息关键字</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时，data包含一个JObject，里面包含参数："Sender"，"RecUsers"，"Title"，"Content"，"HasWorkFlow"；</para>
        /// <para>当消息的附件里面有文档时，会在data里面添加一个或多个JObject，每个JObject里面包含参数"Attachment"：文档名称，"AttaKeyword"：文档Keyword，"AttaType"：文档类型值为"Doc"</para>
        /// <para>当消息的附件里面有目录时，会在data里面添加一个或多个JObject，每个JObject里面包含参数"Attachment"：目录名称，"AttaKeyword"：目录Keyword，"AttaType"：文档类型值为"Proj"</para>
        /// <para>例子：</para>
        /// </returns>
        public static JObject GetMessage(string sid,string MessageKeyword)
        {
            ExReJObject reJo = new ExReJObject();
            try
            {
                if (string.IsNullOrEmpty(MessageKeyword))
                {
                    reJo.msg = "错误的提交数据。";
                    return reJo.Value;
                }
                else
                {

                    User curUser = DBSourceController.GetCurrentUser(sid);
                    if (curUser == null)
                    {
                        reJo.msg = "登录验证失败！请尝试重新登录！";
                        return reJo.Value;
                    }

                    //登录用户
                    DBSource dbsource = curUser.dBSource;
                    if (dbsource == null)
                    {
                        reJo.msg = "登录验证失败！请尝试重新登录！";
                        return reJo.Value;
                    }

                    JArray jaResult = new JArray();
                    JObject reJObject = new JObject();

                    Object obj = null;

                    obj = dbsource.GetObjectByKeyWord(MessageKeyword);

                    if (obj == null) return null;
                    Message curMessage;
                    if (obj is MsgUser)
                        curMessage = ((MsgUser)obj).message;
                    else if (obj is Message)
                        curMessage = (Message)obj;
                    else
                        return null;


                    //1.发送者
                    User sender = curMessage.dBSource.GetUserByID(curMessage.SenderID);
                    string strSender = sender != null ? sender.ToString : "";
                    reJObject.Add(new JProperty("sender", strSender));
                    string SenderKeyword= sender != null ? sender.KeyWord : "";

                    //2.接收者
                    String recUsers = "";
                    String recUserList = "";
                    if (curMessage.UserList != null && curMessage.UserList.Count > 0)
                    {
                        JObject JoRecUsers = new JObject();
                        int indexRecUsers = 0;
                        foreach (MsgUser u in curMessage.UserList)
                        {
                            String strRead = "";
                            indexRecUsers = indexRecUsers + 1;
                            if (u.ReadDate.HasValue)
                                strRead = "(已读)";
                            User user=u.dBSource.GetUserByID(u.UserID);
                            if (String.IsNullOrEmpty(recUsers))
                            {
                                recUsers = user.ToString + strRead;
                                recUserList = user.KeyWord;
                            }
                            else
                            { 
                                recUsers = recUsers + ";" + u.dBSource.GetUserByID(u.UserID).ToString + strRead;
                                recUserList = recUserList + "," + user.KeyWord;
                            }
                        }
                    }
                    reJObject.Add(new JProperty("recUsers", recUsers));
                    reJObject.Add(new JProperty("recUserList", recUserList));
                    

                    //3.附件
                    JArray JaProjList = new JArray();
                    if (curMessage.projectList != null && curMessage.projectList.Count > 0)
                    {
                        JObject JoProjList = new JObject();
                        int indexProjList = 0;
                        foreach (Project project in curMessage.projectList)
                        {
                            indexProjList = indexProjList + 1;
                            JoProjList.Add(new JProperty("Proj" + indexProjList.ToString(), new JArray(new JObject(new JProperty("Proj", project.ToString),
                                new JProperty("KeyWord", project.KeyWord)))));
                        }
                        JaProjList.Add(JoProjList);
                    }
                    reJObject.Add(new JProperty("ProjList", JaProjList));

                    JArray JaDocList = new JArray();
                    if (curMessage.docList != null && curMessage.docList.Count > 0)
                    {
                        JObject JoDocList = new JObject();
                        int indexDocList = 0;
                        foreach (Doc d in curMessage.docList)
                        {
                            indexDocList = indexDocList + 1;
                            JoDocList.Add(new JProperty("Doc" + indexDocList.ToString(), new JArray(new JObject(new JProperty("Doc", d.ToString),
                                new JProperty("KeyWord", d.KeyWord),new JProperty("FileSize", d.FileSize)))));
                        }
                        JaDocList.Add(JoDocList);
                    }
                    reJObject.Add(new JProperty("DocList", JaDocList));

                    //4.发送时间
                    reJObject.Add(new JProperty("sendDate", curMessage.SendDate.Value.ToString()));

                    //获取抄送人
                    #region 获取抄送人
                    string strChaoSongs = "";
                    string strCCUserList = "";
                    List<User> chaosongs = new List<User>();

                    if (curMessage.UserList != null && curMessage.UserList.Count > 0)
                    {
                        foreach (MsgUser mU in curMessage.UserList)
                        {
                            if (mU.UserType == enMsgUserType.CCUser)
                            {
                                //抄送人
                                User chaosong = mU.dBSource.GetUserByID(mU.UserID);

                                if (chaosong == null)
                                    continue;

                                chaosongs.Add(chaosong);

                                if (String.IsNullOrEmpty(strChaoSongs))
                                {
                                    strChaoSongs = chaosong.ToString;
                                    strCCUserList = chaosong.KeyWord;
                                }
                                else
                                {
                                    strChaoSongs += ";" + chaosong.ToString;
                                    strCCUserList += "," + chaosong.KeyWord;
                                }
                            }
                            else if (mU.UserType == enMsgUserType.ToUser)
                            {
                                //接收人
                                //User receiver = this.cuMessage.dBSource.GetUserByID(mU.UserID);
                                //if (receiver == null)
                                //    continue;

                                //if (String.IsNullOrEmpty(strReceivers))
                                //    strReceivers = receiver.ToString;
                                //else
                                //    strReceivers += ";" + receiver.ToString; 
                            }

                        }
                    } 
                    #endregion

                    //5.标题
                    reJObject.Add(new JProperty("Title", curMessage.Title));

                    //6.消息内容
                    reJObject.Add(new JProperty("Messages", curMessage.Messages));

                    //7.有无流程
                    if (curMessage.workFlow != null && curMessage.workFlow.DefWorkFlow != null)
                    {
                        reJObject.Add(new JProperty("hasWorkFlow", "true"));
                        reJObject.Add(new JProperty("WorkFlowKeyword", curMessage.workFlow.KeyWord));
                    }
                    else if (curMessage.projectList!=null && curMessage.projectList.Count>0 && curMessage.projectList[0]!=null && curMessage.projectList[0].WorkFlow!=null)
                    {
                        reJObject.Add(new JProperty("hasWorkFlow", "true"));
                        reJObject.Add(new JProperty("WorkFlowKeyword", curMessage.projectList[0].WorkFlow.KeyWord));
                    }
                    else
                    {
                        reJObject.Add(new JProperty("hasWorkFlow", "false"));
                        reJObject.Add(new JProperty("WorkFlowKeyword", ""));
                    }

                    JObject joMessage = reJObject;

                    jaResult.Add(new JObject {
                        new JProperty("Sender",joMessage["sender"]),
                        new JProperty("SenderKeyword", SenderKeyword),
                        new JProperty("RecUsers",joMessage["recUsers"]),
                        new JProperty("RecUserList",joMessage["recUserList"]),
                        new JProperty("CCUsers",strChaoSongs),
                        new JProperty("CCUserList",strCCUserList),
                        new JProperty("SendDate",joMessage["sendDate"]),
                        new JProperty("Title",joMessage["Title"]),
                        new JProperty("Content",joMessage["Messages"]),
                        new JProperty("HasWorkFlow",joMessage["hasWorkFlow"]),
                        new JProperty("WorkFlowKeyword",joMessage["WorkFlowKeyword"]),
                    });
                    JArray jaDocList = (JArray)joMessage["DocList"];
                    if (jaDocList.Count > 0)
                    {
                        JObject joDocList = (JObject)jaDocList[0];
                        for (int i = 1; i <= joDocList.Count; i++)
                        {
                            string strDoc = "Doc" + i.ToString();
                            string DocName = (string)(((JArray)joDocList[strDoc])[0]["Doc"]);
                            string DocKeyWord = (string)(((JArray)joDocList[strDoc])[0]["KeyWord"]);
                            string DocFileSize= (string)(((JArray)joDocList[strDoc])[0]["FileSize"]);
                            string AttaType = "Doc";
                            jaResult.Add(new JObject {
                        new JProperty("Attachment", DocName),//附件
                        new JProperty("AttaKeyword",DocKeyWord),
                        new JProperty("AttaType",AttaType),
                        new JProperty("FileSize",DocFileSize)
                    });
                        }
                    }
                    JArray jaProjList = (JArray)joMessage["ProjList"];
                    if (jaProjList.Count > 0)
                    {
                        JObject joProjList = (JObject)jaProjList[0];
                        for (int i = 1; i <= joProjList.Count; i++)
                        {
                            string strProj = "Proj" + i.ToString();
                            string ProjName = (string)(((JArray)joProjList[strProj])[0]["Proj"]);
                            string ProjKeyWord = (string)(((JArray)joProjList[strProj])[0]["KeyWord"]);
                            string AttaType = "Proj";
                            jaResult.Add(new JObject {
                        new JProperty("Attachment", ProjName),//附件
                        new JProperty("AttaKeyword",ProjKeyWord),
                        new JProperty("AttaType",AttaType),
                    });
                        }
                    }

                    reJo.data = jaResult;
                    reJo.success = true;
                }
            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(e.Message);
            }
            return reJo.Value;

        }


        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="MessageKeyword">消息关键字</param>
        /// <returns></returns>
        public static JObject DeleteMessage(string sid,  string MessageKeyword)
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
                if (dbsource == null)//登录并获取dbsource成功
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }


                if (string.IsNullOrEmpty(MessageKeyword))
                {
                    reJo.msg = "删除消息的时候出现错误：消息不存在！";
                    return reJo.Value;
                }

                Object curObj = dbsource.GetObjectByKeyWord(MessageKeyword);
                if (curObj == null)
                {
                    reJo.msg = "删除消息的时候出现错误：消息不存在！";
                    return reJo.Value;
                }

                Message msg = curObj as Message;

                if (msg.UserList != null || msg.UserList.Count > 0)
                {
                    foreach (MsgUser msgUser in msg.UserList)

                    {
                        if (msgUser.UserID == curUser.O_userno)//&& (!msgUser->ReadDate.HasValue))
                        {
                            if (!msg.Delete())
                            {

                                reJo.msg = "删除消息失败！" + dbsource.LastError;
                                return reJo.Value;
                            }
                            else {
                                 reJo.success = true;
                                 reJo.data = new JArray(new JObject(new JProperty("state", "delSuccess")));//返回删除成功消息给客户端
                            }
                        }
                    }

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
        /// 把消息设置为已读状态
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="MessageKeyword">消息关键字</param>
        /// <returns></returns>
        public static JObject SetMessageRead(string sid, string MessageKeyword)
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
                if (dbsource == null)//登录并获取dbsource成功
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

                if (string.IsNullOrEmpty(MessageKeyword))
                {
                    reJo.msg = "设置消息为已读的时候出现错误：消息不存在！";
                    return reJo.Value;
                }

                Object curObj = dbsource.GetObjectByKeyWord(MessageKeyword);
                if (curObj == null)
                {
                    reJo.msg = "设置消息为已读的时候出现错误：消息不存在！";
                    return reJo.Value;
                }

                Message msg = curObj as Message;

                AVEVA.CDMS.Server.MsgUser mU = null; //(AVEVA.CDMS.Server.MsgUser)lvi.Tag;
                if (msg.UserList != null || msg.UserList.Count > 0)
                {
                    foreach (MsgUser msgUser in msg.UserList)

                    {
                        if (msgUser.UserID == curUser.O_userno)//&& (!msgUser->ReadDate.HasValue))
                        {
                            mU = msgUser;
                            break;
                        }
                    }
                }

                //mU = m_dbs.GetObjectByKeyWord(mU.KeyWord) as MsgUser;
                if (mU == null)
                {
                    reJo.msg = "设置消息为已读的时候出现错误：用户消息不存在！";
                    return reJo.Value;
                }
                //lvi.ImageIndex = 1;

                //lvi.SubItems[3].Text = "已阅";

                //if (this.lvUserMsgs.Items.Contains(lvi))
                //{
                //    this.lvUserMsgs.Items.Remove(lvi);

                //    this.lvUserOldMsgs.Items.Add(lvi);
                //}

                if (curUser.NoReadMessageList.Contains(mU))
                    curUser.NoReadMessageList.Remove(mU);

                if (!curUser.ReadedMessageList.Contains(mU))
                    curUser.ReadedMessageList.Add(mU);



                mU.ReadDate = DateTime.Now;
                mU.Modify();

                curUser.Modify();


                //TIM 2009-07-27 清空显示
                //this.ShowMsg(null);

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
        /// 发送消息
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="RecUserlist">接收消息的用户列表，用","分隔的用户关键字</param>
        /// <param name="CCUserlist">抄送消息的用户列表，用","分隔的用户关键字</param>
        /// <param name="Title">消息标题</param>
        /// <param name="Content">消息内容</param>
        /// <param name="Attalist">附件（文档和目录）关键字列表，用","分隔的文档和目录关键字</param>
        /// <returns></returns>
        public static JObject SendMessage(string sid, string RecUserlist, string CCUserlist, string Title, string Content,string Attalist)
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
                if (dbsource == null)//登录并获取dbsource成功
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

                List<User> receivers = new List<User>();
                List<User> chaoSongs = new List<User>();

                if (string.IsNullOrEmpty(RecUserlist))
                {
                    reJo.msg = "发送消息失败！请输入接收者信息！";
                    return reJo.Value;
                }


                #region 获取接收用户列表
                string[] arrayUser = RecUserlist.Split(new char[] { ',' });

                foreach (string userKeyword in arrayUser)
                {

                    Object obj = null;

                    obj = dbsource.GetObjectByKeyWord(userKeyword);

                    if (obj == null) continue;
                    User u;
                    if (obj is User)
                    {
                        u = (User)obj;
                        receivers.Add(u);
                    }
                }

                if (receivers.Count <= 0)
                {
                    reJo.msg = "发送消息失败！没有指定合法的接收者,请选择系统中的存在的人员！";
                    return reJo.Value;
                }
                #endregion

                #region 获取抄送用户列表
                string[] arrayCCUser = CCUserlist.Split(new char[] { ',' });

                foreach (string userKeyword in arrayCCUser)
                {

                    Object obj = null;

                    obj = dbsource.GetObjectByKeyWord(userKeyword);

                    if (obj == null) continue;
                    User u;
                    if (obj is User)
                    {
                        u = (User)obj;
                        chaoSongs.Add(u);
                    }
                }

                #endregion

                #region 获取附件列表
                List<Doc> addFiles = new List<Doc>();
                List<Project> addProjects = new List<Project>();
                string[] arrayAtta = Attalist.Split(new char[] { ',' });

                foreach (string attaKeyword in arrayAtta)
                {

                    Object obj = null;

                    obj = dbsource.GetObjectByKeyWord(attaKeyword);

                    if (obj == null) continue;
                    Doc doc;
                    Project proj;
                    if (obj is Doc)
                    {
                        Doc ddoc = (Doc)obj;
                        doc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;
                        addFiles.Add(doc);
                    }
                    else if (obj is Project)
                    {
                        proj = (Project)obj;
                        addProjects.Add(proj);
                    }


                }
                #endregion

                if (String.IsNullOrEmpty(Title))
                    Title = "无标题";

                Server.Message newMsg = dbsource.NewMessage(receivers, chaoSongs, Title, Content, addProjects, addFiles);

                if (newMsg == null)
                {
                    // MessageBox.Show("消息发送失败!", GlobalVariable.FormTitle);
                    //return;
                    reJo.msg = "消息发送失败！";
                    return reJo.Value;
                }
                else
                {
                    //TIM 2009-08-26 需要根据当前页面类型,来确定发送的操作
                    //1.如果是草稿,发送后,还需要保存
                    //if (this.cuFormType == enMessageFormType.DraftMessage && this.cuMessage != null && this.cuMessage.IsDraft)
                    //{
                    //    //this.cuMessage.docList = addFiles;
                    //    //this.cuMessage.projectList = addProjects;

                    //    if (MessageBox.Show("消息成功发送! 是否把改动保存到草稿?", GlobalVariable.FormTitle, MessageBoxButtons.YesNo) == DialogResult.Yes)
                    //    {
                    //        this.cuMessage.Title = title;
                    //        this.cuMessage.Messages = message;
                    //        this.cuMessage.Modify();
                    //    }
                    //}
                    //else
                    reJo.msg = "消息发送成功！";
                    reJo.success = true;
                    return reJo.Value;

                    //if (this.toReplyMsg != null)
                    //{
                    //    //this.toReplyMsg.ReplyID = newMsg.ID;
                    //    //this.toReplyMsg.ReplyDate = DateTime.Now; 
                    //    //this.toReplyMsg.Modify(); 
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
        /// 保存草稿消息，当MessageKeyword为空时，创建草稿消息
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="MessageKeyword">消息关键字</param>
        /// <param name="RecUserlist">接收消息的用户列表，用","分隔的用户关键字</param>
        /// <param name="CCUserlist">抄送消息的用户列表，用","分隔的用户关键字</param>
        /// <param name="Title">息标题</param>
        /// <param name="Content">消息内容</param>
        /// <param name="Attalist">附件（文档和目录）关键字列表，用","分隔的文档和目录关键字</param>
        /// <returns></returns>
        public static JObject SaveDraftMessage(string sid, string MessageKeyword,string RecUserlist, string CCUserlist, string Title, string Content, string Attalist)
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
                if (dbsource == null)//登录并获取dbsource成功
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

                List<User> receivers = new List<User>();
                List<User> chaoSongs = new List<User>();

                if (string.IsNullOrEmpty(RecUserlist))
                {
                    reJo.msg = "发送消息失败！请输入接收者信息！";
                    return reJo.Value;
                }


                #region 获取接收用户列表
                string[] arrayUser = RecUserlist.Split(new char[] { ',' });

                foreach (string userKeyword in arrayUser)
                {

                    Object obj = null;

                    obj = dbsource.GetObjectByKeyWord(userKeyword);

                    if (obj == null) continue;
                    User u;
                    if (obj is User)
                    {
                        u = (User)obj;
                        receivers.Add(u);
                    }
                }

                if (receivers.Count <= 0)
                {
                    reJo.msg = "发送消息失败！没有指定合法的接收者,请选择系统中的存在的人员！";
                    return reJo.Value;
                }
                #endregion

                #region 获取抄送用户列表
                string[] arrayCCUser = CCUserlist.Split(new char[] { ',' });

                foreach (string userKeyword in arrayCCUser)
                {

                    Object obj = null;

                    obj = dbsource.GetObjectByKeyWord(userKeyword);

                    if (obj == null) continue;
                    User u;
                    if (obj is User)
                    {
                        u = (User)obj;
                        chaoSongs.Add(u);
                    }
                }

                #endregion

                #region 获取附件列表
                List<Doc> addFiles = new List<Doc>();
                List<Project> addProjects = new List<Project>();
                string[] arrayAtta = Attalist.Split(new char[] { ',' });

                foreach (string attaKeyword in arrayAtta)
                {

                    Object obj = null;

                    obj = dbsource.GetObjectByKeyWord(attaKeyword);

                    if (obj == null) continue;
                    Doc doc;
                    Project proj;
                    if (obj is Doc)
                    {
                        Doc ddoc = (Doc)obj;
                        doc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;
                        addFiles.Add(doc);
                    }
                    else if (obj is Project)
                    {
                        proj = (Project)obj;
                        addProjects.Add(proj);
                    }


                }
                #endregion

                if (String.IsNullOrEmpty(Title))
                    Title = "无标题";

                if (!string.IsNullOrEmpty(MessageKeyword))
                {
                    Object obj = dbsource.GetObjectByKeyWord(MessageKeyword);
                    if (obj == null || !(obj is Message)) {
                        reJo.msg = "保存草稿失败!草稿参数错误!";
                        return reJo.Value;
                    }

                    //只需更新当前草稿消息便可
                    Message cuMessage = (Message)obj;
                    cuMessage.Messages = Content;
                    cuMessage.Title = Title;

                    //TODO: 需要把收信人以及附件都记录进去
                    //处理接收人和抄送人
                    SetMessageUsers(cuMessage, receivers, chaoSongs);

                    //处理附件
                    SetMessageFiles(cuMessage, addProjects, addFiles);

                    if (cuMessage.Modify())
                    {
                        DBSourceController.RefreshDBSource(sid);
                        reJo.success = true;
                        return reJo.Value;
                    }
                    else
                    {
                        reJo.msg = "保存草稿失败!\r\n错误: " + cuMessage.dBSource.LastError;
                        return reJo.Value;
                    }
                }
                else
                {
                    //新建草稿消息
                    Message cuMessage = dbsource.NewMessage(receivers, chaoSongs, Title, Content, addProjects, addFiles);

                    if (cuMessage != null)
                    {
                        cuMessage.IsDraft = true;

                        if (cuMessage.Modify())
                        {
                            reJo.success = true;
                            return reJo.Value;
                        }
                        else
                        {
                            reJo.msg = "保存草稿失败!";
                            return reJo.Value;
                        }
                    }
                    else
                    {
                        reJo.msg = "保存草稿失败!";
                        return reJo.Value;
                    }
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
        /// 设置某个消息的发送人/抄送人
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="receivers"></param>
        /// <param name="ccUsers"></param>
        /// <returns></returns>
        private static bool SetMessageUsers(AVEVA.CDMS.Server.Message msg, List<User> receivers, List<User> ccUsers)
        {
            try
            {
                if (msg == null) return false;

                List<MsgUser> msgUserList = new List<MsgUser>();

                //先删除所有
                if (msg.UserList != null && msg.UserList.Count > 0)
                {
                    foreach (MsgUser msgUser in msg.UserList)
                    {
                        if (!msgUserList.Contains(msgUser))
                            msgUserList.Add(msgUser);
                    }

                    foreach (MsgUser msgUser in msgUserList)
                    {
                        msg.DeleteUser(msgUser.dBSource.GetUserByID(msgUser.UserID));
                    }

                    msg.UserList.Clear();

                    msg.Modify();


                }

                //添加选择的用户
                if (receivers != null && receivers.Count > 0)
                {
                    foreach (User user in receivers)
                    {
                        msg.AddUser(user, enMsgUserType.ToUser);
                    }
                    msg.Modify();
                }

                if (ccUsers != null && ccUsers.Count > 0)
                {
                    foreach (User user in ccUsers)
                    {
                        msg.AddUser(user, enMsgUserType.CCUser);
                    }
                    msg.Modify();
                }
                //List<MsgUser> oldReceiveMsgUserList = new List<MsgUser>() ; 
                //List<MsgUser> oldCCMsgUserList = new List<MsgUser>() ; 
                //if(msg.UserList != null && msg.UserList.Count >0)
                //{
                //    foreach(MsgUser msgUser in msg.UserList)
                //    {
                //        if(msgUser.UserType == enMsgUserType.CCUser)
                //        {
                //            if(!oldCCMsgUserList.Contains(msgUser))
                //                oldCCMsgUserList.Add(msgUser) ;
                //        }
                //        else if(msgUser.UserType == enMsgUserType.ToUser)
                //        {
                //            if(!oldReceiveMsgUserList.Contains(msgUser))
                //                oldReceiveMsgUserList.Add(msgUser) ; 
                //        }

                //    }
                //}

                //if(receivers == null || receivers.Count <=0)
                //{
                //    //删除所有接收用户
                //    if(oldReceiveMsgUserList.Count >0)
                //    {
                //        foreach(MsgUser msgUser in oldReceiveMsgUserList)
                //        {
                //            if(!msgUser.Delete() )
                //                return false ; 
                //        }
                //    }
                //}

                //if(ccUsers == null || ccUsers.Count <=0)
                //{
                //    //删除所有抄送用户
                //    if(oldCCMsgUserList.Count >0)
                //    {
                //        foreach(MsgUser msgUser in oldCCMsgUserList)
                //        {
                //            if(!msgUser.Delete())
                //                return false ; 
                //        }
                //    }

                //}



                return true;


            }
            catch (Exception e)
            { }

            return false;

        }

        private static bool SetMessageFiles(AVEVA.CDMS.Server.Message msg, List<Project> pList, List<Doc> dList)
        {
            try
            {
                if (msg == null) return false;

                List<Project> oldProjectList = new List<Project>();
                List<Doc> oldDocList = new List<Doc>();

                if (msg.projectList != null && msg.projectList.Count > 0)
                {
                    foreach (Project project in msg.projectList)
                    {
                        if (!oldProjectList.Contains(project))
                            oldProjectList.Add(project);
                    }

                    foreach (Project project in oldProjectList)
                    {
                        msg.DeleteProject(project);
                    }
                    msg.projectList.Clear();
                    msg.Modify();
                }

                if (msg.docList != null && msg.docList.Count > 0)
                {
                    foreach (Doc doc in msg.docList)
                    {
                        if (!oldDocList.Contains(doc))
                            oldDocList.Add(doc);
                    }

                    foreach (Doc doc in oldDocList)
                    {
                        msg.DeleteDoc(doc);
                    }
                    msg.docList.Clear();
                    msg.Modify();
                }

                if (pList != null && pList.Count > 0)
                {
                    foreach (Project project in pList)
                    {
                        msg.AddProject(project);
                    }

                    msg.Modify();
                }

                if (dList != null && dList.Count > 0)
                {
                    foreach (Doc doc in dList)
                    {
                        msg.AddDoc(doc);
                    }

                    msg.Modify();
                }
                return true;
            }
            catch (Exception e)
            { }

            return false;
        }
    }
}
