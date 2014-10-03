using Abp.Application.Services;
using Abp.WebApi.Controllers.Dynamic.Builders;
using Shouldly;
using Xunit;

namespace Abp.Web.Api.Tests.DynamicApiController
{
    public class DynamicApiControllerActionFinder_Tests
    {
        [Fact]
        public void Should_Find_Right_Methods()
        {
            var methods = DynamicApiControllerActionFinder.GetMethodsToBeAction(typeof(MyApplicationService));
            methods.Count.ShouldBe(3);
        }

        private class MyApplicationService : ApplicationService
        {
            public void MyMethod1()
            {
                
            }

            public int MyMethod2()
            {
                return 42;
            }

            public void MyMethod3(string arg1)
            {
                
            }
        }
    }
}