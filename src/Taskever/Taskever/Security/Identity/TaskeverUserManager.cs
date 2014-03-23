using Abp.Security.IdentityFramework;
using Taskever.Security.Users;

namespace Taskever.Security.Identity
{
    public class TaskeverUserManager : AbpUserManagerBase<TaskeverUser>
    {
        public TaskeverUserManager(UserStore<TaskeverUser, ITaskeverUserRepository> store)
            : base(store)
        {
            
        }
    }
}