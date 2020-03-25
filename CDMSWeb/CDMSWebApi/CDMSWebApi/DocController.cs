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
using Microsoft.Win32;
using System.Diagnostics;
using SharpCompress;
using System.Drawing;
using System.Xml;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Concurrent;

namespace AVEVA.CDMS.WebApi
{
    /// <summary>  
    /// Doc操作类
    /// </summary>  
    public class DocController : Controller
    {

        //由于测试时无法使用IIS虚拟目录，所以保留复制到新文件下载，发布时才使用虚拟目录下载
        internal static bool onTestDownload = false;


        /// <summary>
        /// 返回一个Project对象下指定DocList的JSON对象
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="ProjectKeyWord">Project节点关键字</param>
        /// <param name="page">要访问的页数，不填就取默认值1</param>
        /// <param name="filterJson">筛选条件,json格式：见下面返回里面的说明</param>
        /// <param name="limit">每一页的记录数，选填，不填就取默认值50</param>
        /// <param name="sort">远程排序参数，如果为空就不排序（使用前端排序），选填，参数格式："[{"property":"Title","direction":"ASC"}]"</param>
        /// <param name="start">选填，如果有start参数，就根据start参数获取page，start=50,page=2;start=100,page=3</param>
        /// <returns>
        /// <para>筛选条件,json格式一：</para>
        /// <para> strSQL = " (o_itemname " + "LIKE" + " '%" + value + "%' or o_itemdesc " + "LIKE" + " '%" + value + "%' ) ";</para>
        /// <para> filterObj = [{ name: 'o_itemname', value: strSQL },{ name: 'searchAttr', value: "true" },{ name: 'filterstr', value: value }];</para>
        /// <para>筛选条件,json格式二：</para>
        /// <para>" (o_itemname " + "LIKE" + " '%" + value + "%' or o_itemdesc " + "LIKE" + " '%" + value + "%' ) ";</para>
        /// <para> filterObj = [{ name: 'o_itemname', value: strSQL }];</para>
        /// <para>'searchAttr'：是否搜索附加属性，可以不填，"filterstr" ：搜索文档的附加属性，直接填写过滤的字符，可以不填 </para>
        /// <para> </para>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,total返回文档总数；操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时，data包含多个JObject，每个个JObject里面包含参数"Keyword"：文档关键字，"O_itemno"：文档Id，"Title"：文档代码加描述，"O_size"，
        /// "O_filename"，"O_dmsstatus_DESC"：文件状态（"检入"，"检出"，"最终状态"），"O_version"：版本，"Creater"，"O_credatetime"，"Updater"，"O_updatetime"，"O_outpath"，"O_flocktime"，
        /// "O_conode"：机器号，"IsShort"：是否快捷方式</para>
        /// <para>例子：</para>
        /// <code>
        /// {
        ///  "success": true,
        ///  "total": 1,
        ///  "msg": "",
        ///  "data": [
        ///    {
        ///      "Keyword": "GJEPCMSP2968D17",
        ///      "O_itemno": 17,
        ///      "Title": "7.填报工时.txt",
        ///      "O_size": "339.00 B",
        ///      "O_filename": "7.填报工时.txt",
        ///      "O_dmsstatus_DESC": "检入",
        ///      "O_version": "A",
        ///      "Creater": "admin__administrator",
        ///      "O_credatetime": "2020/1/20 14:18:13",
        ///      "Updater": "admin__administrator",
        ///      "O_updatetime": "2020/1/20 14:18:35",
        ///      "O_outpath": "",
        ///      "O_flocktime": "0001/1/1 0:00:00",     //文件锁定时间
        ///      "O_conode": "192.168.1.17",
        ///      "IsShort": "false",         //是否快捷方式
        ///      "WriteRight": "true"        //是否有写文件权限
        ///      "HasAttachFiles": true   //是否有附加文件/版本文件/参考文件
        ///    }
        ///  ]
        /// }
        /// </code>
        /// </returns>
        public static JObject GetDocList(string sid, string ProjectKeyWord, string filterJson, string page, string limit,string sort,string start)
        {
            ExReJObject reJo = new ExReJObject();
            try
            {
                ProjectKeyWord = ProjectKeyWord ?? "";
                page = page ?? "1";
                limit = limit ?? "50";

                #region 如果有start参数，就根据start参数获取page，start=50,page=2;start=100,page=3
                if (!string.IsNullOrEmpty(start))
                {
                    int intStart = (int.Parse(start) / int.Parse(limit)) + 1;
                    page = intStart.ToString();
                } 
                #endregion

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
                CurPage--;


                if (string.IsNullOrEmpty(ProjectKeyWord))
                {
                    reJo.msg = "错误的提交数据。";
                    return reJo.Value;
                }

                if (sid == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }



                //登录用户
                User curUser = DBSourceController.GetCurrentUser(sid);
                if (curUser == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

                DBSource dbsource = curUser.dBSource;


                //获取对象
                Project project = dbsource.GetProjectByKeyWord(ProjectKeyWord);
                if (project == null)
                {
                    reJo.msg = "提交参数错误，文件夹不存在！";
                    return reJo.Value;
                }
                else
                {

                    //获取同一DBsource 对象里面的 User, 避免发生线程冲突
                    curUser = dbsource.GetUserByID(curUser.O_userno);


                    List<Doc> AllDocList = new List<Doc>();// null;
                    if (string.IsNullOrEmpty(filterJson))
                    {

                        //不带筛选获取文件列表
                        #region 不带筛选获取文件列表

                        //按照分页模式获取当前页的文档列表
                        if (project.O_type == enProjectType.GlobSearch || project.O_type == enProjectType.UserSearch)
                        {
                            List<Doc> ProjDocList = project.dBSource.SelectDoc(project.SearchSQL, ShowNum, CurPage);

                            #region 根据用户设置的条件，筛选需要显示的文档
                            //根据用户设置的条件，筛选需要显示的文档
                            foreach (WebDocEvent.Before_Get_Doc_List_Event_Class BeforeGetDocs in WebDocEvent.ListBeforeGetDocs)
                            {
                                if (BeforeGetDocs.Event != null)
                                {
                                    //如果在用户事件中筛选过了，就跳出事件循环
                                    if (BeforeGetDocs.Event(BeforeGetDocs.PluginName, project, ref ProjDocList, ""))
                                    {
                                        break;
                                    }
                                }
                            }
                            #endregion

                            int index = 0;
                            foreach (Doc doc in ProjDocList)
                            {
                                index = index + 1;
                                if (index > ShowNum * (CurPage) && index <= ShowNum * (CurPage + 1))
                                {
                                    AllDocList.Add(doc);
                                }
                                if (index >= ShowNum * (CurPage + 1))
                                {
                                    break;
                                }
                            }

                            //AllDocList = ProjDocList;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(sort))
                            {
                                //按默认的docname排序
                                //2018-6-30 小钱，按照华西能源要求，文件列表需要按用户提出的条件显示
                                //先获取当前目录下的所有文件列表
                                List<Doc> ProjDocList = dbsource.DContext.Doc.Where(d => d.O_dmsstatus != enDocStatus.Delete &&
                                      d.O_filetype != enFileType.ProjectModifyExcel && d.O_projectno == project.O_projectno &&
                                      d.O_cuversion == true).ToList<Doc>();

                                #region 根据用户设置的条件，筛选需要显示的文档
                                //根据用户设置的条件，筛选需要显示的文档
                                foreach (WebDocEvent.Before_Get_Doc_List_Event_Class BeforeGetDocs in WebDocEvent.ListBeforeGetDocs)
                                {
                                    if (BeforeGetDocs.Event != null)
                                    {
                                        //如果在用户事件中筛选过了，就跳出事件循环
                                        if (BeforeGetDocs.Event(BeforeGetDocs.PluginName, project, ref ProjDocList, ""))
                                        {
                                            break;
                                        }
                                    }
                                }
                                #endregion

                                AllDocList = ProjDocList.OrderBy(doc => doc.O_itemname).
                                    Skip(CurPage * ShowNum).Take(ShowNum).ToList<Doc>();

                                AllDocList.ForEach(d => d.dBSource = dbsource);
                            }
                            else
                            {

                                #region 按排序条件排序

                                #region 获取排序参数
                                //参数格式："[{"property":"Title","direction":"ASC"}]"
                                string property = "", direction = "";

                                JArray jaSort = (JArray)JsonConvert.DeserializeObject(sort);
                                foreach (JObject joAttr in jaSort)
                                {
                                    property = joAttr["property"].ToString();
                                    direction = joAttr["direction"].ToString();
                                }
                                if (property == "Title")
                                {
                                    property = "O_itemname";
                                }
                                else if (property == "Updater")
                                {
                                    property = "O_updaterno";
                                }
                                else if (property == "Creater")
                                {
                                    property = "O_creatorno";
                                }

                                #endregion

                                //方法一：
                                //Func<Doc, object> func = s => s.GetType().GetProperty(property).GetValue(s, null);

                                //方法二： 是用Expression完成的，加了缓存，效率会高很多
                                ConcurrentDictionary<string, Delegate> exp_handlers = new ConcurrentDictionary<string, Delegate>();
                                var key = typeof(Doc).FullName + "," + property;
                                var func = (Func<Doc, object>)exp_handlers.GetOrAdd(key, k =>
                                {
                                    var exp_param = Expression.Parameter(typeof(Doc), "param");
                                    var exp_property = Expression.Property(exp_param, property);
                                    var exp_return = Expression.Convert(exp_property, typeof(object));
                                    return Expression.Lambda<Func<Doc, object>>(Expression.Block(exp_property, exp_return), exp_param).Compile();
                                });

                                List<Doc> ProjDocList = dbsource.DContext.Doc.Where(d => d.O_dmsstatus != enDocStatus.Delete &&
                                       d.O_filetype != enFileType.ProjectModifyExcel && d.O_projectno == project.O_projectno &&
                                       d.O_cuversion == true).ToList<Doc>(); ;

                                #region 根据用户设置的条件，筛选需要显示的文档
                                //根据用户设置的条件，筛选需要显示的文档
                                bool EventState = false;
                                foreach (WebDocEvent.Before_Get_Doc_List_Event_Class BeforeGetDocs in WebDocEvent.ListBeforeGetDocs)
                                {
                                    if (BeforeGetDocs.Event != null)
                                    {
                                        //如果在用户事件中筛选过了，就跳出事件循环
                                        if (BeforeGetDocs.Event(BeforeGetDocs.PluginName, project, ref ProjDocList, ""))
                                        {
                                            break;
                                        }
                                    }
                                }
                                #endregion

                                if (direction == "ASC")
                                {
                                    //正序排序

                                    AllDocList = ProjDocList.OrderBy(func).
                                        Skip(CurPage * ShowNum).Take(ShowNum).ToList<Doc>();
                                    AllDocList.ForEach(d => d.dBSource = dbsource);


                                }
                                else
                                {
                                    //倒序排序

                                    AllDocList = ProjDocList.OrderByDescending(func).
                                     Skip(CurPage * ShowNum).Take(ShowNum).ToList<Doc>();
                                    AllDocList.ForEach(d => d.dBSource = dbsource);
                                }
                                #endregion

                            }



                        }

                        //组装JSON,并检查读取权限 
                        reJo.data = docListToJson(AllDocList, curUser);

                        //文档总数
                        reJo.total = project.DocCount;

                        reJo.success = true; 
                        #endregion
                    }
                    else
                    {

                        //界面上输入的条件
                        String strSQL = "";string strFilter = "";bool bSearchAttr = false;
                        JArray jaAttr = (JArray)JsonConvert.DeserializeObject(filterJson);
                        int index = 0;
                        foreach (JObject joAttr in jaAttr)
                        {

                            string strName = joAttr["name"].ToString();
                            string strValue = joAttr["value"].ToString();
                            if (strName == "filterstr")
                            {
                                strFilter = strValue;
                            }
                            else if (strName == "searchAttr")
                            {
                                if (strValue == "true") { bSearchAttr = true; }
                            }
                            else
                            {
                                if (index == 0)
                                {
                                    strSQL = strSQL + "(" + strValue + ") ";
                                }
                                else
                                {
                                    strSQL = strSQL + "AND (" + strValue + ") ";
                                }
                                index = index + 1;
                            }

                        }

                        //查找指定的目录下的文档
                        if (index == 0)
                            strSQL = strSQL + "(o_projectno IN (select [ID] from CDMS_FindSubDoc('" + project.O_projectno.ToString() + "'))) ";
                        else
                            strSQL = strSQL + "AND (o_projectno IN (select [ID] from CDMS_FindSubDoc('" + project.O_projectno.ToString() + "'))) ";

                        ///如果是搜索属性，就要把前面的搜索条件去掉，获取目录下的所有文档
                        //if (bSearchAttr == true) {
                        //    strSQL =  "(o_projectno IN (select [ID] from CDMS_FindSubDoc('" + project.O_projectno.ToString() + "'))) ";
                        //}

                        strSQL = dbsource.ParseExpression(project, strSQL);
                        strSQL = "Select * FROM CDMS_Doc Where " + strSQL;

                        
                        List<Doc> docList = dbsource.SelectDoc(strSQL, true);

                   

                        if (docList == null || (docList != null && docList.Count == 0))
                        {
                            reJo.msg = "没有满足条件的对象.";
                            reJo.success = true;
                            return reJo.Value;
                        }
                        else
                        {

                            #region 根据用户设置的条件，筛选需要显示的文档
                            //根据用户设置的条件，筛选需要显示的文档
                            foreach (WebDocEvent.Before_Get_Doc_List_Event_Class BeforeGetDocs in WebDocEvent.ListBeforeGetDocs)
                            {
                                if (BeforeGetDocs.Event != null)
                                {
                                    //如果在用户事件中筛选过了，就跳出事件循环
                                    if (BeforeGetDocs.Event(BeforeGetDocs.PluginName, project, ref docList, strFilter))
                                    {
                                        break;
                                    }
                                }
                            }
                            #endregion

                            #region 分页筛选
                            AllDocList = new List<Doc>();

                            int indexFilter = 0;
                            foreach (Doc doc in docList)
                            {
                                indexFilter = indexFilter + 1;
                                if (indexFilter > ShowNum * (CurPage) && indexFilter <= ShowNum * (CurPage +1))
                                {
                                    AllDocList.Add(doc);
                                }
                                if (indexFilter >= ShowNum * (CurPage +1))
                                {
                                    break;
                                }
                            } 
                            #endregion

                            //组装JSON,并检查读取权限 
                            reJo.data = docListToJson(AllDocList, curUser);
                           // reJo.data = docListToJson(docList, curUser);
                            reJo.success = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(e.Message);
            }
            return reJo.Value;

        }

        //private static IList<T> IListOrderBy<T>(IList<T> list, string propertyName) where T : new()
        //{
        //    if (list == null || list.Count == 0) return list;
        //    Type elementType = list[0].GetType();
        //    PropertyInfo propertyInfo=elementType.GetProperty(propertyName);

        //    ParameterExpression parameter = Expression.Parameter(elementType,"");
        //    Expression body = Expression.Property(parameter,propertyInfo);

        //    Expression sourceExpression = list.AsQueryable().Expression;

        //    Type sourcePropertyType = propertyInfo.PropertyType;

        //    Expression lambda = Expression.Call(
        //        typeof(Queryable), "OrderBy",
        //        new Type[] { elementType, sourcePropertyType },
        //        sourceExpression,Expression.Lambda(body,parameter)
        //        );
        //    return list.AsQueryable().Provider.CreateQuery<T>(lambda).ToList<T>();
        //}

        private static object GetPropertyValue(object obj, string property)
        {
            System.Reflection.PropertyInfo propertyInfo = obj.GetType().GetProperty(property);
            return propertyInfo.GetValue(obj, null);
        }


        //文档列表转换成Jarray
        //参数：CheckRight：是否需要检查doc的读取权限
        private static JArray docListToJson(List<Doc> docList, User curUser)
        {
            JArray reJa = new JArray();
            //组装JSON 
            foreach (Doc doc in docList)
            {


                bool IsShort = false;  //快捷文档

                Doc ddoc = doc;


                //快捷文档
                if (doc.ShortCutDoc != null)
                {
                    ddoc = doc.ShortCutDoc;
                    IsShort = true;
                }

                //权限

                Right r = ddoc.acceData.GetRight(curUser);
                if (r != null && r.DRead)
                {
                    //文档状态
                    int ST = (int)ddoc.O_dmsstatus;
                    string O_dmsstatus_DESC = "";
                    if (ST == 1)//COMING_IN
                    {
                        O_dmsstatus_DESC = "正在检入";
                    }
                    else if (ST == 2)//IN 
                    {
                        O_dmsstatus_DESC = "检入";
                    }
                    else if (ST == 3)//GOING_OUT
                    {
                        O_dmsstatus_DESC = "正在检出";
                    }
                    else if (ST == 4)//OUT
                    {
                        O_dmsstatus_DESC = "检出";
                    }
                    else if (ST == 5)//OUT_BRIEFCASE
                    {
                        O_dmsstatus_DESC = "检出到公文包";
                    }
                    else if (ST == 6)//OUT_EXPORTED
                    {
                        O_dmsstatus_DESC = "检出到本机";
                    }
                    else if (ST == 7)//COPYING
                    {
                        O_dmsstatus_DESC = "正在拷贝";
                    }
                    else if (ST == 8)//IN_FINAL
                    {
                        O_dmsstatus_DESC = "最终状态";
                    }
                    else if (ST == 9)//Delete
                    {
                        O_dmsstatus_DESC = "已删除";
                    }
                    else if (ST == 10)//Hide
                    {
                        O_dmsstatus_DESC = "隐藏文件";
                    }

                    #region 1.不存在附件,则去掉附件图标
                    bool bHasAttachFiles = false;
                    if ((doc.AttachFileList == null || doc.AttachFileList.Count <= 0)
                        && (doc.DocVersionList == null || doc.DocVersionList.Count <= 0)
                        && (doc.RefDocList == null || doc.RefDocList.Count <= 0)
                        && (doc.RelationDocList == null || doc.RelationDocList.Count <= 0))
                    {
                        //TODO: 不存在附件,则去掉附件图标
                        bHasAttachFiles = false;
                    }
                    else {
                        //判断是否是主版本
                        if (doc.MainDoc == null )
                        {
                            bHasAttachFiles = true;
                        }
                        else if (doc.ID == doc.MainDoc.ID) {
                            bHasAttachFiles = true;
                        }
                        else if (!((doc.AttachFileList == null || doc.AttachFileList.Count <= 0)
                        && (doc.RefDocList == null || doc.RefDocList.Count <= 0)
                        && (doc.RelationDocList == null || doc.RelationDocList.Count <= 0)))
                        {
                            //如果不是主版本，但是有参考文件或附加文件，也显示加号
                            bHasAttachFiles = true;
                        }
                    }
                    #endregion

                    reJa.Add(new JObject 
                             { 
                                //new JProperty("Keyword",doc.KeyWord),//这里必须是doc.KeyWord,这个才是真实的文件编码
                                new JProperty("Keyword",doc.KeyWord),
                                new JProperty("O_itemno",ddoc.O_itemno),
                                new JProperty("Title",ddoc.ToString),
                                new JProperty("O_size",ddoc.FileSize),
                                //new JProperty("O_size",ddoc.O_size == null ? "0" : FormatFileSize(ddoc.O_size.Value)),///1024).ToString()+" KB"),
                                new JProperty("O_filename",ddoc.O_filename == null ? "" : ddoc.O_filename),
                                new JProperty("O_dmsstatus_DESC",O_dmsstatus_DESC),
                                new JProperty("O_version",ddoc.O_version),
                                new JProperty("Creater",(ddoc.Creater == null) ? "" : ddoc.Creater.ToString),
                                new JProperty("O_credatetime",((ddoc.O_credatetime == null) ? DateTime.MinValue : ddoc.O_credatetime).ToString()),
                                new JProperty("Updater",(ddoc.Updater == null) ? "" : ddoc.Updater.ToString),
                                new JProperty("O_updatetime",((DateTime)(ddoc.O_updatetime == null ? DateTime.MinValue : ddoc.O_updatetime)).ToString()),
                                new JProperty("O_outpath",ddoc.O_outpath == null ? "" : ddoc.O_outpath),
                                new JProperty("O_flocktime",((DateTime)(ddoc.O_flocktime == null ? DateTime.MinValue : ddoc.O_flocktime)).ToString()),
                                new JProperty("O_conode",ddoc.O_conode == null ? "" : ddoc.O_conode),
                                new JProperty("IsShort",IsShort ? "true":"false"),//1 : 0),
                                new JProperty("WriteRight",r.DWrite?"true":"false"),
                                new JProperty("HasAttachFiles",bHasAttachFiles)
                             });


                }
            }
            return reJa;
        }

        /// <summary>
        /// 根据DOC的Keyword返回一个Doc基本属性，在显示文档属性栏时使用
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="DocKeyword">DOC关键字</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时，data包含多个JObject， 每个JObject里面包含参数：</para>
        /// <para>主要属性包含的参数："AttrCode"：属性代码,"AttrName"：属性描述"AttrValue":属性值，"AttrType",属性类型("Attr":属性),"Visible":是否显示("True")</para>
        /// "ShowData"：下拉列表的子项
        /// </para>
        /// <para>例子：</para>
        /// <code>
        /// {
        ///  "success": true,
        ///  "total": 0,
        ///  "msg": "",
        ///  "data": [
        ///    {
        ///      "AttrCode": "O_filename",
        ///      "AttrName": "文件名",
        ///      "AttrValue": "7.填报工时.txt",
        ///      "AttrType": "Attr",
        ///      "Visible": "True"
        ///    },
        ///    {
        ///      "AttrCode": "O_itemname",
        ///      "AttrName": "文档",
        ///      "AttrValue": "7.填报工时.txt",
        ///      "AttrType": "Attr",
        ///      "Visible": "True"
        ///    },
        ///    {
        ///      "AttrCode": "O_itemdesc",
        ///      "AttrName": "文档描述",
        ///      "AttrValue": "",
        ///      "AttrType": "Attr",
        ///      "Visible": "True"
        ///    },
        ///    {
        ///      "AttrCode": "O_credatetime",
        ///      "AttrName": "创建时间",
        ///      "AttrValue": "2020/1/20 14:18:13",
        ///      "AttrType": "Attr",
        ///      "Visible": "True"
        ///    },
        ///    {
        ///      "AttrCode": "O_updatetime",
        ///      "AttrName": "属性更新时间",
        ///      "AttrValue": "2020/1/20 14:18:35",
        ///      "AttrType": "Attr",
        ///      "Visible": "True"
        ///    },
        ///    {
        ///      "AttrCode": "O_dmsstatus",
        ///      "AttrName": "文件状态",
        ///      "AttrValue": "检入",
        ///      "AttrType": "Attr",
        ///      "Visible": "True"
        ///    },
        ///    {
        ///      "AttrCode": ".O_dmsdate",
        ///      "AttrName": "文件编辑时间",
        ///      "AttrValue": "",
        ///      "AttrType": "Attr",
        ///      "Visible": "True"
        ///    },
        ///    {
        ///      "AttrCode": "O_fupdatetime",
        ///      "AttrName": "文件更新时间",
        ///      "AttrValue": "",
        ///      "AttrType": "Attr",
        ///      "Visible": "True"
        ///    },
        ///    {
        ///      "AttrCode": "O_flocktime",
        ///      "AttrName": "文件锁定时间",
        ///      "AttrValue": "",
        ///      "AttrType": "Attr",
        ///      "Visible": "True"
        ///    },
        ///    {
        ///      "AttrCode": "FileSize",
        ///      "AttrName": "文件大小",
        ///      "AttrValue": "339.00 B",
        ///      "AttrType": "Attr",
        ///      "Visible": "True"
        ///    },
        ///    {
        ///      "AttrCode": "O_filetype",
        ///      "AttrName": "文件类型",
        ///      "AttrValue": "Nomal",
        ///      "AttrType": "Attr",
        ///      "Visible": "True"
        ///    },
        ///    {
        ///      "AttrCode": "O_WorkFlowno",    //判断是否显示流程页
        ///      "AttrName": "流程ID",
        ///      "AttrValue": "13136",
        ///      "AttrType": "Attr",
        ///      "Visible": "True"
        ///    },
        ///    {
        ///      "AttrCode": "Creater",
        ///      "AttrName": "创建者",
        ///      "AttrValue": "admin__administrator",
        ///      "AttrType": "Attr",
        ///      "Visible": "True"
        ///    },
        ///    {
        ///      "AttrCode": "DMSUser",
        ///      "AttrName": "文件编辑者",
        ///      "AttrValue": "",
        ///      "AttrType": "Attr",
        ///      "Visible": "True"
        ///    },
        ///    {
        ///      "AttrCode": "FLocker",
        ///      "AttrName": "文件锁定者",
        ///      "AttrValue": "",
        ///      "AttrType": "Attr",
        ///      "Visible": "True"
        ///    },
        ///    {
        ///      "AttrCode": "Manager",
        ///      "AttrName": "管理者",
        ///      "AttrValue": "admin__administrator",
        ///      "AttrType": "Attr",
        ///      "Visible": "True"
        ///    },
        ///    {
        ///      "AttrCode": "Updater",
        ///      "AttrName": "属性更新者",
        ///      "AttrValue": "admin__administrator",
        ///      "AttrType": "Attr",
        ///      "Visible": "True"
        ///    },
        ///    {
        ///      "AttrCode": "TempDef",
        ///      "AttrName": "模板",
        ///      "AttrValue": "",
        ///      "AttrType": "Attr",
        ///      "Visible": "True"
        ///    },
        ///    {
        ///      "AttrCode": "AttrDataCount",   //判断是否显示文档附加属性页
        ///      "AttrName": "附加属性数量",
        ///      "AttrValue": "0",
        ///      "AttrType": "Attr",
        ///      "Visible": "False"
        ///    },
        ///    {
        ///      "AttrCode": "hasDFWriteRight",
        ///      "AttrName": "替换文件权限",
        ///      "AttrValue": "True",
        ///      "AttrType": "Attr",
        ///      "Visible": "False"
        ///    },
        ///    {
        ///      "AttrCode": "hasEditRight",
        ///      "AttrName": "文档修改权限",
        ///      "AttrValue": "True",
        ///      "AttrType": "Attr",
        ///      "Visible": "False"
        ///    },
        ///    {
        ///      "AttrCode": "isAdmin",     //判断是否显示权限页
        ///      "AttrName": "是否管理员",
        ///      "AttrValue": "True",
        ///      "AttrType": "Attr",
        ///      "Visible": "False"
        ///    }
        ///  ]
        /// }
        /// </code>
        /// </returns>
        public static JObject GetDocBaseAttrByKeyword(string sid, string DocKeyword)
        {
            ExReJObject reJo = new ExReJObject();

            try
            {

                if (string.IsNullOrEmpty(DocKeyword))
                {
                    reJo.msg = "错误的提交数据,文档不存在。";
                    return reJo.Value;
                }



                //登录用户
                User curUser = DBSourceController.GetCurrentUser(sid);
                if (curUser == null) //return null;
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }


                DBSource dbsource = curUser.dBSource;

                //获取目录对象
                Doc ddoc = dbsource.GetDocByKeyWord(DocKeyword);
                if (ddoc == null) //return null;
                {
                    reJo.msg = "错误的提交数据,文档不存在。";
                    return reJo.Value;
                }
                else
                {
                    Doc doc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;

                    JArray jaResult = new JArray();

                    //判断是否有修改文件模板权限
                    //有修改文档权限才可以修改文档属性
                    //以父目录的权限来限定当前文档属性的修改权 
                    bool hasDWrite = GetDocDCntrlRight(doc, curUser);

                    //文件名
                    jaResult.Add(new JObject { 
                                new JProperty("AttrCode","O_filename"),
                                new JProperty("AttrName","文件名"),
                                new JProperty("AttrValue",doc.O_filename),
                                new JProperty("AttrType","Attr"),
                                new JProperty("Visible","True"),
                            });



                    //文档
                    jaResult.Add(new JObject { 
                                new JProperty("AttrCode","O_itemname"),
                                new JProperty("AttrName","文档"),
                                new JProperty("AttrValue",doc.O_itemname),
                                new JProperty("AttrType","Attr"),
                                new JProperty("Visible","True"),
                            });


                    //文档描述
                    jaResult.Add(new JObject { 
                                new JProperty("AttrCode","O_itemdesc"),
                                new JProperty("AttrName","文档描述"),
                                new JProperty("AttrValue",doc.O_itemdesc),
                                new JProperty("AttrType","Attr"),
                                new JProperty("Visible","True"),
                            });


                    //创建时间
                    jaResult.Add(new JObject { 
                                new JProperty("AttrCode","O_credatetime"),
                                new JProperty("AttrName","创建时间"),
                                new JProperty("AttrValue",doc.O_credatetime == null ? "" : doc.O_credatetime.ToString()),
                                new JProperty("AttrType","Attr"),
                                new JProperty("Visible","True"),
                            });


                    //属性更新时间
                    jaResult.Add(new JObject { 
                                new JProperty("AttrCode","O_updatetime"),
                                new JProperty("AttrName","属性更新时间"),
                                new JProperty("AttrValue",doc.O_updatetime == null ? "" : doc.O_updatetime.ToString()),
                                new JProperty("AttrType","Attr"),
                                new JProperty("Visible","True"),
                            });


                    //文档状态
                    string O_dmsstatus_DESC = doc.O_dmsstatus.ToString();
                    if (doc.O_dmsstatus == enDocStatus.IN)
                    {
                        O_dmsstatus_DESC = "检入";
                    }
                    else if (doc.O_dmsstatus == enDocStatus.OUT)
                    {
                        O_dmsstatus_DESC = "检出";
                    }
                    else if (doc.O_dmsstatus == enDocStatus.IN_FINAL)
                    {
                        O_dmsstatus_DESC = "最终状态";
                    }


                    //文件状态
                    jaResult.Add(new JObject { 
                                new JProperty("AttrCode","O_dmsstatus"),//doc.O_dmsstatus.ToString()),
                                new JProperty("AttrName","文件状态"),
                                new JProperty("AttrValue",O_dmsstatus_DESC),
                                new JProperty("AttrType","Attr"),
                                new JProperty("Visible","True"),
                            });


                    //文件编辑时间
                    jaResult.Add(new JObject { 
                                new JProperty("AttrCode",".O_dmsdate"),//doc.O_dmsdate.ToString()),
                                new JProperty("AttrName","文件编辑时间"),
                                new JProperty("AttrValue",doc.O_dmsdate == null ? "" : doc.O_dmsdate.ToString()),
                                new JProperty("AttrType","Attr"),
                                new JProperty("Visible","True"),
                            });



                    //文件更新时间
                    jaResult.Add(new JObject { 
                                new JProperty("AttrCode","O_fupdatetime"),//doc.O_fupdatetime.ToString()),
                                new JProperty("AttrName","文件更新时间"),
                                new JProperty("AttrValue",doc.O_fupdatetime == null ? "" : doc.O_fupdatetime.ToString()),
                                new JProperty("AttrType","Attr"),
                                new JProperty("Visible","True"),
                            });




                    //文件锁定时间
                    jaResult.Add(new JObject { 
                                new JProperty("AttrCode","O_flocktime"),//doc.O_flocktime.ToString()),
                                new JProperty("AttrName","文件锁定时间"),
                                new JProperty("AttrValue",doc.O_flocktime == null ? "" : doc.O_flocktime.ToString()),
                                new JProperty("AttrType","Attr"),
                                new JProperty("Visible","True"),
                            });




                    //文件大小
                    jaResult.Add(new JObject { 
                                new JProperty("AttrCode","FileSize"),//doc.FileSize.ToString()),
                                new JProperty("AttrName","文件大小"),
                                new JProperty("AttrValue",doc.FileSize),
                                new JProperty("AttrType","Attr"),
                                new JProperty("Visible","True"),
                            });


                    //文件类型
                    jaResult.Add(new JObject { 
                                new JProperty("AttrCode","O_filetype"),//doc.O_filetype.ToString()),
                                new JProperty("AttrName","文件类型"),
                                new JProperty("AttrValue",((enDocType)doc.O_filetype).ToString()),
                                new JProperty("AttrType","Attr"),
                                new JProperty("Visible","True"),
                            });


                    //流程号
                    jaResult.Add(new JObject { 
                                new JProperty("AttrCode","O_WorkFlowno"),//doc.O_WorkFlowno.ToString()),
                                new JProperty("AttrName","流程ID"),
                                new JProperty("AttrValue",doc.O_WorkFlowno == null ? "" : doc.O_WorkFlowno.ToString()),
                                new JProperty("AttrType","Attr"),
                                new JProperty("Visible","True"),
                            });


                    //创建者
                    jaResult.Add(new JObject { 
                                new JProperty("AttrCode","Creater"),
                                new JProperty("AttrName","创建者"),
                                new JProperty("AttrValue",doc.Creater == null ? "" : doc.Creater.ToString),
                                new JProperty("AttrType","Attr"),
                                new JProperty("Visible","True"),
                            });


                    //文件编辑者
                    jaResult.Add(new JObject { 
                                new JProperty("AttrCode","DMSUser"),
                                new JProperty("AttrName","文件编辑者"),
                                new JProperty("AttrValue",doc.DMSUser == null ? "" : doc.DMSUser.ToString),
                                new JProperty("AttrType","Attr"),
                                new JProperty("Visible","True"),
                            });


                    //文件锁定者
                    jaResult.Add(new JObject { 
                                new JProperty("AttrCode","FLocker"),
                                new JProperty("AttrName","文件锁定者"),
                                new JProperty("AttrValue",doc.FLocker == null ? "" : doc.FLocker.ToString),
                                new JProperty("AttrType","Attr"),
                                new JProperty("Visible","True"),
                            });


                    //管理者
                    jaResult.Add(new JObject { 
                                new JProperty("AttrCode","Manager"),
                                new JProperty("AttrName","管理者"),
                                new JProperty("AttrValue",doc.Manager == null ? "" : doc.Manager.ToString),
                                new JProperty("AttrType","Attr"),
                                new JProperty("Visible","True"),
                            });


                    //属性更新者
                    jaResult.Add(new JObject { 
                                new JProperty("AttrCode","Updater"),
                                new JProperty("AttrName","属性更新者"),
                                new JProperty("AttrValue",doc.Updater == null ? "" : doc.Updater.ToString),
                                new JProperty("AttrType","Attr"),
                                new JProperty("Visible","True"),
                            });

                    string strTempDef = "";
                    if (doc.TempDefn != null)
                        strTempDef = doc.TempDefn.Code + "_" + doc.TempDefn.Description;
                    //文档模板
                    jaResult.Add(new JObject { 
                                new JProperty("AttrCode","TempDef"),
                                new JProperty("AttrName","模板"),
                                new JProperty("AttrValue",strTempDef),
                                new JProperty("AttrType","Attr"),
                                new JProperty("Visible","True"),
                            });

                    //附加属性数量
                    jaResult.Add(new JObject { 
                                new JProperty("AttrCode","AttrDataCount"),
                                new JProperty("AttrName","附加属性数量"),
                                new JProperty("AttrValue",doc.AttrDataList.Count().ToString()),
                                new JProperty("AttrType","Attr"),
                                new JProperty("Visible","False"),
                            });

                    //判断是否有替换文件权限
                    bool hasDFWrite = ProjectController.GetProjectDFWriteRight(doc.Project, curUser);

                    //文档写权限
                    jaResult.Add(new JObject { 
                                    new JProperty("AttrCode","hasDFWriteRight"),
                                    new JProperty("AttrName","替换文件权限"),
                                    new JProperty("AttrValue",hasDFWrite?"True":"False"),
                                    new JProperty("AttrType","Attr"),
                                    new JProperty("Visible","False"),
                                });

                    //判断是否有修改文件模板权限（文档属性修改权限）
                    jaResult.Add(new JObject { 
                                    new JProperty("AttrCode","hasEditRight"),
                                    new JProperty("AttrName","文档修改权限"),
                                    new JProperty("AttrValue",hasDWrite?"True":"False"),
                                    new JProperty("AttrType","Attr"),
                                    new JProperty("Visible","False"),
                                });

                    //判断是否管理员
                    jaResult.Add(new JObject { 
                                    new JProperty("AttrCode","isAdmin"),
                                    new JProperty("AttrName","是否管理员"),
                                    new JProperty("AttrValue",curUser.IsAdmin?"True":"False"),
                                    new JProperty("AttrType","Attr"),
                                    new JProperty("Visible","False"),
                                });

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
        /// 根据DOC的Keyword返回一个Doc附加属性，在显示文档附加属性栏时使用
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="DocKeyword">DOC关键字</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时，data包含多个JObject， 每个JObject里面包含参数：</para>
        /// <para>附加属性包含的参数："AttrCode"：属性代码,"AttrName"：属性描述"AttrValue":属性值，"AttrType",属性类型("AddiAttr":附加属性),"Visible":是否显示("True")，
        /// "TempAttrType"：附加属性的类别(Common = 3, User = 4)，"DataType"：附加属性的数据类型类别("user":用户类型),"DefaultCode"：编辑属性的默认值，在下拉选择时用到，
        /// "ShowData"：下拉列表的子项
        /// </para>
        /// <para>例子：</para>
        /// </returns>
        public static JObject GetDocAttrDataByKeyword(string sid, string DocKeyword)
        {
            ExReJObject reJo = new ExReJObject();

            try
            {

                if (string.IsNullOrEmpty(DocKeyword))
                {
                    reJo.msg = "错误的提交数据,文档不存在。";
                    return reJo.Value;
                }


                //登录用户
                User curUser = DBSourceController.GetCurrentUser(sid);
                if (curUser == null) //return null;
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

                DBSource dbsource = curUser.dBSource;


                //获取目录对象
                Doc ddoc = dbsource.GetDocByKeyWord(DocKeyword);
                if (ddoc == null) 
                {
                    reJo.msg = "错误的提交数据,文档不存在。";
                    return reJo.Value;
                }
                else
                {
                    Doc doc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;

                    JArray jaResult = new JArray();

                    //判断是否有修改文件模板权限
                    //有修改文档权限才可以修改文档属性
                    //以父目录的权限来限定当前文档属性的修改权 

                    bool hasDWrite = GetDocDCntrlRight(doc, curUser);

                    //添加附加属性
                    if (doc.AttrDataList != null && doc.AttrDataList.Count() > 0)
                    {
                        jaResult = AttrDataController.GetAttrDataListJson(doc.AttrDataList, hasDWrite);

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
        /// 新建一个Doc对象，返回Doc对象及附加属性的详细JSON字符串
        /// </summary>
        /// <param name="sid">登录关键字</param>
        /// <param name="keyword"> Doc keyword</param>
        /// <returns>返回Doc对象及AttrData对象的JSON字符串</returns>
        private static Doc NewDoc(string sid, string keyword, string mFileName, string mItemName, string mItemDesc)
        {
            try
            {

                //登录用户
                User curUser = DBSourceController.GetCurrentUser(sid);
                if (curUser == null) return null;

                //新建了doc，必须使用newDbsource
                DBSource dbsource = curUser.dBSource;


                Project proj = dbsource.GetProjectByKeyWord(keyword);


                //获取目录对象
                Doc doc = proj.NewDoc(mFileName, mItemName, mItemDesc);
                if (doc == null) return null;
                return doc;

            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
            }

            return null;
        }

        
        /// <summary>
        /// 删除文档
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="ProjectKeyword">Project关键字,这里传Project过来，是因为要判断doc是否是快捷方式，
        /// 如果不传project并且doc是快捷方式，就会直接删除DOC原文件，而不是删除快捷方式了</param>
        /// <param name="DocKeyword">Doc关键字</param>
        /// <param name="sureDel">确认是否删除，"false"或"true"</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时，data包含一个JObject，里面包含参数"state"；</para>
        /// <para>当 "state"的值为"seleSureDel"时，表示提示客户端选择是否确认删除</para>
        /// <para>当 "state"的值为"delSuccess"时，表示删除成功</para>
        /// <para>例子：</para>
        /// </returns>
        public static JObject DeleteDoc(string sid, string ProjectKeyword, string DocKeyword, string sureDel)
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


                //获取文档对应的Project对象
                Project project = dbsource.GetProjectByKeyWord(ProjectKeyword);
                if (project == null)
                {
                    reJo.msg = "删除文档的时候出现错误：所在的目录不存在！";
                    return reJo.Value;
                }


                if (string.IsNullOrEmpty(DocKeyword) || project == null || project.DocList == null || project.DocList.Count <= 0)
                {
                    reJo.msg = "删除文档的时候出现错误：文档不存在！";
                    return reJo.Value;
                }

                Doc curDoc = dbsource.GetDocByKeyWord(DocKeyword);
                if (curDoc == null)
                {
                    reJo.msg = "删除文档的时候出现错误：文档不存在！";
                    return reJo.Value;
                }


                //判断该Doc是不是对应于一个快捷Doc对象
                if (!project.DocList.Contains(curDoc))
                {
                    Doc shortDoc = new Doc();
                    bool isShortDoc = false;
                    //可能就是curDoc就是替换了某个快捷Doc对象的实在对象,需要找到该快捷Doc,并删除该快捷Doc
                    foreach (Doc doc in project.DocList)
                    {
                        if (doc.ShortCutDoc != null && doc.ShortCutDoc == curDoc)
                        {
                            shortDoc = doc;
                            isShortDoc = true;
                            break;
                        }
                    }

                    bool hasDDeleteRight = ProjectController.GetProjectDDeleteRight(project, curUser);
                    if (!hasDDeleteRight)
                    {
                        reJo.msg = "您无权删除快捷方式 " + curDoc.O_itemname + " !";
                        return reJo.Value;
                    }

                    if (isShortDoc && sureDel == "false")
                    {
                        reJo.msg = "确认要删除该快捷方式？";
                        reJo.success = true;
                        reJo.data = new JArray(new JObject(new JProperty("state", "seleSureDel")));//选择确定删除
                        return reJo.Value;
                    }
                    else if (isShortDoc)
                    {
                        //删除快捷方式
                        shortDoc.Delete();

                        //所在目录的文档列表移除本文档
                        if (project.DocList.Contains(shortDoc))
                        {
                            List<Doc> docList = project.DocList;
                            docList.Remove(shortDoc);
                            project.DocList = docList;
                        }
                        reJo.success = true;
                        reJo.data = new JArray(new JObject(new JProperty("state", "delSuccess")));//返回删除成功消息给客户端
                        return reJo.Value;
                    }
                }

                //非快捷方式文档
                else
                {
                    bool hasDDeleteRight = DocController.GetDocDDeleteRight(curDoc, curUser);
                    if (!hasDDeleteRight)
                    {
                        reJo.msg = "您无权删除文档 " + curDoc.O_itemname + " !";
                        return reJo.Value;
                    }


                    if (curDoc != null && curDoc.O_dmsstatus != (enDocStatus)2)
                    {
                        reJo.msg = "文档 " + curDoc.O_itemname + " 被锁定,无法删除!";
                        return reJo.Value;
                    }

                    if (sureDel == "false")
                    {
                        reJo.msg = "确认要删除该文档？";
                        reJo.success = true;
                        reJo.data = new JArray(new JObject(new JProperty("state", "seleSureDel")));//选择确定删除
                        return reJo.Value;
                    }

                    if (curDoc != null && curDoc.O_dmsstatus == (enDocStatus)2)
                    {

                        List<string> attachFileList = curDoc.AttachFileList;


                        if (!curDoc.Delete())
                        {
                            reJo.msg = "删除文档 " + curDoc.O_itemname + " 失败!";
                            return reJo.Value;
                        }

                        /*curDoc->Modify();*/

                        //所在目录的文档列表移除本文档
                        if (curDoc.Project.DocList != null && curDoc.Project.DocList.Contains(curDoc))
                        {
                            List<Doc> docList = curDoc.Project.DocList;
                            docList.Remove(curDoc);
                            curDoc.Project.DocList = docList;
                        }

                        reJo.success = true;
                        reJo.data = new JArray(new JObject(new JProperty("state", "delSuccess")));//返回删除成功消息给客户端
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
        /// 放弃文档
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="DocKeyword">Doc关键字</param>
        /// <param name="sureFree">确认是否放弃，"false"或"true"</param>
        /// <returns></returns>
        public static JObject FreeDoc(string sid, string DocKeyword, string sureFree)
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

                Doc doc = dbsource.GetDocByKeyWord(DocKeyword);
                if (doc == null)
                {
                    reJo.msg = "放弃文档的时候出现错误：文档不存在！";
                    return reJo.Value;
                }

                if (sureFree == "false")
                {
                    reJo.msg = "确认要放弃该文档？";
                    reJo.success = true;
                    reJo.data = new JArray(new JObject(new JProperty("state", "seleSureFree")));//选择确定删除
                    return reJo.Value;
                }
                
                bool hasDFullRight = DocController.GetDocDFullRight(doc, sid);

                if (!(((doc.O_dmsstatus == (enDocStatus)4 || doc.O_dmsstatus == (enDocStatus.OUT_EXPORTED) || doc.O_dmsstatus == (enDocStatus)1) && doc.FLocker == doc.dBSource.LoginUser/*输出到了本地*/) || ((doc.dBSource.LoginUser.IsAdmin || hasDFullRight) && doc.O_dmsstatus != (enDocStatus)2 && doc.O_dmsstatus != (enDocStatus.IN_FINAL))))
                {
                    reJo.msg = "放弃文档的时候出现错误：用户没有文档放弃权限！";
                    return reJo.Value;
                }
                doc.OperateDocStatus = (enDocStatus) 2 ;


                reJo.success = true;
                reJo.data = new JArray(new JObject(new JProperty("state", "freeSuccess")));//返回放弃成功消息给客户端
            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(reJo.msg);
            }
            return reJo.Value;
        }
        /// <summary>
        /// 判断文件是否为图片
        /// </summary>
        /// <param name="path">文件的完整路径</param>
        /// <returns>返回结果</returns>
        private static Boolean IsImage(string path)
        {
            try
            {                
                System.Drawing.Image img = System.Drawing.Image.FromFile(path);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        } 



        /// <summary>
        /// Checks the file is textfile or not.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        private static bool CheckIsTextFile(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            bool isTextFile = true;
            try
            {
                int i = 0;
                int length = (int)fs.Length;
                byte data;
                while (i < length && isTextFile && i < 5)
                {
                    data = (byte)fs.ReadByte();
                    isTextFile = (data != 0);
                    i++;
                }
                return isTextFile;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }


        /// <summary>
        /// 预览文档
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="DocKeyword">Doc关键字</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功且预览文件为压缩文件时，data包含一个JObject，里面包含参数"filetype"：文件类型值为"rar"，"path"：文件解压路径，
        /// "filelist"：解压后文件列表，"isUnrar":是否压缩文件值为"true"</para>
        /// <para>其中"filelist"是一个JArray字符串，包含参数："type"：解压后文档或目录类型（"dir"，"file"），"name"：文件或目录名称，"size",
        /// "path":路径，"parentpath":父目录路径</para>
        /// <para>操作成功且预览文件为word，excel,pdf,文本，图片等文档时，data包含一个JObject，里面包含参数"filetype"：文件类型，"path"：文件预览路径，
        /// "isUnrar":是否解压缩文件后的文件（值为"true"或"false"）</para>
        /// <para>例子：</para>
        /// </returns>
        /// 
        public static JObject PreviewDoc(string sid, string DocKeyword)
        {
            ExReJObject reJo = new ExReJObject();
            try
            {
                if (string.IsNullOrEmpty(sid))
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

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



                //获取文档对象
                
                Doc ddoc = dbsource.GetDocByKeyWord(DocKeyword);

                Doc curDoc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;

                //判断是否有打开文件的权限
                bool ReadRight = DocController.GetDocDFReadRight(curDoc, curUser);
                if (ReadRight == false)
                {
                    reJo.msg = "当前用户没有权限查看该文档:"+curDoc.ToString + "！";
                    return reJo.Value;
                }

                #region 预览文档前触发的事件 ，PVRight被修改为false时不能预览文档
                //预览文档前触发的事件 ，PVRight被修改为false时不能预览文档
                foreach (WebDocEvent.Before_Preview_Doc_Event_Class BeforePreviewDoc in WebDocEvent.ListBeforePreviewDoc)
                {
                    if (BeforePreviewDoc.Event != null)
                    {
                        //如果在用户事件中筛选过了，就跳出事件循环
                        if (BeforePreviewDoc.Event(BeforePreviewDoc.PluginName, curDoc, ref ReadRight))
                        //if (BeforePreviewDoc.Event(curDoc, ref ReadRight))
                        {
                            break;
                        }
                    }
                }
                if (ReadRight == false) {
                    reJo.msg = "当前用户没有权限查看该文档:" + curDoc.ToString + "！";
                    return reJo.Value;
                }
                #endregion

                //共享网络，在WEB服务器上把Storage 设置为共享，如果Storage就在WEB服务器，不共享
                //string WorkingPath = string.IsNullOrEmpty(curDoc.Storage.O_protocol) ? curDoc.Storage.O_path : curDoc.Storage.O_protocol;
                //if (!WorkingPath.EndsWith("\\") && !WorkingPath.EndsWith("/"))
                //{
                //    WorkingPath = WorkingPath + "\\" + curDoc.Project.O_projectcode + "\\";
                //}
                string WorkingPath = curDoc.Storage.O_protocol + (curDoc.Storage.O_protocol.EndsWith("\\") || curDoc.Storage.O_protocol.EndsWith("/") ? "" : "\\") + curDoc.Project.O_projectcode + "\\";

                //文件名称
                string sourceFileName = WorkingPath + curDoc.O_filename;

                if (!System.IO.File.Exists(sourceFileName))
                {
                    reJo.msg = "文件不存在！";
                    return reJo.Value;
                }

                //获取扩展名
                string ext = sourceFileName.Substring(sourceFileName.LastIndexOf(".") + 1).ToLower(); //扩展名

                //获取文件的Mime类型，根据Mime类型选择预览的方式
                string contentType = MimeMapping.GetMimeMapping(sourceFileName);


                //WEB文件路径
                string webFile = "";


                //是否压缩文件
                string isUnrar = "false";


                //获取不同类型文件的WEB路径
                if (ext == "pdf")
                {
                    //下载文件限制：sid加文档权限加下载链接有效时间
                    DateTime dt = DateTime.Now;
                    string para = AVEVA.CDMS.WebApi.EnterPointController.enProcessRequest(sid + "___" + DocKeyword + "___" + dt.ToString().Replace("/", "-"));

                    webFile = "Scripts/PDFJSInNet/web/viewer.html?file=/" + curDoc.Storage.O_storname + "/" +
                        curDoc.Project.O_projectcode + "/" + HttpClient.EncodeUrl(curDoc.O_filename);//添加PDFjs路径


                    webFile = webFile + "&p=" + para;
                }


                //office文件
                else if (ext == "doc" || ext == "docx" || ext == "xls" || ext == "xlsx" || ext == "dwg")
                {

                    //Office文件转换为PDF ， 直接在 CDMS0001 下建立一个目录 PDF， 放置PDF文件
                    string pdfFileName = WorkingPath + "pdf\\" + curDoc.O_filename;
                    pdfFileName = pdfFileName.Substring(0, pdfFileName.Length - ext.Length - 1) + ".pdf";

                    //替换#符号为全角，否则显示不了pdf预览
                    pdfFileName = pdfFileName.Replace("#", "＃");

                    //源office文件更新日期
                    FileInfo srcFileInfo = new FileInfo(sourceFileName);
                    DateTime srcUpdateTime = srcFileInfo.LastWriteTime;


                    //生成的预览文件名
                    bool mustConvertToPdf = true;
                    if (System.IO.File.Exists(pdfFileName))
                    {
                        //判断是否需要转换为PDF文件
                        //pdf文件更新日期
                        FileInfo pdfFileInfo = new FileInfo(pdfFileName);
                        DateTime pdfUpdateTime = pdfFileInfo.LastWriteTime;


                        //判断源文件修改日期是否比pdf预览文件要新
                        if (pdfUpdateTime > srcUpdateTime)
                        {
                            mustConvertToPdf = false;
                        }
                    }


                    //需要转换为pdf文件
                    if (mustConvertToPdf)
                    {

                        //创建一个PDF目录
                        if (!Directory.Exists(WorkingPath + "pdf")) Directory.CreateDirectory(WorkingPath + "pdf");

                        CDMSPdf.ConvertToPdf(sourceFileName, pdfFileName);


                        //再修改源文件时间
                        System.IO.File.SetLastWriteTime(sourceFileName, srcUpdateTime);

                    }

                    //添加PDFjs路径
                    webFile = "Scripts/PDFJSInNet/web/viewer.html?file=/" + curDoc.Storage.O_storname + "/" + curDoc.Project.O_projectcode + "/pdf/" + // + pdfFileName;
                        (curDoc.O_filename.Substring(0, curDoc.O_filename.Length - ext.Length)).Replace("#", "＃") + "pdf"; //;//添加PDFjs路径


                    //下载文件限制：sid加文档权限加下载链接有效时间
                    DateTime dt = DateTime.Now;
                    string para = AVEVA.CDMS.WebApi.EnterPointController.enProcessRequest(sid + "___" + DocKeyword + "___" + dt.ToString().Replace("/", "-"));

                    webFile = webFile + "&p=" + para;

                    //添加时间戳，强制更新浏览器pdf缓存
                    System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
                    long t = (DateTime.Now.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
                    webFile = webFile + "&time=" + t.ToString();
                    //webFile = "Scripts/PDFJSInNet/web/viewer.html?file=/" + curDoc.Storage.O_storname + "/" + curDoc.Project.O_projectcode + "/pdf/" +
                    //    HttpClient.EncodeUrl(curDoc.O_filename.Substring(0, curDoc.O_filename.Length - ext.Length - 1)) + ".pdf"; //;//添加PDFjs路径

                }

                //thm模型文件
                else if (ext == "thm")
                {

                    webFile = "Scripts/WebGL/index.html?modelUrl=/" + curDoc.Storage.O_storname + "/" + curDoc.Project.O_projectcode + "/" +
                        HttpClient.EncodeUrl(curDoc.O_filename);//添加BIM模型路径
                }

                else if (ext == "txt" || contentType == "text/xml")
                {
                    //Office文件转换为PDF ， 直接在 CDMS0001 下建立一个目录 PDF， 放置PDF文件
                    string pdfFileName = WorkingPath + "pdf\\" + curDoc.O_filename;
                    pdfFileName = pdfFileName.Substring(0, pdfFileName.Length - ext.Length - 1) + ".pdf";

                    //替换#符号为全角，否则显示不了pdf预览
                    pdfFileName = pdfFileName.Replace("#", "＃");

                    //源office文件更新日期
                    FileInfo srcFileInfo = new FileInfo(sourceFileName);
                    DateTime srcUpdateTime = srcFileInfo.LastWriteTime;


                    //生成的预览文件名
                    bool mustConvertToPdf = true;
                    if (System.IO.File.Exists(pdfFileName))
                    {
                        //判断是否需要转换为PDF文件
                        //pdf文件更新日期
                        FileInfo pdfFileInfo = new FileInfo(pdfFileName);
                        DateTime pdfUpdateTime = pdfFileInfo.LastWriteTime;


                        //判断源文件修改日期是否比pdf预览文件要新
                        if (pdfUpdateTime > srcUpdateTime)
                        {
                            mustConvertToPdf = false;
                        }
                    }

                    //需要转换为pdf文件
                    if (mustConvertToPdf)
                    {
                        //创建一个PDF目录
                        if (!Directory.Exists(WorkingPath + "pdf")) Directory.CreateDirectory(WorkingPath + "pdf");

                        //设置文本的字体大小及颜色
                        int r = 0;
                        int g = 0;
                        int b = 0;
                        int size = 12;

                        string txtFileContent="";
                        //读取文本文件
                        String line;
                        StreamReader sb = new StreamReader(sourceFileName);
                        //int lineIndex = 0;
                        while ((line = sb.ReadLine()) != null)
                        {
                            String lines;
                            lines = line;
                            txtFileContent = txtFileContent + lines+"\r\n";

                            //richTextBox1.AppendText(lines);
                            //richTextBox1.Focus();
                        }
                        sb.Close();

                        //转换为pdf文件
                        Document document = new Document();
                        PdfWriter.GetInstance(document, new FileStream(pdfFileName, FileMode.Create));
                        document.Open();
                        BaseFont bfChinese = BaseFont.CreateFont("C://WINDOWS//Fonts//simsun.ttc,1", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                        iTextSharp.text.Font fontChinese = new iTextSharp.text.Font(bfChinese, size, iTextSharp.text.Font.NORMAL, new iTextSharp.text.Color(r, g, b));

                        //导出label或文本框的内容：
                        document.Add(new Paragraph(txtFileContent, fontChinese));
                        document.Close();

                        //再修改源文件时间
                        System.IO.File.SetLastWriteTime(sourceFileName, srcUpdateTime);
                    }

                    //添加PDFjs路径
                    webFile = "Scripts/PDFJSInNet/web/viewer.html?file=/" + curDoc.Storage.O_storname + "/" + curDoc.Project.O_projectcode + "/pdf/" + // + pdfFileName;
                        (curDoc.O_filename.Substring(0, curDoc.O_filename.Length - ext.Length)).Replace("#", "＃") + "pdf"; //;//添加PDFjs路径

                    //下载文件限制：sid加文档权限加下载链接有效时间
                    DateTime dt = DateTime.Now;
                    string para = AVEVA.CDMS.WebApi.EnterPointController.enProcessRequest(sid + "___" + DocKeyword + "___" + dt.ToString().Replace("/", "-"));

                    webFile = webFile + "&p=" + para;
                }

                //图片及文本文件
                else if (ext == "txt" || ext == "jpg" || ext == "jpeg" || ext == "bmp" || ext == "gif" || ext == "png" || ext == "ico" || ext == "psd" || contentType == "text/xml" || contentType == "image/png")
                {

                    webFile = curDoc.Storage.O_storname + "/" + curDoc.Project.O_projectcode + "/" + HttpClient.EncodeUrl(curDoc.O_filename);//添加PDFjs路径

                }

                //压缩文件
                else if (ext == "zip" || ext == "rar")
                {
                    //获取原文件信息
                    FileInfo srcFileInfo = new FileInfo(WorkingPath + curDoc.O_filename);


                    //创建一个zip目录
                    if (!Directory.Exists(WorkingPath + "zip")) Directory.CreateDirectory(WorkingPath + "zip");

                    string unrarMainpath = WorkingPath + "zip\\";

                    string infoFilePath = unrarMainpath + "CDMSUnrar" + ".zipinfo";

                    //当没有解压信息文件时，创建一个信息文件
                    if (!System.IO.File.Exists(infoFilePath))
                    {
                        #region 添加压缩文档信息到信息文件
                        XmlDocument xmldoc;
                        XmlNode xmlnode;
                        XmlElement xmlelem;

                        xmldoc = new XmlDocument();
                        //加入XML的声明段落,<?xml version="1.0" encoding="gb2312"?>
                        XmlDeclaration xmldecl;
                        xmldecl = xmldoc.CreateXmlDeclaration("1.0", "gb2312", null);
                        xmldoc.AppendChild(xmldecl);

                        //加入一个根元素
                        xmlelem = xmldoc.CreateElement("", "ZipFileInfo", "");
                        xmldoc.AppendChild(xmlelem);
                        //加入另外一个元素


                        XmlNode rootCreate = xmldoc.SelectSingleNode("ZipFileInfo");//查找<Employees> 


                        XmlElement xeCreate = xmldoc.CreateElement("ZipFiles");//创建一个<Node>节点 
                        xeCreate.SetAttribute("lastIndex", "0");//设置最后文件索引

                        rootCreate.AppendChild(xeCreate);//添加到<Employees>节点中 

                        //保存创建好的信息文档,文件名为原文件加后缀 .zipinfo
                        //xmldoc.Save(unrarMainpath + "CDMSWeb" + ".zipinfo");
                        xmldoc.Save(infoFilePath);


                        #endregion
                    }

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(infoFilePath);
                    XmlNode root = xmlDoc.SelectSingleNode("ZipFileInfo");//查找<Employees> 

                    bool readJson = false;

                    JArray jaFiles = new JArray();

                    //文件解压后的子目录名
                    string subFloderName = "";

                    //存放压缩文件解压后的json文件信息
                    string jsonFile = WorkingPath + "zip\\" + srcFileInfo.Name + ".filejson";

                    //查找是否已经存在这个文件节点
                    XmlNodeList xnList = xmlDoc.SelectNodes("//ZipFiles//ZipFile");
                    foreach (XmlNode xn in xnList)
                    {
                        //找到当前解压文件所属的节点
                        string nodeFileName = (xn.SelectSingleNode("fileName")).InnerText;
                        if (nodeFileName == srcFileInfo.Name)
                        {
                            //如果有这个解压的文件，并且文件正在解压，就返回让客户端等待解压完成
                            string strExtracting = ((XmlElement)xn).GetAttribute("isExtracting");
                            if (strExtracting == "true")
                            {
                                //加快回收XmlDocument对象
                                xmlDoc.RemoveAll();
                                xmlDoc = null;
                                reJo.msg = "isExtracting";
                                return reJo.Value;
                            }

                            string nodeFileSize = (xn.SelectSingleNode("size")).InnerText;
                            string nodeUpdate = (xn.SelectSingleNode("upDate")).InnerText;

                            //获取文件解压后的子目录名
                            subFloderName = (xn.SelectSingleNode("targetFloder")).InnerText;

                            //如果文件没有改变，就读取json文件
                            if (nodeUpdate == srcFileInfo.LastWriteTime.ToString() && nodeFileSize == srcFileInfo.Length.ToString())
                            {

                                //读取json信息文件
                                readJson = true;
                            }
                            else
                            {
                                try
                                {
                                    //如果压缩文件的更新时间不一样，就删除解压目录和json文件
                                    if (System.IO.File.Exists(jsonFile))
                                    {
                                        System.IO.File.Delete(jsonFile);
                                    }

                                    if (Directory.Exists(WorkingPath + "zip\\" + subFloderName))
                                    {
                                        //目录为空，删除目录
                                        DirectoryInfo directoryInfo = new DirectoryInfo(WorkingPath + "zip\\" + subFloderName);
                                        //directoryInfo.Delete();
                                        //如果有子目录，先循环删除子目录，再删除当前目录
                                        directoryInfo.Delete(true);
                                    }

                                }
                                catch (Exception exd) { }

                                //删除解压主目录的压缩文件节点
                                XmlElement xnParent = (XmlElement)xmlDoc.SelectSingleNode("//ZipFiles");
                                xnParent.RemoveChild((XmlElement)xn);
                                subFloderName = "";
                            }
                            break;
                        }
                    }



                    //读取解压后的json文件信息
                    if (readJson == true)
                    {
                        if (System.IO.File.Exists(jsonFile))
                        {
                            //读取json文件
                            string jsonContent = "";

                            //读取json文件，每次读取一行，
                            StreamReader sr = new StreamReader(jsonFile, Encoding.UTF8);
                            String line;
                            while ((line = sr.ReadLine()) != null)
                            {
                                jsonContent = jsonContent + line.ToString();
                            }
                            sr.Close();

                            //压缩文件所包含的文件列表信息
                            jaFiles = (JArray)JsonConvert.DeserializeObject(jsonContent);

                        }
                        else
                        {
                            //如果不存在json文件，就重新生成一个
                            readJson = false;
                        }
                    }

                    //如果之前没有解压过文档，或者压缩文件的更新时间不一样，就重新解压文档
                    if (readJson == false)
                    {
                        //在信息文件添加一个文件节点
                        #region MyRegion
                        //XmlDocument xmlDoc = new XmlDocument();
                        //xmlDoc.Load(infoFilePath);
                        //XmlNode root = xmlDoc.SelectSingleNode("ZipFileInfo");//查找<Employees> 
                        XmlElement xe1 = (XmlElement)root.SelectSingleNode("ZipFiles");
                        string strIndex = xe1.GetAttribute("lastIndex");
                        strIndex = (Convert.ToInt32(strIndex) + 1).ToString();

                        //文件解压后的子目录名
                        subFloderName = "CDMSUnrar" + strIndex.PadLeft(8, '0');


                        XmlElement xeItem = xmlDoc.CreateElement("ZipFile");//创建一个<Node>节点 
                        //修改是否正在解压缩的属性
                        xeItem.SetAttribute("isExtracting", "true");

                        XmlElement xesub1 = xmlDoc.CreateElement("fileName");
                        xesub1.InnerText = srcFileInfo.Name;//设置文本节点 
                        xeItem.AppendChild(xesub1);//添加到<Node>节点中 
                        XmlElement xesub2 = xmlDoc.CreateElement("upDate");
                        xesub2.InnerText = srcFileInfo.LastWriteTime.ToString();
                        xeItem.AppendChild(xesub2);
                        XmlElement xesub3 = xmlDoc.CreateElement("size");
                        xesub3.InnerText = srcFileInfo.Length.ToString();
                        xeItem.AppendChild(xesub3);
                        XmlElement xesub4 = xmlDoc.CreateElement("targetFloder");//解压后的目录名
                        xesub4.InnerText = subFloderName;// unrarTargetpath;
                        xeItem.AppendChild(xesub4);

                        xe1.AppendChild(xeItem);//添加到<Employees>节点中  
                        #endregion

                        //修改最后索引的属性
                        xe1.SetAttribute("lastIndex", strIndex);

                        xmlDoc.Save(infoFilePath);

                        //解压路径
                        string unrarTargetpath = WorkingPath + "zip\\" + subFloderName + "\\";

                        //创建目录用于解压当前压缩文件
                        if (!Directory.Exists(unrarTargetpath)) Directory.CreateDirectory(unrarTargetpath);

                        //添加线程锁来解压文档，防止多并发解压文件

                        ///解压文档
                        jaFiles = WebCDMSRar.SharpUnRar(sourceFileName, unrarTargetpath);

                        //把已经解压完成的消息，保存到配置文件里面
                        XmlNodeList xnRarList = xmlDoc.SelectNodes("//ZipFiles//ZipFile");
                        foreach (XmlNode xn in xnRarList)
                        {
                            string nodeFileName = (xn.SelectSingleNode("fileName")).InnerText;
                            if (nodeFileName == srcFileInfo.Name)
                            {
                                ((XmlElement)xn).SetAttribute("isExtracting", "false");
                            }
                        }
                        xmlDoc.Save(infoFilePath);


                        // 生成Json文件
                        if (System.IO.File.Exists(jsonFile))
                        {
                            System.IO.File.Delete(jsonFile);
                        }
                        using (FileStream fileStream = System.IO.File.Create(jsonFile))
                        {
                            byte[] bytes = new UTF8Encoding(true).GetBytes(JsonConvert.SerializeObject(jaFiles));
                            fileStream.Write(bytes, 0, bytes.Length);
                        }

                    }

                    reJo.data = new JArray(new JObject(new JProperty("filetype", "rar"),
                        new JProperty("path", curDoc.Storage.O_storname + "/" + curDoc.Project.O_projectcode + "/" + "zip" + "/" + subFloderName + "/"), //压缩文档所在路径
                        new JProperty("filelist", jaFiles),
                        new JProperty("subFolder", subFloderName),
                        new JProperty("isUnrar", "true")));


                    reJo.success = true;
                    return reJo.Value;

                }
                //未能识别的文件，显示预览错误页
                else
                {

                    webFile = "Shared/ErrorPreview";

                }

                ///判断是否为图片或者文本文件
                //else if (CheckIsTextFile(sourceFileName) || IsImage(sourceFileName))
                //{
                //    webFile = curDoc.Storage.O_storname + "/" + curDoc.Project.O_projectcode + "/" + curDoc.O_filename;//添加PDFjs路径
                //}



                reJo.data = new JArray(new JObject(new JProperty("filetype", "common"), new JProperty("path", webFile),
                    new JProperty("filename", curDoc.O_filename),
                    new JProperty("isUnrar", isUnrar)));
                reJo.success = true;
            }

            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(reJo.msg);
            }
            return reJo.Value;
        }


        /// <summary>
        /// 处理调用CAD参照功能后，设置文档的参照文档属性
        /// </summary>
        /// <param name="sid">连接密钥</param>
        /// <param name="DocKeyword">文档关键字</param>
        /// <param name="refDocKeyword">参照文档的关键字</param>
        /// <returns></returns>
        public static JObject AfterRefCAD(string sid, string DocKeyword,string refDocKeyword)
        {
            ExReJObject reJo = new ExReJObject();
            try
            {
                if (string.IsNullOrEmpty(sid))
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

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



                //获取文档对象
                Doc curDoc = dbsource.GetDocByKeyWord(DocKeyword);

                if (curDoc == null)
                {
                    reJo.msg = "当前活动文档不存在！";
                    return reJo.Value;
                }

                //写会数据库
                Doc refDoc= dbsource.GetDocByKeyWord(refDocKeyword);
                if (curDoc == null)
                {
                    reJo.msg = "当前参照文档不存在！";
                    return reJo.Value;
                }

                //curDoc.RefDocList.Clear();
                //先从参照列表移除原有的参照doc
                foreach (Doc rdoc in curDoc.RefDocList)
                {
                    if (rdoc.KeyWord == refDoc.KeyWord)
                    {
                        curDoc.DeleteRefDoc(rdoc);
                        break;
                    }
                }
                //curDoc.DeleteRefDoc(refDoc);

                //如果列表里面包含有多个相同的文档，删除后可能还会包含有此文档，
                //所以还要判断一次是否包含需要添加的参照文档
                bool isContains = false;
                foreach (Doc rdoc in curDoc.RefDocList)
                {
                    if (rdoc.KeyWord == refDoc.KeyWord)
                    {
                        isContains = true;
                        break;
                    }
                }

                //if (!curDoc.RefDocList.Contains(refDoc))
                if (isContains == false)
                {
                    curDoc.AddRefDoc(refDoc);   //记录参考文件，下次打开curDoc 的时候，系统自动会下载对应的参考文件。
                }

                reJo.success = true;
                return reJo.Value;

            }
            catch  {

            }
            return reJo.Value;
        }


        /// <summary>
        /// 获取文档是否有参照文档等信息
        /// </summary>
        /// <param name="sid">连接密钥</param>
        /// <param name="DocKeyword">doc关键字</param>
        /// <returns></returns>
        public static JObject GetRefCADList(string sid, string DocKeyword)
        {
            ExReJObject reJo = new ExReJObject();
            try
            {
                if (string.IsNullOrEmpty(sid))
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

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

                //获取文档对象
                Doc curDoc = dbsource.GetDocByKeyWord(DocKeyword);

                if (curDoc == null)
                {
                    reJo.msg = "当前活动文档不存在！";
                    return reJo.Value;
                }

                //如果有参照文档，就返回第一个参照文档
                if (curDoc.RefDocList.Count > 0)
                {
                    JObject joData = new JObject();
                    joData.Add(new JProperty("hasRefDoc", true));

                    //JObject joRefDocList = new JObject();
                    JArray jaRefDocList = new JArray();
                    foreach (Doc refDoc in curDoc.RefDocList) {
                        JObject joRefDoc = new JObject(new JProperty("RefDocKeyword", refDoc.KeyWord));
                        //joRefDocList.Add(refDoc.KeyWord);
                        jaRefDocList.Add(joRefDoc);
                    }
                    joData.Add(new JProperty("RefDocList", jaRefDocList));
                    
                    //reJo.data = new JArray(new JObject(
                    //    new JProperty("hasRefDoc", true),
                    //    new JProperty("RefDocKeyword", curDoc.RefDocList[0].KeyWord)
                    //    ));

                    reJo.data = new JArray(joData);
                    reJo.success = true;
                    return reJo.Value;
                }
                else {
                    reJo.data = new JArray(new JObject(
                        new JProperty("hasRefDoc", false),
                        new JProperty("RefDocKeyword", "")));
                    reJo.success = true;
                    return reJo.Value;
                }

            }
            catch
            {

            }
            return reJo.Value;
        }

        /// <summary>
        /// 获取Doc在服务器上完整的路径，此路径可用System.IO.File类直接操作文件,例如："\\127.0.0.1\storage\CDMSxxxxxx\xxx.xxx"
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="DocKeyword"></param>
        /// <returns></returns>
        public static JObject GetDocFilePath(string sid, string DocKeyword) {

            ExReJObject reJo = new ExReJObject();
            try
            {
                if (string.IsNullOrEmpty(sid))
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

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

                //获取文档对象
                Doc ddoc = dbsource.GetDocByKeyWord(DocKeyword);

                Doc curDoc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;

                //共享网络，在WEB服务器上把Storage 设置为共享，如果Storage就在WEB服务器，不共享
                //string WorkingPath = string.IsNullOrEmpty(curDoc.Storage.O_protocol) ? curDoc.Storage.O_path : curDoc.Storage.O_protocol;
                //if (!WorkingPath.EndsWith("\\") && !WorkingPath.EndsWith("/"))
                //{
                //    WorkingPath = WorkingPath + "\\" + curDoc.Project.O_projectcode + "\\";
                //}

                string WorkingPath = curDoc.Storage.O_protocol + (curDoc.Storage.O_protocol.EndsWith("\\") || curDoc.Storage.O_protocol.EndsWith("/") ? "" : "\\") + curDoc.Project.O_projectcode + "\\";

                //文件名称
                string sourceFileName = WorkingPath + curDoc.O_filename;

                reJo.msg = sourceFileName;
                reJo.success = true;
            }

            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(reJo.msg);
            }
            return reJo.Value;

        }

        /// <summary>
        /// 预览压缩文件里面文件
        /// </summary>
        /// <param name="sid">SID</param>
        /// <param name="DocKeyword">doc 关键字</param>
        /// <param name="path">文件路径(前后不要加\\或者/)， 例子:\\127.0.0.1\LFWebStorage\CDMS0408318\Preview\测试文件夹\测试 Microsoft Excel 工作表33.xlsx  path = 测试文件夹 </param>
        /// <param name="filename">文件名称，例子:\\127.0.0.1\LFWebStorage\CDMS0408318\Preview\测试文件夹\测试 Microsoft Excel 工作表33.xlsx， filename = 测试 Microsoft Excel 工作表33.xlsx</param>
        /// <returns></returns>
        public static JObject PreviewZipDoc(string sid, string DocKeyword, string path, string filename)
        {
            ExReJObject reJo = new ExReJObject();
            try
            {
                if (string.IsNullOrEmpty(sid))
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

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



                //获取文档对象
                Doc ddoc = dbsource.GetDocByKeyWord(DocKeyword);

                Doc curDoc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;

                //获取扩展名
                string ext = filename.Substring(filename.LastIndexOf(".") + 1).ToLower(); //扩展名

                //获取文件的Mime类型，根据Mime类型选择预览的方式
                string contentType = MimeMapping.GetMimeMapping(filename);

                //共享网络，在WEB服务器上把Storage 设置为共享，如果Storage就在WEB服务器，不共享
                //string WorkingPath = string.IsNullOrEmpty(curDoc.Storage.O_protocol) ? curDoc.Storage.O_path : curDoc.Storage.O_protocol;

                string WorkingPath = curDoc.Storage.O_protocol + (curDoc.Storage.O_protocol.EndsWith("\\") || curDoc.Storage.O_protocol.EndsWith("/") ? "" : "\\") + curDoc.Project.O_projectcode + "\\";

                //if (!WorkingPath.EndsWith("\\") && !WorkingPath.EndsWith("/"))
                {
                    WorkingPath = WorkingPath + "\\zip\\" + path;
                }
                 if (!WorkingPath.EndsWith("\\") && !WorkingPath.EndsWith("/"))
                 {
                     WorkingPath = WorkingPath + "\\";
                 }
                 string webPath = curDoc.Storage.O_storname + "/" + curDoc.Project.O_projectcode + "/zip/" + path;
                if (!webPath.EndsWith("/"))
                {
                    webPath = webPath + "/";
                }


                //WEB文件路径
                string webFile = "";


                //获取不同类型文件的WEB路径
                if (ext == "pdf")
                {
                    webFile = "Scripts/PDFJSInNet/web/viewer.html?file=/" + webPath + HttpClient.EncodeUrl(filename);//添加PDFjs路径
                }


                //office文件
                else if (ext == "doc" || ext == "docx" || ext == "xls" || ext == "xlsx" || ext == "dwg")
                {

                    //Office文件转换为PDF ， 直接在 CDMS0001 下建立一个目录 PDF， 放置PDF文件
                    string pdfFileName = WorkingPath + "pdf\\" + filename;
                    pdfFileName = pdfFileName.Substring(0, pdfFileName.Length - ext.Length) + "pdf";

                    //替换#符号为全角，否则显示不了pdf预览
                    pdfFileName = pdfFileName.Replace("#", "＃");

                    //源office文件更新日期
                    FileInfo srcFileInfo = new FileInfo(WorkingPath + filename);
                    DateTime srcUpdateTime = srcFileInfo.LastWriteTime;


                    //生成的预览文件名
                    bool mustConvertToPdf = true;
                    if (System.IO.File.Exists(pdfFileName))
                    {
                        //判断是否需要转换为PDF文件
                        //pdf文件更新日期
                        FileInfo pdfFileInfo = new FileInfo(pdfFileName);
                        DateTime pdfUpdateTime = pdfFileInfo.LastWriteTime;


                        //判断源文件修改日期是否比pdf预览文件要新
                        if (pdfUpdateTime > srcUpdateTime)
                        {
                            mustConvertToPdf = false;
                        }
                    }



                    //需要转换为pdf文件
                    if (mustConvertToPdf)
                    {

                        //创建一个PDF目录
                        if (!Directory.Exists(WorkingPath + "pdf")) Directory.CreateDirectory(WorkingPath + "pdf");

                        CDMSPdf.ConvertToPdf(WorkingPath + filename, pdfFileName);


                        //再修改源文件时间
                        System.IO.File.SetLastWriteTime(WorkingPath + filename, srcUpdateTime);

                    }

                    webFile = "Scripts/PDFJSInNet/web/viewer.html?file=/" + webPath + "pdf/" + 
                        HttpClient.EncodeUrl(filename.Substring(0, filename.Length - ext.Length).Replace("#", "＃")) + "pdf"; ;//添加PDFjs路径
                }



                //图片及文本文件
                else if (ext == "txt" || ext == "jpg" || ext == "jpeg" || ext == "bmp" || ext == "gif" || ext == "png" || ext == "ico" || ext == "psd" || contentType== "text/xml" || contentType == "image/png")
                {

                    //webFile = webPath + HttpClient.EncodeUrl(filename); //添加PDFjs路径
                    webFile = webPath + filename; //添加PDFjs路径


                }

                // //判断是否为图片或者文本文件
                //else if (CheckIsTextFile(filename) || IsImage(filename))
                //{
                //    webFile = webPath + filename; //添加PDFjs路径
                //}

                //压缩文件
                //if (ext == "zip" || ext == "rar")
                //{

                //    //解压路径
                //    string unrarTargetpath = WorkingPath + "zip";

                //    ////解压文档
                //    JArray getJa = WebCDMSRar.SharpUnRar(sourceFileName, unrarTargetpath);

                //    reJo.data = new JArray(new JObject(new JProperty("filetype", "rar"),
                //        new JProperty("path", curDoc.Storage.O_storname + "/" + curDoc.Project.O_projectcode + "/"), //压缩文档所在路径
                //        new JProperty("filelist", getJa), new JProperty("isUnrar", "true")));

                //}




                reJo.data = new JArray(new JObject(new JProperty("filetype", "common"), new JProperty("path", webFile), new JProperty("isUnrar", "true")));

                reJo.success = true;
            }

            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(reJo.msg);
            }
            return reJo.Value;
        }



        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="DocKeyword">Doc关键字</param>
        /// <param name="RefDocKeyword">主文件的参考文档，或附加文档的关键字，可选填，如果有填，就根据主文档（DocKeyword）的权限，来下载参考文档（RefDocKeyword）</param>
        /// <param name="Server_MapPath">网站根目录,系统自动生成</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时，data包含一个JObject，包含参数"path"，用于存放下载文件的路径</para>
        /// <para>例子：</para>
        /// </returns>
        public static JObject FileDownload(string sid, string DocKeyword, string RefDocKeyword, string Server_MapPath)
        {
            ExReJObject reJo = new ExReJObject();
            try
            {
                //if (sid == null)
                //{
                //    reJo.msg = "登录验证失败！请尝试重新登录！";
                //    return reJo.Value;
                //}


                //登录用户
                User curUser = DBSourceController.GetCurrentUser(sid);
                if (curUser == null || curUser.dBSource==null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }
                DBSource dbsource = curUser.dBSource;


                //获取文档对象
                Doc ddoc = dbsource.GetDocByKeyWord(DocKeyword);


                //doc.O_dmsstatus = (enDocStatus)2;
                if (ddoc == null)
                {
                    reJo.msg = "下载失败,文档不存在！";
                    return reJo.Value;
                }
                Doc doc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;
                if (doc.O_iuser4 == 1)
                {
                    reJo.msg = "下载失败,文档正在签名！";
                    return reJo.Value;
                }

                #region 判断参照文档是否存在
                Doc refDoc = null;
                if (!string.IsNullOrEmpty(RefDocKeyword))
                {
                    Doc refddoc = dbsource.GetDocByKeyWord(RefDocKeyword);
                    if (refddoc == null)
                    {
                        reJo.msg = "下载失败,参照文档不存在！";
                        return reJo.Value;
                    }
                    refDoc = refddoc.ShortCutDoc == null ? refddoc : refddoc.ShortCutDoc;
                    if (refDoc.O_iuser4 == 1)
                    {
                        reJo.msg = "下载失败,文档正在签名！";
                        return reJo.Value;
                    }
                } 
                #endregion

                //判断是否有打开文件的权限
                bool ReadRight = DocController.GetDocDFReadRight(doc, curUser);
                if (ReadRight==false)
                {
                    reJo.msg = "当前用户没有权限下载该文档:" + doc.ToString + "！";
                    return reJo.Value;
                }

                #region 下载文件前触发的事件 ，DLRight被修改为false时不能下载文档
                //下载文件前触发的事件 ，DLRight被修改为false时不能下载文档
                if (refDoc == null)
                {
                    foreach (WebDocEvent.Before_Download_File_Event_Class BeforeDownloadFile in WebDocEvent.ListBeforeDownloadFile)
                    {
                        if (BeforeDownloadFile.Event != null)
                        {
                            //如果在用户事件中筛选过了，就跳出事件循环
                            if (BeforeDownloadFile.Event(BeforeDownloadFile.PluginName, doc, ref ReadRight))
                            {
                                break;
                            }
                        }
                    }
                    if (ReadRight == false)
                    {
                        reJo.msg = "当前用户没有权限下载该文档:" + doc.ToString + "！";
                        return reJo.Value;
                    }
                }
                #endregion

                //if (doc.O_dmsstatus == enDocStatus.OUT && doc.FLocker!=curUser)
                //{
                //    reJo.msg = "下载失败,文档已经被用户 "+doc.FLocker.ToString+" 编辑，当前处于锁定状态！";
                //    return reJo.Value;
                //}

                #region 获取文件最后修改时间，md5等信息，返回给客户端
                
                if (refDoc != null)
                {
                    //如果是下载参照文件
                    doc = refDoc;
                }

                string lastModified = "";
                string fileMd5 = "";
                string ServerFileName = doc.O_filename;
                if (string.IsNullOrEmpty(ServerFileName))
                {
                    reJo.msg = "下载失败,文件不存在！";
                    return reJo.Value;
                }

                //获取文件MD5
                if (string.IsNullOrEmpty(doc.O_suser2))
                {
                    fileMd5 = FileMD5.GetMD5HashFromFile(doc.FullPathFile);
                    if (!string.IsNullOrEmpty(fileMd5))
                    {
                        doc.O_suser2 = fileMd5;
                        doc.Modify();
                    }
                }
                else {
                    fileMd5 = doc.O_suser2;
                }

                //组成服务器上全路径文件名称
                string FullFileName = "";
                if (ServerFileName.Contains(":") || ServerFileName.StartsWith("\\\\"))
                {
                    //用户指定文件
                    FullFileName = ServerFileName;
                }
                else if (doc != null)
                {
                    //放置在存储下，每个Storage的另一个路径必须保存ISS的虚拟目录
                    FullFileName = doc.Storage.O_protocol + (doc.Storage.O_protocol.EndsWith("\\") || doc.Storage.O_protocol.EndsWith("/") ? "" : "\\") + doc.Project.O_projectcode + "\\" + ServerFileName;
                }
                else
                {
                    //获取ISO文件
                    FullFileName = AppDomain.CurrentDomain.BaseDirectory + ServerFileName;
                }


                FullFileName = FullFileName.Replace("\\", "/");

                if (!System.IO.File.Exists(FullFileName))
                {
                    reJo.msg = "下载失败,文件不存在！";
                    return reJo.Value;
                }
                //定义文件信息处理对象
                FileInfo FileInfoMess = new FileInfo(FullFileName);

                lastModified = FileInfoMess.LastWriteTime.ToString();

                string FileSize = FileInfoMess.Length.ToString();

                ///修改文件状态
                ///记录LocalPath，且把Doc状态等记录在注册表，该注册表信息，直到重新把Doc检入后才清除
                ///文件下载类型{DownExport /*输出*/ , DownCheckOut/*检出*/ , DownFreely/*发送到*/
                //switch (DownLoadType)
                //{
                //    case "DownCheckOut":
                //        doc.FLocker = curUser;
                //        doc.O_flocktime = DateTime.Now;
                //        //doc.O_conode = Net::Dns::GetHostName();
                //        doc.OperateDocStatus = enDocStatus.GOING_OUT;
                //        doc.Modify();
                //        break;
                //    case "DownExport":
                //        doc.FLocker = curUser;
                //        doc.O_flocktime = DateTime.Now;
                //        //doc.O_conode = Net::Dns::GetHostName();
                //        doc.OperateDocStatus = enDocStatus.GOING_OUT;
                //        doc.Modify();
                //        break;
                //    case "DownFreely":
                //        break;
                //}


                #endregion



                //获取下载路径
                string webPath = doc.Storage.O_storname + "/" + doc.Project.O_projectcode + "/" + doc.O_filename;// HttpClient.EncodeUrl(doc.O_filename);//.Replace("#","%23");


                //webPath = webPath + "?sid="+sid+"&DocKeyword="+doc.KeyWord;
                //下载文件限制：sid加文档权限加下载链接有效时间
                DateTime dt = DateTime.Now;
                string para = AVEVA.CDMS.WebApi.EnterPointController.enProcessRequest(sid + "___" + doc.KeyWord + "___" + dt.ToString().Replace("/", "-"));
                webPath = webPath + "?p=" + para;

                reJo.data = new JArray(new JObject(new JProperty("path", webPath),//文件下载地址
                    new JProperty("filename", doc.O_filename),//文件名
                    new JProperty("fileSize", FileSize),//文件名
                    new JProperty("lastModified", lastModified),//最后修改时间
                    new JProperty("fileMD5", fileMd5),
                    new JProperty("workingPath", doc.WorkingPath),
                    new JProperty("prePath", doc.Storage.O_storname + "/" + doc.Project.O_projectcode + "/"),//下载路径前缀
                    new JProperty("para",para)//把下载路径前缀，文件名和参数传给客户端，在客户端把文件名转码后再重新组装
                    ));
                reJo.success = true;


            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
                reJo.msg = "下载失败！" + e.Message;
            }
            return reJo.Value;
        }

        #region 多个文件打包下载
        /// <summary>
        /// 多个文件打包下载
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="DocKeyword"></param>
        /// <param name="Server_MapPath"></param>
        /// <returns></returns>
        //public static JObject MultiFileDownload(string sid, string DocKeyword, string Server_MapPath) {
        //    ExReJObject reJo = new ExReJObject();
        //    try
        //    {
        //        if (sid == null)
        //        {
        //            reJo.msg = "登录验证失败！请尝试重新登录！";
        //            return reJo.Value;
        //        }


        //        //登录用户
        //        User curUser = DBSourceController.GetCurrentUser(sid);
        //        if (curUser == null)
        //        {
        //            reJo.msg = "登录验证失败！请尝试重新登录！";
        //            return reJo.Value;
        //        }
        //        DBSource dbsource = curUser.dBSource;


        //        //工作路径
        //        string WorkingPath = string.Empty;

        //        //压缩文件存放路径
        //        string zipTargetpath = string.Empty;

        //        string[] DocKeywordArray = DocKeyword.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

        //        string errMsg = "";
        //        foreach (string docKeyword in DocKeywordArray)
        //        {
        //            //获取文档对象
        //            Doc doc = dbsource.GetDocByKeyWord(docKeyword);
        //            if (doc == null)
        //            {
        //                continue;
        //            }
        //            if (doc.O_iuser4 == 1)
        //            {
        //                errMsg=errMsg+ "下载失败,文档"+doc.O_filename+"正在签名！ ";
        //                continue;
        //            }

        //            //获取工作路径
        //            if (string.IsNullOrEmpty(WorkingPath))
        //            {
        //                //共享网络，在WEB服务器上把Storage 设置为共享，如果Storage就在WEB服务器，不共享
        //                WorkingPath = string.IsNullOrEmpty(doc.Storage.O_protocol) ? doc.Storage.O_path : doc.Storage.O_protocol;
        //                if (!WorkingPath.EndsWith("\\") && !WorkingPath.EndsWith("/"))
        //                {
        //                    WorkingPath = WorkingPath + "\\" + doc.Project.O_projectcode + "\\";
        //                }

        //                //创建一个zip目录
        //                if (!Directory.Exists(WorkingPath + "zip")) Directory.CreateDirectory(WorkingPath + "zip");

        //                DateTime dtNow = DateTime.Now;
        //                //压缩文件存放路径
        //                zipTargetpath = WorkingPath + "zip\\"+doc.ToString+".zip";

        //            }

        //            //如果目标路径不为空
        //            if (!string.IsNullOrEmpty(zipTargetpath))
        //            {
        //                if (System.IO.File.Exists(WorkingPath + doc.O_filename))
        //                {
        //                    JArray getJa = WebCDMSRar.SharpZip(WorkingPath + doc.O_filename, zipTargetpath);
        //                }
        //            }

        //        }


        //        ///下载文件限制：sid加文档权限加下载链接有效时间
        //        //DateTime dt = DateTime.Now;
        //        //string para = AVEVA.CDMS.WebApi.EnterPointController.enProcessRequest(sid + "___" + doc.KeyWord + "___" + dt.ToString().Replace("/", "-"));
        //        //webPath = webPath + "?p=" + para;
        //        //reJo.data = new JArray(new JObject(new JProperty("path", webPath),//文件下载地址
        //        //    new JProperty("filename", doc.O_filename),//文件名
        //        //    new JProperty("lastModified", lastModified),//最后修改时间
        //        //    new JProperty("fileMD5", fileMd5)
        //        //    ));
        //        reJo.success = true;


        //    }
        //    catch (Exception e)
        //    {
        //        CommonController.WebWriteLog(e.Message);
        //        reJo.msg = "下载失败！" + e.Message;
        //    }
        //    return reJo.Value;
        //} 
        #endregion

        private static int rep = 0;
        /// 
        /// 生成随机字母字符串(数字字母混和)
        /// 
        /// 待生成的位数
        /// 生成的字母字符串
        private static string GenerateCheckCode(int codeCount)
        {
            string str = string.Empty;
            long num2 = DateTime.Now.Ticks + rep;
            rep++;
            Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> rep)));
            for (int i = 0; i < codeCount; i++)
            {
                char ch;
                int num = random.Next();
                if ((num % 2) == 0)
                {
                    ch = (char)(0x30 + ((ushort)(num % 10)));
                }
                else
                {
                    ch = (char)(0x41 + ((ushort)(num % 0x1a)));
                }
                str = str + ch.ToString();
            }

            return str;
        }

        /// <summary>
        /// 获取doc属性,在调用文档的“新建文档”菜单和“编辑属性”菜单，初始化表单时使用
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="action">操作类型，传递调用的页面，"ModiDoc":修改文档,"CreateDoc":创建文档</param>
        /// <param name="DocKeyword">Doc关键字</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时且操作类型为"ModiDoc"(修改文档)时，data包含一个JObject，里面包含参数"docdesc"：Doc描述，"defkeyword"：Doc模板的IndexKeyWord，"defkeyid"：Doc模板的Id,"defname":Doc模板的代码,"defdesc":Doc模板的描述,"filename"：文件名，"filesize"：文件大小</para>
        /// <para>操作成功时且操作类型为"CreateDoc"(创建文档)时，success返回true，data包含一个空的JObject</para>
        /// <para>例子：</para>
        /// </returns>
        public static JObject GetDocAttr(string sid, string action, string DocKeyword)
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

                //登录用户
                DBSource dbsource = curUser.dBSource;
                if (dbsource == null)//登录并获取dbsource成功
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }
                JObject attrJo = new JObject();
                if (action == "ModiDoc")//如果是修改文档
                {
                    if (string.IsNullOrEmpty(DocKeyword))
                    {
                        reJo.msg = "错误的数据：文档ID错误！";
                        return reJo.Value;
                    }

                    Doc ddoc = dbsource.GetDocByKeyWord(DocKeyword);
                    if (ddoc == null)
                    {
                        reJo.msg = "错误的数据：找不到对应ID的文档！";
                        return reJo.Value;
                    }

                    Doc doc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;

                    //添加属性页信息

                    attrJo.Add(new JProperty("doccode", doc.Code));
                    attrJo.Add(new JProperty("docdesc", doc.Description));
                    if (doc.TempDefn != null)
                    {
                        attrJo.Add(new JProperty("defkeyword", doc.TempDefn.IndexKeyWord));
                        attrJo.Add(new JProperty("defkeyid", doc.TempDefn.ID.ToString()));
                        attrJo.Add(new JProperty("defname", doc.TempDefn.Code));
                        attrJo.Add(new JProperty("defdesc", doc.TempDefn.Description));
                    }
                    else
                    {
                        attrJo.Add(new JProperty("defkeyword", ""));
                        attrJo.Add(new JProperty("defkeyid", ""));
                        attrJo.Add(new JProperty("defname", ""));
                        attrJo.Add(new JProperty("defdesc", ""));
                    }

                    attrJo.Add(new JProperty("filename", doc.O_filename));
                    attrJo.Add(new JProperty("filesize", doc.O_size));

                    reJo.data = new JArray(attrJo);
                    reJo.success = true;
                    return reJo.Value;

                }
                else if (action == "CreateDoc")
                {
                    reJo.success = true;
                }

            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(e.Message);
                return reJo.Value;
            }
            return reJo.Value;
        }

        /// <summary>
        /// 创建Doc前的事件
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="ProjectKeyword">目录关键字</param>
        /// <param name="FileName">文件名</param>
        /// <returns>
        /// <para>例子：</para>
        /// </returns>
        public static JObject BeforeCreateDoc(string sid, string ProjectKeyword,string FileName) {
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

                Project tProject = dbsource.GetProjectByKeyWord(ProjectKeyword);

                if (tProject == null)
                {
                    reJo.msg = "父目录不存在！";
                    return reJo.Value;
                }

                //重名判断，移到了CreateDocByFileName
                //string sRemoteFilename = FileName;

                ////找doc，看看远程重名文件属于哪个doc
                //Doc sameDoc = null;
                //foreach (Doc dd in tProject.DocList)

                //{
                //    //找到就停止
                //    if (dd.O_filename == sRemoteFilename)
                //    {
                //        sameDoc = dd;
                //        break;
                //    }
                //    //找不到，在版本中找
                //    foreach (Doc dVersionDoc in dd.DocVersionList)

                //    {
                //        if (dVersionDoc.O_filename == sRemoteFilename)
                //        {
                //            sameDoc = dVersionDoc;
                //            break;
                //        }
                //    }
                //    //找不到，在参照中找
                //    foreach (Doc dVersionDoc in dd.RefDocList)

                //    {
                //        if (dVersionDoc.O_filename == sRemoteFilename)
                //        {
                //            sameDoc = dVersionDoc;
                //            break;
                //        }
                //    }
                //    //找不到，在关联 中找
                //    foreach (Doc dVersionDoc in dd.RelationDocList)

                //    {
                //        if (dVersionDoc.O_filename == sRemoteFilename)
                //        {
                //            sameDoc = dVersionDoc;
                //            break;
                //        }
                //    }
                //}

                ////当找到了重名的文档
                //if (sameDoc != null)
                //{
                //    reJo.msg = "目录[" + tProject.Code + "]下已存在与拖拽文件名[" + FileName + "]同名的文件[" + sameDoc.Code + "]！";
                //    return reJo.Value;
                //}

                //reJo.data = new JArray(new JObject());
                reJo.success = true;
                
                return reJo.Value;

            }
            catch (Exception ex)
            {
                CommonController.WebWriteLog(ex.Message);
                reJo.msg = "运行错误！错误信息：" + ex.Message;
            }
            return reJo.Value;
        }


        /// <summary>
        /// 创建文档
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="projectKeyword">目录关键字</param>
        /// <param name="docAttrJson">文档属性，附加属性列表，每个JObject包含："name"，"value","attrtype"三个属性。"attrtype"属性的值为""(空的字符串)时，name有以下值：docCode(Doc代码)，docDesc(Doc描述)，
        /// tempDefnId(模板)；当"attrtype"属性的值为"attrData"(附加属性)时,"name"为附加属性的代码,例如：[{ name: 'docCode', value: docCode },{ name: 'docDesc', value: docDesc },{ name: 'tempDefnKeyword', value: docTemp }];</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时且操作类型为"ModiDoc":修改文档时，success返回true,data包含一个空的JObject</para>
        /// <para>操作成功时且操作类型为"CreateDoc":创建文档时，success返回true,data包含一个空的JObject</para>
        /// <para>例子：</para>
        /// </returns>
        public static JObject CreateDoc(string sid, string projectKeyword, string docAttrJson) //string docKeyword, string docAttrJson)
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

                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(docAttrJson);
                //创建新文档

                string docCode = "", docDesc = "", tempDefnId = "", tempDefnKeyword="";
                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();
                    if (strName == "docCode") docCode = strValue;
                    else if (strName == "docDesc")
                    {
                        docDesc = strValue;
                    }
                    else if (strName == "tempDefnId") tempDefnId = strValue;
                    else if (strName == "tempDefnKeyword") tempDefnKeyword = strValue;
                }

                //if (!string.IsNullOrEmpty(docCode) && docCode.IndexOf("#") >= 0)
                //{
                //    reJo.msg = "文件“"+docCode+ "”上传失败，文件名称带有非法字符“#”！";
                //    reJo.success = false;
                //    return reJo.Value;
                //}

                Project parentProj = dbsource.GetProjectByKeyWord(projectKeyword);
                
                //reJo.Value = CreateDoc(curUser, dbsource, parentProj, strDocName, strDocDesc, strTempDefnId);

                if (parentProj == null)
                {
                    reJo.msg = "父目录不存在！";
                    return reJo.Value;
                }

                if (!(parentProj.O_type == enProjectType.Local || parentProj.O_type == enProjectType.UserCustom || 
                    parentProj.O_type == enProjectType.GlobCustom || parentProj.ShortProject != null))
                {
                    reJo.msg = "无法创建文档 " + docCode + ":当前目录无法创建文档！";
                    return reJo.Value;
                }

                bool hasRight = ProjectController.GetProjectDCreateRight(parentProj, curUser);

                //有创建文档权限才可以创建目录
                if (parentProj.O_type == enProjectType.Local && !hasRight)
                {
                    reJo.msg = "无法创建文档 " + docCode + ":用户没有创建文档权限！";
                    return reJo.Value;
                }



                //查询是否已经有同名的文档
                Doc doc = parentProj.DocList.Find(d => d.O_itemname == docCode);
                if (doc != null)
                {

                    if (ReUploadFile(doc))
                    {

                        //需要断点续传 
                        reJo.data = docListToJson(new List<Doc> { doc }, curUser);
                        reJo.success = true;
                        return reJo.Value;
                    }
                    reJo.msg = "无法创建文档 " + docCode + ":指定的文件名与现有文档重名. 请指定另一个文档名称！";
                    //reJo.success = true;
                    //reJo.msg = "FileExists";
                    return reJo.Value;
                }

                Doc docByName = new Doc();
                if (!string.IsNullOrEmpty(tempDefnKeyword))
                {
                   List<TempDefn> tempDefnList= dbsource.GetTempDefnByKeyWord(tempDefnKeyword);
                    if (tempDefnList != null && tempDefnList.Count>0)
                    if (!string.IsNullOrEmpty(tempDefnKeyword))
                    {
                        TempDefn tempDefn = tempDefnList[0];
                        docByName = parentProj.NewDoc("", docCode, docDesc, tempDefn);//创建文档（带模板）
                    }
                    else
                        docByName = parentProj.NewDoc("", docCode, docDesc);//创建文档（不带模板）
                }
                else if (!string.IsNullOrEmpty(tempDefnId))
                {
                    TempDefn tempDefn = dbsource.GetTempDefnByID(Convert.ToInt32(tempDefnId));
                    if (tempDefn != null )
                        if (!string.IsNullOrEmpty(tempDefnId))
                        {
                            
                            docByName = parentProj.NewDoc("", docCode, docDesc, tempDefn);//创建文档（带模板）
                        }
                        else
                            docByName = parentProj.NewDoc("", docCode, docDesc);//创建文档（不带模板）
                }
                else
                    docByName = parentProj.NewDoc("", docCode, docDesc);//创建文档（不带模板）

                if (docByName != null)
                {
                    //返回新建的doc信息给客户端
                    List<Doc> docList = new List<Doc>();
                    docList.Add(docByName);

                    //组装JSON,并检查读取权限 
                    reJo.data = docListToJson(docList, curUser);
                    reJo.success = true;

                }
                return reJo.Value;

            }
            catch (Exception ex)
            {
                CommonController.WebWriteLog(ex.Message);
                reJo.msg = "运行错误！错误信息：" + ex.Message;
            }
            return reJo.Value;
        }

