using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace AVEVA.CDMS.WebApi.Helper
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


        //public static void ModelStateToJObject(ModelStateDictionary ModelState, JObject errors)
        //{
        //    foreach (var c in ModelState.Keys)
        //    {
        //        if (!ModelState.IsValidField(c))
        //        {
        //            string errStr = "";
        //            foreach (var err in ModelState[c].Errors)
        //            {
        //                errStr += err.ErrorMessage + "<br/>";
        //            }
        //            errors.Add(new JProperty(c, errStr));
        //        }
        //    }
        //}

        //将错误代码转换为信息返回

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