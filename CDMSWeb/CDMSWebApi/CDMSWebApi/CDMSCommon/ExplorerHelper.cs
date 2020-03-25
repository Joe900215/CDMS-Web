using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AVEVA.CDMS.WebApi
{
    public class ExplorerHelper
    {
        /// <summary>
        /// 把Dict转换为JArray
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static JArray DictToJArray(Dictionary<string, string> context)
        {
            JArray ja = new JArray();
            foreach (KeyValuePair<string, string> kvp in context)
            {
                ja.Add(new JObject(new JProperty("name", kvp.Key), new JProperty("value", kvp.Value)));
            }
            return new JArray();
        }
    }
}
