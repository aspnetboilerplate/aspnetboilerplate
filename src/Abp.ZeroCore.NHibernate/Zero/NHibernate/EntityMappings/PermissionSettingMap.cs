using Abp.Authorization;
using Abp.NHibernate.EntityMappings;

namespace Abp.Zero.NHibernate.EntityMappings;

public class PermissionSettingMap : EntityMap<PermissionSetting, long>
{
    public PermissionSettingMap()
        : base("AbpPermissions")
    {
        DiscriminateSubClassesOnColumn("Discriminator")
            .Length(Extensions.NvarcharMax)
            .Not.Nullable();

        Map(x => x.Name)
            .Length(PermissionSetting.MaxNameLength)
            .Not.Nullable();
        Map(x => x.IsGranted)
            .Not.Nullable();
        Map(x => x.TenantId);

        this.MapCreationAudited();
    }
}