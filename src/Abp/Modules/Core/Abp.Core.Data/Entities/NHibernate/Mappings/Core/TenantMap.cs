using Abp.Entities.Core;

namespace Abp.Entities.NHibernate.Mappings.Core
{
    public class TenantMap : EntityMap<Tenant>
    {
        public TenantMap()
            : base("AbpTenants")
        {
            Map(x => x.CompanyName);
            References(x => x.Owner).Column("OwnerUserId");
        }
    }
}