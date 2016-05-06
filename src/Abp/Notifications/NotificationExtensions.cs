using System;
using System.Collections.Generic;
using Abp.Domain.Entities;
using Abp.Threading;

namespace Abp.Notifications
{
    /// <summary>
    /// Extension methods for
    /// <see cref="INotificationSubscriptionManager"/>, 
    /// <see cref="INotificationPublisher"/>, 
    /// <see cref="IUserNotificationManager"/>.
    /// </summary>
    public static class NotificationExtensions
    {
        #region INotificationSubscriptionManager

        /// <summary>
        /// Subscribes to a notification.
        /// </summary>
        /// <param name="notificationSubscriptionManager">Notification subscription manager</param>
        /// <param name="user">User.</param>
        /// <param name="notificationName">Name of the notification.</param>
        /// <param name="entityIdentifier">entity identifier</param>
        public static void Subscribe(this INotificationSubscriptionManager notificationSubscriptionManager, UserIdentifier user, string notificationName, EntityIdentifier entityIdentifier = null)
        {
            AsyncHelper.RunSync(() => notificationSubscriptionManager.SubscribeAsync(user, notificationName, entityIdentifier));
        }

        /// <summary>
        /// Subscribes to all available notifications for given user.
        /// It does not subscribe entity related notifications.
        /// </summary>
        /// <param name="notificationSubscriptionManager">Notification subscription manager</param>
        /// <param name="user">User.</param>
        public static void SubscribeToAllAvailableNotifications(this INotificationSubscriptionManager notificationSubscriptionManager, UserIdentifier user)
        {
            AsyncHelper.RunSync(() => notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync(user));            
        }

        /// <summary>
        /// Unsubscribes from a notification.
        /// </summary>
        /// <param name="notificationSubscriptionManager">Notification subscription manager</param>
        /// <param name="user">User.</param>
        /// <param name="notificationName">Name of the notification.</param>
        /// <param name="entityIdentifier">entity identifier</param>
        public static void Unsubscribe(this INotificationSubscriptionManager notificationSubscriptionManager, UserIdentifier user, string notificationName, EntityIdentifier entityIdentifier = null)
        {
            AsyncHelper.RunSync(() => notificationSubscriptionManager.UnsubscribeAsync(user, notificationName, entityIdentifier));
        }

        /// <summary>
        /// Gets all subscribtions for given notification.
        /// TODO: Can work only for single database approach!
        /// </summary>
        /// <param name="notificationSubscriptionManager">Notification subscription manager</param>
        /// <param name="notificationName">Name of the notification.</param>
        /// <param name="entityIdentifier">entity identifier</param>
        public static List<NotificationSubscription> GetSubscriptions(this INotificationSubscriptionManager notificationSubscriptionManager, string notificationName, EntityIdentifier entityIdentifier = null)
        {
            return AsyncHelper.RunSync(() => notificationSubscriptionManager.GetSubscriptionsAsync(notificationName, entityIdentifier));
        }

        /// <summary>
        /// Gets all subscribtions for given notification.
        /// </summary>
        /// <param name="notificationSubscriptionManager">Notification subscription manager</param>
        /// <param name="tenantId">Tenant id. Null for the host.</param>
        /// <param name="notificationName">Name of the notification.</param>
        /// <param name="entityIdentifier">entity identifier</param>
        public static List<NotificationSubscription> GetSubscriptions(this INotificationSubscriptionManager notificationSubscriptionManager, int? tenantId, string notificationName, EntityIdentifier entityIdentifier = null)
        {
            return AsyncHelper.RunSync(() => notificationSubscriptionManager.GetSubscriptionsAsync(tenantId, notificationName, entityIdentifier));
        }

        /// <summary>
        /// Gets subscribed notifications for a user.
        /// </summary>
        /// <param name="notificationSubscriptionManager">Notification subscription manager</param>
        /// <param name="user">User.</param>
        public static List<NotificationSubscription> GetSubscribedNotifications(this INotificationSubscriptionManager notificationSubscriptionManager, UserIdentifier user)
        {
            return AsyncHelper.RunSync(() => notificationSubscriptionManager.GetSubscribedNotificationsAsync(user));
        }

        /// <summary>
        /// Checks if a user subscribed for a notification.
        /// </summary>
        /// <param name="notificationSubscriptionManager">Notification subscription manager</param>
        /// <param name="user">User.</param>
        /// <param name="notificationName">Name of the notification.</param>
        /// <param name="entityIdentifier">entity identifier</param>
        public static bool IsSubscribed(this INotificationSubscriptionManager notificationSubscriptionManager, UserIdentifier user, string notificationName, EntityIdentifier entityIdentifier = null)
        {
            return AsyncHelper.RunSync(() => notificationSubscriptionManager.IsSubscribedAsync(user, notificationName, entityIdentifier));
        }

