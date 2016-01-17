using System.Threading.Tasks;

namespace Abp.Notifications
{
    /// <summary>
    /// Used to store (persist) notifications.
    /// </summary>
    public interface INotificationStore
    {
        Task Insert(UserNotification userNotification);

        //TODO: ...
    }

    public class UserNotification
    {
        public long UserId { get; set; }

        public Notification Notification { get; set; }

        public bool IsDelivered { get; set; }

        //TODO: Channel...
    }
}