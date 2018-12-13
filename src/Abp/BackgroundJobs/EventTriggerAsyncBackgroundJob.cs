using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Events.Bus;

namespace Abp.BackgroundJobs
{
    public class EventTriggerAsyncBackgroundJob<TEvent> : AsyncBackgroundJob<TEvent>, ITransientDependency
        where TEvent : EventData
    {
        public EventTriggerAsyncBackgroundJob(IEventBus eventBus)
        {
            EventBus = NullEventBus.Instance;
        }

        public IEventBus EventBus { get; set; }

        protected override async Task ExecuteAsync(TEvent e)
        {
            await EventBus.TriggerAsync(e);
        }
    }
}