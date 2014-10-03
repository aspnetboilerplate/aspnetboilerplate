using System.Reflection;
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
            DynamicApiControllerBuilder
                .ForAll<IMyServiceInterface>(Assembly.GetExecutingAssembly(), "myapp/ns1")
                .Build();

            DynamicApiControllerManager.FindOrNull("myapp/ns1/mySample1").ShouldNotBe(null);
            DynamicApiControllerManager.FindOrNull("myapp/ns1/mySample2").ShouldNotBe(null);
            DynamicApiControllerManager.FindOrNull("myapp/ns1/mySampleNotDefined").ShouldBe(null);
        }
    }

    public interface IMyServiceInterface
    {
        
    }

    public abstract class MyServiceBase : IMyServiceInterface
    {
        
    }

    public class MySample1AppService : MyServiceBase
    {
        
    }

    public class MySample2Service : MyServiceBase
    {

    }
}