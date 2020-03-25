using System;
using System.Collections;
using System.Collections.Generic;
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

namespace AVEVA.CDMS.TBSBIM_Plugins
{
    public class EnterPoint
    {
        public static string PluginName = "TBSBIM_Plugins";
        public static void Init()
        {
            //if (WebWorkFlowEvent.BeforeWFSelectUsers1 == null)
            //{
            //    WebWorkFlowEvent.BeforeWFSelectUsers1 = (WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event1)Delegate.Combine(WebWorkFlowEvent.BeforeWFSelectUsers1, new WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event1(AVEVA.CDMS.ZHEPC_Plugins.EnterPoint.BeforeWF));
            //}
            //else
            //{
            //    WebWorkFlowEvent.BeforeWFSelectUsers1 = new WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event1(AVEVA.CDMS.ZHEPC_Plugins.EnterPoint.BeforeWF);
            //}

            //记录本插件的唯一标记
            //string PluginName = "ZHEPC_Plugins";
            //记录是否已加载
            bool isLoad = false;

            foreach (WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event_Class EventClass in WebWorkFlowEvent.ListBeforeWFSelectUsers)
            {
                if (EventClass.PluginName == PluginName)
                {
                    isLoad = true;
                    break;
                }
            }

            if (isLoad == false)
            {

                ////添加流程按钮事件处理
                //WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event BeforeWFSelectUsers = new WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event(AVEVA.CDMS.ZHEPC_Plugins.EnterPoint.BeforeWF);
                //WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event_Class Before_WorkFlow_SelectUsers_Event_Class = new WebWorkFlowEvent.Before_WorkFlow_SelectUsers_Event_Class();
                //Before_WorkFlow_SelectUsers_Event_Class.Event = BeforeWFSelectUsers;
                //Before_WorkFlow_SelectUsers_Event_Class.PluginName = PluginName;
                //WebWorkFlowEvent.ListBeforeWFSelectUsers.Add(Before_WorkFlow_SelectUsers_Event_Class);
            }
        }

        public static List<ExWebMenu> CreateNewExMenu()
        {
            try
            {
                List<ExWebMenu> menuList = new List<ExWebMenu>();

                
                ////编辑本单位用户..
                //EditUnitUserMenu EditUnitUser = new EditUnitUserMenu();
                //EditUnitUser.MenuName = "编辑本单位用户..";
                //EditUnitUser.MenuPosition = enWebMenuPosition.TVProject;
                //EditUnitUser.MenuType = enWebMenuType.Single;

                //menuList.Add(EditUnitUser);

                return menuList;
            }
            catch { }
            return null;
        }
        

    }

}
