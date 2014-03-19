using Abp.Application.Services;
using MySpaProject.Tasks.Dtos;

namespace MySpaProject.Tasks
{
    public interface ITaskAppService : IApplicationService
    {
        GetTasksOutput GetTasks(GetTasksInput input);

        void UpdateTask(UpdateTaskInput input);

        void CreateTask(CreateTaskInput input);
    }
}
