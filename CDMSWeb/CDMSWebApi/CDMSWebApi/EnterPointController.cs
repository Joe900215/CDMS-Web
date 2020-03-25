using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AVEVA.CDMS.Server;

namespace AVEVA.CDMS.WebApi
{
    public class EnterPointController : Controller
    {
        //
        // GET: /EnterPoint/


        public ActionResult Index()
        {
            return View();
        }


        private static List<Type> mTypeList;


        internal static List<Type> TypeList
        {
            get
            {
                if (mTypeList == null)
                {
                    mTypeList = new List<Type>();

                    //添加WebApi系统类库
                    mTypeList.Add(typeof(AVEVA.CDMS.WebApi.EnterPointController));
                    mTypeList.Add(typeof(AVEVA.CDMS.WebApi.CommonController));
                    mTypeList.Add(typeof(AVEVA.CDMS.WebApi.DBSourceController));
                    mTypeList.Add(typeof(AVEVA.CDMS.WebApi.DocController));
                    mTypeList.Add(typeof(AVEVA.CDMS.WebApi.MessageController));
                    mTypeList.Add(typeof(AVEVA.CDMS.WebApi.ProjectController));
                    mTypeList.Add(typeof(AVEVA.CDMS.WebApi.UserController));
                    mTypeList.Add(typeof(AVEVA.CDMS.WebApi.GroupController));
                    mTypeList.Add(typeof(AVEVA.CDMS.WebApi.WorkFlowController));
                    mTypeList.Add(typeof(AVEVA.CDMS.WebApi.FileController));
                    //项目数据字典类
                    mTypeList.Add(typeof(AVEVA.CDMS.WebApi.ProjectDictController));
                    
                }

                return mTypeList;
            }
            set
            {
                mTypeList = value;
            }
        }


