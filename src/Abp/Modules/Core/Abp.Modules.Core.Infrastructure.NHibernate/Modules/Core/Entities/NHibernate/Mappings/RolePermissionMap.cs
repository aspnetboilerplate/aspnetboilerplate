using Abp.Domain.Entities.Mapping;
using Abp.Security.Roles;

namespace Abp.Modules.Core.Entities.NHibernate.Mappings
{
    public class RolePermissionMap : EntityMap<RolePermission>
    {
        public RolePermissionMap()
            : base("AbpRolePermissions")
        {
            Map(x => x.RoleId);
            Map(x => x.PermissionName);
            Map(x => x.IsGranted);
            this.MapCreationAudited();
        }
    }
}