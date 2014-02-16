using Abp.Domain.Services;
using Abp.Security.Users;
using Abp.Users;

namespace Taskever.Friendships
{
    public interface IFriendshipDomainService : IDomainService
    {
        bool HasFriendship(AbpUser user, AbpUser probableFriend);
    }
}