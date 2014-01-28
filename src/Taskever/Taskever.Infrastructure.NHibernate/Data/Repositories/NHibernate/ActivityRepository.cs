using Abp.Domain.Repositories.NHibernate;
using Taskever.Activities;
using Taskever.Data.Repositories.NHibernate.Base;

namespace Taskever.Data.Repositories.NHibernate
{
    public class ActivityRepository : NhRepositoryBase<Activity, long>, IActivityRepository
    {

    }
}