﻿using System;
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
    public class D4
    {
        internal static Hashtable GetHashtable(Doc doc, string docType, string fileCode, string title, string docAttrJson)
        {
            Hashtable htUserKeyWord = new Hashtable();
            try
            {
                string rpType = "", TEXT1="";

                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(docAttrJson);

                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();

                    //获取文件编码 
                    if (strName == "TEXT1") TEXT1 = strValue.Trim();
                }
            
                htUserKeyWord.Add("FILECODE", fileCode);
                htUserKeyWord.Add("TEXT1", TEXT1);//工程名称
                //htUserKeyWord.Add("TEXT2", rpType);//施工组织设计（项目管理实施规划）
                htUserKeyWord.Add("ENCLOSURE", TEXT1);//施工组织设计（项目管理实施规划）
            }
            catch
            {

            }
            return htUserKeyWord;
        }
    
    }
}
