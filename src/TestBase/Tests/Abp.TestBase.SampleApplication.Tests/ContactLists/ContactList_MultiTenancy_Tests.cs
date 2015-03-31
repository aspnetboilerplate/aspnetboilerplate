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
        public void MustHaveTenant_Filter_Tests()
        {
            //A tenant can reach to it's own data
            AbpSession.TenantId = 1;
            _contactListRepository.GetAllList().Any(cl => cl.TenantId != AbpSession.TenantId).ShouldBe(false);
            
            //A tenant can reach to it's own data
            AbpSession.TenantId = 2;
            _contactListRepository.GetAllList().Any(cl => cl.TenantId != AbpSession.TenantId).ShouldBe(false);

            //Tenant 999999 has no data
            AbpSession.TenantId = 999999;
            _contactListRepository.GetAllList().Count.ShouldBe(0);

            //Host can reach to all tenant data
            AbpSession.TenantId = null;
            _contactListRepository.GetAllList().Count.ShouldBe(2);

            //Host can filter tenant data if it wants
            _contactListRepository.GetAllList().Count(t => t.TenantId == 1).ShouldBe(1);
            _contactListRepository.GetAllList().Count(t => t.TenantId == 2).ShouldBe(1);
            _contactListRepository.GetAllList().Count(t => t.TenantId == 999999).ShouldBe(0);
        }
    }
}
