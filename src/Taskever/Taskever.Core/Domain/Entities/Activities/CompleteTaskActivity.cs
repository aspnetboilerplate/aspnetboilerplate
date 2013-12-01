using Abp.Modules.Core.Domain.Entities;

namespace Taskever.Domain.Entities.Activities
{
    public class CompleteTaskActivity : Activity
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

        public override User[] GetRelatedUsers()
        {
            return new[] {Task.CreatorUser};
        }
    }
}