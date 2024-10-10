using Abp.EntityHistory;
using Abp.NHibernate.EntityMappings;

namespace Abp.Zero.NHibernate.EntityMappings;

public class EntityChangeSetMap : EntityMap<EntityChangeSet, long>
{
    public EntityChangeSetMap() : base("AbpEntityChangeSets")
    {
        Map(x => x.BrowserInfo)
            .Length(EntityChangeSet.MaxBrowserInfoLength);
        Map(x => x.ClientIpAddress)
            .Length(EntityChangeSet.MaxClientIpAddressLength);
        Map(x => x.ClientName)
            .Length(EntityChangeSet.MaxClientNameLength);
        this.MapCreationTime();
        Map(x => x.ExtensionData)
            .Length(Extensions.NvarcharMax);
        Map(x => x.ImpersonatorTenantId);
        Map(x => x.ImpersonatorUserId);
        Map(x => x.Reason)
            .Length(EntityChangeSet.MaxReasonLength);
        Map(x => x.TenantId);
        Map(x => x.UserId);
    }
}