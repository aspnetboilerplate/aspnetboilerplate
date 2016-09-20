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
        /// <summary>
        /// Inserts a notification subscription.
        /// </summary>
        Task InsertSubscriptionAsync(NotificationSubscriptionInfo subscription);

        /// <summary>
        /// Deletes a notification subscription.
        /// </summary>
        Task DeleteSubscriptionAsync(UserIdentifier user, string notificationName, string entityTypeName, string entityId);

        /// <summary>
        /// Inserts a notification.
        /// </summary>
        Task InsertNotificationAsync(NotificationInfo notification);

        /// <summary>
        /// Gets a notification by Id, or returns null if not found.
        /// </summary>
        Task<NotificationInfo> GetNotificationOrNullAsync(Guid notificationId);

        /// <summary>
        /// Inserts a user notification.
        /// </summary>
        Task InsertUserNotificationAsync(UserNotificationInfo userNotification);

        /// <summary>
        /// Gets subscriptions for a notification.
        /// </summary>
        Task<List<NotificationSubscriptionInfo>> GetSubscriptionsAsync(string notificationName, string entityTypeName, string entityId);

        /// <summary>
        /// Gets subscriptions for a notification for specified tenant(s).
        /// </summary>
        Task<List<NotificationSubscriptionInfo>> GetSubscriptionsAsync(int?[] tenantIds, string notificationName, string entityTypeName, string entityId);

        /// <summary>
        /// Gets subscriptions for a user.
        /// </summary>
        Task<List<NotificationSubscriptionInfo>> GetSubscriptionsAsync(UserIdentifier user);

        /// <summary>
        /// Checks if a user subscribed for a notification
        /// </summary>
        Task<bool> IsSubscribedAsync(UserIdentifier user, string notificationName, string entityTypeName, string entityId);

        /// <summary>
        /// Updates a user notification state.
        /// </summary>
        Task UpdateUserNotificationStateAsync(int? notificationId, Guid userNotificationId, UserNotificationState state);

        /// <summary>
        /// Updates all notification states for a user.
        /// </summary>
        Task UpdateAllUserNotificationStatesAsync(UserIdentifier user, UserNotificationState state);

        /// <summary>
        /// Deletes a user notification.
        /// </summary>
        Task DeleteUserNotificationAsync(int? notificationId, Guid userNotificationId);

        /// <summary>
        /// Deletes all notifications of a user.
        /// </summary>
        Task DeleteAllUserNotificationsAsync(UserIdentifier user);

        /// <summary>
        /// Gets notifications of a user.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="skipCount">Skip count.</param>
        /// <param name="maxResultCount">Maximum result count.</param>
        /// <param name="state">State</param>
        Task<List<UserNotificationInfoWithNotificationInfo>> GetUserNotificationsWithNotificationsAsync(UserIdentifier user, UserNotificationState? state = null, int skipCount = 0, int maxResultCount = int.MaxValue);

        /// <summary>
        /// Gets user notification count.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="state">The state.</param>
        Task<int> GetUserNotificationCountAsync(UserIdentifier user, UserNotificationState? state = null);

        /// <summary>
        /// Gets a user notification.
        /// </summary>
        /// <param name="tenantId">Tenant Id</param>
        /// <param name="userNotificationId">Skip count.</param>
        Task<UserNotificationInfoWithNotificationInfo> GetUserNotificationWithNotificationOrNullAsync(int? tenantId, Guid userNotificationId);

        /// <summary>
        /// Inserts notification for a tenant.
        /// </summary>
        Task InsertTenantNotificationAsync(TenantNotificationInfo tenantNotificationInfo);

        /// <summary>
        /// Deletes the notification.
        /// </summary>
        /// <param name="notification">The notification.</param>
        Task DeleteNotificationAsync(NotificationInfo notification);
    }
}