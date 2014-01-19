using Abp.Domain.Services;
using Abp.Users;

namespace Taskever.Friendships
{
    public interface IFriendshipDomainService : IDomainService
    {
        bool HasFriendship(User user, User probableFriend);
    }
}