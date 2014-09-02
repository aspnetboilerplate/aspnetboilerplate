using System.Web;
using System.Web.Routing;

namespace Abp.Web.Mvc.Resources.Embedded.Handlers
{
    internal class EmbeddedResourceHttpHandler : IHttpHandler
    {
        private readonly string _rootPath;
        private readonly RouteData _routeData;

        public EmbeddedResourceHttpHandler(string rootPath, RouteData routeData)
        {
            _rootPath = rootPath;
            _routeData = routeData;
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Clear();

            var fileName = _routeData.Values["pathInfo"] as string;
            if (fileName == null)
            {
                context.Response.StatusCode = 404; //TODO: Is this enough?
                return;
            }

            context.Response.ContentType = MimeMapping.GetMimeMapping(fileName);

            var resource = WebResourceHelper.GetEmbeddedResource(_rootPath + "/" + fileName);
            context.Response.OutputStream.Write(resource.Content, 0, resource.Content.Length);
        }

        public bool IsReusable { get { return false; } }
    }
}