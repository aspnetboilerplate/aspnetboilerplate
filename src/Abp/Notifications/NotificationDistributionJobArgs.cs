using System;

namespace Abp.Notifications
{
    [Serializable]
    public class NotificationDistributionJobArgs
    {
        public Guid NotificationId { get; set; }

        public NotificationDistributionJobArgs(Guid notificationId)
        {
            NotificationId = notificationId;
        }
    }
}