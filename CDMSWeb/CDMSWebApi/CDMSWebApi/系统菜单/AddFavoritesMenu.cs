using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVEVA.CDMS.WebApi;
using AVEVA.CDMS.Server;
using System.Collections;

namespace AVEVA.CDMS.WebApi
{
    internal class AddFavoritesMenu : ExWebMenu
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
                ////用户获取系统子菜单前的事件
                //foreach (WebMenuEvent.Before_Get_Sys_Menu_Item_Event_Class BeforeGetSysMenuItem in WebMenuEvent.ListGetSysMenuItem)
                //{
                //    if (BeforeGetSysMenuItem.Event != null)
                //    {
                //        //如果在用户事件中筛选过了，就跳出事件循环
                //        if (BeforeGetSysMenuItem.Event(BeforeGetSysMenuItem.PluginName, base.LoginUser, base.MenuName, ref MenuState))
                //        {
                //            return MenuState;
                //        }
                //    }
                //}

                //if (base.SelProjectList.Count <= 0)
                //{
                //    return enWebMenuState.Hide;
                //}

                Project project = base.SelProjectList[0];

                if (project.O_type == enProjectType.UserCustom || project.O_type == enProjectType.GlobCustom
                    || project.O_type == enProjectType.GlobSearch
                    )
                {
                    //如果是个人工作台，把菜单置灰
                    MenuState = enWebMenuState.Hide;
                }
                else{
                        MenuState = enWebMenuState.Enabled;
                }
            }
            catch (Exception ex)
            {
                CommonController.WebWriteLog(ex.Message);
            }
            return MenuState;
        }

    }
}
