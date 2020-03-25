using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AVEVA.CDMS.Server;
using AVEVA.CDMS.Common;
using AVEVA.CDMS.WebApi;
using System.Runtime.Serialization;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
//using LinqToDB;

namespace AVEVA.CDMS.TBSBIM_Plugins
{
    public class Family
    {

        /// <summary>
        /// 获取创建信函表单的默认配置
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="ProjectKeyword"></param>
        /// <returns></returns>
        public static JObject GetFamilyCategory(string sid)
        {
            return FamilyService.GetFamilyCategory(sid);

        }

        /// <summary>
        /// 获取族文档列表
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="ProjectKeyWord"></param>
        /// <returns></returns>
        public static JObject GetFamilyDocList(string sid, string ProjectKeyWord)
        {
            return FamilyService.GetFamilyDocList(sid, ProjectKeyWord);
        }

    }
}