        /// <summary>
        /// 上传文件时，根据文件名创建文档
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="projectKeyword">目录关键字</param>
        /// <param name="fileName">文件名</param>
        /// <param name="confirmUpgrade">升版确认，值可以是"","true","false",默认是""，详见下面说明</param>
        /// <param name="bAutoCode">是否自动编码，值可以是"true","false",默认是"true"</param>
        /// <returns>
        /// <para>confirmUpgrade升版确认参数说明： </para>
        /// <para>第一次提交时无需填写，当有相同名字的文件时，返回success为true，msg为"ConfirmUpgrade",需要填写confirmUpgrade后再次提交 </para>
        /// <para>第二次提交时，当值是"true"，就升版文件，当值时"false"，就覆盖同名文件</para>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时且操作类型为"ModiDoc":修改文档时，success返回true,data包含一个空的JObject</para>
        /// <para>操作成功时且操作类型为"CreateDoc":创建文档时，success返回true,data包含一个空的JObject</para>
        /// <para>例子：</para>
        /// <para>1.当目录里有相同文件名的文件，且提交的参数里"confirmUpgrade"为空时，</para>
        /// <para>返回success为true，msg为"ConfirmUpgrade",需要填写confirmUpgrade后再次提交</para>
        /// <para>2.没有成功创建文件时，返回新建的文档信息</para>
        /// </returns>
        public static JObject CreateDocByFileName(string sid, string projectKeyword, string fileName, string confirmUpgrade, string bAutoCode) //string docKeyword, string docAttrJson)
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

