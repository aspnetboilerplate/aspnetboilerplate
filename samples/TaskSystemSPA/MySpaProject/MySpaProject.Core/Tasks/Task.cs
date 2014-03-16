using System;
using Abp.Domain.Entities;
using MySpaProject.People;

namespace MySpaProject.Tasks
{
    public class Task : Entity<long> //, IHasCreationTime
    {
        public virtual Person AssignedPerson { get; set; }

        public virtual string Description { get; set; }

        public virtual DateTime CreationTime { get; set; }

        public virtual TaskState State { get; set; }

        public Task()
        {
            CreationTime = DateTime.Now;
            State = TaskState.Active;
        }
    }
}
