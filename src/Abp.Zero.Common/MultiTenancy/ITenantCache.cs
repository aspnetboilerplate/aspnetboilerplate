using System.Threading.Tasks;

namespace Abp.MultiTenancy
{
    public interface ITenantCache
    {
        TenantCacheItem Get(int tenantId);

        TenantCacheItem Get(string tenancyName);

        TenantCacheItem GetOrNull(string tenancyName);

        TenantCacheItem GetOrNull(int tenantId);

        Task<TenantCacheItem> GetAsync(int tenantId);

        Task<TenantCacheItem> GetAsync(string tenancyName);

        Task<TenantCacheItem> GetOrNullAsync(string tenancyName);

        Task<TenantCacheItem> GetOrNullAsync(int tenantId);
    }
}
