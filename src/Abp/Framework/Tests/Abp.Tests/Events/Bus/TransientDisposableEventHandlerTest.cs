using Xunit;

namespace Abp.Tests.Events.Bus
{
    public class TransientDisposableEventHandlerTest : EventBusTestBase
    {
        [Fact]
        public void Should_Call_Handler_AndDispose()
        {
            EventBus.Register<MySimpleEventData, MySimpleTransientEventHandler>();
            
            EventBus.Trigger(new MySimpleEventData(1));
            EventBus.Trigger(new MySimpleEventData(2));
            EventBus.Trigger(new MySimpleEventData(3));

            Assert.Equal(MySimpleTransientEventHandler.HandleCount, 3);
            Assert.Equal(MySimpleTransientEventHandler.DisposeCount, 3);
        }
    }
}