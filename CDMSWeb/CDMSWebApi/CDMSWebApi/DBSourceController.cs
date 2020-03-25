using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using AVEVA.CDMS.Server;
using System.Web.Security;
using System.Threading;

namespace AVEVA.CDMS.WebApi
{
    /// <summary>  
    /// DBSource操作类
    /// </summary>  
    public class DBSourceController : Controller
    {

        /// <summary>
        /// 索引
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        //应用的本地地址
        public static String Server_MapPath = string.Empty;

        /// <summary>
        /// 用户信息表
        /// </summary>
        //public static Hashtable UserInfoList = new Hashtable();
        internal static List<UserInfo> UserInfoList = new List<UserInfo>();

        /// <summary>
        /// 静态数据管理器
        /// </summary>
        private static DBManager mdbManager;

        /// <summary>
        /// 数据刷新间隔，单位是分钟
        /// </summary>
        private static int DBRefreshInterval = 10;

        /// <summary>
        /// 静态数据管理器刷新时间
        /// </summary>
        private static DateTime DBManagerRefreshTime = DateTime.Now;

        /// <summary>
        /// DBManager 全局变量
        /// </summary>
        public static DBManager dbManager
        {
            get
            {
                if (mdbManager == null) mdbManager = new DBManager();
                if (mdbManager != null && mdbManager.DBSourceList != null && mdbManager.DBSourceList.Count() > 0)
                {
                    mdbManager.DBSourceList[0].GUIDLogin(CommonController.mGetString_mStrs());
                    if (mdbManager.shareDBManger != null) mdbManager.shareDBManger.DBSourceList[0].GUIDLogin(CommonController.mGetString_mStrs());
                }
                return mdbManager;
            }
        }


        /// <summary>
        /// 强制刷新DBManager
        /// </summary>
        /// <returns></returns>
        //internal static bool RefreshShareDBManager()
        public static bool RefreshShareDBManager()
        {
            try
            {
                mdbManager = new DBManager();
                return true;
            }
            catch { }
            return false;
        }


        /// <summary>
        /// 处理从客户端发送过来的刷新数据源命令
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="RefreshInterval">强制刷新DBManager的时间间隔，单位是分钟，默认值10，最小值是1</param>
        /// <returns></returns>
        public static JObject refreshDBSource(string sid, string RefreshInterval = "")
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

                int intRefreshInterval = Convert.ToInt32(RefreshInterval);
                if (intRefreshInterval <= 0)
                {
                    intRefreshInterval = DBRefreshInterval;
                }

                //每间隔一分钟可以强制刷新一次DBManager,防止过度刷新
                if (DBManagerRefreshTime.AddMinutes(intRefreshInterval) < DateTime.Now)
                {
                    DBManagerRefreshTime = DateTime.Now;
                    mdbManager = new DBManager();
                }

