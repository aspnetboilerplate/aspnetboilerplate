using System;

namespace Abp.Notifications
{
    public class NotificationPublishOptions
    {
        public Notification Notification { get; private set; }

        public NotificationPublishOptions(Notification notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException("notification");
            }

            Notification = notification;
        }

        //TODO ...
    }
}