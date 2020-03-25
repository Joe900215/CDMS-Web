using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ConvertDwgToPdf
{
    class Program
    {
        static void Main(string[] args)
        {
            string sourcePath = @"C:\Temp\1号楼照明平面图.dwg";
            //string sourcePath = @"C:\Temp\TKA13.dwg";
            //string sourcePath = @"C:\Temp\CEEC_TK_A1++_V.dwg";
            string targetPath = @"d:\Temp\textcdwg.pdf";
            string targetBmpPath = @"d:\Temp\textcdwg.jpg";

            if (args.Length >= 2)
            {
                //foreach (var item in args)
                {
                    sourcePath = args[0];
                    targetPath = args[1];
                }
            }


            CDMSPdf.ConverDwgToPdfByTd(sourcePath, targetPath);

            CDMSPdf.ConverDwgToBitmap(sourcePath, targetBmpPath);

            //ViewDWG viewDWG = new ViewDWG();
            //System.Drawing.Image img = viewDWG.GetDwgImage(sourcePath);

            //using (var bmp = new System.Drawing.Bitmap(img.Width, img.Height))
            //{
            //    bmp.SetResolution(img.HorizontalResolution, img.VerticalResolution);

            //    using (var g = System.Drawing.Graphics.FromImage(bmp))
            //    {
            //        g.Clear(System.Drawing.Color.White);
            //        g.DrawImageUnscaled(img, 0, 0);
            //    }
            //    bmp.Save(targetBmpPath, System.Drawing.Imaging.ImageFormat.Jpeg);

            //    //  if (File.Exists(Application.StartupPath + "\\Temp.jpg"))
            //    //  {
            //    //      byte[] bTemp = File.ReadAllBytes(Application.StartupPath + "\\Temp.jpg");
            //    //      bResult = bTemp;
            //    //  }

            //}
        }
    }
}
