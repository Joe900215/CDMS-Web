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
    public class A7
    {
        internal static Hashtable GetHashtable(Doc doc, string docType, string fileCode, string title, string docAttrJson)
        {
            Hashtable htUserKeyWord = new Hashtable();
            try
            {
                string rpType = "", TEXT1 = "", TEXT2 = "", TEXT3 = "", TEXT4 = "",
                    TEXT5 = "", TEXT6 = "";

                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(docAttrJson);

                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();

                    //获取文件编码 
                    if (strName == "rpType") rpType = strValue.Trim();
                }

                if (rpType == "主要管理人员") { 
                    TEXT1="";TEXT2="主要管理人员";
                    TEXT3 = "特殊工种"; TEXT4 = "";
                    TEXT5 = "特种作业人员"; TEXT6 = "";
                }
                if (rpType == "特殊工种")
                {
                    TEXT1 = "主要管理人员"; TEXT2 = "";
                    TEXT3 = ""; TEXT4 = "特殊工种";
                    TEXT5 = "特种作业人员"; TEXT6 = "";
                }
                if (rpType == "特种作业人员")
                {
                    TEXT1 = "主要管理人员"; TEXT2 = "";
                    TEXT3 = "特殊工种"; TEXT4 = "";
                    TEXT5 = ""; TEXT6 = "特种作业人员";
                }
                htUserKeyWord.Add("FILECODE", fileCode);
                //htUserKeyWord.Add("TEXT1", title);//工程名称
                //htUserKeyWord.Add("TEXT2", rpType);//施工组织设计（项目管理实施规划）
                htUserKeyWord.Add("TEXT1", TEXT1);//主要管理人员 有删除线
                htUserKeyWord.Add("TEXT2", TEXT2);//主要管理人员 无删除线
                htUserKeyWord.Add("TEXT3", TEXT3);//特殊工种 有删除线
                htUserKeyWord.Add("TEXT4", TEXT4);//特殊工种 无删除线
                htUserKeyWord.Add("TEXT5", TEXT5);//特种作业人员 有删除线
                htUserKeyWord.Add("TEXT6", TEXT6);//特种作业人员 无删除线
                htUserKeyWord.Add("ENCLOSURE", "相关资格证件");//施工组织设计（项目管理实施规划）
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
                        string uname = joContent["name"].ToString();    //姓名
                        string jobs = joContent["jobs"].ToString();     //岗位/工种
                        string certiName = joContent["certiName"].ToString();       //证件名称
                        string certiSerial = joContent["certiSerial"].ToString();   //证件编号
                        string issueUnit = joContent["issueUnit"].ToString();       //发证单位
                        string strExpiryDate = joContent["expiryDate"].ToString();     //有效期

                        DateTime dt= Convert.ToDateTime(strExpiryDate);
                        string expiryDate = dt.ToString("d");

                        list3.Add(uname);
                        list3.Add(jobs);
                        list3.Add(certiName);
                        list3.Add(certiSerial);
                        list3.Add(issueUnit);
                        list3.Add(expiryDate);

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
