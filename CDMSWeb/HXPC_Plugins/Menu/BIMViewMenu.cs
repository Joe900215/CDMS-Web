using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVEVA.CDMS.WebApi;
using AVEVA.CDMS.Server;
using System.Collections;

namespace AVEVA.CDMS.HXPC_Plugins
{
    internal class BIMViewMenu : ExWebMenu
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
                //string str2 = "CONTACTTYPE";
                if (base.SelDocList != null && base.SelDocList.Count == 1)
                {
                    if (base.SelDocList[0].O_filename.ToLower().EndsWith(".thm"))
                    {
                        //bool hasRight = WebApi.DocController.ProjectController.GetProjectPCreateRight(base.SelProjectList[0], base.Sid);

                        //有创建权限才可以创建目录
                        //if (hasRight)
                            hide = enWebMenuState.Enabled;
                    }
                }
            }
            catch { }
            return hide;
        }

    }
}