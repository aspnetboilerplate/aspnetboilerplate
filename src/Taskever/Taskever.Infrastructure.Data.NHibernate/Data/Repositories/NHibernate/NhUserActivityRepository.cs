using Abp.Data.Repositories.NHibernate;
using Taskever.Domain.Entities;

namespace Taskever.Data.Repositories.NHibernate
{
    public class NhUserActivityRepository : NhRepositoryBase<UserActivity, long>, IUserActivityRepository
    {

    }
}