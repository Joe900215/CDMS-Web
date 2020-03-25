using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Script.Serialization;
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

namespace AVEVA.CDMS.HXEPC_Plugins
{
    public class DocumentCommon
    {
        //填写著录表属性内容
        internal static bool SetGataloguDocAttr(CataloguDoc caDoc, string CataAttrJson) {
            try
            {
                JArray jaCataAttr = (JArray)JsonConvert.DeserializeObject(CataAttrJson);
                foreach (JObject joAttr in jaCataAttr)
                {
                    if (joAttr["reference"] != null) { caDoc.CA_REFERENCE = joAttr["reference"].ToString(); }
                    if (joAttr["volumenumber"] != null) { caDoc.CA_VOLUMENUMBER = joAttr["volumenumber"].ToString(); }
                    if (joAttr["responsibility"] != null) { caDoc.CA_FILETITLE = joAttr["responsibility"].ToString(); }
                    if (joAttr["responsibility"] != null) { caDoc.CA_RESPONSIBILITY = joAttr["responsibility"].ToString(); }
                    if (joAttr["desc"] != null) { caDoc.CA_FILETITLE = joAttr["desc"].ToString(); }
                    if (joAttr["page"] != null) { caDoc.CA_PAGE = joAttr["page"].ToString(); }
                    if (joAttr["share"] != null) { caDoc.CA_NUMBER = joAttr["share"].ToString(); }
                    if (joAttr["medium"] != null) { caDoc.CA_MEDIUM = joAttr["medium"].ToString(); }
                    if (joAttr["languages"] != null) { caDoc.CA_LANGUAGES = joAttr["languages"].ToString(); }
                    if (joAttr["procode"] != null) { caDoc.CA_PROCODE = joAttr["procode"].ToString(); }
                    if (joAttr["proname"] != null) { caDoc.CA_PRONAME = joAttr["proname"].ToString(); }
                    if (joAttr["major"] != null) { caDoc.CA_MAJOR = joAttr["major"].ToString(); }
                    if (joAttr["crew"] != null) { caDoc.CA_CREW = joAttr["crew"].ToString(); }
                    if (joAttr["factoryCode"] != null) { caDoc.CA_FACTORY = joAttr["factorycode"].ToString(); }
                    if (joAttr["factoryname"] != null) { caDoc.CA_FACTORYNAME = joAttr["factoryname"].ToString(); }
                    if (joAttr["systemcode"] != null) { caDoc.CA_SYSTEM = joAttr["systemcode"].ToString(); }
                    if (joAttr["systemname"] != null) { caDoc.CA_SYSTEMNAME = joAttr["systemname"].ToString(); }
                    if (joAttr["relationfilecode"] != null) { caDoc.CA_RELATIONFILECODE = joAttr["relationfilecode"].ToString(); }
                    if (joAttr["relationfilename"] != null) { caDoc.CA_RELATIONFILENAME = joAttr["relationfilename"].ToString(); }
                    if (joAttr["filespec"] != null) { caDoc.CA_FILESPEC = joAttr["filespec"].ToString(); }
                    if (joAttr["fileunit"] != null) { caDoc.CA_FILEUNIT = joAttr["fileunit"].ToString(); }
                    if (joAttr["secretgrade"] != null) { caDoc.CA_SECRETGRADE = joAttr["secretgrade"].ToString(); }
                    if (joAttr["keepingtime"] != null) { caDoc.CA_KEEPINGTIME = joAttr["keepingtime"].ToString(); }
                    if (joAttr["filelistcode"] != null) { caDoc.CA_FILELISTCODE = joAttr["filelistcode"].ToString(); }
                    if (joAttr["filelisttime"] != null) { caDoc.CA_FILELISTTIME = joAttr["filelisttime"].ToString(); }
                    if (joAttr["racknumber"] != null) { caDoc.CA_RACKNUMBER = joAttr["racknumber"].ToString(); }
                    if (joAttr["note"] != null) { caDoc.CA_NOTE = joAttr["note"].ToString(); }
                    if (joAttr["workClass"] != null) { caDoc.CA_WORKTYPE = joAttr["workClass"].ToString(); }
                    if (joAttr["workSub"] != null) { caDoc.CA_WORKSUBTIEM = joAttr["workSub"].ToString(); }
                    if (joAttr["department"] != null) { caDoc.CA_UNIT = joAttr["department"].ToString(); }
                    if (joAttr["receiveType"] != null) { caDoc.CA_FILETYPE = joAttr["receiveType"].ToString(); }
                    if (joAttr["fNumber"] != null) { caDoc.CA_FLOWNUMBER = joAttr["fNumber"].ToString(); }
                    if (joAttr["edition"] != null) { caDoc.CA_EDITION = joAttr["edition"].ToString(); }

                    //保存主送方代码和发送方代码
                    if (joAttr["sendUnitCode"] != null) { caDoc.CA_SENDERCODE = joAttr["sendUnitCode"].ToString(); }
                    if (joAttr["recUnitCode"] != null) { caDoc.CA_MAINFEEDERCODE = joAttr["recUnitCode"].ToString(); }
                    if (joAttr["copyUnitCode"] != null) { caDoc.CA_COPYCODE = joAttr["copyUnitCode"].ToString(); }

                    //批量导入运营类文件时所用到的属性
                    if (joAttr["design"] != null) { caDoc.CA_DESIGN = joAttr["design"].ToString(); }
                    if (joAttr["approvtime"] != null) { caDoc.CA_APPROVTIME = joAttr["approvtime"].ToString(); }
                    if (joAttr["edition"] != null) { caDoc.CA_EDITION = joAttr["edition"].ToString(); }
                    if (joAttr["mainfeeder"] != null) { caDoc.CA_MAINFEEDER = joAttr["mainfeeder"].ToString(); }
                    if (joAttr["copy"] != null) { caDoc.CA_COPY = joAttr["copy"].ToString(); }
                    if (joAttr["senddate"] != null) { caDoc.CA_SENDDATE = joAttr["senddate"].ToString(); }
                    if (joAttr["ifreply"] != null) { caDoc.CA_IFREPLY = joAttr["ifreply"].ToString(); }
                    if (joAttr["replydate"] != null) { caDoc.CA_REPLYDATE = joAttr["replydate"].ToString(); }
                    if (joAttr["replycode"] != null) { caDoc.CA_REPLYCODE = joAttr["replycode"].ToString(); }
                    if (joAttr["replytime"] != null) { caDoc.CA_REPLYTIME = joAttr["replytime"].ToString(); }

                    caDoc.SaveAttrData();
                }
            }
            catch {
                return false;
            }
            return true;
        }


