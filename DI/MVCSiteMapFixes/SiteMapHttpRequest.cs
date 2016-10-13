using MvcSiteMapProvider;
using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcTestApplication.DI.MVCSiteMapFixes
{
    public class BaseSiteMapHttpRequest
        : HttpRequestWrapper {
        private readonly RequestContext requestContext;
        /// <summary>
        /// Initializes a new instance of the <see cref="SiteMapHttpRequest"/> class.
        /// </summary>
        /// <param name="httpRequest">The object that this wrapper class provides access to.</param>
        /// <param name="node">The site map node to fake node access context for or <c>null</c>.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="httpRequest"/> is null.
        /// </exception>
        public BaseSiteMapHttpRequest(HttpRequest httpRequest, RequestContext requestContext)
            : base(httpRequest) {
            this.requestContext = requestContext;
        }

        /// <summary>
        /// Gets the virtual path of the application root and makes it relative by using the tilde (~) notation for the application root (as in "~/page.aspx").
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The virtual path of the application root for the current request with the tilde operator added.
        /// </returns>
        public override string AppRelativeCurrentExecutionFilePath {
            get {
                return VirtualPathUtility.ToAppRelative(this.CurrentExecutionFilePath);
            }
        }

        /// <summary>
        /// Gets the virtual path of the current request.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The virtual path of the page handler that is currently executing.
        /// </returns>
        public override string CurrentExecutionFilePath {
            get { return base.FilePath; }
        }
        public override System.Web.Routing.RequestContext RequestContext {
            get {
                return this.requestContext ?? base.RequestContext;
            }
        }
    }

    /// <summary>
    /// HttpRequest wrapper.
    /// </summary>
    public class SiteMapHttpRequest
        : BaseSiteMapHttpRequest 
    {
        private readonly ISiteMapNode node;
        //private readonly RequestContext requestContext;
        private readonly HttpRequest currentRequest;
        /// <summary>
        /// Initializes a new instance of the <see cref="SiteMapHttpRequest"/> class.
        /// </summary>
        /// <param name="httpRequest">The object that this wrapper class provides access to.</param>
        /// <param name="node">The site map node to fake node access context for or <c>null</c>.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="httpRequest"/> is null.
        /// </exception>
        public SiteMapHttpRequest(HttpRequest httpRequest, ISiteMapNode node, RequestContext requestContext, Uri nodeUri)
            : base(httpRequest, requestContext)
        {
            this.node = node;
            this.currentRequest = new HttpRequest(
                filename: string.Empty,
                url: nodeUri.ToString(),
                queryString: string.IsNullOrEmpty(nodeUri.Query) ? string.Empty : nodeUri.Query.Substring(1));
        }

        /// <summary>
        /// Gets the HTTP data-transfer method (such as GET, POST, or HEAD) that was used by the client.
        /// </summary>
        /// <returns>
        /// The HTTP data-transfer method that was used by the client.
        /// </returns>
        public override string HttpMethod
        {
            get
            {
                bool useRequest = this.node == null || 
                    string.Equals(this.node.HttpMethod, "*") || 
                    string.Equals(this.node.HttpMethod, "request", StringComparison.OrdinalIgnoreCase);
                if (!useRequest)
                {
                    return string.IsNullOrEmpty(this.node.HttpMethod)
                        ? HttpVerbs.Get.ToString().ToUpperInvariant()
                        : this.node.HttpMethod.ToUpperInvariant();
                }
                return base.HttpMethod;
            }
        }
        public override System.Collections.Specialized.NameValueCollection QueryString {
            get {
                return currentRequest.QueryString;
            }
        }
        public override System.Collections.Specialized.NameValueCollection Form {
            get {
                return new System.Collections.Specialized.NameValueCollection();// empty
            }
        }
        /*public override HttpCookieCollection Cookies {
            get {
                return base.Cookies;
            }
        }*/
        //Cookie and ServerVariables use from real httpContext
        public override string this[string key] {
            get {
                return Params[key];
            }
        }
        private NameValueCollection _params;
        private void FillInParamsCollection() {
            _params = new NameValueCollection();
            _params.Add(currentRequest.QueryString);
            //_params.Add(this.Form);
            Add(_params, this.Cookies);
            _params.Add(this.ServerVariables);
        }

        private void Add(NameValueCollection collection,  HttpCookieCollection c) {
            int n = c.Count;

            for(int i = 0; i < n; i++) {
                HttpCookie cookie = c.Get(i);
                collection.Add(cookie.Name, cookie.Value);
            }
        } 
 
        public override System.Collections.Specialized.NameValueCollection Params {
            get {
                if(_params == null) {
                    FillInParamsCollection();
                }
                return _params;
            }
        }
        public override Uri Url {
            get {
                return currentRequest.Url;
            }
        }
    }
}