namespace Abp.MultiTenancy
{
    public interface ITenantResolver
    {
        int? ResolveTenantId();
    }
}