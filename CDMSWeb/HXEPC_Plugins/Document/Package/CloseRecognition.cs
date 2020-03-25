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
    public class CloseRecognition
    {
        //线程锁 
        internal static Mutex muxConsole = new Mutex();

        /// <summary>
        /// 创建校审意见单
        /// </summary>
        /// <param name="wf"></param>
        /// <param name="wsb"></param>
        /// <returns></returns>
        public static bool CreateAuditSheet(WorkFlow wf, WorkStateBranch wsb)
        {
            try
            {
                DBSource dbsource = wf.dBSource;
                #region 创建校审意见单

                CataloguDoc cdoc = new CataloguDoc();
                cdoc.doc = CommonFunction.GetWorkFlowDoc(wf);
                string strRecCode = cdoc.CA_FILECODE;

                if (cdoc.doc == null) return false;

                //获取发文目录
                Project sendProj = null;
                sendProj = cdoc.Project;

                //

                #region 3.收文目录新建校审意见单,根据校审意见单模板，生成校审意见单文档

                //获取立项单文档所在的目录
                //Project m_Project = m_NewProject;

                List<TempDefn> docTempDefnByCode = dbsource.GetTempDefnByCode("CATALOGUING");
                TempDefn docTempDefn = (docTempDefnByCode != null && docTempDefnByCode.Count > 0) ? docTempDefnByCode[0] : null;
                if (docTempDefn == null)
                {
                    //reJo.msg = "没有与其相关的模板管理，创建无法正常完成";
                    //return reJo.Value;
                    return false;
                }

                //创建校审意见单
                CataloguDoc recdoc = new CataloguDoc();

                string filename = strRecCode + " 校审意见单";
                bool isCreate = false;

                //判断是否已经生成校审意见单,如果已经生成就直接返回
                foreach (Doc docx in cdoc.Project.DocList) {
                    if (docx.Code==(filename)) {
                        return true;
                        //recdoc.doc = docx;
                        //isCreate = true;
                        //break;
                    }
                }

                if (!isCreate)
                {
                    recdoc.doc = sendProj.NewDoc(filename + ".docx", filename, "", docTempDefn);

                }

                //IEnumerable<string> source = from docx in cdoc.Project.DocList select docx.Code;

                //if (source.Contains<string>(filename))
                //{

                //    for (int i = 1; i < 0x3e8; i++)
                //    {
                //        filename = strRecCode + " 校审意见单" + i.ToString();
                //        if (!source.Contains<string>(filename))
                //        {
                //            break;
                //        }
                //    }
                //}



                //文档名称
                //recdoc.doc = recProj.NewDoc(filename + ".docx", filename, "", docTempDefn);
                //recdoc.doc = sendProj.NewDoc(filename + ".docx", filename, "", docTempDefn);

                if (recdoc.doc == null)
                {
                    //reJo.msg = "新建信函出错！";
                    //return reJo.Value;
                    return false;
                }



                #endregion


                Hashtable htUserKeyWord = new Hashtable();
                #region 项目专工

                //获取项目专工
                WorkState wsSec = wf.WorkStateList.Find(ws => ws.DefWorkState.O_Code == "PRODESIGN");
                if (wsSec != null && wsSec.WorkUserList != null && wsSec.WorkUserList.Count > 0)
                {
                    string userName = wsSec.WorkUserList[0].User.Description;
                    htUserKeyWord["A1"] = userName;
                    string userCode = wsSec.WorkUserList[0].User.Code;
                    htUserKeyWord["A4"] = userCode;

                    //获取项目专工意见
                    WorkAudit waMainHandle = wsSec.WorkAuditList.Find(wa => wa.workUser.User.Description == userName);
                    if (waMainHandle != null)
                    {
                        string mainHandleAudit = waMainHandle.O_Problom;
                        if (!string.IsNullOrEmpty(mainHandleAudit))
                        {
                            htUserKeyWord["A2"] = mainHandleAudit;
                            string AuditDate = waMainHandle.O_CheckDate.ToString("yyyy.MM.dd");
                            htUserKeyWord["A3"] = AuditDate;
                        }
                    }
                }


                #endregion

                #region 项目造价员

                //获取项目造价员
                wsSec = wf.WorkStateList.Find(ws => ws.DefWorkState.O_Code == "DIRCOSTCLERK");
                if (wsSec != null && wsSec.WorkUserList != null && wsSec.WorkUserList.Count > 0)
                {
                    string userName = wsSec.WorkUserList[0].User.Description;
                    htUserKeyWord["B1"] = userName;
                    string userCode = wsSec.WorkUserList[0].User.Code;
                    htUserKeyWord["B4"] = userCode;

                    //获取项目造价员意见
                    WorkAudit waMainHandle = wsSec.WorkAuditList.Find(wa => wa.workUser.User.Description == userName);
                    if (waMainHandle != null)
                    {
                        string mainHandleAudit = waMainHandle.O_Problom;
                        if (!string.IsNullOrEmpty(mainHandleAudit))
                        {
                            htUserKeyWord["B2"] = mainHandleAudit;
                            string AuditDate = waMainHandle.O_CheckDate.ToString("yyyy.MM.dd");//.ToString();
                            htUserKeyWord["B3"] = AuditDate;
                        }
                    }
                }


                #endregion

                #region 项目经理

                //获取项目经理
                wsSec = wf.WorkStateList.Find(ws => ws.DefWorkState.O_Code == "PROAPPROV");
                if (wsSec != null && wsSec.WorkUserList != null && wsSec.WorkUserList.Count > 0)
                {
                    string userName = wsSec.WorkUserList[0].User.Description;
                    htUserKeyWord["C1"] = userName;
                    string userCode = wsSec.WorkUserList[0].User.Code;
                    htUserKeyWord["C4"] = userCode;

                    //获取项目经理意见
                    WorkAudit waMainHandle = wsSec.WorkAuditList.Find(wa => wa.workUser.User.Description == userName);
                    if (waMainHandle != null)
                    {
                        string mainHandleAudit = waMainHandle.O_Problom;
                        if (!string.IsNullOrEmpty(mainHandleAudit))
                        {
                            htUserKeyWord["C2"] = mainHandleAudit;
                            string AuditDate = waMainHandle.O_CheckDate.ToString("yyyy.MM.dd");
                            htUserKeyWord["C3"] = AuditDate;

                        }
                    }
                }


                #endregion

                #region 成本控制部长

                //获取成本控制员
                wsSec = wf.WorkStateList.Find(ws => ws.DefWorkState.O_Code == "DIRLEADER");
                if (wsSec != null && wsSec.WorkUserList != null && wsSec.WorkUserList.Count > 0)
                {
                    string userName = wsSec.WorkUserList[0].User.Description;
                    htUserKeyWord["D1"] = userName;
                    string userCode = wsSec.WorkUserList[0].User.Code;
                    htUserKeyWord["D4"] = userCode;

                    //获取成本控制员意见
                    WorkAudit waMainHandle = wsSec.WorkAuditList.Find(wa => wa.workUser.User.Description == userName);
                    if (waMainHandle != null)
                    {
                        string mainHandleAudit = waMainHandle.O_Problom;
                        if (!string.IsNullOrEmpty(mainHandleAudit))
                        {
                            htUserKeyWord["D2"] = mainHandleAudit;
                            string AuditDate = waMainHandle.O_CheckDate.ToString("yyyy.MM.dd");
                            htUserKeyWord["D3"] = AuditDate;
                        }
                    }
                }


                #endregion

                #region 成本控制员

                //获取成本控制员
                wsSec = wf.WorkStateList.Find(ws => ws.DefWorkState.O_Code == "DIRECTOR");
                if (wsSec != null && wsSec.WorkUserList != null && wsSec.WorkUserList.Count > 0)
                {
                    string userName = wsSec.WorkUserList[0].User.Description;
                    htUserKeyWord["E1"] = userName;
                    string userCode = wsSec.WorkUserList[0].User.Code;
                    htUserKeyWord["E4"] = userCode;

                    //获取成本控制员意见
                    WorkAudit waMainHandle = wsSec.WorkAuditList.Find(wa => wa.workUser.User.Description == userName);
                    if (waMainHandle != null)
                    {
                        string mainHandleAudit = waMainHandle.O_Problom;
                        if (!string.IsNullOrEmpty(mainHandleAudit))
                        {
                            htUserKeyWord["E2"] = mainHandleAudit;
                            string AuditDate = waMainHandle.O_CheckDate.ToString("yyyy.MM.dd");
                            htUserKeyWord["E3"] = AuditDate;
                        }

                    }
                }


                #endregion


                #region 财务部部长

                //获取财务部部长
                wsSec = wf.WorkStateList.Find(ws => ws.DefWorkState.O_Code == "FINE");
                if (wsSec != null && wsSec.WorkUserList != null && wsSec.WorkUserList.Count > 0)
                {
                    string userName = wsSec.WorkUserList[0].User.Description;
                    htUserKeyWord["F1"] = userName;
                    string userCode = wsSec.WorkUserList[0].User.Code;
                    htUserKeyWord["F4"] = userCode;

                    //获取财务部部长意见
                    WorkAudit waMainHandle = wsSec.WorkAuditList.Find(wa => wa.workUser.User.Description == userName);
                    if (waMainHandle != null)
                    {
                        string mainHandleAudit = waMainHandle.O_Problom;
                        if (!string.IsNullOrEmpty(mainHandleAudit))
                        {
                            htUserKeyWord["F2"] = mainHandleAudit;
                            string AuditDate = waMainHandle.O_CheckDate.ToString("yyyy.MM.dd");
                            htUserKeyWord["F3"] = AuditDate;
                        }

                    }
                }


                #endregion


                #region 招标部部长

                //获取招标部部长
                wsSec = wf.WorkStateList.Find(ws => ws.DefWorkState.O_Code == "ZTBB");
                if (wsSec != null && wsSec.WorkUserList != null && wsSec.WorkUserList.Count > 0)
                {
                    string userName = wsSec.WorkUserList[0].User.Description;
                    htUserKeyWord["G1"] = userName;
                    string userCode = wsSec.WorkUserList[0].User.Code;
                    htUserKeyWord["G4"] = userCode;

                    //获取招标部部长意见
                    WorkAudit waMainHandle = wsSec.WorkAuditList.Find(wa => wa.workUser.User.Description == userName);
                    if (waMainHandle != null)
                    {
                        string mainHandleAudit = waMainHandle.O_Problom;
                        if (!string.IsNullOrEmpty(mainHandleAudit))
                        {
                            htUserKeyWord["G2"] = mainHandleAudit;
                            string AuditDate = waMainHandle.O_CheckDate.ToString("yyyy.MM.dd");
                            htUserKeyWord["G3"] = AuditDate;
                        }
                    }
                }


                #endregion



                string workingPath = dbsource.LoginUser.WorkingPath;


                try
                {
                    //上传下载文档
                    string exchangfilename = "校审意见单";

                    //获取网站路径
                    string sPath = System.Web.HttpContext.Current.Server.MapPath("/ISO/HXEPC/");

                    //获取模板文件路径
                    string modelFileName = sPath + exchangfilename + ".docx";

                    //获取即将生成的联系单文件路径
                    string locFileName = recdoc.FullPathFile;

                    FileInfo info = new FileInfo(locFileName);

                    if (System.IO.File.Exists(modelFileName))
                    {
                        //如果存储子目录不存在，就创建目录
                        if (!Directory.Exists(info.Directory.FullName))
                        {
                            Directory.CreateDirectory(info.Directory.FullName);
                        }

                        //复制模板文件到存储目录，并覆盖同名文件
                        System.IO.File.Copy(modelFileName, locFileName, true);


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
                            office.WriteDataToDocument(recdoc.doc, locFileName, htUserKeyWord, htUserKeyWord);
                        }
                        catch { }
                        finally
                        {

                            //解锁
                            muxConsole.ReleaseMutex();
                        }
                    }


                    int length = (int)info.Length;
                    recdoc.O_size = new int?(length);
                    recdoc.Modify();

                    //sendProj.NewDoc(recdoc.doc);
                    #region 获取收文目录
                    //管理员身份登录创建快捷方式
                    DBSource adminDBSource = DBSourceController.dbManager.DBSourceList[0];

                    //运营管理类文件目录
                    Project rootProj = adminDBSource.RootLocalProjectList.Find(itemProj => itemProj.TempDefn.KeyWord == "OPERATEADMIN");

                    //流程管理目录
                    Project storaProj = CommonFunction.GetProjectByDesc(rootProj, "流程管理");
                    //认质认价目录
                    Project recProj = CommonFunction.GetProjectByDesc(storaProj, "认质认价");
                    #endregion

                    recProj.NewDoc(recdoc.doc);

                    //添加意见校审单到流程
                    wf.DocList.Add(recdoc.doc);
                    wf.Modify();
                    recdoc.doc.WorkFlow = wf;
                    recdoc.doc.Modify();

                    return true;
                }
                catch (Exception ex)
                {
                }
                return false; 
                #endregion

            }
            catch (Exception ex)
            {
            }
            return false;
        }

        //确认人员确认
        public static bool Confire(WorkFlow wf, WorkStateBranch wsb) {
            try
            {
                //检查当前流程状态下其他用户是否全部都确认通过
                DBSource dbsource = wf.dBSource;
                bool isAllConfire = false;
                foreach (WorkUser wu in wsb.workState.WorkUserList) {
                    if (wu.User != dbsource.LoginUser) {
                        if (wu.O_pass == null || wu.O_pass == false) {
                            return false;
                        }
                    }
                }

                //如果所有人员都已经确认通过，就把文档转换成PDF格式，移动到 
                //当前项目 下的 存档管理 下的 非通信类 下的 项目管理 目录

                //项目管理类文件目录
                Project rootProj = CommonFunction.getParentProjectByTempDefn(wf.doc.Project, "HXNY_DOCUMENTSYSTEM");
                //存档管理目录
                Project storaProj = CommonFunction.GetProjectByDesc(rootProj, "存档管理");
                //通信类目录
                Project nocommProj = CommonFunction.GetProjectByDesc(storaProj, "非通信类");
                //存档目录
                Project ArchProj = CommonFunction.GetProjectByDesc(nocommProj, "项目管理");

                if (ArchProj == null)
                {
                    return false;
                }

                //流程结束后将文件归档
                foreach (Doc docItem in wf.DocList)
                {

                    //创建目录
                    if (!Directory.Exists(ArchProj.FullPath))
                    {
                        Directory.CreateDirectory(ArchProj.FullPath);
                    }

                    //拷贝文件
                    File.Move(docItem.FullPathFile, ArchProj.FullPath + docItem.O_filename);

                    //改变文件父目录
                    ArchProj.DocList.Add(docItem);
                    ArchProj.Modify();

                    docItem.O_projectno = ArchProj.O_projectno;
                    docItem.Modify();

                }

                #region 获取自动生成的校审意见单，并转换成PDF格式
                CataloguDoc cdoc = new CataloguDoc();
                cdoc.doc = CommonFunction.GetWorkFlowDoc(wf);
                string strRecCode = cdoc.CA_FILECODE;

                if (cdoc.doc == null) return false;

                string filename = strRecCode + " 校审意见单";

                Doc auditDoc = wf.DocList.Find(d => d.Code == filename);

                if (auditDoc != null)
                {
                    //转换PDF
                    CommonFunction.ConventDocToPdf(auditDoc);
                }

                #endregion

            }
            catch (Exception ex)
            {
            }
            return false;
        }
    }
}
