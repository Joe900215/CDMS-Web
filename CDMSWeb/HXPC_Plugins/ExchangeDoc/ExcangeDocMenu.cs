using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVEVA.CDMS.WebApi;
using AVEVA.CDMS.Server;
using System.Collections;

namespace AVEVA.CDMS.HXPC_Plugins
{
    /// <summary>
    /// 内部互提，只有：专业主设或者专业设计人才可以提资
    /// </summary>
    internal class ExcangeDocMenu : ExWebMenu
    {
        private Project SelectedProject;

        /// <summary>
        /// 提资类型 1_新提资 2_继续提资 3_提资升版  0表示不提资
        /// </summary>
        private int menustate = 0;

        /// <summary>
        /// 决定菜单的状态
        /// </summary>
        /// <returns></returns>
        public override enWebMenuState MeasureMenuState()
        {
            enWebMenuState hide = enWebMenuState.Hide;
            try
            {
                if (base.SelProjectList.Count <= 0)
                {
                    return enWebMenuState.Hide;
                }

                Project project = base.SelProjectList[0];   //选择目录
                Project DesigPhaseProject = null;           //阶段
                Project ProfessionProject = null;           //专业

                if (project != null)
                {
                    Project parentProject = project;


                    //查找阶段和专业
                    bool flag = false;
                    while (parentProject != null)
                    {
                        if (parentProject.Code == "内部接口")
                        {
                            flag = true;
                        }
                        if ((parentProject.TempDefn != null) && (parentProject.TempDefn.Code == "PROFESSION"))
                        {
                            ProfessionProject = parentProject;
                        }
                        if ((parentProject.TempDefn != null) && (parentProject.TempDefn.Code == "DESIGNPHASE"))
                        {
                            DesigPhaseProject = parentProject;
                            break;
                        }
                        parentProject = parentProject.ParentProject;
                    }
                    if (ProfessionProject == null || DesigPhaseProject == null || flag == false)
                    {
                        return enWebMenuState.Hide;
                    }


                    //判断是否为主设或者专业设计人
                    flag = false;
                    string valueByKeyWord = ProfessionProject.GetValueByKeyWord("PROFESSIONOWNER");  //主设
                    if (!string.IsNullOrEmpty(valueByKeyWord) && (valueByKeyWord.ToLower() == project.dBSource.LoginUser.Code.ToLower()))
                    {
                        flag = true;
                    }
                    else
                    {
                        valueByKeyWord = ProfessionProject.GetValueByKeyWord("PROFESSIONDESIGN");      //专业设计人
                        if (!string.IsNullOrEmpty(valueByKeyWord) && (valueByKeyWord.ToLower() == project.dBSource.LoginUser.Code.ToLower()))
                        {
                            flag = true;
                        }
                    }
                    if (!flag)
                    {
                        return enWebMenuState.Hide;
                    }


                    //判断是否为升版或者继续提资
                    AttrData attrDataByKeyWord = project.GetAttrDataByKeyWord("ISSAVE");
                    if (attrDataByKeyWord != null )
                    {
                        //return enWebMenuState.Hide;
                        if (attrDataByKeyWord.ToString == "是" && base.MenuName == "继续提资...")
                        {
                            menustate = 2;
                            //base.MenuName = "继续提资";
                            return enWebMenuState.Enabled;
                        }

                        if ((base.SelProjectList[0].DocList != null) && (base.SelProjectList[0].DocList.Count > 0) && base.MenuName == "提资升版...")
                        {
                            menustate = 3;
                            //base.MenuName = "提资升版";
                            return enWebMenuState.Enabled;
                        }
                    }
                    else if (base.MenuName == "生成提资单...")
                    {
                       // base.MenuName = "生成提资单...";
                        menustate = 1;  //新提资

                        return enWebMenuState.Enabled;
                    }



                }
            }
            catch { }
            return enWebMenuState.Hide;
            //return hide;
            //try
            //{
            //    this.SelectedProject = base.SelProjectList[0];


            //    if (((this.SelectedProject == null) || (this.SelectedProject.TempDefn == null)) || (!(this.SelectedProject.TempDefn.Code == "DESIGNPROJECT") ))//&& !(this.SelectedProject.TempDefn.Code == "DESIGNPHASE")))
            //    {
            //        return hide;
            //    }



            //    //如果是设总，也可以启动该菜单
            //    foreach (Project p in this.SelectedProject.ChildProjectList)
            //    {
            //        AttrData ad = p.GetAttrDataByKeyWord("PROJECTOWNER");//设总
            //        if (ad != null && ad.group != null && ad.group.UserList != null && ad.group.UserList.Count > 0 && ad.group.UserList[0].O_userno == this.SelectedProject.dBSource.LoginUser.O_userno && ad.Code == p.dBSource.LoginUser.Code)
            //        {
            //            return enWebMenuState.Enabled;
            //        }
            //    }


            //    //系统管理员也可以启动该菜单
            //    if (this.SelectedProject.dBSource.LoginUser.IsAdmin)
            //    {
            //        return enWebMenuState.Enabled;
            //    }

            //    //string str2 = "CONTACTTYPE";
            //    //if (base.SelDocList != null && base.SelDocList.Count == 1)
            //    //{
            //    //    if (base.SelDocList[0].O_filename.ToLower().EndsWith(".thm"))
            //    //    {
            //    //        //bool hasRight = WebApi.DocController.ProjectController.GetProjectPCreateRight(base.SelProjectList[0], base.Sid);

            //    //        //有创建权限才可以创建目录
            //    //        //if (hasRight)
            //    //            hide = enWebMenuState.Enabled;
            //    //    }
            //    //}
            //}
            //catch { }
            //return hide;
        }

    }
}