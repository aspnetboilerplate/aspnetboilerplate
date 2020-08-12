namespace Abp.MultiTenancy
{
    public interface ITenantResolveContributor
    {
        long? ResolveTenantId();
    }
    public interface IBranchResolveContributor
    {
        long? ResolveBranchId();
    }
}