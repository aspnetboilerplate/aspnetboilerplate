using System.Collections.Generic;
using AutoMapper;
using MySpaProject.Tasks.Dtos;

namespace MySpaProject.Tasks
{
    public class TaskAppService : ITaskAppService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskAppService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public GetAllTasksOutput GetAllTasks()
        {
            var tasks = _taskRepository.GetAllWithPeople();
            return new GetAllTasksOutput
                   {
                       Tasks = Mapper.Map<List<TaskDto>>(tasks)
                   };
        }
    }
}