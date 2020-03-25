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
namespace AVEVA.CDMS.ZHEPC_Plugins
{
    public class Company
    {
        public static JObject GetGroupByUserSid(string sid, string ProjectKeyword)
        {
            return EditUnitUser.GetGroupByUserSid(sid, ProjectKeyword);
        }

        public static JObject CreateUser(string sid, string ProjectKeyword, string GroupName,
    string UserCode, string UserDesc, string UserEmail,// string UserType,
string UserStatus, string Phone, string UserPwd, string UserConfirmPwd)
        {
            return EditUnitUser.CreateUser(sid, ProjectKeyword, GroupName,
                UserCode, UserDesc, UserEmail,// UserType,
                UserStatus, Phone, UserPwd, UserConfirmPwd);
        }
        public static JObject SaveUserInfo(string sid, string ProjectKeyword, string GroupName,
            string UserKeyword, string UserCode, string UserDesc, string UserEmail, //string UserType,
        string UserStatus, string Phone, string UserPwd, string UserConfirmPwd)
        {
            return EditUnitUser.SaveUserInfo(sid, ProjectKeyword, GroupName,
               UserKeyword, UserCode, UserDesc, UserEmail, //UserType,
                UserStatus, Phone, UserPwd, UserConfirmPwd);
        }
    }
}
