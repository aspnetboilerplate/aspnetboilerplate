using System;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities.Auditing;

namespace Abp.Notifications
{
    /// <summary>
    /// Represents a published/sent notification.
    /// </summary>
    [Serializable]
    public class NotificationInfo : CreationAuditedEntity<Guid>
    {
        /// <summary>
        /// <see cref="NotificationDefinition.Name"/>.
        /// </summary>
        [Required]
        public virtual string NotificationName { get; set; }

        /// <summary>
        /// Notification data as JSON string.
        /// </summary>
        public virtual string Data { get; set; }

        /// <summary>
        /// Gets/sets entity type, if this is an entity level notification.
        /// </summary>
        public virtual Type EntityType { get; set; }

        /// <summary>
        /// Gets/sets primary key of the entity, if this is an entity level notification.
        /// </summary>
        public virtual object EntityId { get; set; }

        /// <summary>
        /// Notification severity.
        /// </summary>
        public virtual NotificationSeverity Severity { get; set; }

        /// <summary>
        /// Target users of the notification.
        /// If this is set, it overrides subscribed users.
        /// If this is null/empty, then notification is sent to all subscribed users.
        /// </summary>
        public virtual string UserIds { get; set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationInfo"/> class.
        /// </summary>
        public NotificationInfo()
        {
            Severity = NotificationSeverity.Info;
            Data = "{}";
        }
    }
}