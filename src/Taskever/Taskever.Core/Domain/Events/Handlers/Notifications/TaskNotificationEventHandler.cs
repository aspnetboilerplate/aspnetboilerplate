using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Events.Bus.Datas.Entities;
using Abp.Events.Bus.Handlers;
using Taskever.Domain.Events.Datas.Tasks;
using Task = Taskever.Domain.Entities.Task;

namespace Taskever.Domain.Events.Handlers.Notifications
{
    public class TaskNotificationEventHandler : IEventHandler<EntityCreatedEventData<Task>>, IEventHandler<TaskCompletedEventData>
    {
        public void HandleEvent(EntityCreatedEventData<Task> eventData)
        {
            
        }

        public void HandleEvent(TaskCompletedEventData eventData)
        {
            
        }
    }
}
