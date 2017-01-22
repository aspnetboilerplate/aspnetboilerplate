using Abp.TestBase;
using Abp.WebApi.Controllers.Dynamic;
using Shouldly;
using Xunit;

namespace Abp.Web.Api.Tests.DynamicApiController.BatchBuilding
{
    public class BatchDynamicApiControllerBuilder_Test : AbpIntegratedTestBase<AbpWebApiTestModule>
    {
        [Fact]
        public void Test1()
        {
            lock (this)
            {
                var services = LocalIocManager.Resolve<DynamicApiControllerManager>().GetAll();
                services.Count.ShouldBe(1);
                services[0].ServiceName.ShouldBe("myapp/myFirst");
                services[0].Actions.Count.ShouldBe(1);
                services[0].Actions.ContainsKey("GetStr").ShouldBe(true);
            }
        }
    }
}