using Abp.Events.Bus.Datas.Entities;
using Abp.Events.Bus.Handlers;
using Taskever.Application.Services;
using Taskever.Domain.Events.Datas.Tasks;
using Taskever.Domain.Services;
using Task = Taskever.Domain.Entities.Task;

namespace Taskever.Domain.Events.Handlers.Notifications
{
    public class TaskNotificationEventHandler : IEventHandler<EntityCreatedEventData<Task>>, IEventHandler<TaskCompletedEventData>
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
