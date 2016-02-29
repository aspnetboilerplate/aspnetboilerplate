namespace Adorable.Notifications
{
    /// <summary>
    /// Extension methods for <see cref="UserNotificationInfo"/>.
    /// </summary>
    public static class UserNotificationInfoExtensions
    {
        /// <summary>
        /// Converts <see cref="UserNotificationInfo"/> to <see cref="UserNotification"/>.
        /// </summary>
        public static UserNotification ToUserNotification(this UserNotificationInfo userNotificationInfo, Notification notification)
        {
            return new UserNotification
            {
                Id = userNotificationInfo.Id,
                Notification = notification,
                UserId = userNotificationInfo.UserId,
                State = userNotificationInfo.State
            };
        }
    }
}