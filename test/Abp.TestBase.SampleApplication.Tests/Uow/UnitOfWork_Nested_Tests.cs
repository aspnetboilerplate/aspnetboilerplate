using System.Transactions;
using Abp.Configuration.Startup;
using Abp.Domain.Uow;
using Shouldly;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.Uow
{
    public class UnitOfWork_Nested_Tests : SampleApplicationTestBase
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public UnitOfWork_Nested_Tests()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = true;
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();
        }

        [Fact]
        public void Should_Copy_Filters_To_Nested_Uow()
        {
            AbpSession.TenantId.ShouldBe(null);

            using (var outerUow = _unitOfWorkManager.Begin())
            {
                _unitOfWorkManager.Current.GetTenantId().ShouldBe(null);

                using (_unitOfWorkManager.Current.SetTenantId(1))
                {
                    _unitOfWorkManager.Current.GetTenantId().ShouldBe(1);

                    using (var nestedUow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
                    {
                        AbpSession.TenantId.ShouldBe(null);
                        _unitOfWorkManager.Current.GetTenantId().ShouldBe(1); //Because nested transaction copies outer uow's filters.

                        nestedUow.Complete();
                    }

                    _unitOfWorkManager.Current.GetTenantId().ShouldBe(1);
                }

                _unitOfWorkManager.Current.GetTenantId().ShouldBe(null);

                outerUow.Complete();
            }
        }
    }
}
