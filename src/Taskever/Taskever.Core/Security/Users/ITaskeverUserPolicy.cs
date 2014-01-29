using Abp.Domain.Policies;
using Abp.Security.Users;

namespace Taskever.Security.Users
{
    public interface ITaskeverUserPolicy : IPolicy
    {
        bool CanSeeProfile(AbpUser requesterUser, AbpUser targetUser);
    }
}
