using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Security.Users;
using Taskever.Tasks;

namespace Taskever.Activities
{
    public abstract class Activity : Entity<long>, IHasCreationTime
    {
        //public virtual ActivityType ActivityType { get; set; }

        [ForeignKey("AssignedUserId")]
        public virtual AbpUser AssignedUser { get; set; }
        public virtual long AssignedUserId { get; set; }

        [ForeignKey("TaskId")]
        public virtual Task Task { get; set; }
        public virtual int TaskId { get; set; }


        public virtual DateTime CreationTime { get; set; }

        protected Activity()
        {
            CreationTime = DateTime.Now;
        }

        public abstract long?[] GetActors();

        public abstract long?[] GetRelatedUsers();
    }
}
