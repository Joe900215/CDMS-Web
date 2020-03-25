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
    public class A54
    {
        internal static Hashtable GetHashtable(Doc doc, string docType, string fileCode, string title, string docAttrJson)
        {
            Hashtable htUserKeyWord = new Hashtable();
            try
            {
                string TEXT1 = "", select1 = "", select2 = "", ENCLOSURE1 = "", ENCLOSURE2 = "";

                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(docAttrJson);

                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();

                    //获取文件编码 
                    if (strName == "TEXT1") TEXT1 = strValue.Trim();
                    if (strName == "select1") select1 = strValue.Trim();
                    if (strName == "select2") select2 = strValue.Trim();
                }

                int sIndex = 0;
                if (select1.ToLower() == "true")
                {
                    sIndex = sIndex + 1;
                    ENCLOSURE1 = sIndex + "、安全文明施工二次策划";
                }

                if (select2.ToLower() == "true")
                {
                    sIndex = sIndex + 1;
                    if (ENCLOSURE1 == "")
                        ENCLOSURE1 = sIndex + "、施工安全文明施工措施费使用计划";
                    else
                        ENCLOSURE2 = sIndex + "、施工安全文明施工措施费使用计划";
                }
                htUserKeyWord.Add("FILECODE", fileCode);
                htUserKeyWord.Add("TEXT1", TEXT1);//工程名称
                htUserKeyWord.Add("ENCLOSURE", ENCLOSURE1);//"1、安全文明施工二次策划");
                htUserKeyWord.Add("ENCLOSURE2", ENCLOSURE2);//"2、施工安全文明施工措施费使用计划");
            }
            catch
            {

            }
            return htUserKeyWord;
        }
    
    }
}
