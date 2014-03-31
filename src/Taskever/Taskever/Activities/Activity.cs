using System;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Security.Users;

namespace Taskever.Activities
{
    public abstract class Activity : Entity<long>, IHasCreationTime
    {
        public virtual ActivityType ActivityType { get; set; }

        public virtual DateTime CreationTime { get; set; }

        protected Activity()
        {
            CreationTime = DateTime.Now;
        }

        public abstract int?[] GetActors();

        public abstract int?[] GetRelatedUsers();
    }
}
