using Abp.Security.Identity;
using Taskever.Security.Users;

namespace Taskever.Security.Identity
{
    public class TaskeverUserManager : AbpUserManager<TaskeverUser>
    {
        public TaskeverUserManager(UserStore<TaskeverUser, ITaskeverUserRepository> store)
            : base(store)
        {

        }
    }
}