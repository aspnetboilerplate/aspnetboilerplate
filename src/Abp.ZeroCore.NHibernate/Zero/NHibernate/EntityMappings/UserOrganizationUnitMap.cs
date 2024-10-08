using Abp.Authorization.Users;
using Abp.NHibernate.EntityMappings;

namespace Abp.Zero.NHibernate.EntityMappings;

public class UserOrganizationUnitMap : EntityMap<UserOrganizationUnit, long>
{
    public UserOrganizationUnitMap()
        : base("AbpUserOrganizationUnits")
    {
        Map(x => x.TenantId);
        Map(x => x.UserId)
            .Not.Nullable();
        Map(x => x.OrganizationUnitId)
            .Not.Nullable();

        this.MapCreationAudited();
        this.MapIsDeleted();
    }
}