                if (string.IsNullOrEmpty(bAutoCode))
                {
                    bAutoCode = "true";
                }


                Project tProject = dbsource.GetProjectByKeyWord(projectKeyword);

                if (tProject == null)
                {
                    reJo.msg = "父目录不存在！";
                    return reJo.Value;
                }

                if (!(tProject.O_type == enProjectType.Local || tProject.O_type == enProjectType.UserCustom ||
                    tProject.O_type == enProjectType.GlobCustom || tProject.ShortProject != null))
                {
                    reJo.msg = "无法创建文档 " + fileName + ":当前目录无法创建文档！";
                    return reJo.Value;
                }

                bool hasRight = ProjectController.GetProjectDCreateRight(tProject, curUser);

                //有创建文档权限才可以升版文档
                if (tProject.O_type == enProjectType.Local && !hasRight)
                {
                    reJo.msg = "无法创建文档 " + fileName + ":用户没有创建文档权限！";
                    return reJo.Value;
                }

                //String fileName = fInfo.Name;
                string extension = Path.GetExtension(fileName);//扩展名 ".aspx"
                String docCode = fileName;
                if (fileName.Contains("."))
                    docCode = fileName.Substring(0, fileName.LastIndexOf('.'));
                String docDesc = "";

                //TIM 2009-11-20 添加自动编码
                TempDefn nullTempDefn = new TempDefn();
                String tempCode = Project_GetNextNewCode(tProject, nullTempDefn, 1);
                if (!string.IsNullOrEmpty(tempCode)/* && tProject.MatchDocNumber(tempCode)*/ && !tProject.IsExistDocNumber(tempCode) && bAutoCode == "true")
                {
                    docCode = tempCode;
                    docDesc = fileName;
                    //fileName = tempCode + fInfo.Extension ; 
                    fileName = tempCode + extension;

                }

