using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Abp.Events.Bus.Entities;
using Abp.NHibernate.Tests.Entities;
using Shouldly;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Abp.NHibernate.Tests
{
    public class Basic_Repository_Tests : NHibernateTestBase
    {
        private readonly IRepository<Person> _personRepository;
        private readonly IRepository<Book> _booksRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public Basic_Repository_Tests()
        {
            _personRepository = Resolve<IRepository<Person>>();
            _booksRepository = Resolve<IRepository<Book>>();
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();

            UsingSession(session => session.Save(new Person() { Name = "emre" }));
            UsingSession(session => session.Save(new Book { Name = "Hitchhikers Guide to the Galaxy" }));
            UsingSession(session => session.Save(new Book { Name = "My First ABCs", IsDeleted = true }));
        }

        [Fact]
        public async Task Should_Get_Null_On_Not_Found()
        {
            var person = _personRepository.FirstOrDefault(-1);
            person.ShouldBeNull();

            person = await _personRepository.FirstOrDefaultAsync(-1);
            person.ShouldBeNull();
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
        public async Task Should_Insert_People_Async()
        {
            await _personRepository.InsertAsync(new Person() { Name = "halil" });

            var insertedPerson = UsingSession(session => session.Query<Person>().FirstOrDefault(p => p.Name == "halil"));
            insertedPerson.ShouldNotBeNull();
            insertedPerson.IsTransient().ShouldBeFalse();
            insertedPerson.Name.ShouldBe("halil");
        }

        [Fact]
        public void Should_Filter_SoftDelete()
        {
            var books = _booksRepository.GetAllList();
            books.All(p => !p.IsDeleted).ShouldBeTrue();
        }

        [Fact]
        public async Task Should_Filter_SoftDelete_Async()
        {
            var books = await _booksRepository.GetAllListAsync();
            books.All(p => !p.IsDeleted).ShouldBeTrue();
        }

        [Fact]
        public void Should_Get_SoftDeleted_Entities_If_Filter_Is_Disabled()
        {
            using (_unitOfWorkManager.Begin())
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {
                var books = _booksRepository.GetAllList();
                books.Any(x => x.IsDeleted).ShouldBe(true);
            }
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
        public async Task Update_With_Action_Test_Async()
        {
            var userBefore = UsingSession(session => session.Query<Person>().Single(p => p.Name == "emre"));

            var updatedUser =await _personRepository.UpdateAsync(userBefore.Id, user =>
            {
                user.Name = "yunus";
                return Task.FromResult(user);
            });
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
        public async Task Should_Trigger_Event_On_Insert_Async()
        {
            var triggerCount = 0;

            Resolve<IEventBus>().Register<EntityCreatedEventData<Person>>(
                eventData =>
                {
                    eventData.Entity.Name.ShouldBe("halil");
                    eventData.Entity.IsTransient().ShouldBeFalse();
                    triggerCount++;
                });

            await _personRepository.InsertAsync(new Person { Name = "halil" });

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

            var emrePerson = _personRepository.Single(p => p.Name == "emre");
            emrePerson.Name = "emre2";
            _personRepository.Update(emrePerson);

            triggerCount.ShouldBe(1);
        }

        [Fact]
        public async Task Should_Trigger_Event_On_Update_Async()
        {
            var triggerCount = 0;

            Resolve<IEventBus>().Register<EntityUpdatedEventData<Person>>(
                eventData =>
                {
                    eventData.Entity.Name.ShouldBe("emre2");
                    triggerCount++;
                });

            var emrePerson = await _personRepository.SingleAsync(p => p.Name == "emre");
            emrePerson.Name = "emre2";
            await _personRepository.UpdateAsync(emrePerson);

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

            var emrePerson = _personRepository.Single(p => p.Name == "emre");
            _personRepository.Delete(emrePerson.Id);

            triggerCount.ShouldBe(1);
            _personRepository.FirstOrDefault(p => p.Name == "emre").ShouldBe(null);
        }

        [Fact]
        public async Task Should_Trigger_Event_On_Delete_Async()
        {
            var triggerCount = 0;
            Resolve<IEventBus>().Register<EntityDeletedEventData<Person>>(
                eventData =>
                {
                    eventData.Entity.Name.ShouldBe("emre");
                    triggerCount++;
                });

            var emrePerson = await _personRepository.SingleAsync(p => p.Name == "emre");
            await _personRepository.DeleteAsync(emrePerson.Id);

            triggerCount.ShouldBe(1);
            var deletedPerson = await _personRepository.FirstOrDefaultAsync(p => p.Name == "emre");
            deletedPerson.ShouldBeNull();
        }
    }
}
