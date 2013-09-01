using System.Collections.Generic;
using System.Linq;
using Abp.Services.Core.Impl;
using Taskever.Data.Repositories;
using Taskever.Entities;
using Taskever.Services.Dto;

namespace Taskever.Services.Impl
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _questionRepository;

        public TaskService(ITaskRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }

        //[UnitOfWork]
        public IList<TaskDto> GetAllTasks()
        {
            return _questionRepository.Query(q => q.ToList()).MapIList<Task, TaskDto>();
        }

        public void Insert(TaskDto task)
        {
            var taskEntity = task.MapTo<Task>();
            _questionRepository.Insert(taskEntity);
            task.Id = taskEntity.Id;
        }
    }
}