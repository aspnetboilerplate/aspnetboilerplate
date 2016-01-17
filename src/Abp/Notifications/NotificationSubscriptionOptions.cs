using System;
using Abp.Extensions;

namespace Abp.Notifications
{
    public class NotificationSubscriptionOptions
    {
        public long UserId
        {
            get { return _userId; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("UserId should be a positive value!", "value");
                }

                _userId = value;
            }
        }
        private long _userId;

        public string NotificationName
        {
            get { return _notificationName; }
            set
            {
                if (value.IsNullOrWhiteSpace())
                {
                    throw new ArgumentNullException("value", "NotificationName can not be null or whitespace");
                }

                _notificationName = value;
            }
        }
        private string _notificationName;

        public NotificationSubscriptionOptions(long userId, string notificationName)
        {
            UserId = userId;
            NotificationName = notificationName;
        }

        public Type EntityType { get; set; }
        
        public object EntityId { get; set; }

        //TODO: We may add generic static factory methods.
    }
}