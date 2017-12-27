using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Tests.Domain;
using Abp.EntityHistory;
using Abp.Extensions;
using Abp.Json;
using Castle.MicroKernel.Registration;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NSubstitute;
using System.Collections.Generic;
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

            var newValue = "http://testblog1-changed.myblogs.com";
            var originalValue = UpdateBlogUrlAndGetOriginalValue(newValue);

            _entityHistoryStore.Received().SaveAsync(Arg.Is<EntityChangeSet>(
                s => s.EntityChanges.Count == 1 &&
                     s.EntityChanges[0].ChangeType == Events.Bus.Entities.EntityChangeType.Updated &&
                     s.EntityChanges[0].EntityId == s.EntityChanges[0].EntityEntry.As<EntityEntry>().Entity.As<IEntity>().Id.ToJsonString(false, false) &&
                     s.EntityChanges[0].EntityTypeAssemblyQualifiedName == typeof(Blog).AssemblyQualifiedName &&
                     s.EntityChanges[0].PropertyChanges.Count == 1 &&
                     s.EntityChanges[0].PropertyChanges.FirstOrDefault().NewValue == newValue.ToJsonString(false, false) &&
                     s.EntityChanges[0].PropertyChanges.FirstOrDefault().OriginalValue == originalValue.ToJsonString(false, false) &&
                     s.EntityChanges[0].PropertyChanges.FirstOrDefault().PropertyName == nameof(Blog.Url) &&
                     s.EntityChanges[0].PropertyChanges.FirstOrDefault().PropertyTypeName == typeof(Blog).GetProperty(nameof(Blog.Url)).PropertyType.AssemblyQualifiedName &&
                     s.EntityChanges[0].TenantId == AbpSession.TenantId &&
                     s.EntityChanges[0].UserId == AbpSession.UserId
            ));
        }

        #endregion

        #region CASES DON'T WRITE HISTORY

        [Fact]
        public void Should_Not_Write_History_If_Disabled()
        {
            Resolve<IEntityHistoryConfiguration>().IsEnabled = false;

            /* Blog has HistoryTracked attribute. */

            var newValue = "http://testblog1-changed.myblogs.com";
            var originalValue = UpdateBlogUrlAndGetOriginalValue(newValue);

            _entityHistoryStore.DidNotReceive().SaveAsync(Arg.Any<EntityChangeSet>());
        }

        #endregion

        private string UpdateBlogUrlAndGetOriginalValue(string newValue)
        {
            string originalValue;

            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                var blog1 = _blogRepository.Single(b => b.Name == "test-blog-1");
                originalValue = blog1.Url;

                blog1.ChangeUrl(newValue);
                _blogRepository.Update(blog1);

                uow.Complete();
            }

            return originalValue;
        }
    }

    #region Helpers

    internal static class IEnumerableExtensions
    {
        internal static EntityPropertyChangeInfo FirstOrDefault(this IEnumerable<EntityPropertyChangeInfo> enumerable)
        {
            var enumerator = enumerable.GetEnumerator();
            enumerator.MoveNext();
            return enumerator.Current;
        }
    }

    #endregion
}
