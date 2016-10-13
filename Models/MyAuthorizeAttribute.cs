using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcTestApplication.Models {
    public class MyAuthorizeAttribute : AuthorizeAttribute {
        public string FieldToCheck { get; set; }
        protected override bool AuthorizeCore(HttpContextBase httpContext) {
            string param = GetValue(httpContext, FieldToCheck ?? "method") ?? GetValue(httpContext, "id");
            Methods method;

            if(Enum.TryParse<Methods>(param, out method)) {
                if(method == Methods.Method1)
                    return false;
                //if(method == Methods.Method2 && httpContext.User.IsInRole("Method2Available"))
                //    return true;
            }
            return true;
        }

        private string GetValue(HttpContextBase httpContext, string name) {
            return httpContext.Request.QueryString[name] ?? (string)httpContext.Request.RequestContext.RouteData.Values[name];
        }
    }
}