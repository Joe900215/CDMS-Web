using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AVEVA.CDMS.Server;
using AVEVA.CDMS.Common;
using AVEVA.CDMS.WebApi;

namespace AVEVA.CDMS.TBSBIM_Plugins
{
    public class TBSProject
    {

        public static JObject CheckProjectKey(string sid, string ProjectKey)
        {
            return TBSProjectService.CheckProjectKey(sid, ProjectKey);
        }

        public static JObject CreateTBSProject(string sid, string ProjectName, string DownloadPsw)
        {
            return TBSProjectService.CreateTBSProject(sid, ProjectName, DownloadPsw);
        }

        public static JObject GetPrjOverviewDefault(string sid, string ProjectKeyword) {
            return TBSProjectService.GetPrjOverviewDefault(sid, ProjectKeyword);
        }

        public static JObject GetArrangeOverviewDefault(string sid, string ProjectKeyword) {
            return TBSProjectService.GetArrangeOverviewDefault(sid, ProjectKeyword);
        }

        public static JObject GetTBSProjectKeyword(string sid, string RootProjectKeyword, string ProjectType)
        {
            return TBSProjectService.GetTBSProjectKeyword(sid, RootProjectKeyword, ProjectType);
        }

        public static JObject GetPlanDocList(string sid, string ProjectKeyword)
        {
            return TBSProjectService.GetPlanDocList(sid, ProjectKeyword);
        }
    }
}
