using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Specialized;

using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Windows.Forms; 


using Teigha;
using Teigha.GraphicsInterface;
using Teigha.Export_Import;
using Db = Teigha.DatabaseServices;
using Rt = Teigha.Runtime;
using Teigha.GraphicsSystem;

using Teigha.Geometry; 


namespace ConvertDwgToPdf
{
    public class CDMSPdf
    {
        internal static bool ConverDwgToPdfByTd2(string sourcePath, string targetPath)
        {
            //A3纸大小：297*420
            string PapWidth = "297";
            string PapHeight = "420";

            bool bEmbedded_fonts = true;
            bool bSHXTextAsGeometry = true;
            bool bTTGeometry = true;
            bool bESimpGeometryOpt = true;
            bool bZoomExtents = true;
            bool bEnableLayerSup_pdfv1_5 = true;
            bool bExportOffLay = true;

            string textTitle = "标题";
            string textAuthor = "作者";
            string textSubject = "Subject";
            string textKeywords = "Keywords";
            string textCreator = "创建者";
            string textProducer = "制作人";

            bool bUseHidLRAlgorithm = false;
            bool bRadioButton_All = true;
            bool bEncodeStream = true;

            //pdf版本 
            PDFExportVersions pdfVer = PDFExportVersions.PDFv1_6;

            Db.Database database;

            using (Rt.Services srv = new Rt.Services())
            {
                using (database = new Db.Database(false, true))
                {
                    String fileName = sourcePath;// @"D:\data\123.dwg";
                    //String fileName =  @"D:\data\123.dwg";
                    database.ReadDwgFile(fileName, Db.FileOpenMode.OpenForReadAndWriteNoShare,
                      true, null, false);


                    if (targetPath.Length > 0)
                    {
                        using (mPDFExportParams param = new mPDFExportParams())
                        {
                            param.Database = database;
                            using (Rt.FileStreamBuf fileStrem = new Rt.FileStreamBuf(targetPath, false, Rt.FileShareMode.DenyNo, Rt.FileCreationDisposition.CreateAlways))
                            {
                                param.OutputStream = fileStrem;

                                param.Flags = (bEmbedded_fonts ? PDFExportFlags.EmbededTTF : 0) |
                                              (bSHXTextAsGeometry ? PDFExportFlags.SHXTextAsGeometry : 0) |
                                              (bTTGeometry ? PDFExportFlags.TTFTextAsGeometry : 0) |
                                              (bESimpGeometryOpt ? PDFExportFlags.SimpleGeomOptimization : 0) |
                                              (bZoomExtents ? PDFExportFlags.ZoomToExtentsMode : 0) |
                                              (bEnableLayerSup_pdfv1_5 ? PDFExportFlags.EnableLayers : 0) |
                                              (bExportOffLay ? PDFExportFlags.IncludeOffLayers : 0);


                                param.Title = textTitle;
                                param.Author = textAuthor;
                                param.Subject = textSubject;
                                param.Keywords = textKeywords;
                                param.Creator = textCreator;
                                param.Producer = textProducer;
                                param.UseHLR = bUseHidLRAlgorithm;
                                param.ASCIIHEXEncodeStream = bEncodeStream;
                                param.Versions = pdfVer;


                                StringCollection strColl = new StringCollection();
                                if (bRadioButton_All)
                                {
                                    using (Db.DBDictionary layouts = (Db.DBDictionary)database.LayoutDictionaryId.GetObject(Db.OpenMode.ForRead))
                                    {
                                        foreach (Db.DBDictionaryEntry entry in layouts)
                                        {
                                            strColl.Add(entry.Key);
                                        }
                                    }
                                }
                                param.Layouts = strColl;

                                int nPages = Math.Max(1, strColl.Count);
                                PageParamsCollection pParCol = new PageParamsCollection();
                                Double width = Double.Parse(PapWidth);
                                Double height = Double.Parse(PapHeight);
                                for (int i = 0; i < nPages; ++i)
                                {
                                    PageParams pp = new PageParams();
                                    pp.setParams(width, height);
                                    pParCol.Add(pp);
                                }
                                param.PageParams = pParCol;
                                Export_Import.ExportPDF(param);
                            }
                        }
                    }
                }
                //Close();

                return true;
            }
        }

