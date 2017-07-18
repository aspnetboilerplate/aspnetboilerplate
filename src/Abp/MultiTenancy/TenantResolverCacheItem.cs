namespace Abp.MultiTenancy
{
    public class TenantResolverCacheItem
    {
        public int? TenantId { get; }

        public TenantResolverCacheItem(int? tenantId)
        {
            TenantId = tenantId;
        }
    }
}