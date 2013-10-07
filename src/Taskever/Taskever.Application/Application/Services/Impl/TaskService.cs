using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Modules.Core.Application.Services.Impl;
using Abp.Modules.Core.Data.Repositories;
using Abp.Modules.Core.Domain.Entities;
using Taskever.Application.Services.Dto;
using Taskever.Application.Services.Dto.TaskService;
using Taskever.Data.Repositories;
using Taskever.Domain.Entities;
using Taskever.Domain.Entities.EventHistories;
using Taskever.Domain.Services;

namespace Taskever.Application.Services.Impl
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEventHistoryRepository _eventHistoryRepository;

        private readonly ITaskPrivilegeService _taskPrivilegeService;

        public TaskService(ITaskRepository taskRepository, IUserRepository userRepository, IEventHistoryRepository eventHistoryRepository, ITaskPrivilegeService taskPrivilegeService)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
            _eventHistoryRepository = eventHistoryRepository;
            _taskPrivilegeService = taskPrivilegeService;
        }

        public virtual GetTasksOfUserOutput GetTasksOfUser(GetTasksOfUserInput input)
        {
            if (!_taskPrivilegeService.CanSeeTasksOfUser(User.CurrentUserId, input.UserId))
            {
                throw new ApplicationException("Can not see tasks of user");
            }

            var tasks = _taskRepository.Query(query => query.Where(task => task.AssignedUser.Id == input.UserId).ToList());
            return new GetTasksOfUserOutput
                       {
                           Tasks = tasks.MapIList<Task, TaskDto>()
                       };
        }

        [UnitOfWork]
        public virtual TaskDto CreateTask(TaskDto task)
        {
            var taskEntity = task.MapTo<Task>();
            taskEntity.AssignedUser = _userRepository.Load(task.AssignedUserId);
            _taskRepository.Insert(taskEntity);

            var history = new CreateAndAssignEventHistoryFormatter(
                User.CurrentUserId,
                taskEntity.Id,
                task.AssignedUserId
                ).CreateEventHistory();

            _eventHistoryRepository.Insert(history);

            return taskEntity.MapTo<TaskDto>();
        }

        public TaskDto UpdateTask(TaskDto task)
        {
            var taskEntity = task.MapTo<Task>();
            _taskRepository.Update(taskEntity);
            return taskEntity.MapTo<TaskDto>();
        }

        public virtual void DeleteTask(int taskId)
        {
            _taskRepository.Delete(taskId);
        }
    }
}