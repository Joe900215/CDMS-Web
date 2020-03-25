using AVEVA.CDMS.Server;
using AVEVA.CDMS.WebApi;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AVEVA.CDMS.HXEPC_Plugins
{
    public class ExportStatisProper
    {

        public static JObject ExportProper(string sid, string ProjectKeyword, string ConditionalAttrJson)
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
                var  project = dbsource.GetProjectByKeyWord(ProjectKeyword);

                if (project == null)
                {
                    reJo.msg = "提交的project对象不存在";
                    return reJo.Value;
                }

               // int docTotal = project.DocList.Count;// DocCount;

                //存放用户选的条件信息
                Dictionary<string, string> paramDictionary = new Dictionary<string, string>();

                if (!string.IsNullOrEmpty(ConditionalAttrJson))//条件
                {
                    JArray jaAttr = (JArray)JsonConvert.DeserializeObject(ConditionalAttrJson);

                    foreach (JObject joAttr in jaAttr)
                    {
                        string strName = joAttr["name"].ToString();
                        string strValue = joAttr["value"].ToString();

                        //获取生成的起始日期
                        if (strName == "stardata") paramDictionary.Add("stardata", strValue);//stardata = strValue; 
                        //获取生成的终止日期
                        else if (strName == "enddata") paramDictionary.Add("enddata", strValue);//enddata = strValue;
                        //获取发送方
                        else if (strName == "sendCompany") paramDictionary.Add("sendCompany", strValue);//sendCompany = strValue;
                        //获取接收方
                        else if (strName == "recCompany") paramDictionary.Add("recCompany", strValue);//recCompany = strValue;
                        //是否回复
                        else if (strName == "reply") { if (strValue == "1") { paramDictionary.Add("reply", "是"); } else if (strValue == "0") { paramDictionary.Add("reply", "否"); } else  if (strValue == "") { paramDictionary.Add("reply", ""); } else  { paramDictionary.Add("reply", "是或否"); } }
                    }
                }

                List<Doc> projDocList = new List<Doc>();

                string searchSQL = "";

                if (!string.IsNullOrEmpty(paramDictionary["stardata"]))
                {
                    DateTime starDateTime = DateTime.ParseExact(paramDictionary["stardata"], "yyyy年MM月dd日", System.Globalization.CultureInfo.CurrentCulture);

                    if (!string.IsNullOrEmpty(paramDictionary["enddata"]))
                    {
                        DateTime endDateTime = DateTime.ParseExact(paramDictionary["enddata"], "yyyy年MM月dd日", System.Globalization.CultureInfo.CurrentCulture);
                        endDateTime = new DateTime(endDateTime.Year, endDateTime.Month, endDateTime.Day, 23, 59, 59);//23时59分59秒
                        if (searchSQL == "")
                        {
                            searchSQL = string.Concat(new object[] { " o_credatetime between '", starDateTime, "' and '", endDateTime, "'" });
                        }
                        else
                        {
                            object obj2 = searchSQL;
                            searchSQL = string.Concat(new object[] { obj2, "and o_credatetime between '", starDateTime, "' and '", endDateTime, "'" });
                        }
                    }
                }

                #region Add projDocList

                if (project != null)
                {
                    //查找指定的目录下的文档(查询语句包括了子目录)
                    if (searchSQL == "")
                        searchSQL = searchSQL + "(o_projectno IN (select [ID] from CDMS_FindSubDoc('" + project.O_projectno.ToString() + "'))) ";
                    else
                        searchSQL = searchSQL + "AND (o_projectno IN (select [ID] from CDMS_FindSubDoc('" + project.O_projectno.ToString() + "'))) ";

                    searchSQL = dbsource.ParseExpression(project, searchSQL);
                    searchSQL = "Select * FROM CDMS_Doc Where " + searchSQL;//这个查询只查 该projectkey下的 ，/创建日期在传入的时间里的。著录表筛选不写在 SQL语句里

                    var tempprojDocList = dbsource.SelectDoc(searchSQL, true);
                    var filterlist = tempprojDocList;
                    // projDocList.AddRange(tempprojDocList);
                    //著录属性筛选

                    if (!String.IsNullOrEmpty(paramDictionary["sendCompany"]))
                    {
                        if (filterlist != null && filterlist.Count > 0)
                        filterlist = filterlist.FindAll(t => (t.GetAttrDataByKeyWord("CA_SENDERCODE") == null ? "" : t.GetAttrDataByKeyWord("CA_SENDERCODE").ToString) == paramDictionary["sendCompany"]);
                    }

                    if (!String.IsNullOrEmpty(paramDictionary["recCompany"]))
                    {
                        if (filterlist != null && filterlist.Count > 0)
                        filterlist = filterlist.FindAll(t => (t.GetAttrDataByKeyWord("CA_MAINFEEDERCODE") == null ? "" : t.GetAttrDataByKeyWord("CA_MAINFEEDERCODE").ToString) == paramDictionary["recCompany"]);
                    }
                    if (!String.IsNullOrEmpty(paramDictionary["reply"]))
                    {
                        if (paramDictionary["reply"] == "是或否")
                        {
                            if (filterlist != null && filterlist.Count > 0)
                                filterlist = filterlist.FindAll(t => (t.GetAttrDataByKeyWord("CA_IFREPLY") == null ? "" : t.GetAttrDataByKeyWord("CA_IFREPLY").ToString) == "是" || (t.GetAttrDataByKeyWord("CA_IFREPLY") == null ? "" : t.GetAttrDataByKeyWord("CA_IFREPLY").ToString) == "否");
                        }
                        else
                        {
                            if (filterlist != null && filterlist.Count > 0)
                                filterlist = filterlist.FindAll(t => (t.GetAttrDataByKeyWord("CA_IFREPLY") == null ? "" : t.GetAttrDataByKeyWord("CA_IFREPLY").ToString) == paramDictionary["reply"]);
                        }
                    }

                    if (filterlist != null && filterlist.Count > 0)
                    {
                        projDocList.AddRange(filterlist);
                    }
                }
                #endregion

                List<Doc> cataloguDoclist = new List<Doc>();
                foreach (var doc in projDocList)
                {
                    if (doc.TempDefn != null && doc.TempDefn.KeyWord == "CATALOGUING")//模板著录
                    {
                        //CataloguDoc caDoc = new CataloguDoc();
                        //caDoc.doc = doc;
                        cataloguDoclist.Add(doc);
                    }
                }

                //服务器
                string path = System.Web.HttpContext.Current.Server.MapPath("~");
                
                string name = "";
                CreateExcel(path, out name, cataloguDoclist, ref reJo);

                //输出Excel表到流，然后删除Excel表
                DownStream(path, name);

                //reJo.success = true;
                return reJo.Value;
            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(reJo.msg);
            }
            return reJo.Value;
        }


        //创建Excel
        public static void CreateExcel(string path, out string name, List<Doc> cataloguDoclist, ref ExReJObject reJo)
        {
            name = "";
            try
            {
                Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();//lauch excel application
                if (excel == null)
                {
                    reJo.msg = "Excel is not properly installed!";
                    reJo.success = false;
                    return ;

                    //MessageBox.Show("Excel is not properly installed!");
                }
                bool IsProject = false;
                if(cataloguDoclist.Count>0)
                {
                     Project rootProj = CommonFunction.getParentProjectByTempDefn(cataloguDoclist[0].Project, "HXNY_DOCUMENTSYSTEM");
                     if (rootProj == null) //运营管理类
                     {
                         name = "运营";
                     }
                     else
                     {
                         IsProject = true;
                         name = "项目";
                     }
                }
                else
                {
                    reJo.msg = "未找到有效的Excel文件";
                    reJo.success = false;
                    return;
                }

                Workbook workBook = excel.Application.Workbooks.Add(true);// new Workbook();
                Worksheet workSheet = workBook.Worksheets[1];

                for (int i = 0; i < cataloguDoclist.Count;i++ )
                {
                    var cataloguDoc = cataloguDoclist[i];

                   // Project rootProj = CommonFunction.getParentProjectByTempDefn(cataloguDoc.Project, "HXNY_DOCUMENTSYSTEM");
                    if (!IsProject) //运营管理类
                    {
                        Array arry = Enum.GetValues(typeof(EnumOperationAttr));
                        SetCells(ref  workSheet, cataloguDoc, arry, false, i);
                    }
                    else//项目管理
                    {
                        Array arry = Enum.GetValues(typeof(EnumProjectAttr));
                        SetCells(ref  workSheet, cataloguDoc, arry, true, i);
                    }
                }

                name = name + "文件属性统计表" + "_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx";
                workBook.SaveAs(path + name, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);

                excel.Workbooks.Close();
                excel.Quit();

            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                reJo.success = false;
                return;
            }
        }


        /// <summary>
        /// 填充workSheet 的单元格
        /// </summary>
        /// <param name="workSheet">workSheet</param>
        /// <param name="cataloguDoc">单个 doc</param>
        /// <param name="arry">运营或项目的 显示在Excel里的 著录属性的枚举</param>
        /// <param name="isProj">是否为运营</param>
        /// <param name="i">行</param>
        public static void SetCells(ref  Worksheet workSheet, Doc cataloguDoc, Array arry, bool isProj, int i)
        {
            for (int j = 0; j < arry.Length; j++)
            {
                var item = arry.GetValue(j);
                var desc = "";
                if (!isProj)
                {
                    desc = GetEnumDescription((EnumOperationAttr)item);
                }
                else
                {
                    desc = GetEnumDescription((EnumProjectAttr)item);
                }

                var attrname = item.ToString();
                //Type type = cataloguDoc.GetType();
                //PropertyInfo[] propertyInfos = type.GetProperties();
                //var cataloguDocprop = propertyInfos.FirstOrDefault(t => t.Name == attrname);

                //workSheet.Cells[ , ] 下标都从 1 开始
                if (i == 0)//i + 1：标题行。 
                {
                    workSheet.Cells[i + 1, j + 1] = desc;//标题
                }

                if (attrname == "index") //i + 2 ：非标题行 ， 的序号列。 i+2 是因为 有个标题行占了一行，正常行只能 + 2
                {
                    workSheet.Cells[i + 2, j + 1] = i + 1;
                }
                else if (attrname == "link") //链接
                {
                    string pathpro = GetProPath(cataloguDoc.Project);
                    workSheet.Cells[i + 2, j + 1] = pathpro;
                }
                else  //其他正常属性值列
                {
                    //if (cataloguDocprop != null) //CA_DESIGN cataloguDocprop没有 ，需要到doc 里面 get 一下看看
                    //{
                    //    workSheet.Cells[i + 2, j + 1] = cataloguDocprop.GetValue(cataloguDoc, null);
                    //}
                    //var data = cataloguDoc.GetAttrDataByKeyWord("CA_DESIGN");
                    workSheet.Cells[i + 2, j + 1] = GetAttrDataValue(attrname, cataloguDoc);//cataloguDocprop.GetValue(cataloguDoc, null);
                }
                
            }
        }

        /// <summary>
        /// 根据著录属性的名，如：CA_DESIGN， 获取 doc 的 该著录属性的值
        /// </summary>
        /// <param name="attrName">属性名</param>
        /// <param name="doc"></param>
        /// <returns>对应属性名的值</returns>
        public static string GetAttrDataValue(string attrName ,Doc doc)
        {
            string result = "";
            if (string.IsNullOrEmpty(attrName)) return result;
            if (doc != null)
            {
                AttrData data;
                //属性名称
                if ((data = doc.GetAttrDataByKeyWord(attrName)) != null)
                {
                    result = data.ToString;
                }
            }
            return result;
        }



        /// <summary>
        /// 节点在目录树上的 路径
        /// </summary>
        /// <param name="propath">Project 节点</param>
        /// <returns></returns>
        public static string GetProPath(Project project)
        {
            //路径位置
            var temppath = "";
            if (project.Description != "")
            { temppath = project.Code + "__" + project.Description; }
            else { temppath = project.Code; }
            while (project != null)
            {
                if (project.ParentProject != null)
                {
                    project = project.ParentProject;
                    if (project.Description != "")
                    {
                        temppath = project.Code + "__" + project.Description + "/" + temppath;
                    }
                    else { temppath = project.Code + "/" + temppath; }
                }
                else
                {
                    break;
                }
            }
            temppath = "HZX://HXCDMS/项目/" + temppath;
            temppath = temppath.Replace("/", "\\");
            return temppath;
        }


        /// <summary>
        /// 需要输出的 运营的 著录属性 枚举
        /// </summary>
        public enum EnumOperationAttr
        {
            [Description("序号")]
            index ,

            [Description("文件编码")]
            CA_FILECODE ,

            [Description("文件题名")]
            CA_FILETITLE,

            [Description("编制")]
            CA_DESIGN,

            [Description("生效日期")]
            CA_APPROVTIME,

            [Description("版本")]
            CA_EDITION,

            [Description("页数")]
            CA_PAGE,

            [Description("主送")]
            CA_MAINFEEDER,

            [Description("抄送")]
            CA_COPY,

            [Description("密级")]
            CA_SECRETGRADE,

            [Description("发送日期")]
            CA_SENDDATE,

            [Description("是否要求回复")]
            CA_IFREPLY,

            [Description("回复时限")]
            CA_REPLYDATE,

            [Description("回文编码")]
            CA_REPLYCODE,

            [Description("回文日期")]
            CA_REPLYTIME,

            [Description("备注")]
            CA_NOTE,

            [Description("链接")]
            link,

        }

        /// <summary>
        ///需要输出的 项目的 著录属性 枚举
        /// </summary>
        public enum EnumProjectAttr
        {
            [Description("序号")]
            index ,
            [Description("档号")]
            CA_REFERENCE  ,

            [Description("卷内序号")]
            CA_VOLUMENUMBER ,

            [Description("文件编码")]
            CA_FILECODE,

            [Description("责任者")]
            CA_RESPONSIBILITY,

            [Description("文件题名")]
            CA_FILETITLE,

            [Description("页数")]
            CA_PAGE,

            [Description("份数")]
            CA_NUMBER,

            [Description("介质")]
            CA_MEDIUM,

            [Description("语种")]
            CA_LANGUAGES,

            [Description("项目名称")]
            CA_PRONAME,

            [Description("项目代码")]
            CA_PROCODE,
            [Description("专业")]
            CA_MAJOR,
            [Description("机组号")]
            CA_CREW,
            [Description("厂房代码")]
            CA_FACTORY,
            [Description("厂房名称")]
            CA_FACTORYNAME,

            [Description("系统代码")]
            CA_SYSTEM,
            [Description("系统名称")]
            CA_SYSTEMNAME,
            [Description("关联文件编码")]
            CA_RELATIONFILECODE,
            [Description("关联文件题名")]
            CA_RELATIONFILENAME,
            [Description("案卷规格")]
            CA_FILESPEC,
            [Description("归档单位")]
            CA_FILEUNIT,
            [Description("密级")]
            CA_SECRETGRADE,
            [Description("保管期限")]
            CA_SECRETTERM,
            [Description("归档文件清单编码")]
            CA_FILELISTCODE,
            [Description("归档日期")]
            CA_FILELISTTIME,
            [Description("排架号")]
            CA_RACKNUMBER,
            [Description("备注")]
            CA_NOTE,

            [Description("链接")]
            link,

        }

        /// <summary>
        /// 获得枚举的Description
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);
            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="serverPath">件物理地址</param>
        protected static void DownStream(string serverPath, string name)
        {
            string fileName = name ;//客户端保存的文件名
            string filePath = serverPath + fileName;//文件在服务器的物理路径


            ////以字符流的形式下载文件,流方式下载文件不能超过400M
            //FileStream fs = new FileStream(filePath, FileMode.Open);
            //byte[] bytes = new byte[(int)fs.Length];
            //fs.Read(bytes, 0, bytes.Length);
            //fs.Close();
            //HttpContext.Current.Response.ContentType = "application/octet-stream";
            ////通知浏览器下载文件而不是打开
            //HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8));
            //File.Delete(serverPath);//删除压缩后的文件
            //// HttpContext.Current.Response.Write("{success:true}");
            //HttpContext.Current.Response.BinaryWrite(bytes);
            //HttpContext.Current.Response.Flush();
            //HttpContext.Current.Response.End();

            //WriteFile实现下载
            FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.Exists == true)
            {
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ClearContent();
                HttpContext.Current.Response.ClearHeaders();
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8));
                HttpContext.Current.Response.AddHeader("Content-Length", fileInfo.Length.ToString());
                HttpContext.Current.Response.AddHeader("Content-Transfer-Encoding", "binary");
                HttpContext.Current.Response.ContentType = "application/octet-stream";
                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("gb2312");
                HttpContext.Current.Response.WriteFile(fileInfo.FullName);
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.End();
                File.Delete(filePath);//删除服务器创建的临时Excel文件
            }
        }

    }
}