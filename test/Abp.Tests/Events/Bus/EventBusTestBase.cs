using Abp.Events.Bus;

namespace Abp.Tests.Events.Bus
{
    public abstract class EventBusTestBase
    {
        protected IEventBus EventBus;

        protected EventBusTestBase()
        {
            EventBus = new EventBus();
        }
    }
}