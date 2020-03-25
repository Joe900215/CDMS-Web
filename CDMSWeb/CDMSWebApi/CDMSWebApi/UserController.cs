using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AVEVA.CDMS.Server;


namespace AVEVA.CDMS.WebApi
{
    /// <summary>  
    /// 用户操作类
    /// </summary>  
    public class UserController : Controller
    {



        /// <summary>
        /// 返回用户列表（不包括子用户组的用户）
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="page">要访问的页数，默认是1</param>
        /// <param name="limit">每一页的记录数，默认是50</param>
        /// <param name="Group">可选，用户组Keyword</param>
        /// <param name="filter">可选，筛选字符串，例如:"陈"</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时，data包含多个JObject，每个JObject里面包含参数"text":用户代码加描述，"id":用户KeyWord</para>
        /// <para>例子：</para>
        /// </returns>
        public static JObject GetUserList(string sid, string page, string limit, string Group, string filter)
        {
            ExReJObject reJo = new ExReJObject();
            try
            {

                page = page ?? "1";
                limit = limit ?? "50";
                string group = Group ?? "";
                filter = filter ?? "";
                if (string.IsNullOrEmpty(page))
                {
                    reJo.msg = "错误的提交数据。";
                }
                else
                {

                    if (sid == null) return CommonController.SidError();
                    page = (Convert.ToInt32(page) - 1).ToString();



                    //登录用户
                    User curUser = DBSourceController.GetCurrentUser(sid);
                    if (curUser == null)
                    {
                        reJo.msg = "登录验证失败！请尝试重新登录！";
                        return reJo.Value;
                    }

                    //获取对象
                    DBSource dbsource = curUser.dBSource;
                    if (dbsource == null)
                    {
                        reJo.msg = "登录验证失败！请尝试重新登录！";
                        return reJo.Value;
                    }

                    string GroupKeyword = group;
                    List<User> allUserList;
                    JObject joResult = new JObject();
                    JArray jaUserList = new JArray();

                    filter = filter.Trim().ToUpper();

                    if (!(String.IsNullOrEmpty(GroupKeyword) || GroupKeyword == "Root"))
                    {
                        Object obj = null;
                        obj = dbsource.GetObjectByKeyWord(GroupKeyword);

                        if (obj == null) return new JObject();
                        AVEVA.CDMS.Server.Group curGroup = new AVEVA.CDMS.Server.Group();
                        if (obj is AVEVA.CDMS.Server.Group)
                            curGroup = (AVEVA.CDMS.Server.Group)obj;

                        List<User> UserList = new List<User>();

                        allUserList = curGroup.UserList;//.AllUserList;
                    }
                    else
                    {
                        allUserList = dbsource.AllUserList;
                    }

                    //筛选用户
                    IEnumerable<User> filterIE = (from u in allUserList where (u.ToString.ToUpper()).Contains(filter) select u);
                    reJo.total = filterIE.Count();
                    int ShowNum = Convert.ToInt32(limit);
                    int CurPage = Convert.ToInt32(page);
                    List<User> resultUserList = filterIE.Skip(CurPage * ShowNum).Take(ShowNum).ToList();
                    foreach (User user in resultUserList)
                        jaUserList.Add(new JObject(new JProperty("text", user.ToString), new JProperty("id", user.KeyWord)));

                    reJo.data = jaUserList;
                    reJo.success = true;
                }

            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
                reJo.msg = e.Message;
            }
            return reJo.Value;
        }

