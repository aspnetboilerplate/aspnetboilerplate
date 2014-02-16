using Abp.Security.Roles;

namespace Taskever.Security.Roles
{
    public interface ITaskeverRoleRepository : IRoleRepository<TaskeverRole>
    {
    }
}