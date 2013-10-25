using System.Collections.Generic;
using Abp.Domain.Repositories;
using Taskever.Domain.Entities;

namespace Taskever.Data.Repositories
{
    public interface IFriendshipRepository : IRepository<Friendship>
    {
        List<Friendship> GetAllWithFriendUser(int userId, bool? canAssignTask);
    }
}