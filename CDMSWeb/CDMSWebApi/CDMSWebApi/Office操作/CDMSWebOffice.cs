using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVEVA.CDMS.Server;
//using System.Windows.Forms;
using AVEVA.CDMS.Common;
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using Microsoft.VisualBasic;
using System.Collections;
using System.Text.RegularExpressions;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

using System.Data.Linq; 

namespace AVEVA.CDMS.WebApi
{
    /// <summary>
    /// 处理 Word Excel
    /// </summary>
    public class CDMSWebOffice
    {
        public CDMSWebOffice()
        {

        }



        ~CDMSWebOffice()
        {
            try
            {
                this.Release(true);
            }
            catch { }
        }

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, ref uint ProcessId);

        #region 定义变量 



        /// <summary>
        /// 是否为最终状态，最终状态，则文字使用" "代替，图片使用空白图片代替
        /// </summary>
        public  bool IsFinial = false;

        //小黎 增加是否为更新数据，若为更新数据则要删除原先的内容
        public bool IsUpdateFileData = false;

        //小黎 是否Word2007
        bool IsWord07 = false;

        //小黎 2012-6-15 增加是否为升版,升版时要求把签过名的图片都去掉
        public bool IsUpdateVersion = false;

        /// <summary>
        /// Word应用对象
        /// </summary>
        protected Word.Application m_wdApp;

        public Word.Application WordApp
        {
            get { return m_wdApp;  }
        }

        /// <summary>
        /// 文档对象
        /// </summary>
        protected Word.Document m_wdDoc;

       

        /// <summary>
        /// Excel应用对象
        /// </summary>
        protected Excel.Application m_exApp;

        public Excel.Application ExcelApp
        {
            get { return m_exApp;  }
        }

        /// <summary>
        /// 工作簿
        /// </summary>
        protected Excel.Workbook m_exWorkBook;

        /// <summary>
        /// 工作表
        /// </summary>
        protected Excel.Worksheet m_exWorksheet;

        /// <summary>
        /// 是否已经插入了表格行
        /// </summary>
        protected bool m_bIsInsertRow;

        /// <summary>
        /// 缺省值
        /// </summary>
        protected object m_oMissing = System.Reflection.Missing.Value;

        /// <summary>
        /// Word是否已经打开
        /// </summary>
        protected bool m_bWordIsRunning;

        /// <summary>
        /// Excel是否已经打开
        /// </summary>
        protected bool m_bExcelIsRunning;

        /// <summary>
        /// 错误信息列表
        /// </summary>
        protected ArrayList _errorList = new ArrayList();

        /// <summary>
        /// 是否显示Word或Excel,默认 显示
        /// </summary>
        protected bool _visibleApp = true;

        /// <summary>
        /// 是否显示Word或Excel,默认 不显示
        /// </summary>
        public bool VisibleApp
        {
            get { return _visibleApp; }
            set { _visibleApp = value; }
        }

        /// <summary>
        /// 是否关闭Word或Excel,默认 关闭
        /// </summary>
        protected bool _closeApp = true;

        /// <summary>
        /// 是否关闭Word或Excel,默认 关闭
        /// </summary>
        public bool CloseApp
        {
            get { return _closeApp; }
            set { _closeApp = value; }
        }

        /// <summary>
        /// 遇上错误是否关闭Word或Excel,默认 false 不关闭
        /// </summary>
        protected bool _closeErrorApp = false;

        /// <summary>
        /// 遇上错误是否关闭Word或Excel,默认 false 不关闭
        /// </summary>
        public bool CloseErrorApp
        {
            get { return _closeErrorApp; }
            set { _closeErrorApp = value; }
        }


        /// <summary>
        /// 错误信息列表
        /// </summary>
        public ArrayList ErrorList
        {
            get { return _errorList; }
            set { _errorList = value; }
        }



        /// <summary>
        /// 所有错误信息
        /// </summary>
        public String LastError
        {
            get
            {
                string _lastError = "";
                foreach (string err in ErrorList)
                {
                    _lastError = _lastError + "\n" + err;
                }
                return _lastError; 
            }
        }

        #endregion

        #region 打开word


        /// <summary>
        /// 打开文档
        /// </summary>
        /// <param name="sFileName">文件名</param>
        /// <returns>打开文件是否成功</returns>
        public void OpenWord(object sFileName)
        {
            OpenWordNew(sFileName);
        }


        /// <summary>
        /// 打开文档
        /// </summary>
        /// <param name="sFileName">文件名</param>
        /// <param name="bVisible">是否显示文档</param>
        /// <returns>打开文件是否成功</returns>
        public bool OpenWordNew(object sFileName)
        {
            try
            {
                string filename = sFileName as string;


                //文件不存在
                if (!File.Exists(filename)) return false;

                try
                {
                    //获取当前正在运行的Word
                    //m_wdApp = Interaction.GetObject(null, "Word.Application") as Word.Application;
                    //m_wdApp = Marshal.GetActiveObject("Word.Application") as Word.Application;
                    m_wdApp = new Microsoft.Office.Interop.Word.ApplicationClass(); 
                    
                }
                catch(Exception ex0)
                {
                    //2010-04-30
                    _errorList.Add(ex0.ToString());
                    AVEVA.CDMS.Server.CDMS.Write(ex0.ToString()); 
                    m_wdApp = null;
                }

                if (m_wdApp == null)
                {
                    //创建Word应用程序
                    //m_wdApp = Interaction.CreateObject("Word.Application", "") as Word.Application;
                    
                    //2009-01-14
                    Type wdType = Type.GetTypeFromProgID("Word.Application");
                    m_wdApp = Activator.CreateInstance(wdType, true) as Word.Application;


                    //m_bWordIsRunning 表示当前是否有另外的word打开,false表示没有
                    m_bWordIsRunning = false;
                }

                if (m_wdApp == null) return false;

                if (!m_bWordIsRunning)
                {
                    //是否显示word
                    m_wdApp.Visible = _visibleApp;
                }


                //打开Word
                m_wdApp.Documents.Open(ref sFileName,
                                       ref m_oMissing,
                                       ref m_oMissing,
                                       ref m_oMissing,
                                       ref m_oMissing,
                                       ref m_oMissing,
                                       ref m_oMissing,
                                       ref m_oMissing,
                                       ref m_oMissing,
                                       ref m_oMissing,
                                       ref m_oMissing,
                                       ref m_oMissing,
                                       ref m_oMissing,
                                       ref m_oMissing,
                                       ref m_oMissing,
                                       ref m_oMissing
                                       );

                m_wdDoc = m_wdApp.ActiveDocument;


                //成功
                return true;
              
            }
            catch (Exception ex)
            {
            
                //2010-04-30
                _errorList.Add(ex.ToString());
                AVEVA.CDMS.Server.CDMS.Write(ex.ToString()); 

                //出现异常则释放资源
                Release(true);


                //失败
                return false;
            }
        }


        #endregion

        #region 判断word是否已经打开

        /// <summary>
        /// 判断word是否已经打开
        /// </summary>
        /// <param name="wdAppl">word应用程序</param>
        /// <param name="sFileName">文件名</param>
        /// <returns>是否正在运行</returns>
        protected bool WordIsRunning(Word.Application wdAppl, string sFileName)
        {
            if (wdAppl == null) return false;

            if (wdAppl.Tasks.Exists(sFileName))
            {
                //已运行
                return true;
            }
            return false;
        }

        #endregion

        #region 保存文档

