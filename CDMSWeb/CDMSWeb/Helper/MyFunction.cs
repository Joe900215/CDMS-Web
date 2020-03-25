using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace CDMSWeb.Helper
{
    public class MyFunction
    {
        //public static JObject WriteJObjectResult(bool success,JObject errors)
        //{
        //    JObject jo = new JObject { 
        //        new JProperty("success",success)
        //    };
        //    if(errors !=null && errors.HasValues)
        //    {
        //        jo.Add(new JProperty("errors", errors));
        //    }
        //    return jo;
        //}

        //定义要返回的数据结构
        //结构中，success表示返回结果是否成功。如果不成功，可通过Msg获取错误信息。如果成功，就从data中读取数据。
        //结构中的total非常重要，客户端会根据该值来计算页数，所以一定要正确返回，不然分页就会乱。
        public static JObject WriteJObjectResult(bool success, int total,string message, JArray data)
        {
            return new JObject { 
                new JProperty("success",success),
                new JProperty("total",total),
                new JProperty("msg",message),
                new JProperty("data",data)
            };
        }

        //定义要返回的数据结构
        //结构中，success表示返回结果是否成功。如果不成功，可通过Msg获取错误信息。如果成功，就从data中读取数据。
        //结构中的total非常重要，客户端会根据该值来计算页数，所以一定要正确返回，不然分页就会乱。


        public static void ModelStateToJObject(ModelStateDictionary ModelState, JObject errors)
        {
            foreach (var c in ModelState.Keys)
            {
                if (!ModelState.IsValidField(c))
                {
                    string errStr = "";
                    foreach (var err in ModelState[c].Errors)
                    {
                        errStr += err.ErrorMessage + "<br/>";
                    }
                    errors.Add(new JProperty(c, errStr));
                }
            }
        }

        //将错误代码转换为信息返回
        public static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "当前用户名已被注册，请使用其他用户名。";

                case MembershipCreateStatus.DuplicateEmail:
                    return "当前电子邮件已被注册，请使用其他电子邮件。";

                case MembershipCreateStatus.InvalidPassword:
                    return "密码错误，请输入正确的密码。";

                case MembershipCreateStatus.InvalidEmail:
                    return "电子邮件地址错误，请重新输入。";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "用户名错误，请输入正确的用户名。";

                case MembershipCreateStatus.ProviderError:
                    return "系统错误，请联系管理员。";

                case MembershipCreateStatus.UserRejected:
                    return "当前请求已被取消，请重新输入并再次尝试提交。如果还存在问题，请与管理员联系。";

                default:
                    return "未知错误，请重新输入并再次尝试提交。如果问题依然存在，请与管理员联系。";
            }
        }

        public static string ProcessSorterString(string[] fields, string sortinfo,string defaultSort)
        {
            if (string.IsNullOrEmpty(sortinfo)) return defaultSort;
            JArray ja = JArray.Parse(sortinfo);
            string result = "";
            foreach (JObject c in ja)
            {
                string field = (string)c["property"];
                if (fields.Contains(field))
                {
                    result += string.Format("it.{0} {1},", field, (string)c["direction"] == "ASC" ? "" : "DESC");
                }
            }
            if (result.Length > 0)
            {
                result = result.Substring(0, result.Length - 1);
            }
            else
            {
                result = defaultSort;
            }
            return result;
        }
    }
}