using System.Reflection;
using Abp.Application.Services;
using Abp.Dependency;
using Abp.WebApi.Controllers.Dynamic;
using Abp.WebApi.Controllers.Dynamic.Builders;
using Shouldly;
using Xunit;

namespace Abp.Web.Api.Tests.DynamicApiController.BatchBuilding
{
    public class BatchDynamicApiControllerBuilder_Test
    {
        [Fact]
        public void Test1()
        {
            //TODO: This test fails since it use static IocManager. We should use a local IocManager.
            //IocManager.Instance.Register<IMyFirstAppService, MyFirstAppService>();

            //DynamicApiControllerBuilder
            //    .ForAll<IApplicationService>(Assembly.GetExecutingAssembly(), "myapp/ns1")
            //    .Build();

            //DynamicApiControllerManager.GetAll().Count.ShouldBe(1);
            //DynamicApiControllerManager.FindOrNull("myapp/ns1/myFirst").ShouldNotBe(null);
        }
    }

    public interface IMyFirstAppService : IApplicationService
    {
        
    }

    public abstract class MyFirstAppService : IMyFirstAppService
    {
        
    }
}