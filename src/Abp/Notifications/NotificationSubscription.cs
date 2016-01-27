using System;
using Abp.Domain.Entities.Auditing;

namespace Abp.Notifications
{
    public class NotificationSubscription : IHasCreationTime
    {
        public long UserId { get; set; }

        public string NotificationName { get; set; }

        public Type EntityType { get; set; }

        public object EntityId { get; set; }

        public DateTime CreationTime { get; set; }
    }
}