using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;

namespace Abp.Notifications
{
    [Serializable]
    [Table("AbpUserNotifications")]
    public class UserNotificationInfo : Entity<Guid>, IHasCreationTime
    {
        public virtual long UserId { get; set; }

        [Required]
        public virtual Guid NotificationId { get; set; }

        public virtual UserNotificationState State { get; set; }

        public virtual DateTime CreationTime { get; set; }

        public UserNotificationInfo()
        {
            Id = Guid.NewGuid();
            State = UserNotificationState.Unread;
            CreationTime = Clock.Now;
        }

        public UserNotificationInfo(long userId, Guid notificationId)
            : this()
        {
            UserId = userId;
            NotificationId = notificationId;
        }
    }
}