using System.Collections.Generic;
using Abp.Services;
using Abp.Services.Dto;
using Taskever.Services.Dto;

namespace Taskever.Services
{
    public interface ITaskService :IService
    {
        IList<TaskDto> GetMyTasks();

        TaskDto CreateTask(TaskDto task);
         
        void DeleteTask(int taskId);
    }
}
