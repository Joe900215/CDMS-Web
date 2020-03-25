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
    public class A20
    {
        internal static Hashtable GetHashtable(Doc doc, string docType, string fileCode, string title, string docAttrJson)
        {
            Hashtable htUserKeyWord = new Hashtable();
            try
            {
                string rpType = "";
                string rpType2 = "";
                string rpType3 = "", CONTENT="";

                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(docAttrJson);

                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();

                    //获取文件编码 
                    if (strName == "rpType") rpType = strValue.Trim();
                    if (strName == "rpType2") rpType2 = strValue.Trim();
                    if (strName == "title2") rpType3 = strValue.Trim();
                    if (strName == "CONTENT") CONTENT = strValue.Trim();
                }
                DateTime dt1 = Convert.ToDateTime(rpType);
                rpType = dt1.ToString("yyyy年MM月dd日");
                DateTime dt2 = Convert.ToDateTime(rpType2);
                rpType2 = dt2.ToString("yyyy年MM月dd日");

                string TEXT2A = dt1.ToString("d");
                string TEXT2Y = dt1.Year.ToString();
                string TEXT2M = dt1.Month.ToString();
                string TEXT2D = dt1.Day.ToString();

                string TEXT3A = dt2.ToString("d");
                string TEXT3Y = dt2.Year.ToString();
                string TEXT3M = dt2.Month.ToString();
                string TEXT3D = dt2.Day.ToString();

                htUserKeyWord.Add("FILECODE", fileCode);
                htUserKeyWord.Add("TEXT1", title);//工程名称
                //htUserKeyWord.Add("TEXT2", rpType);//时间1
                htUserKeyWord.Add("TEXT4", rpType3);//原因
                //htUserKeyWord.Add("TEXT3", rpType3);//时间2

                htUserKeyWord.Add("TEXT2Y", TEXT2Y);//时间1
                htUserKeyWord.Add("TEXT2M", TEXT2M);//
                htUserKeyWord.Add("TEXT2D", TEXT2D);//

                htUserKeyWord.Add("TEXT3Y", TEXT3Y);//时间2
                htUserKeyWord.Add("TEXT3M", TEXT3M);//
                htUserKeyWord.Add("TEXT3D", TEXT3D);//

                htUserKeyWord.Add("ENCLOSURE", CONTENT);//附件
            }
            catch
            {

            }
            return htUserKeyWord;
        }

    }
}
