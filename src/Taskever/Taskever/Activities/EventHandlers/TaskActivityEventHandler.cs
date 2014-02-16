using Abp.Dependency;
using Abp.Events.Bus.Datas.Entities;
using Abp.Events.Bus.Handlers;
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

        public TaskActivityEventHandler(IActivityService activityService)
        {
            _activityService = activityService;
        }

        public void HandleEvent(EntityCreatedEventData<Task> eventData)
        {
            _activityService.AddActivity(
                new CreateTaskActivity
                {
                    CreatorUser = eventData.Entity.CreatorUser,
                    AssignedUser = eventData.Entity.AssignedUser,
                    Task = eventData.Entity
                });
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
