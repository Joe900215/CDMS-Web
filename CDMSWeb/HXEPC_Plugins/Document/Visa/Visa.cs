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
    public class Visa
    {
        /// <summary>
        /// 获取创建认质认价表单的默认配置
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="ProjectKeyword"></param>
        /// <returns></returns>
        public static JObject GetDraftVisaDefault(string sid, string ProjectKeyword, string DraftOnProject)
        {
            return DraftGetDefault.GetDefaultInfo(sid, ProjectKeyword, DraftOnProject);

        }


        //线程锁 
        internal static Mutex muxConsole = new Mutex();
        public static JObject DraftVisa(string sid, string DocKeyword, string FileName,
            string ProjectKeyword, string DocAttrJson, string CataAttrJson, string FileListJson)
        {
            ExReJObject reJo = new ExReJObject();

            try
            {
                User curUser = DBSourceController.GetCurrentUser(sid);
                if (curUser == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }

                DBSource dbsource = curUser.dBSource;
                if (dbsource == null)
                {
                    reJo.msg = "登录验证失败！请尝试重新登录！";
                    return reJo.Value;
                }


                Project m_Project = dbsource.GetProjectByKeyWord(ProjectKeyword);


                if (m_Project == null)
                {
                    reJo.msg = "参数错误！文件夹不存在！";
                    return reJo.Value;
                }

                #region 获取信函参数内容

                //获取信函参数内容
                string fileCode = "", projectName = "", contractCode = "",
                   sendDate = "", sendCode = "", recCode = "",
                   recType = "", materialType = "", fileId = "";

                //{ name: 'projectName', value: projectName },
                //{ name: 'construcSite', value: construcSite },
                //{ name: 'instrucManName', value: instrucManName },
                //{ name: 'noticeDate', value: noticeDate },
                //{ name: 'maxMoney', value: maxMoney },
                //{ name: 'smallMoney', value: smallMoney },

                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(DocAttrJson);

                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();

                    ////获取函件编号
                    //if (strName == "documentCode") documentCode = strValue.Trim();

                    //获取文件编码 
                    if (strName == "fileCode") fileCode = strValue.Trim();

                    //获取项目名称 
                    else if (strName == "projectName") projectName = strValue;

                    //获取合同号 
                    else if (strName == "contractCode") contractCode = strValue;

                    //获取发文日期
                    else if (strName == "sendDate") sendDate = strValue;

                    //获取发文编码
                    else if (strName == "sendCode") sendCode = strValue;

                    //获取收文编码
                    else if (strName == "recCode") recCode = strValue;

                    //获取来文类型
                    else if (strName == "recType") recType = strValue;

                    //获取物资类型
                    else if (strName == "materialType") materialType = strValue;

                    //获取物资类型
                    else if (strName == "fileId") fileId = strValue;
                }


                if (string.IsNullOrEmpty(fileCode))
                {
                    reJo.msg = "请填写文件编号！";
                    return reJo.Value;
                }
                else if (string.IsNullOrEmpty(projectName))
                {
                    reJo.msg = "请填写合同名称！";
                    return reJo.Value;
                }


                #endregion



                #region 根据信函模板，生成信函文档

                //获取立项单文档所在的目录
                //Project m_Project = m_NewProject;

                List<TempDefn> docTempDefnByCode = m_Project.dBSource.GetTempDefnByCode("CATALOGUING");
                TempDefn docTempDefn = (docTempDefnByCode != null && docTempDefnByCode.Count > 0) ? docTempDefnByCode[0] : null;
                if (docTempDefn == null)
                {
                    reJo.msg = "没有与其相关的模板管理，创建无法正常完成";
                    return reJo.Value;
                }


                string docCode = fileCode + " " + projectName + "签证报审单";


                //文档名称
                //Doc docItem = m_Project.NewDoc("", filename, "", docTempDefn);

                Doc ddoc = dbsource.GetDocByKeyWord(DocKeyword);

                //docItem. = filename;

                if (ddoc == null)
                {
                    reJo.msg = "新建认质认价单出错！";
                    return reJo.Value;
                }
                Doc docItem = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;

                docItem.TempDefn = docTempDefn;
                docItem.O_itemname = docCode;
                docItem.O_filename = FileName;
                docItem.Modify();

                #endregion

                #region 获取著录表属性内容
                CataloguDoc caDoc = new CataloguDoc();
                caDoc.doc = docItem;

                DocumentCommon.SetGataloguDocAttr(caDoc, CataAttrJson);

                #endregion

                #region 设置信函文档附加属性
                //文件编码
                caDoc.CA_FILECODE = fileCode;
                //工程名称
                caDoc.CA_PRONAME = projectName;
                //合同号
                caDoc.CA_CONTRACTCODE = contractCode;
                //来文编号
                caDoc.CA_RECEIPTCODE = recCode;
                //来文类型
                caDoc.CA_RECTYPE = recType;
                //采购类型
                caDoc.CA_PURCHASETYPE = materialType;
                //文件模板
                caDoc.CA_ATTRTEMP = "VISAFILE";

                //保存项目属性，存进数据库
                caDoc.SaveAttrData();



                #endregion

                string strDocList = "";//获取附件
                if (string.IsNullOrEmpty(strDocList))
                {
                    strDocList = docItem.KeyWord;
                }
                else
                {
                    strDocList = docItem.KeyWord + "," + strDocList;
                }

                //这里刷新数据源，否则创建流程的时候获取不了专业字符串
                DBSourceController.RefreshDBSource(sid);

                reJo.success = true;
                reJo.data = new JArray(new JObject(new JProperty("ProjectKeyword", docItem.Project.KeyWord),
                    new JProperty("DocKeyword", docItem.KeyWord), new JProperty("DocList", strDocList),
                    new JProperty("DocCode", docItem.Code)));

                //reJo.data = new JArray(new JObject(new JProperty("projectKeyword", m_Project.KeyWord)));
                reJo.success = true;
                return reJo.Value;

                //AVEVA.CDMS.WebApi.DBSourceController.RefreshDBSource(sid);


            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(reJo.msg);
            }

            return reJo.Value;
        }
    }
}
