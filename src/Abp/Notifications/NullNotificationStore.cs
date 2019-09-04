using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.Notifications
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

        public Task DeleteSubscriptionAsync(UserIdentifier user, string notificationName, string entityTypeName, string entityId)
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

        public Task<List<NotificationSubscriptionInfo>> GetSubscriptionsAsync(UserIdentifier user)
        {
            return Task.FromResult(new List<NotificationSubscriptionInfo>());
        }

        public Task<bool> IsSubscribedAsync(UserIdentifier user, string notificationName, string entityTypeName, string entityId)
        {
            return Task.FromResult(false);
        }

        public Task UpdateUserNotificationStateAsync(int? notificationId, Guid userNotificationId, UserNotificationState state)
        {
            return Task.FromResult(0);
        }

        public Task UpdateAllUserNotificationStatesAsync(UserIdentifier user, UserNotificationState state)
        {
            return Task.FromResult(0);
        }

        public Task DeleteUserNotificationAsync(int? notificationId, Guid userNotificationId)
        {
            return Task.FromResult(0);
        }

        public Task DeleteAllUserNotificationsAsync(
            UserIdentifier user, 
            UserNotificationState? state = null,
            DateTime? startDate = null, 
            DateTime? endDate = null)
        {
            return Task.FromResult(0);
        }


        public Task<List<UserNotificationInfoWithNotificationInfo>> GetUserNotificationsWithNotificationsAsync(UserIdentifier user, UserNotificationState? state = null, int skipCount = 0, int maxResultCount = int.MaxValue, DateTime? startDate = null, DateTime? endDate = null)
        {
            return Task.FromResult(new List<UserNotificationInfoWithNotificationInfo>());
        }

        public Task<int> GetUserNotificationCountAsync(UserIdentifier user, UserNotificationState? state = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            return Task.FromResult(0);
        }

        public Task<UserNotificationInfoWithNotificationInfo> GetUserNotificationWithNotificationOrNullAsync(int? tenantId, Guid userNotificationId)
        {
            return Task.FromResult((UserNotificationInfoWithNotificationInfo)null);
        }

        public Task InsertTenantNotificationAsync(TenantNotificationInfo tenantNotificationInfo)
        {
            return Task.FromResult(0);
        }

        public Task DeleteNotificationAsync(NotificationInfo notification)
        {
            return Task.FromResult(0);
        }

        public void InsertSubscription(NotificationSubscriptionInfo subscription)
        {
        }

        public void DeleteSubscription(UserIdentifier user, string notificationName, string entityTypeName, string entityId)
        {
        }

        public void InsertNotification(NotificationInfo notification)
        {
        }

        public NotificationInfo GetNotificationOrNull(Guid notificationId)
        {
            return new NotificationInfo();
        }

        public void InsertUserNotification(UserNotificationInfo userNotification)
        {
        }

        public List<NotificationSubscriptionInfo> GetSubscriptions(string notificationName, string entityTypeName, string entityId)
        {
            return new List<NotificationSubscriptionInfo>();
        }

        public List<NotificationSubscriptionInfo> GetSubscriptions(int?[] tenantIds, string notificationName, string entityTypeName, string entityId)
        {
            return new List<NotificationSubscriptionInfo>();
        }

        public List<NotificationSubscriptionInfo> GetSubscriptions(UserIdentifier user)
        {
            return new List<NotificationSubscriptionInfo>();
        }

        public bool IsSubscribed(UserIdentifier user, string notificationName, string entityTypeName, string entityId)
        {
            return false;
        }

        public void UpdateUserNotificationState(int? tenantId, Guid userNotificationId, UserNotificationState state)
        {
        }

        public void UpdateAllUserNotificationStates(UserIdentifier user, UserNotificationState state)
        {
        }

        public void DeleteUserNotification(int? tenantId, Guid userNotificationId)
        {
        }

        public void DeleteAllUserNotifications(UserIdentifier user, 
            UserNotificationState? state = null,
            DateTime? startDate = null, 
            DateTime? endDate = null)
        {
        }

        public List<UserNotificationInfoWithNotificationInfo> GetUserNotificationsWithNotifications(UserIdentifier user, UserNotificationState? state = null, int skipCount = 0, int maxResultCount = int.MaxValue, DateTime? startDate = null, DateTime? endDate = null)
        {
            return new List<UserNotificationInfoWithNotificationInfo>();
        }

        public int GetUserNotificationCount(UserIdentifier user, UserNotificationState? state = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            return 0;
        }

        public UserNotificationInfoWithNotificationInfo GetUserNotificationWithNotificationOrNull(int? tenantId, Guid userNotificationId)
        {
            return (UserNotificationInfoWithNotificationInfo)null;
        }

        public void InsertTenantNotification(TenantNotificationInfo tenantNotificationInfo)
        {
        }

        public void DeleteNotification(NotificationInfo notification)
        {
        }
    }
}