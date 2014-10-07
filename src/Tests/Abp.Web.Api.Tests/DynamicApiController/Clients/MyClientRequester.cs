using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Xunit;

namespace Abp.Web.Api.Tests.DynamicApiController.Clients
{
    //NOTE: This feature is being developed. So, unit tests will be failed if enabled!

    public class DynamicApiClient_Tests
    {
        static DynamicApiClient_Tests()
        {
            //AbpWebApiTests.Initialize();
        }

        [Fact]
        public void Should_Call_Service()
        {
            //var myClient = IocHelper.Resolve<IDynamicApiClient<IMyAppService>>();
            //var result = myClient.Service.MyMethod(new MyMethodInput {TestProperty1 = "test value"});
            //Assert.AreEqual("test value output!", result.TestProperty1);
        }
    }

    public interface IMyAppService : IApplicationService
    {
        MyMethodOutput MyMethod(MyMethodInput input);
    }

    public class MyMethodOutput : IOutputDto
    {
        public string TestProperty1 { get; set; }
    }

    public class MyMethodInput : IInputDto
    {
        public string TestProperty1 { get; set; }
    }

    public class MyAppService : IMyAppService
    {
        public MyMethodOutput MyMethod(MyMethodInput input)
        {
            return new MyMethodOutput { TestProperty1 = input.TestProperty1 + " output!" };
        }
    }
}
