using System.Collections.Generic;
using Abp.Domain.Repositories;
using Taskever.Domain.Entities;
using Taskever.Domain.Enums;

namespace Taskever.Data.Repositories
{
    public interface IFriendshipRepository : IRepository<Friendship>
    {
        List<Friendship> GetAllWithFriendUser(int userId, FriendshipStatus? status, bool? canAssignTask);

        Friendship GetOrNull(int userId, int friendId);
    }
}