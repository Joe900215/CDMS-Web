using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVEVA.CDMS.WebApi;
using AVEVA.CDMS.Server;
using System.Collections;

namespace AVEVA.CDMS.WebApi
{
    internal class CopyDocMenu : ExWebMenu
    {


        /// <summary>
        /// 复制文档菜单项
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

                if (base.SelDocList.Count <= 0)
                {
                    return enWebMenuState.Hide;
                }

                Doc doc = base.SelDocList[0];

                bool hasDocDFWriteRight = DocController.GetDocDFWriteRight(doc, base.Sid);
                //有复制目录权限才可以删除目录
                if (hasDocDFWriteRight)// && doc.O_dmsstatus == (enDocStatus)2)
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