using Abp.Domain.Repositories;
using Abp.Events.Bus;
using Abp.Events.Bus.Entities;
using Abp.TestBase.SampleApplication.People;
using Shouldly;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.People
{
    public class PersonRepository_Tests_For_EntityChangeEvents : SampleApplicationTestBase
    {
        private readonly IRepository<Person> _personRepository;

        public PersonRepository_Tests_For_EntityChangeEvents()
        {
            _personRepository = Resolve<IRepository<Person>>();
        }

        [Fact]
        public void Should_Trigger_All_Events_On_Create()
        {
            var changingTriggerCount = 0;
            var creatingTriggerCount = 0;

            var changedTriggerCount = 0;
            var createdTriggerCount = 0;

            Resolve<IEventBus>().Register<EntityChangingEventData<Person>>(
                eventData =>
                {
                    eventData.Entity.Name.ShouldBe("halil");
                    eventData.Entity.IsTransient().ShouldBe(true);
                    changingTriggerCount++;
                });

            Resolve<IEventBus>().Register<EntityCreatingEventData<Person>>(
                eventData =>
                {
                    eventData.Entity.Name.ShouldBe("halil");
                    eventData.Entity.IsTransient().ShouldBe(true);
                    creatingTriggerCount++;
                });

            Resolve<IEventBus>().Register<EntityChangedEventData<Person>>(
                eventData =>
                {
                    eventData.Entity.Name.ShouldBe("halil");
                    eventData.Entity.IsTransient().ShouldBe(false);
                    changedTriggerCount++;
                });

            Resolve<IEventBus>().Register<EntityCreatedEventData<Person>>(
                eventData =>
                {
                    eventData.Entity.Name.ShouldBe("halil");
                    eventData.Entity.IsTransient().ShouldBe(false);
                    createdTriggerCount++;
                });

            _personRepository.Insert(new Person { ContactListId = 1, Name = "halil" });

            changingTriggerCount.ShouldBe(1);
            creatingTriggerCount.ShouldBe(1);

            changedTriggerCount.ShouldBe(1);
            createdTriggerCount.ShouldBe(1);
        }

        [Fact]
        public void Should_Trigger_Event_On_Update()
        {
            var triggerCount = 0;

            Resolve<IEventBus>().Register<EntityUpdatedEventData<Person>>(
                eventData =>
                {
                    eventData.Entity.Name.ShouldBe("halil2");
                    triggerCount++;
                });

            var person = _personRepository.Single(p => p.Name == "halil");
            person.Name = "halil2";
            _personRepository.Update(person);

            triggerCount.ShouldBe(1);
        }


        [Fact]
        public void Should_Trigger_Event_On_Delete()
        {
            var triggerCount = 0;

            Resolve<IEventBus>().Register<EntityDeletedEventData<Person>>(
                eventData =>
                {
                    eventData.Entity.Name.ShouldBe("halil");
                    triggerCount++;
                });

            var person = _personRepository.Single(p => p.Name == "halil");
            _personRepository.Delete(person.Id);

            triggerCount.ShouldBe(1);
        }
    }
}
