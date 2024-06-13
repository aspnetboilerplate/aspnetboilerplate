using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Abp.Domain.Repositories;
using Abp.EntityHistory;
using Abp.Events.Bus.Entities;
using Abp.Extensions;
using Abp.Json;
using Abp.Timing;
using Abp.Zero.SampleApp.EntityHistory;
using Abp.Zero.SampleApp.NHibernate;
using Abp.Zero.SampleApp.TPH;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.EntityHistory
{
    public class SimpleEntityHistory_Test : NHibernateTestBase
    {
        private readonly IRepository<Advertisement> _advertisementRepository;
        private readonly IRepository<Blog> _blogRepository;
        private readonly IRepository<Post, Guid> _postRepository;
        private readonly IRepository<Comment> _commentRepository;
        private readonly IRepository<Student> _studentRepository;
        private readonly IRepository<Foo> _fooRepository;
        private readonly IRepository<Employee> _employeeRepository;

        private readonly IEntityHistoryStore _entityHistoryStore;

        public SimpleEntityHistory_Test()
        {
            _advertisementRepository = Resolve<IRepository<Advertisement>>();
            _blogRepository = Resolve<IRepository<Blog>>();
            _postRepository = Resolve<IRepository<Post, Guid>>();
            _commentRepository = Resolve<IRepository<Comment>>();
            _studentRepository = Resolve<IRepository<Student>>();
            _fooRepository = Resolve<IRepository<Foo>>();
            _employeeRepository = Resolve<IRepository<Employee>>();
            _entityHistoryStore = Resolve<IEntityHistoryStore>();

            var user = GetDefaultTenantAdmin();
            AbpSession.TenantId = user.TenantId;
            AbpSession.UserId = user.Id;

            Resolve<IEntityHistoryConfiguration>().IsEnabledForAnonymousUsers = true;
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();
            // _entityHistoryStore = Substitute.For<IEntityHistoryStore>();
            // LocalIocManager.IocContainer.Register(
            //     Component.For<IEntityHistoryStore>().Instance(_entityHistoryStore).LifestyleSingleton()
            // );
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
                var advertisement = new Advertisement {Banner = "tracked-advertisement"};
                advertisementId = _advertisementRepository.InsertAndGetId(advertisement);
            });

            UsingSession(session =>
            {
                session.Query<EntityChange>().Count().ShouldBe(1);

                var entityChange =
                    session.Query<EntityChange>().Single(ec => ec.EntityTypeFullName == typeof(Advertisement).FullName);
                ((DateTime?) entityChange.ChangeTime).ShouldNotBe(null);
                entityChange.ChangeType.ShouldBe(EntityChangeType.Created);
                entityChange.EntityId.ShouldBe(advertisementId.ToJsonString());

                var propertyChanges = session.Query<EntityPropertyChange>()
                    .Where(pc => pc.EntityChangeId == entityChange.Id).ToList();
                propertyChanges.Count.ShouldBe(1);

                var propertyChange1 =
                    propertyChanges.Single(pc => pc.PropertyName == nameof(Advertisement.Banner));
                propertyChange1.OriginalValue.ShouldBeNull();
                propertyChange1.NewValue.ShouldNotBeNull();

                // Check "who did this change"
                var changeSet = session.Query<EntityChangeSet>().Single();
                changeSet.ImpersonatorTenantId.ShouldBe(AbpSession.ImpersonatorTenantId);
                changeSet.ImpersonatorUserId.ShouldBe(AbpSession.ImpersonatorUserId);
                changeSet.TenantId.ShouldBe(AbpSession.TenantId);
                changeSet.UserId.ShouldBe(AbpSession.UserId);
            });
        }

        [Fact]
        public void Should_Write_History_For_Tracked_Entities_Create_To_Database()
        {
            UsingSession((session) =>
            {
                session.Query<EntityChange>().Count(e => e.TenantId == 1).ShouldBe(0);
                session.Query<EntityChangeSet>().Count(e => e.TenantId == 1).ShouldBe(0);
                session.Query<EntityPropertyChange>().Count(e => e.TenantId == 1).ShouldBe(0);
            });

            /* Advertisement does not have Audited attribute. */
            Resolve<IEntityHistoryConfiguration>().Selectors.Add("Selected", typeof(Advertisement));

            var justNow = Clock.Now;
            Thread.Sleep(1);

            WithUnitOfWork(() =>
            {
                _advertisementRepository.InsertAndGetId(new Advertisement {Banner = "tracked-advertisement"});
            });

            UsingSession((session) =>
            {
                session.Query<EntityChange>().Count(e => e.TenantId == 1).ShouldBe(1);
                session.Query<EntityChangeSet>().Count(e => e.TenantId == 1).ShouldBe(1);
                session.Query<EntityChangeSet>().Single().CreationTime.ShouldBeGreaterThan(justNow);
                session.Query<EntityPropertyChange>().Count(e => e.TenantId == 1).ShouldBe(1);
            });
        }

        [Fact]
        public void Should_Write_History_For_TPH_Tracked_Entities_Create()
        {
            Resolve<IEntityHistoryConfiguration>().Selectors.Add("Selected", typeof(Student));

            var student = new Student
            {
                Name = "TestName",
                IdCard = "TestIdCard",
                Address = "TestAddress",
                Grade = 1
            };

            _studentRepository.Insert(student);

            UsingSession(session =>
            {
                session.Query<EntityChange>().Count().ShouldBe(1);

                var entityChange = session.Query<EntityChange>()
                    .Single(ec => ec.EntityTypeFullName == typeof(Student).FullName);
                ((DateTime?) entityChange.ChangeTime).ShouldNotBe(null);
                entityChange.ChangeType.ShouldBe(EntityChangeType.Created);
                entityChange.EntityId.ShouldBe(student.Id.ToJsonString());

                var propertyChanges = session.Query<EntityPropertyChange>()
                    .Where(pc => pc.EntityChangeId == entityChange.Id).ToList();
                propertyChanges.Count().ShouldBe(4); //Name,IdCard,Address,Grade

                var propertyChange1 =
                    propertyChanges.Single(pc => pc.PropertyName == nameof(Student.Name));
                propertyChange1.OriginalValue.ShouldBeNull();
                propertyChange1.NewValue.ShouldNotBeNull();

                var propertyChange2 =
                    propertyChanges.Single(pc => pc.PropertyName == nameof(Student.IdCard));
                propertyChange2.OriginalValue.ShouldBeNull();
                propertyChange2.NewValue.ShouldNotBeNull();

                var propertyChange3 =
                    propertyChanges.Single(pc => pc.PropertyName == nameof(Student.Address));
                propertyChange3.OriginalValue.ShouldBeNull();
                propertyChange3.NewValue.ShouldNotBeNull();

                var propertyChange4 =
                    propertyChanges.Single(pc => pc.PropertyName == nameof(Student.Grade));
                propertyChange4.OriginalValue.ShouldBeNull();
                propertyChange4.NewValue.ShouldNotBeNull();

                // Check "who did this change"
                var changeSet = session.Query<EntityChangeSet>().Single();
                changeSet.ImpersonatorTenantId.ShouldBe(AbpSession.ImpersonatorTenantId);
                changeSet.ImpersonatorUserId.ShouldBe(AbpSession.ImpersonatorUserId);
                changeSet.TenantId.ShouldBe(AbpSession.TenantId);
                changeSet.UserId.ShouldBe(AbpSession.UserId);
            });
        }

        [Fact]
        public void Should_Write_History_For_TPH_Tracked_Entities_Create_To_Database()
        {
            UsingSession((session) =>
            {
                session.Query<EntityChange>().Count(e => e.TenantId == 1).ShouldBe(0);
                session.Query<EntityChangeSet>().Count(e => e.TenantId == 1).ShouldBe(0);
                session.Query<EntityPropertyChange>().Count(e => e.TenantId == 1).ShouldBe(0);
            });

            Resolve<IEntityHistoryConfiguration>().Selectors.Add("Selected", typeof(Student));

            var justNow = Clock.Now;
            Thread.Sleep(1);

            var student = new Student()
            {
                Name = "TestName",
                IdCard = "TestIdCard",
                Address = "TestAddress",
                Grade = 1
            };

            _studentRepository.Insert(student);

            UsingSession((session) =>
            {
                session.Query<EntityChange>().Count(e => e.TenantId == 1).ShouldBe(1);
                session.Query<EntityChangeSet>().Count(e => e.TenantId == 1).ShouldBe(1);
                session.Query<EntityChangeSet>().Single().CreationTime.ShouldBeGreaterThan(justNow);
                session.Query<EntityPropertyChange>().Count(e => e.TenantId == 1)
                    .ShouldBe(4); // Name,IdCard,Address,Grade
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

            UsingSession(session =>
            {
                session.Query<EntityChange>().Count().ShouldBe(1);

                var entityChange = session.Query<EntityChange>().Single(
                    ec => ec.EntityTypeFullName == typeof(Advertisement).FullName
                );
                entityChange.ChangeType.ShouldBe(EntityChangeType.Updated);
                
                var propertyChanges = session.Query<EntityPropertyChange>()
                    .Where(pc => pc.EntityChangeId == entityChange.Id).ToList();
                propertyChanges.Count.ShouldBe(1);

                var propertyChange = propertyChanges.Single(
                    pc => pc.PropertyName == nameof(Advertisement.Banner)
                );
                propertyChange.NewValue.ShouldBe("test-advertisement-1-updated".ToJsonString());
                propertyChange.OriginalValue.ShouldBe("test-advertisement-1".ToJsonString());
                propertyChange.PropertyTypeFullName.ShouldBe(typeof(Advertisement)
                    .GetProperty(nameof(Advertisement.Banner)).PropertyType.FullName);
            });
        }

        [Fact]
        public void Should_Write_History_For_Audited_Entities_Create()
        {
            /* Blog has Audited attribute. */

            var blog2Id = CreateBlogAndGetId();


            UsingSession(session =>
            {
                var entityChanges = session.Query<EntityChange>().ToList();
                entityChanges.Count.ShouldBe(1);

                var entityChange = entityChanges.Single(ec => ec.EntityTypeFullName == typeof(Blog).FullName);
                entityChange.ChangeType.ShouldBe(EntityChangeType.Created);
                entityChange.EntityId.ShouldBe(blog2Id.ToJsonString());

                var propertyChanges = session.Query<EntityPropertyChange>()
                    .Where(x => x.EntityChangeId == entityChange.Id).ToList();

                propertyChanges.Count.ShouldBe(3);

                var propertyChange1 = propertyChanges.Single(pc => pc.PropertyName == nameof(Blog.Url));
                propertyChange1.OriginalValue.ShouldBeNull();
                propertyChange1.NewValue.ShouldNotBeNull();

                var propertyChange2 = propertyChanges.Single(pc => pc.PropertyName == nameof(Blog.More));
                propertyChange2.OriginalValue.ShouldBeNull();
                propertyChange2.NewValue.ShouldNotBeNull();

                var propertyChange3 =
                    propertyChanges.Single(pc => pc.PropertyName == nameof(Blog.CreationTime));
                propertyChange3.OriginalValue.ShouldBeNull();
                propertyChange3.NewValue.ShouldNotBeNull();

                // Check "who did this change"
                var changeSet = session.Query<EntityChangeSet>().Single();
                changeSet.ImpersonatorTenantId.ShouldBe(AbpSession.ImpersonatorTenantId);
                changeSet.ImpersonatorUserId.ShouldBe(AbpSession.ImpersonatorUserId);
                changeSet.TenantId.ShouldBe(AbpSession.TenantId);
                changeSet.UserId.ShouldBe(AbpSession.UserId);
            });
        }

        [Fact]
        public void Should_Write_History_For_Audited_Entities_Create_To_Database()
        {
            UsingSession((session) =>
            {
                session.Query<EntityChange>().Count(e => e.TenantId == 1).ShouldBe(0);
                session.Query<EntityChangeSet>().Count(e => e.TenantId == 1).ShouldBe(0);
                session.Query<EntityPropertyChange>().Count(e => e.TenantId == 1).ShouldBe(0);
            });

            var justNow = Clock.Now;
            Thread.Sleep(1);

            var blog2Id = CreateBlogAndGetId();

            UsingSession((session) =>
            {
                session.Query<EntityChange>().Count(e => e.TenantId == 1).ShouldBe(1);
                session.Query<EntityChangeSet>().Count(e => e.TenantId == 1).ShouldBe(1);
                session.Query<EntityChangeSet>().Single().CreationTime.ShouldBeGreaterThan(justNow);
                session.Query<EntityPropertyChange>().Count(e => e.TenantId == 1).ShouldBe(3);
            });
        }

        [Fact]
        public void Should_Write_History_For_Audited_Entities_Update()
        {
            /* Blog has Audited attribute. */

            var newValue = "http://testblog1-changed.myblogs.com";
            var originalValue = UpdateBlogUrlAndGetOriginalValue(newValue);

            UsingSession(session =>
            {
                session.Query<EntityChange>().Count().ShouldBe(1);

                var entityChange = session.Query<EntityChange>()
                    .Single(ec => ec.EntityTypeFullName == typeof(Blog).FullName);
                entityChange.ChangeType.ShouldBe(EntityChangeType.Updated);

                session.Query<EntityPropertyChange>().Count().ShouldBe(1);

                var propertyChange = session.Query<EntityPropertyChange>()
                    .Single(pc => pc.PropertyName == nameof(Blog.Url));
                propertyChange.NewValue.ShouldBe(newValue.ToJsonString());
                propertyChange.OriginalValue.ShouldBe(originalValue.ToJsonString());
                propertyChange.PropertyTypeFullName.ShouldBe(typeof(Blog).GetProperty(nameof(Blog.Url)).PropertyType
                    .FullName);
            });
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

            UsingSession(session =>
            {
                session.Query<EntityChange>().Count().ShouldBe(1);

                var entityChange = session.Query<EntityChange>()
                    .Single(ec => ec.EntityTypeFullName == typeof(Blog).FullName);
                entityChange.ChangeType.ShouldBe(EntityChangeType.Updated);

                session.Query<EntityPropertyChange>().Count().ShouldBe(1);


                var propertyChange = session.Query<EntityPropertyChange>()
                    .Single(pc => pc.PropertyName == nameof(Blog.Url));
                propertyChange.NewValue.ShouldBe(newValue.ToJsonString());
                propertyChange.OriginalValue.ShouldBe(originalValue.ToJsonString());
                propertyChange.PropertyTypeFullName.ShouldBe(typeof(Blog).GetProperty(nameof(Blog.Url)).PropertyType
                    .FullName);
            });
        }

        [Fact]
        public void Should_Write_History_For_Audited_Entities_Update_Complex()
        {
            /* Blog has Audited attribute. */

            int blog1Id = 0;
            var newValue = new BlogEx {BloggerName = "blogger-2"};
            BlogEx originalValue = null;

            WithUnitOfWork(() =>
            {
                var blog1 = _blogRepository.Single(b => b.More.BloggerName == "blogger-1");
                blog1Id = blog1.Id;

                originalValue = new BlogEx {BloggerName = blog1.More.BloggerName};
                blog1.More.BloggerName = newValue.BloggerName;
                _blogRepository.Update(blog1);
            });

            UsingSession(session =>
            {
                session.Query<EntityChange>().Count().ShouldBe(1);

                var entityChange = session.Query<EntityChange>()
                    .Single(ec => ec.EntityTypeFullName == typeof(Blog).FullName);
                entityChange.ChangeType.ShouldBe(EntityChangeType.Updated);
                entityChange.EntityId.ShouldBe(blog1Id.ToJsonString());

                session.Query<EntityPropertyChange>().Count().ShouldBe(1);

                var propertyChange = session.Query<EntityPropertyChange>()
                    .Single(pc => pc.PropertyName == nameof(Blog.More));
                propertyChange.NewValue.ShouldBe(newValue.ToJsonString());
                propertyChange.OriginalValue.ShouldBe(originalValue.ToJsonString());
                propertyChange.PropertyTypeFullName.ShouldBe(typeof(Blog).GetProperty(nameof(Blog.More)).PropertyType
                    .FullName);
            });
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

            UsingSession(session =>
            {
                session.Query<EntityChange>().Count().ShouldBe(1);

                var entityChange = session.Query<EntityChange>()
                    .Single(ec => ec.EntityTypeFullName == typeof(Post).FullName);
                entityChange.ChangeType.ShouldBe(EntityChangeType.Updated);
                entityChange.EntityId.ShouldBe(post1Id.ToJsonString());

                session.Query<EntityPropertyChange>().Count().ShouldBe(1);

                var propertyChange = session.Query<EntityPropertyChange>()
                    .Single(pc => pc.PropertyName == nameof(Post.BlogId));
                propertyChange.NewValue.ShouldBe("2");
                propertyChange.OriginalValue.ShouldBe("1");
                propertyChange.PropertyTypeFullName.ShouldBe(typeof(Post).GetProperty(nameof(Post.BlogId)).PropertyType
                    .FullName);
            });
        }

        [Fact]
        public void Should_Write_History_For_Audited_Property_Foreign_Key_Collection()
        {
            WithUnitOfWork(() =>
            {
                var blog1 = _blogRepository.Single(b => b.Name == "test-blog-1");
                var post10 = new Post {Blog = blog1, Title = "test-post-10-title", Body = "test-post-10-body"};

                // Change navigation property by adding into collection
                blog1.Posts.Add(post10);
                _blogRepository.Update(blog1);
            });

            UsingSession(session =>
            {
                session.Query<EntityChange>().Count().ShouldBe(2);

                /* Post is not in Configuration.Selectors */
                /* Post.Blog has Audited attribute */
                var entityChangePost = session.Query<EntityChange>()
                    .Single(ec => ec.EntityTypeFullName == typeof(Post).FullName);
                entityChangePost.ChangeType.ShouldBe(EntityChangeType.Created);

                session.Query<EntityPropertyChange>().Count().ShouldBe(1);
                session.Query<EntityPropertyChange>().Count(pc => pc.EntityChangeId == entityChangePost.Id).ShouldBe(1);

                var propertyChange1 = session.Query<EntityPropertyChange>().Single(
                    pc => pc.EntityChangeId == entityChangePost.Id && pc.PropertyName == nameof(Post.BlogId)
                );
                propertyChange1.OriginalValue.ShouldBeNull();
                propertyChange1.NewValue.ShouldNotBeNull();

                /* Blog has Audited attribute. */
                var entityChangeBlog = session.Query<EntityChange>()
                    .Single(ec => ec.EntityTypeFullName == typeof(Blog).FullName);
                entityChangeBlog.ChangeType.ShouldBe(EntityChangeType.Updated);
                session.Query<EntityPropertyChange>().Count(pc => pc.EntityChangeId == entityChangeBlog.Id).ShouldBe(0);
            });
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
                propertyChange.PropertyTypeFullName.ShouldBe(typeof(Comment).GetProperty(nameof(Comment.Post))
                    .PropertyType.FullName);

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
                blog1.ChangeUrl("test");
                _blogRepository.Update(blog1);
            });

            UsingSession(session =>
            {
                var entityChange = session.Query<EntityChange>()
                    .Single(ec => ec.EntityTypeFullName == typeof(Blog).FullName);
                entityChange.ShouldNotBeNull();
                entityChange.ChangeType.ShouldBe(EntityChangeType.Updated);

                session.Query<EntityPropertyChange>().Count().ShouldBe(1);
            });
        }


        [Fact]
        public void Should_Write_History_For_Enum_Property_When_Entity_Created()
        {
            Resolve<IEntityHistoryConfiguration>().Selectors.Add("Selected", typeof(Employee));

            int? employeeId = null;
            WithUnitOfWork(() =>
            {
                var john = new Employee
                {
                    FullName = "John Doe",
                    Department = Department.Sales
                };

                employeeId = _employeeRepository.InsertAndGetId(john);
            });

            UsingSession(session =>
            {
                var entityChange = session.Query<EntityChange>().FirstOrDefault();
                entityChange.ShouldNotBeNull();
                entityChange.EntityTypeFullName.ShouldBe(typeof(Employee).FullName);
                ((DateTime?) entityChange.ChangeTime).ShouldNotBe(null);
                entityChange.ChangeType.ShouldBe(EntityChangeType.Created);
                entityChange.EntityId.ShouldBe(employeeId.ToJsonString());

                var propertyChanges = session.Query<EntityPropertyChange>()
                    .Where(ec => ec.EntityChangeId == entityChange.Id).ToList();
                propertyChanges.Count.ShouldBe(5);
                var enumPropertyChange = propertyChanges
                    .FirstOrDefault(pc =>
                        pc.PropertyName == nameof(Employee.Department)
                    );

                enumPropertyChange.ShouldNotBeNull();
                enumPropertyChange.OriginalValue.ShouldBeNull();
                enumPropertyChange.NewValue.ShouldBe(Convert.ToInt32(Department.Sales).ToString());
            });
        }

        private int CreateStudentAndGetId()
        {
            var student = new Student()
            {
                Name = "TestName",
                IdCard = "TestIdCard",
                Address = "TestAddress",
                Grade = 1,
            };

            return _studentRepository.InsertAndGetId(student);
        }

        private int CreateStudentWithCitizenshipAndGetId()
        {
            var student = new Student()
            {
                Name = "TestName",
                IdCard = "TestIdCard",
                Address = "TestAddress",
                Grade = 1,
                CitizenshipInformation = new CitizenshipInformation()
                {
                    CitizenShipId = "123qwe"
                }
            };

            return _studentRepository.InsertAndGetId(student);
        }

        [Fact]
        public void Should_Not_Save_Empty_PropertyChanges()
        {
            // Act
            int itemId = 0;
            WithUnitOfWork(() =>
            {
                var foo = new Foo
                {
                    Audited = "s1"
                };

                itemId = _fooRepository.InsertAndGetId(foo);
            });

            UsingSession(session =>
            {
                session.Query<EntityChange>().Count(e => e.TenantId == 1).ShouldBe(1);
                session.Query<EntityChangeSet>().Count(e => e.TenantId == 1).ShouldBe(1);
                session.Query<EntityPropertyChange>().Count(e => e.TenantId == 1).ShouldBe(1);
            });

            WithUnitOfWork(() =>
            {
                var foo = _fooRepository.Get(itemId);
                foo.NonAudited = "s2";
                _fooRepository.Update(foo);
            });

            // Assert
            UsingSession((session) =>
            {
                session.Query<EntityChange>().Count(e => e.TenantId == 1).ShouldBe(1);
                session.Query<EntityChangeSet>().Count(e => e.TenantId == 1).ShouldBe(1);
                session.Query<EntityPropertyChange>().Count(e => e.TenantId == 1).ShouldBe(1);
            });
        }

        [Fact]
        public void Should_Work_Properly_With_Large_Data()
        {
            var stringBuilder = new StringBuilder();
            for (int i = 0; i <= EntityPropertyChange.MaxValueLength + 1; i++)
            {
                stringBuilder.Append("a");
            }

            var bigStringWithTruncateWithPostfix = stringBuilder.ToString().ToJsonString()
                .TruncateWithPostfix(EntityPropertyChange.MaxValueLength);

            // Act
            int itemId = 0;
            WithUnitOfWork(() =>
            {
                var foo = new Foo
                {
                    Audited = stringBuilder.ToString()
                };

                itemId = _fooRepository.InsertAndGetId(foo);
            });

            UsingSession((session) =>
            {
                session.Query<EntityChange>().Count(e => e.TenantId == 1).ShouldBe(1);
                session.Query<EntityChangeSet>().Count(e => e.TenantId == 1).ShouldBe(1);
                session.Query<EntityPropertyChange>().Count(e => e.TenantId == 1).ShouldBe(1);
                var change = session.Query<EntityPropertyChange>().Single();
                change.OriginalValue.ShouldBeNull();
                change.NewValue.ShouldBe(bigStringWithTruncateWithPostfix);
            });

            WithUnitOfWork(() =>
            {
                var foo = _fooRepository.Get(itemId);
                foo.Audited = stringBuilder.ToString() + "bbbbbbbbbbbbbb";
                _fooRepository.Update(foo);
            });

            UsingSession((context) =>
            {
                context.Query<EntityChange>().Count(e => e.TenantId == 1).ShouldBe(2);
                context.Query<EntityChangeSet>().Count(e => e.TenantId == 1).ShouldBe(2);
                context.Query<EntityPropertyChange>().Count(e => e.TenantId == 1).ShouldBe(2);
                var changes = context.Query<EntityPropertyChange>().ToList();

                changes[0].OriginalValue.ShouldBeNull();
                changes[0].NewValue.ShouldBe(bigStringWithTruncateWithPostfix);

                //even though the original value and new value are equal, changes will be detected on entity
                //(the actual values have been truncated because they are too large to be stored. truncated values are equal but actual values are not)
                changes[1].OriginalValue.ShouldBe(bigStringWithTruncateWithPostfix);
                changes[1].NewValue.ShouldBe(bigStringWithTruncateWithPostfix);
                //hashes must be different
                changes[1].NewValueHash.ShouldNotBe(changes[1].OriginalValueHash);
            });
        }

        #endregion

        #region CASES DON'T WRITE HISTORY

        [Fact]
        public void Should_Not_Write_History_If_Disabled()
        {
            Resolve<IEntityHistoryConfiguration>().IsEnabled = false;

            /* Blog has Audited attribute. */
            var originalValue = UpdateBlogUrlAndGetOriginalValue("http://testblog1-changed.myblogs.com");

            UsingSession(session => { session.Query<EntityChangeSet>().Count().ShouldBe(0); });
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

            UsingSession(session => { session.Query<EntityChangeSet>().Count().ShouldBe(0); });
        }

        [Fact]
        public void Should_Not_Write_History_If_Ignored()
        {
            Resolve<IEntityHistoryConfiguration>().IgnoredTypes.Add(typeof(Blog));

            /* Blog has Audited attribute. */
            var originalValue = UpdateBlogUrlAndGetOriginalValue("http://testblog1-changed.myblogs.com");

            UsingSession(session => { session.Query<EntityChangeSet>().Count().ShouldBe(0); });
        }

        [Fact]
        public void Should_Not_Write_History_If_Selected_But_Ignored()
        {
            Resolve<IEntityHistoryConfiguration>().Selectors.Add("Selected", typeof(Blog));
            Resolve<IEntityHistoryConfiguration>().IgnoredTypes.Add(typeof(Blog));

            /* Blog has Audited attribute. */

            UpdateBlogUrlAndGetOriginalValue("http://testblog1-changed.myblogs.com");

            UsingSession(session => { session.Query<EntityChangeSet>().Count().ShouldBe(0); });
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

            UsingSession(session => { session.Query<EntityChangeSet>().Count().ShouldBe(0); });
        }

        [Fact]
        public void Should_Not_Write_History_If_Invalid_Entity_Has_Property_With_Audited_Attribute_Created()
        {
            //Act
            UsingSession(session => { session.Save(new Category {DisplayName = "My Category"}); });

            //Assert
            UsingSession(session => { session.Query<EntityChangeSet>().Count().ShouldBe(0); });
        }

        [Fact]
        public void Should_Not_Write_History_If_Invalid_Entity_Has_Property_With_Audited_Attribute_Updated()
        {
            //Arrange
            UsingSession((session) => { session.Save(new Category {DisplayName = "My Category"}); });

            //Act
            UsingSession(session =>
            {
                var category = session.Query<Category>().Single(c => c.DisplayName == "My Category");
                category.DisplayName = "Invalid Category";
                session.Update(category);
            });

            //Assert
            UsingSession(session => { session.Query<EntityChangeSet>().Count().ShouldBe(0); });
        }

        [Fact]
        public void Should_Not_Write_History_If_Invalid_Entity_Has_Property_With_Audited_Attribute_Deleted()
        {
            //Arrange
            UsingSession((session) => { session.Save(new Category {DisplayName = "My Category"}); });

            //Act
            UsingSession((session) =>
            {
                var category = session.Query<Category>().Single(c => c.DisplayName == "My Category");
                session.Delete(category);
            });

            //Assert
            UsingSession(session => { session.Query<EntityChangeSet>().Count().ShouldBe(0); });
        }

        [Fact]
        public void Should_Not_Write_History_For_Audited_Entity_By_Default()
        {
            //Arrange
            UsingSession((session) => { session.Save(new Country {CountryCode = "My Country"}); });

            //Assert
            UsingSession(session =>
            {
                session.Query<EntityChangeSet>().Count().ShouldBe(0);
                session.Query<EntityChange>().Count(ec => ec.EntityTypeFullName == typeof(Country).FullName)
                    .ShouldBe(0);
            });
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