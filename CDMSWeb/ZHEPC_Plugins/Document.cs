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
//using LinqToDB;

namespace AVEVA.CDMS.ZHEPC_Plugins
{
    public class Document
    {

        //线程锁 
        internal static Mutex muxConsole = new Mutex();

        /// <summary>
        /// 获取创建信函表单的默认配置
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="ProjectKeyword"></param>
        /// <returns></returns>
        public static JObject GetFileCodeDefaultInfo(string sid, string ProjectKeyword)
        {
            return Report.GetFileCodeDefaultInfo(sid, ProjectKeyword);
        }

        public static JObject GetCrewList(string sid, string ProjectKeyword,string Filter)
        {
            return Report.GetCrewList(sid, ProjectKeyword,Filter);
        }

        public static JObject GetUnitList(string sid, string ProjectKeyword, string Filter)
        {
            return Report.GetUnitList(sid, ProjectKeyword, Filter);
        }

        public static JObject GetProfessionList(string sid, string ProjectKeyword, string Filter)
        {
            return Report.GetProfessionList(sid, ProjectKeyword, Filter);
        }

        public static JObject GetKindList(string sid, string ProjectKeyword, string Filter)
        {
            return Report.GetKindList(sid, ProjectKeyword, Filter);
        }


        public static JObject GetFileCodeNumber(string sid, string FileCodePerfix)
        {
            return Report.GetFileCodeNumber(sid, FileCodePerfix);
        }

        public static JObject CreateDocument(string sid, string ProjectKeyword, string documentType, string fileCode, string title, string docAttrJson)
        {
            return Report.CreateDocument(sid, ProjectKeyword, documentType, fileCode, title, docAttrJson);
        }

        public static JObject CreateDocShortcut(string sid, string ProjectKeyword, string documentType, string DocList, string docAttrJson) { 
            return Report.CreateDocShortcut(sid, ProjectKeyword, documentType, DocList, docAttrJson) ;
        }

        public static JObject GetNavNewDocs(string sid, string page, string limit)
        { 
            return Report.GetNavNewDocs(sid, page, limit);
        }

        public static JObject AfterMountCloseFile(string sid, string DocKeyword)
        {
            return Report.AfterMountCloseFile(sid,DocKeyword);
        }

        public static JObject UnCloseFile(string sid, string DocKeyword)
        {
            return Report.UnCloseFile(sid, DocKeyword);
        }
    }
}
