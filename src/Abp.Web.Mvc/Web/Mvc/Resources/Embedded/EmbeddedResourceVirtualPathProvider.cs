using System;
using System.Collections;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using Abp.Dependency;
using Abp.Extensions;
using Abp.Resources.Embedded;

namespace Abp.Web.Mvc.Resources.Embedded
{
    public class EmbeddedResourceVirtualPathProvider : VirtualPathProvider, ITransientDependency
    {
        private readonly IEmbeddedResourceManager _embeddedResourceManager;

        public EmbeddedResourceVirtualPathProvider(IEmbeddedResourceManager embeddedResourceManager)
        {
            _embeddedResourceManager = embeddedResourceManager;
        }

        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            var resource = GetResource(virtualPath);
            if (resource != null)
            {
                return new EmbeddedResourceItemCacheDependency(resource);
            }

            return base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }

        public override bool FileExists(string virtualPath)
        {
            if (base.FileExists(virtualPath))
            {
                return true;
            }

            return GetResource(virtualPath) != null;
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            if (Previous != null && base.FileExists(virtualPath))
            {
                return Previous.GetFile(virtualPath);
            }
            
            var resource = GetResource(virtualPath);
            if (resource != null)
            {
                return new EmbeddedResourceItemVirtualFile(virtualPath, resource);
            }

            return base.GetFile(virtualPath);
        }

        private EmbeddedResourceItem GetResource(string virtualPath)
        {
            return _embeddedResourceManager.GetResource(VirtualPathUtility.ToAppRelative(virtualPath).RemovePreFix("~"));
        }
    }
}