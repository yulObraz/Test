using MvcSiteMapProvider.Caching;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
#if !NET35
using System.Web.UI;
using MvcSiteMapProvider.Web.Mvc;
using MvcSiteMapProvider;
using MvcSiteMapProvider.Web;
using System.Reflection;
#endif

namespace MvcTestApplication.DI.MVCSiteMapFixes
{
    /// <summary>
    /// An abstract factory that can be used to create new instances of MVC
    /// context-related instances at runtime.
    /// </summary>
    public class MvcContextFactory
        : IMvcContextFactory
    {
        #region IMvcContextFactory Members

        public virtual HttpContextBase CreateHttpContext()
        {
            return CreateHttpContext(null);
        }

        protected virtual HttpContextBase CreateHttpContext(ISiteMapNode node)
        {
            return new SiteMapHttpContext(HttpContext.Current, node, null);
        }
        private HttpContextBase CreateHttpContext(ISiteMapNode node, Uri uri) {
            if(uri == null)
                throw new ArgumentNullException("uri");

            /*var request = new HttpRequest(
                filename: string.Empty,
                url: uri.ToString(),
                queryString: string.IsNullOrEmpty(uri.Query) ? string.Empty : uri.Query.Substring(1));
            var response = HttpContext.Current.Response;
            var httpContext = new HttpContext(request, response);*/
            return new SiteMapHttpContext(HttpContext.Current, node, null);
        }

        public virtual HttpContextBase CreateHttpContext(ISiteMapNode node, Uri uri, TextWriter writer)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");
            if (writer == null)
                throw new ArgumentNullException("writer");

            var request = new HttpRequest(
                filename: string.Empty, 
                url: uri.ToString(), 
                queryString: string.IsNullOrEmpty(uri.Query) ? string.Empty : uri.Query.Substring(1));
            var response = new HttpResponse(writer);
            var httpContext = new HttpContext(request, response);
            return new SiteMapHttpContext(httpContext, node, uri);
        }

        public virtual RequestContext CreateRequestContext(ISiteMapNode node, RouteData routeData)
        {
            var nodeUri = new Uri(HttpContext.Current.Request.Url, node.Url);

            var httpContext = this.CreateHttpContext(node, nodeUri);
            var rcontext = new RequestContext(httpContext, routeData);
            if(httpContext is SiteMapHttpContext) {
                (httpContext as SiteMapHttpContext).RequestContext = rcontext;
            }
            //rcontext.HttpContext.Request.RequestContext = rcontext;
            return rcontext;
        }

        public virtual RequestContext CreateRequestContext()
        {
            var httpContext = this.CreateHttpContext();
            if (httpContext.Handler is MvcHandler)
                return ((MvcHandler)httpContext.Handler).RequestContext;
#if !NET35
            else if (httpContext.Handler is Page) // Fixes #15 for interop with ASP.NET Webforms
                return new RequestContext(httpContext, ((Page)HttpContext.Current.Handler).RouteData);
#endif
            else
                return new RequestContext(httpContext, new RouteData());
        }

        public virtual RequestContext CreateRequestContext(HttpContextBase httpContext)
        {
            return new RequestContext(httpContext, new RouteData());
        }

        public virtual RequestContext CreateRequestContext(HttpContextBase httpContext, RouteData routeData)
        {
            return new RequestContext(httpContext, routeData);
        }

        public virtual ControllerContext CreateControllerContext(RequestContext requestContext, ControllerBase controller)
        {
            if (requestContext == null)
                throw new ArgumentNullException("requestContext");
            if (controller == null)
                throw new ArgumentNullException("controller");

            var result = new ControllerContext(requestContext, controller);

            // Fixes #271 - set controller's ControllerContext property for MVC
            result.Controller.ControllerContext = result;

            return result;
        }

        public virtual IRequestCache GetRequestCache()
        {
            return new RequestCache(this);
        }

        public virtual RouteCollection GetRoutes()
        {
            return RouteTable.Routes;
        }

        public virtual IUrlHelper CreateUrlHelper(RequestContext requestContext)
        {
            return new UrlHelperAdapter(requestContext, this.GetRoutes());
        }

        public virtual IUrlHelper CreateUrlHelper()
        {
            var requestContext = this.CreateRequestContext();
            return new UrlHelperAdapter(requestContext, this.GetRoutes());
        }

        public virtual AuthorizationContext CreateAuthorizationContext(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            if (controllerContext == null)
                throw new ArgumentNullException("controllerContext");
            if (actionDescriptor == null)
                throw new ArgumentNullException("actionDescriptor");

            return new AuthorizationContext(controllerContext, actionDescriptor);
        }

        #endregion
    }
}
