using Abp.Domain.Repositories.NHibernate;
using Taskever.Activities;

namespace Taskever.Data.Repositories.NHibernate
{
    public class NhActivityRepository : NhRepositoryBase<Activity, long>, IActivityRepository
    {

    }
}