                ////小黎 2011-8-3 增加doc.o_filename去掉后缀时 等于 fileName的文件名去掉后缀的处理方法
                ////思路是，拖进去的文件名，xxx.test，如果当前目录存在xxx.其他，就直接作为附件加上去
                //if ( fileName.Contains(".") )
                //{
                //    String FileNameExcu = fInfo.Extension;//(fileName.Substring(fInfo.Name.LastIndexOf('.'))).ToLower() ;
                //    if(tProject.DocList!=null && tProject.DocList.Count >0)
                //    {
                //        foreach(Doc doc in tProject.DocList)
                //        {
                //            //不处理没实体文件或没扩展名的
                //            if ( string.IsNullOrEmpty(doc.O_filename) || !doc.O_filename.Contains("."))
                //                continue ;

                //            //如果原来doc是pdf，也不处理
                //            String excu = (doc.O_filename.Substring( doc.O_filename.LastIndexOf('.'))).ToLower();
                //            if ( excu == ".pdf" )
                //                continue ;

                //            //如果拖进去的文件名跟doc实体文件名一模一样，也不处理
                //            if ( fileName.ToLower() == doc.O_filename.ToLower() )
                //                continue ;

                //            String mFileName = doc.O_filename.Substring( 0,doc.O_filename.LastIndexOf('.'));
                //            //文件名相等，那么作为附件附加上去，并把文件拷贝到AttachFiles目录下
                //            if ( mFileName.ToLower() == docCode.ToLower() )
                //            {
                //                //FTPFactory^ ftp = gcnew FTPFactory( doc.Storage );
                //                //if (!ftp)
                //                //{
                //                //    continue ;
                //                //}
                //                ////注意，直接附加，没有newdoc
                //                //String FileDirPath = doc.FullPathFile.Substring( 0,doc.FullPathFile.LastIndexOf('\\'));
                //                //ftp.upload( gcnew String(filePath), FileDirPath+"\\"+"AttachFiles\\"+fileName );
                //                //ftp.RemoteCopyFile( FileDirPath+"\\"+"AttachFiles\\"+fileName , FileDirPath + "\\" + fileName );
                //                doc.AddAttachFile( fileName );	
                //                return true;
                //            }				
                //        }
                //    }
                //}
                ////小黎 End  

