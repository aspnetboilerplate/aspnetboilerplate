using System;

namespace Abp.Notifications
{
    public class NotificationSubscriptionOptions
    {
        public string NotificationName { get; private set; }
        
        public Type EntityType { get; set; }
        
        public object EntityId { get; set; }

        public NotificationSubscriptionOptions(string notificationName, Type entityType = null, object entityId = null)
        {
            if (notificationName == null)
            {
                throw new ArgumentNullException("notificationName");
            }

            NotificationName = notificationName;
            EntityType = entityType;
            EntityId = entityId;
        }

        //TODO: We may add generic static factory methods.
    }
}