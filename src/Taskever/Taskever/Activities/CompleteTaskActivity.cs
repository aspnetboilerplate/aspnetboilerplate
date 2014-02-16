using Abp.Security.Users;
using Abp.Users;
using Taskever.Tasks;

namespace Taskever.Activities
{
    public class CompleteTaskActivity : Activity
    {
        public virtual AbpUser AssignedUser { get; set; }

        public virtual Task Task { get; set; }

        public CompleteTaskActivity()
        {
            ActivityType = ActivityType.CompleteTask;            
        }

        public override AbpUser[] GetActors()
        {
            return new [] { AssignedUser };
        }

        public override AbpUser[] GetRelatedUsers()
        {
            return new[] {Task.CreatorUser};
        }
    }
}