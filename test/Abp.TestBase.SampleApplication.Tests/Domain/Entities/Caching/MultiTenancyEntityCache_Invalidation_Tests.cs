using Abp.AutoMapper;
using Abp.Dependency;
using Abp.Domain.Entities.Caching;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Caching;
using Abp.TestBase.SampleApplication.ContactLists;
using Abp.TestBase.SampleApplication.Messages;
using Shouldly;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.Domain.Entities.Caching
{
    public class MultiTenancyEntityCache_Invalidation_Tests : SampleApplicationTestBase
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IMessageCache _messageCache;
        private readonly IContactListCache _contactListCache;
        private readonly IRepository<Message> _messageRepository;
        private readonly IRepository<ContactList> _contactListRepository;

        public MultiTenancyEntityCache_Invalidation_Tests()
        {
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();
            _messageCache = Resolve<IMessageCache>();
            _contactListCache = Resolve<IContactListCache>();
            _messageRepository = Resolve<IRepository<Message>>();
            _contactListRepository = Resolve<IRepository<ContactList>>();
        }

        [Fact]
        public void Cached_May_Have_Tenant_Entities_Should_Be_Refreshed_On_Change_Host_Side()
        {
            //Arrange
            Message message1;

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(null))
                {
                    message1 = _messageRepository.Single(m => m.Text == "host-message-1");

                    //Act & Assert
                    _messageCache.Get(message1.Id).Text.ShouldBe(message1.Text);

                    //Arrange: Update the entity
                    message1.Text = "host-message-1-updated";
                    _messageRepository.Update(message1);
                }

                uow.Complete();
            }

            //Arrange
            MessageCacheItem cachedMessage1;
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(null))
                {
                     cachedMessage1 = _messageCache.Get(message1.Id);
                }

                uow.Complete();
            }
            //Assert: Cached object should be updated
            cachedMessage1.Text.ShouldBe(message1.Text);
        }

        [Fact]
        public void Cached_May_Have_Tenant_Entities_Should_Be_Refreshed_On_Change_Tenant_Side()
        {
            //Arrange
            Message message1;

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(1))
                {
                    message1 = _messageRepository.Single(m => m.Text == "tenant-1-message-1");

                    //Act & Assert
                    _messageCache.Get(message1.Id).Text.ShouldBe(message1.Text);

                    //Arrange: Update the entity
                    message1.Text = "tenant-1-message-1-updated";
                    _messageRepository.Update(message1);
                }

                uow.Complete();
            }

            //Arrange
            MessageCacheItem cachedMessage1;
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(1))
                {
                    cachedMessage1 = _messageCache.Get(message1.Id);
                }

                uow.Complete();
            }
            //Assert: Cached object should be updated
            cachedMessage1.Text.ShouldBe(message1.Text);
        }

        [Fact]
        public void Cached_May_Have_Tenant_Entities_Should_Be_Multi_Tenant_Safe()
        {
            //Arrange
            Message message1;

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(1))
                {
                    message1 = _messageRepository.Single(m => m.Text == "tenant-1-message-1");

                    //Act & Assert
                    _messageCache.Get(message1.Id).Text.ShouldBe(message1.Text);
                }

                uow.Complete();
            }

            //Arrange
            MessageCacheItem cachedMessage1;
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(2))
                {
                    cachedMessage1 = _messageCache.Get(message1.Id);
                }

                uow.Complete();
            }
            //Assert: Cached object should be not retrieved for other tenant
            cachedMessage1.ShouldBeNull();
        }

        [Fact]
        public void Cached_Must_Have_Tenant_Entities_Should_Be_Refreshed_On_Change_Tenant_Side()
        {
            //Arrange
            ContactList contact1;

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(1))
                {
                    contact1 = _contactListRepository.Single(c => c.Name == "List of Tenant-1");

                    //Act & Assert
                    _contactListCache.Get(contact1.Id).Name.ShouldBe(contact1.Name);

                    //Arrange: Update the entity
                    contact1.Name = "List of Tenant-1 Updated";
                    _contactListRepository.Update(contact1);
                }

                uow.Complete();
            }

            //Arrange
            ContactListCacheItem cachedContact1;
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(1))
                {
                    cachedContact1 = _contactListCache.Get(contact1.Id);
                }

                uow.Complete();
            }
            //Assert: Cached object should be updated
            cachedContact1.Name.ShouldBe(contact1.Name);
        }

        public interface IMessageCache : IMultiTenancyEntityCache<MessageCacheItem>
        {

        }

        public interface IContactListCache : IMultiTenancyEntityCache<ContactListCacheItem>
        {

        }

        public class MessageCache : MayHaveTenantEntityCache<Message, MessageCacheItem>, IMessageCache, ITransientDependency
        {
            public MessageCache(ICacheManager cacheManager, IUnitOfWorkManager unitOfWorkManager, IRepository<Message, int> repository)
                : base(cacheManager, unitOfWorkManager, repository)
            {

            }
        }

        public class ContactListCache : MustHaveTenantEntityCache<ContactList, ContactListCacheItem>, IContactListCache, ITransientDependency
        {
            public ContactListCache(ICacheManager cacheManager, IUnitOfWorkManager unitOfWorkManager, IRepository<ContactList, int> repository)
                : base(cacheManager, unitOfWorkManager, repository)
            {

            }
        }

        [AutoMapFrom(typeof(Message))]
        public class MessageCacheItem
        {
            public string Text { get; set; }
        }

        [AutoMapFrom(typeof(ContactList))]
        public class ContactListCacheItem
        {
            public string Name { get; set; }
        }
    }
}
