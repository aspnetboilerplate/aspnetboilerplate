using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.Notifications
{
    /// <summary>
    /// Main service to subscribe to and publish/send notifications.
    /// </summary>
    public interface INotificationManager
    {
        Task SubscribeAsync(NotificationSubscriptionOptions options);

        Task PublishAsync(NotificationPublishOptions options);

        Task SendAsync(NotificationSendOptions options);

        //TODO: ...
    }
}
