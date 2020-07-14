namespace Abp.MultiTenancy
{
    public interface ITenantResolver
    {
        int? ResolveTenantId();
    }

    public interface IBranchResolver
    {
        long? ResolveBranchId();
    }
}