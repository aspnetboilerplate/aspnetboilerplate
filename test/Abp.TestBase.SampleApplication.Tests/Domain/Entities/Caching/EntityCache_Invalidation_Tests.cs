using Abp.AutoMapper;
using Abp.Dependency;
using Abp.Domain.Entities.Caching;
using Abp.Domain.Repositories;
using Abp.Runtime.Caching;
using Abp.TestBase.SampleApplication.Messages;
using Shouldly;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.Domain.Entities.Caching
{
    public class EntityCache_Invalidation_Tests : SampleApplicationTestBase
    {
        private readonly IMessageCache _messageCache;
        private readonly IRepository<Message> _messageRepository;

        public EntityCache_Invalidation_Tests()
        {
            _messageCache = Resolve<IMessageCache>();
            _messageRepository = Resolve<IRepository<Message>>();
        }

        [Fact]
        public void Cached_Entities_Should_Be_Refreshed_On_Change()
        {
            //Arrange
            AbpSession.TenantId = 1;
            var message1 = _messageRepository.Single(m => m.Text == "tenant-1-message-1");

            //Act & Assert
            _messageCache.Get(message1.Id).Text.ShouldBe(message1.Text);

            //Arrange: Update the entity
            message1.Text = "host-message-1-updated";
            _messageRepository.Update(message1);

            //Act & Assert: Cached object should be updated
            _messageCache.Get(message1.Id).Text.ShouldBe(message1.Text);
        }

        public interface IMessageCache : IEntityCache<MessageCacheItem>
        {

        }

        public class MessageCache : EntityCache<Message, MessageCacheItem>, IMessageCache, ITransientDependency
        {
            public MessageCache(ICacheManager cacheManager, IRepository<Message, int> repository)
                : base(cacheManager, repository)
            {

            }
        }

        [AutoMapFrom(typeof(Message))]
        public class MessageCacheItem
        {
            public string Text { get; set; }
        }
    }
}
