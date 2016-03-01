using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Adorable.Notifications
{
    /// <summary>
    /// Null pattern implementation of <see cref="INotificationStore"/>.
    /// </summary>
    public class NullNotificationStore : INotificationStore
    {
        public Task InsertSubscriptionAsync(NotificationSubscriptionInfo subscription)
        {
            return Task.FromResult(0);
        }

        public Task DeleteSubscriptionAsync(long userId, string notificationName, string entityTypeName, string entityId)
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

        public Task<List<NotificationSubscriptionInfo>> GetSubscriptionsAsync(string notificationName, string entityTypeName = null, string entityId = null)
        {
            return Task.FromResult(new List<NotificationSubscriptionInfo>());
        }

        public Task<List<NotificationSubscriptionInfo>> GetSubscriptionsAsync(int?[] tenantIds, string notificationName, string entityTypeName, string entityId)
        {
            return Task.FromResult(new List<NotificationSubscriptionInfo>());
        }

        public Task<List<NotificationSubscriptionInfo>> GetSubscriptionsAsync(long userId)
        {
            return Task.FromResult(new List<NotificationSubscriptionInfo>());
        }

        public Task<bool> IsSubscribedAsync(long userId, string notificationName, string entityTypeName, string entityId)
        {
            return Task.FromResult(false);
        }

        public Task UpdateUserNotificationStateAsync(Guid userNotificationId, UserNotificationState state)
        {
            return Task.FromResult(0);
        }

        public Task UpdateAllUserNotificationStatesAsync(long userId, UserNotificationState state)
        {
            return Task.FromResult(0);
        }

        public Task DeleteUserNotificationAsync(Guid userNotificationId)
        {
            return Task.FromResult(0);
        }

        public Task DeleteAllUserNotificationsAsync(long userId)
        {
            return Task.FromResult(0);
        }

        public Task<List<UserNotificationInfoWithNotificationInfo>> GetUserNotificationsWithNotificationsAsync(long userId, UserNotificationState? state = null, int skipCount = 0, int maxResultCount = int.MaxValue)
        {
            return Task.FromResult(new List<UserNotificationInfoWithNotificationInfo>());
        }

        public Task<int> GetUserNotificationCountAsync(long userId, UserNotificationState? state = null)
        {
            return Task.FromResult(0);
        }

        public Task<UserNotificationInfoWithNotificationInfo> GetUserNotificationWithNotificationOrNullAsync(Guid userNotificationId)
        {
            return Task.FromResult((UserNotificationInfoWithNotificationInfo)null);
        }
    }
}