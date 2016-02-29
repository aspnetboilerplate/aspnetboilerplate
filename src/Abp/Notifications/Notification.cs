using System;
using Adorable.Application.Services.Dto;
using Adorable.Domain.Entities.Auditing;
using Adorable.Timing;

namespace Adorable.Notifications
{
    /// <summary>
    /// Represents a published notification.
    /// </summary>
    [Serializable]
    public class Notification : EntityDto<Guid>, IHasCreationTime
    {
        /// <summary>
        /// Unique notification name.
        /// </summary>
        public string NotificationName { get; set; }

        /// <summary>
        /// Notification data.
        /// </summary>
        public NotificationData Data { get; set; }

        /// <summary>
        /// Gets or sets the type of the entity.
        /// </summary>
        public Type EntityType { get; set; }

        /// <summary>
        /// Name of the entity type (including namespaces).
        /// </summary>
        public string EntityTypeName { get; set; }

        /// <summary>
        /// Entity id.
        /// </summary>
        public object EntityId { get; set; }

        /// <summary>
        /// Severity.
        /// </summary>
        public NotificationSeverity Severity { get; set; }

        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Notification"/> class.
        /// </summary>
        public Notification()
        {
            CreationTime = Clock.Now;
        }
    }
}