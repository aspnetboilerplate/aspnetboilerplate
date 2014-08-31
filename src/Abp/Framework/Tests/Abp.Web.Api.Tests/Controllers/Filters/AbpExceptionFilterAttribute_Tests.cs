using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Abp.Events.Bus;
using Abp.Events.Bus.Exceptions;
using Abp.Events.Bus.Handlers;
using Abp.WebApi.Controllers.Filters;
using NUnit.Framework;

namespace Abp.Web.Api.Tests.Controllers.Filters
{
    //TODO: This tests fail because LocalizationSourceManager is not registered. It will be fixed when #58 fixed.
    [TestFixture]
    public class AbpExceptionFilterAttribute_Tests
    {
        [Test]
        public void ShouldHandleExceptions()
        {
            var exHandler = new MyExceptionHandler();
            EventBus.Default.Register(exHandler);

            var filter = new AbpExceptionFilterAttribute();
            filter.OnException(new HttpActionExecutedContext(new HttpActionContext(), new Exception("my exception message")));

            Assert.NotNull(exHandler.Exception);
            Assert.AreEqual("my exception message", exHandler.Exception.Message);
        }
    }

    public class MyExceptionHandler : IEventHandler<AbpHandledExceptionData>
    {
        public Exception Exception { get; private set; }

        public void HandleEvent(AbpHandledExceptionData eventData)
        {
            Exception = eventData.Exception;
        }
    }
}
