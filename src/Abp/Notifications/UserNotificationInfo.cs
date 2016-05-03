using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;

namespace Abp.Notifications
{
    /// <summary>
    /// Used to store a user notification.
    /// </summary>
    [Serializable]
    [Table("AbpUserNotifications")]
    public class UserNotificationInfo : Entity<Guid>, IHasCreationTime, IMayHaveTenant
    {
        /// <summary>
        /// Tenant Id.
        /// </summary>
        public virtual int? TenantId { get; set; }

        /// <summary>
        /// User Id.
        /// </summary>
        public virtual long UserId { get; set; }

        /// <summary>
        /// Notification Id.
        /// </summary>
        [Required]
        public virtual Guid TenantNotificationId { get; set; }

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
    }
}