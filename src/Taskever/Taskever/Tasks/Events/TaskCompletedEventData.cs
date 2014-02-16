using Abp.Events.Bus.Datas.Entities;

namespace Taskever.Tasks.Events
{
    public class TaskCompletedEventData : EntityEventData<Task>
    {
        public TaskCompletedEventData(Task entity)
            : base(entity)
        {
        }
    }
}
