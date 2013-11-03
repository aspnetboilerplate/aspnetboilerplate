using System.Collections.Generic;
using Abp.Application.Services;
using Taskever.Application.Services.Dto;
using Taskever.Application.Services.Dto.Tasks;

namespace Taskever.Application.Services
{
    public interface ITaskService :IApplicationService
    {
        GetTaskOutput GetTask(GetTaskInput input);

        GetTasksOutput GetTasks(GetTasksInput args);

        CreateTaskOutput CreateTask(CreateTaskInput input);

        TaskDto UpdateTask(TaskDto task);
         
        void DeleteTask(int taskId);
    }
}
