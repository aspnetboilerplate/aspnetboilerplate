using Abp.Domain.Repositories.NHibernate;
using Taskever.Domain.Entities;
using Taskever.Domain.Repositories;

namespace Taskever.Data.Repositories.NHibernate
{
    public class NhTaskRepository : NhRepositoryBase<Task>, ITaskRepository
    {

    }
}