        /// <summary>
        /// 返回所有用户列表（包括子用户组的用户）
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="page">要访问的页数，默认是1</param>
        /// <param name="limit">每一页的记录数，默认是50</param>
        /// <param name="Group">可选，用户组Keyword</param>
        /// <param name="filter">可选，筛选字符串，例如:"陈"</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时，data包含多个JObject，每个JObject里面包含参数"text":用户代码加描述，"id":用户KeyWord</para>
        /// <para>例子：</para>
        /// <code>
        /// {
        ///  "success": true,
        ///  "total": 63,
        ///  "msg": "",
        ///  "data": [
        ///    {
        ///      "text": "admin__administrator",
        ///      "id": "GJEPCMSU1"      //用户关键字
        ///    },
        ///    {
        ///      "text": "baifeng__白锋",
        ///      "id": "GJEPCMSU10186"
        ///    }
        ///  ]
        /// }
                /// </code>
        /// </returns>
        public static JObject GetAllUserList(string sid, string page, string limit, string Group, string filter)
        {
            ExReJObject reJo = new ExReJObject();
            try
            {

                page = page ?? "1";
                limit = limit ?? "50";
                string group = Group ?? "";
                filter = filter ?? "";
                if (string.IsNullOrEmpty(page))
                {
                    reJo.msg = "错误的提交数据。";
                }
                else
                {

                    if (sid == null) return CommonController.SidError();
                    page = (Convert.ToInt32(page) - 1).ToString();



                    //登录用户
                    User curUser = DBSourceController.GetCurrentUser(sid);
                    if (curUser == null)
                    {
                        reJo.msg = "登录验证失败！请尝试重新登录！";
                        return reJo.Value;
                    }

                    //获取对象
                    DBSource dbsource = curUser.dBSource;
                    if (dbsource == null)
                    {
                        reJo.msg = "登录验证失败！请尝试重新登录！";
                        return reJo.Value;
                    }

                    string GroupKeyword = group;
                    List<User> allUserList;
                    JObject joResult = new JObject();
                    JArray jaUserList = new JArray();

                    filter = filter.Trim().ToUpper();

                    if (!(String.IsNullOrEmpty(GroupKeyword) || GroupKeyword == "Root"))
                    {
                        Object obj = null;
                        obj = dbsource.GetObjectByKeyWord(GroupKeyword);

                        if (obj == null) obj = dbsource.GetGroupByName(GroupKeyword);

                        if (obj == null) return new JObject();
                        AVEVA.CDMS.Server.Group curGroup = new AVEVA.CDMS.Server.Group();
                        if (obj is AVEVA.CDMS.Server.Group)
                            curGroup = (AVEVA.CDMS.Server.Group)obj;

                        List<User> UserList = new List<User>();

                        allUserList = curGroup.AllUserList;
                    }
                    else
                    {
                        allUserList = dbsource.AllUserList;
                    }

                    //去除重复
                    allUserList = allUserList.GroupBy(item => item.KeyWord).Select(item => item.First()).ToList();

                    //筛选用户
                    IEnumerable<User> filterIE = (from u in allUserList where (u.ToString.ToUpper()).Contains(filter) select u);
                    reJo.total = filterIE.Count();
                    int ShowNum = Convert.ToInt32(limit);
                    int CurPage = Convert.ToInt32(page);
                    List<User> resultUserList = filterIE.Skip(CurPage * ShowNum).Take(ShowNum).ToList();
                    foreach (User user in resultUserList)
                        jaUserList.Add(new JObject(new JProperty("text", user.ToString), new JProperty("id", user.KeyWord)));

                    reJo.data = jaUserList;
                    reJo.success = true;
                }

            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
                reJo.msg = e.Message;
            }
            return reJo.Value;
        }

