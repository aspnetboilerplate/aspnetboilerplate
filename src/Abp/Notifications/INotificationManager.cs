using System.Threading.Tasks;

namespace Abp.Notifications
{
    /// <summary>
    /// Main service to subscribe to and publish notifications.
    /// </summary>
    public interface INotificationManager
    {
        /// <summary>
        /// Subscribes to a notification list.
        /// </summary>
        Task SubscribeAsync(NotificationSubscriptionOptions options);

        /// <summary>
        /// Unsubscribes from a notification list.
        /// </summary>
        Task UnsubscribeAsync(NotificationSubscriptionOptions options);

        /// <summary>
        /// Publishes a new notification.
        /// </summary>
        Task PublishAsync(NotificationPublishOptions options);

        //TODO: Get subscribed users for a notification

        //TODO: Get subscriptions of a user
    }
}
