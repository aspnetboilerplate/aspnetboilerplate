using Abp.Runtime.Caching;

namespace Abp.MultiTenancy
{
    public static class TenantCacheManagerExtensions
    {
        public static ITypedCache<long, TenantCacheItem> GetTenantCache(this ICacheManager cacheManager)
        {
            return cacheManager.GetCache<long, TenantCacheItem>(TenantCacheItem.CacheName);
        }

        public static ITypedCache<string, long?> GetTenantByNameCache(this ICacheManager cacheManager)
        {
            return cacheManager.GetCache<string, long?>(TenantCacheItem.ByNameCacheName);
        }
    }
}