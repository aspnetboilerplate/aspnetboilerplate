using System.Collections.Generic;
using Abp.Domain.Repositories;

namespace MySpaProject.Tasks
{
    public interface ITaskRepository : IRepository<Task, long>
    {
        List<Task> GetAllWithPeople(int? assignedPersonId, TaskState? state);
    }
}
