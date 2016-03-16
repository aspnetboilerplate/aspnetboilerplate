using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.Notifications
{
    public class NullNotificationStore : INotificationStore
    {
        public Task InsertSubscriptionAsync(NotificationSubscriptionInfo subscription)
        {
            return Task.FromResult(0);
        }

        public Task DeleteSubscriptionAsync(NotificationSubscriptionInfo subscription)
        {
            return Task.FromResult(0);
        }

        public Task InsertNotificationAsync(NotificationInfo notification)
        {
            return Task.FromResult(0);
        }

        public Task<NotificationInfo> GetNotificationOrNullAsync(Guid notificationId)
        {
            return Task.FromResult(null as NotificationInfo);
        }

        public Task InsertUserNotificationAsync(UserNotificationInfo userNotification)
        {
            return Task.FromResult(0);
        }

        public Task<List<NotificationSubscriptionInfo>> GetSubscriptions(NotificationInfo notification)
        {
            return Task.FromResult(new List<NotificationSubscriptionInfo>());
        }
    }
}