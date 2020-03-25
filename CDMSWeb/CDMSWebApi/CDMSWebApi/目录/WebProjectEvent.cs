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



    public class WebProjectEvent
    {
        # region 根据条件需要显示的文件夹图标
        public static List<After_Get_Project_Icon_Event_Class> ListAfterGetProjectIconEvent = new List<After_Get_Project_Icon_Event_Class> { };

        //2018-6-30 小钱，按照华西能源要求，文件夹图标需要按用户提出的条件显示
        //获取文件夹信息后，根据条件需要显示的文件夹图标
        public delegate bool After_Get_Project_Icon_Event(string PlugName, Project project, ref string iconClass);

        public class After_Get_Project_Icon_Event_Class
        {
            public After_Get_Project_Icon_Event Event;
            public string PluginName;
        }
        #endregion


        #region 筛选需要显示的文件夹列表
        public static List<Before_Get_Project_List_Event_Class> ListBeforeGetProjects = new List<Before_Get_Project_List_Event_Class> { };

        //2018-6-30 小钱，按照华西能源要求，文件夹列表需要按用户提出的条件显示
        //获取文件夹列表后，根据条件再筛选需要显示的文件夹列表
        public delegate bool Before_Get_Project_List_Event(string PlugName, ref List<Project> ProjectList);//, string filter);

        public class Before_Get_Project_List_Event_Class
        {
            public Before_Get_Project_List_Event Event;
            public string PluginName;
        } 
        #endregion

    }
}
