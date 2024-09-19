using Abp.EntityHistory;
using Abp.Events.Bus.Entities;
using Abp.NHibernate.EntityMappings;

namespace Abp.Zero.NHibernate.EntityMappings;

public class EntityChangeMap : EntityMap<EntityChange, long>
{
    public EntityChangeMap() : base("AbpEntityChanges")
    {
        Map(x => x.ChangeTime)
            .Not.Nullable();
        Map(x => x.ChangeType)
            .CustomType<EntityChangeType>()
            .Not.Nullable();
        Map(x => x.EntityChangeSetId)
            .Not.Nullable();
        Map(x => x.EntityId)
            .Length(EntityChange.MaxEntityIdLength);
        Map(x => x.EntityTypeFullName)
            .Length(EntityChange.MaxEntityTypeFullNameLength);
        Map(x => x.TenantId);
    }
}