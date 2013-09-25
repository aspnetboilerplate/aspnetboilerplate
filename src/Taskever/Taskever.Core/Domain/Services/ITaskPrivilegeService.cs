using Abp.Domain.Services;

namespace Taskever.Domain.Services
{
    public interface ITaskPrivilegeService : IDomainService
    {
        bool CanSeeTasksOfUser(int currentUserId, int userId);
    }
}