using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CDMSWebTest.Controllers
{
    public class WebApiController : Controller // ApiController//
    {
        //get方式调用web函数，返回JObject
        //sid:传输秘钥
        //C：插件类库名
        //A:调用的函数名
        [HttpGet]
        public JObject Get()
        {
            HttpRequestBase hrb = Request;
            return Enter(hrb);
        }

        //get方式调用web函数，返回JObject
        //sid:传输秘钥
        //C：插件类库名
        //A:调用的函数名
        [HttpGet]
        public void GetJsonp()
        {
            string callback = Request["callback"];

            HttpRequestBase hrb = Request;
            JObject reJo = Enter(hrb);

            //转换成jsonp返回
            string response = JsonConvert.SerializeObject(reJo);
            string call = callback + "(" + response + ")";
            Response.Write(call);
        }

        //post方式调用web函数，返回JObject
        //sid:传输秘钥
        //C：插件路径+类库名
        //A:调用的函数名
        [HttpPost]
        public JObject Post()
        {
            HttpRequestBase hrb = Request;
            return Enter(hrb);
        }


        private List<Type> mPluginTypeList;


        /// <summary>
        /// 调用插件组件
        /// </summary>
        private List<Type> PluginTypeList
        {
            get
            {
                if (mPluginTypeList == null)
                {
                    //添加调用的类库
                    mPluginTypeList = new List<Type>();
                    //mPluginTypeList.Add(typeof(AVEVA.CDMS.SHEPC_Plugins.EnterPoint));

                    ////添加华西项目用到的类库
                    mPluginTypeList.Add(typeof(AVEVA.CDMS.HXPC_Plugins.EnterPoint));
                    mPluginTypeList.Add(typeof(AVEVA.CDMS.HXPC_Plugins.CreateProject));
                    mPluginTypeList.Add(typeof(AVEVA.CDMS.HXPC_Plugins.Profession));
                    mPluginTypeList.Add(typeof(AVEVA.CDMS.HXPC_Plugins.ExchangeDoc));
                    mPluginTypeList.Add(typeof(AVEVA.CDMS.HXPC_Plugins.SelectUser));
                    mPluginTypeList.Add(typeof(AVEVA.CDMS.HXPC_Plugins.Document));
                    mPluginTypeList.Add(typeof(AVEVA.CDMS.HXPC_Plugins.Company));
                    mPluginTypeList.Add(typeof(AVEVA.CDMS.HXPC_Plugins.Prodect));
                    mPluginTypeList.Add(typeof(AVEVA.CDMS.HXPC_Plugins.CommonFunction));
                    mPluginTypeList.Add(typeof(AVEVA.CDMS.HXPC_Plugins.WorkTask));

                    //添加华西EPC项目用到的类库
                    //mPluginTypeList.Add(typeof(AVEVA.CDMS.HXEPC_Plugins.EnterPoint));
                    //mPluginTypeList.Add(typeof(AVEVA.CDMS.HXEPC_Plugins.HXProject));
                    //mPluginTypeList.Add(typeof(AVEVA.CDMS.HXEPC_Plugins.Document));
                    //mPluginTypeList.Add(typeof(AVEVA.CDMS.HXEPC_Plugins.CommonFunction));
                    //mPluginTypeList.Add(typeof(AVEVA.CDMS.HXEPC_Plugins.Company));

                    //添加珠海钰海项目用到的类库
                    mPluginTypeList.Add(typeof(AVEVA.CDMS.ZHEPC_Plugins.EnterPoint));
                    mPluginTypeList.Add(typeof(AVEVA.CDMS.ZHEPC_Plugins.Document));
                    mPluginTypeList.Add(typeof(AVEVA.CDMS.ZHEPC_Plugins.Company));


                    //添加中建三局临建标准化项目用到的类库
                    mPluginTypeList.Add(typeof(AVEVA.CDMS.TBSBIM_Plugins.EnterPoint));
                    mPluginTypeList.Add(typeof(AVEVA.CDMS.TBSBIM_Plugins.Family));
                    mPluginTypeList.Add(typeof(AVEVA.CDMS.TBSBIM_Plugins.TBSProject));

                    //添加工作任务管理系统项目用到的类库
                    mPluginTypeList.Add(typeof(AVEVA.CDMS.WTMS_Plugins.EnterPoint));
                    mPluginTypeList.Add(typeof(AVEVA.CDMS.WTMS_Plugins.Company));
                    mPluginTypeList.Add(typeof(AVEVA.CDMS.WTMS_Plugins.WorkTask));

                    //mPluginTypeList.Add(typeof(AVEVA.CDMS.WTMS_Plugins.TBSProject));

                    //调用Init事件
                    //AVEVA.CDMS.HXEPC_Plugins.EnterPoint.Init();
                    AVEVA.CDMS.HXPC_Plugins.EnterPoint.Init();
                    //AVEVA.CDMS.SHEPC_Plugins.EnterPoint.Init();
                    AVEVA.CDMS.ZHEPC_Plugins.EnterPoint.Init();
                    AVEVA.CDMS.TBSBIM_Plugins.EnterPoint.Init();
                    AVEVA.CDMS.WTMS_Plugins.EnterPoint.Init();
                }

                return mPluginTypeList;

            }
        }

        public JObject Enter(HttpRequestBase Request)
        {

            //传送网站根目录在本地硬盘上的路径
            string Server_MapPath = Server.MapPath("/");

            //调用WebApi,处理对应函数
            return AVEVA.CDMS.WebApi.EnterPointController.Enter(Request, PluginTypeList, Server_MapPath);

        }

        //[HttpGet]
        //public JObject Login(string UserName, string Password)
        //{
        //    JObject reJo = new JObject();
        //    try
        //    {
        //        //Password = AVEVA.CDMS.WebApi.EnterPointController.ProcessRequest(Password);
        //        string hostname = Request.UserHostName;
        //        reJo = AVEVA.CDMS.WebApi.DBSourceController.Login(UserName, Password, hostname);

        //        JObject joMsg = (JObject)reJo["msg"];
        //        string guid = (string)joMsg["guid"];
        //        if (!string.IsNullOrEmpty(guid))
        //        {
        //            Session["guid"] = guid;
        //            Session["IsAuthenticated"] = "true";
        //        }
        //    }
        //    catch { reJo = new JObject(); }
        //    return reJo;
        //}

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
                //Password = AVEVA.CDMS.WebApi.EnterPointController.ProcessRequest(Password);
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