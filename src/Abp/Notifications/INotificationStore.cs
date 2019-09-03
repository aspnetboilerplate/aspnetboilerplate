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
        /// Inserts a notification subscription.
        /// </summary>
        void InsertSubscription(NotificationSubscriptionInfo subscription);

        /// <summary>
        /// Deletes a notification subscription.
        /// </summary>
        Task DeleteSubscriptionAsync(UserIdentifier user, string notificationName, string entityTypeName, string entityId);

        /// <summary>
        /// Deletes a notification subscription.
        /// </summary>
        void DeleteSubscription(UserIdentifier user, string notificationName, string entityTypeName, string entityId);

        /// <summary>
        /// Inserts a notification.
        /// </summary>
        Task InsertNotificationAsync(NotificationInfo notification);

        /// <summary>
        /// Inserts a notification.
        /// </summary>
        void InsertNotification(NotificationInfo notification);

        /// <summary>
        /// Gets a notification by Id, or returns null if not found.
        /// </summary>
        Task<NotificationInfo> GetNotificationOrNullAsync(Guid notificationId);

        /// <summary>
        /// Gets a notification by Id, or returns null if not found.
        /// </summary>
        NotificationInfo GetNotificationOrNull(Guid notificationId);

        /// <summary>
        /// Inserts a user notification.
        /// </summary>
        Task InsertUserNotificationAsync(UserNotificationInfo userNotification);

        /// <summary>
        /// Inserts a user notification.
        /// </summary>
        void InsertUserNotification(UserNotificationInfo userNotification);

        /// <summary>
        /// Gets subscriptions for a notification.
        /// </summary>
        Task<List<NotificationSubscriptionInfo>> GetSubscriptionsAsync(string notificationName, string entityTypeName, string entityId);

        /// <summary>
        /// Gets subscriptions for a notification.
        /// </summary>
        List<NotificationSubscriptionInfo> GetSubscriptions(string notificationName, string entityTypeName, string entityId);

        /// <summary>
        /// Gets subscriptions for a notification for specified tenant(s).
        /// </summary>
        Task<List<NotificationSubscriptionInfo>> GetSubscriptionsAsync(int?[] tenantIds, string notificationName, string entityTypeName, string entityId);

        /// <summary>
        /// Gets subscriptions for a notification for specified tenant(s).
        /// </summary>
        List<NotificationSubscriptionInfo> GetSubscriptions(int?[] tenantIds, string notificationName, string entityTypeName, string entityId);

        /// <summary>
        /// Gets subscriptions for a user.
        /// </summary>
        Task<List<NotificationSubscriptionInfo>> GetSubscriptionsAsync(UserIdentifier user);

        /// <summary>
        /// Gets subscriptions for a user.
        /// </summary>
        List<NotificationSubscriptionInfo> GetSubscriptions(UserIdentifier user);

        /// <summary>
        /// Checks if a user subscribed for a notification
        /// </summary>
        Task<bool> IsSubscribedAsync(UserIdentifier user, string notificationName, string entityTypeName, string entityId);

        /// <summary>
        /// Checks if a user subscribed for a notification
        /// </summary>
        bool IsSubscribed(UserIdentifier user, string notificationName, string entityTypeName, string entityId);

        /// <summary>
        /// Updates a user notification state.
        /// </summary>
        Task UpdateUserNotificationStateAsync(int? tenantId, Guid userNotificationId, UserNotificationState state);

        /// <summary>
        /// Updates a user notification state.
        /// </summary>
        void UpdateUserNotificationState(int? tenantId, Guid userNotificationId, UserNotificationState state);

        /// <summary>
        /// Updates all notification states for a user.
        /// </summary>
        Task UpdateAllUserNotificationStatesAsync(UserIdentifier user, UserNotificationState state);

        /// <summary>
        /// Updates all notification states for a user.
        /// </summary>
        void UpdateAllUserNotificationStates(UserIdentifier user, UserNotificationState state);

        /// <summary>
        /// Deletes a user notification.
        /// </summary>
        Task DeleteUserNotificationAsync(int? tenantId, Guid userNotificationId);

        /// <summary>
        /// Deletes a user notification.
        /// </summary>
        void DeleteUserNotification(int? tenantId, Guid userNotificationId);

        /// <summary>
        /// Deletes all notifications of a user.
        /// </summary>
        Task DeleteAllUserNotificationsAsync(UserIdentifier user, UserNotificationState? state = null, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// Deletes all notifications of a user.
        /// </summary>
        void DeleteAllUserNotifications(UserIdentifier user, UserNotificationState? state = null, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// Gets notifications of a user.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="state">State</param>
        /// <param name="skipCount">Skip count.</param>
        /// <param name="maxResultCount">Maximum result count.</param>
        /// <param name="startDate">List notifications published after startDateTime</param>
        /// <param name="endDate">List notifications published before startDateTime</param>
        Task<List<UserNotificationInfoWithNotificationInfo>> GetUserNotificationsWithNotificationsAsync(UserIdentifier user, UserNotificationState? state = null, int skipCount = 0, int maxResultCount = int.MaxValue, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// Gets notifications of a user.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="skipCount">Skip count.</param>
        /// <param name="maxResultCount">Maximum result count.</param>
        /// <param name="state">State</param>
        List<UserNotificationInfoWithNotificationInfo> GetUserNotificationsWithNotifications(UserIdentifier user, UserNotificationState? state = null, int skipCount = 0, int maxResultCount = int.MaxValue, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// Gets user notification count.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="state">The state.</param>
        /// <param name="startDate">List notifications published after startDateTime</param>
        /// <param name="endDate">List notifications published before startDateTime</param>
        Task<int> GetUserNotificationCountAsync(UserIdentifier user, UserNotificationState? state = null, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// Gets user notification count.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="state">The state.</param>
        /// <param name="startDate">List notifications published after startDateTime</param>
        /// <param name="endDate">List notifications published before startDateTime</param>
        int GetUserNotificationCount(UserIdentifier user, UserNotificationState? state = null, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// Gets a user notification.
        /// </summary>
        /// <param name="tenantId">Tenant Id</param>
        /// <param name="userNotificationId">Skip count.</param>
        Task<UserNotificationInfoWithNotificationInfo> GetUserNotificationWithNotificationOrNullAsync(int? tenantId, Guid userNotificationId);

        /// <summary>
        /// Gets a user notification.
        /// </summary>
        /// <param name="tenantId">Tenant Id</param>
        /// <param name="userNotificationId">Skip count.</param>
        UserNotificationInfoWithNotificationInfo GetUserNotificationWithNotificationOrNull(int? tenantId, Guid userNotificationId);

        /// <summary>
        /// Inserts notification for a tenant.
        /// </summary>
        Task InsertTenantNotificationAsync(TenantNotificationInfo tenantNotificationInfo);

        /// <summary>
        /// Inserts notification for a tenant.
        /// </summary>
        void InsertTenantNotification(TenantNotificationInfo tenantNotificationInfo);

        /// <summary>
        /// Deletes the notification.
        /// </summary>
        /// <param name="notification">The notification.</param>
        Task DeleteNotificationAsync(NotificationInfo notification);

        /// <summary>
        /// Deletes the notification.
        /// </summary>
        /// <param name="notification">The notification.</param>
        void DeleteNotification(NotificationInfo notification);
    }
}