using Abp.Application.Features;
using Abp.NHibernate.EntityMappings;

namespace Abp.Zero.NHibernate.EntityMappings;

public class FeatureSettingMap : EntityMap<FeatureSetting, long>
{
    public FeatureSettingMap()
        : base("AbpFeatures")
    {
        DiscriminateSubClassesOnColumn("Discriminator")
            .Length(Extensions.NvarcharMax)
            .Not.Nullable();

        Map(x => x.TenantId);
        Map(x => x.Name)
            .Length(FeatureSetting.MaxNameLength)
            .Not.Nullable();
        Map(x => x.Value)
            .Length(FeatureSetting.MaxValueLength)
            .Not.Nullable();

        this.MapCreationAudited();
    }
}