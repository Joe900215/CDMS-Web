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

namespace AVEVA.CDMS.ZHEPC_Plugins
{
    public class C1
    {
        internal static Hashtable GetHashtable(Doc doc, string docType, string fileCode, string title, string docAttrJson)
        {
            Hashtable htUserKeyWord = new Hashtable();
            try
            {
                string rpType = "", TEXT2 = "", TEXT3 = "", TEXT4 = "", ENCLOSURE2 = "", ENCLOSURE3 = "", ENCLOSURE4 = "";

                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(docAttrJson);

                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();

                    //获取文件编码 
                    if (strName == "rpType") rpType = strValue.Trim();
                }

                //TEXT3,TEXT5有删除线，TEXT4没有删除线
                if (rpType == "设计计划")
                {
                    TEXT2 = ""; TEXT3 = "设计计划/"; TEXT4 = "图纸交付进度计划";
                    ENCLOSURE2 = ""; ENCLOSURE3 = "设计计划/"; ENCLOSURE4 = "图纸交付进度计划";
                }
                else
                {
                    TEXT2 = "设计计划"; TEXT3 = "/图纸交付进度计划"; TEXT4 = "";
                    ENCLOSURE2 = "计划"; ENCLOSURE3 = "/图纸交付进度计划"; ENCLOSURE4 = "";
                }

                htUserKeyWord.Add("TEXT2", TEXT2);//有删除线
                htUserKeyWord.Add("TEXT3", TEXT3);//没有删除线
                htUserKeyWord.Add("TEXT4", TEXT4);//有删除线

                htUserKeyWord.Add("FILECODE", fileCode);
                htUserKeyWord.Add("TEXT1", title);//工程名称
                //htUserKeyWord.Add("TEXT2", rpType);//施工组织设计（项目管理实施规划）
                htUserKeyWord.Add("ENCLOSURE", rpType);//施工组织设计（项目管理实施规划）
            }
            catch
            {

            }
            return htUserKeyWord;
        }
    
    }
}
