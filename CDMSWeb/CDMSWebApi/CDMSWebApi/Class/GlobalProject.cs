using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AVEVA.CDMS.Server;

namespace AVEVA.CDMS.WebApi
{
    public class GlobalProject
    {
        //***********************************************************************************************↓
        //获取Project对象的在CDMS结构中的全路径,DBName||DBSERVERIP\Project1\Project2\Project3\Project4.....
        internal  static string GetProjectShowPath(Project project, bool bRelate = false, bool bForDoc = false)
        {
            string path = "";
            try
            {
                if (project == null)
                    return path;
                path = project.ToString;

                Project tempProject = project.ParentProject;

                while (tempProject != null && !string.IsNullOrEmpty(tempProject.ToString))
                {

                    if (bRelate && bForDoc)
                    {
                        path = "...\\" + path;
                        break;
                    }

                    path = tempProject.ToString + "\\" + path;

                    tempProject = tempProject.ParentProject;


                    if (bRelate && tempProject != null)
                    {
                        path = "..\\" + path;
                        break;
                    }
                }

                string projectType="";
                if (project.O_type == enProjectType.Local)
                {
                    projectType = /*"文档"*/"项目";
                }

                if (project.O_type == enProjectType.GlobCustom)
                {
                    projectType = "逻辑目录";
                }

                if (project.O_type == enProjectType.UserCustom)
                {
                    projectType = "用户工作台";

                }

                if (project.O_type == enProjectType.GlobSearch)
                {
                    projectType = "查询\\全局查询";
                }

                if (project.O_type == enProjectType.UserSearch)
                {
                    projectType = "查询\\个人查询";
                }
                if (!string.IsNullOrEmpty(projectType) && projectType.Length > 0)
                    path = project.dBSource.DBAppServerName + ":\\\\" + project.dBSource.DBName + "\\" + projectType + "\\" + path;
                else
                    path = project.dBSource.DBAppServerName + ":\\\\" + project.dBSource.DBName + "\\" + path;

            }
            catch (Exception e)
            {
                WebApi.CommonController.WebWriteLog(e.Message + "," + e.StackTrace);
            }

            return path;
        }


        // 根据地址栏里的地址，获取对象
        internal static System.Object GetObjectByShowPath(DBSource dbsource,string strPath)
        {
            //0          1    2    3                      4 5    6
            //ydserver:\\GEDI\项目\H0031S__陆丰核电厂工程\J\YALF\YALF-0004
            string  sPath = strPath;
            //array<wchar_t> ^ spliter = { '\\'};
            //new char[] { ',' }
            //按照\来切碎
            string[] lstPrjString = sPath.Split(new char[] { '\\' }, System.StringSplitOptions.RemoveEmptyEntries);

            if (lstPrjString.Length <= 1)
                return null;

            //foreach(DBSource  dbs in dbsource)
            DBSource dbs = dbsource;


    {
                //数据源跟第一项一致，并且数据源已经登录
                //必须转化为CString来比较，直接用String^比较会有问题，才修正过来！
                //if (((string)dbs.DBName).CompareNoCase((string)(lstPrjString[1])) == 0 && !dbs.IsLogin)
                //    continue;
                if (!dbs.IsLogin)
                {
                    return null;
                }

                //2017-6-27 小钱 查找文档功能，这里添加了逻辑目录
                if (lstPrjString[2] == "项目" || lstPrjString[2] == "逻辑目录")
                {
                    //解析出第一项，第一项要从dbs上来获取成list，依据是父目录为0的那一个
                    string  code;
                    //解析出code
                    if (lstPrjString[3].Contains("__"))
                        code = lstPrjString[3].Substring(0, lstPrjString[3].IndexOf("__"));
                    else
                        code = lstPrjString[3];
                    List < Project > lstPrj = dbs.GetProjectByCode(code);
                    if (lstPrj == null)
                        return null;
                        //continue;

                    Project  parentPrj=null;
                    foreach (Project prj in lstPrj)
                    {
                        if (prj.O_parentno == 0)
                        {
                            parentPrj = prj;
                            break;
                        }
                    }
                    if (parentPrj == null)
                        return null;
                        //continue;

                    //再对剩下的目录，逐级获取
                    for (int i = 4; i < lstPrjString.Length; i++)
                    {
                        //解析出code
                        if (lstPrjString[i].Contains("__"))
                            code = lstPrjString[i].Substring(0, lstPrjString[i].IndexOf("__"));
                        else
                            code = lstPrjString[i];

                        //2017-6-27 小钱 这里添加了查找文档功能
                        //parentPrj = parentPrj.GetProjectByName(code);
                        ////为空说明中间乱输入，不存在，就中断
                        //if (!parentPrj)
                        //	break;

                        Project  prjTemp = parentPrj.GetProjectByName(code);
                        //如果按照代码切出来获取的prj为空，就整串get一次（防止有人把目录命名成code__desc作为code）
                        if (prjTemp==null)
                            prjTemp = parentPrj.GetProjectByName(lstPrjString[i]);

                        //为空说明中间乱输入，不存在，就中断
                        //或者到了末尾doc那里
                        if (prjTemp==null)
                        {
                            foreach(Doc  d in parentPrj.DocList)
        
                        if (d.Code == lstPrjString[lstPrjString.Length - 1])
                                    return d;

                            parentPrj = prjTemp;
                            break;
                        }
                        parentPrj = prjTemp;


                    }
                    //能到这里说明整个串都解析成功，就返回这个prj
                    if (parentPrj!=null)
                        return parentPrj;
                }
                else if (lstPrjString[2] == "用户工作台" || lstPrjString[2] == "查询")
                {
                    //这些是个性化，不作处理，每个用户都基本不同的
                }
            }
            return null;
        }
    }
}
