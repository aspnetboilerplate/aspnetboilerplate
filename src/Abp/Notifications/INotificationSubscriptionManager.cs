using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.Notifications
{
    /// <summary>
    /// Used to manage subscriptions for notifications.
    /// </summary>
    public interface INotificationSubscriptionManager
    {
        /// <summary>
        /// Subscribes to a notification for given user and notification informations.
        /// </summary>
        /// <param name="tenantId">Tenant id of the user. Null for host users.</param>
        /// <param name="userId">The user id (which belongs to given tenantId).</param>
        /// <param name="notificationName">Name of the notification.</param>
        /// <param name="entityIdentifier">entity identifier</param>
        Task SubscribeAsync(Guid? tenantId, Guid userId, string notificationName, EntityIdentifier entityIdentifier = null);

        /// <summary>
        /// Subscribes to all available notifications for given user.
        /// It does not subscribe entity related notifications.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="userId">The user identifier.</param>
        Task SubscribeToAllAvailableNotificationsAsync(Guid? tenantId, Guid userId);

        /// <summary>
        /// Unsubscribes from a notification.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="notificationName">Name of the notification.</param>
        /// <param name="entityIdentifier">entity identifier</param>
        Task UnsubscribeAsync(Guid userId, string notificationName, EntityIdentifier entityIdentifier = null);

        /// <summary>
        /// Gets all subscribtions for given notification (including all tenants).
        /// </summary>
        /// <param name="notificationName">Name of the notification.</param>
        /// <param name="entityIdentifier">entity identifier</param>
        Task<List<NotificationSubscription>> GetSubscriptionsAsync(string notificationName, EntityIdentifier entityIdentifier = null);

        /// <summary>
        /// Gets all subscribtions for given notification.
        /// </summary>
        /// <param name="tenantId">Tenant id. Null for the host.</param>
        /// <param name="notificationName">Name of the notification.</param>
        /// <param name="entityIdentifier">entity identifier</param>
        Task<List<NotificationSubscription>> GetSubscriptionsAsync(Guid? tenantId, string notificationName, EntityIdentifier entityIdentifier = null);

        /// <summary>
        /// Gets subscribed notifications for a user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        Task<List<NotificationSubscription>> GetSubscribedNotificationsAsync(Guid userId);

        /// <summary>
        /// Checks if a user subscribed for a notification.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="notificationName">Name of the notification.</param>
        /// <param name="entityIdentifier">entity identifier</param>
        Task<bool> IsSubscribedAsync(Guid userId, string notificationName, EntityIdentifier entityIdentifier = null);
    }
}