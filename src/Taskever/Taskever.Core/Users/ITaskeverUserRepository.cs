using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Security.Users;
using Abp.Users;

namespace Taskever.Users
{
    public interface ITaskeverUserRepository : IUserRepository<TaskeverUser>
    {

    }
}
