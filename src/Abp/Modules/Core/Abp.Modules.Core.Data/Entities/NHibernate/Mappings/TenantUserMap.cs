using Abp.Modules.Core.Domain.Entities;

namespace Abp.Modules.Core.Entities.NHibernate.Mappings
{
    public class TenantUserMap : EntityMap<TenantUser>
    {
        public TenantUserMap()
            : base("AbpTenantUsers")
        {
            References(x => x.User).Column("UserId").LazyLoad();
            Map(x => x.MembershipStatus).CustomType<TenantMembershipStatus>();
            References(x => x.ApprovedUser).Column("ApprovedUserId").Nullable().LazyLoad();
        }
    }
}