        /// <summary>
        /// 返回用户组对象下的子用户组列表
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="node">用户组节点关键字</param>
        /// <param name="GroupType">用户组类别，默认为"Org"(机构用户组)，也可以是"Project"(项目用户组)</param>
        /// <param name="Filter">可选，筛选字符串，例如:"陈"</param>
        /// <returns>
        /// <para>返回JObject,有四个参数：success:是否操作成功,total:记录总数,msg:错误消息,data(JArray字符串):返回的数据。</para>
        /// <para>操作成功时，success返回true,total返回记录总数;操作失败时在msg里返回错误消息</para>
        /// <para>操作成功时，data包含多个JObject，每个JObject里面包含参数"text":用户组描述，"id"：用户组KeyWord，"leaf"：是否有子用户组</para>
        /// <para>例子：</para>
        /// <code>
        /// {
        ///  "success": true,
        ///  "total": 2,
        ///  "msg": "",
        ///  "data": [
        ///    {
        ///      "text": "adminVicepresident__行政副院长",   //用户组的代码和描述
        ///      "id": "GJEPCMSG433",   //用户组Keyword
        ///      "leaf": true           //是否没有子用户组，为true是
        ///    },
        ///    {
        ///      "text": "ChiefEngineer__总工程师",
        ///      "id": "GJEPCMSG434",
        ///      "leaf": true
        ///    }
        ///  ]
        /// }
        /// </code>
        /// </returns>
        public static JObject GetUserGroupList(string sid, string node, string GroupType, string Filter)
        {
            ExReJObject reJo = new ExReJObject();
            try
            {
                string keyword = node ?? "Root";
                string groupType = GroupType ?? "Org";

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

                //获取对象
                DBSource dbsource = curUser.dBSource;
                if (dbsource == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

                //if (groupType == "Org")
                enGroupType o_GroupType = enGroupType.Organization;
                if (groupType == "Project")
                    o_GroupType = enGroupType.Project;

                    JArray jaGroupList = new JArray();
                if (keyword == "Root")
                {
                    //组织机构用户组
                    foreach (AVEVA.CDMS.Server.Group groupOrg in dbsource.AllGroupList)
                    {
                        if ((groupOrg.ParentGroup == null) && (groupOrg.O_grouptype == o_GroupType))// enGroupType.Organization))
                        {
                            //过滤用户组
                            if (!string.IsNullOrEmpty(Filter) && groupOrg.ToString.IndexOf(Filter) < 0)
                            {
                                continue;
                            }

                            JObject joGroup = new JObject();
                            int childCount = groupOrg.GroupList.Count + groupOrg.ChildGroupList.Count;
                            bool flag = false;
                            if (childCount <= 0)
                                flag = true;
                            joGroup = new JObject(new JProperty("text", groupOrg.ToString), new JProperty("id", groupOrg.KeyWord), new JProperty("leaf", flag));
                            jaGroupList.Add(joGroup);
                        }

                    }
                }
                else
                {
                    Object obj = null;
                    obj = dbsource.GetObjectByKeyWord(keyword);

                    if (obj == null) return new JObject();
                    AVEVA.CDMS.Server.Group curGroup;
                    if (obj is AVEVA.CDMS.Server.Group)
                        curGroup = (AVEVA.CDMS.Server.Group)obj;
                    else
                        return new JObject();
                    //循环获取用户组
                    jaGroupList = GetUserGroup(curGroup);

                }

                reJo.data = jaGroupList;

                reJo.total = jaGroupList.Count;
                reJo.success = true;

            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
                reJo.msg = e.Message;
            }
            return reJo.Value;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="UserKeyword">用户关键字</param>
        /// <returns></returns>
        public static JObject GetUserInfo(string sid, string UserKeyword) {
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

                //获取对象
                DBSource dbsource = curUser.dBSource;
                if (dbsource == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }


                if (string.IsNullOrEmpty(UserKeyword))
                {
                    reJo.msg = "错误的提交数据。";
                    return reJo.Value;
                }

                User user = dbsource.GetUserByKeyWord(UserKeyword);
                if (user == null)
                {
                    //return null; //没有返回值
                    reJo.msg = "参数错误，用户不存在！";
                    return reJo.Value;
                }

                int UserType = 0;
                try
                {
                    UserType = (int)user.O_usertype - 1;
                }
                catch
                {
                    UserType = 0;
                }

                int UserFlags = 0;
                try
                {
                    UserFlags = (int)user.O_flags - 1;
                }
                catch
                {
                    UserFlags = 0;
                }

                string Phone = user.O_suser3 == null ? "" : user.O_suser3.ToString();

                int groupIndex = 0;
                JObject joUserMbrOf = new JObject();
                foreach (Group group in user.GroupList)
                {
                    //joUserMbrOf.Add(new JProperty("Group"+groupIndex.ToString(), group.ToString));
                    joUserMbrOf.Add(new JProperty(group.KeyWord, group.ToString));
                }

                reJo.data = new JArray(new JObject(
                    new JProperty("O_userdesc", user.O_userdesc),
                    new JProperty("O_email", user.O_email),
                    new JProperty("O_username", user.O_username),
                    new JProperty("Phone", Phone),
                    new JProperty("UserType", UserType),//.ToString()),
                    new JProperty("UserFlags", UserFlags),//.ToString())
                    new JProperty("UserMbrOf", joUserMbrOf)
                    ));

                reJo.success = true;

            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
                reJo.msg = e.Message;
            }
            return reJo.Value;
        }

        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="UserKeyword">用户关键字</param>
        /// <param name="UserCode">用户代码</param>
        /// <param name="UserDesc">用户描述</param>
        /// <param name="UserEmail">用户Email</param>
        /// <param name="UserType">用户类型，值是0-3,分别代表：Default：0，Windows：1，DefaultAdmin：2，WindowsAdmin：3</param>
        /// <param name="UserStatus">用户状态，值是0-2，分别代表：OnLine：1 ,OffLine:2,Disabled:3</param>
        /// <param name="Phone">用户电话</param>
        /// <param name="UserPwd">用户密码</param>
        /// <param name="UserConfirmPwd">确认用户密码</param>
        /// <returns></returns>
        public static JObject SaveUserInfo(string sid, string UserKeyword, string UserCode, string UserDesc, string UserEmail, string UserType,
            string UserStatus, string Phone, string UserPwd, string UserConfirmPwd) {
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

                //获取对象
                DBSource dbsource = curUser.dBSource;
                if (dbsource == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }


                if (!curUser.IsAdmin)
                {
                    reJo.msg = "当前用户没有修改用户权限";
                    return reJo.Value;
                }

                if (string.IsNullOrEmpty(UserKeyword))
                {
                    reJo.msg = "错误的提交数据。";
                    return reJo.Value;
                }

                User user = dbsource.GetUserByKeyWord(UserKeyword);
                if (user == null)
                {
                    //return null; //没有返回值
                    reJo.msg = "参数错误，用户不存在！";
                    return reJo.Value;
                }

                //用户名不能为空
                if (UserCode.Trim() == string.Empty || UserDesc.Trim() == "")
                {
                    reJo.msg = "代码，描述不能为空!";
                    return reJo.Value;
                }

                UserPwd = UserPwd.Trim();
                UserConfirmPwd = UserConfirmPwd.Trim();
                UserEmail = UserEmail.Trim();
                //验证输入的密码和确认密码是否一致
                if (UserPwd != UserConfirmPwd)
                {
                    reJo.msg = "输入的密码和确认密码不一致!";
                    return reJo.Value;
                }

                //验证邮箱格式是否正确
                string CheckFormatResult = SetControls.CheckFormat(UserEmail, SetControls.enRegularExpressions.Email);
                if (CheckFormatResult != "true")
                {
                    reJo.msg = CheckFormatResult;
                    return reJo.Value;
                }

                //修改后的代码是否重叠
                if (user != null)
                {
                    if (Validation.ExistsObject(dbsource, user, UserCode))
                    {
                        reJo.msg = UserCode + "已存在";
                        return reJo.Value;
                    }
                }
                else
                {
                    //新添加的用户不能与现存的用户重复

                    bool DuplicateUser = false;
                    foreach (User userItem in dbsource.AllUserList)
                    {
                        if (userItem.Code.ToLower() == UserCode.ToLower())
                        {
                            DuplicateUser = true;
                        }
                    }


                    //if (DuplicateUser(dbsource, UserCode))
                    if (DuplicateUser)
                    {
                        reJo.msg = "新增项已存在!";
                        return reJo.Value;
                    }
                }

                //先判断是否有修改过内容
                //if (CDMSAdmin.g_clsModified.IsDifferent())
                {

                    //先验证,验证再前面的代码已经实现
                    //if (PassValidate(m_user))
                    {
                        User m_user = user;

                        m_user.O_email = UserEmail;

                        bool isSavePwd = false;
                        if (!string.IsNullOrEmpty(UserPwd) && m_user.O_passwd != UserPwd)
                        {
                            m_user.O_passwd = UserPwd;
                            isSavePwd = true;
                        }


                        m_user.O_userdesc = UserDesc.Trim();
                        m_user.O_username = UserCode.Trim();
                        m_user.O_usertype = GetUserType(Convert.ToInt32(UserType) + 1);
                        m_user.O_flags = GetUserFlags(Convert.ToInt32(UserStatus) + 1);
                        m_user.O_suser1 = m_user.dBSource.GUID;
                        m_user.O_suser3 = Phone;

                        //更新数据失败
                        if (!m_user.Modify())
                        {
                            //fmErrorDialog fmError = new fmErrorDialog() { Error = "更新数据失败!", DetailError = m_source.LastError };
                            //fmError.ShowDialog();
                            reJo.msg = "更新数据失败!" + dbsource.LastError;
                            return reJo.Value;
                        }

                        if (isSavePwd)
                        {
                            // 强制刷新共享数据源
                            //DBSourceController.RefreshShareDBSource();
                            DBSourceController.RefreshShareDBManager();
                        }

                        //上传图片
                        //UpLoadPicture();

                        //表示是否单击过新建按钮,新建、保存、删除后不用再提示“是否保存修改”
                        //CDMSAdmin.g_bClickNewButton = false;

                        //修改主界面的treeview和listview
                        //DelegateEvent.DoModifyTreeViewListView(m_user.ToString, m_user.Code, m_user.Description);

                        //设置按钮状态
                        //SetCtrlEnable();
                        //btnCancelUser.Enabled = false;

                        //成功修改后提示
                        //MessageBox.Show("保存成功!", "提示", MessageBoxButtons.OK);

                    }

                    ////保存数据
                    //DelegateEvent.SaveCurRunningUserControlData();

                    reJo.success = true;
                }




            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
                reJo.msg = e.Message;
            }
            return reJo.Value;
        }

