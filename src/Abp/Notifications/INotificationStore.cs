using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.Notifications
{
    /// <summary>
    /// Used to store (persist) notifications.
    /// </summary>
    public interface INotificationStore
    {
        Task InsertSubscriptionAsync(NotificationSubscriptionOptions options);

        Task DeleteSubscriptionAsync(NotificationSubscriptionOptions options);

        Task InsertNotificationAsync(NotificationInfo notification);

        Task<NotificationInfo> GetNotificationOrNullAsync(Guid notificationId);
        
        Task InsertUserNotificationAsync(UserNotificationInfo userNotification);

        Task<long[]> GetSubscribedUserIdsAsync(NotificationInfo notification);
    }
}