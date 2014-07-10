using Abp.Domain.Repositories.NHibernate;
using Abp.Modules.Core.Data.Repositories.NHibernate;
using Taskever.Security.Users;

namespace Taskever.Data.Repositories.NHibernate
{
    public class TaskeverUserRepository : NhRepositoryBase<TaskeverUser, long>, ITaskeverUserRepository
    {
        
    }
}