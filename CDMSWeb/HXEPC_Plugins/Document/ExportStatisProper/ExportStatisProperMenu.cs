using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVEVA.CDMS.WebApi;
using AVEVA.CDMS.Server;
using System.Collections;

namespace AVEVA.CDMS.HXEPC_Plugins
{
    /// <summary>
    /// 统计功能按钮
    /// 把文件属性加到列表,写进统计表格,下载到本地
    /// </summary>
    internal class ExportStatisProperMenu : ExWebMenu
    {
        /// <summary>
        /// 决定菜单的状态
        /// </summary>
        /// <returns></returns>
        public override enWebMenuState MeasureMenuState()
        {
            return enWebMenuState.Enabled;
        }

    }
}
