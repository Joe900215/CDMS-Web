using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AVEVA.CDMS.Server;
using AVEVA.CDMS.Common;
using AVEVA.CDMS.WebApi;
using System.Runtime.Serialization;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
//using LinqToDB;

namespace AVEVA.CDMS.HXEPC_Plugins
{
    public class ReplyLetterMenu : ExWebMenu
    {

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

                Doc mdoc = base.SelDocList[0];

                Doc doc = mdoc.ShortCutDoc == null ? mdoc : mdoc.ShortCutDoc;

                if (doc != null)
                {
                    User docCreater = doc.Creater;

                    TempDefn docTemp = doc.TempDefn;
                    if (docTemp == null || docTemp.KeyWord != "CATALOGUING")
                    {
                        return enWebMenuState.Hide;
                    }

                    AttrData data;
                    if ((data = doc.GetAttrDataByKeyWord("CA_ATTRTEMP")) == null)
                    {
                        return enWebMenuState.Hide;
                    }

                    //不是信函文件不显示菜单
                    if (data.ToString != "LETTERFILE")
                    {
                        return enWebMenuState.Hide;
                    }

                    //判断是否是接收方部门成员
                    if ((data = doc.GetAttrDataByKeyWord("CA_MAINFEEDERCODE")) == null || string.IsNullOrEmpty(data.ToString))
                    {
                        return enWebMenuState.Hide;
                    }

                    string recerCode = data.ToString;

                    AVEVA.CDMS.Server.Group gp = doc.dBSource.GetGroupByName(recerCode);
                    if (gp.AllUserList.Contains(base.LoginUser)) {
                        return enWebMenuState.Enabled;
                    }
                }
            }
            catch { }
            return enWebMenuState.Hide;
        }
    }
}
