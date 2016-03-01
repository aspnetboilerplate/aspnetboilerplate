using System.Web;
using System.Web.Routing;

namespace Adorable.Web.Mvc.Resources.Embedded.Handlers
{
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