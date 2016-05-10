using System;

namespace Abp.Notifications
{
    /// <summary>
    ///     Can be used to store a simple message as notification data.
    /// </summary>
    [Serializable]
    public class MessageNotificationData : NotificationData
    {
        /// <summary>
        ///     Needed for serialization.
        /// </summary>
        private MessageNotificationData()
        {
        }

        public MessageNotificationData(string message)
        {
            Message = message;
        }

        /// <summary>
        ///     The message.
        /// </summary>
        public string Message { get; set; }
    }
}