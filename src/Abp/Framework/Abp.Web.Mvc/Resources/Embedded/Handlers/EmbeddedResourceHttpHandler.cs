using System.IO;
using System.Web;
using System.Web.Routing;

namespace Abp.Web.Mvc.Resources.Embedded.Handlers
{
    internal class EmbeddedResourceHttpHandler : IHttpHandler
    {
        private readonly RouteData _routeData;

        public EmbeddedResourceHttpHandler(RouteData routeData)
        {
            _routeData = routeData;
        }

        public void ProcessRequest(HttpContext context)
        {
            var fileName = _routeData.Values["pathInfo"] as string;
            if (fileName == null)
            {
                return;
            }

            string nameSpace = GetType().Assembly.GetName().Name;
            string manifestResourceName = string.Format("{0}.{1}", nameSpace, fileName.Replace("/", "."));

            using (Stream stream = GetType().Assembly.GetManifestResourceStream(manifestResourceName))
            {
                if (stream == null)
                {
                    return;
                }

                context.Response.Clear();
                if (fileName.EndsWith(".js"))
                    context.Response.ContentType = "text/javascript";
                else if (fileName.EndsWith(".css"))
                    context.Response.ContentType = "text/css";

                stream.CopyTo(context.Response.OutputStream);
            }
        }

        public bool IsReusable { get { return false; } }
    }
}