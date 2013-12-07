using Abp.Application.Services;
using Taskever.Application.Services.Dto.Tasks;

namespace Taskever.Application.Services
{
    public interface ITaskService : IApplicationService
    {
        GetTaskOutput GetTask(GetTaskInput input);

        GetTasksOutput GetTasks(GetTasksInput args);

        GetTasksByImportanceOutput GetTasksByImportance(GetTasksByImportanceInput input);

        CreateTaskOutput CreateTask(CreateTaskInput input);

        void UpdateTask(UpdateTaskInput input);

        DeleteTaskOutput DeleteTask(DeleteTaskInput input);
    }
}
