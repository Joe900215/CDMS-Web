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
//using System.Data.SQLite;
//using LinqToDB;

namespace AVEVA.CDMS.HXPC_Plugins
{
    public class Document
    {
        /// <summary>
        /// 获取创建发文单表单的默认配置
        /// </summary>
        /// <param name="sid">连接密钥</param>
        /// <param name="ProjectKeyword">发文单位目录关键字</param>
        /// <returns></returns>
        public static JObject GetSendDocumentDefault(string sid, string ProjectKeyword)
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

                //定位到发文目录
                m_Project = LocalProject(m_Project);

                if (m_Project == null)
                {
                    reJo.msg = "参数错误！文件夹不存在！";
                    return reJo.Value;
                }

                //获取项目号
                string RootProjectCode = m_Project.GetValueByKeyWord("DESIGNPROJECT_CODE");

                //设计阶段目录
                Project designphase = CommonFunction.GetDesign(m_Project);


                //收文单位列表
                JObject joCompany = new JObject();
                List<Project> companyList = new List<Project>();
                if (designphase != null)
                {
                    //收文目录
                    Project getDoc = null;
                    foreach (Project pp in designphase.ChildProjectList)
                    {
                        //************************* 正常情况下需要改为： 收文。
                        if (pp.Code == "收文")
                        {
                            getDoc = pp;
                            break;
                        }
                    }

                    //收文单位目录
                    foreach (Project pp in getDoc.ChildProjectList)
                    {
                        if (pp.TempDefn != null && pp.TempDefn.KeyWord == "FAXCOMPANY")
                        {
                            companyList.Add(pp);
                            //cbCompany.Items.Add(pp.ToString);

                            joCompany.Add(new JProperty(pp.ToString, pp.ToString));
                        }
                    }
                }

                //如果没有设置收文单位，则提示用户
                if (companyList == null || companyList.Count == 0)
                {
                    //MessageBox.Show("请先创建收文单位，才能发文！");
                    //this.Close();
                    //return;

                    reJo.msg = "请先创建收文单位，才能发文！";
                    return reJo.Value;
                }

                string DocNumber = getDocNumber(m_Project, companyList[0].ToString);//设置编号

