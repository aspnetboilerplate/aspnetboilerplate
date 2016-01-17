using System.Threading.Tasks;

namespace Abp.Notifications
{
    /// <summary>
    /// Main service to subscribe to and publish/send notifications.
    /// </summary>
    public interface INotificationManager
    {
        Task SubscribeAsync(long userId, NotificationSubscriptionOptions options); //TODO: Move userId to NotificationSubscriptionOptions?

        Task PublishAsync(NotificationPublishOptions options);

        Task SendAsync(NotificationSendOptions options, long[] userIds);

        //TODO: ...
    }
}
