using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVEVA.CDMS.WebApi;
using AVEVA.CDMS.Server;
using System.Collections;

namespace AVEVA.CDMS.WebApi
{
    internal class CCreateGlobCustomMenu : ExWebMenu
    {


        /// <summary>
        /// 创建目录菜单项
        /// 决定菜单的状态
        /// </summary>
        /// <returns></returns>
        public override enWebMenuState MeasureMenuState()
        {
            enWebMenuState MenuState = enWebMenuState.Disabled;
            try
            {
                //只有在逻辑目录页面，才显示
                if (base.SelProjectList != null && base.SelProjectList.Count > 0 && base.SelProjectList[0].O_type != enProjectType.GlobCustom)
                {
                    return enWebMenuState.Hide;
                }

                //下载文件前触发的事件 ，DLRight被修改为false时不能下载文档
                foreach (WebMenuEvent.Before_Get_Sys_Menu_Item_Event_Class BeforeGetSysMenuItem in WebMenuEvent.ListGetSysMenuItem)
                {
                    if (BeforeGetSysMenuItem.Event != null)
                    {
                        //如果在用户事件中筛选过了，就跳出事件循环
                        if (BeforeGetSysMenuItem.Event(BeforeGetSysMenuItem.PluginName, base.LoginUser,base.MenuName, ref MenuState))
                        {
                            return MenuState;
                        }
                    }
                }

                if (base.TVMenuPositon != enWebTVMenuPosition.TVLogicProject)
                    return enWebMenuState.Hide;

                if (base.LoginUser.IsAdmin)
                  MenuState = enWebMenuState.Enabled;

                return MenuState;

                //if (base.SelProjectList.Count <= 0)
                //{
                //    return enWebMenuState.Hide;
                //}

                //Project project = base.SelProjectList[0];
                //if (project != null && (project.O_type == enProjectType.Local || project.O_type == enProjectType.UserCustom || project.ShortProject != null))
                //{
                //    if (project.ParentProject != null) {
                //        //只有在根目录才会弹出创建根目录的菜单
                //        return enWebMenuState.Hide;
                //    }

                //    bool hasRight = ProjectController.GetProjectPCreateRight(project, base.Sid);

                //    //有创建权限才可以创建目录
                //    if (hasRight)
                //        MenuState = enWebMenuState.Enabled;
                //}
            }
            catch (Exception ex)
            {
                CommonController.WebWriteLog(ex.Message);
            }
            return MenuState;
        }

    }
}