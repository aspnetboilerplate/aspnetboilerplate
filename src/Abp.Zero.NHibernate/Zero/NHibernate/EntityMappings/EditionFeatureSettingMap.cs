using Abp.Application.Features;
using FluentNHibernate.Mapping;

namespace Abp.Zero.NHibernate.EntityMappings
{
    public class EditionFeatureSettingMap : SubclassMap<EditionFeatureSetting>
    {
        public EditionFeatureSettingMap()
        {
            DiscriminatorValue("EditionFeatureSetting");

            References(x => x.Edition).Column("EditionId").Not.Nullable();
        }
    }
}