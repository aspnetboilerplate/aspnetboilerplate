using Abp.Security.Users;
using Taskever.Security.Users;
using Taskever.Tasks;

namespace Taskever.Activities
{
    public class CreateTaskActivity : Activity
    {
        public virtual TaskeverUser CreatorUser { get; set; }

        public virtual TaskeverUser AssignedUser { get; set; }

        public virtual Task Task { get; set; } //TODO: Create abstract TaskActivity class and put Task there?

        public CreateTaskActivity()
        {
            ActivityType = ActivityType.CreateTask;
        }

        public override int?[] GetActors()
        {
            return new[] { (int?)CreatorUser.Id, (int?)AssignedUser.Id };
        }

        public override int?[] GetRelatedUsers()
        {
            return new int?[] { };
        }
    }
}