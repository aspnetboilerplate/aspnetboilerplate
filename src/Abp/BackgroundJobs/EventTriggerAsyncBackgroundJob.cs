using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Events.Bus;

namespace Abp.BackgroundJobs
{
    public class EventTriggerAsyncBackgroundJob<TEvent> : AsyncBackgroundJob<TEvent>, ITransientDependency
        where TEvent : EventData
    {
        public IEventBus EventBus { get; set; }

        public EventTriggerAsyncBackgroundJob()
        {
            EventBus = NullEventBus.Instance;
        }

        protected override async Task ExecuteAsync(TEvent e)
        {
            await EventBus.TriggerAsync(e);
        }
    }
}