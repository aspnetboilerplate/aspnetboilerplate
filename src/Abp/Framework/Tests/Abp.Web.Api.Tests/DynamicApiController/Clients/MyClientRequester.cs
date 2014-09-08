using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Dependency;
using Abp.WebApi.Controllers.Dynamic.Clients;
using NUnit.Framework;

namespace Abp.Web.Api.Tests.DynamicApiController.Clients
{
    [TestFixture]
    public class DynamicApiClient_Tests
    {
        [TestFixtureSetUp]
        public void Initialize()
        {
            AbpWebApiTests.Initialize();
        }

        [Test]
        public void Should_Call_Service()
        {
            var myClient = IocHelper.Resolve<IDynamicApiClient<IMyAppService>>();
            var result = myClient.Service.MyMethod(new MyMethodInput {TestProperty1 = "test value"});
            Assert.AreEqual("test value output!", result.TestProperty1);
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
