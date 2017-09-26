using Abp.Authorization.Roles;
using FluentNHibernate.Mapping;

namespace Abp.Zero.NHibernate.EntityMappings
{
    public class RolePermissionSettingMap : SubclassMap<RolePermissionSetting>
    {
        public RolePermissionSettingMap()
        {
            DiscriminatorValue("RolePermissionSetting");

            Map(x => x.RoleId).Not.Nullable();
        }
    }
}