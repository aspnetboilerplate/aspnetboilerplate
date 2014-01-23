using Abp.Roles;

namespace Abp.Modules.Core.Entities.NHibernate.Mappings
{
    public class RolePermissionMap : EntityMap<RolePermission>
    {
        public RolePermissionMap()
            : base("AbpRolePermissions")
        {
            References(x => x.Role).Column("RoleId").Not.Nullable().Not.LazyLoad();
            Map(x => x.PermissionName);
            Map(x => x.IsGranted);
        }
    }
}