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
    /// Project操作类
    /// </summary>  
    public class ProjectController : Controller
    {


        /// <summary>
        /// 获取根目录
        /// </summary>
        /// <param name="sid">登录关键字</param>
        /// <param name="ProjetType">根目录文件类型</param>
        /// <returns>Project List</returns>
        private static List<Project> GetRootProjectList(DBSource dbsource, int ProjetType)
        {

            //目录类型
            enProjectType ptype = (enProjectType)ProjetType;
            switch (ptype)
            {
                case enProjectType.Local:

                    //return DBSourceController.dbManager.shareDBManger.DBSourceList[0].RootLocalProjectList;
                    return dbsource.RootLocalProjectList;

                case enProjectType.GlobCustom:

                    return dbsource.RootGCustProjectList;

                case enProjectType.GlobSearch:

                    return dbsource.RootGSchProjectList;

                case enProjectType.UserCustom:

                    return dbsource.RootUCustProjectList;

                case enProjectType.UserSearch:

                    return dbsource.RootUSchProjectList;

            }

            //return null;
            return null;
        }



        /// <summary>
        /// 获取Project对象的子Project列表
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="node">Project节点关键字完整路径，用"_"分割,用于区分快捷目录，例子：
        /// SHEPCP8289SLocal_SHEPCP8290SLocal_SHEPCP8315SLocal_SHEPCP8332SLocal</param>
        /// <param name="ProjectType">Project类型，1：项目目录，4，：查询，5：个人工作台，7：逻辑目录</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,total返回记录总数；操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时，data包含多个JObject，每个JObject里面包含参数"id":节点关键字,"text":节点文本,"leaf":是否有子节点,
        /// "iconCls"：设置图标</para>
        /// <para>例子：</para>
        /// </returns>

        public static JObject GetProjectListJson(string sid, string node, string ProjectType)
        {
            ExReJObject reJo = new ExReJObject();
            try
            {

                //获取变量
                //获取Project的keyword
                string keyword = node ?? "Root";
                keyword = keyword.LastIndexOf("_") < 0 ? keyword : keyword.Substring(keyword.LastIndexOf("_") + 1);
                string projectType = ProjectType ?? "1";


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

                //获取Project List 
                List<Project> projectList;
                if (keyword == "Root")
                {
                    //如果是访问根目录，就刷新一下数据源（按下F5自动刷新数据源）
                    DBSourceController.refreshDBSource(sid, "1");

                    projectList = GetRootProjectList(dbsource, Convert.ToInt32(projectType));
                }
                else
                {


                    Project p = dbsource.GetProjectByKeyWord(keyword);

                    if (p.ShortProject != null)
                    {
                        //判断是否快捷目录
                        p = p.ShortProject;
                    }

                    projectList = p.ChildProjectList;

                }

                //2018-6-30 小钱，按照华西能源要求，文件夹图标需要按用户的条件显示
                #region 根据用户设置的条件，修个显示的文件夹图标
                //根据用户设置的条件，修个显示的文件夹图标
                //foreach (WebDocEvent.Before_Get_Doc_List_Event_Class BeforeGetDocs in WebDocEvent.ListBeforeGetDocs)
                foreach (WebProjectEvent.Before_Get_Project_List_Event_Class BeforeGetProject in WebProjectEvent.ListBeforeGetProjects)
                {
                    if (BeforeGetProject.Event != null)
                    {
                        //如果在用户事件中筛选过了，就跳出事件循环
                        if (BeforeGetProject.Event(BeforeGetProject.PluginName, ref projectList))
                        {
                            break;
                        }
                    }
                }
                #endregion

                //获取同一DBsource 对象里面的 User, 避免发生线程冲突
                curUser = dbsource.GetUserByID(curUser.O_userno);

                JArray reJa = new JArray();

                List<string> AddedProjList = new List<string>();

                //循环处理
                foreach (var p in projectList)  //查找某个字段与值
                {
                    try
                    {
                        //2019-9-20 qian 去掉已经添加过的目录
                        if (AddedProjList.Contains(p.Code))
                        {
                            continue;
                        }

                        //快捷目录
                        bool IsShort = false;
                        Project pp = p;


                        if (p.ShortProject != null)
                        {
                            //小钱2017-11-22 由于extjs里面Keyword只能是唯一的值，如果把快捷目录转换成实体目录传给js端，
                            //展开实体目录后再展开快捷目录，会出现冲突问题，这里把快捷目录转成实体目录取消掉
                            pp = p.ShortProject;
                            IsShort = true;
                        }

                        //当前用户的权限权限
                        AcceData accedata = new AcceData();
                        try
                        {
                            accedata = pp.acceData;
                        }
                        catch
                        {

                            //如果获取文件夹权限出错，尝试删除文件夹流程，修复文件夹权限
                            if (pp.WorkFlow != null)
                            {
                                pp.WorkFlow.Delete();
                                pp.WorkFlow.Delete();
                                accedata = pp.acceData;
                            }
                        }
                        Right r = accedata.GetRight(curUser);


                        //有权限读
                        if (r != null && r.PRead)
                        {

                            //设置目录图标
                            string folderType = "";
                            if (IsShort)
                            {
                                folderType = "readonlyfolder";  //快捷目录
                            }
                            //else if (projectType == "1" || projectType == "5")//当查询类型是项目目录或个人工作台或逻辑目录
                            else if (pp.O_type == enProjectType.Local || pp.O_type == enProjectType.UserCustom || pp.O_type == enProjectType.GlobCustom)
                            {
                                if (r != null && r.PWrite)
                                    folderType = "writefolder";
                                else
                                    folderType = "readonlyfolder";
                            }
                            //else if (projectType == "4" || projectType == "7") //当查询类型是个人查询，全局查询
                            else if (pp.O_type == enProjectType.GlobSearch || pp.O_type == enProjectType.UserSearch)
                            {
                                folderType = "searchfolder";
                            }

                            //2018-6-30 小钱，按照华西能源要求，文件夹图标需要按用户的条件显示
                            #region 根据用户设置的条件，修个显示的文件夹图标
                            //根据用户设置的条件，修个显示的文件夹图标
                            //foreach (WebDocEvent.Before_Get_Doc_List_Event_Class BeforeGetDocs in WebDocEvent.ListBeforeGetDocs)
                            foreach (WebProjectEvent.After_Get_Project_Icon_Event_Class AfterGetProjectIcon in WebProjectEvent.ListAfterGetProjectIconEvent)
                            {
                                if (AfterGetProjectIcon.Event != null)
                                {
                                    //如果在用户事件中筛选过了，就跳出事件循环
                                    if (AfterGetProjectIcon.Event(AfterGetProjectIcon.PluginName, pp, ref folderType))
                                    {
                                        break;
                                    }
                                }
                            }
                            #endregion

                            //是否有子节点
                            bool projectLeaf = pp.ShortProject == null ?
                                (pp.O_subprojects == "Y" ? false : true) :
                                (pp.ShortProject.O_subprojects == "Y" ? false : true);

                            AddedProjList.Add(p.Code);

                            //组装js 字符串，把Jobject添加到返回数据Jarray
                            reJa.Add(new JObject
                        { 
                            new JProperty("Keyword",p.KeyWord),//节点关键字
                            new JProperty("id",node=="Root"?p.KeyWord:node+"_"+p.KeyWord),//project完整路径
                            new JProperty("text",pp.ToString),//节点文本
                            new JProperty("leaf", projectLeaf),//没有子节点
                            new JProperty("iconCls",folderType),//设置图标
                            new JProperty("isShort",IsShort),//是否快捷目录
                            //new JProperty("docTotal",pp.DocList.Count),//文档统计
                        });
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonController.WebWriteLog(ex.Message);
                    }
                }

                reJo.data = reJa;
                reJo.total = reJo.data.Count;
                reJo.success = true;

            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(reJo.msg);
            }
            //return Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
            return reJo.Value;
        }

        /// <summary>
        /// 返回一个Project对象的基本属性，在显示目录基本属性栏时使用
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="ProjectKeyWord">Project节点关键字</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时，data包含多个JObject， 每个JObject里面包含参数：</para>
        /// <para>基本属性包含的参数："AttrCode"：属性代码,"AttrName"：属性描述"AttrValue":属性值，"AttrType",属性类型("Attr":属性),"Visible":是否显示("True")</para>
        /// <para>例子：</para>
        /// <code>
        /// {
        ///  "success": true,
        ///  "total": 0,
        ///  "msg": "",
        ///  "data": [
        ///    {
        ///      "AttrCode": "GHEPCMS200119H",
        ///      "AttrName": "目录名",
        ///      "AttrValue": "GHEPCMS200119H",
        ///      "AttrType": "Attr",
        ///      "Visible": "True"
        ///    },
        ///    {
        ///      "AttrCode": "广建EPC项目H",
        ///      "AttrName": "目录描述",
        ///      "AttrValue": "广建EPC项目H",
        ///      "AttrType": "Attr",
        ///      "Visible": "True"
        ///    },
        ///    {
        ///      "AttrCode": "",
        ///      "AttrName": "版本号",
        ///      "AttrValue": "",
        ///      "AttrType": "Attr",
        ///      "Visible": "True"
        ///    },
        ///    {
        ///      "AttrCode": "0",
        ///      "AttrName": "版本顺序",
        ///      "AttrValue": 0,
        ///      "AttrType": "Attr",
        ///      "Visible": "True"
        ///    },
        ///    {
        ///      "AttrCode": "2020/1/19 15:55:33",
        ///      "AttrName": "创建时间",
        ///      "AttrValue": "2020/1/19 15:55:33",
        ///      "AttrType": "Attr",
        ///      "Visible": "True"
        ///    },
        ///    {
        ///      "AttrCode": "2020/1/19 15:55:33",
        ///      "AttrName": "更新时间",
        ///      "AttrValue": "2020/1/19 15:55:33",
        ///      "AttrType": "Attr",
        ///      "Visible": "True"
        ///    },
        ///    {
        ///      "AttrCode": "IN",
        ///      "AttrName": "目录状态",
        ///      "AttrValue": "IN",
        ///      "AttrType": "Attr",
        ///      "Visible": "True"
        ///    },
        ///    {
        ///      "AttrCode": "Local",
        ///      "AttrName": "目录类型",
        ///      "AttrValue": "Local",
        ///      "AttrType": "Attr",
        ///      "Visible": "True"
        ///    },
        ///    {
        ///      "AttrCode": "10174",
        ///      "AttrName": "创建者",
        ///      "AttrValue": "suhuahui__苏华卉",
        ///      "AttrType": "Attr",
        ///      "Visible": "True"
        ///    },
        ///    {
        ///      "AttrCode": "10174",
        ///      "AttrName": "更新者",
        ///      "AttrValue": "suhuahui__苏华卉",
        ///      "AttrType": "Attr",
        ///      "Visible": "True"
        ///    },
        ///    {
        ///      "AttrCode": "",
        ///      "AttrName": "工作流",     
        ///      "AttrValue": "",
        ///      "AttrType": "Attr",
        ///      "Visible": "True"
        ///    },
        ///    {
        ///      "AttrCode": "O_WorkFlowno",    //判断是否显示流程页
        ///      "AttrName": "流程ID",
        ///      "AttrValue": "",
        ///      "AttrType": "Attr",
        ///      "Visible": "True"
        ///    },
        ///    {
        ///      "AttrCode": "",
        ///      "AttrName": "存储空间",
        ///      "AttrValue": "GJEPCStorage__文件存储",
        ///      "AttrType": "Attr",
        ///      "Visible": "True"
        ///    },
        ///    {
        ///      "AttrCode": "1726",
        ///      "AttrName": "模板",
        ///      "AttrValue": "DESIGNPROJECT__设计项目模板",
        ///      "AttrType": "Attr",
        ///      "Visible": "True"
        ///    },
        ///    {
        ///      "AttrCode": "AttrDataCount",   //判断是否显示目录附加属性页
        ///      "AttrName": "附加属性数量",
        ///      "AttrValue": "22",
        ///      "AttrType": "Attr",
        ///      "Visible": "False"
        ///    },
        ///    {
        ///      "AttrCode": "hasProjectEditRight",
        ///      "AttrName": "修改目录权限",
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
        public static JObject GetProjectBaseAttrByKeyword(string sid, string ProjectKeyWord)
        {
            ExReJObject reJo = new ExReJObject();
            try
            {
                ProjectKeyWord = ProjectKeyWord ?? "";
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
                if (dbsource == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }   
                    
                //获取目录对象
                Project project = dbsource.GetProjectByKeyWord(ProjectKeyWord);
                JArray jaResult = new JArray();

                //获取project修改属性权限
                bool hasProjectEditRight = GetProjectPCntrlRight(project, curUser);

                    //目录名称
                    jaResult.Add(new JObject 
                        { 
                            new JProperty("AttrCode", project.O_projectname.ToString()),
                            new JProperty("AttrName", "目录名"),
                            new JProperty("AttrValue",project.O_projectname),
                            new JProperty("AttrType","Attr"),
                            new JProperty("Visible","True")
                        });



                    //目录描述
                    jaResult.Add(new JObject 
                        { 
                            new JProperty("AttrCode", project.O_projectdesc.ToString()),
                            new JProperty("AttrName", "目录描述"),
                            new JProperty("AttrValue",project.O_projectdesc),
                            new JProperty("AttrType","Attr"),
                            new JProperty("Visible","True")
                        });



                    //版本号
                    jaResult.Add(new JObject 
                        { 
                            new JProperty("AttrCode", project.O_version.ToString()),
                            new JProperty("AttrName", "版本号"),
                            new JProperty("AttrValue",project.O_version),
                            new JProperty("AttrType","Attr"),
                            new JProperty("Visible","True")
                        });


                    //版本顺序
                    jaResult.Add(new JObject 
                        { 
                            new JProperty("AttrCode", project.O_version_seq.ToString()),
                            new JProperty("AttrName", "版本顺序"),
                            new JProperty("AttrValue",project.O_version_seq),
                            new JProperty("AttrType","Attr"),
                            new JProperty("Visible","True")
                        });


                    //创建时间
                    jaResult.Add(new JObject 
                        { 
                            new JProperty("AttrCode", project.O_credatetime.ToString()),
                            new JProperty("AttrName", "创建时间"),
                            new JProperty("AttrValue",project.O_credatetime == null ? "" : project.O_credatetime.ToString()),
                            new JProperty("AttrType","Attr"),
                            new JProperty("Visible","True")
                        });


                    //更新时间
                    jaResult.Add(new JObject 
                        { 
                            new JProperty("AttrCode", project.O_updatetime.ToString()),
                            new JProperty("AttrName", "更新时间"),
                            new JProperty("AttrValue",project.O_updatetime == null ? "" : project.O_updatetime.ToString()),
                            new JProperty("AttrType","Attr"),
                            new JProperty("Visible","True")
                        });


                    //目录状态
                    jaResult.Add(new JObject 
                        { 
                            new JProperty("AttrCode", project.O_status.ToString()),
                            new JProperty("AttrName", "目录状态"),
                            new JProperty("AttrValue",((enProjectStatus)project.O_status).ToString()),
                            new JProperty("AttrType","Attr"),
                            new JProperty("Visible","True")
                        });


                    //目录类型
                    jaResult.Add(new JObject 
                        { 
                            new JProperty("AttrCode", project.O_type.ToString()),
                            new JProperty("AttrName", "目录类型"),
                            new JProperty("AttrValue",((enProjectType)project.O_type).ToString()),
                            new JProperty("AttrType","Attr"),
                            new JProperty("Visible","True")
                        });


                    //创建者
                    jaResult.Add(new JObject 
                        { 
                            new JProperty("AttrCode", project.O_creatorno.ToString()),
                            new JProperty("AttrName", "创建者"),
                            new JProperty("AttrValue",project.O_creatorno <=0 ? "" : (project.Creater == null ? "" : project.Creater.ToString)),
                            new JProperty("AttrType","Attr"),
                            new JProperty("Visible","True")
                        });



                    //更新者
                    jaResult.Add(new JObject 
                        { 
                            new JProperty("AttrCode", project.O_updaterno.ToString()),
                            new JProperty("AttrName", "更新者"),
                            new JProperty("AttrValue",project.O_updaterno <= 0 ? "" : (project.Updater == null ? "" : project.Updater.ToString)),
                            new JProperty("AttrType","Attr"),
                            new JProperty("Visible","True")
                        });


                    //工作流
                    jaResult.Add(new JObject 
                        { 
                            new JProperty("AttrCode", project.O_WorkFlowno.ToString()),
                            new JProperty("AttrName", "工作流"),
                            new JProperty("AttrValue", project.O_WorkFlowno <= 0 ? "" : (project.WorkFlow == null ? "" : project.WorkFlow.ToString)),
                            new JProperty("AttrType","Attr"),
                            new JProperty("Visible","True")
                        });

                    //流程号
                    jaResult.Add(new JObject { 
                                new JProperty("AttrCode","O_WorkFlowno"),
                                new JProperty("AttrName","流程ID"),
                                new JProperty("AttrValue",project.O_WorkFlowno == null ? "" : project.O_WorkFlowno.ToString()),
                                new JProperty("AttrType","Attr"),
                                new JProperty("Visible","True"),
                            });

                    //存储空间
                    jaResult.Add(new JObject 
                        { 
                            new JProperty("AttrCode", project.O_shortno.ToString()),
                            new JProperty("AttrName", "存储空间"),
                            new JProperty("AttrValue", project.O_shortno <= 0 ? "" : (project.Storage == null ? "" : project.Storage.ToString)),
                            new JProperty("AttrType","Attr"),
                            new JProperty("Visible","True")
                        });


                    //模板
                    jaResult.Add(new JObject 
                        { 
                            new JProperty("AttrCode", project.O_DefnID.ToString()),
                            new JProperty("AttrName", "模板"),
                            new JProperty("AttrValue", project.O_DefnID <= 0 ? "" : (project.TempDefn == null ? "" : project.TempDefn.ToString)),
                            new JProperty("AttrType","Attr"),
                            new JProperty("Visible","True")
                        });

                    //附加属性数量
                    jaResult.Add(new JObject { 
                                new JProperty("AttrCode","AttrDataCount"),
                                new JProperty("AttrName","附加属性数量"),
                                new JProperty("AttrValue",project.AttrDataList.Count().ToString()),
                                new JProperty("AttrType","Attr"),
                                new JProperty("Visible","False"),
                            });


                    //修改目录权限
                    jaResult.Add(new JObject { 
                                new JProperty("AttrCode","hasProjectEditRight"),
                                new JProperty("AttrName","修改目录权限"),
                                new JProperty("AttrValue",hasProjectEditRight?"True":"False"),
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
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(reJo.msg);
            }


            return reJo.Value;
        }

        /// <summary>
        /// 返回一个Project对象的附加属性，在显示目录附加属性栏时使用
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="ProjectKeyWord">Project节点关键字</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时，data包含多个JObject， 每个JObject里面包含参数：</para>
        /// <para>附加属性包含的参数："AttrCode"：属性代码,"AttrName"：属性描述"AttrValue":属性值，"AttrType",属性类型("AddiAttr":附加属性),"Visible":是否显示("True")，"TempAttrType"：附加属性的类别(Common = 3, User = 4)，"DataType"：附加属性的数据类型类别("user":用户类型),"DefaultCode"：编辑属性的默认值，在下拉选择时用到，"ShowData"：下拉列表的子项</para>
        /// <para>例子：</para>
        /// <code>
        /// {  //未写完描述，记得补上
        ///  "success": true,
        ///  "total": 0,
        ///  "msg": "",
        ///  "data": [
        ///    {
        ///      "AttrCode": "DESIGNPHASE",
        ///      "TempAttrType": 1,
        ///      "DataType": 0,
        ///      "DefaultCode": "",
        ///      "ShowData": [
        ///        {
        ///          "text": "C__初设阶段",
        ///          "value": "C__初设阶段"
        ///        },
        ///        {
        ///          "text": "S__施工图阶段",
        ///          "value": "S__施工图阶段"
        ///        }
        ///      ],
        ///      "AttrName": "设计阶段模板",
        ///      "AttrValue": "A__初步可行性研究",
        ///      "AttrType": "AddiAttr",
        ///      "CanEdit": "True",
        ///      "Visible": "True"
        ///    },
        ///    {
        ///      "AttrCode": "PROJECTOWNER",
        ///      "TempAttrType": 4,
        ///      "DataType": 0,
        ///      "DefaultCode": "",
        ///      "ShowData": [],
        ///      "AttrName": "设总(项目负责人)",
        ///      "AttrValue": "",
        ///      "AttrType": "AddiAttr",
        ///      "CanEdit": "True",
        ///      "Visible": "True"
        ///    },
        ///    {
        ///      "AttrCode": "DOCUMENTMANAGER",
        ///      "TempAttrType": 4,
        ///      "DataType": 0,
        ///      "DefaultCode": "",
        ///      "ShowData": [],
        ///      "AttrName": "文控经理",
        ///      "AttrValue": "",
        ///      "AttrType": "AddiAttr",
        ///      "CanEdit": "True",
        ///      "Visible": "True"
        ///    }
        ///  ]
        /// }
        /// </code>
        /// </returns>
        public static JObject GetProjectAttrDataByKeyword(string sid, string ProjectKeyWord)
        {
            ExReJObject reJo = new ExReJObject();
            try
            {
                ProjectKeyWord = ProjectKeyWord ?? "";
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
                if (dbsource == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }
                
                //获取目录对象
                Project project = dbsource.GetProjectByKeyWord(ProjectKeyWord);
                JArray jaResult = new JArray();

                //获取修改目录权限
                //有修改目录权限才可以修改目录属性
                //以父目录的权限来限定当前目录属性的修改权 

                //获取project修改属性权限
                bool hasProjectEditRight = GetProjectPCntrlRight(project, curUser);
             
                //添加附加属性
                if (project.AttrDataList != null && project.AttrDataList.Count() > 0)
                {
                    jaResult = AttrDataController.GetAttrDataListJson(project.AttrDataList, hasProjectEditRight);
                }
                reJo.data = jaResult;
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
        /// 获取目录的用户和用户组权限列表
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="ProjectKeyword">目录Keyword</param>
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
        public static JObject GetProjectRightList(string sid, string ProjectKeyword) 
        {
            return AcceDataController.GetObjectRightList(sid,ProjectKeyword);
        }

        /// <summary>
        /// 设置目录权限
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="ProjectKeyword">目录Keyword</param>
        /// <param name="rightAttrJson">权限json字符串，例如：例如：[{ObjectKeyword: dataItem.ObjectKeyWord(用户或用户组的关键字),
        /// PFull: dataItem.PFull(值是"True"或"False"), PCreate: dataItem.PCreate, PRead: dataItem.PRead, PWrite: dataItem.PWrite, PDelete: dataItem.PDelete, PCntrl: dataItem.PCntrl, PNone: dataItem.PNone,
        /// DFull: dataItem.DFull, DCreate: dataItem.DCreate, DRead: dataItem.DRead, DWrite: dataItem.DWrite, DDelete: dataItem.DDelete, 
        /// DFRead: dataItem.DFRead, DFWrite: dataItem.DFWrite, DCntrl: dataItem.DCntrl, DNone: dataItem.DNone}]</param>
        /// <returns></returns>
        public static JObject SetProjectRightList(string sid, string ProjectKeyword,string rightAttrJson)
        {
            return AcceDataController.SetObjectRightList(sid, ProjectKeyword, rightAttrJson);
        }


        //判断当前用户是否有project修改属性权限
        internal static bool GetProjectPCntrlRight(Project project,User user)
        {

            bool hasProjectEditRight = false;

            if (user.IsAdmin)
            {
                return true;
            }

            Right right = null;

            if (project.acceData != null && project.acceData.Count>0)
                right = project.acceData.GetRight(user);
            else if (project.ParentAcceData != null && project.ParentAcceData.Count>0)
                //right = project.ParentAcceData.Right;
                right = project.ParentAcceData.GetRight(user);

            if (right!=null && right.PCntrl)
            { hasProjectEditRight = true; }
            return hasProjectEditRight;
        }

        //判断当前用户是否有project修改属性权限
         internal static bool GetProjectDCreateRight(Project project, string sid)
        {
            User curUser = DBSourceController.GetCurrentUser(sid);
            if (curUser != null)
                return GetProjectDCreateRight(project, curUser);
            else
                return false;
        }

        //判断当前用户是否有project修改属性权限
        internal static bool GetProjectDCreateRight(Project project, User user)
        {
            bool hasRight = false;
            Right right = null;
            if (project.acceData != null && project.acceData.Count > 0)
                right = project.acceData.GetRight(user);
            else if (project.ParentAcceData != null && project.ParentAcceData.Count > 0)
                right = project.ParentAcceData.GetRight(user);

            //有创建文档权限才可以创建目录
            if (right != null && right.DCreate)
            {
                hasRight = true;
            }
            return hasRight;
        }


        //判断当前用户是否有project修改文件权限（替换文件权限）
        internal static bool GetProjectDFWriteRight(Project project, User user)
        {
            bool hasRight = false;
            Right right = null;
            if (project.acceData != null && project.acceData.Count > 0)
                right = project.acceData.GetRight(user);
            else if (project.ParentAcceData != null && project.ParentAcceData.Count > 0)
                right = project.ParentAcceData.GetRight(user);

            //有创建文档权限才可以创建目录
            if (right != null && right.DFWrite)
            { 
                hasRight = true;
            }
            return hasRight;
        }

        //判断当前用户是否有project创建子文件夹权限
        public static bool GetProjectPCreateRight(Project project, string sid)
        {
            User curUser = DBSourceController.GetCurrentUser(sid);
            if (curUser != null)
                return GetProjectPCreateRight(project, curUser);
            else
                return false;
        }


        //判断当前用户是否有project创建子文件夹权限
        internal static bool GetProjectPCreateRight(Project project, User user)
        {
            bool hasRight = false;

            if (user == project.Creater )//|| user.O_usertype == enUserType.DefaultAdmin)
            {
                //如果的目录的创建者，就有创建子目录的权限
                return true;
            }

            Right right = null;
            if (project.acceData != null && project.acceData.Count > 0)
                right = project.acceData.GetRight(user);
            else if (project.ParentAcceData != null && project.ParentAcceData.Count > 0)
                right = project.ParentAcceData.GetRight(user);

            //有创建权限才可以创建目录
            if (right != null && right.PCreate)
            {
                hasRight = true;
            }
            return hasRight;
        }

        
        //判断当前用户是否有project写文件夹权限
        public static bool GetProjectPWriteRight(Project project, string sid)
        {
            User curUser = DBSourceController.GetCurrentUser(sid);
            if (curUser != null)
                return GetProjectPWriteRight(project, curUser);
            else
                return false;
        }

        //判断当前用户是否有project写子文件夹权限
        internal static bool GetProjectPWriteRight(Project project, User user)
        {
            bool hasRight = false;

            if (user == project.Creater)//|| user.O_usertype == enUserType.DefaultAdmin)
            {
                //如果的目录的创建者，就有复制子目录的权限
                return true;
            }

            Right right = null;
            if (project.acceData != null && project.acceData.Count > 0)
                right = project.acceData.GetRight(user);

            else if (project.ParentAcceData != null && project.ParentAcceData.Count > 0)
                right = project.ParentAcceData.GetRight(user);

            //有写目录权限才可以复制目录
            if (right != null && right.PWrite)
            {
                hasRight = true;
            }
            return hasRight;

        }

        //判断当前用户是否有project删除子文件夹权限
        public static bool GetProjectPDeleteRight(Project project, string sid)
        {
            User curUser = DBSourceController.GetCurrentUser(sid);
            if (curUser != null)
                return GetProjectPDeleteRight(project, curUser);
            else
                return false;
        }

        //判断当前用户是否有project删除子文件夹权限
        internal static bool GetProjectPDeleteRight(Project project, User user)
        {
            bool hasRight = false;

            if (user == project.Creater )//|| user.O_usertype == enUserType.DefaultAdmin)
            {
                //文档创建者获取删除文档权限
                if (project.DocList.Count<=0 && project.ChildProjectList.Count<=0 )
                { 
                    //如果的目录的创建者，并且目录里面没有文档,就有删除目录的权限
                    return true;

                }
            }

                Right right = null;
                if (project.acceData != null && project.acceData.Count > 0)
                    right = project.acceData.GetRight(user);
                //right = project.acceData.Right;
                else if (project.ParentAcceData != null && project.ParentAcceData.Count > 0)
                    right = project.ParentAcceData.GetRight(user);

                //有删除目录权限才可以删除目录
                if (right != null && right.PDelete)
                {
                    hasRight = true;
                }
                return hasRight;

        }

        internal static bool GetProjectDDeleteRight(Project project, User user)
        {
            bool hasRight = false;
            Right right = null;
            if (project.acceData != null && project.acceData.Count > 0)
                right = project.acceData.GetRight(user);
            else if (project.ParentAcceData != null && project.ParentAcceData.Count > 0)
                right = project.ParentAcceData.GetRight(user);

            //有删除目录权限才可以删除目录
            if (right != null && right.DDelete)
            {
                hasRight = true;
            }
            return hasRight;
        }

        /// <summary>
        /// 退出时保存最后访问的目录
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="ProjectKeyword">Project关键字</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时，data包含一个空的JObject</para>
        /// <para>例子：</para>
        /// </returns>

        public static JObject SaveLastProject(string sid,string ProjectKeyword)
        {

            ExReJObject reJo = new ExReJObject();
            try
            {
                ProjectKeyword = ProjectKeyword ?? "";
                if (string.IsNullOrEmpty(ProjectKeyword))
                {
                    reJo.msg = "参数错误，文件夹不存在！";
                    return reJo.Value;
                }
                else
                {

                    //登录用户
                    User curUser = DBSourceController.GetCurrentUser(sid);
                    if (curUser == null)
                    {
                        reJo.msg = "登录验证失败！请尝试重新登录！";
                        return reJo.Value;
                    }


                    //保存最后点击目录
                    curUser.SetUserConfig(enUserConfig.LastProject, ProjectKeyword);
                    curUser.Modify();

                    reJo.success = true;
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
        /// 保留一个空函数，修改js端tree后更新用
        /// </summary>
        /// <returns>
        /// <para>返回一个空的JObject</para>
        /// <para>例子：</para>
        /// </returns>
        public static JObject SaveProject()
        { 
            return new JObject();
        }

        /// <summary>
        /// 获取Project的完整目录路径
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="Keyword">Project、Doc或者WorkFlow的关键字，当为"LastProject"时，获取当前用户最后访问目录的路径</param>
        /// <param name="IgnoreShortCut">是否能跳转到快捷方式，而不是快捷方式的实体文档或目录,值是"true"或"false"</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时，data包含一个JObject，里面包含参数"ProjectPath"：节点路径（格式："/Root/节点路径"）,"NodeId":Project的Keyword</para>
        /// <para>例子：</para>
        /// </returns>
        [HttpPost]
        public static JObject GetProjectPath(string sid,string Keyword,string IgnoreShortCut)
        {
            ExReJObject reJo = new ExReJObject();
            try
            {
                if (string.IsNullOrEmpty(Keyword))
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
                bool bIgnoreShortCut = false;
                if (IgnoreShortCut == "true") bIgnoreShortCut = true;
                Project project = null;
                if (Keyword == "LastProject")
                {

                    //获取用户上次退出的目录
                    if (curUser.UserCFGList.Contains(enUserConfig.LastProject.ToString()))
                    {
                        UserCfg cfg = (UserCfg)curUser.UserCFGList[enUserConfig.LastProject.ToString()];
                        if (cfg != null)
                        {
                            Keyword = cfg.SValue1;
                        }
                    }
                }

                DBSource dbsource = curUser.dBSource;

                string DocKeyword = "";

                //获取目录对象
                object obj = dbsource.GetObjectByKeyWord(Keyword);
                if (obj == null)
                {
                    //return null; //没有返回值
                    reJo.msg = "参数错误，对象不存在！";
                    return reJo.Value;
                }
                if (obj is Doc)
                {
                    Doc ddoc = (Doc)obj;
                    Doc doc = bIgnoreShortCut ? ddoc : (ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc);
                    project = doc.Project;
                    DocKeyword = doc.KeyWord;
                }
                else if (obj is WorkFlow)
                {
                    WorkFlow workFlow = (WorkFlow)obj;
                    project = (workFlow.Project!=null)? workFlow.Project:((workFlow.doc!=null)? workFlow.doc.Project:null);
                }
                else if (obj is Project)
                {
                    project = (Project)obj;
                }



                //将project 及 ParentProject 对象的Keyword返回
                string projpath = "";//文档路径
                string nodeId = "";// project.KeyWord;
                string ProjectKeyword = project.KeyWord;
                string temppath = "";
                List<Project> projList = new List<Project>() { project };

                //先获取所有父project
                while (project != null)
                {
                    if (project.ParentProject != null)
                    {
                        
                        Project parentProj = project.ParentProject;
                        project = parentProj;
                        projList.Add(project);
                    }
                    else
                        project = null;
                }

                //再组装路径
                for (int i = projList.Count - 1; i >= 0; i--)
                {
                    Project proj = projList[i];
                    if (temppath == "") {
                        temppath =  proj.KeyWord ;

                        projpath =  temppath;
                    }
                    else
                    {
                        temppath =  temppath + "_" + proj.KeyWord;

                        projpath = projpath + "/" + temppath;
                    }
                }

                nodeId = temppath;

                //while (project != null)
                //{
                //    temppath = temppath == "" ? project.KeyWord : project.KeyWord + "_" + temppath;
                //    projpath = temppath + "/" + projpath;
                //    //projpath = project.KeyWord + "/" + projpath;
                //    //projpath = project.KeyWord + "/" + temppath;// projpath;
                //    if (project.ParentProject != null)
                //    {
                //        Project parentProj = project.ParentProject;
                //        project = parentProj;
                //    }
                //    else
                //        project = null;
                //}

                if (projpath != "") projpath = "/" + "Root" + "/" + projpath;

                reJo.data = new JArray(new JObject(
                    new JProperty("ProjectPath", projpath),
                    new JProperty("NodeId", nodeId),
                    new JProperty("ProjectKeyword", ProjectKeyword),
                    new JProperty("DocKeyword", DocKeyword)
                    ));

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
        /// 获取Project地址栏的地址路径
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="ProjectKeyword">目录关键字</param>
        /// <returns></returns>
        /// 
        public static JObject GetProjectShowPath(string sid, string ProjectKeyword)
        {
            ExReJObject reJo = new ExReJObject();
            try
            {
                //登录用户
                User curUser = DBSourceController.GetCurrentUser(sid);
                if (curUser == null || curUser.dBSource == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

                DBSource dbsource = curUser.dBSource;

                Project project = dbsource.GetProjectByKeyWord(ProjectKeyword);

                if (project == null)
                {
                    reJo.msg = "参数错误！文件夹不存在！";
                    return reJo.Value;
                }

                string path=GlobalProject.GetProjectShowPath(project,false,false);

                reJo.data = new JArray(new JObject (new JProperty("path",path)));
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
        /// 获取地址栏的对象
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="path">地址栏获取到的路径</param>
        /// <returns></returns>
        public static JObject GetShowPathObject(string sid, string path) {
            ExReJObject reJo = new ExReJObject();
            try
            {
                //登录用户
                User curUser = DBSourceController.GetCurrentUser(sid);
                if (curUser == null || curUser.dBSource == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

                DBSource dbsource = curUser.dBSource;

               object obj = GlobalProject.GetObjectByShowPath(dbsource,path);

                if (obj == null) {
                    reJo.msg = "您输入的CDMS目录地址格式不正确。支持的格式为：\n数据源服务器机器名:\\\\数据源代码\\项目\\目录1\\目录2\\……";
                    return reJo.Value;
                }

                JObject joData = new JObject();
                if (obj is Project)
                {
                    Project proj=(Project)obj;
                    
                    reJo.data = new JArray(new JObject(
                        new JProperty("objectType", "Project"),
                        new JProperty("objectKeyword", proj.KeyWord)
                        ));

                    reJo.success = true;
                }
                else if (obj is Doc)
                {
                    Doc doc = (Doc)obj;

                    reJo.data = new JArray(new JObject(
                        new JProperty("objectType", "Doc"),
                        new JProperty("objectKeyword", doc.KeyWord)
                        ));

                    reJo.success = true;
                }
                else {
                    reJo.msg = "您输入的CDMS目录地址格式不正确。当前地址格式对象不支持！";
                    return reJo.Value;
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
        /// 获取按钮权限
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="ProjectKeyword">目录关键字</param>
        /// <param name="Menu">按钮菜单名称，"CreateNewProject"：创建目录权限,"CreateNewRootProject":创建根目录权限，"CreateNewDoc"：创建文档权限，"ModiProjAttr":修改目录属性权限</param>
        /// <returns></returns>
        public static JObject GetMenuRight(string sid, string ProjectKeyword, string Menu)
        {
            ExReJObject reJo = new ExReJObject();
            try
            {

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
                if (dbsource == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

                Project project = dbsource.GetProjectByKeyWord(ProjectKeyword);

                //判断是否有创建目录权限
                if (Menu == "CreateNewProject")
                {
                    bool hasRight = GetProjectPCreateRight(project,curUser);

                    //有创建权限才可以创建目录
                    if (hasRight)
                    {
                        reJo.success = true;
                        return reJo.Value;
                    }
                    else
                    {
                        reJo.msg = "创建目录失败,您不拥有在目标文件夹下创建子目录的权限!";
                        return reJo.Value;
                    }
                }
                else if (Menu == "CreateNewRootProject") {
                    //bool hasRight = GetProjectPCreateRight(project, curUser);

                    //有创建权限才可以创建目录
                    if (curUser.IsAdmin)
                    {
                        reJo.success = true;
                        return reJo.Value;
                    }
                    else
                    {
                        reJo.msg = "创建目录失败,您不拥有在目标文件夹下创建子目录的权限!";
                        return reJo.Value;
                    }
                }

                //判断是否有创建文档权限
                else if (Menu == "CreateNewDoc")
                {
                    bool hasRight = ProjectController.GetProjectDCreateRight(project, curUser);

                    //有创建文档权限才可以创建目录
                    if (hasRight)
                    {
                        reJo.success = true;
                        return reJo.Value;
                    }
                    else
                    {
                        reJo.msg = "创建文档失败,您不拥有在目标文件夹下创建文档的权限!";
                        return reJo.Value;
                    }
                }
                else if (Menu == "ModiProjAttr")
                {

                    bool hasRight = ProjectController.GetProjectPCntrlRight(project, curUser);

                    //有创建文档权限才可以创建目录
                    if (hasRight)
                    {
                        reJo.success = true;
                        return reJo.Value;
                    }
                    else
                    {
                        reJo.msg = "修改目标文件夹属性失败：用户没有修改目标文件夹属性权限！";
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
        /// 返回一个Project对象下所有子project的文档合计数量
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="ProjectList">Project关键字列表，用","分割</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时，data包含一个JObject，里面包含多个参数，参数名为Project关键字，值为该Project里文档的数量</para>
        /// <para>例子：</para>
        /// </returns>
        public static JObject GetChildsDocsCount(string sid,string ProjectList)
        {
            ExReJObject reJo = new ExReJObject();            
            try
            {

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
                if (dbsource == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

                //JObject joChild = GetProjectList(ProjectKeyWord,ProjectType);
                string[] strArray = (string.IsNullOrEmpty(ProjectList) ? "" : ProjectList).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                Project project;
                JObject reJoData= new JObject();
                
                int docTotal;
                foreach (string strProj in strArray)
                {
                    project = dbsource.GetProjectByKeyWord(strProj);
                    if (project != null)
                    {
                        docTotal = project.DocList.Count;// DocCount;
                        reJoData.Add(new JProperty(strProj, docTotal));
                    }
                }
                reJo.data = new JArray(reJoData);
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

       
             
        /// <summary>
        /// 获取各级子Project
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="node">Project节点关键字</param>
        /// <param name="ProjectType">Project类型，1：项目目录，4，：查询，5：个人工作台，7：逻辑目录</param>
        /// <param name="Level">获取目录的深度，1级："1",2级："2",3级:"3",4级:"4"</param>
        /// <param name="Peer">是否获取同级目录，值是"false"或"true"</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时，data包含多个JObject，每个JObject表示一个一级目录，里面包含参数"id":节点关键字,"text":节点文本,"leaf":是否有子节点,
        /// "children"：循环嵌套下一级目录，里面包含多个JObject每个JObject表示一个下级目录，参数与一级目录相同</para>
        /// <para>例子：</para>
        /// </returns>
        public static JObject getProjectListWithLevel(string sid, string node, string ProjectType, string Level,string Peer)
        {
            //JObject joMsg = new JObject();
            ExReJObject reJo = new ExReJObject();
            try
            {
                
                JArray jaResult = new JArray();

                         

                //登录用户
                User curUser = DBSourceController.GetCurrentUser(sid);
                if (curUser == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

                JObject reObj = new JObject();

                JObject reJoData = new JObject();
                string level = Level ?? "1";

                if (Peer == "false")
                {
                    JObject joMsg = new JObject();
                    //获取第一级子目录
                    joMsg = AVEVA.CDMS.WebApi.ProjectController.GetProjectListJson(sid, node, ProjectType);

                    if (level != "1")
                    {
                        JToken jtMsg = joMsg;
                        JArray jaChild = new JArray();
                        foreach (JProperty jpMsg in jtMsg)
                        {
                            if (jpMsg.Name == "data" && !string.IsNullOrEmpty(jpMsg.Value.ToString()) && jpMsg.Value.ToString() != "null")
                            {
                                JArray ja5 = (JArray)(jpMsg.Value);
                                foreach (JObject joOneLevel in ja5)
                                {
                                    //遍历添加第二级子目录
                                    string one_level_node_id = (string)joOneLevel["id"];

                                    LoopGetChildProject(joOneLevel, sid, one_level_node_id, ProjectType, Level);
                                }
                            }
                        }
                    }
                    reJo.Value = joMsg;
                }
        
                reJo.success = true;
            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(e.Message);
            }
            return reJo.Value;
        }


        //循环获取各级子目录
        private static void LoopGetChildProject(JObject joOneLevel, string sid, string nodeid, string ProjectType, string level)
        {
           
            JObject joMsgChild = AVEVA.CDMS.WebApi.ProjectController.GetProjectListJson(sid, nodeid, ProjectType);
            JToken jtMsgChild = joMsgChild;
    
            foreach (JProperty jpMsgChild in jtMsgChild)
            {
                if (jpMsgChild.Name == "data" && !string.IsNullOrEmpty(jpMsgChild.Value.ToString()) && jpMsgChild.Value.ToString() != "null")
                {
                    //if (level == "0")//如果level等于零，则无限循环获取子目录
                    if (Convert.ToInt32(level) > 3)//最大四级
                    {
                        JArray ja5 = (JArray)(jpMsgChild.Value);
                        foreach (JObject joTwoLevel in ja5)
                        {
                            //遍历添加第二级子目录
                            string two_level_node_id = (string)joTwoLevel["id"];
                            LoopGetChildProject(joTwoLevel, sid, two_level_node_id, ProjectType, (Convert.ToInt32(level) - 1).ToString());
                        }
                    }

                    joOneLevel.Add(new JProperty("children", jpMsgChild.Value));
                }
            }
        }


        /// <summary>
        /// 模板创建各级子Project
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="Projects">包含多个Project节点的JArray字符串，每个JObject包含有:"id","text","parentId"，例如：[{"id": nd.data.id,"text": nd.data.text,"parentId": nd.data.parentId}]</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时，data包含一个空的JObject</para>
        /// <para>例子：</para>
        /// </returns>
        public static JObject CreateProjectByDef(string sid, string Projects)
        {
            ExReJObject reJo = new ExReJObject();
            try
            {
                JArray ja = (JArray)JsonConvert.DeserializeObject(Projects);

                string sourceProjId = "", childProjId;
                foreach (JObject joProject in ja)
                {
                    //遍历添加第二级子目录
                    string projId = (string)joProject["id"];
                    string parentId = (string)joProject["parentId"];
                    if (parentId == "/" || parentId == "Root")
                    {
                        sourceProjId = projId;
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(sourceProjId) && !string.IsNullOrEmpty(sid))
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

                    if (dbsource != null)//登录并获取dbsource成功
                    {
                        Project sourceProj = dbsource.GetProjectByKeyWord(sourceProjId);
                        if (sourceProj != null)
                        {
                            bool hasRight = GetProjectPCreateRight(sourceProj,curUser);

                            //增加创建权限控制
                            if (!hasRight)
                            {
                                reJo.msg = "创建目录失败,您不拥有在目标文件夹下创建子目录的权限!";
                                return reJo.Value;
                            }

                            List<ProjectIndex> projectIndexList = new List<ProjectIndex>();
                            foreach (JObject joProject in ja)
                            {
                                //遍历添加第二级子目录
                                string projId = (string)joProject["id"];
                                string parentId = (string)joProject["parentId"];
                                if (parentId != "/" && parentId != "Root")
                                {
                                    childProjId = projId;
                                    Project childProj = dbsource.GetProjectByKeyWord(childProjId);
                                    if (childProj != null)
                                    {
                                        //获取要创建目录的父目录
                                        Project parentProj = new Project();
                                        if (parentId == sourceProjId)
                                        {
                                            parentProj = sourceProj;
                                        }
                                        else
                                        {
                                            //如果是刚才新建的目录，就获取新建目录作为父目录
                                            bool flag = true;
                                            foreach (ProjectIndex pi in projectIndexList)
                                            {
                                                if (pi.oldProjectId == parentId)
                                                { parentProj = pi.newProject; flag = false; break; }
                                            }
                                            if (flag) { continue; }
                                        }
                                        try
                                        {
                                            
                                            //创建目录
                                            Project projectByName = parentProj.NewProject(childProj.O_projectname, childProj.O_projectdesc, childProj.Storage, childProj.TempDefn);
                                           
                                            //记录新建的目录信息，为建立再下一级目录做准备
                                            ProjectIndex projectIndex = new ProjectIndex();
                                            projectIndex.oldProjectId = childProjId;
                                            projectIndex.newProject = projectByName;
                                            projectIndexList.Add(projectIndex);

                                        }
                                        catch (Exception e)
                                        {
                                            CommonController.WebWriteLog(e.Message);
                                        }
                                    }
                                }
                            }
                            reJo.success = true;
                            return reJo.Value;
                        }
                    }
                }
            }
            catch (Exception ex){
                reJo.msg=ex.Message;
                CommonController.WebWriteLog(ex.Message);
            }
            return reJo.Value;
        }

        public class ProjectIndex
        {
            private string _oldProjectId;
            private Project _newProject;
            public string oldProjectId
            {
                get
                {
                    return _oldProjectId;
                }
                set
                {
                    _oldProjectId = value;
                }
            }

            public Project newProject
            {
                get
                {
                    return _newProject;
                }
                set
                {
                    _newProject = value;
                }
            }
        }


        /// <summary>
        /// 在目录下粘贴一个目录或文档
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="ProjectKeyword">粘贴到目录的关键字</param>
        /// <param name="ObjectKeyword">被粘贴的目录或文档关键字</param>
        /// <param name="isCut">是否剪切，值是"true"或"false"</param>
        /// <returns></returns>
        public static JObject ProjectPaste(string sid, string ProjectKeyword, string ObjectKeyword, string isCut)
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

                object obj = dbsource.GetObjectByKeyWord(ObjectKeyword);
                if (obj is Doc)
                {
                    return PasteDoc(sid, ProjectKeyword, ObjectKeyword, isCut);
                }
                else if (obj is Project){ 
                    //IDIGNORE:如果已存在相同名字的目录，就跳过
                    //isCopyFile="false":不复制目录下的文件
                    //isCopySubFolder="true":复制子目录
                    //isCopyTempDefn="true":复制目录模板
                    //isCopyProperty="true":复制目录属性
                    return PasteProject(sid, ProjectKeyword, ObjectKeyword, isCut, "IDIGNORE", "", "",
                        "false", "true", "true", "true");
                }


                return reJo.Value;
            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(e.Message);
            }

            return reJo.Value;
        }

        /// <summary>
        /// 把目录对象粘贴到目录
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="ProjectKeyword">粘贴到的目录对象关键字</param>
        /// <param name="PasteProjectKeyword">被粘贴的目录对象关键字</param>
        /// <param name="isCut">是否剪切，值是"true"或"false"</param>
        /// <param name="iRet">当目录中有相同名称对象的时候，执行的操作,传递的值："IDIGNORE":跳过,"IDOK":新起名字,"IDABORT":覆盖</param>
        /// <param name="newCopyCode">当目录中有相同名称对象的时候，新起了名字,且名字不会有重名</param>
        /// <param name="newCopyDesc">当目录中有相同名称对象的时候，新起了名字描述,且名字不会有重名</param>
        /// <param name="isCopyFile">是否复制文件，值是"true"或"false"</param>
        /// <param name="isCopySubFolder">是否复制子目录，值是"true"或"false"</param>
        /// <param name="isCopyTempDefn">是否复制模板，值是"true"或"false"</param>
        /// <param name="isCopyProperty">是否复制属性，值是"true"或"false"</param>
        /// <returns></returns>
        public static JObject PasteProject(string sid, string ProjectKeyword, string PasteProjectKeyword,
            string isCut,string iRet, string newCopyCode,string newCopyDesc,
           string isCopyFile, string isCopySubFolder,string isCopyTempDefn,string isCopyProperty)
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

                bool IsCopyFile = isCopyFile.ToLower() == "true" ? true : false;

                bool IsCopySubFolder= isCopySubFolder.ToLower() == "true" ? true : false;

                bool IsCopyTempDefn= isCopyTempDefn.ToLower() == "true" ? true : false;

                bool IsCopyProperty= isCopyProperty.ToLower() == "true" ? true : false;

                //删除当前节点对应的Project对象
                Project prj = dbsource.GetProjectByKeyWord(ProjectKeyword);

                if (prj == null) {
                    reJo.msg = "无法复制:目标文件夹不存在！";
                    return reJo.Value;
                }

                //判断是否有目标文件夹下的创建权限
                bool hasRight = GetProjectPCreateRight(prj, curUser);

                if (!hasRight) {
                    reJo.msg = "无法复制: 您不拥有在目标文件夹下创建子目录的权限!" ;
                    return reJo.Value;
                }

                //获取目录对象
                object obj = dbsource.GetObjectByKeyWord(PasteProjectKeyword);
                if (obj == null)
                {
                    //return null; //没有返回值
                    reJo.msg = "参数错误，对象不存在！";
                    return reJo.Value;
                }

                Doc doc = null;
                Project pastePrj = null;

                if (obj is Doc)
                {
                    Doc ddoc = (Doc)obj;
                    doc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;
                    //project = doc.Project;
                    //DocKeyword = doc.KeyWord;
                }
                else if (obj is Project)
                {
                    pastePrj = (Project)obj;
                }
                else
                {
                    reJo.msg = "您输入的CDMS目录地址格式不正确。当前地址格式对象不支持！";
                    return reJo.Value;
                }

                //复制目录
                if (pastePrj != null)
                {
                    if (pastePrj == prj)
                    {
                        //AfxMessageBox(L"无法复制: " + (CString)pastePrj.O_projectname + L" 目标文件夹和源文件夹相同");
                        //continue;
                        reJo.msg = "无法复制:"+ pastePrj.O_projectname + " 目标文件夹和源文件夹相同";
                        return reJo.Value;
                    }

                    if (Global_ProjectFuns.IsProjectUnderAnother(pastePrj, prj))
                    {
                        //AfxMessageBox(L"无法复制: " + (CString)pastePrj.O_projectname + L" 目标文件夹是源文件夹的子文件夹");
                        //continue;
                        reJo.msg = "无法复制:" + pastePrj.O_projectname + " 目标文件夹是源文件夹的子文件夹";
                        return reJo.Value;
                    }

                    string copyPrjName = pastePrj.O_projectname;//::CreateNewProjectName(prj , pastePrj.O_projectname) ; 
                    string copyPrjDesc = pastePrj.O_projectdesc;

                    //2008-11-24
                    //HTREEITEM hNewCopyItem = this.m_hCurItem;

                    //2009-06-26如果存在同名目录,则提示用户是否覆盖
                    bool bSameName = false;
                    Project  sameProject;

                    if (prj.ChildProjectList!=null && prj.ChildProjectList.Count > 0)
                    {
                        foreach(Project  subProject in prj.ChildProjectList)

                            {
                            if (subProject.O_projectname.ToLower() == pastePrj.O_projectname.ToLower())
                            {
                                bSameName = true;
                                sameProject = subProject;
                                break;
                            }
                        }
                    }

                    if (bSameName)
                    {
                        string title;
                        //title.Format(L"此目录下已包含名为 \"%s\" 的子目录。\r\n\r\n如果现有目录中的文件名与正移动或复制的目录中的文件名\r\n相同,这些文件将被替换。请选择相应的操作!", (CString)pastePrj.O_projectname);
                        if (string.IsNullOrEmpty(iRet)) {
                            reJo.msg = "selectiRet";
                            return reJo.Value;
                        }
                        /*if(AfxMessageBox(title ,MB_YESNO) == IDNO)
                            continue; */

                        //string newCopyCode = "";
                        //string newCopyDesc = "";

                        //UINT iRet = ::ShowNormalRenameDlg(L"", title, sameProject, newCopyCode, newCopyDesc);

                        //根据用户设定进行处理,或跳过,或覆盖,或用了新的名字
                        if (iRet == "IDIGNORE")
                        {
                            //跳过
                            //continue;
                            return reJo.Value;
                        }

                        else if (iRet == "IDOK")
                        {
                            //新起了名字,且名字不会有重名
                            copyPrjName = newCopyCode;
                            copyPrjDesc = newCopyDesc;

                        }

                        else if (iRet == "IDABORT")
                        {
                            //覆盖
                            copyPrjName = pastePrj.O_projectname;

                        }
                        else
                        {
                            //continue;
                            return reJo.Value;
                        }



                    }


                    Project  copyPrj;
                    //if (optionDlg.m_bIsCopyTempDefn && pastePrj.TempDefn)
                    if (pastePrj.TempDefn!=null)
                    {
                        copyPrj = prj.NewProject((string.IsNullOrEmpty(copyPrjName) ? pastePrj.O_projectname : copyPrjName),
                            copyPrjDesc, prj.Storage, pastePrj.TempDefn, false);
                    }
                    else
                        copyPrj = prj.NewProject((string.IsNullOrEmpty(copyPrjName) ? pastePrj.O_projectname : copyPrjName),
                        copyPrjDesc, null, null, false);

                    if (copyPrj!=null)
                    {
                        //if (Global_ProjectFuns.CopyProject(pastePrj, copyPrj, optionDlg.m_bIsCopyFile, optionDlg.m_bIsCopySubFolder, optionDlg.m_bIsCopyTempDefn, optionDlg.m_bIsCopyProperty))
                        if (Global_ProjectFuns.CopyProject(pastePrj, copyPrj, IsCopyFile, IsCopySubFolder, IsCopyTempDefn, IsCopyProperty))
                        {
                            //::TreeView_InsertProjectOnItem( hNewCopyItem,copyPrj  ,this) ; 
                            //处理显示，在TreeView中加入子目录，同时在ListView中也要加入

                        }
                        else
                        {
                            //把新创建的copyPrj删除，并反馈用户，该目录粘贴没能成功完成。
                            //AfxMessageBox((CString)("复制过程出现错误,部分对象复制不成功!"));


                        }

                        //if (Global::ydMainFrame)
                        //{
                        //    Global::ydMainFrame.m_wndStatusBar.SetPaneText(0, (CString)("就绪"));

                        //}
                        reJo.success = true;
                    }
                }

                return reJo.Value;
            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(e.Message);
            }

            return reJo.Value;
        }

        /// <summary>
        /// 粘贴文档到目录
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="ProjectKeyword">粘贴到的目录对象关键字</param>
        /// <param name="PasteDocKeyword">被粘贴的文档对象关键字</param>
        /// <param name="isCut">是否剪切，值是"true"或"false"</param>
        /// <returns></returns>
        public static JObject PasteDoc(string sid, string ProjectKeyword, string PasteDocKeyword,string isCut)
            // string iRet, string newCopyCode, string newCopyDesc,
   //string isCopyFile, string isCopySubFolder, string isCopyTempDefn, string isCopyProperty)
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

                //bool IsCopyFile = isCopyFile.ToLower() == "true" ? true : false;

                //bool IsCopySubFolder = isCopySubFolder.ToLower() == "true" ? true : false;

                //bool IsCopyTempDefn = isCopyTempDefn.ToLower() == "true" ? true : false;

                //bool IsCopyProperty = isCopyProperty.ToLower() == "true" ? true : false;

                bool IsCut = isCut.ToLower() == "true" ? true : false;

                //explorer版本里bDocument用于判断是否是在根目录创建文档
                bool bDocument = false;

                //删除当前节点对应的Project对象
                Project prj = dbsource.GetProjectByKeyWord(ProjectKeyword);

                if (prj == null)
                {
                    reJo.msg = "无法复制:目标文件夹不存在！";
                    return reJo.Value;
                }

                //判断是否有目标文件夹下的创建权限
                bool hasRight = GetProjectPCreateRight(prj, curUser);

                if (!hasRight)
                {
                    reJo.msg = "无法复制: 您不拥有在目标文件夹下创建子目录的权限!";
                    return reJo.Value;
                }

                //获取目录对象
                object obj = dbsource.GetObjectByKeyWord(PasteDocKeyword);
                if (obj == null)
                {
                    //return null; //没有返回值
                    reJo.msg = "参数错误，对象不存在！";
                    return reJo.Value;
                }

                Doc pasteDoc = null;
                Project pastePrj = null;

                if (obj is Doc)
                {
                    Doc ddoc = (Doc)obj;
                    pasteDoc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;
                    //project = doc.Project;
                    //DocKeyword = doc.KeyWord;
                }
                else if (obj is Project)
                {
                    pastePrj = (Project)obj;
                }
                else
                {
                    reJo.msg = "您输入的CDMS目录地址格式不正确。当前地址格式对象不支持！";
                    return reJo.Value;
                }

                if (pasteDoc!=null)
                { 
                //小黎 增加剪切功能 2011-5-5
                    if (IsCut)
                    {
                        bool IsShortCut = false;
                        //剪切实现  2011-5-5
                        if (bDocument)
                            return reJo.Value;

                        //Doc pasteDoc = (Doc)ob;
                        //if (!pasteDoc)
                        //    continue;
                        //if (prj.acceData && prj.acceData.Right && prj.acceData.Right.DCreate == false)
                        //{
                        //    AfxMessageBox(L"无法复制文档: " + (CString)pasteDoc.O_itemname + L" ,您不拥有在目标目录下创建文件的权限!");
                        //    bDCreate = false;
                        //    continue;
                        //}

                        //HTREEITEM hPre;
                        //if (this.m_hPreItem == NULL || this.m_hPreItem == this.m_hCurItem)
                        //{
                        //    return;
                        //}
                        //else
                        //{
                        //    hPre = this.m_hPreItem;
                        //}

                        Project proj;
                        Doc DelDoc = null;
                        //if (hPre)
                        //{
                        //proj = GetProjectFromHTREEITEM(hPre);

                        //foreach(Doc doc in proj.DocList)

                        //        {
                        //    if (doc.ShortCutDoc!=null && doc.ShortCutDoc == pasteDoc)
                        //    {
                        //        DelDoc = doc;
                        //        IsShortCut = true;
                        //        break;
                        //    }
                        //}
                        //}

                        proj = pasteDoc.Project;
                        //判断是剪切了快捷方式
                        if (pasteDoc.ShortCutDoc != null && pasteDoc.ShortCutDoc == pasteDoc)
                        {
                            DelDoc = pasteDoc;
                            IsShortCut = true;
                        }

                        if (IsShortCut)
                        {
                            //如果是剪切了快捷方式
                            Doc newShortDoc = prj.NewDoc(pasteDoc);
                            DelDoc.Delete();
                            proj.DocList.Remove(DelDoc);
                            //if (Global::listHashTable.Contains(DelDoc.O_itemno))
                            //{
                            //    Global::listHashTable.Remove(DelDoc.O_itemno);
                            //}


                        }
                        else
                        {
                            //如果是剪切了实体文档
                            string OldFilePath;
                            string OldFileDir;
                            OldFileDir = pasteDoc.Project.FullPath;
                            OldFilePath = pasteDoc.FullPathFile;

                            pasteDoc.O_projectno = prj.O_projectno;
                            pasteDoc.Modify();

                            string NewFilePath;
                            string NewFileDir;
                            NewFileDir = prj.FullPath;
                            NewFilePath = prj.FullPath + pasteDoc.O_filename;
                            if (string.IsNullOrEmpty(OldFilePath) || string.IsNullOrEmpty(NewFilePath))
                            {
                                return reJo.Value;
                            }

                            if (System.IO.File.Exists(OldFilePath))
                            {
                                if (!System.IO.File.Exists(NewFilePath))
                                {
                                    //创建目录
                                    try
                                    {
                                        string FullFileName = NewFilePath;
                                        //取得要保存文件的路径
                                        String FileDir = FullFileName.Substring(0, FullFileName.LastIndexOf("\\"));

                                        //查看路径是否存在,创建路径
                                        if (!System.IO.Directory.Exists(FileDir)) System.IO.Directory.CreateDirectory(FileDir);
                                    }
                                    catch { }

                                    System.IO.File.Move(OldFilePath, NewFilePath);
                                }
                            }

                            ////获取Doc相应的FTP对象  2011-5-5
                            //FTPFactory ^ Ftp;
                            //Ftp = gcnew FTPFactory(pasteDoc.Storage);
                            //if (Ftp)
                            //{
                            //    if (Ftp.CheckFileIsExit(OldFilePath))
                            //    {
                            //        Ftp.RemoteCopyFile(OldFilePath, NewFilePath);
                            //    }
                            //    //移动之后进行删除操作 2011-5-5
                            //    if (Ftp.CheckFileIsExit(NewFilePath))
                            //    {
                            //        Ftp.deleteRemoteFile(OldFilePath);
                            //    }
                            //}
                            //else
                            //{
                            //    return;
                            //}

                            //如果存在附加文件,则也需把附加文件进行复制 2011-5-5
                            if (pasteDoc.AttachList != null && pasteDoc.AttachFileList.Count > 0)
                            {
                                List<string> FileList = new List<string>();
                                foreach (string sAttachFile in pasteDoc.AttachFileList)
                                {
                                    //直接在服务器上面移动
                                    string OldAttachFilePath = OldFileDir + "AttachFiles\\" + sAttachFile;
                                    string NewAttachFilePath = NewFileDir + "AttachFiles\\" + sAttachFile;
                                    FileList.Add(NewAttachFilePath);
                                    //if (Ftp && Ftp.CheckFileIsExit(OldAttachFilePath))
                                    //{
                                    //    Ftp.RemoteCopyFile(OldAttachFilePath, NewAttachFilePath);
                                    //}
                                    ////移动之后进行删除操作  2011-5-5
                                    //if (Ftp.CheckFileIsExit(NewAttachFilePath))
                                    //{
                                    //    Ftp.deleteRemoteFile(OldAttachFilePath);
                                    //}
                                }
                                if (FileList.Count > 0)
                                {
                                    foreach (string str in FileList)
                                    {
                                        pasteDoc.AddAttachFile(str);
                                    }
                                }
                            }

                            //存在附加文件2   2011-5-5
                            if (pasteDoc.AttachList != null && pasteDoc.AttachList.Count > 0)
                            {
                                List<AVEVA.CDMS.Server.Attach> AttachList = new List<AVEVA.CDMS.Server.Attach>();
                                foreach (AVEVA.CDMS.Server.Attach attach in pasteDoc.AttachList)
                                {
                                    string OldAttachPath = OldFileDir + attach.O_content;
                                    string NewAttachPath = NewFileDir + attach.O_content;
                                    AttachList.Add(attach);
                                    //if (Ftp && Ftp.CheckFileIsExit(OldAttachPath))
                                    //{
                                    //    Ftp.RemoteCopyFile(OldAttachPath, NewAttachPath);
                                    //}
                                    ////移动之后进行删除操作   2011-5-5
                                    //if (Ftp.CheckFileIsExit(NewAttachPath) && Ftp.CheckFileIsExit(OldAttachPath))
                                    //{
                                    //    Ftp.deleteRemoteFile(OldAttachPath);
                                    //}
                                }
                                if (AttachList.Count > 0)
                                {
                                    foreach (AVEVA.CDMS.Server.Attach temp in AttachList)
                                    {
                                        pasteDoc.AddAttach(temp.O_typeno, temp.O_content, "", 0, 0);
                                    }
                                }
                            }

                            //存在多个版本   2011-5-5
                            if (pasteDoc.DocVersionList != null && pasteDoc.DocVersionList.Count > 0)
                            {
                                foreach (Doc doc in pasteDoc.DocVersionList)
                                {
                                    string OldVersionPath = OldFileDir + doc.O_filename;
                                    string NewVersionPath = NewFileDir + doc.O_filename;
                                    doc.O_projectno = prj.O_projectno;
                                    doc.Modify();
                                    //if (Ftp && Ftp.CheckFileIsExit(OldVersionPath))
                                    //{
                                    //    Ftp.RemoteCopyFile(OldVersionPath, NewVersionPath);
                                    //}
                                    ////移动之后进行删除操作  2011-5-5
                                    //if (Ftp.CheckFileIsExit(NewVersionPath) && Ftp.CheckFileIsExit(OldVersionPath))
                                    //{
                                    //    Ftp.deleteRemoteFile(OldVersionPath);
                                    //}

                                    //多个版本里面存在其他附件  2011-5-5
                                    if (doc.AttachList != null && doc.AttachList.Count > 0)
                                    {
                                        List<AVEVA.CDMS.Server.Attach> AttachList = new List<AVEVA.CDMS.Server.Attach>();
                                        foreach (AVEVA.CDMS.Server.Attach attach in doc.AttachList)
                                        {
                                            string OldAttachPath = OldFileDir + attach.O_content;
                                            string NewAttachPath = NewFileDir + attach.O_content;
                                            AttachList.Add(attach);
                                            //if (Ftp && Ftp.CheckFileIsExit(OldAttachPath))
                                            //{
                                            //    Ftp.RemoteCopyFile(OldAttachPath, NewAttachPath);
                                            //}
                                            ////移动之后进行删除操作  2011-5-5
                                            //if (Ftp.CheckFileIsExit(NewAttachPath) && Ftp.CheckFileIsExit(OldAttachPath))
                                            //{
                                            //    Ftp.deleteRemoteFile(OldAttachPath);
                                            //}
                                        }
                                        if (AttachList.Count > 0)
                                        {
                                            foreach (AVEVA.CDMS.Server.Attach temp in AttachList)
                                            {
                                                doc.AddAttach(temp.O_typeno, temp.O_content, "", 0, 0);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        reJo.success = true;


                    }
                    else
                    {
                        //复制文档（不是剪切文档）

                        if (bDocument)
                            return reJo.Value;

                        //Doc pasteDoc = (Doc)ob;


                        //if (!pasteDoc)
                        //    continue;

                        //if (!bDCreate)
                        //    continue;

                        if (prj.acceData != null && prj.acceData.Right != null && prj.acceData.Right.DCreate == false)
                        {
                            //AfxMessageBox(L"无法复制文档: " + (CString)pasteDoc.O_itemname + L" ,您不拥有在目标目录下创建文件的权限!");

                            //bDCreate = false;

                            //continue;
                            reJo.msg = "无法复制文档: " + pasteDoc.O_itemname + " ,您不拥有在目标目录下创建文件的权限!";
                            return reJo.Value;
                        }

                        if (!Global_ProjectFuns.CopyDocOnToProject(pasteDoc, prj, /*bCopyTempDefn */true, true/*bCopyProperty*/))
                        {
                            reJo.msg = "复制文档 " + pasteDoc.ToString + " 时发生错误,复制过程退出!";
                        }

                        reJo.success = true;
                    }


            }
                

                return reJo.Value;
            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(e.Message);
            }

            return reJo.Value;
        }


        //ProjectPaste
        /// <summary>
        /// 在目录下粘贴一个目录或文档快捷方式
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="ProjectKeyword">粘贴到的目录对象关键字</param>
        /// <param name="ObjectKeyword">被粘贴的文档或目录对象关键字</param>
        /// <returns></returns>
        public static JObject ProjectPasteShortcut(string sid, string ProjectKeyword, string ObjectKeyword)
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

                Project prj = dbsource.GetProjectByKeyWord(ProjectKeyword);

                if (prj == null) {
                    reJo.msg = "无法复制:目标文件夹不存在！";
                    return reJo.Value;
                }


                object obj = dbsource.GetObjectByKeyWord(ObjectKeyword);
                if (obj is Project)
                {
                    Project pastePrj = (Project)obj;

                    ////小黎 2011-5-3  
                    ////HTREEITEM Parent=this->m_hCurItem;
                    //if ( this->GetTreeCtrl().GetChildItem(Parent) )
                    //{
                    //    //HTREEITEM Children=this->GetTreeCtrl().GetChildItem(Parent);
                    //    BOOL exist=FALSE;
                    //    while ( Children )
                    //    {
                    //        Project tempProj=GetProjectFromHTREEITEM(Children);
                    //        if ( tempProj.ToString == pastePrj.ToString )
                    //        {
                    //            exist=true;
                    //            break;
                    //        }
                    //        Children=this.GetTreeCtrl().GetNextSiblingItem(Children);
                    //    }
                    //    if ( exist )
                    //    {
                    //        return reJo.Value;
                    //    }
                    //}
                    ////END


                    if (pastePrj == null)
                    {
                        reJo.msg = "无法复制: 目录不存在!";
                        return reJo.Value;
                    }


                    //判断是否有目标文件夹下的创建权限
                    bool hasRight = GetProjectPCreateRight(prj, curUser);

                    if (!hasRight)
                    {
                        reJo.msg = "无法复制: 您不拥有在目标文件夹下创建子目录的权限!";
                        return reJo.Value;
                    }



                    if (pastePrj == prj)
                    {

                        reJo.msg = "无法复制:" + pastePrj.O_projectname + " 目标文件夹和源文件夹相同";
                        return reJo.Value;
                    }

                    if (Global_ProjectFuns.IsProjectUnderAnother(pastePrj, prj))
                    {
                        reJo.msg = "无法复制:" + pastePrj.O_projectname + " 目标文件夹是源文件夹的子文件夹";
                        return reJo.Value;
                    }


                    Project copyPrj;



                    copyPrj = prj.NewProject(pastePrj);


                    if (copyPrj != null)
                    {

                        //if(Global::ydMainFrame)
                        //{
                        //    Global::ydMainFrame->m_wndStatusBar.SetPaneText(0 , (CString)("就绪")) ; 					

                        //}
                        reJo.success = true;
                        return reJo.Value;
                    }
                }
                else if (obj is Doc)
                {
                    // return PasteDoc(sid, ProjectKeyword, ObjectKeyword);

                    //                if(bDocument)
                    //continue; 

                    Doc pasteDoc = (Doc)obj;


                    if (pasteDoc == null)
                    {
                        reJo.msg = "无法复制: 文档不存在!";
                        return reJo.Value;
                    }

                    //判断是否有目标文件夹下的创建权限
                    bool hasRight = GetProjectPCreateRight(prj, curUser);

                    if (!hasRight)
                    {
                        reJo.msg = "无法复制: 您不拥有在目标文件夹下创建子目录的权限!";
                        return reJo.Value;
                    }

                    Doc newShortDoc = prj.NewDoc(pasteDoc);

                    //小黎 2012-7-9 把流程和状态都复制过来 
                    newShortDoc.O_dmsstatus = pasteDoc.O_dmsstatus;
                    if (pasteDoc.WorkFlow != null)
                    {
                        newShortDoc.WorkFlow = pasteDoc.WorkFlow;
                    }
                    //END

                    reJo.success = true;
                    return reJo.Value;
                }
                


                return reJo.Value;
            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(e.Message);
            }

            return reJo.Value;
        }

        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="ProjectKeyword">Project节点关键字</param>
        /// <param name="sureDel">确定是否要删除，当为"true"时直接删除，当为"false"时返回提示确认是否删除</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时，data包含一个JObject，里面包含参数"state"；</para>
        /// <para>当 "state"的值为"seleSureDel"时，表示提示客户端选择是否确认删除</para>
        /// <para>当 "state"的值为"delSuccess"时，表示删除成功</para>
        /// <para>例子：</para>
        /// </returns>
        public static JObject DelProject(string sid, string ProjectKeyword, string sureDel)
        {
            ExReJObject reJo = new ExReJObject();
            try
            {
                User curUser = DBSourceController.GetCurrentUser(sid);
                if (curUser==null)
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


                //删除当前节点对应的Project对象
                Project project = dbsource.GetProjectByKeyWord(ProjectKeyword);

                bool hasPDeleteRight = ProjectController.GetProjectPDeleteRight(project,curUser);

                if (!hasPDeleteRight && (!(project.O_type==enProjectType.GlobSearch && curUser.IsAdmin)))
                {
                    reJo.msg = "您无权删除目录" + project.ToString + "！";
                    return reJo.Value;
                }

                //对搜索目录的删除操作
                if (!string.IsNullOrEmpty(project.SearchSQL))
                {
                    project.Delete();
                    project.Modify();
                    reJo.success = true;
                }
                else
                {

                    int nSubProjectCount = 0;
                    int nDocCount = 0;

                    List<Doc> lockDocList = new List<Doc>();

                    GetAllSubProjectCount(project, ref nSubProjectCount); //计算Project下面子孙目录的个数
                    GetAllDocCount(project, ref nDocCount, ref lockDocList); //计算Project下面所有文档的个数
          


                    if (nSubProjectCount > 0 && sureDel == "false")
                    {

                        if (nDocCount > 0)
                            reJo.msg = "选择删除的目录下存在: " + nSubProjectCount.ToString() + " 个子目录、" + nDocCount.ToString() + " 个文档 , 继续删除则所有这些目录和文档都会被删除(*删除快捷方式不会删除实体文件) , 是否继续?";
                        else if (nSubProjectCount > 0)
                            reJo.msg = "选择删除的目录下存在: " + nSubProjectCount.ToString() + " 个子目录 , 继续删除则所有这些目录都会被删除 , 是否继续?";
                        reJo.success = true;
                        reJo.data = new JArray(new JObject(new JProperty("state", "seleSureDel")));//选择确定删除
                        return reJo.Value;
                    }
                    else if (sureDel == "false")//当目录里面没有文档或子目录的时候，也要提示是否删除目录
                    {
                        string projectName = project.Code + (string.IsNullOrEmpty(project.Description) ? "" : "__" + project.Description);
                        reJo.msg = "请确认是否要删除目录： " + projectName;
                        reJo.success = true;
                        reJo.data = new JArray(new JObject(new JProperty("state", "seleSureDel")));//选择确定删除
                        return reJo.Value;
                    }
                    else
                    {
                        if (nDocCount > 0 && sureDel == "false")
                        {
                            reJo.msg = "选择删除的目录下存在: " + nDocCount.ToString() + " 个文档 , 继续删除则所有这些文档都会被删除(*删除快捷方式不会删除实体文件) , 是否继续?";
                            reJo.success = true;
                            reJo.data = new JArray(new JObject(new JProperty("state", "seleSureDel")));//选择确定删除
                            return reJo.Value;
                        }
                        else if (lockDocList.Count > 0)
                        {
                            reJo.msg = "选择删除的目录下存在: " + lockDocList.Count.ToString() + " 个文档被锁定！";
                            return reJo.Value;
                        }
                        else
                        {
                            //判断project下是否存在不能删除的子project或者doc
                            reJo = DeleteProject(dbsource, project);
                            return reJo.Value;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                reJo.msg=e.Message;
                CommonController.WebWriteLog(e.Message);
            }

            return reJo.Value;
        }

    

         //计算Project下面子孙目录的个数
        private static void GetAllSubProjectCount(Project project, ref int nCount)
        {
            if (project == null)
                return;
            try
            {

                if (project.ChildProjectList != null && project.ChildProjectList.Count > 0)
                {

                    foreach (Project subProject in project.ChildProjectList)
                    {
                        GetAllSubProjectCount(subProject, ref nCount);
                    }

                    nCount += project.ChildProjectList.Count;
                }

            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
            }

        }

        //计算Project下面所有文档的个数
        private static void GetAllDocCount(Project project, ref int nCount, ref List<Doc> lockDocList)
        {
            try
            {

                if (project == null || lockDocList == null)
                    return;

                if (project.ChildProjectList != null && project.ChildProjectList.Count > 0)
                {
                    foreach (Project subProject in project.ChildProjectList)
                    {
                        GetAllDocCount(subProject, ref nCount, ref lockDocList);
                    }
                }

                if (project.DocList != null && project.DocList.Count > 0)
                {
                    foreach (Doc doc in project.DocList)
                    {
                        if (doc.O_dmsstatus != (enDocStatus)2)
                        {
                            lockDocList.Add(doc);
                        }
                    }
                    nCount += project.DocList.Count;

                }

            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
            }


        }
        
        //循环删除目录
        private static ExReJObject DeleteProject(DBSource dbsource, Project project)
        {
            ExReJObject reJo = new ExReJObject();
            try
            {
                if (project == null)
                {
                    reJo.msg = "不存在此目录！";
                    return reJo;
                }
                else
                {
                    bool delChildProjectSuccess = true;
                    if (project.ChildProjectList != null && project.ChildProjectList.Count > 0)
                    {
                        int pCount = project.ChildProjectList.Count;

                        for (int i = 0; i < pCount; i++)
                        {
                            if (project.ChildProjectList.Count <= 0)
                                break;

                            Project subProject = project.ChildProjectList[0];
                            ExReJObject reJoItem = DeleteProject(dbsource, subProject);
                            if (!reJoItem.success)
                            {
                                delChildProjectSuccess = false;
                                reJo = reJoItem;
                                break;
                            }
                        }
                    }

                    if (!delChildProjectSuccess)
                    {
                        //reJo.msg = "存在不能删除的目录或文档！";
                        //前面用reJo = reJoItem;已经把错误消息传递过来了
                    }
                    else
                    {
                        bool delChildDocSuccess = true;
                        string strErrDoc = "";
                        if (project.DocList != null && project.DocList.Count > 0)
                        {
                            int dCount = project.DocList.Count;
                            for (int i = 0; i < dCount; i++)
                            {
                                if (project.DocList.Count <= 0)
                                    break;

                                Doc doc = project.DocList[0];

                                List<string> attachFileList = doc.AttachFileList;

                                if (!doc.Delete())
                                {
                                    delChildDocSuccess = false;
                                }

                            }
                        }

                        if (!delChildDocSuccess)
                        {
                            reJo.msg = "删除文档 " + strErrDoc + " 失败! \r\n 退出删除过程,可能部分对象没能成功删除!";
                            return reJo;
                        }
                        else
                        {
                            if (!project.Delete())
                            {
                                reJo.msg = "删除目录 " + project.ToString + " 失败! \r\n 退出删除过程,可能部分对象没能成功删除!";
                                return reJo;
                            }
                            else
                            {

                                try
                                {
                                    //如果是项目根目录，删除项目根目录
                                    if (project.O_parentno <= 0)
                                    {
                                        if (dbsource.RootGCustProjectList.Contains(project))
                                        {
                                            dbsource.RootGCustProjectList.Remove(project);
                                        }
                                        else if (dbsource.RootGSchProjectList.Contains(project))
                                        {
                                            dbsource.RootGSchProjectList.Remove(project);
                                        }
                                        else if (dbsource.RootLocalProjectList.Contains(project))
                                        {
                                            dbsource.RootLocalProjectList.Remove(project);
                                        }
                                        else if (dbsource.RootUCustProjectList.Contains(project))
                                        {
                                            dbsource.RootUCustProjectList.Remove(project);
                                        }
                                        else if (dbsource.RootUSchProjectList.Contains(project))
                                        {
                                            dbsource.RootUSchProjectList.Remove(project);
                                        }

                                    }
                                }
                                catch (Exception sex)
                                {
                                    CommonController.WebWriteLog(sex.Message);
                                }

                                reJo.success = true;
                                reJo.data = new JArray(new JObject(new JProperty("state", "delSuccess")));//返回删除成功消息给客户端
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                reJo.msg= e.Message;
                CommonController.WebWriteLog(e.Message);
            }

            return reJo;
        }

        /// <summary>
        /// 返回目录属性，在调用目录的“新建目录”菜单和“编辑属性”菜单，初始化表单时使用
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="action">操作类型，当值为"CreateProject"时，返回存储空间，当值为"ModiProject"时，返回详细的目录属性</param>
        /// <param name="ProjectKeyword">Project节点关键字</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时且操作类型为"ModiProject"(修改目录)时，data包含一个JObject，里面包含参数"projname"：project代码，
        /// "projdesc"：Project描述，"defkeyword"：Project模板的IndexKeyWord，"defkeyid"：Project模板的Id,"defname":Project模板的代码,"defdesc":Project模板的描述,
        /// "storagekeyword"：存储空间keyword，"storagename"：存储空间的代码，"storagedesc"：存储空间的描述，"isHide"</para>
        /// <para>操作成功时且操作类型为"CreateProject"(创建目录)时，success返回true，data包含一个空的JObject</para>
        /// <para>例子：</para>
        /// </returns>
        /// 

        public static JObject GetProjectAttr(string sid, string action, string ProjectKeyword)
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


                JObject JoProject = new JObject();
                if (action != "CreateProject")//如果不是新建目录
                {
                    Project proj = dbsource.GetProjectByKeyWord(ProjectKeyword);
                    if (proj != null)
                    {
                        //添加属性页信息

                        JoProject.Add(new JProperty("projname", proj.Code));
                        JoProject.Add(new JProperty("projdesc", proj.Description));
                        if (proj.TempDefn != null)
                        {
                            JoProject.Add(new JProperty("defkeyword", proj.TempDefn.IndexKeyWord));
                            JoProject.Add(new JProperty("defkeyid", proj.TempDefn.ID.ToString()));
                            JoProject.Add(new JProperty("defname", proj.TempDefn.Code));
                            JoProject.Add(new JProperty("defdesc", proj.TempDefn.Description));
                        }
                        else
                        {
                            JoProject.Add(new JProperty("defkeyword", ""));
                            JoProject.Add(new JProperty("defkeyid", ""));
                            JoProject.Add(new JProperty("defname", ""));
                            JoProject.Add(new JProperty("defdesc", ""));
                        }

                        if (proj.Storage != null)
                        {
                            JoProject.Add(new JProperty("storagekeyword", proj.Storage.KeyWord));
                            JoProject.Add(new JProperty("storagename", proj.Storage.Code));
                            JoProject.Add(new JProperty("storagedesc", proj.Storage.Description));
                        }
                        else
                        {
                            JoProject.Add(new JProperty("storagekeyword", ""));
                            JoProject.Add(new JProperty("storagename", ""));
                            JoProject.Add(new JProperty("storagedesc", ""));
                        }

                        JoProject.Add(new JProperty("isHide", proj.Hide));
                    }
                }


                JObject joStorage = new JObject();
                foreach (Storage storage in dbsource.AllStorageList)
                {
                    joStorage.Add(new JProperty("storagekeyword", storage.KeyWord));
                    joStorage.Add(new JProperty("storagename", storage.Code));//Description));
                    joStorage.Add(new JProperty("storagetext", storage.ToString));
                }
                JoProject.Add(new JProperty("storagelist", new JArray(joStorage)));
                JoProject.Add(new JProperty("storagetotal", dbsource.AllStorageList.Count));

                reJo.data = new JArray(JoProject);
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


        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="projectKeyword">Project节点关键字</param>
        /// <param name="projectAttrJson">目录属性Json，每个JObject包含有："name"，"value"两个属性，
        /// "name"="projectName"时修改目录名，"name"="projectDesc"时修改目录描述，"name"="tempDefnId"时修改模板ID,name"="isHide"时修改是否隐藏
        /// 例如:[{ name: 'projectName', value: projectName },{ name: 'projectDesc', value: projectDesc },{ name: 'tempDefnId', value: tempDefnId },{ name: 'storageName', value: storageName },{ name: 'isHide', value: isHide }];</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时，data包含一个空的JObject</para>
        /// <para>例子：</para>
        /// </returns>
        public static JObject CreateProject(string sid,  string projectKeyword, string projectAttrJson)
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

                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(projectAttrJson);

                string strProjectName = "", strProjectDesc = "", strStorageName = "", strTempDefnId = "";
                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();
                    if (strName == "projectName") strProjectName = strValue;
                    else if (strName == "projectDesc") strProjectDesc = strValue;
                    else if (strName == "storageName") strStorageName = strValue;
                    else if (strName == "tempDefnId") strTempDefnId = strValue;
                }

                Project projectByName;
                string parentKeyword = "";

                if (projectKeyword == "Root")
                {
                    //创建根文件夹
                    projectByName = dbsource.NewProject(enProjectType.Local);

                    string tempDefnCode = "";
                    if (!string.IsNullOrEmpty(strTempDefnId))
                    {
                        TempDefn tempDefn = dbsource.GetTempDefnByID(Convert.ToInt32(strTempDefnId));
                        if (tempDefn == null)
                        {
                            reJo.msg = "欲创建的文件夹关联的模板不存在！不能完成创建";
                            return reJo.Value;
                        }
                        tempDefnCode = tempDefn.Code;
                    }
                    
                    return CreateRootProject(sid, strProjectName, strProjectDesc, strStorageName, tempDefnCode);
                    //parentKeyword = "Root";
                }

                Project parentproject = dbsource.GetProjectByKeyWord(projectKeyword);
                if (parentproject == null)
                {
                    reJo.msg = "创建目录失败，上级目录不存在！";
                    return reJo.Value;
                }

                bool hasRight = GetProjectPCreateRight(parentproject, curUser);

                //增加创建权限控制
                if (!hasRight)
                {
                    reJo.msg = "创建目录失败,您不拥有在目标文件夹下创建子目录的权限!";
                    return reJo.Value;
                }


                //获取存储空间
                Storage storage = string.IsNullOrEmpty(strStorageName) ? parentproject.Storage : dbsource.GetStorageByName(strStorageName);

                projectByName = new Project();
                if (!string.IsNullOrEmpty(strTempDefnId))
                {
                    TempDefn tempDefn = dbsource.GetTempDefnByID(Convert.ToInt32(strTempDefnId));
                    if (storage != null && tempDefn != null)
                        projectByName = parentproject.NewProject(strProjectName, strProjectDesc, storage, tempDefn);
                    else if (storage != null)
                        projectByName = parentproject.NewProject(strProjectName, strProjectDesc, storage);
                }
                else
                {
                    projectByName = parentproject.NewProject(strProjectName, strProjectDesc, storage);
                }
                parentKeyword = parentproject.KeyWord;

                //DBSourceController.RefreshDBSource(sid);

                if (projectByName == null)
                {
                    reJo.msg = "创建目录失败！";
                    return reJo.Value;
                }

                //是否有子节点
                bool projectLeaf = projectByName.ShortProject == null ?
                    (projectByName.O_subprojects == "Y" ? false : true) :
                    (projectByName.ShortProject.O_subprojects == "Y" ? false : true);

                reJo.data = new JArray(new JObject(new JProperty("id", projectByName.KeyWord),
                    new JProperty("Keyword", projectByName.KeyWord),
                    new JProperty("text", projectByName.ToString),
                    new JProperty("leaf", projectLeaf),//默认没有子节点
                    new JProperty("iconCls", "writefolder"),//设置图标
                    new JProperty("parentid", parentKeyword)
                    ));

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

        //
        //参数：目录名，目录描述，模板ID,存储keyword,是否隐藏
        /// <summary>
        /// 更新project属性，属性页
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="projectKeyword">Project节点关键字</param>
        /// <param name="projectAttrJson">目录属性Json，每个JObject包含有："name"，"value"两个属性，"name"="projectName"时修改目录名，"name"="projectDesc"时修改目录描述，"name"="tempDefnId"时修改模板ID,name"="isHide"时修改是否隐藏
        /// 例如:[{ name: 'projectName', value: projectName },{ name: 'projectDesc', value: projectDesc },{ name: 'tempDefnId', value: tempDefnId },{ name: 'storageName', value: storageName },{ name: 'isHide', value: isHide }];</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时，data包含一个空的JObject</para>
        /// <para>例子：</para>
        /// </returns>
        public static JObject UpdateProjectAttr(string sid, string projectKeyword, string projectAttrJson)
        {
            ExReJObject reJo = new ExReJObject();
            try
            {
                User curUser = DBSourceController.GetCurrentUser(sid);
                if (curUser==null)
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

                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(projectAttrJson);

                Project project = dbsource.GetProjectByKeyWord(projectKeyword);
                if (project == null)
                {
                    reJo.msg = "修改目录属性失败，目录不存在！";
                    return reJo.Value;
                }
                //有修改目录权限才可以修改目录属性
                //以父目录的权限来限定当前目录属性的修改权 

                bool hasProjectEditRight = GetProjectPCntrlRight(project, curUser);
                if (!hasProjectEditRight)
                {
                    reJo.msg = "修改目标文件夹属性失败：用户没有目标修改文件夹属性权限！";
                    return reJo.Value;
                }

                bool itemNameError = false;
                string strProjectName = "";

                //先检查一遍文件名有没有错误
                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();

                    if (strName == "projectName")
                    {
                        if (project.O_projectname != strValue)//如果修改了文件名
                        {
                            //查询是否已经有同名的文档
                            Project parentproject = project.ParentProject;
                            if (parentproject != null)
                            {
                                bool existItemName = false;
                                foreach (Project proj in parentproject.ChildProjectList)
                                {
                                    if (proj.O_projectname == strValue)
                                    {
                                        existItemName = true;
                                        break;
                                    }
                                }
                                if (!existItemName)
                                {
                                    project.O_projectname = strValue;
                                }
                                else
                                {
                                    itemNameError = true;
                                    strProjectName = strValue;
                                }
                            }
                        }
                        break;
                    }

                }

                if (itemNameError == true)//如果修改了文件名，并且目录没有提交的文档的名字，就修改文档属性，否则就返回错误信息
                {
                    reJo.msg = "无法修改目录 " + strProjectName + ":指定的文件夹名与现有文件夹重名. 请指定另一个文件夹名称！";
                    return reJo.Value;
                }

                foreach (JObject joAttr in jaAttr)
                {
                    
                    //修改目录属性
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();
                    string strType = "";
                    if (joAttr.Property("attrtype") != null)//判断是否是附加属性
                        strType = joAttr["attrtype"].ToString();

                    if (strType != "attrData")//一般属性
                    {
                        if (strName == "projectName")//如果参数是文件描述
                        {
                            project.O_projectname = strValue;
                            
                        }
                        else if (strName == "projectDesc")//如果参数是文件描述
                        {
                            project.O_projectdesc = strValue;
                        }
                        else if (strName == "storageName")//存储空间
                        {
                            Storage storage = dbsource.GetStorageByName(strValue);
                            if (storage != null)
                            {
                                project.Storage = storage;
                            }
                        }
                        else if (strName == "tempDefnId")//目录模板
                        {
                            if (!string.IsNullOrEmpty(strValue))
                            {
                                TempDefn tempDefn = dbsource.GetTempDefnByID(Convert.ToInt32(strValue));
                                if (tempDefn != null)
                                {
                                    project.TempDefn = tempDefn;
                                }
                            }

                        }
                    }
                    else
                    { 
                        //修改目录附加属性
                        AttrDataController.UpdateAttrData(project.AttrDataList, strName, strValue);
                    }
                }
                project.Modify();
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
        /// 返回Project模板下所有Project和Doc子模板列表
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="node">Project节点关键字</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,total返回记录总数；操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时，data包含多个JObject，每个JObject表示一个模板，里面包含参数"text"：模板描述，"id"：模板ID，"leaf"：是否有子模板
        /// "type"：模板类型，"iconCls"：设置图标</para>
        /// <para>例子：</para>
        /// </returns>
        public static JObject GetTempDefList(string sid, string node)
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


                string keyword = node ?? "Root";

                JArray jaGetList = new JArray();

                JArray jaDefnList = new JArray();
                List<TempDefn> tempDefList = new List<TempDefn>();
                if (keyword == "Root")
                {
                    tempDefList = dbsource.AllTempDefnList;
                }
                else
                {
                    TempDefn td = dbsource.GetTempDefnByID(Convert.ToInt32(keyword));
                    tempDefList = td.ChildTempDefnList;
                }

                //循环获取模板列表
                foreach (TempDefn tempDefn in tempDefList)
                {
                    JObject joDefn = new JObject();
                    int childCount = tempDefn.ChildTempDefnList.Count + tempDefn.AttrTempDefnList.Count;
                    bool flag = false;
                    if (childCount <= 0 || tempDefn.Attr_type.ToString() == "Doc")
                        flag = true;
                    string img = "tempdefnfolder";
                    if (tempDefn.Attr_type.ToString() == "Doc") img = "tempdefndoc";

                    joDefn = new JObject(new JProperty("text", tempDefn.ToString), new JProperty("id", tempDefn.ID.ToString()), new JProperty("leaf", flag),
                        new JProperty("type", tempDefn.Attr_type.ToString()), new JProperty("iconCls", img)//设置图标
                         );
                    jaDefnList.Add(joDefn);
                }

                reJo.data = jaDefnList;
                reJo.total = jaDefnList.Count;
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
        /// 创建根文件夹
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="projectCode">文件夹的代码</param>
        /// <param name="projectDesc">文件夹的描述</param>
        /// <param name="storageName">存储空间名称</param>
        /// <param name="TempDefnCode">文件夹的模板代码</param>
        /// <returns></returns>
        public static JObject CreateRootProject(string sid, string projectCode, string projectDesc,string storageName, string TempDefnCode)
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

                if (string.IsNullOrEmpty(projectCode))
                {
                    reJo.msg = "请填写文件夹代码！";
                    return reJo.Value;
                }
                //else if (string.IsNullOrEmpty(projectDesc))
                //{
                //    reJo.msg = "请填写文件夹名称！";
                //    return reJo.Value;
                //}

                TempDefn mTempDefn = null;
                if (!string.IsNullOrEmpty(TempDefnCode))
                {
                    //  根据名称查找项目模板(根目录)对象
                    List<TempDefn> tempDefnByCode = dbsource.GetTempDefnByCode(TempDefnCode);
                    mTempDefn = (tempDefnByCode != null) ? tempDefnByCode[0] : null;
                    if (mTempDefn == null)
                    {
                        reJo.msg = "欲创建的文件夹关联的模板不存在！不能完成创建";
                        return reJo.Value;
                    }
                }

                Project pro = dbsource.NewProject(enProjectType.Local);

                //查找项目是否已经创建
                Project findProj = dbsource.RootLocalProjectList.Find(itemProj => itemProj.Code == projectCode);
                if (findProj != null)
                {
                    reJo.msg = "文件夹[" + projectCode + "]已存在！不能完成创建";
                    return reJo.Value;
                }

                //获取存储空间
                Storage storage = string.IsNullOrEmpty(storageName) ? null : dbsource.GetStorageByName(storageName);

                Project NewProject = pro.NewProject(projectCode, projectDesc, storage, mTempDefn);

                if (NewProject == null)
                {
                    reJo.msg = "新建文件夹失败！";
                    return reJo.Value;
                }

                //增加全体用户组权限
                AVEVA.CDMS.Server.User adminUser = dbsource.GetUserByName("admin");
                if (adminUser != null)
                {
                    NewProject.acceData.AddAcce(adminUser, 3460); //读目录，读写文件权限
                }

                //DBSourceController.RefreshShareDBManager();
                DBSourceController.RefreshDBSource(sid);

                //return GetProjectListJson(sid, NewProject.KeyWord, "1");

                //是否有子节点
                bool projectLeaf = NewProject.ShortProject == null ?
                    (NewProject.O_subprojects == "Y" ? false : true) :
                    (NewProject.ShortProject.O_subprojects == "Y" ? false : true);

                reJo.data = new JArray(new JObject(new JProperty("id", NewProject.KeyWord),
                    new JProperty("Keyword", NewProject.KeyWord),
                    new JProperty("text", NewProject.ToString),
                    new JProperty("leaf", projectLeaf),//默认没有子节点
                    new JProperty("iconCls", "writefolder"),//设置图标
                    new JProperty("parentid", "Root")
                    ));

                reJo.success = true;
                return reJo.Value;

            }
            catch (Exception ex) { reJo.msg = "新建文件夹失败！" + ex.Message; }
            return reJo.Value;

        }

        /// <summary>
        /// 创建个人工作台，逻辑目录，或者查询
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="projectKeyword">父文件夹关键字</param>
        /// <param name="projectCode">文件夹的代码</param>
        /// <param name="projectDesc">文件夹的描述</param>
        /// <param name="projectType">文件夹的类型，1：项目目录，4，：查询，5：个人工作台，7：逻辑目录</param>
        /// <param name="SQLString">当新建查询时，传递查询字符串</param>
        /// <returns></returns>
        public static JObject NewGlobalOrUserProject(string sid, string projectKeyword, string projectCode, string projectDesc, string projectType, string SQLString)
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

                if (string.IsNullOrEmpty(projectCode))
                {
                    reJo.msg = "请填写文件夹代码！";
                    return reJo.Value;
                }

                if (!curUser.IsAdmin)
                {
                    reJo.msg = "当前用户没有新建文件夹权限！";
                    return reJo.Value;
                }

                string newName = projectCode;
                string newDesc = projectDesc;

                //判断当前节点是哪个类型的根节点'逻辑目录','个人工作台','全局查询','个人查询'
                enProjectType projType = (enProjectType)Enum.Parse(typeof(enProjectType), projectType);

                if (projType == null)
                {
                    reJo.msg = "新建文件夹失败!获取文件夹类型参数错误!";
                    return reJo.Value;
                }

                Project project;

                if (projectKeyword == "Root")
                {
                    if (projType == enProjectType.GlobCustom || projType == enProjectType.UserCustom || projType == enProjectType.GlobSearch)
                    {
                        project = dbsource.NewProject(projType);
                    }
                    else
                    {
                        reJo.msg = "新建文件夹失败!文件夹类型参数错误!";
                        return reJo.Value;
                    }
                }
                else
                {
                    project = dbsource.GetProjectByKeyWord(projectKeyword);
                    if (project == null)
                    {
                        reJo.msg = "新建文件夹失败!父文件夹参数错误!";
                        return reJo.Value;
                    }
                }

                ////新建逻辑目录
                //if(obj->ToString()->ToLower() == Global::tv_srLogic->ToLower())
                //{
                //    project = dbsource.NewProject(enProjectType.GlobCustom) ;
                //}

                ////新建个人工作台
                //if(obj->ToString()->ToLower() == Global::tv_srDesktop->ToLower())
                //{
                //    project = dbsource.NewProject(enProjectType.UserCustom) ; 
                //}

                ////新建全局查询
                //if(obj->ToString()->ToLower() == Global::tv_srGlobalQuery->ToLower() )
                //{
                //    project = dbsource.NewProject(enProjectType.GlobSearch) ; 


                //}

                //if(obj->ToString()->ToLower() ==  Global::tv_srPrivateQuery->ToLower() )
                //{
                //    project = dbs->NewProject(enProjectType::UserSearch) ; 
                //    dlg.m_title = L"新建个人查询" ;

                //}

                bool bNameExisted = false;


                if (project.ChildProjectList != null && project.ChildProjectList.Count > 0)
                {
                    foreach (Project subProject in project.ChildProjectList)
                    {
                        if (subProject.O_projectname == newName)
                        {
                            bNameExisted = true;
                            break;
                        }
                    }
                }


                if (bNameExisted)
                {
                    reJo.msg = "无法新建文件夹 " + newName + " :指定的文件名与现有文件夹重名. 请指定另一个文件名!";
                    return reJo.Value;
                }

                Project newProject = null;

                if (projType != enProjectType.GlobSearch )//|| (projType == enProjectType.GlobSearch && projectKeyword == "Root"))
                {
                    //新建逻辑目录或个人工作台或根查询
                    newProject = project.NewProject(newName, newDesc, project.Storage);

                    if (newProject == null)
                    {
                        reJo.msg = "新建文件夹失败!";
                        return reJo.Value;
                    }
                }
                else 
                {
                    //在逻辑目录或个人工作台新建查询
                    Project selProject = project;
                    //判断是否存在同名的子目录
                    bool bExistedChild = false;
                    Project existedChild = null;
                    if (selProject.ChildProjectList != null && selProject.ChildProjectList.Count > 0)
                    {
                        foreach (Project cProject in selProject.ChildProjectList)
                        {
                            if (cProject.O_projectname.ToLower() == newName.Trim().ToLower())
                            {
                                bExistedChild = true;
                                existedChild = cProject;
                                break;
                            }
                        }
                    }

                    if (bExistedChild && existedChild != null)
                    {
                        //if (MessageBox.Show("已存在代码为 : " + this.newSearchChildName.Text + " 的节点,是否覆盖原有节点?", "提示", MessageBoxButtons.YesNo) == DialogResult.No)
                        reJo.msg= "已存在代码为 : " + newName + " 的节点!";
                        return reJo.Value;

                        //sProject = existedChild;
                    }
                    else
                    {
                       newProject = selProject.NewProject(newName, "");
                    }

                    if (newProject != null)
                    {
                       newProject.O_type = (selProject.O_type == enProjectType.UserSearch || selProject.O_type == enProjectType.GlobSearch) ? selProject.O_type : enProjectType.GlobSearch;
                        //sProject.AddAttach(enAttachFileType.AttachSQL, this.strSQL, "", 0, 0); 
                        newProject.SearchSQL = SQLString;
                        newProject.Modify();
                    }

                }

                DBSourceController.RefreshDBSource(sid);

                Project parentproject = newProject.ParentProject;
                string parentKeyword = (projectKeyword == "Root") ? "Root" : parentproject.KeyWord;

                string folderType = "";
                if (projType == enProjectType.Local || projType == enProjectType.GlobCustom || projType == enProjectType.UserCustom)//当查询类型是项目目录或个人工作台
                {
                    folderType = "writefolder";

                }
                else if ( projType == enProjectType.GlobSearch) //当查询类型是个人查询，全局查询或逻辑目录
                {
                    folderType = "searchfolder";
                }

                reJo.data = new JArray(new JObject(new JProperty("id", newProject.KeyWord),
                    new JProperty("Keyword", newProject.KeyWord),
                    new JProperty("text", newProject.ToString),
                    new JProperty("leaf", true),//默认没有子节点
                    new JProperty("iconCls", folderType),//设置图标
                    new JProperty("parentid", parentKeyword)
                    ));

                reJo.success = true;
                return reJo.Value;

            }
            catch (Exception ex) { reJo.msg = "新建文件夹失败！" + ex.Message; }
            return reJo.Value;
        }

        /// <summary>
        /// 修改个人工作台，逻辑目录，或者查询
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="projectKeyword">文件夹关键字</param>
        /// <param name="projectCode">文件夹的代码</param>
        /// <param name="projectDesc">文件夹的描述</param>
        /// <param name="projectType">文件夹的类型，1：项目目录，4，：查询，5：个人工作台，7：逻辑目录</param>
        /// <param name="SQLString">当修改查询时，传递查询字符串</param>
        /// <returns></returns>
        public static JObject UpdateGlobalOrUserProject(string sid, string projectKeyword, string projectCode, string projectDesc, string projectType, string SQLString)
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

                if (string.IsNullOrEmpty(projectCode))
                {
                    reJo.msg = "请填写文件夹代码！";
                    return reJo.Value;
                }

                if (!curUser.IsAdmin)
                {
                    reJo.msg = "当前用户没有编辑文件夹权限！";
                    return reJo.Value;
                }

                string newName = projectCode;
                string newDesc = projectDesc;

                //判断当前节点是哪个类型的根节点'逻辑目录','个人工作台','全局查询','个人查询'
                enProjectType projType = (enProjectType)Enum.Parse(typeof(enProjectType), projectType);

                if (projType == null)
                {
                    reJo.msg = "新建文件夹失败!获取文件夹类型参数错误!";
                    return reJo.Value;
                }

                Project project;

                    project = dbsource.GetProjectByKeyWord(projectKeyword);
                    if (project == null)
                    {
                        reJo.msg = "新建文件夹失败!文件夹参数错误!";
                        return reJo.Value;
                    }

               
                bool bNameExisted = false;


                if (project.ChildProjectList != null && project.ChildProjectList.Count > 0)
                {
                    foreach (Project subProject in project.ChildProjectList)
                    {
                        if (subProject.O_projectname == newName && subProject.KeyWord != projectKeyword)
                        {
                            bNameExisted = true;
                            break;
                        }
                    }
                }


                if (bNameExisted)
                {
                    reJo.msg = "无法编辑文件夹 " + newName + " :指定的文件名与现有文件夹重名. 请指定另一个文件名!";
                    return reJo.Value;
                }

                if (projType != enProjectType.GlobSearch)
                {

                        reJo.msg = "编辑文件夹失败!";
                        return reJo.Value;
                }
                else
                {

                        project.O_projectname = projectCode;
                        project.O_projectdesc = projectDesc;
                        project.SearchSQL = SQLString;
                        project.Modify();
   
                }

                reJo.success = true;
                return reJo.Value;

            }
            catch (Exception ex) { reJo.msg = "编辑文件夹失败！" + ex.Message; }
            return reJo.Value;
        }

        /// <summary>
        /// 获取个人工作台，逻辑目录，或者查询
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="projectKeyword">文件夹关键字</param>
        /// <returns></returns>
        public static JObject GetGlobalOrUserProject(string sid, string projectKeyword)
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

                Project proj = dbsource.GetProjectByKeyWord(projectKeyword);

                if (proj == null || proj.O_type != enProjectType.GlobSearch)
                {
                    reJo.msg = "文件夹参数错误！";
                    return reJo.Value;
                }

                reJo.data = new JArray(
                    new JObject(
                  new JProperty("Keyword", proj.KeyWord),
                  new JProperty("Code", proj.Code),
                  new JProperty("Desc", proj.Description),
                  new JProperty("SQLString", proj.SearchSQL)
                  )
                 );

                reJo.success = true;
                return reJo.Value;

            }
            catch (Exception ex) { reJo.msg = "获取文件夹信息失败！" + ex.Message; }
            return reJo.Value;
        }
    }
}