        /// <summary>
        /// 新建用户
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="UserCode">用户代码</param>
        /// <param name="UserDesc">用户描述</param>
        /// <param name="UserEmail">用户Email</param>
        /// <param name="UserType">用户类型，值是0-3,分别代表：Default：0，Windows：1，DefaultAdmin：2，WindowsAdmin：3</param>
        /// <param name="UserStatus">用户状态，值是0-2，分别代表：OnLine：1 ,OffLine:2,Disabled:3</param>
        /// <param name="Phone">用户电话</param>
        /// <param name="UserPwd">用户密码</param>
        /// <param name="UserConfirmPwd">确认用户密码</param>
        /// <returns></returns>
        public static JObject CreateUser(string sid, string UserCode, string UserDesc, string UserEmail, string UserType,
    string UserStatus, string Phone, string UserPwd, string UserConfirmPwd)
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

                //获取对象
                DBSource dbsource = curUser.dBSource;
                if (dbsource == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

                if (!curUser.IsAdmin)
                {
                    reJo.msg = "当前用户没有添加用户权限";
                    return reJo.Value;
                }

                //用户名不能为空
                if (UserCode.Trim() == string.Empty || UserDesc.Trim() == "")
                {
                    reJo.msg = "代码，描述不能为空!";
                    return reJo.Value;
                }

                UserPwd = UserPwd.Trim();
                UserConfirmPwd = UserConfirmPwd.Trim();
                UserEmail = UserEmail.Trim();
                UserCode=UserCode.Trim();

                if (string.IsNullOrEmpty(UserPwd)) {
                    reJo.msg = "请输入密码!";
                    return reJo.Value;
                }

                //验证输入的密码和确认密码是否一致
                if (UserPwd != UserConfirmPwd)
                {
                    reJo.msg = "输入的密码和确认密码不一致!";
                    return reJo.Value;
                }

                //验证邮箱格式是否正确
                string CheckFormatResult = SetControls.CheckFormat(UserEmail, SetControls.enRegularExpressions.Email);
                if (CheckFormatResult != "true")
                {
                    reJo.msg = CheckFormatResult;
                    return reJo.Value;
                }

                bool DuplicateUser = false;
                foreach (User user in dbsource.AllUserList)
                {
                    if (user.Code.ToLower() == UserCode.ToLower())
                    {
                        DuplicateUser= true;
                    }
                }

                //新添加的用户不能与现存的用户重复
                if (DuplicateUser)
                {
                    reJo.msg = "新增项已存在!";
                    return reJo.Value;
                }
                ////修改后的代码是否重叠
                //if (user != null)
                //{
                //    if (Validation.ExistsObject(dbsource, user, UserCode))
                //    {
                //        reJo.msg = UserCode + "已存在";
                //        return reJo.Value;
                //    }
                //}
                //else
                //{

                //新建User
                enUserFlage userFlage = GetUserFlags(Convert.ToInt32(UserStatus) + 1);
                User newUser = dbsource.NewUser(
                                            userFlage,
                                            GetUserType(Convert.ToInt32(UserType) + 1),
                                                "",
                                                UserCode,
                                                UserDesc,
                                                UserPwd,
                                                UserEmail,
                                                null
                                                );
           

                    //判断是否创建对象成功
                    if (newUser == null)
                    {
                        reJo.msg= "创建对象失败!"+dbsource.LastError ;
                        return reJo.Value;
                    }

                    User m_user = newUser;
                    m_user.O_suser1 = m_user.dBSource.GUID;
                    m_user.O_suser3 = Phone;
                    //设置用户状态，解决删除用户后不能新建同名用户问题
                    m_user.O_flags = userFlage; 
                    m_user.Modify();

                // 强制刷新共享数据源
                //
                DBSourceController.RefreshShareDBManager();
                DBSourceController.RefreshDBSource(sid);

                reJo.success = true;
                //}
            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
                reJo.msg = e.Message;
            }
            return reJo.Value;
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="UserKeyword">用户关键字</param>
        /// <returns></returns>
        public static JObject DropUser(string sid, string UserKeyword)
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

