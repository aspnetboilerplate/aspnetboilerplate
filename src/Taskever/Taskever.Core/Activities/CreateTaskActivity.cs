using Abp.Users;
using Taskever.Tasks;

namespace Taskever.Activities
{
    public class CreateTaskActivity : Activity
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

        public override User[] GetRelatedUsers()
        {
            return new User[] { };
        }
    }
}