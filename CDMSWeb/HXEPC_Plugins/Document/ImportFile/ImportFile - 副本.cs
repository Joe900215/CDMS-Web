using AVEVA.CDMS.Server;
using AVEVA.CDMS.WebApi;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVEVA.CDMS.HXEPC_Plugins
{
    public class ImportFile
    {
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

                Doc doc = dbsource.GetDocByKeyWord(DocKeyword);
                if (doc == null)
                {
                    reJo.msg = "错误的文档操作信息！指定的文档不存在！";
                    return reJo.Value;
                }

                List<Doc> docList = new List<Doc>() { doc };
                WorkFlow flow = dbsource.NewWorkFlow(docList, "CHECKFILE");
                //if (flow == null || flow.CuWorkState == null || flow.CuWorkState.workStateBranchList == null || (flow.CuWorkState.workStateBranchList.Count <= 0))
                if (flow == null)
                {
                    reJo.msg = "自动启动流程失败!请手动启动";
                    return reJo.Value;
                }

                Group group = new Group() ;
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

        public static JObject ReadAttrFromExcel(string sid, string DocKeyword) {
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

                Doc doc = dbsource.GetDocByKeyWord(DocKeyword);
                if (doc == null)
                {
                    reJo.msg = "错误的文档操作信息！指定的文档不存在！";
                    return reJo.Value;
                }

                string strFileName = doc.FullPathFile;


                object missing = System.Reflection.Missing.Value;
                Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();//lauch excel application
                if (excel == null)
                {

                    // MessageBox.Show("alert('Can't access excel')");
                    reJo.msg = "打开excel失败！";
                    return reJo.Value;
                }
                else
                {
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
                    if (rowsint <= 2) {
                        reJo.msg = "表格行数需要大于2，请选择正确的表格！";
                        return reJo.Value;
                    }
                    //得到总列数
                    int colsint = ws.UsedRange.Cells.Columns.Count;

                    Range rngHead = ws.Cells.get_Range("A1", "AB1");   //item


                    object[,] arryHead = (object[,])rngHead.Value2;   //get range's value

                    //if (colsint > 21) { colsint = 21; }

                    //定义标题行
                    CataHead headRow = new CataHead();
                    //.ColName


                    for (int i = 1; i <= colsint - 1; i++)
                    {
                        try
                        {
                            if (arryHead[1, i] == null) continue;

                            string strhead = arryHead[1, i].ToString().Replace("\n", "");
                            // "文件题名" )//|| strhead == "文件\n题名")
                            if (strhead == headRow.FileName.ColName)
                            {

                                headRow.FileName.ColIndex = IntToCols(i);
                                headRow.FileName.ColIndexInt = i;
                            }
                            else if (strhead == headRow.Reference.ColName)
                            {

                                headRow.Reference.ColIndex = IntToCols(i);
                                headRow.Reference.ColIndexInt = i;
                            }
                            else if (strhead == headRow.FileTitle.ColName)
                            {

                                headRow.FileTitle.ColIndex = IntToCols(i);
                                headRow.FileTitle.ColIndexInt = i;
                            }
                          
                            else if (strhead == headRow.Volumenumber.ColName)
                            {

                                headRow.Volumenumber.ColIndex = IntToCols(i);
                            }
                            else if (strhead == headRow.Filecode.ColName)
                            {

                                headRow.Filecode.ColIndex = IntToCols(i);
                                headRow.Filecode.ColIndexInt = i;
                            }
                            else if (strhead == headRow.Responsibility.ColName)
                            {

                                headRow.Responsibility.ColIndex = IntToCols(i);
                            }

                            else if (strhead == headRow.Page.ColName)
                            {

                                headRow.Page.ColIndex = IntToCols(i);
                            }
                            else if (strhead == headRow.Number.ColName)
                            {

                                headRow.Number.ColIndex = IntToCols(i);
                            }
                            else if (strhead == headRow.Medium.ColName)
                            {

                                headRow.Medium.ColIndex = IntToCols(i);
                            }
                            else if (strhead == headRow.Languages.ColName)
                            {

                                headRow.Languages.ColIndex = IntToCols(i);
                            }
                            else if (strhead == headRow.Proname.ColName)
                            {

                                headRow.Proname.ColIndex = IntToCols(i);
                            }
                            else if (strhead == headRow.Procode.ColName)
                            {

                                headRow.Procode.ColIndex = IntToCols(i);
                            }
                            else if (strhead == headRow.Major.ColName)
                            {

                                headRow.Major.ColIndex = IntToCols(i);
                            }
                            else if (strhead == headRow.Crew.ColName)
                            {

                                headRow.Crew.ColIndex = IntToCols(i);
                            }
                            else if (strhead == headRow.FactoryCode.ColName)
                            {

                                headRow.FactoryCode.ColIndex = IntToCols(i);
                            }
                            else if (strhead == headRow.FileTitle.ColName)
                            {

                                headRow.Factoryname.ColIndex = IntToCols(i);
                            }
                            else if (strhead == headRow.Factoryname.ColName)
                            {

                                headRow.Factoryname.ColIndex = IntToCols(i);
                            }
                            else if (strhead == headRow.Systemcode.ColName)
                            {

                                headRow.Systemcode.ColIndex = IntToCols(i);
                            }
                            else if (strhead == headRow.Systemname.ColName)
                            {

                                headRow.Systemname.ColIndex = IntToCols(i);
                            }
                            else if (strhead == headRow.Relationfilecode.ColName)
                            {

                                headRow.Relationfilecode.ColIndex = IntToCols(i);
                            }
                            else if (strhead == headRow.Relationfilename.ColName)
                            {

                                headRow.Relationfilename.ColIndex = IntToCols(i);
                            }
                            else if (strhead == headRow.Filespec.ColName)
                            {

                                headRow.Filespec.ColIndex = IntToCols(i);
                            }
                            else if (strhead == headRow.Fileunit.ColName)
                            {

                                headRow.Fileunit.ColIndex = IntToCols(i);
                            }
                            else if (strhead == headRow.Secretgrade.ColName)
                            {

                                headRow.Secretgrade.ColIndex = IntToCols(i);
                            }
                            else if (strhead == headRow.FileTitle.ColName)
                            {

                                headRow.Keepingtime.ColIndex = IntToCols(i);
                            }
                            else if (strhead == headRow.Keepingtime.ColName)
                            {

                                headRow.Keepingtime.ColIndex = IntToCols(i);
                            }
                            else if (strhead == headRow.Filelistcode.ColName)
                            {

                                headRow.Filelistcode.ColIndex = IntToCols(i);
                            }
                            else if (strhead == headRow.Filelisttime.ColName)
                            {

                                headRow.Filelisttime.ColIndex = IntToCols(i);
                            }
                            else if (strhead == headRow.Note.ColName)
                            {

                                headRow.Note.ColIndex = IntToCols(i);
                            }


                        }
                        catch { }

                    }

                    //if (flangeIndex == "" || lengthIndex == "" || boltspecIndex == "")
                    if (string.IsNullOrEmpty(headRow.FileTitle.ColIndex))
                    {
                        // MessageBox.Show("获取文件题名列名失败，请选择正确的表格！");
                        reJo.msg = "获取文件题名列名失败，请选择正确的表格！";
                        return reJo.Value;
                    }

                    //Range rngFileTitle = ws.Cells.get_Range(headRow.FileTitle.ColIndex + "2", headRow.FileTitle.ColIndex + rowsint);   //法兰

                    //Range rngReference = ws.Cells.get_Range(headRow.Reference.ColIndex + "2", headRow.Reference.ColIndex + rowsint);   //法兰

                    //Range rngFileCode = ws.Cells.get_Range(headRow.Filecode.ColIndex + "2", headRow.Filecode.ColIndex + rowsint);
                    

                    //object[,] arryFileTitle = (object[,])rngFileTitle.Value2;   //get range's value

                    //object[,] arryReference = (object[,])rngReference.Value2;

                    //object[,] arryFileCode= (object[,])rngFileCode.Value2;

                    string MaxColIndex = IntToCols(colsint);

                    List<CataRow> cataRowList = new List<CataRow>();
                    for (int i = 2; i <= rowsint - 1; i++)
                    {
                        // if (arryFileTitle[i, 1] == null || arryFileTitle[i, 1] == null || arryFileTitle[i, 1] == null) { continue; }

                        //读取一行
                        CataRow cRow=GetGataRow(ws,headRow,i,MaxColIndex);
                        
                        //var bl = boltLenList.Find(b => b.flange == flange && b.boltspec == boltspec);
                        //if (bl == null)
                        //{
                        if (cRow != null)
                        {
                            cataRowList.Add(cRow);
                        }
                        //}

                    }

                    JArray dataJa = new JArray();


                    foreach (CataRow cRow in cataRowList)
                    {

                        dataJa.Add(new JObject(
                            new JProperty(headRow.Reference.ColCode, cRow.Reference),
                            new JProperty(headRow.FileTitle.ColCode, cRow.FileTitle),
                             new JProperty(headRow.Filecode.ColCode, cRow.Filecode)
                            ));
                    }

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

        public static CataRow GetGataRow(Worksheet ws, CataHead headRow, int RowIndex, string MaxColIndex)
        {
            //CataRow cRow = GetGataRow(ws, MaxColIndex);
            CataRow cRow = null;
            try
            {
                //读取一行
                Range rngRow = ws.Cells.get_Range("A" + RowIndex, MaxColIndex + RowIndex);   //法兰
                object[,] arryRow = (object[,])rngRow.Value2;

                string filetitle = GetRowValue(arryRow,headRow.FileTitle);
                string reference = GetRowValue(arryRow, headRow.Reference);
                string filecode = GetRowValue(arryRow, headRow.Filecode);

                //string filetitle = (headRow.FileTitle==null ||
                //    arryRow[1, headRow.FileTitle.ColIndexInt] == null) ?
                //    "" : arryRow[1, headRow.FileTitle.ColIndexInt].ToString();

                //string reference = arryRow[1, headRow.Reference.ColIndexInt] == null ?
                //    "" : arryRow[1, headRow.Reference.ColIndexInt].ToString();

                //string filecode = arryRow[1, headRow.Filecode.ColIndexInt] == null ?
                //    "" : arryRow[1, headRow.Filecode.ColIndexInt].ToString();
                //string reference
                //序号列
                //string filetitle = arryFileTitle[i, 1] == null ? "" : arryFileTitle[i, 1].ToString();
                //string reference = arryReference[i, 1] == null ? "" : arryReference[i, 1].ToString();
                //string filecode = arryReference[i, 1] == null ? "" : arryFileCode[i, 1].ToString();

                if (filetitle == "" && filecode == "")
                {
                    return null;
                }
                cRow = new CataRow()
                {
                    FileTitle = filetitle,
                    Reference = reference,
                    Filecode = filecode

                };
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
            return cRow;
        }

        public static string GetRowValue(object[,] arryRow, CataCol col)
        {
            string result = (col == null || col.ColIndexInt == 0 ||
                arryRow[1, col.ColIndexInt] == null) ?
                "" : arryRow[1, col.ColIndexInt].ToString();
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
            if (index > 52)
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
                    v.ColCode = "factoryCode";
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
                if (_Filecode == null)
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


    }

    //著录表列
    public class CataCol {
        public int ColIndexInt { get; set; }
        public string ColIndex { get; set; }
        public string RawIndex { get; set; }

        public string ColName { get; set; }

        public string ColCode { get; set; }
        public string ColCaCode { get; set; }
        public string Value { get; set; }
    }
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
            get { if (_Filecode == null) _Filecode = ""; return _Filecode; }
            set { _Filecode = value; }
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

        private string _Note;
        public string Note
        {
            get { if (_Note == null) _Note = ""; return _Note; }
            set { _Note = value; }
        }
    }
}
