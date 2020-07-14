namespace Abp.MultiTenancy
{
    public class NullTenantResolverCache : ITenantResolverCache
    {
        public TenantResolverCacheItem Value
        {
            get { return null; }
            set {  }
        }
    }

    public class NullBranchResolverCache : IBranchResolverCache
    {
        public BranchResolverCacheItem Value
        {
            get { return null; }
            set { }
        }
    }
}