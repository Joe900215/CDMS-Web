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

namespace AVEVA.CDMS.TBSBIM_Plugins
{
    public class FamilyService
    {

        /// <summary>
        /// 获取族库大类
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public static JObject GetFamilyCategory(string sid)
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

                //获取族库根目录
                List<Project> rootPrjLst = dbsource.RootLocalProjectList.FindAll(p => p.TempDefn.KeyWord == "TBS_FAMILYLIBRARY");
                if (rootPrjLst.Count <= 0 || rootPrjLst[0] == null) {
                    reJo.msg = "参数错误，目录不存在！";
                    return reJo.Value;
                }

                JArray jaData = new JArray();
                
                Project rootPrj = rootPrjLst[0];
                foreach (Project prj in rootPrj.ChildProjectList)
                {
                    jaData.Add(new JObject(
                        new JProperty("ProjectKeyword", prj.KeyWord),
                        new JProperty("ProjectText", prj.ToString)
                        ));

                }
          
                reJo.data = jaData;

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


        public static JObject GetFamilyDocList(string sid, string ProjectKeyWord) { 
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

                Project m_prj = dbsource.GetProjectByKeyWord(ProjectKeyWord);
                if (m_prj == null)
                {
                    reJo.msg = "参数错误，目录不存在！";
                    return reJo.Value;
                }


                List<Doc> docList = dbsource.SelectDoc("select * from CDMS_Doc where o_dmsstatus !=10 and o_projectno=" + m_prj.ID.ToString());

                List<Doc> rfaDocList = new List<Doc>();
                List<Doc> jpgDocList = new List<Doc>();
                
                foreach (Doc doc in docList)
                {
                    string docTempDefn=doc.TempDefn.KeyWord;
                    //获取族文件列表
                    if (docTempDefn == "TBS_FAMILYDOCUMENT")
                    {
                        rfaDocList.Add(doc);
                    }
                    //获取族预览图文件列表
                    else if (docTempDefn == "TBS_FAMILYPREVIEW")
                    {
                        jpgDocList.Add(doc);
                    }
                }

                JArray jaData = new JArray();

                foreach (Doc doc in rfaDocList)
                {

                    string jpgFileName = doc.Code + "预览图";
                    Doc jpgDoc = jpgDocList.Find(d => d.Code == jpgFileName);
      
                    jaData.Add(new JObject(new JProperty("DocKeyword", doc.KeyWord),
                        new JProperty("Title", doc.ToString),
                        new JProperty("JpgDocKeyword", jpgDoc == null ? "" : jpgDoc.KeyWord),
                        new JProperty("JpgFilePath", jpgDoc == null ? "" : jpgDoc.FullPathFile),
                        new JProperty("WorkingPath", doc.WorkingPath),
                        new JProperty("JpgFileName", jpgDoc == null ? "" : jpgDoc.O_filename)
                        ));
                }

                reJo.data = jaData;

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
