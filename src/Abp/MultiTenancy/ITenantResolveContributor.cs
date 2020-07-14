namespace Abp.MultiTenancy
{
    public interface ITenantResolveContributor
    {
        int? ResolveTenantId();
    }
    public interface IBranchResolveContributor
    {
        long? ResolveBranchId();
    }
}