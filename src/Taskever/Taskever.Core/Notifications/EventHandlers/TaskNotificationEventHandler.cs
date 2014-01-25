using Abp.Dependency;
using Abp.Events.Bus.Datas.Entities;
using Abp.Events.Bus.Handlers;
using Taskever.Notifications.Tasks;
using Taskever.Tasks;
using Taskever.Tasks.Events;

namespace Taskever.Notifications.EventHandlers
{
    public class TaskNotificationEventHandler : 
        IEventHandler<EntityCreatedEventData<Task>>, 
        IEventHandler<TaskCompletedEventData>,
        ITransientDependency
    {
        private readonly INotificationService _notificationService;
        
        public TaskNotificationEventHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public void HandleEvent(EntityCreatedEventData<Task> eventData)
        {
            if (eventData.Entity.AssignedUser.Id != eventData.Entity.CreatorUser.Id)
            {
                _notificationService.Notify(new AssignedToTaskNotification(eventData.Entity));
            }
        }

        public void HandleEvent(TaskCompletedEventData eventData)
        {
            _notificationService.Notify(new CompletedTaskNotification(eventData.Entity));
        }
    }
}
