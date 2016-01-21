
namespace Abp.Notifications
{
    public interface IUserNotificationQueue
    {
        void Add(NotificationInfo notification);
    }
}