using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Security.Users;
using Taskever.Security.Users;
using Taskever.Tasks;

namespace Taskever.Activities
{
    public class CreateTaskActivity : Activity
    {
        [ForeignKey("CreatorUserId")]
        public virtual TaskeverUser CreatorUser { get; set; }

        public virtual long CreatorUserId { get; set; }

        public CreateTaskActivity()
        {
            //ActivityType = ActivityType.CreateTask;
        }

        public override long?[] GetActors()
        {
            return new long?[] { CreatorUser.Id, AssignedUser.Id };
        }

        public override long?[] GetRelatedUsers()
        {
            return new long?[] { };
        }
    }
}