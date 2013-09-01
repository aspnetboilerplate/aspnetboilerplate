using System.Collections.Generic;
using Abp.Services;
using Taskever.Services.Dto;

namespace Taskever.Services
{
    public interface ITaskService :IService
    {
        IList<TaskDto> GetAllTasks();

        void Create(TaskDto task);
    }
}
