using Abp.Domain.Services;
using Abp.Modules.Core.Domain.Entities;

namespace Taskever.Domain.Services
{
    public interface IFriendshipDomainService : IDomainService
    {
        bool HasFriendship(User user, User probableFriend);
    }
}