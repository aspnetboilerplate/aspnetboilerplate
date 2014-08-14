using System.Collections.Concurrent;
using System.Reflection;
using System.Web.Routing;
using Abp.Dependency;
using Abp.Web.Resources.Embedded.Handlers;

namespace Abp.Web.Resources.Embedded
{
    /// <summary>
    /// 
    /// </summary>
    public class EmbeddedResourceManager : IEmbeddedResourceManager, ISingletonDependency
    {
        private readonly ConcurrentDictionary<string, EmbeddedResourceInfo> _resources;

        /// <summary>
        /// Constructor.
        /// </summary>
        public EmbeddedResourceManager()
        {
            _resources = new ConcurrentDictionary<string, EmbeddedResourceInfo>();
        }

        public void Expose(string resourceName, Assembly assembly, string resourceNamespace)
        {
            if (_resources.ContainsKey(resourceName))
            {
                throw new AbpException("There is already an embedded resource with given resourceName: " + resourceName);
            }

            _resources[resourceName] = new EmbeddedResourceInfo(assembly, resourceNamespace);
        }

        public void Initialize()
        {
            RouteTable.Routes.Insert(0,
                new Route(
                    "AbpRes/{resourceName}/{*pathInfo}", //TODO@Halil: We're here, working on route!
                    new RouteValueDictionary(new {}),
                    new RouteValueDictionary(new {}),
                    new RouteValueDictionary(new {}),
                    new UiResourceRouteHandler()
                    ));
        }
    }
}