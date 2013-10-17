using Abp.Domain.Services;
using Abp.Modules.Core.Domain.Entities;

namespace Taskever.Domain.Services
{
    public interface ITaskPrivilegeService : IDomainService
    {
        bool CanSeeTasksOfUser(User requesterUser, User userOfTasks);
    }
}