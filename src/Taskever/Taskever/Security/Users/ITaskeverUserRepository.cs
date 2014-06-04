using Abp.Domain.Repositories;
using Abp.Security.Users;

namespace Taskever.Security.Users
{
    public interface ITaskeverUserRepository : IRepository<TaskeverUser>
    {

    }
}
