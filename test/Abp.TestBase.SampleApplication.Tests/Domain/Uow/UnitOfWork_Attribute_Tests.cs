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
            service.DoIt();
        }
    }

    public interface IMyTestDomainService : IDomainService
    {
        void DoIt();
    }

    public class MyTestDomainService : DomainService, IMyTestDomainService
    {
        [UnitOfWork]
        public void DoIt()
        {
            CurrentUnitOfWork.ShouldNotBeNull();
        }
    }
}
