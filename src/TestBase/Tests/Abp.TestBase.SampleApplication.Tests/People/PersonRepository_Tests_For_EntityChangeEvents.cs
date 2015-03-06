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

            UsingDbContext(context => context.People.Add(new Person() { Name = "emre" }));
        }

        [Fact]
        public void Should_Trigger_Event_On_Create()
        {
            var triggerCount = 0;

            Resolve<IEventBus>().Register<EntityCreatedEventData<Person>>(
                eventData =>
                {
                    eventData.Entity.Name.ShouldBe("halil");
                    eventData.Entity.IsTransient().ShouldBe(false);
                    triggerCount++;
                });

            _personRepository.Insert(new Person { Name = "halil" });

            triggerCount.ShouldBe(1);
        }

        [Fact]
        public void Should_Trigger_Event_On_Update()
        {
            var triggerCount = 0;

            Resolve<IEventBus>().Register<EntityUpdatedEventData<Person>>(
                eventData =>
                {
                    eventData.Entity.Name.ShouldBe("emre2");
                    triggerCount++;
                });

            var emrePeson = _personRepository.Single(p => p.Name == "emre");
            emrePeson.Name = "emre2";
            _personRepository.Update(emrePeson);

            triggerCount.ShouldBe(1);
        }


        [Fact]
        public void Should_Trigger_Event_On_Delete()
        {
            var triggerCount = 0;

            Resolve<IEventBus>().Register<EntityDeletedEventData<Person>>(
                eventData =>
                {
                    eventData.Entity.Name.ShouldBe("emre");
                    triggerCount++;
                });

            var emrePeson = _personRepository.Single(p => p.Name == "emre");
            _personRepository.Delete(emrePeson.Id);

            triggerCount.ShouldBe(1);
        }
    }
}
