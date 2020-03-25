using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVEVA.CDMS.Server;
using AVEVA.CDMS.Common;
using AVEVA.CDMS.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace AVEVA.CDMS.WTMS_Plugins
{
    public class WorkTask
    {
        /// <summary>
        /// 创建报审单文档
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="ProjectKeyword">目录Keyword</param>
        /// <param name="documentType">文档表式</param>
        /// <param name="fileCode">文档编号</param>
        /// <param name="docAttrJson">文档属性</param>
        /// <returns></returns>
        public static JObject CreateWorkTask(string sid, string ProjectKeyword, string title, string docAttrJson)
        {
            return WorkTaskService.CreateWorkTask(sid, ProjectKeyword, title, docAttrJson);
        }

        /// <summary>
        /// 回复工作任务
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="ProjectKeyword"></param>
        /// <returns></returns>
        public static JObject ReplyWorkTask(string sid, string ProjectKeyword)
        {
            return WorkTaskService.ReplyWorkTask(sid, ProjectKeyword);
        }

        /// <summary>
        /// 获取用户工作任务列表（包括需要回复的任务和已回复的任务）
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public static JObject GetUserWorkTaskList(string sid, string page, string limit)
        {
            return WorkTaskService.GetUserWorkTaskList(sid, page, limit);
        }

        public static JObject GetWorkTaskByKeyword(string sid, string ProjectKeyword) {
            return WorkTaskService.GetWorkTaskByKeyword(sid, ProjectKeyword);
        }



    }
}
