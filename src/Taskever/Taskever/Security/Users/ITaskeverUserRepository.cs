using Abp.Security.Users;

namespace Taskever.Security.Users
{
    public interface ITaskeverUserRepository : IUserRepository<TaskeverUser>
    {

    }
}
