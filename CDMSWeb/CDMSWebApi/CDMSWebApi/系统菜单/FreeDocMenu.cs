using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVEVA.CDMS.WebApi;
using AVEVA.CDMS.Server;
using System.Collections;

namespace AVEVA.CDMS.WebApi
{
    internal class FreeDocMenu : ExWebMenu
    {


        /// <summary>
        /// 删除文档菜单项
        /// 决定菜单的状态
        /// </summary>
        /// <returns></returns>
        public override enWebMenuState MeasureMenuState()
        {
            enWebMenuState MenuState = enWebMenuState.Hide;
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

                bool hasDFullRight = DocController.GetDocDFullRight(doc, base.Sid);

                //if(((doc->O_dmsstatus == (enDocStatus) 4|| doc->O_dmsstatus ==(enDocStatus::OUT_EXPORTED) || doc->O_dmsstatus == (enDocStatus) 1) && isExistedToLocal && doc->FLocker == doc->dBSource->LoginUser/*输出到了本地*/) || ( (doc->dBSource->LoginUser->IsAdmin|| right->DFull) && doc->O_dmsstatus != (enDocStatus) 2 && doc->O_dmsstatus != (enDocStatus::IN_FINAL)))
                if (((doc.O_dmsstatus == (enDocStatus)4 || doc.O_dmsstatus == (enDocStatus.OUT_EXPORTED) || doc.O_dmsstatus == (enDocStatus)1) && doc.FLocker == doc.dBSource.LoginUser/*输出到了本地*/) || ((doc.dBSource.LoginUser.IsAdmin || hasDFullRight) && doc.O_dmsstatus != (enDocStatus)2 && doc.O_dmsstatus != (enDocStatus.IN_FINAL)))
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