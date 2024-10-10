using Abp.NHibernate.EntityMappings;
using Abp.Notifications;
using System;

namespace Abp.Zero.NHibernate.EntityMappings;

public class TenantNotificationInfoMap : EntityMap<TenantNotificationInfo, Guid>
{
    public TenantNotificationInfoMap() : base("AbpTenantNotifications")
    {
        Map(x => x.Data)
            .Length(NotificationInfo.MaxDataLength);
        Map(x => x.DataTypeName)
            .Length(NotificationInfo.MaxDataTypeNameLength);
        Map(x => x.EntityId)
            .Length(NotificationInfo.MaxEntityIdLength);
        Map(x => x.EntityTypeAssemblyQualifiedName)
            .Length(NotificationInfo.MaxEntityTypeAssemblyQualifiedNameLength);
        Map(x => x.EntityTypeName)
            .Length(NotificationInfo.MaxEntityTypeNameLength);
        Map(x => x.NotificationName)
            .Length(NotificationInfo.MaxNotificationNameLength)
            .Not.Nullable();
        Map(x => x.Severity)
            .CustomType<NotificationSeverity>()
            .Not.Nullable();
        Map(x => x.TenantId);

        this.MapCreationAudited();
    }
}