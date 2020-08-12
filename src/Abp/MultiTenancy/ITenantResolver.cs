namespace Abp.MultiTenancy
{
    public interface ITenantResolver
    {
        long? ResolveTenantId();
    }

    public interface IBranchResolver
    {
        long? ResolveBranchId();
    }
}