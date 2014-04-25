using NUnit.Framework;

namespace Abp.Tests.Events.Bus
{
    [TestFixture]
    public class TransientDisposableEventHandlerTest : EventBusTestBase
    {
        [Test]
        public void Should_Call_Handler_AndDispose()
        {
            EventBus.Register<MySimpleEventData, MySimpleTransientEventHandler>();
            
            EventBus.Trigger(new MySimpleEventData(1));
            EventBus.Trigger(new MySimpleEventData(2));
            EventBus.Trigger(new MySimpleEventData(3));

            Assert.AreEqual(MySimpleTransientEventHandler.HandleCount, 3);
            Assert.AreEqual(MySimpleTransientEventHandler.DisposeCount, 3);
        }
    }
}