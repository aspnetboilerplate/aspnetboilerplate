using Abp.Authorization.Roles;
using Abp.NHibernate.EntityMappings;

namespace Abp.Zero.NHibernate.EntityMappings
{
    public class RoleOrganizationUnitMap : EntityMap<RoleOrganizationUnit, long>
    {
        public RoleOrganizationUnitMap()
            : base("AbpRoleOrganizationUnits")
        {
            Map(x => x.TenantId);
            Map(x => x.RoleId);
            Map(x => x.OrganizationUnitId);

            this.MapCreationAudited();
        }
    }
}