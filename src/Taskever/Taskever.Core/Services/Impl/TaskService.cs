using System.Collections.Generic;
using System.Linq;
using Abp.Data.Repositories;
using Abp.Modules.Core.Entities;
using Abp.Modules.Core.Services.Impl;
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

        public virtual IList<TaskDto> GetMyTasks()
        {
            return _taskRepository.Query(q => q.ToList()).MapIList<Task, TaskDto>();
        }

        public virtual TaskDto Create(TaskDto task)
        {
            //TODO: Automatically set Tenant and Creator User informations, bu where?
            var taskEntity = task.MapTo<Task>();
            taskEntity.Tenant = Tenant.Current;
            _taskRepository.Insert(taskEntity);
            return taskEntity.MapTo<TaskDto>();
        }

        public void Delete(int taskId)
        {
            _taskRepository.Delete(taskId);
        }
    }
}