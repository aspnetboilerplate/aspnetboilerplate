using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Tests.Domain;
using Abp.Events.Bus;
using Shouldly;
using Xunit;

namespace Abp.EntityFrameworkCore.Tests.Tests
{
    public class DomainEvents_Tests : EntityFrameworkCoreModuleTestBase
    {
        private readonly IRepository<Blog> _blogRepository;
        private readonly IEventBus _eventBus;

        public DomainEvents_Tests()
        {
            _blogRepository = Resolve<IRepository<Blog>>();
            _eventBus = Resolve<IEventBus>();
        }

        [Fact]
        public void Should_Trigger_Domain_Events_For_Aggregate_Root()
        {
            //Arrange

            var isTriggered = false;

            _eventBus.Register<BlogUrlChangedEventData>((data) =>
            {
                data.OldUrl.ShouldBe("http://testblog1.myblogs.com");
                isTriggered = true;
            });

            //Act

            var blog1 = _blogRepository.Single(b => b.Name == "test-blog-1");
            blog1.ChangeUrl("http://testblog1-changed.myblogs.com");
            _blogRepository.Update(blog1);

            //Assert

            isTriggered.ShouldBeTrue();
        }
    }
}