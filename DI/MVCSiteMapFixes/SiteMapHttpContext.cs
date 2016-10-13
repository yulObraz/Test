using MvcSiteMapProvider;
using MvcSiteMapProvider.Web.Mvc;
using System;
using System.Web;

namespace MvcTestApplication.DI.MVCSiteMapFixes
{
    /// <summary>
    /// HttpContext wrapper.
    /// </summary>
    public class SiteMapHttpContext 
        : HttpContextWrapper
    {
        private readonly HttpContext httpContext;
        private readonly ISiteMapNode node;
        private readonly Uri nodeUri;

        /// <summary>
        /// Initializes a new instance of the <see cref="SiteMapHttpContext"/> class.
        /// </summary>
        /// <param name="httpContext">The object that this wrapper class provides access to.</param>
        /// <param name="node">The site map node to fake node access context for or <c>null</c>.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="httpContext"/> is null.
        /// </exception>
        public SiteMapHttpContext(HttpContext httpContext, ISiteMapNode node, Uri uri)
            : base(httpContext)
        {
            this.httpContext = httpContext;
            this.node = node;
            if(node != null) {
                nodeUri = uri ?? new Uri(HttpContext.Current.Request.Url, node.Url);
            }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Web.HttpRequestBase"/> object for the current HTTP request.
        /// </summary>
        /// <returns>The current HTTP request.</returns>
        public override HttpRequestBase Request
        {
            get {
                if(node == null) {
                    return new BaseSiteMapHttpRequest(this.httpContext.Request, RequestContext);
                } else {
                    return new SiteMapHttpRequest(this.httpContext.Request, this.node, RequestContext, nodeUri);
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="System.Web.HttpResponseBase"/> object for the current HTTP response.
        /// </summary>
        /// <returns>The current HTTP response.</returns>
        public override HttpResponseBase Response
        {
            get { return new SiteMapHttpResponse(this.httpContext.Response); }
        }

        public System.Web.Routing.RequestContext RequestContext { get; set; }
    }
}