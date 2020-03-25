using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVEVA.CDMS.WebApi;
using AVEVA.CDMS.Server;
using System.Collections;

namespace AVEVA.CDMS.HXEPC_Plugins
{
    /// <summary>
    /// 自检
    /// </summary>
    internal class DraftSelfCheckMenu : ExWebMenu
    {
        private Project SelectedProject;

        /// <summary>
        /// 决定菜单的状态
        /// </summary>
        /// <returns></returns>
        public override enWebMenuState MeasureMenuState()
        {
            try
            {
                if (base.SelDocList.Count <= 0)
                {
                    return enWebMenuState.Hide;
                }

                Doc doc = base.SelDocList[0];

                if (doc == null || doc.WorkFlow==null || 
                    doc.WorkFlow.DefWorkFlow.O_Code!="CHECKFILE" || 
                    doc.WorkFlow.CuWorkState.DefWorkState.O_Code!="CHECK")
                {
                       return enWebMenuState.Hide;
                }

                if (doc.WorkFlow.CuWorkState.CuWorkUser.User != base.LoginUser)
                {
                    return enWebMenuState.Hide;
                }
                return enWebMenuState.Enabled;


            }
            catch { }
            return enWebMenuState.Hide;
            //return enWebMenuState.Enabled;
        }

    }
}
