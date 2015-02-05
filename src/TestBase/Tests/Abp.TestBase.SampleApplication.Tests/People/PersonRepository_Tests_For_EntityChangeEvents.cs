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

        //[Fact] //Not implemented yet
        public void Should_Trigger_Event_On_Create()
        {
            var triggeredEvent = false;

            Resolve<IEventBus>().Register<EntityCreatedEventData<Person>>(
                eventData =>
                {
                    eventData.Entity.Name.ShouldBe("halil");
                    eventData.Entity.IsTransient().ShouldBe(false);
                    triggeredEvent = true;
                });

            _personRepository.Insert(new Person { Name = "halil" });

            triggeredEvent.ShouldBe(true);
        }

        //[Fact] //Not implemented yet
        public void Should_Trigger_Event_On_Update()
        {
            var triggeredEvent = false;

            Resolve<IEventBus>().Register<EntityUpdatedEventData<Person>>(
                eventData =>
                {
                    eventData.Entity.Name.ShouldBe("emre2");
                    triggeredEvent = true;
                });

            var emrePeson = _personRepository.Single(p => p.Name == "emre");
            emrePeson.Name = "emre2";
            _personRepository.Update(emrePeson);

            triggeredEvent.ShouldBe(true);
        }
    }
}
