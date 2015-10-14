using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcTestApplication.Models {
    public class MyAuthorizeAttribute : AuthorizeAttribute {
        public string FieldToCheck { get; set; }
        public override void OnAuthorization(AuthorizationContext filterContext) {
            var attempted = filterContext.Controller.ValueProvider.GetValue(FieldToCheck ?? "otherMethod") ?? filterContext.Controller.ValueProvider.GetValue("method") ?? filterContext.Controller.ValueProvider.GetValue("id");
            string param = attempted.AttemptedValue;
            Methods method;

            if(Enum.TryParse<Methods>(param, out method)) {
                if(method != Methods.Method1)
                    return;
                //if(method == Methods.Method2 && filterContext.HttpContext.User.IsInRole("Method2Available"))
                //    return;

                filterContext.Result = new HttpUnauthorizedResult();
            }
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext) {
            //string field = FieldToCheck ?? "method";
            return base.AuthorizeCore(httpContext);
        }
        protected override HttpValidationStatus OnCacheAuthorization(HttpContextBase httpContext) {
            return base.OnCacheAuthorization(httpContext);
        }
    }
}