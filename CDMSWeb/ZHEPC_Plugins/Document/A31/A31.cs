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
    public class A31
    {
        internal static Hashtable GetHashtable(Doc doc, string docType, string fileCode, string title, string docAttrJson)
        {
            Hashtable htUserKeyWord = new Hashtable();
            try
            {
                string rpType = "", TEXT1 = "", TEXT2 = "", TEXT3 = "", TEXT4 = "", 
                    ENCLOSURE2 = "", ENCLOSURE3 = "", ENCLOSURE4 = "";

                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(docAttrJson);

                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();

                    //获取文件编码 
                    if (strName == "TEXT1") TEXT1 = strValue.Trim();
                    if (strName == "rpType") rpType = strValue.Trim();
                }

                //TEXT2,TEXT4有删除线，TEXT3没有删除线
                if (rpType == "计划")
                {
                    TEXT2 = ""; TEXT3 = "计划/"; TEXT4 = "细则";
                    ENCLOSURE2 = ""; ENCLOSURE3 = "计划/"; ENCLOSURE4 = "细则";
                }
                else
                {
                    TEXT2 = "计划"; TEXT3 = "/细则"; TEXT4 = "";
                    ENCLOSURE2 = "计划"; ENCLOSURE3 = "/细则"; ENCLOSURE4 = "";
                }

                htUserKeyWord.Add("FILECODE", fileCode);
                htUserKeyWord.Add("TEXT1", TEXT1);//X强制

                htUserKeyWord.Add("TEXT2", TEXT2);//有删除线
                htUserKeyWord.Add("TEXT3", TEXT3);//没有删除线
                htUserKeyWord.Add("TEXT4", TEXT4);//有删除线

                htUserKeyWord.Add("ENCLOSURE", title);//附件
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
