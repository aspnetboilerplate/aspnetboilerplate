using System;
using System.Linq;
using Abp.Domain.Uow;
using Abp.Modules.Core.Application.Services.Impl;
using Abp.Modules.Core.Data.Repositories;
using Abp.Modules.Core.Domain.Entities;
using Abp.Utils.Extensions;
using Taskever.Application.Services.Dto;
using Taskever.Application.Services.Dto.Tasks;
using Taskever.Data.Repositories;
using Taskever.Domain.Business.Acitivities;
using Taskever.Domain.Entities;
using Taskever.Domain.Enums;
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
                           Task = task.MapTo<TaskWithAssignedUserDto>()
                       };
        }

        [UnitOfWork]
        public virtual GetTasksOutput GetTasks(GetTasksInput input)
        {
            var currentUser = _userRepository.Load(User.CurrentUserId);
            var userOfTasks = _userRepository.Load(input.AssignedUserId);

            if (!_taskPrivilegeService.CanSeeTasksOfUser(currentUser, userOfTasks))
            {
                throw new ApplicationException("Can not see tasks of user");
            }

            var query = _taskRepository
                .GetAll()
                .Where(task => task.AssignedUser.Id == input.AssignedUserId);

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
        public virtual CreateTaskOutput CreateTask(CreateTaskInput input)
        {
            //Get entities from database
            var creatorUser = _userRepository.Get(User.CurrentUserId);
            var assignedUser = _userRepository.Get(input.Task.AssignedUserId);

            //TODO: Can assign the task to the user?

            //Create the task
            var taskEntity = input.Task.MapTo<Task>();
            taskEntity.AssignedUser = _userRepository.Load(input.Task.AssignedUserId);
            taskEntity.State = TaskState.New;
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

            //TODO: Make with auto mapper!
            //AutoMapper.Mapper.DynamicMap(input, task); //TODO: Change it to be static map for performance reasons? Also check performance!

            if (task.AssignedUser.Id != input.AssignedUserId)
            {
                //TODO: Can assign the task to the user?
                //TODO: Check if assigned user does exists
                task.AssignedUser = _userRepository.Load(input.AssignedUserId);
            }

            task.Description = input.Description;
            task.Priority = (TaskPriority)input.Priority;
            task.State = (TaskState)input.State;
            task.Title = input.Title;

            //TODO: Write a 'task complete' activity if needed
            if (task.State == TaskState.Completed)
            {
                _activityService.AddActivity(
                    new CompleteTaskActivityInfo(
                        task.Id,
                        task.Title,
                        task.AssignedUser.Id,
                        task.AssignedUser.NameAndSurname
                        )
                    );
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
    }
}