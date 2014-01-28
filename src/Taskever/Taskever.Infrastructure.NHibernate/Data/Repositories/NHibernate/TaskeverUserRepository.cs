using Abp.Modules.Core.Data.Repositories.NHibernate;
using Taskever.Users;

namespace Taskever.Data.Repositories.NHibernate
{
    public class TaskeverUserRepository : NhUserRepository<TaskeverUser>, ITaskeverUserRepository
    {
        
    }
}