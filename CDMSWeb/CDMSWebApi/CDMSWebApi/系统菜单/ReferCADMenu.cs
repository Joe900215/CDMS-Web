using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVEVA.CDMS.WebApi;
using AVEVA.CDMS.Server;
using System.Collections;

namespace AVEVA.CDMS.WebApi
{
    internal class ReferCADMenu : ExWebMenu
    {


        /// <summary>
        /// 当后缀名为DWG时，显示调用CAD参照功能
        /// </summary>
        /// <returns></returns>
        public override enWebMenuState MeasureMenuState()
        {
            enWebMenuState MenuState = enWebMenuState.Hide;
            try
            {
                if (base.SelDocList.Count <= 0)
                {
                    return enWebMenuState.Hide;
                }

                Doc doc = base.SelDocList[0];

                //获取扩展名
                string fullPath = doc.O_filename;
                string extension = System.IO.Path.GetExtension(fullPath);//扩展名

                if (extension.ToLower() == ".dwg")
                {
                    MenuState = enWebMenuState.Enabled;
                }
            }
            catch (Exception ex)
            {
                CommonController.WebWriteLog(ex.Message);
            }
            return MenuState;
        }

    }
}