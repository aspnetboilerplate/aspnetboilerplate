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
using Abp.Zero.SampleApp.EntityHistory.Nhibernate;
using Abp.Zero.SampleApp.NHibernate;
using Abp.Zero.SampleApp.TPH.NHibernate;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.EntityHistory
{
    public class SimpleEntityHistory_Test : NHibernateTestBase
    {
        private readonly IRepository<NhAdvertisement> _advertisementRepository;
        private readonly IRepository<NhBlog> _blogRepository;
        private readonly IRepository<NhPost, Guid> _postRepository;
        private readonly IRepository<NhComment> _commentRepository;
        private readonly IRepository<NhStudent> _studentRepository;
        private readonly IRepository<NhFoo> _fooRepository;
        private readonly IRepository<NhEmployee> _employeeRepository;

        private readonly IEntityHistoryStore _entityHistoryStore;

        public SimpleEntityHistory_Test()
        {
            _advertisementRepository = Resolve<IRepository<NhAdvertisement>>();
            _blogRepository = Resolve<IRepository<NhBlog>>();
            _postRepository = Resolve<IRepository<NhPost, Guid>>();
            _commentRepository = Resolve<IRepository<NhComment>>();
            _studentRepository = Resolve<IRepository<NhStudent>>();
            _fooRepository = Resolve<IRepository<NhFoo>>();
            _employeeRepository = Resolve<IRepository<NhEmployee>>();
            _entityHistoryStore = Resolve<IEntityHistoryStore>();

            var user = GetDefaultTenantAdmin();
            AbpSession.TenantId = user.TenantId;
            AbpSession.UserId = user.Id;

            Resolve<IEntityHistoryConfiguration>().IsEnabledForAnonymousUsers = true;
        }

        #region CASES WRITE HISTORY

        [Fact]
        public void Should_Write_History_For_Tracked_Entities_Create()
        {
            /* Advertisement does not have Audited attribute. */
            Resolve<IEntityHistoryConfiguration>().Selectors.Add("Selected", typeof(NhAdvertisement));

            int? advertisementId = null;
            WithUnitOfWork(() =>
            {
                var advertisement = new NhAdvertisement {Banner = "tracked-advertisement"};
                advertisementId = _advertisementRepository.InsertAndGetId(advertisement);
            });

            UsingSession(session =>
            {
                session.Query<EntityChange>().Count().ShouldBe(1);

                var entityChange =
                    session.Query<EntityChange>().Single(ec => ec.EntityTypeFullName == typeof(NhAdvertisement).FullName);
                ((DateTime?) entityChange.ChangeTime).ShouldNotBe(null);
                entityChange.ChangeType.ShouldBe(EntityChangeType.Created);
                entityChange.EntityId.ShouldBe(advertisementId.ToJsonString());

                var propertyChanges = session.Query<EntityPropertyChange>()
                    .Where(pc => pc.EntityChangeId == entityChange.Id).ToList();
                propertyChanges.Count.ShouldBe(1);

                var propertyChange1 =
                    propertyChanges.Single(pc => pc.PropertyName == nameof(NhAdvertisement.Banner));
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
            Resolve<IEntityHistoryConfiguration>().Selectors.Add("Selected", typeof(NhAdvertisement));

            var justNow = Clock.Now;
            Thread.Sleep(1);

            WithUnitOfWork(() =>
            {
                _advertisementRepository.InsertAndGetId(new NhAdvertisement {Banner = "tracked-advertisement"});
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
            Resolve<IEntityHistoryConfiguration>().Selectors.Add("Selected", typeof(NhStudent));

            var student = new NhStudent
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
                    .Single(ec => ec.EntityTypeFullName == typeof(NhStudent).FullName);
                ((DateTime?) entityChange.ChangeTime).ShouldNotBe(null);
                entityChange.ChangeType.ShouldBe(EntityChangeType.Created);
                entityChange.EntityId.ShouldBe(student.Id.ToJsonString());

                var propertyChanges = session.Query<EntityPropertyChange>()
                    .Where(pc => pc.EntityChangeId == entityChange.Id).ToList();
                propertyChanges.Count().ShouldBe(4); //Name,IdCard,Address,Grade

                var propertyChange1 =
                    propertyChanges.Single(pc => pc.PropertyName == nameof(NhStudent.Name));
                propertyChange1.OriginalValue.ShouldBeNull();
                propertyChange1.NewValue.ShouldNotBeNull();

                var propertyChange2 =
                    propertyChanges.Single(pc => pc.PropertyName == nameof(NhStudent.IdCard));
                propertyChange2.OriginalValue.ShouldBeNull();
                propertyChange2.NewValue.ShouldNotBeNull();

                var propertyChange3 =
                    propertyChanges.Single(pc => pc.PropertyName == nameof(NhStudent.Address));
                propertyChange3.OriginalValue.ShouldBeNull();
                propertyChange3.NewValue.ShouldNotBeNull();

                var propertyChange4 =
                    propertyChanges.Single(pc => pc.PropertyName == nameof(NhStudent.Grade));
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

            Resolve<IEntityHistoryConfiguration>().Selectors.Add("Selected", typeof(NhStudent));

            var justNow = Clock.Now;
            Thread.Sleep(1);

            var student = new NhStudent()
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
            Resolve<IEntityHistoryConfiguration>().Selectors.Add("Selected", typeof(NhAdvertisement));

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
                    ec => ec.EntityTypeFullName == typeof(NhAdvertisement).FullName
                );
                entityChange.ChangeType.ShouldBe(EntityChangeType.Updated);
                
                var propertyChanges = session.Query<EntityPropertyChange>()
                    .Where(pc => pc.EntityChangeId == entityChange.Id).ToList();
                propertyChanges.Count.ShouldBe(1);

                var propertyChange = propertyChanges.Single(
                    pc => pc.PropertyName == nameof(NhAdvertisement.Banner)
                );
                propertyChange.NewValue.ShouldBe("test-advertisement-1-updated".ToJsonString());
                propertyChange.OriginalValue.ShouldBe("test-advertisement-1".ToJsonString());
                propertyChange.PropertyTypeFullName.ShouldBe(typeof(NhAdvertisement)
                    .GetProperty(nameof(NhAdvertisement.Banner)).PropertyType.FullName);
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

                var entityChange = entityChanges.Single(ec => ec.EntityTypeFullName == typeof(NhBlog).FullName);
                entityChange.ChangeType.ShouldBe(EntityChangeType.Created);
                entityChange.EntityId.ShouldBe(blog2Id.ToJsonString());

                var propertyChanges = session.Query<EntityPropertyChange>()
                    .Where(x => x.EntityChangeId == entityChange.Id).ToList();

                propertyChanges.Count.ShouldBe(3);

                var propertyChange1 = propertyChanges.Single(pc => pc.PropertyName == nameof(NhBlog.Url));
                propertyChange1.OriginalValue.ShouldBeNull();
                propertyChange1.NewValue.ShouldNotBeNull();

                var propertyChange2 = propertyChanges.Single(pc => pc.PropertyName == nameof(NhBlog.More));
                propertyChange2.OriginalValue.ShouldBeNull();
                propertyChange2.NewValue.ShouldNotBeNull();

                var propertyChange3 =
                    propertyChanges.Single(pc => pc.PropertyName == nameof(NhBlog.CreationTime));
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
                    .Single(ec => ec.EntityTypeFullName == typeof(NhBlog).FullName);
                entityChange.ChangeType.ShouldBe(EntityChangeType.Updated);

                session.Query<EntityPropertyChange>().Count().ShouldBe(1);

                var propertyChange = session.Query<EntityPropertyChange>()
                    .Single(pc => pc.PropertyName == nameof(NhBlog.Url));
                propertyChange.NewValue.ShouldBe(newValue.ToJsonString());
                propertyChange.OriginalValue.ShouldBe(originalValue.ToJsonString());
                propertyChange.PropertyTypeFullName.ShouldBe(typeof(NhBlog).GetProperty(nameof(NhBlog.Url)).PropertyType
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
                    .Single(ec => ec.EntityTypeFullName == typeof(NhBlog).FullName);
                entityChange.ChangeType.ShouldBe(EntityChangeType.Updated);

                session.Query<EntityPropertyChange>().Count().ShouldBe(1);


                var propertyChange = session.Query<EntityPropertyChange>()
                    .Single(pc => pc.PropertyName == nameof(NhBlog.Url));
                propertyChange.NewValue.ShouldBe(newValue.ToJsonString());
                propertyChange.OriginalValue.ShouldBe(originalValue.ToJsonString());
                propertyChange.PropertyTypeFullName.ShouldBe(typeof(NhBlog).GetProperty(nameof(NhBlog.Url)).PropertyType
                    .FullName);
            });
        }

        [Fact]
        public void Should_Write_History_For_Audited_Entities_Update_Complex()
        {
            /* Blog has Audited attribute. */

            int blog1Id = 0;
            var newValue = new NhBlogEx {BloggerName = "blogger-2"};
            NhBlogEx originalValue = null;

            WithUnitOfWork(() =>
            {
                var blog1 = _blogRepository.Single(b => b.More.BloggerName == "blogger-1");
                blog1Id = blog1.Id;

                originalValue = new NhBlogEx {BloggerName = blog1.More.BloggerName};
                blog1.More.BloggerName = newValue.BloggerName;
                _blogRepository.Update(blog1);
            });

            UsingSession(session =>
            {
                session.Query<EntityChange>().Count().ShouldBe(1);

                var entityChange = session.Query<EntityChange>()
                    .Single(ec => ec.EntityTypeFullName == typeof(NhBlog).FullName);
                entityChange.ChangeType.ShouldBe(EntityChangeType.Updated);
                entityChange.EntityId.ShouldBe(blog1Id.ToJsonString());

                session.Query<EntityPropertyChange>().Count().ShouldBe(1);

                var propertyChange = session.Query<EntityPropertyChange>()
                    .Single(pc => pc.PropertyName == nameof(NhBlog.More));
                propertyChange.NewValue.ShouldBe(newValue.ToJsonString());
                propertyChange.OriginalValue.ShouldBe(originalValue.ToJsonString());
                propertyChange.PropertyTypeFullName.ShouldBe(typeof(NhBlog).GetProperty(nameof(NhBlog.More)).PropertyType
                    .FullName);
            });
        }
        
        [Fact]
        public void Should_Write_History_For_Enum_Property_When_Entity_Created()
        {
            Resolve<IEntityHistoryConfiguration>().Selectors.Add("Selected", typeof(NhEmployee));

            int? employeeId = null;
            WithUnitOfWork(() =>
            {
                var john = new NhEmployee
                {
                    FullName = "John Doe",
                    Department = NhDepartment.Sales
                };

                employeeId = _employeeRepository.InsertAndGetId(john);
            });

            UsingSession(session =>
            {
                var entityChange = session.Query<EntityChange>().FirstOrDefault();
                entityChange.ShouldNotBeNull();
                entityChange.EntityTypeFullName.ShouldBe(typeof(NhEmployee).FullName);
                ((DateTime?) entityChange.ChangeTime).ShouldNotBe(null);
                entityChange.ChangeType.ShouldBe(EntityChangeType.Created);
                entityChange.EntityId.ShouldBe(employeeId.ToJsonString());

                var propertyChanges = session.Query<EntityPropertyChange>()
                    .Where(ec => ec.EntityChangeId == entityChange.Id).ToList();
                propertyChanges.Count.ShouldBe(5);
                var enumPropertyChange = propertyChanges
                    .FirstOrDefault(pc =>
                        pc.PropertyName == nameof(NhEmployee.Department)
                    );

                enumPropertyChange.ShouldNotBeNull();
                enumPropertyChange.OriginalValue.ShouldBeNull();
                enumPropertyChange.NewValue.ShouldBe(Convert.ToInt32(NhDepartment.Sales).ToString());
            });
        }

        private int CreateStudentAndGetId()
        {
            var student = new NhStudent()
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
            var student = new NhStudent()
            {
                Name = "TestName",
                IdCard = "TestIdCard",
                Address = "TestAddress",
                Grade = 1,
                CitizenshipInformation = new NhCitizenshipInformation()
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
                var foo = new NhFoo
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
                var foo = new NhFoo
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
                _advertisementRepository.Insert(new NhAdvertisement
                {
                    Banner = "not-selected-advertisement"
                });
            });

            UsingSession(session => { session.Query<EntityChangeSet>().Count().ShouldBe(0); });
        }

        [Fact]
        public void Should_Not_Write_History_If_Ignored()
        {
            Resolve<IEntityHistoryConfiguration>().IgnoredTypes.Add(typeof(NhBlog));

            /* Blog has Audited attribute. */
            var originalValue = UpdateBlogUrlAndGetOriginalValue("http://testblog1-changed.myblogs.com");

            UsingSession(session => { session.Query<EntityChangeSet>().Count().ShouldBe(0); });
        }

        [Fact]
        public void Should_Not_Write_History_If_Selected_But_Ignored()
        {
            Resolve<IEntityHistoryConfiguration>().Selectors.Add("Selected", typeof(NhBlog));
            Resolve<IEntityHistoryConfiguration>().IgnoredTypes.Add(typeof(NhBlog));

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
            UsingSession(session => { session.Save(new NhCategory {DisplayName = "My Category"}); });

            //Assert
            UsingSession(session => { session.Query<EntityChangeSet>().Count().ShouldBe(0); });
        }

        [Fact]
        public void Should_Not_Write_History_If_Invalid_Entity_Has_Property_With_Audited_Attribute_Updated()
        {
            //Arrange
            UsingSession((session) => { session.Save(new NhCategory {DisplayName = "My Category"}); });

            //Act
            UsingSession(session =>
            {
                var category = session.Query<NhCategory>().Single(c => c.DisplayName == "My Category");
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
            UsingSession((session) => { session.Save(new NhCategory {DisplayName = "My Category"}); });

            //Act
            UsingSession((session) =>
            {
                var category = session.Query<NhCategory>().Single(c => c.DisplayName == "My Category");
                session.Delete(category);
            });

            //Assert
            UsingSession(session => { session.Query<EntityChangeSet>().Count().ShouldBe(0); });
        }

        [Fact]
        public void Should_Not_Write_History_For_Audited_Entity_By_Default()
        {
            //Arrange
            UsingSession((session) => { session.Save(new NhCountry {CountryCode = "My Country"}); });

            //Assert
            UsingSession(session =>
            {
                session.Query<EntityChangeSet>().Count().ShouldBe(0);
                session.Query<EntityChange>().Count(ec => ec.EntityTypeFullName == typeof(NhCountry).FullName)
                    .ShouldBe(0);
            });
        }

        #endregion

        private int CreateBlogAndGetId()
        {
            int blog2Id = 0;

            WithUnitOfWork(() =>
            {
                var blog2 = new NhBlog("test-blog-2", "http://testblog2.myblogs.com", "blogger-2");
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