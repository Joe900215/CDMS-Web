namespace AVEVA.CDMS.WebApi
{
    using AVEVA.CDMS.Server;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;



    public class WebMenuEvent
    {
        #region 用户获取系统子菜单前的事件
        public static List<Before_Get_Sys_Menu_Item_Event_Class> ListGetSysMenuItem = new List<Before_Get_Sys_Menu_Item_Event_Class> { };

        //2018-8-24 小钱，用户获取系统子菜单前的事件
        public delegate bool Before_Get_Sys_Menu_Item_Event(string PluginName, User User, string MenuName, ref enWebMenuState MenuState);

        public class Before_Get_Sys_Menu_Item_Event_Class
        {
            public Before_Get_Sys_Menu_Item_Event Event;
            public string PluginName;
        }
        #endregion
    }
}
