using Abp.Localization;

namespace Abp.Notifications
{
    /// <summary>
    /// Represents a published/sent notification.
    /// </summary>
    public class Notification
    {
        //TODO: Id...?

        /// <summary>
        /// <see cref="NotificationDefinition.Name"/>.
        /// </summary>
        public string NotificationName { get; set; }
        
        /// <summary>
        /// Notification title.
        /// </summary>
        public ILocalizableString Title { get; set; } //TODO: Make localization optional?

        /// <summary>
        /// Format arguments if Title contains format parameters like {0}.
        /// </summary>
        public object[] TitleArgs { get; set; }

        /// <summary>
        /// Notification body.
        /// </summary>
        public ILocalizableString Body { get; set; } //TODO: Make localization optional?

        /// <summary>
        /// Format arguments if Body contains format parameters like {0}.
        /// </summary>
        public object[] BodyArgs { get; set; }

        /// <summary>
        /// Notification type.
        /// </summary>
        public NotificationType Type { get; set; }

        public Notification() //TODO: Add constructor parameters...
        {
            Type = NotificationType.Info;
        }
    }
}