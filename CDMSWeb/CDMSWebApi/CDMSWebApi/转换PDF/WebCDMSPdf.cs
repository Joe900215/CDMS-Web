using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using System.Collections.Specialized;

using System.Runtime.InteropServices;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Aspose.Cells;
using Aspose.Words;
using Aspose.CAD;
using Aspose.CAD.ImageOptions;
using Aspose.CAD.FileFormats.Cad;

using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;

//using Teigha;
//using Teigha.GraphicsInterface;
//using Teigha.Export_Import;
//using Db = Teigha.DatabaseServices;
//using Rt = Teigha.Runtime;
//using Teigha.GraphicsSystem;

namespace AVEVA.CDMS.WebApi
{
    public class CDMSPdf
    {
        //public enum ShowCommands : int
        //{
        //    SW_HIDE = 0,
        //    SW_SHOWNORMAL = 1,
        //    SW_NORMAL = 1,
        //    SW_SHOWMINIMIZED = 2,
        //    SW_SHOWMAXIMIZED = 3,
        //    SW_MAXIMIZE = 3,
        //    SW_SHOWNOACTIVATE = 4,
        //    SW_SHOW = 5,
        //    SW_MINIMIZE = 6,
        //    SW_SHOWMINNOACTIVE = 7,
        //    SW_SHOWNA = 8,
        //    SW_RESTORE = 9,
        //    SW_SHOWDEFAULT = 10,
        //    SW_FORCEMINIMIZE = 11,
        //    SW_MAX = 11
        //}
        //[DllImport("shell32.dll")]
        //static extern IntPtr ShellExecute(
        //    IntPtr hwnd,
        //    string lpOperation,
        //    string lpFile,
        //    string lpParameters,
        //    string lpDirectory,
        //    ShowCommands nShowCmd);

        public static string ConvertDwgToPdfProcessPath;

        private static bool AsposeModifyInMemoryLock = false;
        //office文件转换pdf
        public static bool ConvertToPdf(string sourcePath, string targetPath)
        {
            //
            #region aspose16.2内存po jie 补丁
            //if (AsposeModifyInMemoryLock == false)
            //{
            //    AsposeModifyInMemoryLock = true;
            //    AsposeModifyInMemory.ActivateMemoryPatching();
            //} 
            #endregion

            bool result = false;
            //获取扩展名
            string ext = sourcePath.Substring(sourcePath.LastIndexOf(".") + 1, (sourcePath.Length - sourcePath.LastIndexOf(".") - 1)); //扩展名
            ext = ext.ToLower();
            if (ext == "doc" || ext == "docx")
            {
                Microsoft.Office.Interop.Word.WdExportFormat wordType = Microsoft.Office.Interop.Word.WdExportFormat.wdExportFormatPDF;
                result = ConvertToPdf(sourcePath, targetPath, wordType);
            }
            else if (ext == "xls" || ext == "xlsx")
            {
                Microsoft.Office.Interop.Excel.XlFixedFormatType excelType = Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF;
                result = ConvertToPdf(sourcePath, targetPath, excelType);
            }
            else if (ext == "dwg")
            {
                    result = ConverDwgToPdfByTd(sourcePath, targetPath);

                if (result==false)
                    result = ConvertDwgToPdf(sourcePath, targetPath);
               
            }
            return result;
        }

