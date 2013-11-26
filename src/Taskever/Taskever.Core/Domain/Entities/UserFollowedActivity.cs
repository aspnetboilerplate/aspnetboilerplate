using System;
using Abp.Domain.Entities;
using Abp.Modules.Core.Domain.Entities;
using Taskever.Domain.Entities.Activities;

namespace Taskever.Domain.Entities
{
    public class UserFollowedActivity : Entity<long>
    {
        public virtual User User { get; set; }

        public virtual Activity Activity { get; set; }

        public virtual DateTime CreationTime { get; set; }

        public virtual bool IsActor { get; set; }

        public UserFollowedActivity()
        {
            CreationTime = DateTime.Now;
        }
    }
}
