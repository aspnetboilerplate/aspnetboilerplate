using Abp.Domain.Policies;
using Abp.Domain.Services;
using Abp.Modules.Core.Domain.Entities;

namespace Taskever.Tasks
{
    //TODO: Renamt this to Policy ?
    public interface ITaskPolicy : IPolicy
    {
        bool CanSeeTasksOfUser(User requesterUser, User userOfTasks);

        bool CanAssignTask(User assignerUser, User userToAssign);

        bool CanUpdateTask(User user, Task task);

        bool CanDeleteTask(User user, Task task);
    }
}