        private static bool ConverDwgToPdfByTd(string sourcePath, string targetPath)
        {
            if (ConvertDwgToPdfProcessPath == "undefind") {
                return false;
            }

            if (ConvertDwgToPdfProcessPath == null)
            {

                //获取配置文件
                string path = System.Web.Hosting.HostingEnvironment.MapPath(@"~/");

                string configFileName = path + "CDMSServerDB.config";
                XmlDocument xmlIDoc = new XmlDocument();
                xmlIDoc.Load(configFileName);
                XmlNode xmlNodeRoot = xmlIDoc.SelectSingleNode("CDMSDataSourceConfig");

                #region 获取转换程序路径
                XmlElement xmlNode = (XmlElement)xmlNodeRoot.SelectSingleNode("DwgToPdfProcessPath");

                if (xmlNode == null)
                {
                    //CDMSLog.WriteLog("共享Revit项目路径没有定义！");
                    //MessageBox.Show("服务器地址没有定义！");
                }
                else
                {
                    ConvertDwgToPdfProcessPath = xmlNode.InnerText;
                }

                #endregion

            }

            if (ConvertDwgToPdfProcessPath == null)
                ConvertDwgToPdfProcessPath = "undefind";

            string TdToolPath = ConvertDwgToPdfProcessPath;// @"D:\ConvertDwgToPdf\";

            string processFilename = TdToolPath + @"\" + @"ConvertDwgToPdf.exe";
            if (!File.Exists(processFilename))
            {
                return false;
            }
            try
            {
                string targetPathDir = System.IO.Path.GetDirectoryName(targetPath);

                if (!System.IO.Directory.Exists(targetPathDir))
                {
                    System.IO.Directory.CreateDirectory(targetPathDir);
                }

                Process process = new Process();


                string processPara = " \"" + sourcePath + "\" \"" + targetPath + "\"";

                //ShellExecute(IntPtr.Zero, "open", processFilename, processPara, TdToolPath, ShowCommands.SW_SHOWNORMAL);

                ProcessStartInfo startInfo = new ProcessStartInfo(processFilename, processPara); // 括号里是(程序名,参数)

                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                //指定默认目录，teigha将从此目录读取字体文件
                startInfo.WorkingDirectory = TdToolPath;

                process.StartInfo = startInfo;


                process.Start();
                //System.Diagnostics.Process.Start(processFilename, processPara);

                //等待revit文件打包完毕
                process.WaitForExit();

                return true;
            }
            catch { }
            return false;
        }

        // 将excel文档转换成PDF格式
        private static bool ConvertToPdf(string sourcePath, string targetPath, Microsoft.Office.Interop.Excel.XlFixedFormatType targetType)
        {
            bool result = false;
            try
            {
                Workbook workbook = new Workbook(sourcePath);
                workbook.Save(targetPath, Aspose.Cells.SaveFormat.Pdf);
                result = true;
            }
            catch
            {

            }
            return result;

        }

        //将word文档转换成PDF格式
        private static bool ConvertToPdf(string sourcePath, string targetPath, Microsoft.Office.Interop.Word.WdExportFormat exportFormat)
        {
            bool result = false;
            try
            {
                Aspose.Words.Document docpdf = new Aspose.Words.Document(sourcePath);
                docpdf.Save(targetPath, Aspose.Words.SaveFormat.Pdf);
                result = true;
            }
            catch { }
            return result;

        }

