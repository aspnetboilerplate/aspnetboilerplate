using System.Collections.Generic;
using System.Linq;
using Abp.Data.Repositories.NHibernate;
using NHibernate.Linq;
using Taskever.Domain.Entities;

namespace Taskever.Data.Repositories.NHibernate
{
    public class NhFriendshipRepository : NhRepositoryBase<Friendship>, IFriendshipRepository
    {
        public List<Friendship> GetAllWithFriendUser(int userId, bool? canAssignTask)
        {
            var qr = GetAll()
                .Fetch(f => f.Friend)
                .Where(f => f.User.Id == userId && f.Status == FriendshipStatus.Accepted);

            if (canAssignTask.HasValue)
            {
                qr = qr.Where(friendship => friendship.CanAssignTask == canAssignTask);
            }

            return qr.ToList();
        }
    }
}