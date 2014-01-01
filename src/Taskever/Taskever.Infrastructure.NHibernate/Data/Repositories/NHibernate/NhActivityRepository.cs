using Abp.Domain.Repositories.NHibernate;
using Taskever.Domain.Entities;
using Taskever.Domain.Entities.Activities;
using Taskever.Domain.Repositories;

namespace Taskever.Data.Repositories.NHibernate
{
    public class NhActivityRepository : NhRepositoryBase<Activity, long>, IActivityRepository
    {

    }
}