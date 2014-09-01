using System;
using Abp.Events.Bus.Exceptions;
using Abp.Events.Bus.Handlers;

namespace Abp.Web.Api.Tests.Controllers.Filters
{
    public class MyExceptionHandler : IEventHandler<AbpHandledExceptionData>
    {
        public static Exception LastException { get; private set; }

        public void HandleEvent(AbpHandledExceptionData eventData)
        {
            LastException = eventData.Exception;
        }
    }
}