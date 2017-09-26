using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using Abp.Extensions;

namespace Abp.Web.Mvc.Resources.Embedded.Handlers
{
    [Obsolete]
    internal class EmbeddedResourceHttpHandler : IHttpHandler
    {
        private static readonly HashSet<string> IgnoredFileExtensions = new HashSet<string>
        {
            ".cshtml",
            ".config"
        };

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
                context.Response.StatusCode = 404;
                return;
            }

            var resource = WebResourceHelper.GetEmbeddedResource(_rootPath.EnsureEndsWith('/') + fileName);
            if (resource == null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            if (resource.FileExtension != null && IgnoredFileExtensions.Contains(resource.FileExtension.EnsureStartsWith('.')))
            {
                context.Response.StatusCode = 404;
                return;
            }
            
            context.Response.ContentType = MimeMapping.GetMimeMapping(fileName);
            context.Response.OutputStream.Write(resource.Content, 0, resource.Content.Length);
        }

        public bool IsReusable => false;
    }
}