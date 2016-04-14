using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Abp.Notifications
{
    /// <summary>
    /// Used to store a user notification.
    /// </summary>
    [Serializable]
    [Table("AbpUserNotifications")]
    public class UserNotificationInfo : Entity<Guid>, IHasCreationTime
    {
        /// <summary>
        /// User Id.
        /// </summary>
        public virtual Guid UserId { get; set; }

        /// <summary>
        /// Notification Id.
        /// </summary>
        [Required]
        public virtual Guid NotificationId { get; set; }

        /// <summary>
        /// Current state of the user notification.
        /// </summary>
        public virtual UserNotificationState State { get; set; }

        public virtual DateTime CreationTime { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotificationInfo"/> class.
        /// </summary>
        public UserNotificationInfo()
        {
            State = UserNotificationState.Unread;
            CreationTime = Clock.Now;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotificationInfo"/> class.
        /// </summary>
        public UserNotificationInfo(Guid userId, Guid notificationId)
            : this()
        {
            UserId = userId;
            NotificationId = notificationId;
        }
    }
}