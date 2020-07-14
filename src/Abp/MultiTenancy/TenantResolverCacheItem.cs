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
    public class BranchResolverCacheItem
    {
        public long? BranchId { get; }

        public BranchResolverCacheItem(long? branchId)
        {
            BranchId = branchId;
        }
    }
}