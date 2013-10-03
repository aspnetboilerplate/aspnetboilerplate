using System.Collections.Generic;
using Abp.Application.Services;
using Taskever.Application.Services.Dto;
using Taskever.Application.Services.Dto.TaskService;

namespace Taskever.Application.Services
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
