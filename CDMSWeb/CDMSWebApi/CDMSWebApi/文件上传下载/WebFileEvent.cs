using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVEVA.CDMS.Server;

namespace AVEVA.CDMS.WebApi
{
    public class WebFileEvent
    {
        #region 文件上传完成后触发的事件
        public static List<After_Upload_File_Event_Class> ListAfterUploadFile = new List<After_Upload_File_Event_Class> { };

        //2019-9-19 小钱，按照钰海要求，添加文件上传完成后触发的事件
        //获取文件列表后，根据条件再筛选需要显示的文档列表
        public delegate bool After_Upload_File_Event(string PlugName, Doc DocKeyword);

        public class After_Upload_File_Event_Class
        {
            public After_Upload_File_Event Event;
            public string PluginName;
        }
        #endregion
    }
}
