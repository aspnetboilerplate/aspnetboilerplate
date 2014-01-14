using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Repositories.NHibernate;
using NHibernate.Linq;
using Taskever.Friendships;

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

            query = query.OrderBy(f => f.Friend.Name).ThenBy(f => f.Friend.Surname);

            return query.ToList();
        }

        public IQueryable<Friendship> GetAllWithFriendUser(int userId)
        {
            return GetAll()
                .Fetch(f => f.Friend)
                .Where(f => f.User.Id == userId);
        }

        public Friendship GetOrNull(int userId, int friendId, bool onlyAccepted = false)
        {
            var query = from friendship in GetAll()
                        where friendship.User.Id == userId && friendship.Friend.Id == friendId
                        select friendship;

            if (onlyAccepted)
            {
                query = query.Where(f => f.Status == FriendshipStatus.Accepted);
            }
            
            return query.FirstOrDefault();
        }
    }
}