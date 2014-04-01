using Abp.Domain.Repositories.EntityFramework;
using Taskever.Activities;

namespace Taskever.Infrastructure.EntityFramework.Data.Repositories.NHibernate
{
    public class ActivityRepository : EfRepositoryBase<Activity, long>, IActivityRepository
    {

    }
}