        #endregion

        #region INotificationPublisher

        /// <summary>
        /// Publishes a new notification.
        /// </summary>
        /// <param name="notificationPublisher">Notification publisher</param>
        /// <param name="notificationName">Unique notification name</param>
        /// <param name="data">Notification data (optional)</param>
        /// <param name="entityIdentifier">The entity identifier if this notification is related to an entity</param>
        /// <param name="severity">Notification severity</param>
        /// <param name="userIds">Target user id(s). Used to send notification to specific user(s). If this is null/empty, the notification is sent to all subscribed users</param>
        public static void Publish(this INotificationPublisher notificationPublisher, string notificationName, NotificationData data = null, EntityIdentifier entityIdentifier = null, NotificationSeverity severity = NotificationSeverity.Info, UserIdentifier[] userIds = null)
        {
            AsyncHelper.RunSync(() => notificationPublisher.PublishAsync(notificationName, data, entityIdentifier, severity, userIds));
        }

        #endregion

        #region IUserNotificationManager

        /// <summary>
        /// Gets notifications for a user.
        /// </summary>
        /// <param name="userNotificationManager">User notificaiton manager</param>
        /// <param name="user">User.</param>
        /// <param name="state">State</param>
        /// <param name="skipCount">Skip count.</param>
        /// <param name="maxResultCount">Maximum result count.</param>
        public static List<UserNotification> GetUserNotifications(this IUserNotificationManager userNotificationManager, UserIdentifier user, UserNotificationState? state = null, int skipCount = 0, int maxResultCount = int.MaxValue)
        {
            return AsyncHelper.RunSync(() => userNotificationManager.GetUserNotificationsAsync(user, state, skipCount: skipCount, maxResultCount: maxResultCount));
        }

        /// <summary>
        /// Gets user notification count.
        /// </summary>
        /// <param name="userNotificationManager">User notificaiton manager</param>
        /// <param name="user">User.</param>
        /// <param name="state">State.</param>
        public static int GetUserNotificationCount(this IUserNotificationManager userNotificationManager, UserIdentifier user, UserNotificationState? state = null)
        {
            return AsyncHelper.RunSync(() => userNotificationManager.GetUserNotificationCountAsync(user, state));
        }

        /// <summary>
        /// Gets a user notification by given id.
        /// </summary>
        /// <param name="userNotificationManager">User notificaiton manager</param>
        /// <param name="tenantId">Tenant Id</param>
        /// <param name="userNotificationId">The user notification id.</param>
        public static UserNotification GetUserNotification(this IUserNotificationManager userNotificationManager, int? tenantId, Guid userNotificationId)
        {
            return AsyncHelper.RunSync(() => userNotificationManager.GetUserNotificationAsync(tenantId, userNotificationId));
        }

        /// <summary>
        /// Updates a user notification state.
        /// </summary>
        /// <param name="userNotificationManager">User notificaiton manager</param>
        /// <param name="tenantId">Tenant Id</param>
        /// <param name="userNotificationId">The user notification id.</param>
        /// <param name="state">New state.</param>
        public static void UpdateUserNotificationState(this IUserNotificationManager userNotificationManager, int? tenantId, Guid userNotificationId, UserNotificationState state)
        {
            AsyncHelper.RunSync(() => userNotificationManager.UpdateUserNotificationStateAsync(tenantId, userNotificationId, state));
        }

        /// <summary>
        /// Updates all notification states for a user.
        /// </summary>
        /// <param name="userNotificationManager">User notificaiton manager</param>
        /// <param name="user">User.</param>
        /// <param name="state">New state.</param>
        public static void UpdateAllUserNotificationStates(this IUserNotificationManager userNotificationManager, UserIdentifier user, UserNotificationState state)
        {
            AsyncHelper.RunSync(() => userNotificationManager.UpdateAllUserNotificationStatesAsync(user, state));
        }

        /// <summary>
        /// Deletes a user notification.
        /// </summary>
        /// <param name="userNotificationManager">User notificaiton manager</param>
        /// <param name="tenantId">Tenant Id</param>
        /// <param name="userNotificationId">The user notification id.</param>
        public static void DeleteUserNotification(this IUserNotificationManager userNotificationManager, int? tenantId, Guid userNotificationId)
        {
            AsyncHelper.RunSync(() => userNotificationManager.DeleteUserNotificationAsync(tenantId, userNotificationId));
        }

        /// <summary>
        /// Deletes all notifications of a user.
        /// </summary>
        /// <param name="userNotificationManager">User notificaiton manager</param>
        /// <param name="user">The user id.</param>
        public static void DeleteAllUserNotifications(this IUserNotificationManager userNotificationManager, UserIdentifier user)
        {
            AsyncHelper.RunSync(() => userNotificationManager.DeleteAllUserNotificationsAsync(user));
        }

        #endregion
    }
}