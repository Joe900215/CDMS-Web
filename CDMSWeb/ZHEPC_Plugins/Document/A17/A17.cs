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
    public class A17
    {
        internal static Hashtable GetHashtable(Doc doc, string docType, string fileCode, string title, string docAttrJson)
        {
            Hashtable htUserKeyWord = new Hashtable();
            try
            {
                string TEXT1 = "", TEXT3 = "",
                    ENCLOSURE1 = "", ENCLOSURE2 = "", ENCLOSURE3 = "",
                    select1 = "", select2 = "", select3="";

                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(docAttrJson);

                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();

                    //获取文件编码 
                    if (strName == "TEXT1") TEXT1 = strValue.Trim();
                    if (strName == "TEXT3") TEXT3 = strValue.Trim();
                    if (strName == "select1") select1 = strValue.Trim();
                    if (strName == "select2") select2 = strValue.Trim();
                    if (strName == "select3") select3 = strValue.Trim();
                }

                int headIndex = 1;
                string ENCLOSUREHead = headIndex.ToString() + "、";
                if (select1.ToLower() == "true")
                {
                    ENCLOSURE1 = ENCLOSUREHead + "索赔的详细理由及经过说明";
                    headIndex = headIndex + 1;
                }

                if (select2.ToLower() == "true")
                {
                    ENCLOSUREHead = headIndex.ToString() + "、";
                    if (ENCLOSURE1 == "")
                        ENCLOSURE1 = ENCLOSUREHead + "索赔金额计算书";
                    else if (ENCLOSURE2 == "")
                        ENCLOSURE2 = ENCLOSUREHead + "索赔金额计算书";
                    headIndex = headIndex + 1;
                }

                if (select3.ToLower() == "true")
                {
                    ENCLOSUREHead = headIndex.ToString() + "、";
                    if (ENCLOSURE1 == "")
                        ENCLOSURE1 = ENCLOSUREHead + "证明材料";
                    else if (ENCLOSURE2 == "")
                        ENCLOSURE2 = ENCLOSUREHead + "证明材料";
                    else if (ENCLOSURE3 == "")
                        ENCLOSURE3 = ENCLOSUREHead + "证明材料";
                }

                htUserKeyWord.Add("FILECODE", fileCode);
                htUserKeyWord.Add("TEXT1", TEXT1);//XX条规定
                htUserKeyWord.Add("TEXT2", title);//XX的原因
                htUserKeyWord.Add("TEXT3", TEXT3);//金额（大写）
                htUserKeyWord.Add("ENCLOSURE1", ENCLOSURE1);//附件
                htUserKeyWord.Add("ENCLOSURE2", ENCLOSURE2);//附件
                htUserKeyWord.Add("ENCLOSURE3", ENCLOSURE3);//附件
            }
            catch
            {

            }
            return htUserKeyWord;
        }

    }
}
