using Abp.Dependency;
using Abp.Events.Bus.Datas.Entities;
using Abp.Events.Bus.Handlers;
using Taskever.Security.Users;
using Taskever.Tasks;
using Taskever.Tasks.Events;

namespace Taskever.Activities.EventHandlers
{
    public class TaskActivityEventHandler : 
        IEventHandler<EntityCreatedEventData<Task>>, 
        IEventHandler<TaskCompletedEventData>,
        ITransientDependency
    {
        private readonly IActivityService _activityService;
        private readonly ITaskeverUserRepository _userRepository;

        public TaskActivityEventHandler(IActivityService activityService, ITaskeverUserRepository userRepository)
        {
            _activityService = activityService;
            _userRepository = userRepository;
        }

        public void HandleEvent(EntityCreatedEventData<Task> eventData)
        {
            var activity = new CreateTaskActivity
                           {
                               CreatorUser =
                                   eventData.Entity.CreatorUserId.HasValue
                                       ? _userRepository.Load(eventData.Entity.CreatorUserId.Value)
                                       : null,
                               AssignedUser = eventData.Entity.AssignedUser,
                               Task = eventData.Entity
                           };


            //activity.AssignedUserId = activity.AssignedUser.Id;
            //activity.CreatorUserId = activity.CreatorUser.Id;
            //activity.TaskId = activity.Task.Id;

            _activityService.AddActivity(activity);
        }
        public void HandleEvent(TaskCompletedEventData eventData)
        {
            _activityService.AddActivity(
                    new CompleteTaskActivity
                    {
                        AssignedUser = eventData.Entity.AssignedUser,
                        Task = eventData.Entity
                    });
        }
    }
}
