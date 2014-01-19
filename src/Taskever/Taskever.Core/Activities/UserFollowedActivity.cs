using System;
using Abp.Domain.Entities;
using Abp.Users;

namespace Taskever.Activities
{
    public class UserFollowedActivity : Entity<long>
    {
        public virtual User User { get; set; }

        public virtual Activity Activity { get; set; }

        public virtual bool IsActor { get; set; }

        public virtual bool IsRelated { get; set; }

        public virtual DateTime CreationTime { get; set; }

        public UserFollowedActivity()
        {
            CreationTime = DateTime.Now;
        }
    }
}
