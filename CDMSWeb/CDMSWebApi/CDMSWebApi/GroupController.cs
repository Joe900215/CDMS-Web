using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AVEVA.CDMS.Server;

namespace AVEVA.CDMS.WebApi
{
    /// <summary>  
    /// 用户组操作类
    /// </summary>  
    public class GroupController : Controller
    {
        /// <summary>
        /// 创建一个用户组Group 
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="groupCode">用户组名称</param>
        /// <param name="groupDesc">用户组描述</param>
        /// <param name="parentGroup">父用户组Keyword</param>
        /// <returns></returns>
        public static JObject NewGroup(string sid, string groupCode, string groupDesc, string parentGroup) {
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

                //是否通过验证，传入null参数表示新建对象
                reJo.Value = PassValidate(dbsource, null, groupCode);
                if (reJo.success == false)
                {
                    return reJo.Value;
                }
                reJo.success = false;

                Group newGroup = null;
                if (parentGroup == "Root")
                {
                    //新建组织机构用户组
                    newGroup = dbsource.NewGroup(groupCode, groupDesc, AVEVA.CDMS.Server.enGroupType.Organization, "");

                }
                else
                {
                    Group ParentGroup = dbsource.GetGroupByKeyWord(parentGroup);

                    if (ParentGroup != null)
                    {
                        newGroup = dbsource.NewGroup(groupCode, groupDesc, ParentGroup);

                    }
                }

                //判断是否创建对象成功
                if (newGroup == null)
                {
                    reJo.msg = "创建对象失败!";
                }
                else
                {
                    DBSourceController.RefreshDBSource(sid);
                    reJo.data = new JArray(new JObject(new JProperty("groupKeyword", newGroup.KeyWord),
                        new JProperty("groupName", newGroup.ToString)));
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
        /// 删除用户组
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="groupKeyword">用户组Keyword</param>
        /// <returns></returns>
        public static JObject DropGroup(string sid, string groupKeyword)
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

                Group selGroup = dbsource.GetGroupByKeyWord(groupKeyword);

                if (selGroup == null)
                {
                    reJo.msg = "用户组不存在！";
                    return reJo.Value;
                }

                //判断要删除的组是否存在父组

                //若存在父组//////////////////////
                if (selGroup.ParentGroup != null)
                {
                    //从数据中移除ChildGroupList
                    reJo.Value = DelChildGroup(selGroup);
                    if (reJo.success == false)
                    {
                        //reJo.msg = "删除失败!" + selGroup.dBSource.LastError;
                        return reJo.Value;
                    }

                    reJo.success = false;

                    //从数据库中移除数据
                    if (selGroup.Delete())
                    {
                        //取父组
                        Group parentGroup = selGroup.ParentGroup;

                        //取父组的ChildGroupList
                        List<Group> childGroupList = parentGroup.ChildGroupList;

                        //从列表中删除
                        childGroupList.Remove(selGroup);

                        //更新列表
                        parentGroup.ChildGroupList = childGroupList;

                        reJo.success = true;
                        return reJo.Value;
                    }
                    else
                    {
                        reJo.msg = "删除失败!" + selGroup.dBSource.LastError;
                        return reJo.Value;
                    }
                }

                //若不存在父组//////////////////////
                else
                {
                    //从数据中移除ChildGroupList

                    reJo.Value = DelChildGroup(selGroup);
                    if (reJo.success == false)
                    {
                        //reJo.msg = "删除失败!" + selGroup.dBSource.LastError;
                        return reJo.Value;
                    }

                    reJo.success = false;

                    //从数据库中移除数据
                    if (selGroup.Delete())
                    {

                        //取数据源的AllGroupList
                        List<Group> allGroupList = dbsource.AllGroupList;

                        //从列表中删除组
                        allGroupList.Remove(selGroup);

                        //更新列表
                        dbsource.AllGroupList = allGroupList;

                        //从数据中移除UserList和GroupList
                        selGroup.Clear();

                        reJo.success = true;
                        return reJo.Value;
                    }
                    else
                    {
                        reJo.msg = "删除失败!" + selGroup.dBSource.LastError;
                        return reJo.Value;
                    }
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
        /// 删除组的用户
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="groupKeyword">用户所在用户组的Keyword</param>
        /// <param name="userKeyword">用户Keyword</param>
        /// <returns></returns>
        public static JObject DropUser(string sid, string groupKeyword, string userKeyword) {
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

                Group selGroup = dbsource.GetGroupByKeyWord(groupKeyword);

                if (selGroup == null)
                {
                    reJo.msg = "用户组不存在！";
                    return reJo.Value;
                }

                User selUser = dbsource.GetUserByKeyWord(userKeyword);

                if (selUser == null)
                {
                    reJo.msg = "组的用户不存在！";
                    return reJo.Value;
                }

                if (selGroup.ContainUser(selUser))
                {
                    selGroup.DeleteUser(selUser);
                    selGroup.Modify();

                    DBSourceController.RefreshDBSource(sid);

                    reJo.success = true;
                    return reJo.Value;
                }
                else
                {
                    reJo.msg = "删除失败!" + selGroup.dBSource.LastError;
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
        /// 向用户组添加用户
        /// </summary>
        /// <param name="sid">连接秘钥</param>
        /// <param name="groupKeyword">用户组的Keyword</param>
        /// <param name="userKeyword">用户Keyword</param>
        /// <returns></returns>
        public static JObject AddUser(string sid, string groupKeyword,string userKeyword)
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

                Group selGroup = dbsource.GetGroupByKeyWord(groupKeyword);

                if (selGroup == null)
                {
                    reJo.msg = "用户组不存在！";
                    return reJo.Value;
                }

                if (string.IsNullOrEmpty(userKeyword) ==true)
                {
                    reJo.msg = "用户参数错误！请输入用户。";
                    return reJo.Value;
                }

                //string[] strArray = (string.IsNullOrEmpty(userlist) ? "" : userlist).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                //foreach (string struser in strArray)
                {
                    //Object objuser = dbsource.GetObjectByKeyWord(struser);
                    Object objuser = dbsource.GetObjectByKeyWord(userKeyword);
                    
                    if (objuser == null)
                    {
                        reJo.msg = "用户参数错误！用户不存在！";
                        return reJo.Value;
                    }

                    if (objuser is User)
                    {

                        User user = objuser as User; ;
                        if (!selGroup.ContainUser(user))
                        {
                            selGroup.AddUser(user);
                            selGroup.Modify();

                            reJo.data = new JArray(new JObject(new JProperty("userKeyword", user.KeyWord)));
                            reJo.success = true;
                            return reJo.Value;
                        }
                        else {
                            reJo.msg = "用户"+ user.ToString+ "已存在！";
                            return reJo.Value;
                        }
                    }
                }

                //reJo.success = true;
                //return reJo.Value;
            }
            catch (Exception e)
            {
                CommonController.WebWriteLog(e.Message);
                reJo.msg = e.Message;
            }
            return reJo.Value;
        }

        /// <summary>
        /// 验证对象数据是否合法
        /// </summary>
        /// <param name="m_source"></param>
        /// <param name="group"></param>
        /// <param name="GroupName"></param>
        /// <returns></returns>
        internal static JObject PassValidate(DBSource m_source, Group group, string GroupName)
        {
            ExReJObject reJo = new ExReJObject();
            try
            {

                //组代码不能为空
                if (GroupName.Trim() == string.Empty)
                {
                    reJo.msg = "新建组的代码不能为空!";
                    return reJo.Value;
                }

                if (m_source != null)
                {
                    if (group == null)
                    {
                        //新建组的代码不能与现存的重复
                        if (DuplicateGroup(m_source, GroupName.Trim()))
                        {
                            reJo.msg = "新建组的代码不能与现存的重复!";
                            return reJo.Value;
                        }
                    }
                    else
                    {   //修改后组的代码不能与现存的重复
                        if (Validation.ExistsObject(m_source, group, GroupName.Trim()))
                        {
                            reJo.msg = "修改后组的代码不能与现存的重复!";
                            return reJo.Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                reJo.msg = ex.Message;
                return reJo.Value;
            }
            reJo.success = true;
            return  reJo.Value;
        }

        /// <summary>
        /// 验证新增的组是否已经存在在 DBSource.AllGroupList
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sGroupCode"></param>
        /// <returns></returns>
        private static bool DuplicateGroup(DBSource source, string sGroupCode)
        {
            if (source != null)
            {
                foreach (Group group in source.AllGroupList)
                {
                    if (group.Code.ToLower() == sGroupCode.ToLower())
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// 删除组的所有ChildGroup
        /// </summary>
        /// <param name="selGroup"></param>
        private static JObject DelChildGroup(Group selGroup)
        {
            ExReJObject reJo = new ExReJObject();

            if (selGroup == null) { reJo.success = true; return reJo.Value; }

            try
            {
                //从最后一项开始删除
                for (int i = selGroup.ChildGroupList.Count - 1; i >= 0; i--)
                {
                    Group childGroup = selGroup.ChildGroupList[i];

                    reJo.Value = DelChildGroup(childGroup);
                    //跌代删除
                    if (reJo.success==false) return reJo.Value;

                    reJo.success = false;

                    //从数据库中删除
                    if (childGroup.Delete())
                    {

                        List<Group> childGroupList = selGroup.ChildGroupList;

                        //从列表中删除
                        childGroupList.Remove(childGroup);

                        //更新列表
                        selGroup.ChildGroupList = childGroupList;

                        //确认删除
                        childGroup.Modify();
                    }
                    else
                    {
                        reJo.msg = "删除过程出错!" + selGroup.dBSource.LastError;
                        return reJo.Value;
                    }
                }

                reJo.success = true;
                return reJo.Value;
            }
            catch (Exception ex)
            {
                reJo.msg = "删除过程出错!" + ex.Message;
                return reJo.Value;
            }
        }
    }
}
