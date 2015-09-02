using System.Linq;
using Abp.Configuration.Startup;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Shouldly;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.ContactLists
{
    public class Messages_MultiTenancy_Tests : SampleApplicationTestBase
    {
        private readonly IRepository<Message> _messageRepository;

        public Messages_MultiTenancy_Tests()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = true;
            _messageRepository = Resolve<IRepository<Message>>();
        }

        [Fact]
        public void MayHaveTenant_Filter_Tests()
        {
            AbpSession.UserId = 1;

            //A tenant can reach to it's own data
            AbpSession.TenantId = 1;
            _messageRepository.Count().ShouldBe(2);
            _messageRepository.GetAllList().Any(m => m.TenantId != AbpSession.TenantId).ShouldBe(false);

            //Tenant 999999 has no data
            AbpSession.TenantId = 999999;
            _messageRepository.Count().ShouldBe(0);

            //Host can reach to it's own data (since MayHaveTenant filter is enabled by default)
            AbpSession.TenantId = null;
            _messageRepository.Count().ShouldBe(1);
            _messageRepository.GetAllList().Any(m => m.TenantId != AbpSession.TenantId).ShouldBe(false);

            var unitOfWorkManager = Resolve<IUnitOfWorkManager>();
            using (var unitOfWork = unitOfWorkManager.Begin())
            {
                //We can also set tenantId parameter's value without changing AbpSession.TenantId
                unitOfWorkManager.Current.SetFilterParameter(AbpDataFilters.MayHaveTenant, AbpDataFilters.Parameters.TenantId, 1);

                //We should only get tenant 1's entities since we set tenantId to 1
                _messageRepository.Count().ShouldBe(2);
                _messageRepository.GetAllList().Any(m => m.TenantId != 1).ShouldBe(false);

                //We can disable the filter to get all entities of host and tenants
                using (unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    _messageRepository.Count().ShouldBe(3);
                }

                unitOfWork.Complete();
            }
        }
    }
}