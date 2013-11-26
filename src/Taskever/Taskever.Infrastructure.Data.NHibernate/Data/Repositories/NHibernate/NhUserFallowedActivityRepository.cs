using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Data.Repositories.NHibernate;
using Taskever.Domain.Entities;
using Taskever.Domain.Entities.Activities;

namespace Taskever.Data.Repositories.NHibernate
{
    public class NhUserFollowedActivityRepository : NhRepositoryBase<UserFollowedActivity, long>, IUserFollowedActivityRepository
    {
        public IList<Activity> GetActivities(int fallowerUserId, int maxResultCount, long beforeFallowedActivityId)
        {
            var query = from fallowedActivity in GetAll()
                        where fallowedActivity.User.Id == fallowerUserId && fallowedActivity.Id < beforeFallowedActivityId
                        orderby fallowedActivity.Id descending
                        select fallowedActivity.Activity;

            return query.Take(maxResultCount).ToList();
        }
    }
}