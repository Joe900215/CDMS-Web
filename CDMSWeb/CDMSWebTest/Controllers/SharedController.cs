﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CDMSWebTest.Controllers
{
    public class SharedController : Controller
    {

        // CAD参照功能

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ErrorPreview()
        {
            return View();
        }
    }
}