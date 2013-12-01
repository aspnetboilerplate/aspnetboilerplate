using System;
using Abp.Domain.Entities;
using Abp.Modules.Core.Domain.Entities;

namespace Taskever.Domain.Entities.Activities
{
    public abstract class Activity : Entity<long>
    {
        public virtual ActivityType ActivityType { get; set; }

        public virtual DateTime CreationTime { get; set; }

        protected Activity()
        {
            CreationTime = DateTime.Now;
        }

        public abstract User[] GetActors();

        public abstract User[] GetRelatedUsers();
    }
}
