using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Repositories.NHibernate;
using MySpaProject.Tasks;
using NHibernate.Linq;

namespace MySpaProject.NHibernate.Repositories
{
    public class TaskRepository : NhRepositoryBase<Task, long>, ITaskRepository
    {
        public List<Task> GetAllWithPeople(int? assignedPersonId, TaskState? state)
        {
            var query = GetAll();
            
            if (assignedPersonId.HasValue)
            {
                query = query.Where(task => task.AssignedPerson.Id == assignedPersonId.Value);
            }

            if (state.HasValue)
            {
                query = query.Where(task => task.State == state);
            }

            query = query.OrderByDescending(task => task.CreationTime);

            query = query.Fetch(task => task.AssignedPerson);
            
            return query.ToList();
        }
    }
}
