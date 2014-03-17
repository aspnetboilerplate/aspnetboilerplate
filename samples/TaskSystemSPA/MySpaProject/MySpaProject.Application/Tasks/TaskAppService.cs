using System.Collections.Generic;
using System.Threading;
using Abp.Domain.Uow;
using AutoMapper;
using MySpaProject.People;
using MySpaProject.Tasks.Dtos;

namespace MySpaProject.Tasks
{
    public class TaskAppService : ITaskAppService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IPersonRepository _personRepository;

        public TaskAppService(ITaskRepository taskRepository, IPersonRepository personRepository)
        {
            _taskRepository = taskRepository;
            _personRepository = personRepository;
        }

        public GetTasksOutput GetTasks(GetTasksInput input)
        {
            var tasks = _taskRepository.GetAllWithPeople(input.AssignedPersonId, input.State);
            return new GetTasksOutput
                   {
                       Tasks = Mapper.Map<List<TaskDto>>(tasks)
                   };
        }

        [UnitOfWork]
        public void UpdateTask(UpdateTaskInput input)
        {
            var task = _taskRepository.Get(input.TaskId);

            if (input.State.HasValue)
            {
                task.State = input.State.Value;
            }

            if (input.AssignedPersonId.HasValue)
            {
                task.AssignedPerson = _personRepository.Load(input.AssignedPersonId.Value);
            }
        }

        public void CreateTask(CreateTaskInput input)
        {
            Thread.Sleep(1000); //TODO: For busy animation!

            var task = new Task { Description = input.Description };

            if (input.AssignedPersonId.HasValue)
            {
                task.AssignedPerson = _personRepository.Load(input.AssignedPersonId.Value);
            }

            _taskRepository.Insert(task);
        }
    }
}