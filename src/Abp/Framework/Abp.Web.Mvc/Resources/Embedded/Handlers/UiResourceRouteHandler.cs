using System.Web;
using System.Web.Routing;

namespace Abp.Web.Mvc.Resources.Embedded.Handlers
{
    internal class UiResourceRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new EmbeddedResourceHttpHandler(requestContext.RouteData);
        }
    }
}