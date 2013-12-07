using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Data.Repositories.NHibernate;
using NHibernate.Linq;
using Taskever.Domain.Entities;
using Taskever.Domain.Entities.Activities;
using Taskever.Domain.Repositories;

namespace Taskever.Data.Repositories.NHibernate
{
    public class NhUserFollowedActivityRepository : NhRepositoryBase<UserFollowedActivity, long>, IUserFollowedActivityRepository
    {
        public IQueryable<UserFollowedActivity> GetAllWithActivity()
        {
            return GetAll().Fetch(fa => fa.Activity);
        }
    }
}