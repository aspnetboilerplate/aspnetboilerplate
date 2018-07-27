using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFramework.Repositories;
using Abp.MultiTenancy;
using Abp.Tests;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Abp.EntityFramework.Tests
{
    public class DbContextTypeMatcher_Tests : TestBaseWithLocalIocManager
    {
        private int? _tenantId = 1;

        private readonly DbContextTypeMatcher matcher;

        public DbContextTypeMatcher_Tests()
        {
            var fakeUow = Substitute.For<IUnitOfWork>();
            fakeUow.GetTenantId().Returns(callInfo => _tenantId);
            var fakeCurrentUowProvider = Substitute.For<ICurrentUnitOfWorkProvider>();
            fakeCurrentUowProvider.Current.Returns(fakeUow);

            matcher = new DbContextTypeMatcher(fakeCurrentUowProvider);
            matcher.Populate(new []
            {
                typeof(MyDerivedDbContext1),
                typeof(MyDerivedDbContext2),
                typeof(MyDerivedDbContext3),
                typeof(MyDerivedDbContext4)
            });
        }

        [Fact]
        public void Should_Get_Same_Types_For_Defined_Non_Abstract_Types()
        {
            matcher.GetConcreteType(typeof(MyDerivedDbContext1)).ShouldBe(typeof(MyDerivedDbContext1));
            matcher.GetConcreteType(typeof(MyDerivedDbContext2)).ShouldBe(typeof(MyDerivedDbContext2));
            matcher.GetConcreteType(typeof(MyDerivedDbContext3)).ShouldBe(typeof(MyDerivedDbContext3));
            matcher.GetConcreteType(typeof(MyDerivedDbContext4)).ShouldBe(typeof(MyDerivedDbContext4));
        }

        [Fact]
        public void Should_Get_Same_Types_For_Undefined_Non_Abstract_Types()
        {
            matcher.GetConcreteType(typeof(MyDerivedDbContextNotDefined)).ShouldBe(typeof(MyDerivedDbContextNotDefined));
        }

        [Fact]
        public void Should_Get_Single_DbContext_For_Current_Tenancy_Side_When_BaseDbContext_Requested()
        {
            //Should return MyDerivedDbContext3 since it defines MultiTenancySides.Tenant
            //and DefaultDbContext
            matcher.GetConcreteType(typeof(MyCommonDbContext)).ShouldBe(typeof(MyDerivedDbContext3));
        }

        [Fact]
        public void Should_Throw_Exception_If_Multiple_DbContext_For_Current_Tenancy_Side_When_BaseDbContext_Requested()
        {
            _tenantId = null; //switching to host side (which have more than 1 dbcontext)
            matcher.GetConcreteType(typeof(MyCommonDbContext)).ShouldBe(typeof(MyDerivedDbContext1));
        }

        private abstract class MyCommonDbContext : AbpDbContext
        {

        }

        [MultiTenancySide(MultiTenancySides.Host)]
        private class MyDerivedDbContext1 : MyCommonDbContext
        {

        }

        [AutoRepositoryTypes( //Does not matter parameters for these tests
            typeof(IRepository<>), 
            typeof(IRepository<,>), 
            typeof(EfRepositoryBase<,>), 
            typeof(EfRepositoryBase<,,>)
            )]
        [MultiTenancySide(MultiTenancySides.Host)]
        private class MyDerivedDbContext2 : MyCommonDbContext
        {

        }

        [DefaultDbContext]
        [MultiTenancySide(MultiTenancySides.Tenant)]
        private class MyDerivedDbContext3 : MyCommonDbContext
        {

        }

        [MultiTenancySide(MultiTenancySides.Tenant)]
        private class MyDerivedDbContext4 : MyCommonDbContext
        {

        }

        private class MyDerivedDbContextNotDefined : MyCommonDbContext
        {

        }

        private class MyCommonEntity : Entity
        {

        }
    }
}
