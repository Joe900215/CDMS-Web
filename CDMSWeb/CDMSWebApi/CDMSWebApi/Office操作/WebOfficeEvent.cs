using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Word = Microsoft.Office.Interop.Word;

namespace AVEVA.CDMS.WebApi
{
    public class WebOfficeEvent
    {

        #region 填写完Word属性项，保存Word前触发的事件
        public static List<Before_Save_Word_Event_Class> ListBeforeSaveWord = new List<Before_Save_Word_Event_Class> { };

        //2018-6-30 小钱，按照华西能源要求，填写完属性后，获取页数
       
        public delegate bool Before_Save_Word_Event(string PluginName, Word.Application WordApp, object oProjectOrDoc,ref bool needReWrite, ref Hashtable htReWrite);
     
        public class Before_Save_Word_Event_Class
        {
            public Before_Save_Word_Event Event;
            public string PluginName;
        }
        #endregion
    }
}
