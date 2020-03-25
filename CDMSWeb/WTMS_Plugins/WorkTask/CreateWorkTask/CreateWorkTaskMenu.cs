using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVEVA.CDMS.Server;
using AVEVA.CDMS.Common;
using AVEVA.CDMS.WebApi;

namespace AVEVA.CDMS.WTMS_Plugins
{
    internal class CreateWorkTaskMenu : ExWebMenu
    {


        /// <summary>
        /// 决定菜单的状态
        /// </summary>
        /// <returns></returns>
        public override enWebMenuState MeasureMenuState()
        {
            //return ReportMenu.GetReportMenuState(base.Sid, base.SelProjectList, "D.3");
            enWebMenuState hide = enWebMenuState.Hide;
            try
            {
                //hide = enWebMenuState.Enabled;

                string str2 = "WTMS_UNITFOLDER";
                //if (((base.SelProjectList != null) && (base.SelProjectList.Count == 1)) && ((base.SelProjectList[0].TempDefn != null) && (base.SelProjectList[0].TempDefn.KeyWord == str2) && (base.SelProjectList[0].Code == "D.3")))
                if (((base.SelProjectList != null) && (base.SelProjectList.Count == 1)) && ((base.SelProjectList[0].TempDefn != null) && (base.SelProjectList[0].TempDefn.KeyWord == str2) ))
                {
                    bool hasRight = ProjectController.GetProjectPCreateRight(base.SelProjectList[0], base.Sid);

                    //有创建权限才可以创建目录
                    if (hasRight)
                        hide = enWebMenuState.Enabled;
                }
            }
            catch { }
            return hide;
        }

    }
}