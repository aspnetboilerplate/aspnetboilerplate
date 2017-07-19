using System.Web.Caching;
using Abp.Resources.Embedded;

namespace Abp.Web.Mvc.Resources.Embedded
{
    public class EmbeddedResourceItemCacheDependency : CacheDependency
    {
        public EmbeddedResourceItemCacheDependency(EmbeddedResourceItem resource)
        {
            SetUtcLastModified(resource.LastModifiedUtc);
        }
    }
}