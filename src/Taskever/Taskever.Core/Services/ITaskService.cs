using System.Collections.Generic;
using Abp.Application.Services;
using Taskever.Services.Dto;

namespace Taskever.Services
{
    public interface ITaskService :IApplicationService
    {
        IList<TaskDto> GetMyTasks();

        GetTasksOfUserOutput GetTasksOfUser(GetTasksOfUserInput args);

        TaskDto CreateTask(TaskDto task);

        TaskDto UpdateTask(TaskDto task);
         
        void DeleteTask(int taskId);
    }
}
