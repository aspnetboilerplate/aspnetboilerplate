using Abp.Domain.Entities.Mapping;
using Abp.MultiTenancy;

namespace Abp.Zero.EntityMappings
{
    public abstract class TenantMapBase<TTenant> : EntityMap<TTenant> where TTenant : AbpTenant
    {
        protected TenantMapBase()
            : base("AbpTenants")
        {
            Map(x => x.Name);
            Map(x => x.CreationTime);

            Polymorphism.Explicit();
        }
    }
}