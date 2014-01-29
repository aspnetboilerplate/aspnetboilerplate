using Abp.Domain.Repositories.NHibernate;
using Taskever.Activities;

namespace Taskever.Data.Repositories.NHibernate
{
    public class ActivityRepository : NhRepositoryBase<Activity, long>, IActivityRepository
    {

    }
}