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
    /// 非通信类文档升版
    /// </summary>
    internal class UpgradeFileMenu : ExWebMenu
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
                    User docCreater = doc.Creater;

                    if (docCreater != base.LoginUser)
                    {
                        return enWebMenuState.Hide;
                    }
                    TempDefn docTemp = doc.TempDefn;
                    if (docTemp == null || docTemp.KeyWord != "CATALOGUING")
                    {
                        return enWebMenuState.Hide;
                    }

                    AttrData data;
                    if ((data = doc.GetAttrDataByKeyWord("CA_ATTRTEMP")) == null) {
                        return enWebMenuState.Hide;
                    }

                    //不是非通信文件不显示菜单
                    if (data.ToString != "NONCOMM") {
                        return enWebMenuState.Hide;
                    }

                    Project project = doc.Project;

                    bool flag = false;

                    
                    {
                            flag = true;
                        }

                    if (flag)
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