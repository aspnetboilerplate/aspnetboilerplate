using System;
using System.Linq;
using Abp.Domain.Uow;
using Abp.Exceptions;
using Abp.Modules.Core.Application.Services.Impl;
using Abp.Modules.Core.Data.Repositories;
using Abp.Modules.Core.Domain.Entities;
using Abp.Utils.Extensions;
using Taskever.Application.Services.Dto;
using Taskever.Application.Services.Dto.Tasks;
using Taskever.Data.Repositories;
using Taskever.Domain.Entities;
using Taskever.Domain.Entities.Activities;
using Taskever.Domain.Enums;
using Taskever.Domain.Services;

namespace Taskever.Application.Services.Impl
{
    public class TaskService : ITaskService
    {
        private readonly IActivityService _activityService;
        private readonly ITaskPrivilegeService _taskPrivilegeService;
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;

        public TaskService(
            IActivityService activityService,
            ITaskPrivilegeService taskPrivilegeService,
            ITaskRepository taskRepository,
            IUserRepository userRepository)
        {
            _activityService = activityService;
            _taskRepository = taskRepository;
            _userRepository = userRepository;
            _taskPrivilegeService = taskPrivilegeService;
        }

        [UnitOfWork]
        public GetTaskOutput GetTask(GetTaskInput input)
        {
            var currentUser = _userRepository.Load(User.CurrentUserId);
            var task = _taskRepository.GetOrNull(input.Id);

            if (task == null)
            {
                throw new Exception("Can not found the task: " + input.Id);
            }

            if (!_taskPrivilegeService.CanSeeTasksOfUser(currentUser, task.AssignedUser))
            {
                throw new ApplicationException("Can not see tasks of user");
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
            taskEntity.AssignedUser = _userRepository.Load(input.Task.AssignedUserId);
            taskEntity.State = TaskState.New;
            _taskRepository.Insert(taskEntity);

            _activityService.AddActivity(
                new CreateTaskActivity
                    {
                        CreatorUser = creatorUser,
                        AssignedUser = assignedUser,
                        Task = taskEntity
                    });

            return new CreateTaskOutput
                       {
                           Task = taskEntity.MapTo<TaskDto>()
                       };
        }

        [UnitOfWork]
        public UpdateTaskOutput UpdateTask(UpdateTaskInput input)
        {
            var task = _taskRepository.GetOrNull(input.Id);
            if (task == null)
            {
                throw new Exception("Can not found the task!");
            }

            var currentUser = _userRepository.Load(User.CurrentUserId); //TODO: Add method LoadCurrentUser and GetCurrentUser
            if (_taskPrivilegeService.CanUpdateTask(currentUser, task))
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
            }

            return new UpdateTaskOutput();
        }

        public DeleteTaskOutput DeleteTask(DeleteTaskInput input)
        {
            var task = _taskRepository.GetOrNull(input.Id);
            if (task == null)
            {
                throw new Exception("Can not found the task!");
            }

            //TODO: Check if this user can delete to this task!

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