using Abp.Events.Bus;
using NUnit.Framework;

namespace Abp.Tests.Events.Bus
{
    public abstract class EventBusTest
    {
        protected IEventBus EventBus;

        [TestFixtureSetUp]
        public void Initialize()
        {
            EventBus = new EventBus();
        }
    }
}