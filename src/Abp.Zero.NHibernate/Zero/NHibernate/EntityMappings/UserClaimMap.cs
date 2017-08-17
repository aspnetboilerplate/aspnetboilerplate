using Abp.Authorization.Users;
using Abp.NHibernate.EntityMappings;

namespace Abp.Zero.NHibernate.EntityMappings
{
    public class UserClaimMap : EntityMap<UserClaim, long>
    {
        public UserClaimMap()
            : base("AbpUserClaims")
        {
            Map(x => x.TenantId);
            Map(x => x.UserId);
            Map(x => x.ClaimType);
            Map(x => x.ClaimValue);

            this.MapCreationAudited();
        }
    }
}