using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using AVEVA.CDMS.Common;
using AVEVA.CDMS.Server;

namespace AVEVA.CDMS.SHEPC_Plugins
{
    class CDMSFunction
    {

        //根据模板对象code查找各级父目录Project
        public static Project GetParentProjectByTempDefn(Project project, string tempDefnCode)
        {
            Project resultProject = project;
            try
            {
                while (resultProject.ParentProject != null)
                {
                    if (resultProject.TempDefn == null)
                    { resultProject = resultProject.ParentProject; }
                    else if (!resultProject.TempDefn.Code.Equals(tempDefnCode))
                    {
                        resultProject = resultProject.ParentProject;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (Exception exception)
            {
                resultProject = null;
            }
            return resultProject;
        }
    }
}
