using Abp.Security.Users;
using Abp.Users;
using Taskever.Tasks;

namespace Taskever.Activities
{
    public class CreateTaskActivity : Activity
    {
        public virtual AbpUser CreatorUser { get; set; }

        public virtual AbpUser AssignedUser { get; set; }

        public virtual Task Task { get; set; } //TODO: Create abstract TaskActivity class and put Task there?

        public CreateTaskActivity()
        {
            ActivityType = ActivityType.CreateTask;
        }

        public override AbpUser[] GetActors()
        {
            return new [] {CreatorUser, AssignedUser};
        }

        public override AbpUser[] GetRelatedUsers()
        {
            return new AbpUser[] { };
        }
    }
}