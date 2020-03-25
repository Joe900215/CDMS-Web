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
    public class A37
    {
        internal static Hashtable GetHashtable(Doc doc, string docType, string fileCode, string title, string docAttrJson)
        {
            Hashtable htUserKeyWord = new Hashtable();
            try
            {
                string TEXT1 = "", TEXT2 = "", TEXT3 = "",
                    TEXT4 = "", TEXT5 = "", TEXT6 = "",
                    TEXT7 = "", TEXT8 = "", TEXT9 = "", 
                    CONTENT = "", CONTENT2 = "", CONTENT3 = "";

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
                    if (strName == "TEXT5") TEXT5 = strValue.Trim();
                    if (strName == "TEXT6") TEXT6 = strValue.Trim();
                    if (strName == "TEXT7") TEXT7 = strValue.Trim();
                    if (strName == "TEXT8") TEXT8 = strValue.Trim();
                    if (strName == "TEXT9") TEXT9 = strValue.Trim();

                    if (strName == "CONTENT") CONTENT = strValue.Trim();
                    if (strName == "CONTENT2") CONTENT2 = strValue.Trim();
                    if (strName == "CONTENT3") CONTENT3 = strValue.Trim();
                }

                DateTime dt1 = Convert.ToDateTime(TEXT5);
                TEXT5 = dt1.ToString("yyyy年MM月dd日");
                DateTime dt2 = Convert.ToDateTime(TEXT8);
                TEXT8 = dt2.ToString("yyyy年MM月dd日");

                htUserKeyWord.Add("FILECODE", fileCode);
                htUserKeyWord.Add("TEXT1", TEXT1);//工程量
                htUserKeyWord.Add("TEXT2", TEXT2);//工程地址
                htUserKeyWord.Add("TEXT3", TEXT3);//工程造价
                htUserKeyWord.Add("TEXT4", TEXT4);//工程性质
                htUserKeyWord.Add("TEXT5", TEXT5);//竣工日期
                htUserKeyWord.Add("TEXT6", TEXT6);//工程结构
                htUserKeyWord.Add("TEXT7", TEXT7);//质量总评
                htUserKeyWord.Add("TEXT8", TEXT8);//手续时间
                htUserKeyWord.Add("TEXT9", TEXT9);//XX证明书

                htUserKeyWord.Add("CONTENT", CONTENT);//工程内容
                htUserKeyWord.Add("CONTENT2", CONTENT2);//验收意见
                htUserKeyWord.Add("CONTENT3", CONTENT3);//验收人员
            }
            catch
            {

            }
            return htUserKeyWord;
        }

    }
}
