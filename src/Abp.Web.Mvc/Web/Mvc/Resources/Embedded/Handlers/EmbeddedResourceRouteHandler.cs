using System;
using System.Web;
using System.Web.Routing;

namespace Abp.Web.Mvc.Resources.Embedded.Handlers
{
    [Obsolete]
    internal class EmbeddedResourceRouteHandler : IRouteHandler
    {
        private readonly string _rootPath;

        public EmbeddedResourceRouteHandler(string rootPath)
        {
            _rootPath = rootPath;
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new EmbeddedResourceHttpHandler(_rootPath, requestContext.RouteData);
        }
    }
}