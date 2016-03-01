using System;
using Adorable.Dependency;
using Adorable.Events.Bus.Exceptions;
using Adorable.Events.Bus.Handlers;

namespace Adorable.Web.Api.Tests.Controllers.Filters
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