using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Events.Bus;

namespace Abp.BackgroundJobs
{
    public class EventTriggerAsyncBackgroundJob<T> : AsyncBackgroundJob<T>, ITransientDependency
        where T:EventData
    {
        public EventTriggerAsyncBackgroundJob(IEventBus eventBus)
        {
            
            EventBus =eventBus ?? NullEventBus.Instance;
        }

        
        /// <summary>
        /// this property is not injected, so we have to inject it in constructor
        /// </summary>
        public IEventBus EventBus { get; set; }
        
        protected override async Task ExecuteAsync(T args)
        {
            await EventBus.TriggerAsync(args);
        }
    }
}