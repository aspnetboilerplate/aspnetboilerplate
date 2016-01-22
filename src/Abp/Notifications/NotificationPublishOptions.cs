using System;

namespace Abp.Notifications
{
    public class NotificationPublishOptions
    {
        public NotificationInfo Notification { get; private set; }

        public NotificationPublishOptions(NotificationInfo notification)
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