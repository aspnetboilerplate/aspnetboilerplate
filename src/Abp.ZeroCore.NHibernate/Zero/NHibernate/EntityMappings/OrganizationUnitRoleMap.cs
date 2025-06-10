using Abp.NHibernate.EntityMappings;
using Abp.Organizations;

namespace Abp.Zero.NHibernate.EntityMappings;

public class OrganizationUnitRoleMap : EntityMap<OrganizationUnitRole, long>
{
    public OrganizationUnitRoleMap()
        : base("AbpOrganizationUnitRoles")
    {
        Map(x => x.TenantId);
        Map(x => x.RoleId)
            .Not.Nullable();
        Map(x => x.OrganizationUnitId)
            .Not.Nullable();

        this.MapCreationAudited();
        this.MapIsDeleted();
    }
}