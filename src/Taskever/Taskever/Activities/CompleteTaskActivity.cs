using Abp.Security.Users;
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

        public override int?[] GetActors()
        {
            return new [] { (int?)AssignedUser.Id };
        }

        public override int?[] GetRelatedUsers()
        {
            return new[] {Task.CreatorUserId};
        }
    }
}