        /// <summary>
        /// 保存文档
        /// </summary>
        /// <param name="oFileName">文档</param>
        public void SaveWord(object oFileName)
        {

            if (oFileName == null || m_wdApp == null || m_wdDoc == null) return;


            //20110416 Word改名保存, 对于表格里面压缩后的文字必须改名后保存才能生效
            string oldfilename = (string)oFileName;
            int indx = oldfilename.LastIndexOf(".");
            string newfilename = oldfilename.Substring(0, indx) + System.DateTime.Now.ToString("yyyyMMddHHmmss") + "." + oldfilename.Substring(indx + 1);
            object nFileName = newfilename;
            //小黎 2012-4-11 对于word2007,为了不弹出保存Normal.dotm模板所要做的设置
            m_wdApp.NormalTemplate.Saved = true;
            //END
            try
            {
                m_wdDoc.SaveAs(ref nFileName,
                               ref m_oMissing,
                               ref m_oMissing,
                               ref m_oMissing,
                               ref m_oMissing,
                               ref m_oMissing,
                               ref m_oMissing,
                               ref m_oMissing,
                               ref m_oMissing,
                               ref m_oMissing,
                               ref m_oMissing,
                               ref m_oMissing,
                               ref m_oMissing,
                               ref m_oMissing,
                               ref m_oMissing,
                               ref m_oMissing
                               );
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK);
                //2010-04-30
                _errorList.Add(ex.ToString());
                AVEVA.CDMS.Server.CDMS.Write(ex.ToString());

                //出错后释放资源
                Release(true);
            }
            finally
            {
                //资源释放,_closeApp表示是否释放资源关闭Word或Excel,ture表示关闭
                Release(_closeApp);


                //文件改名
                File.Delete(oldfilename);
                File.Move(newfilename, oldfilename);
            }
        }
      

        /// <summary>
        /// 资源释放,是否释放资源,关闭Word或Excel
        /// </summary>
        /// <param name="bRelease">是否释放资源,关闭Word或Excel</param>
        public void Release(bool bRelease)
        {
            if (bRelease == false) return;


            try
            {

                if (m_bWordIsRunning)
                {
                    //************
                    //释放Word资源
                    if (m_wdDoc != null)
                    {
                        try
                        {
                            //关闭文档对象
                            ((Word._Document)m_wdDoc).Close(ref m_oMissing, ref m_oMissing, ref m_oMissing);
                        }
                        catch { }
                    }
                }
                else
                {
                    //************
                    //释放Word资源
                    if (m_wdDoc != null)
                    {
                        try
                        {
                            //关闭文档对象
                            ((Word._Document)m_wdDoc).Close(ref m_oMissing, ref m_oMissing, ref m_oMissing);
                        }
                        catch { }
                    }
                    if (m_wdApp != null)
                    {
                        try
                        {
                            ((Word._Application)m_wdApp).Quit(ref m_oMissing, ref m_oMissing, ref m_oMissing);
                        }
                        catch { }
                    }
                }

                if (m_bExcelIsRunning)
                {
                    //*************
                    //释放Excel资源
                    if (m_exWorkBook != null)
                    {
                        try
                        {
                            m_exWorkBook.Close(m_oMissing, m_oMissing, m_oMissing);
                        }
                        catch { }
                    }
                }
                else
                {
                    //*************
                    //释放Excel资源
                    if (m_exWorkBook != null)
                    {
                        try
                        {
                            m_exWorkBook.Close(m_oMissing, m_oMissing, m_oMissing);
                        }
                        catch { }
                    }
                    if (m_exApp != null)
                    {
                      
                            //关闭Excel
                            IntPtr ptr = new IntPtr(m_exApp.Hwnd);
                            uint pid = 0;
                            GetWindowThreadProcessId(ptr, ref pid);

                            try
                            {
                                m_exApp.Quit();
                                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(m_exApp);
                            }
                            catch { }

                            Process appProcess = null; 
                            appProcess = System.Diagnostics.Process.GetProcessById((int)pid);

                            if (appProcess != null)
                            {
                                appProcess.Kill();
                            }
                       
                            m_exApp = null; 
                    }
                }
            }
            catch (Exception ex)
            {
                CDMS.Server.CDMS.Write(ex.ToString()); 
            }
        }


        #endregion

        #region 设置word大小

        /// <summary>
        /// 设置Word页面的大小
        /// </summary>
        /// <param name="left">与左侧的距离</param>
        /// <param name="top">与上侧的距离</param>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        protected void SetWordSize(int left, int top, int width, int height)
        {
            try
            {
                if (m_wdApp == null) return;

                m_wdApp.WindowState = Word.WdWindowState.wdWindowStateNormal;
                m_wdApp.Left = left;
                m_wdApp.Top = top;
                m_wdApp.Width = width;
                m_wdApp.Height = height;

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK);
                //2010-04-30
                _errorList.Add(ex.ToString());
                AVEVA.CDMS.Server.CDMS.Write(ex.ToString()); 
            }
        }

        #endregion

        #region 在文档中插入 单值域(其中包括自定义域、图片域) ； 多值域

        /// <summary>
        /// 插入自定义域
        /// </summary>
        /// <param name="sKeyWord">关键字</param>
        public Word.Field InsertField(string sKeyWord)
        {
            //域
            Word.Field wdField = null;
            try
            {
                //当前选中对象
                Word.Selection wdSelection;

                //折叠方向
                object oDirection;

                //域的类型
                object oWdFieldType;

                oDirection = Word.WdCollapseDirection.wdCollapseEnd;

                //自定义域的类型
                oWdFieldType = Word.WdFieldType.wdFieldAddin;

                m_wdApp.Selection.Collapse(ref oDirection);

                wdSelection = m_wdApp.Selection;

                //表示多值域
                if ((sKeyWord.Substring(sKeyWord.Length - 1) == "F"))
                {
                    if (!IsCell(wdSelection))
                    {
                        _errorList.Add("该位置不是单元格,请选择单元格");
                        return null;
                    }
                }

                wdField = m_wdDoc.Fields.Add(wdSelection.Range, ref oWdFieldType, ref m_oMissing, ref m_oMissing);
                wdField.Data = sKeyWord;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK);
                //2010-04-30
                _errorList.Add(ex.ToString());
                AVEVA.CDMS.Server.CDMS.Write(ex.ToString()); 

                //出现异常则释放资源
                Release(true);
            }
            return wdField;
        }


        /// <summary>
        /// 插入图片域
        /// </summary>
        /// <param name="sKeyWord">关键字</param>
        public Word.Field InsertPictureField(string sKeyWord)
        {
            Word.Field wdField = null;
            try
            {
                Word.Selection wdSelection;
                object oDirection;

                //域类型
                object oWdFieldType;

                object oKeyWord;

                oDirection = Word.WdCollapseDirection.wdCollapseEnd;

                //Word中IncludePicture类型的域
                oWdFieldType = Word.WdFieldType.wdFieldIncludePicture;

                oKeyWord = sKeyWord;

                m_wdApp.Selection.Collapse(ref oDirection);

                wdSelection = m_wdApp.Selection;

                //增加域
                wdField = m_wdDoc.Fields.Add(wdSelection.Range, ref oWdFieldType, ref oKeyWord, ref m_oMissing);

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK);
                //2010-04-30
                _errorList.Add(ex.ToString());
                AVEVA.CDMS.Server.CDMS.Write(ex.ToString()); 
                //出现异常则释放资源
                Release(true);
            }
            return wdField;
        }

        #endregion

        #region 选择点(光标)是否是单元格

        /// <summary>
        /// 选择点是否为单元格
        /// </summary>
        /// <param name="selection"></param>
        /// <returns></returns>
        protected bool IsCell(Word.Selection selection)
        {

            if (selection.Tables.Count > 0)
                return true;
            else
                return false;
        }

        #endregion

        #region 计算选择点(光标)的单元格的域数

        /// <summary>
        /// 计算选择点(光标)的单元格的域数
        /// </summary>
        /// <param name="selection">所选区域</param>
        /// <returns>单元格的域数</returns>
        protected int CellFiledCount(Word.Selection selection)
        {
            return selection.Cells[1].Range.Fields.Count;
        }


        #endregion

        #region 调用选择表达式窗口

        ///// <summary>
        ///// 调用选择表达式窗口,并返回选择的字符串
        ///// 修改者：张灿学
        ///// 修改时间：2008-10-17
        ///// 修改原因：选择关键字对话框的构造形式错误，不能传入Project 或 Doc的模板，而是传入整个DBSource
        /////           传入模板的形式会将Project 或 Doc的这个模板给排出在外，
        ///// </summary>
        ///// <param name="oProjectOrDoc">Project或Doc对象</param>
        ///// <returns>所选的表达式</returns>
        //protected string GetExpression(object oProjectOrDoc)
        //{
        //    string expression = "";

        //    try
        //    {
        //        if (oProjectOrDoc == null) return "";

        //        DBSource source = null;

        //        if (oProjectOrDoc is Project)
        //        {
        //            source = (oProjectOrDoc as Project).dBSource;
        //        }
        //        else if (oProjectOrDoc is Doc)
        //        {
        //            source = (oProjectOrDoc as Doc).dBSource;
        //        }
        //        if (source == null) return "";

        //        //选择关键字窗口
        //        fmSelKeyWord fmSel = new fmSelKeyWord(source);

        //        //表达式
        //        fmSel.IsExpression = true;
        //        fmSel.ShowDialog();

        //        if (fmSel.KeyWord != null && fmSel.KeyWord.Trim().Length > 0)
        //        {
        //            expression = fmSel.KeyWord;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK);
        //        //2010-04-30
        //        _errorList.Add(ex.ToString());
        //        AVEVA.CDMS.Server.CDMS.Write(ex.ToString()); 
        //    }
        //    return expression;
        //}

        #endregion

        #region 填写Word的域值

        List<Word.Field> picFieldList = new List<Microsoft.Office.Interop.Word.Field>(); 
        /// <summary>
        /// 填写Word的域得值，包括自定义域(Word.WdFieldType.wdFieldAddin)、图片域(Word.WdFieldType.wdFieldIncludePicture)。
        /// </summary>
        /// <param name="oProjectOrDoc">Project或Doc</param>
        /// <param name="htUserKeyWord">存放用户关键字</param>
        /// <returns>是否成功填写完毕域的值 true:成功；false:失败。</returns>
        public bool FillWdFieldValue(object oProjectOrDoc, Hashtable htUserKeyWord)
        {
            try
            {
                picFieldList.Clear(); 
                if (oProjectOrDoc == null) return false;

                //从选择关键字窗口中获取到的表达式。
                string sExpression;

                //解析表达式后的值,调用ExcuteDefnExpression函数后返回的值, 
                //ExcuteDefnExpression : project.ExcuteDefnExpression(sKeyWord)，返回的是数组,获取第一个元素。
                string sExpressValue;

                //用来保存图片域(Word.WdFieldType.wdFieldIncludePicture)
                Hashtable htPicFields = new Hashtable();

                //用来保存自定义域(Word.WdFieldType.wdFieldAddin类型的)
                Hashtable htAddinFields = new Hashtable();




                if (m_wdDoc.Fields != null && m_wdDoc.Fields.Count > 0)
                {
                    //*******************************************
                    //查找所有域，用实际的值进行替换（图片和文字）
                    for (int i = 1; i <= m_wdDoc.Fields.Count; i++)
                    {
                        if (!this.IsWord07)
                        {
                            m_wdDoc.Fields[i].Update();
                        }

                        //**********************************************
                        #region 处理域类型为WdFieldType.wdFieldAddin的域

                        if (m_wdDoc.Fields[i].Type == Word.WdFieldType.wdFieldAddin)
                        {
                            sExpression = m_wdDoc.Fields[i].Data;

                            if (String.IsNullOrEmpty(sExpression))
                                continue;

                            //加上"F"表示多值域
                            if (sExpression.Substring(sExpression.Length - 1) != "F")
                            {
                                if (m_wdDoc.Fields[i].Locked == false)
                                {
                                    //若包含"Pic:",表示该域是由图片域转换至自定义域的
                                    if (sExpression.Contains("Pic:"))
                                    {
                                        //*******************************************************************************
                                        //保存到哈希表htAddinFields，用来保存自定义域(Word.WdFieldType.wdFieldAddin类型的)
                                        //哈希表key,哈希表htAddinFields,htAddinFields的value为Field
                                        string sFieldKey = m_wdDoc.Fields[i].Index.ToString() + m_wdDoc.Fields[i].Code.Text;
                                        if (!htAddinFields.ContainsKey(sFieldKey))
                                        {
                                            htAddinFields.Add(sFieldKey, m_wdDoc.Fields[i]);
                                        }
                                        continue;
                                    }
                                    #region 填写自定义域的值

                                    //获取表达式的值
                                    sExpressValue = GetExpressValue(oProjectOrDoc, htUserKeyWord, sExpression);


                                    //2011.8.1 如果是文字，并且设置了最终状态，则写入空字符串
                                    if (this.IsFinial && string.IsNullOrEmpty(sExpressValue))
                                    {
                                        sExpressValue = " ";
                                    }


                                    //填写自定义域的值,填写完毕后为域加锁
                                    FillAddInFiled(sExpressValue, m_wdDoc.Fields[i]);

                                    #endregion
                                }
                            }
                        }
                        #endregion

                        //**********************************************************************
                        #region 处理图片  $(CurrentUser_Code).JPG，用于取得用户上传的数字签名图片

                        else if (m_wdDoc.Fields[i].Type == Word.WdFieldType.wdFieldIncludePicture)
                        {
                            //图片域未加锁表示，图片未填写
                            if (m_wdDoc.Fields[i].Locked == false)
                            {
                                if (FillPicField(oProjectOrDoc, htUserKeyWord, m_wdDoc.Fields[i], true) == false)
                                {

                                    //WrapFormat.AllowOverlap = True
                                    //Selection.ShapeRange.WrapFormat.Side = wdWrapBoth
                                    //Selection.ShapeRange.WrapFormat.DistanceTop = CentimetersToPoints(0)
                                    //Selection.ShapeRange.WrapFormat.DistanceBottom = CentimetersToPoints(0)
                                    //Selection.ShapeRange.WrapFormat.DistanceLeft = CentimetersToPoints(0.32)
                                    //Selection.ShapeRange.WrapFormat.DistanceRight = CentimetersToPoints(0.32)
                                    //Selection.ShapeRange.WrapFormat.Type = 3




                                    //保存未处理的图片域
                                    //htPicFields.Add(m_wdDoc.Fields[i].Index.ToString() + m_wdDoc.Fields[i].Code.Text, m_wdDoc.Fields[i]);
                                }
                                else
                                {
                                    //htPicFields.Add(m_wdDoc.Fields[i].Index.ToString() + m_wdDoc.Fields[i].Code.Text, m_wdDoc.Fields[i]); ;
                                    //i--; 
                                }


                            }
                        }

                        #endregion
                    }

                    #region 把保存在htAddinFields的自定义域进行解析，把其解析为图片域,并填写图片域
                    foreach (DictionaryEntry de in htAddinFields)
                    {
                        //自定义域 (Word.WdFieldType.wdFieldAddin)
                        Word.Field addinField = de.Value as Word.Field;

                        //自定义域的域代码
                        string addinCodeText = addinField.Data;

                        //取自定义域中"Pic:"之后的域代码(如有域代码 Pic: INCLUDEPICTURE "C:\\k.jpg" \* MERGEFORMAT ，只需取
                        //INCLUDEPICTURE "C:\\k.jpg" \* MERGEFORMAT ,其中INCLUDEPICTURE "C:\\k.jpg" \* MERGEFORMAT表示图片域)
                        string sPicCodeText = addinCodeText.Split(new string[] { "Pic:" }, StringSplitOptions.RemoveEmptyEntries)[0];

                        //根据从自定义域中获取的图片域中的表达式域代码生成图片域
                        //取表达式 从类似" INCLUDEPICTURE $(USER_CODE).JPG \\* MERGEFORMAT "的域代码取表达式
                        int index = sPicCodeText.IndexOf("INCLUDEPICTURE") + 14;
                        string sExprInField = sPicCodeText.Substring(index, sPicCodeText.LastIndexOf("\\") - index);

                        addinField.Select();
                        addinField.Delete();
                        Word.Field picField = InsertPictureField(sExprInField);

                        //若不能正常填写图片域，则保存到哈希表
                        if (FillPicField(oProjectOrDoc, htUserKeyWord, picField) == false)
                        {
                            if (!htPicFields.ContainsKey(picField.Index.ToString() + picField.Code.Text))
                            {
                                //保存未处理的图片域
                                htPicFields.Add(picField.Index.ToString() + picField.Code.Text, picField);
                            }
                        }
                    }

                    #endregion


                    //清除保存在哈希表中的每一个图片域，并相应地增加一个文字域，把原来图片域的域代码得信息保存到文字域，并隐藏文字域
                    foreach (DictionaryEntry de in htPicFields)
                    {

                        /* TIM 2010-10-26  图片域填写失败,则不做任何处理
                        //图片域 (Word.WdFieldType.wdFieldIncludePicture)
                        Word.Field picField = de.Value as Word.Field;
                        string sFieldCodeText = picField.Code.Text;
                        picField.Select();
                        picField.Delete();

                        //取图片域的域代码并在前加上"Pic:"生成一个自定义域(Word.WdFieldType.wdFieldAddin)
                        Word.Field addinField = InsertField("Pic:" + sFieldCodeText);
                        if (addinField != null)
                        {
                            addinField.ShowCodes = false;
                        }
                       */
                        //Word.Field picField = de.Value as Word.Field;
                        //picField.InlineShape.ConvertToShape(); 

                    }

                }

                //*********************************
                //扫描Shpe对象中的域,并填写其域的值
                FillShapeField(oProjectOrDoc, htUserKeyWord);




                ////TODO: 需要删除的测试代码
                ////扫描页眉中的域 , 并填写其域的值
                this.m_wdApp.ActiveWindow.ActivePane.View.SeekView =
                      Microsoft.Office.Interop.Word.WdSeekView.wdSeekCurrentPageHeader;







                this.m_wdApp.Selection.WholeStory();





                if (this.m_wdApp.Selection.Fields != null && this.m_wdApp.Selection.Fields.Count > 0)
                {
                    htAddinFields.Clear();
                    htPicFields.Clear();
                    //*******************************************
                    //查找所有域，用实际的值进行替换（图片和文字）
                    for (int i = 1; i <= this.m_wdApp.Selection.Fields.Count; i++)
                    {
                        //**********************************************
                        #region 处理域类型为WdFieldType.wdFieldAddin的域

                        if (this.m_wdApp.Selection.Fields[i].Type == Word.WdFieldType.wdFieldAddin)
                        {
                            sExpression = this.m_wdApp.Selection.Fields[i].Data;

                            if (String.IsNullOrEmpty(sExpression))
                                continue;
                            //加上"F"表示多值域
                            if (sExpression.Substring(sExpression.Length - 1) != "F")
                            {
                                if (this.m_wdApp.Selection.Fields[i].Locked == false)
                                {
                                    //若包含"Pic:",表示该域是由图片域转换至自定义域的
                                    if (sExpression.Contains("Pic:"))
                                    {
                                        //*******************************************************************************
                                        //保存到哈希表htAddinFields，用来保存自定义域(Word.WdFieldType.wdFieldAddin类型的)
                                        //哈希表key,哈希表htAddinFields,htAddinFields的value为Field
                                        string sFieldKey = this.m_wdApp.Selection.Fields[i].Index.ToString() + m_wdApp.Selection.Fields[i].Code.Text;
                                        if (!htAddinFields.ContainsKey(sFieldKey))
                                        {
                                            htAddinFields.Add(sFieldKey, m_wdApp.Selection.Fields[i]);
                                        }
                                        continue;
                                    }
                                    #region 填写自定义域的值

                                    //获取表达式的值
                                    sExpressValue = GetExpressValue(oProjectOrDoc, htUserKeyWord, sExpression);



                                    //2011.8.1 如果是文字，并且设置了最终状态，则写入空字符串
                                    if (this.IsFinial && string.IsNullOrEmpty(sExpressValue))
                                    {
                                        sExpressValue = " ";
                                    }



                                    //填写自定义域的值,填写完毕后为域加锁
                                    FillAddInFiled(sExpressValue, this.m_wdApp.Selection.Fields[i]);

                                    #endregion
                                }
                            }
                        }
                        #endregion

                        //**********************************************************************
                        #region 处理图片  $(CurrentUser_Code).JPG，用于取得用户上传的数字签名图片

                        else if (this.m_wdApp.Selection.Fields[i].Type == Word.WdFieldType.wdFieldIncludePicture)
                        {
                            //图片域未加锁表示，图片未填写
                            if (this.m_wdApp.Selection.Fields[i].Locked == false)
                            {
                                if (FillPicField(oProjectOrDoc, htUserKeyWord, this.m_wdApp.Selection.Fields[i]) == false)
                                {
                                    //保存未处理的图片域
                                    htPicFields.Add(this.m_wdApp.Selection.Fields[i].Index.ToString() + this.m_wdApp.Selection.Fields[i].Code.Text, this.m_wdApp.Selection.Fields[i]);
                                }
                            }
                        }
                        #endregion



                        this.m_wdApp.Selection.WholeStory();
                    }

                    #region 把保存在htAddinFields的自定义域进行解析，把其解析为图片域,并填写图片域
                    foreach (DictionaryEntry de in htAddinFields)
                    {
                        //自定义域 (Word.WdFieldType.wdFieldAddin)
                        Word.Field addinField = de.Value as Word.Field;

                        //自定义域的域代码
                        string addinCodeText = addinField.Data;

                        //取自定义域中"Pic:"之后的域代码(如有域代码 Pic: INCLUDEPICTURE "C:\\k.jpg" \* MERGEFORMAT ，只需取
                        //INCLUDEPICTURE "C:\\k.jpg" \* MERGEFORMAT ,其中INCLUDEPICTURE "C:\\k.jpg" \* MERGEFORMAT表示图片域)
                        string sPicCodeText = addinCodeText.Split(new string[] { "Pic:" }, StringSplitOptions.RemoveEmptyEntries)[0];

                        //根据从自定义域中获取的图片域中的表达式域代码生成图片域
                        //取表达式 从类似" INCLUDEPICTURE $(USER_CODE).JPG \\* MERGEFORMAT "的域代码取表达式
                        int index = sPicCodeText.IndexOf("INCLUDEPICTURE") + 14;
                        string sExprInField = sPicCodeText.Substring(index, sPicCodeText.LastIndexOf("\\") - index);

                        addinField.Select();
                        addinField.Delete();
                        Word.Field picField = InsertPictureField(sExprInField);

                        //若不能正常填写图片域，则保存到哈希表
                        if (FillPicField(oProjectOrDoc, htUserKeyWord, picField) == false)
                        {
                            if (!htPicFields.ContainsKey(picField.Index.ToString() + picField.Code.Text))
                            {
                                //保存未处理的图片域
                                htPicFields.Add(picField.Index.ToString() + picField.Code.Text, picField);
                            }
                        }
                    }

                    #endregion

                    //清除保存在哈希表中的每一个图片域，并相应地增加一个文字域，把原来图片域的域代码得信息保存到文字域，并隐藏文字域
                    foreach (DictionaryEntry de in htPicFields)
                    {
                        /* TIM 2010-10-26  图片域填写失败,则不做任何处理
                       //图片域 (Word.WdFieldType.wdFieldIncludePicture)
                       Word.Field picField = de.Value as Word.Field;
                       string sFieldCodeText = picField.Code.Text;
                       picField.Select();
                       picField.Delete();

                       //取图片域的域代码并在前加上"Pic:"生成一个自定义域(Word.WdFieldType.wdFieldAddin)
                       Word.Field addinField = InsertField("Pic:" + sFieldCodeText);
                       if (addinField != null)
                       {
                           addinField.ShowCodes = false;
                       }
                       */
                    }
                }


                //HDF2011.4.19 页眉中表格里面的文字进行压缩处理 
                if (this.m_wdApp.Selection.Tables != null && this.m_wdApp.Selection.Tables.Count > 0)
                {
                    foreach (Word.Table tb in this.m_wdApp.Selection.Tables)
                    {
                        for (int rw = 0; rw < tb.Rows.Count; rw++)
                        {
                            for (int cl = 0; cl < tb.Columns.Count; cl++)
                            {
                                try
                                {

                                    //需要压缩才进行处理
                                    if (tb.Cell(rw, cl).FitText)
                                    {
                                        string text = tb.Cell(rw, cl).Range.Text;
                                        if (text.Contains("\r\a")) text = text.Replace("\r\a", " ");
                                        if (!string.IsNullOrEmpty(text.Trim()) && Asc(text.Substring(0, 1)) > 30)
                                        {
                                            tb.Cell(rw, cl).Range.Text = "";
                                            tb.Cell(rw, cl).Range.Text = text;

                                        }
                                    }
                                }
                                catch (Exception ex)
                                {

                                }
                            }

                        }
                    }
                }



                //扫描页脚中的域 , 并填写其域的值
                this.m_wdApp.ActiveWindow.ActivePane.View.SeekView =
                      Microsoft.Office.Interop.Word.WdSeekView.wdSeekCurrentPageFooter;

                this.m_wdApp.Selection.WholeStory();




                if (this.m_wdApp.Selection.Fields != null && this.m_wdApp.Selection.Fields.Count > 0)
                {
                    htAddinFields.Clear();
                    htPicFields.Clear();
                    //*******************************************
                    //查找所有域，用实际的值进行替换（图片和文字）
                    for (int i = 1; i <= this.m_wdApp.Selection.Fields.Count; i++)
                    {
                        //**********************************************
                        #region 处理域类型为WdFieldType.wdFieldAddin的域

                        if (this.m_wdApp.Selection.Fields[i].Type == Word.WdFieldType.wdFieldAddin)
                        {
                            sExpression = this.m_wdApp.Selection.Fields[i].Data;

                            if (String.IsNullOrEmpty(sExpression))
                                continue;
                            //加上"F"表示多值域
                            if (sExpression.Substring(sExpression.Length - 1) != "F")
                            {
                                if (this.m_wdApp.Selection.Fields[i].Locked == false)
                                {
                                    //若包含"Pic:",表示该域是由图片域转换至自定义域的
                                    if (sExpression.Contains("Pic:"))
                                    {
                                        //*******************************************************************************
                                        //保存到哈希表htAddinFields，用来保存自定义域(Word.WdFieldType.wdFieldAddin类型的)
                                        //哈希表key,哈希表htAddinFields,htAddinFields的value为Field
                                        string sFieldKey = this.m_wdApp.Selection.Fields[i].Index.ToString() + m_wdApp.Selection.Fields[i].Code.Text;
                                        if (!htAddinFields.ContainsKey(sFieldKey))
                                        {
                                            htAddinFields.Add(sFieldKey, m_wdApp.Selection.Fields[i]);
                                        }
                                        continue;
                                    }
                                    #region 填写自定义域的值

                                    //获取表达式的值
                                    sExpressValue = GetExpressValue(oProjectOrDoc, htUserKeyWord, sExpression);


                                    //2011.8.1 如果是文字，并且设置了最终状态，则写入空字符串
                                    if (this.IsFinial && string.IsNullOrEmpty(sExpressValue))
                                    {
                                        sExpressValue = " ";
                                    }



                                    //填写自定义域的值,填写完毕后为域加锁
                                    FillAddInFiled(sExpressValue, this.m_wdApp.Selection.Fields[i]);

                                    #endregion
                                }
                            }
                        }
                        #endregion

                        //**********************************************************************
                        #region 处理图片  $(CurrentUser_Code).JPG，用于取得用户上传的数字签名图片

                        else if (this.m_wdApp.Selection.Fields[i].Type == Word.WdFieldType.wdFieldIncludePicture)
                        {
                            //图片域未加锁表示，图片未填写
                            if (this.m_wdApp.Selection.Fields[i].Locked == false)
                            {
                                if (FillPicField(oProjectOrDoc, htUserKeyWord, this.m_wdApp.Selection.Fields[i]) == false)
                                {
                                    //保存未处理的图片域
                                    htPicFields.Add(this.m_wdApp.Selection.Fields[i].Index.ToString() + this.m_wdApp.Selection.Fields[i].Code.Text, this.m_wdApp.Selection.Fields[i]);
                                }
                            }
                        }
                        #endregion



                        this.m_wdApp.Selection.WholeStory();
                    }

                    #region 把保存在htAddinFields的自定义域进行解析，把其解析为图片域,并填写图片域
                    foreach (DictionaryEntry de in htAddinFields)
                    {
                        //自定义域 (Word.WdFieldType.wdFieldAddin)
                        Word.Field addinField = de.Value as Word.Field;

                        //自定义域的域代码
                        string addinCodeText = addinField.Data;

                        //取自定义域中"Pic:"之后的域代码(如有域代码 Pic: INCLUDEPICTURE "C:\\k.jpg" \* MERGEFORMAT ，只需取
                        //INCLUDEPICTURE "C:\\k.jpg" \* MERGEFORMAT ,其中INCLUDEPICTURE "C:\\k.jpg" \* MERGEFORMAT表示图片域)
                        string sPicCodeText = addinCodeText.Split(new string[] { "Pic:" }, StringSplitOptions.RemoveEmptyEntries)[0];

                        //根据从自定义域中获取的图片域中的表达式域代码生成图片域
                        //取表达式 从类似" INCLUDEPICTURE $(USER_CODE).JPG \\* MERGEFORMAT "的域代码取表达式
                        int index = sPicCodeText.IndexOf("INCLUDEPICTURE") + 14;
                        string sExprInField = sPicCodeText.Substring(index, sPicCodeText.LastIndexOf("\\") - index);

                        addinField.Select();
                        addinField.Delete();
                        Word.Field picField = InsertPictureField(sExprInField);

                        //若不能正常填写图片域，则保存到哈希表
                        if (FillPicField(oProjectOrDoc, htUserKeyWord, picField) == false)
                        {
                            if (!htPicFields.ContainsKey(picField.Index.ToString() + picField.Code.Text))
                            {
                                //保存未处理的图片域
                                htPicFields.Add(picField.Index.ToString() + picField.Code.Text, picField);
                            }
                        }
                    }

                    #endregion

                    //清除保存在哈希表中的每一个图片域，并相应地增加一个文字域，把原来图片域的域代码得信息保存到文字域，并隐藏文字域
                    foreach (DictionaryEntry de in htPicFields)
                    {
                        /* TIM 2010-10-26  图片域填写失败,则不做任何处理
                       //图片域 (Word.WdFieldType.wdFieldIncludePicture)
                       Word.Field picField = de.Value as Word.Field;
                       string sFieldCodeText = picField.Code.Text;
                       picField.Select();
                       picField.Delete();

                       //取图片域的域代码并在前加上"Pic:"生成一个自定义域(Word.WdFieldType.wdFieldAddin)
                       Word.Field addinField = InsertField("Pic:" + sFieldCodeText);
                       if (addinField != null)
                       {
                           addinField.ShowCodes = false;
                       }
                        */
                    }
                }


                //HDF2011.4.19 页脚中表格里面的文字进行压缩处理 
                if (this.m_wdApp.Selection.Tables != null && this.m_wdApp.Selection.Tables.Count > 0)
                {
                    foreach (Word.Table tb in this.m_wdApp.Selection.Tables)
                    {
                        for (int rw = 0; rw < tb.Rows.Count; rw++)
                        {
                            for (int cl = 0; cl < tb.Columns.Count; cl++)
                            {
                                try
                                {

                                    //需要压缩的格才进行处理
                                    if (tb.Cell(rw, cl).FitText)
                                    {
                                        string text = tb.Cell(rw, cl).Range.Text;
                                        if (text.Contains("\r\a")) text = text.Replace("\r\a", " ");
                                        if (!string.IsNullOrEmpty(text.Trim()) && Asc(text.Substring(0, 1)) > 30)
                                        {
                                            tb.Cell(rw, cl).Range.Text = "";
                                            tb.Cell(rw, cl).Range.Text = text;

                                        }
                                    }
                                }
                                catch { }
                            }

                        }
                    }
                }
              
                //2011.4.16 DOE要求在表格里面进行压缩，将表格里面的文字进行重新设置
                if (m_wdDoc != null)
                {
                    foreach (Word.Table tb in m_wdDoc.Tables)
                    {

                        for (int rw = 0; rw < tb.Rows.Count; rw++)
                        {
                            for (int cl = 0; cl < tb.Columns.Count; cl++)
                            {
                                try
                                {
                                    //需要压缩的格才进行处理
                                    if (tb.Cell(rw, cl).FitText)
                                    {
                                        string text = tb.Cell(rw, cl).Range.Text;
                                        if (text.Contains("\r\a")) text = text.Replace("\r\a", " ");
                                        if (!string.IsNullOrEmpty(text.Trim()) && Asc(text.Substring(0, 1)) > 30)
                                        {
                                            tb.Cell(rw, cl).Range.Text = "";
                                            tb.Cell(rw, cl).Range.Text = text;
                                        }
                                    }
                                }
                                catch { }
                            }

                        }
                    }
                }


                //if (picFieldList.Count >= 0)
                //{
                //    for (int it = 0; it < picFieldList.Count; it++)
                //    {
                //        if (picFieldList[it].InlineShape != null)
                //            picFieldList[it].InlineShape.ConvertToShape(); 
                //    }
                //}

                return true;
            }
            catch (Exception ex)
            {
                //添加错误信息到错误列表中
                if (_errorList != null)
                {
                    {
                        _errorList.Add(ex.Message);
                    }
                }
                AVEVA.CDMS.Server.CDMS.Write(ex.Message); 

                //出现异常则释放资源
                Release(true);
            }
            return false;
        }

        #endregion

        #region 填写自定义域值
        /// <summary>
        /// 填写自定义域值
        /// </summary>
        /// <param name="sExpressValue">要填写的域值</param>
        /// <param name="i"></param>
        protected bool FillAddInFiled(string sExpressValue, Word.Field field)
        {
            try
            {
                //有返回值 则填写
                if (sExpressValue.Length > 0 && field != null)
                {

                    //选中域
                    field.Select();

                    if ( IsUpdateFileData == true )
                    {
                        ChearAddrInValue(field);
                    }

   
                    field.Result.Text = sExpressValue;
                                 
                    //隐藏域代码
                    field.ShowCodes = false;

                    //填写完毕为域加锁
                    field.Locked = true;
                    return true;
                }
            }
            catch(Exception ex)
            {
                //添加错误信息到错误列表中
                if (_errorList != null)
                {
                    {
                        _errorList.Add(ex.Message);
                    }
                }

                //2010-04-30
                AVEVA.CDMS.Server.CDMS.Write(ex.ToString()); 
            }
            return false;
        }
        #endregion

        #region 扫描Shpe对象中的域,并填写其域的值

        /// <summary>
        /// 扫描Shpe对象中的域,并填写其域的值
        /// </summary>
        /// <param name="oProjectOrDoc"></param>
        /// <param name="htUserKeyWord"></param>
        protected void FillShapeField(object oProjectOrDoc, Hashtable htUserKeyWord)
        {
            //Shape对象中的文本层
            Word.TextFrame wdTextFram;

            Word.Range wdTextRange;

            //用来保存图片域(Word.WdFieldType.wdFieldIncludePicture)
            Hashtable htPicFields = new Hashtable();

            //表达式的值
            string[] sShapeValue = new string[] { };

            string sShapeKeyWord;
            try
            {
                //检查所有的Shape对象中的域,并给 WdFieldType.wdFieldAddin 的类型的域赋值
                foreach (Word.Shape shape in m_wdDoc.Shapes)
                {
                    //2018.5.18 小钱 添加容错处理，解决某些审批路径下，部分校审人无法签名问题
                    try
                    {
                        if (shape.AutoShapeType == MsoAutoShapeType.msoShapeRectangle)
                        {


                            shape.Select(ref m_oMissing);

                            if (shape.TextFrame == null) continue;

                            if (shape.TextFrame.TextRange == null) continue;

                            wdTextFram = shape.TextFrame;

                            if (wdTextFram.TextRange != null)
                            {
                                wdTextRange = wdTextFram.TextRange;

                                foreach (Word.Field field in wdTextRange.Fields)
                                {
                                    if (field.Type == Word.WdFieldType.wdFieldAddin)
                                    {
                                        sShapeKeyWord = field.Data;

                                        //加上F表示多值域
                                        if (sShapeKeyWord.Substring(sShapeKeyWord.Length - 1) != "F")
                                        {
                                            //修改如果填了之后就将其锁住
                                            //Added by zcx@yandingsoft at 2008-10-17
                                            if (!field.Locked)
                                            {
                                                //获取表达式的值
                                                if (oProjectOrDoc is Project)
                                                {
                                                    Project project = oProjectOrDoc as Project;
                                                    sShapeValue = project.ExcuteDefnExpression(sShapeKeyWord, htUserKeyWord);
                                                }
                                                else if (oProjectOrDoc is Doc)
                                                {
                                                    Doc doc = oProjectOrDoc as Doc;
                                                    sShapeValue = doc.ExcuteDefnExpression(sShapeKeyWord, htUserKeyWord);
                                                }

                                                //填值并加锁
                                                ////Added by zcx@yandingsoft at 2008-10-17
                                                FillAddInFiled(sShapeValue[0], field);
                                            }
                                        }
                                    }
                                    #region TIM 2010-06-07 处理图片 $(CurrentUser_Code).JPG , 用于取得用户上传的数字签名图片
                                    else if (field.Type == Word.WdFieldType.wdFieldIncludePicture)
                                    {
                                        if (field.Locked == false)
                                        {
                                            if (FillPicField(oProjectOrDoc, htUserKeyWord, field) == false)
                                            {
                                                htPicFields.Add(field.Index.ToString() + field.Code.Text, field);
                                            }
                                        }
                                    }
                                    #endregion
                                }


                                foreach (DictionaryEntry de in htPicFields)
                                {
                                    /* TIM 2010-10-26  图片域填写失败,则不做任何处理
                                   //图片域 (Word.WdFieldType.wdFieldIncludePicture)
                                   Word.Field picField = de.Value as Word.Field;
                                   string sFieldCodeText = picField.Code.Text;
                                   picField.Select();
                                   picField.Delete();

                                   //取图片域的域代码并在前加上"Pic:"生成一个自定义域(Word.WdFieldType.wdFieldAddin)
                                   Word.Field addinField = InsertField("Pic:" + sFieldCodeText);
                                   if (addinField != null)
                                   {
                                       addinField.ShowCodes = false;
                                   }
                                     */
                                }


                            }
                        }
                    }
                    catch { }
                }


                htPicFields.Clear();


                try
                {

                    // 20010-03-24 增加对页眉页脚中的Shapes的处理
                    this.m_wdApp.ActiveWindow.ActivePane.View.SeekView =
                            Microsoft.Office.Interop.Word.WdSeekView.wdSeekCurrentPageHeader;
                    this.m_wdApp.Selection.WholeStory();
                    //检查所有的Shape对象中的域,并给 WdFieldType.wdFieldAddin 的类型的域赋值
                    foreach (Word.Shape shape in this.m_wdApp.Selection.HeaderFooter.Shapes)
                    {

                        if (shape.AutoShapeType == MsoAutoShapeType.msoShapeRectangle)
                        {


                            shape.Select(ref m_oMissing);

                            if (shape.TextFrame == null) continue;

                            if (shape.TextFrame.TextRange == null) continue;

                            wdTextFram = shape.TextFrame;

                            if (wdTextFram.TextRange != null)
                            {
                                wdTextRange = wdTextFram.TextRange;

                                foreach (Word.Field field in wdTextRange.Fields)
                                {
                                    if (field.Type == Word.WdFieldType.wdFieldAddin)
                                    {
                                        sShapeKeyWord = field.Data;

                                        //加上F表示多值域
                                        if (sShapeKeyWord.Substring(sShapeKeyWord.Length - 1) != "F")
                                        {
                                            //修改如果填了之后就将其锁住
                                            //Added by zcx@yandingsoft at 2008-10-17
                                            if (!field.Locked)
                                            {
                                                //获取表达式的值
                                                if (oProjectOrDoc is Project)
                                                {
                                                    Project project = oProjectOrDoc as Project;
                                                    sShapeValue = project.ExcuteDefnExpression(sShapeKeyWord, htUserKeyWord);
                                                }
                                                else if (oProjectOrDoc is Doc)
                                                {
                                                    Doc doc = oProjectOrDoc as Doc;
                                                    sShapeValue = doc.ExcuteDefnExpression(sShapeKeyWord, htUserKeyWord);
                                                }

                                                //填值并加锁
                                                ////Added by zcx@yandingsoft at 2008-10-17
                                                FillAddInFiled(sShapeValue[0], field);
                                            }
                                        }
                                    }

                                    #region 处理图片 $(CurrentUser_Code).JPG , 用于取得用户上传的数字签名图片
                                    else if (field.Type == Word.WdFieldType.wdFieldIncludePicture)
                                    {
                                        if (field.Locked == false)
                                        {
                                            if (FillPicField(oProjectOrDoc, htUserKeyWord, field) == false)
                                            {
                                                htPicFields.Add(field.Index.ToString() + field.Code.Text, field);
                                            }
                                        }
                                    }
                                    #endregion
                                }



                                //清除保存在哈希表中的每一个图片域，并相应地增加一个文字域，把原来图片域的域代码得信息保存到文字域，并隐藏文字域
                                foreach (DictionaryEntry de in htPicFields)
                                {
                                    /* TIM 2010-10-26  图片域填写失败,则不做任何处理
                                   //图片域 (Word.WdFieldType.wdFieldIncludePicture)
                                   Word.Field picField = de.Value as Word.Field;
                                   string sFieldCodeText = picField.Code.Text;
                                   picField.Select();
                                   picField.Delete();

                                   //取图片域的域代码并在前加上"Pic:"生成一个自定义域(Word.WdFieldType.wdFieldAddin)
                                   Word.Field addinField = InsertField("Pic:" + sFieldCodeText);
                                   if (addinField != null)
                                   {
                                       addinField.ShowCodes = false;
                                   }
                                     */
                                }


                            }
                        }
                    }
                }
                catch (Exception subEx)
                {
                    //2010-04-30
                    _errorList.Add(subEx.ToString());
                    AVEVA.CDMS.Server.CDMS.Write(subEx.ToString());
                }


                htPicFields.Clear(); 


                try
                {

                    // 20010-03-24 增加对页眉页脚中的Shapes的处理
                    this.m_wdApp.ActiveWindow.ActivePane.View.SeekView =
                            Microsoft.Office.Interop.Word.WdSeekView.wdSeekCurrentPageFooter;
                    this.m_wdApp.Selection.WholeStory();
                    //检查所有的Shape对象中的域,并给 WdFieldType.wdFieldAddin 的类型的域赋值
                    foreach (Word.Shape shape in this.m_wdApp.Selection.HeaderFooter.Shapes)
                    {
                        if (shape.AutoShapeType == MsoAutoShapeType.msoShapeRectangle)
                        {


                            shape.Select(ref m_oMissing);

                            if (shape.TextFrame == null) continue;

                            if (shape.TextFrame.TextRange == null) continue;

                            wdTextFram = shape.TextFrame;

                            if (wdTextFram.TextRange != null)
                            {
                                wdTextRange = wdTextFram.TextRange;

                                foreach (Word.Field field in wdTextRange.Fields)
                                {
                                    if (field.Type == Word.WdFieldType.wdFieldAddin)
                                    {
                                        sShapeKeyWord = field.Data;

                                        //加上F表示多值域
                                        if (sShapeKeyWord.Substring(sShapeKeyWord.Length - 1) != "F")
                                        {
                                            //修改如果填了之后就将其锁住
                                            //Added by zcx@yandingsoft at 2008-10-17
                                            if (!field.Locked)
                                            {
                                                //获取表达式的值
                                                if (oProjectOrDoc is Project)
                                                {
                                                    Project project = oProjectOrDoc as Project;
                                                    sShapeValue = project.ExcuteDefnExpression(sShapeKeyWord, htUserKeyWord);
                                                }
                                                else if (oProjectOrDoc is Doc)
                                                {
                                                    Doc doc = oProjectOrDoc as Doc;
                                                    sShapeValue = doc.ExcuteDefnExpression(sShapeKeyWord, htUserKeyWord);
                                                }

                                                //填值并加锁
                                                ////Added by zcx@yandingsoft at 2008-10-17
                                                FillAddInFiled(sShapeValue[0], field);
                                            }
                                        }
                                    }

                                    #region 处理图片 $(CurrentUser_Code).JPG , 用于取得用户上传的数字签名图片
                                    else if (field.Type == Word.WdFieldType.wdFieldIncludePicture)
                                    {
                                        if (field.Locked == false)
                                        {
                                            if (FillPicField(oProjectOrDoc, htUserKeyWord, field) == false)
                                            {
                                                htPicFields.Add(field.Index.ToString() + field.Code.Text, field);
                                            }
                                        }
                                    }
                                    #endregion
                                }



                                //清除保存在哈希表中的每一个图片域，并相应地增加一个文字域，把原来图片域的域代码得信息保存到文字域，并隐藏文字域
                                foreach (DictionaryEntry de in htPicFields)
                                {
                                    /*
                                    //图片域 (Word.WdFieldType.wdFieldIncludePicture)
                                    Word.Field picField = de.Value as Word.Field;
                                    string sFieldCodeText = picField.Code.Text;
                                    picField.Select();
                                    picField.Delete();

                                    //取图片域的域代码并在前加上"Pic:"生成一个自定义域(Word.WdFieldType.wdFieldAddin)
                                    Word.Field addinField = InsertField("Pic:" + sFieldCodeText);
                                    if (addinField != null)
                                    {
                                        addinField.ShowCodes = false;
                                    }
                                     */
                                }


                            }
                        }
                    }
                }
                catch (Exception subEx2)
                {
                    //2010-04-30
                    _errorList.Add(subEx2.ToString());
                    AVEVA.CDMS.Server.CDMS.Write(subEx2.ToString());
                }
                


               
            }
            catch (Exception ex)
            {
                //添加错误信息到错误列表中
                if (_errorList != null)
                {
                    {
                        _errorList.Add(ex.Message);
                    }
                }
                //遇上错误时是否关闭正在处理的程序,默认不关闭,false
                //2010-04-30
 
                AVEVA.CDMS.Server.CDMS.Write(ex.ToString()); 
                Release(_closeErrorApp);
            }
        }
        #endregion

        #region 扫描页眉中域,并填写其域的值

        /// <summary>
        /// 扫描页眉中的域,并填写其域的值
        /// </summary>
        /// <param name="oProjectOrDoc"></param>
        /// <param name="htUserKeyWord"></param>
        protected void FillFooterField(object oProjectOrDoc, Hashtable htUserKeyWord)
        {
            //Shape对象中的文本层
            Word.TextFrame wdTextFram;

            Word.Range wdTextRange;

            //表达式的值
            string[] sShapeValue = new string[] { };

            string sShapeKeyWord;
            try
            {
                //激活页眉
                            m_wdDoc.Application.ActiveWindow.ActivePane.View.SeekView =
                Microsoft.Office.Interop.Word.WdSeekView.wdSeekCurrentPageFooter;


                //检查所有的Shape对象中的域,并给 WdFieldType.wdFieldAddin 的类型的域赋值
   

                foreach (Word.Field field in m_wdDoc.Application.Selection.Fields)
                {
                    if (field.Type == Word.WdFieldType.wdFieldAddin)
                    {
                        sShapeKeyWord = field.Data;

                        //加上F表示多值域
                        if (sShapeKeyWord.Substring(sShapeKeyWord.Length - 1) != "F")
                        {
                            //修改如果填了之后就将其锁住
                            //Added by zcx@yandingsoft at 2008-10-17
                            if (!field.Locked)
                            {
                                //获取表达式的值
                                if (oProjectOrDoc is Project)
                                {
                                    Project project = oProjectOrDoc as Project;
                                    sShapeValue = project.ExcuteDefnExpression(sShapeKeyWord, htUserKeyWord);
                                }
                                else if (oProjectOrDoc is Doc)
                                {
                                    Doc doc = oProjectOrDoc as Doc;
                                    sShapeValue = doc.ExcuteDefnExpression(sShapeKeyWord, htUserKeyWord);
                                }

                                //填值并加锁
                                ////Added by zcx@yandingsoft at 2008-10-17
                                FillAddInFiled(sShapeValue[0], field);
                            }
                        }
                    }
                }
                        
                    
                
            }
            catch (Exception ex)
            {
                //添加错误信息到错误列表中
                if (_errorList != null)
                {
                    {
                        _errorList.Add(ex.Message);
                    }
                }
                //遇上错误时是否关闭正在处理的程序,默认不关闭,false
                //2010-04-30
                AVEVA.CDMS.Server.CDMS.Write(ex.ToString()); 
                Release(_closeErrorApp);
            }
        }
        #endregion

        #region 填写图片域


        protected bool FillPicField(object oProjectOrDoc, Hashtable htUserKeyWord, Word.Field wdField)
        {
            return FillPicField(oProjectOrDoc, htUserKeyWord, wdField, false); 
        }
        /// <summary>
        /// 填写图片域
        /// </summary>
        /// <param name="oProjectOrDoc">Project或Doc</param>
        /// <param name="htUserKeyWord">哈希表</param>
        /// <param name="wdField">图片域</param>
        /// <returns>是否成功填写图片域</returns>
        protected bool FillPicField(object oProjectOrDoc, Hashtable htUserKeyWord, Word.Field wdField , bool needBringFront)
        {

            try
            {
                //域代码
                string sCodeText;

                //图片的全路径
                string sPicFullPath = "";

                //在域中的表达式
                string sExprInField;

                //域代码 如 INCLUDEPICTURE $(USER_CODE).JPG \\* MERGEFORMAT
                sCodeText = wdField.Code.Text;

                //取表达式 从类似" INCLUDEPICTURE $(USER_CODE).JPG \\* MERGEFORMAT "的域代码取表达式
                int index = sCodeText.IndexOf("INCLUDEPICTURE") + 14;
 //               sExprInField = sCodeText.Substring(index, sCodeText.LastIndexOf("\\") - index);

                //小黎 2011-11-28 支持word2007的图片域
                sExprInField = wdField.LinkFormat.SourceName;

                //加上F表示多值域，多值域暂不处理
                if (sCodeText.Substring(sCodeText.Length - 1) != "F")
                {
                    //删除图片 modify小黎
                    if (this.IsUpdateFileData == true )
                    {
                        wdField.Select();
                        wdField.InlineShape.Width = 1;
                        object oMoveUnitCharacter2 = Word.WdUnits.wdCharacter;
                        object oCount2 = 1;
                        object oCount3 = 1;
                        m_wdApp.Selection.MoveRight(ref oMoveUnitCharacter2, ref oCount2, ref m_oMissing);
                        m_wdApp.Selection.Delete(ref oMoveUnitCharacter2, ref oCount3);
                        return true;
                    }

                    //******************************
                    //获取表达式的值后下载图片
                    sPicFullPath = GetPicture(oProjectOrDoc, htUserKeyWord, sExprInField);

                    //******************
                    //成功下载则替换图片
                       
                             if (sPicFullPath.Length > 0)
                             {
                               //  m_wdApp.Options.PictureWrapType = Microsoft.Office.Interop.Word.WdWrapTypeMerged.wdWrapMergeFront;
                                 wdField.Select();

                                 //路径名称中\\替换为//,sPicFullPath表示图片的全路径
                                 if (File.Exists(sPicFullPath))
                                 {
                                     Object olink = false;
                                     Object oSave = true;
                                     //Object temprange = wdField.InlineShape.Range;

                                     //Object left = m_wdApp.Selection.ShapeRange.Left ;
                                     //Object top =  m_wdApp.Selection.ShapeRange.Top ;
                                     //Object width = wdField.InlineShape.Width ;
                                     //Object height = wdField.InlineShape.Height;

                                     wdField.ShowCodes = false;
                                     if (needBringFront && wdField.InlineShape != null)
                                     {

                                         wdField.InlineShape.Width = 1;

                                         if (this.IsWord07)
                                         {
                                             wdField.InlineShape.Height = 1;
                                         }

                                         object oMoveUnitCharacter = Word.WdUnits.wdCharacter;

                                         object oCount = 1;


                                         m_wdApp.Selection.MoveRight(ref oMoveUnitCharacter, ref oCount, ref m_oMissing);

                                         //广电院海鸥用上面这句,大庆用下面那一句
                                         //Word.InlineShape inshape = this.m_wdApp.Selection.InlineShapes.AddPicture(sPicFullPath, ref olink, ref oSave, ref temprange);
                                         Word.InlineShape inshape = this.m_wdApp.Selection.InlineShapes.AddPicture(sPicFullPath, ref olink, ref oSave, ref this.m_oMissing);

                                         Word.Shape shape = inshape.ConvertToShape();

                                         //小黎 2011-11-30 增加对word2007的空白图片的修改
                                         if ( this.IsWord07 && this.IsFinial && sPicFullPath.ToLower().Contains("blank.jpg") )
                                         {
                                             shape.WrapFormat.Type = Microsoft.Office.Interop.Word.WdWrapType.wdWrapTight;
                                         }
                                     }
                                     else
                                     {
                                         sPicFullPath = sPicFullPath.Replace("\\", "//");
                                         wdField.Code.Text = " INCLUDEPICTURE  \"" + sPicFullPath + "\"";

                                         //更新域代码
                                         //小黎 2011-12-1 2007的不能更新域代码
                                         if (!this.IsWord07)
                                         {
                                             wdField.Update();
                                             wdField.UpdateSource();
                                         }
                                     }


                                     //shape.ConvertToShape().WrapFormat.Type = Microsoft.Office.Interop.Word.WdWrapType.wdWrapNone;
                                     //shape.ConvertToShape().ZOrder(MsoZOrderCmd.msoBringInFrontOfText); 

                                     //wdField.InlineShape.Borders.

                                     //this.m_wdDoc.Shapes.AddPicture(sPicFullPath,ref olink ,ref oSave ,ref left  ,ref top,ref width , ref height ,ref m_oMissing); 

                                     //sPicFullPath = sPicFullPath.Replace("\\", "//");
                                     //wdField.Code.Text = " INCLUDEPICTURE  \"" + sPicFullPath + "\"";

                                     ////更新域代码
                                     //wdField.Update();
                                     //wdField.UpdateSource();

                                     //////不显示域代码
                           



                                     //////tim 2010-12-09
                                     ////this.SetFieldInFrontOfText(wdField); 


                                     //////锁定域，锁定填写过的域下次就不用填写
                                     wdField.Locked = true;

                                     //wdField.InlineShape.ConvertToShape(); 

                                     //if (!picFieldList.Contains(wdField))
                                     //    picFieldList.Add(wdField); 

                                     return true;
                                 }
                             }
                }

            }
            catch (Exception ex)
            {
                CDMS.Server.CDMS.Write(ex.ToString()); 
            }
            return false;
        }

        #endregion

        #region 获取表达式的值

        /// <summary>
        /// 获取表达式的值
        /// </summary>
        /// <param name="oProjectOrDoc">Project或Doc</param>
        /// <param name="htUserKeyWord">存放用户关键字的哈希表</param>
        /// <param name="sExpress">表达式</param>
        /// <returns>表达式的值</returns>
        protected string GetExpressValue(object oProjectOrDoc, Hashtable htUserKeyWord, string sExpress)
        {
            try
            {
                string[] strsExprValues = new string[] { };

                if (oProjectOrDoc is Project)
                {
                    //获取表达式的值
                    strsExprValues = (oProjectOrDoc as Project).ExcuteDefnExpression(sExpress.Trim(), htUserKeyWord);
                }
                else if (oProjectOrDoc is Doc)
                {
                    //获取表达式的值
                    strsExprValues = (oProjectOrDoc as Doc).ExcuteDefnExpression(sExpress.Trim(), htUserKeyWord);
                }

                //TIM 2010-10-11
                if (strsExprValues.Length > 0 && strsExprValues[0].Trim().Length > 0 && strsExprValues[0] != sExpress)
                {
                    return strsExprValues[0].Trim();
                }
            }
            catch (Exception ex)
            {
                //添加错误信息到错误列表中
                if (_errorList != null)
                {
                    {
                        _errorList.Add(ex.Message);
                    }
                }
                //2010-04-30
               
                AVEVA.CDMS.Server.CDMS.Write(ex.ToString()); 
            }
            return "";
        }

        #endregion 

        #region 从服务器上获取图片

        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="project"></param>
        /// <param name="htUserKeyWord"></param>
        /// <param name="sExprInField"></param>
        /// <param name="sPicFullPath">图片的全路径(包括文件名)</param>
        /// <returns></returns>
        protected string GetPicture(object oProjectOrDoc, Hashtable htUserKeyWord, string sExprInField)
        {
            //图片的全路径(包括文件名)
            string sPicFullPath = "";

            try
            {
                //表达式的值
                string sExprValue;

                //用户的临时工作目录,sTempPath = doc.dBSource.LoginUser.WorkingPath;
                string sUserTempPath = "";

                sExprValue = GetExpressValue(oProjectOrDoc, htUserKeyWord, sExprInField);



                //2011.8.1 如果是图片，并且设置了最终状态，则插入空白图片
                if (this.IsFinial && sExprValue.Substring(0, 1) == ".")
                {
                    sExprValue = "BLANK.jpg";
                }


                //sExprValue可能为 .JPG
                if (!string.IsNullOrEmpty(sExprValue) && sExprValue.Substring(0, 1) != ".")
                {
                    //2018.5.18 小钱 如果没有jpg后缀名，就加上后缀名
                    if (!sExprValue.ToUpper().EndsWith(".JPG"))
                    {
                        sExprValue += ".jpg";
                    }

                    if (oProjectOrDoc is Project)
                    {
                        if ((oProjectOrDoc as Project).dBSource.LoginUser.WorkingPath != null)
                        {
                            sUserTempPath = (oProjectOrDoc as Project).dBSource.LoginUser.WorkingPath;
                        }
                    }
                    else if (oProjectOrDoc is Doc)
                    {
                        if ((oProjectOrDoc as Doc).dBSource.LoginUser.WorkingPath != null)
                        {
                            sUserTempPath = (oProjectOrDoc as Doc).dBSource.LoginUser.WorkingPath;
                        }
                    }
                    //小黎 2012-9-7 按照新的标准,把下载的图片都放到c:\temp\CDMSBMP
                    sUserTempPath = "C:\\temp\\CDMSBMP\\";

                    
                    if (sExprValue.Length > 0 && sUserTempPath.Length > 0)
                    {
                        //图片的全路径(包括文件名),sTempPath表示用户的临时工作目录，sExprValue表示图片的名字
                        sPicFullPath = sUserTempPath + sExprValue;

                        try
                        {
                            //小钱 2018-8-24 查看路径是否存在,不存在就创建路径
                            if (!Directory.Exists(sUserTempPath)) Directory.CreateDirectory(sUserTempPath);

                            //把文件下载到用户的临时工作路径
                            DownLoadFile(oProjectOrDoc, (IsFullPath(sExprValue) ? "" : "BMP\\") + sExprValue, sPicFullPath);
                        }
                        catch (Exception ex)
                        {
                            //添加错误信息到错误列表中
                            if (_errorList != null)
                            {
                                {
                                    _errorList.Add(ex.Message);
                                }
                            }
                            //2010-04-30
                            
                            AVEVA.CDMS.Server.CDMS.Write(ex.ToString()); 
                            //遇上错误时是否关闭正在处理的程序,默认不关闭,false
                            Release(_closeErrorApp);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                //2010-04-30
                _errorList.Add(e.ToString());
                AVEVA.CDMS.Server.CDMS.Write(e.ToString()); 
            }
            return sPicFullPath;
        }

        #endregion

        #region 插入多值域 ----暂不处理多值域

        /// <summary>
        /// 插入多值域
        /// </summary>
        /// <param name="oProjectOrDoc">Project或Doc对象</param>
        /// <returns></returns>
        protected bool InsertCollection(object oProjectOrDoc)
        {
            try
            {
                InsertCollection(oProjectOrDoc, null);
                return true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK);
                //2010-04-30
                _errorList.Add(ex.ToString());
                AVEVA.CDMS.Server.CDMS.Write(ex.ToString()); 
            }
            return false;
        }


        /// <summary>
        /// 插入多值域
        /// </summary>
        /// <param name="oProjectOrDoc">Project或Doc对象</param>
        /// <param name="htUserKeyWord">用户关键字哈希表</param>
        /// <returns></returns>
        protected bool InsertCollection(object oProjectOrDoc, Hashtable htUserKeyWord)
        {
            try
            {
                string sKeyWord;
                string[] sValue = new string[] { };
                Word.Selection selection;
                object oMoveUnit = Word.WdUnits.wdLine;
                object oCount = 1;
                int recCount;
                Project project = null;
                Doc doc = null;

                if (oProjectOrDoc == null) return false;

                if (oProjectOrDoc is Project)
                {
                    project = oProjectOrDoc as Project;
                }
                else if (oProjectOrDoc is Doc)
                {
                    doc = oProjectOrDoc as Doc;
                }

                for (int i = 1; i <= m_wdDoc.Fields.Count; i++)
                {
                    if (m_wdDoc.Fields[i].Type == Microsoft.Office.Interop.Word.WdFieldType.wdFieldAddin)
                    {
                        sKeyWord = m_wdDoc.Fields[i].Data;

                        if (sKeyWord.Substring(sKeyWord.Length - 1) == "F")
                        {

                            if (oProjectOrDoc is Project)
                            {
                                if (project == null) return false;

                                //获取值
                                sValue = project.ExcuteDefnExpression(sKeyWord, htUserKeyWord);
                            }
                            else if (oProjectOrDoc is Doc)
                            {
                                if (doc == null) return false;

                                //获取值
                                sValue = doc.ExcuteDefnExpression(sKeyWord, htUserKeyWord);
                            }

                            //选择有域的单元格
                            m_wdDoc.Fields[i].Select();

                            //插入点
                            selection = m_wdApp.Selection;

                            selection.Cells[1].Select();

                            //清除原值
                            selection.Delete(ref m_oMissing, ref m_oMissing);

                            //还原原域并更新值
                            InsertField(sKeyWord + "F");

                            m_wdDoc.Fields[i].Result.Text = sValue[0];

                            //光标下移
                            selection.Select();
                            selection.MoveDown(ref oMoveUnit, ref oCount, ref m_oMissing);
                            

                            recCount = sValue.GetLength(0) - 1;

                            for (int j = 1; j < recCount; j++)
                            {
                                if (m_bIsInsertRow == false)
                                {
                                    InsertRow(selection, recCount);
                                }
                                selection.Cells[1].Select();
                                selection.Delete(ref m_oMissing, ref m_oMissing);
                                selection.TypeText(sValue[j]);
                                selection.MoveDown(ref oMoveUnit, ref oCount, ref m_oMissing);
                            }
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK);
                //2010-04-30
                _errorList.Add(ex.ToString());
                AVEVA.CDMS.Server.CDMS.Write(ex.ToString()); 
                return true;
            }
        }


        /// <summary>
        /// 插入校审意见到Word中
        /// 校审意见域中的表达式为： WORKFLOWAUDITS:行数:列数
        /// </summary>
        /// <param name="dataList"></param>
        protected void InsertAuditCollection(String userKeyWord , List<String> dataList)
        {
            try
            {
                if (dataList == null || dataList.Count <= 0)
                    return; 

                string sKeyWord;
                string[] sValue = new string[] { };
                Word.Selection selection;
                object oMoveUnitLine = Word.WdUnits.wdLine;
                object oMoveUnitCol = Word.WdUnits.wdCell; 
                object oCount = 1;
                int recCount;



                for (int i = 1; i <= m_wdDoc.Fields.Count; i++)
                {
                    m_wdDoc.Fields[i].Update(); 

                    if (m_wdDoc.Fields[i].Type == Microsoft.Office.Interop.Word.WdFieldType.wdFieldAddin)
                    {
                        sKeyWord = m_wdDoc.Fields[i].Data;

                        if (sKeyWord.ToUpper().StartsWith(userKeyWord.ToUpper()))
                        {
                            //获取表达式中指定的列数
                            int rowCount = 0;
                            int colCount = 0;
                            char[] split = { ':' };
                            String[] sList = sKeyWord.Split(split);

                            if (sList == null || sList.Length < 3)
                                continue;

                            
                            colCount = int.Parse(sList[2]);
                            //小钱 2018-12-20
                            //rowCount = int.Parse(sList[1])*colCount;
                            rowCount = dataList.Count / colCount;

                            if (colCount <= 0)
                                continue;

                            Microsoft.Office.Interop.Word.Field firstField = m_wdDoc.Fields[i];

                            //选择有域的单元格
                            //m_wdDoc.Fields[i].Select();
                            firstField.Select();

                            int dataCount = dataList.Count;

                            for (int n = 0; n <= dataCount - colCount; )
                            {
                                //插入点
                                //2018.5.17 小钱 如果插入的位置在表格中部而不是尾部，表格的下一行跟第一行格式不一致，从第二行才插入，则插入的是第二行的格式，导致格式不一致
                                //改为从第一行插入，但是最后面会出现一行空白行
                                //if (n >= rowCount && rowCount > 0) { System.Object __r = 1; m_wdApp.Selection.InsertRows(ref __r); }
                                if (n + 1 < rowCount && rowCount > 0)
                                {
                                    System.Object __r = 1; m_wdApp.Selection.InsertRows(ref __r);
                                }

                                selection = m_wdApp.Selection;

                                selection.Cells[1].Select();
                                

                                //清除原值
                                selection.Delete(ref m_oMissing, ref m_oMissing);
                                if (n == 0)
                                {
                                    //还原原域并更新值
                                    InsertField(sKeyWord);
                                }

                                selection.TypeText(dataList[n]);

                                //m_wdDoc.Fields[i].Result.Text = sValue[0];
                                //firstField.Result.Text = dataList[n];

                                //光标左移
                                selection.Select();
                                selection.MoveRight(ref oMoveUnitCol, ref oCount, ref m_oMissing);

                                for (int j = 1; j < colCount; j++)
                                {
                                    //selection.MoveRight(ref oMoveUnitCol, ref oCount, ref m_oMissing);
                                    selection.Cells[1].Select();
                                    selection.Delete(ref m_oMissing, ref m_oMissing);
                                    selection.TypeText(dataList[n + j]);

                                    //不是最后一行,最后一列,才移动
                                    //if(n < dataCount-colCount && j <colCount -1)
                                    selection.MoveRight(ref oMoveUnitCol, ref oCount, ref m_oMissing);
                                }

                                //selection.MoveDown(ref oMoveUnitLine, ref oCount, ref m_oMissing);

                                n = n + colCount;


                            }


                            return;
                        }
                        else if (sKeyWord.ToUpper() == "PAGECOUNT" || sKeyWord.ToUpper() == "PAGEINDEX")
                        {
                            //m_wdDoc.Fields[i].Select();
                            //selection = m_wdApp.Selection;
                            //selection.Cells[1].Select();
                            //selection.Delete(ref m_oMissing, ref m_oMissing);
                       
                            ////还原原域并更新值
                            //InsertField(sKeyWord);

                            

                            //int pagesCount = m_wdDoc.ComputeStatistics(Microsoft.Office.Interop.Word.WdStatistic.wdStatisticPages, ref m_oMissing);
                            //int pageIndex = 1;
                            //if (sKeyWord.ToUpper() == "PAGECOUNT")
                            //    selection.TypeText(pagesCount.ToString());
                            //else if (sKeyWord.ToUpper() == "PAGEINDEX")
                            //    selection.TypeText(pageIndex.ToString());


                        }


                    }
                }
                return;
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message + "\r\n"+ e.StackTrace, "错误提示", MessageBoxButtons.OK); 
                //2010-04-30
                _errorList.Add(e.ToString());
                AVEVA.CDMS.Server.CDMS.Write(e.ToString()); 
            }
        }


        /// <summary>
        /// 插入校审意见到Word中
        /// 校审意见域中的表达式为： WORKFLOWAUDITS:行数:列数
        /// </summary>
        /// <param name="htAuditDataList">[用户关键字]-[校审意见数据列表]</param>
        protected void InsertAuditCollection(Hashtable htAuditDataList)
        {
            try
            {
                if (htAuditDataList == null || htAuditDataList.Count <= 0)
                    return;


                foreach (DictionaryEntry de in htAuditDataList)
                {
                    String userKeyWord = "";
                    List<String> auditDataList = null; 
                    if (de.Key is String)
                    {
                        userKeyWord = (String)de.Key; 
                    }
                    if (de.Value is List<String>)
                    {
                        auditDataList = de.Value as List<String>; 
                    }

                    if(auditDataList!= null && auditDataList.Count >0)
                        InsertAuditCollection(userKeyWord, auditDataList); 
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message, "错误提示", MessageBoxButtons.OK);
                //2010-04-30
                _errorList.Add(e.ToString());
                AVEVA.CDMS.Server.CDMS.Write(e.ToString()); 
            }
        }
  

        /// <summary>
        /// 插入行
        /// </summary>
        /// <param name="selection">选中的区域</param>
        /// <param name="iCount"></param>
        /// <returns></returns>
        protected bool InsertRow(Word.Selection selection, int iCount)
        {
            try
            {
                //插入的行数
                object oInsertRowCount;

                //移动的单位
                object oMoveUnit = Word.WdUnits.wdLine;

                Word.Cell currCell;

                currCell = selection.Cells[1];
                oInsertRowCount = iCount - selection.Tables[1].Rows.Count + 1;

                if (int.Parse(oInsertRowCount.ToString()) > 0)
                {
                    selection.InsertRows(ref oInsertRowCount);
                    currCell.Select();
                    selection.MoveUp(ref oMoveUnit, ref oInsertRowCount, ref m_oMissing);

                    m_bIsInsertRow = true;

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK);
                //2010-04-30
                _errorList.Add(ex.ToString());
                AVEVA.CDMS.Server.CDMS.Write(ex.ToString()); 
                return false;
            }
        }

        #endregion


        #region 打开Excel

        /// <summary>
        /// 打开Excel
        /// </summary>
        /// <param name="sFileName">Excel文件</param>
        /// <returns>无返回值</returns>
        public void OpenExcel(string sFileName)
        {
            OpenExcelNew(sFileName);
        }


        /// <summary>
        /// 打开Excel
        /// </summary>
        /// <param name="sFileName">Excel文件</param>
        /// <returns>是否成功</returns>
        public bool OpenExcelNew(string sFileName)
        {
            try
            {
                try
                {
                    //获取当前正在运行的Excel
                    //m_exApp = Interaction.GetObject(null, "excel.application") as Excel.Application;
                    //m_exApp = Marshal.GetActiveObject("excel.application") as Excel.Application;
                    m_exApp = new Microsoft.Office.Interop.Excel.ApplicationClass(); 
                }
                catch
                {
                    m_exApp = null;
                }

                if (m_exApp == null)
                {
                    //m_exApp = Interaction.CreateObject("excel.application", "") as Excel.Application;

                    Type exType = Type.GetTypeFromProgID("excel.application");
                    m_exApp = Activator.CreateInstance(exType, true) as Excel.Application; 

                    //表示当前没有另外的Excel运行
                    m_bExcelIsRunning = false;
                }
                else
                {
                    //表示当前有Excel正在运行
                    //m_bExcelIsRunning = true;
                }

                if (m_exApp != null)
                {
                    m_exWorkBook = m_exApp.Workbooks.Open(sFileName,
                                                        m_oMissing,
                                                        m_oMissing,
                                                        m_oMissing,
                                                        m_oMissing,
                                                        m_oMissing,
                                                        m_oMissing,
                                                        m_oMissing,
                                                        m_oMissing,
                                                        m_oMissing,
                                                        m_oMissing,
                                                        m_oMissing,
                                                        m_oMissing,
                                                        m_oMissing,
                                                        m_oMissing
                                                    );

                    //是否显示应用程序
                    if (!m_bExcelIsRunning)
                    {
                        m_exApp.Visible = _visibleApp;
                        m_exApp.ScreenUpdating = _visibleApp;
                        m_exApp.DisplayAlerts = _visibleApp; 
                    }

                    m_exWorksheet = m_exWorkBook.Worksheets[1] as Worksheet;

                    return true;
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK);
                //2010-04-30
                _errorList.Add(ex.ToString());
                AVEVA.CDMS.Server.CDMS.Write(ex.ToString()); 

                //出现异常则释放资源
                Release(true);
            }

            return false;
        }

        #endregion

        #region 判断excel是否已经打开

        /// <summary>
        /// 判断excel是否已经打开
        /// </summary>
        /// <param name="exAppl">当前的excel</param>
        /// <param name="sFileName">excel文件名</param>
        /// <returns></returns>
        protected bool ExcelIsRunning(Excel.Application exAppl, string sFileName)
        {
            if (exAppl == null) return false;

            foreach (Excel.Workbook wb in exAppl.Workbooks)
            {
                if (exAppl.ActiveWorkbook.FullName == wb.FullName)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region 保存Excel

        /// <summary>
        /// 保存Excel
        /// </summary>
        public void SaveExcel()
        {
            try
            {
                if (m_exWorkBook == null) return;

                m_exWorkBook.Save();

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK);

                //出现异常则释放资源
                //2010-04-30
                _errorList.Add(ex.ToString());
                AVEVA.CDMS.Server.CDMS.Write(ex.ToString()); 
                Release(true);
            }
            finally
            {
                //释放资源
                Release(_closeApp);
            }
        }

        #endregion

        #region 查找Excel 中的表达式并为表达式赋值

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oProjectOrDoc"></param>
        /// <param name="sFileName"></param>
        /// <param name="bApplVisible">是否显示应用程序</param>
        protected void FillExcelData(object oProjectOrDoc, string sFileName)
        {
            try
            {
                FillExcelData(oProjectOrDoc, sFileName, 0, 0, "", null);

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK);
                //2010-04-30
                _errorList.Add(ex.ToString());
                AVEVA.CDMS.Server.CDMS.Write(ex.ToString()); 
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="oProjectOrDoc"></param>
        /// <param name="sFileName"></param>
        /// <param name="bApplVisible">是否显示应用程序</param>
        /// <param name="lMinRow">最小行</param>
        /// <param name="lMaxRow">最大行</param>
        /// <param name="sRowValue">行的值 B,C,D等</param>
        /// <param name="arrValueList">要写入的值的集合</param>
        public void FillExcelData(object oProjectOrDoc, string sFileName, int lMinRow, int lMaxRow, string sRowValue, ArrayList arrValueList)
        {
            try
            {
                //定义excel中sheet名的变量
                string sSheetName;

                //要写的页数
                int lSheetNumbers = 1;

                //连续为空的数据量
                int lNoDataNumbers = 4;

                //空格的个数
                int lSpaceNumbers = 0;

                //记录行中是否为空
                int lLine = 0;

                Excel.Worksheet worksheet;

                Project project = null;

                Doc doc = null;

                ArrayList arrCol = new ArrayList();

                if (m_exWorkBook == null || oProjectOrDoc == null) return;

                //检查要写多少页
                if (arrValueList != null && arrValueList.Count > 0 && lMinRow > 0 && lMaxRow > 0 && sRowValue.Length > 0)
                {

                    arrCol = DivideStrByCommaToDataCol(sRowValue);
                    lSheetNumbers = arrValueList.Count / arrCol.Count / (lMaxRow - lMinRow + 1);

                    //复制表
                    for (int i = 1; i <= lSheetNumbers; i++)
                    {
                        m_exWorkBook.Sheets.Copy(m_oMissing, m_exWorkBook.Sheets[1]);
                    }

                    //修改名称
                    string name;
                    name = ((Excel.Worksheet)m_exWorkBook.Sheets[1]).Name;
                    lSheetNumbers = lSheetNumbers + 1;

                    for (int n = 1; n <= lSheetNumbers; n++)
                    {
                        ((Excel.Worksheet)m_exWorkBook.Sheets[n]).Name = name + "(" + n + "-" + lSheetNumbers + ")";
                    }
                }
                else
                {
                    lSheetNumbers = 1;
                }

                if (oProjectOrDoc is Project)
                {
                    project = oProjectOrDoc as Project;
                }
                else if (oProjectOrDoc is Doc)
                {
                    doc = oProjectOrDoc as Doc;
                }


                worksheet = (Excel.Worksheet)m_exWorkBook.Sheets[1];

                //将第一个Sheet名赋给sSheetName
                sSheetName = ((Excel.Worksheet)m_exWorkBook.Sheets[1]).Name;


                //逐个Sheet填写
                for (int j = 1; j <= lSheetNumbers; j++)
                {
                    //设置heet
                    worksheet = (Excel.Worksheet)m_exWorkBook.Sheets[j];


                    //替换Cell 中的 $(AAAA)

                    string temp;
                    for (int k = 1; k <= 25; k++)
                    {
                        for (int m = 1; m <= 100; m++)
                        {
                            temp = ((Excel.Range)worksheet.Cells[m, k]).Value2 as string;

                            if (temp == null) continue;

                            if (temp.Length > 0)
                            {
                                //处理多次填写问题
                                if (Strings.InStr(1, temp, "$(*", CompareMethod.Text) > 0)
                                {
                                    //若定义关键字为: $(*AAAA), 则表示第二次填写， 若定义$(**AAAA) 表示第三次填写
                                    temp = "$(" + Strings.Mid(temp, 4);
                                    ((Excel.Range)worksheet.Cells[m, k]).Value2 = temp;
                                }
                                else if (Strings.InStr(1, temp, "$(", CompareMethod.Text) > 0 || Strings.InStr(1, temp, ("@@"), CompareMethod.Text) > 0 || Strings.InStr(1, temp, (":"), CompareMethod.Text) > 0 || Strings.InStr(1, temp, ("$["), CompareMethod.Text) > 0)
                                {
                                    if (oProjectOrDoc is Project && project != null)
                                    {
                                        ((Excel.Range)worksheet.Cells[m, k]).Value2 = project.ExcuteDefnExpression(temp)[0];
                                    }
                                    else if (oProjectOrDoc is Doc && doc != null)
                                    {
                                        ((Excel.Range)worksheet.Cells[m, k]).Value2 = doc.ExcuteDefnExpression(temp)[0];
                                    }
                                }
                            }

                            if (temp.Length == 0)
                            {
                                lSpaceNumbers = lSpaceNumbers + 1;
                            }
                            else
                            {
                                lSpaceNumbers = 0;
                            }

                            if (lNoDataNumbers == lSpaceNumbers)
                            {
                                if (m > lNoDataNumbers)
                                {
                                    lSpaceNumbers = 0;
                                    break;
                                }
                            }
                        }

                        //行中是否为空
                        if (lSpaceNumbers > 0)
                        {
                            lLine = lLine + 1;
                        }
                        else
                        {
                            lLine = 0;
                        }

                        //超过4个空格，则不处理
                        if (lLine == lNoDataNumbers)
                        {
                            break;
                        }
                    }

                    //替换图文框中的$(AAAA)
                    Excel.Shape shape = null;
                    Excel.Drawing drawing;
                    foreach (Excel.Shape s in worksheet.Shapes)
                    {
                        if ((shape.AlternativeText.Length > 0) && shape.AlternativeText.IndexOf("$(") > 0)
                        {
                            drawing = (Excel.Drawing)shape.DrawingObject;

                            if (oProjectOrDoc is Project && project != null)
                            {
                                drawing.Caption = project.ExcuteDefnExpression(drawing.Caption)[0];
                            }
                            else if (oProjectOrDoc is Doc && doc != null)
                            {
                                drawing.Caption = doc.ExcuteDefnExpression(drawing.Caption)[0];
                            }
                        }
                    }


                    //写入用户数据 
                    if (arrValueList != null && arrValueList.Count > 0)
                    {
                        for (int p = lMinRow; p <= lMaxRow; p++)
                        {
                            j = 1;
                            foreach (string col in arrCol)
                            {
                                //赋值
                                lLine = (j - 1) * (lMaxRow - lMinRow) * arrValueList.Count + (p - lMinRow) * arrCol.Count + j;
                                if (lLine > arrValueList.Count)
                                {
                                    break;
                                }
                                else
                                {
                                    worksheet.get_Range(col + p, m_oMissing).Value2 = arrValueList[lLine - 1];
                                }
                                j = j + 1;
                            }
                        }
                    }
                }


                if (lMinRow == 8)
                {
                    m_exApp.Rows.AutoFit();
                    m_exApp.Columns.AutoFit();
                }

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK);
                //2010-04-30
                _errorList.Add(ex.ToString());
                AVEVA.CDMS.Server.CDMS.Write(ex.ToString()); 
                //出现异常则释放资源
                Release(true);
            }
        }




        #endregion

        #region 将以","分开的数值列表分解为一个数

        /// <summary>
        /// 将以","分开的数值列表分解为一个数
        /// </summary>
        /// <param name="sExpression"></param>
        /// <param name="sDivideMark"></param>
        /// <returns></returns>
        protected ArrayList DivideStrByCommaToDataCol(string sExpression, params string[] sDivideMark)
        {
            //要返回的列表
            ArrayList arrReturnList = new ArrayList();

            string sDivideComma = ",";

            if (sDivideMark.Length > 0)
            {
                sDivideComma = sDivideMark[0];
            }

            //分隔符的位置
            int iDivideMarkPosition;

            try
            {
                //判断是否为空
                if (sExpression == null || sExpression.Length <= 0) return null;

                //检查表达式中是否有$[],$(,若出现了"#",则使用"#"做分隔符
                if (Strings.InStr(1, sExpression, "$[", CompareMethod.Text) > 0 && Strings.InStr(1, sExpression, "#", CompareMethod.Text) > 0)
                {
                    sDivideComma = "#";
                }

                //用英文逗号替换中文逗号
                sExpression = sExpression.Replace("，", ",");
                sExpression = sExpression.Replace("、", ",");

                if (Strings.Mid(sExpression, Strings.Len(sExpression), 1) == sDivideComma)
                {
                    sExpression = sExpression + " ";
                }

                do
                {
                    iDivideMarkPosition = Strings.InStr(1, sExpression, sDivideComma, CompareMethod.Text);

                    if (sDivideComma == ",")
                    {
                        if (iDivideMarkPosition == 0)
                        {
                            iDivideMarkPosition = Strings.InStr(1, sExpression, "，", CompareMethod.Text);
                            if (iDivideMarkPosition > 0)
                            {
                                sExpression = Strings.Mid(sExpression, 1, iDivideMarkPosition - 1) + " " + Strings.Mid(sExpression, iDivideMarkPosition + 1);
                            }
                        }

                        if (iDivideMarkPosition == 0)
                        {
                            iDivideMarkPosition = Strings.InStr(1, sExpression, "、", CompareMethod.Text);
                            if (iDivideMarkPosition > 0)
                            {
                                sExpression = Strings.Mid(sExpression, 1, iDivideMarkPosition - 1) + " " + Strings.Mid(sExpression, iDivideMarkPosition + 1);
                            }
                        }

                        if (iDivideMarkPosition == 0)
                        {
                            iDivideMarkPosition = Strings.InStr(1, sExpression, ":", CompareMethod.Text);
                        }
                    }

                    if (iDivideMarkPosition > 0)
                    {
                        arrReturnList.Add(Strings.Mid(sExpression, 1, iDivideMarkPosition - 1));

                        sExpression = Strings.Mid(sExpression, iDivideMarkPosition + 1);
                    }
                    else
                    {
                        arrReturnList.Add(Strings.Mid(sExpression, iDivideMarkPosition + 1));
                        sExpression = "";
                    }
                }
                while (Strings.Len(sExpression) > 0);

                return arrReturnList;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK);
                //2010-04-30
                _errorList.Add(ex.ToString());
                AVEVA.CDMS.Server.CDMS.Write(ex.ToString()); 
                return null;
            }
        }

        #endregion

        #region 下载文件

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="project">Project</param>
        /// <param name="sServerFileName">服务器上的文件名称</param>
        /// <param name="sLocalFileName">下载到本机的文件名称</param>
        /// <returns></returns>
        protected bool DownLoadFile(object oProjectOrDoc, string sServerFileName, string sLocalFileName)
        {
            //FTPFactory ftp = null;
            Project project = null;
            if (oProjectOrDoc == null || sServerFileName.Trim().Length <= 0) return false;
            try
            {
                if (oProjectOrDoc is Project)
                {
                    project = oProjectOrDoc as Project;
                }
                else if (oProjectOrDoc is Doc)
                {
                    project = (oProjectOrDoc as Doc).Project;
                }


                //TIM 2009-08-25
                //ftp = new FTPFactory(project.dBSource.dBManager.GetServerName(), "AVEVA", "AVEVA_CDMS");
                //    ftp = new FTPFactory(project.dBSource.RemoteDBSource.dBManager.GetServerName(), "AVEVA", "AVEVA_CDMS");

                //获取文件完整路径
                sServerFileName = WebApi.DBSourceController.Server_MapPath + sServerFileName;

                //复制文件
                //System.IO.File.Copy(sServerFileName, sLocalFileName, false);
                System.IO.File.Copy(sServerFileName, sLocalFileName, true);

                //ftp = new FTPFactory(project.Storage);
                //ftp.download(sServerFileName, sLocalFileName, false);
                //ftp.close();

                return true;
            }
            catch(Exception ex)
            {
                //2010-04-30
                _errorList.Add(ex.ToString());
                AVEVA.CDMS.Server.CDMS.Write(ex.ToString()); 

                //if (ftp != null) { ftp.close(); }
                return false;
            }
        }

        #endregion

        #region 下载文件后并填写文档

        /// <summary>
        /// 检查一个文件名称中包含全路径
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>是否包含</returns>
        protected bool IsFullPath(string path)
        {
            if (path.Contains(":\\"))
                return true;
            else if (path.Contains(":/"))
                return true;

            return false;
        }


        /// <summary>
        /// 从服务器上ISO模板文件下载模板，填写值
        /// </summary>
        /// <param name="oProjectOrDoc">Project或Doc</param>
        /// <param name="sTempDefnFile">服务器上模板文件名称</param>
        /// <param name="sOutFileName">全路径</param>
        /// <param name="bVisibleAppl">是否显示Wrod或Excel</param>
        /// <param name="htUserKeyWord">存放用户关键字的哈希表</param>
        /// <returns></returns>
        public bool WriteDataToDocument(object oProjectOrDoc, string sTempDefnFile, string sOutFileName, Hashtable htUserKeyWord)
        {

            try
            {

                //下载文件
                if (oProjectOrDoc is Project)
                {
                    DownLoadFile(oProjectOrDoc as Project, (IsFullPath(sTempDefnFile) ? "" : "ISO\\") + sTempDefnFile, sOutFileName);
                }
                else if (oProjectOrDoc is Doc)
                {
                    DownLoadFile((oProjectOrDoc as Doc).Project, (IsFullPath(sTempDefnFile) ? "" : "ISO\\") + sTempDefnFile, sOutFileName);
                }

                //填写文档
                WriteDataToDocument(oProjectOrDoc, sOutFileName, htUserKeyWord);

                return true;
            }
            catch (Exception ex)
            {
                //添加错误信息到错误列表中
                if (_errorList != null)
                {
                    {
                        _errorList.Add(ex.Message);
                    }
                }
                //2010-04-30
      
                AVEVA.CDMS.Server.CDMS.Write(ex.ToString()); 
                return false;
            }
        }

        #endregion

        #region 写入文档

        /// <summary>
        ///  填写文档的域值
        /// </summary>
        /// <param name="oProjectOrDoc">Project或Doce</param>
        /// <param name="sTempDefnFile">模板文件名</param>
        /// <param name="bVisibleAppl">是否显示word </param>
        /// <returns></returns>
        public bool WriteDataToDocument(object oProjectOrDoc, string sTempDefnFile)
        {
            try
            {
                return WriteDataToDocument(oProjectOrDoc, sTempDefnFile, null);

            }
            catch (Exception ex)
            {
                //添加错误信息到错误列表中
                if (_errorList != null)
                {
                    {
                        _errorList.Add(ex.Message);
                    }
                }
                //2010-04-30
     
                AVEVA.CDMS.Server.CDMS.Write(ex.ToString()); 
                return false;
            }
        }


#region 更新文件数据专门的调用
        public bool UpdateFileData(object oProjectOrDoc, string sFileName, Hashtable htUserKeyWord)
        {
            try
            {
                IsUpdateFileData = true;
                //判断文件名是否存在
                if (File.Exists(sFileName))
                {

                    //先判断要打开的文件是什么类型
                    if (sFileName.ToUpper().IndexOf(".DOC") > 0)
                    {
                        //先判断文件是否已经打开
                        if (!WordIsRunning(m_wdApp, sFileName))
                        {
                            //根据文件类型打开文件
                            if (!OpenWordNew(sFileName)) return false;

                            //插入单值域的值
                            FillWdFieldValueUpdateData(oProjectOrDoc, htUserKeyWord);


                            //插入多值域的值
                            InsertCollection(oProjectOrDoc, htUserKeyWord);



                            //完成操作后保存文件
                            SaveWord(sFileName as object);
                        }
                        else
                        {
                            _errorList.Add("[" + sFileName + "已经打开!]");
                        }

                    }
                    else if (sFileName.ToUpper().IndexOf(".XLS") > 0)
                    {
                        //在执行更新数据之前要先把excel表格的关键字保存起来,替换已经写入过的数据










                        //先判断文件是否已经打开
                        if (!ExcelIsRunning(m_exApp, sFileName))
                        {

                            //打开文件
                            if (!OpenExcelNew(sFileName)) return false;


                            //扫描excel中的表达式并为其赋值
                            //ParseExcelField(oProjectOrDoc, htUserKeyWord,new Hashtable());
                            ParseExcelFieldEx(oProjectOrDoc, htUserKeyWord, new Hashtable());

                            //关闭文档
                            SaveExcel();
                        }
                        else
                        {
                            _errorList.Add("[" + sFileName + "已经打开!]");
                        }
                    }

                    return true;
                }
                else
                {
                    _errorList.Add("[" + sFileName + "]不存在!");
                }

                return false;
            }
            catch (Exception ex)
            {
                //添加错误信息到错误列表中
                _errorList.Add(ex.Message);

                //2010-04-30
                AVEVA.CDMS.Server.CDMS.Write(ex.ToString());

                return false;
            }


        }

        ////小钱 增加更新Word数据
        //public static bool UpdateWordDocumentData(Word.Application WordApp, Hashtable htUserKeyWord) {
        //    //FillWdFieldValueUpdateData(null,null);
        //    return false;
        //}

        public bool FillWdFieldValueUpdateData(object oProjectOrDoc, Hashtable htUserKeyWord)
        {
            try
            {
                picFieldList.Clear();
                if (oProjectOrDoc == null) return false;

                //从选择关键字窗口中获取到的表达式。
                string sExpression;

                //解析表达式后的值,调用ExcuteDefnExpression函数后返回的值, 
                //ExcuteDefnExpression : project.ExcuteDefnExpression(sKeyWord)，返回的是数组,获取第一个元素。
                string sExpressValue;

                //用来保存图片域(Word.WdFieldType.wdFieldIncludePicture)
                Hashtable htPicFields = new Hashtable();

                //用来保存自定义域(Word.WdFieldType.wdFieldAddin类型的)
                Hashtable htAddinFields = new Hashtable();


                if (m_wdDoc.Fields != null && m_wdDoc.Fields.Count > 0)
                {
                    //*******************************************
                    //查找所有域，用实际的值进行替换（图片和文字）
                    for (int i = 1; i <= m_wdDoc.Fields.Count; i++)
                    {
                        m_wdDoc.Fields[i].Update();

                        //**********************************************
                        #region 处理域类型为WdFieldType.wdFieldAddin的域

                        if (m_wdDoc.Fields[i].Type == Word.WdFieldType.wdFieldAddin)
                        {
                            sExpression = m_wdDoc.Fields[i].Data;

                            if (String.IsNullOrEmpty(sExpression))
                                continue;

                            //加上"F"表示多值域
                            if (sExpression.Substring(sExpression.Length - 1) != "F")
                            {
                                //更新数据时要开锁
                                if (m_wdDoc.Fields[i].Locked == true )
                                {
                                    m_wdDoc.Fields[i].Locked = false;
                                }

                                //若包含"Pic:",表示该域是由图片域转换至自定义域的
                                if (sExpression.Contains("Pic:"))
                                {
                                    //*******************************************************************************
                                    //保存到哈希表htAddinFields，用来保存自定义域(Word.WdFieldType.wdFieldAddin类型的)
                                    //哈希表key,哈希表htAddinFields,htAddinFields的value为Field
                                    string sFieldKey = m_wdDoc.Fields[i].Index.ToString() + m_wdDoc.Fields[i].Code.Text;
                                    if (!htAddinFields.ContainsKey(sFieldKey))
                                    {
                                        htAddinFields.Add(sFieldKey, m_wdDoc.Fields[i]);
                                    }
                                    continue;
                                }
                                #region 填写自定义域的值

                                //获取表达式的值
                                sExpressValue = GetExpressValue(oProjectOrDoc, htUserKeyWord, sExpression);


                                //2011.8.1 如果是文字，并且设置了最终状态，则写入空字符串
                                if (this.IsFinial && string.IsNullOrEmpty(sExpressValue))
                                {
                                    sExpressValue = " ";
                                }


                                //填写自定义域的值,填写完毕后为域加锁
                                FillAddInFiled(sExpressValue, m_wdDoc.Fields[i]);

                                #endregion
                            }
                        }
                        #endregion

                        //小钱 2018-8-5 由于没有要求修改的图片被覆盖，先忽略图片的更新
                        //**********************************************************************
                        #region 处理图片  $(CurrentUser_Code).JPG，用于取得用户上传的数字签名图片

                        else if (m_wdDoc.Fields[i].Type == Word.WdFieldType.wdFieldIncludePicture)
                        {

                            //更新文件数据时要开锁
                            if ( m_wdDoc.Fields[i].Locked == true )
                            {
                                m_wdDoc.Fields[i].Locked = false;
                            }
                            //图片域未加锁表示，图片未填写
                            FillPicField(oProjectOrDoc, htUserKeyWord, m_wdDoc.Fields[i], true);

                        }

                        #endregion
                    }


                   // 小钱 2018 - 8 - 5 由于没有要求修改的图片被覆盖，先忽略图片的更新
                    #region 把保存在htAddinFields的自定义域进行解析，把其解析为图片域,并填写图片域
                    foreach (DictionaryEntry de in htAddinFields)
                    {
                        //自定义域 (Word.WdFieldType.wdFieldAddin)
                        Word.Field addinField = de.Value as Word.Field;

                        //自定义域的域代码
                        string addinCodeText = addinField.Data;

                        //取自定义域中"Pic:"之后的域代码(如有域代码 Pic: INCLUDEPICTURE "C:\\k.jpg" \* MERGEFORMAT ，只需取
                        //INCLUDEPICTURE "C:\\k.jpg" \* MERGEFORMAT ,其中INCLUDEPICTURE "C:\\k.jpg" \* MERGEFORMAT表示图片域)
                        string sPicCodeText = addinCodeText.Split(new string[] { "Pic:" }, StringSplitOptions.RemoveEmptyEntries)[0];

                        //根据从自定义域中获取的图片域中的表达式域代码生成图片域
                        //取表达式 从类似" INCLUDEPICTURE $(USER_CODE).JPG \\* MERGEFORMAT "的域代码取表达式
                        int index = sPicCodeText.IndexOf("INCLUDEPICTURE") + 14;
                        string sExprInField = sPicCodeText.Substring(index, sPicCodeText.LastIndexOf("\\") - index);

                        addinField.Select();
                        addinField.Delete();
                        Word.Field picField = InsertPictureField(sExprInField);

                        //若不能正常填写图片域，则保存到哈希表
                        if (FillPicField(oProjectOrDoc, htUserKeyWord, picField) == false)
                        {
                            if (!htPicFields.ContainsKey(picField.Index.ToString() + picField.Code.Text))
                            {
                                //保存未处理的图片域
                                htPicFields.Add(picField.Index.ToString() + picField.Code.Text, picField);
                            }
                        }
                    }

                    #endregion
                }

                //小钱 2018-8-5 由于没有要求修改的图片被覆盖，先忽略图片的更新
                //*********************************
                //扫描Shpe对象中的域,并填写其域的值
                //FillShapeFieldUpdateData(oProjectOrDoc, htUserKeyWord);


                ////TODO: 需要删除的测试代码
                ////扫描页眉中的域 , 并填写其域的值
                //this.m_wdApp.ActiveWindow.ActivePane.View.SeekView =
                //      Microsoft.Office.Interop.Word.WdSeekView.wdSeekCurrentPageHeader;

                //this.m_wdApp.Selection.WholeStory();

                //if (this.m_wdApp.Selection.Fields != null && this.m_wdApp.Selection.Fields.Count > 0)
                //{
                //    htAddinFields.Clear();
                //    htPicFields.Clear();
                //    //*******************************************
                //    //查找所有域，用实际的值进行替换（图片和文字）
                //    for (int i = 1; i <= this.m_wdApp.Selection.Fields.Count; i++)
                //    {
                //        //**********************************************
                //        #region 处理域类型为WdFieldType.wdFieldAddin的域

                //        if (this.m_wdApp.Selection.Fields[i].Type == Word.WdFieldType.wdFieldAddin)
                //        {
                //            sExpression = this.m_wdApp.Selection.Fields[i].Data;

                //            if (String.IsNullOrEmpty(sExpression))
                //                continue;
                //            //加上"F"表示多值域
                //            if (sExpression.Substring(sExpression.Length - 1) != "F")
                //            {
                //                //更新文件数据时要开锁
                //                if ( this.m_wdApp.Selection.Fields[i].Locked == true )
                //                {
                //                    this.m_wdApp.Selection.Fields[i].Locked = false;
                //                }
                //                //若包含"Pic:",表示该域是由图片域转换至自定义域的
                //                if (sExpression.Contains("Pic:"))
                //                {
                //                    //*******************************************************************************
                //                    //保存到哈希表htAddinFields，用来保存自定义域(Word.WdFieldType.wdFieldAddin类型的)
                //                    //哈希表key,哈希表htAddinFields,htAddinFields的value为Field
                //                    string sFieldKey = this.m_wdApp.Selection.Fields[i].Index.ToString() + m_wdApp.Selection.Fields[i].Code.Text;
                //                    if (!htAddinFields.ContainsKey(sFieldKey))
                //                    {
                //                        htAddinFields.Add(sFieldKey, m_wdApp.Selection.Fields[i]);
                //                    }
                //                    continue;
                //                }
                //                #region 填写自定义域的值

                //                //获取表达式的值
                //                sExpressValue = GetExpressValue(oProjectOrDoc, htUserKeyWord, sExpression);
                //                //2011.8.1 如果是文字，并且设置了最终状态，则写入空字符串
                //                if (this.IsFinial && string.IsNullOrEmpty(sExpressValue))
                //                {
                //                    sExpressValue = " ";
                //                }

                //                //填写自定义域的值,填写完毕后为域加锁
                //                FillAddInFiled(sExpressValue, this.m_wdApp.Selection.Fields[i]);

                //                #endregion
                //            }
                //        }
                //        #endregion

                //        //**********************************************************************
                //        #region 处理图片  $(CurrentUser_Code).JPG，用于取得用户上传的数字签名图片

                //        else if (this.m_wdApp.Selection.Fields[i].Type == Word.WdFieldType.wdFieldIncludePicture)
                //        {
                //            //图片域未加锁表示，图片未填写
                //            //更新文件数据时要开锁
                //            if (this.m_wdApp.Selection.Fields[i].Locked == true)
                //            {
                //                this.m_wdApp.Selection.Fields[i].Locked = false;
                //            }

                //            if (FillPicField(oProjectOrDoc, htUserKeyWord, this.m_wdApp.Selection.Fields[i]) == false)
                //            {
                //                //保存未处理的图片域
                //                htPicFields.Add(this.m_wdApp.Selection.Fields[i].Index.ToString() + this.m_wdApp.Selection.Fields[i].Code.Text, this.m_wdApp.Selection.Fields[i]);
                //            }

                //        }
                //        #endregion



                //        this.m_wdApp.Selection.WholeStory();
                //    }

                //    #region 把保存在htAddinFields的自定义域进行解析，把其解析为图片域,并填写图片域
                //    foreach (DictionaryEntry de in htAddinFields)
                //    {
                //        //自定义域 (Word.WdFieldType.wdFieldAddin)
                //        Word.Field addinField = de.Value as Word.Field;

                //        //自定义域的域代码
                //        string addinCodeText = addinField.Data;

                //        //取自定义域中"Pic:"之后的域代码(如有域代码 Pic: INCLUDEPICTURE "C:\\k.jpg" \* MERGEFORMAT ，只需取
                //        //INCLUDEPICTURE "C:\\k.jpg" \* MERGEFORMAT ,其中INCLUDEPICTURE "C:\\k.jpg" \* MERGEFORMAT表示图片域)
                //        string sPicCodeText = addinCodeText.Split(new string[] { "Pic:" }, StringSplitOptions.RemoveEmptyEntries)[0];

                //        //根据从自定义域中获取的图片域中的表达式域代码生成图片域
                //        //取表达式 从类似" INCLUDEPICTURE $(USER_CODE).JPG \\* MERGEFORMAT "的域代码取表达式
                //        int index = sPicCodeText.IndexOf("INCLUDEPICTURE") + 14;
                //        string sExprInField = sPicCodeText.Substring(index, sPicCodeText.LastIndexOf("\\") - index);

                //        addinField.Select();
                //        addinField.Delete();
                //        Word.Field picField = InsertPictureField(sExprInField);

                //        //若不能正常填写图片域，则保存到哈希表
                //        if (FillPicField(oProjectOrDoc, htUserKeyWord, picField) == false)
                //        {
                //            if (!htPicFields.ContainsKey(picField.Index.ToString() + picField.Code.Text))
                //            {
                //                //保存未处理的图片域
                //                htPicFields.Add(picField.Index.ToString() + picField.Code.Text, picField);
                //            }
                //        }
                //    }

                //    #endregion
                //}


                ////HDF2011.4.19 页眉中表格里面的文字进行压缩处理 
                //if (this.m_wdApp.Selection.Tables != null && this.m_wdApp.Selection.Tables.Count > 0)
                //{
                //    foreach (Word.Table tb in this.m_wdApp.Selection.Tables)
                //    {
                //        for (int rw = 0; rw < tb.Rows.Count; rw++)
                //        {
                //            for (int cl = 0; cl < tb.Columns.Count; cl++)
                //            {
                //                try
                //                {

                //                    //需要压缩才进行处理
                //                    if (tb.Cell(rw, cl).FitText)
                //                    {
                //                        string text = tb.Cell(rw, cl).Range.Text;
                //                        if (text.Contains("\r\a")) text = text.Replace("\r\a", " ");
                //                        if (!string.IsNullOrEmpty(text.Trim()) && Asc(text.Substring(0, 1)) > 30)
                //                        {
                //                            tb.Cell(rw, cl).Range.Text = "";
                //                            tb.Cell(rw, cl).Range.Text = text;

                //                        }
                //                    }
                //                }
                //                catch (Exception ex)
                //                {

                //                }
                //            }

                //        }
                //    }
                //}



                ////扫描页脚中的域 , 并填写其域的值
                //this.m_wdApp.ActiveWindow.ActivePane.View.SeekView =
                //      Microsoft.Office.Interop.Word.WdSeekView.wdSeekCurrentPageFooter;

                //this.m_wdApp.Selection.WholeStory();




                //if (this.m_wdApp.Selection.Fields != null && this.m_wdApp.Selection.Fields.Count > 0)
                //{
                //    htAddinFields.Clear();
                //    htPicFields.Clear();
                //    //*******************************************
                //    //查找所有域，用实际的值进行替换（图片和文字）
                //    for (int i = 1; i <= this.m_wdApp.Selection.Fields.Count; i++)
                //    {
                //        //**********************************************
                //        #region 处理域类型为WdFieldType.wdFieldAddin的域

                //        if (this.m_wdApp.Selection.Fields[i].Type == Word.WdFieldType.wdFieldAddin)
                //        {
                //            sExpression = this.m_wdApp.Selection.Fields[i].Data;

                //            if (String.IsNullOrEmpty(sExpression))
                //                continue;
                //            //加上"F"表示多值域
                //            if (sExpression.Substring(sExpression.Length - 1) != "F")
                //            {
                //                //更新文件数据时要开锁
                //                if ( this.m_wdApp.Selection.Fields[i].Locked == true )
                //                {
                //                    this.m_wdApp.Selection.Fields[i].Locked = false;
                //                }

                //                //若包含"Pic:",表示该域是由图片域转换至自定义域的
                //                if (sExpression.Contains("Pic:"))
                //                {
                //                    //*******************************************************************************
                //                    //保存到哈希表htAddinFields，用来保存自定义域(Word.WdFieldType.wdFieldAddin类型的)
                //                    //哈希表key,哈希表htAddinFields,htAddinFields的value为Field
                //                    string sFieldKey = this.m_wdApp.Selection.Fields[i].Index.ToString() + m_wdApp.Selection.Fields[i].Code.Text;
                //                    if (!htAddinFields.ContainsKey(sFieldKey))
                //                    {
                //                        htAddinFields.Add(sFieldKey, m_wdApp.Selection.Fields[i]);
                //                    }
                //                    continue;
                //                }
                //                #region 填写自定义域的值

                //                //获取表达式的值
                //                sExpressValue = GetExpressValue(oProjectOrDoc, htUserKeyWord, sExpression);


                //                //2011.8.1 如果是文字，并且设置了最终状态，则写入空字符串
                //                if (this.IsFinial && string.IsNullOrEmpty(sExpressValue))
                //                {
                //                    sExpressValue = " ";
                //                }
                //                //填写自定义域的值,填写完毕后为域加锁
                //                FillAddInFiled(sExpressValue, this.m_wdApp.Selection.Fields[i]);

                //                #endregion
                //        }
                //        }
                //        #endregion

                //        //**********************************************************************
                //        #region 处理图片  $(CurrentUser_Code).JPG，用于取得用户上传的数字签名图片

                //        else if (this.m_wdApp.Selection.Fields[i].Type == Word.WdFieldType.wdFieldIncludePicture)
                //        {
                //            //更新文件时要开锁
                //            if (this.m_wdApp.Selection.Fields[i].Locked == true )
                //            {
                //                this.m_wdApp.Selection.Fields[i].Locked = false;
                //            }
                //            //图片域未加锁表示，图片未填写
                //            if (FillPicField(oProjectOrDoc, htUserKeyWord, this.m_wdApp.Selection.Fields[i]) == false)
                //            {
                //                //保存未处理的图片域
                //                htPicFields.Add(this.m_wdApp.Selection.Fields[i].Index.ToString() + this.m_wdApp.Selection.Fields[i].Code.Text, this.m_wdApp.Selection.Fields[i]);
                //            }
                //        }
                //        #endregion



                //        this.m_wdApp.Selection.WholeStory();
                //    }

                //    #region 把保存在htAddinFields的自定义域进行解析，把其解析为图片域,并填写图片域
                //    foreach (DictionaryEntry de in htAddinFields)
                //    {
                //        //自定义域 (Word.WdFieldType.wdFieldAddin)
                //        Word.Field addinField = de.Value as Word.Field;

                //        //自定义域的域代码
                //        string addinCodeText = addinField.Data;

                //        //取自定义域中"Pic:"之后的域代码(如有域代码 Pic: INCLUDEPICTURE "C:\\k.jpg" \* MERGEFORMAT ，只需取
                //        //INCLUDEPICTURE "C:\\k.jpg" \* MERGEFORMAT ,其中INCLUDEPICTURE "C:\\k.jpg" \* MERGEFORMAT表示图片域)
                //        string sPicCodeText = addinCodeText.Split(new string[] { "Pic:" }, StringSplitOptions.RemoveEmptyEntries)[0];

                //        //根据从自定义域中获取的图片域中的表达式域代码生成图片域
                //        //取表达式 从类似" INCLUDEPICTURE $(USER_CODE).JPG \\* MERGEFORMAT "的域代码取表达式
                //        int index = sPicCodeText.IndexOf("INCLUDEPICTURE") + 14;
                //        string sExprInField = sPicCodeText.Substring(index, sPicCodeText.LastIndexOf("\\") - index);

                //        addinField.Select();
                //        addinField.Delete();
                //        Word.Field picField = InsertPictureField(sExprInField);

                //        //若不能正常填写图片域，则保存到哈希表
                //        if (FillPicField(oProjectOrDoc, htUserKeyWord, picField) == false)
                //        {
                //            if (!htPicFields.ContainsKey(picField.Index.ToString() + picField.Code.Text))
                //            {
                //                //保存未处理的图片域
                //                htPicFields.Add(picField.Index.ToString() + picField.Code.Text, picField);
                //            }
                //        }
                //    }

                //    #endregion
                //}


                ////HDF2011.4.19 页脚中表格里面的文字进行压缩处理 
                //if (this.m_wdApp.Selection.Tables != null && this.m_wdApp.Selection.Tables.Count > 0)
                //{
                //    foreach (Word.Table tb in this.m_wdApp.Selection.Tables)
                //    {
                //        for (int rw = 0; rw < tb.Rows.Count; rw++)
                //        {
                //            for (int cl = 0; cl < tb.Columns.Count; cl++)
                //            {
                //                try
                //                {

                //                    //需要压缩的格才进行处理
                //                    if (tb.Cell(rw, cl).FitText)
                //                    {
                //                        string text = tb.Cell(rw, cl).Range.Text;
                //                        if (text.Contains("\r\a")) text = text.Replace("\r\a", " ");
                //                        if (!string.IsNullOrEmpty(text.Trim()) && Asc(text.Substring(0, 1)) > 30)
                //                        {
                //                            tb.Cell(rw, cl).Range.Text = "";
                //                            tb.Cell(rw, cl).Range.Text = text;

                //                        }
                //                    }
                //                }
                //                catch { }
                //            }

                //        }
                //    }
                //}

                ////2011.4.16 DOE要求在表格里面进行压缩，将表格里面的文字进行重新设置
                //if (m_wdDoc != null)
                //{
                //    foreach (Word.Table tb in m_wdDoc.Tables)
                //    {

                //        for (int rw = 0; rw < tb.Rows.Count; rw++)
                //        {
                //            for (int cl = 0; cl < tb.Columns.Count; cl++)
                //            {
                //                try
                //                {
                //                    //需要压缩的格才进行处理
                //                    if (tb.Cell(rw, cl).FitText)
                //                    {
                //                        string text = tb.Cell(rw, cl).Range.Text;
                //                        if (text.Contains("\r\a")) text = text.Replace("\r\a", " ");
                //                        if (!string.IsNullOrEmpty(text.Trim()) && Asc(text.Substring(0, 1)) > 30)
                //                        {
                //                            tb.Cell(rw, cl).Range.Text = "";
                //                            tb.Cell(rw, cl).Range.Text = text;
                //                        }
                //                    }
                //                }
                //                catch { }
                //            }

                //        }
                //    }
                //}
                return true;
            }
            catch (Exception ex)
            {
                //添加错误信息到错误列表中
                if (_errorList != null)
                {
                    {
                        _errorList.Add(ex.Message);
                    }
                }
                AVEVA.CDMS.Server.CDMS.Write(ex.Message);

                //出现异常则释放资源
                Release(true);
            }
            return false;
        }

        protected void FillShapeFieldUpdateData(object oProjectOrDoc, Hashtable htUserKeyWord)
        {
            //Shape对象中的文本层
            Word.TextFrame wdTextFram;

            Word.Range wdTextRange;

            //用来保存图片域(Word.WdFieldType.wdFieldIncludePicture)
            Hashtable htPicFields = new Hashtable();

            //表达式的值
            string[] sShapeValue = new string[] { };

            string sShapeKeyWord;
            try
            {
                //检查所有的Shape对象中的域,并给 WdFieldType.wdFieldAddin 的类型的域赋值
                foreach (Word.Shape shape in m_wdDoc.Shapes)
                {
                    if (shape.AutoShapeType == MsoAutoShapeType.msoShapeRectangle)
                    {


                        shape.Select(ref m_oMissing);

                        if (shape.TextFrame == null) continue;

                        if (shape.TextFrame.TextRange == null) continue;

                        wdTextFram = shape.TextFrame;

                        if (wdTextFram.TextRange != null)
                        {
                            wdTextRange = wdTextFram.TextRange;

                            foreach (Word.Field field in wdTextRange.Fields)
                            {
                                if (field.Type == Word.WdFieldType.wdFieldAddin)
                                {
                                    sShapeKeyWord = field.Data;

                                    //加上F表示多值域
                                    if (sShapeKeyWord.Substring(sShapeKeyWord.Length - 1) != "F")
                                    {
                                        //修改如果填了之后就将其锁住
                                        //更新文件数据要开锁
                                        if ( field.Locked == true )
                                        {
                                            field.Locked = false;
                                        }

                                        //获取表达式的值
                                        if (oProjectOrDoc is Project)
                                        {
                                            Project project = oProjectOrDoc as Project;
                                            sShapeValue = project.ExcuteDefnExpression(sShapeKeyWord, htUserKeyWord);
                                        }
                                        else if (oProjectOrDoc is Doc)
                                        {
                                            Doc doc = oProjectOrDoc as Doc;
                                            sShapeValue = doc.ExcuteDefnExpression(sShapeKeyWord, htUserKeyWord);
                                        }

                                        //填值并加锁
                                        ////Added by zcx@yandingsoft at 2008-10-17
                                        FillAddInFiled(sShapeValue[0], field);
                                    }
                                }
                                #region TIM 2010-06-07 处理图片 $(CurrentUser_Code).JPG , 用于取得用户上传的数字签名图片
                                else if (field.Type == Word.WdFieldType.wdFieldIncludePicture)
                                {
                                    //更新文件数据时要开锁
                                    if (field.Locked == true)
                                    {
                                        field.Locked = false;
                                    }

                                    if (FillPicField(oProjectOrDoc, htUserKeyWord, field) == false)
                                    {
                                        htPicFields.Add(field.Index.ToString() + field.Code.Text, field);
                                    }

                                }
                                #endregion
                            }
                        }
                    }
                }


                htPicFields.Clear();


                try
                {

                    // 20010-03-24 增加对页眉页脚中的Shapes的处理
                    this.m_wdApp.ActiveWindow.ActivePane.View.SeekView =
                            Microsoft.Office.Interop.Word.WdSeekView.wdSeekCurrentPageHeader;
                    this.m_wdApp.Selection.WholeStory();
                    //检查所有的Shape对象中的域,并给 WdFieldType.wdFieldAddin 的类型的域赋值
                    foreach (Word.Shape shape in this.m_wdApp.Selection.HeaderFooter.Shapes)
                    {
                        if (shape.AutoShapeType == MsoAutoShapeType.msoShapeRectangle)
                        {                            
                            shape.Select(ref m_oMissing);

                            if (shape.TextFrame == null) continue;

                            if (shape.TextFrame.TextRange == null) continue;

                            wdTextFram = shape.TextFrame;                           

                            if (wdTextFram.TextRange != null)
                            {
                                wdTextRange = wdTextFram.TextRange;

                                foreach (Word.Field field in wdTextRange.Fields)
                                {
                                    if (field.Type == Word.WdFieldType.wdFieldAddin)
                                    {
                                        sShapeKeyWord = field.Data;

                                        //加上F表示多值域
                                        if (sShapeKeyWord.Substring(sShapeKeyWord.Length - 1) != "F")
                                        {
                                            //修改如果填了之后就将其锁住
                                            //Added by zcx@yandingsoft at 2008-10-17
                                            //更新文件数据时要开锁
                                            if (field.Locked == true)
                                            {
                                                field.Locked = false;
                                            }
                                            //获取表达式的值
                                            if (oProjectOrDoc is Project)
                                            {
                                                Project project = oProjectOrDoc as Project;
                                                sShapeValue = project.ExcuteDefnExpression(sShapeKeyWord, htUserKeyWord);
                                            }
                                            else if (oProjectOrDoc is Doc)
                                            {
                                                Doc doc = oProjectOrDoc as Doc;
                                                sShapeValue = doc.ExcuteDefnExpression(sShapeKeyWord, htUserKeyWord);
                                            }

                                            //填值并加锁
                                            ////Added by zcx@yandingsoft at 2008-10-17
                                            //删除原先的内容
                                            FillAddInFiled(sShapeValue[0], field);
                                        }
                                    }

                                    #region 处理图片 $(CurrentUser_Code).JPG , 用于取得用户上传的数字签名图片
                                    else if (field.Type == Word.WdFieldType.wdFieldIncludePicture)
                                    {
                                        //更新文件数据时要开锁
                                        if (field.Locked == true)
                                        {
                                            field.Locked = false;
                                        }

                                        if (FillPicField(oProjectOrDoc, htUserKeyWord, field) == false)
                                        {
                                            htPicFields.Add(field.Index.ToString() + field.Code.Text, field);
                                        }

                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                }
                catch (Exception subEx)
                {
                    //2010-04-30
                    _errorList.Add(subEx.ToString());
                    AVEVA.CDMS.Server.CDMS.Write(subEx.ToString());
                }


                htPicFields.Clear();


                try
                {

                    // 20010-03-24 增加对页眉页脚中的Shapes的处理
                    this.m_wdApp.ActiveWindow.ActivePane.View.SeekView =
                            Microsoft.Office.Interop.Word.WdSeekView.wdSeekCurrentPageFooter;
                    this.m_wdApp.Selection.WholeStory();
                    //检查所有的Shape对象中的域,并给 WdFieldType.wdFieldAddin 的类型的域赋值
                    foreach (Word.Shape shape in this.m_wdApp.Selection.HeaderFooter.Shapes)
                    {
                        if (shape.AutoShapeType == MsoAutoShapeType.msoShapeRectangle)
                        {


                            shape.Select(ref m_oMissing);

                            if (shape.TextFrame == null) continue;

                            if (shape.TextFrame.TextRange == null) continue;

                            wdTextFram = shape.TextFrame;

                            if (wdTextFram.TextRange != null)
                            {
                                wdTextRange = wdTextFram.TextRange;

                                foreach (Word.Field field in wdTextRange.Fields)
                                {
                                    if (field.Type == Word.WdFieldType.wdFieldAddin)
                                    {
                                        sShapeKeyWord = field.Data;

                                        //加上F表示多值域
                                        if (sShapeKeyWord.Substring(sShapeKeyWord.Length - 1) != "F")
                                        {
                                            //修改如果填了之后就将其锁住
                                            //Added by zcx@yandingsoft at 2008-10-17
                                            //更新文件数据时要开锁
                                            if (field.Locked == true )
                                            {
                                                field.Locked = false;
                                            }

                                            //获取表达式的值
                                            if (oProjectOrDoc is Project)
                                            {
                                                Project project = oProjectOrDoc as Project;
                                                sShapeValue = project.ExcuteDefnExpression(sShapeKeyWord, htUserKeyWord);
                                            }
                                            else if (oProjectOrDoc is Doc)
                                            {
                                                Doc doc = oProjectOrDoc as Doc;
                                                sShapeValue = doc.ExcuteDefnExpression(sShapeKeyWord, htUserKeyWord);
                                            }

                                            //填值并加锁
                                            ////Added by zcx@yandingsoft at 2008-10-17
                                            FillAddInFiled(sShapeValue[0], field);
                                            
                                        }
                                    }

                                    #region 处理图片 $(CurrentUser_Code).JPG , 用于取得用户上传的数字签名图片
                                    else if (field.Type == Word.WdFieldType.wdFieldIncludePicture)
                                    {
                                        //更新文件数据时要开锁
                                        if (field.Locked == true)
                                        {
                                            field.Locked = false;
                                        }
                                        if (FillPicField(oProjectOrDoc, htUserKeyWord, field) == false)
                                        {
                                            htPicFields.Add(field.Index.ToString() + field.Code.Text, field);
                                        }
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                }
                catch (Exception subEx2)
                {
                    //2010-04-30
                    _errorList.Add(subEx2.ToString());
                    AVEVA.CDMS.Server.CDMS.Write(subEx2.ToString());
                }




            }
            catch (Exception ex)
            {
                //添加错误信息到错误列表中
                if (_errorList != null)
                {
                    {
                        _errorList.Add(ex.Message);
                    }
                }
                //遇上错误时是否关闭正在处理的程序,默认不关闭,false
                //2010-04-30

                AVEVA.CDMS.Server.CDMS.Write(ex.ToString());
                Release(_closeErrorApp);
            }
        }
        
        //小黎 增加删除自定义域值的方法
        public void ChearAddrInValue( Word.Field field )
        {
            field.Select();
            Word.Range range = field.Result;
            this.m_wdApp.Selection.WholeStory();
            string rangeValue = range.Text;
            int index = range.Start;
            if (string.IsNullOrEmpty(rangeValue))
            {
                rangeValue = "";
            }
            while (!rangeValue.Contains("\r"))
            {
                range.End = index++;
                rangeValue = range.Text;
                if (string.IsNullOrEmpty(rangeValue))
                {
                    rangeValue = "";
                }
            }
            range.Text = "\r\a";
        }


#endregion






        /// <summary>
        /// 写入文档
        /// </summary>
        /// <param name="oProjectOrDoc">Project 或 Doc</param>
        /// <param name="sFileName">文件名</param>
        /// <param name="bVisibleAppl">是否显示Word 或 Excel</param>
        /// <returns>是否成功</returns>
        public bool WriteDataToDocument(object oProjectOrDoc, string sFileName, Hashtable htUserKeyWord)
        {
            try
            {
                //判断文件名是否存在
                if (File.Exists(sFileName))
                {

                    //先判断要打开的文件是什么类型
                    if (sFileName.ToUpper().IndexOf(".DOC") > 0)
                    {
                        //小黎 2011-11-28 增加对word2007图片域的更新
                        if ( sFileName.ToLower().EndsWith(".docx") )
                        {
                            this.IsWord07 = true;
                        }


                        //先判断文件是否已经打开
                        if (!WordIsRunning(m_wdApp, sFileName))
                        {
                            //根据文件类型打开文件
                            if (!OpenWordNew(sFileName)) return false;

                            //插入单值域的值
                            FillWdFieldValue(oProjectOrDoc, htUserKeyWord);

                            //插入多值域的值
                            InsertCollection(oProjectOrDoc, htUserKeyWord);



                            //完成操作后保存文件
                            SaveWord(sFileName as object);
                        }
                        else
                        {
                            _errorList.Add("[" + sFileName + "已经打开!]");
                        }

                    }
                    else if (sFileName.ToUpper().IndexOf(".XLS") > 0)
                    {
                        //先判断文件是否已经打开
                        if (!ExcelIsRunning(m_exApp, sFileName))
                        {

                            //打开文件
                            if (!OpenExcelNew(sFileName)) return false;

                            
                            //扫描excel中的表达式并为其赋值
                            //ParseExcelField(oProjectOrDoc, htUserKeyWord,new Hashtable());
                            ParseExcelFieldEx(oProjectOrDoc, htUserKeyWord, new Hashtable());

                            //关闭文档
                            SaveExcel();
                        }
                        else
                        {
                            _errorList.Add("[" + sFileName + "已经打开!]");
                        }
                    }

                    return true;
                }
                else
                {
                    _errorList.Add("[" + sFileName + "]不存在!");
                }

                return false;
            }
            catch (Exception ex)
            {
                //添加错误信息到错误列表中
                _errorList.Add(ex.Message);
             
                //2010-04-30
                AVEVA.CDMS.Server.CDMS.Write(ex.ToString()); 

                return false;
            }
        }


        /// <summary>
        /// 写入文档
        /// </summary>
        /// <param name="oProjectOrDoc">Project 或 Doc</param>
        /// <param name="sFileName">文件名</param>
        /// <param name="bVisibleAppl">是否显示Word 或 Excel</param>
        /// <param name="auditDataList">校审意见列表</param>
        /// <returns></returns>
        public bool WriteDataToDocument(object oProjectOrDoc, string sFileName, Hashtable htUserKeyWord , List<String> auditDataList)
        {
            try
            {
                //判断文件名是否存在
                if (File.Exists(sFileName))
                {

                    //先判断要打开的文件是什么类型
                    if (sFileName.ToUpper().IndexOf(".DOC") > 0)
                    {
                        //先判断文件是否已经打开
                        if (!WordIsRunning(m_wdApp, sFileName))
                        {
                            
                            //根据文件类型打开文件
                            if (!OpenWordNew(sFileName)) return false;

                            //插入单值域的值
                            FillWdFieldValue(oProjectOrDoc, htUserKeyWord);

                            //插入多值域的值
                            InsertCollection(oProjectOrDoc, htUserKeyWord);

                            //插入校审意见
                            InsertAuditCollection("WORKFLOWAUDITS" , auditDataList); 

                            //完成操作后保存文件
                            SaveWord(sFileName as object);
                        }
                        else
                        {
                            _errorList.Add("[" + sFileName + "已经打开!]");
                        }

                    }
                    else if (sFileName.ToUpper().IndexOf(".XLS") > 0)
                    {
                        //先判断文件是否已经打开
                        if (!ExcelIsRunning(m_exApp, sFileName))
                        {

                            //打开文件
                            if (!OpenExcelNew(sFileName)) return false;

                            Hashtable hash = new Hashtable();
                            hash.Add("WORKFLOWAUDITS",auditDataList);
                            //扫描excel中的表达式并为其赋值
                            //ParseExcelField(oProjectOrDoc, htUserKeyWord,hash);
                            ParseExcelFieldEx(oProjectOrDoc, htUserKeyWord, hash);

                            //关闭文档
                            SaveExcel();
                        }
                        else
                        {
                            _errorList.Add("[" + sFileName + "已经打开!]");
                        }
                    }

                    return true;
                }
                else
                {
                    _errorList.Add("[" + sFileName + "]不存在!");
                }
                return false;
            }
            catch (Exception ex)
            {
                //添加错误信息到错误列表中
                _errorList.Add(ex.Message);
               
                //2010-04-30
                AVEVA.CDMS.Server.CDMS.Write(ex.ToString()); 

                return false;
            }
        }

       
        /// <summary>
        /// 写入文档
        /// </summary>
        /// <param name="oProjectOrDoc">Project 或 Doc</param>
        /// <param name="sFileName">文件名</param>
        /// <param name="bVisibleAppl">是否显示Word 或 Excel</param>
        /// <param name="htAuditDataList">校审意见列表,其中每一项都是一个 [用户关键字]-[校审意见数据列表]</param>
        /// <returns></returns>
        public bool WriteDataToDocument(object oProjectOrDoc, string sFileName, Hashtable htUserKeyWord, Hashtable htAuditDataList)
        {
            try
            {
                //判断文件名是否存在
                if (File.Exists(sFileName))
                {

                    //先判断要打开的文件是什么类型
                    if (sFileName.ToUpper().IndexOf(".DOC") > 0)
                    {
                        //先判断文件是否已经打开
                        if (!WordIsRunning(m_wdApp, sFileName))
                        {

                            //根据文件类型打开文件
                            if (!OpenWordNew(sFileName)) return false;

                            //插入单值域的值
                            FillWdFieldValue(oProjectOrDoc, htUserKeyWord);

                            //插入多值域的值
                            InsertCollection(oProjectOrDoc, htUserKeyWord);

                            //插入校审意见
                            InsertAuditCollection(htAuditDataList);


                            #region 填写完Word属性项，保存Word前触发的事件
                            bool needReWrite = false;
                            Hashtable htReWrite = new Hashtable();
                            foreach (WebOfficeEvent.Before_Save_Word_Event_Class BeforeSaveWord in WebOfficeEvent.ListBeforeSaveWord)
                            {
                                if (BeforeSaveWord.Event != null)
                                {

                                    if (BeforeSaveWord.Event(BeforeSaveWord.PluginName, m_wdApp,  oProjectOrDoc,ref needReWrite,ref htReWrite))
                                    {
                                        if (needReWrite) {
                                            //插入单值域的值(重新把页数写入Word文档)
                                            FillWdFieldValue(oProjectOrDoc, htReWrite);
                                        }

                                        break;
                                    }
                                }
                            }
                  
                            #endregion

                            //完成操作后保存文件
                            SaveWord(sFileName as object);
                        }
                        else
                        {
                            _errorList.Add("[" + sFileName + "已经打开!]");
                        }

                    }
                    else if (sFileName.ToUpper().IndexOf(".XLS") > 0)
                    {
                        //先判断文件是否已经打开
                        if (!ExcelIsRunning(m_exApp, sFileName))
                        {

                            //打开文件
                            if (!OpenExcelNew(sFileName)) return false;

                            //扫描excel中的表达式并为其赋值
                            //ParseExcelField(oProjectOrDoc, htUserKeyWord,htAuditDataList);
                            ParseExcelFieldEx(oProjectOrDoc, htUserKeyWord, htAuditDataList);

                            //关闭文档
                            SaveExcel();
                        }
                        else
                        {
                            _errorList.Add("[" + sFileName + "已经打开!]");
                        }
                    }

                    return true;
                }
                else
                {
                    _errorList.Add("[" + sFileName + "]不存在!");
                }
                return false;
            }
            catch (Exception ex)
            {
                //添加错误信息到错误列表中
                _errorList.Add(ex.Message);
               
                //2010-04-30
                AVEVA.CDMS.Server.CDMS.Write(ex.ToString()); 

                return false;
            }
        }


        #endregion

        #region 填写Excel

        /// <summary>
        /// 填写Excel
        /// </summary>
        /// <param name="sFileName">excel文件名</param>
        /// <param name="iRowNum">从第几行开始填写</param>
        /// <param name="arrColumnName">列名</param>
        /// <param name="arrData">填写的数据</param>
        /// <returns>是否成功</returns>
        public bool FillExcel(string sSaveAsPath, int iRowNum, string[] arrColumnName, string[] arrData)
        {
            if (iRowNum <= 0)
            {
                _errorList.Add("行号不能小于等于0!");
                return false ;
            }

            if (arrColumnName.Length <= 0)
            {
                _errorList.Add("列名不存在!");
                return false;
            }

            if (arrData.Length <= 0)
            {
                _errorList.Add("数据不存在!");
                return false;
            }

            m_exApp = null;
            m_exWorkBook = null;
            m_exWorksheet = null;

            //列数
            int iColumn = arrColumnName.Length;

            //行数
            int iRow = arrData.Length / arrColumnName.Length;

            try
            {
                try
                {
                    //获取当前正在运行的Excel
                    //m_exApp = Interaction.GetObject(null, "excel.application") as Excel.Application;
                    //m_exApp = Marshal.GetActiveObject("excel.application") as Excel.Application;
                    m_exApp = new Microsoft.Office.Interop.Excel.ApplicationClass(); 
                }
                catch
                {
                    m_exApp = null;
                }

                if (m_exApp == null)
                {
                    //m_exApp = Interaction.CreateObject("excel.application", "") as Excel.Application;
                    Type exType = Type.GetTypeFromProgID("excel.application");
                    m_exApp = Activator.CreateInstance(exType, true) as Excel.Application;
                }

                if (m_exApp == null) return false;

                //是否显示Excel
                m_exApp.Visible = _visibleApp;

                m_exWorkBook = m_exApp.Workbooks.Add(m_oMissing);

                m_exWorksheet = m_exWorkBook.Worksheets[1] as Worksheet;

                //列头
                for (int i = 0; i < arrColumnName.Length; i++)
                {
                    m_exWorksheet.Cells[iRowNum, i + 1] = arrColumnName[i];
                    
                }

                //数据
                for (int n = 0; n < arrData.Length; n++)
                {
                    m_exWorksheet.Cells[n / iColumn + 1 + iRowNum, n % iColumn + 1] = arrData[n];
                }

                //自动对齐
                m_exWorksheet.Cells.EntireColumn.AutoFit();

                m_exWorkBook.SaveAs(sSaveAsPath, m_oMissing, m_oMissing, m_oMissing, m_oMissing,

                    m_oMissing, Excel.XlSaveAsAccessMode.xlShared, m_oMissing, m_oMissing, m_oMissing,

                    m_oMissing, m_oMissing);

                //*********
                //保存Excel
                SaveExcel();

                return true;
            }

            catch (Exception ex)
            {
               
                _errorList.Add(ex.ToString());
                AVEVA.CDMS.Server.CDMS.Write(ex.ToString()); 
               
                //释放资源
                Release(true);

                return false;
            }
        }

        #endregion

        #region 填充Excel文档中的索引的值,索引来自于lstHtIndex

        /// <summary>
        /// 填充Excel文档中的索引的值,索引来自于lstHtIndex
        /// </summary>
        /// <param name="sFileName">文件名,该文件为全路径,若在本地不存在,则到服务器下载</param>
        /// <param name="lstHtIndex">键:Excel的列头 如A,B,C 等</param>
        /// <returns>是否成功</returns>
        public bool FillExcelIndexValue(string sFileName, string[] IndexCel, List<Hashtable> DataList)
        {
            try
            {
                //文件名为空或哈希表没值则退出
                if (sFileName.Trim() == string.Empty || DataList.Count == 0)
                {
                    return false;
                }

                //取用于比较的数据
                string[] IndexData = new string[IndexCel.Length];


                //若文件存在于本地计算机
                if (File.Exists(sFileName))
                {

                    //打开Excel
                    if (!OpenExcelNew(sFileName)) return false;


                    //没有返回Excel对象
                    if (m_exWorksheet == null) return false;


                    //逐行比较
                    int CuRow = 1;           //当前行
                    int NoDataNumber = 0;    //没有数据的行数


                    //5行没有值退出
                    int i;
                    bool mark;


                    //哈希表的值
                    string value;

                    while (NoDataNumber < 5)
                    {

                        //取值
                        i = 0;
                        foreach (string data in IndexCel)
                        {
                            //取值
                            IndexData[i] = ((Range)m_exWorksheet.Cells[CuRow, ConverToInt(data)]).Text.ToString().Trim();
                            i++;
                        }


                        //是否有值
                        mark = false;
                        foreach (string data in IndexData)
                        {
                            if (data.Trim().Length > 0)
                            {
                                mark = true;
                                break;
                            }
                        }


                        //本行没有数据
                        if (mark == false)
                            NoDataNumber++;
                        else
                            NoDataNumber = 0;


                        //如果有值进行比较
                        if (mark)
                        {

                            //逐个进行比较
                            foreach (Hashtable hasData in DataList)
                            {

                                //判断当前的值是否和电子表格的当前行的值是否相同
                                mark = true;
                                i = 0;

                                foreach (string data in IndexData)
                                {
                                    value = hasData[IndexCel[i]] == null ? "" : hasData[IndexCel[i]].ToString();

                                    if (data.ToUpper() != value.ToUpper())
                                    {
                                        mark = false;
                                        break;
                                    }

                                    i++;
                                }

                                //如果相等
                                if (mark)
                                {

                                    //赋值
                                    foreach (DictionaryEntry dd in hasData)
                                    {
                                        m_exWorksheet.Cells[CuRow, ConverToInt(dd.Key.ToString())] = dd.Value;
                                    }

                                    //删除已经赋值的数据
                                    DataList.Remove(hasData);

                                    break;
                                }
                            }

                        }


                        //处理下一行
                        CuRow++;

                    }


                    //没有找到对应行的值
                    CuRow = CuRow - NoDataNumber;
                    foreach (Hashtable hasData in DataList)
                    {
                        foreach (DictionaryEntry dd in hasData)
                        {
                            //填写值
                            m_exWorksheet.Cells[CuRow, ConverToInt(dd.Key.ToString())] = dd.Value;
                        }

                        //增加一行
                        CuRow++;
                    }

                    //保存Excel
                    SaveExcel();

                    return true;
                }
                else
                {
                    _errorList.Add(sFileName + "不存在");
                }
            }
            catch (Exception ex)
            {
                _errorList.Add(ex.ToString());
                AVEVA.CDMS.Server.CDMS.Write(ex.ToString()); 
                //出现异常,释放资源
                Release(true);
            }

            return false;
        }

        #endregion

        /// <summary>
        /// 填充Excel文档中页眉
        /// </summary>
        /// <param name="sFileName"></param>
        /// <param name="LeftHeader"></param>
        /// <param name="CenterHeader"></param>
        /// <param name="RightHeader"></param>
        /// <returns></returns>
        public bool FillExcelHeader(string sFileName,string LeftHeader,string CenterHeader,string RightHeader)
        {

            //文件名为空或哈希表没值则退出
            if (sFileName.Trim() == string.Empty || (string.IsNullOrEmpty(LeftHeader) && string.IsNullOrEmpty(CenterHeader) && string.IsNullOrEmpty(RightHeader)))
            {
                return false;
            }



            return false;
        }

        #region 根据行号，列名取Excel的一行数据

        /// <summary>
        /// 根据行号，列名取Excel的一行数据
        /// </summary>
        /// <param name="rowNo">行号</param>
        /// <param name="colNames">列名</param>
        /// <returns>当读取正确时在哈希表中加入键值RESULT=OK,否则RESULT=EXCEPTION</returns>
        public Hashtable GetExcelData(int rowNo, string[] colNames)
        {
            Hashtable ht = null;
            try
            {
                //单元格的值
                string value;

                if (rowNo <= 0)
                {
                    _errorList.Add("行号必须大于等于1！");
                    return ht;
                }
                if (colNames == null || colNames.Length <= 0)
                {
                    _errorList.Add("列名数组中没有任何数据！");
                    return ht;
                }
                if (m_exWorksheet != null)
                {
                    ht = new Hashtable();
                    for (int i = 0; i < colNames.Length; i++)
                    {
                        if (!ht.ContainsKey(colNames[i]))
                        {
                            value = ((Excel.Range)m_exWorksheet.Cells[rowNo, ConverToInt(colNames[i])]).Text as string;
                            if (value != null)
                            {
                                ht.Add(colNames[i], value);
                            }
                            else
                            {
                                ht.Add("RESULT", "EXCEPTION");
                            }
                        }
                    }
                    ht.Add("RESULT", "OK");
                }
            }
            catch (Exception ex)
            {
                //2010-04-30
                _errorList.Add(ex.ToString());
                AVEVA.CDMS.Server.CDMS.Write(ex.ToString()); 

                ht.Add("RESULT", "EXCEPTION");
            }
            return ht;
        }

        #endregion

        #region  字符转ASCII码，ASCII码转字符

        /// <summary>
        /// 字符转ASCII码
        /// </summary>
        /// <param name="character">字符</param>
        /// <returns></returns>
        public static int Asc(string character)
        {
            if (character.Length == 1)
            {
                System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                int intAsciiCode = (int)asciiEncoding.GetBytes(character)[0];
                return (intAsciiCode);
            }
            else
            {
                throw new Exception("Character is not valid.");
            }

        }


        /// <summary>
        /// ASCII码转字符
        /// </summary>
        /// <param name="asciiCode">ascii码</param>
        /// <returns></returns>
        public static string Chr(int asciiCode)
        {
            if (asciiCode >= 0 && asciiCode <= 255)
            {
                System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                byte[] byteArray = new byte[] { (byte)asciiCode };
                string strCharacter = asciiEncoding.GetString(byteArray);
                return (strCharacter);
            }
            else
            {
                throw new Exception("ASCII Code is not valid.");
            }
        }

        #endregion

        #region 把A转换为1 把B转换为2 ....把AA转换为27...

        /// <summary>
        /// 把A转换为1 把B转换为2 ....把AA转换为27...
        /// </summary>
        /// <param name="s">要转换的字符串</param>
        /// <returns></returns>
        protected int ConverToInt(string s)
        {
            int num = 0;
            string sFirst;
            int iFirstCharAsc = 0;
            sFirst = s.Substring(0, 1);

            if (HasTwoChars(s))
            {
                string sSecond = s.Substring(1, 1);

                iFirstCharAsc = Asc(sFirst) - 65 + 1;
                int iSecondCharAsc = Asc(sSecond) - 65 + 1;

                for (int i = iFirstCharAsc; i < 27; i++)
                {
                    for (int j = iSecondCharAsc; j < 27; )
                    {
                        num = iFirstCharAsc * 26 + iSecondCharAsc;
                        return num;
                    }
                }
            }
            else
            {
                num = iFirstCharAsc = Asc(sFirst) - 65 + 1;
            }

            return num;
        }

        #endregion

        #region 判断字符串是否包含两个字符

        /// <summary>
        /// 判断字符串是否包含两个字符
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        protected bool HasTwoChars(string s)
        {
            int i = 0;
            foreach (char c in s)
            {
                i++;
            }
            if (i == 2)
            {
                return true;
            }
            return false;
        }

        #endregion

        #region 
        public bool CopyDocumentData(String mainFilePath, List<String> fileList)
        {
            try
            {
                if (String.IsNullOrEmpty(mainFilePath) || !File.Exists(mainFilePath))
                    return false;

                if (fileList == null || fileList.Count <= 0)
                    return true;

                foreach (String file in fileList)
                {
                    if (String.IsNullOrEmpty(file) || !File.Exists(file))
                        continue;

                    this.OpenWordNew(file);

                    this.m_wdApp.Selection.WholeStory();

                    this.m_wdApp.Selection.Copy();


                    this.Release(true);

                    System.Threading.Thread.Sleep(2000); 

                    this.OpenWordNew(mainFilePath);

                    object oMoveUnitWord = Word.WdUnits.wdStory;

                    object oMoveExtend = m_oMissing; 

                    
                    this.m_wdApp.Selection.EndKey(ref oMoveUnitWord , ref oMoveExtend) ;

                    this.m_wdApp.Selection.TypeParagraph();

                    //this.m_wdApp.Selection.MoveDown(ref object obj= Word.WdUnits.wdLine ,(object ) 1 , oMoveExtend); 

                    this.m_wdApp.Selection.PasteAndFormat(Microsoft.Office.Interop.Word.WdRecoveryType.wdPasteDefault);

                    this.SaveWord(mainFilePath);

                    //this.Release(true); 
                    
                }

                return true; 
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message + "\r\nDetail:\r\n          " + e.StackTrace); 
                //2010-04-30
                _errorList.Add(e.ToString());
                AVEVA.CDMS.Server.CDMS.Write(e.ToString()); 
            }

            return false; 
        }
        #endregion

        public void CreateExcelField(string key,string createType)
        {
            if (m_exApp == null) return;

            string type = (createType.ToUpper() == "BMP") ? "BMP:" : "TEXT:";
            Excel.Range range = m_exApp.ActiveCell;

            if (range == null) return;

            string text = type + key;

            range.Value2 = text;

           // range.Hidden = true;

            m_exApp.ActiveWorkbook.RefreshAll();
        }

        public void ParseExcelField(object obj, Hashtable htUserKeyWord, Hashtable htAuditDataList)
        {
            Project proj = null; Doc doc = null;
            if (obj is Project) proj = obj as Project;
            else if (obj is Doc) doc = obj as Doc;
            else return;

            if (m_exWorkBook == null) return;

            Regex r = new Regex("(TEXT|BMP)(:)(.*)");
            foreach (Excel.Worksheet workSheet in m_exWorkBook.Worksheets)
            {
                int curRow = 1;
                int curCol = 1;
                int noval = 0;
                int maxRow = 1000;
                int maxCol = 100;
                int rval = 0;

                while (curRow <= maxRow && rval <= 10)
                {
                    noval = 0;
                    curCol = 1;
                    int bc = 0;

                    while (curCol <= maxCol && noval <= 20)
                    {
                        Excel.Range range = (Excel.Range)workSheet.Cells[curRow, curCol];
                        string val = (range).Value2 as string;

                        if (string.IsNullOrEmpty(val))
                        {
                            noval++;
                            if (curCol == 0) bc = 0;
                            if (noval > 20 && bc == 0) rval++;
                            curCol++;
                            continue;
                        }

                        noval = 0;
                        Match m = r.Match(val);

                        if (m == null && !m.Success)
                        {
                            curCol++;
                            continue;
                        }

                        string exp = m.Groups[3].Value;
                        string type = m.Groups[1].Value;


                        if (exp.ToUpper().Contains("$(*") || exp.ToUpper().Contains("$[*"))
                        {
                            exp = exp.Replace("$(*", "$(");
                            exp = exp.Replace("$[*", "$[");

                            range.Value2 = type + ":" + exp;
                        }
                        else
                        {
                            if (proj != null)
                            {
                                exp = proj.ExcuteDefnExpression(exp, htUserKeyWord)[0];
                            }
                            else if (doc != null)
                            {
                                exp = doc.ExcuteDefnExpression(exp, htUserKeyWord)[0];
                            }

                            if (type.ToUpper() == "BMP")
                            {
                                if (!exp.ToUpper().EndsWith(".JPG"))
                                {
                                    exp += ".jpg";
                                }

                                string lPath = DownLoadFile((proj != null) ? proj : (doc.Project != null) ? doc.Project : null, "BMP\\" + exp);

                                if (File.Exists(lPath))
                                {
                                    range.Value2 = "";
                                    range.Select();
                                    //workSheet.Shapes.AddPicture(zkLocalPath,MsoTriState.msoFalse,MsoTriState.msoTrue,rang.
                                    Excel.Pictures pic = (Excel.Pictures)workSheet.Pictures(System.Reflection.Missing.Value);
                                    pic.Insert(lPath, System.Reflection.Missing.Value);
                                }


                            }
                            else if (type.ToUpper() == "TEXT")
                            {
                                if (!exp.Contains(":"))
                                {
                                    range.Value2 = exp;
                                }
                                else
                                {
                                    String[] strs = exp.Split(new char[] { ':' });
                                    if (strs == null || strs.Length != 3)
                                    {
                                        range.Value2 = exp;
                                    }
                                    else
                                    {
                                        int col = curCol, row = curRow - 1;
                                        List<String> data = htAuditDataList[strs[0]] as List<string>;
                                        int colCount = int.Parse(strs[2]);
                                        int rowCount = int.Parse(strs[1]) * colCount;
                                        Excel.Range rng = null;

                                        for (int n = 0; n <= data.Count - colCount; )
                                        {

                                            if (rowCount > 0 && n >= rowCount)
                                            {
                                                Excel.Range rrr = (workSheet.Cells[row + 1, col] as Excel.Range).EntireRow;
                                                rrr.Insert(Excel.XlInsertShiftDirection.xlShiftDown, Excel.XlInsertFormatOrigin.xlFormatFromLeftOrAbove);
                                            }

                                            row++;
                                            for (int rc = 0,cc=rc; rc < colCount; rc++)
                                            {
                                                rng = (Excel.Range)workSheet.Cells[row, cc + col];
                                                //workSheet.Rows.set_Item(row, rc + col, data[n + rc]);
                                                if (rng.MergeCells.ToString().ToLower() == "false")
                                                {
                                                    rng.Value2 = data[n + rc];
                                                    cc++;
                                                }
                                                else
                                                {
                                                    Excel.Range mrng = (Excel.Range)rng.MergeArea;
                                                    mrng.set_Item(1, 1, data[n + rc]);
                                                    cc += mrng.Count;
                                                }
                                                
                                            }
                                            

                                            n = n + colCount;
                                        }

                                        //(workSheet.Cells[row+1, col] as Excel.Range).EntireRow.Delete(Excel.XlDeleteShiftDirection.xlShiftUp);
                                        //curRow = row;
                                    }
                                }
                            }
                        }

                        m_exWorkBook.RefreshAll();
                        curCol++;

                    }

                    curRow++;
                }
            }
        }



        private bool isFirstWriteData = true;


        /// <summary>
        /// 是否为第一次写属性
        /// </summary>
        public bool IsFirstWriteData
        {
            get { return isFirstWriteData; }
            set { isFirstWriteData = value; }
        }



        public void ParseExcelFieldEx(object obj, Hashtable htUserKeyWord, Hashtable htAuditDataList)
        {
            Project proj = null; Doc doc = null;
            if (obj is Project) proj = obj as Project;
            else if (obj is Doc) doc = obj as Doc;
            else return;


            

            try
            {
                if (m_exWorkBook == null) return;

               
  
                
                Regex r = new Regex("(TEXT|BMP)(:)(.*)");
                //int iCur = 0; 
                //while (true)
                //{
                //    try
                //    {
                //        if (iCur > 60000 || (m_exWorkBook.Worksheets != null && m_exWorkBook.Worksheets.Count > 0))
                //            break; 
                //    }
                //    catch (Exception stempEx)
                //    { }


                //    System.Threading.Thread.Sleep(1000); 

                //    iCur += 1000; 
                    
                //}
                
               

                foreach (Excel.Worksheet workSheet in m_exWorkBook.Worksheets)
                {

                    #region 小钱 2018.5.14 添加填充页眉功能

                    string rHeaderText = "$(RIGHT__HEADER)";
                    //解析返回值
                    if (proj != null)
                    {
                        rHeaderText = proj.ExcuteDefnExpression(rHeaderText, htUserKeyWord)[0];
                    }
                    else if (doc != null)
                    {
                        rHeaderText = doc.ExcuteDefnExpression(rHeaderText, htUserKeyWord)[0];
                    }


                    //返回值为空 则不处理
                    if (!string.IsNullOrEmpty(rHeaderText))
                    {
                        workSheet.PageSetup.RightHeader = rHeaderText;
                        //worksheet.PageSetup.CenterHeader = @"&""Arial""&16 页眉".。这样能设置字体和大小
                    }
                    #endregion

                    int curRow = 1;
                    int curCol = 1;
                    int maxRow = 1000;
                    int maxCol = 100;
                 
                
                    Range usedRange = workSheet.UsedRange;


                    maxRow = workSheet.UsedRange.Rows.Count;
                    maxCol = workSheet.UsedRange.Columns.Count; 

                 
                   
                    
                    int iRangeCount = 0;



                    //TIM 2010-06-28





                    List<Range> rangeList = new List<Range>(); 
                    object[,] values = null ;


                    //TIM 2010-11-11 如果
                    if (IsFirstWriteData)
                    {

                        values = (object[,])usedRange.Value2;

                        if (values == null)
                            continue;

                        for (int i = 1; i <= values.GetLength(0); i++)
                        {

                            for (int j = 1; j <= values.GetLength(1); j++)
                            {
                                if (values[i, j] != null && !String.IsNullOrEmpty(values[i, j].ToString()))
                                {
                                    rangeList.Add(workSheet.Cells[i, j] as Range);
                                }
                            }

                        }

                    }
                    else
                    {
                        foreach (Range range in usedRange)
                        {
                            rangeList.Add(range); 
                        }
                    }


                    foreach (Range range in rangeList /*usedRange*/)
                    {



                        iRangeCount++;
                        //Excel.Range range = (Excel.Range)workSheet.Cells[curRow, curCol];
                        //小黎 更改更新数据的获取关键字的写法 2011-9-23 下面注释的那一句为原代码
                        //    string val = (range).Value2 as string;
                        string val = "";
                        if (this.IsUpdateFileData == true)
                        {   //加入异常处理,若该区域的数据有效性规则为空,则com调用会产生异常
                            string temp = "";
                            try
                            {
                                temp = range.Validation.InputMessage;
                            }
                            catch (System.Exception ex)
                            {
                                temp = "";
                            }
                            if (string.IsNullOrEmpty(temp))
                            {
                                val = "";
                            }
                            else
                            {
                                val = temp;
                            }
                        }
                        else
                        {
                            val = (range).Value2 as string;
                        }



                        #region 解析并写入值
                        curRow = range.Row;
                        curCol = range.Column;

                        if (!IsFirstWriteData)
                        {
                            val = range.Validation.InputMessage;
                        }
                        //没有表达式
                        if (string.IsNullOrEmpty(val))
                        {
                            continue;
                        }



                        //TIM 2010-11-15
                        if (IsFirstWriteData && val.ToLower().Contains("$"))
                        {
                            //需要把表达式记录到InputMessage中

                            try
                            {
                                range.Validation.Add(XlDVType.xlValidateInputOnly, XlDVAlertStyle.xlValidAlertInformation, Type.Missing, Type.Missing, Type.Missing);
                                range.Validation.InputMessage = val;
                            }
                            catch (Exception subTempEx)
                            {

                            }
                        }


                        //解析表达式
                        Match m = r.Match(val);
                        if (m == null || !m.Success)
                        {
                            continue;
                        }

                        string exp = m.Groups[3].Value;
                        string type = m.Groups[1].Value;


                        //返回值为空 则不处理
                        if (string.IsNullOrEmpty(exp))
                        {
                            continue;
                        }


                        //没有包含表达式
                        if (!exp.Contains("$[") && !exp.Contains("$(") && !exp.ToUpper().Contains("SELECT ") && !val.ToUpper().Contains("TEXT:") && !val.ToUpper().Contains("BMP:"))
                        {
                            continue;
                        }


                        //解析返回值
                        if (proj != null)
                        {
                            exp = proj.ExcuteDefnExpression(exp, htUserKeyWord)[0];
                        }
                        else if (doc != null)
                        {
                            exp = doc.ExcuteDefnExpression(exp, htUserKeyWord)[0];
                        }


                        //返回值为空 则不处理
                        if (string.IsNullOrEmpty(exp))
                        {
                            continue;
                        }


                        //第二次填写
                        if (exp.ToUpper().Contains("$(*") || exp.ToUpper().Contains("$[*"))
                        {
                            exp = exp.Replace("$(*", "$(");
                            exp = exp.Replace("$[*", "$[");

                            range.Value2 = type + ":" + exp;
                        }
                        else
                        {

                            if (type.ToUpper() == "BMP")
                            {

                                //加上后缀
                                if (!exp.ToUpper().EndsWith(".JPG"))
                                {
                                    exp += ".jpg";
                                }

                                //文件名称不存在
                                if (exp.Length < 5)
                                    continue;


                                string lPath = DownLoadFile((proj != null) ? proj : (doc.Project != null) ? doc.Project : null, "BMP\\" + exp);

                                if (!String.IsNullOrEmpty(lPath) && File.Exists(lPath))
                                {
                                    range.Value2 = "";
                                    range.Select();
                                    //workSheet.Shapes.AddPicture(zkLocalPath,MsoTriState.msoFalse,MsoTriState.msoTrue,rang.
                                    //Excel.Pictures pic = (Excel.Pictures)workSheet.Pictures(System.Reflection.Missing.Value);
                                    //pic.Insert(lPath, System.Reflection.Missing.Value);
                                    workSheet.Shapes.AddPicture(lPath, MsoTriState.msoFalse, MsoTriState.msoCTrue, Convert.ToSingle(range.Left) + 2, Convert.ToSingle(range.Top) + 2, Convert.ToSingle(range.MergeArea.Width) - 4, Convert.ToSingle(range.MergeArea.Height) - 4);
                                }


                            }
                            else if (type.ToUpper() == "TEXT")
                            {
                                //表达式: TEXT:**$(....)
                                //表达式: TEXT:keyword:行数:列数 

                                if (val.Contains("$("))
                                {
                                    range.Value2 = exp;
                                    //小黎 2012-6-15 增加签名后的颜色为黑色,这样若定制模板的时候若为白色,打印时就不会出现未签署的表达式了
                                    range.Font.ColorIndex = 1; //1代表为黑色
                                }
                                else
                                {
                                    // 用户自定义表达式
                                    // TEXT:keyword:行数:列数 
                                    String[] strs = exp.Split(new char[] { ':' });
                                    if (strs == null || strs.Length != 3)
                                    {
                                        range.Value2 = exp;
                                    }
                                    else if (htAuditDataList != null && htAuditDataList.Count > 0 && htAuditDataList.Contains(strs[0]))
                                    {
                                        int col = curCol, row = curRow - 1;
                                        List<String> data = htAuditDataList[strs[0]] as List<string>;



                                        int colCount = int.Parse(strs[2]);
                                        int rowCount = int.Parse(strs[1]) * colCount;
                                        Excel.Range rng = null;

                                        if (data == null || data.Count < colCount)
                                            continue;


                                        for (int n = 0; n <= data.Count - colCount;)
                                        {

                                            if (rowCount > 0 && n >= rowCount)
                                            {
                                                Excel.Range rrr = (workSheet.Cells[row + 1, col] as Excel.Range).EntireRow;
                                                rrr.Insert(Excel.XlInsertShiftDirection.xlShiftDown, Excel.XlInsertFormatOrigin.xlFormatFromLeftOrAbove);
                                            }

                                            row++;
                                            for (int rc = 0, cc = rc; rc < colCount; rc++)
                                            {
                                                rng = (Excel.Range)workSheet.Cells[row, cc + col];
                                                //workSheet.Rows.set_Item(row, rc + col, data[n + rc]);
                                                if (rng.MergeCells.ToString().ToLower() == "false")
                                                {
                                                    rng.Value2 = data[n + rc];
                                                    cc++;
                                                }
                                                else
                                                {
                                                    Excel.Range mrng = (Excel.Range)rng.MergeArea;
                                                    mrng.set_Item(1, 1, data[n + rc]);
                                                    cc += mrng.Count;
                                                }

                                            }


                                            n = n + colCount;
                                        }



                                        //(workSheet.Cells[row+1, col] as Excel.Range).EntireRow.Delete(Excel.XlDeleteShiftDirection.xlShiftUp);
                                        //curRow = row;
                                    }
                                }
                            }
                            
                        }

                        m_exWorkBook.RefreshAll();
                        curCol++;


                        #endregion 解析并写入值

                    }

                    curRow++;

                    //小黎 2012-6-15 增加升版后签过名的图片全部去掉
                    if (this.IsUpdateVersion && workSheet.Shapes != null && workSheet.Shapes.Count > 0)
                    {
                        foreach (Excel.Shape shape in workSheet.Shapes)
                        {
                            if (!string.IsNullOrEmpty(shape.AlternativeText))
                            {
                                try
                                {
                                    //小黎 2012-7-27 删除图片后把图片域再写回去，下次再签的时候才可以签上
                                    if (shape.TopLeftCell != null && shape.TopLeftCell.Validation != null)
                                    {
                                        shape.TopLeftCell.Value2 = shape.TopLeftCell.Validation.InputMessage;
                                    }
                                    shape.Delete();
                                }
                                catch (Exception)
                                { }

                            }
                            
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                //2010-04-30
                _errorList.Add(ex.ToString());
                AVEVA.CDMS.Server.CDMS.Write(ex.ToString()); 
            }

            
        }


        private FTPFactory ftp = null; 


        /// <summary>
        /// 下载文件，返回本地文件的路径
        /// </summary>
        /// <param name="proj"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        protected string DownLoadFile(Project proj, string path)
        {

            if (proj == null) return "";
            string zkLocalPath = "";
            try
            {
                if (ftp == null)
                {
                    ftp = new FTPFactory(proj.Storage);
                }
                if (ftp != null)
                {
                    string zkWorkingPath = proj.dBSource.LoginUser.WorkingPath;
                    //小黎 2012-9-7 按照新的标准,把下载的图片都放到c:\temp\CDMSBMP
                    if (path.ToLower().EndsWith(".jpg") || path.ToLower().EndsWith(".bmp") || path.ToLower().EndsWith(".dwg"))
                    {
                        zkLocalPath = "C:\\temp\\CDMSBMP\\" + path;
                    }
                    else
                    {
                        zkLocalPath = zkWorkingPath + "\\" + path;
                    }
                    ftp.download(path, zkLocalPath, false);
                }

            }
            catch (Exception ex)
            {        //2010-04-30
                _errorList.Add(ex.ToString());
                AVEVA.CDMS.Server.CDMS.Write(ex.ToString());
            }
          
            return zkLocalPath;
        }


        #region  将单元格中的域及域的值放过Hashtable中返回
        public Hashtable GetWdFieldValue(string sFileName)
        {

            try
            {
                //判断文件是否存在
                if (File.Exists(sFileName))
                {

                    //先判断要打开的文件是什么类型
                    if (sFileName.ToUpper().IndexOf(".DOC") > 0)
                    {
                        //先判断文件是否已经打开
                        if (!WordIsRunning(m_wdApp, sFileName))
                        {
                            //根据文件类型打开文件
                            OpenWordNew(sFileName);
                        }

                    }

                }
                else
                {
                    _errorList.Add("[" + sFileName + "]不存在!");
                    return null;
                }
            }
            catch (Exception ex)
            {
                //添加错误信息到错误列表中
                _errorList.Add(ex.Message);
               

                //2010-04-30
                AVEVA.CDMS.Server.CDMS.Write(ex.ToString()); 

            }

            if (m_wdDoc == null)
            {
                return null;
            }

            //要返回的哈希表
            Hashtable ht = new Hashtable();


            //遍历所有域
            if (m_wdDoc.Fields != null && m_wdDoc.Fields.Count > 0)
            {
                string key;
                string value;

                foreach (Word.Field field in m_wdDoc.Fields)
                {

                    if (field.Type == Word.WdFieldType.wdFieldAddin)
                    {
                        bool bShowCode = field.ShowCodes;
                        if (!bShowCode)
                            field.ShowCodes = true;

                        string str = field.Result.Cells[1].Range.Text;

                        int index = str.LastIndexOf("ADDIN  \\* MERGEFORMAT");
                        string str1 = "", str2 = "";

                        //域代码前面有信息
                        if (index > 2)
                        {
                            str1 = str.Substring(0, index - 2);
                        }

                        //域代码后面有信息
                        if (index > 0 && index + 25 < str.Length)
                        {
                            str2 = str.Substring(index + 23, str.Length - index - 25);
                        }

                        //得到用户输入的信息
                        str = str1 + str2;


                        //如果自定义域的单元格包含用户输入信息
                        if (!string.IsNullOrEmpty(str.Trim()))
                        {
                            key = field.Data;

                            if (key.StartsWith("$"))
                            {
                                //去掉$()
                                key = key.Substring(2);
                                key = key.Substring(0, key.Length - 1);

                            }

                            value = str;

                            //添加到哈希表中
                            if (!ht.ContainsKey(key))
                            {
                                ht.Add(key, value);

                            }
                        }

                        field.ShowCodes = bShowCode;

                    }

                }

            }

            //关闭Word，释放资源
            Release(_closeApp);

            return ht;
        }
        #endregion



        /// <summary>
        /// 计算word、excel、pdf文件当前尺寸下的打印页数
        /// </summary>
        /// <param name="filepath">文件路径</param>
        /// <returns>文件页数</returns>
        public static int CountFilePage(string filepath)
        {

            if (!File.Exists(filepath))
            {
                //文件不存在
                return 0;
            }

            try
            {
                //文件页数
                int pageCount = 0;

                #region Excel文档
                if (filepath.ToLower().EndsWith(".xls") || filepath.ToLower().EndsWith(".xlsx"))
                {
                    Excel.Application myExcelApp = new Excel.ApplicationClass();

                    object oMissing = System.Reflection.Missing.Value;

                    Excel.Workbook myWorkBook = myExcelApp.Workbooks.Open(filepath,
                                                               oMissing,
                                                               oMissing,
                                                               oMissing,
                                                               oMissing,
                                                               oMissing,
                                                               oMissing,
                                                               oMissing,
                                                               oMissing,
                                                               oMissing,
                                                               oMissing,
                                                               oMissing,
                                                               oMissing,
                                                               oMissing,
                                                               oMissing
                                                           );

                    for (int index = 1; index <= myWorkBook.Sheets.Count; index++)
                    {

                        Excel.Worksheet curSheet = (Excel.Worksheet)(myWorkBook.Sheets[index]);

                        //空白页
                        if (curSheet.UsedRange.Value2 == null)
                            continue;

                        //水平打印页数
                        int hPage = curSheet.HPageBreaks.Count + 1;

                        //垂直打印页数
                        int vPage = curSheet.VPageBreaks.Count + 1;

                        pageCount += hPage * vPage;
                    }

                    myWorkBook.Close(oMissing, oMissing, oMissing);
                    myExcelApp.Quit();

                }

                #endregion

                #region Word文档

                else if (filepath.ToLower().EndsWith(".doc") || filepath.ToLower().EndsWith(".docx"))
                {
                    Word.Application myWordApp = new Word.ApplicationClass();

                    object oMissing = System.Reflection.Missing.Value;

                    object filePath = filepath as object;

                    Word.Document myWordDoc = myWordApp.Documents.Open(
                        ref filePath, ref oMissing, ref oMissing, ref oMissing,
                        ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                        ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                        ref oMissing, ref oMissing, ref oMissing, ref oMissing);


                    pageCount = myWordDoc.ComputeStatistics(Word.WdStatistic.wdStatisticPages, ref oMissing);

                    myWordDoc.Close(ref oMissing, ref oMissing, ref oMissing);
                    myWordApp.Quit(ref oMissing, ref oMissing, ref oMissing);
                }

                #endregion


                #region PDF文档
                else if (filepath.ToLower().EndsWith(".pdf"))
                {
                    FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read);
                    StreamReader r = new StreamReader(fs);
                    string pdfText = r.ReadToEnd();

                    Regex rx = new Regex(@"/Type\s*/Page[^s]");
                    MatchCollection matches = rx.Matches(pdfText);

                    pageCount = matches.Count;

                }
                #endregion

                return pageCount;

            }
            catch (Exception ex)
            {
                //异常处理
                //2010-04-30
                
                AVEVA.CDMS.Server.CDMS.Write(ex.ToString()); 
                return 0;
            }

        }


        /// <summary>
        /// 获取word、Excel的当前张大小
        /// </summary>
        /// <param name="filepath">文件路径</param>
        /// <returns>A3、A4、Custom等（缺省为A4）</returns>
        public static  string GetPageSize(string filepath)
        {
            if (!File.Exists(filepath))
            {
                //文件不存在
                return "";
            }

            try
            {
                //文件纸张大小
                string pageSize = "A4";

                #region Excel文档
                if (filepath.ToLower().EndsWith(".xls") || filepath.ToLower().EndsWith(".xlsx"))
                {
                    Excel.Application myExcelApp = new Excel.ApplicationClass();

                    object oMissing = System.Reflection.Missing.Value;

                    Excel.Workbook myWorkBook = myExcelApp.Workbooks.Open(filepath,
                                                               oMissing,
                                                               oMissing,
                                                               oMissing,
                                                               oMissing,
                                                               oMissing,
                                                               oMissing,
                                                               oMissing,
                                                               oMissing,
                                                               oMissing,
                                                               oMissing,
                                                               oMissing,
                                                               oMissing,
                                                               oMissing,
                                                               oMissing
                                                           );

                    Excel.Worksheet curSheet = myWorkBook.ActiveSheet as Excel.Worksheet;
                    string xlPaperSize = curSheet.PageSetup.PaperSize.ToString();
                    if (xlPaperSize.StartsWith("xlPaper"))
                        pageSize = xlPaperSize.Substring(7);

                    myWorkBook.Close(oMissing, oMissing, oMissing);
                    myExcelApp.Quit();

                }

                #endregion

                #region Word文档

                else if (filepath.ToLower().EndsWith(".doc") || filepath.ToLower().EndsWith(".docx"))
                {
                    Word.Application myWordApp = new Word.ApplicationClass();

                    object oMissing = System.Reflection.Missing.Value;

                    object filePath = filepath as object;

                    Word.Document myWordDoc = myWordApp.Documents.Open(
                        ref filePath, ref oMissing, ref oMissing, ref oMissing,
                        ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                        ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                        ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                    string wdPaperSize = myWordDoc.PageSetup.PaperSize.ToString();
                    if (wdPaperSize.StartsWith("wdPaper"))
                        pageSize = wdPaperSize.Substring(7);

                    myWordDoc.Close(ref oMissing, ref oMissing, ref oMissing);
                    myWordApp.Quit(ref oMissing, ref oMissing, ref oMissing);
                }

                #endregion


                return pageSize;

            }
            catch (Exception ex)
            {
                //异常处理
                //2010-04-30
          
                AVEVA.CDMS.Server.CDMS.Write(ex.ToString()); 
                return "";
            }
        }


        /// <summary>
        /// 设置当前文档对象（word、excel、pdf）的页数和尺寸
        /// </summary>
        /// <param name="d">doc对象</param>
        /// <param name="pagesKeyword">记录页数的模板关键字</param>
        /// <param name="sizeKeyword">记录文件纸张大小的模板关键字</param>
        /// <returns>是否设置成功</returns>
        public static  bool SetFilePagesInfo(Doc d, string pagesKeyword, string sizeKeyword)
        {
            if (d == null || d.O_filename == null)
                return false;

            //没有相关模板
            if (string.IsNullOrEmpty(pagesKeyword) || d.GetAttrDataByKeyWord(pagesKeyword) == null)
                return false;

            FTPFactory ftp = null;
            try
            {
                ftp = d.Storage.FTP ?? new FTPFactory(d.Storage);
                if (ftp == null || !ftp.CheckFileIsExit(d.FullPathFile))
                {
                    return false;
                }


                //把文件下载到本地打开
                string localFilepath = d.dBSource.LoginUser.WorkingPath + d.O_filename;
                ftp.download(d.FullPathFile, localFilepath, false);

                if (File.Exists(localFilepath))
                {

                    List<string> fileInfoList = GetFileSizeAndPages(localFilepath);

                    if (fileInfoList == null || fileInfoList.Count < 2)
                        return false;

                    //纸张大小
                    string pageSize = fileInfoList[0];

                    if (!string.IsNullOrEmpty(sizeKeyword) && !string.IsNullOrEmpty(pageSize))
                    {
                        AttrData sizeAD = d.GetAttrDataByKeyWord(sizeKeyword);
                        if (sizeAD != null)
                        {
                            sizeAD.SetCodeDesc(pageSize);
                        }
                    }

                    //文件页数
                    string filePages = fileInfoList[1];

                    if (!string.IsNullOrEmpty(pagesKeyword) && !string.IsNullOrEmpty(filePages))
                    {
                        AttrData pagesAD = d.GetAttrDataByKeyWord(pagesKeyword);
                        if (pagesAD != null)
                        {
                            pagesAD.SetCodeDesc(filePages);
                        }
                    }


                    d.AttrDataList.SaveData();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                //2010-04-30

                AVEVA.CDMS.Server.CDMS.Write(ex.ToString()); 
                return false;
            }
            finally
            {
                if (ftp != null)
                {
                    ftp.close();
                    ftp = null;
                }
            }
        }

        /// <summary>
        /// 设置当前文档对象（word、excel、pdf）的页数
        /// </summary>
        /// <param name="d">doc对象</param>
        /// <param name="pagesKeyword">记录页数的模板关键字</param>
        /// <returns>是否设置成功</returns>
        public static bool SetFilePagesInfo(Doc d, string pagesKeyword)
        {
            try
            {
                return SetFilePagesInfo(d, pagesKeyword, "");
            }
            catch (Exception ex)
            {
                //2010-04-30
    
                AVEVA.CDMS.Server.CDMS.Write(ex.ToString()); 
                return false;
            }
        }



        /// <summary>
        /// 获取word、Excel、PDF文件的纸张大小及当前大小下的页数
        /// </summary>
        /// <param name="filepath">文件路径</param>
        /// <returns></returns>
        public static  List<string> GetFileSizeAndPages(string filepath)
        {
            if (!File.Exists(filepath))
            {
                //文件不存在
                return null;
            }

            try
            {
                List<string> fileInfoList = new List<string>();

                //文件纸张大小
                string pageSize = "A4";

                //文件页数
                int pageCount = 0;

                #region Excel文档
                if (filepath.ToLower().EndsWith(".xls") || filepath.ToLower().EndsWith(".xlsx"))
                {
                    Excel.Application myExcelApp=null;
                    try
                    {
                        myExcelApp = new Excel.ApplicationClass();

                        object oMissing = System.Reflection.Missing.Value;

                        Excel.Workbook myWorkBook = myExcelApp.Workbooks.Open(filepath,
                                                                   oMissing,
                                                                   oMissing,
                                                                   oMissing,
                                                                   oMissing,
                                                                   oMissing,
                                                                   oMissing,
                                                                   oMissing,
                                                                   oMissing,
                                                                   oMissing,
                                                                   oMissing,
                                                                   oMissing,
                                                                   oMissing,
                                                                   oMissing,
                                                                   oMissing
                                                               );


                        Excel.Worksheet activeSheet = (Excel.Worksheet)(myWorkBook.ActiveSheet);
                        string xlPaperSize = activeSheet.PageSetup.PaperSize.ToString();
                        if (xlPaperSize.StartsWith("xlPaper"))
                            pageSize = xlPaperSize.Substring(7);

                        for (int index = 1; index <= myWorkBook.Sheets.Count; index++)
                        {

                            Excel.Worksheet curSheet = (Excel.Worksheet)(myWorkBook.Sheets[index]);

                            //空白页
                            if (curSheet.UsedRange.Value2 == null)
                                continue;

                            curSheet.Activate();

                            pageCount += int.Parse(myExcelApp.ExecuteExcel4Macro("Get.Document(50)").ToString());

                        }

                        activeSheet.Activate();

                        myWorkBook.Close(oMissing, oMissing, oMissing);
                        myExcelApp.Quit();
                    }
                    catch (Exception e)
                    {
                        if (myExcelApp != null) myExcelApp.Quit();
                        CDMS.Server.CDMS.Write("统计Execl页数出错:" + filepath + " " + e.ToString());
                    }
                }

                #endregion

                #region Word文档

                else if (filepath.ToLower().EndsWith(".doc") || filepath.ToLower().EndsWith(".docx"))
                {
                    Word.Application myWordApp=null;
                    object oMissing = System.Reflection.Missing.Value;
                    try
                    {
                        myWordApp = new Word.ApplicationClass();
                        object filePath = filepath as object;

                        Word.Document myWordDoc = myWordApp.Documents.Open(
                            ref filePath, ref oMissing, ref oMissing, ref oMissing,
                            ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                            ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                            ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                        string wdPaperSize = myWordDoc.PageSetup.PaperSize.ToString();
                        if (wdPaperSize.StartsWith("wdPaper"))
                            pageSize = wdPaperSize.Substring(7);

                        pageCount = myWordDoc.ComputeStatistics(Word.WdStatistic.wdStatisticPages, ref oMissing);

                        myWordDoc.Close(ref oMissing, ref oMissing, ref oMissing);
                        myWordApp.Quit(ref oMissing, ref oMissing, ref oMissing);
                    }
                    catch (Exception e)
                    {
                        if (myWordApp != null) myWordApp.Quit(ref oMissing, ref oMissing, ref oMissing);
                        CDMS.Server.CDMS.Write("统计Word页数出错:" + filepath + " " + e.ToString());
                    }
                }

                #endregion


                #region PDF文档
                else if (filepath.ToLower().EndsWith(".pdf"))
                {
                    FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read);
                    StreamReader r = new StreamReader(fs);
                    string pdfText = r.ReadToEnd();

                    Regex rx = new Regex(@"/Type\s*/Page[^s]");
                    MatchCollection matches = rx.Matches(pdfText);

                    pageCount = matches.Count;

                }
                #endregion

                fileInfoList.Add(pageSize);
                fileInfoList.Add(pageCount.ToString());

                return fileInfoList;
            }
            catch (Exception ex)
            {
                //2010-04-30
      
                AVEVA.CDMS.Server.CDMS.Write(ex.ToString()); 
                return null;
            }


        }

        /// <summary>
        /// 清空Excel中的表达式
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool ClearExcelExpressions(String filePath)
        {
            try
            {

                if (!String.IsNullOrEmpty(filePath) && File.Exists(filePath) && (filePath.ToLower().EndsWith(".xls") || filePath.ToLower().EndsWith(".xlsx")))
                {

                    if (!ExcelIsRunning(m_exApp, filePath))
                    {
                        if (!OpenExcelNew(filePath)) return false; ;

                        ClearExcelFiled(); 

                        SaveExcel();
 
                        return true;
                    }
                    else
                    {
                        _errorList.Add("[" + filePath + "已经打开!]");
                    }
                     
                }
                
            }
            catch (Exception ex)
            {
                AVEVA.CDMS.Server.CDMS.Write(ex.ToString()); 
            }

            return false ; 
        }



        /// <summary>
        /// 清理Excel字段
        /// </summary>
        private void ClearExcelFiled()
        {
            try
            {
                if (m_exWorkBook == null) return;


                Regex r = new Regex("(TEXT|BMP)(:)(.*)");
                foreach (Excel.Worksheet workSheet in m_exWorkBook.Worksheets)
                {
                    Range usedRange = workSheet.UsedRange;
                    foreach (Range range in usedRange)
                    {

                        string val = (range).Value2 as string;
                        

                        #region 解析并写入值

                        //没有表达式
                        if (string.IsNullOrEmpty(val))
                        {
                            continue;
                        }



                        //解析表达式
                        Match m = r.Match(val);
                        if (m == null || !m.Success)
                        {
                            continue;
                        }



                        //没有包含表达式
                        if (/*(val.Contains("$[") || val.Contains("$(")) && */(val.ToUpper().Contains("SELECT ") || val.ToUpper().Contains("TEXT:") || val.ToUpper().Contains("BMP:")))
                        {
                             range.Value2 = ""; 
                        }


              
                        #endregion 解析并写入值

                    }

                    m_exWorkBook.RefreshAll();
                }
            }
            catch (Exception ex)
            {
                //2010-04-30
                _errorList.Add(ex.ToString());
                AVEVA.CDMS.Server.CDMS.Write(ex.ToString());
            }

 
        }


        /// <summary>
        /// 删除指定Excel属性页中的指定行
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="iRow"></param>
        /// <returns></returns>
        public static bool  DeleteExcelRow(Worksheet sheet , int iRow)
        {
            try
            {
                if (sheet != null && sheet.Rows != null && sheet.Rows.Count >= iRow  && sheet.Application != null )
                {


                    Range range = sheet.Cells[iRow, 1] as Range;

                    range.EntireRow.Select(); 
                    
                    Range selection = sheet.Application.Selection as Range;

                    if (selection != null)
                    {
                        selection.Delete(XlDirection.xlUp);

                        return true; 
                    }
 
                }
            }
            catch (Exception ex)
            {
                CDMS.Server.CDMS.Write(ex.ToString()); 
            }

            return false; 
        }


        /// <summary>
        /// 本函数只能处理表达式唯一的情况,且复制数据都仅仅是插入到Excel数据的末端
        /// 当指定数据超出固定行数时,需要通过复制生成新的行,并修改相应的表达式
        /// </summary>
        /// <param name="filePath">excel文件路径</param>
        /// <param name="iNeedRow">所有需要填的数据</param>
        /// <param name="expressionKey">需要填的表达式中的关键字</param>
        /// <param name="copyLeftTop">复制左上角</param>
        /// <param name="copyRightButtom">复制右下角</param>
        /// <param name="iFixRowCount">固定可填的行数</param>
        /// <param name="iCopyRowCount">可复制的区域可容纳的数据行数</param>
        /// <param name="bCopyRowContainFix">可复制区域是否包含了固定可填行</param>
        /// <param name="sumLessThanFixNeedDelete">需要填写数据行数少于固定行数是,如果可复制区域不包含固定区域,是否需要删掉可复制区域</param>
        /// <returns>返回超出了固定可填行数后,可复制区域以及通过可复制区域新增的表达式</returns>
        public static List<String>  ApplyExcelTable(String filePath , int iNeedRow , String expressionKey ,String copyLeftTop ,String copyRightBottom  ,int iFixRowCount , int iCopyRowCount ,bool bCopyRowContainFix  , bool bSumLessThanFixNeedDelete , int iAllRowCount)
        {
            List<String> newExpressionList = null; 
            try
            {
                if(string.IsNullOrEmpty(filePath) || !File.Exists(filePath) || (!filePath.ToLower().EndsWith(".xlsx") && !filePath.ToLower().EndsWith(".xls")))
                    return null ;

                if (string.IsNullOrEmpty(expressionKey) || string.IsNullOrEmpty(copyLeftTop) || string.IsNullOrEmpty(copyRightBottom))
                    return null;


                Excel.Application sApp = new Microsoft.Office.Interop.Excel.Application(); 
                Object sMissingObj = System.Reflection.Missing.Value ;
                Excel.Workbook sWorkBook = sApp.Workbooks.Open(filePath, sMissingObj, sMissingObj, sMissingObj,
                                                sMissingObj, sMissingObj, sMissingObj,
                                                sMissingObj, sMissingObj, sMissingObj, 
                                                sMissingObj, sMissingObj, sMissingObj, 
                                                sMissingObj, sMissingObj);

                if (sWorkBook == null || sWorkBook.Worksheets.Count <=0)
                    return null;

                Excel.Worksheet sWorkSheet = sWorkBook.Worksheets[1] as Excel.Worksheet;


                Excel.Range copyRange = sWorkSheet.get_Range(copyLeftTop, copyRightBottom);

                if (copyRange != null)
                {
                    //当数据少于固定长度时
                    if (iNeedRow <= iFixRowCount)
                    {
                        //判断是否需要删除可复制区域
                        if (!bCopyRowContainFix && bSumLessThanFixNeedDelete)
                        {
                            //删除可复制区域
                            copyRange.Select();
                            copyRange.Delete(Excel.XlDirection.xlUp); 
                        }
                    }
                    else
                    {
                        bool bNeedCopyNew = false; 
                        //需要复制
                        
                        //已经存在了Fix+Copy , 只需增加其它需要的页面便可,如果Fix+Copy的行数已经满足,则不需要增加,只需修改Copy区域相应的表达式
                        if ( !bCopyRowContainFix && iFixRowCount + iCopyRowCount >= iNeedRow)
                        {
                            //修改Copy区域的表达式
                            Range expressionRange = null ; 

                    

                            List<Range> hasValuesRanges = null;
                            hasValuesRanges = GetHasValueRanges(copyRange);

                            if (hasValuesRanges != null && hasValuesRanges.Count > 0)
                            {
                                //遍历获取存在表达式的Cell,并修改表达式
                                foreach (Range range in hasValuesRanges)
                                {
                                    if (range.Value2 != null && range.Value2.ToString().ToUpper().Contains(expressionKey.ToUpper()))
                                    {
                                        expressionRange = range;    
                                        break ; 
                                    }
                                }

                            }

                            if (expressionRange != null)
                            {
                                int i = 2 ;
                                string sOldExpress = expressionRange.Value2.ToString(); 

                                expressionRange.Value2 = sOldExpress.ToUpper().Replace(expressionKey.ToUpper() , expressionKey.ToUpper() + i.ToString());

                                if (newExpressionList == null)
                                    newExpressionList = new List<string>();

                                newExpressionList.Add(expressionKey.ToUpper() + i.ToString()); 

                            }


                        }
                        else 
                        {
                            bNeedCopyNew = true; 
                        }
                        


                        if (bNeedCopyNew == true)
                        {
                            //计算需要Copy的份数,并复制
                            int iNeedCopy = 0 ; 
                            iNeedCopy = GetCopyCount(iNeedRow , iFixRowCount , iCopyRowCount , bCopyRowContainFix) ;
                           

                            if(iNeedCopy >0)
                            {
                                // 复制相应的份数,并修改其中的
                                 List<Range> newAddRanges = new List<Range>() ; 

                                if(!bCopyRowContainFix)
                                    newAddRanges.Add(copyRange) ; 

                                copyRange.Select() ; 
                                //copyRange.Copy(Excel.XlDirection.xlUp) ; 
                                (sApp.Selection as Range).Copy(sMissingObj); 
                                

                                for(int i = 0 ; i<iNeedCopy ; i++)
                                {
                                    //
                                    int allRow = 0 ; 
                                    allRow = sWorkSheet.UsedRange.Rows.Count  ;

                                    //List<Range> tempHasValueRange = null ; 
                                    //tempHasValueRange = GetHasValueRanges(sWorkSheet.Cells); 

                                    //if(tempHasValueRange == null || tempHasValueRange.Count <=0)
                                    //    continue ;



                                    Range curNewRange = null;  //sWorkSheet.Cells[allRow+1 , 1] as Range  ; 

                                    curNewRange = sWorkSheet.Cells[iAllRowCount + 1, 1] as Range; //sWorkSheet.Cells[tempHasValueRange[tempHasValueRange.Count - 1].Row + 1, 1] as Range ; 

                                    if(curNewRange != null )
                                        curNewRange.Select() ; 

                                    sWorkSheet.Paste(sMissingObj , sMissingObj) ; 

                                    if(sApp.Selection != null)
                                    {
                                        newAddRanges.Add(sApp.Selection as Range) ; 
                                    }

                                    iAllRowCount += copyRange.Rows.Count; 


                                }


                                //遍历所有复制的Range,修改表达式
                                if (newAddRanges.Count > 0)
                                {

                                    int iIndex = 2;


                                    foreach (Range newCopyRange in newAddRanges)
                                    {

                                        Range expressionRange = null; 
                                        List<Range> hasValuesRanges = null;
                                        hasValuesRanges = GetHasValueRanges(newCopyRange);

                                        if (hasValuesRanges != null && hasValuesRanges.Count > 0)
                                        {
                                            //遍历获取存在表达式的Cell,并修改表达式
                                            foreach (Range range in hasValuesRanges)
                                            {
                                                if (range.Value2 != null && range.Value2.ToString().ToUpper().Contains(expressionKey.ToUpper()))
                                                {
                                                    expressionRange = range;
                                                    break;
                                                }
                                            }

                                        }

                                        if (expressionRange != null)
                                        {
                                           
                                            string sOldExpress = expressionRange.Value2.ToString();

                                            expressionRange.Value2 = sOldExpress.ToUpper().Replace(expressionKey.ToUpper(), expressionKey.ToUpper() + iIndex.ToString());

                                            if (newExpressionList == null)
                                                newExpressionList = new List<string>();

                                            newExpressionList.Add(expressionKey.ToUpper() + iIndex.ToString());

                                            iIndex++; 

                                        }
                                    }
                                }


 
                            }

                        }
                    }
                }

                sWorkBook.Save();

                sWorkBook.Close(sMissingObj, sMissingObj, sMissingObj);

                sApp.Quit(); 





            }
            catch (Exception ex)
            {
                CDMS.Server.CDMS.Write(ex.ToString()); 
            }


            return newExpressionList; 

    
        
        }


        public static List<Excel.Range> GetHasValueRanges(Excel.Range fromRange)
        {
            
            try
            {
                if (fromRange == null || fromRange.Cells == null || fromRange.Cells.Count <= 0)
                    return null; 

                List<Range> rangeList = new List<Range>();
                object[,] values = null;
                values = (object[,])fromRange.Value2;

                if (values == null)
                    return null  ;

                for (int i = 1; i <= values.GetLength(0); i++)
                {

                    for (int j = 1; j <= values.GetLength(1); j++)
                    {
                        if (values[i, j] != null && !String.IsNullOrEmpty(values[i, j].ToString()))
                        {
                            rangeList.Add(fromRange.Cells[i, j] as Range);
                        }
                    }

                }

                return rangeList; 
            }
            catch (Exception ex)
            {
                CDMS.Server.CDMS.Write(ex.ToString()); 
            }

            return null; 
        }


        /// <summary>
        /// 根据需要填写的Excel
        /// </summary>
        /// <param name="iNeedRow"></param>
        /// <param name="iFixRow"></param>
        /// <param name="iCopyRow"></param>
        /// <param name="bCopyRowContainFix"></param>
        /// <returns></returns>
        public static int  GetCopyCount(int iNeedRow , int iFixRow , int iCopyRow , bool bCopyRowContainFix)
        {
            int iNeedCopy = 0;

            try
            {
                if (iNeedRow <= 0 || iCopyRow <= 0)
                    return 0;

                int leftRow = iNeedRow;

                if (iFixRow >= 0)
                    leftRow = iNeedRow - iFixRow;

                if (!bCopyRowContainFix)
                {
                    leftRow = leftRow - iCopyRow; 
                }

                while (leftRow > 0)
                {
                    leftRow = leftRow - iCopyRow;
                    iNeedCopy++; 
                }

                return iNeedCopy; 

              
            }
            catch { }
            return 0; 

        }


        public static int GetHasValueRowsCount(Excel.Range fromRange)
        {
            int iCount = 0 ; 
            try
            {
                if (fromRange == null || fromRange.Cells == null || fromRange.Cells.Count <= 0)
                    return 0;

                List<Range> rangeList = new List<Range>();
                object[,] values = null;
                values = (object[,])fromRange.Value2;

                if (values == null)
                    return 0;

                for (int i = 1; i <= values.GetLength(0); i++)
                {
                    bool bHasValue = false; 

                    for (int j = 1; j <= values.GetLength(1); j++)
                    {
                        if (values[i, j] != null && !String.IsNullOrEmpty(values[i, j].ToString()))
                        {
                            bHasValue = true; 
                        }
                    }

                    if (bHasValue)
                        iCount++; 

                }

                return iCount ;
            }
            catch (Exception ex)
            {
                CDMS.Server.CDMS.Write(ex.ToString());
            }

            return iCount ;
        }

        /// <summary>
        /// 读取doc文件中特定单元格的值,其中单元格上定义了域 , 域代码为 :   CELLDATA:Keyword 
        /// </summary>
        /// <param name="wordFilePath"></param>
        /// <returns></returns>
        public static Hashtable GetCellDataFromWord(string  wordFilePath)
        {
            Hashtable valuesHash = new Hashtable() ; 
            try
            {
                if(string.IsNullOrEmpty(wordFilePath) || !File.Exists(wordFilePath) || (!wordFilePath.ToLower().EndsWith(".doc") && !wordFilePath.ToLower().EndsWith(".docx")))
                    return null ;

                object sMissing = System.Reflection.Missing.Value ; 

                Word.Application sApp = new Microsoft.Office.Interop.Word.ApplicationClass();
                Word.Document sDocument = null;

                object objFilePath = wordFilePath;

                sApp.Documents.Open(ref objFilePath,
                             ref sMissing,
                             ref sMissing,
                             ref sMissing,
                             ref sMissing,
                             ref sMissing,
                             ref sMissing,
                             ref sMissing,
                             ref sMissing,
                             ref sMissing,
                             ref sMissing,
                             ref sMissing,
                             ref sMissing,
                             ref sMissing,
                             ref sMissing,
                             ref sMissing
                             );

                 sDocument  = sApp.ActiveDocument;

                 if (sDocument == null)
                 {
                     sApp.Quit(ref sMissing, ref sMissing, ref sMissing); 
                     return null;
                 }
                
                //读取所有域数据
                if (sDocument.Fields != null && sDocument.Fields.Count > 0)
                {
                    foreach (Word.Field field in sDocument.Fields)
                    {
                        if (field.Type != Microsoft.Office.Interop.Word.WdFieldType.wdFieldAddin)
                            continue;

                        String sExpression = "";
                        String keyWord = ""; 

                        sExpression = field.Data;

                        if (String.IsNullOrEmpty(sExpression) || !sExpression.ToUpper().StartsWith("CELLDATA:"))
                            continue; 

                        //获取该区域的数据

                        keyWord = sExpression.ToUpper().Replace("CELLDATA:", "");

                        if (String.IsNullOrEmpty(keyWord))
                            continue;


                        try
                        {
                            if (field.Code == null || field.Code.Cells == null || field.Code.Cells.Count <= 0)
                                continue;

                            field.Code.Cells[1].Select();

                            if (!valuesHash.Contains(keyWord))
                                valuesHash.Add(keyWord, sApp.Selection.Text);
                        }
                        catch (Exception subex)
                        {
                            //CDMS.Server.CDMS.Write("sub " + subex.ToString()); 
                        }
                        
                    }
                }


                sDocument.Close(ref sMissing, ref sMissing, ref sMissing);

                sApp.Quit(ref sMissing , ref sMissing , ref sMissing ); 
                

            }
            catch (Exception ex)
            {
                CDMS.Server.CDMS.Write(ex.ToString());
                return null; 
            }
            return valuesHash; 
        }

        /// <summary>
        /// 获取Excel某一行中数值等于value的列索引,用于获取指定属性名对应的列
        /// </summary>
        /// <param name="sWorkSheet"></param>
        /// <param name="iRow"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetColumnIndexByValue(Worksheet sWorkSheet, int iRow, String value)
        {
            int index = 0; 
            try
            {
                if (sWorkSheet == null || iRow <= 0 || String.IsNullOrEmpty(value))
                    return 0;


                object[,] values = null;
                values = (object[,])sWorkSheet.UsedRange.Value2;

                if (values == null)
                    return 0 ;

                
                for (int j = 1; j <= values.GetLength(1); j++)
                {
                    if (values[2, j] != null && values[2, j].ToString() == value )
                    {
                        index = j;
                        break;  
                    }
                }


                return index; 


            }
            catch (Exception ex)
            {
                CDMS.Server.CDMS.Write(ex.ToString()); 
            }
            return 0; 
        }




        /// <summary>
        /// 将Excel中自定义表达式存储到数据验证栏InputMessage
        /// </summary>
        /// <param name="refSheet">Excel.Sheet</param>
        /// <param name="refRow">行</param>
        /// <param name="refCol">列</param>
        /// <param name="refExpression">Excel.Cells[i,j]中的表达式</param>
        /// <returns></returns>
        public bool ExcelFieldSetExpression(Excel.Worksheet refSheet, int refRow, int refCol, string refExpression)
        {
            if (string.IsNullOrEmpty(refExpression))
            {
                return false;
            }
            try
            {
                Range CellRange = ((Excel.Range)refSheet.Cells[refRow, refCol]);
                CellRange.Validation.Add(XlDVType.xlValidateInputOnly, XlDVAlertStyle.xlValidAlertInformation, Type.Missing, Type.Missing, Type.Missing);
                CellRange.Validation.InputMessage = refExpression;
                return true;

            }
            catch (Exception ex)
            {

            }
            return false;
        }



        /// <summary>
        /// 获取存贮在Excel数据验证栏中的InputMessage;
        /// </summary>
        /// <param name="refSheet">Excel.Sheet</param>
        /// <param name="refRow"></param>
        /// <param name="refCol"></param>
        /// <returns>Excel.Cells[i,j]中的表达式</returns>
        public string ExcelFieldGetExpression(Excel.Worksheet refSheet, int refRow, int refCol)
        {
            string ReturnValue = string.Empty;
            try
            {
                Range CellRange = ((Excel.Range)refSheet.Cells[refRow, refCol]);
                ReturnValue = CellRange.Validation.InputMessage;

                return ReturnValue;
            }
            catch (Exception ex)
            {

            }
            return null;
        }


        private void SetFieldInFrontOfText(Microsoft.Office.Interop.Word.Field field)
        {
            try
            {
                if(field == null) return ;

               
                field.InlineShape.Select();

               
                


                return; 

                field.InlineShape.Select(); 


                //field.Select();

                Microsoft.Office.Interop.Word.Selection Selection = this.m_wdApp.Selection;

                if (Selection == null || Selection.ShapeRange == null)
                    return;


                Selection.ShapeRange.Fill.Visible = MsoTriState.msoFalse;
                Selection.ShapeRange.Fill.Solid();
                Selection.ShapeRange.Fill.Transparency = (float)0.0;
                Selection.ShapeRange.Line.Weight = (float)0.75;
                Selection.ShapeRange.Line.DashStyle = MsoLineDashStyle.msoLineSolid;
                Selection.ShapeRange.Line.Style = MsoLineStyle.msoLineSingle;
                Selection.ShapeRange.Line.Transparency = (float)0.0;
                Selection.ShapeRange.Line.Visible = MsoTriState.msoFalse;
                Selection.ShapeRange.LockAspectRatio = MsoTriState.msoTrue;
                //Selection.ShapeRange.Left = 316.9
                //Selection.ShapeRange.Top = 318.85
                Selection.ShapeRange.RelativeHorizontalPosition = Microsoft.Office.Interop.Word.WdRelativeHorizontalPosition.wdRelativeHorizontalPositionColumn;

                Selection.ShapeRange.RelativeVerticalPosition = Microsoft.Office.Interop.Word.WdRelativeVerticalPosition.wdRelativeVerticalPositionParagraph;

                //Selection.ShapeRange.RelativeHorizontalSize = wdRelativeHorizontalSizePage;
                //Selection.ShapeRange.RelativeVerticalSize = wdRelativeVerticalSizePage;
                //Selection.ShapeRange.Left = wdShapeRight;
                //Selection.ShapeRange.LeftRelative = wdShapePositionRelativeNone
                //Selection.ShapeRange.Top = CentimetersToPoints(0.06)
                //Selection.ShapeRange.TopRelative = wdShapePositionRelativeNone
                //Selection.ShapeRange.WidthRelative = wdShapeSizeRelativeNone
                //Selection.ShapeRange.HeightRelative = wdShapeSizeRelativeNone
                Selection.ShapeRange.LockAnchor = 0;
                Selection.ShapeRange.LayoutInCell = 1;
                Selection.ShapeRange.WrapFormat.AllowOverlap = 0;
                Selection.ShapeRange.WrapFormat.Side = Microsoft.Office.Interop.Word.WdWrapSideType.wdWrapBoth;
                //Selection.ShapeRange.WrapFormat.DistanceTop = CentimetersToPoints(0);
                //Selection.ShapeRange.WrapFormat.DistanceBottom = CentimetersToPoints(0);
                //Selection.ShapeRange.WrapFormat.DistanceLeft = CentimetersToPoints(0.32);
                //Selection.ShapeRange.WrapFormat.DistanceRight = CentimetersToPoints(0.32);
                Selection.ShapeRange.WrapFormat.Type = Microsoft.Office.Interop.Word.WdWrapType.wdWrapTopBottom;
                Selection.ShapeRange.ZOrder(MsoZOrderCmd.msoBringInFrontOfText); 


            }catch(Exception ex)
            {
                CDMS.Server.CDMS.Write(ex.ToString()); 
            }
        }


        //小黎 2012-4-27 重写签名方法
        /// <summary>
        /// 重写签名程序,将数据更新与签名放到一起.使以后可以调用更新或者调用签名都只用这个方法
        /// </summary>
        /// <param name="oProjectOrDoc">传进来的Doc或Project</param>
        /// <param name="sFileName">下载到本地的模板文件</param>
        /// <param name="htUserKeyWord">用户自定义的关键字</param>
        /// <param name="IsDebug">是否生成调试日志</param>
        /// <returns></returns>
        public void WriteAndUpdateDataToDocument(object oProjectOrDoc, string sFileName, Hashtable htUserKeyWord,bool IsDebug)
        {
#region 判断是否符合签名的条件,签名条件为: oProjectOrDoc为Doc或Project,sFileName必须已下载好至本地
            if (IsDebug)
            {
                string Log = "";
                Log = "签名程序调试信息:\r\n";
                if (oProjectOrDoc is Project)
                {
                    Log += "传进来参数(oProjectOrDoc)为Project: Project.ToString = " + ((Project)oProjectOrDoc).ToString + "\r\n";
                }
                else if (oProjectOrDoc is Doc)
                {
                    Log += "传进来参数(oProjectOrDoc)为Doc: Doc.ToString = " + ((Doc)oProjectOrDoc).ToString + "\r\n";
                }
                else
                {
                    Log += "传进来参数(oProjectOrDoc)不能识别为Project或Doc\r\n";
                }

                Log += "sFileName = " + sFileName + "\r\n";

                Log += "HtUserKeyWord 对应信息:\r\n";
                if (htUserKeyWord != null && htUserKeyWord.Count > 0)
                {
                    foreach (DictionaryEntry de in htUserKeyWord)
                    {
                        Log += "Key: " + de.Key.ToString() + "\t\t " + de.Value.ToString() + "\r\n";
                    }
                }

                WriteLog(Log);
            }

            if ( !(oProjectOrDoc is Project) &&  !(oProjectOrDoc is Doc) )
            {
                return ;
            }


            if ( !File.Exists(sFileName))
            {
                if ( IsDebug )
                {
                   WriteLog("文件: " + sFileName + "不存在!");
                }
                return ;
            }
#endregion

            //先判断要打开的文件是什么类型
            if (sFileName.ToUpper().IndexOf(".DOC") > 0)
            {
                //先判断文件是否已经打开
                if (!WordIsRunning(m_wdApp, sFileName))
                {
                    //根据文件类型打开文件
                    if (!OpenWordNew(sFileName))
                    {
                        if (IsDebug)
                        {
                            WriteLog("OpenWordNew返回值为False");
                        }
                        return;
                    }

                    
#region 扫描文件中所有的域
                    List<Word.Field> FieldList;

                    FieldList = ScanFieldsInWord(IsDebug);
                    
#endregion


#region 处理所有的域
                    UpdateWordFields(FieldList, oProjectOrDoc, htUserKeyWord, IsDebug);
#endregion

                    #region 表格压缩,按doe的要求,把表格里面的文字进行压缩
                    FitWordTable();
                    #endregion

                    //完成操作后保存文件
                    SaveWord(sFileName as object);
                }
                else
                {
                    if (IsDebug)
                    {
                        WriteLog("[" + sFileName + "已经打开!]");
                    }
                }

            }
            else if (sFileName.ToUpper().IndexOf(".XLS") > 0)
            {
                //先判断文件是否已经打开
                if (!ExcelIsRunning(m_exApp, sFileName))
                {

                    //打开文件
                    if (!OpenExcelNew(sFileName)) return ;
                    
                    //扫描excel中的表达式并为其赋值
                    ParseExcelFieldEx(oProjectOrDoc, htUserKeyWord, new Hashtable());

                    //关闭文档
                    SaveExcel();
                }
                else
                {
                    if (IsDebug)
                    {
                        WriteLog("[" + sFileName + "已经打开!]");
                    }
                }
            }

            //小黎 2012-9-7 按照新的标准,把下载的图片都放到c:\temp\CDMSBMP
            try
            {
                if (Directory.Exists("C:\\temp\\CDMSBMP\\"))
                {
                    Directory.Delete("C:\\temp\\CDMSBMP\\", true);
                }
            }
            catch (System.Exception ex)
            {
                AVEVA.CDMS.Server.CDMS.Write(ex.ToString());
            }
        }

        /// <summary>
        /// 处理所有的域,要求能更新和第一次写入
        /// </summary>
        /// <param name="MainDocFields">正文的域</param>
        /// <param name="HeaderFields">页眉的域</param>
        /// <param name="FootFields">页脚的域</param>
        /// <param name="IsDebug">是否生成调试信息</param>
        private void UpdateWordFields(List<Word.Field> FieldList, object oProjectOrDoc,Hashtable htUserKeyWord,bool IsDebug)
        {
            //处理所有的域
            foreach (Word.Field field in FieldList)
            {
                try
                {
                    FillWordFieldNew(field, oProjectOrDoc, htUserKeyWord, IsDebug);
                }
                catch (Exception ex)
                {
                    WriteLog(ex.ToString());
                }
            }
        }


        /// <summary>
        /// 根据域的类型去填写
        /// </summary>
        /// <param name="field">Word的域</param>
        /// <param name="IsDebug">是否生成调试信息</param>
        private void FillWordFieldNew(Word.Field field,object oProjectOrDoc,Hashtable htUserKeyWord, bool IsDebug)
        {
            field.Locked = false;
            switch (field.Type)
            {
                case Microsoft.Office.Interop.Word.WdFieldType.wdFieldAddin:                       //类型为自定义域
                    {
                        string ExpressionFull = field.Data;   //域表达式
                        string Expression = "";               //需解析的表达式
                        int length = -1;                      //已签过名的文字的长度
                        bool IsFinish = false;                //代表TableImage图多值是否需要更新


                        if (ExpressionFull.Contains("@@@"))   //代表已经操作过该域
                        {
                            string[] arrayStr = ExpressionFull.Split("@@@".ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                            Expression = arrayStr[0];
                            for (int i = 1; i < arrayStr.Length; i++)
                            {
                                string temp = arrayStr[i];
                                if (temp.Contains("Length:")) //如果有这个参数，说明已经被签过一次名,length为以前签署过的文字的长度
                                {
                                    length = Convert.ToInt32(temp.Substring(7));
                                }

                                if (temp.Contains("ImageFinish"))
                                {
                                    IsFinish = true;
                                }
                            }
                        }
                        else                                  //未签过
                        {
                            Expression = ExpressionFull ;
                        }
                
                        if (string.IsNullOrEmpty(Expression))
                            return;



                        if (Expression.Contains(":"))
                        {
                            //TODO:处理多值域
                            string[] Commands = Expression.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            string Command = Commands[0];

                            //处理图片表格,格式为IMAGETABLE:xx:yy:zz
                            //其中IMAGETABLE为固定关键字,xx为要解析的关键字,yy为要贴图片的行数,zz为每行的单元格数
                            if (Command == "IMAGETABLE")
                            {
                                FillImageTableField(field, oProjectOrDoc, htUserKeyWord, Commands,IsFinish, IsDebug);                                 
                            }

                            if (Command == "TEXTTABLE")
                            {
                                FillTextTableField(field, oProjectOrDoc, htUserKeyWord, Commands, length, IsDebug); 
                            }

                            //小黎 2012-6-11 增加多值图的处理方案,主要是用于不用通过插件就能实现签名
                            //格式为:MULTIPICS:xx:yy: 
                            //MULTIPICS为固定关键字,xx为方向,yy为需要解析的关键字(需要加$号)
                            //方向只支持RIGHT和DOWN
                            //例:MULTIPICS:RIGHT:$(TEST)
                            if (Command == "MULTIPICS")
                            {
                                FillMultiPicField(field, oProjectOrDoc, htUserKeyWord, Commands, length, IsDebug);
                            }
 

                        }
                        else
                        {
                            //TODO:处理单值域
                            FillSingleValueField(field, oProjectOrDoc, htUserKeyWord, Expression, length, IsDebug);
                        }
                    }
                    break;
                case Microsoft.Office.Interop.Word.WdFieldType.wdFieldIncludePicture:              //类型为图片域
                    {
                        const string BLANKJPG = "blank.jpg";
                        string FieldCode = field.Code.Text;

                        if ( !FieldCode.Contains("INCLUDEPICTURE") )
                        {
                            return ;
                        }
                        int index = FieldCode.IndexOf("INCLUDEPICTURE") + 14;
                        string Expression  = FieldCode.Substring(index, FieldCode.LastIndexOf("\\") - index);
                        if ( string.IsNullOrEmpty(Expression))
                            return;

                        string PicFullPath = GetPictureNew(oProjectOrDoc, htUserKeyWord, Expression, IsDebug);
                        if (string.IsNullOrEmpty(PicFullPath))
                        {
                            return;
                        }


                        //带有@@@Pic:表示已经签过一次名,@@@Pic:后面为签过名的图片
                        string OrignalFieldCode = FieldCode;

                        //小黎 2012-6-15 增加升版的支持,升版时默认用空白图片代替所有的签名图片 增加&& !this.IsUpdateVersion
                        if (PicFullPath.Contains(BLANKJPG) && OrignalFieldCode.Contains("@@@Pic:") && !this.IsUpdateVersion)  //如果返回的结果为空图片,且原来也有签过名,则不更新图片
                        {
                            return;
                        }

                        if (OrignalFieldCode.Contains("@@@Pic:"))
                        {
                             OrignalFieldCode = FieldCode.Substring(0, FieldCode.LastIndexOf("@@@Pic:"));
                        }                     
                        //插入图片,在域里面的InlineShape插入图片
                        field.InlineShape.LinkFormat.SourceFullName = PicFullPath;
                        field.InlineShape.LinkFormat.SavePictureWithDocument = true;
                        //插入图片后会把域的代码改变,所以要在插入图片后写回原来的值
                        field.Code.Text = OrignalFieldCode + "@@@Pic:" + field.InlineShape.LinkFormat.SourceName;

                    }
                    break;
                default:
                    break;
                   
            }
            
        }


        /// <summary>
        /// 处理图片表格域(ADDIN)
        /// </summary>
        /// <param name="field"></param>
        /// <param name="oProjectOrDoc"></param>
        /// <param name="htUserKeyWord"></param>
        /// <param name="Expression"></param>
        /// <param name="length"></param>
        /// <param name="IsDebug"></param>
        private void FillImageTableField(Word.Field field, object oProjectOrDoc, Hashtable htUserKeyWord, string[] Commands,bool IsFinish, bool IsDebug)
        {
            if (Commands.Length != 4)
            {
                if (IsDebug)
                {
                    WriteLog(field.Data + "关键字不符合IMAGETABLE约定,不再该域签名");
                }
                return;
            }


            string Keyword = Commands[1];
            int RowCount = Convert.ToInt32(Commands[2]);
            int ColumnCount = Convert.ToInt32(Commands[3]);

            //已经写入过一次,现在需要更新,先判断ht里面是否有该关键字的信息,若有才更新(先删除旧的图片,再插入新的图片)
            if (IsFinish)
            {
                if (htUserKeyWord.Contains(Keyword))
                {
                    field.Select();

                    for (int i = 1; i <= RowCount; i++)
                    {
                        for (int j = 1; j <= ColumnCount; j++)
                        {
                            if (field.Application.Selection.Tables[1].Rows[i].Cells[j].Range.InlineShapes.Count > 0)
                            {
                                field.Application.Selection.Tables[1].Rows[i].Cells[j].Range.InlineShapes[1].Delete();
                            }
                        }
                    }
                }
            }


            if (htUserKeyWord == null || !htUserKeyWord.Contains(Keyword))
            {
                if (IsDebug)
                {
                    WriteLog(field.Data + "关键字解析不成功,htUserKeyWord没有该关键字的信息");
                }
                return;
            }

            string ResultTexts = htUserKeyWord[Keyword].ToString();

            if (string.IsNullOrEmpty(ResultTexts))
            {
                if (IsDebug)
                {
                    WriteLog(field.Data + "解析后的值为空");
                }
                return;
            }

            if (IsDebug)
            {
                WriteLog(field.Data + "解析后的值为 " + ResultTexts);
            }

            string[] Images = ResultTexts.Split("|||".ToCharArray(),StringSplitOptions.RemoveEmptyEntries);

            //工作目录
            string LocaltempDir = "";
            if (oProjectOrDoc is Project)
            {
                if ((oProjectOrDoc as Project).dBSource.LoginUser.WorkingPath != null)
                {
                    LocaltempDir = (oProjectOrDoc as Project).dBSource.LoginUser.WorkingPath;
                }
            }
            else if (oProjectOrDoc is Doc)
            {
                if ((oProjectOrDoc as Doc).dBSource.LoginUser.WorkingPath != null)
                {
                    LocaltempDir = (oProjectOrDoc as Doc).dBSource.LoginUser.WorkingPath;
                }
            }
            //小黎 2012-9-7 按照新的标准,把下载的图片都放到c:\temp\CDMSBMP
            LocaltempDir  = "C:\\temp\\CDMSBMP\\";

            string Blank = LocaltempDir + "blank.jpg";
            DownLoadFile(oProjectOrDoc, "\\BMP\\blank.jpg", Blank);

            Queue<string> ImageList = new Queue<string>();
            foreach (string str in Images)
            {
                string PicLocalPath = LocaltempDir + str;
                DownLoadFile(oProjectOrDoc, "\\BMP\\" + str, PicLocalPath);
                if (File.Exists(PicLocalPath))
                {
                    if (IsDebug)
                    {
                        WriteLog(field.Data + " 下载图片: " + str + "成功");
                    }
                    ImageList.Enqueue(PicLocalPath);
                }
                else
                {
                    if (IsDebug)
                    {
                        WriteLog(field.Data + " 下载图片: " + str + "失败");
                    }
                    ImageList.Enqueue(Blank);
                }
            }

            field.Select();

            for (int i = 1; i <= RowCount; i++)
            {
                for (int j = 1; j <= ColumnCount; j++)
                {
                    Word.Range TempRange = field.Application.Selection.Tables[1].Rows[i].Cells[j].Range;
                    object obj = TempRange;
                    if (ImageList.Count > 0)
                    {
                        field.Application.Selection.InlineShapes.AddPicture(ImageList.Dequeue(), ref m_oMissing, ref m_oMissing, ref obj);
                    }
                }
            }

            if ( !field.Data.Contains("@@@ImageFinish") )
            {
                 field.Data += "@@@ImageFinish";
            }


        }

        /// <summary>
        /// 处理多值的图片域
        /// </summary>
        /// <param name="field"></param>
        /// <param name="oProjectOrDoc"></param>
        /// <param name="htUserKeyWord"></param>
        /// <param name="Commands"></param>
        /// <param name="IsFinish"></param>
        /// <param name="IsDebug"></param>
        private void FillMultiPicField(Word.Field field, object oProjectOrDoc, Hashtable htUserKeyWord, string[] Commands, int Length, bool IsDebug)
        {
            if (Commands.Length != 3)
            {
                if (IsDebug)
                {
                    WriteLog(field.Data + "关键字不符合IMAGETABLE约定,不再该域签名");
                }
                return;
            }

            string Direction = Commands[1];
            string Keyword = Commands[2];

            string[] ImageList = this.GetPictureExNew(oProjectOrDoc, htUserKeyWord, Keyword, IsDebug);
            field.ShowCodes = true;//先显示其代码,要不然位置会去到域的左边
            field.Select();
            object unit = Word.WdUnits.wdCharacter;
            object count = 1;
            int _length = 0;  //lengh代表签署的Range,每一张图片或换行也作为一个长度


            //先清空当前域之前所写过的图片
            if (Length != -1)
            {
                m_wdApp.Selection.MoveRight(ref unit, ref count, ref m_oMissing);
                for (int i = 0; i < Length; i++)
                {
                    m_wdApp.Selection.Range.Delete(ref unit, ref count);
                }
                //m_wdApp.Selection.Range.Delete(ref unit,ref TLength); //奇怪了,上面的写法和注释这句的作法效果不一样
                field.Select();
            }



            switch (Direction.ToUpper())
            {
                case "UP":
                    //TODO:no sense
                    break;
                case "LEFT":
                    //TODO:no sense
                    break;
                case "RIGHT":
                default:
                    m_wdApp.Selection.MoveRight(ref unit, ref count, ref m_oMissing);
                    foreach (string image in ImageList)
                    {
                        m_wdApp.Selection.InlineShapes.AddPicture(image, ref m_oMissing, ref m_oMissing, ref m_oMissing);
                        _length++;
                    }
                    break;
                case "DOWN":
                    m_wdApp.Selection.MoveRight(ref unit, ref count, ref m_oMissing);
                    for (int i = 0; i < ImageList.Length;i++ )
                    {
                        if (i != 0)
                        {
                            m_wdApp.Selection.Range.Text = "\r\n";
                            m_wdApp.Selection.MoveRight(ref unit, ref count, ref m_oMissing);
                            _length++;
                        }
                        m_wdApp.Selection.InlineShapes.AddPicture(ImageList[i], ref m_oMissing, ref m_oMissing, ref m_oMissing);
                        _length++;
                    }
                    break;
            }
            field.ShowCodes = false;

            if (field.Data.Contains("@@@Length:"))
            {
                field.Data = field.Data.Substring(0, field.Data.IndexOf("@@@Length:"));
            }
            field.Data += "@@@Length:" + _length;
        }


        /// <summary>
        /// 处理文字表格域(ADDIN)
        /// </summary>
        /// <param name="field"></param>
        /// <param name="oProjectOrDoc"></param>
        /// <param name="htUserKeyWord"></param>
        /// <param name="Expression"></param>
        /// <param name="length"></param>
        /// <param name="IsDebug"></param>
        private void FillTextTableField(Word.Field field, object oProjectOrDoc, Hashtable htUserKeyWord, string[] Commands, int Length, bool IsDebug)
        {
            if (Commands.Length != 4)
            {
                if (IsDebug)
                {
                    WriteLog(field.Data + "关键字不符合IMAGETABLE约定,不再该域签名");
                }
                return;
            }


            string Keyword = Commands[1];
            int RowCount = Convert.ToInt32(Commands[2]);
            int ColumnCount = Convert.ToInt32(Commands[3]);

            //小黎 2012-6-28 增加查找域的位置,从域的位置再往下签文字
            int FieldRowIndex = 1;
            int FieldColumnIndex = 1;
            field.Select();
            for (int i = 1; i <= field.Application.Selection.Tables[1].Rows.Count; i++)   //RowCount改成表格的行
            {
                for (int j = 1; j <= field.Application.Selection.Tables[1].Columns.Count; j++)//ColumnCount改成表格的列
                {
                    try
                    {
                        if (field.Application.Selection.Tables[1].Cell(i, j).Range.Fields.Count > 0 && field.Application.Selection.Tables[1].Cell(i, j).Range.Fields[1].Data == field.Data)
                        {
                            FieldRowIndex = i;
                            FieldColumnIndex = j;
                            break;
                        }
                    }catch(Exception e)
                    {
                        //关于一些合并单元格的的表格，有些单元格是不存在的
                    }
                }
            }

            //已经写入过一次,现在需要更新,先判断ht里面是否有该关键字的信息,若有才更新(先删除旧的文字,再插入新的文字)
            if ( Length != -1 )
            {
                if (htUserKeyWord.Contains(Keyword))
                {
                    field.Select();

                    for (int i = FieldRowIndex; i <= RowCount; i++)
                    {
                        for (int j = 1; j <= ColumnCount; j++)
                        {
                            //对于第1个域要特殊处理
                            if (i == FieldRowIndex && j == FieldColumnIndex)
                            {
                                Word.Range range = field.Result;
                                field.Result.Select();
                                range.End = range.Start + Length;
                                range.Text = "";
                                continue;
                            }
                            field.Application.Selection.Tables[1].Cell(i,j).Range.Text = "";
                        }
                    }
                }
            }


            if (htUserKeyWord == null || !htUserKeyWord.Contains(Keyword))
            {
                if (IsDebug)
                {
                    WriteLog(field.Data + "关键字解析不成功,htUserKeyWord没有该关键字的信息");
                }
                return;
            }

            string ResultTexts = htUserKeyWord[Keyword].ToString();

            if (IsDebug)
            {
                WriteLog(field.Data + "解析后的值为 " + ResultTexts);
            }

            string[] ArrayText = ResultTexts.Split("|||".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            //工作目录
            string LocaltempDir = "";
            if (oProjectOrDoc is Project)
            {
                if ((oProjectOrDoc as Project).dBSource.LoginUser.WorkingPath != null)
                {
                    LocaltempDir = (oProjectOrDoc as Project).dBSource.LoginUser.WorkingPath;
                }
            }
            else if (oProjectOrDoc is Doc)
            {
                if ((oProjectOrDoc as Doc).dBSource.LoginUser.WorkingPath != null)
                {
                    LocaltempDir = (oProjectOrDoc as Doc).dBSource.LoginUser.WorkingPath;
                }
            }

            Queue<string> QueueList = new Queue<string>();
            foreach (string str in ArrayText)
            {
                QueueList.Enqueue(str);
            }

            field.Select();
            int length = -1;


            for (int i = FieldRowIndex; i <= RowCount; i++)
            {
                for (int j = 1; j <= ColumnCount; j++)
                {
                    //域的位置要特殊处理
                    if (i == FieldRowIndex && j == FieldColumnIndex)
                    {
                        string temp = QueueList.Dequeue();
                        field.Result.Text = temp;
                        length = temp.Length;
                        continue;
                    }

                    if (QueueList.Count > 0)
                    {
                        field.Application.Selection.Tables[1].Cell(i,j).Range.Text = QueueList.Dequeue();
                    }
                    else
                    {
                        field.Application.Selection.Tables[1].Cell(i, j).Range.Text = "";
                    }
                }
            }

            if ( field.Data.Contains("@@@Length:"))
            {
                field.Data = field.Data.Substring(0, field.Data.IndexOf("@@@Length:"));
            }
            field.Data += "@@@Length:" + length;

        }

        /// <summary>
        /// 处理Word的单值域(ADDIN)
        /// </summary>
        /// <param name="field"></param>
        /// <param name="oProjectOrDoc"></param>
        /// <param name="htUserKeyWord"></param>
        /// <param name="Expression"></param>
        /// <param name="length"></param>
        /// <param name="IsDebug"></param>
        private void FillSingleValueField(Word.Field field, object oProjectOrDoc, Hashtable htUserKeyWord,string Expression,int length, bool IsDebug)
        {
            string Result = GetExpressValue(oProjectOrDoc, htUserKeyWord, Expression);

            //小黎 2012-5-14 增加用户没传进来值,就会清空域
            if ( string.IsNullOrEmpty(Result) && Expression.Contains("$(") && Expression.Contains(")") )
            {
                string keyword = Expression.Substring(Expression.IndexOf("$(") + 2, Expression.LastIndexOf(")") - Expression.IndexOf("$(")-2);
                if (htUserKeyWord == null || !htUserKeyWord.Contains(keyword))
                {
                    if (IsDebug)
                    {
                        string Log = "Addin域: " + Expression + " 解析后值为: " + Result + "\r\n用户没有传进来关键字,不执行更新该域";
                        WriteLog(Log);
                    }
                    return;
                }
            }


            if (IsDebug)
            {
                string Log = "Addin域: " + Expression + " 解析后值为: " + Result;
                WriteLog(Log);
            }

            //若以前签过一次名，则现在需要更新
            if (length != -1)
            {
                Word.Range range = field.Result;
                field.Result.Select();
                range.End = range.Start + length;
                range.Text = "";
            }
            field.Result.Text = Result;
            field.Data = Expression + "@@@Length:" + Result.Length.ToString(); //记录所签文字的长度 
        }




        /// <summary>
        /// 返回下载好的图片地址
        /// </summary>
        /// <param name="oProjectOrDoc"></param>
        /// <param name="htUserKeyWord"></param>
        /// <param name="Expression">表达式</param>
        /// <returns></returns>
        private string GetPictureNew(object oProjectOrDoc, Hashtable htUserKeyWord, string Expression,bool IsDebug)
        {
            //下载好图片后的本地全路径
            string PicLocalFullPath = "";
            //解析的结果
            string Result = "";
            //工作目录
            string LocaltempDir = "";
            if (oProjectOrDoc is Project)
            {
                if ((oProjectOrDoc as Project).dBSource.LoginUser.WorkingPath != null)
                {
                    LocaltempDir = (oProjectOrDoc as Project).dBSource.LoginUser.WorkingPath;
                }
            }
            else if (oProjectOrDoc is Doc)
            {
                if ((oProjectOrDoc as Doc).dBSource.LoginUser.WorkingPath != null)
                {
                    LocaltempDir = (oProjectOrDoc as Doc).dBSource.LoginUser.WorkingPath;
                }
            }

            //小黎 2012-9-7 按照新的标准,把下载的图片都放到c:\temp\CDMSBMP
            LocaltempDir = "C:\\temp\\CDMSBMP\\";

            //小黎 2012-6-15 增加升版的支持,升版时默认用空白图片代替所有的签名图片
            if (this.IsUpdateVersion)
            {
                DownLoadFile(oProjectOrDoc, "\\BMP\\" + "blank.jpg", LocaltempDir + "blank.jpg");
                if (IsDebug)
                {
                    string Log = "IncludePic域: " + Expression + " 目前为升版状态,用空白图片代替";
                    WriteLog(Log);
                }
                return LocaltempDir + "blank.jpg";
            }

            Result = GetExpressValue(oProjectOrDoc, htUserKeyWord, Expression);
            if (Result.ToLower() == ".jpg")
            {
                Result = "blank.jpg";
            }

            PicLocalFullPath = LocaltempDir + Result; 
            DownLoadFile(oProjectOrDoc, "\\BMP\\" + Result, PicLocalFullPath);

            if (IsDebug)
            {
                string Log = "IncludePic域: " + Expression + " 解析后值为: " + Result;
                WriteLog(Log);
            }


            if (!File.Exists(PicLocalFullPath))
            {
                if (IsDebug)
                {
                    string Log = "IncludePic域: " + Expression + " 解析后值为: " + Result + " 文件下载失败: " + PicLocalFullPath + "\r\n现已改成用blank.jpg代替";
                    WriteLog(Log);
                }

                PicLocalFullPath = LocaltempDir + "blank.jpg";
                DownLoadFile(oProjectOrDoc, "\\BMP\\" + "blank.jpg", PicLocalFullPath);
                return PicLocalFullPath;
            }

            return PicLocalFullPath;
        }

        /// <summary>
        /// 返回下载好的图片地址,新方法只适用于多值图片域MULTIPICS格式
        /// </summary>
        /// <param name="oProjectOrDoc"></param>
        /// <param name="htUserKeyWord"></param>
        /// <param name="Expression">表达式</param>
        /// <returns></returns>
        private string[] GetPictureExNew(object oProjectOrDoc, Hashtable htUserKeyWord, string Expression, bool IsDebug)
        {
            //下载好图片后的本地全路径
            string[] PicLocalPathList ;
            //解析的结果
            string Result = "";
            //工作目录
            string LocaltempDir = "";
            if (oProjectOrDoc is Project)
            {
                if ((oProjectOrDoc as Project).dBSource.LoginUser.WorkingPath != null)
                {
                    LocaltempDir = (oProjectOrDoc as Project).dBSource.LoginUser.WorkingPath;
                }
            }
            else if (oProjectOrDoc is Doc)
            {
                if ((oProjectOrDoc as Doc).dBSource.LoginUser.WorkingPath != null)
                {
                    LocaltempDir = (oProjectOrDoc as Doc).dBSource.LoginUser.WorkingPath;
                }
            }
            //小黎 2012-9-7 按照新的标准,把下载的图片都放到c:\temp\CDMSBMP
            LocaltempDir = "C:\\temp\\CDMSBMP\\";

            Result = GetExpressValue(oProjectOrDoc, htUserKeyWord, Expression);
            if (Result.ToLower() == ".jpg")
            {
                Result = "blank.jpg";
            }

            if (Result.Contains(",") && Result.Contains(".jpg"))
            {
                if (IsDebug)
                {
                    string Log = "多值图: " + Expression + " 解析后值为: " + Result;
                    WriteLog(Log);
                }
                string temp = Result.Substring(0, Result.LastIndexOf(".jpg"));
                string[] pics = temp.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                PicLocalPathList = new string[pics.Length];
                for (int i = 0; i < pics.Length; i++)
                {
                    PicLocalPathList[i] = LocaltempDir + pics[i]+".jpg";
                    DownLoadFile(oProjectOrDoc, "\\BMP\\" + pics[i] + ".jpg", PicLocalPathList[i]);

                    if (!File.Exists(PicLocalPathList[i]))
                    {
                        if (IsDebug)
                        {
                            string Log = "多值图: " + Expression + " 解析后值为: " + pics[i] + ".jpg" + " 文件下载失败: " + PicLocalPathList[i] + "\r\n现已改成用blank.jpg代替";
                            WriteLog(Log);
                        }

                        PicLocalPathList[i] = LocaltempDir + "blank.jpg";
                        DownLoadFile(oProjectOrDoc, "\\BMP\\" + "blank.jpg", PicLocalPathList[i]);
                    }
                }
                return PicLocalPathList;
            }
            else
            {
                PicLocalPathList = new string[1];
                PicLocalPathList[0] = LocaltempDir + Result;
                DownLoadFile(oProjectOrDoc, "\\BMP\\" + Result, PicLocalPathList[0]);

                if (IsDebug)
                {
                    string Log = "IncludePic域: " + Expression + " 解析后值为: " + Result;
                    WriteLog(Log);
                }


                if (!File.Exists(PicLocalPathList[0]))
                {
                    if (IsDebug)
                    {
                        string Log = "IncludePic域: " + Expression + " 解析后值为: " + Result + " 文件下载失败: " + PicLocalPathList[0] + "\r\n现已改成用blank.jpg代替";
                        WriteLog(Log);
                    }

                    PicLocalPathList[0] = LocaltempDir + "blank.jpg";
                    DownLoadFile(oProjectOrDoc, "\\BMP\\" + "blank.jpg", PicLocalPathList[0]);
                    return PicLocalPathList;
                }
            }

            return PicLocalPathList;
        }


        /// <summary>
        /// 扫描Word中所有的域，并分别启示录在MainDocFields,HeaderFields,FootFields
        /// </summary>
        /// <param name="MainDocFields">正文的域</param>
        /// <param name="HeaderFields">页眉的域</param>
        /// <param name="FootFields">页脚的域</param>
        /// <param name="IsDebug">是否生成日志</param>
        private List<Word.Field>  ScanFieldsInWord( bool IsDebug)
        {
            List<Word.Field> FieldList = new List<Microsoft.Office.Interop.Word.Field>();
            string Log = "";
            if (this.m_wdDoc == null)
            {
                if (IsDebug)
                {
                    Log += ("m_wdDoc为空\r\n");
                }
                return FieldList;
            }

            #region 正文的域
            if (IsDebug)
            {
                Log += "正文的域数量为: " + this.m_wdDoc.Fields.Count + "个\r\n由以下域组成:\r\n";
            }
            foreach (Word.Field field in this.m_wdDoc.Fields)
            {
                if (IsDebug)
                {
                    if (field.Type == Microsoft.Office.Interop.Word.WdFieldType.wdFieldAddin)
                    {
                        Log += "域类型为: " + field.Type.ToString() + "\t\t域内容: " + field.Data + "\r\n";
                    }
                    else
                    {
                        Log += "域类型为: " + field.Type.ToString() + "\t\t域内容: " + field.Code.Text + "\r\n"; 
                    }
                }
                FieldList.Add(field);
            }

            //正文的shapes里面的域
            Word.TextFrame wdTextFram;
            Word.Range wdTextRange;
            foreach (Word.Shape shape in m_wdDoc.Shapes)
            {
                if (shape.AutoShapeType == MsoAutoShapeType.msoShapeRectangle)
                {
                    shape.Select(ref m_oMissing);
                    if (shape.TextFrame == null) continue;
                    if (shape.TextFrame.TextRange == null) continue;
                    wdTextFram = shape.TextFrame;
                    if (wdTextFram.TextRange != null)
                    {
                        wdTextRange = wdTextFram.TextRange;
                        if (IsDebug)
                        {
                            Log += "正文Shapes对象的域数量为: " + wdTextRange.Fields.Count + "个\r\n由以下域组成:\r\n";
                        }
                        foreach (Word.Field field in wdTextRange.Fields)
                        {
                            if (IsDebug)
                            {
                                if (field.Type == Microsoft.Office.Interop.Word.WdFieldType.wdFieldAddin)
                                {
                                    Log += "域类型为: " + field.Type.ToString() + "\t\t域内容: " + field.Data + "\r\n";
                                }
                                else
                                {
                                    Log += "域类型为: " + field.Type.ToString() + "\t\t域内容: " + field.Code.Text + "\r\n";
                                }
                            }
                            FieldList.Add(field);
                        }
                    }
                }
            }

            #endregion 

            #region 页眉的域
            this.m_wdApp.ActiveWindow.ActivePane.View.SeekView =  Microsoft.Office.Interop.Word.WdSeekView.wdSeekCurrentPageHeader;
            this.m_wdApp.Selection.WholeStory();
            if (IsDebug)
            {
                Log += "页眉的域数量为: " + this.m_wdApp.Selection.Fields.Count + "个\r\n由以下域组成:\r\n";
            }
            if (this.m_wdApp.Selection.Fields != null && this.m_wdApp.Selection.Fields.Count > 0)
            {
                foreach (Word.Field field in this.m_wdApp.Selection.Fields)
                {

                    if (IsDebug)
                    {
                        if (field.Type == Microsoft.Office.Interop.Word.WdFieldType.wdFieldAddin)
                        {
                            Log += "域类型为: " + field.Type.ToString() + "\t\t域内容: " + field.Data + "\r\n";
                        }
                        else
                        {
                            Log += "域类型为: " + field.Type.ToString() + "\t\t域内容: " + field.Code.Text + "\r\n";
                        }
                    }
                    FieldList.Add(field);
                }
            }


            //页眉面脚Shapes的域
            foreach (Word.Shape shape in this.m_wdApp.Selection.HeaderFooter.Shapes)
            {
                if (shape.AutoShapeType == MsoAutoShapeType.msoShapeRectangle)
                {
                    shape.Select(ref m_oMissing);
                    if (shape.TextFrame == null) continue;
                    if (shape.TextFrame.TextRange == null) continue;
                    wdTextFram = shape.TextFrame;
                    if (wdTextFram.TextRange != null)
                    {
                        wdTextRange = wdTextFram.TextRange;
                        if (IsDebug)
                        {
                            Log += "页眉页脚Shapes对象的域数量为: " + wdTextRange.Fields.Count + "个\r\n由以下域组成:\r\n";
                        }
                        foreach (Word.Field field in wdTextRange.Fields)
                        {
                            if (IsDebug)
                            {
                                if (field.Type == Microsoft.Office.Interop.Word.WdFieldType.wdFieldAddin)
                                {
                                    Log += "域类型为: " + field.Type.ToString() + "\t\t域内容: " + field.Data + "\r\n";
                                }
                                else
                                {
                                    Log += "域类型为: " + field.Type.ToString() + "\t\t域内容: " + field.Code.Text + "\r\n";
                                }
                            }
                            FieldList.Add(field);
                        }
                    }
                }
            }


            #endregion 

            #region 页脚的域

            //页脚的域
            this.m_wdApp.ActiveWindow.ActivePane.View.SeekView =  Microsoft.Office.Interop.Word.WdSeekView.wdSeekCurrentPageFooter;
            this.m_wdApp.Selection.WholeStory();
            if (IsDebug)
            {
                Log += "页脚的域数量为: " + this.m_wdApp.Selection.Fields.Count + "个\r\n由以下域组成:\r\n";
            }
            if (this.m_wdApp.Selection.Fields != null && this.m_wdApp.Selection.Fields.Count > 0)
            {
                foreach (Word.Field field in this.m_wdApp.Selection.Fields)
                {
                    if (IsDebug)
                    {
                        if (field.Type == Microsoft.Office.Interop.Word.WdFieldType.wdFieldAddin)
                        {
                            Log += "域类型为: " + field.Type.ToString() + "\t\t域内容: " + field.Data + "\r\n";
                        }
                        else
                        {
                            Log += "域类型为: " + field.Type.ToString() + "\t\t域内容: " + field.Code.Text + "\r\n";
                        }
                    }
                    FieldList.Add(field);
                }
            }


            //由于页脚的shapes的域在页眉Region里面找到了，这里就不再重复找

            #endregion 

            if (IsDebug)
            {
                WriteLog(Log);
            }

            return FieldList;
        }

        /// <summary>
        /// 写日志的方法,会在当前目录下生成 签名程序调试信息.log
        /// </summary>
        /// <param name="msg">传进来的消息</param>
        public static void WriteLog(string msg)
        {
            try
            {
                string LogFilePath = System.AppDomain.CurrentDomain.BaseDirectory + "签名程序调试信息.log";
                if (File.Exists(LogFilePath))
                {
                    StreamWriter sw = File.AppendText(LogFilePath);
                    sw.WriteLine(System.DateTime.Now.ToString());
                    sw.WriteLine(msg);
                    sw.WriteLine();
                    sw.Close();
                }
                else
                {
                    StreamWriter sw = File.CreateText(LogFilePath);
                    sw.WriteLine(System.DateTime.Now.ToString());
                    sw.WriteLine(msg);
                    sw.WriteLine();
                    sw.Close();
                }
            }
            catch (System.Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }

        }

        /// <summary>
        /// 对表格里面的文字进行压缩,若文字比表格的Range还要大才进行压缩
        /// </summary>
        private void FitWordTable()
        {
            //HDF2011.4.19 页眉中表格里面的文字进行压缩处理 
            if (this.m_wdApp.Selection.Tables != null && this.m_wdApp.Selection.Tables.Count > 0)
            {
                foreach (Word.Table tb in this.m_wdApp.Selection.Tables)
                {
                    for (int rw = 0; rw < tb.Rows.Count; rw++)
                    {
                        for (int cl = 0; cl < tb.Columns.Count; cl++)
                        {
                            try
                            {

                                //需要压缩才进行处理
                                if (tb.Cell(rw, cl).FitText)
                                {
                                    string text = tb.Cell(rw, cl).Range.Text;
                                    if (text.Contains("\r\a")) text = text.Replace("\r\a", " ");
                                    if (!string.IsNullOrEmpty(text.Trim()) && Asc(text.Substring(0, 1)) > 30)
                                    {
                                        tb.Cell(rw, cl).Range.Text = "";
                                        tb.Cell(rw, cl).Range.Text = text;

                                    }
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }

                    }
                }
            }

            //HDF2011.4.19 页脚中表格里面的文字进行压缩处理 
            if (this.m_wdApp.Selection.Tables != null && this.m_wdApp.Selection.Tables.Count > 0)
            {
                foreach (Word.Table tb in this.m_wdApp.Selection.Tables)
                {
                    for (int rw = 0; rw < tb.Rows.Count; rw++)
                    {
                        for (int cl = 0; cl < tb.Columns.Count; cl++)
                        {
                            try
                            {

                                //需要压缩的格才进行处理
                                if (tb.Cell(rw, cl).FitText)
                                {
                                    string text = tb.Cell(rw, cl).Range.Text;
                                    if (text.Contains("\r\a")) text = text.Replace("\r\a", " ");
                                    if (!string.IsNullOrEmpty(text.Trim()) && Asc(text.Substring(0, 1)) > 30)
                                    {
                                        tb.Cell(rw, cl).Range.Text = "";
                                        tb.Cell(rw, cl).Range.Text = text;

                                    }
                                }
                            }
                            catch { }
                        }

                    }
                }
            }

            //2011.4.16 DOE要求在表格里面进行压缩，将表格里面的文字进行重新设置
            if (m_wdDoc != null)
            {
                foreach (Word.Table tb in m_wdDoc.Tables)
                {

                    for (int rw = 0; rw < tb.Rows.Count; rw++)
                    {
                        for (int cl = 0; cl < tb.Columns.Count; cl++)
                        {
                            try
                            {
                                //需要压缩的格才进行处理
                                if (tb.Cell(rw, cl).FitText)
                                {
                                    string text = tb.Cell(rw, cl).Range.Text;
                                    if (text.Contains("\r\a")) text = text.Replace("\r\a", " ");
                                    if (!string.IsNullOrEmpty(text.Trim()) && Asc(text.Substring(0, 1)) > 30)
                                    {
                                        tb.Cell(rw, cl).Range.Text = "";
                                        tb.Cell(rw, cl).Range.Text = text;
                                    }
                                }
                            }
                            catch { }
                        }

                    }
                }
            }
        }
        //END
    }
}
