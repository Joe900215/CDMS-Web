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
    /// 获取发文编号菜单
    /// </summary>
    internal class GetSendCodeMenu : ExWebMenu
    {
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
                else
                {
                    if (base.SelDocList[0].WorkFlow != null &&
                        base.SelDocList[0].WorkFlow.DefWorkFlow.O_Code=="COMMUNICATIONWORKFLOW" &&
                        base.SelDocList[0].WorkFlow.CuWorkState.DefWorkState.O_Code == "SECRETARILMAN")
                    {
                        return enWebMenuState.Enabled;
                    }

                }
            }
            catch { }
            return enWebMenuState.Hide;
        }
    }
}
