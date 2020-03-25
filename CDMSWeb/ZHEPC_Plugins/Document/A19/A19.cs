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
    public class A19
    {
        internal static Hashtable GetHashtable(Doc doc, string docType, string fileCode, string title, string docAttrJson)
        {
            Hashtable htUserKeyWord = new Hashtable();
            try
            {
                string rpType = "", TEXT1 = "", TEXT2 = "", TEXT3 = "", 
                    TEXT4 = "", TEXT5 = "", TEXT6 = "", select1 = "", ENCLOSURE1="";

                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(docAttrJson);

                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();

                    //获取文件编码 
                    if (strName == "TEXT1") TEXT1 = strValue.Trim();//时间1
                    if (strName == "TEXT2") TEXT2 = strValue.Trim();//时间2
                    if (strName == "TEXT3") TEXT3 = strValue.Trim();//X元
                    if (strName == "TEXT4") TEXT4 = strValue.Trim();// 预付款
                    if (strName == "TEXT5") TEXT5 = strValue.Trim();//质量
                    if (strName == "TEXT6") TEXT6 = strValue.Trim();//X元
                    if (strName == "select1") select1 = strValue.Trim();
                }


                DateTime dt1 = Convert.ToDateTime(TEXT1);
                string TEXT1A = dt1.ToString("d");
                string TEXT1Y = dt1.Year.ToString();
                string TEXT1M = dt1.Month.ToString();
                string TEXT1D = dt1.Day.ToString();
                DateTime dt2 = Convert.ToDateTime(TEXT2);
                string TEXT2A = dt2.ToString("d");
                string TEXT2Y = dt2.Year.ToString();
                string TEXT2M = dt2.Month.ToString();
                string TEXT2D = dt2.Day.ToString();

                if (select1.ToLower() == "true")
                {
                    ENCLOSURE1 = "产值报表、报表详表及汇总表";
                }

                htUserKeyWord.Add("FILECODE", fileCode);
                htUserKeyWord.Add("TEXT1", TEXT1A);//
                htUserKeyWord.Add("TEXT2", TEXT2A);//

                htUserKeyWord.Add("TEXT1Y", TEXT1Y);//
                htUserKeyWord.Add("TEXT1M", TEXT1M);//
                htUserKeyWord.Add("TEXT1D", TEXT1D);//
                htUserKeyWord.Add("TEXT2Y", TEXT2Y);//
                htUserKeyWord.Add("TEXT2M", TEXT2M);//
                htUserKeyWord.Add("TEXT2D", TEXT2D);//
                htUserKeyWord.Add("TEXT3", TEXT3);//
                htUserKeyWord.Add("TEXT4", TEXT4);//
                htUserKeyWord.Add("TEXT5", TEXT5);//
                htUserKeyWord.Add("TEXT6", TEXT6);//
                htUserKeyWord.Add("ENCLOSURE", ENCLOSURE1);//施工组织设计（项目管理实施规划）
            }
            catch
            {

            }
            return htUserKeyWord;
        }

    }
}
