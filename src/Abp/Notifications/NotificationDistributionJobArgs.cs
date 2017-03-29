using System;

namespace Abp.Notifications
{
    /// <summary>
    /// Arguments for <see cref="NotificationDistributionJob"/>.
    /// </summary>
    [Serializable]
    public class NotificationDistributionJobArgs
    {
        /// <summary>
        /// Notification Id.
        /// </summary>
        public Guid NotificationId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationDistributionJobArgs"/> class.
        /// </summary>
        public NotificationDistributionJobArgs(Guid notificationId)
        {
            NotificationId = notificationId;
        }
    }
}