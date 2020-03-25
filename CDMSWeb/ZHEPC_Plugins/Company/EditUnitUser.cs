using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVEVA.CDMS.WebApi;
using AVEVA.CDMS.Server;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AVEVA.CDMS.ZHEPC_Plugins
{
    public class EditUnitUser
    {
        public static JObject GetGroupByUserSid(string sid, string ProjectKeyword)
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

                Project m_prj = dbsource.GetProjectByKeyWord(ProjectKeyword);
                if (m_prj == null)
                {
                    reJo.msg = "参数错误，目录不存在！";
                    return reJo.Value;
                }

                string groupName = m_prj.Code + "_ALL_Group";
                //从组织机构里面查找文控
                Server.Group gp = dbsource.GetGroupByName(groupName);
                if (gp == null)
                {
                    reJo.msg = "参数错误，用户组不存在！";
                    return reJo.Value;
                }
                JArray jaData = new JArray();

                jaData.Add(new JObject(
               new JProperty("GroupKeyword", gp.KeyWord),
               new JProperty("GroupName", gp.O_groupname),
               new JProperty("GroupDesc", gp.Description)
               ));

               reJo.data = jaData;

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
        /// 新建用户
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="UserCode"></param>
        /// <param name="UserDesc"></param>
        /// <param name="UserEmail"></param>
        /// <param name="UserType"></param>
        /// <param name="UserStatus"></param>
        /// <param name="Phone"></param>
        /// <param name="UserPwd"></param>
        /// <param name="UserConfirmPwd"></param>
        /// <returns></returns>
        public static JObject CreateUser(string sid, string ProjectKeyword, string GroupName, 
            string UserCode, string UserDesc, string UserEmail, //string UserType,
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

                Project m_prj = dbsource.GetProjectByKeyWord(ProjectKeyword);
                if (m_prj == null)
                {
                    reJo.msg = "参数错误，目录不存在！";
                    return reJo.Value;
                }

                string groupName = m_prj.Code + "_ALL_Group";
                //从组织机构里面查找用户组
                Server.Group group = dbsource.GetGroupByName(groupName);
                if (group == null)
                {
                    reJo.msg = "参数错误，用户组不存在！";
                    return reJo.Value;
                }

                #region 判断是否有用户操作权限

                string str2 = "CONSTRUCTIONUNIT";
                bool hasRight = false;

                {
                    Project proj = m_prj;

                    if (((proj.TempDefn != null) && (proj.TempDefn.KeyWord == str2)))
                    {


                        AttrData secData = proj.GetAttrDataByKeyWord("PROJSECRETARY");
                        if (secData == null)
                        {
                            reJo.msg = "参数错误，项目文控属性不存在！";
                            return reJo.Value;
                        }

                        string[] strAry = secData.ToString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);


                        foreach (string strUser in strAry)
                        {
                            User secUser = CommonFunction.GetUserByFullName(proj.dBSource, strUser);

                            if (secUser == curUser)
                            {
                                hasRight = true;
                            }
                        }

                    }
                }

                if (!curUser.IsAdmin && !hasRight)
                {
                    reJo.msg = "当前用户没有修改用户权限";
                    return reJo.Value;
                }
                #endregion

                //用户名不能为空
                if (UserCode.Trim() == string.Empty || UserDesc.Trim() == "")
                {
                    reJo.msg = "代码，描述不能为空!";
                    return reJo.Value;
                }

                UserPwd = UserPwd.Trim();
                UserConfirmPwd = UserConfirmPwd.Trim();
                UserEmail = UserEmail.Trim();
                UserCode = UserCode.Trim();

                if (string.IsNullOrEmpty(UserPwd))
                {
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
                        DuplicateUser = true;
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
                DBSource adminDbs = dbsource.dBManager.shareDBManger.DBSourceList[0];
                //新建User
                User newUser = adminDbs.NewUser(

                                            UserController.GetUserFlags(Convert.ToInt32(UserStatus) + 1),
                                            enUserType.Default,
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
                    reJo.msg = "创建对象失败!" + dbsource.LastError;
                    return reJo.Value;
                }

                User m_user = newUser;
                m_user.O_suser1 = m_user.dBSource.GUID;
                m_user.O_suser3 = Phone;
                m_user.Modify();


                // 强制刷新共享数据源
                //
                DBSourceController.RefreshShareDBManager();
                DBSourceController.RefreshDBSource(sid);


                if (!group.UserList.Contains(m_user))
                {
                    group.AddUser(m_user);
                    group.Modify();
                }
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

        public static JObject SaveUserInfo(string sid, string ProjectKeyword,string GroupName, 
            string UserKeyword, string UserCode, string UserDesc, string UserEmail, //string UserType,
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

                Project m_prj = dbsource.GetProjectByKeyWord(ProjectKeyword);
                if (m_prj == null)
                {
                    reJo.msg = "参数错误，目录不存在！";
                    return reJo.Value;
                }

                string groupName = m_prj.Code + "_ALL_Group";
                //从组织机构里面查找用户组
                Server.Group gp = dbsource.GetGroupByName(groupName);
                if (gp == null)
                {
                    reJo.msg = "参数错误，用户组不存在！";
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

                #region 判断是否有用户操作权限

                string str2 = "CONSTRUCTIONUNIT";
                bool hasRight = false;

                {
                    Project proj = m_prj;

                    if (((proj.TempDefn != null) && (proj.TempDefn.KeyWord == str2)))
                    {


                        AttrData secData = proj.GetAttrDataByKeyWord("PROJSECRETARY");
                        if (secData == null)
                        {
                            reJo.msg = "参数错误，项目文控属性不存在！";
                            return reJo.Value;
                        }

                        string[] strAry = secData.ToString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);


                        foreach (string strUser in strAry)
                        {
                            User secUser = CommonFunction.GetUserByFullName(proj.dBSource, strUser);

                            if (secUser == curUser)
                            {
                                hasRight = true;
                            }
                        }

                    }
                }
                bool isUserInGroup = gp.AllUserList.Contains(user);

                //登录用户是管理员，或者是项目的文控，如果是项目文控，就只可以操作项目所在用户组的用户
                if (!curUser.IsAdmin && (!hasRight || !isUserInGroup))
                {
                    reJo.msg = "当前用户没有修改用户权限";
                    return reJo.Value;
                }
                #endregion

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
                        //这里不允许修改用户类型
                        //m_user.O_usertype = UserController.GetUserType(Convert.ToInt32(UserType) + 1);
                        m_user.O_flags = UserController.GetUserFlags(Convert.ToInt32(UserStatus) + 1);
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
    }
}