        /// <summary>
        /// 获取文件发文编码
        /// </summary>
        /// <param name="dbsource"></param>
        /// <param name="RootProjectCode"></param>
        /// <param name="docType"></param>
        /// <param name="strSendCompany"></param>
        /// <param name="strRecCompany"></param>
        /// <returns></returns>
        internal static string getDocSendCode(DBSource dbsource, string RootProjectCode, string docType, string strSendCompany, string strRecCompany)
        {
            //获取文档前缀
            string sendCompanyCode = strSendCompany.IndexOf("__") >= 0 ? strSendCompany.Substring(0, strSendCompany.IndexOf("__")) : strSendCompany;

            string recCompanyCode = strRecCompany.IndexOf("__") >= 0 ? strRecCompany.Substring(0, strRecCompany.IndexOf("__")) : strRecCompany;

            string number = getDocNumber(dbsource,RootProjectCode,docType,sendCompanyCode,recCompanyCode);

            //编码前缀
            string filecode = "";
            if (!string.IsNullOrEmpty(RootProjectCode))
            {
                //项目管理类

                filecode = RootProjectCode + "-" + sendCompanyCode + "-" + recCompanyCode + "-" + docType + "-" + number;
            }
            else
            {
                //运营管理类

                filecode = sendCompanyCode + "-" + recCompanyCode + "-" + docType + "-"+number;
            }
            return filecode;
        }
        /// <summary>
        /// 获取文件发文编码里的流水号
        /// </summary>
        /// <param name="dbsource"></param>
        /// <param name="RootProjectCode"></param>
        /// <param name="docType"></param>
        /// <param name="strSendCompany"></param>
        /// <param name="strRecCompany"></param>
        /// <returns></returns>

        internal static string getDocNumber(DBSource dbsource, string RootProjectCode, string docType, string strSendCompany, string strRecCompany)
        {
            try
            {
                //string RootProjectCode = proj.GetValueByKeyWord("DESIGNPROJECT_CODE");

                //获取文档前缀
                string sendCompanyCode = strSendCompany.IndexOf("__") >= 0 ? strSendCompany.Substring(0, strSendCompany.IndexOf("__")) : strSendCompany;

                string recCompanyCode = strRecCompany.IndexOf("__") >= 0 ? strRecCompany.Substring(0, strRecCompany.IndexOf("__")) : strRecCompany;


                //编码前缀
                string strPrefix = "";
                if (!string.IsNullOrEmpty(RootProjectCode))
                {
                    //项目管理类
                    //strPrefix = RootProjectCode + "-" + sendCompanyCode + "-" + recCompanyCode + "-LET-" + "SLET";
                    strPrefix = RootProjectCode + "-" + sendCompanyCode + "-" + recCompanyCode + "-" + docType + "-";
                }
                else
                {
                    //运营管理类
                    //strPrefix = sendCompanyCode + "-" + recCompanyCode + "-LET-" + "SLET";
                    strPrefix = sendCompanyCode + "-" + recCompanyCode + "-" + docType + "-";
                }

                List<Doc> docList = dbsource.SelectDoc(string.Format(
                    "select * from CDMS_Doc where o_itemname like '%{0}[0-9]%' and o_dmsstatus !=10 order by o_itemname",
                    strPrefix));
                if (docList == null || docList.Count == 0)
                {
                    //return "SLET"+"00001"; "-" + docType + "-" + commType + docType;
                    return "001";
                }
                else
                {
                    Doc doc = docList[docList.Count - 1];

                    int tempNum = Convert.ToInt32(doc.O_itemname.Substring(strPrefix.Length, 3));
                    //3位数，不够位数补零
                    //return "SLET" + (tempNum + 1).ToString("d5");
                    return (tempNum + 1).ToString("d3");
                }
            }
            catch
            {
                //return "SLET" + "00001";
                return "001";
            }
        }

    }
}
