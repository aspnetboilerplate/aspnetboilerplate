using System;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;

namespace Abp.Notifications
{
    public class Notification : EntityDto<Guid>, IHasCreationTime
    {
        public string NotificationName { get; set; }

        public object Data { get; set; }

        public Type EntityType { get; set; }

        public string EntityTypeName { get; set; }

        public object EntityId { get; set; }

        public NotificationSeverity Severity { get; set; }

        public DateTime CreationTime { get; set; }
    }
}