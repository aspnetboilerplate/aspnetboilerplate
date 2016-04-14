using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;

namespace Abp.Notifications
{
    /// <summary>
    /// Represents a user subscription to a notification.
    /// </summary>
    public class NotificationSubscription : IHasCreationTime
    {
        /// <summary>
        /// User Id.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Notification unique name.
        /// </summary>
        public string NotificationName { get; set; }

        /// <summary>
        /// Entity type.
        /// </summary>
        public Type EntityType { get; set; }

        /// <summary>
        /// Name of the entity type (including namespaces).
        /// </summary>
        public string EntityTypeName { get; set; }

        /// <summary>
        /// Entity Id.
        /// </summary>
        public object EntityId { get; set; }

        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationSubscription"/> class.
        /// </summary>
        public NotificationSubscription()
        {
            CreationTime = Clock.Now;
        }
    }
}