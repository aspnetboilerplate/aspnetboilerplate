using System;
using System.Threading;
using System.Threading.Tasks;
using Abp.Events.Bus;
using Shouldly;
using Xunit;

namespace Abp.Tests.Events.Bus
{
    public class EventRegisterUnregisterTest
    {
        IEventBus eventBus;

        public EventRegisterUnregisterTest()
        {
            eventBus = new EventBus();
        }

        [Fact]
        public void EventBustTest_UnregisterWhileHandlingEvent_ShouldNotThrow()
        {
            // Arrange
            Func<MyEvent, Task> eventHandler = async (axel) => await axel.Semaphore.WaitAsync();
            eventBus.AsyncRegister(eventHandler);
            var myEvent = new MyEvent();

            // Act
            var triggerTask = eventBus.TriggerAsync(myEvent);
            eventBus.AsyncUnregister(eventHandler);
            myEvent.Semaphore.Release();

            // Assert
            Should.NotThrow(async () => await triggerTask);
        }
        
        class MyEvent : EventData
        {
            public SemaphoreSlim Semaphore { get; set; } = new SemaphoreSlim(0);
        }
    }
}