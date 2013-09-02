using System.Collections.Generic;
using System.Linq;
using Abp.Data.Repositories;
using Abp.Services.Core.Impl;
using Taskever.Entities;
using Taskever.Services.Dto;

namespace Taskever.Services.Impl
{
    public class TaskService : ITaskService
    {
        private readonly IRepository<Task> _taskRepository;

        public TaskService(IRepository<Task> taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public IList<TaskDto> GetMyTasks()
        {
            return _taskRepository.Query(q => q.ToList()).MapIList<Task, TaskDto>();
        }

        public void Create(TaskDto task)
        {
            var taskEntity = task.MapTo<Task>();
            _taskRepository.Insert(taskEntity);
            task.Id = taskEntity.Id;
        }
    }
}