using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Security.Users;

namespace Taskever.Tasks
{
    /// <summary>
    /// Represents a task.
    /// </summary>
    public class Task : AuditedEntity
    {
        /// <summary>
        /// Task title.
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// Task description.
        /// </summary>
        public virtual string Description { get; set; }

        public virtual AbpUser AssignedUser { get; set; }

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
