using System.Data.Entity;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.EntityFramework.Repositories;
using Abp.Tests;
using Castle.MicroKernel.Registration;
using Shouldly;
using Xunit;

namespace Abp.EntityFramework.Tests.Repositories
{
    public class EntityFrameworkGenericRepositoryRegistrar_Tests : TestBaseWithLocalIocManager
    {
        [Fact]
        public void Should_Resolve_Generic_Repositories()
        {
            var fakeDbContextProvider = NSubstitute.Substitute.For<IDbContextProvider<MyDbContext>>();

            LocalIocManager.IocContainer.Register(
                Component.For<IDbContextProvider<MyDbContext>>().UsingFactoryMethod(() => fakeDbContextProvider)
                );

            EntityFrameworkGenericRepositoryRegistrar.RegisterForDbContext(typeof(MyDbContext), LocalIocManager);

            var entity1Repository = LocalIocManager.Resolve<IRepository<MyEntity1>>();
            entity1Repository.ShouldNotBe(null);

            var entity1RepositoryWithPk = LocalIocManager.Resolve<IRepository<MyEntity1, int>>();
            entity1RepositoryWithPk.ShouldNotBe(null);

            var entity2Repository = LocalIocManager.Resolve<IRepository<MyEntity2, long>>();
            entity2Repository.ShouldNotBe(null);
        }

        public class MyDbContext : MyBaseDbContext
        {
            public DbSet<MyEntity2> MyEntities2 { get; set; }
        }

        public abstract class MyBaseDbContext : AbpDbContext
        {
            public IDbSet<MyEntity1> MyEntities1 { get; set; }            
        }

        public class MyEntity1 : Entity
        {

        }

        public class MyEntity2 : Entity<long>
        {

        }
    }
}
