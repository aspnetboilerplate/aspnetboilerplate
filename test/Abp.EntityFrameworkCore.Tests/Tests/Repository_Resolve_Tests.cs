using System;
using System.Linq;
using System.Reflection;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Repositories;
using Abp.EntityFrameworkCore.Tests.Domain;
using Abp.EntityFrameworkCore.Tests.Ef;
using Shouldly;
using Xunit;

namespace Abp.EntityFrameworkCore.Tests.Tests
{
    public class Repository_Resolve_Tests : EntityFrameworkCoreModuleTestBase
    {
        [Fact]
        public void Should_Resolve_Custom_Repository_If_Registered()
        {
            var postRepository = Resolve<IRepository<Post, Guid>>();

            postRepository.GetAllList().Any().ShouldBeTrue();

            Assert.Throws<Exception>(
                () => postRepository.Count()
            ).Message.ShouldBe("can not get count of posts");

            //Should also resolve by custom interface and implementation
            Resolve<IPostRepository>();
            Resolve<PostRepository>();
        }

        [Fact]
        public void Should_Resolve_Default_Repositories_For_Second_DbContext()
        {
            var repo1 = Resolve<IRepository<Ticket>>();
            var repo2 = Resolve<IRepository<Ticket, int>>();

            Assert.Throws<Exception>(
                () => repo1.Count()
            ).Message.ShouldBe("can not get count!");

            Assert.Throws<Exception>(
                () => repo2.Count()
            ).Message.ShouldBe("can not get count!");
        }

        [Fact]
        public void Should_Resolve_Custom_Repositories_For_Second_DbContext()
        {
            var repo1 = Resolve<ISupportRepository<Ticket>>();
            var repo2 = Resolve<ISupportRepository<Ticket, int>>();

            typeof(ISupportRepository<Ticket>).GetTypeInfo().IsInstanceOfType(repo1).ShouldBeTrue();
            typeof(ISupportRepository<Ticket, int>).GetTypeInfo().IsInstanceOfType(repo1).ShouldBeTrue();
            typeof(ISupportRepository<Ticket, int>).GetTypeInfo().IsInstanceOfType(repo2).ShouldBeTrue();

            Assert.Throws<Exception>(
                () => repo1.Count()
            ).Message.ShouldBe("can not get count!");

            Assert.Throws<Exception>(
                () => repo2.Count()
            ).Message.ShouldBe("can not get count!");

            var activeTickets = repo1.GetActiveList();
            activeTickets.Count.ShouldBeGreaterThan(0);
            activeTickets.All(t => t.IsActive).ShouldBeTrue();

            activeTickets = repo2.GetActiveList();
            activeTickets.Count.ShouldBeGreaterThan(0);
            activeTickets.All(t => t.IsActive).ShouldBeTrue();
        }

        [Fact]
        public void Should_Get_DbContext()
        {
            Resolve<IPostRepository>().GetDbContext().ShouldBeOfType<BloggingDbContext>();
        }

        [Fact]
        public void Should_Get_DbContext_2()
        {
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                Resolve<IRepository<Blog>>().GetDbContext().ShouldBeOfType<BloggingDbContext>();

                uow.Complete();
            }
        }

        [Fact]
        public void Should_Get_DbContext_From_Second_DbContext()
        {
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                Resolve<IRepository<Ticket>>().GetDbContext().ShouldBeOfType<SupportDbContext>();

                uow.Complete();
            }
        }

        [Fact]
        public void Should_Get_DbContext_From_Second_DbContext_With_Custom_Repository()
        {
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                Resolve<ISupportRepository<Ticket>>().GetDbContext().ShouldBeOfType<SupportDbContext>();

                uow.Complete();
            }
        }
    }
}