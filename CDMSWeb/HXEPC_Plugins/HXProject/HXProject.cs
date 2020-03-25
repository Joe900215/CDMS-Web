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

namespace AVEVA.CDMS.HXEPC_Plugins
{
    public class HXProject
    {

        public static JObject GetCreateProjectListingDefault(string sid) {
            return CreateProject.GetCreateProjectListingDefault(sid);
        }

        public static JObject GetProjectTypeII(string sid, string ProjectType) {
            return CreateProject.GetProjectTypeII(sid, ProjectType);
        }

        public static JObject CreateRootProject(string sid, string projectAttrJson) {
            return CreateProject.CreateRootProject(sid, projectAttrJson);
        }

        public static JObject CreateProjectListing(string sid, string ProjectKeyword, string projectAttrJson) {
            return CreateProject.CreateProjectListing(sid, ProjectKeyword, projectAttrJson);
        }

        public static JObject GetEditCrewDefault(string sid, string ProjectKeyword) {
            return EditCrew.GetEditCrewDefault(sid, ProjectKeyword);
        }

        public static JObject EDITCrew(string sid, string ProjectKeyword, string crewAttrJson)
        {
            return EditCrew.EDITCrew(sid, ProjectKeyword, crewAttrJson);
        }

        public static JObject CreateCrew(string sid, string ProjectKeyword, string crewAttrJson)
        {
            return EditCrew.CreateCrew(sid, ProjectKeyword, crewAttrJson);
        }

        public static JObject DeleteCrew(string sid, string ProjectKeyword, string CrewCode, string CrewId)
        {
            return EditCrew.DeleteCrew(sid, ProjectKeyword, CrewCode, CrewId);
        }


        public static JObject GetEditSystemDefault(string sid, string ProjectKeyword)
        {
            return EditSystem.GetEditSystemDefault(sid, ProjectKeyword);
        }

        public static JObject EDITSystem(string sid, string ProjectKeyword, string systemAttrJson)
        {
            return EditSystem.EDITSystem(sid, ProjectKeyword, systemAttrJson);
        }

        public static JObject CreateSystem(string sid, string ProjectKeyword, string systemAttrJson)
        {
            return EditSystem.CreateSystem(sid, ProjectKeyword, systemAttrJson);
        }

        /// <summary>
        /// 删除系统
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="ProjectKeyword"></param>
        /// <param name="CrewCode"></param>
        /// <returns></returns>
        public static JObject DeleteSystem(string sid, string ProjectKeyword, string SystemCode, string SystemId)
        {
            return EditSystem.DeleteSystem(sid, ProjectKeyword, SystemCode, SystemId);
        }

        public static JObject GetEditFactoryDefault(string sid, string ProjectKeyword)
        {
            return EditFactory.GetEditFactoryDefault(sid, ProjectKeyword);
        }

        public static JObject EDITFactory(string sid, string ProjectKeyword, string factoryAttrJson)
        {
            return EditFactory.EDITFactory(sid, ProjectKeyword, factoryAttrJson);
        }

        public static JObject CreateFactory(string sid, string ProjectKeyword, string factoryAttrJson)
        {
            return EditFactory.CreateFactory(sid, ProjectKeyword, factoryAttrJson);
        }

        public static JObject DeleteFactory(string sid, string ProjectKeyword, string FactoryCode, string FactoryId)
        {
            return EditFactory.DeleteFactory(sid, ProjectKeyword, FactoryCode, FactoryId);
        }

        public static JObject GetEditContractDefault(string sid, string ProjectKeyword)
        {
            return EditContract.GetEditContractDefault(sid, ProjectKeyword);
        }

        public static JObject EDITContract(string sid, string ProjectKeyword, string contractAttrJson)
        {
            return EditContract.EDITContract(sid, ProjectKeyword, contractAttrJson);
        }

        public static JObject CreateContract(string sid, string ProjectKeyword, string contractAttrJson)
        {
            return EditContract.CreateContract(sid, ProjectKeyword, contractAttrJson);
        }

        public static JObject DeleteContract(string sid, string ProjectKeyword, string ContractCode, string ContractId)
        {
            return EditContract.DeleteContract(sid, ProjectKeyword, ContractCode, ContractId);
        }

    }
}
