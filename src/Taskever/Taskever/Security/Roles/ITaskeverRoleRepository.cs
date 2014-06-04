using Abp.Domain.Repositories;
using Abp.Security.Roles;

namespace Taskever.Security.Roles
{
    public interface ITaskeverRoleRepository : IRepository<TaskeverRole>
    {

    }
}