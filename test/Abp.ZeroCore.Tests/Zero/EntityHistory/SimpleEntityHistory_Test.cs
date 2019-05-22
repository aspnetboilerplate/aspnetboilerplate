using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityHistory;
using Abp.Events.Bus.Entities;
using Abp.Extensions;
using Abp.Json;
using Abp.Threading;
using Abp.Timing;
using Abp.ZeroCore.SampleApp.Core.EntityHistory;
using Castle.MicroKernel.Registration;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Abp.Zero.EntityHistory
{
    public class SimpleEntityHistory_Test : AbpZeroTestBase
    {
        private readonly IRepository<Blog> _blogRepository;
        private readonly IRepository<Post, Guid> _postRepository;
        private readonly IRepository<Comment> _commentRepository;

        private IEntityHistoryStore _entityHistoryStore;

        public SimpleEntityHistory_Test()
        {
            _blogRepository = Resolve<IRepository<Blog>>();
            _postRepository = Resolve<IRepository<Post, Guid>>();
            _commentRepository = Resolve<IRepository<Comment>>();

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
        public void Should_Write_History_For_Tracked_Entities_Create()
        {
            /* Blog has Audited attribute. */

            var blog2Id = CreateBlogAndGetId();

            _entityHistoryStore.Received().Save(Arg.Is<EntityChangeSet>(
                s => s.EntityChanges.Count == 2 &&
                     s.EntityChanges[0].ChangeTime == s.EntityChanges[0].EntityEntry.As<EntityEntry>().Entity.As<IHasCreationTime>().CreationTime &&
                     s.EntityChanges[0].ChangeType == EntityChangeType.Created &&
                     s.EntityChanges[0].EntityId == blog2Id.ToJsonString(false, false) &&
                     s.EntityChanges[0].EntityTypeFullName == typeof(Blog).FullName &&
                     s.EntityChanges[0].PropertyChanges.Count == 2 && // Blog.Name, Blog.Url

                     s.EntityChanges[1].ChangeType == EntityChangeType.Created &&
                     s.EntityChanges[1].EntityId == blog2Id.ToJsonString(false, false) &&
                     s.EntityChanges[1].EntityTypeFullName == typeof(BlogEx).FullName &&
                     s.EntityChanges[1].PropertyChanges.Count == 1 && // BlogEx.BloggerName

                     // Check "who did this change"
                     s.ImpersonatorTenantId == AbpSession.ImpersonatorTenantId &&
                     s.ImpersonatorUserId == AbpSession.ImpersonatorUserId &&
                     s.TenantId == AbpSession.TenantId &&
                     s.UserId == AbpSession.UserId
            ));
        }

        [Fact]
        public void Should_Write_History_For_Tracked_Entities_Create_To_Database()
        {
            // Forward calls from substitute to implementation
            var entityHistoryStore = Resolve<EntityHistoryStore>();
            _entityHistoryStore.When(x => x.SaveAsync(Arg.Any<EntityChangeSet>()))
                .Do(callback => AsyncHelper.RunSync(() =>
                    entityHistoryStore.SaveAsync(callback.Arg<EntityChangeSet>()))
                );
            _entityHistoryStore.When(x => x.Save(Arg.Any<EntityChangeSet>()))
                .Do(callback => entityHistoryStore.Save(callback.Arg<EntityChangeSet>()));

            const int tenantId = 1;

            UsingDbContext(tenantId, (context) =>
            {
                context.EntityChanges.Count(f => f.TenantId == tenantId).ShouldBe(0);
            });

            UsingDbContext(tenantId, (context) =>
            {
                context.EntityChangeSets.Count(f => f.TenantId == tenantId).ShouldBe(0);
            });

            UsingDbContext(tenantId, (context) =>
            {
                context.EntityPropertyChanges.Count(f => f.TenantId == tenantId).ShouldBe(0);
            });

            var justNow = Clock.Now;
            var blog2Id = CreateBlogAndGetId();

            UsingDbContext(tenantId, (context) =>
            {
                context.EntityChanges.Count(f => f.TenantId == tenantId).ShouldBe(2);
            });

            UsingDbContext(tenantId, (context) =>
            {
                context.EntityChangeSets.Count(f => f.TenantId == tenantId).ShouldBe(1);
                context.EntityChangeSets.Single().CreationTime.ShouldBeGreaterThan(justNow);
            });

            UsingDbContext(tenantId, (context) =>
            {
                context.EntityPropertyChanges.Count(f => f.TenantId == tenantId).ShouldBe(3);
            });
        }

        [Fact]
        public void Should_Write_History_For_Tracked_Entities_Update()
        {
            /* Blog has Audited attribute. */

            var newValue = "http://testblog1-changed.myblogs.com";
            var originalValue = UpdateBlogUrlAndGetOriginalValue(newValue);

            _entityHistoryStore.Received().Save(Arg.Is<EntityChangeSet>(
                s => s.EntityChanges.Count == 1 &&
                     s.EntityChanges[0].ChangeType == EntityChangeType.Updated &&
                     s.EntityChanges[0].EntityId == s.EntityChanges[0].EntityEntry.As<EntityEntry>().Entity.As<IEntity>().Id.ToJsonString(false, false) &&
                     s.EntityChanges[0].EntityTypeFullName == typeof(Blog).FullName &&
                     s.EntityChanges[0].PropertyChanges.Count == 1 &&
                     s.EntityChanges[0].PropertyChanges.FirstOrDefault().NewValue == newValue.ToJsonString(false, false) &&
                     s.EntityChanges[0].PropertyChanges.FirstOrDefault().OriginalValue == originalValue.ToJsonString(false, false) &&
                     s.EntityChanges[0].PropertyChanges.FirstOrDefault().PropertyName == nameof(Blog.Url) &&
                     s.EntityChanges[0].PropertyChanges.FirstOrDefault().PropertyTypeFullName == typeof(Blog).GetProperty(nameof(Blog.Url)).PropertyType.FullName
            ));
        }

        [Fact]
        public void Should_Write_History_For_Tracked_Entities_Update_Owned()
        {
            /* Blog has Audited attribute. */

            int blog1Id;
            var newValue = "blogger-2";
            string originalValue;

            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                var blog1 = _blogRepository.Single(b => b.Name == "test-blog-1");
                blog1Id = blog1.Id;

                originalValue = blog1.More.BloggerName;
                blog1.More.BloggerName = newValue;

                uow.Complete();
            }

            Predicate<EntityChangeSet> predicate = s =>
            {
                s.EntityChanges.Count.ShouldBe(1);

                var entityChange = s.EntityChanges[0];
                entityChange.ChangeType.ShouldBe(EntityChangeType.Updated);
                entityChange.EntityId.ShouldBe(blog1Id.ToJsonString());
                entityChange.EntityTypeFullName.ShouldBe(typeof(BlogEx).FullName);
                entityChange.PropertyChanges.Count.ShouldBe(1);

                var propertyChange = entityChange.PropertyChanges.First();
                propertyChange.NewValue.ShouldBe(newValue.ToJsonString());
                propertyChange.OriginalValue.ShouldBe(originalValue.ToJsonString());
                propertyChange.PropertyName.ShouldBe(nameof(BlogEx.BloggerName));
                propertyChange.PropertyTypeFullName.ShouldBe(typeof(BlogEx).GetProperty(nameof(BlogEx.BloggerName)).PropertyType.FullName);

                return true;
            };

            _entityHistoryStore.Received().Save(Arg.Is<EntityChangeSet>(s => predicate(s)));
        }

        [Fact]
        public void Should_Write_History_For_Tracked_Property_Foreign_Key()
        {
            /* Post.BlogId has Audited attribute. */

            var blogId = CreateBlogAndGetId();
            _entityHistoryStore.ClearReceivedCalls();

            Guid post1Id;

            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                var blog1 = _blogRepository.Single(b => b.Id == 1);
                var blog2 = _blogRepository.Single(b => b.Id == 2);
                var post1 = _postRepository.Single(b => b.Body == "test-post-1-body");
                post1Id = post1.Id;

                // Change foreign key by assigning navigation property
                post1.Blog = blog2;
                _postRepository.Update(post1);

                uow.Complete();
            }

            _entityHistoryStore.Received().Save(Arg.Is<EntityChangeSet>(
                s => s.EntityChanges.Count == 1 &&
                     s.EntityChanges[0].ChangeType == EntityChangeType.Updated &&
                     s.EntityChanges[0].EntityId == post1Id.ToJsonString(false, false) &&
                     s.EntityChanges[0].EntityTypeFullName == typeof(Post).FullName &&
                     s.EntityChanges[0].PropertyChanges.Count == 1 // Post.BlogId
            ));
        }

        [Fact]
        public void Should_Write_History_For_Tracked_Property_Foreign_Key_Shadow()
        {
            /* Comment has Audited attribute. */

            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                var comment1 = _commentRepository.Single(b => b.Content == "test-comment-1-content");
                var post2 = _postRepository.Single(b => b.Body == "test-post-2-body");

                // Change foreign key by assigning navigation property
                comment1.Post = post2;
                _commentRepository.Update(comment1);

                uow.Complete();
            }

            _entityHistoryStore.Received().Save(Arg.Is<EntityChangeSet>(
                s => s.EntityChanges.Count == 1 &&
                     s.EntityChanges[0].PropertyChanges.Count == 1 &&
                     s.EntityChanges[0].PropertyChanges.First().PropertyName == "PostId"
            ));
        }

        [Fact]
        public void Should_Write_History_But_Not_For_Property_If_Disabled_History_Tracking()
        {
            /* Blog.Name has DisableAuditing attribute. */

            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                var blog1 = _blogRepository.Single(b => b.Name == "test-blog-1");

                blog1.Name = null;
                _blogRepository.Update(blog1);

                uow.Complete();
            }

            _entityHistoryStore.Received().Save(Arg.Is<EntityChangeSet>(
                s => s.EntityChanges.Count == 1 &&
                     s.EntityChanges[0].ChangeType == EntityChangeType.Updated &&
                     s.EntityChanges[0].EntityId == s.EntityChanges[0].EntityEntry.As<EntityEntry>().Entity.As<IEntity>().Id.ToJsonString(false, false) &&
                     s.EntityChanges[0].EntityTypeFullName == typeof(Blog).FullName &&
                     s.EntityChanges[0].PropertyChanges.Count == 0
            ));
        }

        #endregion

        #region CASES DON'T WRITE HISTORY

        [Fact]
        public void Should_Not_Write_History_If_Disabled()
        {
            Resolve<IEntityHistoryConfiguration>().IsEnabled = false;

            /* Blog has Audited attribute. */

            var newValue = "http://testblog1-changed.myblogs.com";
            var originalValue = UpdateBlogUrlAndGetOriginalValue(newValue);

            _entityHistoryStore.DidNotReceive().SaveAsync(Arg.Any<EntityChangeSet>());
        }

        [Fact]
        public void Should_Not_Write_History_If_Property_Has_No_Audited_Attribute()
        {
            /* Post.Body does not have Audited attribute. */

            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                var post1 = _postRepository.Single(b => b.Body == "test-post-1-body");

                post1.Body = null;
                _postRepository.Update(post1);

                uow.Complete();
            }

            _entityHistoryStore.DidNotReceive().SaveAsync(Arg.Any<EntityChangeSet>());
        }

        #endregion

        private int CreateBlogAndGetId()
        {
            int blog2Id;

            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                var blog2 = new Blog("test-blog-2", "http://testblog2.myblogs.com", "blogger-2");

                blog2Id = _blogRepository.InsertAndGetId(blog2);

                uow.Complete();
            }

            return blog2Id;
        }

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
        internal static EntityPropertyChange FirstOrDefault(this IEnumerable<EntityPropertyChange> enumerable)
        {
            var enumerator = enumerable.GetEnumerator();
            enumerator.MoveNext();
            return enumerator.Current;
        }
    }

    #endregion
}
