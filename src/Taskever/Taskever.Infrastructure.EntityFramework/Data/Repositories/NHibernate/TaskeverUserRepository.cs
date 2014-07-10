using Abp.Modules.Core.Data.Repositories.EntityFramework;
using Taskever.Security.Users;

namespace Taskever.Infrastructure.EntityFramework.Data.Repositories.NHibernate
{
    public class TaskeverUserRepository : TaskeverEfRepositoryBase<TaskeverUser, long>, ITaskeverUserRepository
    {
        
    }
}