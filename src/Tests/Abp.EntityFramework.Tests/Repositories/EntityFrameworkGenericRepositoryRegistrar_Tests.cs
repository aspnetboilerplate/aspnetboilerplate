using System;
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
        public EntityFrameworkGenericRepositoryRegistrar_Tests()
        {
            var fakeMainDbContextProvider = NSubstitute.Substitute.For<IDbContextProvider<MyMainDbContext>>();
            var fakeModuleDbContextProvider = NSubstitute.Substitute.For<IDbContextProvider<MyModuleDbContext>>();

            LocalIocManager.IocContainer.Register(
                Component.For<IDbContextProvider<MyMainDbContext>>().UsingFactoryMethod(() => fakeMainDbContextProvider),
                Component.For<IDbContextProvider<MyModuleDbContext>>().UsingFactoryMethod(() => fakeModuleDbContextProvider)
                );

            EntityFrameworkGenericRepositoryRegistrar.RegisterForDbContext(typeof(MyMainDbContext), LocalIocManager);
            EntityFrameworkGenericRepositoryRegistrar.RegisterForDbContext(typeof(MyModuleDbContext), LocalIocManager);
        }

        [Fact]
        public void Should_Resolve_Generic_Repositories()
        {
            var entity1Repository = LocalIocManager.Resolve<IRepository<MyEntity1>>();
            entity1Repository.ShouldNotBe(null);
            (entity1Repository is EfRepositoryBase<MyMainDbContext, MyEntity1>).ShouldBe(true);

            var entity1RepositoryWithPk = LocalIocManager.Resolve<IRepository<MyEntity1, int>>();
            entity1RepositoryWithPk.ShouldNotBe(null);
            (entity1RepositoryWithPk is EfRepositoryBase<MyMainDbContext, MyEntity1, int>).ShouldBe(true);

            var entity2Repository = LocalIocManager.Resolve<IRepository<MyEntity2, long>>();
            (entity2Repository is EfRepositoryBase<MyMainDbContext, MyEntity2, long>).ShouldBe(true);
            entity2Repository.ShouldNotBe(null);

            var entity3Repository = LocalIocManager.Resolve<IRepository<MyEntity3, Guid>>();
            (entity3Repository is EfRepositoryBase<MyModuleDbContext, MyEntity3, Guid>).ShouldBe(true);
            entity3Repository.ShouldNotBe(null);
        }

        public class MyMainDbContext : MyBaseDbContext
        {
            public DbSet<MyEntity2> MyEntities2 { get; set; }
        }

        public class MyModuleDbContext : MyBaseDbContext
        {
            public DbSet<MyEntity3> MyEntities3 { get; set; }
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

        public class MyEntity3 : Entity<Guid>
        {

        }
    }
}
