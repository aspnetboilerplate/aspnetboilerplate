using System;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
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

        [Fact]
        public void Should_Rollback_UOW_In_Updating_Event()
        {
            //Arrange
            var updatingTriggerCount = 0;
            var updatedTriggerCount = 0;

            Resolve<IEventBus>().Register<EntityUpdatingEventData<Person>>(
                eventData =>
                {
                    eventData.Entity.Name.ShouldBe("halil2");
                    updatingTriggerCount++;

                    throw new ApplicationException("A sample exception to rollback the UOW.");
                });

            Resolve<IEventBus>().Register<EntityUpdatedEventData<Person>>(
                eventData =>
                {
                    //Should not come here, since UOW is failed
                    updatedTriggerCount++;
                });

            //Act
            try
            {
                using (var uow = Resolve<IUnitOfWorkManager>().Begin())
                {
                    var person = _personRepository.Single(p => p.Name == "halil");
                    person.Name = "halil2";
                    _personRepository.Update(person);

                    uow.Complete();
                }

                Assert.True(false, "Should not come here since ApplicationException is thrown!");
            }
            catch (ApplicationException)
            {
                //hiding exception
            }

            //Assert
            updatingTriggerCount.ShouldBe(1);
            updatedTriggerCount.ShouldBe(0);

            _personRepository
                .FirstOrDefault(p => p.Name == "halil")
                .ShouldNotBeNull(); //should not be changed since we throw exception to rollback the transaction
        }
    }
}
