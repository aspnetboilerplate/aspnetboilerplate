using Abp.Application.Services;
using MyProject.Tasks.Dtos;
using System.Collections.Generic;

namespace MyProject.Tasks
{
    /// <summary>
    /// Defines an application service for <see cref="Task"/> operations.
    /// 
    /// It extends <see cref="IApplicationService"/>.
    /// Thus, ABP enables automatic dependency injection, validation and other benefits for it.
    /// 
    /// Application services works with DTOs (Data Transfer Objects).
    /// Service methods gets and returns DTOs.
    /// </summary>
    public interface ITaskAppService : IApplicationService
    {
        GetTasksOutput GetTasks(GetTasksInput input);

        void UpdateTask(UpdateTaskInput input);
        
        void CreateTask(CreateTaskInput input);

        IList<Task> GetAll();
    }
}
