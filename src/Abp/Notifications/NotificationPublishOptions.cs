using System;
using Abp.Extensions;

namespace Abp.Notifications
{
    /// <summary>
    /// Notification publish properties.
    /// </summary>
    [Serializable]
    public class NotificationPublishOptions
    {
        /// <summary>
        /// Gets/sets notification name.
        /// </summary>
        public string NotificationName
        {
            get { return _notificationName; }
            set
            {
                if (value.IsNullOrWhiteSpace())
                {
                    throw new ArgumentException("NotificationName can not be null or whitespace!", "value");
                }

                _notificationName = value;
            }
        }
        private string _notificationName;

        /// <summary>
        /// Gets/sets notification data.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public NotificationData Data
        {
            get { return _data; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentException("Notification data (Data) can not be null or whitespace!", "value");
                }

                _data = value;
            }
        }
        private NotificationData _data;

        /// <summary>
        /// Gets/sets entity type, if this is an entity level notification.
        /// </summary>
        public Type EntityType { get; set; }

        /// <summary>
        /// Gets/sets primary key of the entity, if this is an entity level notification.
        /// </summary>
        public object EntityId { get; set; }

        /// <summary>
        /// Notification severity.
        /// </summary>
        public NotificationSeverity Severity { get; set; }

        /// <summary>
        /// Target users of the notification.
        /// If this is set, it overrides subscribed users.
        /// </summary>
        public long[] UserIds { get; set; }

        /// <summary>
        /// Default constructor (needed for serialization/deserialization)
        /// </summary>
        public NotificationPublishOptions()
        {
            
        }

        /// <summary>
        /// Creates a new <see cref="NotificationPublishOptions"/>.
        /// </summary>
        public NotificationPublishOptions(string notificationName, NotificationData data, Type entityType = null, object entityId = null, NotificationSeverity severity = NotificationSeverity.Info, long[] userIds = null)
        {
            NotificationName = notificationName;
            Data = data;
            EntityType = entityType;
            EntityId = entityId;
            Severity = severity;
            UserIds = userIds;
        }
    }
}