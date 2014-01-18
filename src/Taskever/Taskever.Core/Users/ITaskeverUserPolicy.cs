using Abp.Domain.Policies;
using Abp.Modules.Core.Domain.Entities;

namespace Taskever.Users
{
    public interface ITaskeverUserPolicy : IPolicy
    {
        bool CanSeeProfile(User requesterUser, User targetUser);
    }
}
