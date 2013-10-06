using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abp.Data.Repositories.NHibernate;
using Taskever.Domain.Entities;

namespace Taskever.Data.Repositories.NHibernate
{
    public class NhTaskRepository : NhRepositoryBase<Task>, ITaskRepository
    {

    }
}
