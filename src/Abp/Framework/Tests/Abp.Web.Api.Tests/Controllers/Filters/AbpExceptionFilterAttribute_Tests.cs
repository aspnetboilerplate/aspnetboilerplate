using System;
using Abp.Events.Bus;
using Abp.Events.Bus.Exceptions;
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
            EventBus.Default.Trigger(this, new AbpHandledExceptionData(new Exception("my exception message")));

            Assert.NotNull(MyExceptionHandler.LastException);
            Assert.AreEqual("my exception message", MyExceptionHandler.LastException.Message);
        }
    }
}
