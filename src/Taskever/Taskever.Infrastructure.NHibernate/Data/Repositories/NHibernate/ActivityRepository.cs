using Taskever.Activities;
using Taskever.Data.Repositories.NHibernate.Base;

namespace Taskever.Data.Repositories.NHibernate
{
    public class ActivityRepository : TaskeverRepositoryBase<Activity, long>, IActivityRepository
    {

    }
}