using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Repositories;
using Taskever.Domain.Entities;
using Taskever.Domain.Enums;

namespace Taskever.Data.Repositories
{
    public interface IFriendshipRepository : IRepository<Friendship>
    {
        List<Friendship> GetAllWithFriendUser(int userId, FriendshipStatus? status, bool? canAssignTask);

        IQueryable<Friendship> GetAllWithFriendUser(int userId);

        Friendship GetOrNull(int userId, int friendId, bool onlyAccepted = false);
    }
}