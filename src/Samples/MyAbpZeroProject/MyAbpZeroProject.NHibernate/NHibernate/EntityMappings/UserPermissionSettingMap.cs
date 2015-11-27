using Abp.Authorization.Users;

namespace MyAbpZeroProject.NHibernate.EntityMappings
{
    public class UserPermissionSettingMap : PermissionSettingMap<UserPermissionSetting>
    {
        public UserPermissionSettingMap()
        {
            Map(x => x.UserId);
        }
    }
}