                //获取对象
                DBSource dbsource = curUser.dBSource;
                if (dbsource == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

                if (string.IsNullOrEmpty(UserKeyword))
                {
                    reJo.msg = "错误的提交数据。";
                    return reJo.Value;
                }

                User selUser = dbsource.GetUserByKeyWord(UserKeyword);
                if (selUser == null)
                {
                    //return null; //没有返回值
                    reJo.msg = "参数错误，用户不存在！";
                    return reJo.Value;
                }

                //从数据库中移除数据
                if (selUser.Delete())
                {
                    //从列表中移除
                    List<User> allUserList = dbsource.AllUserList;
                    allUserList.Remove(selUser);
                    dbsource.AllUserList = allUserList;

                    //删除TreeNode和ListView中的项
                    //DelNodeAndItem(selUser);

                    reJo.success = true;
                    return reJo.Value;
                }



            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
                reJo.msg = e.Message;
            }
            return reJo.Value;
        }
/// <summary>
/// 修改用户密码
/// </summary>
/// <param name="sid">连接秘钥</param>
/// <param name="OldPassword">旧密码</param>
/// <param name="NewPassword">新密码</param>
/// <returns></returns>
public static JObject SetUserPassword(string sid, string OldPassword, string NewPassword)
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

                //获取对象
                DBSource dbsource = curUser.dBSource;
                if (dbsource == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

                if (string.IsNullOrEmpty(OldPassword))
                {
                    reJo.msg = "请输入旧密码！";
                    return reJo.Value;
                }

                if (string.IsNullOrEmpty(NewPassword))
                {
                    reJo.msg = "请输入新密码！";
                    return reJo.Value;
                }

                if (!curUser.PasswordIsRight(OldPassword))
                {
                    reJo.msg = "您输入的旧密码不对,请重新输入!";
                    return reJo.Value;
                }


                curUser.O_passwd = NewPassword;
                //curUser.O_passwd = dbsource.EnCrypt( NewPassword);

                curUser.Modify();

                // 强制刷新共享数据源
                //DBSourceController.RefreshShareDBSource();
                DBSourceController.RefreshShareDBManager();

                //刷新数据源，否则退出后再次登录，还是原来的密码
                //DBSource DB = DBSourceController.RefreshDBSource(sid,false,curUser.O_username,NewPassword);
                //DBSource DB = dbsource.NewDBSource(false);
                //DB.Login(curUser.O_username,NewPassword);

                reJo.success = true;

            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
                reJo.msg = e.Message;
            }
            return reJo.Value;
        }


