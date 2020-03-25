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
    public class A5
    {
        internal static Hashtable GetHashtable(Doc doc, string docType, string fileCode, string title, string docAttrJson)
        {
            Hashtable htUserKeyWord = new Hashtable();
            try
            {
                string amountCount = "", scaleCount = "", select1 = "", select2 = "", ENCLOSURE1 = "", ENCLOSURE2="";

                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(docAttrJson);

                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();

                    //获取金额合计 
                    if (strName == "amountCount") amountCount = strValue.Trim();
                    //获取比例合计
                    if (strName == "scaleCount") scaleCount = strValue.Trim();

                    if (strName == "select1") select1 = strValue.Trim();
                    if (strName == "select2") select2 = strValue.Trim();
                }

                int headIndex = 1;
                string ENCLOSUREHead = headIndex.ToString() + "、";
                if (select1.ToLower() == "true")
                {
                    ENCLOSURE1 = ENCLOSUREHead + "分包单位资质材料";
                    headIndex = headIndex + 1;
                }

                if (select2.ToLower() == "true")
                {
                    ENCLOSUREHead = headIndex.ToString() + "、";
                    if (ENCLOSURE1 == "")
                        ENCLOSURE1 = ENCLOSUREHead + "分包单位业绩资料";
                    else if (ENCLOSURE2 == "")
                        ENCLOSURE2 = ENCLOSUREHead + "分包单位业绩资料";
                    headIndex = headIndex + 1;
                }
                htUserKeyWord.Add("FILECODE", fileCode);
                htUserKeyWord.Add("TEXT1", title);//工程名称
                htUserKeyWord.Add("ENCLOSURE", ENCLOSURE1);//附件
                htUserKeyWord.Add("ENCLOSURE2", ENCLOSURE2);//附件
                htUserKeyWord.Add("FORM1", amountCount);//金额合计 
                htUserKeyWord.Add("FORM2", scaleCount);//比例合计
            }
            catch
            {

            }
            return htUserKeyWord;
        }

        internal static Hashtable GetAuditHashtable(Doc doc, string docType, string fileCode, string title, string docAttrJson)
        {
            Hashtable htUserKeyWord = new Hashtable();
            try
            {
                string rpType = "", strContent = "";

                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(docAttrJson);

                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();

                    //获取文件编码 
                    if (strName == "auditAry")
                    {
                        strContent = strValue.Trim();
                    }
                }

                //添加人员
                List<string> list3 = new List<string>();

                if (!string.IsNullOrEmpty(strContent))
                {
                    JArray jaContent = (JArray)JsonConvert.DeserializeObject(strContent);
                    foreach (JObject joContent in jaContent)
                    {
                        string uname = joContent["name"].ToString();    //分包工程名称(部位)
                        string quantity = joContent["quantity"].ToString();     //工程量
                        string amount = joContent["amount"].ToString();       //拟分包工程合同额
                        string scale = joContent["scale"].ToString();   //分包工程占全部工程比例
   

                        list3.Add(uname);
                        list3.Add(quantity);
                        list3.Add(amount);
                        list3.Add(scale);

                    }
                }

                //用htAuditDataList传送附件列表到word
                Hashtable htAuditDataList = new Hashtable();
                //word里面表格关键字的设置公式(不需要加"$()") ：表格关键字+":"+已画好表格线的行数+":"+表格列数
                //例如关键字是"DRAWING",画了一行表格线，从第二行起画表格线,每行有6列，则公式是："DRAWING:1:6"
                htAuditDataList.Add("DRAWING", list3);

                return htAuditDataList;
            }
            catch
            {

            }
            return htUserKeyWord;
        }
    }
}
