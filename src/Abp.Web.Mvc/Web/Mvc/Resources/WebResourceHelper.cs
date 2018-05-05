using System;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Abp.Dependency;
using Abp.Extensions;
using Abp.Resources.Embedded;
using Abp.Web.Mvc.Resources.Embedded.Handlers;

namespace Abp.Web.Mvc.Resources
{
    /// <summary>
    /// A helper class to simplify expose web resources.
    /// </summary>
    [Obsolete("No need to use WebResourceHelper anymore. Use Configuration.EmbeddedResources.Sources.Add(...) in PreInitialize of your module.")]
    public static class WebResourceHelper
    {
        /// <summary>
        /// Exposes one or more embedded resources to web clients.
        /// It can be used to embed JavaScript/CSS files into assemblies and use them in html pages easily.
        /// </summary>
        /// <param name="rootPath">
        /// Root path of the resource. Can include '/' for deeper paths.
        /// </param>
        /// <param name="assembly">The assembly contains resources</param>
        /// <param name="resourceNamespace">Root namespace of the resources</param>
        public static void ExposeEmbeddedResources(string rootPath, Assembly assembly, string resourceNamespace)
        {
            SingletonDependency<IEmbeddedResourcesConfiguration>.Instance.Sources.Add(
                new EmbeddedResourceSet(rootPath, assembly, resourceNamespace)
            );

            /* @hikalkan: Default values are a workaround. If it's not set, @Url.Action in views can not run
             * properly and use this route accidently.
             * We should find a better way of serving embedded resources in the future, but this works as I tested.
             */
            RouteTable.Routes.MapRoute(
                name: "EmbeddedResource:" + rootPath,
                url: rootPath.EnsureEndsWith('/') + "{*pathInfo}",
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
        public static EmbeddedResourceItem GetEmbeddedResource(string fullResourcePath)
        {
            return SingletonDependency<IEmbeddedResourceManager>.Instance.GetResource(fullResourcePath);
        }
    }
}
