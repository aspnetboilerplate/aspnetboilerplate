using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Web.Resources.Embedded;

namespace Abp.Web.Resources
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
            LazyEmbeddedResourceManager = new Lazy<IEmbeddedResourceManager>(IocHelper.Resolve<IEmbeddedResourceManager>, true);
        }

        /// <summary>
        /// Exposes one or more embedded resources to web clients.
        /// It can be used to embed javascript/css files into assemblies and use them in html pages easily.
        /// </summary>
        /// <param name="resourceName">
        /// Unique name of the resource. It can include path sign (/).
        /// </param>
        /// <param name="assembly">The assembly contains resources</param>
        /// <param name="resourceNamespace">Root namespace of the resources</param>
        public static void ExposeEmbeddedResource(string resourceName, Assembly assembly, string resourceNamespace)
        {
            EmbeddedResourceManager.Expose(resourceName, assembly, resourceNamespace);
        }
    }
}
