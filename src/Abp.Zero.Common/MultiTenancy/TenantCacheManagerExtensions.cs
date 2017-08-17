using Abp.Runtime.Caching;

namespace Abp.MultiTenancy
{
    public static class TenantCacheManagerExtensions
    {
        public static ITypedCache<int, TenantCacheItem> GetTenantCache(this ICacheManager cacheManager)
        {
            return cacheManager.GetCache<int, TenantCacheItem>(TenantCacheItem.CacheName);
        }

        public static ITypedCache<string, int?> GetTenantByNameCache(this ICacheManager cacheManager)
        {
            return cacheManager.GetCache<string, int?>(TenantCacheItem.ByNameCacheName);
        }
    }
}