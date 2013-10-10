using System;
using Abp.Domain.Entities;
using Abp.Modules.Core.Domain.Entities;

namespace Taskever.Domain.Entities
{
    public class UserFallowedActivity : Entity<long>
    {
        public virtual User User { get; set; }

        public virtual Activity Activity { get; set; }

        public virtual DateTime CreationTime { get; set; }

        public UserFallowedActivity()
        {
            CreationTime = DateTime.Now;
        }
    }
}
