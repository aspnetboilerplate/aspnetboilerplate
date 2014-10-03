using System.Data.Entity;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Tests;
using Shouldly;
using Xunit;

namespace Abp.EntityFramework.Tests.Repositories
{
    public class EntityFrameworkGenericRepositoryRegistrar_Tests : TestBaseWithSelfIocManager
    {
        [Fact]
        public void Should_Resolve_Generic_Repositories()
        {
            EntityFrameworkGenericRepositoryRegistrar.RegisterDbContext(typeof(MyDbContext), LocalIocManager);

            var entity1Repository = LocalIocManager.Resolve<IRepository<MyEntity1>>();
            entity1Repository.ShouldNotBe(null);

            var entity1RepositoryWithPk = LocalIocManager.Resolve<IRepository<MyEntity1, int>>();
            entity1RepositoryWithPk.ShouldNotBe(null);
            
            var entity2Repository = LocalIocManager.Resolve<IRepository<MyEntity2, long>>();
            entity2Repository.ShouldNotBe(null);
        }

        public class MyDbContext : AbpDbContext
        {
            public IDbSet<MyEntity1> MyEntities1 { get; set; }

            public DbSet<MyEntity2> MyEntities2 { get; set; }
        }

        public class MyEntity1 : Entity
        {

        }

        public class MyEntity2 : Entity<long>
        {

        }
    }
}
