using System.Web;
using Abp.Dependency;
using Abp.MultiTenancy;

namespace Abp.Web.MultiTenancy
{
    public class HttpContextTenantResolverCache : ITenantResolverCache, ITransientDependency
    {
        private const string CacheItemKey = "Abp.MultiTenancy.TenantResolverCacheItem";

        public TenantResolverCacheItem Value
        {
            get
            {
                return HttpContext.Current?.Items[CacheItemKey] as TenantResolverCacheItem;
            }

            set
            {
                var httpContext = HttpContext.Current;
                if (httpContext == null)
                {
                    return;
                }

                httpContext.Items[CacheItemKey] = value;
            }
        }
    }
}
