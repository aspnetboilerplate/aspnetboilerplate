using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.Notifications
{
    /// <summary>
    /// Main service to subscribe to and publish notifications.
    /// </summary>
    public interface INotificationManager //TODO: Split into 3 services: INotificationPublisher, IUserNotificationManager and INotificationSubscriptionManager
    {
        /// <summary>
        /// Subscribes to a notification.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="notificationName">Name of the notification.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="entityId">The entity id.</param>
        Task SubscribeAsync(long userId, string notificationName, Type entityType = null, object entityId = null);

        /// <summary>
        /// Unsubscribes from a notification.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="notificationName">Name of the notification.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="entityId">The entity id.</param>
        Task UnsubscribeAsync(long userId, string notificationName, Type entityType = null, object entityId = null);

        /// <summary>
        /// Gets all subscribtions for given notification.
        /// </summary>
        /// <param name="notificationName">Name of the notification.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="entityId">The entity id.</param>
        Task<List<NotificationSubscription>> GetSubscriptionsAsync(string notificationName, Type entityType = null, object entityId = null);

        /// <summary>
        /// Gets subscribed notifications for a user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        Task<List<NotificationSubscription>> GetSubscribedNotificationsAsync(long userId);

        /// <summary>
        /// Checks if a user subscribed for a notification.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="notificationName">Name of the notification.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="entityId">The entity id.</param>
        Task<bool> IsSubscribedAsync(long userId, string notificationName, Type entityType = null, object entityId = null);

        /// <summary>
        /// Publishes a new notification.
        /// </summary>
        Task PublishAsync(NotificationPublishOptions options);

        /// <summary>
        /// Gets notifications for a user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="skipCount">Skip count.</param>
        /// <param name="maxResultCount">Maximum result count.</param>
        /// <returns></returns>
        Task<List<UserNotification>> GetUserNotifications(long userId, int skipCount = 0, int maxResultCount = int.MaxValue);

        /// <summary>
        /// Gets a user notification by given id.
        /// </summary>
        /// <param name="userNotificationId">The user notification id.</param>
        Task<List<UserNotification>> GetUserNotification(Guid userNotificationId);

        /// <summary>
        /// Updates a user notification state.
        /// </summary>
        /// <param name="userNotificationId">The user notification id.</param>
        /// <param name="state">New state.</param>
        Task UpdateUserNotificationStateAsync(Guid userNotificationId, UserNotificationState state);

        /// <summary>
        /// Updates all notification states for a user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="state">New state.</param>
        Task UpdateAllUserNotificationStatesAsync(long userId, UserNotificationState state);

        /// <summary>
        /// Deletes a user notification.
        /// </summary>
        /// <param name="userNotificationId">The user notification id.</param>
        Task DeleteUserNotificationAsync(Guid userNotificationId);

        /// <summary>
        /// Deletes all notifications of a user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        Task DeleteAllUserNotificationsAsync(long userId);
    }
}