                JObject joData = new JObject(
                    new JProperty("RootProjectCode", RootProjectCode),
                    new JProperty("DocNumber", DocNumber),
                    new JProperty("CompanyList", joCompany)
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
        /// 创建发文单
        /// </summary>
        /// <param name="sid">连接密钥</param>
        /// <param name="ProjectKeyword">文档关键字</param>
        /// <param name="docAttrJson">参数字符串，详见下面例子</param>
        /// <returns>
        /// <para>docAttrJson例子：</para>
        /// <code>
        /// [
        ///    { name: 'RootProject', value: strRootProject },  //项目号
        ///    { name: 'Company', value: strCompany },  //收文单位
        ///    { name: 'Number', value: strNumber },    //函件流水号
        ///    { name: 'Title', value: strDesc },   //函件主题
        ///    { name: 'If', value: strIf },        //是否回文
        ///    { name: 'ReplyDate', value: replyDate }, //任务下达时间
        ///    { name: 'Language', value: strLanguage },     //发送版本
        ///    { name: 'Mif', value: strMif },  //是否保密
        ///    { name: 'Route', value: strRoute },  //发放路径
        ///    { name: 'File', value: strFile },    //文件形式
        ///    { name: 'SendObj', value: strSendObj },  //提交目的
        ///    { name: 'Note', value: strNote } //增加的意见
        /// ]
        /// </code>
        /// <para>发文流程的服务端处理流程：</para>
        /// <para>1.创建发文单，由本函数处理</para>
        /// <para>2.上传附件</para>
        /// <para>3.发起发文流程,由DocumentStartWorkFlow函数处理</para>
        /// </returns>
        public static JObject SendDocument(string sid, string ProjectKeyword, string docAttrJson) {
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

                //定位到提出资料目录
                m_Project = LocalProject(m_Project);

                if (m_Project == null)
                {
                    reJo.msg = "参数错误！文件夹不存在！";
                    return reJo.Value;
                }

                #region 获取发文处理表单的属性

                string strRootProject = "", strCompany = "", strNumber = "",
                    strTitle = "", strIf = "", strReplyDate = "",
                    strLanguage = "", strMif = "", strRoute = "",
                     strFile = "", strSendObj = "", strNote = "";

                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(docAttrJson);

                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();

                    //获取项目号
                    if (strName == "RootProject") strRootProject = strValue;

                    //获取收文单位
                    else if (strName == "Company") strCompany = strValue;

                    //获取函件流水号
                    else if (strName == "Number") strNumber = strValue;

                    //获取函件主题
                    else if (strName == "Title") strTitle = strValue;

                    //获取是否回文
                    else if (strName == "If") strIf = strValue;

                    //获取任务下达时间
                    else if (strName == "ReplyDate") strReplyDate = strValue;

                    //获取发送版本
                    else if (strName == "Language") strLanguage = strValue;

                    //获取是否保密
                    else if (strName == "Mif") strMif = strValue;

                    //获取发放路径
                    else if (strName == "Route") strRoute = strValue;

                    //获取文件形式
                    else if (strName == "File") strFile = strValue;

                    //获取提交目的
                    else if (strName == "SendObj") strSendObj = strValue;

                    //获取增加的意见
                    else if (strName == "Note") strNote = strValue;
                }
                #endregion

                #region 创建收文单位目录

                //创建收文单位
                //string cpt = (string)cbCompany.SelectedItem;

                List<TempDefn> tempDefnByCode = dbsource.GetTempDefnByCode("FAXCOMPANY");
                TempDefn mTempDefn = (tempDefnByCode != null) ? tempDefnByCode[0] : null;
                Project cp = m_Project.NewProject(strCompany.Substring(0, strCompany.IndexOf("__")), strCompany.Substring(strCompany.IndexOf("__") + 1),null, mTempDefn);
                if (cp == null)
                {
                    //AssistFun.PopUpPrompt("创建目录:" + strCompany + "失败！");
                    //return;
                    reJo.msg = "创建目录:" + strCompany + "失败！";
                    return reJo.Value;
                }
                #endregion

                #region 创建发文单
                //创建发文单
                string SendFile = strRootProject + "-" + cp.Code + "-S" + strNumber;

                //查找编号是否重复
                foreach (Doc doc in cp.DocList)
                {
                    if (doc.O_itemname.ToUpper() == SendFile.ToUpper())
                    {
                        //AssistFun.PopUpPrompt("发文编号:" + SendFile + "已经存在！请输入文件编号！");
                        //return;
                        reJo.msg = "发文编号:" + SendFile + "已经存在！请输入文件编号！";
                        return reJo.Value;
                    }
                }


                //查找该模板是否存在
                List<TempDefn> tdlist = dbsource.GetTempDefnByCode("DOCUMENTFILE");
                if (tdlist == null || tdlist.Count == 0)
                {
                    //AssistFun.PopUpPrompt("发文模板:" + "DOCUMENTFILE" + "不存在，请与系统管理员联系！");
                    //return;
                    reJo.msg = "发文模板:" + "DOCUMENTFILE" + "不存在，请与系统管理员联系！";
                    return reJo.Value;
                }

                //创建文档
                Doc maindoc = cp.NewDoc(SendFile + ".doc", SendFile, strTitle, tdlist[0]);
                if (maindoc == null)
                {
                    //AssistFun.PopUpPrompt("创建发文单:" + SendFile + "失败！");
                    //return;
                    reJo.msg = "创建发文单:" + SendFile + "失败！";
                    return reJo.Value;
                } 
                #endregion

                //处理附件
                //string enclosure = this.ProcessAddFile(SendFile, cp);//处理函件



                #region 设置文档附加属性
                //设置附加属性
                maindoc.GetAttrDataByKeyWord("IF_REFILENO").SetCodeDesc(SendFile.ToString());           //文件编号
                maindoc.GetAttrDataByKeyWord("IF_TITLE").SetCodeDesc(strTitle);                      //标题
                maindoc.GetAttrDataByKeyWord("IF_REPLYSTATE").SetCodeDesc("未回复");                    //回复情况
                maindoc.GetAttrDataByKeyWord("IFR_NOTE").SetCodeDesc(strNote);                //备注
                maindoc.GetAttrDataByKeyWord("IF_RESULT").SetCodeDesc(strIf);
                if (strIf == "是")
                {
                    maindoc.GetAttrDataByKeyWord("IF_REPLYDATE").SetCodeDesc((Convert.ToDateTime(strReplyDate)).ToString("yyyy-MM-dd"));            //回文时间
                }
                maindoc.GetAttrDataByKeyWord("DRAFTPROFESSION").SetCodeDesc(m_Project.Description.ToString());//起草专业
                maindoc.GetAttrDataByKeyWord("DRAFTMAN").SetCodeDesc(maindoc.dBSource.LoginUser.ToString); //起草人
                //maindoc.GetAttrDataByKeyWord("IF_SENDFILE").SetCodeDesc(enclosure);

                maindoc.AttrDataList.SaveData(); //保存数据 
                #endregion

                #region 录入数据进入word表单:获取参数

                //设计阶段目录
                Project projDesign = CommonFunction.GetDesign(m_Project);

                //获取厂家资料
                string CompName="";
                string CompAddress = "";
                string CompProvince = "";
                string CompPostcode = "";
                string CompRecevier = "";
                string CompEmail = "";
                string CompPhone = "";
                string Language = "";

                if (projDesign != null)
                {
                    //查找收文厂家
                    foreach (Project pp in projDesign.ChildProjectList)
                    {
                        if (pp.Code == "收文")
                        {
                            foreach (Project cj in pp.ChildProjectList)
                            {
                                //查找厂家
                                Project Comp = maindoc.Project;
                                if (cj.Code == Comp.Code)
                                {
                                    //查找厂家数据
                                    try
                                    {
                                        CompName = cj.GetAttrDataByKeyWord("FC_COMPANYCHINESE").ToString;    //厂家名称
                                        CompAddress = cj.GetAttrDataByKeyWord("FC_ADDRESS").ToString;        //厂家地址
                                        CompProvince = cj.GetAttrDataByKeyWord("FC_PROVINCE").ToString;      //厂家省份
                                        CompPostcode = cj.GetAttrDataByKeyWord("FC_POSTCODE").ToString;      //厂家邮政
                                        CompRecevier = cj.GetAttrDataByKeyWord("FC_RECEIVER").ToString;      //厂家收件人
                                        CompEmail = cj.GetAttrDataByKeyWord("FC_EMAIL").ToString;            //厂家邮箱
                                        CompPhone = cj.GetAttrDataByKeyWord("FC_PHONE").ToString;            //收件人电话
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                }


                //录入数据进入表单
                Hashtable htUserKeyWord = new Hashtable();
                htUserKeyWord.Add("PrjCode", strRootProject);                   //项目号
                htUserKeyWord.Add("FILECODE", SendFile);                   //文件传输号
                htUserKeyWord.Add("SENDTIME", DateTime.Now.ToString("yyyy年MM月dd日"));    //发出日期
                htUserKeyWord.Add("DRAFTMAN", maindoc.dBSource.LoginUser.Description);     //发起人
                htUserKeyWord.Add("FC_COMPANYCHINESE", CompName);                     //厂家名称
                htUserKeyWord.Add("FC_ADDRESS", CompAddress);                         //厂家地址
                htUserKeyWord.Add("FC_PROVINCE", CompProvince);                       //厂家省份
                htUserKeyWord.Add("FC_POSTCODE", CompPostcode);                       //厂家邮政
                htUserKeyWord.Add("FC_RECEIVER", CompRecevier);                       //厂家收件人
                htUserKeyWord.Add("FC_EMAIL", CompEmail);                             //厂家邮箱
                htUserKeyWord.Add("PHONE", CompPhone);                                //电话
                htUserKeyWord.Add("TITLE", strTitle);                                   //主题
                htUserKeyWord.Add("CONT", strNote);                              //意见

                //循环查找附件读取出来  
                int index = 1;
                foreach (Doc doc in maindoc.Project.DocList)
                {
                    string[] x_name = doc.O_filename.Split('_');
                    //判断附件的文件编码和源文件编码是否相同
                    if (SendFile.CompareTo(x_name[0].ToString()) == 0)
                    {
                        htUserKeyWord.Add("FNAME" + index.ToString(), doc.O_itemname.ToString());               //文件名
                        htUserKeyWord.Add("EDITTION" + index.ToString(), "A");                                  //版次
                        htUserKeyWord.Add("FDESC" + index.ToString(), doc.O_itemdesc.ToString());               //标题
                        htUserKeyWord.Add("NUMBER" + index.ToString(), "1");                                    //数量
                        string ext = doc.O_filename.Substring(doc.O_filename.LastIndexOf(".") + 1).ToLower();   //扩展名
                        htUserKeyWord.Add("FTYPE" + index.ToString(), ext.ToString());                          //格式
                        htUserKeyWord.Add("SUBOBJ" + index.ToString(), strSendObj);                         //提交目的
                        index++;
                    }
                }

                //是否保密
                if (strMif == "是")
                { htUserKeyWord.Add("TBMIF1", "☑"); htUserKeyWord.Add("TBMIF2", "□"); }
                else
                { htUserKeyWord.Add("TBMIF1", "□"); htUserKeyWord.Add("TBMIF2", "☑"); }

                //是否回文
                if (strIf == "是")//回文时间
                { htUserKeyWord.Add("IFTIEM", (Convert.ToDateTime(strReplyDate)).ToString("yyyy年MM月dd日")); }
                else//无回文
                { htUserKeyWord.Add("IFTIME", "无"); }

                //发放路径
                if (strRoute == "邮寄")//TBROUTE1
                {
                    htUserKeyWord.Add("TBROUTE1", "☑"); htUserKeyWord.Add("TBROUTE2", "□"); htUserKeyWord.Add("TBROUTE3", "□");
                    htUserKeyWord.Add("TBROUTE4", "□"); htUserKeyWord.Add("TBROUTE5", "□"); htUserKeyWord.Add("TBROUTE6", "□");
                }
                if (strRoute == "当面递交")//TBROUTE2
                {
                    htUserKeyWord.Add("TBROUTE1", "□"); htUserKeyWord.Add("TBROUTE2", "☑"); htUserKeyWord.Add("TBROUTE3", "□");
                    htUserKeyWord.Add("TBROUTE4", "□"); htUserKeyWord.Add("TBROUTE5", "□"); htUserKeyWord.Add("TBROUTE6", "□");
                }
                if (strRoute == "邮件")//TBROUTE3
                {
                    htUserKeyWord.Add("TBROUTE1", "□"); htUserKeyWord.Add("TBROUTE2", "□"); htUserKeyWord.Add("TBROUTE3", "☑");
                    htUserKeyWord.Add("TBROUTE4", "□"); htUserKeyWord.Add("TBROUTE5", "□"); htUserKeyWord.Add("TBROUTE6", "□");
                }
                if (strRoute == "内部OA")//TBROUTE4
                {
                    htUserKeyWord.Add("TBROUTE1", "□"); htUserKeyWord.Add("TBROUTE2", "□"); htUserKeyWord.Add("TBROUTE3", "□");
                    htUserKeyWord.Add("TBROUTE4", "☑"); htUserKeyWord.Add("TBROUTE5", "□"); htUserKeyWord.Add("TBROUTE6", "□");
                }
                if (strRoute == "共享盘")//TBROUTE5
                {
                    htUserKeyWord.Add("TBROUTE1", "□"); htUserKeyWord.Add("TBROUTE2", "□"); htUserKeyWord.Add("TBROUTE3", "□");
                    htUserKeyWord.Add("TBROUTE4", "□"); htUserKeyWord.Add("TBROUTE5", "☑"); htUserKeyWord.Add("TBROUTE6", "□");
                }
                if (strRoute == "业主共享平台")//TBROUTE6
                {
                    htUserKeyWord.Add("TBROUTE1", "□"); htUserKeyWord.Add("TBROUTE2", "□"); htUserKeyWord.Add("TBROUTE3", "□");
                    htUserKeyWord.Add("TBROUTE4", "□"); htUserKeyWord.Add("TBROUTE5", "□"); htUserKeyWord.Add("TBROUTE6", "☑");
                }

                //文件形式
                if (strFile == "EN_电子可修改版")//TBFILE1
                {
                    htUserKeyWord.Add("TBFILE1", "☑"); htUserKeyWord.Add("TBFILE2", "□"); htUserKeyWord.Add("TBFILE3", "□");
                    htUserKeyWord.Add("TBFILE4", "□"); htUserKeyWord.Add("TBFILE5", "□");
                }
                if (strFile == "P_打印件")//TBFILE2
                {
                    htUserKeyWord.Add("TBFILE1", "□"); htUserKeyWord.Add("TBFILE2", "☑"); htUserKeyWord.Add("TBFILE3", "□");
                    htUserKeyWord.Add("TBFILE4", "□"); htUserKeyWord.Add("TBFILE5", "□");
                }
                if (strFile == "O_原件")//TBFILE3
                {
                    htUserKeyWord.Add("TBFILE1", "□"); htUserKeyWord.Add("TBFILE2", "□"); htUserKeyWord.Add("TBFILE3", "☑");
                    htUserKeyWord.Add("TBFILE4", "□"); htUserKeyWord.Add("TBFILE5", "□");
                }
                if (strFile == "C_拷贝盘")//TBFILE4
                {
                    htUserKeyWord.Add("TBFILE1", "□"); htUserKeyWord.Add("TBFILE2", "□"); htUserKeyWord.Add("TBFILE3", "□");
                    htUserKeyWord.Add("TBFILE4", "☑"); htUserKeyWord.Add("TBFILE5", "□");
                }
                if (strFile == "E_电子版(.pdf)")//TBFILE5
                {
                    htUserKeyWord.Add("TBFILE1", "□"); htUserKeyWord.Add("TBFILE2", "□"); htUserKeyWord.Add("TBFILE3", "□");
                    htUserKeyWord.Add("TBFILE4", "□"); htUserKeyWord.Add("TBFILE5", "☑");
                }

                //提交目的
                if (strSendObj == "1_按需求提交")//TBSENDOBJ1
                {
                    htUserKeyWord.Add("TBSENDOBJ1", "☑"); htUserKeyWord.Add("TBSENDOBJ2", "□"); htUserKeyWord.Add("TBSENDOBJ3", "□"); htUserKeyWord.Add("TBSENDOBJ4", "□"); htUserKeyWord.Add("TBSENDOBJ5", "□");
                    htUserKeyWord.Add("TBSENDOBJ6", "□"); htUserKeyWord.Add("TBSENDOBJ7", "□"); htUserKeyWord.Add("TBSENDOBJ8", "□"); htUserKeyWord.Add("TBSENDOBJ9", "□"); htUserKeyWord.Add("TBSENDOBJ10", "□");
                    htUserKeyWord.Add("TBSENDOBJ11", "□");
                }
                if (strSendObj == "2_审查")//TBSENDOBJ2
                {
                    htUserKeyWord.Add("TBSENDOBJ1", "□"); htUserKeyWord.Add("TBSENDOBJ2", "☑"); htUserKeyWord.Add("TBSENDOBJ3", "□"); htUserKeyWord.Add("TBSENDOBJ4", "□"); htUserKeyWord.Add("TBSENDOBJ5", "□");
                    htUserKeyWord.Add("TBSENDOBJ6", "□"); htUserKeyWord.Add("TBSENDOBJ7", "□"); htUserKeyWord.Add("TBSENDOBJ8", "□"); htUserKeyWord.Add("TBSENDOBJ9", "□"); htUserKeyWord.Add("TBSENDOBJ10", "□");
                    htUserKeyWord.Add("TBSENDOBJ11", "□");
                }
                if (strSendObj == "3_告知")//TBSENDOBJ3
                {
                    htUserKeyWord.Add("TBSENDOBJ1", "□"); htUserKeyWord.Add("TBSENDOBJ2", "□"); htUserKeyWord.Add("TBSENDOBJ3", "☑"); htUserKeyWord.Add("TBSENDOBJ4", "□"); htUserKeyWord.Add("TBSENDOBJ5", "□");
                    htUserKeyWord.Add("TBSENDOBJ6", "□"); htUserKeyWord.Add("TBSENDOBJ7", "□"); htUserKeyWord.Add("TBSENDOBJ8", "□"); htUserKeyWord.Add("TBSENDOBJ9", "□"); htUserKeyWord.Add("TBSENDOBJ10", "□");
                    htUserKeyWord.Add("TBSENDOBJ11", "□");
                }
                if (strSendObj == "4_采购")//TBSENDOBJ4
                {
                    htUserKeyWord.Add("TBSENDOBJ1", "□"); htUserKeyWord.Add("TBSENDOBJ2", "□"); htUserKeyWord.Add("TBSENDOBJ3", "□"); htUserKeyWord.Add("TBSENDOBJ4", "☑"); htUserKeyWord.Add("TBSENDOBJ5", "□");
                    htUserKeyWord.Add("TBSENDOBJ6", "□"); htUserKeyWord.Add("TBSENDOBJ7", "□"); htUserKeyWord.Add("TBSENDOBJ8", "□"); htUserKeyWord.Add("TBSENDOBJ9", "□"); htUserKeyWord.Add("TBSENDOBJ10", "□");
                    htUserKeyWord.Add("TBSENDOBJ11", "□");
                }
                if (strSendObj == "5_供应商")//TBSENDOBJ5
                {
                    htUserKeyWord.Add("TBSENDOBJ1", "□"); htUserKeyWord.Add("TBSENDOBJ2", "□"); htUserKeyWord.Add("TBSENDOBJ3", "□"); htUserKeyWord.Add("TBSENDOBJ4", "□"); htUserKeyWord.Add("TBSENDOBJ5", "☑");
                    htUserKeyWord.Add("TBSENDOBJ6", "□"); htUserKeyWord.Add("TBSENDOBJ7", "□"); htUserKeyWord.Add("TBSENDOBJ8", "□"); htUserKeyWord.Add("TBSENDOBJ9", "□"); htUserKeyWord.Add("TBSENDOBJ10", "□");
                    htUserKeyWord.Add("TBSENDOBJ11", "□");
                }
                if (strSendObj == "6_交工资料")//TBSENDOBJ6
                {
                    htUserKeyWord.Add("TBSENDOBJ1", "□"); htUserKeyWord.Add("TBSENDOBJ2", "□"); htUserKeyWord.Add("TBSENDOBJ3", "□"); htUserKeyWord.Add("TBSENDOBJ4", "□"); htUserKeyWord.Add("TBSENDOBJ5", "□");
                    htUserKeyWord.Add("TBSENDOBJ6", "☑"); htUserKeyWord.Add("TBSENDOBJ7", "□"); htUserKeyWord.Add("TBSENDOBJ8", "□"); htUserKeyWord.Add("TBSENDOBJ9", "□"); htUserKeyWord.Add("TBSENDOBJ10", "□");
                    htUserKeyWord.Add("TBSENDOBJ11", "□");
                }
                if (strSendObj == "7_批准")//TBSENDOBJ7
                {
                    htUserKeyWord.Add("TBSENDOBJ1", "□"); htUserKeyWord.Add("TBSENDOBJ2", "□"); htUserKeyWord.Add("TBSENDOBJ3", "□"); htUserKeyWord.Add("TBSENDOBJ4", "□"); htUserKeyWord.Add("TBSENDOBJ5", "□");
                    htUserKeyWord.Add("TBSENDOBJ6", "□"); htUserKeyWord.Add("TBSENDOBJ7", "☑"); htUserKeyWord.Add("TBSENDOBJ8", "□"); htUserKeyWord.Add("TBSENDOBJ9", "□"); htUserKeyWord.Add("TBSENDOBJ10", "□");
                    htUserKeyWord.Add("TBSENDOBJ11", "□");
                }
                if (strSendObj == "8_预制")//TBSENDOBJ8
                {
                    htUserKeyWord.Add("TBSENDOBJ1", "□"); htUserKeyWord.Add("TBSENDOBJ2", "□"); htUserKeyWord.Add("TBSENDOBJ3", "□"); htUserKeyWord.Add("TBSENDOBJ4", "□"); htUserKeyWord.Add("TBSENDOBJ5", "□");
                    htUserKeyWord.Add("TBSENDOBJ6", "□"); htUserKeyWord.Add("TBSENDOBJ7", "□"); htUserKeyWord.Add("TBSENDOBJ8", "☑"); htUserKeyWord.Add("TBSENDOBJ9", "□"); htUserKeyWord.Add("TBSENDOBJ10", "□");
                    htUserKeyWord.Add("TBSENDOBJ11", "□");
                }
                if (strSendObj == "9_施工")//TBSENDOBJ9
                {
                    htUserKeyWord.Add("TBSENDOBJ1", "□"); htUserKeyWord.Add("TBSENDOBJ2", "□"); htUserKeyWord.Add("TBSENDOBJ3", "□"); htUserKeyWord.Add("TBSENDOBJ4", "□"); htUserKeyWord.Add("TBSENDOBJ5", "□");
                    htUserKeyWord.Add("TBSENDOBJ6", "□"); htUserKeyWord.Add("TBSENDOBJ7", "□"); htUserKeyWord.Add("TBSENDOBJ8", "□"); htUserKeyWord.Add("TBSENDOBJ9", "☑"); htUserKeyWord.Add("TBSENDOBJ10", "□");
                    htUserKeyWord.Add("TBSENDOBJ11", "□");
                }
                if (strSendObj == "10_提交")//TBSENDOBJ10
                {
                    htUserKeyWord.Add("TBSENDOBJ1", "□"); htUserKeyWord.Add("TBSENDOBJ2", "□"); htUserKeyWord.Add("TBSENDOBJ3", "□"); htUserKeyWord.Add("TBSENDOBJ4", "□"); htUserKeyWord.Add("TBSENDOBJ5", "□");
                    htUserKeyWord.Add("TBSENDOBJ6", "□"); htUserKeyWord.Add("TBSENDOBJ7", "□"); htUserKeyWord.Add("TBSENDOBJ8", "□"); htUserKeyWord.Add("TBSENDOBJ9", "□"); htUserKeyWord.Add("TBSENDOBJ10", "☑");
                    htUserKeyWord.Add("TBSENDOBJ11", "□");
                }
                if (strSendObj == "11_其他")//TBSENDOBJ11
                {
                    htUserKeyWord.Add("TBSENDOBJ1", "□"); htUserKeyWord.Add("TBSENDOBJ2", "□"); htUserKeyWord.Add("TBSENDOBJ3", "□"); htUserKeyWord.Add("TBSENDOBJ4", "□"); htUserKeyWord.Add("TBSENDOBJ5", "□");
                    htUserKeyWord.Add("TBSENDOBJ6", "□"); htUserKeyWord.Add("TBSENDOBJ7", "□"); htUserKeyWord.Add("TBSENDOBJ8", "□"); htUserKeyWord.Add("TBSENDOBJ9", "□"); htUserKeyWord.Add("TBSENDOBJ10", "□");
                    htUserKeyWord.Add("TBSENDOBJ11", "☑");
                }



                if (strLanguage == "中文")
                {
                    Language = "外部文件传输单（中文）";
                }
                else
                {
                    Language = "外部文件传输单（英文）";
                }
                #endregion

                ///填写发文单
                //try
                //{
                //    //上传下载文件
                //    FTPFactory factory = m_Project.Storage.FTP ?? new FTPFactory(m_Project.Storage);
                //    string locFileName = m_Project.dBSource.LoginUser.WorkingPath + maindoc.Code + ".doc";
                //    factory.download(@"\ISO\" + Language + ".doc", locFileName, false);
                //    CDMSOffice office = new CDMSOffice
                //    {
                //        CloseApp = true,
                //        VisibleApp = false
                //    };
                //    office.Release(true);
                //    office.WriteDataToDocument(maindoc, locFileName, htUserKeyWord, htUserKeyWord);
                //    factory.upload(locFileName, maindoc.FullPathFile);
                //    factory.close();
                //    FileInfo info = new FileInfo(locFileName);
                //    int length = (int)info.Length;
                //    maindoc.O_size = new int?(length);
                //    maindoc.Modify();
                //    base.DialogResult = DialogResult.OK;
                //    this.Cursor = Cursors.Default;
                //    base.Close();
                //    CommonFunction.InsertDocListAndOpenDoc(this.m_DocList, maindoc);
                //}
                //catch (Exception ex)
                //{
                //    AssistFun.PopUpPrompt(ex.ToString());
                //}

                //填写发文单
                try
                {
                    //上传下载文档
                    //string exchangfilename = "专业间互提资料单";

                    //获取网站路径
                    string sPath = System.Web.HttpContext.Current.Server.MapPath("/ISO/HXPC/");

                    //获取模板文件路径
                    string modelFileName = sPath + Language + ".doc";

                    //获取即将生成的联系单文件路径
                    string locFileName = maindoc.FullPathFile;

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
                            office.WriteDataToDocument(maindoc, locFileName, htUserKeyWord, htUserKeyWord);
                        }
                        catch { }
                        finally
                        {

                            //解锁
                            muxConsole.ReleaseMutex();
                        }
                    }


                    int length = (int)info.Length;
                    maindoc.O_size = new int?(length);
                    maindoc.Modify();

                    string strDocList = "";//获取附件
                    if (string.IsNullOrEmpty(strDocList))
                    {
                        strDocList = maindoc.KeyWord;
                    }
                    else
                    {
                        strDocList = maindoc.KeyWord + "," + strDocList;
                    }

                    //这里刷新数据源，否则创建流程的时候获取不了专业字符串
                    DBSourceController.RefreshDBSource(sid);

                    reJo.success = true;
                    reJo.data = new JArray(new JObject(new JProperty("ProjectKeyword", maindoc.Project.KeyWord),
                        new JProperty("DocKeyword", maindoc.KeyWord), new JProperty("DocList", strDocList),
                        new JProperty("DocCode", maindoc.Code)));
                    return reJo.Value;
                }
                catch { }

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



        /// <summary>
        /// 创建发文单后，发起发文流程
        /// </summary>
        /// <param name="sid">连接密钥</param>
        /// <param name="docKeyword">发文单文档关键字</param>
        /// <param name="DocList">本流程所有文档列表关键字，用','分隔</param>
        /// <returns></returns>
        public static JObject DocumentStartWorkFlow(string sid, string docKeyword, string DocList)
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

                Doc doc = dbsource.GetDocByKeyWord(docKeyword);
                if (doc == null)
                {
                    reJo.msg = "错误的文档操作信息！指定的文档不存在！";
                    return reJo.Value;
                }

                //if (MessageBox.Show("是否启动校审流程？", "工作流启动", MessageBoxButtons.YesNo) != DialogResult.No)
                //{
                //    Thread.Sleep(300);
                //    if (((doc.OperateDocStatus == enDocStatus.OUT) && (doc.FLocker == doc.dBSource.LoginUser)) || (doc.OperateDocStatus == enDocStatus.COMING_IN))
                //    {
                //        MessageBox.Show("文档处于检出状态，请先保存文档，并关闭该文档");
                //        this.StartWorkFlow(doc, defWFCode);
                //    }
                //     else
                {


                    #region 获取文档列表
                    string[] strArray = (string.IsNullOrEmpty(DocList) ? "" : DocList).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    List<Doc> m_DocList = new List<Doc>();
                    //启动工作流程
                    m_DocList.Reverse();
                    foreach (string strObj in strArray)
                    {
                        object obj = dbsource.GetObjectByKeyWord(strObj);

                        if (obj is Doc)
                        {
                            m_DocList.Add((Doc)obj);
                        }
                    }
                    #endregion

                    WorkFlow flow = dbsource.NewWorkFlow(m_DocList, "SENDDOC");
                    if (flow == null || flow.CuWorkState == null || flow.CuWorkState.workStateBranchList == null || (flow.CuWorkState.workStateBranchList.Count <= 0))
                    {
                        reJo.msg = "自动启动流程失败!请手动启动";
                        return reJo.Value;
                    }

                    //查找主任
                    AttrData ad = doc.GetAttrDataByKeyWord("PROFESSIONMANAGER");
                    if (ad == null || ad.group == null || ad.group.UserList.Count <= 0)
                    {
                        //AssistFun.PopUpPrompt("本专业没有设置主设，不能启动流程，请设置主设后再启动流程！");
                        reJo.msg = "本专业没有设置主设，不能启动流程，请设置主设后再启动流程！";
                        return reJo.Value;
                    }

                    //启动流程
                    WorkStateBranch branch = flow.CuWorkState.workStateBranchList[0];
                    branch.NextStateAddGroup(ad.group);

                    ExReJObject GotoNextReJo = WebWorkFlowEvent.GotoNextStateAndSelectUser(flow.CuWorkState.workStateBranchList[0]);

                    if (!GotoNextReJo.success)
                    {
                        //  doc.dBSource.ProgramRun = false;
                        flow.Delete();
                        flow.Delete();

                        reJo.msg = "自动启动流程失败！请手动启动流程";
                        return reJo.Value;
                    }

                    DBSourceController.RefreshDBSource(sid);

                    return GotoNextReJo.Value;
                    //if (ExMenu.callTheApp != null)
                    //{
                    //    CallBackParam param = new CallBackParam
                    //    {
                    //        callType = enCallBackType.UpdateDBSource,
                    //        dbs = flow.dBSource
                    //    };
                    //    CallBackResult result = null;
                    //    ExMenu.callTheApp(param, out result);
                    //}
                }
                //   }

            }
            catch (Exception exception)
            {
                WebApi.CommonController.WebWriteLog(exception.Message + "\r\n" + exception.Source + "\r\n" + exception.StackTrace);
                //AssistFun.PopUpPrompt(exception.Message + "\r\n" + exception.Source + "\r\n" + exception.StackTrace);
                reJo.msg = "启动流程失败！" + exception.Message + "\r\n" + exception.Source + "\r\n" + exception.StackTrace;
            }
            return reJo.Value;

        }

        /// <summary>
        /// 处理拖拽文件到DocGrid控件的处理事件
        /// </summary>
        /// <param name="sid">连接密钥</param>
        /// <param name="ProjectKeyword">目录关键字</param>
        /// <returns></returns>
        public static JObject OnBeforeFileAddEvent(string sid, string ProjectKeyword) {
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

                //判断是否设置了主设
                //查找设计阶段
                //找设计阶段
                //Doc doc = (Doc)obj;
                //Project p = doc.Project;


                //收文目录可以使用
                if (m_Project.TempDefn == null || m_Project.TempDefn.KeyWord != "FAXCOMPANY")
                {
                    //return;
                    //reJo.msg = "";
                    //当success返回true,msg返回""时，继续上传文件
                    reJo.success = true;
                    return reJo.Value;
                }


                //查找设计阶段
                Project DesignProject = CommonFunction.GetDesign(m_Project);
                if (DesignProject != null)
                {

                    //查找专业
                    foreach (Project pp in DesignProject.ChildProjectList)
                    {
                        if (pp.TempDefn != null && pp.TempDefn.KeyWord == "PROFESSION")
                        {
                            AttrData ad = pp.GetAttrDataByKeyWord("PROFESSIONOWNER");
                            if (ad == null || ad.group == null || ad.group.UserList.Count == 0)
                            {
                                //doc.Delete(true);
                                //MessageBox.Show("专业:" + pp.ToString + "没有设置主设人，请先设置主设人后，再分发文件！拖拽的文件已经删除");
                                //return;
                                reJo.msg = "专业:" + pp.ToString + "没有设置主设人，请先设置主设人后，再分发文件！拖拽的文件已经删除";
                                return reJo.Value;
                            }
                        }
                    }
                }



                ///放置在函件单位下的分类目录下
                //m_Project = doc.Project;
                string docControl = m_Project.GetValueByKeyWord("DOCUMENTMANAGER");
                if (m_Project != null && curUser.O_username == docControl && m_Project.TempDefn != null && m_Project.TempDefn.KeyWord == "FAXCOMPANY")
                {
                    //    new fmGetDocument(doc).ShowDialog();
                    reJo.msg = "GetDocument";
                }
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

        public static ExReJObject OnAfterCreateNewObject(string PlugName, object obj)
        {
            ExReJObject reJo = new ExReJObject();
            reJo.success = true;
            if (PlugName != EnterPoint.PluginName)
            {
                //当reJo的成功状态返回为真时，继续流转到下一流程状态的操作
                reJo.success = true;
                return reJo;
            }
            //当返回false时，向客户端发送返回，返回为true时，就不向客户端返回
            reJo.success = true;
            try
            {

                //判断是否设置了主设
                //查找设计阶段
                //找设计阶段
                Doc doc = (Doc)obj;
                Project m_Project = doc.Project;


                //收文目录可以使用
                if (m_Project.TempDefn == null || m_Project.TempDefn.KeyWord != "FAXCOMPANY")
                {
                    //return;
                    //reJo.msg = "";
                    //当success返回true,msg返回""时，继续上传文件
                    reJo.success = true;
                    return reJo;
                }


                //查找设计阶段
                Project DesignProject = CommonFunction.GetDesign(m_Project);
                if (DesignProject != null)
                {

                    //查找专业
                    foreach (Project pp in DesignProject.ChildProjectList)
                    {
                        if (pp.TempDefn != null && pp.TempDefn.KeyWord == "PROFESSION")
                        {
                            AttrData ad = pp.GetAttrDataByKeyWord("PROFESSIONOWNER");
                            if (ad == null || ad.group == null || ad.group.UserList.Count == 0)
                            {
                                //doc.Delete(true);
                                //MessageBox.Show("专业:" + pp.ToString + "没有设置主设人，请先设置主设人后，再分发文件！拖拽的文件已经删除");
                                //return;
                                //reJo.msg = "专业:" + pp.ToString + "没有设置主设人，请先设置主设人后，再分发文件！拖拽的文件已经删除";
                                //return reJo;
                            }
                        }
                    }
                }



                ///放置在函件单位下的分类目录下
                //m_Project = doc.Project;
                string docControl = m_Project.GetValueByKeyWord("DOCUMENTMANAGER");
                if (m_Project != null && doc.dBSource.LoginUser.O_username == docControl && m_Project.TempDefn != null && m_Project.TempDefn.KeyWord == "FAXCOMPANY")
                {
                    //    new fmGetDocument(doc).ShowDialog();
                    reJo.msg = "GetDocument";
                    reJo.data = new JArray(new JObject(
                        new JProperty("plugins", "HXPC_Plugins"),
                        new JProperty("FuncName", "getDocument"),
                        new JProperty("DocKeyword",doc.KeyWord)
                        ));

                    //当返回false时，向客户端发送返回，返回为true时，就不向客户端返回
                    reJo.success = false;
                    return reJo;
                }
                reJo.success = true;
                return reJo;
            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(reJo.msg);
            }
            return reJo;
        }



        /// <summary>
        /// 获取创建收文单表单的默认配置
        /// </summary>
        /// <param name="sid">连接密钥</param>
        /// <param name="DocKeyword">文档关键字</param>
        /// <returns></returns>
        public static JObject GetReceiveDocumentDefault(string sid, string DocKeyword)
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

                Doc m_Doc = dbsource.GetDocByKeyWord(DocKeyword);

                if (m_Doc == null)
                {
                    reJo.msg = "参数错误！文档不存在！";
                    return reJo.Value;
                }

                string RootProjectCode = m_Doc.GetValueByKeyWord("DESIGNPROJECT_CODE");
                string strCompany = m_Doc.GetValueByKeyWord("FAXCOMPANY_CODE");
                string DocNumber = getDocNumber(m_Doc.Project, RootProjectCode, strCompany);
                string strDesc= m_Doc.O_itemname;

                JObject joData = new JObject(
                    new JProperty("RootProjectCode", RootProjectCode),
                    new JProperty("DocNumber", DocNumber),
                    new JProperty("Company", strCompany)
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

        /// <summary>
        /// 处理收文表单
        /// </summary>
        /// <param name="sid">连接密钥</param>
        /// <param name="DocKeyword">文档关键字</param>
        /// <param name="docAttrJson">参数字符串，详见下面例子</param>
        /// <returns>
        /// <para>docAttrJson例子：</para>
        /// <code>
        ///[
        ///    { name: 'RootProject', value: strRootProject },  //项目号
        ///    { name: 'Company', value: strCompany },  //收文单位
        ///    { name: 'Number', value: strNumber },    //函件流水号
        ///    { name: 'Title', value: strDesc }    //函件主题
        ///]
        /// </code>
        /// </returns>
        public static JObject ReceiveDocument(string sid, string DocKeyword,string docAttrJson)
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

                Doc m_Doc = dbsource.GetDocByKeyWord(DocKeyword);

                if (m_Doc == null)
                {
                    reJo.msg = "参数错误！文档不存在！";
                    return reJo.Value;
                }

                #region 获取收文处理表单的属性

                string strRootProject = "", strCompany = "", strNumber = "",
                    strTitle = "";

                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(docAttrJson);

                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();

                    //获取项目号
                    if (strName == "RootProject") strRootProject = strValue;

                    //获取收文单位
                    else if (strName == "Company") strCompany = strValue;

                    //获取函件流水号
                    else if (strName == "Number") strNumber = strValue;

                    //获取函件主题
                    else if (strName == "Title") strTitle = strValue;

  
                }
                #endregion

                //修改文档编码
                m_Doc.O_itemname = strRootProject + "-" + strCompany + "-G" + strNumber;
                m_Doc.O_itemdesc = strTitle;
                m_Doc.Modify();

                ///启动流程
                WorkFlow flow = dbsource.NewWorkFlow(new List<Doc> { m_Doc }, "RECEIVEDOC");
                if (flow == null)
                {
                    //AssistFun.PopUpPrompt("自动启动流程失败!请手动启动");
                    reJo.msg = "自动启动流程失败!请手动启动";
                    return reJo.Value;
                }
                else
                {
                    if ((flow != null) && (flow.CuWorkState != null))
                    {
                        if (((flow.CuWorkState == null) || (flow.CuWorkState.workStateBranchList == null)) || (flow.CuWorkState.workStateBranchList.Count <= 0))
                        {
                            //MessageBox.Show("新建流程不存在下一状态,提交失败!");
                            //doc.dBSource.ProgramRun = false;
                            flow.Delete();
                            reJo.msg = "新建流程不存在下一状态,提交失败!";
                            return reJo.Value;
                            //return;
                        }
                        WorkStateBranch branch = flow.CuWorkState.workStateBranchList[0];
                        if (branch == null)
                        {
                            reJo.msg = "获取流程分支失败!";
                            return reJo.Value;
                        }
                            ExReJObject wfReJo = WebWorkFlowEvent.GotoNextStateAndSelectUser(flow.CuWorkState.workStateBranchList[0]);
                        //if (!WorkFlowEvent.GotoNextStateAndSelectUser(flow.CuWorkState.workStateBranchList[0]))
                        if (!wfReJo.success)
                        {
                            //doc.dBSource.ProgramRun = false;
                            flow.Delete();
                            flow.Delete();
                            reJo.msg = "自动启动收文流程失败，请手动启动流程!";
                            return reJo.Value;
                        }

                        reJo.data = new JArray(new JObject(new JProperty ("ProjectKeyword",m_Doc.Project.KeyWord)));
                        reJo.success = true;
                        return reJo.Value;

                    }


                }

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

        /// <summary>
        /// 收文流程分发专业获取所在设计阶段的专业列表
        /// </summary>
        /// <param name="sid">连接密钥</param>
        /// <param name="WorkFlowKeyword">流程关键字</param>
        /// <returns></returns>
        public static JObject GetProfessionList(string sid, string WorkFlowKeyword)
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

                Object obj = dbsource.GetObjectByKeyWord(WorkFlowKeyword);
                if (!(obj is WorkFlow))
                {
                    reJo.msg = "流程不存在！";
                    return reJo.Value;
                }


                WorkFlow m_workflow = (WorkFlow)obj;


                //找设计阶段
                Project p = m_workflow.doc.Project;
                while (p != null && p.ParentProject != null)
                {
                    if (p != null && p.TempDefn != null && p.TempDefn.KeyWord == "DESIGNPHASE")
                    {
                        break;
                    }
                    p = p.ParentProject;
                }

                JObject joData = new JObject();

                //查找专业列表
                foreach (Project pp in p.ChildProjectList)
                {
                    if (pp.TempDefn != null && pp.TempDefn.KeyWord == "PROFESSION")
                    {
                        //pList.Add(pp);
                        //this.clbProfession.Items.Add(pp.ToString);
                        joData.Add(new JProperty(pp.KeyWord, pp.ToString));
                    }
                }

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

        /// <summary>
        /// 选择收文流程分发专业
        /// </summary>
        /// <param name="sid">连接密钥</param>
        /// <param name="WorkFlowKeyword">流程关键字</param>
        /// <param name="professionList">选取的专业的projectKeyword列表,使用','分割</param>
        /// <returns></returns>
        public static JObject SetReceiveDocProfession(string sid, string WorkFlowKeyword, string professionList)
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

                Object obj = dbsource.GetObjectByKeyWord(WorkFlowKeyword);
                if (!(obj is WorkFlow))
                {
                    reJo.msg = "流程不存在！";
                    return reJo.Value;
                }

                WorkFlow workFlow = (WorkFlow)obj;

                //获取当前分支
                WorkStateBranch wb = workFlow.CuWorkState.workStateBranchList[0];

                string[] professionArray = (string.IsNullOrEmpty(professionList) ? "" : professionList).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                List<Project> splist = new List<Project>();
                string sprolist = "";
                //遍历所有专业
                foreach (string strProfession in professionArray)
                {
                    Project proj = dbsource.GetProjectByKeyWord(strProfession);
                    if (proj != null)
                    {
                        splist.Add(proj);
                        sprolist += (proj.ToString + ", ");
                    }
                }

                if (splist.Count < 1)
                {
                    reJo.msg = "您没有选择专业！";
                    return reJo.Value;
                }

                //检查各个专业是否有主设人
                foreach (Project p in splist)
                {
                    AttrData ad = p.GetAttrDataByKeyWord("PROFESSIONOWNER");
                    if (ad == null || ad.group == null || ad.group.UserList.Count == 0)
                    {
                        reJo.msg = "专业:" + p.ToString + "没有设置主设人，请先设置主设人后，再选择分发专业！";
                        return reJo.Value;
                    }
                }

                //每个专业创建一个状态
                foreach (Project p in splist)
                {

                    //已经创建了一个接收专业状态，第一个状态就使用系统已经创建的状态
                    WorkState ws = null;
                    if (p == splist[0])
                    {

                        //查找专业状态
                        foreach (WorkState wss in workFlow.WorkStateList)
                        {
                            if (wss.DefWorkState.O_Code == "PROFESSION")
                            {
                                ws = wss;
                                break;
                            }
                        }


                        //没有找到则创建
                        if (ws == null)

                            //新状态
                            ws = workFlow.NewWorkState(workFlow.DefWorkFlow.GetWorkStateByCode("PROFESSION"));
                    }
                    else
                    {
                        //新状态
                        ws = workFlow.NewWorkState(workFlow.DefWorkFlow.GetWorkStateByCode("PROFESSION"));
                    }


                    //专业Project ID 记录在o_iuser4对象里面
                    ws.O_iuser4 = p.O_projectno;
                    ws.Modify();

                    //获取主设
                    AVEVA.CDMS.Server.Group g = p.GetGroupByKeyWord("PROFESSIONOWNER");

                    wb.NextWorkState = ws;

                    //分支上增加人
                    wb.NextStateAddGroup(g);

                    //移动分支
                    wb.SetCurrentUserPass();
                    wb.SetCurrent();

                }


                //状态移动
                workFlow.CuWorkState.SetCuWorkStatePass();
                workFlow.O_iuser5 = 999;  //为了在Common不做后续处理，增加了一个标示。
                workFlow.Modify();

                DBSourceController.RefreshDBSource(sid);

                reJo.data = new JArray(new JObject(new JProperty("ProjectKeyword", workFlow.doc.Project.KeyWord)));
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

        //获取文档最大编号
        private static string getDocNumber(Project proj,string ProjectCode, string CompanyCode)
        {
            try
            {
                //获取文档前缀
                string companyCode = CompanyCode.IndexOf("__") >= 0 ? CompanyCode.Substring(0, CompanyCode.IndexOf("__")) : CompanyCode;


                //编码前缀
                string strPrefix = ProjectCode + "-" + companyCode + "-G";




                List<Doc> docList = proj.dBSource.SelectDoc(string.Format("select * from CDMS_Doc where o_itemname like '%{0}%' and o_dmsstatus !=10 order by o_itemname", strPrefix));
                if (docList == null || docList.Count == 0)
                {
                    return "0001";
                }
                else
                {
                    Doc doc = docList[docList.Count - 1];
                    int tempNum = Convert.ToInt32(doc.O_itemname.Substring(strPrefix.Length, 4));
                    return (tempNum + 1).ToString("d4");
                }
            }
            catch
            {
                return "0001";
            }
        }


        /// <summary>
        /// 定位到发文目录
        /// </summary>
        /// <param name="prj"></param>
        /// <returns></returns>
        private static Project LocalProject(Project prj)
        {
            //找到专业
            //this.p = prj;
            while (prj != null)
            {
                if (prj.TempDefn != null && prj.TempDefn.KeyWord == "PROFESSION")
                {
                    break;
                }
                prj = prj.ParentProject;
            }

            //创建目录
            Project pp = prj.NewProject("收发文", "", prj.Storage, prj.dBSource.GetTempDefnByCode("DOCUMENT")[0]);
            if (pp != null)
            {
                prj = pp.NewProject("发文", "");
                return prj;
            }
            {

                //AssistFun.PopUpPrompt("不能创建：收发文 目录！不能提资！");
                //this.Close();
            }
            return null;
        }


        //获取文档最大编号
        private static string getDocNumber(Project proj,string strCompany)
        {
            try
            {
                string RootProjectCode = proj.GetValueByKeyWord("DESIGNPROJECT_CODE");

                //获取文档前缀
                string companyCode = strCompany.IndexOf("__") >= 0 ? strCompany.Substring(0, strCompany.IndexOf("__")) : strCompany;


                //编码前缀
                string strPrefix = RootProjectCode + "-" + companyCode + "-S";

                List<Doc> docList = proj.dBSource.SelectDoc(string.Format("select * from CDMS_Doc where o_itemname like '%{0}%' and o_dmsstatus !=10 order by o_itemname", strPrefix));
                if (docList == null || docList.Count == 0)
                {
                    return "0001";
                }
                else
                {
                    Doc doc = docList[docList.Count - 1];
                    int tempNum = Convert.ToInt32(doc.O_itemname.Substring(strPrefix.Length, 4));
                    return (tempNum + 1).ToString("d4");
                }
            }
            catch
            {
                return "0001";
            }

        }
    }
}
