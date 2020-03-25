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
    public class ReplyLetter
    {
        /// <summary>
        /// 回复信函时，获取占号文档的信息
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="DocKeyword"></param>
        /// <returns></returns>
        public static JObject GetPreReplyLetterInfo(string sid, string DocKeyword)
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

                Doc ddoc = dbsource.GetDocByKeyWord(DocKeyword);


                if (ddoc == null)
                {
                    reJo.msg = "参数错误！文件不存在！";
                    return reJo.Value;
                }
                Doc m_Doc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;

                CataloguDoc cadoc = new CataloguDoc();
                cadoc.doc = (m_Doc.ShortCutDoc == null ? m_Doc : m_Doc.ShortCutDoc);
                if (cadoc.doc == null)
                {
                    reJo.msg = "参数错误！发文文件不存在！";
                    return reJo.Value;
                }

                string recCode = cadoc.CA_SENDCODE;
                //precode = cadoc.doc.GetAttrDataByKeyWord("CA_SENDCODE").ToString;

                string strSql = string.Format(
                    "select * from CDMS_Doc as cd inner join " +
                    "(select Itemno, CA_SENDCODE AS SENDCODE from User_CATALOGUING " +
                    "  where CA_RECEIPTCODE='{0}' AND CA_ATTRTEMP='PRECODE')" +
                    " as lf " +
                    "on  cd.o_itemno = lf.Itemno " +
                    " where cd.o_dmsstatus != 10 order by lf.SENDCODE ",
                    recCode);

                List<Doc> docList = dbsource.SelectDoc(strSql);
                if (docList == null || docList.Count == 0)
                {
                    reJo.msg = "未找到预先占用的发文编号！";
                    return reJo.Value;
                }

                CataloguDoc preCodeDoc = new CataloguDoc();

                preCodeDoc.doc = docList[0];

                //预发文编码
                string preCode = preCodeDoc.CA_SENDCODE;
                //获取收文单位编码
                string recUnitCode = cadoc.CA_SENDERCODE; //preCodeDoc.CA_MAINFEEDERCODE;
                string recUnitDesc = cadoc.CA_SENDER; //preCodeDoc.CA_MAINFEEDER;
                //获取占号的文档关键字
                string preDocKeyword = preCodeDoc.doc.KeyWord;

                string projectKeyword = preCodeDoc.Project.KeyWord;

                JObject joData = new JObject(
                    new JProperty("SendCode", preCode),
                    new JProperty("RecCode", recCode),
                    new JProperty("MainfeederCode", recUnitCode),
                    new JProperty("MainfeederDesc", recUnitDesc),
                    new JProperty("DocKeyword", preDocKeyword),
                    new JProperty("ProjectKeyword", projectKeyword)
                    );

                reJo.data = new JArray(joData);
                reJo.success = true;
                return reJo.Value;
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
