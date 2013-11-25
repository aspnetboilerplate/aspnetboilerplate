using Abp.Data.Repositories.NHibernate;
using Taskever.Domain.Entities;
using Taskever.Domain.Entities.Activities;

namespace Taskever.Data.Repositories.NHibernate
{
    public class NhActivityRepository : NhRepositoryBase<Activity, long>, IActivityRepository
    {

    }
}