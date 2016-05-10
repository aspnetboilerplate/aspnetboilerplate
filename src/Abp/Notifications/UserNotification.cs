using System;
using Abp.Application.Services.Dto;

namespace Abp.Notifications
{
    /// <summary>
    ///     Represents a notification sent to a user.
    /// </summary>
    public class UserNotification : EntityDto<Guid>, IUserIdentifier
    {
        /// <summary>
        ///     Current state of the user notification.
        /// </summary>
        public UserNotificationState State { get; set; }

        /// <summary>
        ///     The notification.
        /// </summary>
        public TenantNotification Notification { get; set; }

        /// <summary>
        ///     TenantId.
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        ///     User Id.
        /// </summary>
        public Guid UserId { get; set; }
    }
}