using System;
using Abp.Domain.Entities;
using Abp.Events.Bus.Entities;
using Shouldly;
using Xunit;

namespace Abp.Tests.Events.Bus
{
    public class GenericInheritanceTest : EventBusTestBase
    {
        public class Person : Entity
        {
        }

        public class Student : Person
        {
        }

        [Fact]
        public void Should_Trigger_For_Inherited_Generic_1()
        {
            var triggeredEvent = false;

            EventBus.Register<EntityChangedEventData<Person>>(
                eventData =>
                {
                    eventData.Entity.Id.ShouldBe(Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000042"));
                    triggeredEvent = true;
                });
            EventBus.Trigger(
                new EntityUpdatedEventData<Person>(new Person {Id = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000042")}));

            triggeredEvent.ShouldBe(true);
        }

        [Fact]
        public void Should_Trigger_For_Inherited_Generic_2()
        {
            var triggeredEvent = false;

            EventBus.Register<EntityChangedEventData<Person>>(
                eventData =>
                {
                    eventData.Entity.Id.ShouldBe(Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000042"));
                    triggeredEvent = true;
                });

            EventBus.Trigger(
                new EntityChangedEventData<Student>(new Student
                {
                    Id = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000042")
                }));

            triggeredEvent.ShouldBe(true);
        }
    }
}