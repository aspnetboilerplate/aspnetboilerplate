using System.Linq;
using Abp.Domain.Repositories;
using Abp.Events.Bus;
using Abp.Events.Bus.Entities;
using Abp.NHibernate.Tests.Entities;
using NHibernate.Linq;
using Shouldly;
using Xunit;

namespace Abp.NHibernate.Tests
{
    public class Basic_Repository_Tests : NHibernateTestBase
    {
        private readonly IRepository<Person> _personRepository;

        public Basic_Repository_Tests()
        {
            _personRepository = Resolve<IRepository<Person>>();
            UsingSession(session => session.Save(new Person() { Name = "emre" }));
        }

        [Fact]
        public void Should_Get_All_People()
        {
            _personRepository.GetAllList().Count.ShouldBe(1);
        }

        [Fact]
        public void Should_Insert_People()
        {
            _personRepository.Insert(new Person() { Name = "halil" });

            var insertedPerson = UsingSession(session => session.Query<Person>().FirstOrDefault(p => p.Name == "halil"));
            insertedPerson.ShouldNotBe(null);
            insertedPerson.IsTransient().ShouldBe(false);
            insertedPerson.Name.ShouldBe("halil");
        }

        [Fact]
        public void Update_With_Action_Test()
        {
            var userBefore = UsingSession(session => session.Query<Person>().Single(p => p.Name == "emre"));

            var updatedUser = _personRepository.Update(userBefore.Id, user => user.Name = "yunus");
            updatedUser.Id.ShouldBe(userBefore.Id);
            updatedUser.Name.ShouldBe("yunus");

            var userAfter = UsingSession(session => session.Get<Person>(userBefore.Id));
            userAfter.Name.ShouldBe("yunus");
        }

        [Fact]
        public void Should_Trigger_Event_On_Insert()
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
            _personRepository.FirstOrDefault(p => p.Name == "emre").ShouldBe(null);
        }
    }
}
