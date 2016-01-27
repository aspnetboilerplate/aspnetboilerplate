using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.Notifications
{
    public interface INotificationSubscriptionManager
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
    }
}
