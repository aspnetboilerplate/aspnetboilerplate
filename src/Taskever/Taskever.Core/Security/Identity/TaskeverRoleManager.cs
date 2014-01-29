using Abp.Security.Identity;
using Taskever.Security.Roles;

namespace Taskever.Security.Identity
{
    public class TaskeverRoleManager : AbpRoleManagerBase<TaskeverRole>
    {
        public TaskeverRoleManager(RoleStore<TaskeverRole, ITaskeverRoleRepository> store)
            : base(store)
        {

        }
    }
}