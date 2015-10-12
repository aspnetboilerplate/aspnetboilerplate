using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Repositories;
using Abp.NHibernate;
using Abp.NHibernate.Repositories;
using NHibernate.Linq;
using MyProject.Tasks;

namespace MyProject.NHibernate.Repositories
{
    /// <summary>
    /// Implements <see cref="ITaskRepository"/> for NHibernate ORM framework.
    /// 
    /// Inherits from <see cref="NhRepositoryBase{TEntity,TPrimaryKey}"/>.
    /// NhRepositoryBase implements <see cref="IRepository{TEntity,TPrimaryKey}"/> automatically.
    /// So, we just implement additional methods of <see cref="ITaskRepository"/>.
    /// </summary>
    public class TaskRepository : NhRepositoryBase<Task, long>, ITaskRepository
    {
        public TaskRepository(ISessionProvider sessionProvider)
            : base(sessionProvider)
        {
        }

        public List<Task> GetAllWithPeople(int? assignedPersonId, TaskState? state)
        {
            
            //In repository methods, we do not deal with create/dispose DB connections (Session) and transactions. ABP handles it.

            var query = GetAll(); //GetAll() returns IQueryable<T>, so we can query over it.
            //var query = Session.Query<Task>(); //Alternatively, we can directly use NHibernate's Session

            //Add some Where conditions...

            if (assignedPersonId.HasValue)
            {
                query = query.Where(task => task.AssignedPerson.Id == assignedPersonId.Value);
            }

            if (state.HasValue)
            {
                query = query.Where(task => task.State == state);
            }

            return query
                .OrderByDescending(task => task.CreationTime)
                .Fetch(task => task.AssignedPerson) //Fetch assigned person in a single query
                .ToList();
        }
    }
}
