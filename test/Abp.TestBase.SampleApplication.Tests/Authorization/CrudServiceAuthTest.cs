using Abp.TestBase.SampleApplication.Crm;
using Shouldly;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.Authorization
{
    public class CrudServiceAuthTest : SampleApplicationTestBase
    {
        public CrudServiceAuthTest()
        {

        }

        [Fact]
        public void Should_Resolve_CrudAppService_With_AbpAuthorize_Attribute()
        {
            var companyAppService = LocalIocManager.Resolve<AsyncTestCompanyAppService>();
            companyAppService.ShouldNotBeNull();
        }
    }
}
