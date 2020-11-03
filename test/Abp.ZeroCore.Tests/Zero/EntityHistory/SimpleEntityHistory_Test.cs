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
using Abp.Authorization.Roles;
using Xunit;

namespace Abp.Zero.EntityHistory
{
    public class SimpleEntityHistory_Test : AbpZeroTestBase
    {
        private readonly IRepository<Advertisement> _advertisementRepository;
        private readonly IRepository<Blog> _blogRepository;
        private readonly IRepository<Post, Guid> _postRepository;
        private readonly IRepository<Comment> _commentRepository;
        private readonly IRepository<Foo> _fooRepository;

        private IEntityHistoryStore _entityHistoryStore;

        public SimpleEntityHistory_Test()
        {
            _advertisementRepository = Resolve<IRepository<Advertisement>>();
            _blogRepository = Resolve<IRepository<Blog>>();
            _postRepository = Resolve<IRepository<Post, Guid>>();
            _commentRepository = Resolve<IRepository<Comment>>();
            _fooRepository = Resolve<IRepository<Foo>>();
            
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
                ((DateTime?)entityChange.ChangeTime).ShouldNotBe(null);
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
        public void Should_Write_History_For_Tracked_Entities_Update()
        {
            /* Advertisement does not have Audited attribute. */
            Resolve<IEntityHistoryConfiguration>().Selectors.Add("Selected", typeof(Advertisement));

            WithUnitOfWork(() =>
            {
                var advertisement1 = _advertisementRepository.Single(a => a.Banner == "test-advertisement-1");
                advertisement1.Banner = "test-advertisement-1-updated";
                _advertisementRepository.Update(advertisement1);
            });

            Predicate<EntityChangeSet> predicate = s =>
            {
                s.EntityChanges.Count.ShouldBe(1);

                var entityChange = s.EntityChanges.Single(ec => ec.EntityTypeFullName == typeof(Advertisement).FullName);
                entityChange.ChangeType.ShouldBe(EntityChangeType.Updated);
                entityChange.EntityId.ShouldBe(entityChange.EntityEntry.As<EntityEntry>().Entity.As<IEntity>().Id.ToJsonString());
                entityChange.PropertyChanges.Count.ShouldBe(1);

                var propertyChange = entityChange.PropertyChanges.Single(pc => pc.PropertyName == nameof(Advertisement.Banner));
                propertyChange.NewValue.ShouldBe("test-advertisement-1-updated".ToJsonString());
                propertyChange.OriginalValue.ShouldBe("test-advertisement-1".ToJsonString());
                propertyChange.PropertyTypeFullName.ShouldBe(typeof(Advertisement).GetProperty(nameof(Advertisement.Banner)).PropertyType.FullName);

                return true;
            };

            _entityHistoryStore.Received().Save(Arg.Is<EntityChangeSet>(s => predicate(s)));
        }

        [Fact]
        public void Should_Write_History_For_Audited_Entities_Create()
        {
            /* Blog has Audited attribute. */

            var blog2Id = CreateBlogAndGetId();

            Predicate<EntityChangeSet> predicate = s =>
            {
                s.EntityChanges.Count.ShouldBe(2);

                var entityChangeBlog = s.EntityChanges[0];
                entityChangeBlog.ChangeTime.ShouldBe(entityChangeBlog.EntityEntry.As<EntityEntry>().Entity.As<IHasCreationTime>().CreationTime);
                entityChangeBlog.ChangeType.ShouldBe(EntityChangeType.Created);
                entityChangeBlog.EntityId.ShouldBe(blog2Id.ToJsonString(false, false));
                entityChangeBlog.EntityTypeFullName.ShouldBe(typeof(Blog).FullName);
                entityChangeBlog.PropertyChanges.Count.ShouldBe(2);  // Blog.Name, Blog.Url

                var entityChangeBlogEx = s.EntityChanges[1];
                entityChangeBlogEx.ChangeType.ShouldBe(EntityChangeType.Created);
                entityChangeBlogEx.EntityId.ShouldBe(blog2Id.ToJsonString(false, false));
                entityChangeBlogEx.EntityTypeFullName.ShouldBe(typeof(BlogEx).FullName);
                entityChangeBlogEx.PropertyChanges.Count.ShouldBe(1); // BlogEx.BloggerName

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
        public void Should_Write_History_For_Audited_Entities_Update()
        {
            /* Blog has Audited attribute. */

            var newValue = "http://testblog1-changed.myblogs.com";
            var originalValue = UpdateBlogUrlAndGetOriginalValue(newValue);

            Predicate<EntityChangeSet> predicate = s =>
            {
                s.EntityChanges.Count.ShouldBe(1);

                var entityChangeBlog = s.EntityChanges.Single(ec => ec.EntityTypeFullName == typeof(Blog).FullName);
                entityChangeBlog.ChangeType.ShouldBe(EntityChangeType.Updated);
                entityChangeBlog.EntityId.ShouldBe(entityChangeBlog.EntityEntry.As<EntityEntry>().Entity.As<IEntity>().Id.ToJsonString(false, false));
                entityChangeBlog.PropertyChanges.Count.ShouldBe(1);

                var propertyChangeUrl = entityChangeBlog.PropertyChanges.Single(pc => pc.PropertyName == nameof(Blog.Url));
                propertyChangeUrl.NewValue.ShouldBe(newValue.ToJsonString(false, false));
                propertyChangeUrl.OriginalValue.ShouldBe(originalValue.ToJsonString(false, false));
                propertyChangeUrl.PropertyTypeFullName.ShouldBe(typeof(Blog).GetProperty(nameof(Blog.Url)).PropertyType.FullName);

                return true;
            };

            _entityHistoryStore.Received().Save(Arg.Is<EntityChangeSet>(s => predicate(s)));
        }

        [Fact]
        public void Should_Write_History_For_Owned_Entity_Of_Audited_Entities_Create()
        {
            // Blog is the owner of BlogEx and has Audited attribute.
            // Therefore, BlogEx should follow Blog and have entity history.

            //Arrange
            int blog2Id;
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                // Owned entities are not available via DbContext -> DbSet,
                var blog2 = _blogRepository.Single(b => b.Name == "test-blog-2");
                blog2Id = blog2.Id;
                blog2.More.ShouldBeNull();

                blog2.More = new BlogEx() { BloggerName = "blogger-2" };
                uow.Complete();
            };

            //Assert
            WithUnitOfWork(() =>
            {
                // Owned entities are not available via DbContext -> DbSet,
                var blog2 = _blogRepository.Single(b => b.Name == "test-blog-2");
                blog2.More.ShouldNotBeNull();
            });

            Predicate<EntityChangeSet> predicate = s =>
            {
                s.EntityChanges.Count.ShouldBe(1);

                var entityChange = s.EntityChanges.Single(ec => ec.EntityTypeFullName == typeof(BlogEx).FullName);
                entityChange.ChangeType.ShouldBe(EntityChangeType.Created);
                // The primary key of BlogEx is a shadow property,
                // EF Core is keeping the values of PK of Blog and PK of BlogEx the same
                // See https://docs.microsoft.com/en-us/ef/core/modeling/owned-entities#implicit-keys
                entityChange.EntityId.ShouldBe(blog2Id.ToJsonString(false, false));
                entityChange.PropertyChanges.Count.ShouldBe(1);

                var propertyChange = entityChange.PropertyChanges.Single(pc => pc.PropertyName == nameof(BlogEx.BloggerName));
                propertyChange.OriginalValue.ShouldBeNull();
                propertyChange.NewValue.ShouldBe("blogger-2".ToJsonString(false, false));
                propertyChange.PropertyTypeFullName.ShouldBe(typeof(BlogEx).GetProperty(nameof(BlogEx.BloggerName)).PropertyType.FullName);

                return true;
            };

            _entityHistoryStore.Received().Save(Arg.Is<EntityChangeSet>(s => predicate(s)));
        }

        [Fact]
        public void Should_Write_History_For_Owned_Entity_Of_Audited_Entities_Update()
        {
            // Blog is the owner of BlogEx and has Audited attribute.
            // Therefore, BlogEx should follow Blog and have entity history.

            //Arrange
            int blog1Id;
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                // Owned entities are not available via DbContext -> DbSet,
                var blog1 = _blogRepository.Single(b => b.Name == "test-blog-1");
                blog1Id = blog1.Id;
                blog1.More.ShouldNotBeNull();

                blog1.More.BloggerName = "blogger-1-updated";
                uow.Complete();
            };

            //Assert
            Predicate<EntityChangeSet> predicate = s =>
            {
                s.EntityChanges.Count.ShouldBe(1);

                var entityChange = s.EntityChanges.Single(ec => ec.EntityTypeFullName == typeof(BlogEx).FullName);
                entityChange.ChangeType.ShouldBe(EntityChangeType.Updated);
                // The primary key of BlogEx is a shadow property,
                // EF Core is keeping the values of PK of Blog and PK of BlogEx the same
                // See https://docs.microsoft.com/en-us/ef/core/modeling/owned-entities#implicit-keys
                entityChange.EntityId.ShouldBe(blog1Id.ToJsonString(false, false));
                entityChange.PropertyChanges.Count.ShouldBe(1);

                var propertyChange = entityChange.PropertyChanges.Single(pc => pc.PropertyName == nameof(BlogEx.BloggerName));
                propertyChange.OriginalValue.ShouldBe("blogger-1".ToJsonString(false, false));
                propertyChange.NewValue.ShouldBe("blogger-1-updated".ToJsonString(false, false));
                propertyChange.PropertyTypeFullName.ShouldBe(typeof(BlogEx).GetProperty(nameof(BlogEx.BloggerName)).PropertyType.FullName);

                return true;
            };

            _entityHistoryStore.Received().Save(Arg.Is<EntityChangeSet>(s => predicate(s)));
        }
        
        [Fact]
        public void Should_Write_History_For_Owned_Entities_Of_Audited_Entities_Create()
        {
            // Blog is the owner of BlogPromotion and has Audited attribute.
            // Advertisement is not the owner of BlogPromotion.
            // Therefore, BlogPromotion should follow Blog and have entity history.

            //Arrange
            int blog1Id;
            int advertisement1Id;
            int advertisement2Id;
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                // Owned entities are not available via DbContext -> DbSet,
                var blog1 = _blogRepository.Single(b => b.Name == "test-blog-1");
                blog1Id = blog1.Id;
                blog1.Promotions.Count.ShouldBe(0);

                var advertisement1 = _advertisementRepository.Single(a => a.Banner == "test-advertisement-1");
                advertisement1Id = advertisement1.Id;
                var advertisement2 = _advertisementRepository.Single(a => a.Banner == "test-advertisement-2");
                advertisement2Id = advertisement2.Id;

                blog1.Promotions.Add(new BlogPromotion { AdvertisementId = advertisement1.Id });
                blog1.Promotions.Add(new BlogPromotion { AdvertisementId = advertisement2.Id });
                uow.Complete();
            };

            //Assert
            WithUnitOfWork(() =>
            {
                // Owned entities are not available via DbContext -> DbSet,
                var blog1 = _blogRepository.Single(b => b.Name == "test-blog-1");
                blog1.Promotions.Count.ShouldBe(2);
            });

            Predicate<EntityChangeSet> predicate = s =>
            {
                s.EntityChanges.Count.ShouldBe(2);

                // The primary key (Id) of BlogPromotion is a shadow property and BlogId being the foreign key to its owner, Blog,
                // EF Core is keeping the values of PK of Blog and FK (BlogId) of BlogPromotion the same
                // PK of BlogPromotion is unique across different owners, e.g. Blog1 has BlogPromotion1, BlogPromotion2 and Blog2 has BlogPromotion3
                // See https://docs.microsoft.com/en-us/ef/core/modeling/owned-entities#collections-of-owned-types

                // We assume that are 2 BlogPromotion being created sequentially for Blog 1
                const int blog1Promotion1Id = 1;
                const int blog1Promotion2Id = 2;

                var entityChange1 = s.EntityChanges.Single(ec => 
                    ec.EntityTypeFullName == typeof(BlogPromotion).FullName
                    && ec.EntityId == blog1Promotion1Id.ToJsonString(false, false)
                );
                entityChange1.ChangeType.ShouldBe(EntityChangeType.Created);
                entityChange1.PropertyChanges.Count.ShouldBe(2);

                var propertyChange1 = entityChange1.PropertyChanges.Single(pc => pc.PropertyName == nameof(BlogPromotion.BlogId));
                propertyChange1.OriginalValue.ShouldBeNull();
                propertyChange1.NewValue.ShouldBe(blog1Id.ToJsonString(false, false));
                propertyChange1.PropertyTypeFullName.ShouldBe(typeof(BlogPromotion).GetProperty(nameof(BlogPromotion.BlogId)).PropertyType.FullName);

                var propertyChange2 = entityChange1.PropertyChanges.Single(pc => pc.PropertyName == nameof(BlogPromotion.AdvertisementId));
                propertyChange2.OriginalValue.ShouldBeNull();
                propertyChange2.NewValue.ShouldBe(advertisement1Id.ToJsonString(false, false));
                propertyChange2.PropertyTypeFullName.ShouldBe(typeof(BlogPromotion).GetProperty(nameof(BlogPromotion.AdvertisementId)).PropertyType.FullName);

                var entityChange2 = s.EntityChanges.Single(ec =>
                    ec.EntityTypeFullName == typeof(BlogPromotion).FullName
                    && ec.EntityId == blog1Promotion2Id.ToJsonString(false, false)
                );
                entityChange2.ChangeType.ShouldBe(EntityChangeType.Created);
                entityChange2.PropertyChanges.Count.ShouldBe(2);

                var propertyChange3 = entityChange2.PropertyChanges.Single(pc => pc.PropertyName == nameof(BlogPromotion.BlogId));
                propertyChange3.OriginalValue.ShouldBeNull();
                propertyChange3.NewValue.ShouldBe(blog1Id.ToJsonString(false, false));
                propertyChange3.PropertyTypeFullName.ShouldBe(typeof(BlogPromotion).GetProperty(nameof(BlogPromotion.BlogId)).PropertyType.FullName);

                var propertyChange4 = entityChange2.PropertyChanges.Single(pc => pc.PropertyName == nameof(BlogPromotion.AdvertisementId));
                propertyChange4.OriginalValue.ShouldBeNull();
                propertyChange4.NewValue.ShouldBe(advertisement2Id.ToJsonString(false, false));
                propertyChange4.PropertyTypeFullName.ShouldBe(typeof(BlogPromotion).GetProperty(nameof(BlogPromotion.AdvertisementId)).PropertyType.FullName);

                return true;
            };

            _entityHistoryStore.Received().Save(Arg.Is<EntityChangeSet>(s => predicate(s)));
        }

