using Abp.Events.Bus.Datas.Entities;
using Abp.Events.Bus.Handlers;
using Taskever.Domain.Entities;
using Taskever.Domain.Entities.Activities;
using Taskever.Domain.Events.Datas.Tasks;
using Taskever.Domain.Services;

namespace Taskever.Domain.Events.Handlers.Activities
{
    public class TaskActivityEventHandler : IEventHandler<EntityCreatedEventData<Task>>, IEventHandler<TaskCompletedEventData>
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
