using Abp.Domain.Policies;
using Abp.Security.Users;

namespace Taskever.Tasks
{
    //TODO: Renamt this to Policy ?
    public interface ITaskPolicy : IPolicy
    {
        bool CanSeeTasksOfUser(AbpUser requesterUser, AbpUser userOfTasks);

        bool CanAssignTask(AbpUser assignerUser, AbpUser userToAssign);

        bool CanUpdateTask(AbpUser user, Task task);

        bool CanDeleteTask(AbpUser user, Task task);
    }
}