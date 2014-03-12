using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Repositories.NHibernate;
using MySpaProject.Tasks;
using NHibernate.Linq;

namespace MySpaProject.NHibernate.Repositories
{
    public class TaskRepository : NhRepositoryBase<Task, long>, ITaskRepository
    {
        public List<Task> GetAllWithPeople()
        {
            return GetAll().Fetch(task => task.AssignedPerson).ToList();
        }
    }
}
