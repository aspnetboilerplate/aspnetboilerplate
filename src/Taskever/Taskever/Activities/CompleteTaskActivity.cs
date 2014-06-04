using System.ComponentModel.DataAnnotations.Schema;
using Abp.Security.Users;
using Taskever.Tasks;

namespace Taskever.Activities
{
    public class CompleteTaskActivity : Activity
    {

        public CompleteTaskActivity()
        {
            //ActivityType = ActivityType.CompleteTask;            
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