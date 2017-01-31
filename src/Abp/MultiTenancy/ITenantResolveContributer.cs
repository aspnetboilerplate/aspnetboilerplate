namespace Abp.MultiTenancy
{
    public interface ITenantResolveContributer
    {
        int? ResolveTenantId();
    }
}