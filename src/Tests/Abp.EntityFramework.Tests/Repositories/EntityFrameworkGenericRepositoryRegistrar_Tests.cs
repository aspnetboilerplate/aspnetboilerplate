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

            EntityFrameworkGenericRepositoryRegistrar.RegisterForDbContext(typeof(MyModuleDbContext), LocalIocManager);
            EntityFrameworkGenericRepositoryRegistrar.RegisterForDbContext(typeof(MyMainDbContext), LocalIocManager);
        }

        [Fact]
        public void Should_Resolve_Generic_Repositories()
        {
            //Entity 1 (with default PK)
            var entity1Repository = LocalIocManager.Resolve<IRepository<MyEntity1>>();
            entity1Repository.ShouldNotBe(null);
            (entity1Repository is EfRepositoryBase<MyMainDbContext, MyEntity1>).ShouldBe(true);

            //Entity 1 (with specified PK)
            var entity1RepositoryWithPk = LocalIocManager.Resolve<IRepository<MyEntity1, int>>();
            entity1RepositoryWithPk.ShouldNotBe(null);
            (entity1RepositoryWithPk is EfRepositoryBase<MyMainDbContext, MyEntity1, int>).ShouldBe(true);

            //Entity 2
            var entity2Repository = LocalIocManager.Resolve<IRepository<MyEntity2, long>>();
            (entity2Repository is EfRepositoryBase<MyMainDbContext, MyEntity2, long>).ShouldBe(true);
            entity2Repository.ShouldNotBe(null);

            //Entity 3
            var entity3Repository = LocalIocManager.Resolve<IMyModuleRepository<MyEntity3, Guid>>();
            (entity3Repository is EfRepositoryBase<MyModuleDbContext, MyEntity3, Guid>).ShouldBe(true);
            entity3Repository.ShouldNotBe(null);
        }

        public class MyMainDbContext : MyBaseDbContext
        {
            public virtual DbSet<MyEntity2> MyEntities2 { get; set; }
        }

        [AutoRepositoryTypes(
            typeof(IMyModuleRepository<>), 
            typeof(IMyModuleRepository<,>), 
            typeof(MyModuleRepositoryBase<>), 
            typeof(MyModuleRepositoryBase<,>)
            )]
        public class MyModuleDbContext : MyBaseDbContext
        {
            public virtual DbSet<MyEntity3> MyEntities3 { get; set; }
        }

        public abstract class MyBaseDbContext : AbpDbContext
        {
            public virtual IDbSet<MyEntity1> MyEntities1 { get; set; }
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

        public interface IMyModuleRepository<TEntity> : IRepository<TEntity>
            where TEntity : class, IEntity<int>
        {

        }

        public interface IMyModuleRepository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
            where TEntity : class, IEntity<TPrimaryKey>
        {

        }

        public class MyModuleRepositoryBase<TEntity, TPrimaryKey> : EfRepositoryBase<MyModuleDbContext, TEntity, TPrimaryKey>, IMyModuleRepository<TEntity, TPrimaryKey>
            where TEntity : class, IEntity<TPrimaryKey>
        {
            public MyModuleRepositoryBase(IDbContextProvider<MyModuleDbContext> dbContextProvider)
                : base(dbContextProvider)
            {
            }
        }

        public class MyModuleRepositoryBase<TEntity> : MyModuleRepositoryBase<TEntity, int>, IMyModuleRepository<TEntity>
            where TEntity : class, IEntity<int>
        {
            public MyModuleRepositoryBase(IDbContextProvider<MyModuleDbContext> dbContextProvider)
                : base(dbContextProvider)
            {
            }
        }
    }
}
