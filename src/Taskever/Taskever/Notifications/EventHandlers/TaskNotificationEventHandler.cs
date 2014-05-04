using Abp.Dependency;
using Abp.Events.Bus.Datas.Entities;
using Abp.Events.Bus.Handlers;
using Taskever.Notifications.Tasks;
using Taskever.Security.Users;
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

        private readonly ITaskeverUserRepository _userRepository;
        
        public TaskNotificationEventHandler(INotificationService notificationService, ITaskeverUserRepository userRepository)
        {
            _notificationService = notificationService;
            _userRepository = userRepository;
        }

        public void HandleEvent(EntityCreatedEventData<Task> eventData)
        {
            if (eventData.Entity.AssignedUser.Id != eventData.Entity.CreatorUserId)
            {
                _notificationService.Notify(new AssignedToTaskNotification(eventData.Entity));
            }
        }

        public void HandleEvent(TaskCompletedEventData eventData)
        {
            if (eventData.Entity.CreatorUserId.HasValue)
            {
                _notificationService.Notify(new CompletedTaskNotification(eventData.Entity, _userRepository.Get(eventData.Entity.CreatorUserId.Value)));
            }
        }
    }
}
