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
    public class ExchangeDoc
    {
        /// <summary>
        /// 获取创建内部互提资料表单的默认选项
        /// </summary>
        /// <param name="sid">连接密钥</param>
        /// <param name="ProjectKeyword">目录关键字</param>
        /// <param name="ExchangeType">互提资料的类型，Create：生成提资单，Continue：继续提资，UpEdition：提资升版</param>
        /// <returns></returns>
        public static JObject GetExchangeDocDefault(string sid, string ProjectKeyword, string ExchangeType)
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

                //定位到提出资料目录
                m_Project = LocalProject(m_Project);

                if (m_Project == null)
                {
                    reJo.msg = "参数错误！文件夹不存在！";
                    return reJo.Value;
                }

                //赋值: 项目代码，项目名称，阶段，专业
                string strPrjName = m_Project.GetValueByKeyWord("DESIGNPROJECT_DESC");
                string strPrjCode = m_Project.GetValueByKeyWord("DESIGNPROJECT_CODE");
                string designPhase = m_Project.GetValueByKeyWord("DESIGNPHASE_DESC");
                if (string.IsNullOrEmpty(designPhase)) designPhase = m_Project.GetValueByKeyWord("DESIGNPHASE_CODE");
                string[] strArray = designPhase.Split(new string[] { "阶段" }, StringSplitOptions.RemoveEmptyEntries);
                //this.txtPhase.Text = strArray[0];
                string strPhase = strArray[0];
                string strOutProfession = m_Project.GetValueByKeyWord("PROFESSION_DESC");

                //获取设计阶段目录
                Project parentProject = m_Project;
                while ((parentProject.TempDefn != null) && (parentProject.TempDefn.Code != "DESIGNPHASE"))
                {
                    parentProject = parentProject.ParentProject;
                    if (parentProject == null)
                    {
                        break;
                    }
                }
                if (parentProject == null)
                {
                    //MessageBox.Show("获取设计阶段目录失败，请联系管理员。");
                    reJo.msg = "获取设计阶段目录失败，请联系管理员。";
                    return reJo.Value;
                }

                //获取专业目录
                IEnumerable<Project> enumerable = from p in parentProject.ChildProjectList
                                                  where (p.TempDefn != null) && (p.TempDefn.Code == "PROFESSION")
                                                  select p;
                if (enumerable == null)
                {
                    //MessageBox.Show("获取专业目录失败，请联系管理员。");
                    reJo.msg = "获取专业目录失败，请联系管理员。";
                    return reJo.Value;
                }

                //设置返回的参数
                JObject joData = new JObject();
                //joData.Add(new JProperty("PrjKeyword", strPrjName));
                joData.Add(new JProperty("PrjName", strPrjName));
                joData.Add(new JProperty("PrjCode", strPrjCode));
                joData.Add(new JProperty("Phase", strPhase));
                //提出专业
                joData.Add(new JProperty("OutProfession", strOutProfession));

                //添加专业列表，不添加本专业
                string strReceiver = "";
                Dictionary<string, string> dicPro2User;
                dicPro2User = new Dictionary<string, string>();

                JObject joPressionList = new JObject();
                JArray jaPressionList = new JArray();

                Project pressionProject = CommonFunction.GetProfession(m_Project);

                foreach (Project project in enumerable)
                {
                    AttrData attrDataByKeyWord = project.GetAttrDataByKeyWord("PROFESSIONOWNER");
                    if (attrDataByKeyWord != null && pressionProject.O_projectno != project.O_projectno)
                    {
                        jaPressionList.Add(new JObject(
                            new JProperty("Pression", project.ToString),//添加专业
                            new JProperty("OwnerUser", attrDataByKeyWord.ToString)//添加主设人
                            ));
                        //joPressionList.Add(new JProperty());
                        //this.cblInProfession.Items.Add(project.ToString);
                        dicPro2User.Add(project.Code, attrDataByKeyWord.ToString);
                        string strOwnerUser = attrDataByKeyWord.ToString;
                        //strReceiver = strReceiver + dicPro2User[str11.Substring(0, str11.IndexOf("__"))] + ",";
                    }
                }
                string strDocNum1 = m_Project.GetValueByKeyWord("PROFESSION_CODE");

                joData.Add(new JProperty("DocNum1", strDocNum1));

                joData.Add(new JProperty("PressionList", jaPressionList));

                string text1 = m_Project.ExcuteDefnExpression("$(DESIGNPROJECT_CODE)")[0];



                //继续提资
                //if (menustate == 2)
                if (ExchangeType == "Continue")
                {
                    string strTitle = m_Project.GetValueByKeyWord("ED_TITLE");
                    string strContent = m_Project.GetValueByKeyWord("ED_CONTENT");

                    string str8 = m_Project.O_projectname.Substring(0, m_Project.O_projectname.LastIndexOf("号"));
                    string strDocNum2 = str8.Substring(str8.IndexOf("字第") + 2);


                    string strDocNum3 = m_Project.ToString.Substring(0, m_Project.O_projectname.IndexOf(str8));
                    //string strEDProfession = m_Project.GetAttrDataByKeyWord("ED_PROFESSION").ToString;
                    //if (toString != "")
                    //{
                    //    for (int i = 0; i < this.cblInProfession.Items.Count; i++)
                    //    {
                    //        string str10 = this.cblInProfession.Items[i].ToString();
                    //        if (toString.Contains(str10.Substring(0, str10.IndexOf("__"))))
                    //        {
                    //            this.cblInProfession.SetItemChecked(i, true);
                    //        }
                    //    }
                    //}


                    //foreach (string str11 in this.cblInProfession.CheckedItems)
                    //{
                    //    strReceiver = strReceiver + dicPro2User[str11.Substring(0, str11.IndexOf("__"))] + ",";
                    //}
                    strReceiver = strReceiver.TrimEnd(new char[] { ',' });
                    strDocNum3 = m_Project.GetValueByKeyWord("EXCHANGELAST");
                    //string strPriNormal = m_Project.GetValueByKeyWord("ED_IMPORTANCEDOC");

                    joData.Add(new JProperty("Title", strTitle));
                    joData.Add(new JProperty("Content", strContent));
                    joData.Add(new JProperty("DocNum2", strDocNum2));
                    //joData.Add(new JProperty("EDProfession", strEDProfession));
                    joData.Add(new JProperty("Receiver", strReceiver));
                    joData.Add(new JProperty("DocNum3", strDocNum3));

                }

                //提资升版
                //else if (menustate == 3)
                else if (ExchangeType == "UpEdition")
                {
                    string[] Edition;
                    Edition = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

                    string str3 = m_Project.O_projectname.Substring(0, m_Project.O_projectname.LastIndexOf("号"));
                    string strDocNum2 = str3.Substring(str3.IndexOf("字第") + 2);
                    string str4 = m_Project.GetValueByKeyWord("UPEDITIONNUM");
                    string strContent = "";
                    string strDocNum3 = "";
                    if (!string.IsNullOrEmpty(str4))
                    {
                        int index = Convert.ToInt32(str4);
                        strDocNum3 = "(修" + Edition[index] + ")";
                        if (index > 0)
                        {
                            strContent = "本提资单取代" + m_Project.O_projectname + "(修" + Edition[index - 1] + ")互提资料单, 原" + m_Project.O_projectname + "(修" + Edition[index - 1] + ")互提资料单作废";
                        }
                        else
                        {
                            strContent = "本提资单取代" + m_Project.O_projectname + "互提资料单, 原" + m_Project.O_projectname + "互提资料单作废。";
                        }
                    }
                    string strTitle = m_Project.GetValueByKeyWord("ED_TITLE");

                    joData.Add(new JProperty("Title", strTitle));
                    joData.Add(new JProperty("Content", strContent));
                    joData.Add(new JProperty("DocNum2", strDocNum2));

                    joData.Add(new JProperty("DocNum3", strDocNum3));
                    //joData.Add(new JProperty("PriNormal", strPriNormal));

                }
                else //if (ExchangeType == "Create") //创建新的互提资料单
                {
                    string strDocNum2 = "";
                    //取资料编号
                    try
                    {
                        Project p = m_Project.ChildProjectList.Last();
                        string str3 = p.O_projectname.Substring(0, p.O_projectname.LastIndexOf("号"));
                        strDocNum2 = (int.Parse(str3.Substring(str3.IndexOf("字第") + 2)) + 1).ToString();
                        strDocNum2 = strDocNum2.PadLeft(3, '0');
                    }
                    catch
                    {
                        strDocNum2 = "001";
                    }
                    joData.Add(new JProperty("DocNum2", strDocNum2));
                }

                //文档重要性
                string strPriNormal = "";
                if (m_Project.GetValueByKeyWord("IMPORTANCE") != null)
                {
                    strPriNormal = m_Project.GetValueByKeyWord("IMPORTANCE");
                }
                joData.Add(new JProperty("PriNormal", strPriNormal));

                //根据之前生成的互提资料单，判断要发送到哪些专业
                string strEDProfession = "";
                string strEDReceiver = "";
                if ((m_Project.DocList != null) && (m_Project.DocList.Count > 0))
                {
                    Doc doc = null;
                    foreach (Doc doc2 in m_Project.DocList)
                    {
                        if ((doc2.TempDefn != null) && (doc2.TempDefn.Code == "EXCHANGEDOC"))
                        {
                            doc = doc2;
                            break;
                        }
                    }
                    if (doc != null)
                    {
                        strEDProfession = doc.GetAttrDataByKeyWord("ED_PROFESSION").ToString;
                        if (strEDProfession != "")
                        {
                            //for (int j = 0; j < this.cblInProfession.Items.Count; j++)
                            //{
                            //    if (str5.Contains(this.cblInProfession.Items[j].ToString()))
                            //    {
                            //        this.cblInProfession.SetItemChecked(j, true);
                            //    }
                            //}


                        }

                        strEDReceiver = doc.GetAttrDataByKeyWord("ED_RECEIVER").ToString;
                        //if (doc.GetAttrDataByKeyWord("ED_RECEIVER").ToString != "")
                        if (strEDReceiver != "")
                        {
                            //this.txtReceiver.Text = "";
                            //foreach (string str7 in this.cblInProfession.CheckedItems)
                            //{
                            //    this.txtReceiver.Text = this.txtReceiver.Text + this.dicPro2User[str7.Substring(0, str7.IndexOf("__"))] + ",";
                            //}
                            //this.txtReceiver.Text = this.txtReceiver.Text.TrimEnd(new char[] { ',' });

                        }
                    }

                }

                joData.Add(new JProperty("EDProfession", strEDProfession));
                joData.Add(new JProperty("Receiver", strEDReceiver));

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
        /// 定位到提出资料目录
        /// </summary>
        /// <param name="prj"></param>
        /// <returns></returns>
        private static Project LocalProject(Project prj)
        {

            //判断是否为升版
            if (prj.TempDefn != null && prj.TempDefn.KeyWord == "INNERNUMBER")
            {
                //this.IsUpEdition = true;
                return prj;
            }
            else
            {

                //找到提出资料目录
                Project p = prj;
                while (p != null)
                {
                    if (p.TempDefn != null && p.TempDefn.KeyWord == "PROFESSION")
                    {
                        break;
                    }
                    p = p.ParentProject;
                }


                //创建目录
                Project pp = p.NewProject("内部接口", "", prj.Storage, prj.dBSource.GetTempDefnByCode("EXCHANGE")[0]);
                if (pp != null)
                {
                    prj = pp.NewProject("提出资料", "");
                    return prj;
                }
                {
                    //AssistFun.PopUpPrompt("没有:提出资料 目录！不能提资");
                    //this.Close();
                }

                return null;
            }

        }

        //线程锁 
        internal static Mutex muxConsole = new Mutex();

        /// <summary>
        /// 处理生成提资单,继续提资和提资升版
        /// </summary>
        /// <param name="sid">sid</param>
        /// <param name="ProjectKeyword">选择的Project</param>
        /// <param name="ExchangeType">互提资料的类型，Create：生成提资单，Continue：继续提资，UpEdition：提资升版</param>
        /// <param name="docAttrJson">参数字符串</param>
        /// <returns>
        /// <para>docAttrJson例子：</para>
        /// <code>
        ///  [
        ///    { name: 'PrjName', value: prjName }, //工程名称
        ///    { name: 'PrjCode', value: PrjCode }, //工程编号
        ///    { name: 'Phase', value: Phase }, //设计阶段
        ///    { name: 'DocNum1', value: DocNum1 }, //资料单编号
        ///    { name: 'DocNum2', value: DocNum2 }, //资料单编号
        ///    { name: 'DocNum3', value: DocNum3 }, //资料单编号
        ///    { name: 'OutProfession', value: OutProfession }, //提出专业
        ///    { name: 'ProfessionList', value: professionList },  //接收专业 
        ///    { name: 'Receiver', value: Receiver },   //签收人
        ///    { name: 'Title', value: Title }, //资料标题
        ///    { name: 'Content', value: content }, //内容
        ///    { name: 'PriImport', value: priImport }, //资料重要性
        ///    { name: 'BookQuantity', value: bookQuantity },   //图纸数量
        ///    { name: 'FormQuantity', value: formQuantity }    //表单数量
        /// ]
        /// 
        /// //生成提资单流程：
        /// //1.客户端发送提资单的参数
        /// //2.服务端创建互提资料单doc，生成存储目录，
        /// //3.客户端上传附件给服务端
        /// //4.服务端创建附件doc,接收附件，并保存到存储目录,把附件doc的Keyword发送给客户端
        /// //5.客户端发送创建流程的消息给服务端
        /// //6.服务端创建提资流程
        /// //本函数负责第2个步骤，函数ExchangeDocStartWorkFlow负责第6个步骤
        ///</code>
        /// </returns>
        public static JObject CreateExchangeDoc(string sid, string ProjectKeyword, string ExchangeType, string docAttrJson)
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

                //定位到提出资料目录
                m_Project = LocalProject(m_Project);

                if (m_Project == null)
                {
                    reJo.msg = "参数错误！文件夹不存在！";
                    return reJo.Value;
                }



                #region 获取互提资料表单的属性

                string strPrjName = "", strPrjCode = "", strPhase = "",
                    strDocNum1 = "", strDocNum2 = "", strDocNum3 = "",
                    strOutProfession = "", strReceiver = "", strTitle = "",
                    strContent = "", strPriImport = "", strProfessionList = "",
                    strBookQuantity="",strFormQuantity = "";

                JArray jaAttr = (JArray)JsonConvert.DeserializeObject(docAttrJson);

                foreach (JObject joAttr in jaAttr)
                {
                    string strName = joAttr["name"].ToString();
                    string strValue = joAttr["value"].ToString();

                    //获取工程名称
                    if (strName == "PrjName") strPrjName = strValue;

                    //获取工程编号
                    else if (strName == "PrjCode") strPrjCode = strValue;

                    //获取设计阶段
                    else if (strName == "Phase") strPhase = strValue;

                    //获取资料单编号
                    else if (strName == "DocNum1") strDocNum1 = strValue;

                    //获取资料单编号
                    else if (strName == "DocNum2") strDocNum2 = strValue;

                    //获取资料单编号
                    else if (strName == "DocNum3") strDocNum3 = strValue;

                    //获取提出专业
                    else if (strName == "OutProfession") strOutProfession = strValue;

                    //获取签收人
                    else if (strName == "Receiver") strReceiver = strValue;

                    //获取资料标题
                    else if (strName == "Title") strTitle = strValue;

                    //获取内容
                    else if (strName == "Content") strContent = strValue;

                    //获取资料重要性
                    else if (strName == "PriImport") strPriImport = strValue;

                    //获取接收专业
                    else if (strName == "ProfessionList") strProfessionList = strValue;

                    //获取图纸数量
                    else if (strName == "BookQuantity") strBookQuantity = strValue;

                    //获取表单数量
                    else if (strName == "FormQuantity") strFormQuantity = strValue;
                }
                #endregion
                
                //判断编号是否重号
                string m_ProjectName = strDocNum1 + "字第" + strDocNum2 + "号" + strDocNum3;
                if (m_Project.Code == "提出资料")
                {
                    List<Project> P_name = m_Project.ChildProjectList;
                    foreach (Project p in P_name)
                    {
                        if (p.Code == m_ProjectName)
                        {
                            //MessageBox.Show("已存在该编号，请正确填写资料单编号！");
                            //return;
                            reJo.msg = "已存在该编号，请正确填写资料单编号！";
                            return reJo.Value;
                        }
                    }
                }


                //if (((this.m_ucFileContainer.Items.Count > 0) || (DialogResult.No != MessageBox.Show("当前尚未选择附件，是否确定不选择附件提资？", "操作确认", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk))) && (m_Project != null))
                {
                    string enclosure;
                    //m_Project.dBSource.ProgramRun = true;
                    //if (this.CheckInputOK())
                    //if (this.cblInProfession.CheckedItems.Count == 0)
                    if (string.IsNullOrEmpty(strProfessionList))
                    {
                        //AssistFun.PopUpPrompt("尚未选择接受专业！");
                        reJo.msg = "尚未选择接受专业！";
                        return reJo.Value;
                    }

                    if (string.IsNullOrEmpty(strReceiver))
                    {
                        reJo.msg = "尚未选择签收人！";
                        return reJo.Value;
                    }

                    {
                        //定义文档模板
                        //this.Cursor = Cursors.WaitCursor;
                        List<TempDefn> tempDefnByCode = m_Project.dBSource.GetTempDefnByCode("EXCHANGEDOC");
                        TempDefn mTempDefn = (tempDefnByCode != null) ? tempDefnByCode[0] : null;
                        if (mTempDefn == null)
                        {
                            //AssistFun.PopUpPrompt("没有与其相关的模板管理，创建无法正常完成");
                            //this.Cursor = Cursors.Default;
                            reJo.msg = "没有与其相关的模板管理，创建无法正常完成";
                            return reJo.Value;
                        }
                        else
                        {
                            string mProjectName = strDocNum1 + "字第" + strDocNum2 + "号" + strDocNum3;


                            //if (!this.IsUpEdition)
                            //如果不是提资升版
                            if (ExchangeType != "UpEdition")
                            {
                                Project projectByName = m_Project.NewProject(mProjectName, strTitle);
                                if (projectByName == null)
                                {
                                    WebApi.CommonController.WebWriteLog("当前m_Project " + m_Project.ToString);
                                    //MessageBox.Show("创建资料目录:" + mProjectName + " 失败！");
                                    //return;
                                    reJo.msg = "创建资料目录:" + mProjectName + " 失败！";
                                    return reJo.Value;
                                }
                                m_Project = projectByName;
                            }
                            AttrData attrDataByKeyWord = m_Project.GetAttrDataByKeyWord("EXCHANGE");
                            if (attrDataByKeyWord != null)
                            {
                                attrDataByKeyWord.SetCodeDesc(strTitle);
                            }

                            //如果是提资升版
                            if (ExchangeType == "UpEdition")
                            {
                                AttrData ad = m_Project.GetAttrDataByKeyWord("UPEDITIONNUM");
                                if (ad != null)
                                {
                                    ad.SetCodeDesc((Convert.ToInt32(ad.ToString) + 1).ToString());
                                }
                            }

                            //文档目录
                            AttrData data3 = m_Project.GetAttrDataByKeyWord("IMPORTANCE");//资料重要性
                            data3.SetCodeDesc(strPriImport);

                            m_Project.AttrDataList.SaveData();


                            //文件列表字符串
                            enclosure = "";

                            ///获取附件列表
                            //if ((this.m_lstAttach != null) && (this.m_lstAttach.Count > 0))
                            //{

                            //    this.m_lstAttach.ForEach(delegate (Doc d)
                            //    {
                            //        enclosure = enclosure + d.ToString + "\r\n";
                            //    });
                            //    this.m_lstAttach.ForEach(delegate (Doc d)
                            //    {
                            //        this.m_DocList.Add(d);
                            //    });
                            //}
                            //else
                            //{
                            //    enclosure = this.ProcessEnclosure(mProjectName + " 互提资料单");
                            //}

                            IEnumerable<string> source = from docx in m_Project.DocList select docx.Code;
                            string filename = mProjectName + " 互提资料单";
                            if (source.Contains<string>(filename))
                            {
                                for (int i = 1; i < 0x3e8; i++)
                                {
                                    filename = mProjectName + " 互提资料单" + i.ToString();
                                    if (!source.Contains<string>(filename))
                                    {
                                        break;
                                    }
                                }
                            }


                            //文档名称
                            Doc item = m_Project.NewDoc(filename + ".doc", filename, "", mTempDefn);
                            if (item == null)
                            {
                                //AssistFun.PopUpPrompt("新建互提资料单出错！");
                                //this.Cursor = Cursors.Default;
                                reJo.msg = "新建互提资料单出错！";
                                return reJo.Value;
                            }
                            else
                            {

                                //添加附加属性
                                //string codeDescStr = "";
                                //string profession = "";
                                //for (int j = 0; j < this.cblInProfession.CheckedItems.Count; j++)
                                //{
                                //    string str = this.cblInProfession.CheckedItems[j].ToString();
                                //    codeDescStr = codeDescStr + str + ",";
                                //    profession = profession + str.Substring(str.IndexOf("__") + 2) + "\n";
                                //}

                                //if (codeDescStr.EndsWith(","))
                                //{
                                //    codeDescStr = codeDescStr.Remove(codeDescStr.Length - 1);
                                //}
                                //if (profession.EndsWith("\n"))
                                //{
                                //    profession = profession.Remove(profession.Length - 1);
                                //}

                                string strDocList = "";//获取附件

                                string codeDescStr = strProfessionList;
                                string profession = strProfessionList.Replace(",", "\n");

                                ///获取接收用户

                                string strRec = CommonFunction.getUserCodelist(strReceiver);

                                //AVEVA.CDMS.Server.Group group = CommonFunction.StrToGroup(dbsource,strReceiver);

                                //string strRec = "";
                                //if (group != null)
                                //{
                                //    foreach (User user in group.UserList)
                                //    {
                                //        strRec = strRec+user.Code + ",";
                                //    }
                                //}

                                //if (strRec.EndsWith(","))
                                //{
                                //    strRec = strRec.Remove(strRec.Length - 1);
                                //}

                                //文档
                                item.GetAttrDataByKeyWord("ED_PROFESSION").SetCodeDesc(codeDescStr);
                                item.GetAttrDataByKeyWord("ED_TITLE").SetCodeDesc(strTitle);
                                item.GetAttrDataByKeyWord("ED_CONTENT").SetCodeDesc(strContent);
                                item.GetAttrDataByKeyWord("ED_FILENO").SetCodeDesc(strDocNum1 + "字第" + strDocNum2 + "号" + strDocNum3);
                                item.GetAttrDataByKeyWord("ED_RECEIVER").SetCodeDesc( strReceiver);//strRec);//
                                item.GetAttrDataByKeyWord("ED_MAKEDATE").SetCodeDesc(DateTime.Now.ToString("yyyy-MM-dd"));
                                item.GetAttrDataByKeyWord("ED_ENCLOSURE").SetCodeDesc(enclosure);

                                //item.Project.AttrDataList.SaveData();
                                //item.Project.Modify();

                                //if (this.rbPriImport.Checked)
                                //{
                                //    data3.SetCodeDesc("综合性重要资料");
                                //}
                                //else if (this.rbPriNormal.Checked)
                                //{
                                //    data3.SetCodeDesc("一般资料");
                                //}
                                item.AttrDataList.SaveData();

                                //录入数据进入表单
                                Hashtable htUserKeyWord = new Hashtable();
                                //if (this.rbPriNormal.Checked)
                                if (strPriImport == "一般资料")
                                {
                                    htUserKeyWord.Add("DATA_NOR", "☑");//一般
                                    htUserKeyWord.Add("DTAT_IMP", "□");//重要
                                }
                                else
                                {
                                    htUserKeyWord.Add("DATA_NOR", "□");//一般
                                    htUserKeyWord.Add("DTAT_IMP", "☑");//重要
                                }
                                htUserKeyWord.Add("CODE", mProjectName);//编号
                                htUserKeyWord.Add("PRJNAME", strPrjName);//项目名
                                htUserKeyWord.Add("PRJCODE", strPrjCode);//项目号
                                htUserKeyWord.Add("DESIGNPHASE", strPhase);//设计阶段
                                htUserKeyWord.Add("DOCNUMBER1", filename.ToString());//资料名称
                                htUserKeyWord.Add("SENDDATE", DateTime.Now.ToString("yyyy年MM月dd日"));//发出日期
                                htUserKeyWord.Add("OUTPROFESSION", strOutProfession);//提出专业
                                htUserKeyWord.Add("INPROFESSION", profession);
                                htUserKeyWord.Add("RECEIVER", strReceiver);
                                htUserKeyWord.Add("MORESIGNIDEA", strTitle + "\r\n       " + strContent);
                                htUserKeyWord.Add("ENCLOSURE", enclosure);
                                htUserKeyWord.Add("PREPAREDSIGN1", m_Project.dBSource.LoginUser.Code);
                                htUserKeyWord.Add("BOOK", strBookQuantity);
                                htUserKeyWord.Add("FORM", strFormQuantity);


                                //this.Cursor = Cursors.WaitCursor;
                                string workingPath = m_Project.dBSource.LoginUser.WorkingPath;
                                AttrData ad = m_Project.GetAttrDataByKeyWord("ISSAVE");
                                if (ad != null)
                                {
                                    ad.SetCodeDesc("");
                                }
                                m_Project.AttrDataList.SaveData();


                                try
                                {
                                    //上传下载文档
                                    string exchangfilename = "专业间互提资料单";

                                    //获取网站路径
                                    string sPath = System.Web.HttpContext.Current.Server.MapPath("/ISO/HXPC/");

                                    //获取模板文件路径
                                    string modelFileName = sPath + exchangfilename + ".doc";

                                    //获取即将生成的联系单文件路径
                                    string locFileName = item.FullPathFile;

                                    //FTPFactory factory = m_Project.Storage.FTP ?? new FTPFactory(m_Project.Storage);
                                    //string locFileName = m_Project.dBSource.LoginUser.WorkingPath + item.Code + ".doc";
                                    //factory.download(@"\ISO\" + exchangfilename + ".doc", locFileName, false);
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
                                            office.WriteDataToDocument(item, locFileName, htUserKeyWord, htUserKeyWord);
                                        }
                                        catch { }
                                        finally
                                        {

                                            //解锁
                                            muxConsole.ReleaseMutex();
                                        }
                                    }


                                    int length = (int)info.Length;
                                    item.O_size = new int?(length);
                                    item.Modify();
                                    //base.DialogResult = DialogResult.OK;
                                    //this.Cursor = Cursors.Default;
                                    //base.Close();
                                    //CommonFunction.InsertDocListAndOpenDoc(this.m_DocList, item);

                                    if (string.IsNullOrEmpty(strDocList))
                                    {
                                        strDocList = item.KeyWord;
                                    }
                                    else
                                    {
                                        strDocList = item.KeyWord + "," + strDocList;
                                    }

                                    //这里刷新数据源，否则创建流程的时候获取不了专业字符串
                                    DBSourceController.RefreshDBSource(sid);

                                    reJo.success = true;
                                    reJo.data = new JArray(new JObject(new JProperty("ProjectKeyword", item.Project.KeyWord),
                                        new JProperty("DocKeyword", item.KeyWord), new JProperty("DocList", strDocList)));
                                    return reJo.Value;
                                }
                                catch { }

                                return reJo.Value;
                                //启动流程
                                //ExReJObject startWfReJo= ExchangeDocStartWorkFlow(item, "EXCHANGEDOC");
                                //if (startWfReJo.success == false)
                                //{
                                //    return startWfReJo.Value;
                                //}


                                //m_Project.dBSource.ProgramRun = false;
                                //CallBackParam param = new CallBackParam();
                                //if (ExMenu.callTheApp != null)
                                //{
                                //    CallBackResult result;
                                //    param.mask = 2;
                                //    param.dList = new List<Doc> { item };
                                //    param.callType = enCallBackType.DocSelectD;
                                //    param.dbs = item.dBSource;
                                //    ExMenu.callTheApp(param, out result);
                                //}
                            }
                        }
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
        /// 启动互提资料流程
        /// </summary>
        /// <param name="sid">连接密钥</param>
        /// <param name="docKeyword">互提资料文档关键字</param>
        /// <param name="DocList">附件关键字列表，使用 ','分隔</param>
        /// <returns></returns>
        public static JObject ExchangeDocStartWorkFlow(string sid, string docKeyword, string DocList)//Doc doc, string defWFCode)
        {
            //ExReJObject exReJo = new ExReJObject();
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
                {
                    //Thread.Sleep(300);
                    //if (((doc.OperateDocStatus == enDocStatus.OUT) && (doc.FLocker == doc.dBSource.LoginUser)) || (doc.OperateDocStatus == enDocStatus.COMING_IN))
                    //{
                    //    //MessageBox.Show("文档处于检出状态，请先保存文档，并关闭该文档");
                    //    //this.StartWorkFlow(doc, defWFCode);
                    //    //StartWorkFlow(doc, defWFCode);
                    //}
                    //else
                    {
                        //获取目录或文档对象列表
                        //List<Object> objList = new List<object>();
                        string[] strArray = (string.IsNullOrEmpty(DocList) ? "" : DocList).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                        List<Doc> m_DocList = new List<Doc>();
                        m_DocList.Reverse();
                        //m_DocList.Add(doc);
                        foreach (string strObj in strArray)
                        {
                            object obj = dbsource.GetObjectByKeyWord(strObj);
                            //objList.Add(obj);
                            if (obj is Doc)
                            {
                                m_DocList.Add((Doc)obj);
                            }
                        }

                        string defWFCode = "EXCHANGEDOC";
                        WorkFlow flow = dbsource.NewWorkFlow(m_DocList, defWFCode.ToUpper());
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
                                    //dbsource.ProgramRun = false;
                                    flow.Delete();
                                    reJo.msg = "新建流程不存在下一状态,提交失败!";
                                    return reJo.Value;
                                    //return;
                                }
                            }


                            //查找主任
                            AttrData ad = doc.GetAttrDataByKeyWord("PROFESSIONMANAGER");
                            if (ad == null || ad.group == null || ad.group.UserList.Count <= 0)
                            {
                                //AssistFun.PopUpPrompt("本专业没有设置主设，不能启动流程，请设置主设后再启动流程！");
                                //return;
                                reJo.msg = "本专业没有设置主设，不能启动流程，请设置主设后再启动流程！";
                                return reJo.Value;
                            }


                            //启动流程
                            WorkStateBranch branch = flow.CuWorkState.workStateBranchList[0];
                            branch.NextStateAddGroup(ad.group);

                            ExReJObject GotoNextReJo = WebWorkFlowEvent.GotoNextStateAndSelectUser(flow.CuWorkState.workStateBranchList[0]);
                            if (!GotoNextReJo.success)
                            {
                              //  dbsource.ProgramRun = false;
                                flow.Delete();
                                flow.Delete();

                                reJo.msg = "自动启动流程失败！请手动启动流程";
                                return reJo.Value;
                            }

                            //流程状态不为空时进入
                            if (doc.WorkFlow != null)
                            {

                                //创建校核状态
                                DefWorkState checkDefWS = doc.WorkFlow.DefWorkFlow.DefWorkStateList.Find(dwsx => dwsx.O_Code == "CHECK");
                                WorkState checkstate = doc.WorkFlow.NewWorkState(checkDefWS);


                                //创建审核状态
                                DefWorkState auditDefWS = doc.WorkFlow.DefWorkFlow.DefWorkStateList.Find(dwsx => dwsx.O_Code == "AUDIT");
                                WorkState auditstate = doc.WorkFlow.NewWorkState(auditDefWS);

                                //查找设计阶段
                                Project p = doc.Project;
                                while (p != null && p.ParentProject != null)
                                {
                                    //找设计阶段
                                    if (p != null && p.TempDefn != null && p.TempDefn.KeyWord == "DESIGNPHASE")
                                    {
                                        break;
                                    }
                                    p = p.ParentProject;
                                }

                                AttrData attrdata = doc.GetAttrDataByKeyWord("ED_PROFESSION");
                                string strProfessionList = attrdata.ToString;

                                string[] arrProfession = strProfessionList.Split(new char[] { ',' });
                                //当前文档专业等于选择专业时添加接受人员状态
                                //循环选择专业
                                //foreach (object obj in this.cblInProfession.CheckedItems)
                                foreach (string strProfession in arrProfession)
                                {
                                    foreach (Project fs in p.ChildProjectList)
                                    {
                                        //string str = obj.ToString();
                                        string mProject_fessionName = strProfession.Substring(0, strProfession.IndexOf("__"));
                                        string mProject_fessiondesc = strProfession.Substring(strProfession.IndexOf("__") + 2);
                                        if (fs.Code == mProject_fessionName)
                                        {

                                            //主设人
                                            AttrData mad = fs.GetAttrDataByKeyWord("PROFESSIONOWNER");
                                            AVEVA.CDMS.Server.Group group = null;
                                            if (mad != null) group = mad.group;

                                            //为每个专业接受人创建一个状态
                                            DefWorkState defWorkState = doc.WorkFlow.DefWorkFlow.DefWorkStateList.Find(dwsx => dwsx.O_Code == "RECEIVE");
                                            WorkState state = doc.WorkFlow.NewWorkState(defWorkState);
                                            state.SaveSelectUser(group);
                                            state.IsRuning = false;
                                            state.PreWorkState = auditstate;
                                            state.O_iuser5 = new int?(doc.WorkFlow.CuWorkState.O_stateno);
                                            state.O_suser3 = fs.Description;  //接受专业
                                            state.Modify();

                                        }
                                    }
                                }

                                DBSourceController.RefreshDBSource(sid);

                                reJo.success = true;
                                return reJo.Value;
                            }

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
                    }
                }
            }
            catch (Exception exception)
            {
                WebApi.CommonController.WebWriteLog(exception.Message + "\r\n" + exception.Source + "\r\n" + exception.StackTrace);
                //AssistFun.PopUpPrompt(exception.Message + "\r\n" + exception.Source + "\r\n" + exception.StackTrace);
                reJo.msg = "启动流程失败！" + exception.Message + "\r\n" + exception.Source + "\r\n" + exception.StackTrace;
            }
            return reJo.Value;
        }

    }
}