/// <summary>
/// 添加目录到用户收藏夹
/// </summary>
/// <param name="sid">连接秘钥</param>
/// <param name="ProjectKeyword">目录关键字</param>
/// <returns></returns>
public static JObject AddFavorites(string sid, string ProjectKeyword)
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


        //删除当前节点对应的Project对象
        Project project = dbsource.GetProjectByKeyWord(ProjectKeyword);

        if (project == null)
        {
            reJo.msg = "目录不存在！";
            return reJo.Value;
        }

        string IFShowNAV = "true";
        string strProjs = "";

        Dictionary<Object, string[]> lstobj = new Dictionary<Object, string[]>();

        string s = "";

        bool bUserExist = false;
        string[] users = dbsource.DBExecuteSQL("select o_userno from User_Favorites where o_userno=" + curUser.ID.ToString(), false);
        if (users.Length >= 1 && users[0] == curUser.ID.ToString())
        {
            bUserExist = true;
        }

        string[] ss = dbsource.DBExecuteSQL("select O_suser5 from User_Favorites where o_userno=" + curUser.ID.ToString(), false);
        if (ss.Length >= 1 && (!string.IsNullOrEmpty(ss[0])))
        {
            s = ss[0];
        }

        //string s = curUser.O_suser5;

        //IFShowNAV|||KW^^^KW^^^KW
        if (!string.IsNullOrEmpty(s))
        {
            var sep = new string[] { "|||" };
            var sep2 = new string[] { "^^^" };

            string[] lstRel = s.Split(sep, StringSplitOptions.None);

            if (string.IsNullOrEmpty(lstRel[0]))
            {
                IFShowNAV = "true";
            }
            else
            {
                IFShowNAV = lstRel[0];
            }


            if (lstRel.Length > 1)
            {
                strProjs = lstRel[1];
                if (string.IsNullOrEmpty(strProjs))
                {
                    strProjs = ProjectKeyword;
                }
                else
                {
                    if (strProjs.IndexOf(ProjectKeyword) < 0)
                    {


                        strProjs = strProjs + "^^^" + ProjectKeyword;


                    }
                    else
                    {
                        //已经收藏了此目录，则忽略
                    }
                }
            }
            else
            {
                strProjs = ProjectKeyword;
            }



        }
        else
        {
            IFShowNAV = "true";
            strProjs = ProjectKeyword;
        }
        //curUser.O_suser5 = IFShowNAV + "|||" + strProjs;
        //curUser.Modify();

        string O_suser5 = IFShowNAV + "|||" + strProjs;

        if (bUserExist == true)
        {
            dbsource.DBExecuteCommand("update User_Favorites set O_suser5='" + O_suser5 + "' where o_userno=" + curUser.ID.ToString(), false);
        }
        else {
            dbsource.DBExecuteCommand("insert INTO User_Favorites (o_userno,O_suser5) values (" + curUser.ID.ToString() + ",'" + O_suser5 + "')", false);
        }
        //string[] users = dbsource.DBExecuteSQL("select O_suser5 from User_Favorites where o_userno=" + curUser.ID.ToString(), false);

        reJo.success = true;
        reJo.data = new JArray(new JObject(new JProperty("state", "addSuccess")));
    }
    catch (Exception e)
    {
        reJo.msg = e.Message;
        CommonController.WebWriteLog(e.Message);
    }

    return reJo.Value;

}


/// <summary>
/// 取消目录收藏
/// </summary>
/// <param name="sid">连接秘钥</param>
/// <param name="ProjectKeyword">目录关键字</param>
/// <returns></returns>
public static JObject DelFavorites(string sid, string ProjectKeyword)
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


        //删除当前节点对应的Project对象
        Project project = dbsource.GetProjectByKeyWord(ProjectKeyword);

        if (project == null)
        {
            reJo.msg = "目录不存在！";
            return reJo.Value;
        }

        string IFShowNAV = "true";
        string strProjs = "";
        string newStrProjs = "";

        Dictionary<Object, string[]> lstobj = new Dictionary<Object, string[]>();
        //string s = curUser.O_suser5;
        string s = "";

        bool bUserExist = false;
        string[] users = dbsource.DBExecuteSQL("select o_userno from User_Favorites where o_userno=" + curUser.ID.ToString(), false);
        if (users.Length >= 1 && users[0] == curUser.ID.ToString())
        {
            bUserExist = true;
        }

        string[] ss = dbsource.DBExecuteSQL("select O_suser5 from User_Favorites where o_userno=" + curUser.ID.ToString(), false);
        if (ss.Length >= 1 && (!string.IsNullOrEmpty(ss[0])))
        {
            s = ss[0];
        }

        //IFShowNAV|||KW^^^KW^^^KW
        if (!string.IsNullOrEmpty(s))
        {
            var sep = new string[] { "|||" };
            var sep2 = new string[] { "^^^" };

            string[] lstRel = s.Split(sep, StringSplitOptions.None);

            if (string.IsNullOrEmpty(lstRel[0]))
            {
                IFShowNAV = "true";
            }
            else
            {
                IFShowNAV = lstRel[0];
            }

           

            if (lstRel.Length > 1)
            {
                strProjs = lstRel[1];
                if (string.IsNullOrEmpty(strProjs))
                {
                    strProjs = ProjectKeyword;
                }
                else
                {
                    string[] projList = strProjs.Split(sep2, StringSplitOptions.None);
                    

                    foreach(string proj in projList){
                        if (proj==ProjectKeyword){
                            continue;
                        }
                        if (newStrProjs == "")
                        {
                            newStrProjs = proj;
                        }
                        else {
                            newStrProjs = newStrProjs + "^^^" + proj;
                        }
                    }
                   
                }
            }
            else
            {
                //strProjs = ProjectKeyword;
            }



            //curUser.O_suser5 = ProjectKeyword;

        }
        else
        {
            IFShowNAV = "true";
            strProjs = "";
        }
        //curUser.O_suser5 = IFShowNAV + "|||" + newStrProjs;
        //curUser.Modify();

        string O_suser5 = IFShowNAV + "|||" + newStrProjs;

        if (bUserExist == true)
        {
            dbsource.DBExecuteCommand("update User_Favorites set O_suser5='" + O_suser5 + "' where o_userno=" + curUser.ID.ToString(), false);
        }
        else
        {
            dbsource.DBExecuteCommand("insert INTO User_Favorites (o_userno,O_suser5) values (" + curUser.ID.ToString() + ",'" + O_suser5 + "')", false);
        }

        reJo.success = true;
        reJo.data = new JArray(new JObject(new JProperty("state","delSuccess")));
    }
    catch (Exception e)
    {
        reJo.msg = e.Message;
        CommonController.WebWriteLog(e.Message);
    }

    return reJo.Value;

}


