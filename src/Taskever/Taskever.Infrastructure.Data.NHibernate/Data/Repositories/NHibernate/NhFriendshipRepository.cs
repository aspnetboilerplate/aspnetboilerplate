using System.Collections.Generic;
using System.Linq;
using Abp.Data.Repositories.NHibernate;
using NHibernate.Linq;
using Taskever.Domain.Entities;
using Taskever.Domain.Enums;

namespace Taskever.Data.Repositories.NHibernate
{
    public class NhFriendshipRepository : NhRepositoryBase<Friendship>, IFriendshipRepository
    {
        public List<Friendship> GetAllWithFriendUser(int userId, FriendshipStatus? status, bool? canAssignTask)
        {
            var query = GetAll()
                .Fetch(f => f.Friend)
                .Where(f => f.User.Id == userId);

            if (status.HasValue)
            {
                query = query.Where(friendship => friendship.Status == status.Value);
            }

            if (canAssignTask.HasValue)
            {
                query = query.Where(friendship => friendship.CanAssignTask == canAssignTask);
            }

            return query.ToList();
        }
    }
}