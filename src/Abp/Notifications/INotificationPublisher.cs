using System.Threading.Tasks;

namespace Abp.Notifications
{
    public interface INotificationPublisher
    {
        /// <summary>
        /// Publishes a new notification.
        /// </summary>
        Task PublishAsync(NotificationPublishOptions options);
    }
}