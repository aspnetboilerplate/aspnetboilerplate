using System;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Abp.Dependency;
using Abp.Resources.Embedded;
using Abp.Web.Mvc.Resources.Embedded.Handlers;

namespace Abp.Web.Mvc.Resources
{
    /// <summary>
    /// A helper class to simplify expose web resources.
    /// </summary>
    public static class WebResourceHelper
    {
        private static IEmbeddedResourceManager EmbeddedResourceManager { get { return LazyEmbeddedResourceManager.Value; } }
        private static readonly Lazy<IEmbeddedResourceManager> LazyEmbeddedResourceManager;

        static WebResourceHelper()
        {
            LazyEmbeddedResourceManager = new Lazy<IEmbeddedResourceManager>(IocManager.Instance.Resolve<IEmbeddedResourceManager>, true);
        }

        /// <summary>
        /// Exposes one or more embedded resources to web clients.
        /// It can be used to embed javascript/css files into assemblies and use them in html pages easily.
        /// </summary>
        /// <param name="rootPath">
        /// Root path of the resource. Can include '/' for deeper paths.
        /// </param>
        /// <param name="assembly">The assembly contains resources</param>
        /// <param name="resourceNamespace">Root namespace of the resources</param>
        public static void ExposeEmbeddedResources(string rootPath, Assembly assembly, string resourceNamespace)
        {
            EmbeddedResourceManager.ExposeResources(rootPath, assembly, resourceNamespace);

            /* @hikalkan: Default values are a workaround. If it's not set, @Url.Action in views can not run
             * properly and use this route accidently.
             * We should find a better way of serving embedded resources in the future, but this works as I tested.
             */
            RouteTable.Routes.MapRoute(
                name: "EmbeddedResource:" + rootPath,
                url: rootPath + "/{*pathInfo}", //TODO: Define extension?
                defaults: new
                {
                    controller = "AbpNoController",
                    action = "AbpNoAction"
                }
                ).RouteHandler = new EmbeddedResourceRouteHandler(rootPath);
        }

        /// <summary>
        /// Gets an embedded resource file.
        /// </summary>
        /// <param name="fullResourcePath">Full path of the resource</param>
        /// <returns>Embedded resource file</returns>
        public static EmbeddedResourceInfo GetEmbeddedResource(string fullResourcePath)
        {
            return EmbeddedResourceManager.GetResource(fullResourcePath);
        }
    }
}
