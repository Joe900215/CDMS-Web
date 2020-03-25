using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVEVA.CDMS.WebApi;
using AVEVA.CDMS.Server;
using System.Collections;

namespace AVEVA.CDMS.ZHEPC_Plugins
{
    internal class MountFileMenu : ExWebMenu
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
                string str2 = "GEDIREPORT";
                if (((base.SelDocList != null) && (base.SelDocList.Count == 1)))
                {
                    Doc selDoc = base.SelDocList[0];

                    Doc doc = selDoc.ShortCutDoc == null ? selDoc : selDoc.ShortCutDoc;

                    if (((doc.TempDefn != null) && (doc.TempDefn.KeyWord == str2)))
                    {
                        Project proj = CommonFunction.getParentProjectByTempDefn(doc.Project, "CONSTRUCTIONUNIT");
                        if (proj == null) return enWebMenuState.Hide;

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

                        string attrdata=doc.GetAttrDataByKeyWord("CD_ISMOUNT").ToString;

                        if (string.IsNullOrEmpty(attrdata) || attrdata != "是")
                        {
                            //有创建权限才可以创建目录
                            //if (hasRight)
                            hide = enWebMenuState.Enabled;
                        }
                        else {
                            return enWebMenuState.Disabled;
                        }
                    }
                }
            }
            catch { }
            return hide;
        }

    }
}