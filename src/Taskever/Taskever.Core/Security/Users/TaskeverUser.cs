using Abp.Security.Users;

namespace Taskever.Security.Users
{
    public class TaskeverUser : AbpUser
    {
        public virtual string TaskField { get; set; }
    }
}
