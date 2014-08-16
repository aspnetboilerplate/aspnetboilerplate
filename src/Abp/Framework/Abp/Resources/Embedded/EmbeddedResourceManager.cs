using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using Abp.Dependency;
using Abp.Utils.Extensions.IO;

namespace Abp.Resources.Embedded
{
    /// <summary>
    /// 
    /// </summary>
    public class EmbeddedResourceManager : IEmbeddedResourceManager, ISingletonDependency
    {
        private readonly ConcurrentDictionary<string, EmbeddedResourcePathInfo> _resourcePaths; //Key: Root path of the resource
        private readonly ConcurrentDictionary<string, byte[]> _resourceCache; //Key: Root path of the resource

        /// <summary>
        /// Constructor.
        /// </summary>
        public EmbeddedResourceManager()
        {
            _resourcePaths = new ConcurrentDictionary<string, EmbeddedResourcePathInfo>();
            _resourceCache = new ConcurrentDictionary<string, byte[]>();
        }

        public void ExposeResources(string rootPath, Assembly assembly, string resourceNamespace)
        {
            if (_resourcePaths.ContainsKey(rootPath))
            {
                throw new AbpException("There is already an embedded resource with given rootPath: " + rootPath);
            }

            _resourcePaths[rootPath] = new EmbeddedResourcePathInfo(rootPath, assembly, resourceNamespace);
        }

        public byte[] GetResource(string fullPath)
        {
            //Get from cache if exists!
            if (_resourceCache.ContainsKey(fullPath))
            {
                return _resourceCache[fullPath];
            }

            var pathInfo = GetPathInfoForFullPath(fullPath);

            using (var stream = pathInfo.Assembly.GetManifestResourceStream(pathInfo.FindManifestName(fullPath)))
            {
                if (stream == null)
                {
                    throw new AbpException("There is no exposed embedded resource for " + fullPath);
                }

                return (_resourceCache[fullPath] = stream.GetAllBytes());
            }
        }

        private EmbeddedResourcePathInfo GetPathInfoForFullPath(string fullPath)
        {
            foreach (var resourcePathInfo in _resourcePaths.Values)
            {
                if (fullPath.StartsWith(resourcePathInfo.Path))
                {
                    return resourcePathInfo;
                }
            }

            throw new AbpException("There is no exposed embedded resource for: " + fullPath);
        }
    }
}