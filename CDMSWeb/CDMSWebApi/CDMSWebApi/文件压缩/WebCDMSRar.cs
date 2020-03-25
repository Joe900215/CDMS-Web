using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpCompress;

namespace AVEVA.CDMS.WebApi
{
    class WebCDMSRar
    {

        private static Object SharpUnRarLock = new Object();
        //解压rar文件
        //sourceFilePath:要解压的文件, targetPath:指定要解压文件到哪个路径，
        public static JArray SharpUnRar(string sourceFilePath, string targetPath)
        {
            JArray reJa = new JArray();

            //解压文件线程锁，生效后文件一个一个地解压。
            //同时解压对服务器CPU的压力大一些，一个一个解压客户端等待的时间长一些。
            //lock (SharpUnRarLock)
            {
                //解压文档
                using (Stream stream = System.IO.File.OpenRead(sourceFilePath))
                {
                    try
                    {
                        var reader = SharpCompress.Reader.ReaderFactory.Open(stream);
                        while (reader.MoveToNextEntry())
                        {

                            //建立解压文件里面的目录
                            if (reader.Entry.IsDirectory)
                            {
                                try
                                {
                                    string unRarItemDir = targetPath + reader.Entry.FilePath;
                                    if (!Directory.Exists(unRarItemDir))
                                    {
                                        Directory.CreateDirectory(unRarItemDir);
                                    }
                                    string path2 = "", dirName = reader.Entry.FilePath, fileWebPath = "";
                                    string filePath = reader.Entry.FilePath;
                                    int begin = filePath.LastIndexOf(@"\");
                                    string parentPath = ""; //filePath.Substring(0, begin);
                                    int end = filePath.Length;
                                    if (begin >= 0)
                                    {
                                        parentPath = (filePath.Substring(0, begin)).Replace("\\", "/");
                                        begin = begin + 1;
                                        path2 = filePath.Substring(0, begin);
                                        fileWebPath = path2.Replace("\\", "/");
                                        dirName = filePath.Substring(begin, end - begin);
                                    }

                                    string upPath = (parentPath == "" ? "" : (parentPath + "/")) + dirName;
                                    //添加返回上一级目录
                                    reJa.Add(new JObject(new JProperty("type", "dir"), new JProperty("name", ".."),
                                            new JProperty("size", ""), new JProperty("path", parentPath), new JProperty("parentpath", upPath))); //".."

                                    //添加目录
                                    reJa.Add(new JObject(new JProperty("type", "dir"), new JProperty("name", dirName),
                                        new JProperty("size", ""), new JProperty("path", fileWebPath), new JProperty("parentpath", parentPath)));

                                }
                                catch { }
                            }
                        }
                        reader.Dispose();
                    }
                    catch { }
                }
                //解压文件
                using (Stream stream = System.IO.File.OpenRead(sourceFilePath))
                {
                    try
                    {
                        var reader = SharpCompress.Reader.ReaderFactory.Open(stream);
                        while (reader.MoveToNextEntry())
                        {
                            if (!reader.Entry.IsDirectory)
                            {
                                try
                                {
                                    Console.WriteLine(reader.Entry.FilePath);
                                    string unrarFileName = reader.Entry.FilePath;
                                    Stream stream2 = System.IO.File.Create(targetPath + unrarFileName);
                                    reader.WriteEntryTo(stream2);
                                    stream2.Dispose();

                                    string path2 = "", fileName = reader.Entry.FilePath, fileWebPath = "";
                                    string filePath = reader.Entry.FilePath;
                                    int begin = filePath.LastIndexOf(@"\");
                                    string parentPath = "";// filePath.Substring(0, begin);
                                    int end = filePath.Length;
                                    if (begin >= 0)
                                    {
                                        parentPath = (filePath.Substring(0, begin)).Replace("\\", "/");
                                        begin = begin + 1;
                                        path2 = filePath.Substring(0, begin);
                                        //fileWebPath = path2.Replace("\\", "/");
                                        fileName = filePath.Substring(begin, end - begin);
                                    }


                                    reJa.Add(new JObject(new JProperty("type", "file"), new JProperty("name", fileName),
                                        new JProperty("size", FormatFileSize((int)reader.Entry.Size)),
                                        new JProperty("path", fileWebPath),
                                        new JProperty("parentpath", parentPath)));
                                }
                                catch (Exception ex) { }
                            }
                        }
                        reader.Dispose();
                    }
                    catch { }
                }
                //break;
            }

            return reJa;
        }

        public static JArray SharpZip(string sourceFilePath, string targetPath) {
            //var Writer= SharpCompress.Writer.WriterFactory.Open();
            //Stream stream = System.IO.File.Create(sourceFilePath);
            using (Stream stream = File.OpenWrite(sourceFilePath))
            using (var writer = SharpCompress.Writer.WriterFactory.Open(stream,SharpCompress.Common.ArchiveType.Zip, SharpCompress.Common.CompressionType.Rar))//stream))
            {
                writer.Write(targetPath,stream,DateTime.Now);//.WriteAll(filesPath, "*", SearchOption.AllDirectories);
            }

            //using (var archive = SharpCompress.Archive.Zip.ZipArchive.Create())
            //{
            //    Stream stream = System.IO.File.Create(sourceFilePath);
            //    archive.AddEntry(targetPath, stream);//.AddAllFromDirectoryEntry(@"C:\\source");
            //    //archive.SaveTo(targetPath);
            //}
            return new JArray();
        }
        private static String FormatFileSize(Int64 fileSize)
        {
            if (fileSize < 0)
            {
                throw new ArgumentOutOfRangeException("fileSize");
            }
            else if (fileSize >= 1024 * 1024 * 1024)
            {
                return string.Format("{0:########0.00} GB", ((Double)fileSize) / (1024 * 1024 * 1024));
            }
            else if (fileSize >= 1024 * 1024)
            {
                return string.Format("{0:####0.00} MB", ((Double)fileSize) / (1024 * 1024));
            }
            else if (fileSize >= 1024)
            {
                return string.Format("{0:####0.00} KB", ((Double)fileSize) / 1024);
            }
            else
            {
                return string.Format("{0} bytes", fileSize);
            }
        } 
    }
}
