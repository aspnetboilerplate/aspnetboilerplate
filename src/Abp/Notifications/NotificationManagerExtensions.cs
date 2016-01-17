using System.Threading.Tasks;

namespace Abp.Notifications
{
    public static class NotificationManagerExtensions
    {
        public static Task SubscribeAsync(this INotificationManager notificationManager, long userId, string notificationName)
        {
            return notificationManager.SubscribeAsync(userId, new NotificationSubscriptionOptions(notificationName));
        }

        public static Task PublishAsync(this INotificationManager notificationManager,Notification notification)
        {
            return notificationManager.PublishAsync(new NotificationPublishOptions(notification));
        }

        //TODO: Add more shortcut methods
    }
}