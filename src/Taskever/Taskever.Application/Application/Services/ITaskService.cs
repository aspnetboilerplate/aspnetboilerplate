using System.Collections.Generic;
using Abp.Application.Services;
using Taskever.Application.Services.Dto;

namespace Taskever.Application.Services
{
    public interface ITaskService :IApplicationService
    {
        IList<TaskDto> GetMyTasks();

        GetTasksOfUserOutputDto GetTasksOfUser(GetTasksOfUserInputDto args);

        TaskDto CreateTask(TaskDto task);

        TaskDto UpdateTask(TaskDto task);
         
        void DeleteTask(int taskId);
    }
}
