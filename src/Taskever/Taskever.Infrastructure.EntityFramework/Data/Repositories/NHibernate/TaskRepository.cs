using Abp.Domain.Repositories.EntityFramework;
using Taskever.Tasks;

namespace Taskever.Infrastructure.EntityFramework.Data.Repositories.NHibernate
{
    public class TaskRepository : EfRepositoryBase<Task>, ITaskRepository
    {

    }
}
