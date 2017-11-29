using System;
using System.Collections.Generic;
using Abp.Collections.Extensions;
using Abp.Dependency;

namespace Abp.Resources.Embedded
{
    public class EmbeddedResourceManager : IEmbeddedResourceManager, ISingletonDependency
    {
        private readonly IEmbeddedResourcesConfiguration _configuration;
        private readonly Lazy<Dictionary<string, EmbeddedResourceItem>> _resources;

        /// <summary>
        /// Constructor.
        /// </summary>
        public EmbeddedResourceManager(IEmbeddedResourcesConfiguration configuration)
        {
            _configuration = configuration;
            _resources = new Lazy<Dictionary<string, EmbeddedResourceItem>>(
                CreateResourcesDictionary,
                true
            );
        }

        /// <inheritdoc/>
        public EmbeddedResourceItem GetResource(string fullPath)
        {
            var resource = _resources.Value.GetOrDefault(EmbeddedResourcePathHelper.NormalizePath(fullPath));
            if (resource != null)
            {
                return new EmbeddedResourceItem(System.IO.Path.GetFileName(fullPath), resource.Content, resource.Assembly);
            }
            return null;
        }

        private Dictionary<string, EmbeddedResourceItem> CreateResourcesDictionary()
        {
            var resources = new Dictionary<string, EmbeddedResourceItem>(StringComparer.OrdinalIgnoreCase);

            foreach (var source in _configuration.Sources)
            {
                source.AddResources(resources);
            }
            return resources;
        }

        class EmbeddedResourceItemComparer : IEqualityComparer<string>
        {
            public bool Equals(string fullPath1, string fullPath2)
            {
                return InvariantResourceName(fullPath1).Equals(InvariantResourceName(fullPath2));
            }

            public int GetHashCode(string fullPath)
            {
                return InvariantResourceName(fullPath).GetHashCode();
            }
            private string InvariantResourceName(string fullPath)
            {
                return fullPath.Replace("/", ".");
            }

        }
    }
}