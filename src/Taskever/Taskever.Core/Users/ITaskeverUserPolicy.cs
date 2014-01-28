using Abp.Domain.Policies;
using Abp.Security.Users;
using Abp.Users;

namespace Taskever.Users
{
    public interface ITaskeverUserPolicy : IPolicy
    {
        bool CanSeeProfile(User requesterUser, User targetUser);
    }
}
