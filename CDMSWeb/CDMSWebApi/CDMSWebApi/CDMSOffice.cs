namespace AVEVA.CDMS.Common
{

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using AVEVA.CDMS.Server;
    using Microsoft.Office.Core;
    //using Microsoft.Office.Interop.Excel;
    //using Microsoft.Office.Interop.Word;
    using Microsoft.VisualBasic;

    public class CDMSOffice
    {
        protected bool _closeApp = true;
        protected bool _closeErrorApp;
        protected ArrayList _errorList = new ArrayList();
        protected bool _visibleApp = true;
        public bool IsFinial;
        private bool isFirstWriteData = true;
        public bool IsUpdateFileData;
        public bool IsUpdateVersion;
        private bool IsWord07;
        protected bool m_bExcelIsRunning;
        private bool m_bForceReWrite;
        protected bool m_bIsInsertRow;
        protected bool m_bWordIsRunning;
        protected Microsoft.Office.Interop.Excel.Application m_exApp;
        protected Microsoft.Office.Interop.Excel.Workbook m_exWorkBook;
        protected Microsoft.Office.Interop.Excel.Worksheet m_exWorksheet;
        protected object m_oMissing = Missing.Value;
        protected Microsoft.Office.Interop.Excel.Application m_wdApp;
        protected Microsoft.Office.Interop.Word.Document m_wdDoc;
        private List<Field> picFieldList = new List<Field>();

        public static List<string> ApplyExcelTable(string filePath, int iNeedRow, string expressionKey, string copyLeftTop, string copyRightBottom, int iFixRowCount, int iCopyRowCount, bool bCopyRowContainFix, bool bSumLessThanFixNeedDelete, int iAllRowCount)
        {
            List<string> list = null;
            try
            {
                if ((string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) || (!filePath.ToLower().EndsWith(".xlsx") && !filePath.ToLower().EndsWith(".xls")))
                {
                    return null;
                }
                if ((string.IsNullOrEmpty(expressionKey) || string.IsNullOrEmpty(copyLeftTop)) || string.IsNullOrEmpty(copyRightBottom))
                {
                    return null;
                }
                Microsoft.Office.Interop.Excel.Application application = new Microsoft.Office.Interop.Excel.ApplicationClass();
                object updateLinks = Missing.Value;
                Microsoft.Office.Interop.Excel.Workbook workbook = application.Workbooks.Open(filePath, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks);
                if ((workbook == null) || (workbook.Worksheets.Count <= 0))
                {
                    return null;
                }
                Microsoft.Office.Interop.Excel.Worksheet worksheet = workbook.Worksheets[1] as Microsoft.Office.Interop.Excel.Worksheet;
                Microsoft.Office.Interop.Excel.Range fromRange = worksheet.get_Range(copyLeftTop, copyRightBottom);
                if (fromRange != null)
                {
                    if (iNeedRow <= iFixRowCount)
                    {
                        if (!bCopyRowContainFix && bSumLessThanFixNeedDelete)
                        {
                            fromRange.Select();
                            fromRange.Delete(Microsoft.Office.Interop.Excel.XlDirection.xlUp);
                        }
                    }
                    else
                    {
                        bool flag = false;
                        if (!bCopyRowContainFix && ((iFixRowCount + iCopyRowCount) >= iNeedRow))
                        {
                            Microsoft.Office.Interop.Excel.Range range2 = null;
                            List<Microsoft.Office.Interop.Excel.Range> hasValueRanges = null;
                            hasValueRanges = GetHasValueRanges(fromRange);
                            if ((hasValueRanges != null) && (hasValueRanges.Count > 0))
                            {
                                foreach (Microsoft.Office.Interop.Excel.Range range3 in hasValueRanges)
                                {
                                    if ((range3.Value2 != null) && range3.Value2.ToString().ToUpper().Contains(expressionKey.ToUpper()))
                                    {
                                        range2 = range3;
                                        break;
                                    }
                                }
                            }
                            if (range2 != null)
                            {
                                int num = 2;
                                string str = range2.Value2.ToString();
                                range2.Value2 = str.ToUpper().Replace(expressionKey.ToUpper(), expressionKey.ToUpper() + num.ToString());
                                if (list == null)
                                {
                                    list = new List<string>();
                                }
                                list.Add(expressionKey.ToUpper() + num.ToString());
                            }
                        }
                        else
                        {
                            flag = true;
                        }
                        if (flag)
                        {
                            int num2 = 0;
                            num2 = GetCopyCount(iNeedRow, iFixRowCount, iCopyRowCount, bCopyRowContainFix);
                            if (num2 > 0)
                            {
                                List<Microsoft.Office.Interop.Excel.Range> list3 = new List<Microsoft.Office.Interop.Excel.Range>();
                                if (!bCopyRowContainFix)
                                {
                                    list3.Add(fromRange);
                                }
                                fromRange.Select();
                                (application.Selection as Microsoft.Office.Interop.Excel.Range).Copy(updateLinks);
                                for (int i = 0; i < num2; i++)
                                {
                                    int count = worksheet.UsedRange.Rows.Count;
                                    Microsoft.Office.Interop.Excel.Range range4 = null;
                                    range4 = worksheet.Cells[iAllRowCount + 1, 1] as Microsoft.Office.Interop.Excel.Range;
                                    if (range4 != null)
                                    {
                                        range4.Select();
                                    }
                                    worksheet.Paste(updateLinks, updateLinks);
                                    if (application.Selection != null)
                                    {
                                        list3.Add(application.Selection as Range);
                                    }
                                    iAllRowCount += fromRange.Rows.Count;
                                }
                                if (list3.Count > 0)
                                {
                                    int num4 = 2;
                                    foreach (Microsoft.Office.Interop.Excel.Range range5 in list3)
                                    {
                                        Microsoft.Office.Interop.Excel.Range range6 = null;
                                        List<Microsoft.Office.Interop.Excel.Range> list4 = null;
                                        list4 = GetHasValueRanges(range5);
                                        if ((list4 != null) && (list4.Count > 0))
                                        {
                                            foreach (Microsoft.Office.Interop.Excel.Range range7 in list4)
                                            {
                                                if ((range7.Value2 != null) && range7.Value2.ToString().ToUpper().Contains(expressionKey.ToUpper()))
                                                {
                                                    range6 = range7;
                                                    break;
                                                }
                                            }
                                        }
                                        if (range6 != null)
                                        {
                                            string str2 = range6.Value2.ToString();
                                            range6.Value2 = str2.ToUpper().Replace(expressionKey.ToUpper(), expressionKey.ToUpper() + num4.ToString());
                                            if (list == null)
                                            {
                                                list = new List<string>();
                                            }
                                            list.Add(expressionKey.ToUpper() + num4.ToString());
                                            num4++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                workbook.Save();
                workbook.Close(updateLinks, updateLinks, updateLinks);
                application.Quit();
            }
            catch (Exception exception)
            {
                CDMS.Write(exception.ToString());
            }
            return list;
        }

        public static int Asc(string character)
        {
            if (character.Length != 1)
            {
                throw new Exception("Character is not valid.");
            }
            ASCIIEncoding encoding = new ASCIIEncoding();
            return encoding.GetBytes(character)[0];
        }

        //protected int CellFiledCount(Microsoft.Office.Interop.Excel.Selection selection)
        //{
        //    return selection.Cells[1].Range.Fields.Count;
        //}

        public void ChearAddrInValue(Field field)
        {
            field.Select();
            Microsoft.Office.Interop.Excel.Range result = field.Result;
            this.m_wdApp.Selection.WholeStory();
            string text = result.Text;
            int start = result.Start;
            if (string.IsNullOrEmpty(text))
            {
                text = "";
            }
            while (!text.Contains("\r"))
            {
                result.End = start++;
                text = result.Text;
                if (string.IsNullOrEmpty(text))
                {
                    text = "";
                }
            }
            result.Text = "\a";
        }

        public static string Chr(int asciiCode)
        {
            if ((asciiCode < 0) || (asciiCode > 0xff))
            {
                throw new Exception("ASCII Code is not valid.");
            }
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] bytes = new byte[] { (byte) asciiCode };
            return encoding.GetString(bytes);
        }

        public bool ClearExcelExpressions(string filePath)
        {
            try
            {
                if ((!string.IsNullOrEmpty(filePath) && File.Exists(filePath)) && (filePath.ToLower().EndsWith(".xls") || filePath.ToLower().EndsWith(".xlsx")))
                {
                    if (!this.ExcelIsRunning(this.m_exApp, filePath))
                    {
                        if (!this.OpenExcelNew(filePath))
                        {
                            return false;
                        }
                        this.ClearExcelFiled();
                        this.SaveExcel();
                        return true;
                    }
                    this._errorList.Add("[" + filePath + "已经打开!]");
                }
            }
            catch (Exception exception)
            {
                CDMS.Write(exception.ToString());
            }
            return false;
        }

        private void ClearExcelFiled()
        {
            try
            {
                if (this.m_exWorkBook != null)
                {
                    Regex regex = new Regex("(TEXT|BMP)(:)(.*)");
                    foreach (Microsoft.Office.Interop.Excel.Worksheet worksheet in this.m_exWorkBook.Worksheets)
                    {
                        foreach (Microsoft.Office.Interop.Excel.Range range2 in worksheet.UsedRange)
                        {
                            string str = range2.Value2 as string;
                            if (!string.IsNullOrEmpty(str))
                            {
                                Match match = regex.Match(str);
                                if (((match != null) && match.Success) && ((str.ToUpper().Contains("SELECT ") || str.ToUpper().Contains("TEXT:")) || str.ToUpper().Contains("BMP:")))
                                {
                                    range2.Value2 = "";
                                }
                            }
                        }
                        this.m_exWorkBook.RefreshAll();
                    }
                }
            }
            catch (Exception exception)
            {
                this._errorList.Add(exception.ToString());
                CDMS.Write(exception.ToString());
            }
        }

        protected int ConverToInt(string s)
        {
            int num = 0;
            int num2 = 0;
            string character = s.Substring(0, 1);
            if (this.HasTwoChars(s))
            {
                string str2 = s.Substring(1, 1);
                num2 = (Asc(character) - 0x41) + 1;
                int num3 = (Asc(str2) - 0x41) + 1;
                for (int i = num2; i < 0x1b; i++)
                {
                    int num5 = num3;
                    while (num5 < 0x1b)
                    {
                        return ((num2 * 0x1a) + num3);
                    }
                }
                return num;
            }
            return (num2 = (Asc(character) - 0x41) + 1);
        }

        public bool CopyDocumentData(string mainFilePath, List<string> fileList)
        {
            try
            {
                if (string.IsNullOrEmpty(mainFilePath) || !File.Exists(mainFilePath))
                {
                    return false;
                }
                if ((fileList != null) && (fileList.Count > 0))
                {
                    foreach (string str in fileList)
                    {
                        if (!string.IsNullOrEmpty(str) && File.Exists(str))
                        {
                            this.OpenWordNew(str);
                            this.m_wdApp.Selection.WholeStory();
                            this.m_wdApp.Selection.Copy();
                            this.Release(true);
                            Thread.Sleep(0x7d0);
                            this.OpenWordNew(mainFilePath);
                            object wdStory = WdUnits.wdStory;
                            object oMissing = this.m_oMissing;
                            this.m_wdApp.Selection.EndKey(ref wdStory, ref oMissing);
                            this.m_wdApp.Selection.TypeParagraph();
                            this.m_wdApp.Selection.PasteAndFormat(WdRecoveryType.wdPasteDefault);
                            this.SaveWord(mainFilePath);
                        }
                    }
                }
                return true;
            }
            catch (Exception exception)
            {
                this._errorList.Add(exception.ToString());
                CDMS.Write(exception.ToString());
            }
            return false;
        }

        public static int CountFilePage(string filepath)
        {
            if (!File.Exists(filepath))
            {
                return 0;
            }
            try
            {
                int count = 0;
                if (filepath.ToLower().EndsWith(".xls") || filepath.ToLower().EndsWith(".xlsx"))
                {
                    Microsoft.Office.Interop.Excel.Application application = new Microsoft.Office.Interop.Excel.ApplicationClass();
                    object updateLinks = Missing.Value;
                    Microsoft.Office.Interop.Excel.Workbook workbook = application.Workbooks.Open(filepath, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks);
                    for (int i = 1; i <= workbook.Sheets.Count; i++)
                    {
                        Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[i];
                        if (worksheet.UsedRange.Value2 != null)
                        {
                            int num3 = worksheet.HPageBreaks.Count + 1;
                            int num4 = worksheet.VPageBreaks.Count + 1;
                            count += num3 * num4;
                        }
                    }
                    workbook.Close(updateLinks, updateLinks, updateLinks);
                    application.Quit();
                }
                else if (filepath.ToLower().EndsWith(".doc") || filepath.ToLower().EndsWith(".docx"))
                {
                    Microsoft.Office.Interop.Excel.Application application2 = new Microsoft.Office.Interop.Excel.ApplicationClass();
                    object confirmConversions = Missing.Value;
                    object fileName = filepath;
                    Microsoft.Office.Interop.Excel.Document document = application2.Documents.Open(ref fileName, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions);
                    count = document.ComputeStatistics(WdStatistic.wdStatisticPages, ref confirmConversions);
                    document.Close(ref confirmConversions, ref confirmConversions, ref confirmConversions);
                    application2.Quit(ref confirmConversions, ref confirmConversions, ref confirmConversions);
                }
                else if (filepath.ToLower().EndsWith(".pdf"))
                {
                    FileStream stream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
                    string input = new StreamReader(stream).ReadToEnd();
                    Regex regex = new Regex(@"/Type\s*/Page[^s]");
                    count = regex.Matches(input).Count;
                }
                return count;
            }
            catch (Exception exception)
            {
                CDMS.Write(exception.ToString());
                return 0;
            }
        }

        public void CreateExcelField(string key, string createType)
        {
            if (this.m_exApp != null)
            {
                string str = (createType.ToUpper() == "BMP") ? "BMP:" : "TEXT:";
                Microsoft.Office.Interop.Excel.Range activeCell = this.m_exApp.ActiveCell;
                if (activeCell != null)
                {
                    string str2 = str + key;
                    activeCell.Value2 = str2;
                    this.m_exApp.ActiveWorkbook.RefreshAll();
                }
            }
        }

        public static bool DeleteExcelRow(Microsoft.Office.Interop.Excel.Worksheet sheet, int iRow)
        {
            try
            {
                if (((sheet != null) && (sheet.Rows != null)) && ((sheet.Rows.Count >= iRow) && (sheet.Application != null)))
                {
                    Microsoft.Office.Interop.Excel.Range range = sheet.Cells[iRow, 1] as Microsoft.Office.Interop.Excel.Range;
                    range.EntireRow.Select();
                    Microsoft.Office.Interop.Excel.Range selection = sheet.Application.Selection as Microsoft.Office.Interop.Excel.Range;
                    if (selection != null)
                    {
                        selection.Delete(Microsoft.Office.Interop.Excel.XlDirection.xlUp);
                        return true;
                    }
                }
            }
            catch (Exception exception)
            {
                CDMS.Write(exception.ToString());
            }
            return false;
        }

        protected ArrayList DivideStrByCommaToDataCol(string sExpression, params string[] sDivideMark)
        {
            ArrayList list = new ArrayList();
            string str = ",";
            if (sDivideMark.Length > 0)
            {
                str = sDivideMark[0];
            }
            try
            {
                if ((sExpression == null) || (sExpression.Length <= 0))
                {
                    return null;
                }
                if ((Strings.InStr(1, sExpression, "$[", CompareMethod.Text) > 0) && (Strings.InStr(1, sExpression, "#", CompareMethod.Text) > 0))
                {
                    str = "#";
                }
                sExpression = sExpression.Replace("，", ",");
                sExpression = sExpression.Replace("、", ",");
                if (Strings.Mid(sExpression, Strings.Len(sExpression), 1) == str)
                {
                    sExpression = sExpression + " ";
                }
                do
                {
                    int num = Strings.InStr(1, sExpression, str, CompareMethod.Text);
                    if (str == ",")
                    {
                        if (num == 0)
                        {
                            num = Strings.InStr(1, sExpression, "，", CompareMethod.Text);
                            if (num > 0)
                            {
                                sExpression = Strings.Mid(sExpression, 1, num - 1) + " " + Strings.Mid(sExpression, num + 1);
                            }
                        }
                        if (num == 0)
                        {
                            num = Strings.InStr(1, sExpression, "、", CompareMethod.Text);
                            if (num > 0)
                            {
                                sExpression = Strings.Mid(sExpression, 1, num - 1) + " " + Strings.Mid(sExpression, num + 1);
                            }
                        }
                        if (num == 0)
                        {
                            num = Strings.InStr(1, sExpression, ":", CompareMethod.Text);
                        }
                    }
                    if (num > 0)
                    {
                        list.Add(Strings.Mid(sExpression, 1, num - 1));
                        sExpression = Strings.Mid(sExpression, num + 1);
                    }
                    else
                    {
                        list.Add(Strings.Mid(sExpression, num + 1));
                        sExpression = "";
                    }
                }
                while (Strings.Len(sExpression) > 0);
                return list;
            }
            catch (Exception exception)
            {
                this._errorList.Add(exception.ToString());
                CDMS.Write(exception.ToString());
                return null;
            }
        }

        protected string DownLoadFile(Project proj, string path)
        {
            if (proj == null)
            {
                return "";
            }
            string str = "";
            try
            {
                string workingPath = proj.dBSource.LoginUser.WorkingPath;
                if ((path.ToLower().EndsWith(".jpg") || path.ToLower().EndsWith(".bmp")) || path.ToLower().EndsWith(".dwg"))
                {
                    str = @"C:\temp\CDMSBMP\" + path;
                }
                else
                {
                    str = workingPath + @"\" + path;
                }
                CDMS.DownloadServerhostFile(proj.dBSource, path, str);
            }
            catch (Exception exception)
            {
                this._errorList.Add(exception.ToString());
                CDMS.Write(exception.ToString());
            }
            return str;
        }

        protected bool DownLoadFile(object oProjectOrDoc, string sServerFileName, string sLocalFileName)
        {
            Project project = null;
            if (oProjectOrDoc is Project)
            {
                project = oProjectOrDoc as Project;
            }
            else if (oProjectOrDoc is Doc)
            {
                project = (oProjectOrDoc as Doc).Project;
            }
            CDMS.DownloadServerhostFile(project.dBSource, sServerFileName, sLocalFileName);
            return File.Exists(sLocalFileName);
        }

        public string ExcelFieldGetExpression(Microsoft.Office.Interop.Excel.Worksheet refSheet, int refRow, int refCol)
        {
            try
            {
                Microsoft.Office.Interop.Excel.Range range = (Microsoft.Office.Interop.Excel.Range) refSheet.Cells[refRow, refCol];
                return range.Validation.InputMessage;
            }
            catch (Exception)
            {
            }
            return null;
        }

        public bool ExcelFieldSetExpression(Microsoft.Office.Interop.Excel.Worksheet refSheet, int refRow, int refCol, string refExpression)
        {
            if (!string.IsNullOrEmpty(refExpression))
            {
                try
                {
                    Microsoft.Office.Interop.Excel.Range range = (Microsoft.Office.Interop.Excel.Range)refSheet.Cells[refRow, refCol];
                    range.Validation.Add(Microsoft.Office.Interop.Excel.XlDVType.xlValidateInputOnly, Microsoft.Office.Interop.Excel.XlDVAlertStyle.xlValidAlertInformation, Type.Missing, Type.Missing, Type.Missing);
                    range.Validation.InputMessage = refExpression;
                    return true;
                }
                catch (Exception)
                {
                }
            }
            return false;
        }

        protected bool ExcelIsRunning(Microsoft.Office.Interop.Excel.Application exAppl, string sFileName)
        {
            if (exAppl != null)
            {
                foreach (Microsoft.Office.Interop.Excel.Workbook workbook in exAppl.Workbooks)
                {
                    if (exAppl.ActiveWorkbook.FullName == workbook.FullName)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected bool FillAddInFiled(string sExpressValue, Field field)
        {
            try
            {
                if ((sExpressValue.Length > 0) && (field != null))
                {
                    field.Select();
                    if (this.IsUpdateFileData)
                    {
                        this.ChearAddrInValue(field);
                    }
                    field.Result.Text = sExpressValue;
                    field.ShowCodes = false;
                    field.Locked = true;
                    return true;
                }
            }
            catch (Exception exception)
            {
                if (this._errorList != null)
                {
                    this._errorList.Add(exception.Message);
                }
                CDMS.Write(exception.ToString());
                throw;
            }
            return false;
        }

        public bool FillExcel(string sSaveAsPath, int iRowNum, string[] arrColumnName, string[] arrData)
        {
            if (iRowNum <= 0)
            {
                this._errorList.Add("行号不能小于等于0!");
                return false;
            }
            if (arrColumnName.Length <= 0)
            {
                this._errorList.Add("列名不存在!");
                return false;
            }
            if (arrData.Length <= 0)
            {
                this._errorList.Add("数据不存在!");
                return false;
            }
            this.m_exApp = null;
            this.m_exWorkBook = null;
            this.m_exWorksheet = null;
            int length = arrColumnName.Length;
            int num1 = arrData.Length / arrColumnName.Length;
            try
            {
                try
                {
                    this.m_exApp = new Microsoft.Office.Interop.Excel.ApplicationClass();
                }
                catch
                {
                    this.m_exApp = null;
                }
                if (this.m_exApp == null)
                {
                    Type typeFromProgID = Type.GetTypeFromProgID("excel.application");
                    this.m_exApp = Activator.CreateInstance(typeFromProgID, true) as Application;
                }
                if (this.m_exApp == null)
                {
                    return false;
                }
                this.m_exApp.Visible = this._visibleApp;
                this.m_exWorkBook = this.m_exApp.Workbooks.Add(this.m_oMissing);
                this.m_exWorksheet = this.m_exWorkBook.Worksheets[1] as Worksheet;
                for (int i = 0; i < arrColumnName.Length; i++)
                {
                    this.m_exWorksheet.Cells[iRowNum, i + 1] = arrColumnName[i];
                }
                for (int j = 0; j < arrData.Length; j++)
                {
                    this.m_exWorksheet.Cells[((j / length) + 1) + iRowNum, (j % length) + 1] = arrData[j];
                }
                this.m_exWorksheet.Cells.EntireColumn.AutoFit();
                this.m_exWorkBook.SaveAs(sSaveAsPath, this.m_oMissing, this.m_oMissing, this.m_oMissing, this.m_oMissing, this.m_oMissing, XlSaveAsAccessMode.xlShared, this.m_oMissing, this.m_oMissing, this.m_oMissing, this.m_oMissing, this.m_oMissing);
                this.SaveExcel();
                return true;
            }
            catch (Exception exception)
            {
                this._errorList.Add(exception.ToString());
                CDMS.Write(exception.ToString());
                this.Release(true);
                return false;
            }
        }

        protected void FillExcelData(object oProjectOrDoc, string sFileName)
        {
            try
            {
                this.FillExcelData(oProjectOrDoc, sFileName, 0, 0, "", null);
            }
            catch (Exception exception)
            {
                this._errorList.Add(exception.ToString());
                CDMS.Write(exception.ToString());
            }
        }

        public void FillExcelData(object oProjectOrDoc, string sFileName, int lMinRow, int lMaxRow, string sRowValue, ArrayList arrValueList)
        {
            try
            {
                int num = 1;
                int num2 = 4;
                int num3 = 0;
                int num4 = 0;
                Project project = null;
                Doc doc = null;
                ArrayList list = new ArrayList();
                if ((this.m_exWorkBook != null) && (oProjectOrDoc != null))
                {
                    if ((((arrValueList != null) && (arrValueList.Count > 0)) && ((lMinRow > 0) && (lMaxRow > 0))) && (sRowValue.Length > 0))
                    {
                        list = this.DivideStrByCommaToDataCol(sRowValue, new string[0]);
                        num = (arrValueList.Count / list.Count) / ((lMaxRow - lMinRow) + 1);
                        for (int j = 1; j <= num; j++)
                        {
                            this.m_exWorkBook.Sheets.Copy(this.m_oMissing, this.m_exWorkBook.Sheets[1]);
                        }
                        string str = ((Microsoft.Office.Interop.Excel.Worksheet) this.m_exWorkBook.Sheets[1]).Name;
                        num++;
                        for (int k = 1; k <= num; k++)
                        {
                            ((Microsoft.Office.Interop.Excel.Worksheet) this.m_exWorkBook.Sheets[k]).Name = string.Concat(new object[] { str, "(", k, "-", num, ")" });
                        }
                    }
                    else
                    {
                        num = 1;
                    }
                    if (oProjectOrDoc is Project)
                    {
                        project = oProjectOrDoc as Project;
                    }
                    else if (oProjectOrDoc is Doc)
                    {
                        doc = oProjectOrDoc as Doc;
                    }
                    Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet) this.m_exWorkBook.Sheets[1];
                    string name = ((Microsoft.Office.Interop.Excel.Worksheet) this.m_exWorkBook.Sheets[1]).Name;
                    for (int i = 1; i <= num; i++)
                    {
                        worksheet = (Microsoft.Office.Interop.Excel.Worksheet) this.m_exWorkBook.Sheets[i];
                        for (int m = 1; m <= 0x19; m++)
                        {
                            for (int n = 1; n <= 100; n++)
                            {
                                string str2 = ((Microsoft.Office.Interop.Excel.Range) worksheet.Cells[n, m]).Value2 as string;
                                if (str2 != null)
                                {
                                    if (str2.Length > 0)
                                    {
                                        if (Strings.InStr(1, str2, "$(*", CompareMethod.Text) > 0)
                                        {
                                            str2 = "$(" + Strings.Mid(str2, 4);
                                            ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[n, m]).Value2 = str2;
                                        }
                                        else if (((Strings.InStr(1, str2, "$(", CompareMethod.Text) > 0) || (Strings.InStr(1, str2, "@@", CompareMethod.Text) > 0)) || ((Strings.InStr(1, str2, ":", CompareMethod.Text) > 0) || (Strings.InStr(1, str2, "$[", CompareMethod.Text) > 0)))
                                        {
                                            if ((oProjectOrDoc is Project) && (project != null))
                                            {
                                                ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[n, m]).Value2 = project.ExcuteDefnExpression(str2)[0];
                                            }
                                            else if ((oProjectOrDoc is Doc) && (doc != null))
                                            {
                                                ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[n, m]).Value2 = doc.ExcuteDefnExpression(str2)[0];
                                            }
                                        }
                                    }
                                    if (str2.Length == 0)
                                    {
                                        num3++;
                                    }
                                    else
                                    {
                                        num3 = 0;
                                    }
                                    if ((num2 == num3) && (n > num2))
                                    {
                                        num3 = 0;
                                        break;
                                    }
                                }
                            }
                            if (num3 > 0)
                            {
                                num4++;
                            }
                            else
                            {
                                num4 = 0;
                            }
                            if (num4 == num2)
                            {
                                break;
                            }
                        }
                        Shape shape = null;
                        using (IEnumerator enumerator = worksheet.Shapes.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                Shape current = (Shape) enumerator.Current;
                                if ((shape.AlternativeText.Length > 0) && (shape.AlternativeText.IndexOf("$(") > 0))
                                {
                                    Drawing drawingObject = (Drawing) shape.DrawingObject;
                                    if ((oProjectOrDoc is Project) && (project != null))
                                    {
                                        drawingObject.Caption = project.ExcuteDefnExpression(drawingObject.Caption)[0];
                                    }
                                    else if ((oProjectOrDoc is Doc) && (doc != null))
                                    {
                                        drawingObject.Caption = doc.ExcuteDefnExpression(drawingObject.Caption)[0];
                                    }
                                }
                            }
                        }
                        if ((arrValueList != null) && (arrValueList.Count > 0))
                        {
                            for (int num10 = lMinRow; num10 <= lMaxRow; num10++)
                            {
                                i = 1;
                                foreach (string str3 in list)
                                {
                                    num4 = ((((i - 1) * (lMaxRow - lMinRow)) * arrValueList.Count) + ((num10 - lMinRow) * list.Count)) + i;
                                    if (num4 > arrValueList.Count)
                                    {
                                        break;
                                    }
                                    worksheet.get_Range(str3 + num10, this.m_oMissing).Value2 = arrValueList[num4 - 1];
                                    i++;
                                }
                            }
                        }
                    }
                    if (lMinRow == 8)
                    {
                        this.m_exApp.Rows.AutoFit();
                        this.m_exApp.Columns.AutoFit();
                    }
                }
            }
            catch (Exception exception)
            {
                this._errorList.Add(exception.ToString());
                CDMS.Write(exception.ToString());
                this.Release(true);
            }
        }

        public bool FillExcelIndexValue(string sFileName, string[] IndexCel, List<Hashtable> DataList)
        {
            try
            {
                if ((sFileName.Trim() == string.Empty) || (DataList.Count == 0))
                {
                    return false;
                }
                string[] strArray = new string[IndexCel.Length];
                if (File.Exists(sFileName))
                {
                    if (!this.OpenExcelNew(sFileName))
                    {
                        return false;
                    }
                    if (this.m_exWorksheet == null)
                    {
                        return false;
                    }
                    int num = 1;
                    int num2 = 0;
                    while (num2 < 5)
                    {
                        int index = 0;
                        foreach (string str2 in IndexCel)
                        {
                            strArray[index] = ((Microsoft.Office.Interop.Excel.Range) this.m_exWorksheet.Cells[num, this.ConverToInt(str2)]).Text.ToString().Trim();
                            index++;
                        }
                        bool flag = false;
                        foreach (string str3 in strArray)
                        {
                            if (str3.Trim().Length > 0)
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            num2++;
                        }
                        else
                        {
                            num2 = 0;
                        }
                        if (flag)
                        {
                            foreach (Hashtable hashtable in DataList)
                            {
                                flag = true;
                                index = 0;
                                foreach (string str4 in strArray)
                                {
                                    string str = (hashtable[IndexCel[index]] == null) ? "" : hashtable[IndexCel[index]].ToString();
                                    if (str4.ToUpper() != str.ToUpper())
                                    {
                                        flag = false;
                                        break;
                                    }
                                    index++;
                                }
                                if (flag)
                                {
                                    foreach (DictionaryEntry entry in hashtable)
                                    {
                                        this.m_exWorksheet.Cells[num, this.ConverToInt(entry.Key.ToString())] = entry.Value;
                                    }
                                    DataList.Remove(hashtable);
                                    break;
                                }
                            }
                        }
                        num++;
                    }
                    num -= num2;
                    foreach (Hashtable hashtable2 in DataList)
                    {
                        foreach (DictionaryEntry entry2 in hashtable2)
                        {
                            this.m_exWorksheet.Cells[num, this.ConverToInt(entry2.Key.ToString())] = entry2.Value;
                        }
                        num++;
                    }
                    this.SaveExcel();
                    return true;
                }
                this._errorList.Add(sFileName + "不存在");
            }
            catch (Exception exception)
            {
                this._errorList.Add(exception.ToString());
                CDMS.Write(exception.ToString());
                this.Release(true);
            }
            return false;
        }

        protected void FillFooterField(object oProjectOrDoc, Hashtable htUserKeyWord)
        {
            string[] strArray = new string[0];
            try
            {
                this.m_wdDoc.Application.ActiveWindow.ActivePane.View.SeekView = WdSeekView.wdSeekCurrentPageFooter;
                foreach (Field field in this.m_wdDoc.Application.Selection.Fields)
                {
                    if (field.Type == WdFieldType.wdFieldAddin)
                    {
                        string data = field.Data;
                        if ((data.Substring(data.Length - 1) != "F") && !field.Locked)
                        {
                            if (oProjectOrDoc is Project)
                            {
                                strArray = (oProjectOrDoc as Project).ExcuteDefnExpression(data, htUserKeyWord);
                            }
                            else if (oProjectOrDoc is Doc)
                            {
                                strArray = (oProjectOrDoc as Doc).ExcuteDefnExpression(data, htUserKeyWord);
                            }
                            this.FillAddInFiled(strArray[0], field);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                if (this._errorList != null)
                {
                    this._errorList.Add(exception.Message);
                }
                CDMS.Write(exception.ToString());
                this.Release(this._closeErrorApp);
            }
        }

        private void FillImageTableField(Field field, object oProjectOrDoc, Hashtable htUserKeyWord, string[] Commands, bool IsFinish, bool IsDebug)
        {
            if (Commands.Length != 4)
            {
                if (IsDebug)
                {
                    WriteLog(field.Data + "关键字不符合IMAGETABLE约定,不再该域签名");
                }
            }
            else
            {
                string key = Commands[1];
                int num = Convert.ToInt32(Commands[2]);
                int num2 = Convert.ToInt32(Commands[3]);
                if (IsFinish && htUserKeyWord.Contains(key))
                {
                    field.Select();
                    for (int i = 1; i <= num; i++)
                    {
                        for (int j = 1; j <= num2; j++)
                        {
                            if (field.Application.Selection.Tables[1].Rows[i].Cells[j].Range.InlineShapes.Count > 0)
                            {
                                field.Application.Selection.Tables[1].Rows[i].Cells[j].Range.InlineShapes[1].Delete();
                            }
                        }
                    }
                }
                if ((htUserKeyWord == null) || !htUserKeyWord.Contains(key))
                {
                    if (IsDebug)
                    {
                        WriteLog(field.Data + "关键字解析不成功,htUserKeyWord没有该关键字的信息");
                    }
                }
                else
                {
                    string str2 = htUserKeyWord[key].ToString();
                    if (string.IsNullOrEmpty(str2))
                    {
                        if (IsDebug)
                        {
                            WriteLog(field.Data + "解析后的值为空");
                        }
                    }
                    else
                    {
                        if (IsDebug)
                        {
                            WriteLog(field.Data + "解析后的值为 " + str2);
                        }
                        string[] strArray = str2.Split("|||".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        string workingPath = "";
                        if (oProjectOrDoc is Project)
                        {
                            if ((oProjectOrDoc as Project).dBSource.LoginUser.WorkingPath != null)
                            {
                                workingPath = (oProjectOrDoc as Project).dBSource.LoginUser.WorkingPath;
                            }
                        }
                        else if ((oProjectOrDoc is Doc) && ((oProjectOrDoc as Doc).dBSource.LoginUser.WorkingPath != null))
                        {
                            workingPath = (oProjectOrDoc as Doc).dBSource.LoginUser.WorkingPath;
                        }
                        workingPath = @"C:\temp\CDMSBMP\";
                        string sLocalFileName = workingPath + "blank.jpg";
                        this.DownLoadFile(oProjectOrDoc, @"\BMP\blank.jpg", sLocalFileName);
                        Queue<string> queue = new Queue<string>();
                        foreach (string str5 in strArray)
                        {
                            string str6 = workingPath + str5;
                            this.DownLoadFile(oProjectOrDoc, @"\BMP\" + str5, str6);
                            if (File.Exists(str6))
                            {
                                if (IsDebug)
                                {
                                    WriteLog(field.Data + " 下载图片: " + str5 + "成功");
                                }
                                queue.Enqueue(str6);
                            }
                            else
                            {
                                if (IsDebug)
                                {
                                    WriteLog(field.Data + " 下载图片: " + str5 + "失败");
                                }
                                queue.Enqueue(sLocalFileName);
                            }
                        }
                        field.Select();
                        for (int k = 1; k <= num; k++)
                        {
                            for (int m = 1; m <= num2; m++)
                            {
                                object range = field.Application.Selection.Tables[1].Rows[k].Cells[m].Range;
                                if (queue.Count > 0)
                                {
                                    field.Application.Selection.InlineShapes.AddPicture(queue.Dequeue(), ref this.m_oMissing, ref this.m_oMissing, ref range);
                                }
                            }
                        }
                        if (!field.Data.Contains("@@@ImageFinish"))
                        {
                            field.Data = field.Data + "@@@ImageFinish";
                        }
                    }
                }
            }
        }

        private void FillMultiPicField(Field field, object oProjectOrDoc, Hashtable htUserKeyWord, string[] Commands, int Length, bool IsDebug)
        {
            if (Commands.Length != 3)
            {
                if (IsDebug)
                {
                    WriteLog(field.Data + "关键字不符合IMAGETABLE约定,不再该域签名");
                }
                return;
            }
            string str = Commands[1];
            string expression = Commands[2];
            string[] strArray = this.GetPictureExNew(oProjectOrDoc, htUserKeyWord, expression, IsDebug);
            field.ShowCodes = true;
            field.Select();
            object wdCharacter = WdUnits.wdCharacter;
            object count = 1;
            int num = 0;
            if (Length != -1)
            {
                this.m_wdApp.Selection.MoveRight(ref wdCharacter, ref count, ref this.m_oMissing);
                for (int i = 0; i < Length; i++)
                {
                    this.m_wdApp.Selection.Range.Delete(ref wdCharacter, ref count);
                }
                field.Select();
            }
            string str4 = str.ToUpper();
            if (str4 != null)
            {
                if ((str4 == "UP") || (str4 == "LEFT"))
                {
                    goto Label_01FB;
                }
                if (!(str4 == "RIGHT") && (str4 == "DOWN"))
                {
                    this.m_wdApp.Selection.MoveRight(ref wdCharacter, ref count, ref this.m_oMissing);
                    for (int j = 0; j < strArray.Length; j++)
                    {
                        if (j != 0)
                        {
                            this.m_wdApp.Selection.Range.Text = "\r\n";
                            this.m_wdApp.Selection.MoveRight(ref wdCharacter, ref count, ref this.m_oMissing);
                            num++;
                        }
                        this.m_wdApp.Selection.InlineShapes.AddPicture(strArray[j], ref this.m_oMissing, ref this.m_oMissing, ref this.m_oMissing);
                        num++;
                    }
                    goto Label_01FB;
                }
            }
            this.m_wdApp.Selection.MoveRight(ref wdCharacter, ref count, ref this.m_oMissing);
            foreach (string str3 in strArray)
            {
                this.m_wdApp.Selection.InlineShapes.AddPicture(str3, ref this.m_oMissing, ref this.m_oMissing, ref this.m_oMissing);
                num++;
            }
        Label_01FB:
            field.ShowCodes = false;
            if (field.Data.Contains("@@@Length:"))
            {
                field.Data = field.Data.Substring(0, field.Data.IndexOf("@@@Length:"));
            }
            field.Data = field.Data + "@@@Length:" + num;
        }

        protected bool FillPicField(object oProjectOrDoc, Hashtable htUserKeyWord, Field wdField)
        {
            return this.FillPicField(oProjectOrDoc, htUserKeyWord, wdField, false);
        }

        protected bool FillPicField(object oProjectOrDoc, Hashtable htUserKeyWord, Field wdField, bool needBringFront)
        {
            try
            {
                string str2 = "";
                string text = wdField.Code.Text;
                text.IndexOf("INCLUDEPICTURE");
                string sourceName = wdField.LinkFormat.SourceName;
                if (text.Substring(text.Length - 1) != "F")
                {
                    str2 = this.GetPicture(oProjectOrDoc, htUserKeyWord, sourceName);
                    if ((!string.IsNullOrEmpty(str2) && (str2.Length > 0)) && this.IsUpdateFileData)
                    {
                        wdField.Select();
                        wdField.InlineShape.Width = 1f;
                        object wdCharacter = WdUnits.wdCharacter;
                        object count = 1;
                        object obj4 = 1;
                        this.m_wdApp.Selection.MoveRight(ref wdCharacter, ref count, ref this.m_oMissing);
                        this.m_wdApp.Selection.Delete(ref wdCharacter, ref obj4);
                        return true;
                    }
                    if (str2.Length > 0)
                    {
                        wdField.Select();
                        if (File.Exists(str2))
                        {
                            object linkToFile = false;
                            object saveWithDocument = true;
                            wdField.ShowCodes = false;
                            if (needBringFront && (wdField.InlineShape != null))
                            {
                                wdField.InlineShape.Width = 1f;
                                if (this.IsWord07)
                                {
                                    wdField.InlineShape.Height = 1f;
                                }
                                object unit = WdUnits.wdCharacter;
                                object obj8 = 1;
                                this.m_wdApp.Selection.MoveRight(ref unit, ref obj8, ref this.m_oMissing);
                                Shape shape2 = this.m_wdApp.Selection.InlineShapes.AddPicture(str2, ref linkToFile, ref saveWithDocument, ref this.m_oMissing).ConvertToShape();
                                if ((this.IsWord07 && this.IsFinial) && str2.ToLower().Contains("blank.jpg"))
                                {
                                    shape2.WrapFormat.Type = WdWrapType.wdWrapTight;
                                }
                            }
                            else
                            {
                                if (!this.IsWord07)
                                {
                                    str2 = str2.Replace(@"\", "//");
                                    wdField.Code.Text = " INCLUDEPICTURE  \"" + str2 + "\"";
                                }
                                else if (wdField.InlineShape == null)
                                {
                                    wdField.LinkFormat.SourceFullName = str2;
                                    wdField.LinkFormat.SavePictureWithDocument = true;
                                }
                                else
                                {
                                    wdField.InlineShape.LinkFormat.SourceFullName = str2;
                                    wdField.InlineShape.LinkFormat.SavePictureWithDocument = true;
                                }
                                if (!this.IsWord07)
                                {
                                    wdField.Update();
                                    wdField.UpdateSource();
                                }
                            }
                            wdField.Locked = true;
                            return true;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                CDMS.Write(exception.ToString());
                throw;
            }
            return false;
        }

        protected void FillShapeField(object oProjectOrDoc, Hashtable htUserKeyWord)
        {
            Hashtable hashtable = new Hashtable();
            string[] strArray = new string[0];
            try
            {
                TextFrame textFrame;
                string data;
                foreach (Shape shape in this.m_wdDoc.Shapes)
                {
                    if (shape.AutoShapeType == MsoAutoShapeType.msoShapeRectangle)
                    {
                        shape.Select(ref this.m_oMissing);
                        if ((shape.TextFrame != null) && (shape.TextFrame.TextRange != null))
                        {
                            textFrame = shape.TextFrame;
                            if (textFrame.TextRange != null)
                            {
                                foreach (Field field in textFrame.TextRange.Fields)
                                {
                                    if (field.Type == WdFieldType.wdFieldAddin)
                                    {
                                        data = field.Data;
                                        if ((data.Substring(data.Length - 1) != "F") && !field.Locked)
                                        {
                                            if (oProjectOrDoc is Project)
                                            {
                                                strArray = (oProjectOrDoc as Project).ExcuteDefnExpression(data, htUserKeyWord);
                                            }
                                            else if (oProjectOrDoc is Doc)
                                            {
                                                strArray = (oProjectOrDoc as Doc).ExcuteDefnExpression(data, htUserKeyWord);
                                            }
                                            this.FillAddInFiled(strArray[0], field);
                                        }
                                    }
                                    else if (((field.Type == WdFieldType.wdFieldIncludePicture) && !field.Locked) && (!this.FillPicField(oProjectOrDoc, htUserKeyWord, field) && !hashtable.ContainsKey(field.Index.ToString() + field.Code.Text)))
                                    {
                                        hashtable.Add(field.Index.ToString() + field.Code.Text, field);
                                    }
                                }
                                using (IDictionaryEnumerator enumerator3 = hashtable.GetEnumerator())
                                {
                                    while (enumerator3.MoveNext())
                                    {
                                        DictionaryEntry current = (DictionaryEntry) enumerator3.Current;
                                    }
                                }
                            }
                        }
                    }
                }
                hashtable.Clear();
                try
                {
                    this.m_wdApp.ActiveWindow.ActivePane.View.SeekView = WdSeekView.wdSeekCurrentPageHeader;
                    this.m_wdApp.Selection.WholeStory();
                    foreach (Shape shape2 in this.m_wdApp.Selection.HeaderFooter.Shapes)
                    {
                        if (shape2.AutoShapeType == MsoAutoShapeType.msoShapeRectangle)
                        {
                            shape2.Select(ref this.m_oMissing);
                            if ((shape2.TextFrame != null) && (shape2.TextFrame.TextRange != null))
                            {
                                textFrame = shape2.TextFrame;
                                if (textFrame.TextRange != null)
                                {
                                    foreach (Field field2 in textFrame.TextRange.Fields)
                                    {
                                        if (field2.Type == WdFieldType.wdFieldAddin)
                                        {
                                            data = field2.Data;
                                            if ((data.Substring(data.Length - 1) != "F") && !field2.Locked)
                                            {
                                                if (oProjectOrDoc is Project)
                                                {
                                                    strArray = (oProjectOrDoc as Project).ExcuteDefnExpression(data, htUserKeyWord);
                                                }
                                                else if (oProjectOrDoc is Doc)
                                                {
                                                    strArray = (oProjectOrDoc as Doc).ExcuteDefnExpression(data, htUserKeyWord);
                                                }
                                                this.FillAddInFiled(strArray[0], field2);
                                            }
                                        }
                                        else if (((field2.Type == WdFieldType.wdFieldIncludePicture) && !field2.Locked) && (!this.FillPicField(oProjectOrDoc, htUserKeyWord, field2) && !hashtable.ContainsKey(field2.Index.ToString() + field2.Code.Text)))
                                        {
                                            hashtable.Add(field2.Index.ToString() + field2.Code.Text, field2);
                                        }
                                    }
                                    using (IDictionaryEnumerator enumerator6 = hashtable.GetEnumerator())
                                    {
                                        while (enumerator6.MoveNext())
                                        {
                                            DictionaryEntry entry2 = (DictionaryEntry) enumerator6.Current;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    this._errorList.Add(exception.ToString());
                    CDMS.Write(exception.ToString());
                    throw;
                }
                hashtable.Clear();
                try
                {
                    this.m_wdApp.ActiveWindow.ActivePane.View.SeekView = WdSeekView.wdSeekCurrentPageFooter;
                    this.m_wdApp.Selection.WholeStory();
                    foreach (Shape shape3 in this.m_wdApp.Selection.HeaderFooter.Shapes)
                    {
                        if (shape3.AutoShapeType == MsoAutoShapeType.msoShapeRectangle)
                        {
                            shape3.Select(ref this.m_oMissing);
                            if ((shape3.TextFrame != null) && (shape3.TextFrame.TextRange != null))
                            {
                                textFrame = shape3.TextFrame;
                                if (textFrame.TextRange != null)
                                {
                                    foreach (Field field3 in textFrame.TextRange.Fields)
                                    {
                                        if (field3.Type == WdFieldType.wdFieldAddin)
                                        {
                                            data = field3.Data;
                                            if ((data.Substring(data.Length - 1) != "F") && !field3.Locked)
                                            {
                                                if (oProjectOrDoc is Project)
                                                {
                                                    strArray = (oProjectOrDoc as Project).ExcuteDefnExpression(data, htUserKeyWord);
                                                }
                                                else if (oProjectOrDoc is Doc)
                                                {
                                                    strArray = (oProjectOrDoc as Doc).ExcuteDefnExpression(data, htUserKeyWord);
                                                }
                                                this.FillAddInFiled(strArray[0], field3);
                                            }
                                        }
                                        else if (((field3.Type == WdFieldType.wdFieldIncludePicture) && !field3.Locked) && (!this.FillPicField(oProjectOrDoc, htUserKeyWord, field3) && !hashtable.ContainsKey(field3.Index.ToString() + field3.Code.Text)))
                                        {
                                            hashtable.Add(field3.Index.ToString() + field3.Code.Text, field3);
                                        }
                                    }
                                    using (IDictionaryEnumerator enumerator9 = hashtable.GetEnumerator())
                                    {
                                        while (enumerator9.MoveNext())
                                        {
                                            DictionaryEntry entry3 = (DictionaryEntry) enumerator9.Current;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception exception2)
                {
                    this._errorList.Add(exception2.ToString());
                    CDMS.Write(exception2.ToString());
                    throw;
                }
            }
            catch (Exception exception3)
            {
                if (this._errorList != null)
                {
                    this._errorList.Add(exception3.Message);
                }
                CDMS.Write(exception3.ToString());
                this.Release(this._closeErrorApp);
                throw;
            }
        }

        protected void FillShapeFieldUpdateData(object oProjectOrDoc, Hashtable htUserKeyWord)
        {
            Hashtable hashtable = new Hashtable();
            string[] strArray = new string[0];
            try
            {
                TextFrame textFrame;
                string data;
                foreach (Shape shape in this.m_wdDoc.Shapes)
                {
                    if (shape.AutoShapeType == MsoAutoShapeType.msoShapeRectangle)
                    {
                        shape.Select(ref this.m_oMissing);
                        if ((shape.TextFrame != null) && (shape.TextFrame.TextRange != null))
                        {
                            textFrame = shape.TextFrame;
                            if (textFrame.TextRange != null)
                            {
                                foreach (Field field in textFrame.TextRange.Fields)
                                {
                                    if (field.Type == WdFieldType.wdFieldAddin)
                                    {
                                        data = field.Data;
                                        if (data.Substring(data.Length - 1) != "F")
                                        {
                                            if (field.Locked)
                                            {
                                                field.Locked = false;
                                            }
                                            if (oProjectOrDoc is Project)
                                            {
                                                strArray = (oProjectOrDoc as Project).ExcuteDefnExpression(data, htUserKeyWord);
                                            }
                                            else if (oProjectOrDoc is Doc)
                                            {
                                                strArray = (oProjectOrDoc as Doc).ExcuteDefnExpression(data, htUserKeyWord);
                                            }
                                            this.FillAddInFiled(strArray[0], field);
                                        }
                                    }
                                    else if (field.Type == WdFieldType.wdFieldIncludePicture)
                                    {
                                        if (field.Locked)
                                        {
                                            field.Locked = false;
                                        }
                                        if (!this.FillPicField(oProjectOrDoc, htUserKeyWord, field) && !hashtable.ContainsKey(field.Index.ToString() + field.Code.Text))
                                        {
                                            hashtable.Add(field.Index.ToString() + field.Code.Text, field);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                hashtable.Clear();
                try
                {
                    this.m_wdApp.ActiveWindow.ActivePane.View.SeekView = WdSeekView.wdSeekCurrentPageHeader;
                    this.m_wdApp.Selection.WholeStory();
                    foreach (Shape shape2 in this.m_wdApp.Selection.HeaderFooter.Shapes)
                    {
                        if (shape2.AutoShapeType == MsoAutoShapeType.msoShapeRectangle)
                        {
                            shape2.Select(ref this.m_oMissing);
                            if ((shape2.TextFrame != null) && (shape2.TextFrame.TextRange != null))
                            {
                                textFrame = shape2.TextFrame;
                                if (textFrame.TextRange != null)
                                {
                                    foreach (Field field2 in textFrame.TextRange.Fields)
                                    {
                                        if (field2.Type == WdFieldType.wdFieldAddin)
                                        {
                                            data = field2.Data;
                                            if (data.Substring(data.Length - 1) != "F")
                                            {
                                                if (field2.Locked)
                                                {
                                                    field2.Locked = false;
                                                }
                                                if (oProjectOrDoc is Project)
                                                {
                                                    strArray = (oProjectOrDoc as Project).ExcuteDefnExpression(data, htUserKeyWord);
                                                }
                                                else if (oProjectOrDoc is Doc)
                                                {
                                                    strArray = (oProjectOrDoc as Doc).ExcuteDefnExpression(data, htUserKeyWord);
                                                }
                                                this.FillAddInFiled(strArray[0], field2);
                                            }
                                        }
                                        else if (field2.Type == WdFieldType.wdFieldIncludePicture)
                                        {
                                            if (field2.Locked)
                                            {
                                                field2.Locked = false;
                                            }
                                            if (!this.FillPicField(oProjectOrDoc, htUserKeyWord, field2) && !hashtable.ContainsKey(field2.Index.ToString() + field2.Code.Text))
                                            {
                                                hashtable.Add(field2.Index.ToString() + field2.Code.Text, field2);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    this._errorList.Add(exception.ToString());
                    CDMS.Write(exception.ToString());
                    throw;
                }
                hashtable.Clear();
                try
                {
                    this.m_wdApp.ActiveWindow.ActivePane.View.SeekView = WdSeekView.wdSeekCurrentPageFooter;
                    this.m_wdApp.Selection.WholeStory();
                    foreach (Shape shape3 in this.m_wdApp.Selection.HeaderFooter.Shapes)
                    {
                        if (shape3.AutoShapeType == MsoAutoShapeType.msoShapeRectangle)
                        {
                            shape3.Select(ref this.m_oMissing);
                            if ((shape3.TextFrame != null) && (shape3.TextFrame.TextRange != null))
                            {
                                textFrame = shape3.TextFrame;
                                if (textFrame.TextRange != null)
                                {
                                    foreach (Field field3 in textFrame.TextRange.Fields)
                                    {
                                        if (field3.Type == WdFieldType.wdFieldAddin)
                                        {
                                            data = field3.Data;
                                            if (data.Substring(data.Length - 1) != "F")
                                            {
                                                if (field3.Locked)
                                                {
                                                    field3.Locked = false;
                                                }
                                                if (oProjectOrDoc is Project)
                                                {
                                                    strArray = (oProjectOrDoc as Project).ExcuteDefnExpression(data, htUserKeyWord);
                                                }
                                                else if (oProjectOrDoc is Doc)
                                                {
                                                    strArray = (oProjectOrDoc as Doc).ExcuteDefnExpression(data, htUserKeyWord);
                                                }
                                                this.FillAddInFiled(strArray[0], field3);
                                            }
                                        }
                                        else if (field3.Type == WdFieldType.wdFieldIncludePicture)
                                        {
                                            if (field3.Locked)
                                            {
                                                field3.Locked = false;
                                            }
                                            if (!this.FillPicField(oProjectOrDoc, htUserKeyWord, field3) && !hashtable.ContainsKey(field3.Index.ToString() + field3.Code.Text))
                                            {
                                                hashtable.Add(field3.Index.ToString() + field3.Code.Text, field3);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception exception2)
                {
                    this._errorList.Add(exception2.ToString());
                    CDMS.Write(exception2.ToString());
                    throw;
                }
            }
            catch (Exception exception3)
            {
                if (this._errorList != null)
                {
                    this._errorList.Add(exception3.Message);
                }
                CDMS.Write(exception3.ToString());
                this.Release(this._closeErrorApp);
                throw;
            }
        }

        private void FillSingleValueField(Field field, object oProjectOrDoc, Hashtable htUserKeyWord, string Expression, int length, bool IsDebug)
        {
            string str = this.GetExpressValue(oProjectOrDoc, htUserKeyWord, Expression);
            string key = Expression.Substring(Expression.IndexOf("$(") + 2, (Expression.LastIndexOf(")") - Expression.IndexOf("$(")) - 2);
            if (((string.IsNullOrEmpty(str) && Expression.Contains("$(")) && Expression.Contains(")")) && ((htUserKeyWord == null) || !htUserKeyWord.Contains(key)))
            {
                if (IsDebug)
                {
                    WriteLog("Addin域: " + Expression + " 解析后值为: " + str + "\r\n用户没有传进来关键字,不执行更新该域");
                }
            }
            else
            {
                if (IsDebug)
                {
                    WriteLog("Addin域: " + Expression + " 解析后值为: " + str);
                }
                if ((htUserKeyWord != null) && htUserKeyWord.Contains(key))
                {
                    if (length != -1)
                    {
                        Microsoft.Office.Interop.Excel.Range result = field.Result;
                        field.Result.Select();
                        result.End = result.Start + length;
                        result.Text = "";
                    }
                    field.Result.Text = str;
                    field.Data = Expression + "@@@Length:" + str.Length.ToString();
                }
                else if (this.m_bForceReWrite && !string.IsNullOrEmpty(str))
                {
                    if (length != -1)
                    {
                        Microsoft.Office.Interop.Excel.Range range2 = field.Result;
                        field.Result.Select();
                        range2.End = range2.Start + length;
                        range2.Text = "";
                    }
                    field.Result.Text = str;
                    field.Data = Expression + "@@@Length:" + str.Length.ToString();
                }
            }
        }

        private void FillTextTableField(Field field, object oProjectOrDoc, Hashtable htUserKeyWord, string[] Commands, int Length, bool IsDebug)
        {
            if (Commands.Length != 4)
            {
                if (IsDebug)
                {
                    WriteLog(field.Data + "关键字不符合IMAGETABLE约定,不再该域签名");
                }
            }
            else
            {
                string key = Commands[1];
                int num = Convert.ToInt32(Commands[2]);
                int num2 = Convert.ToInt32(Commands[3]);
                int num3 = 1;
                int num4 = 1;
                field.Select();
                for (int i = 1; i <= field.Application.Selection.Tables[1].Rows.Count; i++)
                {
                    for (int j = 1; j <= field.Application.Selection.Tables[1].Columns.Count; j++)
                    {
                        try
                        {
                            if ((field.Application.Selection.Tables[1].Cell(i, j).Range.Fields.Count > 0) && (field.Application.Selection.Tables[1].Cell(i, j).Range.Fields[1].Data == field.Data))
                            {
                                num3 = i;
                                num4 = j;
                                num += num3;
                                break;
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                if ((Length != -1) && htUserKeyWord.Contains(key))
                {
                    field.Select();
                    for (int k = num3; k <= num; k++)
                    {
                        for (int m = 1; m <= num2; m++)
                        {
                            if ((k == num3) && (m == num4))
                            {
                                Microsoft.Office.Interop.Excel.Range result = field.Result;
                                field.Result.Select();
                                result.End = result.Start + Length;
                                result.Text = "";
                            }
                            else
                            {
                                field.Application.Selection.Tables[1].Cell(k, m).Range.Text = "";
                            }
                        }
                    }
                }
                if ((htUserKeyWord == null) || !htUserKeyWord.Contains(key))
                {
                    if (IsDebug)
                    {
                        WriteLog(field.Data + "关键字解析不成功,htUserKeyWord没有该关键字的信息");
                    }
                }
                else
                {
                    string str2 = htUserKeyWord[key].ToString();
                    if (IsDebug)
                    {
                        WriteLog(field.Data + "解析后的值为 " + str2);
                    }
                    string[] strArray = str2.Split("|||".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (oProjectOrDoc is Project)
                    {
                        if ((oProjectOrDoc as Project).dBSource.LoginUser.WorkingPath != null)
                        {
                            string workingPath = (oProjectOrDoc as Project).dBSource.LoginUser.WorkingPath;
                        }
                    }
                    else if ((oProjectOrDoc is Doc) && ((oProjectOrDoc as Doc).dBSource.LoginUser.WorkingPath != null))
                    {
                        string text2 = (oProjectOrDoc as Doc).dBSource.LoginUser.WorkingPath;
                    }
                    Queue<string> queue = new Queue<string>();
                    foreach (string str3 in strArray)
                    {
                        queue.Enqueue(str3);
                    }
                    field.Select();
                    int length = -1;
                    for (int n = num3; n <= num; n++)
                    {
                        for (int num11 = 1; num11 <= num2; num11++)
                        {
                            if ((n == num3) && (num11 == num4))
                            {
                                string str4 = queue.Dequeue();
                                field.Result.Text = str4;
                                length = str4.Length;
                            }
                            else if (queue.Count > 0)
                            {
                                field.Application.Selection.Tables[1].Cell(n, num11).Range.Text = queue.Dequeue();
                            }
                            else
                            {
                                field.Application.Selection.Tables[1].Cell(n, num11).Range.Text = "";
                            }
                        }
                    }
                    if (field.Data.Contains("@@@Length:"))
                    {
                        field.Data = field.Data.Substring(0, field.Data.IndexOf("@@@Length:"));
                    }
                    field.Data = field.Data + "@@@Length:" + length;
                }
            }
        }

        public bool FillWdFieldValue(object oProjectOrDoc, Hashtable htUserKeyWord)
        {
            try
            {
                string data;
                string str2;
                this.picFieldList.Clear();
                if (oProjectOrDoc == null)
                {
                    return false;
                }
                Hashtable hashtable = new Hashtable();
                Hashtable hashtable2 = new Hashtable();
                if ((this.m_wdDoc.Fields != null) && (this.m_wdDoc.Fields.Count > 0))
                {
                    for (int i = 1; i <= this.m_wdDoc.Fields.Count; i++)
                    {
                        if (!this.IsWord07)
                        {
                            this.m_wdDoc.Fields[i].Update();
                        }
                        if (this.m_wdDoc.Fields[i].Type == WdFieldType.wdFieldAddin)
                        {
                            data = this.m_wdDoc.Fields[i].Data;
                            if ((!string.IsNullOrEmpty(data) && (data.Substring(data.Length - 1) != "F")) && !this.m_wdDoc.Fields[i].Locked)
                            {
                                if (data.Contains("Pic:"))
                                {
                                    string key = this.m_wdDoc.Fields[i].Index.ToString() + this.m_wdDoc.Fields[i].Code.Text;
                                    if (!hashtable2.ContainsKey(key))
                                    {
                                        hashtable2.Add(key, this.m_wdDoc.Fields[i]);
                                    }
                                }
                                else
                                {
                                    str2 = this.GetExpressValue(oProjectOrDoc, htUserKeyWord, data);
                                    if (this.IsFinial && string.IsNullOrEmpty(str2))
                                    {
                                        str2 = " ";
                                    }
                                    this.FillAddInFiled(str2, this.m_wdDoc.Fields[i]);
                                }
                            }
                        }
                        else if ((this.m_wdDoc.Fields[i].Type == WdFieldType.wdFieldIncludePicture) && !this.m_wdDoc.Fields[i].Locked)
                        {
                            this.FillPicField(oProjectOrDoc, htUserKeyWord, this.m_wdDoc.Fields[i], true);
                        }
                    }
                    foreach (DictionaryEntry entry in hashtable2)
                    {
                        Field field = entry.Value as Field;
                        string str5 = field.Data.Split(new string[] { "Pic:" }, StringSplitOptions.RemoveEmptyEntries)[0];
                        int startIndex = str5.IndexOf("INCLUDEPICTURE") + 14;
                        string sKeyWord = str5.Substring(startIndex, str5.LastIndexOf(@"\") - startIndex);
                        field.Select();
                        field.Delete();
                        Field wdField = this.InsertPictureField(sKeyWord);
                        if (!this.FillPicField(oProjectOrDoc, htUserKeyWord, wdField) && !hashtable.ContainsKey(wdField.Index.ToString() + wdField.Code.Text))
                        {
                            hashtable.Add(wdField.Index.ToString() + wdField.Code.Text, wdField);
                        }
                    }
                    using (IDictionaryEnumerator enumerator2 = hashtable.GetEnumerator())
                    {
                        while (enumerator2.MoveNext())
                        {
                            DictionaryEntry current = (DictionaryEntry) enumerator2.Current;
                        }
                    }
                }
                this.FillShapeField(oProjectOrDoc, htUserKeyWord);
                this.m_wdApp.ActiveWindow.ActivePane.View.SeekView = WdSeekView.wdSeekCurrentPageHeader;
                this.m_wdApp.Selection.WholeStory();
                if ((this.m_wdApp.Selection.Fields != null) && (this.m_wdApp.Selection.Fields.Count > 0))
                {
                    hashtable2.Clear();
                    hashtable.Clear();
                    for (int j = 1; j <= this.m_wdApp.Selection.Fields.Count; j++)
                    {
                        if (this.m_wdApp.Selection.Fields[j].Type == WdFieldType.wdFieldAddin)
                        {
                            data = this.m_wdApp.Selection.Fields[j].Data;
                            if (string.IsNullOrEmpty(data))
                            {
                                continue;
                            }
                            if ((data.Substring(data.Length - 1) != "F") && !this.m_wdApp.Selection.Fields[j].Locked)
                            {
                                if (data.Contains("Pic:"))
                                {
                                    string str7 = this.m_wdApp.Selection.Fields[j].Index.ToString() + this.m_wdApp.Selection.Fields[j].Code.Text;
                                    if (!hashtable2.ContainsKey(str7))
                                    {
                                        hashtable2.Add(str7, this.m_wdApp.Selection.Fields[j]);
                                    }
                                    continue;
                                }
                                str2 = this.GetExpressValue(oProjectOrDoc, htUserKeyWord, data);
                                if (this.IsFinial && string.IsNullOrEmpty(str2))
                                {
                                    str2 = " ";
                                }
                                this.FillAddInFiled(str2, this.m_wdApp.Selection.Fields[j]);
                            }
                        }
                        else if (((this.m_wdApp.Selection.Fields[j].Type == WdFieldType.wdFieldIncludePicture) && !this.m_wdApp.Selection.Fields[j].Locked) && !this.FillPicField(oProjectOrDoc, htUserKeyWord, this.m_wdApp.Selection.Fields[j]))
                        {
                            hashtable.Add(this.m_wdApp.Selection.Fields[j].Index.ToString() + this.m_wdApp.Selection.Fields[j].Code.Text, this.m_wdApp.Selection.Fields[j]);
                        }
                        this.m_wdApp.Selection.WholeStory();
                    }
                    foreach (DictionaryEntry entry2 in hashtable2)
                    {
                        Field field3 = entry2.Value as Field;
                        string str9 = field3.Data.Split(new string[] { "Pic:" }, StringSplitOptions.RemoveEmptyEntries)[0];
                        int num4 = str9.IndexOf("INCLUDEPICTURE") + 14;
                        string str10 = str9.Substring(num4, str9.LastIndexOf(@"\") - num4);
                        field3.Select();
                        field3.Delete();
                        Field field4 = this.InsertPictureField(str10);
                        if (!this.FillPicField(oProjectOrDoc, htUserKeyWord, field4) && !hashtable.ContainsKey(field4.Index.ToString() + field4.Code.Text))
                        {
                            hashtable.Add(field4.Index.ToString() + field4.Code.Text, field4);
                        }
                    }
                    using (IDictionaryEnumerator enumerator4 = hashtable.GetEnumerator())
                    {
                        while (enumerator4.MoveNext())
                        {
                            DictionaryEntry entry4 = (DictionaryEntry) enumerator4.Current;
                        }
                    }
                }
                if ((this.m_wdApp.Selection.Tables != null) && (this.m_wdApp.Selection.Tables.Count > 0))
                {
                    foreach (Table table in this.m_wdApp.Selection.Tables)
                    {
                        for (int k = 0; k < table.Rows.Count; k++)
                        {
                            for (int m = 0; m < table.Columns.Count; m++)
                            {
                                try
                                {
                                    if (table.Cell(k, m).FitText)
                                    {
                                        string text = table.Cell(k, m).Range.Text;
                                        if (text.Contains("\r\a"))
                                        {
                                            text = text.Replace("\r\a", " ");
                                        }
                                        if (!string.IsNullOrEmpty(text.Trim()) && (Asc(text.Substring(0, 1)) > 30))
                                        {
                                            table.Cell(k, m).Range.Text = "";
                                            table.Cell(k, m).Range.Text = text;
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }
                }
                this.m_wdApp.ActiveWindow.ActivePane.View.SeekView = WdSeekView.wdSeekCurrentPageFooter;
                this.m_wdApp.Selection.WholeStory();
                if ((this.m_wdApp.Selection.Fields != null) && (this.m_wdApp.Selection.Fields.Count > 0))
                {
                    hashtable2.Clear();
                    hashtable.Clear();
                    for (int n = 1; n <= this.m_wdApp.Selection.Fields.Count; n++)
                    {
                        if (this.m_wdApp.Selection.Fields[n].Type == WdFieldType.wdFieldAddin)
                        {
                            data = this.m_wdApp.Selection.Fields[n].Data;
                            if (string.IsNullOrEmpty(data))
                            {
                                continue;
                            }
                            if ((data.Substring(data.Length - 1) != "F") && !this.m_wdApp.Selection.Fields[n].Locked)
                            {
                                if (data.Contains("Pic:"))
                                {
                                    string str12 = this.m_wdApp.Selection.Fields[n].Index.ToString() + this.m_wdApp.Selection.Fields[n].Code.Text;
                                    if (!hashtable2.ContainsKey(str12))
                                    {
                                        hashtable2.Add(str12, this.m_wdApp.Selection.Fields[n]);
                                    }
                                    continue;
                                }
                                str2 = this.GetExpressValue(oProjectOrDoc, htUserKeyWord, data);
                                if (this.IsFinial && string.IsNullOrEmpty(str2))
                                {
                                    str2 = " ";
                                }
                                this.FillAddInFiled(str2, this.m_wdApp.Selection.Fields[n]);
                            }
                        }
                        else if (((this.m_wdApp.Selection.Fields[n].Type == WdFieldType.wdFieldIncludePicture) && !this.m_wdApp.Selection.Fields[n].Locked) && !this.FillPicField(oProjectOrDoc, htUserKeyWord, this.m_wdApp.Selection.Fields[n]))
                        {
                            hashtable.Add(this.m_wdApp.Selection.Fields[n].Index.ToString() + this.m_wdApp.Selection.Fields[n].Code.Text, this.m_wdApp.Selection.Fields[n]);
                        }
                        this.m_wdApp.Selection.WholeStory();
                    }
                    foreach (DictionaryEntry entry3 in hashtable2)
                    {
                        Field field5 = entry3.Value as Field;
                        string str14 = field5.Data.Split(new string[] { "Pic:" }, StringSplitOptions.RemoveEmptyEntries)[0];
                        int num8 = str14.IndexOf("INCLUDEPICTURE") + 14;
                        string str15 = str14.Substring(num8, str14.LastIndexOf(@"\") - num8);
                        field5.Select();
                        field5.Delete();
                        Field field6 = this.InsertPictureField(str15);
                        if (!this.FillPicField(oProjectOrDoc, htUserKeyWord, field6) && !hashtable.ContainsKey(field6.Index.ToString() + field6.Code.Text))
                        {
                            hashtable.Add(field6.Index.ToString() + field6.Code.Text, field6);
                        }
                    }
                    using (IDictionaryEnumerator enumerator = hashtable.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            DictionaryEntry entry5 = (DictionaryEntry) enumerator.Current;
                        }
                    }
                }
                if ((this.m_wdApp.Selection.Tables != null) && (this.m_wdApp.Selection.Tables.Count > 0))
                {
                    foreach (Table table2 in this.m_wdApp.Selection.Tables)
                    {
                        for (int num9 = 0; num9 < table2.Rows.Count; num9++)
                        {
                            for (int num10 = 0; num10 < table2.Columns.Count; num10++)
                            {
                                try
                                {
                                    if (table2.Cell(num9, num10).FitText)
                                    {
                                        string str16 = table2.Cell(num9, num10).Range.Text;
                                        if (str16.Contains("\r\a"))
                                        {
                                            str16 = str16.Replace("\r\a", " ");
                                        }
                                        if (!string.IsNullOrEmpty(str16.Trim()) && (Asc(str16.Substring(0, 1)) > 30))
                                        {
                                            table2.Cell(num9, num10).Range.Text = "";
                                            table2.Cell(num9, num10).Range.Text = str16;
                                        }
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                }
                if (this.m_wdDoc != null)
                {
                    foreach (Table table3 in this.m_wdDoc.Tables)
                    {
                        for (int num11 = 0; num11 < table3.Rows.Count; num11++)
                        {
                            for (int num12 = 0; num12 < table3.Columns.Count; num12++)
                            {
                                try
                                {
                                    if (table3.Cell(num11, num12).FitText)
                                    {
                                        string str17 = table3.Cell(num11, num12).Range.Text;
                                        if (str17.Contains("\r\a"))
                                        {
                                            str17 = str17.Replace("\r\a", " ");
                                        }
                                        if (!string.IsNullOrEmpty(str17.Trim()) && (Asc(str17.Substring(0, 1)) > 30))
                                        {
                                            table3.Cell(num11, num12).Range.Text = "";
                                            table3.Cell(num11, num12).Range.Text = str17;
                                        }
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception exception)
            {
                if (this._errorList != null)
                {
                    this._errorList.Add(exception.Message);
                }
                CDMS.Write(exception.Message);
                this.Release(true);
            }
            return false;
        }

        public bool FillWdFieldValueUpdateData(object oProjectOrDoc, Hashtable htUserKeyWord)
        {
            bool flag;
            try
            {
                string data;
                string str2;
                this.picFieldList.Clear();
                if (oProjectOrDoc == null)
                {
                    return false;
                }
                Hashtable hashtable = new Hashtable();
                Hashtable hashtable2 = new Hashtable();
                if ((this.m_wdDoc.Fields != null) && (this.m_wdDoc.Fields.Count > 0))
                {
                    for (int i = 1; i <= this.m_wdDoc.Fields.Count; i++)
                    {
                        this.m_wdDoc.Fields[i].Update();
                        if (this.m_wdDoc.Fields[i].Type == WdFieldType.wdFieldAddin)
                        {
                            data = this.m_wdDoc.Fields[i].Data;
                            if (!string.IsNullOrEmpty(data) && (data.Substring(data.Length - 1) != "F"))
                            {
                                if (this.m_wdDoc.Fields[i].Locked)
                                {
                                    this.m_wdDoc.Fields[i].Locked = false;
                                }
                                if (data.Contains("Pic:"))
                                {
                                    string key = this.m_wdDoc.Fields[i].Index.ToString() + this.m_wdDoc.Fields[i].Code.Text;
                                    if (!hashtable2.ContainsKey(key))
                                    {
                                        hashtable2.Add(key, this.m_wdDoc.Fields[i]);
                                    }
                                }
                                else
                                {
                                    str2 = this.GetExpressValue(oProjectOrDoc, htUserKeyWord, data);
                                    if (this.IsFinial && string.IsNullOrEmpty(str2))
                                    {
                                        str2 = " ";
                                    }
                                    this.FillAddInFiled(str2, this.m_wdDoc.Fields[i]);
                                }
                            }
                        }
                        else if (this.m_wdDoc.Fields[i].Type == WdFieldType.wdFieldIncludePicture)
                        {
                            if (this.m_wdDoc.Fields[i].Locked)
                            {
                                this.m_wdDoc.Fields[i].Locked = false;
                            }
                            this.FillPicField(oProjectOrDoc, htUserKeyWord, this.m_wdDoc.Fields[i], true);
                        }
                    }
                    foreach (DictionaryEntry entry in hashtable2)
                    {
                        Field field = entry.Value as Field;
                        string str5 = field.Data.Split(new string[] { "Pic:" }, StringSplitOptions.RemoveEmptyEntries)[0];
                        int startIndex = str5.IndexOf("INCLUDEPICTURE") + 14;
                        string sKeyWord = str5.Substring(startIndex, str5.LastIndexOf(@"\") - startIndex);
                        field.Select();
                        field.Delete();
                        Field wdField = this.InsertPictureField(sKeyWord);
                        if (!this.FillPicField(oProjectOrDoc, htUserKeyWord, wdField) && !hashtable.ContainsKey(wdField.Index.ToString() + wdField.Code.Text))
                        {
                            hashtable.Add(wdField.Index.ToString() + wdField.Code.Text, wdField);
                        }
                    }
                }
                this.FillShapeFieldUpdateData(oProjectOrDoc, htUserKeyWord);
                this.m_wdApp.ActiveWindow.ActivePane.View.SeekView = WdSeekView.wdSeekCurrentPageHeader;
                this.m_wdApp.Selection.WholeStory();
                if ((this.m_wdApp.Selection.Fields != null) && (this.m_wdApp.Selection.Fields.Count > 0))
                {
                    hashtable2.Clear();
                    hashtable.Clear();
                    for (int j = 1; j <= this.m_wdApp.Selection.Fields.Count; j++)
                    {
                        if (this.m_wdApp.Selection.Fields[j].Type == WdFieldType.wdFieldAddin)
                        {
                            data = this.m_wdApp.Selection.Fields[j].Data;
                            if (string.IsNullOrEmpty(data))
                            {
                                continue;
                            }
                            if (data.Substring(data.Length - 1) != "F")
                            {
                                if (this.m_wdApp.Selection.Fields[j].Locked)
                                {
                                    this.m_wdApp.Selection.Fields[j].Locked = false;
                                }
                                if (data.Contains("Pic:"))
                                {
                                    string str7 = this.m_wdApp.Selection.Fields[j].Index.ToString() + this.m_wdApp.Selection.Fields[j].Code.Text;
                                    if (!hashtable2.ContainsKey(str7))
                                    {
                                        hashtable2.Add(str7, this.m_wdApp.Selection.Fields[j]);
                                    }
                                    continue;
                                }
                                str2 = this.GetExpressValue(oProjectOrDoc, htUserKeyWord, data);
                                if (this.IsFinial && string.IsNullOrEmpty(str2))
                                {
                                    str2 = " ";
                                }
                                this.FillAddInFiled(str2, this.m_wdApp.Selection.Fields[j]);
                            }
                        }
                        else if (this.m_wdApp.Selection.Fields[j].Type == WdFieldType.wdFieldIncludePicture)
                        {
                            if (this.m_wdApp.Selection.Fields[j].Locked)
                            {
                                this.m_wdApp.Selection.Fields[j].Locked = false;
                            }
                            if (!this.FillPicField(oProjectOrDoc, htUserKeyWord, this.m_wdApp.Selection.Fields[j]) && !hashtable.ContainsKey(this.m_wdApp.Selection.Fields[j].Index.ToString() + this.m_wdApp.Selection.Fields[j].Code.Text))
                            {
                                hashtable.Add(this.m_wdApp.Selection.Fields[j].Index.ToString() + this.m_wdApp.Selection.Fields[j].Code.Text, this.m_wdApp.Selection.Fields[j]);
                            }
                        }
                        this.m_wdApp.Selection.WholeStory();
                    }
                    foreach (DictionaryEntry entry2 in hashtable2)
                    {
                        Field field3 = entry2.Value as Field;
                        string str9 = field3.Data.Split(new string[] { "Pic:" }, StringSplitOptions.RemoveEmptyEntries)[0];
                        int num4 = str9.IndexOf("INCLUDEPICTURE") + 14;
                        string str10 = str9.Substring(num4, str9.LastIndexOf(@"\") - num4);
                        field3.Select();
                        field3.Delete();
                        Field field4 = this.InsertPictureField(str10);
                        if (!this.FillPicField(oProjectOrDoc, htUserKeyWord, field4) && !hashtable.ContainsKey(field4.Index.ToString() + field4.Code.Text))
                        {
                            hashtable.Add(field4.Index.ToString() + field4.Code.Text, field4);
                        }
                    }
                }
                if ((this.m_wdApp.Selection.Tables != null) && (this.m_wdApp.Selection.Tables.Count > 0))
                {
                    foreach (Table table in this.m_wdApp.Selection.Tables)
                    {
                        for (int k = 0; k < table.Rows.Count; k++)
                        {
                            for (int m = 0; m < table.Columns.Count; m++)
                            {
                                try
                                {
                                    if (table.Cell(k, m).FitText)
                                    {
                                        string text = table.Cell(k, m).Range.Text;
                                        if (text.Contains("\r\a"))
                                        {
                                            text = text.Replace("\r\a", " ");
                                        }
                                        if (!string.IsNullOrEmpty(text.Trim()) && (Asc(text.Substring(0, 1)) > 30))
                                        {
                                            table.Cell(k, m).Range.Text = "";
                                            table.Cell(k, m).Range.Text = text;
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    throw;
                                }
                            }
                        }
                    }
                }
                this.m_wdApp.ActiveWindow.ActivePane.View.SeekView = WdSeekView.wdSeekCurrentPageFooter;
                this.m_wdApp.Selection.WholeStory();
                if ((this.m_wdApp.Selection.Fields != null) && (this.m_wdApp.Selection.Fields.Count > 0))
                {
                    hashtable2.Clear();
                    hashtable.Clear();
                    for (int n = 1; n <= this.m_wdApp.Selection.Fields.Count; n++)
                    {
                        if (this.m_wdApp.Selection.Fields[n].Type == WdFieldType.wdFieldAddin)
                        {
                            data = this.m_wdApp.Selection.Fields[n].Data;
                            if (string.IsNullOrEmpty(data))
                            {
                                continue;
                            }
                            if (data.Substring(data.Length - 1) != "F")
                            {
                                if (this.m_wdApp.Selection.Fields[n].Locked)
                                {
                                    this.m_wdApp.Selection.Fields[n].Locked = false;
                                }
                                if (data.Contains("Pic:"))
                                {
                                    string str12 = this.m_wdApp.Selection.Fields[n].Index.ToString() + this.m_wdApp.Selection.Fields[n].Code.Text;
                                    if (!hashtable2.ContainsKey(str12))
                                    {
                                        hashtable2.Add(str12, this.m_wdApp.Selection.Fields[n]);
                                    }
                                    continue;
                                }
                                str2 = this.GetExpressValue(oProjectOrDoc, htUserKeyWord, data);
                                if (this.IsFinial && string.IsNullOrEmpty(str2))
                                {
                                    str2 = " ";
                                }
                                this.FillAddInFiled(str2, this.m_wdApp.Selection.Fields[n]);
                            }
                        }
                        else if (this.m_wdApp.Selection.Fields[n].Type == WdFieldType.wdFieldIncludePicture)
                        {
                            if (this.m_wdApp.Selection.Fields[n].Locked)
                            {
                                this.m_wdApp.Selection.Fields[n].Locked = false;
                            }
                            if (!this.FillPicField(oProjectOrDoc, htUserKeyWord, this.m_wdApp.Selection.Fields[n]) && !hashtable.ContainsKey(this.m_wdApp.Selection.Fields[n].Index.ToString() + this.m_wdApp.Selection.Fields[n].Code.Text))
                            {
                                hashtable.Add(this.m_wdApp.Selection.Fields[n].Index.ToString() + this.m_wdApp.Selection.Fields[n].Code.Text, this.m_wdApp.Selection.Fields[n]);
                            }
                        }
                        this.m_wdApp.Selection.WholeStory();
                    }
                    foreach (DictionaryEntry entry3 in hashtable2)
                    {
                        Field field5 = entry3.Value as Field;
                        string str14 = field5.Data.Split(new string[] { "Pic:" }, StringSplitOptions.RemoveEmptyEntries)[0];
                        int num8 = str14.IndexOf("INCLUDEPICTURE") + 14;
                        string str15 = str14.Substring(num8, str14.LastIndexOf(@"\") - num8);
                        field5.Select();
                        field5.Delete();
                        Field field6 = this.InsertPictureField(str15);
                        if (!this.FillPicField(oProjectOrDoc, htUserKeyWord, field6) && !hashtable.ContainsKey(field6.Index.ToString() + field6.Code.Text))
                        {
                            hashtable.Add(field6.Index.ToString() + field6.Code.Text, field6);
                        }
                    }
                }
                if ((this.m_wdApp.Selection.Tables != null) && (this.m_wdApp.Selection.Tables.Count > 0))
                {
                    foreach (Table table2 in this.m_wdApp.Selection.Tables)
                    {
                        for (int num9 = 0; num9 < table2.Rows.Count; num9++)
                        {
                            for (int num10 = 0; num10 < table2.Columns.Count; num10++)
                            {
                                try
                                {
                                    if (table2.Cell(num9, num10).FitText)
                                    {
                                        string str16 = table2.Cell(num9, num10).Range.Text;
                                        if (str16.Contains("\r\a"))
                                        {
                                            str16 = str16.Replace("\r\a", " ");
                                        }
                                        if (!string.IsNullOrEmpty(str16.Trim()) && (Asc(str16.Substring(0, 1)) > 30))
                                        {
                                            table2.Cell(num9, num10).Range.Text = "";
                                            table2.Cell(num9, num10).Range.Text = str16;
                                        }
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                }
                if (this.m_wdDoc != null)
                {
                    foreach (Table table3 in this.m_wdDoc.Tables)
                    {
                        for (int num11 = 0; num11 < table3.Rows.Count; num11++)
                        {
                            for (int num12 = 0; num12 < table3.Columns.Count; num12++)
                            {
                                try
                                {
                                    if (table3.Cell(num11, num12).FitText)
                                    {
                                        string str17 = table3.Cell(num11, num12).Range.Text;
                                        if (str17.Contains("\r\a"))
                                        {
                                            str17 = str17.Replace("\r\a", " ");
                                        }
                                        if (!string.IsNullOrEmpty(str17.Trim()) && (Asc(str17.Substring(0, 1)) > 30))
                                        {
                                            table3.Cell(num11, num12).Range.Text = "";
                                            table3.Cell(num11, num12).Range.Text = str17;
                                        }
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                }
                flag = true;
            }
            catch (Exception exception)
            {
                if (this._errorList != null)
                {
                    this._errorList.Add(exception.Message);
                }
                CDMS.Write(exception.Message);
                this.Release(true);
                throw;
            }
            return flag;
        }

        private void FillWordFieldNew(Field field, object oProjectOrDoc, Hashtable htUserKeyWord, bool IsDebug)
        {
            field.Locked = false;
            WdFieldType type = field.Type;
            if (type == WdFieldType.wdFieldIncludePicture)
            {
                string text = field.Code.Text;
                if (text.Contains("INCLUDEPICTURE"))
                {
                    int startIndex = text.IndexOf("INCLUDEPICTURE") + 14;
                    string sourceName = null;
                    if (!this.IsWord07)
                    {
                        if (text.Contains<char>('\\'))
                        {
                            sourceName = text.Substring(startIndex, text.LastIndexOf(@"\") - startIndex);
                        }
                    }
                    else
                    {
                        sourceName = field.LinkFormat.SourceName;
                    }
                    if (!string.IsNullOrEmpty(sourceName))
                    {
                        string str7 = this.GetPictureNew(oProjectOrDoc, htUserKeyWord, sourceName, IsDebug);
                        if (!string.IsNullOrEmpty(str7))
                        {
                            string str8 = text;
                            if (((!str7.Contains("blank.jpg") || !str8.Contains("@@@Pic:")) || this.IsUpdateVersion) && (!str7.Contains("blank.jpg") || this.m_bForceReWrite))
                            {
                                if (str8.Contains("@@@Pic:"))
                                {
                                    str8 = text.Substring(0, text.LastIndexOf("@@@Pic:"));
                                }
                                field.InlineShape.LinkFormat.SourceFullName = str7;
                                field.InlineShape.LinkFormat.SavePictureWithDocument = true;
                                field.Code.Text = str8 + "@@@Pic:" + field.InlineShape.LinkFormat.SourceName;
                            }
                        }
                    }
                }
            }
            else if (type == WdFieldType.wdFieldAddin)
            {
                string data = field.Data;
                string str2 = "";
                int length = -1;
                bool isFinish = false;
                if (data.Contains("@@@"))
                {
                    string[] strArray = data.Split("@@@".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    str2 = strArray[0];
                    for (int i = 1; i < strArray.Length; i++)
                    {
                        string str3 = strArray[i];
                        if (str3.Contains("Length:"))
                        {
                            length = Convert.ToInt32(str3.Substring(7));
                        }
                        if (str3.Contains("ImageFinish"))
                        {
                            isFinish = true;
                        }
                    }
                }
                else
                {
                    str2 = data;
                }
                if (!string.IsNullOrEmpty(str2))
                {
                    if (str2.Contains(":"))
                    {
                        string[] commands = str2.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        switch (commands[0])
                        {
                            case "IMAGETABLE":
                                this.FillImageTableField(field, oProjectOrDoc, htUserKeyWord, commands, isFinish, IsDebug);
                                break;

                            case "TEXTTABLE":
                                this.FillTextTableField(field, oProjectOrDoc, htUserKeyWord, commands, length, IsDebug);
                                break;

                            case "MULTIPICS":
                                this.FillMultiPicField(field, oProjectOrDoc, htUserKeyWord, commands, length, IsDebug);
                                return;
                        }
                    }
                    else
                    {
                        this.FillSingleValueField(field, oProjectOrDoc, htUserKeyWord, str2, length, IsDebug);
                    }
                }
            }
        }

        ~CDMSOffice()
        {
            try
            {
                this.Release(true);
            }
            catch
            {
            }
        }

        private void FitWordTable()
        {
            if ((this.m_wdApp.Selection.Tables != null) && (this.m_wdApp.Selection.Tables.Count > 0))
            {
                foreach (Table table in this.m_wdApp.Selection.Tables)
                {
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        for (int j = 0; j < table.Columns.Count; j++)
                        {
                            try
                            {
                                if (table.Cell(i, j).FitText)
                                {
                                    string text = table.Cell(i, j).Range.Text;
                                    if (text.Contains("\r\a"))
                                    {
                                        text = text.Replace("\r\a", " ");
                                    }
                                    if (!string.IsNullOrEmpty(text.Trim()) && (Asc(text.Substring(0, 1)) > 30))
                                    {
                                        table.Cell(i, j).Range.Text = "";
                                        table.Cell(i, j).Range.Text = text;
                                    }
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
            }
            if ((this.m_wdApp.Selection.Tables != null) && (this.m_wdApp.Selection.Tables.Count > 0))
            {
                foreach (Table table2 in this.m_wdApp.Selection.Tables)
                {
                    for (int k = 0; k < table2.Rows.Count; k++)
                    {
                        for (int m = 0; m < table2.Columns.Count; m++)
                        {
                            try
                            {
                                if (table2.Cell(k, m).FitText)
                                {
                                    string str2 = table2.Cell(k, m).Range.Text;
                                    if (str2.Contains("\r\a"))
                                    {
                                        str2 = str2.Replace("\r\a", " ");
                                    }
                                    if (!string.IsNullOrEmpty(str2.Trim()) && (Asc(str2.Substring(0, 1)) > 30))
                                    {
                                        table2.Cell(k, m).Range.Text = "";
                                        table2.Cell(k, m).Range.Text = str2;
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
            if (this.m_wdDoc != null)
            {
                foreach (Table table3 in this.m_wdDoc.Tables)
                {
                    for (int n = 0; n < table3.Rows.Count; n++)
                    {
                        for (int num6 = 0; num6 < table3.Columns.Count; num6++)
                        {
                            try
                            {
                                if (table3.Cell(n, num6).FitText)
                                {
                                    string str3 = table3.Cell(n, num6).Range.Text;
                                    if (str3.Contains("\r\a"))
                                    {
                                        str3 = str3.Replace("\r\a", " ");
                                    }
                                    if (!string.IsNullOrEmpty(str3.Trim()) && (Asc(str3.Substring(0, 1)) > 30))
                                    {
                                        table3.Cell(n, num6).Range.Text = "";
                                        table3.Cell(n, num6).Range.Text = str3;
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
        }

        public static Hashtable GetCellDataFromWord(string wordFilePath)
        {
            Hashtable hashtable = new Hashtable();
            try
            {
                if ((string.IsNullOrEmpty(wordFilePath) || !File.Exists(wordFilePath)) || (!wordFilePath.ToLower().EndsWith(".doc") && !wordFilePath.ToLower().EndsWith(".docx")))
                {
                    return null;
                }
                object confirmConversions = Missing.Value;
                Application application = new Microsoft.Office.Interop.Excel.ApplicationClass();
                Document activeDocument = null;
                object fileName = wordFilePath;
                application.Documents.Open(ref fileName, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions);
                activeDocument = application.ActiveDocument;
                if (activeDocument == null)
                {
                    application.Quit(ref confirmConversions, ref confirmConversions, ref confirmConversions);
                    return null;
                }
                if ((activeDocument.Fields != null) && (activeDocument.Fields.Count > 0))
                {
                    foreach (Field field in activeDocument.Fields)
                    {
                        if (field.Type == WdFieldType.wdFieldAddin)
                        {
                            string data = "";
                            string str2 = "";
                            data = field.Data;
                            if (!string.IsNullOrEmpty(data) && data.ToUpper().StartsWith("CELLDATA:"))
                            {
                                str2 = data.ToUpper().Replace("CELLDATA:", "");
                                if (!string.IsNullOrEmpty(str2))
                                {
                                    try
                                    {
                                        if (((field.Code != null) && (field.Code.Cells != null)) && (field.Code.Cells.Count > 0))
                                        {
                                            field.Code.Cells[1].Select();
                                            if (!hashtable.Contains(str2))
                                            {
                                                hashtable.Add(str2, application.Selection.Text);
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                            }
                        }
                    }
                }
                activeDocument.Close(ref confirmConversions, ref confirmConversions, ref confirmConversions);
                application.Quit(ref confirmConversions, ref confirmConversions, ref confirmConversions);
            }
            catch (Exception exception)
            {
                CDMS.Write(exception.ToString());
                return null;
            }
            return hashtable;
        }

        public static int GetColumnIndexByValue(Microsoft.Office.Interop.Excel.Worksheet sWorkSheet, int iRow, string value)
        {
            int num = 0;
            try
            {
                if (((sWorkSheet == null) || (iRow <= 0)) || string.IsNullOrEmpty(value))
                {
                    return 0;
                }
                object[,] objArray = null;
                objArray = (object[,]) sWorkSheet.UsedRange.Value2;
                if (objArray == null)
                {
                    return 0;
                }
                for (int i = 1; i <= objArray.GetLength(1); i++)
                {
                    if ((objArray[2, i] != null) && (objArray[2, i].ToString() == value))
                    {
                        return i;
                    }
                }
                return num;
            }
            catch (Exception exception)
            {
                CDMS.Write(exception.ToString());
            }
            return 0;
        }

        public static int GetCopyCount(int iNeedRow, int iFixRow, int iCopyRow, bool bCopyRowContainFix)
        {
            int num = 0;
            try
            {
                if ((iNeedRow <= 0) || (iCopyRow <= 0))
                {
                    return 0;
                }
                int num2 = iNeedRow;
                if (iFixRow >= 0)
                {
                    num2 = iNeedRow - iFixRow;
                }
                if (!bCopyRowContainFix)
                {
                    num2 -= iCopyRow;
                }
                while (num2 > 0)
                {
                    num2 -= iCopyRow;
                    num++;
                }
                return num;
            }
            catch
            {
            }
            return 0;
        }

        public Hashtable GetExcelData(int rowNo, string[] colNames)
        {
            Hashtable hashtable = null;
            try
            {
                if (rowNo <= 0)
                {
                    this._errorList.Add("行号必须大于等于1！");
                    return hashtable;
                }
                if ((colNames == null) || (colNames.Length <= 0))
                {
                    this._errorList.Add("列名数组中没有任何数据！");
                    return hashtable;
                }
                if (this.m_exWorksheet == null)
                {
                    return hashtable;
                }
                hashtable = new Hashtable();
                for (int i = 0; i < colNames.Length; i++)
                {
                    if (!hashtable.ContainsKey(colNames[i]))
                    {
                        string text = ((Microsoft.Office.Interop.Excel.Range) this.m_exWorksheet.Cells[rowNo, this.ConverToInt(colNames[i])]).Text as string;
                        if (text != null)
                        {
                            hashtable.Add(colNames[i], text);
                        }
                        else
                        {
                            hashtable.Add("RESULT", "EXCEPTION");
                        }
                    }
                }
                hashtable.Add("RESULT", "OK");
            }
            catch (Exception exception)
            {
                this._errorList.Add(exception.ToString());
                CDMS.Write(exception.ToString());
                hashtable.Add("RESULT", "EXCEPTION");
            }
            return hashtable;
        }

        protected string GetExpression(object oProjectOrDoc)
        {
            string keyWord = "";
            try
            {
                if (oProjectOrDoc == null)
                {
                    return "";
                }
                DBSource dBSource = null;
                if (oProjectOrDoc is Project)
                {
                    dBSource = (oProjectOrDoc as Project).dBSource;
                }
                else if (oProjectOrDoc is Doc)
                {
                    dBSource = (oProjectOrDoc as Doc).dBSource;
                }
                if (dBSource == null)
                {
                    return "";
                }
                fmSelKeyWord word = new fmSelKeyWord(dBSource) {
                    IsExpression = true
                };
                word.ShowDialog();
                if ((word.KeyWord != null) && (word.KeyWord.Trim().Length > 0))
                {
                    keyWord = word.KeyWord;
                }
            }
            catch (Exception exception)
            {
                this._errorList.Add(exception.ToString());
                CDMS.Write(exception.ToString());
            }
            return keyWord;
        }

        protected string GetExpressValue(object oProjectOrDoc, Hashtable htUserKeyWord, string sExpress)
        {
            try
            {
                string[] strArray = new string[0];
                if (oProjectOrDoc is Project)
                {
                    strArray = (oProjectOrDoc as Project).ExcuteDefnExpression(sExpress.Trim(), htUserKeyWord);
                }
                else if (oProjectOrDoc is Doc)
                {
                    strArray = (oProjectOrDoc as Doc).ExcuteDefnExpression(sExpress.Trim(), htUserKeyWord);
                }
                if (((strArray.Length > 0) && (strArray[0].Trim().Length > 0)) && (strArray[0] != sExpress))
                {
                    return strArray[0].Trim();
                }
            }
            catch (Exception exception)
            {
                if (this._errorList != null)
                {
                    this._errorList.Add(exception.Message);
                }
                CDMS.Write(exception.ToString());
                throw;
            }
            return "";
        }

        public static List<string> GetFileSizeAndPages(string filepath)
        {
            if (!File.Exists(filepath))
            {
                return null;
            }
            try
            {
                List<string> list = new List<string>();
                string item = "A4";
                int count = 0;
                if (filepath.ToLower().EndsWith(".xls") || filepath.ToLower().EndsWith(".xlsx"))
                {
                    Application application = null;
                    try
                    {
                        application = new Microsoft.Office.Interop.Excel.ApplicationClass();
                        object updateLinks = Missing.Value;
                        Workbook workbook = application.Workbooks.Open(filepath, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks);
                        Microsoft.Office.Interop.Excel.Worksheet activeSheet = (Microsoft.Office.Interop.Excel.Worksheet) workbook.ActiveSheet;
                        string str2 = activeSheet.PageSetup.PaperSize.ToString();
                        if (str2.StartsWith("xlPaper"))
                        {
                            item = str2.Substring(7);
                        }
                        for (int i = 1; i <= workbook.Sheets.Count; i++)
                        {
                            Microsoft.Office.Interop.Excel.Worksheet worksheet2 = (Microsoft.Office.Interop.Excel.Worksheet) workbook.Sheets[i];
                            if (worksheet2.UsedRange.Value2 != null)
                            {
                                worksheet2.Activate();
                                count += int.Parse(application.ExecuteExcel4Macro("Get.Document(50)").ToString());
                            }
                        }
                        activeSheet.Activate();
                        workbook.Close(updateLinks, updateLinks, updateLinks);
                        application.Quit();
                    }
                    catch (Exception exception)
                    {
                        if (application != null)
                        {
                            application.Quit();
                        }
                        CDMS.Write("统计Execl页数出错:" + filepath + " " + exception.ToString());
                    }
                }
                else if (filepath.ToLower().EndsWith(".doc") || filepath.ToLower().EndsWith(".docx"))
                {
                    Application application2 = null;
                    object confirmConversions = Missing.Value;
                    try
                    {
                        application2 = new Microsoft.Office.Interop.Excel.ApplicationClass();
                        object fileName = filepath;
                        Document document = application2.Documents.Open(ref fileName, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions);
                        string str3 = document.PageSetup.PaperSize.ToString();
                        if (str3.StartsWith("wdPaper"))
                        {
                            item = str3.Substring(7);
                        }
                        count = document.ComputeStatistics(WdStatistic.wdStatisticPages, ref confirmConversions);
                        document.Close(ref confirmConversions, ref confirmConversions, ref confirmConversions);
                        application2.Quit(ref confirmConversions, ref confirmConversions, ref confirmConversions);
                    }
                    catch (Exception exception2)
                    {
                        if (application2 != null)
                        {
                            application2.Quit(ref confirmConversions, ref confirmConversions, ref confirmConversions);
                        }
                        CDMS.Write("统计Word页数出错:" + filepath + " " + exception2.ToString());
                    }
                }
                else if (filepath.ToLower().EndsWith(".pdf"))
                {
                    FileStream stream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
                    string input = new StreamReader(stream).ReadToEnd();
                    Regex regex = new Regex(@"/Type\s*/Page[^s]");
                    count = regex.Matches(input).Count;
                }
                list.Add(item);
                list.Add(count.ToString());
                return list;
            }
            catch (Exception exception3)
            {
                CDMS.Write(exception3.ToString());
                return null;
            }
        }

        public static List<Microsoft.Office.Interop.Excel.Range> GetHasValueRanges(Microsoft.Office.Interop.Excel.Range fromRange)
        {
            try
            {
                if (((fromRange == null) || (fromRange.Cells == null)) || (fromRange.Cells.Count <= 0))
                {
                    return null;
                }
                List<Microsoft.Office.Interop.Excel.Range> list = new List<Microsoft.Office.Interop.Excel.Range>();
                object[,] objArray = null;
                objArray = (object[,]) fromRange.Value2;
                if (objArray == null)
                {
                    return null;
                }
                for (int i = 1; i <= objArray.GetLength(0); i++)
                {
                    for (int j = 1; j <= objArray.GetLength(1); j++)
                    {
                        if ((objArray[i, j] != null) && !string.IsNullOrEmpty(objArray[i, j].ToString()))
                        {
                            list.Add(fromRange.Cells[i, j] as Range);
                        }
                    }
                }
                return list;
            }
            catch (Exception exception)
            {
                CDMS.Write(exception.ToString());
            }
            return null;
        }

        public static int GetHasValueRowsCount(Microsoft.Office.Interop.Excel.Range fromRange)
        {
            int num = 0;
            try
            {
                if (((fromRange == null) || (fromRange.Cells == null)) || (fromRange.Cells.Count <= 0))
                {
                    return 0;
                }
                new List<Microsoft.Office.Interop.Excel.Range>();
                object[,] objArray = null;
                objArray = (object[,]) fromRange.Value2;
                if (objArray == null)
                {
                    return 0;
                }
                for (int i = 1; i <= objArray.GetLength(0); i++)
                {
                    bool flag = false;
                    for (int j = 1; j <= objArray.GetLength(1); j++)
                    {
                        if ((objArray[i, j] != null) && !string.IsNullOrEmpty(objArray[i, j].ToString()))
                        {
                            flag = true;
                        }
                    }
                    if (flag)
                    {
                        num++;
                    }
                }
                return num;
            }
            catch (Exception exception)
            {
                CDMS.Write(exception.ToString());
            }
            return num;
        }

        public static string GetPageSize(string filepath)
        {
            if (!File.Exists(filepath))
            {
                return "";
            }
            try
            {
                string str = "A4";
                if (filepath.ToLower().EndsWith(".xls") || filepath.ToLower().EndsWith(".xlsx"))
                {
                    Application application = new Microsoft.Office.Interop.Excel.ApplicationClass();
                    object updateLinks = Missing.Value;
                    Workbook workbook = application.Workbooks.Open(filepath, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks, updateLinks);
                    Microsoft.Office.Interop.Excel.Worksheet activeSheet = workbook.ActiveSheet as Worksheet;
                    string str2 = activeSheet.PageSetup.PaperSize.ToString();
                    if (str2.StartsWith("xlPaper"))
                    {
                        str = str2.Substring(7);
                    }
                    workbook.Close(updateLinks, updateLinks, updateLinks);
                    application.Quit();
                }
                else if (filepath.ToLower().EndsWith(".doc") || filepath.ToLower().EndsWith(".docx"))
                {
                    Application application2 = new Microsoft.Office.Interop.Excel.ApplicationClass();
                    object confirmConversions = Missing.Value;
                    object fileName = filepath;
                    Document document = application2.Documents.Open(ref fileName, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions, ref confirmConversions);
                    string str3 = document.PageSetup.PaperSize.ToString();
                    if (str3.StartsWith("wdPaper"))
                    {
                        str = str3.Substring(7);
                    }
                    document.Close(ref confirmConversions, ref confirmConversions, ref confirmConversions);
                    application2.Quit(ref confirmConversions, ref confirmConversions, ref confirmConversions);
                }
                return str;
            }
            catch (Exception exception)
            {
                CDMS.Write(exception.ToString());
                return "";
            }
        }

        protected string GetPicture(object oProjectOrDoc, Hashtable htUserKeyWord, string sExprInField)
        {
            string sLocalFileName = "";
            try
            {
                string workingPath = "";
                string str2 = this.GetExpressValue(oProjectOrDoc, htUserKeyWord, sExprInField);
                if (string.IsNullOrEmpty(str2))
                {
                    return "";
                }
                if (this.IsFinial && (str2.Substring(0, 1) == "."))
                {
                    str2 = "BLANK.jpg";
                }
                if (!(str2.Substring(0, 1) != "."))
                {
                    return sLocalFileName;
                }
                if (oProjectOrDoc is Project)
                {
                    if ((oProjectOrDoc as Project).dBSource.LoginUser.WorkingPath != null)
                    {
                        workingPath = (oProjectOrDoc as Project).dBSource.LoginUser.WorkingPath;
                    }
                }
                else if ((oProjectOrDoc is Doc) && ((oProjectOrDoc as Doc).dBSource.LoginUser.WorkingPath != null))
                {
                    workingPath = (oProjectOrDoc as Doc).dBSource.LoginUser.WorkingPath;
                }
                workingPath = @"C:\temp\CDMSBMP\";
                if ((str2.Length <= 0) || (workingPath.Length <= 0))
                {
                    return sLocalFileName;
                }
                sLocalFileName = workingPath + str2;
                try
                {
                    this.DownLoadFile(oProjectOrDoc, (this.IsFullPath(str2) ? "" : @"BMP\") + str2, sLocalFileName);
                }
                catch (Exception exception)
                {
                    if (this._errorList != null)
                    {
                        this._errorList.Add(exception.Message);
                    }
                    CDMS.Write(exception.ToString());
                    this.Release(this._closeErrorApp);
                }
            }
            catch (Exception exception2)
            {
                this._errorList.Add(exception2.ToString());
                CDMS.Write(exception2.ToString());
                throw;
            }
            return sLocalFileName;
        }

        private string[] GetPictureExNew(object oProjectOrDoc, Hashtable htUserKeyWord, string Expression, bool IsDebug)
        {
            string[] strArray;
            string str = "";
            string workingPath = "";
            if (oProjectOrDoc is Project)
            {
                if ((oProjectOrDoc as Project).dBSource.LoginUser.WorkingPath != null)
                {
                    workingPath = (oProjectOrDoc as Project).dBSource.LoginUser.WorkingPath;
                }
            }
            else if ((oProjectOrDoc is Doc) && ((oProjectOrDoc as Doc).dBSource.LoginUser.WorkingPath != null))
            {
                workingPath = (oProjectOrDoc as Doc).dBSource.LoginUser.WorkingPath;
            }
            workingPath = @"C:\temp\CDMSBMP\";
            str = this.GetExpressValue(oProjectOrDoc, htUserKeyWord, Expression);
            if (str.ToLower() == ".jpg")
            {
                str = "blank.jpg";
            }
            if (str.Contains(",") && str.Contains(".jpg"))
            {
                if (IsDebug)
                {
                    WriteLog("多值图: " + Expression + " 解析后值为: " + str);
                }
                string[] strArray2 = str.Substring(0, str.LastIndexOf(".jpg")).Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                strArray = new string[strArray2.Length];
                for (int i = 0; i < strArray2.Length; i++)
                {
                    strArray[i] = workingPath + strArray2[i] + ".jpg";
                    this.DownLoadFile(oProjectOrDoc, @"\BMP\" + strArray2[i] + ".jpg", strArray[i]);
                    if (!File.Exists(strArray[i]))
                    {
                        if (IsDebug)
                        {
                            WriteLog("多值图: " + Expression + " 解析后值为: " + strArray2[i] + ".jpg 文件下载失败: " + strArray[i] + "\r\n现已改成用blank.jpg代替");
                        }
                        strArray[i] = workingPath + "blank.jpg";
                        this.DownLoadFile(oProjectOrDoc, @"\BMP\blank.jpg", strArray[i]);
                    }
                }
                return strArray;
            }
            strArray = new string[] { workingPath + str };
            this.DownLoadFile(oProjectOrDoc, @"\BMP\" + str, strArray[0]);
            if (IsDebug)
            {
                WriteLog("IncludePic域: " + Expression + " 解析后值为: " + str);
            }
            if (!File.Exists(strArray[0]))
            {
                if (IsDebug)
                {
                    WriteLog("IncludePic域: " + Expression + " 解析后值为: " + str + " 文件下载失败: " + strArray[0] + "\r\n现已改成用blank.jpg代替");
                }
                strArray[0] = workingPath + "blank.jpg";
                this.DownLoadFile(oProjectOrDoc, @"\BMP\blank.jpg", strArray[0]);
            }
            return strArray;
        }

        private string GetPictureNew(object oProjectOrDoc, Hashtable htUserKeyWord, string Expression, bool IsDebug)
        {
            string sLocalFileName = "";
            string str2 = "";
            string workingPath = "";
            if (oProjectOrDoc is Project)
            {
                if ((oProjectOrDoc as Project).dBSource.LoginUser.WorkingPath != null)
                {
                    workingPath = (oProjectOrDoc as Project).dBSource.LoginUser.WorkingPath;
                }
            }
            else if ((oProjectOrDoc is Doc) && ((oProjectOrDoc as Doc).dBSource.LoginUser.WorkingPath != null))
            {
                workingPath = (oProjectOrDoc as Doc).dBSource.LoginUser.WorkingPath;
            }
            workingPath = @"C:\temp\CDMSBMP\";
            if (this.IsUpdateVersion)
            {
                this.DownLoadFile(oProjectOrDoc, @"\BMP\blank.jpg", workingPath + "blank.jpg");
                if (IsDebug)
                {
                    WriteLog("IncludePic域: " + Expression + " 目前为升版状态,用空白图片代替");
                }
                return (workingPath + "blank.jpg");
            }
            str2 = this.GetExpressValue(oProjectOrDoc, htUserKeyWord, Expression);
            if (str2.ToLower() == ".jpg")
            {
                str2 = "blank.jpg";
            }
            sLocalFileName = workingPath + str2;
            this.DownLoadFile(oProjectOrDoc, @"\BMP\" + str2, sLocalFileName);
            if (IsDebug)
            {
                WriteLog("IncludePic域: " + Expression + " 解析后值为: " + str2);
            }
            if (!File.Exists(sLocalFileName))
            {
                if (IsDebug)
                {
                    WriteLog("IncludePic域: " + Expression + " 解析后值为: " + str2 + " 文件下载失败: " + sLocalFileName + "\r\n现已改成用blank.jpg代替");
                }
                sLocalFileName = workingPath + "blank.jpg";
                this.DownLoadFile(oProjectOrDoc, @"\BMP\blank.jpg", sLocalFileName);
            }
            return sLocalFileName;
        }

        public Hashtable GetWdFieldValue(string sFileName)
        {
            try
            {
                if (File.Exists(sFileName))
                {
                    if ((sFileName.ToUpper().IndexOf(".DOC") > 0) && !this.WordIsRunning(this.m_wdApp, sFileName))
                    {
                        this.OpenWordNew(sFileName);
                    }
                }
                else
                {
                    this._errorList.Add("[" + sFileName + "]不存在!");
                    return null;
                }
            }
            catch (Exception exception)
            {
                this._errorList.Add(exception.Message);
                CDMS.Write(exception.ToString());
            }
            if (this.m_wdDoc == null)
            {
                return null;
            }
            Hashtable hashtable = new Hashtable();
            if ((this.m_wdDoc.Fields != null) && (this.m_wdDoc.Fields.Count > 0))
            {
                foreach (Field field in this.m_wdDoc.Fields)
                {
                    if (field.Type == WdFieldType.wdFieldAddin)
                    {
                        bool showCodes = field.ShowCodes;
                        if (!showCodes)
                        {
                            field.ShowCodes = true;
                        }
                        string text = field.Result.Cells[1].Range.Text;
                        int num = text.LastIndexOf(@"ADDIN  \* MERGEFORMAT");
                        string str4 = "";
                        string str5 = "";
                        if (num > 2)
                        {
                            str4 = text.Substring(0, num - 2);
                        }
                        if ((num > 0) && ((num + 0x19) < text.Length))
                        {
                            str5 = text.Substring(num + 0x17, (text.Length - num) - 0x19);
                        }
                        text = str4 + str5;
                        if (!string.IsNullOrEmpty(text.Trim()))
                        {
                            string data = field.Data;
                            if (data.StartsWith("$"))
                            {
                                data = data.Substring(2);
                                data = data.Substring(0, data.Length - 1);
                            }
                            string str2 = text;
                            if (!hashtable.ContainsKey(data))
                            {
                                hashtable.Add(data, str2);
                            }
                        }
                        field.ShowCodes = showCodes;
                    }
                }
            }
            this.Release(this._closeApp);
            return hashtable;
        }

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, ref uint ProcessId);
        protected bool HasTwoChars(string s)
        {
            int num = 0;
            string str = s;
            for (int i = 0; i < str.Length; i++)
            {
                char ch1 = str[i];
                num++;
            }
            return (num == 2);
        }

        protected void InsertAuditCollection(Hashtable htAuditDataList)
        {
            try
            {
                if ((htAuditDataList != null) && (htAuditDataList.Count > 0))
                {
                    foreach (DictionaryEntry entry in htAuditDataList)
                    {
                        string userKeyWord = "";
                        List<string> dataList = null;
                        if (entry.Key is string)
                        {
                            userKeyWord = (string) entry.Key;
                        }
                        if (entry.Value is List<string>)
                        {
                            dataList = entry.Value as List<string>;
                        }
                        if ((dataList != null) && (dataList.Count > 0))
                        {
                            this.InsertAuditCollection(userKeyWord, dataList);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                this._errorList.Add(exception.ToString());
                CDMS.Write(exception.ToString());
                throw;
            }
        }

        protected void InsertAuditCollection(string userKeyWord, List<string> dataList)
        {
            try
            {
                if ((dataList != null) && (dataList.Count > 0))
                {
                    object wdCell = WdUnits.wdCell;
                    object count = 1;
                    for (int i = 1; i <= this.m_wdDoc.Fields.Count; i++)
                    {
                        this.m_wdDoc.Fields[i].Update();
                        if (this.m_wdDoc.Fields[i].Type == WdFieldType.wdFieldAddin)
                        {
                            string data = this.m_wdDoc.Fields[i].Data;
                            if (data.ToUpper().StartsWith(userKeyWord.ToUpper()))
                            {
                                int num2 = 0;
                                int num3 = 0;
                                char[] separator = new char[] { ':' };
                                string[] strArray = data.Split(separator);
                                if ((strArray == null) || (strArray.Length < 3))
                                {
                                    continue;
                                }
                                num3 = int.Parse(strArray[2]);
                                num2 = int.Parse(strArray[1]) * num3;
                                if (num3 <= 0)
                                {
                                    continue;
                                }
                                this.m_wdDoc.Fields[i].Select();
                                int num4 = dataList.Count;
                                for (int j = 0; j <= (num4 - num3); j += num3)
                                {
                                    if ((j >= num2) && (num2 > 0))
                                    {
                                        object numRows = 1;
                                        this.m_wdApp.Selection.InsertRows(ref numRows);
                                    }
                                    Selection selection = this.m_wdApp.Selection;
                                    selection.Cells[1].Select();
                                    selection.Delete(ref this.m_oMissing, ref this.m_oMissing);
                                    if (j == 0)
                                    {
                                        this.InsertField(data);
                                    }
                                    selection.TypeText(dataList[j]);
                                    selection.Select();
                                    selection.MoveRight(ref wdCell, ref count, ref this.m_oMissing);
                                    for (int k = 1; k < num3; k++)
                                    {
                                        selection.Cells[1].Select();
                                        selection.Delete(ref this.m_oMissing, ref this.m_oMissing);
                                        selection.TypeText(dataList[j + k]);
                                        selection.MoveRight(ref wdCell, ref count, ref this.m_oMissing);
                                    }
                                }
                                return;
                            }
                            if (!(data.ToUpper() == "PAGECOUNT"))
                            {
                                bool flag1 = data.ToUpper() == "PAGEINDEX";
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                this._errorList.Add(exception.ToString());
                CDMS.Write(exception.ToString());
                throw;
            }
        }

        protected bool InsertCollection(object oProjectOrDoc)
        {
            try
            {
                this.InsertCollection(oProjectOrDoc, null);
                return true;
            }
            catch (Exception exception)
            {
                this._errorList.Add(exception.ToString());
                CDMS.Write(exception.ToString());
            }
            return false;
        }

        protected bool InsertCollection(object oProjectOrDoc, Hashtable htUserKeyWord)
        {
            bool flag;
            try
            {
                string[] strArray = new string[0];
                object wdLine = WdUnits.wdLine;
                object count = 1;
                Project project = null;
                Doc doc = null;
                if (oProjectOrDoc == null)
                {
                    return false;
                }
                if (oProjectOrDoc is Project)
                {
                    project = oProjectOrDoc as Project;
                }
                else if (oProjectOrDoc is Doc)
                {
                    doc = oProjectOrDoc as Doc;
                }
                for (int i = 1; i <= this.m_wdDoc.Fields.Count; i++)
                {
                    if (this.m_wdDoc.Fields[i].Type == WdFieldType.wdFieldAddin)
                    {
                        string data = this.m_wdDoc.Fields[i].Data;
                        if (data.Substring(data.Length - 1) == "F")
                        {
                            if (oProjectOrDoc is Project)
                            {
                                if (project == null)
                                {
                                    return false;
                                }
                                strArray = project.ExcuteDefnExpression(data, htUserKeyWord);
                            }
                            else if (oProjectOrDoc is Doc)
                            {
                                if (doc == null)
                                {
                                    return false;
                                }
                                strArray = doc.ExcuteDefnExpression(data, htUserKeyWord);
                            }
                            this.m_wdDoc.Fields[i].Select();
                            Selection selection = this.m_wdApp.Selection;
                            selection.Cells[1].Select();
                            selection.Delete(ref this.m_oMissing, ref this.m_oMissing);
                            this.InsertField(data + "F");
                            this.m_wdDoc.Fields[i].Result.Text = strArray[0];
                            selection.Select();
                            selection.MoveDown(ref wdLine, ref count, ref this.m_oMissing);
                            int iCount = strArray.GetLength(0) - 1;
                            for (int j = 1; j < iCount; j++)
                            {
                                if (!this.m_bIsInsertRow)
                                {
                                    this.InsertRow(selection, iCount);
                                }
                                selection.Cells[1].Select();
                                selection.Delete(ref this.m_oMissing, ref this.m_oMissing);
                                selection.TypeText(strArray[j]);
                                selection.MoveDown(ref wdLine, ref count, ref this.m_oMissing);
                            }
                            return true;
                        }
                    }
                }
                flag = false;
            }
            catch (Exception exception)
            {
                this._errorList.Add(exception.ToString());
                CDMS.Write(exception.ToString());
                throw;
            }
            return flag;
        }

        public Field InsertField(string sKeyWord)
        {
            Field field = null;
            try
            {
                object wdCollapseEnd = WdCollapseDirection.wdCollapseEnd;
                object wdFieldAddin = WdFieldType.wdFieldAddin;
                this.m_wdApp.Selection.Collapse(ref wdCollapseEnd);
                Selection selection = this.m_wdApp.Selection;
                if ((sKeyWord.Substring(sKeyWord.Length - 1) == "F") && !this.IsCell(selection))
                {
                    this._errorList.Add("该位置不是单元格,请选择单元格");
                    return null;
                }
                field = this.m_wdDoc.Fields.Add(selection.Range, ref wdFieldAddin, ref this.m_oMissing, ref this.m_oMissing);
                field.Data = sKeyWord;
            }
            catch (Exception exception)
            {
                this._errorList.Add(exception.ToString());
                CDMS.Write(exception.ToString());
                this.Release(true);
                throw;
            }
            return field;
        }

        public Field InsertPictureField(string sKeyWord)
        {
            Field field = null;
            try
            {
                object wdCollapseEnd = WdCollapseDirection.wdCollapseEnd;
                object wdFieldIncludePicture = WdFieldType.wdFieldIncludePicture;
                object text = sKeyWord;
                this.m_wdApp.Selection.Collapse(ref wdCollapseEnd);
                Selection selection = this.m_wdApp.Selection;
                field = this.m_wdDoc.Fields.Add(selection.Range, ref wdFieldIncludePicture, ref text, ref this.m_oMissing);
            }
            catch (Exception exception)
            {
                this._errorList.Add(exception.ToString());
                CDMS.Write(exception.ToString());
                this.Release(true);
                throw;
            }
            return field;
        }

        protected bool InsertRow(Microsoft.Office.Interop.Word.Selection selection, int iCount)
        {
            try
            {
                object wdLine = WdUnits.wdLine;
                Cell cell = selection.Cells[1];
                object numRows = (iCount - selection.Tables[1].Rows.Count) + 1;
                if (int.Parse(numRows.ToString()) > 0)
                {
                    selection.InsertRows(ref numRows);
                    cell.Select();
                    selection.MoveUp(ref wdLine, ref numRows, ref this.m_oMissing);
                    this.m_bIsInsertRow = true;
                    return true;
                }
                return false;
            }
            catch (Exception exception)
            {
                this._errorList.Add(exception.ToString());
                CDMS.Write(exception.ToString());
                return false;
            }
        }

        protected bool IsCell(Microsoft.Office.Interop.Word.Selection selection)
        {
            return (selection.Tables.Count > 0);
        }

        protected bool IsFullPath(string path)
        {
            return (path.Contains(@":\") || path.Contains(":/"));
        }

        public void OpenExcel(string sFileName)
        {
            this.OpenExcelNew(sFileName);
        }

        public bool OpenExcelNew(string sFileName)
        {
            try
            {
                try
                {
                    this.m_exApp = new Microsoft.Office.Interop.Excel.ApplicationClass();
                }
                catch
                {
                    this.m_exApp = null;
                }
                if (this.m_exApp == null)
                {
                    Type typeFromProgID = Type.GetTypeFromProgID("excel.application");
                    this.m_exApp = Activator.CreateInstance(typeFromProgID, true) as Application;
                    this.m_bExcelIsRunning = false;
                }
                if (this.m_exApp != null)
                {
                    this.m_exWorkBook = this.m_exApp.Workbooks.Open(sFileName, this.m_oMissing, this.m_oMissing, this.m_oMissing, this.m_oMissing, this.m_oMissing, this.m_oMissing, this.m_oMissing, this.m_oMissing, this.m_oMissing, this.m_oMissing, this.m_oMissing, this.m_oMissing, this.m_oMissing, this.m_oMissing);
                    if (!this.m_bExcelIsRunning)
                    {
                        this.m_exApp.Visible = this._visibleApp;
                        this.m_exApp.ScreenUpdating = this._visibleApp;
                        this.m_exApp.DisplayAlerts = this._visibleApp;
                    }
                    this.m_exWorksheet = this.m_exWorkBook.Worksheets[1] as Worksheet;
                    return true;
                }
            }
            catch (Exception exception)
            {
                this._errorList.Add(exception.ToString());
                CDMS.Write(exception.ToString());
                this.Release(true);
                throw;
            }
            return false;
        }

        public void OpenWord(object sFileName)
        {
            this.OpenWordNew(sFileName);
        }

        public bool OpenWordNew(object sFileName)
        {
            bool flag;
            try
            {
                string path = sFileName as string;
                if (!File.Exists(path))
                {
                    return false;
                }
                try
                {
                    this.m_wdApp = new Microsoft.Office.Interop.Excel.ApplicationClass();
                }
                catch (Exception exception)
                {
                    this._errorList.Add(exception.ToString());
                    CDMS.Write(exception.ToString());
                    this.m_wdApp = null;
                    throw;
                }
                if (this.m_wdApp == null)
                {
                    Type typeFromProgID = Type.GetTypeFromProgID("Word.Application");
                    this.m_wdApp = Activator.CreateInstance(typeFromProgID, true) as Application;
                    this.m_bWordIsRunning = false;
                }
                if (this.m_wdApp == null)
                {
                    return false;
                }
                if (!this.m_bWordIsRunning)
                {
                    this.m_wdApp.Visible = this._visibleApp;
                }
                this.m_wdApp.Documents.Open(ref sFileName, ref this.m_oMissing, ref this.m_oMissing, ref this.m_oMissing, ref this.m_oMissing, ref this.m_oMissing, ref this.m_oMissing, ref this.m_oMissing, ref this.m_oMissing, ref this.m_oMissing, ref this.m_oMissing, ref this.m_oMissing, ref this.m_oMissing, ref this.m_oMissing, ref this.m_oMissing, ref this.m_oMissing);
                this.m_wdDoc = this.m_wdApp.ActiveDocument;
                flag = true;
            }
            catch (Exception exception2)
            {
                this._errorList.Add(exception2.ToString());
                CDMS.Write(exception2.ToString());
                this.Release(true);
                throw;
            }
            return flag;
        }

        public void ParseExcelField(object obj, Hashtable htUserKeyWord, Hashtable htAuditDataList)
        {
            Project project = null;
            Doc doc = null;
            if (obj is Project)
            {
                project = obj as Project;
            }
            else if (obj is Doc)
            {
                doc = obj as Doc;
            }
            else
            {
                return;
            }
            if (this.m_exWorkBook != null)
            {
                Regex regex = new Regex("(TEXT|BMP)(:)(.*)");
                foreach (Microsoft.Office.Interop.Excel.Worksheet worksheet in this.m_exWorkBook.Worksheets)
                {
                    int num = 1;
                    int num2 = 1;
                    int num3 = 0;
                    int num4 = 0x3e8;
                    int num5 = 100;
                    int num6 = 0;
                    while ((num <= num4) && (num6 <= 10))
                    {
                        num3 = 0;
                        num2 = 1;
                        int num7 = 0;
                        while ((num2 <= num5) && (num3 <= 20))
                        {
                            Microsoft.Office.Interop.Excel.Range range = (Microsoft.Office.Interop.Excel.Range) worksheet.Cells[num, num2];
                            string str = range.Value2 as string;
                            if (string.IsNullOrEmpty(str))
                            {
                                num3++;
                                if (num2 == 0)
                                {
                                    num7 = 0;
                                }
                                if ((num3 > 20) && (num7 == 0))
                                {
                                    num6++;
                                }
                                num2++;
                            }
                            else
                            {
                                num3 = 0;
                                Match match = regex.Match(str);
                                if ((match == null) && !match.Success)
                                {
                                    num2++;
                                    continue;
                                }
                                string expression = match.Groups[3].Value;
                                string str3 = match.Groups[1].Value;
                                if (expression.ToUpper().Contains("$(*") || expression.ToUpper().Contains("$[*"))
                                {
                                    expression = expression.Replace("$(*", "$(").Replace("$[*", "$[");
                                    range.Value2 = str3 + ":" + expression;
                                }
                                else
                                {
                                    if (project != null)
                                    {
                                        expression = project.ExcuteDefnExpression(expression, htUserKeyWord)[0];
                                    }
                                    else if (doc != null)
                                    {
                                        expression = doc.ExcuteDefnExpression(expression, htUserKeyWord)[0];
                                    }
                                    if (str3.ToUpper() == "BMP")
                                    {
                                        if (!expression.ToUpper().EndsWith(".JPG"))
                                        {
                                            expression = expression + ".jpg";
                                        }
                                        string path = this.DownLoadFile((project != null) ? project : ((doc.Project != null) ? doc.Project : null), @"BMP\" + expression);
                                        if (File.Exists(path))
                                        {
                                            range.Value2 = "";
                                            range.Select();
                                            ((Pictures) worksheet.Pictures(Missing.Value)).Insert(path, Missing.Value);
                                        }
                                    }
                                    else if (str3.ToUpper() == "TEXT")
                                    {
                                        if (!expression.Contains(":"))
                                        {
                                            range.Value2 = expression;
                                        }
                                        else
                                        {
                                            string[] strArray = expression.Split(new char[] { ':' });
                                            if ((strArray == null) || (strArray.Length != 3))
                                            {
                                                range.Value2 = expression;
                                            }
                                            else
                                            {
                                                int num8 = num2;
                                                int num9 = num - 1;
                                                List<string> list = htAuditDataList[strArray[0]] as List<string>;
                                                int num10 = int.Parse(strArray[2]);
                                                int num11 = int.Parse(strArray[1]) * num10;
                                                Microsoft.Office.Interop.Excel.Range range2 = null;
                                                for (int i = 0; i <= (list.Count - num10); i += num10)
                                                {
                                                    if ((num11 > 0) && (i >= num11))
                                                    {
                                                        (worksheet.Cells[num9 + 1, num8] as Range).EntireRow.Insert(XlInsertShiftDirection.xlShiftDown, XlInsertFormatOrigin.xlFormatFromLeftOrAbove);
                                                    }
                                                    num9++;
                                                    int num13 = 0;
                                                    int num14 = num13;
                                                    while (num13 < num10)
                                                    {
                                                        range2 = (Microsoft.Office.Interop.Excel.Range) worksheet.Cells[num9, num14 + num8];
                                                        if (range2.MergeCells.ToString().ToLower() == "false")
                                                        {
                                                            range2.Value2 = list[i + num13];
                                                            num14++;
                                                        }
                                                        else
                                                        {
                                                            Microsoft.Office.Interop.Excel.Range mergeArea = range2.MergeArea;
                                                            mergeArea.set_Item(1, 1, list[i + num13]);
                                                            num14 += mergeArea.Count;
                                                        }
                                                        num13++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                this.m_exWorkBook.RefreshAll();
                                num2++;
                            }
                        }
                        num++;
                    }
                }
            }
        }

        public void ParseExcelFieldEx(object obj, Hashtable htUserKeyWord, Hashtable htAuditDataList)
        {
            Project project = null;
            Doc doc = null;
            if (obj is Project)
            {
                project = obj as Project;
            }
            else if (obj is Doc)
            {
                doc = obj as Doc;
            }
            else
            {
                return;
            }
            try
            {
                if (this.m_exWorkBook != null)
                {
                    Regex regex = new Regex("(TEXT|BMP)(:)(.*)");
                    foreach (Microsoft.Office.Interop.Excel.Worksheet worksheet in this.m_exWorkBook.Worksheets)
                    {
                        int row = 1;
                        int column = 1;
                        Microsoft.Office.Interop.Excel.Range usedRange = worksheet.UsedRange;
                        int count = worksheet.UsedRange.Rows.Count;
                        int num13 = worksheet.UsedRange.Columns.Count;
                        int num3 = 0;
                        List<Microsoft.Office.Interop.Excel.Range> list = new List<Microsoft.Office.Interop.Excel.Range>();
                        object[,] objArray = null;
                        if (this.IsFirstWriteData)
                        {
                            objArray = (object[,]) usedRange.Value2;
                            if (objArray == null)
                            {
                                continue;
                            }
                            for (int i = 1; i <= objArray.GetLength(0); i++)
                            {
                                for (int j = 1; j <= objArray.GetLength(1); j++)
                                {
                                    if ((objArray[i, j] != null) && !string.IsNullOrEmpty(objArray[i, j].ToString()))
                                    {
                                        list.Add(worksheet.Cells[i, j] as Range);
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (Microsoft.Office.Interop.Excel.Range range2 in usedRange)
                            {
                                list.Add(range2);
                            }
                        }
                        foreach (Microsoft.Office.Interop.Excel.Range range3 in list)
                        {
                            num3++;
                            string str = "";
                            if (this.IsUpdateFileData)
                            {
                                string inputMessage = "";
                                try
                                {
                                    inputMessage = range3.Validation.InputMessage;
                                }
                                catch (Exception)
                                {
                                    inputMessage = "";
                                }
                                if (string.IsNullOrEmpty(inputMessage))
                                {
                                    str = "";
                                }
                                else
                                {
                                    str = inputMessage;
                                }
                            }
                            else
                            {
                                str = range3.Value2 as string;
                            }
                            row = range3.Row;
                            column = range3.Column;
                            if (!this.IsFirstWriteData)
                            {
                                str = range3.Validation.InputMessage;
                            }
                            if (!string.IsNullOrEmpty(str))
                            {
                                if (this.IsFirstWriteData && str.ToLower().Contains("$"))
                                {
                                    try
                                    {
                                        range3.Validation.Add(XlDVType.xlValidateInputOnly, XlDVAlertStyle.xlValidAlertInformation, Type.Missing, Type.Missing, Type.Missing);
                                        range3.Validation.InputMessage = str;
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                                Match match = regex.Match(str);
                                if ((match != null) && match.Success)
                                {
                                    string str3 = match.Groups[3].Value;
                                    string str4 = match.Groups[1].Value;
                                    if (!string.IsNullOrEmpty(str3) && ((str3.Contains("$[") || str3.Contains("$(")) || ((str3.ToUpper().Contains("SELECT ") || str.ToUpper().Contains("TEXT:")) || str.ToUpper().Contains("BMP:"))))
                                    {
                                        if (project != null)
                                        {
                                            str3 = project.ExcuteDefnExpression(str3, htUserKeyWord)[0];
                                        }
                                        else if (doc != null)
                                        {
                                            str3 = doc.ExcuteDefnExpression(str3, htUserKeyWord)[0];
                                        }
                                        if (!string.IsNullOrEmpty(str3))
                                        {
                                            if (str3.ToUpper().Contains("$(*") || str3.ToUpper().Contains("$[*"))
                                            {
                                                str3 = str3.Replace("$(*", "$(").Replace("$[*", "$[");
                                                range3.Value2 = str4 + ":" + str3;
                                            }
                                            else if (str4.ToUpper() == "BMP")
                                            {
                                                if (!str3.ToUpper().EndsWith(".JPG"))
                                                {
                                                    str3 = str3 + ".jpg";
                                                }
                                                if (str3.Length < 5)
                                                {
                                                    continue;
                                                }
                                                string str5 = this.DownLoadFile((project != null) ? project : ((doc.Project != null) ? doc.Project : null), @"BMP\" + str3);
                                                if (!string.IsNullOrEmpty(str5) && File.Exists(str5))
                                                {
                                                    range3.Value2 = "";
                                                    range3.Select();
                                                    worksheet.Shapes.AddPicture(str5, MsoTriState.msoFalse, MsoTriState.msoCTrue, Convert.ToSingle(range3.Left) + 2f, Convert.ToSingle(range3.Top) + 2f, Convert.ToSingle(range3.MergeArea.Width) - 4f, Convert.ToSingle(range3.MergeArea.Height) - 4f);
                                                }
                                            }
                                            else if (str4.ToUpper() == "TEXT")
                                            {
                                                if (str.Contains("$("))
                                                {
                                                    range3.Value2 = str3;
                                                    range3.Font.ColorIndex = 1;
                                                }
                                                else
                                                {
                                                    string[] strArray = str3.Split(new char[] { ':' });
                                                    if ((strArray == null) || (strArray.Length != 3))
                                                    {
                                                        range3.Value2 = str3;
                                                    }
                                                    else if (((htAuditDataList != null) && (htAuditDataList.Count > 0)) && htAuditDataList.Contains(strArray[0]))
                                                    {
                                                        int num6 = column;
                                                        int num7 = row - 1;
                                                        List<string> list2 = htAuditDataList[strArray[0]] as List<string>;
                                                        int num8 = int.Parse(strArray[2]);
                                                        int num9 = int.Parse(strArray[1]) * num8;
                                                        Microsoft.Office.Interop.Excel.Range range4 = null;
                                                        if ((list2 == null) || (list2.Count < num8))
                                                        {
                                                            continue;
                                                        }
                                                        for (int k = 0; k <= (list2.Count - num8); k += num8)
                                                        {
                                                            if ((num9 > 0) && (k >= num9))
                                                            {
                                                                (worksheet.Cells[num7 + 1, num6] as Range).EntireRow.Insert(XlInsertShiftDirection.xlShiftDown, XlInsertFormatOrigin.xlFormatFromLeftOrAbove);
                                                            }
                                                            num7++;
                                                            int num11 = 0;
                                                            int num12 = num11;
                                                            while (num11 < num8)
                                                            {
                                                                range4 = (Microsoft.Office.Interop.Excel.Range) worksheet.Cells[num7, num12 + num6];
                                                                if (range4.MergeCells.ToString().ToLower() == "false")
                                                                {
                                                                    range4.Value2 = list2[k + num11];
                                                                    num12++;
                                                                }
                                                                else
                                                                {
                                                                    Microsoft.Office.Interop.Excel.Range mergeArea = range4.MergeArea;
                                                                    mergeArea.set_Item(1, 1, list2[k + num11]);
                                                                    num12 += mergeArea.Count;
                                                                }
                                                                num11++;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            this.m_exWorkBook.RefreshAll();
                                            column++;
                                        }
                                    }
                                }
                            }
                        }
                        row++;
                        if ((this.IsUpdateVersion && (worksheet.Shapes != null)) && (worksheet.Shapes.Count > 0))
                        {
                            foreach (Shape shape in worksheet.Shapes)
                            {
                                if (!string.IsNullOrEmpty(shape.AlternativeText))
                                {
                                    try
                                    {
                                        if ((shape.TopLeftCell != null) && (shape.TopLeftCell.Validation != null))
                                        {
                                            shape.TopLeftCell.Value2 = shape.TopLeftCell.Validation.InputMessage;
                                        }
                                        shape.Delete();
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                this._errorList.Add(exception.ToString());
                CDMS.Write(exception.ToString());
                throw;
            }
        }

        public void Release(bool bRelease)
        {
            if (bRelease)
            {
                try
                {
                    if (this.m_bWordIsRunning)
                    {
                        if (this.m_wdDoc != null)
                        {
                            try
                            {
                                this.m_wdDoc.Close(ref this.m_oMissing, ref this.m_oMissing, ref this.m_oMissing);
                            }
                            catch
                            {
                            }
                        }
                    }
                    else
                    {
                        if (this.m_wdDoc != null)
                        {
                            try
                            {
                                this.m_wdDoc.Close(ref this.m_oMissing, ref this.m_oMissing, ref this.m_oMissing);
                            }
                            catch
                            {
                            }
                        }
                        if (this.m_wdApp != null)
                        {
                            try
                            {
                                this.m_wdApp.Quit(ref this.m_oMissing, ref this.m_oMissing, ref this.m_oMissing);
                            }
                            catch
                            {
                            }
                        }
                    }
                    if (this.m_bExcelIsRunning)
                    {
                        if (this.m_exWorkBook == null)
                        {
                            return;
                        }
                        try
                        {
                            this.m_exWorkBook.Close(this.m_oMissing, this.m_oMissing, this.m_oMissing);
                            return;
                        }
                        catch
                        {
                            return;
                        }
                    }
                    if (this.m_exWorkBook != null)
                    {
                        try
                        {
                            this.m_exWorkBook.Close(this.m_oMissing, this.m_oMissing, this.m_oMissing);
                        }
                        catch
                        {
                        }
                    }
                    if (this.m_exApp != null)
                    {
                        IntPtr hWnd = new IntPtr(this.m_exApp.Hwnd);
                        uint processId = 0;
                        GetWindowThreadProcessId(hWnd, ref processId);
                        try
                        {
                            this.m_exApp.Quit();
                            Marshal.FinalReleaseComObject(this.m_exApp);
                        }
                        catch
                        {
                        }
                        Process processById = null;
                        processById = Process.GetProcessById((int) processId);
                        if (processById != null)
                        {
                            processById.Kill();
                        }
                        this.m_exApp = null;
                    }
                }
                catch (Exception exception)
                {
                    CDMS.Write(exception.ToString());
                }
            }
        }

        public static void RenewSign(Doc RenewDoc, Hashtable htUserKeyWord)
        {
            try
            {
                if (RenewDoc != null)
                {
                    if (htUserKeyWord == null)
                    {
                        htUserKeyWord = new Hashtable();
                        htUserKeyWord.Add("Create", "true");
                    }
                    DBSource dBSource = RenewDoc.dBSource;
                    List<User> mtoUser = null;
                    if ((dBSource.AllAdminUserList != null) && (dBSource.AllAdminUserList.Count > 0))
                    {
                        mtoUser = dBSource.AllAdminUserList;
                    }
                    else
                    {
                        User userByCode = dBSource.GetUserByCode("admin");
                        if (userByCode != null)
                        {
                            mtoUser = new List<User> {
                                userByCode
                            };
                        }
                    }
                    if ((mtoUser != null) && (mtoUser.Count != 0))
                    {
                        dBSource.SendMessage(dBSource.LoginUser, mtoUser, null, "【客户端签名失败】", dBSource.LoginUser.ToString + "客户端签名失败，可能由于客户端机器环境调用签名程序失败等原因造成，请您跟踪处理！", null, null);
                        string sQL = "select * from sysObjects where id = Object_Id(N'CDMS_RenewDoc') and ObjectProperty(id, N'IsUserTable') ='1'";
                        string[] source = dBSource.DBExecuteSQL(sQL);
                        bool flag = true;
                        if (((source == null) || (source.Count<string>() <= 0)) || string.IsNullOrEmpty(source[0]))
                        {
                            try
                            {
                                flag = false;
                                string str2 = "create table CDMS_RenewDoc (DOCID int , HashtableList nvarchar(max), isFinish int )";
                                bool flag2 = false;
                                flag2 = dBSource.DBExecuteCommand(str2);
                                if (!flag2)
                                {
                                    flag2 = dBSource.DBExecuteCommand(str2);
                                }
                                if (!flag2)
                                {
                                    CDMS.Write("补签程序创建数据表失败。");
                                    if ((mtoUser != null) && (mtoUser.Count > 0))
                                    {
                                        dBSource.SendMessage(dBSource.LoginUser, mtoUser, null, "【客户端签名失败】", "客户端签名失败，调用补签程序创建数据表失败，请联系CDMS工作人员。", null, null);
                                    }
                                }
                                else
                                {
                                    flag = true;
                                }
                            }
                            catch (Exception exception)
                            {
                                CDMS.Write("补签程序创建数据表异常：" + exception.ToString());
                            }
                        }
                        if (flag)
                        {
                            string format = "insert into cdms_RenewDoc (DOCID,hashtableList,isFinish) values({0},'{1}','{2}')";
                            string str4 = "";
                            try
                            {
                                if (htUserKeyWord != null)
                                {
                                    foreach (DictionaryEntry entry in htUserKeyWord)
                                    {
                                        if (str4 == "")
                                        {
                                            str4 = entry.Key.ToString() + "&&&" + entry.Value.ToString();
                                        }
                                        else
                                        {
                                            string str5 = str4;
                                            str4 = str5 + "$$$" + entry.Key.ToString() + "&&&" + entry.Value.ToString();
                                        }
                                    }
                                }
                                format = string.Format(format, RenewDoc.ID, str4, "0");
                                dBSource.DBExecuteCommand(format);
                            }
                            catch (Exception exception2)
                            {
                                CDMS.Write("补签程序插入表数据表异常：" + exception2.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception exception3)
            {
                CDMS.Write("补签程序出错：" + exception3.ToString());
            }
        }

        public void SaveExcel()
        {
            try
            {
                if (this.m_exWorkBook != null)
                {
                    this.m_exWorkBook.Save();
                }
            }
            catch (Exception exception)
            {
                this._errorList.Add(exception.ToString());
                CDMS.Write(exception.ToString());
                this.Release(true);
            }
            finally
            {
                this.Release(this._closeApp);
            }
        }

        public void SaveWord(object oFileName)
        {
            if (((oFileName != null) && (this.m_wdApp != null)) && (this.m_wdDoc != null))
            {
                string path = (string) oFileName;
                int length = path.LastIndexOf(".");
                string sourceFileName = path.Substring(0, length) + DateTime.Now.ToString("yyyyMMddHHmmss") + "." + path.Substring(length + 1);
                object fileName = sourceFileName;
                this.m_wdApp.NormalTemplate.Saved = true;
                try
                {
                    this.m_wdDoc.SaveAs(ref fileName, ref this.m_oMissing, ref this.m_oMissing, ref this.m_oMissing, ref this.m_oMissing, ref this.m_oMissing, ref this.m_oMissing, ref this.m_oMissing, ref this.m_oMissing, ref this.m_oMissing, ref this.m_oMissing, ref this.m_oMissing, ref this.m_oMissing, ref this.m_oMissing, ref this.m_oMissing, ref this.m_oMissing);
                }
                catch (Exception exception)
                {
                    this._errorList.Add(exception.ToString());
                    CDMS.Write(exception.ToString());
                    this.Release(true);
                }
                finally
                {
                    this.Release(this._closeApp);
                    File.Delete(path);
                    File.Move(sourceFileName, path);
                }
            }
        }

        private List<Field> ScanFieldsInWord(bool IsDebug)
        {
            TextFrame textFrame;
            Microsoft.Office.Interop.Excel.Range textRange;
            List<Field> list = new List<Field>();
            string msg = "";
            if (this.m_wdDoc == null)
            {
                if (IsDebug)
                {
                    msg = msg + "m_wdDoc为空\r\n";
                }
                return list;
            }
            if (IsDebug)
            {
                object obj2 = msg;
                msg = string.Concat(new object[] { obj2, "正文的域数量为: ", this.m_wdDoc.Fields.Count, "个\r\n由以下域组成:\r\n" });
            }
            foreach (Field field in this.m_wdDoc.Fields)
            {
                if (IsDebug)
                {
                    if (field.Type == WdFieldType.wdFieldAddin)
                    {
                        string str2 = msg;
                        msg = str2 + "域类型为: " + field.Type.ToString() + "\t\t域内容: " + field.Data + "\r\n";
                    }
                    else
                    {
                        string str3 = msg;
                        msg = str3 + "域类型为: " + field.Type.ToString() + "\t\t域内容: " + field.Code.Text + "\r\n";
                    }
                }
                list.Add(field);
            }
            foreach (Shape shape in this.m_wdDoc.Shapes)
            {
                if (shape.AutoShapeType == MsoAutoShapeType.msoShapeRectangle)
                {
                    shape.Select(ref this.m_oMissing);
                    if ((shape.TextFrame != null) && (shape.TextFrame.TextRange != null))
                    {
                        textFrame = shape.TextFrame;
                        if (textFrame.TextRange != null)
                        {
                            textRange = textFrame.TextRange;
                            if (IsDebug)
                            {
                                object obj3 = msg;
                                msg = string.Concat(new object[] { obj3, "正文Shapes对象的域数量为: ", textRange.Fields.Count, "个\r\n由以下域组成:\r\n" });
                            }
                            foreach (Field field2 in textRange.Fields)
                            {
                                if (IsDebug)
                                {
                                    if (field2.Type == WdFieldType.wdFieldAddin)
                                    {
                                        string str4 = msg;
                                        msg = str4 + "域类型为: " + field2.Type.ToString() + "\t\t域内容: " + field2.Data + "\r\n";
                                    }
                                    else
                                    {
                                        string str5 = msg;
                                        msg = str5 + "域类型为: " + field2.Type.ToString() + "\t\t域内容: " + field2.Code.Text + "\r\n";
                                    }
                                }
                                list.Add(field2);
                            }
                        }
                    }
                }
            }
            this.m_wdApp.ActiveWindow.ActivePane.View.SeekView = WdSeekView.wdSeekCurrentPageHeader;
            this.m_wdApp.Selection.WholeStory();
            if (IsDebug)
            {
                object obj4 = msg;
                msg = string.Concat(new object[] { obj4, "页眉的域数量为: ", this.m_wdApp.Selection.Fields.Count, "个\r\n由以下域组成:\r\n" });
            }
            if ((this.m_wdApp.Selection.Fields != null) && (this.m_wdApp.Selection.Fields.Count > 0))
            {
                foreach (Field field3 in this.m_wdApp.Selection.Fields)
                {
                    if (IsDebug)
                    {
                        if (field3.Type == WdFieldType.wdFieldAddin)
                        {
                            string str6 = msg;
                            msg = str6 + "域类型为: " + field3.Type.ToString() + "\t\t域内容: " + field3.Data + "\r\n";
                        }
                        else
                        {
                            string str7 = msg;
                            msg = str7 + "域类型为: " + field3.Type.ToString() + "\t\t域内容: " + field3.Code.Text + "\r\n";
                        }
                    }
                    list.Add(field3);
                }
            }
            foreach (Shape shape2 in this.m_wdApp.Selection.HeaderFooter.Shapes)
            {
                if (shape2.AutoShapeType == MsoAutoShapeType.msoShapeRectangle)
                {
                    shape2.Select(ref this.m_oMissing);
                    if ((shape2.TextFrame != null) && (shape2.TextFrame.TextRange != null))
                    {
                        textFrame = shape2.TextFrame;
                        if (textFrame.TextRange != null)
                        {
                            textRange = textFrame.TextRange;
                            if (IsDebug)
                            {
                                object obj5 = msg;
                                msg = string.Concat(new object[] { obj5, "页眉页脚Shapes对象的域数量为: ", textRange.Fields.Count, "个\r\n由以下域组成:\r\n" });
                            }
                            foreach (Field field4 in textRange.Fields)
                            {
                                if (IsDebug)
                                {
                                    if (field4.Type == WdFieldType.wdFieldAddin)
                                    {
                                        string str8 = msg;
                                        msg = str8 + "域类型为: " + field4.Type.ToString() + "\t\t域内容: " + field4.Data + "\r\n";
                                    }
                                    else
                                    {
                                        string str9 = msg;
                                        msg = str9 + "域类型为: " + field4.Type.ToString() + "\t\t域内容: " + field4.Code.Text + "\r\n";
                                    }
                                }
                                list.Add(field4);
                            }
                        }
                    }
                }
            }
            this.m_wdApp.ActiveWindow.ActivePane.View.SeekView = WdSeekView.wdSeekCurrentPageFooter;
            this.m_wdApp.Selection.WholeStory();
            if (IsDebug)
            {
                object obj6 = msg;
                msg = string.Concat(new object[] { obj6, "页脚的域数量为: ", this.m_wdApp.Selection.Fields.Count, "个\r\n由以下域组成:\r\n" });
            }
            if ((this.m_wdApp.Selection.Fields != null) && (this.m_wdApp.Selection.Fields.Count > 0))
            {
                foreach (Field field5 in this.m_wdApp.Selection.Fields)
                {
                    if (IsDebug)
                    {
                        if (field5.Type == WdFieldType.wdFieldAddin)
                        {
                            string str10 = msg;
                            msg = str10 + "域类型为: " + field5.Type.ToString() + "\t\t域内容: " + field5.Data + "\r\n";
                        }
                        else
                        {
                            string str11 = msg;
                            msg = str11 + "域类型为: " + field5.Type.ToString() + "\t\t域内容: " + field5.Code.Text + "\r\n";
                        }
                    }
                    list.Add(field5);
                }
            }
            if (IsDebug)
            {
                WriteLog(msg);
            }
            return list;
        }

        private void SetFieldInFrontOfText(Field field)
        {
            try
            {
                if (field != null)
                {
                    field.InlineShape.Select();
                }
            }
            catch (Exception exception)
            {
                CDMS.Write(exception.ToString());
            }
        }

        public static bool SetFilePagesInfo(Doc d, string pagesKeyword)
        {
            try
            {
                return SetFilePagesInfo(d, pagesKeyword, "");
            }
            catch (Exception exception)
            {
                CDMS.Write(exception.ToString());
                return false;
            }
        }

        public static bool SetFilePagesInfo(Doc d, string pagesKeyword, string sizeKeyword)
        {
            if ((d == null) || (d.O_filename == null))
            {
                return false;
            }
            if (string.IsNullOrEmpty(pagesKeyword) || (d.GetAttrDataByKeyWord(pagesKeyword) == null))
            {
                return false;
            }
            try
            {
                string path = d.dBSource.LoginUser.WorkingPath + d.O_filename;
                CDMS.CDMSDownload(d, null, path);
                if (File.Exists(path))
                {
                    List<string> fileSizeAndPages = GetFileSizeAndPages(path);
                    if ((fileSizeAndPages == null) || (fileSizeAndPages.Count < 2))
                    {
                        return false;
                    }
                    string str2 = fileSizeAndPages[0];
                    if (!string.IsNullOrEmpty(sizeKeyword) && !string.IsNullOrEmpty(str2))
                    {
                        AttrData attrDataByKeyWord = d.GetAttrDataByKeyWord(sizeKeyword);
                        if (attrDataByKeyWord != null)
                        {
                            attrDataByKeyWord.SetCodeDesc(str2);
                        }
                    }
                    string str3 = fileSizeAndPages[1];
                    if (!string.IsNullOrEmpty(pagesKeyword) && !string.IsNullOrEmpty(str3))
                    {
                        AttrData data2 = d.GetAttrDataByKeyWord(pagesKeyword);
                        if (data2 != null)
                        {
                            data2.SetCodeDesc(str3);
                        }
                    }
                    d.AttrDataList.SaveData();
                    return true;
                }
                return false;
            }
            catch (Exception exception)
            {
                CDMS.Write(exception.ToString());
                return false;
            }
        }

        protected void SetWordSize(int left, int top, int width, int height)
        {
            try
            {
                if (this.m_wdApp != null)
                {
                    this.m_wdApp.WindowState = WdWindowState.wdWindowStateNormal;
                    this.m_wdApp.Left = left;
                    this.m_wdApp.Top = top;
                    this.m_wdApp.Width = width;
                    this.m_wdApp.Height = height;
                }
            }
            catch (Exception exception)
            {
                this._errorList.Add(exception.ToString());
                CDMS.Write(exception.ToString());
            }
        }

        public bool UpdateFileData(object oProjectOrDoc, string sFileName, Hashtable htUserKeyWord)
        {
            try
            {
                this.IsUpdateFileData = true;
                if (File.Exists(sFileName))
                {
                    if (sFileName.ToUpper().IndexOf(".DOC") > 0)
                    {
                        if (!this.WordIsRunning(this.m_wdApp, sFileName))
                        {
                            if (!this.OpenWordNew(sFileName))
                            {
                                return false;
                            }
                            this.FillWdFieldValueUpdateData(oProjectOrDoc, htUserKeyWord);
                            this.InsertCollection(oProjectOrDoc, htUserKeyWord);
                            this.SaveWord(sFileName);
                        }
                        else
                        {
                            this._errorList.Add("[" + sFileName + "已经打开!]");
                        }
                    }
                    else if (sFileName.ToUpper().IndexOf(".XLS") > 0)
                    {
                        if (!this.ExcelIsRunning(this.m_exApp, sFileName))
                        {
                            if (!this.OpenExcelNew(sFileName))
                            {
                                return false;
                            }
                            this.ParseExcelFieldEx(oProjectOrDoc, htUserKeyWord, new Hashtable());
                            this.SaveExcel();
                        }
                        else
                        {
                            this._errorList.Add("[" + sFileName + "已经打开!]");
                        }
                    }
                    return true;
                }
                this._errorList.Add("[" + sFileName + "]不存在!");
                return false;
            }
            catch (Exception exception)
            {
                this._errorList.Add(exception.Message);
                CDMS.Write(exception.ToString());
                return false;
            }
        }

        private void UpdateWordFields(List<Field> FieldList, object oProjectOrDoc, Hashtable htUserKeyWord, bool IsDebug)
        {
            foreach (Field field in FieldList)
            {
                try
                {
                    this.FillWordFieldNew(field, oProjectOrDoc, htUserKeyWord, IsDebug);
                }
                catch (Exception exception)
                {
                    WriteLog(exception.ToString());
                    throw;
                }
            }
        }

        protected bool WordIsRunning(Microsoft.Office.Interop.Word.Application wdAppl, string sFileName)
        {
            if (wdAppl == null)
            {
                return false;
            }
            return wdAppl.Tasks.Exists(sFileName);
        }

        public void WriteAndUpdateDataToDocument(object oProjectOrDoc, string sFileName, Hashtable htUserKeyWord, bool IsDebug)
        {
            this.WriteAndUpdateDataToDocument(oProjectOrDoc, sFileName, htUserKeyWord, IsDebug, false);
        }

        public void WriteAndUpdateDataToDocument(object oProjectOrDoc, string sFileName, Hashtable htUserKeyWord, bool IsDebug, bool bForceReWrite)
        {
            try
            {
                this.m_bForceReWrite = bForceReWrite;
                if (IsDebug)
                {
                    string msg = "";
                    msg = "签名程序调试信息:\r\n";
                    if (oProjectOrDoc is Project)
                    {
                        msg = msg + "传进来参数(oProjectOrDoc)为Project: Project.ToString = " + ((Project) oProjectOrDoc).ToString + "\r\n";
                    }
                    else if (oProjectOrDoc is Doc)
                    {
                        msg = msg + "传进来参数(oProjectOrDoc)为Doc: Doc.ToString = " + ((Doc) oProjectOrDoc).ToString + "\r\n";
                    }
                    else
                    {
                        msg = msg + "传进来参数(oProjectOrDoc)不能识别为Project或Doc\r\n";
                    }
                    msg = (msg + "sFileName = " + sFileName + "\r\n") + "HtUserKeyWord 对应信息:\r\n";
                    if ((htUserKeyWord != null) && (htUserKeyWord.Count > 0))
                    {
                        foreach (DictionaryEntry entry in htUserKeyWord)
                        {
                            string str2 = msg;
                            msg = str2 + "Key: " + entry.Key.ToString() + "\t\t " + entry.Value.ToString() + "\r\n";
                        }
                    }
                    WriteLog(msg);
                }
                if ((oProjectOrDoc is Project) || (oProjectOrDoc is Doc))
                {
                    if (!File.Exists(sFileName))
                    {
                        if (IsDebug)
                        {
                            WriteLog("文件: " + sFileName + "不存在!");
                        }
                    }
                    else
                    {
                        if (sFileName.ToUpper().IndexOf(".DOC") > 0)
                        {
                            if (!this.WordIsRunning(this.m_wdApp, sFileName))
                            {
                                if (!this.OpenWordNew(sFileName))
                                {
                                    if (IsDebug)
                                    {
                                        WriteLog("OpenWordNew返回值为False");
                                    }
                                    return;
                                }
                                List<Field> fieldList = this.ScanFieldsInWord(IsDebug);
                                this.UpdateWordFields(fieldList, oProjectOrDoc, htUserKeyWord, IsDebug);
                                this.FitWordTable();
                                this.SaveWord(sFileName);
                            }
                            else if (IsDebug)
                            {
                                WriteLog("[" + sFileName + "已经打开!]");
                            }
                        }
                        else if (sFileName.ToUpper().IndexOf(".XLS") > 0)
                        {
                            if (!this.ExcelIsRunning(this.m_exApp, sFileName))
                            {
                                if (!this.OpenExcelNew(sFileName))
                                {
                                    return;
                                }
                                this.ParseExcelFieldEx(oProjectOrDoc, htUserKeyWord, new Hashtable());
                                this.SaveExcel();
                            }
                            else if (IsDebug)
                            {
                                WriteLog("[" + sFileName + "已经打开!]");
                            }
                        }
                        try
                        {
                            if (Directory.Exists(@"C:\temp\CDMSBMP\"))
                            {
                                Directory.Delete(@"C:\temp\CDMSBMP\", true);
                            }
                        }
                        catch (Exception exception)
                        {
                            CDMS.Write(exception.ToString());
                        }
                    }
                }
            }
            catch (Exception exception2)
            {
                CDMS.Write(exception2.ToString());
                if (oProjectOrDoc is Doc)
                {
                    Doc renewDoc = (Doc) oProjectOrDoc;
                    RenewSign(renewDoc, htUserKeyWord);
                }
            }
        }

        public bool WriteDataToDocument(object oProjectOrDoc, string sTempDefnFile)
        {
            try
            {
                return this.WriteDataToDocument(oProjectOrDoc, sTempDefnFile, null);
            }
            catch (Exception exception)
            {
                if (this._errorList != null)
                {
                    this._errorList.Add(exception.Message);
                }
                CDMS.Write(exception.ToString());
                return false;
            }
        }

        public bool WriteDataToDocument(object oProjectOrDoc, string sFileName, Hashtable htUserKeyWord)
        {
            try
            {
                if (File.Exists(sFileName))
                {
                    if (sFileName.ToUpper().IndexOf(".DOC") > 0)
                    {
                        if (sFileName.ToLower().EndsWith(".docx"))
                        {
                            this.IsWord07 = true;
                        }
                        if (!this.WordIsRunning(this.m_wdApp, sFileName))
                        {
                            if (!this.OpenWordNew(sFileName))
                            {
                                return false;
                            }
                            this.FillWdFieldValue(oProjectOrDoc, htUserKeyWord);
                            this.InsertCollection(oProjectOrDoc, htUserKeyWord);
                            this.SaveWord(sFileName);
                        }
                        else
                        {
                            this._errorList.Add("[" + sFileName + "已经打开!]");
                        }
                    }
                    else if (sFileName.ToUpper().IndexOf(".XLS") > 0)
                    {
                        if (!this.ExcelIsRunning(this.m_exApp, sFileName))
                        {
                            if (!this.OpenExcelNew(sFileName))
                            {
                                return false;
                            }
                            this.ParseExcelFieldEx(oProjectOrDoc, htUserKeyWord, new Hashtable());
                            this.SaveExcel();
                        }
                        else
                        {
                            this._errorList.Add("[" + sFileName + "已经打开!]");
                        }
                    }
                    return true;
                }
                this._errorList.Add("[" + sFileName + "]不存在!");
                return false;
            }
            catch (Exception exception)
            {
                this._errorList.Add(exception.Message);
                CDMS.Write(exception.ToString());
                if (oProjectOrDoc is Doc)
                {
                    Doc renewDoc = (Doc) oProjectOrDoc;
                    RenewSign(renewDoc, htUserKeyWord);
                }
                return false;
            }
        }

        public bool WriteDataToDocument(object oProjectOrDoc, string sFileName, Hashtable htUserKeyWord, List<string> auditDataList)
        {
            try
            {
                if (File.Exists(sFileName))
                {
                    if (sFileName.ToUpper().IndexOf(".DOC") > 0)
                    {
                        if (!this.WordIsRunning(this.m_wdApp, sFileName))
                        {
                            if (!this.OpenWordNew(sFileName))
                            {
                                return false;
                            }
                            this.FillWdFieldValue(oProjectOrDoc, htUserKeyWord);
                            this.InsertCollection(oProjectOrDoc, htUserKeyWord);
                            this.InsertAuditCollection("WORKFLOWAUDITS", auditDataList);
                            this.SaveWord(sFileName);
                        }
                        else
                        {
                            this._errorList.Add("[" + sFileName + "已经打开!]");
                        }
                    }
                    else if (sFileName.ToUpper().IndexOf(".XLS") > 0)
                    {
                        if (!this.ExcelIsRunning(this.m_exApp, sFileName))
                        {
                            if (!this.OpenExcelNew(sFileName))
                            {
                                return false;
                            }
                            Hashtable htAuditDataList = new Hashtable();
                            htAuditDataList.Add("WORKFLOWAUDITS", auditDataList);
                            this.ParseExcelFieldEx(oProjectOrDoc, htUserKeyWord, htAuditDataList);
                            this.SaveExcel();
                        }
                        else
                        {
                            this._errorList.Add("[" + sFileName + "已经打开!]");
                        }
                    }
                    return true;
                }
                this._errorList.Add("[" + sFileName + "]不存在!");
                return false;
            }
            catch (Exception exception)
            {
                this._errorList.Add(exception.Message);
                CDMS.Write(exception.ToString());
                if (oProjectOrDoc is Doc)
                {
                    Doc renewDoc = (Doc) oProjectOrDoc;
                    RenewSign(renewDoc, htUserKeyWord);
                }
                return false;
            }
        }

        public bool WriteDataToDocument(object oProjectOrDoc, string sFileName, Hashtable htUserKeyWord, Hashtable htAuditDataList)
        {
            try
            {
                if (File.Exists(sFileName))
                {
                    if (sFileName.ToUpper().IndexOf(".DOC") > 0)
                    {
                        if (!this.WordIsRunning(this.m_wdApp, sFileName))
                        {
                            if (!this.OpenWordNew(sFileName))
                            {
                                return false;
                            }
                            this.FillWdFieldValue(oProjectOrDoc, htUserKeyWord);
                            this.InsertCollection(oProjectOrDoc, htUserKeyWord);
                            this.InsertAuditCollection(htAuditDataList);
                            this.SaveWord(sFileName);
                        }
                        else
                        {
                            this._errorList.Add("[" + sFileName + "已经打开!]");
                        }
                    }
                    else if (sFileName.ToUpper().IndexOf(".XLS") > 0)
                    {
                        if (!this.ExcelIsRunning(this.m_exApp, sFileName))
                        {
                            if (!this.OpenExcelNew(sFileName))
                            {
                                return false;
                            }
                            this.ParseExcelFieldEx(oProjectOrDoc, htUserKeyWord, htAuditDataList);
                            this.SaveExcel();
                        }
                        else
                        {
                            this._errorList.Add("[" + sFileName + "已经打开!]");
                        }
                    }
                    return true;
                }
                this._errorList.Add("[" + sFileName + "]不存在!");
                return false;
            }
            catch (Exception exception)
            {
                this._errorList.Add(exception.Message);
                CDMS.Write(exception.ToString());
                if (oProjectOrDoc is Doc)
                {
                    Doc renewDoc = (Doc) oProjectOrDoc;
                    RenewSign(renewDoc, htUserKeyWord);
                }
                return false;
            }
        }

        public bool WriteDataToDocument(object oProjectOrDoc, string sTempDefnFile, string sOutFileName, Hashtable htUserKeyWord)
        {
            try
            {
                if (oProjectOrDoc is Project)
                {
                    this.DownLoadFile(oProjectOrDoc as Project, (this.IsFullPath(sTempDefnFile) ? "" : @"ISO\") + sTempDefnFile, sOutFileName);
                }
                else if (oProjectOrDoc is Doc)
                {
                    this.DownLoadFile((oProjectOrDoc as Doc).Project, (this.IsFullPath(sTempDefnFile) ? "" : @"ISO\") + sTempDefnFile, sOutFileName);
                }
                this.WriteDataToDocument(oProjectOrDoc, sOutFileName, htUserKeyWord);
                return true;
            }
            catch (Exception exception)
            {
                if (this._errorList != null)
                {
                    this._errorList.Add(exception.Message);
                }
                CDMS.Write(exception.ToString());
                return false;
            }
        }

        public static void WriteLog(string msg)
        {
            CDMS.Write(msg);
        }

        public bool CloseApp
        {
            get
            {
                return this._closeApp;
            }
            set
            {
                this._closeApp = value;
            }
        }

        public bool CloseErrorApp
        {
            get
            {
                return this._closeErrorApp;
            }
            set
            {
                this._closeErrorApp = value;
            }
        }

        public ArrayList ErrorList
        {
            get
            {
                return this._errorList;
            }
            set
            {
                this._errorList = value;
            }
        }

        public Microsoft.Office.Interop.Excel.Application ExcelApp
        {
            get
            {
                return this.m_exApp;
            }
        }

        public bool IsFirstWriteData
        {
            get
            {
                return this.isFirstWriteData;
            }
            set
            {
                this.isFirstWriteData = value;
            }
        }

        public string LastError
        {
            get
            {
                string str = "";
                foreach (string str2 in this.ErrorList)
                {
                    str = str + "\n" + str2;
                }
                return str;
            }
        }

        public bool VisibleApp
        {
            get
            {
                return this._visibleApp;
            }
            set
            {
                this._visibleApp = value;
            }
        }

        public Microsoft.Office.Interop.Word.Application WordApp
        {
            get
            {
                return this.m_wdApp;
            }
        }
    }
}

