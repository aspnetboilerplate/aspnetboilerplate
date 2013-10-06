using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abp.Domain.Repositories;
using Taskever.Domain.Entities;

namespace Taskever.Data.Repositories
{
    public interface ITaskRepository : IRepository<Task>
    {
    }
}
