using System;
using Abp.Localization;

namespace Abp.Notifications
{
    /// <summary>
    /// Can be used to store a simple message as notification data.
    /// </summary>
    [Serializable]
    public class LocalizableMessageNotificationData : NotificationData
    {
        /// <summary>
        /// The message.
        /// </summary>
        public LocalizableString Message { get; private set; }

        /// <summary>
        /// Needed for serialization.
        /// </summary>
        private LocalizableMessageNotificationData()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizableMessageNotificationData"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public LocalizableMessageNotificationData(LocalizableString message)
        {
            Message = message;
        }
    }
}