using System.Collections.Generic;
using Abp.Services;
using Abp.Services.Dto;
using Taskever.Services.Dto;

namespace Taskever.Services
{
    public interface ITaskService :IAppService
    {
        IList<TaskDto> GetMyTasks();

        GetTasksOfUserOutput GetTasksOfUser(GetTasksOfUserInput args);

        TaskDto CreateTask(TaskDto task);

        TaskDto UpdateTask(TaskDto task);
         
        void DeleteTask(int taskId);
    }
}
