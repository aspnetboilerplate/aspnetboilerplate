using Abp.Domain.Repositories;
using System.Collections.Generic;

namespace MyProject.Tasks
{
    /// <summary>
    /// Defines a repository to perform database operations for <see cref="Task"/> Entities.
    /// 
    /// Extends <see cref="IRepository{TEntity, TPrimaryKey}"/> to inherit base repository functionality. 
    /// </summary>
    public interface ITaskRepository : IRepository<Task, long>
    {
        /// <summary>
        /// Gets all tasks with <see cref="Task.AssignedPerson"/> is retrived (Include for EntityFramework, Fetch for NHibernate)
        /// filtered by given conditions.
        /// </summary>
        /// <param name="assignedPersonId">Optional assigned person filter. If it's null, not filtered.</param>
        /// <param name="state">Optional state filter. If it's null, not filtered.</param>
        /// <returns>List of found tasks</returns>
        List<Task> GetAllWithPeople(int? assignedPersonId, TaskState? state);
    }
}
