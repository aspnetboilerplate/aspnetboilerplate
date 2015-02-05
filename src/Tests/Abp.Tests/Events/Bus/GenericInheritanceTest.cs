using Abp.Domain.Entities;
using Abp.Events.Bus.Entities;
using Shouldly;
using Xunit;

namespace Abp.Tests.Events.Bus
{
    public class GenericInheritanceTest : EventBusTestBase
    {
        //[Fact] //not implemented yet
        public void Should_Trigger_For_Inherited_Generic_1()
        {
            var triggeredEvent = false;

            EventBus.Register<EntityUpdatedEventData<Person>>(
                eventData =>
                {
                    eventData.Entity.Id.ShouldBe(42);
                    triggeredEvent = true;
                });

            EventBus.Trigger(new EntityUpdatedEventData<Student>(new Student { Id = 42 }));

            triggeredEvent.ShouldBe(true);
        }
        
        public class Person : Entity
        {
            
        }

        public class Student : Person
        {
            
        }
    }
}