using Abp.MultiTenancy;

namespace Abp.Zero.EntityFrameworkCore
{
    public interface IMultiTenantSeed
    {
        AbpTenantBase Tenant { get; set; }
    }
}