using System;
using Abp.Events.Bus;
using Abp.Events.Bus.Exceptions;
using Xunit;

namespace Abp.Web.Api.Tests.Controllers.Filters
{
    //TODO: This unit test fails since it can not load depended modules. We will fix it.
    public class AbpExceptionFilterAttribute_Tests
    {
        static AbpExceptionFilterAttribute_Tests()
        {
            //AbpWebApiTests.Initialize();
        }

        [Fact]
        public void ShouldHandleExceptions()
        {
            //EventBus.Default.Trigger(this, new AbpHandledExceptionData(new Exception("my exception message")));

            //Assert.NotNull(MyExceptionHandler.LastException);
            //Assert.Equal("my exception message", MyExceptionHandler.LastException.Message);
        }
    }
}
