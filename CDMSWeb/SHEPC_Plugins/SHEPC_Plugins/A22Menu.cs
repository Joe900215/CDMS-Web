using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVEVA.CDMS.WebApi;
using AVEVA.CDMS.Server;
using System.Collections;

namespace AVEVA.CDMS.SHEPC_Plugins
{
    internal class A22Menu : ExWebMenu
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
                string str2 = "CONTACTTYPE";
                if (((base.SelProjectList != null) && (base.SelProjectList.Count == 1)) && ((base.SelProjectList[0].TempDefn != null) && (base.SelProjectList[0].TempDefn.KeyWord == str2) && (base.SelProjectList[0].Code == "A.22")))
                {
                    bool hasRight = WebApi.ProjectController.GetProjectPCreateRight(base.SelProjectList[0], base.Sid);

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