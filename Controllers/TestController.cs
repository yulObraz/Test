using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcSiteMapProvider;
using MvcTestApplication.Models;
using System.Reflection;

namespace MvcTestApplication.Controllers {
    public class TestController : Controller {
        //
        // GET: /Test/
        [MvcSiteMapNode(ParentKey = "Home", Key = "Operation1.1", Title = "Operation 1.1", Attributes = "{id:\"Method1\"}")]
        [MvcSiteMapNode(ParentKey = "Home", Key = "Operation1.2", Title = "Operation 1.2", Attributes = "{id:\"Method2\"}")]
        [MyAuthorize]
        public ActionResult Operation1(Methods id) {
            return View("Message", (object)MethodBase.GetCurrentMethod().Name);
        }
        [MvcSiteMapNode(ParentKey = "Home", Key = "Operation2.1", Title = "Operation 2.1", Attributes = "{method:\"Method1\"}")]
        [MvcSiteMapNode(ParentKey = "Home", Key = "Operation2.2", Title = "Operation 2.2", Attributes = "{method:\"Method2\"}")]
        [MyAuthorize]
        public ActionResult Operation2(Methods method) {
            return View("Message", (object)MethodBase.GetCurrentMethod().Name);
        }
        [MvcSiteMapNode(ParentKey = "Home", Key = "Operation3.1", Title = "Operation 3.1", Attributes = "{unexpected:\"Method1\"}")]
        [MvcSiteMapNode(ParentKey = "Home", Key = "Operation3.2", Title = "Operation 3.2", Attributes = "{unexpected:\"Method2\"}")]
        [MyAuthorize(FieldToCheck = "unexpected")]
        public ActionResult Operation3(Methods unexpected) {
            return View("Message", (object)MethodBase.GetCurrentMethod().Name);
        }
    }
}
