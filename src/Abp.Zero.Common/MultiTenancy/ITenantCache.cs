namespace Abp.MultiTenancy
{
    public interface ITenantCache
    {
        TenantCacheItem Get(long tenantId);

        TenantCacheItem Get(string tenancyName);

        TenantCacheItem GetOrNull(string tenancyName);

        TenantCacheItem GetOrNull(long tenantId);
    }
}