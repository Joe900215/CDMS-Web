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



    public class WebDocEvent
    {
        #region 文件列表需要按用户提出的条件显示
        public static List<Before_Get_Doc_List_Event_Class> ListBeforeGetDocs = new List<Before_Get_Doc_List_Event_Class> { };

        //2018-6-30 小钱，按照华西能源要求，文件列表需要按用户提出的条件显示
        //获取文件列表后，根据条件再筛选需要显示的文档列表
        public delegate bool Before_Get_Doc_List_Event(string PlugName, Project project, ref List<Doc> docList, string filter);//,out bool EventState);

        public class Before_Get_Doc_List_Event_Class
        {
            public Before_Get_Doc_List_Event Event;
            public string PluginName;
        }
        #endregion

        #region 预览文档前触发的事件 ，PVRight被修改为false时不能预览文档
        public static List<Before_Preview_Doc_Event_Class> ListBeforePreviewDoc = new List<Before_Preview_Doc_Event_Class> { };

        //2018-6-30 小钱，按照华西能源要求，预览文档按用户条件显示
        //获取文件列表后，根据条件再筛选需要显示的文档列表
        public delegate bool Before_Preview_Doc_Event(string PluginName, Doc doc, ref bool PVRight);
        //public delegate bool Before_Preview_Doc_Event(Doc doc, ref bool PVRight);

        public class Before_Preview_Doc_Event_Class
        {
            public Before_Preview_Doc_Event Event;
            public string PluginName;
        }
        #endregion

        #region 用户下载文件前的事件，DLRight被修改为false时不能下载文件
        public static List<Before_Download_File_Event_Class> ListBeforeDownloadFile = new List<Before_Download_File_Event_Class> { };

        //2018-6-30 小钱，按照华西能源要求，预览文档按用户条件显示
        //获取文件列表后，根据条件再筛选需要显示的文档列表
        public delegate bool Before_Download_File_Event(string PluginName, Doc doc, ref bool DLRight);

        public class Before_Download_File_Event_Class
        {
            public Before_Download_File_Event Event;
            public string PluginName;
        }
        #endregion


    }
}
