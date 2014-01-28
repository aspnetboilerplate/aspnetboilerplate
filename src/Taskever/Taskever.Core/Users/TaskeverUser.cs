using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Security.Users;
using Abp.Users;

namespace Taskever.Users
{
    public class TaskeverUser : User
    {
        public virtual string TaskField { get; set; }
    }
}
