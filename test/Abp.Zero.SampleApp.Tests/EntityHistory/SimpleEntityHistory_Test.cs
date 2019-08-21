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
using Abp.Zero.SampleApp.EntityHistory;
using Castle.MicroKernel.Registration;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.EntityHistory
{
    public class SimpleEntityHistory_Test : SampleAppTestBase
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

            Predicate<EntityChangeSet> predicate = s =>
            {
                s.EntityChanges.Count.ShouldBe(1);

                var entityChange = s.EntityChanges[0];
                entityChange.ChangeTime.ShouldBe(entityChange.EntityEntry.As<DbEntityEntry>().Entity.As<IHasCreationTime>().CreationTime);
                entityChange.ChangeType.ShouldBe(EntityChangeType.Created);
                entityChange.EntityId.ShouldBe(blog2Id.ToJsonString());
                entityChange.EntityTypeFullName.ShouldBe(typeof(Blog).FullName);
                entityChange.PropertyChanges.Count.ShouldBe(4);
                // TODO: 3 should be the correct value, Blog.Category is null for both new/original values
                // entityChange.PropertyChanges.Count.ShouldBe(3);

                var propertyChange1 = entityChange.PropertyChanges.Single(pc => pc.PropertyName == nameof(Blog.Url));
                propertyChange1.OriginalValue.ShouldBeNull();
                propertyChange1.NewValue.ShouldNotBeNull();

                var propertyChange2 = entityChange.PropertyChanges.Single(pc => pc.PropertyName == nameof(Blog.More));
                propertyChange2.OriginalValue.ShouldBeNull();
                propertyChange2.NewValue.ShouldNotBeNull();

                var propertyChange3 = entityChange.PropertyChanges.Single(pc => pc.PropertyName == nameof(Blog.CreationTime));
                propertyChange3.OriginalValue.ShouldBeNull();
                propertyChange3.NewValue.ShouldNotBeNull();

                // Check "who did this change"
                s.ImpersonatorTenantId.ShouldBe(AbpSession.ImpersonatorTenantId);
                s.ImpersonatorUserId.ShouldBe(AbpSession.ImpersonatorUserId);
                s.TenantId.ShouldBe(AbpSession.TenantId);
                s.UserId.ShouldBe(AbpSession.UserId);

                return true;
            };

            _entityHistoryStore.Received().SaveAsync(Arg.Is<EntityChangeSet>(s => predicate(s)));
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

            UsingDbContext((context) =>
            {
                context.EntityChanges.Count(e => e.TenantId == null).ShouldBe(0);
                context.EntityChangeSets.Count(e => e.TenantId == null).ShouldBe(0);
                context.EntityPropertyChanges.Count(e => e.TenantId == null).ShouldBe(0);
            });

            var justNow = Clock.Now;
            var blog2Id = CreateBlogAndGetId();

            UsingDbContext((context) =>
            {
                context.EntityChanges.Count(e => e.TenantId == null).ShouldBe(1);
                context.EntityChangeSets.Count(e => e.TenantId == null).ShouldBe(1);
                context.EntityChangeSets.Single().CreationTime.ShouldBeGreaterThan(justNow);
                context.EntityPropertyChanges.Count(e => e.TenantId == null).ShouldBe(4);
                // TODO: 3 should be the correct value, Blog.Category is null for both new/original values
                // context.EntityPropertyChanges.Count(e => e.TenantId == 1).ShouldBe(3);
            });
        }

        [Fact]
        public void Should_Write_History_For_Tracked_Entities_Update()
        {
            /* Blog has Audited attribute. */

            var newValue = "http://testblog1-changed.myblogs.com";
            var originalValue = UpdateBlogUrlAndGetOriginalValue(newValue);

            Predicate<EntityChangeSet> predicate = s =>
            {
                s.EntityChanges.Count.ShouldBe(1);

                var entityChange = s.EntityChanges[0];
                entityChange.ChangeType.ShouldBe(EntityChangeType.Updated);
                entityChange.EntityId.ShouldBe(entityChange.EntityEntry.As<DbEntityEntry>().Entity.As<IEntity>().Id.ToJsonString());
                entityChange.EntityTypeFullName.ShouldBe(typeof(Blog).FullName);
                entityChange.PropertyChanges.Count.ShouldBe(1);

                var propertyChange = entityChange.PropertyChanges.First();
                propertyChange.NewValue.ShouldBe(newValue.ToJsonString());
                propertyChange.OriginalValue.ShouldBe(originalValue.ToJsonString());
                propertyChange.PropertyName.ShouldBe(nameof(Blog.Url));
                propertyChange.PropertyTypeFullName.ShouldBe(typeof(Blog).GetProperty(nameof(Blog.Url)).PropertyType.FullName);

                return true;
            };

            _entityHistoryStore.Received().SaveAsync(Arg.Is<EntityChangeSet>(s => predicate(s)));
        }

        [Fact]
        public void Should_Write_History_For_Tracked_Entities_Update_Only_Modified_Properties()
        {
            var originalValue = "http://testblog2.myblogs.com";
            var newValue = "http://testblog2-changed.myblogs.com";

            WithUnitOfWork(() =>
            {
                var blog2 = _blogRepository.Single(b => b.Url == originalValue && b.Category == null);

                // Update only the Url of the Blog
                blog2.ChangeUrl(newValue);
                _blogRepository.Update(blog2);
            });

            Predicate<EntityChangeSet> predicate = s =>
            {
                s.EntityChanges.Count.ShouldBe(1);

                var entityChange = s.EntityChanges[0];
                entityChange.ChangeType.ShouldBe(EntityChangeType.Updated);
                entityChange.EntityId.ShouldBe(entityChange.EntityEntry.As<DbEntityEntry>().Entity.As<IEntity>().Id.ToJsonString());
                entityChange.EntityTypeFullName.ShouldBe(typeof(Blog).FullName);
                entityChange.PropertyChanges.Count.ShouldBe(1);

                var propertyChange = entityChange.PropertyChanges.First();
                propertyChange.PropertyName.ShouldBe(nameof(Blog.Url));
                propertyChange.NewValue.ShouldBe(newValue.ToJsonString());
                propertyChange.OriginalValue.ShouldBe(originalValue.ToJsonString());
                propertyChange.PropertyTypeFullName.ShouldBe(typeof(Blog).GetProperty(nameof(Blog.Url)).PropertyType.FullName);

                return true;
            };

            _entityHistoryStore.Received().SaveAsync(Arg.Is<EntityChangeSet>(s => predicate(s)));
        }

        [Fact]
        public void Should_Write_History_For_Tracked_Entities_Update_Complex()
        {
            /* Blog has Audited attribute. */

            int blog1Id = 0;
            var newValue = new BlogEx { BloggerName = "blogger-2" };
            BlogEx originalValue = null;

            WithUnitOfWork(() =>
            {
                var blog1 = _blogRepository.Single(b => b.More.BloggerName == "blogger-1");
                blog1Id = blog1.Id;

                originalValue = new BlogEx { BloggerName = blog1.More.BloggerName };
                blog1.More.BloggerName = newValue.BloggerName;
                _blogRepository.Update(blog1);
            });

            Predicate<EntityChangeSet> predicate = s =>
            {
                s.EntityChanges.Count.ShouldBe(1);

                var entityChange = s.EntityChanges[0];
                entityChange.ChangeType.ShouldBe(EntityChangeType.Updated);
                entityChange.EntityId.ShouldBe(blog1Id.ToJsonString());
                entityChange.EntityTypeFullName.ShouldBe(typeof(Blog).FullName);
                entityChange.PropertyChanges.Count.ShouldBe(1);

                var propertyChange = entityChange.PropertyChanges.First();
                propertyChange.NewValue.ShouldBe(newValue.ToJsonString());
                propertyChange.OriginalValue.ShouldBe(originalValue.ToJsonString());
                propertyChange.PropertyName.ShouldBe(nameof(Blog.More));
                propertyChange.PropertyTypeFullName.ShouldBe(typeof(Blog).GetProperty(nameof(Blog.More)).PropertyType.FullName);

                return true;
            };

            _entityHistoryStore.Received().SaveAsync(Arg.Is<EntityChangeSet>(s => predicate(s)));
        }

        [Fact]
        public void Should_Write_History_For_Tracked_Property_Foreign_Key()
        {
            /* Post.BlogId has Audited attribute. */

            var blogId = CreateBlogAndGetId();
            Guid post1Id = Guid.Empty;

            WithUnitOfWork(() =>
            {
                var blog2 = _blogRepository.Single(b => b.Id == 2);
                var post1 = _postRepository.Single(p => p.Body == "test-post-1-body");
                post1Id = post1.Id;

                // Change foreign key by assigning navigation property
                post1.Blog = blog2;
                _postRepository.Update(post1);
            });

            Predicate<EntityChangeSet> predicate = s =>
            {
                s.EntityChanges.Count.ShouldBe(1);

                var entityChange = s.EntityChanges[0];
                entityChange.ChangeType.ShouldBe(EntityChangeType.Updated);
                entityChange.EntityId.ShouldBe(post1Id.ToJsonString());
                entityChange.EntityTypeFullName.ShouldBe(typeof(Post).FullName);
                entityChange.PropertyChanges.Count.ShouldBe(1);

                var propertyChange = entityChange.PropertyChanges.First();
                propertyChange.NewValue.ShouldBe("2");
                propertyChange.OriginalValue.ShouldBe("1");
                propertyChange.PropertyName.ShouldBe(nameof(Post.BlogId));
                propertyChange.PropertyTypeFullName.ShouldBe(typeof(Post).GetProperty(nameof(Post.BlogId)).PropertyType.FullName);

                return true;
            };

            _entityHistoryStore.Received().SaveAsync(Arg.Is<EntityChangeSet>(s => predicate(s)));
        }

        [Fact]
        public void Should_Write_History_For_Tracked_Property_Foreign_Key_Collection()
        {
            WithUnitOfWork(() =>
            {
                var blog1 = _blogRepository.Single(b => b.Name == "test-blog-1");
                var post10 = new Post { Blog = blog1, Title = "test-post-10-title", Body = "test-post-10-body" };

                // Change navigation property by adding into collection
                blog1.Posts.Add(post10);
                _blogRepository.Update(blog1);
            });

            Predicate<EntityChangeSet> predicate = s =>
            {
                s.EntityChanges.Count.ShouldBe(2);

                var entityChangePost = s.EntityChanges[0];
                entityChangePost.ChangeType.ShouldBe(EntityChangeType.Created);
                entityChangePost.EntityTypeFullName.ShouldBe(typeof(Post).FullName);
                entityChangePost.PropertyChanges.Count.ShouldBe(1); // Post.BlogId

                /* Blog has Audited attribute. */

                var entityChangeBlog = s.EntityChanges[1];
                entityChangeBlog.ChangeType.ShouldBe(EntityChangeType.Updated);
                entityChangeBlog.EntityTypeFullName.ShouldBe(typeof(Blog).FullName);
                entityChangeBlog.PropertyChanges.Count.ShouldBe(0);

                return true;
            };

            _entityHistoryStore.Received().SaveAsync(Arg.Is<EntityChangeSet>(s => predicate(s)));
        }

        [Fact]
        public void Should_Write_History_For_Tracked_Property_Foreign_Key_Shadow()
        {
            /* Comment has Audited attribute. */

            var post1KeyValue = new Dictionary<string, object>();
            var post2KeyValue = new Dictionary<string, object>();

            WithUnitOfWork(() =>
            {
                var post2 = _postRepository.Single(p => p.Body == "test-post-2-body");
                post2KeyValue.Add("Id", post2.Id);

                var comment1 = _commentRepository.Single(c => c.Content == "test-comment-1-content");
                post1KeyValue.Add("Id", comment1.Post.Id);

                // Change foreign key by assigning navigation property
                comment1.Post = post2;
                _commentRepository.Update(comment1);
            });

            Predicate<EntityChangeSet> predicate = s =>
            {
                s.EntityChanges.Count.ShouldBe(1);

                var entityChange = s.EntityChanges[0];
                entityChange.PropertyChanges.Count.ShouldBe(1);

                var propertyChange = entityChange.PropertyChanges.First();
                propertyChange.NewValue.ShouldBe(post2KeyValue.ToJsonString());
                propertyChange.OriginalValue.ShouldBe(post1KeyValue.ToJsonString());
                propertyChange.PropertyName.ShouldBe(nameof(Comment.Post));
                propertyChange.PropertyTypeFullName.ShouldBe(typeof(Comment).GetProperty(nameof(Comment.Post)).PropertyType.FullName);

                return true;
            };

            _entityHistoryStore.Received().SaveAsync(Arg.Is<EntityChangeSet>(s => predicate(s)));
        }

        [Fact]
        public void Should_Write_History_But_Not_For_Property_If_Disabled_History_Tracking()
        {
            /* Blog.Name has DisableAuditing attribute. */

            WithUnitOfWork(() =>
            {
                var blog1 = _blogRepository.Single(b => b.Name == "test-blog-1");
                blog1.Name = null;
                _blogRepository.Update(blog1);
            });

            Predicate<EntityChangeSet> predicate = s =>
            {
                s.EntityChanges.Count.ShouldBe(1);

                var entityChange = s.EntityChanges[0];
                entityChange.ChangeType.ShouldBe(EntityChangeType.Updated);
                entityChange.EntityId.ShouldBe(entityChange.EntityEntry.As<DbEntityEntry>().Entity.As<IEntity>().Id.ToJsonString());
                entityChange.EntityTypeFullName.ShouldBe(typeof(Blog).FullName);
                entityChange.PropertyChanges.Count.ShouldBe(0);

                return true;
            };

            _entityHistoryStore.Received().SaveAsync(Arg.Is<EntityChangeSet>(s => predicate(s)));
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

            WithUnitOfWork(() =>
            {
                var post1 = _postRepository.Single(p => p.Body == "test-post-1-body");
                post1.Body = null;
                _postRepository.Update(post1);
            });

            _entityHistoryStore.DidNotReceive().SaveAsync(Arg.Any<EntityChangeSet>());
        }

        #endregion

        private int CreateBlogAndGetId()
        {
            int blog2Id = 0;

            WithUnitOfWork(() =>
            {
                var blog2 = new Blog("test-blog-2", "http://testblog2.myblogs.com", "blogger-2");
                blog2Id = _blogRepository.InsertAndGetId(blog2);
            });

            return blog2Id;
        }

        private string UpdateBlogUrlAndGetOriginalValue(string newValue)
        {
            string originalValue = null;

            WithUnitOfWork(() =>
            {
                var blog1 = _blogRepository.Single(b => b.Name == "test-blog-1");
                originalValue = blog1.Url;

                blog1.ChangeUrl(newValue);
                _blogRepository.Update(blog1);
            });

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
