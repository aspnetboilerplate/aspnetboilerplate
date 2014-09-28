using Xunit;

namespace Abp.Tests.Events.Bus
{
    public class InheritanceTest : EventBusTestBase
    {
        [Fact]
        public void Should_Handle_Events_For_Derived_Classes()
        {
            var totalData = 0;

            EventBus.Register<MySimpleEventData>(
                eventData =>
                {
                    totalData += eventData.Value;
                    Assert.Equal(this, eventData.EventSource);
                });

            EventBus.Trigger(this, new MySimpleEventData(1)); //Should handle directly registered class
            EventBus.Trigger(this, new MySimpleEventData(2)); //Should handle directly registered class
            EventBus.Trigger(this, new MyDerivedEventData(3)); //Should handle derived class too
            EventBus.Trigger(this, new MyDerivedEventData(4)); //Should handle derived class too

            Assert.Equal(10, totalData);
        }

        [Fact]
        public void Should_Not_Handle_Events_For_Base_Classes()
        {
            var totalData = 0;

            EventBus.Register<MyDerivedEventData>(
                eventData =>
                {
                    totalData += eventData.Value;
                    Assert.Equal(this, eventData.EventSource);
                });

            EventBus.Trigger(this, new MySimpleEventData(1)); //Should not handle
            EventBus.Trigger(this, new MySimpleEventData(2)); //Should not handle
            EventBus.Trigger(this, new MyDerivedEventData(3)); //Should handle
            EventBus.Trigger(this, new MyDerivedEventData(4)); //Should handle

            Assert.Equal(7, totalData);
        }   
    }
}