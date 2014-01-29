using Abp.Domain.Repositories.NHibernate;
using Taskever.Tasks;

namespace Taskever.Data.Repositories.NHibernate
{
    public class TaskRepository : NhRepositoryBase<Task>, ITaskRepository
    {

    }
}
