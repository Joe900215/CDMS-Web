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
    /// 编辑函件正文
    /// </summary>
    internal class EditLetterTextMenu : ExWebMenu
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

                if (doc != null)
                {
                    if (doc.TempDefn.KeyWord != "CATALOGUING" || doc.WorkFlow==null) {
                        return enWebMenuState.Hide;
                    }

                    
                    #region 只有批准人才可以编辑正文
                    if (doc.WorkFlow.CuWorkState.Code != "APPROV")
                    {
                        return enWebMenuState.Hide;
                    }
                    bool flag = false;
                    foreach (WorkUser wu in doc.WorkFlow.CuWorkState.WorkUserList)
                    {
                        if (wu.User == doc.dBSource.LoginUser)
                        {
                            flag = true;
                        }
                    }
                    if (flag == false)
                    {
                        return enWebMenuState.Hide;
                    }

                    #endregion

                    CataloguDoc caDoc = new CataloguDoc();
                    caDoc.doc = doc;
                    string docAttrTemp = caDoc.CA_ATTRTEMP;
                    
                    if (docAttrTemp == "LETTERFILE"|| docAttrTemp== "FILETRANSMIT" || docAttrTemp == "MEETINGSUMMARY")
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