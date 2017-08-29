using Abp.MultiTenancy;

namespace Abp.Zero.EntityFramework
{
    public interface IMultiTenantSeed
    {
        AbpTenantBase Tenant { get; set; }
    }
}