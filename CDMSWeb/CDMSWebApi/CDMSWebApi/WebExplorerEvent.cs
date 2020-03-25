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
    public class WebExploreEvent
    {
        /// <summary>
        /// Explorer 中新建对象后调用的委托事件
        /// </summary>
        /// <param name="obj">新建的对象实例, 可为Project/Doc/....</param>
        public delegate ExReJObject Explorer_AfterCreateNewObject(object obj);

        public static Explorer_AfterCreateNewObject OnAfterCreateNewObject = null;

        public delegate ExReJObject Explorer_AfterCreateNewObject_Event(string PlugName, object obj);

        public class Explorer_AfterCreateNewObject_Event_Class
        {
            public Explorer_AfterCreateNewObject_Event Event;
            public string PluginName;
        }

        public static List<Explorer_AfterCreateNewObject_Event_Class> ListAfterCreateNewObject = new List<Explorer_AfterCreateNewObject_Event_Class> { };
    }
}
