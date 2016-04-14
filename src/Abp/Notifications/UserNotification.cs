using Abp.Application.Services.Dto;
using System;

namespace Abp.Notifications
{
    /// <summary>
    /// Represents a notification sent to a user.
    /// </summary>
    public class UserNotification : EntityDto<Guid>
    {
        /// <summary>
        /// User Id.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Current state of the user notification.
        /// </summary>
        public UserNotificationState State { get; set; }

        /// <summary>
        /// The notification.
        /// </summary>
        public Notification Notification { get; set; }
    }
}