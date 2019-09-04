using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Events.Bus;

namespace Abp.BackgroundJobs
{
    public class EventTriggerAsyncBackgroundJob<TEventData> : AsyncBackgroundJob<TEventData>, ITransientDependency
        where TEventData : EventData
    {
        public IEventBus EventBus { get; set; }

        public EventTriggerAsyncBackgroundJob()
        {
            EventBus = NullEventBus.Instance;
        }

        protected override async Task ExecuteAsync(TEventData eventData)
        {
            await EventBus.TriggerAsync(eventData);
        }

        public override void Execute(TEventData eventData)
        {
            EventBus.Trigger(eventData);
        }
    }
}