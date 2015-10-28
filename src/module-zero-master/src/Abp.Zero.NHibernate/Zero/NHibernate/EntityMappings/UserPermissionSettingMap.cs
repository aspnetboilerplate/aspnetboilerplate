using Abp.Authorization.Users;

namespace Abp.Zero.NHibernate.EntityMappings
{
    public class UserPermissionSettingMap : PermissionSettingMap<UserPermissionSetting>
    {
        public UserPermissionSettingMap()
        {
            Map(x => x.UserId);
        }
    }
}