        /// <summary>
        /// 入口函数
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="tList"></param>
        /// <param name="Server_MapPath"></param>
        /// <returns></returns>
        public static JObject Enter(HttpRequestBase Request, List<Type> tList, string Server_MapPath)
        {

            //函数类型列表
            foreach(Type tp in tList)
            {
                if (!TypeList.Contains(tp)) TypeList.Add(tp);
            }
            tList = TypeList;

            //获取应用的本地地址
            if (WebApi.DBSourceController.Server_MapPath == string.Empty)
            {
                WebApi.DBSourceController.Server_MapPath = Server_MapPath;
            }

            string _dc= Request["_DC"] ?? "";//HTTP连接的ID,每次连接都生成一个连接ID
            string strClass = Request["C"] ?? "";//命名空间+类名
            string strMethod = Request["A"] ?? "";//方法名
            bool success = false;
            string msg = "";
            JArray jaResult = new JArray();
            int total = 0;
            try
            {
                object[] parameters = new object[] { };
                JObject reObj = new JObject();

                Type t = typeof(string);
                object obj;

                foreach (Type mt in tList)
                {
                    if (mt.FullName == strClass)
                    {
                        t = mt;
                        break;
                    }
                }

                if (t.Name == "string")
                { 
                    msg = "参数无效，错误的类名:" + strClass; 
                }
                else
                {
                    System.Reflection.MethodInfo method = t.GetMethod(strMethod);//通过string类型的strMethod获得同名的方法“method”
                    if (method == null)
                    { 
                        msg = "参数无效，错误的方法名:" + strMethod; 
                    }
                    else
                    {
                        //添加参数
                        ParameterInfo[] pInfos = method.GetParameters();
                        List<object> objList = new List<object>();
                        foreach (ParameterInfo pInfo in pInfos)
                        {
                            //添加应用目录
                            if (pInfo.Name == "Server_MapPath")
                            {
                                objList.Add(Server_MapPath);
                            }
                            //添加传递HTTP参数
                            else if (pInfo.Name == "_HttpRequest")
                            {
                                objList.Add(Request);
                            }
                            else
                            {
                                string strType = pInfo.ParameterType.FullName;
                                //转换参数类型，参数类型可以是字符型，整型，双精度型等等
                                if (strType == "System.String")
                                {
                                    objList.Add(Request[pInfo.Name] ?? "");
                                }
                                //整数型参数
                                else if (strType == "System.Int32" )
                                {
                                    int intValue=0;
                                    if (!string.IsNullOrEmpty(Request[pInfo.Name]))
                                    {
                                        intValue = Convert.ToInt32(Request[pInfo.Name]);
                                    }
                                    objList.Add(intValue);
                                }
                                else if (strType == "System.Decimal" )
                                {
                                    Decimal DecimalValue=0;
                                    if (!string.IsNullOrEmpty(Request[pInfo.Name]))
                                    {
                                        DecimalValue = Convert.ToDecimal(Request[pInfo.Name]);
                                    }
                                    objList.Add(DecimalValue);
                                }
                                else if (strType == "System.Double")
                                {
                                    Double DoubleValue = 0;
                                    if (!string.IsNullOrEmpty(Request[pInfo.Name]))
                                    {
                                        DoubleValue = Convert.ToDouble(Request[pInfo.Name]);
                                    }
                                    objList.Add(DoubleValue);
                                }
                                else if (strType == "System.Int64" )
                                {
                                    long longValue = 0;
                                    if (!string.IsNullOrEmpty(Request[pInfo.Name]))
                                    {
                                        longValue = Convert.ToInt64(Request[pInfo.Name]);
                                    }
                                    objList.Add(longValue);
                                }
                                else if (strType == "System.Int16")
                                {
                                    short shortValue = 0;
                                    if (!string.IsNullOrEmpty(Request[pInfo.Name]))
                                    {
                                        shortValue = Convert.ToInt16(Request[pInfo.Name]);
                                    }
                                    objList.Add(shortValue);
                                }
                                else if (strType == "System.Single")
                                {
                                    float floatValue = 0;
                                    if (!string.IsNullOrEmpty(Request[pInfo.Name]))
                                    {
                                        floatValue = Convert.ToSingle(Request[pInfo.Name]);
                                    }
                                    objList.Add(floatValue);
                                }
                                
                            }
                        }
                        object[] paraObjs = objList.ToArray();

                        reObj = new JObject();
                        obj = System.Activator.CreateInstance(t);//创建t类的实例 "obj"
                        reObj = (JObject)method.Invoke(obj, paraObjs);//t类实例obj,调用方法"method(testcase)"

                        //reObj.Add("_resdc",_dc);
                    }
                    return reObj;
                }
            }
            catch (Exception e)
            {
                msg = e.Message;
                CommonController.WebWriteLog(msg);
            }
            JObject reObject= AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
            reObject.Add("_resdc", _dc);
            return reObject;
            //return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
        }

