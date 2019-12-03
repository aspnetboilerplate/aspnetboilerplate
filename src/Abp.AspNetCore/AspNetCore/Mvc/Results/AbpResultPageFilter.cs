using System.Net;
using System.Threading.Tasks;
using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.Mvc.Results.Wrapping;
using Abp.Authorization;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Events.Bus;
using Abp.Events.Bus.Exceptions;
using Abp.Logging;
using Abp.Reflection;
using Abp.Runtime.Validation;
using Abp.Web.Models;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.Results
{
    public class AbpResultPageFilter : IAsyncPageFilter, ITransientDependency
    {
        private readonly IAbpAspNetCoreConfiguration _configuration;
        private readonly IAbpActionResultWrapperFactory _actionResultWrapperFactory;

        public AbpResultPageFilter(IAbpAspNetCoreConfiguration configuration, IAbpActionResultWrapperFactory actionResultWrapperFactory)
        {
            _configuration = configuration;
            _actionResultWrapperFactory = actionResultWrapperFactory;
        }

        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            return Task.CompletedTask;
        }

        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            if (context.HandlerMethod == null)
            {
                await next();
                return;
            }

            var pageHandlerExecutedContext = await next();

            var methodInfo = context.HandlerMethod.MethodInfo;

            var wrapResultAttribute =
                ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault(
                    methodInfo,
                    _configuration.DefaultWrapResultAttribute
                );

            if (!wrapResultAttribute.WrapOnSuccess)
            {
                return;
            }

            _actionResultWrapperFactory.CreateFor(pageHandlerExecutedContext).Wrap(pageHandlerExecutedContext);
        }

    }
}