        [Fact]
        public void Should_Write_History_For_Owned_Entities_Of_Audited_Entities_Update()
        {
            // Blog is the owner of BlogPromotion and has Audited attribute.
            // Advertisement is not the owner of BlogPromotion.
            // Therefore, BlogPromotion should follow Blog and have entity history.

            //Arrange
            int advertisement2Id;
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                // Owned entities are not available via DbContext -> DbSet,
                var blog2 = _blogRepository.Single(b => b.Name == "test-blog-2");
                blog2.Promotions.Count.ShouldBe(0);

                var advertisement1 = _advertisementRepository.Single(a => a.Banner == "test-advertisement-1");
                blog2.Promotions.Add(new BlogPromotion { AdvertisementId = advertisement1.Id });

                uow.Complete();

                blog2.Promotions.Count.ShouldBe(1);
            };
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                // Owned entities are not available via DbContext -> DbSet,
                var blog1 = _blogRepository.Single(b => b.Name == "test-blog-1");
                blog1.Promotions.Count.ShouldBe(0);

                var advertisement2 = _advertisementRepository.Single(a => a.Banner == "test-advertisement-2");
                advertisement2Id = advertisement2.Id;

                blog1.Promotions.Add(new BlogPromotion { AdvertisementId = advertisement2.Id, Title = "test-promotion-2" });
                uow.Complete();

