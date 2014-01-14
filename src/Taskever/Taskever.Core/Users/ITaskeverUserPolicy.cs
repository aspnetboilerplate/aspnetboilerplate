using Abp.Modules.Core.Domain.Entities;

namespace Taskever.Users
{
    public interface ITaskeverUserPolicy
    {
        bool CanSeeProfile(User requesterUser, User targetUser);
    }
}