                Doc newDoc = null;

                #region 查找是否已有同名文档
                if (tProject.DocList != null && tProject.DocList.Count > 0)
                {
                    #region 遍历所在目录的所有文件，查找是否有相同文件名的文件
                    string path = tProject.FullPath;
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    if (!Directory.Exists(path))
                    {
                        reJo.msg = "创建存储文件夹失败！";
                        return reJo.Value;
                    }
                    var lstFilename = Directory.GetFiles(path, fileName);

                    foreach (String sRemoteFilePath in lstFilename)
                    {
                        String sRemoteFilename = Path.GetFileName(sRemoteFilePath);

                        //找doc，看看远程重名文件属于哪个doc
                        Doc sameDoc = null;
                        foreach (Doc dd in tProject.DocList)
                        {
                            //找到就停止
                            if (dd.O_filename == sRemoteFilename)
                            {
                                sameDoc = dd;
                                break;
                            }
                            //找不到，在版本中找
                            foreach (Doc dVersionDoc in dd.DocVersionList)
                            {
                                if (dVersionDoc.O_filename == sRemoteFilename)
                                {
                                    sameDoc = dVersionDoc;
                                    break;
                                }
                            }
                            //找不到，在参照中找
                            foreach (Doc dVersionDoc in dd.RefDocList)
                            {
                                if (dVersionDoc.O_filename == sRemoteFilename)
                                {
                                    sameDoc = dVersionDoc;
                                    break;
                                }
                            }
                            //找不到，在关联 中找
                            foreach (Doc dVersionDoc in dd.RelationDocList)
                            {
                                if (dVersionDoc.O_filename == sRemoteFilename)
                                {
                                    sameDoc = dVersionDoc;
                                    break;
                                }
                            }
                        }

                        if (sameDoc != null)
                        {
                            //当有相同文件名，且用户没有输入升版确认的时候，返回让用户确认升版
                            if (string.IsNullOrEmpty(confirmUpgrade))
                            {
                                reJo.success = true;
                                reJo.msg = "ConfirmUpgrade";
                                return reJo.Value;
                            }

                            //int iRet = AfxMessageBox(L"目录["+(CString)tProject.Code+"]下已存在与拖拽文件名["+(CString)fileName+"]同名的文件["+(CString)sRemoteFilename+"]，是否确定升版？\n选择“是”将升版，选择“否”将覆盖替换最高版本文件，选择“取消”跳过该文件。", MB_YESNOCANCEL);
                            //if (CMessagebox::YES == iRet || CMessagebox::YESALL == iRet)
                            if (confirmUpgrade == "true")
                            {
                                //升版
                                newDoc = tProject.NewDoc(fileName, sameDoc.MainDoc.Code, sameDoc.MainDoc.Description);
                                //string newCode = sameDoc.MainDoc.Code;
                                //string newDesc = sameDoc.MainDoc.Description;
                                //newDoc = sameDoc.MainDoc.NewDocVersion();
                                //if (newDoc.O_itemname != newCode) newDoc.O_itemname = newCode;
                                //if (newDoc.O_itemdesc != newDesc) newDoc.O_itemdesc = newDesc;
                                //newDoc.Modify();

                                //if (!newDoc.DocVersionList.Contains(sameDoc))
                                //{
                                //    newDoc.DocVersionList.Add(sameDoc);
                                //    newDoc.Modify();
                                //}
                                //DBSourceController.refreshDBSource(sid);
                                break;
                            }
                            else if (confirmUpgrade == "false")
                            {
                                //覆盖，找到最高版本的doc
                                if (sameDoc.DocVersionList.Count == 0)
                                    newDoc = sameDoc;
                                else
                                    newDoc = sameDoc.MainDoc;
                                break;
                            }
                        }

                    }

                    #endregion

                    //如果上面获取不到newDoc，再看看有没有code相同的
                    if (newDoc == null)
                    {
                        foreach (Doc doc in tProject.DocList)
                        {
                            /*
                            //判断存储服务器那边同目录下是否存在同名文件，或者去掉(VB)(VC)同名，提示覆盖掉最高版本的实体，还是再次升版
                            String^ filename = doc.O_filename;
                            if (!String::IsNullOrEmpty(filename) && filename.IndexOf(docCode) == 0 && filename.EndsWith(fInfo.Extension))
                            {
                                int iRet = AfxMessageBox(L"目录["+(CString)tProject.Code+"]下已存在与拖拽文件名["+(CString)fileName+"]同名的文件["+(CString)filename+"]，是否确定升版？\n选择“是”将升版，选择“否”将覆盖替换最高版本文件，选择“取消”跳过该文件。", MB_YESNOCANCEL);
                                if (IDCANCEL == iRet)
                                    return true;
                                else if (IDYES == iRet)
                                {
                                    //升版
                                    newDoc = tProject.NewDoc( fileName, doc.MainDoc.Code, doc.MainDoc.Description );
                                    break;
                                }
                                else
                                {
                                    //覆盖，找到最高版本的doc
                                    if (doc.DocVersionList.Count == 0)
                                        newDoc = doc;
                                    else
                                        newDoc = doc.MainDoc;
                                    break;
                                }
                            }
                            */

                            //再对去掉扩展名后的文件名跟文档code比较，如果code相同，而扩展名不同，则带扩展名创建doc；code相同，扩展名相同，则直接newdoc，默认升版
                            //取消以上逻辑，改为：code相同，原来没实体，则附加上去；其余情况一律带扩展名NewDoc
                            String filename = doc.O_filename;
                            if (doc.Code == docCode)
                            {
                                if (String.IsNullOrEmpty(filename))
                                {
                                    newDoc = doc;
                                    newDoc.O_filename = fileName;
                                    newDoc.Modify();
                                    break;
                                }
                                //else if (filename.EndsWith(fInfo.Extension))	//扩展名相同
                                //{
                                //升版
                                //	newDoc = tProject.NewDoc( fileName, doc.Code, doc.Description );
                                //	break;
                                //}
                                else
                                {
                                    //带扩展名newdoc，空描述
                                    newDoc = tProject.NewDoc(fileName, fileName, "");
                                    break;
                                }
                            }
                        }
                    }

                }
                #endregion


