using System.Collections.Generic;
using Abp.Services;
using Taskever.Services.Dto;

namespace Taskever.Services
{
    public interface ITaskService :IService
    {
        IList<TaskDto> GetMyTasks();

        TaskDto Create(TaskDto task);

        void Delete(int taskId);
    }
}