                DBSource newDB = RefreshDBSource(sid);
                if (newDB != null)
                {
                    reJo.success = true;
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
        /// 不登录刷新数据源
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="isLogin"></param>
        /// <returns></returns>
        //public static DBSource RefreshDBSource(string sid)
        //{
        //    return 
        //}

        ///// <summary>
        ///// 不登录刷新数据源
        ///// </summary>
        ///// <param name="sid">连接秘钥</param>
        ///// <param name="isLogin"></param>
        ///// <returns></returns>
        //public static DBSource RefreshDBSource(string sid, bool isLogin,string userName,string psw)
        //{
        //    try
        //    {
        //        //客户端创建一个新DBsource对象
        //        User curUser = (User)UserInfoList[sid];

        //        //删除旧对象
        //        UserInfoList.Remove(sid);


        //        //创建新DBSource
        //        DBSource dbs = curUser.dBSource.NewDBSource(isLogin);
        //        bool loginBool = dbs.Login(userName, psw);

        //        curUser = dbs.LoginUser;
        //        UserInfoList.Add(sid, curUser);

        //        return dbs;
        //    }
        //    catch { return null; }
        //}

        /// <summary>
        /// 强制刷新DBSource
        /// </summary>
        /// <returns></returns>
        public static DBSource RefreshDBSource(string sid)
        {
            try
            {
                //客户端创建一个新DBsource对象
                UserInfo userinfo = UserInfoList.Find(u => u.sid == sid);
                if (userinfo == null) { return null; }

                User curUser = userinfo.user;

                //创建新DBSource
                DBSource dbs = curUser.dBSource.NewDBSource();
                curUser = dbs.LoginUser;

                userinfo.user = curUser;

                return dbs;
            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
                return null;
            }
        }

        /// <summary>
        /// 根据sid返回用户
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <returns></returns>
        public static User GetUserBySid(string sid)
        {
            try
            {
                //User curUser = (User)UserInfoList[sid];
                UserInfo userinfo = UserInfoList.Find(u => u.sid == sid);
                if (userinfo == null) { return null; }

                return userinfo.user;
            }
            catch
            {
                return null;
            }
        }

        //线程锁 
        private static Mutex muxConsole = new Mutex();
        /// <summary>
        /// 获取当前用户
        /// </summary>
        /// <returns></returns>
        public static User GetCurrentUser(string sid)
        {
            try
            {

                //获取用户登录信息
                if (string.IsNullOrEmpty(sid)) return null;

                UserInfo userinfo = UserInfoList.Find(u => u.sid == sid);
                if (userinfo == null)
                {
                    //CommonController.WebWriteLog("Login Error ! UserInfoList count" + UserInfoList.Count + ",sid:" + sid);
                    #region 2020-2-21 如果内存里面找不到，就到数据表里面去找
                    //2020-2-21 如果内存里面找不到，就到数据表里面去找
                    string SQL = "select o_userno from WAPI_UserAuths where o_sid='" + sid + "'";
                    string[] strArry = DBSourceController.dbManager.DBSourceList[0].DBExecuteSQL(SQL);

                    if (strArry.Length > 0 && (!string.IsNullOrEmpty(strArry[0])))
                    {
                        int uId = Convert.ToInt32(strArry[0]);
                        User user = DBSourceController.dbManager.DBSourceList[0].GetUserByID(uId);
                        if (user != null)
                        {
                            userinfo = new UserInfo();
                            userinfo.sid = sid;
                            userinfo.user = user;
                            UserInfoList.Add(userinfo);
                            //CommonController.WebWriteLog("数据表查找到了sid:" + sid);
                        }
                        else
                        {
                            //CommonController.WebWriteLog("数据表查找不到sid:" + sid);
                            return null;
                        }
                    }
                    else
                    {
                        //CommonController.WebWriteLog("数据表查找不到sid:" + sid);
                        return null;
                    }
                    #endregion


                }


                //线程锁 
                muxConsole.WaitOne();

                //客户端每10分钟创建一个新DBsource对象
                //User curUser = (User)UserInfoList[sid];
                User curUser = userinfo.user;

                DateTime culoginTime = curUser.LoginTime == null ? DateTime.Now.AddMinutes(-20) : curUser.LoginTime;

                if (culoginTime.AddMinutes(DBRefreshInterval) < DateTime.Now)
                {
                    //小钱2019-1-2 删除后刷新数据源和DBManager，会导致不能添加sid,这里改为删除后马上添加到UserInfoList

                    //每十分钟刷新一次DBManager,保持和Explorer端的同步
                    if (DBManagerRefreshTime.AddMinutes(DBRefreshInterval) < DateTime.Now)
                    {
                        DBManagerRefreshTime = DateTime.Now;

                        //删除登录超过24小时的sid记录
                        string SQL = "delete from WAPI_UserAuths where dateadd(day,1,o_logintime)<getdate()";
                        string[] strArry = DBSourceController.dbManager.DBSourceList[0].DBExecuteSQL(SQL);

                        mdbManager = new DBManager();
                    }

                    //创建新DBSource
                    DBSource dbs = curUser.dBSource.NewDBSource();
                    curUser = dbs.LoginUser;

                    userinfo.user = curUser;

                    ////删除旧对象
                    //UserInfoList.Remove(sid);

                    //UserInfoList.Add(sid, curUser);

                    //登录24小时以上账户，则清除
                    List<UserInfo> delUserInfolist = new List<UserInfo>();
                    foreach (UserInfo uifo in UserInfoList)
                    {
                        User u = uifo.user;
                        DateTime loginTime = u.LoginTime;
                        if (loginTime.AddDays(1) < DateTime.Now)
                        {
                            delUserInfolist.Add(uifo);
                        }
                    }
                    foreach (UserInfo uifo in delUserInfolist)
                    {
                        UserInfoList.Remove(uifo);
                    }

                }


                //返回
                curUser.dBSource.ProgramRun = false;
                //curUser.dBSource.ProgramRun = true;

                //解锁
                muxConsole.ReleaseMutex();

                return curUser;
            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
                //解锁
                muxConsole.ReleaseMutex();
            }

            return null;
        }


        /// <summary>
        /// 登出数据源
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <returns>返回一个JSON对象</returns>
        [HttpPost]
        public static JObject Logout(string UserName)
        {
            ExReJObject reJo = new ExReJObject();

            try
            {
                UserInfo rmUifo = null;

                foreach (UserInfo uifo in UserInfoList)
                {
                    User u = uifo.user;
                    if (u.O_username == UserName)
                    {
                        rmUifo = uifo;
                        break;
                    }
                }

                if (rmUifo != null)
                {
                    UserInfoList.Remove(rmUifo);
                }

                reJo.success = true;
                return reJo.Value;
            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
            }
            return reJo.Value;
        }

        /// <summary>
        /// 登出数据源
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <returns>返回一个JSON对象</returns>
        [HttpPost]
        public static JObject LogoutBySid(string sid)
        {
            ExReJObject reJo = new ExReJObject();

            try
            {

                UserInfo userinfo = UserInfoList.Find(u => u.sid == sid);
                if (userinfo == null) { return reJo.Value; }

                UserInfoList.Remove(userinfo);

                //删除登录的sid记录
                string SQL = "delete from WAPI_UserAuths where o_sid='" + sid + "'";
                string[] strArry = DBSourceController.dbManager.DBSourceList[0].DBExecuteSQL(SQL);

                reJo.success = true;
                return reJo.Value;
            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
            }
            return reJo.Value;
        }

        /// <summary>
        /// 登录数据源
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="Password">用户密码</param>
        /// <param name="hostname">登录主机名</param>
        /// <returns>返回一个JSON对象</returns>
        [HttpPost]
        public static JObject Login(string UserName, string Password, string hostname)
        {

            bool success = false;
            JObject errors = new JObject();
            JObject joMsg = new JObject();
            try
            {

                //环境正常
                if (DBSourceController.dbManager == null) return null;
                if (DBSourceController.dbManager.DBSourceList == null) return null;
                if (DBSourceController.dbManager.DBSourceList.Count() == 0) return null;

                if (string.IsNullOrEmpty(Password)) return null;

                //登录
                bool loginFlag = DBSourceController.dbManager.DBSourceList[0].Login(UserName, Password, hostname);
                if (loginFlag == false)
                {

                    //登录失败
                    errors.Add("Password", DBSourceController.dbManager.DBSourceList[0].LastError);
                    errors.Add("errorMsg", DBSourceController.dbManager.DBSourceList[0].LastError); //"登录密码不正确！");
                }
                else
                {

                    DBManager dbm = new DBManager();
                    bool newDbmLoginFlag = dbm.DBSourceList[0].Login(UserName, Password, hostname);

                    string strerr = dbm.DBSourceList[0].LastError;

                    if (newDbmLoginFlag == false)
                    {
                        //登录失败
                        errors.Add("Password", DBSourceController.dbManager.DBSourceList[0].LastError);
                        errors.Add("errorMsg", DBSourceController.dbManager.DBSourceList[0].LastError);
                    }

                    else
                    {
                        //登录用户
                        User user = dbm.DBSourceList[0].LoginUser; //.GetUserByCode(UserName);

                        //获取User GUID
                        string guid = getGuid(user.KeyWord);


                        //去掉原来的信息
                        try
                        {
                            UserInfo userinfo = UserInfoList.Find(u => u.sid == guid);
                            if (userinfo != null)
                            {
                                userinfo.user = user;
                            }
                            else
                            {
                                userinfo = new UserInfo();
                                userinfo.sid = guid;
                                userinfo.user = user;
                                UserInfoList.Add(userinfo);
                            }
                        }
                        catch (Exception e)
                        {
                            CommonController.WebWriteLog(e.Message);
                        }

                        //2020-2-21 把登录sid保存到数据表"WAPI_UserAuths"
                        //INSERT INTO Persons (LastName, Address) VALUES ('Wilson', 'Champs-Elysees')
                        string SQL = "insert into WAPI_UserAuths (o_userno,o_type,o_sid,o_logintime) VALUES (" +
                            user.ID.ToString() + "," +
                            "'web'" + "," +
                            "'" + guid + "'" + "," +
                            "GETDATE()" +
                            ")";
                        string[] strArry = dbm.DBSourceList[0].DBExecuteSQL(SQL);

                        //登录成功
                        success = true;
                        joMsg.Add("guid", guid);  //返回关键字
                        joMsg.Add("username", UserName);
                        joMsg.Add("userdesc", user.Description);

                    }

                }
            }
            catch (Exception ex)
            {
                errors.Add("Password", ex.Message);
                errors.Add("errorMsg", ex.Message);
                CommonController.WebWriteLog(ex.Message);
            }

            JObject reJo = new JObject { 
            new JProperty("success",success)
            };
            if (errors != null && errors.HasValues)
            {
                reJo.Add(new JProperty("errors", errors));
            }
            reJo.Add(new JProperty("msg", joMsg));

            return reJo;
        }

        /// <summary>
        /// 获取guid
        /// </summary>
        /// <param name="userKeyword"></param>
        /// <returns></returns>
        public static string getGuid(string userKeyword)
        {

            //获取User GUID
            string guid = AVEVA.CDMS.Server.CDMS.cryptoStr.Encrypting(DateTime.Now.Second.ToString() + userKeyword + DateTime.Now.Millisecond.ToString());
            if (!string.IsNullOrEmpty(guid) && guid.Length > 5)
                guid = guid.Substring(0, guid.Length - 2);
            else
                guid = userKeyword;


            //去掉特殊字符串
            guid = guid.Replace("/", "");
            guid = guid.Replace("+", "");
            guid = guid.Replace("-", "");
            guid = guid.Replace("\\", "");

            return guid;
        }
        /// <summary>
        /// 添加新sid
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <returns></returns>
        public static JObject GetNewSid(string sid)
        {
            ExReJObject reJo = new ExReJObject();
            try
            {

                //客户端创建一个新DBsource对象

                UserInfo userinfo = UserInfoList.Find(u => u.sid == sid);
                if (userinfo == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

                User curUser = userinfo.user;
                if (curUser == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

                //获取User GUID
                string guid = getGuid(curUser.KeyWord);


                //创建新DBSource
                DBSource dbs = curUser.dBSource.NewDBSource();
                curUser = dbs.LoginUser;
                // UserInfoList.Add(guid, curUser);


                //去掉原来的信息
                try
                {
                    userinfo.user = curUser;
                }
                catch (Exception e)
                {
                    CommonController.WebWriteLog(e.Message);
                    reJo.msg = e.Message;
                    return reJo.Value;
                }



                //登录成功
                JObject joMsg = new JObject();
                reJo.success = true;
                joMsg.Add("guid", guid);  //返回关键字
                joMsg.Add("username", curUser.Code);
                reJo.data = new JArray(joMsg);

                return reJo.Value;


            }
            catch (Exception ex) { reJo.msg = ex.Message; }

            return reJo.Value;
        }

        /// <summary>
        /// 获取菜单列表
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="ParentId">父目录的Keyword</param>
        /// <param name="ProjectList">选中的Project列表,用","分隔</param>
        /// <param name="DocList">选中的Doc列表,用","分隔</param>
        /// <param name="Position">菜单的位置，有四个选项：(目录树)TVProject = 0,(文件列表里的目录)LVProject = 1,(文件)LVDoc = 2,(文件列表里的多个文档或目录)LVMultiSelected = 3</param>
        /// <param name="TvPosition">调用菜单的页面，有五个选项：(默认)Default = 0,(文档管理页)TVDocument = 1,(逻辑目录页)TVLogicProject = 2,(个人工作台页)TVUserWorkSpace = 3,(全局查询页)TVGlobalSearch = 4,(个人查询页)TVUserSearch = 5</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时，data包含多个JObject，每个JObject包含：</para><para>"Id"：菜单Id,"Name":菜单名，"State"：菜单状态，（禁用，启用），"Position"：菜单位置</para>
        /// <para>例子：</para>
        /// <code>
        /// {
        ///"success": true,
        ///"total": 0,
        /// "msg": "",
        /// "data": [
        /// {
        ///       "Id": null,
        ///       "Name": "新建目录",
        ///       "State": "Enabled",
        ///       "Position": 0
        ///     },
        ///     {
        ///       "Id": null,
        ///       "Name": "新建文档",
        ///       "State": "Enabled",
        ///       "Position": 0
        ///     }
        ///   ]
        /// }</code>
        /// </returns>
        public static JObject GetMenuList(string sid, string ParentId, string ProjectList, string DocList, string Position, string TvPosition)
        {
            //获取菜单

            //定义菜单
            List<ExWebMenu> menuList = new List<ExWebMenu>();
            List<ExWebMenu> reMenuList = new List<ExWebMenu>();


            ExReJObject reJo = new ExReJObject();
            try
            {
                if (string.IsNullOrEmpty(sid))
                { reJo.msg = "登录验证失败！请尝试重新登录！"; }
                else
                {


                    //登录用户
                    User curUser = DBSourceController.GetCurrentUser(sid);
                    if (curUser == null)
                    {
                        reJo.msg = "登录验证失败！请尝试重新登录！";
                        return reJo.Value;
                    }

                    DBSource dbsource = curUser.dBSource; // DBSourceController.dbManager.shareDBManger.DBSourceList[0].ShareLoaclDBSource;//DBSource dbsource = curUser.dBSource.NewDBSource();//
                    if (dbsource == null)
                    {
                        reJo.msg = "登录验证失败！请尝试重新登录！";
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(TvPosition)) TvPosition = "0";

                        //登录并获取dbsource成功
                        JArray reja = new JArray();
                        string seleObjList = "";
                        if (!string.IsNullOrEmpty(ProjectList)) seleObjList = ProjectList;
                        else if (!string.IsNullOrEmpty(DocList)) seleObjList = DocList;

                        //获取目录或文档对象列表
                        List<Object> objList = new List<object>();
                        string[] strArray = (string.IsNullOrEmpty(seleObjList) ? "" : seleObjList).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string strObj in strArray)
                        {
                            object obj = dbsource.GetObjectByKeyWord(strObj);

                            if (obj == null) return null;

                            //转换JSON字符串
                            objList.Add(obj);
                        }

                        JObject joResult = new JObject();
                        bool IsDocList = false;
                        bool IsProjList = false;
                        List<Doc> seleDocList = new List<Doc>();
                        List<Project> seleProjectList = new List<Project>();
                        foreach (Object obj in objList)
                        {
                            if (obj is Doc)
                            {
                                IsDocList = true;
                                seleDocList.Add((Doc)obj);
                            }
                            else if (obj is Project)
                            {
                                IsProjList = true;
                                seleProjectList.Add((Project)obj);
                            }
                        }

                        //遍历插件，获取所有菜单
                        foreach (Type type in AVEVA.CDMS.WebApi.EnterPointController.TypeList)
                        {
                            try
                            {
                                //取得创建菜单的方法
                                System.Reflection.MethodInfo method = type.GetMethod("CreateNewExMenu");
                                if (method != null)
                                {
                                    object getObj = new object();
                                    object[] parameters = new object[0];
                                    List<ExWebMenu> getList = (List<ExWebMenu>)method.Invoke(getObj, parameters);
                                    if ((getList != null) && (getList.Count > 0))
                                    {
                                        foreach (ExWebMenu menu in getList)
                                        {
                                            menuList.Add(menu);
                                        }
                                    }
                                }

                            }
                            catch (Exception e)
                            {
                                CommonController.WebWriteLog(e.Message);
                            }
                        }

                        //遍历所有菜单列表，根据客户端传送过来的条件，判断是否显示菜单
                        enWebMenuPosition clientPosition = (enWebMenuPosition)Enum.Parse(typeof(enWebMenuPosition), Position);//获取客户端传递的菜单位置
                        enWebTVMenuPosition clientTvPosition = (enWebTVMenuPosition)Enum.Parse(typeof(enWebTVMenuPosition), TvPosition);
                        for (int i = 0; i < menuList.Count; i++)
                        {
                            ExWebMenu menu = menuList[i];
                            if (menu != null)
                            {
                                //把客户端传送过来的信息，推送到菜单项
                                menu.SelDocList = seleDocList;
                                menu.SelProjectList = seleProjectList;
                                //传递登录的用户
                                //menu.LoginUser = curUser;
                                menu.Sid = sid;

                                //传递菜单的位置
                                menu.TVMenuPositon = clientTvPosition;

                                //enWebMenuState menuState = menu.MeasureMenuInitState();
                                //判断是否需要显示菜单项
                                enWebMenuState menuState = menu.MeasureMenuState();
                                if ((menuState == enWebMenuState.Enabled || menuState == enWebMenuState.Disabled) &&
                                    menu.MenuPosition == clientPosition)
                                //&&
                                // (menu.TVMenuPositon == clientTvPosition || clientTvPosition==enWebTVMenuPosition.Default
                                //    || menu.TVMenuPositon == enWebTVMenuPosition.Default))
                                {

                                    menu.MenuState = menuState;
                                    reMenuList.Add(menu);//如果经过判断后的菜单向，不是隐藏状态，就推送到返回菜单列表
                                }
                            }
                        }


                        //返回需要显示的菜单
                        foreach (ExWebMenu menu in reMenuList)
                        {
                            string menuState = "Hide";
                            if (menu.MenuState == enWebMenuState.Disabled) menuState = "Disabled";
                            else if (menu.MenuState == enWebMenuState.Enabled) menuState = "Enabled";

                            if (menu.MenuState != enWebMenuState.Hide)
                            {
                                JObject menuJo = new JObject(new JProperty("Id", menu.MenuId),
                                    new JProperty("Name", menu.MenuName),
                                    new JProperty("State", menuState),
                                    new JProperty("Position", menu.MenuPosition)
                                );
                                reja.Add(menuJo);
                            }
                        }

                        reJo.success = true;
                        reJo.data = reja;
                    }

                }
            }
            catch (Exception ex2)
            {
                reJo.msg = ex2.Message;
                CommonController.WebWriteLog(ex2.Message);
            }

            return reJo.Value;

        }


        /// <summary>
        /// 运行查询语句,只可以运行select
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="SQL">查询语句，必须包含select，不可以包含 insert+into , update+set , delete+from等字符</param>
        /// <returns>数据包含："str1": Dockeyword1,"str2": Dockeyword2</returns>
        public static JObject DBSelectSQL(string sid, string SQL)
        {
            ExReJObject reJo = new ExReJObject();
            try
            {

                //客户端创建一个新DBsource对象
                //User curUser = (User)UserInfoList[sid];
                UserInfo userinfo = UserInfoList.Find(u => u.sid == sid);
                if (userinfo == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }
                User curUser = userinfo.user;
                if (curUser == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

                string lSql = SQL.ToLower();

                //过滤掉插入语句，更新和删除语句
                if ((lSql.IndexOf("insert") >= 0 && lSql.IndexOf("into") > 0) ||
                    (lSql.IndexOf("update") >= 0 && lSql.IndexOf("set") > 0) ||
                    (lSql.IndexOf("delete") >= 0 && lSql.IndexOf("from") > 0))
                {
                    reJo.msg = "查询语句带有非法关键词！";
                    return reJo.Value;
                }

                if (lSql.IndexOf("select") < 0 && lSql.IndexOf("from") < 0)
                {
                    reJo.msg = "请输入查询语句！";
                    return reJo.Value;
                }

                string[] strArry = curUser.dBSource.DBExecuteSQL(SQL);

                //登录成功
                JObject joMsg = new JObject();
                reJo.success = true;
                int index = 1;
                foreach (string strItem in strArry)
                {
                    joMsg.Add("str" + index, strItem);  //返回关键字
                    index = index + 1;
                }
                reJo.data = new JArray(joMsg);

                return reJo.Value;

            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
            }
            return reJo.Value;
        }

        /// <summary>
        /// 查询专用
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="AJson">json字符串，包含子项：type：对象类型，action：操作类型，keyword（选填）：对象关键字，select：查询返回的对象字段，page：选取第几页，pagesize：每页记录数</param>
        /// <returns></returns>
        public static JObject GetAJson(string sid, string AJson)
        {
            ExReJObject reJo = new ExReJObject();
            try
            {

                //客户端创建一个新DBsource对象
                UserInfo userinfo = UserInfoList.Find(u => u.sid == sid);
                if (userinfo == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }
                User curUser = userinfo.user;
                if (curUser == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(AJson);

                JToken jt = jaAttr[0];

                string type = GetJPropertyValue(jt, "type");
                string action = GetJPropertyValue(jt, "action");
                string select = GetJPropertyValue(jt, "select");    //查询返回的对象字段
                string page = GetJPropertyValue(jt, "page");        //选取第几页
                string pagesize = GetJPropertyValue(jt, "pagesize");    //每页记录数

                string keyword = GetJPropertyValue(jt, "keyword");

                JObject joData = new JObject();

                //这里暂时只有查询Doc的功能
                if (type == "Doc" && keyword != null)
                {
                    Doc doc = curUser.dBSource.GetDocByKeyWord(keyword);
                    if (doc != null)
                    {
                        joData.Add(new JProperty("Keyword", doc.KeyWord));
                        joData.Add(new JProperty("Title", doc.ToString));
                        if (!string.IsNullOrEmpty(select))
                        {
                            string[] selAry = select.Split(new char[] { ',' });
                            foreach (string selitem in selAry)
                            {
                                if (selitem == "WorkingPath")
                                {
                                    joData.Add(new JProperty("WorkingPath", doc.WorkingPath));
                                }
                            }
                        }
                    }
                }

                reJo.data = new JArray(joData);
                reJo.success = true;

                return reJo.Value;

            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
            }
            return reJo.Value;
        }

        internal static string GetJPropertyValue(JToken jt, string name)
        {
            foreach (JProperty jp in jt)
            {
                if (jp.Name == name)
                {
                    return jp.Value.ToString();
                }
            }
            return null;
        }


        /// <summary>
        /// 返回数据源对象
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public static JArray GetDBSource()
        {
            try
            {
                JArray ja = new JArray((JObject)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(DBSourceController.dbManager.DBSourceList[0])));

                //返回工作流其他页XML
                return CommonController.TranStringToJson(ja);
            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
            }

            //return null;
            return CommonController.NullToJson("");

        }


    }

    internal class UserInfo
    {
        public string sid;
        public User user;
    }
}
