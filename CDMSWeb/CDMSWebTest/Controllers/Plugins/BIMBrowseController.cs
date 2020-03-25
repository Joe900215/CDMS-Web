using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CDMSWebTest.Controllers.Plugins
{
    /// <summary>
    /// BIM模型显示所用到的控制器
    /// </summary>
    public class BIMBrowseController : Controller
    {
        //
        // GET: /BIMBrowse/

        public ActionResult Index(string docKeyword, string docDesc)
        {
            ViewData["docKeyword"] = docKeyword;
            ViewData["docDesc"] = docDesc;
            return View();
        }

    }
}