using Abp.EntityHistory;
using Abp.NHibernate.EntityMappings;

namespace Abp.Zero.NHibernate.EntityMappings;

public class EntityPropertyChangeMap : EntityMap<EntityPropertyChange, long>
{
    public EntityPropertyChangeMap() : base("AbpEntityPropertyChanges")
    {
        Map(x => x.EntityChangeId)
            .Not.Nullable();
        Map(x => x.NewValue)
            .Length(EntityPropertyChange.MaxValueLength);
        Map(x => x.OriginalValue)
            .Length(EntityPropertyChange.MaxValueLength);
        Map(x => x.PropertyName)
            .Length(EntityPropertyChange.MaxPropertyNameLength);
        Map(x => x.PropertyTypeFullName)
            .Length(EntityPropertyChange.MaxPropertyTypeFullNameLength);
        Map(x => x.TenantId);
        Map(x => x.NewValueHash)
            .Length(Extensions.NvarcharMax);
        Map(x => x.OriginalValueHash)
            .Length(Extensions.NvarcharMax);
    }
}