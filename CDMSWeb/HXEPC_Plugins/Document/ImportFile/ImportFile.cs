using AVEVA.CDMS.Server;
using AVEVA.CDMS.WebApi;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace AVEVA.CDMS.HXEPC_Plugins
{
    public class ImportFile
    {
        public static JObject GetImportFileDefault(string sid, string ProjectKeyword, string DraftOnProject)
        {
            return DraftGetDefault.GetDefaultInfo(sid, ProjectKeyword, DraftOnProject);
        }

        public static JObject StartCheckFileWorkFlow(string sid, string DocKeyword)
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

                Doc ddoc = dbsource.GetDocByKeyWord(DocKeyword);
                if (ddoc == null)
                {
                    reJo.msg = "错误的文档操作信息！指定的文档不存在！";
                    return reJo.Value;
                }
                Doc doc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;

                List<Doc> docList = new List<Doc>() { doc };
                WorkFlow flow = dbsource.NewWorkFlow(docList, "CHECKFILE");
                //if (flow == null || flow.CuWorkState == null || flow.CuWorkState.workStateBranchList == null || (flow.CuWorkState.workStateBranchList.Count <= 0))
                if (flow == null)
                {
                    reJo.msg = "自动启动流程失败!请手动启动";
                    return reJo.Value;
                }

                Group group = new Group();
                group.AddUser(curUser);
                WorkState ws = new WorkState();

                //放置检查状态人员
                WorkState state = flow.WorkStateList.Find(wsx => (wsx.Code == "CHECK") && (wsx.CheckGroup.AllUserList.Count == 0));
                if (state == null)
                {
                    DefWorkState defWorkState = flow.DefWorkFlow.DefWorkStateList.Find(dwsx => dwsx.O_Code == "CHECK");
                    state = flow.NewWorkState(defWorkState);
                    state.SaveSelectUser(group);

                    state.IsRuning = true;

                    state.PreWorkState = flow.CuWorkState;
                    state.O_iuser5 = new int?(flow.CuWorkState.O_stateno);
                    state.Modify();
                }

                ////启动流程
                WorkStateBranch branch = flow.CuWorkState.workStateBranchList[0];
                //  branch.NextStateAddGroup(secGroup);

                ExReJObject GotoNextReJo = WebWorkFlowEvent.GotoNextStateAndSelectUser(flow.CuWorkState.workStateBranchList[0]);

                DBSourceController.RefreshDBSource(sid);

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

        //读取著录属性
        public static JObject ReadAttrFromExcel(string sid, string DocKeyword)
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

                Doc ddoc = dbsource.GetDocByKeyWord(DocKeyword);
                if (ddoc == null)
                {
                    reJo.msg = "错误的文档操作信息！指定的文档不存在！";
                    return reJo.Value;
                }
                Doc doc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;

                string strFileName = doc.FullPathFile;


                object missing = System.Reflection.Missing.Value;
                Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();//lauch excel application
                if (excel == null)
                {

                    reJo.msg = "打开excel文件失败！";
                    return reJo.Value;
                }
                else
                {
                    #region 打开EXCEL文件
                    excel.Visible = false; excel.UserControl = true;
                    // 以只读的形式打开EXCEL文件
                    Workbook wb = excel.Application.Workbooks.Open(strFileName, missing, true, missing, missing, missing,
                     missing, missing, missing, true, missing, missing, missing, missing, missing);

                    //获取“正文”工作表
                    int wsLength = wb.Worksheets.Count;
                    Worksheet ws = null;
                    ws = (Worksheet)wb.Worksheets.get_Item(1);
                    //for (int i = 1; i <= wsLength - 1; i++)
                    //{
                    //    Worksheet wsItem = (Worksheet)wb.Worksheets.get_Item(i);
                    //    if (wsItem.Name == "正文")
                    //    {
                    //        ws = wsItem;
                    //    }
                    //}
                    if (ws == null)
                    {
                        //MessageBox.Show("获取正文工作表失败，请选择正确的零件表格！");
                        reJo.msg = "获取工作表失败，请选择正确的表格！";
                        return reJo.Value;
                    }

                    //取得总记录行数   (包括标题列)
                    int rowsint = ws.UsedRange.Cells.Rows.Count; //得到行数
                    if (rowsint <= 2)
                    {
                        reJo.msg = "表格行数需要大于2，请选择正确的表格！";
                        return reJo.Value;
                    }
                    //得到总列数
                    int colsint = ws.UsedRange.Cells.Columns.Count;
                    #endregion

                    #region 获取excel表格的列名 
                    Range rngHead = ws.Cells.get_Range("A1", "AB1");   //item


                    object[,] arryHead = (object[,])rngHead.Value2;   //get range's value

                    //if (colsint > 21) { colsint = 21; }

                    //获取标题行
                    CataHead headRow = getHeadRow(arryHead, colsint);


                    if (string.IsNullOrEmpty(headRow.FileTitle.ColIndex))
                    {
                        reJo.msg = "获取文件题名列名失败，请选择正确的表格！";
                        return reJo.Value;
                    }
                    #endregion


                    #region 读取表格

                    string MaxColIndex = IntToCols(colsint);

                    List<CataRow> cataRowList = new List<CataRow>();
                    for (int i = 2; i <= rowsint; i++)
                    {
                        // if (arryFileTitle[i, 1] == null || arryFileTitle[i, 1] == null || arryFileTitle[i, 1] == null) { continue; }

                        //读取一行
                        CataRow cRow = GetGataRow(ws, headRow, i, MaxColIndex);


                        if (cRow != null)
                        {
                            cataRowList.Add(cRow);
                        }

                    }
                    #endregion

                    #region 返回数据

                    wb.Close(missing, missing, missing);
                    excel.Workbooks.Close();
                    excel.Quit();

                    JArray dataJa = new JArray();


                    foreach (CataRow cRow in cataRowList)
                    {
                        JObject dataJo = new JObject();
                        
                        //遍历当前行的所有属性，添加到JObject
                        Type t = headRow.GetType();
                        PropertyInfo[] PropertyList = t.GetProperties();
                        foreach (PropertyInfo item in PropertyList)
                        {
                            string name = item.Name;
                            object obj = item.GetValue(headRow, null);

                            if (obj is CataCol)
                            {
                                var cataCol = (CataCol)obj;
                                if (cataCol != null) {
                                    dataJo.Add(new JProperty(cataCol.ColCode, GetRawValueByName(cRow, name)));
                                }
                            }
                        }

                        //if (headRow.Reference != null)
                        //{
                        //    dataJo.Add(new JProperty(headRow.Reference.ColCode, cRow.Reference));
                        //}
                        //if (headRow.FileTitle != null)
                        //{
                        //    dataJo.Add(new JProperty(headRow.FileTitle.ColCode, cRow.FileTitle));
                        //}
                        //if (headRow.Filecode != null)
                        //{
                        //    dataJo.Add(new JProperty(headRow.Filecode.ColCode, cRow.Filecode));
                        //}

                        //if (headRow.Volumenumber != null)
                        //{
                        //    dataJo.Add(new JProperty(headRow.Volumenumber.ColCode, cRow.Volumenumber));
                        //}
                        //if (headRow.Responsibility != null)
                        //{
                        //    dataJo.Add(new JProperty(headRow.Responsibility.ColCode, cRow.Responsibility));
                        //}
                        //if (headRow.Page != null)
                        //{
                        //    dataJo.Add(new JProperty(headRow.Page.ColCode, cRow.Page));
                        //}
                        //if (headRow.Number != null)
                        //{
                        //    dataJo.Add(new JProperty(headRow.Number.ColCode, cRow.Number));
                        //}
                        //if (headRow.Medium != null)
                        //{
                        //    dataJo.Add(new JProperty(headRow.Medium.ColCode, cRow.Medium));
                        //}


                        //if (headRow.Languages != null)
                        //{
                        //    dataJo.Add(new JProperty(headRow.Languages.ColCode, cRow.Languages));
                        //}
                        //if (headRow.Proname != null)
                        //{
                        //    dataJo.Add(new JProperty(headRow.Proname.ColCode, cRow.Proname));
                        //}
                        //if (headRow.Procode != null)
                        //{
                        //    dataJo.Add(new JProperty(headRow.Procode.ColCode, cRow.Procode));
                        //}
                        //if (headRow.Major != null)
                        //{
                        //    dataJo.Add(new JProperty(headRow.Major.ColCode, cRow.Major));
                        //}
                        //if (headRow.Crew != null)
                        //{
                        //    dataJo.Add(new JProperty(headRow.Crew.ColCode, cRow.Crew));
                        //}
                        //if (headRow.FactoryCode != null)
                        //{
                        //    dataJo.Add(new JProperty(headRow.FactoryCode.ColCode, cRow.FactoryCode));
                        //}
                        //if (headRow.Factoryname != null)
                        //{
                        //    dataJo.Add(new JProperty(headRow.Factoryname.ColCode, cRow.Factoryname));
                        //}
                        //if (headRow.Systemcode != null)
                        //{
                        //    dataJo.Add(new JProperty(headRow.Systemcode.ColCode, cRow.Systemcode));
                        //}
                        //if (headRow.Systemname != null)
                        //{
                        //    dataJo.Add(new JProperty(headRow.Systemname.ColCode, cRow.Systemname));
                        //}
                        //if (headRow.Relationfilecode != null)
                        //{
                        //    dataJo.Add(new JProperty(headRow.Relationfilecode.ColCode, cRow.Relationfilecode));
                        //}
                        //if (headRow.Relationfilename != null)
                        //{
                        //    dataJo.Add(new JProperty(headRow.Relationfilename.ColCode, cRow.Relationfilename));
                        //}
                        //if (headRow.Filespec != null)
                        //{
                        //    dataJo.Add(new JProperty(headRow.Filespec.ColCode, cRow.Filespec));
                        //}
                        //if (headRow.Fileunit != null)
                        //{
                        //    dataJo.Add(new JProperty(headRow.Fileunit.ColCode, cRow.Fileunit));
                        //}
                        //if (headRow.Secretgrade != null)
                        //{
                        //    dataJo.Add(new JProperty(headRow.Secretgrade.ColCode, cRow.Secretgrade));
                        //}
                        //if (headRow.Keepingtime != null)
                        //{
                        //    dataJo.Add(new JProperty(headRow.Keepingtime.ColCode, cRow.Keepingtime));
                        //}
                        //if (headRow.Filelistcode != null)
                        //{
                        //    dataJo.Add(new JProperty(headRow.Filelistcode.ColCode, cRow.Filelistcode));
                        //}
                        //if (headRow.Filelisttime != null)
                        //{
                        //    dataJo.Add(new JProperty(headRow.Filelisttime.ColCode, cRow.Filelisttime));
                        //}

                        //if (headRow.Racknumber != null)
                        //{
                        //    dataJo.Add(new JProperty(headRow.Racknumber.ColCode, cRow.Racknumber));
                        //}
                        //if (headRow.Note != null)
                        //{
                        //    dataJo.Add(new JProperty(headRow.Note.ColCode, cRow.Note));
                        //}

                        //if (headRow.Design != null)
                        //{
                        //    dataJo.Add(new JProperty(headRow.Design.ColCode, cRow.Design));
                        //}
                        //if (headRow.Approvtime != null)
                        //{
                        //    dataJo.Add(new JProperty(headRow.Approvtime.ColCode, cRow.Approvtime));
                        //}
                        //if (headRow.Edition != null)
                        //{
                        //    dataJo.Add(new JProperty(headRow.Edition.ColCode, cRow.Edition));
                        //}
                        //if (headRow.Mainfeeder != null)
                        //{
                        //    dataJo.Add(new JProperty(headRow.Mainfeeder.ColCode, cRow.Mainfeeder));
                        //}
                        //if (headRow.Copy != null)
                        //{
                        //    dataJo.Add(new JProperty(headRow.Copy.ColCode, cRow.Copy));
                        //}
                        //if (headRow.SendDate != null)
                        //{
                        //    dataJo.Add(new JProperty(headRow.SendDate.ColCode, cRow.SendDate));
                        //}
                        //if (headRow.IFReply != null)
                        //{
                        //    dataJo.Add(new JProperty(headRow.IFReply.ColCode, cRow.IFReply));
                        //}
                        //if (headRow.ReplyDate != null)
                        //{
                        //    dataJo.Add(new JProperty(headRow.ReplyDate.ColCode, cRow.ReplyDate));
                        //}
                        //if (headRow.ReplyCode != null)
                        //{
                        //    dataJo.Add(new JProperty(headRow.ReplyCode.ColCode, cRow.ReplyCode));
                        //}
                        //if (headRow.ReplyTime != null)
                        //{
                        //    dataJo.Add(new JProperty(headRow.ReplyTime.ColCode, cRow.ReplyTime));
                        //}

                        dataJa.Add(dataJo);

                   
                    }
                    #endregion
                    reJo.data = dataJa;

                    reJo.success = true;
                    return reJo.Value;
                }

            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(reJo.msg);
            }
            return reJo.Value;
        }

        /// <summary>
        /// 下载未匹配表格
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="DocKeyword"></param>
        /// <param name="FileListJson"></param>
        /// <param name="Server_MapPath">网站地址，系统自动生成</param>
        /// <returns></returns>
        public static JObject GetFailAttrFromExcel(string sid, string DocKeyword, string FileListJson, string Server_MapPath)
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

                Doc ddoc = dbsource.GetDocByKeyWord(DocKeyword);
                if (ddoc == null)
                {
                    reJo.msg = "错误的文档操作信息！指定的文档不存在！";
                    return reJo.Value;
                }
                Doc doc = ddoc.ShortCutDoc == null ? ddoc : ddoc.ShortCutDoc;

                string FileName = doc.FullPathFile;

                #region 获取文件列表
                List<LetterAttaFile> attaFiles = new List<LetterAttaFile>();
                if (!string.IsNullOrEmpty(FileListJson))
                {
                    int index = 0;
                    JArray jaFiles = (JArray)JsonConvert.DeserializeObject(FileListJson);

                    foreach (JObject joAttr in jaFiles)
                    {
                        string strFileName = joAttr["fn"].ToString();//文件名
                        string strFileCode = joAttr["fc"].ToString();//文件编码
                        string strFileDesc = joAttr["fd"].ToString();//文件题名

                        index++;
                        string strIndex = index.ToString();
                        LetterAttaFile afItem = new LetterAttaFile()
                        {
                            No = strIndex,
                            Name = strFileName,
                            Code = strFileCode,
                            Desc = strFileDesc
                        };

                        attaFiles.Add(afItem);
                    }
                }
                #endregion


                object missing = System.Reflection.Missing.Value;
                Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();//lauch excel application
                if (excel == null)
                {

                    reJo.msg = "打开excel文件失败！";
                    return reJo.Value;
                }

                #region 打开EXCEL文件
                excel.Visible = false; excel.UserControl = true;
                // 以只读的形式打开EXCEL文件
                // Workbook wb = excel.Application.Workbooks.Open(FileName, missing, true, missing, missing, missing,
                //  missing, missing, missing, true, missing, missing, missing, missing, missing);
                // 以可写的形式打开EXCEL文件
                Workbook wb = excel.Application.Workbooks.Open(FileName, missing, false, missing, missing, missing,
                    true, missing, missing, missing, missing, missing, missing, missing, missing);
                //获取“正文”工作表
                int wsLength = wb.Worksheets.Count;
                Worksheet ws = null;
                ws = (Worksheet)wb.Worksheets.get_Item(1);
                //for (int i = 1; i <= wsLength - 1; i++)
                //{
                //    Worksheet wsItem = (Worksheet)wb.Worksheets.get_Item(i);
                //    if (wsItem.Name == "正文")
                //    {
                //        ws = wsItem;
                //    }
                //}
                if (ws == null)
                {
                    reJo.msg = "获取工作表失败，请选择正确的表格！";
                    return reJo.Value;
                }

                //取得总记录行数   (包括标题列)
                int rowsint = ws.UsedRange.Cells.Rows.Count; //得到行数
                if (rowsint <= 2)
                {
                    reJo.msg = "表格行数需要大于2，请选择正确的表格！";
                    return reJo.Value;
                }
                //得到总列数
                int colsint = ws.UsedRange.Cells.Columns.Count;
                #endregion

                #region 获取excel表格的列名 
                Range rngHead = ws.Cells.get_Range("A1", "AB1");   //item


                object[,] arryHead = (object[,])rngHead.Value2;   //get range's value

                //if (colsint > 21) { colsint = 21; }

                //获取标题行
                CataHead headRow = getHeadRow(arryHead, colsint);


                if (string.IsNullOrEmpty(headRow.FileTitle.ColIndex))
                {
                    reJo.msg = "获取文件题名列名失败，请选择正确的表格！";
                    return reJo.Value;
                }
                #endregion


                #region 读取表格

                string MaxColIndex = IntToCols(colsint);

                List<int> successFileIndexList = new List<int>();

                List<CataRow> cataRowList = new List<CataRow>();
                //for (int i = 2; i <= rowsint - 1; i++)
                for (int i =  rowsint ; i >=2; i--)
                {
                    // if (arryFileTitle[i, 1] == null || arryFileTitle[i, 1] == null || arryFileTitle[i, 1] == null) { continue; }

                    //读取一行
                    CataRow cRow = GetGataRow(ws, headRow, i, MaxColIndex);


                    if (cRow != null)
                    {
                        if (cRow.Filecode != null) {
                            //遍历文件列表，判断是否有匹配的文件
                            foreach (LetterAttaFile fItem in attaFiles)
                            {
                                if (fItem.Code == cRow.Filecode)
                                {
                                    successFileIndexList.Add(i);
                                    break;
                                }
                               
                            }
                        }
                    }

                }
                #endregion



                foreach (int rowIndex in successFileIndexList)
                {
  
                    Range range = (Range)ws.Rows[rowIndex, missing];
                    range.Delete(Microsoft.Office.Interop.Excel.XlDeleteShiftDirection.xlShiftUp);
                }

                excel.Application.DisplayAlerts = false;
                wb.Save();
                wb.Close(missing, missing, missing);
                excel.Workbooks.Close();
                excel.Quit();

                return DocController.FileDownload(sid, DocKeyword,"", Server_MapPath);


                //reJo.success = true;
                //return reJo.Value;


            }
            catch (Exception e)
            {
                reJo.msg = e.Message;
                CommonController.WebWriteLog(reJo.msg);
            }
            return reJo.Value;

        }

        /// <summary>
        /// 获取标题行，并出示化标题字段
        /// </summary>
        /// <param name="arryHead"></param>
        /// <param name="colsint"></param>
        /// <returns></returns>
        public static CataHead getHeadRow(object[,] arryHead, int colsint)
        {

            //定义标题行
            CataHead headRow = new CataHead();



            for (int i = 1; i <= colsint - 1; i++)
            {
                try
                {
                    if (arryHead[1, i] == null) continue;

                    string strhead = arryHead[1, i].ToString().Replace("\n", "");

                    //反射遍历headRow的各个列属性，获取列名
                    Type t = headRow.GetType();
                    PropertyInfo[] PropertyList = t.GetProperties();// t.GetProperties();
                    foreach (PropertyInfo item in PropertyList)
                    {
                        string name = item.Name;
                        object value = item.GetValue(headRow, null);

                        if (value is CataCol)
                        {
                            var ccValue = (CataCol)value;
                            if (strhead == ccValue.ColName)
                            {
                                ccValue.ColIndex = IntToCols(i);
                                ccValue.ColIndexInt = i;
                            }
                        }
                    }


                }
                catch { }

            }

            return headRow;
        }

        /// <summary>
        /// 在Excel表里面获取一行数据
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="headRow"></param>
        /// <param name="RowIndex"></param>
        /// <param name="MaxColIndex"></param>
        /// <returns></returns>
        public static CataRow GetGataRow(Worksheet ws, CataHead headRow, int RowIndex, string MaxColIndex)
        {
            CataRow cRow = null;
            try
            {
                //读取一行
                Range rngRow = ws.Cells.get_Range("A" + RowIndex, MaxColIndex + RowIndex);   //法兰
                object[,] arryRow = (object[,])rngRow.Value2;

               

                //反射遍历CataRow的各个列属性，获取列名
                //再获取Excel的当前行的各个Cell的值，赋值给CataRow
                cRow = new CataRow();
                Type t = cRow.GetType();
                PropertyInfo[] PropertyList = t.GetProperties();
                foreach (PropertyInfo item in PropertyList)
                {
                    string name = item.Name;
                    object obj = item.GetValue(cRow, null);

                    if (obj is string)
                    {
                        var cRowValue = (string)obj;
                        //根据列名获取列
                        var cCal = GetCataColByName(headRow, name);
                        if (cCal != null) {


                            //获取Excel的当前行的各个Cell的值，赋值给CataRow
                            string m_value = GetRangeCellValue(arryRow, cCal);
                            item.SetValue(cRow, m_value);

                            //判断字段是否是必填，如果是必填又没有填写，就忽略当前行
                            if (cCal.AllowNull != null && cCal.AllowNull == "false" && 
                                string.IsNullOrEmpty(m_value))
                            {
                                return null;
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
            return cRow;
        }

       /// <summary>
       /// 根据列名获取列
       /// </summary>
       /// <param name="headRow"></param>
       /// <param name="colName"></param>
       /// <returns></returns>
        public static CataCol GetCataColByName(CataHead headRow, string colName)
        {
            //反射遍历headRow的各个列属性，获取列名
            Type t = headRow.GetType();
            PropertyInfo[] PropertyList = t.GetProperties();
            foreach (PropertyInfo item in PropertyList)
            {
                string name = item.Name;
                if (name == colName)
                {
                    object value = item.GetValue(headRow, null);


                    if (value is CataCol)
                    {
                        var ccValue = (CataCol)value;


                        return ccValue;
                    }

                }
            }
            return null;
        }

        /// <summary>
        /// 在某一行里面，获取某个列名对应的单元格的值
        /// </summary>
        /// <param name="cataRow"></param>
        /// <param name="colName"></param>
        /// <returns></returns>
        public static string GetRawValueByName(CataRow cataRow, string colName)
        {
            //反射遍历headRow的各个列属性，获取列名
            Type t = cataRow.GetType();
            PropertyInfo[] PropertyList = t.GetProperties();
            foreach (PropertyInfo item in PropertyList)
            {
                string name = item.Name;
                if (name == colName)
                {
                    object value = item.GetValue(cataRow, null);


                    if (value is string)
                    {
                        var ccValue = (string)value;


                        return ccValue;
                    }

                }
            }
            return null;
        }

       /// <summary>
       /// 获取某个单元格的值
       /// </summary>
       /// <param name="arryRow"></param>
       /// <param name="col"></param>
       /// <returns></returns>
        public static string GetRangeCellValue(object[,] arryRow, CataCol col)
        {
            string result = (col == null || col.ColIndexInt == 0 ||
                arryRow[1, col.ColIndexInt] == null) ?
                "" : arryRow[1, col.ColIndexInt].ToString();
            
            if (col.DataType != null && col.DataType == "datetime")
            {
                result = GetOADate(result);
            }

            return result;
        }

        /// <summary>
        /// 把Excel里面的数字格式的日期转换成格式化的字符串，例如："2017-10-1"
        /// </summary>
        /// <param name="arryRow"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static string GetOADate(string date)
        {
            string result = date;
            try
            {
                string strDate = DateTime.FromOADate(Convert.ToInt32(date)).ToString("d");
                result = DateTime.Parse(strDate).ToString("yyyy-MM-dd");
            }
            catch { }
            return result;
        }

        /// <summary>
        /// 数字转 EXCEL 行号
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string IntToCols(int index)
        {
            string character = "A";
            string strRef = "";
            if (index > 78)
            {
                index = index - 78;
                strRef = "C";
            }
            else if (index > 52)
            {
                index = index - 52;
                strRef = "B";
            }
            else if (index > 26)
            {
                index = index - 26;
                strRef = "A";
            }
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            int intAsciiCode = (int)asciiEncoding.GetBytes(character)[0];

            intAsciiCode = intAsciiCode + index - 1;

            //System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            byte[] byteArray = new byte[] { (byte)intAsciiCode };
            string strCharacter = strRef+ asciiEncoding.GetString(byteArray);

            return strCharacter;

        }
    }

    //著录表列头
    public class CataHead {
        private CataCol _FileName;

        public CataCol FileName
        {
            get
            {
                if (_FileName == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "文件名称";
                    v.ColCaCode = "CA_FILENAME";
                    v.ColCode = "filename";
                    _FileName = v;
                }
                return _FileName;
            }
            set { _FileName = value; }
        }

        private CataCol _Reference;
        public CataCol Reference
        {
            get
            {
                if (_Reference == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "档号";
                    v.ColCaCode = "CA_REFERENCE";
                    v.ColCode = "reference";
                    _Reference = v;
                }
                return _Reference;
            }
            set { _Reference = value; }
        }

        private CataCol _FileTitle;
        public CataCol FileTitle
        {
            get
            {
                if (_FileTitle == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "文件题名";
                    v.ColCaCode = "CA_FILETITLE";
                    v.ColCode = "desc";
                    v.AllowNull = "false";
                    _FileTitle = v;
                }
                return _FileTitle;
            }
            set { _FileTitle = value; }
        }


        //卷内序号
        private CataCol _Volumenumber;
        public CataCol Volumenumber
        {
            get
            {
                if (_Volumenumber == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "卷内序号";
                    v.ColCaCode = "CA_VOLUMENUMBER";
                    v.ColCode = "volumenumber";
                    _Volumenumber = v;
                }
                return _Volumenumber;
            }
            set { _Volumenumber = value; }
        }
        //文件编码
        private CataCol _Filecode;
        public CataCol Filecode
        {
            get
            {
                if (_Filecode == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "文件编码";
                    v.ColCaCode = "CA_FILECODE";
                    v.ColCode = "code";
                    v.AllowNull = "false";
                    _Filecode = v;
                }
                return _Filecode;
            }
            set { _Filecode = value; }
        }
        //责任者
        private CataCol _Responsibility;
        public CataCol Responsibility
        {
            get
            {
                if (_Responsibility == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "责任者";
                    v.ColCaCode = "CA_RESPONSIBILITY";
                    v.ColCode = "responsibility";
                    _Responsibility = v;
                }
                return _Responsibility;
            }
            set { _Responsibility = value; }
        }

        ////文件题名
        //private CataCol _FileTitle;
        //public CataCol FileTitle
        //{
        //    get
        //    {
        //        if (_FileTitle == null)
        //        {
        //            CataCol v = new CataCol();
        //            v.ColName = "文件题名";
        //            v.ColCaCode = "CA_FILETITLE";
        //            v.ColCode = "desc";
        //            _FileTitle = v;
        //        }
        //        return _FileTitle;
        //    }
        //    set { _FileTitle = value; }
        //}

        //页数
        private CataCol _Page;
        public CataCol Page
        {
            get
            {
                if (_Page == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "页数";
                    v.ColCaCode = "CA_PAGE";
                    v.ColCode = "page";
                    _Page = v;
                }
                return _Page;
            }
            set { _Page = value; }
        }

        //份数
        private CataCol _Number;
        public CataCol Number
        {
            get
            {
                if (_Number == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "份数";
                    v.ColCaCode = "CA_NUMBER";
                    v.ColCode = "share";
                    _Number = v;
                }
                return _Number;
            }
            set { _Number = value; }
        }

        //介质
        private CataCol _Medium;
        public CataCol Medium
        {
            get
            {
                if (_Medium == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "介质";
                    v.ColCaCode = "CA_MEDIUM";
                    v.ColCode = "medium";
                    _Medium = v;
                }
                return _Medium;
            }
            set { _Medium = value; }
        }


        //语种
        private CataCol _Languages;
        public CataCol Languages
        {
            get
            {
                if (_Languages == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "语种";
                    v.ColCaCode = "CA_LANGUAGES";
                    v.ColCode = "languages";
                    _Languages = v;
                }
                return _Languages;
            }
            set { _Languages = value; }
        }


        //项目名称
        private CataCol _Proname;
        public CataCol Proname
        {
            get
            {
                if (_Proname == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "项目名称";
                    v.ColCaCode = "CA_PRONAME";
                    v.ColCode = "proname";
                    _Proname = v;
                }
                return _Proname;
            }
            set { _Proname = value; }
        }

        //项目代码
        private CataCol _Procode;
        public CataCol Procode
        {
            get
            {
                if (_Procode == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "项目代码";
                    v.ColCaCode = "CA_PROCODE";
                    v.ColCode = "procode";
                    _Procode = v;
                }
                return _Procode;
            }
            set { _Procode = value; }
        }

        //专业
        private CataCol _Major;
        public CataCol Major
        {
            get
            {
                if (_Major == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "专业";
                    v.ColCaCode = "CA_MAJOR";
                    v.ColCode = "major";
                    _Major = v;
                }
                return _Major;
            }
            set { _Major = value; }
        }

        //机组号
        private CataCol _Crew;
        public CataCol Crew
        {
            get
            {
                if (_Crew == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "机组号";
                    v.ColCaCode = "CA_CREW";
                    v.ColCode = "crew";
                    _Crew = v;
                }
                return _Crew;
            }
            set { _Crew = value; }
        }

        //厂房代码
        private CataCol _FactoryCode;
        public CataCol FactoryCode
        {
            get
            {
                if (_FactoryCode == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "厂房代码";
                    v.ColCaCode = "CA_FACTORY";
                    v.ColCode = "factorycode";
                    _FactoryCode = v;
                }
                return _FactoryCode;
            }
            set { _FactoryCode = value; }
        }

        //厂房名称
        private CataCol _Factoryname;
        public CataCol Factoryname
        {
            get
            {
                if (_Factoryname == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "厂房名称";
                    v.ColCaCode = "CA_FACTORYNAME";
                    v.ColCode = "factoryname";
                    _Factoryname = v;
                }
                return _Factoryname;
            }
            set { _Factoryname = value; }
        }

        //系统代码
        private CataCol _Systemcode;
        public CataCol Systemcode
        {
            get
            {
                if (_Systemcode == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "系统代码";
                    v.ColCaCode = "CA_SYSTEM";
                    v.ColCode = "systemcode";
                    _Systemcode = v;
                }
                return _Systemcode;
            }
            set { _Systemcode = value; }
        }

        //系统名称
        private CataCol _Systemname;
        public CataCol Systemname
        {
            get
            {
                if (_Systemname == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "系统名称";
                    v.ColCaCode = "CA_SYSTEMNAME";
                    v.ColCode = "systemname";
                    _Systemname = v;
                }
                return _Systemname;
            }
            set { _Systemname = value; }
        }

        //关联文件编码
        private CataCol _Relationfilecode;
        public CataCol Relationfilecode
        {
            get
            {
                if (_Relationfilecode == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "关联文件编码";
                    v.ColCaCode = "CA_RELATIONFILECODE";
                    v.ColCode = "relationfilecode";
                    _Relationfilecode = v;
                }
                return _Relationfilecode;
            }
            set { _Relationfilecode = value; }
        }

        //关联文件题名
        private CataCol _Relationfilename;
        public CataCol Relationfilename
        {
            get
            {
                if (_Relationfilename == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "关联文件题名";
                    v.ColCaCode = "CA_RELATIONFILENAME";
                    v.ColCode = "relationfilename";
                    _Relationfilename = v;
                }
                return _Relationfilename;
            }
            set { _Relationfilename = value; }
        }

        //案卷规格
        private CataCol _Filespec;
        public CataCol Filespec
        {
            get
            {
                if (_Filespec == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "案卷规格";
                    v.ColCaCode = "CA_FILESPEC";
                    v.ColCode = "filespec";
                    _Filespec = v;
                }
                return _Filespec;
            }
            set { _Filespec = value; }
        }

        //归档单位
        private CataCol _Fileunit;
        public CataCol Fileunit
        {
            get
            {
                if (_Fileunit == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "归档单位";
                    v.ColCaCode = "CA_FILEUNIT";
                    v.ColCode = "fileunit";
                    _Fileunit = v;
                }
                return _Fileunit;
            }
            set { _Fileunit = value; }
        }

        //密级
        private CataCol _Secretgrade;
        public CataCol Secretgrade
        {
            get
            {
                if (_Secretgrade == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "密级";
                    v.ColCaCode = "CA_SECRETGRADE";
                    v.ColCode = "secretgrade";
                    _Secretgrade = v;
                }
                return _Secretgrade;
            }
            set { _Secretgrade = value; }
        }

        //保管期限
        private CataCol _Keepingtime;
        public CataCol Keepingtime
        {
            get
            {
                if (_Keepingtime == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "保管期限";
                    v.ColCaCode = "CA_SECRETTERM";
                    v.ColCode = "keepingtime";
                    _Keepingtime = v;
                }
                return _Keepingtime;
            }
            set { _Keepingtime = value; }
        }

        //归档文件清单编码
        private CataCol _Filelistcode;
        public CataCol Filelistcode
        {
            get
            {
                if (_Filelistcode == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "归档文件清单编码";
                    v.ColCaCode = "CA_FILELISTCODE";
                    v.ColCode = "filelistcode";
                    _Filelistcode = v;
                }
                return _Filelistcode;
            }
            set { _Filelistcode = value; }
        }

        //归档日期
        private CataCol _Filelisttime;
        public CataCol Filelisttime
        {
            get
            {
                if (_Filelisttime == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "归档日期";
                    v.ColCaCode = "CA_FILELISTTIME";
                    v.ColCode = "filelisttime";
                    _Filelisttime = v;
                }
                return _Filelisttime;
            }
            set { _Filelisttime = value; }
        }

        //排架号
        private CataCol _Racknumber;
        public CataCol Racknumber
        {
            get
            {
                if (_Racknumber == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "排架号";
                    v.ColCaCode = "CA_RACKNUMBER";
                    v.ColCode = "racknumber";
                    _Racknumber = v;
                }
                return _Racknumber;
            }
            set { _Racknumber = value; }
        }

        //备注
        private CataCol _Note;
        public CataCol Note
        {
            get
            {
                if (_Note == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "备注";
                    v.ColCaCode = "CA_NOTE";
                    v.ColCode = "note";
                    _Note = v;
                }
                return _Note;
            }
            set { _Note = value; }
        }


        //编制人
        private CataCol _Design;
        public CataCol Design
        {
            get
            {
                if (_Design == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "编制";
                    v.ColCaCode = "CA_DESIGN";
                    v.ColCode = "design";
                    _Design = v;
                }
                return _Design;
            }
            set { _Design = value; }
        }

        //生效日期
        private CataCol _Approvtime;
        public CataCol Approvtime
        {
            get
            {
                if (_Approvtime == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "生效日期";
                    v.ColCaCode = "CA_APPROVTIME";
                    v.ColCode = "approvtime";
                    v.DataType = "datetime";
                    _Approvtime = v;
                }
                return _Approvtime;
            }
            set { _Approvtime = value; }
        }

        //版本
        private CataCol _Edition;
        public CataCol Edition
        {
            get
            {
                if (_Edition == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "版本";
                    v.ColCaCode = "CA_EDITION";
                    v.ColCode = "edition";
                    _Edition = v;
                }
                return _Edition;
            }
            set { _Edition = value; }
        }

        //主送
        private CataCol _Mainfeeder;
        public CataCol Mainfeeder
        {
            get
            {
                if (_Mainfeeder == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "主送";
                    v.ColCaCode = "CA_MAINFEEDER";
                    v.ColCode = "mainfeeder";
                    _Mainfeeder = v;
                }
                return _Mainfeeder;
            }
            set { _Mainfeeder = value; }
        }

        //抄送
        private CataCol _Copy;
        public CataCol Copy
        {
            get
            {
                if (_Copy == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "抄送";
                    v.ColCaCode = "CA_COPY";
                    v.ColCode = "copy";
                    _Copy = v;
                }
                return _Copy;
            }
            set { _Copy = value; }
        }


        //发送日期
        private CataCol _SendDate;
        public CataCol SendDate
        {
            get
            {
                if (_SendDate == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "发送日期";
                    v.ColCaCode = "CA_SENDDATE";
                    v.ColCode = "senddate";
                    v.DataType = "datetime";
                    _SendDate = v;
                }
                return _SendDate;
            }
            set { _SendDate = value; }
        }

        //是否要求回复
        private CataCol _IFReply;
        public CataCol IFReply
        {
            get
            {
                if (_IFReply == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "是否要求回复";
                    v.ColCaCode = "CA_IFREPLY";
                    v.ColCode = "ifreply";
                    _IFReply = v;
                }
                return _IFReply;
            }
            set { _IFReply = value; }
        }


        //回复时限
        private CataCol _ReplyDate;
        public CataCol ReplyDate
        {
            get
            {
                if (_ReplyDate == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "回复时限";
                    v.ColCaCode = "CA_REPLYDATE";
                    v.ColCode = "replydate";
                    v.DataType = "datetime";
                    _ReplyDate = v;
                }
                return _ReplyDate;
            }
            set { _ReplyDate = value; }
        }

        //回文编码
        private CataCol _ReplyCode;
        public CataCol ReplyCode
        {
            get
            {
                if (_ReplyCode == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "回文编码";
                    v.ColCaCode = "CA_REPLYCODE";
                    v.ColCode = "replycode";
                    _ReplyCode = v;
                }
                return _ReplyCode;
            }
            set { _ReplyCode = value; }
        }

        //回文日期
        private CataCol _ReplyTime;
        public CataCol ReplyTime
        {
            get
            {
                if (_ReplyTime == null)
                {
                    CataCol v = new CataCol();
                    v.ColName = "回文日期";
                    v.ColCaCode = "CA_REPLYTIME";
                    v.ColCode = "replytime";
                    v.DataType = "datetime";
                    _ReplyTime = v;
                }
                return _ReplyTime;
            }
            set { _ReplyTime = value; }
        }
    }

    //著录表列
    public class CataCol {
        public int ColIndexInt { get; set; }
        public string ColIndex { get; set; }
        public string RawIndex { get; set; }

        public string ColName { get; set; }

        public string ColCode { get; set; }
        public string ColCaCode { get; set; }
        //是否允许空值
        public string AllowNull { get; set; }
        //数据类型，当列的数据类型是"datetime"时，
        //需要把excel里面的数字型日期，转换成"yyyy-MM-dd"的日期类型
        public string DataType { get; set; }
        public string Value { get; set; }
    }

    //著录表行
    public class CataRow {
        private string _FileName;
        public string FileName
        {
            get { if (_FileName == null) _FileName = ""; return _FileName; }
            set { _FileName = value; }
        }

        private string _Reference;
        public string Reference
        {
            get { if (_Reference == null) _Reference = ""; return _Reference; }
            set { _Reference = value; }
        }
        private string _Volumenumber;
        public string Volumenumber
        {
            get { if (_Volumenumber == null) _Volumenumber = ""; return _Volumenumber; }
            set { _Volumenumber = value; }
        }

        private string _Filecode;
        public string Filecode
        {
            get { if (_Filecode == null) _Filecode = ""; return _Filecode; }
            set { _Filecode = value; }
        }

        private string _Responsibility;
        public string Responsibility
        {
            get { if (_Responsibility == null) _Responsibility = ""; return _Responsibility; }
            set { _Responsibility = value; }
        }

        private string _FileTitle;
        public string FileTitle
        {
            get { if (_FileTitle == null) _FileTitle = ""; return _FileTitle; }
            set { _FileTitle = value; }
        }

        private string _Page;
        public string Page
        {
            get { if (_Page == null) _Page = ""; return _Page; }
            set { _Page = value; }
        }

        private string _Number;
        public string Number
        {
            get { if (_Number == null) _Number = ""; return _Number; }
            set { _Number = value; }
        }

        private string _Medium;
        public string Medium
        {
            get { if (_Medium == null) _Medium = ""; return _Medium; }
            set { _Medium = value; }
        }

        private string _Languages;
        public string Languages
        {
            get { if (_Languages == null) _Languages = ""; return _Languages; }
            set { _Languages = value; }
        }

        private string _Proname;
        public string Proname
        {
            get { if (_Proname == null) _Proname = ""; return _Proname; }
            set { _Proname = value; }
        }

        private string _Procode;
        public string Procode
        {
            get { if (_Procode == null) _Procode = ""; return _Procode; }
            set { _Procode = value; }
        }

        private string _Major;
        public string Major
        {
            get { if (_Major == null) _Major = ""; return _Major; }
            set { _Major = value; }
        }

        private string _Crew;
        public string Crew
        {
            get { if (_Crew == null) _Crew = ""; return _Crew; }
            set { _Crew = value; }
        }

        private string _FactoryCode;
        public string FactoryCode
        {
            get { if (_FactoryCode == null) _FactoryCode = ""; return _FactoryCode; }
            set { _FactoryCode = value; }
        }

        private string _Factoryname;
        public string Factoryname
        {
            get { if (_Factoryname == null) _Factoryname = ""; return _Factoryname; }
            set { _Factoryname = value; }
        }

        private string _Systemcode;
        public string Systemcode
        {
            get { if (_Systemcode == null) _Systemcode = ""; return _Systemcode; }
            set { _Systemcode = value; }
        }

        private string _Systemname;
        public string Systemname
        {
            get { if (_Systemname == null) _Systemname = ""; return _Systemname; }
            set { _Systemname = value; }
        }

        private string _Relationfilecode;
        public string Relationfilecode
        {
            get { if (_Relationfilecode == null) _Relationfilecode = ""; return _Relationfilecode; }
            set { _Relationfilecode = value; }
        }

        private string _Relationfilename;
        public string Relationfilename
        {
            get { if (_Relationfilename == null) _Relationfilename = ""; return _Relationfilename; }
            set { _Relationfilename = value; }
        }

        private string _Filespec;
        public string Filespec
        {
            get { if (_Filespec == null) _Filespec = ""; return _Filespec; }
            set { _Filespec = value; }
        }

        private string _Fileunit;
        public string Fileunit
        {
            get { if (_Fileunit == null) _Fileunit = ""; return _Fileunit; }
            set { _Fileunit = value; }
        }

        private string _Secretgrade;
        public string Secretgrade
        {
            get { if (_Secretgrade == null) _Secretgrade = ""; return _Secretgrade; }
            set { _Secretgrade = value; }
        }

        private string _Keepingtime;
        public string Keepingtime
        {
            get { if (_Keepingtime == null) _Keepingtime = ""; return _Keepingtime; }
            set { _Keepingtime = value; }
        }

        private string _Filelistcode;
        public string Filelistcode
        {
            get { if (_Filelistcode == null) _Filelistcode = ""; return _Filelistcode; }
            set { _Filelistcode = value; }
        }

        private string _Filelisttime;
        public string Filelisttime
        {
            get { if (_Filelisttime == null) _Filelisttime = ""; return _Filelisttime; }
            set { _Filelisttime = value; }
        }

        
        private string _Racknumber;
        public string Racknumber
        {
            get { if (_Racknumber == null) _Racknumber = ""; return _Racknumber; }
            set { _Racknumber = value; }
        }

        private string _Note;
        public string Note
        {
            get { if (_Note == null) _Note = ""; return _Note; }
            set { _Note = value; }
        }

        private string _Design;
        public string Design
        {
            get { if (_Design == null) _Design = ""; return _Design; }
            set { _Design = value; }
        }

        private string _Approvtime;
        public string Approvtime
        {
            get { if (_Approvtime == null) _Approvtime = ""; return _Approvtime; }
            set { _Approvtime = value; }
        }

        private string _Edition;
        public string Edition
        {
            get { if (_Edition == null) _Edition = ""; return _Edition; }
            set { _Edition = value; }
        }

        private string _Mainfeeder;
        public string Mainfeeder
        {
            get { if (_Mainfeeder == null) _Mainfeeder = ""; return _Mainfeeder; }
            set { _Mainfeeder = value; }
        }

        private string _Copy;
        public string Copy
        {
            get { if (_Copy == null) _Copy = ""; return _Copy; }
            set { _Copy = value; }
        }

        private string _SendDate;
        public string SendDate
        {
            get { if (_SendDate == null) _SendDate = ""; return _SendDate; }
            set { _SendDate = value; }
        }

        private string _IFReply;
        public string IFReply
        {
            get { if (_IFReply == null) _IFReply = ""; return _IFReply; }
            set { _IFReply = value; }
        }

        private string _ReplyDate;
        public string ReplyDate
        {
            get { if (_ReplyDate == null) _ReplyDate = ""; return _ReplyDate; }
            set { _ReplyDate = value; }
        }

        private string _ReplyCode;
        public string ReplyCode
        {
            get { if (_ReplyCode == null) _ReplyCode = ""; return _ReplyCode; }
            set { _ReplyCode = value; }
        }

        private string _ReplyTime;
        public string ReplyTime
        {
            get { if (_ReplyTime == null) _ReplyTime = ""; return _ReplyTime; }
            set { _ReplyTime = value; }
        }
    }

}
