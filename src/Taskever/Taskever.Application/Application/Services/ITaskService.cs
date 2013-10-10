using System.Collections.Generic;
using Abp.Application.Services;
using Taskever.Application.Services.Dto;
using Taskever.Application.Services.Dto.Tasks;

namespace Taskever.Application.Services
{
    public interface ITaskService :IApplicationService
    {
        GetTasksOfUserOutput GetTasksOfUser(GetTasksOfUserInput args);

        TaskDto CreateTask(TaskDto task);

        TaskDto UpdateTask(TaskDto task);
         
        void DeleteTask(int taskId);
    }
}
