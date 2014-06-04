using System.ComponentModel.DataAnnotations.Schema;
using Abp.Security.Users;
using Taskever.Security.Users;
using Taskever.Tasks;

namespace Taskever.Activities
{
    public class CreateTaskActivity : Activity
    {
        [ForeignKey("CreatorUserId")]
        public virtual TaskeverUser CreatorUser { get; set; }

        public virtual int CreatorUserId { get; set; }

        public CreateTaskActivity()
        {
            //ActivityType = ActivityType.CreateTask;
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