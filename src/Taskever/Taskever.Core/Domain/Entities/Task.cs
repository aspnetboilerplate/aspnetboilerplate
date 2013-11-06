using Abp.Modules.Core.Domain.Entities;
using Abp.Modules.Core.Domain.Entities.Utils;
using Taskever.Domain.Enums;

namespace Taskever.Domain.Entities
{
    /// <summary>
    /// Represents a task.
    /// </summary>
    public class Task : AuditedEntity, IHasTenant
    {
        /// <summary>
        /// The tenant account which this entity is belong to.
        /// </summary>
        public virtual Tenant Tenant { get; set; } 

        /// <summary>
        /// Task title.
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// Task description.
        /// </summary>
        public virtual string Description { get; set; }

        public virtual User AssignedUser { get; set; }

        public virtual TaskPriority Priority { get; set; }

        public virtual TaskPrivacy Privacy { get; set; }

        public virtual TaskState State { get; set; }

        public Task()
        {
            Priority = TaskPriority.Normal;
            Privacy = TaskPrivacy.Protected;
            State = TaskState.New;
        }
    }
}
