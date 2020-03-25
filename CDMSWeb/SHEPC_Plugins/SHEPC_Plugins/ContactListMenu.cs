using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVEVA.CDMS.WebApi;
using AVEVA.CDMS.Server;
using System.Collections;

namespace AVEVA.CDMS.SHEPC_Plugins
{
    internal class ContactListMenu : ExWebMenu
    {


        /// <summary>
        /// 决定菜单的状态
        /// </summary>
        /// <returns></returns>
        public override enWebMenuState MeasureMenuState()
        {
            enWebMenuState hide = enWebMenuState.Hide;
            try
            {
                string str = "CONTACTTYPE";
                //if (((base.SelProjectList != null) && (base.SelProjectList.Count == 1)) && ((base.SelProjectList[0].TempDefn != null) && (base.SelProjectList[0].TempDefn.KeyWord == str2) && (base.SelProjectList[0].Code == "D.3")))
                    if (((base.SelProjectList != null) && (base.SelProjectList.Count == 1)) && ((base.SelProjectList[0].TempDefn != null) && (base.SelProjectList[0].TempDefn.KeyWord == str)
        && (base.SelProjectList[0].Code != "A.1") && (base.SelProjectList[0].Code != "A.3") && (base.SelProjectList[0].Code != "A.4") && (base.SelProjectList[0].Code != "A.6") && (base.SelProjectList[0].Code != "A.7")
        && (base.SelProjectList[0].Code != "A.9") && (base.SelProjectList[0].Code != "A.10") && (base.SelProjectList[0].Code != "A.11") && (base.SelProjectList[0].Code != "D.3") && (base.SelProjectList[0].Code != "D.1")
        && (base.SelProjectList[0].Code != "A.12") && (base.SelProjectList[0].Code != "A.15") && (base.SelProjectList[0].Code != "A.16") && (base.SelProjectList[0].Code != "A.18") && (base.SelProjectList[0].Code != "A.19")
        && (base.SelProjectList[0].Code != "A.21") && (base.SelProjectList[0].Code != "A.22") && (base.SelProjectList[0].Code != "A.25") && (base.SelProjectList[0].Code != "A.26") && (base.SelProjectList[0].Code != "A.27")
        && (base.SelProjectList[0].Code != "A.32") && (base.SelProjectList[0].Code != "B.1") && (base.SelProjectList[0].Code != "B.2") && (base.SelProjectList[0].Code != "B.3")))
                {
                    hide = enWebMenuState.Enabled;
                }
            }
            catch { }
            return hide;
        }

    }
}