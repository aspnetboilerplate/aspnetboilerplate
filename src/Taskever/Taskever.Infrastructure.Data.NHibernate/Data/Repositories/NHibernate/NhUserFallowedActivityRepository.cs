using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Data.Repositories.NHibernate;
using Taskever.Domain.Entities;

namespace Taskever.Data.Repositories.NHibernate
{
    public class NhUserFallowedActivityRepository : NhRepositoryBase<UserFallowedActivity, long>, IUserFallowedActivityRepository
    {
        public IList<Activity> GetActivities(int fallowerUserId)
        {
            var oneWeekAgo = DateTime.Now.AddDays(-7); //TODO: Make 7 configurable?

            var query = from fallowedActivity in GetAll()
                        where fallowedActivity.User.Id == fallowerUserId && fallowedActivity.CreationTime >  oneWeekAgo
                        orderby fallowedActivity.CreationTime descending
                        select fallowedActivity.Activity;

            return query.Take(20).ToList(); //TODO: Make 20 configurable?
        }
    }
}