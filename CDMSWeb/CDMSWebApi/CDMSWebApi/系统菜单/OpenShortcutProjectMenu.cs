using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVEVA.CDMS.WebApi;
using AVEVA.CDMS.Server;
using System.Collections;

namespace AVEVA.CDMS.WebApi
{
    internal class OpenShortcutProjectMenu : ExWebMenu
    {


        /// <summary>
        /// 普通文档，逻辑目录，个人工作台，查询等页面的转到源目录，创建目录菜单项
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

                if (!(base.SelProjectList == null || base.SelProjectList.Count <= 0))
                {
                    Project proj = base.SelProjectList[0];
                }

                if (base.SelDocList == null || base.SelDocList.Count <= 0)
                    return enWebMenuState.Hide;

                //如果不是在逻辑目录，查询，和个人工作台页，且是快捷方式
                if (base.TVMenuPositon == enWebTVMenuPosition.TVDocument && base.SelDocList[0].ShortCutDoc == null)
                    return enWebMenuState.Hide;

                //如果是建立在逻辑目录，或个人工作台的实体文件，就不转到源目录了
                if (base.TVMenuPositon != enWebTVMenuPosition.TVDocument && base.SelDocList[0].ShortCutDoc == null &&
                    base.SelDocList[0].Project.O_type!=enProjectType.Local)
                    return enWebMenuState.Hide;

                MenuState = enWebMenuState.Enabled;
            }
            catch (Exception ex)
            {
                CommonController.WebWriteLog(ex.Message);
            }
            return MenuState;
        }

    }
}