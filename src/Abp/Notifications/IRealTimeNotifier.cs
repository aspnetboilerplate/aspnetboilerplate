using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;

namespace Abp.Notifications
{
    /// <summary>
    /// Interface to send real time notifications to users.
    /// </summary>
    public interface IRealTimeNotifier
    {
        /// <summary>
        /// This method tries to deliver real time notifications to specified users.
        /// If a user is not online, it should ignore him.
        /// </summary>
        Task SendNotificationsAsync(UserNotification[] userNotifications);
    }

    public class UserNotification : EntityDto<Guid>
    {
        public long UserId { get; set; }

        public UserNotificationState State { get; set; }

        public Notification Notification { get; set; }
    }

    public class Notification : EntityDto<Guid>, IHasCreationTime
    {
        public string NotificationName { get; set; }

        public object Data { get; set; }

        public Type EntityType { get; set; }

        public string EntityTypeName { get; set; }

        public object EntityId { get; set; }

        public NotificationSeverity Severity { get; set; }

        public DateTime CreationTime { get; set; }
    }
}