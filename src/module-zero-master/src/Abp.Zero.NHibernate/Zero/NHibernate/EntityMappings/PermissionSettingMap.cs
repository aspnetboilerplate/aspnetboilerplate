using Abp.Authorization;
using Abp.NHibernate.EntityMappings;

namespace Abp.Zero.NHibernate.EntityMappings
{
    public abstract class PermissionSettingMap<TPermissionSetting> : EntityMap<TPermissionSetting, long>
        where TPermissionSetting : PermissionSetting
    {
        protected PermissionSettingMap()
            : base("AbpPermissions")
        {
            Map(x => x.Name);
            Map(x => x.IsGranted);
            this.MapCreationAudited();
        }
    }
}