        public static List<ExWebMenu> CreateNewExMenu()
        {
            try
            {
                List<ExWebMenu> menuList = new List<ExWebMenu>();

                ////////////////目录树菜单//////////////////
                CreateNewProjectMenu CreateNewProject = new CreateNewProjectMenu();
                CreateNewProject.MenuId = "MS_CreateNewProject";
                CreateNewProject.MenuName = "新建目录";
                CreateNewProject.MenuPosition = enWebMenuPosition.TVProject;
                CreateNewProject.MenuType = enWebMenuType.Single;

                menuList.Add(CreateNewProject);

                CreateNewRootProjectMenu CreateNewRootProject = new CreateNewRootProjectMenu();
                CreateNewRootProject.MenuId = "MS_CreateNewRootProject";
                CreateNewRootProject.MenuName = "新建根目录";
                CreateNewRootProject.MenuPosition = enWebMenuPosition.TVProject;
                CreateNewRootProject.MenuType = enWebMenuType.Single;

                menuList.Add(CreateNewRootProject);

                CCreateNewRootProjectMenu CCreateNewRootProject = new CCreateNewRootProjectMenu();
                CCreateNewRootProject.MenuId = "MS_CreateNewRootProjec_2";
                CCreateNewRootProject.MenuName = "新建根目录...";
                CCreateNewRootProject.MenuPosition = enWebMenuPosition.TVContainer;
                CCreateNewRootProject.MenuType = enWebMenuType.Single;
                //CCreateNewRootProject.TVMenuPositon = enWebTVMenuPosition.TVDocument;

                menuList.Add(CCreateNewRootProject);

                CCreateUserCustomMenu CCreateUserCustom = new CCreateUserCustomMenu();
                CCreateUserCustom.MenuId = "MS_CreateUserCustom";
                CCreateUserCustom.MenuName = "新建个人工作台...";
                CCreateUserCustom.MenuPosition = enWebMenuPosition.TVContainer;
                CCreateUserCustom.MenuType = enWebMenuType.Single;
                //CCreateUserCustom.TVMenuPositon = enWebTVMenuPosition.TVUserWorkSpace;

                menuList.Add(CCreateUserCustom);

                CreateGlobCustomMenu CreateGlobCustom = new CreateGlobCustomMenu();
                CreateGlobCustom.MenuId = "MS_CreateGlobCustom";
                CreateGlobCustom.MenuName = "新建逻辑目录";
                CreateGlobCustom.MenuPosition = enWebMenuPosition.TVProject;//.TVContainer;
                CreateGlobCustom.MenuType = enWebMenuType.Single;
                //CreateGlobCustom.TVMenuPositon = enWebTVMenuPosition.TVLogicProject;

                menuList.Add(CreateGlobCustom);

                CCreateGlobCustomMenu CCreateGlobCustom = new CCreateGlobCustomMenu();
                CCreateGlobCustom.MenuId = "MS_CreateGlobCustom_2";
                CCreateGlobCustom.MenuName = "新建逻辑目录...";
                CCreateGlobCustom.MenuPosition = enWebMenuPosition.TVContainer;
                CCreateGlobCustom.MenuType = enWebMenuType.Single;
                //CCreateGlobCustom.TVMenuPositon = enWebTVMenuPosition.TVLogicProject;

                menuList.Add(CCreateGlobCustom);

                CreateGlobSearchMenu CreateGlobSearch = new CreateGlobSearchMenu();
                CreateGlobSearch.MenuId = "MS_CreateGlobSearch";
                CreateGlobSearch.MenuName = "新建查询";
                CreateGlobSearch.MenuPosition = enWebMenuPosition.TVProject;
                CreateGlobSearch.MenuType = enWebMenuType.Single;
                //CreateGlobSearch.TVMenuPositon = enWebTVMenuPosition.Default;

                menuList.Add(CreateGlobSearch);

                CCreateGlobSearchMenu CCreateGlobSearch = new CCreateGlobSearchMenu();
                CCreateGlobSearch.MenuId = "MS_CreateGlobSearch_2";
                CCreateGlobSearch.MenuName = "新建查询...";
                CCreateGlobSearch.MenuPosition = enWebMenuPosition.TVContainer;
                CCreateGlobSearch.MenuType = enWebMenuType.Single;
                //CCreateGlobSearch.TVMenuPositon = enWebTVMenuPosition.Default;

                menuList.Add(CCreateGlobSearch);

                MdsqlGlobaloruserProjectMenu MdsqlGlobaloruserProject = new MdsqlGlobaloruserProjectMenu();
                MdsqlGlobaloruserProject.MenuId = "MS_MdsqlGlobaloruserProject";
                MdsqlGlobaloruserProject.MenuName = "编辑查询条件";
                MdsqlGlobaloruserProject.MenuPosition = enWebMenuPosition.TVProject;
                MdsqlGlobaloruserProject.MenuType = enWebMenuType.Single;
                //MdsqlGlobaloruserProject.TVMenuPositon = enWebTVMenuPosition.Default;

                menuList.Add(MdsqlGlobaloruserProject);

                CreateProjByDefMenu CreateProjByDef = new CreateProjByDefMenu();
                CreateProjByDef.MenuId = "MS_CreateProjByDef";
                CreateProjByDef.MenuName = "复制创建目录";
                CreateProjByDef.MenuPosition = enWebMenuPosition.TVProject;
                CreateProjByDef.MenuType = enWebMenuType.Single;

                menuList.Add(CreateProjByDef);

                CreateNewDocMenu CreateNewDoc = new CreateNewDocMenu();
                CreateNewDoc.MenuId = "MS_CreateNewDoc";
                CreateNewDoc.MenuName = "新建文档";
                CreateNewDoc.MenuPosition = enWebMenuPosition.TVProject;
                CreateNewDoc.MenuType = enWebMenuType.Single;

                menuList.Add(CreateNewDoc);

                BatchCreateDocMenu BatchCreateDoc = new BatchCreateDocMenu();
                BatchCreateDoc.MenuId = "MS_BatchCreateDoc";
                BatchCreateDoc.MenuName = "批量创建文档";
                BatchCreateDoc.MenuPosition = enWebMenuPosition.TVProject;
                BatchCreateDoc.MenuType = enWebMenuType.Single;

                menuList.Add(BatchCreateDoc);

                ModiProjAttrMenu ModiProjAttr = new ModiProjAttrMenu();//编辑目录属性菜单
                ModiProjAttr.MenuId = "MS_ModiProjAttr";
                ModiProjAttr.MenuName = "属性编辑";
                ModiProjAttr.MenuPosition = enWebMenuPosition.TVProject;
                ModiProjAttr.MenuType = enWebMenuType.Single;

                menuList.Add(ModiProjAttr);

                startProjectFlowMenu StartProjectFlow = new startProjectFlowMenu();
                StartProjectFlow.MenuId = "MS_StartProjectFlow";
                StartProjectFlow.MenuName = "启动流程";
                StartProjectFlow.MenuPosition = enWebMenuPosition.TVProject;
                StartProjectFlow.MenuType = enWebMenuType.Single;

                menuList.Add(StartProjectFlow);

                ModiProjAttrMenu SearchProj = new ModiProjAttrMenu();//搜索目录菜单
                SearchProj.MenuId = "MS_SearchProj";
                SearchProj.MenuName = "搜索...";
                SearchProj.MenuPosition = enWebMenuPosition.TVProject;
                SearchProj.MenuType = enWebMenuType.Single;

                menuList.Add(SearchProj);

                AddFavoritesMenu AddFavorites = new AddFavoritesMenu();//搜索目录菜单
                AddFavorites.MenuId = "MS_AddFavorites";
                AddFavorites.MenuName = "添加到收藏夹";
                AddFavorites.MenuPosition = enWebMenuPosition.TVProject;
                AddFavorites.MenuType = enWebMenuType.Single;

                menuList.Add(AddFavorites);

                CopyProjectMenu CopyProject = new CopyProjectMenu();//复制目录菜单
                CopyProject.MenuId = "MS_CopyProject";
                CopyProject.MenuName = "复制";
                CopyProject.MenuPosition = enWebMenuPosition.TVProject;
                CopyProject.MenuType = enWebMenuType.Single;

                menuList.Add(CopyProject);

                ProjectPasteMenu ProjectPaste = new ProjectPasteMenu();//粘贴目录菜单
                ProjectPaste.MenuId = "MS_ProjectPaste";
                ProjectPaste.MenuName = "粘贴";
                ProjectPaste.MenuPosition = enWebMenuPosition.TVProject;
                ProjectPaste.MenuType = enWebMenuType.Single;

                menuList.Add(ProjectPaste);

                ProjectPasteShortcutMenu ProjectPasteShortcut = new ProjectPasteShortcutMenu();//粘贴快捷方式到目录菜单
                ProjectPasteShortcut.MenuId = "MS_ProjectPasteShortcut";
                ProjectPasteShortcut.MenuName = "粘贴快捷方式";
                ProjectPasteShortcut.MenuPosition = enWebMenuPosition.TVProject;
                ProjectPasteShortcut.MenuType = enWebMenuType.Single;

                menuList.Add(ProjectPasteShortcut);

                DeleteProjectMenu DeleteProject = new DeleteProjectMenu();//删除目录菜单
                DeleteProject.MenuId = "MS_DeleteProject";
                DeleteProject.MenuName = "删除";
                DeleteProject.MenuPosition = enWebMenuPosition.TVProject;
                DeleteProject.MenuType = enWebMenuType.Single;

                menuList.Add(DeleteProject);

                ////////////////文档列表菜单//////////////////
                OpenShortcutProjectMenu OpenShortcutProject = new OpenShortcutProjectMenu();
                OpenShortcutProject.MenuId = "MS_OpenShortcutProject";
                OpenShortcutProject.MenuName = "转到源目录";
                OpenShortcutProject.MenuPosition = enWebMenuPosition.LVDoc;
                OpenShortcutProject.MenuType = enWebMenuType.Single;

                menuList.Add(OpenShortcutProject);

                startProcessMenu startProcess = new startProcessMenu();
                startProcess.MenuId = "MS_StartProcess";
                startProcess.MenuName = "启动流程";
                startProcess.MenuPosition = enWebMenuPosition.LVDoc;
                startProcess.MenuType = enWebMenuType.Single;

                menuList.Add(startProcess);

                ModiDocAttrMenu ModiDocAttr = new ModiDocAttrMenu();//编辑文档属性菜单
                ModiDocAttr.MenuId = "MS_ModiDocAttr";
                ModiDocAttr.MenuName = "属性编辑";
                ModiDocAttr.MenuPosition = enWebMenuPosition.LVDoc;
                ModiDocAttr.MenuType = enWebMenuType.Single;

                menuList.Add(ModiDocAttr);

                PreviewDocMenu PreviewDoc = new PreviewDocMenu();//编辑文档属性菜单
                PreviewDoc.MenuId = "MS_PreviewDoc";
                PreviewDoc.MenuName = "预览文件";
                PreviewDoc.MenuPosition = enWebMenuPosition.LVDoc;
                PreviewDoc.MenuType = enWebMenuType.Single;

                menuList.Add(PreviewDoc);

                ReplaceDocMenu ReplaceDoc = new ReplaceDocMenu();//替换文件菜单
                ReplaceDoc.MenuId = "MS_ReplaceDoc";
                ReplaceDoc.MenuName = "替换文件";
                ReplaceDoc.MenuPosition = enWebMenuPosition.LVDoc;
                ReplaceDoc.MenuType = enWebMenuType.Single;

                menuList.Add(ReplaceDoc);

                ReferCADMenu ReferCAD = new ReferCADMenu();//编辑文档属性菜单
                ReferCAD.MenuId = "MS_ReferCAD";
                ReferCAD.MenuName = "参照";
                ReferCAD.MenuPosition = enWebMenuPosition.LVDoc;
                ReferCAD.MenuType = enWebMenuType.Single;

                menuList.Add(ReferCAD);

                CopyDocMenu CopyDoc = new CopyDocMenu();//复制目录菜单
                CopyDoc.MenuId = "MS_CopyDoc";
                CopyDoc.MenuName = "复制";
                CopyDoc.MenuPosition = enWebMenuPosition.LVDoc;
                CopyDoc.MenuType = enWebMenuType.Single;

                menuList.Add(CopyDoc);

                MoveDocMenu MoveDoc = new MoveDocMenu();//剪切目录菜单
                MoveDoc.MenuId = "MS_MoveDoc";
                MoveDoc.MenuName = "剪切";
                MoveDoc.MenuPosition = enWebMenuPosition.LVDoc;
                MoveDoc.MenuType = enWebMenuType.Single;

                menuList.Add(MoveDoc);

                DeleteDocMenu DeleteDoc = new DeleteDocMenu();//删除目录菜单
                DeleteDoc.MenuId = "MS_DeleteDoc";
                DeleteDoc.MenuName = "删除";
                DeleteDoc.MenuPosition = enWebMenuPosition.LVDoc;
                DeleteDoc.MenuType = enWebMenuType.Single;

                menuList.Add(DeleteDoc);

                FreeDocMenu FreeDoc = new FreeDocMenu();//删除目录菜单
                FreeDoc.MenuId = "MS_FreeDoc";
                FreeDoc.MenuName = "放弃";
                FreeDoc.MenuPosition = enWebMenuPosition.LVDoc;
                FreeDoc.MenuType = enWebMenuType.Single;

                menuList.Add(FreeDoc);

                return menuList;
            }
            catch (Exception ex)
            {
                CommonController.WebWriteLog(ex.Message);
            }
            return null;
        }


