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
    public class C3
    {
        internal static Hashtable GetHashtable(Doc doc, string docType, string fileCode, string title, string docAttrJson)
        {
            Hashtable htUserKeyWord = new Hashtable();
            try
            {
                string rpType = "", TEXT1 = "", TEXT2 = "", TEXT3 = "",
                    TEXT4 = "", TEXT5 = "", TEXT6 = "", CONTENT = "",
                    select1 = "", select2 = "", select3 = "", select4 = "",
                    select5 = "", select6 = "", select7 = "", select8 = "",
                    select9 = "";

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

                    if (strName == "CONTENT") CONTENT = strValue.Trim();

                    if (strName == "select1") select1 = strValue.Trim();
                    if (strName == "select2") select2 = strValue.Trim();
                    if (strName == "select3") select3 = strValue.Trim();
                    if (strName == "select4") select4 = strValue.Trim();
                    if (strName == "select5") select5 = strValue.Trim();
                    if (strName == "select6") select6 = strValue.Trim();
                    if (strName == "select7") select7 = strValue.Trim();
                    if (strName == "select8") select8 = strValue.Trim();
                    if (strName == "select9") select9 = strValue.Trim();
                }

                //转换时间格式
                DateTime dt = Convert.ToDateTime(TEXT4);
                TEXT4 = dt.ToString("yyyy.MM.dd");

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

                if (select4.ToLower() == "true")
                    select4 = "☑";
                else
                    select4 = "□";

                if (select5.ToLower() == "true")
                    select5 = "☑";
                else
                    select5 = "□";

                if (select6.ToLower() == "true")
                    select6 = "☑";
                else
                    select6 = "□";

                if (select7.ToLower() == "true")
                    select7 = "☑";
                else
                    select7 = "□";

                if (select8.ToLower() == "true")
                    select8 = "☑";
                else
                    select8 = "□";

                if (select9.ToLower() == "true")
                    select9 = "☑";
                else
                    select9 = "□";
            
                htUserKeyWord.Add("FILECODE", fileCode);
                htUserKeyWord.Add("TEXT1", TEXT1);//卷册名称
                htUserKeyWord.Add("TEXT2", TEXT2);//图号
                htUserKeyWord.Add("TEXT3", TEXT3);//提出专业
                htUserKeyWord.Add("TEXT4", TEXT4);//提出时间
                htUserKeyWord.Add("TEXT5", TEXT5);//修改原因
                htUserKeyWord.Add("TEXT6", TEXT6);//估算
                htUserKeyWord.Add("CONTENT", CONTENT);//修改内容

                //htUserKeyWord.Add("ENCLOSURE", rpType);//施工组织设计（项目管理实施规划）

                htUserKeyWord.Add("SELECT1", select1);//
                htUserKeyWord.Add("SELECT2", select2);//
                htUserKeyWord.Add("SELECT3", select3);//
                htUserKeyWord.Add("SELECT4", select4);//
                htUserKeyWord.Add("SELECT5", select5);//
                htUserKeyWord.Add("SELECT6", select6);//
                htUserKeyWord.Add("SELECT7", select7);//
                htUserKeyWord.Add("SELECT8", select8);//
                htUserKeyWord.Add("SELECT9", select9);//
            }
            catch
            {

            }
            return htUserKeyWord;
        }
    
    }
}
