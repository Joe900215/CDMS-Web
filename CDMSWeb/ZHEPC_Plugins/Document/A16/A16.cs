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
    public class A16
    {
        internal static Hashtable GetHashtable(Doc doc, string docType, string fileCode, string title, string docAttrJson)
        {
            Hashtable htUserKeyWord = new Hashtable();
            try
            {
                string rpType = "", TEXT2 = "", TEXT3 = "", TEXT4 = "", TEXT5= "",
                    ENCLOSURE2 = "", ENCLOSURE3 = "", ENCLOSURE4 = "";

                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(docAttrJson);

                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();

                    //获取文件编码 
                    if (strName == "TEXT2") TEXT2 = strValue.Trim();
                    if (strName == "rpType") rpType = strValue.Trim();
                }
            
                htUserKeyWord.Add("FILECODE", fileCode);
                htUserKeyWord.Add("TEXT1", title);//工程名称
                htUserKeyWord.Add("TEXT2", TEXT2);//施工组织设计（项目管理实施规划）

                //TEXT3,TEXT5有删除线，TEXT4没有删除线
                if (rpType == "计划")
                {
                    TEXT3 = ""; TEXT4 = "计划/"; TEXT5 = "调整计划";
                    ENCLOSURE2 = ""; ENCLOSURE3 = "计划/"; ENCLOSURE4 = "调整计划";
                }
                else {
                    TEXT3 = "计划"; TEXT4 = "/调整计划"; TEXT5 = "";
                    ENCLOSURE2 = "计划"; ENCLOSURE3 = "/调整计划"; ENCLOSURE4 = "";
                }

                htUserKeyWord.Add("TEXT3", TEXT3);//有删除线
                htUserKeyWord.Add("TEXT4", TEXT4);//没有删除线
                htUserKeyWord.Add("TEXT5", TEXT5);//有删除线

                htUserKeyWord.Add("ENCLOSURE", TEXT2);//附件名称
                htUserKeyWord.Add("ENCLOSURE2", ENCLOSURE2);//附件
                htUserKeyWord.Add("ENCLOSURE3", ENCLOSURE3);//附件
                htUserKeyWord.Add("ENCLOSURE4", ENCLOSURE4);//附件
            }
            catch
            {

            }
            return htUserKeyWord;
        }

    }
}
