namespace Abp.Notifications
{
    /// <summary>
    /// A class contains a <see cref="UserNotificationInfo"/> and related <see cref="NotificationInfo"/>.
    /// </summary>
    public class UserNotificationInfoWithNotificationInfo
    {
        /// <summary>
        /// User notification.
        /// </summary>
        public UserNotificationInfo UserNotification { get; set; }

        /// <summary>
        /// Notification.
        /// </summary>
        public TenantNotificationInfo Notification { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotificationInfoWithNotificationInfo"/> class.
        /// </summary>
        public UserNotificationInfoWithNotificationInfo(UserNotificationInfo userNotification, TenantNotificationInfo notification)
        {
            UserNotification = userNotification;
            Notification = notification;
        }
    }
}