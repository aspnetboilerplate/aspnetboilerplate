namespace Abp.MultiTenancy
{
    public class TenantResolverCacheItem
    {
        public long? TenantId { get; }

        public TenantResolverCacheItem(long? tenantId)
        {
            TenantId = tenantId;
        }
    }
    public class BranchResolverCacheItem
    {
        public long? BranchId { get; }

        public BranchResolverCacheItem(long? branchId)
        {
            BranchId = branchId;
        }
    }
}