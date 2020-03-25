using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVEVA.CDMS.WebApi;
using AVEVA.CDMS.Server;
using System.Collections;

namespace AVEVA.CDMS.ZHEPC_Plugins
{
    internal class EditUnitUserMenu : ExWebMenu
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
                string str2 = "CONSTRUCTIONUNIT";
                if (((base.SelProjectList != null) && (base.SelProjectList.Count == 1)))
                {
                    Project proj = base.SelProjectList[0];

                    if (((proj.TempDefn != null) && (proj.TempDefn.KeyWord == str2)))
                    {


                        AttrData secData = proj.GetAttrDataByKeyWord("PROJSECRETARY");
                        if (secData == null) return enWebMenuState.Hide;

                        string[] strAry = secData.ToString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        bool hasRight = false;

                        foreach (string strUser in strAry)
                        {
                            User secUser = CommonFunction.GetUserByFullName(proj.dBSource, strUser);

                            if (secUser == base.LoginUser)
                            {
                                hasRight = true;
                            }
                        }
                        if (!hasRight)
                            return enWebMenuState.Disabled;

                        return enWebMenuState.Enabled;
                    }
                }
            }
            catch { }
            return hide;
        }

    }
}