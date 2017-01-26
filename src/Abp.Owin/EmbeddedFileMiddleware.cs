using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using Abp.Dependency;
using Abp.Extensions;
using Abp.Resources.Embedded;
using Microsoft.Owin;
using Owin;

namespace Abp.Owin
{
    internal static class EmbeddedFileMiddleware
    {
        public static void UseEmbeddedFiles(this IAppBuilder app)
        {
            app.Use(async (context, next) =>
            {
                await HandleEmbeddedFileRequests(next, context);
            });
        }

        public static async Task HandleEmbeddedFileRequests(Func<Task> next, IOwinContext context)
        {
            var httpContext = HttpContext.Current;
            if (httpContext == null)
            {
                await next();
                return;
            }

            if (!context.Request.Path.HasValue || context.Request.Path.Value.IsNullOrEmpty())
            {
                await next();
                return;
            }

            var path = context.Request.Path.Value;

            var resource = SingletonDependency<IEmbeddedResourceManager>.Instance.GetResource(path);
            if (resource != null)
            {
                if (!File.Exists(httpContext.Server.MapPath(path.EnsureStartsWith('~'))))
                {
                    context.Response.ContentLength = resource.Content.Length;
                    context.Response.ContentType = MimeMapping.GetMimeMapping(resource.FileName);
                    //TODO: Other headers

                    await context.Response.WriteAsync(resource.Content);
                    return;
                }
            }

            await next();
        }
    }
}