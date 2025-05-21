using Abp.Authorization.Roles;
using Abp.NHibernate.EntityMappings;

namespace Abp.Zero.NHibernate.EntityMappings;

public class RoleClaimMap : EntityMap<RoleClaim, long>
{
    public RoleClaimMap() : base("AbpRoleClaims")
    {
        Map(x => x.ClaimType)
            .Length(RoleClaim.MaxClaimTypeLength);
        Map(x => x.ClaimValue)
            .Length(Extensions.NvarcharMax);
        Map(x => x.RoleId)
            .Not.Nullable();
        Map(x => x.TenantId);

        this.MapCreationAudited();
    }
}