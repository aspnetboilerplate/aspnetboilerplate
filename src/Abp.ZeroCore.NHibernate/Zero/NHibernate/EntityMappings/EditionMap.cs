using Abp.Application.Editions;
using Abp.NHibernate.EntityMappings;

namespace Abp.Zero.NHibernate.EntityMappings;

public class EditionMap : EntityMap<Edition>
{
    public EditionMap()
        : base("AbpEditions")
    {
        Map(x => x.Name)
            .Length(Edition.MaxNameLength)
            .Not.Nullable();
        Map(x => x.DisplayName)
            .Length(Edition.MaxDisplayNameLength)
            .Not.Nullable();

        this.MapFullAudited();
    }
}