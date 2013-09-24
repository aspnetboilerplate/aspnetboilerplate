using Abp.Services;

namespace Taskever.Services
{
    public interface ITaskPrivilegeService : IService
    {
        bool CanSeeTasksOfUser(int currentUserId, int userId);
    }
}