using System.Linq;
using Abp.Configuration.Startup;
using Abp.Domain.Repositories;
using Shouldly;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.ContactLists
{
    public class ContactList_MultiTenancy_Tests : SampleApplicationTestBase
    {
        private readonly IRepository<ContactList> _contactListRepository;

        public ContactList_MultiTenancy_Tests()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = true;
            _contactListRepository = Resolve<IRepository<ContactList>>();
        }

        [Fact]
        public void Should_Filter_ContactLists_Of_Other_Tenants()
        {
            AbpSession.TenantId = 1;
            _contactListRepository.GetAllList().Any(cl => cl.TenantId != AbpSession.TenantId).ShouldBe(false);
            
            AbpSession.TenantId = 2;
            _contactListRepository.GetAllList().Any(cl => cl.TenantId != AbpSession.TenantId).ShouldBe(false);
            
            AbpSession.TenantId = 999999;
            _contactListRepository.GetAllList().Count.ShouldBe(0);

            AbpSession.TenantId = null;
            _contactListRepository.GetAllList().Count.ShouldBe(0);
        }
    }
}