        //加密字符串
        private static string public_key = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQC0w036ClSD0LvxPROMun0u022ROJlZE6P3m+gjq3gpi4n7lo8jhTqMqgccDbVJqnIfMzWS9O3lnlQXWTxJ3B4XJ52FAcriY5brOXUVgBLx5QMHLLd1gtJnmG4i7r4ytgX7XVKRnojR6zca1YnS0lbGGDF1CGllB1riNrdksSQP+wIDAQAB";
        private static string private_key = "MIICXQIBAAKBgQC0w036ClSD0LvxPROMun0u022ROJlZE6P3m+gjq3gpi4n7lo8jhTqMqgccDbVJqnIfMzWS9O3lnlQXWTxJ3B4XJ52FAcriY5brOXUVgBLx5QMHLLd1gtJnmG4i7r4ytgX7XVKRnojR6zca1YnS0lbGGDF1CGllB1riNrdksSQP+wIDAQABAoGAIOyl6lIxXKULZoBKbEqXfIz0GwxlGg1ywyn5mW2lAGQzKMken0ioBnD9xIVWrOlHyhkIvBCyuC0jgfE2Avn93MlB3j0WRuXMFlJpCBlEklMilO9Zgmwl+vTB3VZb8VzdrEEEUBio7LWP/KvSo+IFlNjDTKgAczbLTwAmj4w6g0ECQQDm4yxPdxcU2ywZ7PyjIMM9qnSah9KcrjU8gjEyHsUpgTjhw1cx7Peo+vRiHqxDy1yaSu1BlwRR52pCjKNnl0QhAkEAyGx3NxEIiLk2oXGGbIMZ4P6geC8gYu01BiRNWVf0Yi7+sCH68eUPoI+G5bJ8bvzXpvHjQi0s2OlRfct/qtPQmwJBALa+2DONbxdy4lUi3lO/esk0QVaOaoTY3gomggnJkQRo4zzOABXkGaIF/6gp3u9J5uG4rFFd1m19XP2Pk0ZK1AECQBYilJAKW4zuF7CA3z3AxOzqckKTwdnrJL4G6FwDsMPfONWvCw4IJE+xSk64BbIkTpTrhhPa9WcHba6c+P6e4h0CQQDWeGMMpkqPG/w4afNCGmvRnM8vNkGUAmDGvCsfkTIDijpKl5SD55hPHsWE5rsv1TLUpkWtrFBcg61bHwMUP3cv";


        
        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="encrypStr"></param>
        /// <returns></returns>
        public static string ProcessRequest(string encrypStr)
        {
            string e = "";
            try
            {
                RSACryptoService rsa = new RSACryptoService(private_key,public_key);
                e = rsa.Decrypt(encrypStr.Replace("%2B", "+"));
            }
            catch (Exception ex)
            {
                CommonController.WebWriteLog(ex.Message);
            }
            return e;
        }



        
        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="decrypStr"></param>
        /// <returns></returns>
         public static string enProcessRequest(string decrypStr)
        {
            string e = "";
            try
            {
                RSACryptoService rsa = new RSACryptoService(private_key,public_key);
                e = rsa.Encrypt(decrypStr).Replace("+","%2B");
            }
            catch (Exception ex)
            {
                CommonController.WebWriteLog(ex.Message);
            }
            return e;
        }
    }


}
