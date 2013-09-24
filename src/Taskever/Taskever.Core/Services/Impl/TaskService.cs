using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Abp.Data.Repositories;
using Abp.Modules.Core.Data.Repositories;
using Abp.Modules.Core.Entities;
using Abp.Modules.Core.Services;
using Abp.Modules.Core.Services.Impl;
using Taskever.Entities;
using Taskever.Services.Dto;

namespace Taskever.Services.Impl
{
    public class TaskService : ITaskService
    {
        private readonly IRepository<Task> _taskRepository;
        private readonly IRepository<User> _userRepository;
        private readonly ITaskPrivilegeService _taskPrivilegeService;

        public TaskService(IRepository<Task> taskRepository, IRepository<User> userRepository, ITaskPrivilegeService taskPrivilegeService)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
            _taskPrivilegeService = taskPrivilegeService;
        }

        public virtual IList<TaskDto> GetMyTasks()
        {
            //var currentUserId = Thread.CurrentPrincipal.Identity.Name
            return _taskRepository.Query(q => q.Where(task => task.AssignedUser.Id == 1).ToList()).MapIList<Task, TaskDto>();
        }

        public virtual GetTasksOfUserOutput GetTasksOfUser(GetTasksOfUserInput args) //TODO: did not worked with GET, why?
        {
            if (!_taskPrivilegeService.CanSeeTasksOfUser(User.CurrentUserId, args.UserId))
            {
                throw new ApplicationException("Can not see tasks of user");
            }

            return new GetTasksOfUserOutput
                       {
                           Tasks = _taskRepository.Query(q => q.Where(task => task.AssignedUser.Id == args.UserId).ToList()).MapIList<Task, TaskDto>()
                       };
        }

        public virtual TaskDto CreateTask(TaskDto task)
        {
            var taskEntity = task.MapTo<Task>();

            //TODO: Automatically set Tenant and Creator User informations!?
            taskEntity.Tenant = Tenant.Current;
            taskEntity.CreatorUser = new User { Id = User.CurrentUserId };

            _taskRepository.Insert(taskEntity);

            return taskEntity.MapTo<TaskDto>();
        }

        public TaskDto UpdateTask(TaskDto task)
        {
            var taskEntity = task.MapTo<Task>();

            //TODO: Automatically set Tenant and Creator User informations!?
            taskEntity.Tenant = Tenant.Current;
            taskEntity.CreatorUser = new User { Id = User.CurrentUserId };

            _taskRepository.Update(taskEntity);
            return taskEntity.MapTo<TaskDto>();
        }

        public virtual void DeleteTask(int taskId)
        {
            _taskRepository.Delete(taskId);
        }
    }
}