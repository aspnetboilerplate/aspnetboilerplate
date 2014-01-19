using Abp.Application.Services;
using Taskever.Tasks.Dto;

namespace Taskever.Tasks
{
    public interface ITaskAppService : IApplicationService
    {
        GetTaskOutput GetTask(GetTaskInput input);

        GetTasksOutput GetTasks(GetTasksInput args);

        GetTasksByImportanceOutput GetTasksByImportance(GetTasksByImportanceInput input);

        CreateTaskOutput CreateTask(CreateTaskInput input);

        void UpdateTask(UpdateTaskInput input);

        DeleteTaskOutput DeleteTask(DeleteTaskInput input);
    }
}
