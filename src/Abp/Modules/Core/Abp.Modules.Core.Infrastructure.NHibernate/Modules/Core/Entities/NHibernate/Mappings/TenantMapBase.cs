using Abp.Domain.Entities.Mapping;
using Abp.Security.Tenants;

namespace Abp.Modules.Core.Entities.NHibernate.Mappings
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