/// <summary>
/// 获取用户收藏夹列表
/// </summary>
/// <param name="sid">连接秘钥</param>
/// <returns></returns>
public static JObject GetFavoritesList(string sid)
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


        string IFShowNAV = "true";
        string strProjs = "";

        Dictionary<Object, string[]> lstobj = new Dictionary<Object, string[]>();
        //string s = curUser.O_suser5;

        string s = "";
        string[] ss = dbsource.DBExecuteSQL("select O_suser5 from User_Favorites where o_userno=" + curUser.ID.ToString(), false);
        if (ss.Length >= 1 && (!string.IsNullOrEmpty(ss[0])))
        {
            s = ss[0];
        }

        //JObject joData =new JObject();
        JArray jaData=new JArray();
        //IFShowNAV|||KW^^^KW^^^KW
        if (!string.IsNullOrEmpty(s))
        {
            var sep = new string[] { "|||" };
            var sep2 = new string[] { "^^^" };

            string[] lstRel = s.Split(sep, StringSplitOptions.None);

            if (string.IsNullOrEmpty(lstRel[0]))
            {
                IFShowNAV = "true";
            }
            else {
                IFShowNAV = lstRel[0];
            }

            if (string.IsNullOrEmpty(lstRel[1]))
            {
                
            }
            else
            {
                 string[] projs = lstRel[1].Split(sep2, StringSplitOptions.None);
                foreach(string proj in projs){
                    Object obj=dbsource.GetObjectByKeyWord(proj);

                    if (obj != null && (obj is Project || obj is Doc))
                    {
                        if (obj is Project)
                        {
                            Project prj = (Project)obj;

                            JArray jaPrj = new JArray();
                            JObject joPrj=new JObject(
                                new JProperty("Keyword",prj.KeyWord),
                                new JProperty("Title",prj.ToString)
                                );
                            jaData.Add(joPrj);
                        }
                    }
                
                }

                
                //reJo.data = new JArray(new JObject(new JProperty("state", "delSuccess")));//返回删除成功消息给客户端
            }


        }
        else
        {
            IFShowNAV = "true";
            //strProjs = ProjectKeyword;
        }

        //reJo.data = new JArray(new JObject(
        //    new JProperty("IFShowNav",IFShowNAV),
        //    new JProperty("Favorites",jaData)));
        reJo.data = jaData;

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
        /// 设置启动时是否显示导航页
        /// </summary>
