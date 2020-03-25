using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVEVA.CDMS.WebApi;
using AVEVA.CDMS.Server;
using System.Collections;

namespace AVEVA.CDMS.ZHEPC_Plugins
{
    internal class ReportMenu
    {
        internal static enWebMenuState GetReportMenuState(string Sid,List<Project> SelProjectList,string DocType)
        {
            enWebMenuState hide = enWebMenuState.Hide;
            try
            {
                string str2 = "CONTACTTYPE";
                if (((SelProjectList != null) && (SelProjectList.Count == 1)) &&
                    ((SelProjectList[0].TempDefn != null) && (SelProjectList[0].TempDefn.KeyWord == str2) &&
                    (SelProjectList[0].Code == DocType) &&
                    SelProjectList[0].ParentProject.ParentProject.Code != "收文"
                    && (SelProjectList[0].ParentProject.ParentProject.Code == "发文" ||
                    SelProjectList[0].ParentProject.Code == "发文")
                    ))
                {

                        bool hasRight = WebApi.ProjectController.GetProjectPCreateRight(SelProjectList[0], Sid);

                        //有创建权限才可以创建目录
                        if (hasRight)
                            hide = enWebMenuState.Enabled;
                    
                }
            }
            catch { }
            return hide;
        }
    }
}
