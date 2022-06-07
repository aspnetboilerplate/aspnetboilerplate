using System;

namespace Abp.Notifications
{
    public class GetNotificationsCreatedByUserOutput
    {
        public string NotificationName { get; set; }

        public string Data { get; set; }

        public string DataTypeName { get; set; }

        public NotificationSeverity Severity { get; set; }

        public bool IsPublished { get; set; }
        
        public DateTime CreationTime { get; set; }
    }
}