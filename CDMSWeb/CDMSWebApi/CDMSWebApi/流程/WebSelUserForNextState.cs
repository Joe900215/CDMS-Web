namespace AVEVA.CDMS.WebApi
{
    using AVEVA.CDMS.Server;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    
    /// <summary>
    /// WEB选择用户
    /// </summary>
    public class WebSelUserForNextState
    {
        
        /// <summary>
        /// 获取当前流程状态的下一流程状态的用户组
        /// </summary>
        /// <param name="wf">工作流对象</param>
        /// <param name="ws">状态</param>
        /// <returns>用户组</returns>
        internal  static Group SelectUser(WorkFlow wf, WorkState ws)
        {

            //DefWorkState.O_Role:当前流程状态校审人员的KeyWord(在模板中定义),也可以定义SQL，该字段长度为4000 
            string[] strArray = (string.IsNullOrEmpty(ws.DefWorkState.O_Role) ? "" : ws.DefWorkState.O_Role).Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            Group checkGroup = null;

            if ((strArray == null) || (strArray.Length <= 0))
            {
                //  ProjectDesc = "";
                return SimpleSelUser(ws);
            }
            if (strArray.Length <= 1)
            {
                checkGroup = ws.CheckGroup;
                if (((checkGroup == null) || (checkGroup.AllUserList == null)) || (checkGroup.AllUserList.Count <= 0))
                {
                    checkGroup = SimpleSelUser(ws);
                }
                return checkGroup;
            }


            //如果有两个以上“：”号，返回null
            if (strArray.Length > 2)
            {
                return checkGroup;
            }


            //处理SELECT:XXX 模式(strArray==2)
            if (strArray[0].ToUpper() == "SELECT")
            {
                return SelUserOnOneProject(ws, (wf.Project != null) ? wf.Project : wf.DocList[0].Project, strArray[1], true);
            }

            //处理GROUP:XXX 模式(strArray==2)
            if (strArray[0].ToUpper() == "GROUP")
            {
                return wf.dBSource.GetGroupByName(strArray[1]);
            }

            //处理XXX:YYY 模式 
            List<Project> list = SelProjectsByKeyWord(strArray[0], wf);
            if ((list != null) && (list.Count > 0))
            {
                List<string> allStringList = new List<string>();
                Hashtable hashtable = new Hashtable();
                foreach (Project project in list)
                {
                    allStringList.Add(project.ToString);
                    hashtable.Add(project.ToString, project);
                }
                //fmSelString str2 = new fmSelString(allStringList, null, true)
                //{
                //    Text = "选择参与 " + ws.DefWorkState.O_Description + " 的" + list[0].TempDefn.Description
                //};
                //str2.ShowDialog();
                //List<Project> list3 = new List<Project>();
                //if ((str2.SelStrings != null) && (str2.SelStrings.Count > 0))
                //{
                //    foreach (string str3 in str2.SelStrings)
                //    {
                //        if (hashtable.ContainsKey(str3))
                //        {
                //            list3.Add((Project)hashtable[str3]);
                //        }
                //    }
                //}
                //if (list3.Count > 0)
                //{
                //    checkGroup = ws.dBSource.NewGroup();
                //    foreach (Project project2 in list3)
                //    {
                //        ProjectDesc = "为 " + project2.ToString + " ";
                //        Group group = SelUserOnOneProject(ws, project2, strArray[1], false);
                //        if (group != null)
                //        {
                //            checkGroup.AddGroup(group);
                //        }
                //    }
                //}
                return checkGroup;
            }
            return SimpleSelUser(ws);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="wf"></param>
        /// <returns></returns>
        internal static List<Project> SelProjectsByKeyWord(string keyword, WorkFlow wf)
        {
            Project parentProject = (wf.Project != null) ? wf.Project : wf.DocList[0].Project;
            Project project = null;
            keyword = keyword.ToUpper();
            while ((parentProject != null) && !string.IsNullOrEmpty(parentProject.ToString))
            {
                if ((parentProject.TempDefn != null) && (parentProject.TempDefn.Code.ToUpper() == keyword))
                {
                    project = parentProject.ParentProject;
                    break;
                }
                parentProject = parentProject.ParentProject;
            }

            List<Project> list = new List<Project>();
            if (project != null)
            {
                foreach (Project projectItem in project.ChildProjectList)
                {
                    if ((projectItem.TempDefn != null) && (projectItem.TempDefn.Code.ToUpper() == keyword))
                    {
                        list.Add(projectItem);
                    }
                }
            }
            return list;
        }

        internal static Group SelUserOnOneProject(WorkState ws, Project proj, string role, bool isSelectedRole)
        {
            Group groupByKeyWord = null;
            if (isSelectedRole)
            {
                if ((ws != null) && (ws.SelectUserList != null))
                {
                    foreach (User user in ws.SelectUserList)
                    {
                        if (groupByKeyWord == null)
                        {
                            groupByKeyWord = ws.dBSource.NewGroup();
                        }
                        if (!groupByKeyWord.ContainUser(user))
                        {
                            groupByKeyWord.AddUser(user);
                        }
                    }
                }
            }
            else
            {
                groupByKeyWord = proj.GetGroupByKeyWord(role);
            }
            if (groupByKeyWord == null)
            {
                return SimpleSelUser(ws, null, isSelectedRole);
            }
            if (((groupByKeyWord != null) && (groupByKeyWord.UserList != null)) && (groupByKeyWord.UserList.Count > 1))
            {
                return SimpleSelUser(ws, role, isSelectedRole);
            }
            return groupByKeyWord;
        }

        internal static Group SimpleSelUser(WorkState ws)
        {
            return SimpleSelUser(ws, null, false);
        }



        internal static Group SimpleSelUser(WorkState ws, string selectRole, bool isSelectRole)
        {
            DBSource dBSource = ws.dBSource;
            List<User> asoUserList = null;
            if (!string.IsNullOrEmpty(selectRole))
            {
                Group group = null;
                if (isSelectRole)
                {
                    if ((ws != null) && (ws.SelectUserList != null))
                    {
                        foreach (User user in ws.SelectUserList)
                        {
                            if (group == null)
                            {
                                group = ws.dBSource.NewGroup();
                            }
                            if (!group.ContainUser(user))
                            {
                                group.AddUser(user);
                            }
                        }
                    }
                }
                else
                {
                    group = (ws.WorkFlow.Project != null) ? ws.WorkFlow.Project.GetGroupByKeyWord(selectRole) : ws.WorkFlow.DocList[0].GetGroupByKeyWord(selectRole);
                }
                if ((group != null) && (group.UserList != null))
                {
                    if (group.UserList.Count == 1)
                    {
                        return group;
                    }
                    if (group.UserList.Count > 1)
                    {
                        List<string> allStringList = new List<string>();
                        foreach (User user2 in group.UserList)
                        {
                            allStringList.Add(user2.ToString);
                        }
                        Group group2 = null;

                        if (((group2 != null) && (group2.UserList != null)) && (group2.UserList.Count > 0))
                        {
                            return group2;
                        }
                        if (((group2 != null) && (group2.GroupList != null)) && (group2.GroupList.Count > 0))
                        {
                            return group2;
                        }
                    }
                }
            }
            if ((ws.SelectUserList != null) && (ws.SelectUserList.Count > 0))
            {
                asoUserList = new List<User>();
                foreach (User user4 in ws.SelectUserList)
                {
                    asoUserList.Add(user4);
                }
            }

            return null;
        }

    }
}

