using System;
using System.Linq;
using System.Threading;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Abp.Events.Bus.Datas.Entities;
using Abp.Exceptions;
using Abp.Modules.Core.Application.Services.Impl;
using Abp.Users;
using Abp.Utils.Extensions;
using Abp.Utils.Extensions.Collections;
using Taskever.Activities;
using Taskever.Application.Services.Dto;
using Taskever.Application.Services.Dto.Tasks;
using Taskever.Notifications;
using Taskever.Tasks;
using Taskever.Tasks.Events;

namespace Taskever.Application.Services.Impl
{
    public class TaskService : ITaskService
    {
        private readonly IActivityService _activityService;
        private readonly ITaskPolicy _taskPolicy;
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;
        private readonly IEventBus _eventBus;

        public TaskService(
            IActivityService activityService,
            ITaskPolicy taskPolicy,
            ITaskRepository taskRepository,
            IUserRepository userRepository,
            INotificationService notificationService,
            IEventBus eventBus)
        {
            _activityService = activityService;
            _taskRepository = taskRepository;
            _userRepository = userRepository;
            _notificationService = notificationService;
            _eventBus = eventBus;
            _taskPolicy = taskPolicy;
        }

        [UnitOfWork]
        public GetTaskOutput GetTask(GetTaskInput input)
        {
            var currentUser = _userRepository.Load(User.CurrentUserId);
            var task = _taskRepository.FirstOrDefault(input.Id);

            if (task == null)
            {
                throw new AbpUserFriendlyException("Can not found the task!");
            }

            if (!_taskPolicy.CanSeeTasksOfUser(currentUser, task.AssignedUser))
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
                           IsEditableByCurrentUser = _taskPolicy.CanUpdateTask(currentUser, task)
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

            if (!_taskPolicy.CanAssignTask(creatorUser, assignedUser))
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

            _eventBus.Trigger(this, new EntityCreatedEventData<Task>(taskEntity));

            return new CreateTaskOutput
                       {
                           Task = taskEntity.MapTo<TaskDto>()
                       };
        }

        [UnitOfWork]
        public void UpdateTask(UpdateTaskInput input)
        {
            var task = _taskRepository.FirstOrDefault(input.Id);
            if (task == null)
            {
                throw new Exception("Can not found the task!");
            }

            var currentUser = _userRepository.Load(User.CurrentUserId); //TODO: Add method LoadCurrentUser and GetCurrentUser
            if (!_taskPolicy.CanUpdateTask(currentUser, task))
            {
                throw new AbpUserFriendlyException("You can not update this task!");
            }

            if (task.AssignedUser.Id != input.AssignedUserId)
            {
                var userToAssign = _userRepository.Load(input.AssignedUserId);

                if (!_taskPolicy.CanAssignTask(currentUser, userToAssign))
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
                _eventBus.Trigger(this, new TaskCompletedEventData(task));
                //_notificationService.Notify(new CompletedTaskNotification(task));
            }
        }

        public DeleteTaskOutput DeleteTask(DeleteTaskInput input)
        {
            var task = _taskRepository.FirstOrDefault(input.Id);
            if (task == null)
            {
                throw new Exception("Can not found the task!");
            }

            var currentUser = _userRepository.Load(User.CurrentUserId);
            if (!_taskPolicy.CanDeleteTask(currentUser, task))
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

            if (!_taskPolicy.CanSeeTasksOfUser(currentUser, userOfTasks))
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