        // 将excel文档转换成PDF格式
        private static bool ConvertToPdfByAcrobat(string sourcePath, string targetPath, Microsoft.Office.Interop.Excel.XlFixedFormatType targetType)
        {
            bool result;
            object missing = Type.Missing;
            Microsoft.Office.Interop.Excel.ApplicationClass application = null;
            Microsoft.Office.Interop.Excel.Workbook workBook = null;
            try
            {
                application = new Microsoft.Office.Interop.Excel.ApplicationClass();
                object target = targetPath;
                object type = targetType;
                workBook = application.Workbooks.Open(sourcePath, missing, missing, missing, missing, missing,
                        missing, missing, missing, missing, missing, missing, missing, missing, missing);

                workBook.ExportAsFixedFormat(targetType, target, Microsoft.Office.Interop.Excel.XlFixedFormatQuality.xlQualityStandard, true, false, missing, missing, missing, missing);
                result = true;
            }
            catch
            {
                result = false;
            }
            finally
            {
                if (workBook != null)
                {
                    workBook.Close(true, missing, missing);
                    workBook = null;
                }
                if (application != null)
                {
                    application.Quit();
                    application = null;
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return result;
        }

        //将word文档转换成PDF格式
        private static bool ConvertToPdfByAcrobat(string sourcePath, string targetPath, Microsoft.Office.Interop.Word.WdExportFormat exportFormat)
        {
            bool result;
            object paramMissing = Type.Missing;
            Microsoft.Office.Interop.Word.ApplicationClass wordApplication = new Microsoft.Office.Interop.Word.ApplicationClass();
            Microsoft.Office.Interop.Word.Document wordDocument = null;
            try
            {
                object paramSourceDocPath = sourcePath;
                string paramExportFilePath = targetPath;

                Microsoft.Office.Interop.Word.WdExportFormat paramExportFormat = exportFormat;
                bool paramOpenAfterExport = false;
                Microsoft.Office.Interop.Word.WdExportOptimizeFor paramExportOptimizeFor =
                        Microsoft.Office.Interop.Word.WdExportOptimizeFor.wdExportOptimizeForPrint;
                Microsoft.Office.Interop.Word.WdExportRange paramExportRange = Microsoft.Office.Interop.Word.WdExportRange.wdExportAllDocument;
                int paramStartPage = 0;
                int paramEndPage = 0;
                Microsoft.Office.Interop.Word.WdExportItem paramExportItem = Microsoft.Office.Interop.Word.WdExportItem.wdExportDocumentContent;
                bool paramIncludeDocProps = true;
                bool paramKeepIRM = true;
                Microsoft.Office.Interop.Word.WdExportCreateBookmarks paramCreateBookmarks =
                        Microsoft.Office.Interop.Word.WdExportCreateBookmarks.wdExportCreateWordBookmarks;
                bool paramDocStructureTags = true;
                bool paramBitmapMissingFonts = true;
                bool paramUseISO19005_1 = false;

                wordDocument = wordApplication.Documents.Open(
                        ref paramSourceDocPath, ref paramMissing, ref paramMissing,
                        ref paramMissing, ref paramMissing, ref paramMissing,
                        ref paramMissing, ref paramMissing, ref paramMissing,
                        ref paramMissing, ref paramMissing, ref paramMissing,
                        ref paramMissing, ref paramMissing, ref paramMissing,
                        ref paramMissing);

                if (wordDocument != null)
                    wordDocument.ExportAsFixedFormat(paramExportFilePath,
                            paramExportFormat, paramOpenAfterExport,
                            paramExportOptimizeFor, paramExportRange, paramStartPage,
                            paramEndPage, paramExportItem, paramIncludeDocProps,
                            paramKeepIRM, paramCreateBookmarks, paramDocStructureTags,
                            paramBitmapMissingFonts, paramUseISO19005_1,
                            ref paramMissing);
                result = true;
            }
            finally
            {
                if (wordDocument != null)
                {
                    wordDocument.Close(ref paramMissing, ref paramMissing, ref paramMissing);
                    wordDocument = null;
                }
                if (wordApplication != null)
                {
                    wordApplication.Quit(ref paramMissing, ref paramMissing, ref paramMissing);
                    wordApplication = null;
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return result;
        }

        //private static bool ConverDwgToPdf(string sourcePath, string targetPath)
        //{
        //    Teigha.Export_Import.mPDFExportParams pdfEp = new Teigha.Export_Import.mPDFExportParams();

        //    //OdDbDatabasePtr pDb = m_pHostApp.readFile(sourcePath);
        //}

        /// <summary>
        /// 转换dwg for aspose.cad 19.9
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="targetPath"></param>
        /// <returns></returns>
        private static bool ConvertDwgToPdf(string sourcePath, string targetPath)
        {
            try
            {
                //ExStart:CADLayoutsToPDF
                // The path to the documents directory.
                //string MyDir = RunExamples.GetDataDir_ConvertingCAD();
                //string sourceFilePath = MyDir + "conic_pyramid.dxf";
                //string MyDir = Path.GetDirectoryName(sourcePath);

                Stopwatch stopWatch = new Stopwatch();

                try
                {
                    stopWatch.Start();
                    
                    // Create an instance of CadImage class and load the file.
                    using (CadImage cadImage = (CadImage)Image.Load(sourcePath))
                    {
                        stopWatch.Stop();

                        // 将经过的时间作为TimeSpan值获取. 
                        TimeSpan ts = stopWatch.Elapsed;

                        //格式化并显示TimeSpan值. 
                        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                            ts.Hours, ts.Minutes, ts.Seconds,
                            ts.Milliseconds / 10);
                        Console.WriteLine("RunTime for loading " + elapsedTime);

                        CadRasterizationOptions rasterizationOptions = new CadRasterizationOptions();
                        rasterizationOptions.PageWidth = 1600;
                        rasterizationOptions.PageHeight = 1600;
                        PdfOptions pdfOptions = new PdfOptions();
                        pdfOptions.VectorRasterizationOptions = rasterizationOptions;

                        stopWatch = new Stopwatch();
                        stopWatch.Start();
                        cadImage.Save(targetPath, pdfOptions);
                        stopWatch.Stop();

                        //将经过的时间作为TimeSpan值获取. 
                        ts = stopWatch.Elapsed;

                        //格式化并显示TimeSpan值. 
                        elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                           ts.Hours, ts.Minutes, ts.Seconds,
                           ts.Milliseconds / 10);
                        Console.WriteLine("RunTime for converting " + elapsedTime);

                     
                    }
                    //ExEnd:CADLayoutsToPDF            
                    //Console.WriteLine("\n3D images exported successfully to PDF.\nFile saved at " + MyDir);
                    return true;
                }
                catch { }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
            return false;

        }

        /// <summary>
        /// 转换dwg for aspose.cad 16.2
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="targetPath"></param>
        /// <returns></returns>
        private static bool ConvertDwgToPdf2(string sourcePath, string targetPath)
        {
            //try
            //{
            //    //ExStart:CADLayoutsToPDF
            //    // The path to the documents directory.
            //    //string MyDir = RunExamples.GetDataDir_ConvertingCAD();
            //    //string sourceFilePath = MyDir + "conic_pyramid.dxf";
            //    //string MyDir = Path.GetDirectoryName(sourcePath);

            //    // Create an instance of CadImage class and load the file.
            //    using (Aspose.CAD.Image cadImage = (Aspose.CAD.Image)Image.Load(sourcePath))
            //    {
            //        // Create an instance of CadRasterizationOptions class
            //        CadRasterizationOptions rasterizationOptions = new CadRasterizationOptions();
            //        //rasterizationOptions.PageWidth = 1600;
            //        //rasterizationOptions.PageHeight = 1600;
            //        rasterizationOptions.PageWidth = 2970;
            //        rasterizationOptions.PageHeight = 4200;

            //        // Set the Entities type property to Entities3D.
            //        //rasterizationOptions.TypeOfEntities = TypeOfEntities.Entities3D;

            //        rasterizationOptions.AutomaticLayoutsScaling = true;
            //        //rasterizationOptions.NoScaling = false;
            //        //rasterizationOptions.ScaleMethod = ScaleType.GrowToFit;
            //        rasterizationOptions.ScaleMethod = ScaleType.None;
            //        rasterizationOptions.ContentAsBitmap = true;

            //        // Set Layouts
            //        rasterizationOptions.Layouts = new string[] { "Model" };

            //        // Create an instance of PDF options class
            //        PdfOptions pdfOptions = new PdfOptions();
            //        pdfOptions.VectorRasterizationOptions = rasterizationOptions;

            //        //MyDir = MyDir + "CADLayoutsToPDF_out.pdf";

            //        // Set Graphics options
            //        rasterizationOptions.GraphicsOptions.SmoothingMode = SmoothingMode.HighQuality;
            //        rasterizationOptions.GraphicsOptions.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            //        rasterizationOptions.GraphicsOptions.InterpolationMode = InterpolationMode.HighQualityBicubic;

            //        //Export to PDF by calling the Save method
            //        cadImage.Save(targetPath, pdfOptions);
            //    }
            //    //ExEnd:CADLayoutsToPDF            
            //    //Console.WriteLine("\n3D images exported successfully to PDF.\nFile saved at " + MyDir);
            //    return true;
            //}
            //catch (Exception ex)
            //{
            //    string msg = ex.Message;
            //}
            return false;

        }

        //private static bool ConverDwgToPdfByTd(string sourcePath, string targetPath)
        //{
        //    //A3纸大小：297*420
        //    string PapWidth = "297";
        //    string PapHeight = "420";

        //    bool bEmbedded_fonts = true;
        //    bool bSHXTextAsGeometry = true;
        //    bool bTTGeometry = true;
        //    bool bESimpGeometryOpt = true;
        //    bool bZoomExtents = true;
        //    bool bEnableLayerSup_pdfv1_5 = true;
        //    bool bExportOffLay = true;

        //    string textTitle = "标题";
        //    string textAuthor = "作者";
        //    string textSubject = "Subject";
        //    string textKeywords = "Keywords";
        //    string textCreator = "创建者";
        //    string textProducer = "制作人";

        //    bool bUseHidLRAlgorithm = false;
        //    bool bRadioButton_All = true;
        //    bool bEncodeStream = true;

        //    //pdf版本 
        //    PDFExportVersions pdfVer = PDFExportVersions.PDFv1_6;

        //    Db.Database database;

        //    using (Rt.Services srv = new Rt.Services())
        //    {
        //        using (database = new Db.Database(false, true))
        //        {
        //            String fileName = sourcePath;// @"D:\data\123.dwg";
        //            database.ReadDwgFile(fileName, Db.FileOpenMode.OpenForReadAndWriteNoShare,
        //              true, null, false);


        //            if (targetPath.Length > 0)
        //            {
        //                using (mPDFExportParams param = new mPDFExportParams())
        //                {
        //                    param.Database = database;
        //                    using (Rt.FileStreamBuf fileStrem = new Rt.FileStreamBuf(targetPath, false, Rt.FileShareMode.DenyNo, Rt.FileCreationDisposition.CreateAlways))
        //                    {
        //                        param.OutputStream = fileStrem;

        //                        param.Flags = (bEmbedded_fonts ? PDFExportFlags.EmbededTTF : 0) |
        //                                      (bSHXTextAsGeometry ? PDFExportFlags.SHXTextAsGeometry : 0) |
        //                                      (bTTGeometry ? PDFExportFlags.TTFTextAsGeometry : 0) |
        //                                      (bESimpGeometryOpt ? PDFExportFlags.SimpleGeomOptimization : 0) |
        //                                      (bZoomExtents ? PDFExportFlags.ZoomToExtentsMode : 0) |
        //                                      (bEnableLayerSup_pdfv1_5 ? PDFExportFlags.EnableLayers : 0) |
        //                                      (bExportOffLay ? PDFExportFlags.IncludeOffLayers : 0);


        //                        param.Title = textTitle;
        //                        param.Author = textAuthor;
        //                        param.Subject = textSubject;
        //                        param.Keywords = textKeywords;
        //                        param.Creator = textCreator;
        //                        param.Producer = textProducer;
        //                        param.UseHLR = bUseHidLRAlgorithm;
        //                        param.ASCIIHEXEncodeStream = bEncodeStream;
        //                        param.Versions = pdfVer;


        //                        StringCollection strColl = new StringCollection();
        //                        if (bRadioButton_All)
        //                        {
        //                            using (Db.DBDictionary layouts = (Db.DBDictionary)database.LayoutDictionaryId.GetObject(Db.OpenMode.ForRead))
        //                            {
        //                                foreach (Db.DBDictionaryEntry entry in layouts)
        //                                {
        //                                    strColl.Add(entry.Key);
        //                                }
        //                            }
        //                        }
        //                        param.Layouts = strColl;

        //                        int nPages = Math.Max(1, strColl.Count);
        //                        PageParamsCollection pParCol = new PageParamsCollection();
        //                        Double width = Double.Parse(PapWidth);
        //                        Double height = Double.Parse(PapHeight);
        //                        for (int i = 0; i < nPages; ++i)
        //                        {
        //                            PageParams pp = new PageParams();
        //                            pp.setParams(width, height);
        //                            pParCol.Add(pp);
        //                        }
        //                        param.PageParams = pParCol;
        //                        Export_Import.ExportPDF(param);
        //                    }
        //                }
        //            }
        //        }
        //        //Close();

        //        return true;
        //    }
        //}
    }
}
