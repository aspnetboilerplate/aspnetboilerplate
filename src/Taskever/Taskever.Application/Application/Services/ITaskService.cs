using Abp.Application.Services;
using Taskever.Application.Services.Dto.Tasks;

namespace Taskever.Application.Services
{
    public interface ITaskService :IApplicationService
    {
        GetTaskOutput GetTask(GetTaskInput input);

        GetTasksOutput GetTasks(GetTasksInput args);

        CreateTaskOutput CreateTask(CreateTaskInput input);

        UpdateTaskOutput UpdateTask(UpdateTaskInput input);
         
        DeleteTaskOutput DeleteTask(DeleteTaskInput input);
    }
}
