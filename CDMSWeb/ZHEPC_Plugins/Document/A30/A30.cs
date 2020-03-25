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
    public class A30
    {
        internal static Hashtable GetHashtable(Doc doc, string docType, string fileCode, string title, string docAttrJson)
        {
            Hashtable htUserKeyWord = new Hashtable();
            try
            {
                string rpType = "", TEXT1 = "", TEXT2="",FORM1="",FORM2="",
                    ENCLOSURE1="",ENCLOSURE2="",
                    select1="",select2="",select3="",select4="";

                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(docAttrJson);

                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();

                    //获取文件编码 
                    if (strName == "TEXT1") TEXT1 = strValue.Trim();
                    if (strName == "TEXT2") TEXT2 = strValue.Trim();

                    if (strName == "select1") select1 = strValue.Trim();
                    if (strName == "select2") select2 = strValue.Trim();
                    if (strName == "select3") select3 = strValue.Trim();
                    if (strName == "select4") select4 = strValue.Trim();

                    if (strName == "FORM1") FORM1 = strValue.Trim();
                    if (strName == "FORM2") FORM2 = strValue.Trim();


                }

                //合同约定，有关规定
                if (select1.ToLower() == "true")
                    select1 = "☑";
                else
                    select1 = "□";

                if (select2.ToLower() == "true")
                    select2 = "☑";
                else
                    select2 = "□";

                //附件描述
                int sIndex = 0;
                if (select3.ToLower() == "true")
                {
                    sIndex = sIndex + 1;
                    ENCLOSURE1 = sIndex + ".承包单位资质材料";
                }

                if (select4.ToLower() == "true")
                {
                    sIndex = sIndex + 1;
                    if (ENCLOSURE1 == "")
                        ENCLOSURE1 = sIndex + ".承包单位业绩证明";
                    else
                        ENCLOSURE2 = sIndex + ".承包单位业绩证明";
                }

                htUserKeyWord.Add("FILECODE", fileCode);
                htUserKeyWord.Add("TEXT1", TEXT1);//工程名称
                htUserKeyWord.Add("TEXT2", TEXT2);//施工组织设计（项目管理实施规划）

                htUserKeyWord.Add("SELECT1", select1);//
                htUserKeyWord.Add("SELECT2", select2);//

                htUserKeyWord.Add("FORM1", FORM1);//工程名称
                htUserKeyWord.Add("FORM2", FORM2);//施工组织设计（项目管理实施规划）

                htUserKeyWord.Add("ENCLOSURE", ENCLOSURE1);//施工组织设计（项目管理实施规划）
                htUserKeyWord.Add("ENCLOSURE2", ENCLOSURE2);//施工组织设计（项目管理实施规划）
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
