using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcSiteMapProvider;

namespace MvcTestApplication.Controllers {
    public class HomeController : Controller {
        [MvcSiteMapNode(Key = "Home", Title="Home")]
        public ActionResult Index() {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }
        public ActionResult Unauthorized() {
            return View("Message", (object)"Unauthorized");
        }
    }
}