                //上面都各种获取不到文档或者不升版了，才new（带上扩展名新建）
                if (newDoc == null)
                    newDoc = tProject.NewDoc(fileName, docCode, docDesc);

                if (newDoc == null)
                {
                    reJo.msg = "创建文档失败！";
                    return reJo.Value;
                }


                //返回新建的doc信息给客户端
                List<Doc> docList = new List<Doc>();
                docList.Add(newDoc);

                //组装JSON,并检查读取权限 
                reJo.data = docListToJson(docList, curUser);
                reJo.success = true;

                return reJo.Value;
            }
            catch (Exception ex)
            {
                CommonController.WebWriteLog(ex.Message);
                reJo.msg = "运行错误！错误信息：" + ex.Message;
            }
            return reJo.Value;
        }

        
        //为Project创建子目录或者文档时,根据指定的模板,以及模板的编码规则,获取新的对象代码
        private static string Project_GetNextNewCode(Project project, TempDefn tempDefn, int iProjectOrDoc)
        {
            String newCode = "";
            //Hashtable^ nullHashTable ;

            try
            {
                if (project == null)
                    return newCode;

                //array<String>^ codeList  ; 

                String tempCode = "";
                String tempNum = "";

                String rule = "";

                //判断当前Project是否存在编码规则

                //1.如果选定了模板,先判断模板是否存在编码规则,只针对文档才这样处理
                /*if(tempDefn && iProjectOrDoc == 1)
                {

                    if(tempDefn.AttrTempDefnList && tempDefn.AttrTempDefnList.Count >0)
                    {
                        for each(TempDefn^ attrDefn in tempDefn.AttrTempDefnList)
                        {
                            if((attrDefn.Box_order == 998 || attrDefn.Box_order == 999) && !String.IsNullOrEmpty(attrDefn.Code_Expression) )
                            {
                                rule = attrDefn.Code_Expression ; 
                                break ;
						
                            }
                        }
                    }
                }*/


                //2.如果仍不存在编码规则,则从Project以及其父目录中查找符合要求的规则
                //if(String.IsNullOrEmpty(rule))
                //{
                //	Project^ tempProject = project ; 

                //	while(tempProject)
                //	{
                //		if(tempProject.TempDefn && tempProject.TempDefn.AttrTempDefnList && tempProject.TempDefn.AttrTempDefnList.Count >0)
                //		{
                //			for each(TempDefn^ attrDefn in tempProject.TempDefn.AttrTempDefnList)
                //			{

                //				if(String.IsNullOrEmpty(attrDefn.Code_Expression))
                //					continue ; 

                //				if(  (attrDefn.Box_order == 998  && iProjectOrDoc == 1 && tempProject == project)
                //					//|| (attrDefn.Box_order == 999 && iProjectOrDoc == 1 && tempDefn)
                //					|| (attrDefn.Box_order == 996 && iProjectOrDoc == 0)   //目录编码
                //					|| (attrDefn.Box_order == 997 && tempProject == project && iProjectOrDoc == 0) )  //目录编码
                //				{
                //					rule = attrDefn.Code_Expression ; 
                //					break ;
                //					
                //				}
                //			}
                //		}

                //		tempProject = tempProject.ParentProject ; 
                //	}
                //}


                //if(String.IsNullOrEmpty(rule))
                //{
                //	return newCode  ; 
                //}
                //

                //codeList = project.ExcuteDefnExpression(rule , nullHashTable) ; 

                //if(!codeList || codeList.Length !=1)
                //	return newCode ; 

                //tempCode = codeList[0] ; 

                //if(! (tempCode.ToUpper().LastIndexOf("NUMBER")> 0))
                //	return newCode ; 


                if (iProjectOrDoc == 0/*tempDefn.Attr_type == enAttrType.Project*/)
                {
                    tempNum = project.GetProjectNumber();

                }
                else if (iProjectOrDoc == 1/*tempDefn.Attr_type == enAttrType.Doc*/)
                {
                    if (tempDefn != null && tempDefn.Attr_type == enAttrType.Doc)
                        tempNum = project.GetDocNumber(tempDefn);
                    else
                        tempNum = project.GetDocNumber();
                }

                newCode = tempNum;//tempCode.ToUpper().Substring(0 , tempCode.ToUpper().LastIndexOf("NUMBER")) +  tempNum ;


            }
            catch (Exception ex)
            {
                CommonController.WebWriteLog(ex.Message);
            }

            return newCode;


        }

        /// <summary>
        /// 更新doc属性，附加属性,在调用文档的“新建文档”菜单和“编辑属性”菜单，保存时使用；以及在属性栏编辑修改单个属性，附加属性时使用
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="docKeyword">Doc关键字</param>
        /// <param name="docAttrJson">文档属性，附加属性列表，每个JObject包含："name"，"value","attrtype"三个属性。"attrtype"属性的值为""(空的字符串)时，name有以下值：docCode(Doc代码)，docDesc(Doc描述)，
        /// tempDefnId(模板)；当"attrtype"属性的值为"attrData"(附加属性)时,"name"为附加属性的代码，例如：
        /// [{ name: "CA_REFERENCE", value: recored.data.reference, attrtype: "attrData" },{ name: "CA_VOLUMENUMBER", value: recored.data.volumenumber, attrtype: "attrData" }]</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时且操作类型为"ModiDoc":修改文档时，success返回true,data包含一个空的JObject</para>
        /// <para>操作成功时且操作类型为"CreateDoc":创建文档时，success返回true,data包含一个空的JObject</para>
        /// <para>例子：</para>
        /// </returns>
        public static JObject UpdateDocAttr(string sid, string docKeyword, string docAttrJson)
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

                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(docAttrJson);

                Doc ddoc = dbsource.GetDocByKeyWord(docKeyword);
                if (ddoc == null)
                {
                    reJo.msg = "错误的文档操作信息！指定的文档不存在！";
                    return reJo.Value;
                }

                Doc mDoc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;

                //有修改文档权限才可以修改文档属性
                //以父目录的权限来限定当前文档属性的修改权 
                //bool hasDocEditAttrRight = GetDocDCntrlRight(mDoc, curUser);
                bool hasDocEditAttrRight = GetDocDWriteRight(mDoc, curUser);

                if (!hasDocEditAttrRight)
                {
                    reJo.msg = "修改文档属性失败：用户没有修改文档属性权限！";
                    return reJo.Value;
                }

                //判断是否有修改属性的参数
                bool isUpdateDocStatus = false;
                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();
                    if (strName == "OperateDocStatus")
                    {
                        dbsource.ProgramRun = true;
                        int intDocStatus=Convert.ToInt32(strValue);

                        //当把文件状态修改为检入时，需要先经过COMING_IN状态
                        if (intDocStatus == 2)
                        {

                            mDoc.OperateDocStatus = enDocStatus.COMING_IN;
                            mDoc.OperateDocStatus = (enDocStatus)(intDocStatus);
                            mDoc.FLocker = null;
                            mDoc.O_flocktime = null;
                        }
                        //当把文件状态修改为检出时，需要先经过GOING_OUT状态三
                        else if (intDocStatus == 4)
                        {
                            //判断用户是否有修改文档权限
                            bool hasDFWriteRight = DocController.GetDocDFWriteRight(mDoc, sid);
                            if (hasDFWriteRight ==false)
                            {
                                reJo.msg = "您没有权限修改该文档: " + mDoc.ToString + " ！";
                                return reJo.Value;
                            }

                            //判断文档是否被占用
                            if (mDoc.O_dmsstatus == enDocStatus.OUT && mDoc.FLocker.O_userno != curUser.O_userno)
                            {
                                reJo.msg = "文件已经被用户 " + mDoc.FLocker.ToString + " 锁定,当前只能以只读方式打开文件！";
                                return reJo.Value;
                            }

                            //判断文档是否被占用
                            if (mDoc.O_dmsstatus !=enDocStatus.IN && !(mDoc.O_dmsstatus == enDocStatus.OUT && mDoc.FLocker.O_userno == curUser.O_userno))
                            {
                                reJo.msg = "文件"+ mDoc .ToString+ "不是检入状态！";
                                return reJo.Value;
                            }

                            mDoc.FLocker = curUser;
                            mDoc.O_flocktime = DateTime.Now;
                            mDoc.OperateDocStatus = enDocStatus.GOING_OUT;
                            mDoc.OperateDocStatus = (enDocStatus)(intDocStatus);
                        }
                        else {
                            mDoc.OperateDocStatus = (enDocStatus)(intDocStatus);

                        }
                        mDoc.Modify();
                        isUpdateDocStatus = true;
                        dbsource.ProgramRun = false;
                        break;
                    }
                }

                //如果文件处于检入状态，并且不是修改文件状态属性的，就返回
                if (mDoc.O_dmsstatus != (enDocStatus)2 && !isUpdateDocStatus)
                {
                    reJo.msg = "修改文档属性失败：文档不是处于检入状态！";
                    return reJo.Value;
                }


                bool itemNameError = false;
                string strDocName = "";
                //先检查一遍文件名有没有错误
                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();

                    if (strName == "docCode")
                    {
                        if (mDoc.O_itemname != strValue)//如果修改了文件名
                        {
                            //查询是否已经有同名的文档
                            Project parentProj = mDoc.Project;
                            if (parentProj != null)
                            {
                                bool existItemName = false;
                                foreach (Doc doc in parentProj.DocList)
                                {
                                    if (doc.O_itemname == strValue)
                                    {
                                        existItemName = true;
                                        break;
                                    }
                                }
                                if (!existItemName)
                                {
                                    mDoc.O_itemname = strValue;
                                }
                                else
                                {
                                    itemNameError = true;
                                    strDocName = strValue;
                                }
                            }
                        }
                        break;
                    }

                }

                if (itemNameError == true)//如果修改了文件名，并且目录没有提交的文档的名字，就修改文档属性，否则就返回错误信息
                {
                    reJo.msg = "无法修改文档 " + strDocName + ":指定的文件名与现有文档重名. 请指定另一个文档名称！";
                }
                else
                {
                    foreach (JObject joAttr in jaAttr)
                    {
                        
                        string strName = joAttr["name"].ToString();
                        string strValue = joAttr["value"].ToString();
                        string strType = "";
                        if (joAttr.Property("attrtype") != null)//判断是否是附加属性
                            strType = joAttr["attrtype"].ToString();

                        if (strType != "attrData")//一般属性
                        {
                            try
                            {
                                switch (strName)
                                {
                                    case "docDesc"://如果参数是文件描述

                                        mDoc.O_itemdesc = strValue;
                                        break;
                                    case "tempDefnId"://如果参数是文件描述

                                        if (!string.IsNullOrEmpty(strValue))//如果参数是模板
                                        {
                                            TempDefn tempDefn = dbsource.GetTempDefnByID(Convert.ToInt32(strValue));
                                            if (tempDefn != null)
                                            {
                                                mDoc.TempDefn = tempDefn;
                                            }
                                            else
                                            {
                                                mDoc.TempDefn = null;
                                            }
                                        }
                                        else
                                        {
                                            mDoc.TempDefn = null;
                                        }
                                        break;
                                    case "O_outpath"://修改文件检出路径
                                        mDoc.O_outpath = strValue;
                                        break;
                                    case "OperateDocStatus"://修改文件状态,由于前面已经修改过，这里不需要再次修改
                                        //mDoc.OperateDocStatus = (enDocStatus)(Convert.ToInt32(strValue));
                                        break;
                                    case "FLocker"://修改文件锁定人
                                        if (string.IsNullOrEmpty(strValue))
                                        {
                                            User flUser = null;
                                            mDoc.FLocker = flUser;
                                        }
                                        else if (strValue=="LoginUser")
                                        {
                                            mDoc.FLocker = curUser;
                                        }
                                        break;
                                    case "O_flocktime":
                                        if (string.IsNullOrEmpty(strValue))
                                        {
                                            //DateTime? dtFlockTime = new DateTime?();
                                            //DateTime dtFlockTime = new DateTime();
                                            //mDoc.O_flocktime = dtFlockTime;
                                            mDoc.O_flocktime = DateTime.Now;
                                        }
                                        else if (strValue == "Now")
                                        {
                                            mDoc.O_flocktime = DateTime.Now;
                                        }
                                        break;
                                    case "O_conode":
                                        mDoc.O_conode = strValue;
                                        break;
                                    case "Updater":
                                        if (string.IsNullOrEmpty(strValue))
                                        {
                                            User flUser = null;
                                            mDoc.Updater = flUser;
                                        }
                                        else if (strValue == "LoginUser")
                                        {
                                            mDoc.Updater = curUser;
                                        }
                                        break;
                                }
                            }
                            catch
                            {
                            }
                        }
                        else
                        { //修改文档附加属性
                            AttrDataController.UpdateAttrData(mDoc.AttrDataList, strName, strValue);
                        }
                    }
                    mDoc.Modify();
                    reJo.success = true;
                    return reJo.Value;
                }

            }
            catch (Exception ex)
            {
                CommonController.WebWriteLog(ex.Message);
                reJo.msg = "运行错误！错误信息：" + ex.Message;
            }
            return reJo.Value;
        }

        /// <summary>
        /// 批量创建文档，在调用“批量创建文档”菜单时使用
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="projectKeyword">目录Keyword</param>
        /// <param name="contentJson">文档Json,每个JObject包含："code",Doc代码；"desc",Doc描述</param>
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
        ////}
        ////</code>
        /// </returns>
        //public static JObject BatchCreateDoc(string sid, string projectKeyword, string contentJson)
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

        //        Project parentProj = dbsource.GetProjectByKeyWord(projectKeyword);

        //        bool hasRight = ProjectController.GetProjectDCreateRight(parentProj, curUser);

        //        //有创建文档权限才可以创建目录
        //        if (!hasRight)
        //        {
        //            reJo.msg = "无法创建文档 : 用户没有创建文档权限！";
        //            return reJo.Value;
        //        }

        //        //登录并获取dbsource成功
        //        JArray ja = (JArray)JsonConvert.DeserializeObject(contentJson);
        //        foreach (JObject joCont in ja)
        //        {
        //            string strCode = joCont["code"].ToString();
        //            string strDesc = joCont["desc"].ToString();
        //            CreateDoc(curUser, dbsource, parentProj, strCode, strDesc, "");
        //        }
        //        reJo.success = true;

        //    }
        //    catch (Exception ex)
        //    {
        //        reJo.msg = "运行错误！错误信息：" + ex.Message;
        //        CommonController.WebWriteLog(ex.Message);
        //    }
        //    return reJo.Value;
        //}

        /// <summary>
        /// 获取文档的用户和用户组权限列表
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="DocKeyword">文档Keyword</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>例子：</para>
        /// <code>
        /// {
        ///  "success": true,
        ///  "total": 0,
        ///  "msg": "",
        ///  "data": [
        ///    {
        ///      "IsUser": true,            //是否是用户类型
        ///      "UserName": "admin__administrator",    //用户名
        ///      "ObjectKeyWord": "GJEPCMSU1",          //这里传递的是用户Keyword
        ///      "PFull": true,     //目录所有权限
        ///      "PCreate": true,   //目录创建权限
        ///      "PRead": true,     //目录读权限
        ///      "PWrite": true,    //目录写权限
        ///      "PDelete": true,   //目录删除权限
        ///      "PCntrl": true,    //目录控制权限
        ///      "PNone": false,    //目录无权限
        ///      "DFull": true,     //文档所有权限
        ///      "DCreate": true,   //文档创建权限
        ///      "DRead": true,     //文档读权限
        ///      "DWrite": true,    //文档写权限 (修改文档属性的权限)
        ///      "DDelete": true,   //文档删除权限
        ///      "DFRead": true,    //文档的实体文件读权限
        ///      "DFWrite": true,   //文档的实体文件写权限
        ///      "DCntrl": true,    //文档控制权限
        ///      "DNone": false,    //文档无权限
        ///      "AcceObj": "发文工作流程",   //权限所在目录的名称
        ///      "Enable": true,              //是否禁用
        ///      "Visible": true              //是否显示
        ///    },
        ///    { //下面传递登录用户是否有权限编辑
        ///      "IsUser": true,            
        ///      "UserName": "LoginUser",
        ///      "UserKeyWord": "GJEPCMSU1",    //用户关键字
        ///      "PCntrl": true,                //登录用户是否有权限编辑
        ///      "Visible": false
        ///    }
        ///  ]
        /// }
        ///</code>
        /// </returns>
        public static JObject GetDocRightList(string sid, string DocKeyword)
        {
            return AcceDataController.GetObjectRightList(sid, DocKeyword);
        }

        /// <summary>
        /// 设置Doc权限
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="DocKeyword">文档Keyword</param>
        /// <param name="rightAttrJson">权限json字符串，例如：[{ObjectKeyword: dataItem.ObjectKeyWord(用户或用户组的关键字),
        /// PFull: dataItem.PFull(值是"True"或"False"), PCreate: dataItem.PCreate, PRead: dataItem.PRead, PWrite: dataItem.PWrite, PDelete: dataItem.PDelete, PCntrl: dataItem.PCntrl, PNone: dataItem.PNone,
        /// DFull: dataItem.DFull, DCreate: dataItem.DCreate, DRead: dataItem.DRead, DWrite: dataItem.DWrite, DDelete: dataItem.DDelete, 
        /// DFRead: dataItem.DFRead, DFWrite: dataItem.DFWrite, DCntrl: dataItem.DCntrl, DNone: dataItem.DNone}]</param>
        /// <returns></returns>
        public static JObject SetDocRightList(string sid, string DocKeyword, string rightAttrJson)
        {
            return AcceDataController.SetObjectRightList(sid, DocKeyword, rightAttrJson);
        }

        /// <summary>
        /// 处理新建Doc后的事件
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="DocKeyword">文档Keyword</param>
        /// <returns></returns>
        public static JObject AfterCreateNewDocEvent(string sid, string DocKeyword)
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

                //JArray jaAttr = (JArray)JsonConvert.DeserializeObject(docAttrJson);

                Doc ddoc = dbsource.GetDocByKeyWord(DocKeyword);
                if (ddoc == null)
                {
                    reJo.msg = "错误的文档操作信息！指定的文档不存在！AfterCreateNewDocEvent!";
                    return reJo.Value;
                }
                Doc mDoc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;
                
                //if (AVEVA.CDMS.WebApi.WebExploreEvent.OnAfterCreateNewObject != null)
                //{

                //    ExReJObject exReJo = AVEVA.CDMS.WebApi.WebExploreEvent.OnAfterCreateNewObject(mDoc);
                //    return exReJo.Value;
                //}

                //拖拽新建文档后，调用客户插件，这里使用了多个委托事件，让不同的客户插件调用
                foreach (AVEVA.CDMS.WebApi.WebExploreEvent.Explorer_AfterCreateNewObject_Event_Class AfterCreateNewObject in AVEVA.CDMS.WebApi.WebExploreEvent.ListAfterCreateNewObject)
                {

                    if (AfterCreateNewObject.Event != null)
                    {
                        ExReJObject AfterCreateNewObjectJo = AfterCreateNewObject.Event(AfterCreateNewObject.PluginName, mDoc);
                        if (AfterCreateNewObjectJo.success == false)
                        {
                            AfterCreateNewObjectJo.success = true;
                            return AfterCreateNewObjectJo.Value;
                        }
                    }

                }
                reJo.success = true;
                return reJo.Value;

            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(e.Message);
                return reJo.Value;
            }
            return reJo.Value;
        }


        /// <summary>
        /// 获取文档的用户和用户组权限列表
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="DocKeyword">文档Keyword</param>
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
        //public static JObject GetDocRightList(string sid, string DocKeyword)
        //{
        //    ExReJObject reJo = new ExReJObject();
        //    try
        //    {
        //        User curUser = DBSourceController.GetCurrentUser(sid);
        //        if (curUser==null)
        //        {
        //            reJo.msg = "登录验证失败！请尝试重新登录！";
        //            return reJo.Value;
        //        }

        //        DBSource dbsource =curUser.dBSource;
        //        if (dbsource == null)
        //        {
        //            reJo.msg = "登录验证失败！请尝试重新登录！";
        //            return reJo.Value;
        //        }

        //        Doc doc = dbsource.GetDocByKeyWord(DocKeyword);
        //        if (doc == null)
        //        {
        //            reJo.msg = "文档不存在！";
        //            return reJo.Value;
        //        }

        //        //JObject reJoProj = ProjectController.GetProjectRightList(sid,doc.Project.KeyWord);
        //        //return reJoProj;

        //        JArray jaAcceDataList = new JArray();

        //        List<Acce> acceDataList = new List<Acce>();
        //        if (doc.acceData != null && doc.acceData.Count <= 0)
        //        {
        //            acceDataList = doc.ParentAcceData;
        //        }
        //        else
        //        {
        //            acceDataList = doc.acceData;
        //        }
        //        foreach (Acce acce in acceDataList )
        //        {
        //            string iID = "";
        //            string UserName = "";
        //            bool bIsUser=false;
        //            int iMask=0;

        //            if ( acce.O_memtype == enUserGroupMeberType.User )   //判断用户
        //            {
        //                    bIsUser = true ; 
        //                    iID = acce.user.KeyWord;
        //                    UserName = acce.user.ToString;
        //                    iMask = acce.O_mask; 
        //            }
        //            else if (acce.O_memtype == enUserGroupMeberType.UserGroup) //判断用户组
        //            {
        //                    bIsUser = false ; 
        //                    iID = acce.group.KeyWord;
        //                    UserName = acce.group.ToString;
        //                    iMask = acce.O_mask ; 
        //            }

        //            Right right = new Right(iMask);


        //            //判断权限是否在当前文档获得
        //            bool Enable=(acce.O_objno2==doc.ID?true:false);
        //            if(Enable==false){
        //                Enable = (acce.O_objno == doc.ID ? true : false) && acce.O_objno2 == 0;
        //            }
        //            //权限所在目录
        //            string acceObj = "";
        //            if (Enable == false)
        //            {
        //                acceObj = dbsource.GetDocByID(acce.O_objno).ToString;
        //            }

        //            jaAcceDataList.Add(new JObject(new JProperty("IsUser", bIsUser), new JProperty("UserName", UserName), new JProperty("UserKeyWord", iID),
        //                new JProperty("DFull", right.DFull), new JProperty("DCreate", right.DCreate),
        //                new JProperty("DRead", right.DRead), new JProperty("DWrite", right.DWrite), new JProperty("DDelete", right.DDelete),
        //                new JProperty("DFRead", right.DFRead), new JProperty("DFWrite", right.DFWrite), new JProperty("DCntrl", right.DCntrl),
        //                new JProperty("DNone", right.DNone), new JProperty("Enable", Enable)
        //                ));

        //        }

        //        reJo.data = jaAcceDataList;
        //        reJo.success = true;

        //    }
        //    catch (Exception ex)
        //    {
        //        reJo.msg = "运行错误！错误信息：" + ex.Message;
        //        CommonController.WebWriteLog(ex.Message);
        //    }
        //    return reJo.Value;
        //}


        //判断当前用户是否有替换文档权限
        internal static bool GetDocDFWriteRight(Doc doc, string sid)
        {
            User curUser = DBSourceController.GetCurrentUser(sid);
            if (curUser != null)
                return GetDocDFWriteRight(doc, curUser);
            else
                return false;
        }

        //判断当前用户是否有替换文档权限
        internal static bool GetDocDFWriteRight(Doc doc, User user)
        {
            bool hasRight = false;
            Right right = null;

            //如果是在逻辑目录上传的实体文档，就所有人都可以查看
            if (doc.Project.O_type != enProjectType.Local && doc.ShortCutDoc == null)
            {
                if (doc.Creater==user)
                return true;
            }

            if (doc.acceData != null && doc.acceData.Count > 0)
                right = doc.acceData.GetRight(user);
            else if (doc.ParentAcceData != null && doc.ParentAcceData.Count > 0)
                right = doc.ParentAcceData.GetRight(user);

            //有删除目录权限才可以删除目录
            if (right != null && right.DFWrite)
            { hasRight = true; }
            return hasRight;
        }

        //判断当前用户是否有打开文档权限
        internal static bool GetDocDFReadRight(Doc doc, string sid)
        {

            User curUser = DBSourceController.GetCurrentUser(sid);
            if (curUser != null)
            {
                return GetDocDFReadRight(doc, curUser);
            }

            return false;
        }

        //判断当前用户是否有打开文档权限
        public static bool GetDocDFReadRight(Doc doc, User user)
        {
            bool hasRight = false;
            Right right = null;

            //如果是在逻辑目录上传的实体文档，就所有人都可以查看
            if (doc.Project.O_type != enProjectType.Local && doc.ShortCutDoc == null)
            {
                    return true;
            }

            if (doc.acceData != null && doc.acceData.Count > 0)
                right = doc.acceData.GetRight(user);
            else if (doc.ParentAcceData != null && doc.ParentAcceData.Count > 0)
                right = doc.ParentAcceData.GetRight(user);

            //有删除目录权限才可以删除目录
            if (right != null && right.DFRead)
            { hasRight = true; }
            return hasRight;
        }

        //判断当前用户是否有文档写权限
        internal static bool GetDocDWriteRight(Doc doc, string sid)
        {

            User curUser = DBSourceController.GetCurrentUser(sid);
            if (curUser != null)
            {
                return GetDocDWriteRight(doc, curUser);
            }

            return false;
        }

        //判断当前用户是否有文档写权限
        internal static bool GetDocDWriteRight(Doc doc, User user)
        {
            bool hasRight = false;
            Right right = null;
            if (doc.acceData != null && doc.acceData.Count > 0)
                right = doc.acceData.GetRight(user);
            else if (doc.ParentAcceData != null && doc.ParentAcceData.Count > 0)
                right = doc.ParentAcceData.GetRight(user);

            //有删除目录权限才可以删除目录
            if (right != null && right.DWrite)
            { hasRight = true; }
            return hasRight;
        }
        //判断当前用户是否有文档完全权限
        internal static bool GetDocDFullRight(Doc doc, string sid)
        {

            User curUser = DBSourceController.GetCurrentUser(sid);
            if (curUser != null)
            {
                return GetDocDFullRight(doc, curUser);
            }

            return false;
        }

        //判断当前用户是否有文档完全权限
        internal static bool GetDocDFullRight(Doc doc, User user)
        {
            bool hasRight = false;

            {

                Right right = null;
                if (doc.acceData != null && doc.acceData.Count > 0)
                    right = doc.acceData.GetRight(user);
                else if (doc.ParentAcceData != null && doc.ParentAcceData.Count > 0)
                    right = doc.ParentAcceData.GetRight(user);

                //是否有文档完全权限
                if (right != null && right.DFull)
                {
                    hasRight = true;
                }
            }
            return hasRight;
        }

        //判断当前用户是否有删除文档权限
        internal static bool GetDocDDeleteRight(Doc doc, string sid)
        {

            User curUser = DBSourceController.GetCurrentUser(sid);
            if (curUser != null)
            {
                return GetDocDDeleteRight(doc, curUser);
            }

            return false;
        }

        //判断当前用户是否有删除文档权限
        internal static bool GetDocDDeleteRight(Doc doc, User user)
        {
            bool hasRight = false;

            if (user == doc.Creater)
            {
                //文档创建者获取删除文档权限
                if (doc.WorkFlow == null)
                    return true;
                else {
                    bool OtherUserPass = false;

                    foreach (WorkState ws in doc.WorkFlow.WorkStateList)
                    {

                        foreach (WorkUser wu in ws.WorkUserList)
                        {
                            //检查是否被其他校审人通过
                            if (wu.O_pass.HasValue && wu.User != user)
                            {
                                return false;
                            }
                        }
                    }
                    //如果的文档的创建者，并且文档流程没有被其他校审人通过,就有删除文档权限
                    return true;

                }
            }
            else
            {

                Right right = null;
                if (doc.acceData != null && doc.acceData.Count > 0)
                    right = doc.acceData.GetRight(user);
                else if (doc.ParentAcceData != null && doc.ParentAcceData.Count > 0)
                    right = doc.ParentAcceData.GetRight(user);

                //有删除目录权限才可以删除目录
                if (right != null && right.DDelete)
                {
                    hasRight = true;
                }
            }
            return hasRight;
        }

        //判断当前用户是否有文档属性的修改权 
        internal static bool GetDocDCntrlRight(Doc doc, User user)
        {
            bool hasDWriteRight = false;
            Right right = null;
            //以父目录的权限来限定当前文档属性的修改权 
            if (doc.acceData != null && doc.acceData.Count > 0)
                right = doc.acceData.GetRight(user);
            else if (doc.ParentAcceData != null && doc.ParentAcceData.Count > 0)
                right = doc.ParentAcceData.GetRight(user);

            if (right != null && right.DCntrl)
            {
                hasDWriteRight = true;
            }
            return hasDWriteRight;

        }

        /// <summary>
        /// 判断是否是断点续传文件
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        private static bool ReUploadFile(Doc doc)
        {

            ///如果不存在o_file, 不断点续传
            //string ServerFileName = doc.O_filename;
            ///如果存在o_file, 不断点续传
            //string ServerFileName = doc.O_filename;
            //如果不存在o_file, 断点续传
            string ServerFileName = doc.O_filename;
            if (string.IsNullOrEmpty(ServerFileName)) return true;


            //组成服务器上全路径文件名称
            string FullFileName = "";
            if (ServerFileName.Contains(":") || ServerFileName.StartsWith("\\\\"))
            {
                //用户指定文件
                FullFileName = ServerFileName;
            }
            else if (doc != null)
            {
                //放置在存储下，每个Storage的另一个路径必须保存ISS的虚拟目录
                FullFileName = doc.Storage.O_protocol + (doc.Storage.O_protocol.EndsWith("\\") || doc.Storage.O_protocol.EndsWith("/") ? "" : "\\") + doc.Project.O_projectcode + "\\" + ServerFileName;
            }
            else
            {
                //获取ISO文件
                FullFileName = AppDomain.CurrentDomain.BaseDirectory + ServerFileName;
            }


            FullFileName = FullFileName.Replace("\\", "/");
            


            //4. 如果传输文件存在，则判断是否断点续传 

            //记录客户端文件信息， 文件格式
            //创建时间
            //编辑时间
            //文件大小
            //MD5
            string AttributeFileName = FullFileName + ".Attr";

            //记录传输的数据, 每传输一块就在后面追加
            string DataFileName = FullFileName + ".Data";


            //中间文件存在
            if (System.IO.File.Exists(AttributeFileName) && System.IO.File.Exists(DataFileName))
            {
                return true;
            }
            return false;


        }


        /// <summary>
        /// 获取菜单按钮权限
        /// </summary>
        /// <param name="sid">链接密钥</param>
        /// <param name="DocKeyword">文档关键字</param>
        /// <param name="Menu">菜单名：值有："ModiDocAttr"（编辑文档属性权限）</param>
        /// <returns></returns>
        public static JObject GetMenuRight(string sid, string DocKeyword, string Menu)
        {
            ExReJObject reJo = new ExReJObject();
            try
            {

                //登录用户
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
                Doc doc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;
                //判断是否有编辑文档属性权限
                if (Menu == "ModiDocAttr")
                {
                    bool hasRight = DocController.GetDocDCntrlRight(doc, curUser);

                    //有创建文档权限才可以创建目录
                    if (hasRight)
                    {
                        reJo.success = true;
                        return reJo.Value;
                    }
                    else
                    {
                        reJo.msg = "修改文档属性失败：用户没有修改文档属性权限！";
                        return reJo.Value;
                    }

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
        /// 获取文档的参考文件-版本文件-附加文件列表
        /// </summary>
        /// <param name="sid">连接密钥</param>
        /// <param name="DocKeyword">文档关键字</param>
        /// <returns>
        /// <para>例子：</para>
        /// <code>
        /// {
        ///  "success": true,
        ///  "total": 0,
        ///  "msg": "",
        ///  "data":
        ///     [
        ///   {
        ///    "VersionList":[		//版本文件列表
        ///        {
        ///            "Version":StrVer,	//版本
        ///            "Title":Title,	//文档的标题
        ///            "DocKeyword":DocKeyword, //文档关键字
        ///            "AttachFiles":[
        ///                //旧版本可能会包括参照文件或附加文件，但是不会包括版本了例如：
        ///                {
        ///                    "VersionList":[		//版本文件列表这里都是空的
        ///                    ],
        ///                    "AttachFileList":[	//附加Doc文件列表
        ///                    ],
        ///                    "RefDocList":[		//参考文件列表
        ///                        {
        ///                                      "Title": StrTitle,
        ///                                      "DocKeyword": StrOldVerDocKeyword;
        ///                        }
        ///                    ],
        ///                    "RelationDocList":[	//附加文件列表
        ///                    ]
        ///                }
        ///            ]
        ///        }
        ///    ],
        ///    "AttachFileList":[	//附加Doc文件列表
        ///        {
        ///            "Title": Title
        ///        }
        ///    ],
        ///    "RefDocList":[		//参考文件列表
        ///        {
        ///                        "Title": StrTitle,
        ///                        "DocKeyword": StrOldVerDocKeyword;
        ///        }
        ///    ]
        ///    "RelationDocList":[	//附加文件列表
        ///        {
        ///                        "Title": StrTitle,
        ///                        "DocKeyword": StrOldVerDocKeyword;
        ///        }
        ///    ]
        ///   }
        ///     ]
        /// }
        /// </code>
        /// </returns>
        public static JObject GetAttachFiles(string sid, string DocKeyword)
        {
            ExReJObject reJo = new ExReJObject();
            try
            {
                if (string.IsNullOrEmpty(sid))
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

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

                //获取文档对象
                Doc doc = dbsource.GetDocByKeyWord(DocKeyword);

                if (doc == null)
                {
                    reJo.msg = "文档不存在！";
                    return reJo.Value;
                }

                //JArray reJa = GetDocAttachFiles(doc, true);
                JArray reJa = GetDocAttachFiles(doc);
                reJo.data = reJa;
                reJo.success = true;
                return reJo.Value;
            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(e.Message);
            }
            return reJo.Value;
        }

        //private static JArray GetDocAttachFiles(Doc doc,bool bFirst) {
        private static JArray GetDocAttachFiles(Doc doc) {
            try
            {
                bool bFirst = false;
                //判断是否是主版本
                if (doc.MainDoc == null)
                {
                    bFirst = true;
                }
                else if (doc.ID == doc.MainDoc.ID)
                {
                    bFirst = true;
                }

                #region 1.不存在附件,则去掉附件图标
                if ((doc.AttachFileList == null || doc.AttachFileList.Count <= 0)
                    && (doc.DocVersionList == null || doc.DocVersionList.Count <= 0)
                    && (doc.RefDocList == null || doc.RefDocList.Count <= 0)
                    && (doc.RelationDocList == null || doc.RelationDocList.Count <= 0))
                {
                    //TODO: 不存在附件,则去掉附件图标
                    return new JArray();
                }
                #endregion

                #region 2.1 添加版本
                //4.添加版本
                User curUser = doc.dBSource.LoginUser;
                JArray jaVersionList = new JArray();
                if (bFirst)
                {
                    if (doc.DocVersionList != null && doc.DocVersionList.Count > 0)
                    {
                        foreach (Doc oldVerDoc in doc.DocVersionList)
                        {
                            //小黎 2012-4-1 增加对多版本的显示,屏蔽以下图像,增加支持多版本
                            String StrVer = oldVerDoc.O_version;
                            String StrTitle = oldVerDoc.ToString;
                            //String StrOldVerDocKeyword=oldVerDoc.O_itemno;
                            String StrOldVerDocKeyword = oldVerDoc.KeyWord;
                            JArray subJa = new JArray();
                            ///小黎 2012-4-1 版本有时也有附件，也要显示出来
                            //g_bFlagInserted = false;
                            if (bFirst)
                            {
                                if (!((oldVerDoc.AttachFileList == null || oldVerDoc.AttachFileList.Count <= 0)
                                    && (oldVerDoc.RefDocList == null || oldVerDoc.RefDocList.Count <= 0)
                                    && (oldVerDoc.RelationDocList == null || oldVerDoc.RelationDocList.Count <= 0)))
                                {
                                    //subJa = GetDocAttachFiles(doc, false);
                                    subJa = GetDocAttachFiles(doc);
                                }

                                //g_bLastVersionLine = false;
                            }
                            List<Doc> docList = new List<Doc>() { oldVerDoc};
                            JArray docData = docListToJson(docList, curUser);

                            jaVersionList.Add(new JObject(
                                new JProperty("Version", StrVer),
                                new JProperty("Title", StrTitle),
                                new JProperty("DocKeyword", StrOldVerDocKeyword),
                                new JProperty("DocInfo",docData),
                                new JProperty("AttachFiles", subJa)
                                ));
                        }
                    }
                }
                #endregion

                #region 2.2 添加附件
                JArray jaAttachFileList = new JArray();
                //2.添加附件
                if (doc.AttachFileList != null && doc.AttachFileList.Count > 0)
                {

                    foreach (String attachFile in doc.AttachFileList)
                    {
                        jaAttachFileList.Add(new JObject("Title", attachFile));
                    }
                }
                #endregion

                #region 2.3.添加参考
                JArray jaRefDocList = new JArray();
                //3.添加参考
                if (doc.RefDocList != null && doc.RefDocList.Count > 0)
                {
                    foreach (Doc oldVerDoc in doc.RefDocList)
                    {
                        String StrTitle = oldVerDoc.ToString;
                        String StrOldVerDocKeyword = oldVerDoc.KeyWord;

                        List<Doc> docList = new List<Doc>() { oldVerDoc };
                        JArray docData = docListToJson(docList, curUser);

                        jaRefDocList.Add(new JObject(
                        new JProperty("Title", StrTitle),
                        new JProperty("DocKeyword", StrOldVerDocKeyword),
                         new JProperty("DocInfo", docData)));

                    }
                }
                #endregion

                #region 2.4.添加附件Doc
                JArray jaRelationDocList = new JArray();
                //5.添加附件Doc
                if (doc.RelationDocList != null && doc.RelationDocList.Count > 0)
                {
                    foreach (Doc oldVerDoc in doc.RelationDocList)
                    {
                        String StrTitle = oldVerDoc.ToString;
                        String StrOldVerDocKeyword = oldVerDoc.KeyWord;

                        List<Doc> docList = new List<Doc>() { oldVerDoc };
                        JArray docData = docListToJson(docList, curUser);

                        jaRelationDocList.Add(new JObject(
                        new JProperty("Title", StrTitle),
                        new JProperty("DocKeyword", StrOldVerDocKeyword),
                        new JProperty("DocInfo", docData)));
                    }
                }
                #endregion

                JObject joData = new JObject();
                //当前版本，就显示其他版本文件的信息；其他版本，就不重复显示了
                if (bFirst)
                    joData.Add(new JProperty("VersionList", jaVersionList));

                joData.Add(new JProperty("AttachFileList", jaAttachFileList));
                joData.Add(new JProperty("RefDocList", jaRefDocList));
                joData.Add(new JProperty("RelationDocList", jaRelationDocList));

                JArray jaData = new JArray(joData);

                //JArray jaData = new JArray(new JObject(
                //    new JProperty("VersionList", jaVersionList),
                //    new JProperty("AttachFileList", jaAttachFileList),
                //    new JProperty("RefDocList", jaRefDocList),
                //    new JProperty("RelationDocList", jaRelationDocList)
                //    ));
                return jaData;
            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
            }
            return new JArray();
        }

//        // 显示或隐藏指定Item的附件/版本
//bool CYDListView::ShowAttachFiles(int iItem, BOOL bShow, bool bFirst)
//{
//    try
//    {

//        if(iItem<0 || iItem >= this->GetListCtrl().GetItemCount())
//            return false ; 

//        int iSubPlugItem = this->GetColumnIndexByName(L"A") ;  



//        //获取iItem对应的对象
//        LVITEM lvi ; 
//        ::ZeroMemory(&lvi ,sizeof(lvi)) ; 

//        lvi.iItem = iItem ; 
//        lvi.mask = LVIF_PARAM ; 

//        if(!this->GetListCtrl().GetItem(&lvi))
//            return false ; 

//        int iParam = (int)lvi.lParam ; 

//        if(!Global::listHashTable->Contains(iParam))
//            return false ; 

//        Object^ obj = Global::listHashTable[iParam] ; 

//        if(obj->ToString()->ToLower() != Global::typeDocStr->ToLower())
//            return false ; 

//        Doc^ doc = (Doc^)obj ; 

//        if(!doc)
//            return false ; 




//#pragma region 
//        CString strColName = L"A" ; 
//        int indexFixAttach = -1  ; 

//        CHeaderCtrl* pHead = this->m_pFixListCtrl->GetHeaderCtrl() ; 

//        enum   { sizeOfBuffer = 256 };
//        TCHAR  lpBuffer[sizeOfBuffer];


//        HDITEM hdi ; 
//        hdi.mask = HDI_TEXT;
//        hdi.pszText = lpBuffer;
//        hdi.cchTextMax = sizeOfBuffer;

//        for(int i = 0 ; i<pHead->GetItemCount() ; i++)
//        {
//            pHead->GetItem(i , &hdi) ; 
//            CString name = hdi.pszText ; 

//            if (name == strColName)
//            {
//                indexFixAttach = i ; 
//                break ; 
//            }

//        }
//#pragma endregion



//        if( (!doc->AttachFileList || doc->AttachFileList->Count <=0) 
//            && (!doc->DocVersionList || doc->DocVersionList->Count <=0) 
//            &&(!doc->RefDocList || doc->RefDocList->Count <=0) 
//            && (!doc->RelationDocList || doc->RelationDocList->Count <= 0))
//        {
//            //TODO: 不存在附件,则去掉附件图标
//            return false ; 
//        }

//        int iIndex = iItem +1 ; 
//        if(bShow)
//        {
//            //判断该Doc的附件是否显示了,如果没有,则添加到ListView中

//            //TODO: 1.判断,删除

//            //4.添加版本
//            if (bFirst)
//            {
//                if(doc->DocVersionList && doc->DocVersionList->Count > 0)
//                {
//                    for each(Doc^ oldVerDoc in doc->DocVersionList)
//                    {
//                        //小黎 2012-4-1 增加对多版本的显示,屏蔽以下图像,增加支持多版本
//                        String^ StrVer =  oldVerDoc->O_version;
//                        int ImageIndex = this->GetDocVersionInImageList(StrVer) ;
//                        if ( ImageIndex == -1 )
//                            ImageIndex = Global::ImageManager->get_iVersionDocImg();

//                        //int iNewAttach = this->GetListCtrl().InsertItem(iIndex++ , (CString)oldVerDoc->ToString , Global::g_iDocImg) ; 

//                        int iNewAttach = this->GetListCtrl().InsertItem(LVIF_IMAGE | LVIF_TEXT | LVIF_STATE|LVIF_PARAM , iIndex, (CString)oldVerDoc->ToString, INDEXTOSTATEIMAGEMASK(0), LVIS_STATEIMAGEMASK,/*Global::g_iVersionDocImg*/ImageIndex ,oldVerDoc->O_itemno/*n-1*/);
//                        //this->GetListCtrl().SetItemText(iNewAttach , iSubPlugItem ,L"┠") ; 

//                        if(iNewAttach>0)
//                        {
//                            LVITEM lvsubItem  ; 
//                            ::ZeroMemory(&lvsubItem , sizeof(lvsubItem)) ; 

//                            lvsubItem.iItem = iNewAttach ; 
//                            lvsubItem.iSubItem = iSubPlugItem ;
//                            lvsubItem.mask = LVIF_IMAGE ;
//                            lvsubItem.iImage = /*Global::g_iAttachFileMaskImg*/Global::ImageManager->get_iAttachFileMaskImg() ; 

//                            this->GetListCtrl().SetItem(&lvsubItem) ; 

//                            ::ListView_InsertDocSubItem(this , oldVerDoc , iNewAttach);
//                            this->GetListCtrl().SetItemState(iNewAttach , INDEXTOSTATEIMAGEMASK(8) ,LVIS_STATEIMAGEMASK) ; 
//                            //::WriteLog((CString)("\r\n"+DateTime::Now.ToString("yyyyMMddHHmmssfff") + "  " + "完成子文档属性显示 :" +doc->ToString )) ; 
					
//                            if(!Global::listHashTable->Contains(oldVerDoc->O_itemno))
//                                Global::listHashTable->Add(oldVerDoc->O_itemno , oldVerDoc);	

//                            lvsubItem.iSubItem = indexFixAttach ; 
//                            this->m_pFixListCtrl->SetItem(&lvsubItem) ; 							

//                            ///小黎 2012-4-1 版本有时也有附件，也要显示出来
//                            g_bFlagInserted = false;
//                            if (bFirst)
//                            {
//                                this->ShowAttachFiles(iIndex, true, false);
//                                g_bLastVersionLine = false;
//                            }
//                        }
//                    }
//                }
//            }
//            //2.添加附件
//            if(doc->AttachFileList && doc->AttachFileList->Count >0)
//            {
				
//                for each(String^ attachFile in doc->AttachFileList)
//                {
//                    int iNewAttach = this->GetListCtrl().InsertItem(iIndex , (CString)attachFile , /*Global::g_iAttachFileImg*/Global::ImageManager->get_iAttachFileImg()) ; 

//                    //this->GetListCtrl().SetItemText(iNewAttach , iSubPlugItem ,L"┠ ") ; 

//                    if(iNewAttach>0)
//                    {
//                        //小黎 2012-4-1 屏蔽掉设置A列的图标,现改成在状态图标里面改动图标
//                        LVITEM lvsubItem  ; 
//                        ::ZeroMemory(&lvsubItem , sizeof(lvsubItem)) ; 

//                        lvsubItem.iItem = iNewAttach ; 
//                        lvsubItem.iSubItem = iSubPlugItem ;
//                        lvsubItem.mask = LVIF_IMAGE ; 
//                        lvsubItem.iImage = g_bLastVersionLine ?  Global::ImageManager->get_iXrefFileMaskImgLast() :Global::ImageManager->get_iXrefFileMaskImg(); 

//                        this->GetListCtrl().SetItem(&lvsubItem) ; 						
//                        this->GetListCtrl().SetItemState(iNewAttach , INDEXTOSTATEIMAGEMASK(7) ,LVIS_STATEIMAGEMASK) ; 

//                        lvsubItem.iSubItem = indexFixAttach ; 
//                        this->m_pFixListCtrl->SetItem(&lvsubItem) ; 

//                        //this->GetListCtrl().SetItemState(iNewAttach , INDEXTOSTATEIMAGEMASK(7) ,LVIS_STATEIMAGEMASK) ; 

//                        //this->m_pFixListCtrl->SetItemState(iNewAttach , INDEXTOSTATEIMAGEMASK(7) ,LVIS_STATEIMAGEMASK) ;


//                    }

//                }
//            }
//            //3.添加参考
//            if(doc->RefDocList && doc->RefDocList->Count >0)
//            {
//                for each(Doc^ oldVerDoc in doc->RefDocList)
//                {
//                    //int iNewAttach = this->GetListCtrl().InsertItem(iIndex++ , (CString)oldVerDoc->ToString , Global::g_iDocImg) ; 
//                    int iNewAttach = this->GetListCtrl().InsertItem(LVIF_IMAGE | LVIF_TEXT | LVIF_STATE|LVIF_PARAM , iIndex, (CString)oldVerDoc->ToString, INDEXTOSTATEIMAGEMASK(0), LVIS_STATEIMAGEMASK,/*Global::g_iRefDocImg */Global::ImageManager->get_iRefDocImg(),oldVerDoc->O_itemno/*n-1*/);
//                    //this->GetListCtrl().SetItemText(iNewAttach , iSubPlugItem ,L"┠") ; 

//                    if(iNewAttach>0)
//                    {
//                        LVITEM lvsubItem  ; 
//                        ::ZeroMemory(&lvsubItem , sizeof(lvsubItem)) ; 

//                        lvsubItem.iItem = iNewAttach ; 
//                        lvsubItem.iSubItem = iSubPlugItem ;
//                        lvsubItem.mask = LVIF_IMAGE ; 
//                        lvsubItem.iImage = /*Global::g_iAttachFileMaskImg*/g_bLastVersionLine ?  Global::ImageManager->get_iXrefFileMaskImgLast() :Global::ImageManager->get_iXrefFileMaskImg() ; 

//                        this->GetListCtrl().SetItem(&lvsubItem) ; 

//                        ::ListView_InsertDocSubItem(this , oldVerDoc , iNewAttach);
//                        this->GetListCtrl().SetItemState(iNewAttach , INDEXTOSTATEIMAGEMASK(7) ,LVIS_STATEIMAGEMASK) ; 
//                        //::WriteLog((CString)("\r\n"+DateTime::Now.ToString("yyyyMMddHHmmssfff") + "  " + "完成子文档属性显示 :" +doc->ToString )) ; 
				
//                        if(!Global::listHashTable->Contains(oldVerDoc->O_itemno))
//                            Global::listHashTable->Add(oldVerDoc->O_itemno , oldVerDoc);	

//                        lvsubItem.iSubItem = indexFixAttach ; 
//                        this->m_pFixListCtrl->SetItem(&lvsubItem) ; 
//                    }					
//                }
//            }

//            //5.添加附件Doc
//            if(doc->RelationDocList && doc->RelationDocList->Count >0)
//            {
//                for each(Doc^ oldVerDoc in doc->RelationDocList)
//                {
//                    //int iNewAttach = this->GetListCtrl().InsertItem(iIndex++ , (CString)oldVerDoc->ToString , Global::g_iDocImg) ; 
//                    int iNewAttach = this->GetListCtrl().InsertItem(LVIF_IMAGE | LVIF_TEXT | LVIF_STATE|LVIF_PARAM , iIndex, (CString)oldVerDoc->ToString, INDEXTOSTATEIMAGEMASK(0), LVIS_STATEIMAGEMASK,/*Global::g_iRefDocImg */Global::ImageManager->get_iAttachFileImg(),oldVerDoc->O_itemno/*n-1*/);
//                    //this->GetListCtrl().SetItemText(iNewAttach , iSubPlugItem ,L"┠") ; 

//                    if(iNewAttach>0)
//                    {
//                        if (!bFirst)
//                            g_bFlagInserted = true;
//                        LVITEM lvsubItem  ; 
//                        ::ZeroMemory(&lvsubItem , sizeof(lvsubItem)) ; 

//                        lvsubItem.iItem = iNewAttach ; 
//                        lvsubItem.iSubItem = iSubPlugItem ;
//                        lvsubItem.mask = LVIF_IMAGE ; 
//                        lvsubItem.iImage = /*Global::g_iAttachFileMaskImg*/g_bLastVersionLine ?  Global::ImageManager->get_iXrefFileMaskImgLast() : Global::ImageManager->get_iXrefFileMaskImg() ; 

//                        this->GetListCtrl().SetItem(&lvsubItem) ; 

//                        ::ListView_InsertDocSubItem(this , oldVerDoc , iNewAttach);
//                        this->GetListCtrl().SetItemState(iNewAttach , INDEXTOSTATEIMAGEMASK(7) ,LVIS_STATEIMAGEMASK) ; 
//                        //::WriteLog((CString)("\r\n"+DateTime::Now.ToString("yyyyMMddHHmmssfff") + "  " + "完成子文档属性显示 :" +doc->ToString )) ; 
				
//                        if(!Global::listHashTable->Contains(oldVerDoc->O_itemno))
//                            Global::listHashTable->Add(oldVerDoc->O_itemno , oldVerDoc);	
						
//                        lvsubItem.iSubItem = indexFixAttach ; 
//                        this->m_pFixListCtrl->SetItem(&lvsubItem) ; 
//                    }					
//                }
//            }

			
//            LVITEM lvi3;
//            ::ZeroMemory(&lvi3 , sizeof(LVITEM));
//            lvi3.iItem = iItem ;
//            lvi3.iSubItem = iSubPlugItem ;

//            lvi3.mask = LVIF_IMAGE ; 

//            this->GetListCtrl().GetItem(&lvi3) ; 

//            if( lvi3.iImage == /*Global::g_iReduceImg*/Global::ImageManager->get_iPlugImg())
//            {
//                lvi3.iImage = /*Global::g_iPlugImg*/ Global::ImageManager->get_iReduceImg(); 

//                this->GetListCtrl().SetItem(&lvi3) ; 


//                lvi3.iSubItem = indexFixAttach ; 
//                this->m_pFixListCtrl->SetItem(&lvi3) ; 
//            }
//            else
//            {
//                lvi3.iSubItem = indexFixAttach ; 

//                this->m_pFixListCtrl->GetItem(&lvi3) ; 


//            if( lvi3.iImage == /*Global::g_iReduceImg*/Global::ImageManager->get_iPlugImg())
//            {
				
//                lvi3.iImage = /*Global::g_iPlugImg*/ Global::ImageManager->get_iReduceImg(); 

		
//                this->m_pFixListCtrl->SetItem(&lvi3) ; 
//            }
//            }

//        }
//        else
//        {



//            LVITEM lvi2;
//            ::ZeroMemory(&lvi2 , sizeof(LVITEM));
//            lvi2.iItem = iItem ;
//            lvi2.iSubItem = iSubPlugItem ;

//            lvi2.mask = LVIF_IMAGE ; 

//            this->GetListCtrl().GetItem(&lvi2) ; 


//            //TIM 2010-06-03
//            if( lvi2.iImage != /*Global::g_iReduceImg*/Global::ImageManager->get_iReduceImg()
//                && lvi2.iImage != Global::ImageManager->get_iPlugImg())
//                return false ; 




//            //判断Doc的附件是否显示了,如果有,则删除掉
//            int ItemCount = iItem +1;

//            LVITEM lvFItem ;
//            ::ZeroMemory(&lvFItem ,sizeof(LVITEM)) ; 
//            lvFItem.iItem = ItemCount ; 
//            lvFItem.iSubItem = iSubPlugItem ; 
//            lvFItem.mask = LVIF_IMAGE ; 

//            while(this->GetListCtrl().GetItem(&lvFItem) && (lvFItem.iImage == Global::ImageManager->get_iAttachFileMaskImg()
//                                                         || lvFItem.iImage == Global::ImageManager->get_iXrefFileMaskImg()
//                                                         || lvFItem.iImage == Global::ImageManager->get_iXrefFileMaskImgLast()))
//            {
				

//                LVITEM lvItem ;
//                ::ZeroMemory(&lvItem , sizeof(LVITEM)) ; 
//                lvItem.iItem = ItemCount ; 
//                lvItem.mask = LVIF_PARAM ; 
//                this->GetListCtrl().GetItem(&lvItem) ; 

//                int iParam = (int)lvItem.lParam ;

//                if(Global::listHashTable->Contains(iParam))
//                    Global::listHashTable->Remove(iParam) ; 

//                this->GetListCtrl().DeleteItem(ItemCount) ; 


//                ItemCount = iItem +1 ; 

//                lvFItem.iItem = ItemCount ; 
//            }



//            if( lvi2.iImage == /*Global::g_iReduceImg*/Global::ImageManager->get_iReduceImg())
//            {
//                lvi2.iImage = /*Global::g_iPlugImg*/ Global::ImageManager->get_iPlugImg(); 

//                this->GetListCtrl().SetItem(&lvi2) ; 

//                lvi2.iSubItem = indexFixAttach ; 
//                this->m_pFixListCtrl->SetItem(&lvi2) ; 
//            }
//            else
//            {
//                lvi2.iSubItem = indexFixAttach ; 

//                this->m_pFixListCtrl->GetItem(&lvi2) ; 


//                if( lvi2.iImage == /*Global::g_iReduceImg*/Global::ImageManager->get_iReduceImg())
//                {
					
//                    lvi2.iImage = /*Global::g_iPlugImg*/ Global::ImageManager->get_iPlugImg(); 

			
//                    this->m_pFixListCtrl->SetItem(&lvi2) ; 
//                }
//            }
			

			
//        }

//    }catch(Exception^ e)
//    {
//        ::ErrorMessage(e->Message , e->StackTrace) ; 
//    }
//    return false;
//}


    }
}
