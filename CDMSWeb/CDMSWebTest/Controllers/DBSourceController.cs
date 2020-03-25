using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CDMSWebTest.Controllers
{
    public class DBSourceController : Controller
    {


        public ActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// JS端调用函数，登录数据源
        /// </summary>
        /// <param name="model">登陆model包括以下参数：UserName用户名，Password密码，Vcode验证码</param>
        /// <returns>返回一个JSON对象</returns>
        [HttpPost]
        public JObject Login(string UserName, string Password)//, string hostname)
        {
            JObject reJo = new JObject();
            try
            {
                Password = AVEVA.CDMS.WebApi.EnterPointController.ProcessRequest(Password);
                string hostname = Request.UserHostName;
                reJo = AVEVA.CDMS.WebApi.DBSourceController.Login(UserName, Password, hostname);

                JObject joMsg = (JObject)reJo["msg"];
                string guid = (string)joMsg["guid"];
                if (!string.IsNullOrEmpty(guid))
                {
                    Session["guid"] = guid;
                    Session["IsAuthenticated"] = "true";
                }
            }
            catch { reJo = new JObject(); }
            return reJo;
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="WebViewType">浏览器类型，如果是"mobile",退出登录后就定位到手机页</param>
        /// <returns></returns>
        public ActionResult Logout(string UserName, string WebViewType)
        {
            //  JObject reJo = new JObject();
            try
            {
                Session["IsAuthenticated"] = "flase";
                AVEVA.CDMS.WebApi.DBSourceController.Logout(UserName);
            }
            catch { }

            string IndexPage = "Home";
            if (WebViewType == "mobile")
            { IndexPage = "M"; }
            return RedirectToAction("Index", IndexPage);//调整到首页
        }


    }
}