using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Repositories;
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
        private readonly IRepository<Advertisement> _advertisementRepository;
        private readonly IRepository<Blog> _blogRepository;
        private readonly IRepository<Post, Guid> _postRepository;
        private readonly IRepository<Comment> _commentRepository;

        private IEntityHistoryStore _entityHistoryStore;

        public SimpleEntityHistory_Test()
        {
            _advertisementRepository = Resolve<IRepository<Advertisement>>();
            _blogRepository = Resolve<IRepository<Blog>>();
            _postRepository = Resolve<IRepository<Post, Guid>>();
            _commentRepository = Resolve<IRepository<Comment>>();

            var user = GetDefaultTenantAdmin();
            AbpSession.TenantId = user.TenantId;
            AbpSession.UserId = user.Id;

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
            /* Advertisement does not have Audited attribute. */
            Resolve<IEntityHistoryConfiguration>().Selectors.Add("Selected", typeof(Advertisement));

            int? advertisementId = null;
            WithUnitOfWork(() =>
            {
                var advertisement = new Advertisement { Banner = "tracked-advertisement" };
                advertisementId = _advertisementRepository.InsertAndGetId(advertisement);
            });

            Predicate<EntityChangeSet> predicate = s =>
            {
                s.EntityChanges.Count.ShouldBe(1);

                var entityChange = s.EntityChanges.Single(ec => ec.EntityTypeFullName == typeof(Advertisement).FullName);
                entityChange.ChangeTime.ShouldNotBeNull();
                entityChange.ChangeType.ShouldBe(EntityChangeType.Created);
                entityChange.EntityId.ShouldBe(advertisementId.ToJsonString());
                entityChange.PropertyChanges.Count.ShouldBe(1);

                var propertyChange1 = entityChange.PropertyChanges.Single(pc => pc.PropertyName == nameof(Advertisement.Banner));
                propertyChange1.OriginalValue.ShouldBeNull();
                propertyChange1.NewValue.ShouldNotBeNull();

                // Check "who did this change"
                s.ImpersonatorTenantId.ShouldBe(AbpSession.ImpersonatorTenantId);
                s.ImpersonatorUserId.ShouldBe(AbpSession.ImpersonatorUserId);
                s.TenantId.ShouldBe(AbpSession.TenantId);
                s.UserId.ShouldBe(AbpSession.UserId);

                return true;
            };

            _entityHistoryStore.Received().Save(Arg.Is<EntityChangeSet>(s => predicate(s)));
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

            UsingDbContext((context) =>
            {
                context.EntityChanges.Count(e => e.TenantId == 1).ShouldBe(0);
                context.EntityChangeSets.Count(e => e.TenantId == 1).ShouldBe(0);
                context.EntityPropertyChanges.Count(e => e.TenantId == 1).ShouldBe(0);
            });

            /* Advertisement does not have Audited attribute. */
            Resolve<IEntityHistoryConfiguration>().Selectors.Add("Selected", typeof(Advertisement));

            var justNow = Clock.Now;
            WithUnitOfWork(() =>
            {
                _advertisementRepository.InsertAndGetId(new Advertisement { Banner = "tracked-advertisement" });
            });

            UsingDbContext((context) =>
            {
                context.EntityChanges.Count(e => e.TenantId == 1).ShouldBe(1);
                context.EntityChangeSets.Count(e => e.TenantId == 1).ShouldBe(1);
                context.EntityChangeSets.Single().CreationTime.ShouldBeGreaterThan(justNow);
                context.EntityPropertyChanges.Count(e => e.TenantId == 1).ShouldBe(1);
            });
        }

        [Fact]
        public void Should_Write_History_For_Audited_Entities_Create()
        {
            /* Blog has Audited attribute. */

            var blog2Id = CreateBlogAndGetId();

            Predicate<EntityChangeSet> predicate = s =>
            {
                s.EntityChanges.Count.ShouldBe(1);

                var entityChange = s.EntityChanges.Single(ec => ec.EntityTypeFullName == typeof(Blog).FullName);
                entityChange.ChangeTime.ShouldBe(entityChange.EntityEntry.As<DbEntityEntry>().Entity.As<IHasCreationTime>().CreationTime);
                entityChange.ChangeType.ShouldBe(EntityChangeType.Created);
                entityChange.EntityId.ShouldBe(blog2Id.ToJsonString());
                entityChange.PropertyChanges.Count.ShouldBe(3);

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

            _entityHistoryStore.Received().Save(Arg.Is<EntityChangeSet>(s => predicate(s)));
        }

        [Fact]
        public void Should_Write_History_For_Audited_Entities_Create_To_Database()
        {
            // Forward calls from substitute to implementation
            var entityHistoryStore = Resolve<EntityHistoryStore>();
            _entityHistoryStore.When(x => x.SaveAsync(Arg.Any<EntityChangeSet>()))
                .Do(callback => AsyncHelper.RunSync(() =>
                    entityHistoryStore.SaveAsync(callback.Arg<EntityChangeSet>()))
                );
            _entityHistoryStore.When(x => x.Save(Arg.Any<EntityChangeSet>()))
                .Do(callback => entityHistoryStore.Save(callback.Arg<EntityChangeSet>()));

            UsingDbContext((context) =>
            {
                context.EntityChanges.Count(e => e.TenantId == 1).ShouldBe(0);
                context.EntityChangeSets.Count(e => e.TenantId == 1).ShouldBe(0);
                context.EntityPropertyChanges.Count(e => e.TenantId == 1).ShouldBe(0);
            });

            var justNow = Clock.Now;
            var blog2Id = CreateBlogAndGetId();

            UsingDbContext((context) =>
            {
                context.EntityChanges.Count(e => e.TenantId == 1).ShouldBe(1);
                context.EntityChangeSets.Count(e => e.TenantId == 1).ShouldBe(1);
                context.EntityChangeSets.Single().CreationTime.ShouldBeGreaterThan(justNow);
                context.EntityPropertyChanges.Count(e => e.TenantId == 1).ShouldBe(3);
            });
        }

        [Fact]
        public void Should_Write_History_For_Audited_Entities_Update()
        {
            /* Blog has Audited attribute. */

            var newValue = "http://testblog1-changed.myblogs.com";
            var originalValue = UpdateBlogUrlAndGetOriginalValue(newValue);

            Predicate<EntityChangeSet> predicate = s =>
            {
                s.EntityChanges.Count.ShouldBe(1);

                var entityChange = s.EntityChanges.Single(ec => ec.EntityTypeFullName == typeof(Blog).FullName);
                entityChange.ChangeType.ShouldBe(EntityChangeType.Updated);
                entityChange.EntityId.ShouldBe(entityChange.EntityEntry.As<DbEntityEntry>().Entity.As<IEntity>().Id.ToJsonString());
                entityChange.PropertyChanges.Count.ShouldBe(1);

                var propertyChange = entityChange.PropertyChanges.Single(pc => pc.PropertyName == nameof(Blog.Url));
                propertyChange.NewValue.ShouldBe(newValue.ToJsonString());
                propertyChange.OriginalValue.ShouldBe(originalValue.ToJsonString());
                propertyChange.PropertyTypeFullName.ShouldBe(typeof(Blog).GetProperty(nameof(Blog.Url)).PropertyType.FullName);

                return true;
            };

            _entityHistoryStore.Received().Save(Arg.Is<EntityChangeSet>(s => predicate(s)));
        }

        [Fact]
        public void Should_Write_History_For_Audited_Entities_Update_Only_Modified_Properties()
        {
            var originalValue = "http://testblog2.myblogs.com";
            var newValue = "http://testblog2-changed.myblogs.com";

            WithUnitOfWork(() =>
            {
                var blog2 = _blogRepository.Single(b => b.Url == originalValue);

                // Update only the Url of the Blog
                blog2.ChangeUrl(newValue);
                _blogRepository.Update(blog2);
            });

            Predicate<EntityChangeSet> predicate = s =>
            {
                s.EntityChanges.Count.ShouldBe(1);

                var entityChange = s.EntityChanges.Single(ec => ec.EntityTypeFullName == typeof(Blog).FullName);
                entityChange.ChangeType.ShouldBe(EntityChangeType.Updated);
                entityChange.EntityId.ShouldBe(entityChange.EntityEntry.As<DbEntityEntry>().Entity.As<IEntity>().Id.ToJsonString());
                entityChange.PropertyChanges.Count.ShouldBe(1);

                var propertyChange = entityChange.PropertyChanges.Single(pc => pc.PropertyName == nameof(Blog.Url));
                propertyChange.NewValue.ShouldBe(newValue.ToJsonString());
                propertyChange.OriginalValue.ShouldBe(originalValue.ToJsonString());
                propertyChange.PropertyTypeFullName.ShouldBe(typeof(Blog).GetProperty(nameof(Blog.Url)).PropertyType.FullName);

                return true;
            };

            _entityHistoryStore.Received().Save(Arg.Is<EntityChangeSet>(s => predicate(s)));
        }

        [Fact]
        public void Should_Write_History_For_Audited_Entities_Update_Complex()
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

                var entityChange = s.EntityChanges.Single(ec => ec.EntityTypeFullName == typeof(Blog).FullName);
                entityChange.ChangeType.ShouldBe(EntityChangeType.Updated);
                entityChange.EntityId.ShouldBe(blog1Id.ToJsonString());
                entityChange.PropertyChanges.Count.ShouldBe(1);

                var propertyChange = entityChange.PropertyChanges.Single(pc => pc.PropertyName == nameof(Blog.More));
                propertyChange.NewValue.ShouldBe(newValue.ToJsonString());
                propertyChange.OriginalValue.ShouldBe(originalValue.ToJsonString());
                propertyChange.PropertyTypeFullName.ShouldBe(typeof(Blog).GetProperty(nameof(Blog.More)).PropertyType.FullName);

                return true;
            };

            _entityHistoryStore.Received().Save(Arg.Is<EntityChangeSet>(s => predicate(s)));
        }

        [Fact]
        public void Should_Write_History_For_Audited_Property_Foreign_Key()
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

                var entityChange = s.EntityChanges.Single(ec => ec.EntityTypeFullName == typeof(Post).FullName);
                entityChange.ChangeType.ShouldBe(EntityChangeType.Updated);
                entityChange.EntityId.ShouldBe(post1Id.ToJsonString());
                entityChange.PropertyChanges.Count.ShouldBe(1);

                var propertyChange = entityChange.PropertyChanges.Single(pc => pc.PropertyName == nameof(Post.BlogId));
                propertyChange.NewValue.ShouldBe("2");
                propertyChange.OriginalValue.ShouldBe("1");
                propertyChange.PropertyTypeFullName.ShouldBe(typeof(Post).GetProperty(nameof(Post.BlogId)).PropertyType.FullName);

                return true;
            };

            _entityHistoryStore.Received().Save(Arg.Is<EntityChangeSet>(s => predicate(s)));
        }

        [Fact]
        public void Should_Write_History_For_Audited_Property_Foreign_Key_Collection()
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

                var entityChangePost = s.EntityChanges.Single(ec => ec.EntityTypeFullName == typeof(Post).FullName);
                entityChangePost.ChangeType.ShouldBe(EntityChangeType.Created);
                entityChangePost.PropertyChanges.Count.ShouldBe(5);

                var propertyChange1 = entityChangePost.PropertyChanges.Single(pc => pc.PropertyName == nameof(Post.BlogId));
                propertyChange1.OriginalValue.ShouldBeNull();
                propertyChange1.NewValue.ShouldNotBeNull();

                var propertyChange2 = entityChangePost.PropertyChanges.Single(pc => pc.PropertyName == nameof(Post.Title));
                propertyChange2.OriginalValue.ShouldBeNull();
                propertyChange2.NewValue.ShouldNotBeNull();

                var propertyChange3 = entityChangePost.PropertyChanges.Single(pc => pc.PropertyName == nameof(Post.Body));
                propertyChange3.OriginalValue.ShouldBeNull();
                propertyChange3.NewValue.ShouldNotBeNull();

                var propertyChange4 = entityChangePost.PropertyChanges.Single(pc => pc.PropertyName == nameof(Post.IsDeleted));
                propertyChange4.OriginalValue.ShouldBeNull();
                propertyChange4.NewValue.ShouldNotBeNull();

                var propertyChange5 = entityChangePost.PropertyChanges.Single(pc => pc.PropertyName == nameof(Post.CreationTime));
                propertyChange5.OriginalValue.ShouldBeNull();
                propertyChange5.NewValue.ShouldNotBeNull();

                /* Blog has Audited attribute. */
                var entityChangeBlog = s.EntityChanges.Single(ec => ec.EntityTypeFullName == typeof(Blog).FullName);
                entityChangeBlog.ChangeType.ShouldBe(EntityChangeType.Updated);
                entityChangeBlog.PropertyChanges.Count.ShouldBe(0);

                return true;
            };

            _entityHistoryStore.Received().Save(Arg.Is<EntityChangeSet>(s => predicate(s)));
        }

        [Fact]
        public void Should_Write_History_For_Audited_Property_Foreign_Key_Shadow()
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

                var entityChange = s.EntityChanges.Single(ec => ec.EntityTypeFullName == typeof(Comment).FullName);
                entityChange.PropertyChanges.Count.ShouldBe(1);

                var propertyChange = entityChange.PropertyChanges.Single(pc => pc.PropertyName == nameof(Comment.Post));
                propertyChange.NewValue.ShouldBe(post2KeyValue.ToJsonString());
                propertyChange.OriginalValue.ShouldBe(post1KeyValue.ToJsonString());
                propertyChange.PropertyTypeFullName.ShouldBe(typeof(Comment).GetProperty(nameof(Comment.Post)).PropertyType.FullName);

                return true;
            };

            _entityHistoryStore.Received().Save(Arg.Is<EntityChangeSet>(s => predicate(s)));
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

                var entityChange = s.EntityChanges.Single(ec => ec.EntityTypeFullName == typeof(Blog).FullName);
                entityChange.ChangeType.ShouldBe(EntityChangeType.Updated);
                entityChange.EntityId.ShouldBe(entityChange.EntityEntry.As<DbEntityEntry>().Entity.As<IEntity>().Id.ToJsonString());
                entityChange.PropertyChanges.Count.ShouldBe(0);

                return true;
            };

            _entityHistoryStore.Received().Save(Arg.Is<EntityChangeSet>(s => predicate(s)));
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

            _entityHistoryStore.DidNotReceive().Save(Arg.Any<EntityChangeSet>());
        }

        [Fact]
        public void Should_Not_Write_History_If_Ignored()
        {
            Resolve<IEntityHistoryConfiguration>().IgnoredTypes.Add(typeof(Blog));

            /* Blog has Audited attribute. */

            var newValue = "http://testblog1-changed.myblogs.com";
            var originalValue = UpdateBlogUrlAndGetOriginalValue(newValue);

            _entityHistoryStore.DidNotReceive().Save(Arg.Any<EntityChangeSet>());
        }

        [Fact]
        public void Should_Not_Write_History_If_Selected_But_Ignored()
        {
            Resolve<IEntityHistoryConfiguration>().Selectors.Add("Selected", typeof(Blog));
            Resolve<IEntityHistoryConfiguration>().IgnoredTypes.Add(typeof(Blog));

            /* Blog has Audited attribute. */

            var newValue = "http://testblog1-changed.myblogs.com";
            var originalValue = UpdateBlogUrlAndGetOriginalValue(newValue);

            _entityHistoryStore.DidNotReceive().Save(Arg.Any<EntityChangeSet>());
        }

        [Fact]
        public void Should_Not_Write_History_If_Property_Has_No_Audited_Attribute()
        {
            /* Advertisement.Banner does not have Audited attribute. */

            WithUnitOfWork(() =>
            {
                var advertisement1 = _advertisementRepository.Single(a => a.Banner == "test-advertisement-1");
                advertisement1.Banner = null;
                _advertisementRepository.Update(advertisement1);
            });

            _entityHistoryStore.DidNotReceive().Save(Arg.Any<EntityChangeSet>());
        }

        [Fact]
        public void Should_Not_Write_History_If_Invalid_Entity_Has_Property_With_Audited_Attribute_Created()
        {
            //Act
            UsingDbContext((context) =>
            {
                context.Categories.Add(new Category { DisplayName = "My Category" });
                context.SaveChanges();
            });

            //Assert
            _entityHistoryStore.DidNotReceive().Save(Arg.Any<EntityChangeSet>());
        }

        [Fact]
        public void Should_Not_Write_History_If_Invalid_Entity_Has_Property_With_Audited_Attribute_Updated()
        {
            //Arrange
            UsingDbContext((context) =>
            {
                context.Categories.Add(new Category { DisplayName = "My Category" });
                context.SaveChanges();
            });
            _entityHistoryStore.ClearReceivedCalls();

            //Act
            UsingDbContext((context) =>
            {
                var category = context.Categories.Single(c => c.DisplayName == "My Category");
                category.DisplayName = "Invalid Category";
                context.SaveChanges();
            });

            //Assert
            _entityHistoryStore.DidNotReceive().Save(Arg.Any<EntityChangeSet>());
        }

        [Fact]
        public void Should_Not_Write_History_If_Invalid_Entity_Has_Property_With_Audited_Attribute_Deleted()
        {
            //Arrange
            UsingDbContext((context) =>
            {
                context.Categories.Add(new Category { DisplayName = "My Category" });
                context.SaveChanges();
            });
            _entityHistoryStore.ClearReceivedCalls();

            //Act
            UsingDbContext((context) =>
            {
                var category = context.Categories.Single(c => c.DisplayName == "My Category");
                context.Categories.Remove(category);
                context.SaveChanges();
            });

            //Assert
            _entityHistoryStore.DidNotReceive().Save(Arg.Any<EntityChangeSet>());
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
