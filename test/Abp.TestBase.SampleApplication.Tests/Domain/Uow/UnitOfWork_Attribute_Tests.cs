using Abp.Domain.Services;
using Abp.Domain.Uow;
using Shouldly;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.Domain.Uow
{
    public class UnitOfWork_Attribute_Tests: SampleApplicationTestBase
    {
        [Fact]
        public void UnitOfWork_Attribute_Should_Work_Without_Virtual_When_Resolved_By_Interface()
        {
            var service = Resolve<IMyTestDomainService>();
            service.DoIt<string>();
        }

        [Fact]
        public void UnitOfWork_Attribute_Should_Work_With_Virtual_When_Resolved_By_Class()
        {
            var service = Resolve<MyTestDomainService>();
            service.DoIt2<string>();
        }
    }

    public interface IMyTestDomainService : IDomainService
    {
        void DoIt<T>();
    }

    public class MyTestDomainService : DomainService, IMyTestDomainService
    {
        [UnitOfWork]
        public void DoIt<T>()
        {
            CurrentUnitOfWork.ShouldNotBeNull();
        }

        [UnitOfWork]
        public virtual void DoIt2<T>()
        {
            CurrentUnitOfWork.ShouldNotBeNull();
        }
    }
}
