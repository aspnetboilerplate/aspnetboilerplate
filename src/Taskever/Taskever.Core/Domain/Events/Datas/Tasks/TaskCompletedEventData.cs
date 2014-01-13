using Abp.Events.Bus.Datas.Entities;
using Taskever.Domain.Entities;

namespace Taskever.Domain.Events.Datas.Tasks
{
    public class TaskCompletedEventData : EntityEventData<Task>
    {
        public TaskCompletedEventData(Task entity)
            : base(entity)
        {
        }
    }
}