/// <param name="sid">连接秘钥</param>
/// <param name="bDisplay">是否显示导航页,值是"false"或"true"</param>
/// <returns></returns>
public static JObject SetNavigationDisplay(string sid, string bDisplay)
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

        if (!(bDisplay == "true" || bDisplay == "false")) {
            reJo.msg = "修改设置失败，参数错误！";
            return reJo.Value;
        }

        string IFShowNAV = bDisplay, strProjs="";

        bool bUserExist = false;
        string[] users = dbsource.DBExecuteSQL("select o_userno from User_Favorites where o_userno=" + curUser.ID.ToString(), false);
        if (users.Length >= 1 && users[0] == curUser.ID.ToString())
        {
            bUserExist = true;
        }

        string s = "";
        string[] ss = dbsource.DBExecuteSQL("select O_suser5 from User_Favorites where o_userno=" + curUser.ID.ToString(), false);
        if (ss.Length >= 1 && (!string.IsNullOrEmpty(ss[0])))
        {
            s = ss[0];
        }

        JArray jaData=new JArray();
        //IFShowNAV|||KW^^^KW^^^KW
        if (!string.IsNullOrEmpty(s))
        {
            var sep = new string[] { "|||" };
            var sep2 = new string[] { "^^^" };

            string[] lstRel = s.Split(sep, StringSplitOptions.None);

            if (string.IsNullOrEmpty(lstRel[1]))
            {
                strProjs = "";
            }
            else { strProjs = lstRel[1];  }
        }

        else
        {
            IFShowNAV = "true";
            strProjs = "";
        }

        string O_suser5 = IFShowNAV + "|||" + strProjs;

        if (bUserExist == true)
        {
            dbsource.DBExecuteCommand("update User_Favorites set O_suser5='" + O_suser5 + "' where o_userno=" + curUser.ID.ToString(), false);
        }
        else
        {
            dbsource.DBExecuteCommand("insert INTO User_Favorites (o_userno,O_suser5) values (" + curUser.ID.ToString() + ",'" + O_suser5 + "')", false);
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


        /// <summary>
/// 获取启动时是否显示导航页
        /// </summary>
/// <param name="sid">连接秘钥</param>
        /// <returns></returns>
public static JObject GetNavigationDisplay(string sid)
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

        string IFShowNAV = "false";

        //bool bUserExist = false;
        //string[] users = dbsource.DBExecuteSQL("select o_userno from User_Favorites where o_userno=" + curUser.ID.ToString(), false);
        //if (users.Length >= 1 && users[0] == curUser.ID.ToString())
        //{
        //    bUserExist = true;
        //}

        string s = "";
        string[] ss = dbsource.DBExecuteSQL("select O_suser5 from User_Favorites where o_userno=" + curUser.ID.ToString(), false);
        if (ss.Length >= 1 && (!string.IsNullOrEmpty(ss[0])))
        {
            s = ss[0];
        }

        JArray jaData = new JArray();
        //IFShowNAV|||KW^^^KW^^^KW
        if (!string.IsNullOrEmpty(s))
        {
            var sep = new string[] { "|||" };
            var sep2 = new string[] { "^^^" };

            string[] lstRel = s.Split(sep, StringSplitOptions.None);

            if (string.IsNullOrEmpty(lstRel[0]))
            {
                IFShowNAV = "true";
            }
            else {IFShowNAV=lstRel[0];}
        }

        else
        {
            IFShowNAV = "true";
           
        }

        reJo.data = new JArray(new JObject(new JProperty("IFShowNAV", IFShowNAV)));
       
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
        /// 检查sid是否可用，如果不可用，客户端就尝试自动登录
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <returns></returns>
        public static JObject CheckSid(string sid) {
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

                reJo.success = true;

            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
                reJo.msg = e.Message;
            }
            return reJo.Value;
        }

        private static JArray GetUserGroup(AVEVA.CDMS.Server.Group parentGroup)
        {
            JArray jaResult = new JArray();
            if (parentGroup != null)
            {
                try
                {
                    foreach (AVEVA.CDMS.Server.Group group in parentGroup.GroupList)
                    {
                        JObject joGroup = new JObject();
                        int childCount = group.GroupList.Count + group.ChildGroupList.Count;
                        bool flag = false;
                        if (childCount <= 0)
                            flag = true;

                        joGroup = new JObject(new JProperty("text", group.ToString), new JProperty("id", group.KeyWord), new JProperty("leaf", flag));
                        jaResult.Add(joGroup);
                    }
                    foreach (AVEVA.CDMS.Server.Group group2 in parentGroup.ChildGroupList)
                    {
                        JObject joGroup = new JObject();

                        int childCount = group2.GroupList.Count + group2.ChildGroupList.Count;
                        bool flag = false;
                        if (childCount <= 0)
                            flag = true;

                        joGroup = new JObject(new JProperty("text", group2.ToString), new JProperty("id", group2.KeyWord), new JProperty("leaf", flag));
                        jaResult.Add(joGroup);
                    }
                }
                catch (Exception e)
                {
                    CommonController.WebWriteLog(e.Message);
                }
            }
            return jaResult;
        }

        /// <summary>
        /// 获取用户类型
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static enUserType GetUserType(int index)
        {
            //Default   1
            //Windows   2    
            //DefaultAdmin  3
            //WindowsAdmin  4

            enUserType userType = new enUserType();
            if (index == (int)enUserType.Default)
            {
                userType = enUserType.Default;
            }
            else if (index == (int)enUserType.DefaultAdmin)
            {
                userType = enUserType.DefaultAdmin;
            }
            else if (index == (int)enUserType.Windows)
            {
                userType = enUserType.Windows;
            }
            else if (index == (int)enUserType.WindowsAdmin)
            {
                userType = enUserType.WindowsAdmin;
            }
            return userType;
        }


        /// <summary>
        /// 获取用户状态
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static enUserFlage GetUserFlags(int index)
        {
            //OnLine    1  
            //OffLine   2
            //Disabled  3
            enUserFlage userFlag = new enUserFlage();
            if (index == (int)enUserFlage.OnLine)
            {
                userFlag = enUserFlage.OnLine;
            }
            else if (index == (int)enUserFlage.OffLine)
            {
                userFlag = enUserFlage.OffLine;
            }
            else if (index == (int)enUserFlage.Disabled)
            {
                userFlag = enUserFlage.Disabled;
            }
            else if (index == (int)enUserFlage.Away)
            {
                userFlag = enUserFlage.Away;
            }
            return userFlag;
        }

    }
}
