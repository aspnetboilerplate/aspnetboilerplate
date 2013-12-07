using Abp.Domain.Repositories;
using Taskever.Domain.Entities;

namespace Taskever.Domain.Repositories
{
    public interface ITaskRepository : IRepository<Task>
    {
    }
}