        internal static bool ConverDwgToPdfByTd(string sourcePath, string targetPath)
        {
            //string sourcePath = "";
            //string targetPath = "";
            //sourcePath = @"C:\Temp\1号楼照明平面图.dwg";
            //targetPath = @"d:\textcdwg.pdf";

            //A3纸大小：297*420
            string PapWidth = "2970";
            string PapHeight = "4200";

            bool bEmbedded_fonts = true;
            bool bSHXTextAsGeometry = true;
            bool bTTGeometry = true;
            bool bESimpGeometryOpt = true;
            bool bZoomExtents = true;
            bool bEnableLayerSup_pdfv1_5 = true;
            bool bExportOffLay = true;

            string textTitle = "标题";
            string textAuthor = "作者";
            string textSubject = "Subject";
            string textKeywords = "Keywords";
            string textCreator = "创建者";
            string textProducer = "制作人";

            bool bUseHidLRAlgorithm = false;
            bool bRadioButton_All = true;
            bool bEncodeStream = true;

            //pdf版本 
            PDFExportVersions pdfVer = PDFExportVersions.PDFv1_5;

            Db.Database database;

            using (Rt.Services srv = new Rt.Services())
            {
                //using (database = new Db.Database(false, true))
                using (database = new Db.Database(false, false))
                {
                    String fileName = sourcePath;// @"D:\data\123.dwg";
                    database.ReadDwgFile(fileName, Db.FileOpenMode.OpenForReadAndWriteNoShare,
                      true, null, false);

                    //Db.AuditInfo ai = new Db.AuditInfo();
                    //ai.FixErrors = true; // Errors must be fixed
                    //db.Audit(ai);
                    //db.SaveAs(fileName, Db.DwgVersion.AC1021);

                    //// Output: Total errors found: 54, fixed: 54.
                    //Console.WriteLine("Total errors found: {0}, fixed: {1}.",
                    //  ai.NumErrors, ai.NumFixes);


                    if (targetPath.Length > 0)
                    {
                        using (mPDFExportParams param = new mPDFExportParams())
                        {
                            param.Database = database;
                            using (Rt.FileStreamBuf fileStrem = new Rt.FileStreamBuf(targetPath, false, Rt.FileShareMode.DenyNo, Rt.FileCreationDisposition.CreateAlways))
                            {
                                param.OutputStream = fileStrem;

                                param.Flags = (bEmbedded_fonts ? PDFExportFlags.EmbededTTF : 0) |
                                              (bSHXTextAsGeometry ? PDFExportFlags.SHXTextAsGeometry : 0) |
                                              (bTTGeometry ? PDFExportFlags.TTFTextAsGeometry : 0) |
                                              (bESimpGeometryOpt ? PDFExportFlags.SimpleGeomOptimization : 0) |
                                              (bZoomExtents ? PDFExportFlags.ZoomToExtentsMode : 0) |
                                              (bEnableLayerSup_pdfv1_5 ? PDFExportFlags.EnableLayers : 0) |
                                              (bExportOffLay ? PDFExportFlags.IncludeOffLayers : 0);

                                //param.Title = textBox_title.Text;
                                //param.Author = textBox_author.Text;
                                //param.Subject = textBox_subject.Text;
                                //param.Keywords = textBox_keywords.Text;
                                //param.Creator = textBox_creator.Text;
                                //param.Producer = textBox_producer.Text;
                                //param.UseHLR = UseHidLRAlgorithm.Checked;
                                ////param.EncodeStream = EncodedSZ.Checked;
                                //param.ASCIIHEXEncodeStream = EncodedSZ.Checked;

                                //bool bV15 = EnableLayerSup_pdfv1_5.Checked || ExportOffLay.Checked;
                                //param.Versions = PDFExportVersions.PDFv1_5 : PDFExportVersions.PDFv1_4;

                                param.Title = textTitle;
                                param.Author = textAuthor;
                                param.Subject = textSubject;
                                param.Keywords = textKeywords;
                                param.Creator = textCreator;
                                param.Producer = textProducer;
                                param.UseHLR = bUseHidLRAlgorithm;
                                param.ASCIIHEXEncodeStream = bEncodeStream;
                                param.Versions = pdfVer;


                                //using Teigha;
                                //using Teigha.GraphicsInterface;
                                //using Teigha.Export_Import;
                                //using Db = Teigha.DatabaseServices;
                                //using Rt = Teigha.Runtime;
                                //using Teigha.GraphicsSystem;

                                StringCollection strColl = new StringCollection();
                                if (bRadioButton_All)
                                {
                                    using (Db.DBDictionary layouts = (Db.DBDictionary)database.LayoutDictionaryId.GetObject(Db.OpenMode.ForRead))
                                    {
                                        foreach (Db.DBDictionaryEntry entry in layouts)
                                        {
                                            strColl.Add(entry.Key);
                                        }
                                    }
                                }
                                param.Layouts = strColl;

                                int nPages = Math.Max(1, strColl.Count);
                                PageParamsCollection pParCol = new PageParamsCollection();
                                Double width = Double.Parse(PapWidth);
                                Double height = Double.Parse(PapHeight);
                                for (int i = 0; i < nPages; ++i)
                                {
                                    PageParams pp = new PageParams();
                                    pp.setParams(width, height);
                                    pParCol.Add(pp);
                                }
                                param.PageParams = pParCol;
                                Export_Import.ExportPDF(param);

                            }
                        }
                    }
                }
                //Close();

                return true;
            }

        }

