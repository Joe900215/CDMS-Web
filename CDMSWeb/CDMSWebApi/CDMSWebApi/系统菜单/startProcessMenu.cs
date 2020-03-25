using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVEVA.CDMS.WebApi;
using AVEVA.CDMS.Server;
using System.Collections;

namespace AVEVA.CDMS.WebApi
{
    internal class startProcessMenu : ExWebMenu
    {


        /// <summary>
        /// 启动流程菜单项
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

                if (base.SelDocList != null && base.SelDocList.Count > 0)
                {
                    Doc doc = base.SelDocList[0];
                    if (base.SelDocList[0].WorkFlow==null)
                    {
                        bool hasDFWriteRight = DocController.GetDocDFWriteRight(doc,base.Sid);
                        if ((hasDFWriteRight != true)
                            || base.SelDocList[0].OperateDocStatus != (enDocStatus)2)
                        { MenuState = enWebMenuState.Disabled; }
                        else
                        {
                            MenuState = enWebMenuState.Enabled;
                        }
                    }
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