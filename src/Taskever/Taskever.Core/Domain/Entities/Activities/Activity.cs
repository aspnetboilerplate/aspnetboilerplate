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
    }

    public class CreateTaskActivity : Activity //: TaskActivity
    {
        public virtual User CreatorUser { get; set; }

        public virtual User AssignedUser { get; set; }

        public virtual Task Task { get; set; } //TODO: Create abstract TaskActivity class and put Task there?

        public CreateTaskActivity()
        {
            ActivityType = ActivityType.CreateTask;
        }

        public override User[] GetActors()
        {
            return new [] {CreatorUser, AssignedUser};
        }
    }

    public class CompleteTaskActivity : Activity //: TaskActivity
    {
        public virtual User AssignedUser { get; set; }

        public virtual Task Task { get; set; }

        public CompleteTaskActivity()
        {
            ActivityType = ActivityType.CompleteTask;            
        }

        public override User[] GetActors()
        {
            return new [] { AssignedUser };
        }
    }

    public enum ActivityType
    {
        CreateTask = 1,
        CompleteTask = 2
    }
}
