using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.EntityFramework.Repositories;
using Abp.Tests;
using Castle.MicroKernel.Registration;
using NSubstitute;
using Shouldly;
using System;
using System.Data.Entity;
using Xunit;

namespace Abp.EntityFramework.Tests.Repositories
{
    public class EntityFrameworkGenericRepositoryRegistrar_Tests : TestBaseWithLocalIocManager
    {
        public EntityFrameworkGenericRepositoryRegistrar_Tests()
        {
            var fakeBaseDbContextProvider = Substitute.For<IDbContextProvider<MyBaseDbContext>>();
            var fakeMainDbContextProvider = Substitute.For<IDbContextProvider<MyMainDbContext>>();
            var fakeModuleDbContextProvider = Substitute.For<IDbContextProvider<MyModuleDbContext>>();

            LocalIocManager.IocContainer.Register(
                Component.For<IDbContextProvider<MyBaseDbContext>>().UsingFactoryMethod(() => fakeBaseDbContextProvider),
                Component.For<IDbContextProvider<MyMainDbContext>>().UsingFactoryMethod(() => fakeMainDbContextProvider),
                Component.For<IDbContextProvider<MyModuleDbContext>>()
                    .UsingFactoryMethod(() => fakeModuleDbContextProvider),
                Component.For<EntityFrameworkGenericRepositoryRegistrar>().LifestyleTransient()
                );

            using (
                var repositoryRegistrar =
                    LocalIocManager.ResolveAsDisposable<EntityFrameworkGenericRepositoryRegistrar>())
            {
                repositoryRegistrar.Object.RegisterForDbContext(typeof(MyModuleDbContext), LocalIocManager);
                repositoryRegistrar.Object.RegisterForDbContext(typeof(MyMainDbContext), LocalIocManager);
            }
        }

        public class MyMainDbContext : MyBaseDbContext
        {
            public virtual DbSet<MyEntity2> MyEntities2 { get; set; }

            public virtual DbSet<MyNonEntity> MyNonEntities { get; set; }
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

        public class MyEntity2 : Entity<Guid>
        {
        }

        public class MyEntity3 : Entity<Guid>
        {
        }

        public class MyNonEntity
        {
        }

        public interface IMyModuleRepository<TEntity> : IRepository<TEntity>
            where TEntity : class, IEntity<Guid>
        {
        }

        public interface IMyModuleRepository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
            where TEntity : class, IEntity<TPrimaryKey>
        {
        }

        public class MyModuleRepositoryBase<TEntity, TPrimaryKey> :
            EfRepositoryBase<MyModuleDbContext, TEntity, TPrimaryKey>, IMyModuleRepository<TEntity, TPrimaryKey>
            where TEntity : class, IEntity<TPrimaryKey>
        {
            public MyModuleRepositoryBase(IDbContextProvider<MyModuleDbContext> dbContextProvider)
                : base(dbContextProvider)
            {
            }
        }

        public class MyModuleRepositoryBase<TEntity> : MyModuleRepositoryBase<TEntity, Guid>,
            IMyModuleRepository<TEntity>
            where TEntity : class, IEntity<Guid>
        {
            public MyModuleRepositoryBase(IDbContextProvider<MyModuleDbContext> dbContextProvider)
                : base(dbContextProvider)
            {
            }
        }

        [Fact]
        public void Should_Resolve_Generic_Repositories()
        {
            //Entity 1 (with default PK)
            var entity1Repository = LocalIocManager.Resolve<IRepository<MyEntity1, Guid>>();
            entity1Repository.ShouldNotBe(null);
            (entity1Repository is EfRepositoryBase<MyBaseDbContext, MyEntity1, Guid>).ShouldBe(true);

            //Entity 1 (with specified PK)
            var entity1RepositoryWithPk = LocalIocManager.Resolve<IRepository<MyEntity1, Guid>>();
            entity1RepositoryWithPk.ShouldNotBe(null);
            (entity1RepositoryWithPk is EfRepositoryBase<MyBaseDbContext, MyEntity1, Guid>).ShouldBe(true);

            //Entity 2
            var entity2Repository = LocalIocManager.Resolve<IRepository<MyEntity2, Guid>>();
            (entity2Repository is EfRepositoryBase<MyMainDbContext, MyEntity2, Guid>).ShouldBe(true);
            entity2Repository.ShouldNotBe(null);

            //Entity 3
            var entity3Repository = LocalIocManager.Resolve<IMyModuleRepository<MyEntity3, Guid>>();
            (entity3Repository is EfRepositoryBase<MyModuleDbContext, MyEntity3, Guid>).ShouldBe(true);
            entity3Repository.ShouldNotBe(null);
        }
    }
}