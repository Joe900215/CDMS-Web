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
    public class A6
    {
        internal static Hashtable GetHashtable(Doc doc, string docType, string fileCode, string title, string docAttrJson)
        {
            Hashtable htUserKeyWord = new Hashtable();
            try
            {
                string TEXT2 = "", select1 = "", select2 = "", select3 = "";

                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(docAttrJson);

                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();

                    //获取文件编码 
                    if (strName == "TEXT2") TEXT2 = strValue.Trim();
                    if (strName == "select1") select1 = strValue.Trim();
                    if (strName == "select2") select2 = strValue.Trim();
                    if (strName == "select3") select3 = strValue.Trim();
                    
                }
                if (select1.ToLower() == "true")
                    select1 = "☑";
                else
                    select1 = "□";
                if (select2.ToLower() == "true")
                    select2 = "☑";
                else
                    select2 = "□";
                if (select3.ToLower() == "true")
                    select3 = "☑";
                else
                    select3 = "□";
      
                htUserKeyWord.Add("FILECODE", fileCode);
                htUserKeyWord.Add("TEXT1", title);//工程名称
                htUserKeyWord.Add("TEXT2", TEXT2);//施工组织设计（项目管理实施规划）
                htUserKeyWord.Add("SELECT1", select1);
                htUserKeyWord.Add("SELECT2", select2);
                htUserKeyWord.Add("SELECT3", select3);
            }
            catch
            {

            }
            return htUserKeyWord;
        }
        
    }
}
