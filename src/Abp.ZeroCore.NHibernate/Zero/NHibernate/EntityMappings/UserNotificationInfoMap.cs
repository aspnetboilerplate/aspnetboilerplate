using System;
using Abp.NHibernate.EntityMappings;
using Abp.Notifications;

namespace Abp.Zero.NHibernate.EntityMappings;

public class UserNotificationInfoMap : EntityMap<UserNotificationInfo, Guid>
{
    public UserNotificationInfoMap() : base("AbpUserNotifications")
    {
        Map(x => x.State)
            .CustomType<UserNotificationState>()
            .Not.Nullable();
        Map(x => x.TenantId);
        Map(x => x.TenantNotificationId)
            .Not.Nullable();
        Map(x => x.UserId)
            .Not.Nullable();

        this.MapCreationTime();
    }
}