        internal static bool ConverDwgToBitmap(string sourcePath, string targetPath)
        {
            try
            {
                //是否绘图生成
                bool bPlotGeneration = true;

                Db.Database database;

                int BitPerPixel = 6;

                System.Drawing.Color bgColor = System.Drawing.Color.White;
                System.Drawing.Color bgColor2 = System.Drawing.Color.Black;

                int PapWidth = 2970;
                int PapHeight = 4200;

                using (Rt.Services srv = new Rt.Services())
                {
                    using (database = new Db.Database(false, true))
                    {
                        //using (GsModule gsModule = (GsModule)Rt.SystemObjects.DynamicLinker.LoadModule("WinDirectX.txv", false, true))
                        using (GsModule gsModule = (GsModule)Rt.SystemObjects.DynamicLinker.LoadModule(sourcePath, false, true))
                        {
                            // create graphics device 
                            using (Teigha.GraphicsSystem.Device dev = gsModule.CreateBitmapDevice())
                            {
                                String fileName = sourcePath;// @"D:\data\123.dwg";
                                database.ReadDwgFile(fileName, Db.FileOpenMode.OpenForReadAndWriteNoShare,
                                  true, null, false);

                                // setup device properties 
                                using (Rt.Dictionary props = dev.Properties)
                                {
                                    //props.AtPut("BitPerPixel", new Rt.RxVariant(int.Parse(comboBox1.Text)));
                                    props.AtPut("BitPerPixel", new Rt.RxVariant(6));
                                }
                                ContextForDbDatabase ctx = new ContextForDbDatabase(database);
                                //ctx.PaletteBackground = colorDialog.Color;
                                ctx.PaletteBackground = bgColor;

                                LayoutHelperDevice helperDevice = LayoutHelperDevice.SetupActiveLayoutViews(dev, ctx);

                                helperDevice.SetLogicalPalette(Device.LightPalette); // light palette 

                                Rectangle rect = new Rectangle(0, 0, PapWidth, PapHeight);
                                helperDevice.OnSize(rect);

                                ctx.SetPlotGeneration(bPlotGeneration);
                                if (ctx.IsPlotGeneration)
                                    helperDevice.BackgroundColor = bgColor;
                                else
                                    helperDevice.BackgroundColor = bgColor2;
                                //helperDevice.BackgroundColor = System.Drawing.FromArgb(0, 173, 174, 173);


                                helperDevice.Update();

                                Export_Import.ExportBitmap(helperDevice, targetPath);

                                return true;
                                //Close();
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
            return false;
        }

        //    internal static bool ConverDwgToPdfByTd3(string sourcePath, string targetPath){

        //                  //A3纸大小：297*420
        //        string PapWidth = "297";
        //        string PapHeight = "420";

        //        bool bEmbedded_fonts = true;
        //        bool bSHXTextAsGeometry = true;
        //        bool bTTGeometry = true;
        //        bool bESimpGeometryOpt = true;
        //        bool bZoomExtents = true;
        //        bool bEnableLayerSup_pdfv1_5 = true;
        //        bool bExportOffLay = true;

        //        string textTitle = "标题";
        //        string textAuthor = "作者";
        //        string textSubject = "Subject";
        //        string textKeywords = "Keywords";
        //        string textCreator = "创建者";
        //        string textProducer = "制作人";

        //        bool bUseHidLRAlgorithm = false;
        //        bool bRadioButton_All = true;
        //        bool bEncodeStream = true;

        //    using (mPDFExportParams param = new mPDFExportParams()) 
        //    { 
        //      param.Database = database; 
        //      using (FileStreamBuf fileStrem = new FileStreamBuf(outputFile.Text, false, FileShareMode.DenyNo, FileCreationDisposition.CreateAlways)) 
        //      { 
        //        param.OutputStream = fileStrem; 

        //                            param.Flags = (bEmbedded_fonts ? PDFExportFlags.EmbededTTF : 0) |
        //                                          (bSHXTextAsGeometry ? PDFExportFlags.SHXTextAsGeometry : 0) |
        //                                          (bTTGeometry ? PDFExportFlags.TTFTextAsGeometry : 0) |
        //                                          (bESimpGeometryOpt ? PDFExportFlags.SimpleGeomOptimization : 0) |
        //                                          (bZoomExtents ? PDFExportFlags.ZoomToExtentsMode : 0) |
        //                                          (bEnableLayerSup_pdfv1_5 ? PDFExportFlags.EnableLayers : 0) |
        //                                          (bExportOffLay ? PDFExportFlags.IncludeOffLayers : 0);

        //                            param.Title = textTitle;
        //                            param.Author = textAuthor;
        //                            param.Subject = textSubject;
        //                            param.Keywords = textKeywords;
        //                            param.Creator = textCreator;
        //                            param.Producer = textProducer;
        //                            param.UseHLR = bUseHidLRAlgorithm;
        //                            param.ASCIIHEXEncodeStream = bEncodeStream;
        //                            param.Versions = pdfVer;

        //        StringCollection strColl = new StringCollection(); 
        //        if (radioButton_All.Checked) 
        //        { 
        //          using (DBDictionary layouts = (DBDictionary)database.LayoutDictionaryId.GetObject(OpenMode.ForRead)) 
        //          { 
        //            foreach (DBDictionaryEntry entry in layouts) 
        //            { 
        //              strColl.Add(entry.Key); 
        //            } 
        //          } 
        //        } 
        //        param.Layouts = strColl; 

        //        int nPages = Math.Max(1, strColl.Count); 
        //        PageParamsCollection pParCol = new PageParamsCollection(); 
        //        Double width = Double.Parse(PapWidth.Text); 
        //        Double height = Double.Parse(PapHeight.Text); 
        //        for (int i = 0; i < nPages; ++i) 
        //        { 
        //          PageParams pp = new PageParams(); 
        //          pp.setParams(width, height); 
        //          pParCol.Add(pp); 
        //        } 
        //        param.PageParams = pParCol; 
        //        Export_Import.ExportPDF(param); 
        //      } 
        //    } 
        //    //Close(); 


        //}
    }
}
