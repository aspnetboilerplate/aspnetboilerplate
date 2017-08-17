using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Abp.TestBase.SampleApplication.Messages;
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
        
        [Theory]
        [InlineData(TransactionScopeOption.Required)]
        [InlineData(TransactionScopeOption.RequiresNew)]
        [InlineData(TransactionScopeOption.Suppress)]
        public void Should_Trigger_All_Events_On_Create_For_All_Transaction_Scopes(TransactionScopeOption scopeOption)
        {
            var changingTriggerCount = 0;
            var creatingTriggerCount = 0;

            var changedTriggerCount = 0;
            var createdTriggerCount = 0;

            Resolve<IEventBus>().Register<EntityChangingEventData<Person>>(
                eventData =>
                {
                    eventData.Entity.Name.ShouldBe("halil");
                    eventData.Entity.IsTransient().ShouldBe(false);
                    changingTriggerCount++;
                    changedTriggerCount.ShouldBe(0);
                });

            Resolve<IEventBus>().Register<EntityCreatingEventData<Person>>(
                eventData =>
                {
                    eventData.Entity.Name.ShouldBe("halil");
                    eventData.Entity.IsTransient().ShouldBe(false);
                    creatingTriggerCount++;
                    createdTriggerCount.ShouldBe(0);
                });

            Resolve<IEventBus>().Register<EntityChangedEventData<Person>>(
                eventData =>
                {
                    eventData.Entity.Name.ShouldBe("halil");
                    eventData.Entity.IsTransient().ShouldBe(false);
                    changingTriggerCount.ShouldBe(1);
                    changedTriggerCount++;
                });

            Resolve<IEventBus>().Register<EntityCreatedEventData<Person>>(
                eventData =>
                {
                    eventData.Entity.Name.ShouldBe("halil");
                    eventData.Entity.IsTransient().ShouldBe(false);
                    creatingTriggerCount.ShouldBe(1);
                    createdTriggerCount++;
                });

            using (var uow = Resolve<IUnitOfWorkManager>().Begin(scopeOption))
            {
                _personRepository.Insert(new Person { ContactListId = 1, Name = "halil" });

                changingTriggerCount.ShouldBe(0);
                creatingTriggerCount.ShouldBe(0);

                changedTriggerCount.ShouldBe(0);
                createdTriggerCount.ShouldBe(0);

                uow.Complete();
            }

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
                    eventData.Entity.CreatorUserId.ShouldNotBeNull();
                    eventData.Entity.CreatorUserId.ShouldBe(42);
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

        [Fact]
        public async Task Should_Insert_A_New_Entity_On_EntityCreating_Event()
        {
            var person = await _personRepository.InsertAsync(new Person { Name = "Tuana", ContactListId = 1 });
            person.IsTransient().ShouldBeFalse();

            var text = string.Format("{0} is being created with Id = {1}!", person.Name, person.Id);
            UsingDbContext(context =>
            {
                var message = context.Messages.FirstOrDefault(m => m.Text == text && m.TenantId == PersonHandler.FakeTenantId);
                message.ShouldNotBeNull();
            });
        }

        [Fact]
        public async Task Should_Insert_A_New_Entity_On_EntityCreated_Event()
        {
            var person = await _personRepository.InsertAsync(new Person { Name = "Tuana", ContactListId = 1 });
            person.IsTransient().ShouldBeFalse();

            var text = string.Format("{0} is created with Id = {1}!", person.Name, person.Id);
            UsingDbContext(context =>
            {
                var message = context.Messages.FirstOrDefault(m => m.Text == text && m.TenantId == PersonHandler.FakeTenantId);
                message.ShouldNotBeNull();
            });
        }

        public class PersonHandler : IEventHandler<EntityCreatingEventData<Person>>, IEventHandler<EntityCreatedEventData<Person>>, ITransientDependency
        {
            public const int FakeTenantId = 65910381;

            private readonly IRepository<Message> _messageRepository;

            public PersonHandler(IRepository<Message> messageRepository)
            {
                _messageRepository = messageRepository;
            }

            public void HandleEvent(EntityCreatingEventData<Person> eventData)
            {
                _messageRepository.Insert(new Message(FakeTenantId, string.Format("{0} is being created with Id = {1}!", eventData.Entity.Name, eventData.Entity.Id)));
            }

            public void HandleEvent(EntityCreatedEventData<Person> eventData)
            {
                _messageRepository.Insert(new Message(FakeTenantId, string.Format("{0} is created with Id = {1}!", eventData.Entity.Name, eventData.Entity.Id)));
            }
        }
    }
}
