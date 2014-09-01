using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Abp.Events.Bus;
using Abp.WebApi.Controllers.Filters;
using NUnit.Framework;

namespace Abp.Web.Api.Tests.Controllers.Filters
{
    //TODO: This tests fail because LocalizationSourceManager is not registered. It will be fixed when #58 fixed.
    [TestFixture]
    public class AbpExceptionFilterAttribute_Tests
    {
        [TestFixtureSetUp]
        public void Initialize()
        {
            AbpWebApiTests.Initialize();
        }

        [Test]
        public void ShouldHandleExceptions()
        {
            var exHandler = new MyExceptionHandler();
            EventBus.Default.Register(exHandler);

            var filter = new AbpExceptionFilterAttribute();
            filter.OnException(new HttpActionExecutedContext(new HttpActionContext(), new Exception("my exception message")));

            Assert.NotNull(MyExceptionHandler.LastException);
            Assert.AreEqual("my exception message", MyExceptionHandler.LastException.Message);
        }
    }
}
