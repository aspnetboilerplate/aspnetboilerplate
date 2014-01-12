using System;
using NUnit.Framework;

namespace Abp.Tests.Events.Bus
{
    [TestFixture]
    public class ActionBasedEventHandlerTest : EventBusTest
    {
        [Test]
        public void Should_Call_Action_On_Event_With_Correct_Source()
        {
            var totalData = 0;

            EventBus.Register<MySimpleEventData>(
                eventData =>
                    {
                        totalData += eventData.Value;
                        Assert.AreEqual(this, eventData.EventSource);
                    });

            EventBus.Trigger(this, new MySimpleEventData(1));
            EventBus.Trigger(this, new MySimpleEventData(2));
            EventBus.Trigger(this, new MySimpleEventData(3));
            EventBus.Trigger(this, new MySimpleEventData(4));

            Assert.AreEqual(10, totalData);
        }

        [Test]
        public void Should_Not_Call_Action_After_Unregister_1()
        {
            var totalData = 0;

            var registerDisposer = EventBus.Register<MySimpleEventData>(
                eventData =>
                {
                    totalData += eventData.Value;
                });

            EventBus.Trigger(this, new MySimpleEventData(1));
            EventBus.Trigger(this, new MySimpleEventData(2));
            EventBus.Trigger(this, new MySimpleEventData(3));

            registerDisposer.Dispose();

            EventBus.Trigger(this, new MySimpleEventData(4));

            Assert.AreEqual(6, totalData);
        }

        [Test]
        public void Should_Not_Call_Action_After_Unregister_2()
        {
            var totalData = 0;

            var action = new Action<MySimpleEventData>(
                eventData =>
                    {
                        totalData += eventData.Value;
                    });

            EventBus.Register(action);

            EventBus.Trigger(this, new MySimpleEventData(1));
            EventBus.Trigger(this, new MySimpleEventData(2));
            EventBus.Trigger(this, new MySimpleEventData(3));

            EventBus.Unregister(action);

            EventBus.Trigger(this, new MySimpleEventData(4));

            Assert.AreEqual(6, totalData);
        }
    }
}
