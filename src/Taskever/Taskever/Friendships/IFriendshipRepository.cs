using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Repositories;

namespace Taskever.Friendships
{
    public interface IFriendshipRepository : IRepository<Friendship>
    {
        List<Friendship> GetAllWithFriendUser(long userId, FriendshipStatus? status, bool? canAssignTask);

        IQueryable<Friendship> GetAllWithFriendUser(long userId);

        Friendship GetOrNull(long userId, long friendId, bool onlyAccepted = false);
    }
}