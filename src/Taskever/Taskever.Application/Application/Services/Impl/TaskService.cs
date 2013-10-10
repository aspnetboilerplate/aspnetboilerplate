using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Modules.Core.Application.Services.Impl;
using Abp.Modules.Core.Data.Repositories;
using Abp.Modules.Core.Domain.Entities;
using Taskever.Application.Services.Dto;
using Taskever.Application.Services.Dto.Tasks;
using Taskever.Data.Repositories;
using Taskever.Domain.Business.Acitivities;
using Taskever.Domain.Entities;
using Taskever.Domain.Services;

namespace Taskever.Application.Services.Impl
{
    public class TaskService : ITaskService
    {
        private readonly IActivityService _activityService;
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;

        private readonly ITaskPrivilegeService _taskPrivilegeService;

        public TaskService(IActivityService activityService, ITaskRepository taskRepository, IUserRepository userRepository, ITaskPrivilegeService taskPrivilegeService)
        {
            _activityService = activityService;
            _taskRepository = taskRepository;
            _userRepository = userRepository;
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
            //Get entities from database
            var creatorUser = _userRepository.Get(User.CurrentUserId);
            var assignedUser = _userRepository.Get(task.AssignedUserId);

            //Create the task
            var taskEntity = task.MapTo<Task>();
            taskEntity.AssignedUser = _userRepository.Load(task.AssignedUserId);
            _taskRepository.Insert(taskEntity);

            //Add to activities (TODO: This must be done by events, not directly by Task service?)
            _activityService.AddActivity(
                new CreateTaskActivityInfo(
                    creatorUser.Id,
                    creatorUser.NameAndSurname,
                    taskEntity.Id,
                    taskEntity.Title,
                    assignedUser.Id,
                    assignedUser.NameAndSurname
                    )
                );

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