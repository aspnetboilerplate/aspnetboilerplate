using System;

namespace Abp.Notifications
{
    [Serializable]
    public class UserNotificationInfo
    {
        public long UserId { get; set; }

        public Guid NotificationId { get; set; }

        public UserNotificationState State { get; set; }

        public UserNotificationInfo()
        {
            State = UserNotificationState.Unread;
        }

        public UserNotificationInfo(long userId, Guid notificationId)
            : this()
        {
            UserId = userId;
            NotificationId = notificationId;
        }
    }
}