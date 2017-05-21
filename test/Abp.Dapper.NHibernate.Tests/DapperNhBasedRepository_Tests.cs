using System.Linq;

using Abp.Dapper.Repositories;
using Abp.Domain.Repositories;
using Abp.Events.Bus;
using Abp.Events.Bus.Entities;

using NHibernate.Linq;

using Shouldly;

using Xunit;

namespace Abp.Dapper.NHibernate.Tests
{
    public class DapperNhBasedRepository_Tests : DapperNhBasedApplicationTestBase
    {
        private readonly IRepository<Person> _personRepository;
        private readonly IDapperRepository<Person> _personDapperRepository;

        public DapperNhBasedRepository_Tests()
        {
            _personRepository = Resolve<IRepository<Person>>();
            _personDapperRepository = Resolve<IDapperRepository<Person>>();
            UsingSession(session => session.Save(new Person("Oguzhan_Initial")));
        }

        [Fact]
        public void Should_Get_All_People()
        {
            _personRepository.Insert(new Person("Oguzhan"));

            _personDapperRepository.GetAll().Count().ShouldBe(2);
        }

        [Fact]
        public void Should_Insert_People()
        {
            _personRepository.Insert(new Person("Oguzhan2"));

            Person insertedPerson = UsingSession(session => session.Query<Person>().FirstOrDefault(p => p.Name == "Oguzhan2"));
            insertedPerson.ShouldNotBe(null);
            insertedPerson.IsTransient().ShouldBe(false);
            insertedPerson.Name.ShouldBe("Oguzhan2");

            Person insertedPersonFromDapper = _personDapperRepository.FirstOrDefault(x => x.Name == "Oguzhan2");
            insertedPersonFromDapper.ShouldNotBe(null);
            insertedPersonFromDapper.IsTransient().ShouldBe(false);
            insertedPersonFromDapper.Name.ShouldBe("Oguzhan2");
        }

        [Fact]
        public void Update_With_Action_Test()
        {
            Person userBefore = UsingSession(session => session.Query<Person>().Single(p => p.Name == "Oguzhan_Initial"));

            Person updatedUser = _personRepository.Update(userBefore.Id, user => user.Name = "Oguzhan_Updated_With_NH");
            updatedUser.Id.ShouldBe(userBefore.Id);
            updatedUser.Name.ShouldBe("Oguzhan_Updated_With_NH");

            Person userAfter = UsingSession(session => session.Get<Person>(userBefore.Id));
            userAfter.Name.ShouldBe("Oguzhan_Updated_With_NH");

            Person updatedWithNh = _personDapperRepository.FirstOrDefault(x => x.Name == "Oguzhan_Updated_With_NH");
            updatedWithNh.Name = "Oguzhan_Updated_With_Dapper";
            _personDapperRepository.Update(updatedWithNh);

            Person updatedWithDapper = _personDapperRepository.Get(updatedWithNh.Id);
            updatedWithDapper.Name.ShouldBe("Oguzhan_Updated_With_Dapper");
        }

        [Fact]
        public void Should_Trigger_Event_On_Insert()
        {
            var triggerCount = 0;

            Resolve<IEventBus>().Register<EntityCreatedEventData<Person>>(
                eventData =>
                {
                    eventData.Entity.Name.ShouldBe("Oguzhan_To_Fire_Event");
                    eventData.Entity.IsTransient().ShouldBe(false);
                    triggerCount++;
                });

            _personDapperRepository.Insert(new Person("Oguzhan_To_Fire_Event"));

            triggerCount.ShouldBe(1);
        }

        [Fact]
        public void Should_Trigger_Event_On_Update()
        {
            var triggerCount = 0;

            Resolve<IEventBus>().Register<EntityUpdatedEventData<Person>>(
                eventData =>
                {
                    eventData.Entity.Name.ShouldBe("Oguzhan_Updated_Event_Fire");
                    triggerCount++;
                });

            Person person = _personDapperRepository.Single(p => p.Name == "Oguzhan_Initial");
            person.Name = "Oguzhan_Updated_Event_Fire";
            _personDapperRepository.Update(person);

            triggerCount.ShouldBe(1);
        }

        [Fact]
        public void Should_Trigger_Event_On_Delete()
        {
            var triggerCount = 0;

            Resolve<IEventBus>().Register<EntityDeletedEventData<Person>>(
                eventData =>
                {
                    eventData.Entity.Name.ShouldBe("Oguzhan_Initial");
                    triggerCount++;
                });

            Person person = _personDapperRepository.Single(p => p.Name == "Oguzhan_Initial");
            _personDapperRepository.Delete(person);

            triggerCount.ShouldBe(1);
            _personDapperRepository.FirstOrDefault(p => p.Name == "Oguzhan_Initial").ShouldBe(null);
        }
    }
}
