using Abp.Authorization.Roles;

namespace Abp.Zero.NHibernate.EntityMappings
{
    public class RolePermissionSettingMap : PermissionSettingMap<RolePermissionSetting>
    {
        public RolePermissionSettingMap()
        {
            Map(x => x.RoleId);
        }
    }
}