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
    public class A34
    {
        internal static Hashtable GetHashtable(Doc doc, string docType, string fileCode, string title, string docAttrJson)
        {
            Hashtable htUserKeyWord = new Hashtable();
            try
            {
                string title2 = "";
                string title3 = "";

                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(docAttrJson);

                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();

                    //获取文件编码 
                    if (strName == "title2") title2 = strValue.Trim();
                    if (strName == "title3") title3 = strValue.Trim();

                }
            
                htUserKeyWord.Add("FILECODE", fileCode);
                htUserKeyWord.Add("TEXT1", title);//工程名称
                htUserKeyWord.Add("TEXT2", title2);//单号
                htUserKeyWord.Add("CONTENT", title3);//内容
                htUserKeyWord.Add("ENCLOSURE", title);//附件
            }
            catch
            {

            }
            return htUserKeyWord;
        }
    //    //创建A3 DOC
    //    public static JObject CreateA3(string sid, string action, string ProjectKeyword, string fileCode, string title, string rpType, string checkList,
    //string approvList, string filelist)
    //    {
    //        bool success = false;
    //        string msg = "";
    //        JArray jaResult = new JArray();
    //        int total = 0;
    //        try
    //        {
    //            string strDocDesc = title + "施工组织设计报审表";
    //            string strDocType = "";//"A.3施工组织设计报审表";//ISO模板文件名

    //            string strTempDefn = "A3";//定义目录模板


    //            CreatePara cp = CreateContent(sid, action, strDocDesc, strTempDefn, ProjectKeyword, fileCode, filelist);
    //            if (cp.success == false)
    //            {
    //                msg = cp.msg;
    //            }
    //            else
    //            {
    //                string unittype = cp.unitType;
    //                if (unittype == "总承包单位")
    //                { strDocType = "A.3总包施工组织设计报审表"; }
    //                else
    //                {
    //                    strDocType = "A.3施工组织设计报审表";
    //                }
    //                cp.strDocType = strDocType;
    //                Doc item = cp.itemDoc;//获取新建的文档
    //                DBSource dbsource = item.dBSource;
    //                //string unittype = cp.unitType;//
    //                string codeDescStr = cp.CodeDescStr;//附件描述属性

    //                string[] docNumArray = (string.IsNullOrEmpty(docNum) ? "" : docNum).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
    //                //定义序号
    //                //int projCodeIndex, unitIndex, sendIndex, professionIndex, docTypeIndex, runNumIndex;
    //                string projCode = docNumArray[0];//工程号
    //                string unit = docNumArray[1];//机组
    //                string send = docNumArray[2];//发文单位
    //                string profession = docNumArray[3];//专业
    //                string docType = docNumArray[4];//文档类型
    //                string runNum = docNumArray[5];//流水号

    //                //向word文档传递参数
    //                Hashtable htUserKeyWord = new Hashtable();
    //                htUserKeyWord.Add("DOCNUMBER", docNum);
    //                htUserKeyWord.Add("TITLE", title);//主题
    //                htUserKeyWord.Add("CONTENT", content);//内容
    //                htUserKeyWord.Add("PREPAREDSIGNTIME", DateTime.Now.ToString("yyyy年MM月dd日"));
    //                //htUserKeyWord.Add("PREPAREDSIGN1", PadName(dbsource.LoginUser.Description));//获取名字

    //                ////设置工程联系单文档属性

    //                //item.GetAttrDataByKeyWord("CD_PROFESSION").Code = (profession);
    //                //item.GetAttrDataByKeyWord("CD_TYPE").Code = (strDocType);//文档类型
    //                //item.GetAttrDataByKeyWord("CD_DTYPE").Code = (docType);//文档类型
    //                //item.GetAttrDataByKeyWord("CD_COMPANYTYPE").Code = (unittype);//发起单位类型
    //                //item.GetAttrDataByKeyWord("CD_FILENO").Code = (docNum);
    //                //item.GetAttrDataByKeyWord("CD_MakeDate").Code = (DateTime.Now.ToString("yyyy-MM-dd"));
    //                //item.GetAttrDataByKeyWord("CD_ENCLOSURE").Code = (codeDescStr);//附件
    //                //item.GetAttrDataByKeyWord("CD_TITLE").Code = (title);//主题
    //                //item.GetAttrDataByKeyWord("CD_CONTENT").Code = (content);//内容
    //                //A3数据-----------------------------------------------------------------------------------------------------------------------------
    //                item.GetAttrDataByKeyWord("CD_TITLE1").SetCodeDesc(title);

    //                bool flag = item.AttrDataList.SaveData();
    //                item.Modify();
    //                List<Doc> m_DocList = new List<Doc>();
    //                m_DocList.Add(item);

    //                string format = "insert into User_AutoSendFlowNo (DocNo,ProjectCode,Unit,Send,Receive,Profession,Type,FlowNo) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')";
    //                format = string.Format(format, new object[] { item.O_itemno, projCode, unit, send, "", profession, docType, runNum });
    //                dbsource.DBExecuteSQL(format);

    //                string format2 = "insert into User_GEDIUnitedWorkflowContent (ITEMNO, UserName, DesignContent, CheckName, AuditName) values ({0}, '{1}', '{2}', '{3}', '{4}')";
    //                string code = "";
    //                string str11 = "";
    //                if (!string.IsNullOrEmpty(checkList))
    //                {
    //                    User checker = dbsource.GetUserByKeyWord(checkList);
    //                    code = checker.Code;
    //                }
    //                if (!string.IsNullOrEmpty(approvList))
    //                {
    //                    User approver = dbsource.GetUserByKeyWord(approvList);
    //                    str11 = approver.Code;
    //                }
    //                dbsource.DBExecuteCommand(string.Format(format2, new object[] { item.ID, dbsource.LoginUser.Code, content.Trim().Replace("\r\n", "|^|"), code, str11 }));
    //                cp.checkerList = code;
    //                cp.auditorList = str11;

    //                saveDoc(cp, htUserKeyWord);
    //                if (cp.success == true)
    //                {
    //                    success = true;
    //                    jaResult.Add(cp.reJo);
    //                }
    //                else { msg = cp.msg; }
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            msg = e.Message;

    //        }
    //        return AVEVA.CDMS.WebApi.Helper.MyFunction.WriteJObjectResult(success, total, msg, jaResult);
    //    }
    }
}
