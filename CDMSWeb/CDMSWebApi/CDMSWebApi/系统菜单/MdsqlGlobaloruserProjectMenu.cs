using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVEVA.CDMS.WebApi;
using AVEVA.CDMS.Server;
using System.Collections;

namespace AVEVA.CDMS.WebApi
{
    internal class MdsqlGlobaloruserProjectMenu : ExWebMenu
    {


        /// <summary>
        /// 创建目录菜单项-编辑查询条件
        /// 决定菜单的状态
        /// </summary>
        /// <returns></returns>
        public override enWebMenuState MeasureMenuState()
        {
            enWebMenuState MenuState = enWebMenuState.Disabled;
            try
            {

                //下载文件前触发的事件 ，DLRight被修改为false时不能下载文档
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

                if (!base.LoginUser.IsAdmin)
                    return enWebMenuState.Hide;

                if (base.SelProjectList == null || base.SelProjectList.Count <= 0 || base.SelProjectList[0] == null)
                    return enWebMenuState.Hide;

                if (base.SelProjectList[0].O_type != enProjectType.GlobSearch)
                    return enWebMenuState.Hide;

                MenuState = enWebMenuState.Enabled;

                return MenuState;

            }
            catch (Exception ex)
            {
                CommonController.WebWriteLog(ex.Message);
            }
            return MenuState;
        }

    }
}