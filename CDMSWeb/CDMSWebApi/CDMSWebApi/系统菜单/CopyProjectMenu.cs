﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVEVA.CDMS.WebApi;
using AVEVA.CDMS.Server;
using System.Collections;

namespace AVEVA.CDMS.WebApi
{
    internal class CopyProjectMenu : ExWebMenu
    {


        /// <summary>
        /// 复制目录菜单项
        /// 决定菜单的状态
        /// </summary>
        /// <returns></returns>
        public override enWebMenuState MeasureMenuState()
        {
            enWebMenuState MenuState = enWebMenuState.Disabled;
            try
            {
                //用户获取系统子菜单前的事件
                foreach (WebMenuEvent.Before_Get_Sys_Menu_Item_Event_Class BeforeGetSysMenuItem in WebMenuEvent.ListGetSysMenuItem)
                {
                    if (BeforeGetSysMenuItem.Event != null)
                    {
                        //如果在用户事件中筛选过了，就跳出事件循环
                        if (BeforeGetSysMenuItem.Event(BeforeGetSysMenuItem.PluginName, base.LoginUser, base.MenuName, ref MenuState))
                        {
                            return MenuState;
                        }
                    }
                }

                if (base.SelProjectList.Count <= 0)
                {
                    return enWebMenuState.Hide;
                }

                Project project = base.SelProjectList[0];
                //	project.O_iUser1(个人工作台) = null  表示用户创建的目录，有值则表示该目录由程序自动生成。
                if (project.O_iuser1 != null)
                {
                    MenuState = enWebMenuState.Disabled; 
                }
                else if (project.O_type == enProjectType.Local || project.O_type == enProjectType.UserCustom || project.ShortProject != null)
                {
                    bool hasPWriteRight = ProjectController.GetProjectPWriteRight(project,base.Sid);

                    //有删除目录权限才可以删除目录
                    if (hasPWriteRight)
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