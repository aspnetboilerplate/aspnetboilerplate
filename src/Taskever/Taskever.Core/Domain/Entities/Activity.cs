using System;
using Abp.Domain.Entities;
using Abp.Modules.Core.Domain.Entities;
using Taskever.Domain.Enums;

namespace Taskever.Domain.Entities
{
    public class Activity : Entity<long>
    {
        public virtual User ActorUser { get; set; }

        public virtual ActivityAction Action { get; set; }

        public virtual string Data { get; set; }

        public virtual DateTime CreationTime { get; set; }

        public Activity()
        {
            CreationTime = DateTime.Now;
        }
    }
}