                blog1.Promotions.Count.ShouldBe(1);
            };
            _entityHistoryStore.ClearReceivedCalls();

            //Act
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                var blog1 = _blogRepository.Single(b => b.Name == "test-blog-1");

                var blog1Promotion2 = blog1.Promotions.Single(p => p.AdvertisementId == advertisement2Id);
                blog1Promotion2.Title = "test-promotion-2-updated";
                uow.Complete();
            };

            WithUnitOfWork(() =>
            {
                // Owned entities are not available via DbContext -> DbSet,
                var blog1 = _blogRepository.Single(b => b.Name == "test-blog-1");
                blog1.Promotions.Count.ShouldBe(1);
            });

            //Assert
            Predicate<EntityChangeSet> predicate = s =>
            {
                s.EntityChanges.Count.ShouldBe(1);

                // The primary key (Id) of BlogPromotion is a shadow property and BlogId being the foreign key to its owner, Blog,
                // EF Core is keeping the values of PK of Blog and FK (BlogId) of BlogPromotion the same
                // PK of BlogPromotion is unique across different owners, e.g. Blog1 has BlogPromotion1, BlogPromotion2 and Blog2 has BlogPromotion3
                // See https://docs.microsoft.com/en-us/ef/core/modeling/owned-entities#collections-of-owned-types

                // We assume that are 2 BlogPromotion (with Id 1, 2) being created sequentially for Blog 2 and Blog 1 respectively
                const int blog1Promotion2Id = 2;

                var entityChange1 = s.EntityChanges.Single(ec =>
                    ec.EntityTypeFullName == typeof(BlogPromotion).FullName
                    && ec.EntityId == blog1Promotion2Id.ToJsonString(false, false)
                );
                entityChange1.ChangeType.ShouldBe(EntityChangeType.Updated);
                entityChange1.PropertyChanges.Count.ShouldBe(1);

               var propertyChange1 = entityChange1.PropertyChanges.Single(pc => pc.PropertyName == nameof(BlogPromotion.Title));
                propertyChange1.OriginalValue.ShouldBe("test-promotion-2".ToJsonString(false, false));
                propertyChange1.NewValue.ShouldBe("test-promotion-2-updated".ToJsonString(false, false));
                propertyChange1.PropertyTypeFullName.ShouldBe(typeof(BlogPromotion).GetProperty(nameof(BlogPromotion.Title)).PropertyType.FullName);

                return true;
            };

            _entityHistoryStore.Received().Save(Arg.Is<EntityChangeSet>(s => predicate(s)));
        }

        [Fact]
        public void Should_Write_History_For_Audited_Property_Foreign_Key()
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

            Predicate<EntityChangeSet> predicate = s =>
            {
                s.EntityChanges.Count.ShouldBe(1);

                var entityChange = s.EntityChanges.Single(ec => ec.EntityTypeFullName == typeof(Post).FullName);
                entityChange.ChangeType.ShouldBe(EntityChangeType.Updated);
                entityChange.EntityId.ShouldBe(post1Id.ToJsonString(false, false));
                entityChange.PropertyChanges.Count.ShouldBe(1);

                var propertyChange = entityChange.PropertyChanges.Single();
                propertyChange.PropertyName.ShouldBe(nameof(Post.BlogId));

                return true;
            };

            _entityHistoryStore.Received().Save(Arg.Is<EntityChangeSet>(s => predicate(s)));
        }

        [Fact]
        public void Should_Write_History_For_Audited_Property_Foreign_Key_Shadow()
        {
            /* Comment has Audited attribute. */

            WithUnitOfWork(() =>
            {
                var comment1 = _commentRepository.Single(b => b.Content == "test-comment-1-content");
                var post2 = _postRepository.Single(b => b.Body == "test-post-2-body");

                // Change foreign key by assigning navigation property
                comment1.Post = post2;
                _commentRepository.Update(comment1);
            });

            Predicate<EntityChangeSet> predicate = s =>
            {
                s.EntityChanges.Count.ShouldBe(1);

                var entityChange = s.EntityChanges[0];
                entityChange.ChangeType.ShouldBe(EntityChangeType.Updated);
                entityChange.EntityTypeFullName.ShouldBe(typeof(Comment).FullName);
                entityChange.PropertyChanges.Count.ShouldBe(1);

                var propertyChange = entityChange.PropertyChanges.Single();
                propertyChange.PropertyName.ShouldBe("PostId");

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

                var entityChange = s.EntityChanges[0];
                entityChange.ChangeType.ShouldBe(EntityChangeType.Updated);
                entityChange.EntityId.ShouldBe(entityChange.EntityEntry.As<EntityEntry>().Entity.As<IEntity>().Id.ToJsonString(false, false));
                entityChange.EntityTypeFullName.ShouldBe(typeof(Blog).FullName);
                entityChange.PropertyChanges.Count.ShouldBe(0);

                return true;
            };

            _entityHistoryStore.Received().Save(Arg.Is<EntityChangeSet>(s => predicate(s)));
        }

        [Fact]
        public void Should_Not_Save_Empty_PropertyChanges()
        {
            // Arrange
            // Forward calls from substitute to implementation
            var entityHistoryStore = Resolve<EntityHistoryStore>();
            _entityHistoryStore.When(x => x.SaveAsync(Arg.Any<EntityChangeSet>()))
                .Do(callback => AsyncHelper.RunSync(() =>
                    entityHistoryStore.SaveAsync(callback.Arg<EntityChangeSet>()))
                );

            _entityHistoryStore.When(x => x.Save(Arg.Any<EntityChangeSet>()))
                .Do(callback => entityHistoryStore.Save(callback.Arg<EntityChangeSet>()));

            // Act
            Foo foo = null;
            WithUnitOfWork(() =>
            {
                foo = new Foo
                {
                    Audited = "s1"
                };
                
                _fooRepository.InsertAndGetId(foo);
            });

            UsingDbContext((context) =>
            {
                context.EntityChanges.Count(e => e.TenantId == 1).ShouldBe(1);
                context.EntityChangeSets.Count(e => e.TenantId == 1).ShouldBe(1);
                context.EntityPropertyChanges.Count(e => e.TenantId == 1).ShouldBe(1);
            });

            WithUnitOfWork(() =>
            {
                foo.NonAudited = "s2";
                _fooRepository.Update(foo);
            });

            // Assert
            UsingDbContext((context) =>
            {
                context.EntityChanges.Count(e => e.TenantId == 1).ShouldBe(1);
                context.EntityChangeSets.Count(e => e.TenantId == 1).ShouldBe(1);
                context.EntityPropertyChanges.Count(e => e.TenantId == 1).ShouldBe(1);
            });
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
        public void Should_Not_Write_History_If_Not_Audited_And_Not_Selected()
        {
            /* Advertisement does not have Audited attribute. */

            Resolve<IEntityHistoryConfiguration>().Selectors.Clear();

            WithUnitOfWork(() =>
            {
                _advertisementRepository.Insert(new Advertisement
                {
                    Banner = "not-selected-advertisement"
                });
            });

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
                /* Category does not inherit from Entity<> and is not an owned entity*/
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
                /* Category does not inherit from Entity<> and is not an owned entity*/
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

        [Fact]
        public void Should_Not_Write_History_For_Audited_Entity_By_Default()
        {
            //Arrange
            UsingDbContext((context) =>
            {
                context.Countries.Add(new Country { CountryCode = "My Country" });
                context.SaveChanges();
            });

            //Assert
            _entityHistoryStore.DidNotReceive().Save(Arg.Any<EntityChangeSet>());
        }

        [Fact]
        public void Should_Not_Write_History_For_Not_Audited_Entities_Shadow_Property()
        {
            // PermissionSetting has Discriminator column (shadow property) for RolePermissionSetting

            //Arrange
            UsingDbContext((context) =>
            {
                var role = context.Roles.FirstOrDefault();
                role.ShouldNotBeNull();

                context.RolePermissions.Add(new RolePermissionSetting()
                {
                    Name = "Test",
                    RoleId = role.Id
                });
                context.SaveChanges();
            });

            //Assert
            _entityHistoryStore.DidNotReceive().Save(Arg.Any<EntityChangeSet>());
        }

        [Fact]
        public void Should_Not_Write_History_For_Owned_Entities_Of_Non_Audited_Entities_Create()
        {
            // Advertisement is the owner of AdvertisementFeedback and does not have Audited attribute.
            // Comment is not the owner of AdvertisementFeedback.
            // Therefore, AdvertisementFeedback should follow Advertisement and not have entity history.

            WithUnitOfWork(() =>
            {
                var advertisement1 = _advertisementRepository.Single(a => a.Banner == "test-advertisement-1");
                var comment1 = _commentRepository.Single(b => b.Content == "test-comment-1-content");

                advertisement1.Feedbacks.Add(new AdvertisementFeedback { AdvertisementId = advertisement1.Id, CommentId = comment1.Id });
            });

            //Assert
            _entityHistoryStore.DidNotReceive().Save(Arg.Any<EntityChangeSet>());
        }

        #endregion

        private int CreateBlogAndGetId()
        {
            var blog2 = new Blog("test-blog-2", "http://testblog2.myblogs.com", "blogger-2");
            var blog2Id = _blogRepository.InsertAndGetId(blog2);
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
