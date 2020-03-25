using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVEVA.CDMS.WebApi;
using AVEVA.CDMS.Server;
using System.Collections;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Threading;

namespace AVEVA.CDMS.HXEPC_Plugins
{
    public class EditLetterText
    {
        public static JObject GetEditLetterTextDefault(string sid, string DocKeyword) {
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
                Doc m_Doc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;

                if (m_Doc == null)
                {
                    reJo.msg = "参数错误！文件不存在！";
                    return reJo.Value;
                }

                CataloguDoc caDoc = new CataloguDoc();
                caDoc.doc = m_Doc;

                string docContent=caDoc.CA_CONTENT;

               JObject joData = new JObject(
                    new JProperty("Content", docContent)
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

        //线程锁 
        internal static Mutex muxConsole = new Mutex();
        /// <summary>
        /// 修改正文内容
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="DocKeyword"></param>
        /// <param name="DocAttrJson"></param>
        /// <returns></returns>
        public static JObject ModiLetterText(string sid, string DocKeyword, string DocAttrJson)
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
                Doc m_Doc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;

                if (m_Doc == null)
                {
                    reJo.msg = "参数错误！文件不存在！";
                    return reJo.Value;
                }


                #region 只有批准人才可以编辑正文
                if (m_Doc.WorkFlow.CuWorkState.Code != "APPROV")
                {
                    reJo.msg = "只有批准人才可以编辑正文！";
                    return reJo.Value;
                }
                bool flag = false;
                foreach (WorkUser wu in m_Doc.WorkFlow.CuWorkState.WorkUserList)
                {
                    if (wu.User == m_Doc.dBSource.LoginUser)
                    {
                        flag = true;
                    }
                }
                if (flag == false)
                {
                    reJo.msg = "只有批准人才可以编辑正文！";
                    return reJo.Value;
                }

                #endregion

                string content = "";

                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(DocAttrJson);

                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();

                    ////获取正文内容
                    if (strName == "content") content = strValue.Trim();

                }

                if (string.IsNullOrEmpty(content))
                {
                    reJo.msg = "请输入正文内容";
                }

                //修改文档属性
                CataloguDoc caDoc = new CataloguDoc();
                caDoc.doc = m_Doc;

                caDoc.CA_CONTENT = content;

                caDoc.SaveAttrData();

                //录入数据进入表单
                #region 录入数据进入表单
                Hashtable htUserKeyWord = new Hashtable();

                htUserKeyWord.Add("CONTENT", content);//信函正文

                //获取即将生成的联系单文件路径
                string locFileName = caDoc.FullPathFile;

                FileInfo info = new FileInfo(locFileName);


                if (System.IO.File.Exists(locFileName))
                {
                   
                    //线程锁 
                    muxConsole.WaitOne();
                    try
                    {
                        //把参数直接写进office
                        CDMSWebOffice office = new CDMSWebOffice
                        {
                            CloseApp = true,
                            VisibleApp = false
                        };
                        office.Release(true);
                        //office.WriteDataToDocument(caDoc.doc, locFileName, htUserKeyWord, htUserKeyWord);
                        office.UpdateFileData(caDoc.doc, locFileName, htUserKeyWord);
                    }
                    catch { }
                    finally
                    {

                        //解锁
                        muxConsole.ReleaseMutex();
                    }
                }
                #endregion


                int length = (int)info.Length;
                caDoc.doc.O_size = new int?(length);
                caDoc.doc.Modify();


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

 
