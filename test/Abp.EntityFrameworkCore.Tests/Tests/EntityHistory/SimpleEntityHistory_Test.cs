using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Tests.Domain;
using Abp.EntityHistory;
using Castle.MicroKernel.Registration;
using NSubstitute;
using Xunit;

namespace Abp.EntityFrameworkCore.Tests.Tests
{
    public class SimpleEntityHistory_Test : EntityFrameworkCoreModuleTestBase
    {
        private readonly IRepository<Blog> _blogRepository;

        private IEntityHistoryStore _entityHistoryStore;

        public SimpleEntityHistory_Test()
        {
            _blogRepository = Resolve<IRepository<Blog>>();
            Resolve<IEntityHistoryConfiguration>().IsEnabledForAnonymousUsers = true;
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();
            _entityHistoryStore = Substitute.For<IEntityHistoryStore>();
            LocalIocManager.IocContainer.Register(
                Component.For<IEntityHistoryStore>().Instance(_entityHistoryStore).LifestyleSingleton()
                );
        }

        #region CASES WRITE HISTORY

        [Fact]
        public void Should_Write_History_For_Tracked_Entities()
        {
            /* Blog has HistoryTracked attribute. */

            var blog1 = _blogRepository.Single(b => b.Name == "test-blog-1");
            blog1.ChangeUrl("http://testblog1-changed.myblogs.com");
            _blogRepository.Update(blog1);

            _entityHistoryStore.Received().SaveAsync(Arg.Any<EntityChangeSet>());
        }

        #endregion

        #region CASES DON'T WRITE HISTORY

        [Fact]
        public void Should_Not_Write_History_If_Disabled()
        {
            Resolve<IEntityHistoryConfiguration>().IsEnabled = false;

            var blog1 = _blogRepository.Single(b => b.Name == "test-blog-1");
            blog1.ChangeUrl("http://testblog1-changed.myblogs.com");
            _blogRepository.Update(blog1);

            _entityHistoryStore.DidNotReceive().SaveAsync(Arg.Any<EntityChangeSet>());
        }

        #endregion
    }
}
