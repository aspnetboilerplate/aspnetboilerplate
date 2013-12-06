using Abp.Domain.Services;
using Abp.Modules.Core.Domain.Entities;
using Taskever.Domain.Entities;

namespace Taskever.Domain.Services
{
    //TODO: Renamt this to Policy ?
    public interface ITaskPrivilegeService : IDomainService
    {
        bool CanSeeTasksOfUser(User requesterUser, User userOfTasks);

        bool CanAssignTask(User assignerUser, User userToAssign);

        bool CanUpdateTask(User user, Task task);

        bool CanDeleteTask(User user, Task task);
    }
}