namespace Abp.Notifications
{
    /// <summary>
    /// Extension methods for <see cref="UserNotificationInfoWithNotificationInfo"/>.
    /// </summary>
    public static class UserNotificationInfoWithNotificationInfoExtensions
    {
        /// <summary>
        /// Converts <see cref="UserNotificationInfoWithNotificationInfo"/> to <see cref="UserNotification"/>.
        /// </summary>
        public static UserNotification ToUserNotification(this UserNotificationInfoWithNotificationInfo userNotificationInfoWithNotificationInfo)
        {
            return userNotificationInfoWithNotificationInfo.UserNotification.ToUserNotification(
                userNotificationInfoWithNotificationInfo.Notification.ToTenantNotification()
                );
        }
    }
}