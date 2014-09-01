using System;
using Abp.Dependency;
using Abp.Events.Bus.Exceptions;
using Abp.Events.Bus.Handlers;

namespace Abp.Web.Api.Tests.Controllers.Filters
{
    public class MyExceptionHandler : IEventHandler<AbpHandledExceptionData>, ITransientDependency
    {
        public static Exception LastException { get; private set; }

        public void HandleEvent(AbpHandledExceptionData eventData)
        {
            LastException = eventData.Exception;
        }
    }
}