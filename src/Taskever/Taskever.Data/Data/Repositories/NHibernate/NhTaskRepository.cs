using Abp.Data.Repositories.NHibernate;
using Taskever.Entities;

namespace Taskever.Data.Repositories.NHibernate
{
    public class NhTaskRepository : NhRepositoryBase<Task, int>, ITaskRepository
    {

    }
}
