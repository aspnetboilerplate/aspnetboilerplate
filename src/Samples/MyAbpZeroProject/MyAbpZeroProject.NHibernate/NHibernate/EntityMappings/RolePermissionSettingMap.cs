using Abp.Authorization.Roles;

namespace MyAbpZeroProject.NHibernate.EntityMappings
{
    public class RolePermissionSettingMap : PermissionSettingMap<RolePermissionSetting>
    {
        public RolePermissionSettingMap()
        {
            Map(x => x.RoleId);
        }
    }
}