using System.Threading.Tasks;

namespace Abp.Notifications
{
    public static class NotificationManagerExtensions
    {
        public static Task SubscribeAsync(this INotificationManager notificationManager, long userId, string notificationName)
        {
            return notificationManager.SubscribeAsync(new NotificationSubscriptionOptions(userId,notificationName));
        }

        public static Task PublishAsync(this INotificationManager notificationManager,NotificationInfo notification)
        {
            return notificationManager.PublishAsync(new NotificationPublishOptions(notification));
        }

        //TODO: Add more shortcut methods
    }
}