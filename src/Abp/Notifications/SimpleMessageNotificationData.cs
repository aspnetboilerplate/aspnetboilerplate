using System;

namespace Abp.Notifications
{
    /// <summary>
    /// Can be used to store a simple message as notification data.
    /// </summary>
    [Serializable]
    public class SimpleMessageNotificationData : NotificationData
    {
        /// <summary>
        /// The message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Needed for serialization.
        /// </summary>
        private SimpleMessageNotificationData()
        {
            
        }

        public SimpleMessageNotificationData(string message)
        {
            Message = message;
        }
    }
}