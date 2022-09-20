using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Extensions;
using Abp.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Abp.Notifications
{
    /// <summary>
    /// Used to store a user notification.
    /// </summary>
    [Serializable]
    [Table("AbpUserNotifications")]
    public class UserNotificationInfo : Entity<Guid>, IHasCreationTime, IMayHaveTenant
    {
        public const int MaxTargetNotifiersLength = 1024;
        
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
        /// which realtime notifiers should handle this notification
        /// </summary>
        [StringLength(MaxTargetNotifiersLength)]
        public virtual string TargetNotifiers { get; set; }

        [NotMapped]
        public virtual List<string> TargetNotifiersList => TargetNotifiers.IsNullOrWhiteSpace()
            ? new List<string>()
            : TargetNotifiers.Split(NotificationInfo.NotificationTargetSeparator).ToList();

        public UserNotificationInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotificationInfo"/> class.
        /// </summary>
        /// <param name="id"></param>
        public UserNotificationInfo(Guid id)
        {
            Id = id;
            State = UserNotificationState.Unread;
            CreationTime = Clock.Now;
        }

        public virtual void SetTargetNotifiers(List<string> list)
        {
            TargetNotifiers = string.Join(NotificationInfo.NotificationTargetSeparator.ToString(), list);
        }
    }
}