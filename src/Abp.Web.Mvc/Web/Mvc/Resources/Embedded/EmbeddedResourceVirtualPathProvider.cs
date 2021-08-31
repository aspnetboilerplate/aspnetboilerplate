using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
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
            if (virtualPathDependencies == null)
            {
                return null;
            }

            var foundAny = false;
            var foundAll = true;
            var dependecyFilenames = new StringCollection();

            var pathDependencies = virtualPathDependencies.Cast<object>().ToList();
            foreach (string virtualPathDependency in pathDependencies)
            {
                dependecyFilenames.Add(virtualPathDependency);
                if (GetResource(virtualPathDependency) != null || GetResources(virtualPathDependency).Any())
                {
                    foundAny = true;
                }
                else
                {
                    foundAll = false;
                }
            }

            if (foundAny)
            {
                if (!foundAll)
                {
                    throw new HttpException("Found some files in resources, but not all of them.");
                }

                return new EmbeddedResourceItemCacheDependency();
            }

            return base.GetCacheDependency(virtualPath, pathDependencies, utcStart);
        }

        public override bool DirectoryExists(string virtualDir)
        {
            var resources = GetResources(virtualDir);
            if (resources != null && resources.Any())
            {
                return true;
            }

            return base.DirectoryExists(virtualDir);
        }
        
        public override VirtualDirectory GetDirectory(string virtualDir)
        {
            var resources = GetResources(virtualDir);
            var embeddedResourceItems = resources.ToList();
            if (embeddedResourceItems.Any())
            {
                return new ComponentsEmbeddedResourceVirtualDirectory(virtualDir, embeddedResourceItems);
            }

            var result = base.GetDirectory(virtualDir);


            return result;
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
        
        private IEnumerable<EmbeddedResourceItem> GetResources(string virtualPath)
        {
            return _embeddedResourceManager.GetResources(VirtualPathUtility.ToAppRelative(virtualPath).RemovePreFix("~"));
        }
    }
}
