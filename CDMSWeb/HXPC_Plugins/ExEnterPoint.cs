using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVEVA.CDMS.Server;
using Newtonsoft.Json.Linq;
using AVEVA.CDMS.WebApi;

namespace AVEVA.CDMS.HXPC_Plugins
{
    class ExEnterPoint
    {
        /// <summary>
        ///创建根项目目录Explorer接口 
        /// </summary>
        /// <param name="m_Project">选择的目录</param>
        /// <param name="projectAttrDict">目录属性字典</param>
        /// <returns>空字符串，表示成功，其他字符串表示失败</returns>
        //public static string CreateRootProject(Project m_Project, Dictionary<string, string> projectAttrDict)
        //{
        //    DBSource dbsource = m_Project.dBSource;
        //    JArray jaAttr = AVEVA.CDMS.WebApi.ExplorerHelper.DictToJArray(projectAttrDict);
        //    //创建项目
        //    ExReJObject reJo = EnterPoint.CreateRootProjectX(dbsource, jaAttr,"");

        //    if (reJo.success == true)
        //    { return ""; }
        //    else
        //    {
        //        return reJo.msg;
        //    }
        //}
    }
}
