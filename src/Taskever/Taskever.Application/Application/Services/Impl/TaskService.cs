using System;
using System.Linq;
using System.Threading;
using Abp.Domain.Uow;
using Abp.Exceptions;
using Abp.Modules.Core.Application.Services.Impl;
using Abp.Modules.Core.Domain.Entities;
using Abp.Modules.Core.Domain.Repositories;
using Abp.Utils.Extensions;
using Abp.Utils.Extensions.Collections;
using Taskever.Application.Services.Dto;
using Taskever.Application.Services.Dto.Tasks;
using Taskever.Domain.Entities;
using Taskever.Domain.Entities.Activities;
using Taskever.Domain.Enums;
using Taskever.Domain.Repositories;
using Taskever.Domain.Services;

namespace Taskever.Application.Services.Impl
{
    public class TaskService : ITaskService
    {
        private readonly IActivityService _activityService;
        private readonly ITaskPrivilegeService _taskPrivilegeService;
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;

        public TaskService(
            IActivityService activityService,
            ITaskPrivilegeService taskPrivilegeService,
            ITaskRepository taskRepository,
            IUserRepository userRepository, INotificationService notificationService)
        {
            _activityService = activityService;
            _taskRepository = taskRepository;
            _userRepository = userRepository;
            _notificationService = notificationService;
            _taskPrivilegeService = taskPrivilegeService;
        }

        [UnitOfWork]
        public GetTaskOutput GetTask(GetTaskInput input)
        {
            var currentUser = _userRepository.Load(User.CurrentUserId);
            var task = _taskRepository.GetOrNull(input.Id);

            if (task == null)
            {
                throw new AbpUserFriendlyException("Can not found the task!");
            }

            if (!_taskPrivilegeService.CanSeeTasksOfUser(currentUser, task.AssignedUser))
            {
                throw new AbpUserFriendlyException("Can not see tasks of " + task.AssignedUser.Name);
            }

            if (task.AssignedUser.Id != currentUser.Id && task.Privacy == TaskPrivacy.Private)
            {
                throw new AbpUserFriendlyException("Can not see this task since it's private!");
            }

            return new GetTaskOutput
                       {
                           Task = task.MapTo<TaskWithAssignedUserDto>(),
                           IsEditableByCurrentUser = _taskPrivilegeService.CanUpdateTask(currentUser, task)
                       };
        }

        [UnitOfWork]
        public virtual GetTasksOutput GetTasks(GetTasksInput input)
        {
            var query = CreateQueryForAssignedTasksOfUser(input.AssignedUserId);
            if (!input.TaskStates.IsNullOrEmpty())
            {
                query = query.Where(task => input.TaskStates.Contains(task.State));
            }

            query = query
                .OrderByDescending(task => task.Priority)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount);

            return new GetTasksOutput
                       {
                           Tasks = query.ToList().MapIList<Task, TaskDto>()
                       };
        }

        [UnitOfWork]
        public GetTasksByImportanceOutput GetTasksByImportance(GetTasksByImportanceInput input)
        {
            var query = CreateQueryForAssignedTasksOfUser(input.AssignedUserId);
            query = query
                .Where(task => task.State != TaskState.Completed)
                .OrderByDescending(task => task.Priority)
                .ThenByDescending(task => task.State)
                .ThenByDescending(task => task.CreationTime)
                .Take(input.MaxResultCount);

            return new GetTasksByImportanceOutput
            {
                Tasks = query.ToList().MapIList<Task, TaskDto>()
            };
        }

        [UnitOfWork]
        public virtual CreateTaskOutput CreateTask(CreateTaskInput input)
        {
            //Get entities from database
            var creatorUser = _userRepository.Get(User.CurrentUserId);
            var assignedUser = _userRepository.Get(input.Task.AssignedUserId);

            if (!_taskPrivilegeService.CanAssignTask(creatorUser, assignedUser))
            {
                throw new AbpUserFriendlyException("You can not assign task to this user!");
            }

            //Create the task
            var taskEntity = input.Task.MapTo<Task>();

            taskEntity.CreatorUser = creatorUser;
            taskEntity.AssignedUser = _userRepository.Load(input.Task.AssignedUserId);
            taskEntity.State = TaskState.New;

            if (taskEntity.AssignedUser.Id != creatorUser.Id && taskEntity.Privacy == TaskPrivacy.Private)
            {
                throw new ApplicationException("A user can not assign a private task to another user!");
            }

            _taskRepository.Insert(taskEntity);

            _activityService.AddActivity(
                new CreateTaskActivity
                    {
                        CreatorUser = creatorUser,
                        AssignedUser = assignedUser,
                        Task = taskEntity
                    });

            if (taskEntity.AssignedUser.Id != creatorUser.Id)
            {
                _notificationService.Notify(new AssignedToTaskNotification(taskEntity));
            }

            return new CreateTaskOutput
                       {
                           Task = taskEntity.MapTo<TaskDto>()
                       };
        }

        [UnitOfWork]
        public void UpdateTask(UpdateTaskInput input)
        {
            var task = _taskRepository.GetOrNull(input.Id);
            if (task == null)
            {
                throw new Exception("Can not found the task!");
            }

            var currentUser = _userRepository.Load(User.CurrentUserId); //TODO: Add method LoadCurrentUser and GetCurrentUser
            if (!_taskPrivilegeService.CanUpdateTask(currentUser, task))
            {
                throw new AbpUserFriendlyException("You can not update this task!");
            }

            if (task.AssignedUser.Id != input.AssignedUserId)
            {
                var userToAssign = _userRepository.Load(input.AssignedUserId);

                if (!_taskPrivilegeService.CanAssignTask(currentUser, userToAssign))
                {
                    throw new AbpUserFriendlyException("You can not assign task to this user!");
                }

                task.AssignedUser = userToAssign;
            }

            var oldTaskState = task.State;

            //TODO: Can we use Auto mapper?

            task.Description = input.Description;
            task.Priority = (TaskPriority)input.Priority;
            task.State = (TaskState)input.State;
            task.Privacy = (TaskPrivacy)input.Privacy;
            task.Title = input.Title;

            if (oldTaskState != TaskState.Completed && task.State == TaskState.Completed)
            {
                _activityService.AddActivity(
                    new CompleteTaskActivity
                        {
                            AssignedUser = task.AssignedUser,
                            Task = task
                        });
                _notificationService.Notify(new CompletedTaskNotification(task));
            }
        }

        public DeleteTaskOutput DeleteTask(DeleteTaskInput input)
        {
            var task = _taskRepository.GetOrNull(input.Id);
            if (task == null)
            {
                throw new Exception("Can not found the task!");
            }

            var currentUser = _userRepository.Load(User.CurrentUserId);
            if (!_taskPrivilegeService.CanDeleteTask(currentUser, task))
            {
                throw new AbpUserFriendlyException("You can not delete this task!");
            }

            _taskRepository.Delete(task);

            return new DeleteTaskOutput();
        }

        private IQueryable<Task> CreateQueryForAssignedTasksOfUser(int assignedUserId)
        {
            var currentUser = _userRepository.Load(User.CurrentUserId);
            var userOfTasks = _userRepository.Load(assignedUserId);

            if (!_taskPrivilegeService.CanSeeTasksOfUser(currentUser, userOfTasks))
            {
                throw new ApplicationException("Can not see tasks of user");
            }

            var query = _taskRepository
                .GetAll()
                .Where(task => task.AssignedUser.Id == assignedUserId);

            if (currentUser.Id != userOfTasks.Id)
            {
                query = query.Where(task => task.Privacy != TaskPrivacy.Private);
            }

            return query;
        }
    }
}