using System;
using System.Collections.Generic;
using Abp.Domain.Entities.Auditing;
using Abp.Localization;
using Abp.Timing;

namespace Abp.Notifications
{
    /// <summary>
    /// Represents a published/sent notification.
    /// </summary>
    [Serializable]
    public class NotificationInfo : IHasCreationTime
    {
        /// <summary>
        /// Id of this object.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// <see cref="NotificationDefinition.Name"/>.
        /// </summary>
        public string NotificationName { get; set; }
        
        /// <summary>
        /// Notification title.
        /// </summary>
        public ILocalizableString Title { get; set; } //TODO: Make localization optional?

        /// <summary>
        /// Format arguments if Title contains format parameters like {ParameterName}.
        /// </summary>
        public Dictionary<string, object> TitleArgs { get; set; }

        /// <summary>
        /// Notification body.
        /// </summary>
        public ILocalizableString Body { get; set; } //TODO: Make localization optional?

        /// <summary>
        /// Format arguments if Body contains format parameters like {ParameterName}.
        /// </summary>
        public Dictionary<string, object> BodyArgs { get; set; }

        /// <summary>
        /// Notification type.
        /// </summary>
        public NotificationType Type { get; set; }

        public DateTime CreationTime { get; set; }

        public NotificationState State { get; set; }
        
        public long[] UserIds { get; set; }

        public NotificationInfo() //TODO: Add constructor parameters...
        {
            Id = Guid.NewGuid();
            Type = NotificationType.Info;
            CreationTime = Clock.Now;
        }
    }
}