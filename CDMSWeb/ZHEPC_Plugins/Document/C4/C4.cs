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
    public class C4
    {
        internal static Hashtable GetHashtable(Doc doc, string docType, string fileCode, string title, string docAttrJson)
        {
            Hashtable htUserKeyWord = new Hashtable();
            try
            {
                string rpType = "", TEXT1 = "", TEXT2 = "", TEXT3 = "",
                    TEXT4 = "", CONTENT = "", CONTENT2 = "", CONTENT3 = "";

                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(docAttrJson);

                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();

                    //获取文件编码 
                    if (strName == "TEXT1") TEXT1 = strValue.Trim();
                    if (strName == "TEXT2") TEXT2 = strValue.Trim();
                    if (strName == "TEXT3") TEXT3 = strValue.Trim();
                    if (strName == "TEXT4") TEXT4 = strValue.Trim();

                    if (strName == "CONTENT") CONTENT = strValue.Trim();
                    if (strName == "CONTENT2") CONTENT2 = strValue.Trim();
                    if (strName == "CONTENT3") CONTENT3 = strValue.Trim();
                }

                htUserKeyWord.Add("FILECODE", fileCode);
                htUserKeyWord.Add("TEXT1", TEXT1);//建设单位
                htUserKeyWord.Add("TEXT2", TEXT2);//监理单位
                htUserKeyWord.Add("TEXT3", TEXT3);//承包单位
                htUserKeyWord.Add("TEXT4", TEXT4);//说明

                htUserKeyWord.Add("CONTENT", CONTENT);//交底内容
                htUserKeyWord.Add("CONTENT2", CONTENT2);//交底内容
                htUserKeyWord.Add("CONTENT3", CONTENT3);//交底内容

                //htUserKeyWord.Add("ENCLOSURE", TEXT1);//施工组织设计（项目管理实施规划）
            }
            catch
            {

            }
            return htUserKeyWord;
        }
    
    }
}
