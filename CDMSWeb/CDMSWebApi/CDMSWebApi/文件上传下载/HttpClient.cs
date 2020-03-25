using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVEVA.CDMS.WebApi
{
    public class HttpClient
    {
        //把中文字符转换成url编码
        public static string EncodeUrl(string zh_cn)
        {
            //把空格变成加号
            string url= System.Web.HttpUtility.UrlEncode(zh_cn, Encoding.UTF8);
            url= url.Replace("\\+", "%20");
            url = url.Replace("+", "%20");
            return url;
            //return System.Web.HttpUtility.UrlEncode(zh_cn, Encoding.UTF8);
            
        }
    }
}
