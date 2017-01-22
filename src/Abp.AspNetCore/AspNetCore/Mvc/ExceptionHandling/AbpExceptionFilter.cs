using System.Net;
using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.Mvc.Extensions;
using Abp.AspNetCore.Mvc.Results;
using Abp.Authorization;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Events.Bus;
using Abp.Events.Bus.Exceptions;
using Abp.Logging;
using Abp.Reflection;
using Abp.Web.Models;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.ExceptionHandling
{
    public class AbpExceptionFilter : IExceptionFilter, ITransientDependency
    {
        public ILogger Logger { get; set; }

        public IEventBus EventBus { get; set; }

        private readonly IErrorInfoBuilder _errorInfoBuilder;
        private readonly IAbpAspNetCoreConfiguration _configuration;

        public AbpExceptionFilter(IErrorInfoBuilder errorInfoBuilder, IAbpAspNetCoreConfiguration configuration)
        {
            _errorInfoBuilder = errorInfoBuilder;
            _configuration = configuration;

            Logger = NullLogger.Instance;
            EventBus = NullEventBus.Instance;
        }

        public void OnException(ExceptionContext context)
        {
            var wrapResultAttribute =
                ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault(
                    context.ActionDescriptor.GetMethodInfo(),
                    _configuration.DefaultWrapResultAttribute
                );

            if (wrapResultAttribute.LogError)
            {
                LogHelper.LogException(Logger, context.Exception);
            }

            if (wrapResultAttribute.WrapOnError)
            {
                HandleAndWrapException(context);
            }
        }

        private void HandleAndWrapException(ExceptionContext context)
        {
            if (!ActionResultHelper.IsObjectResult(context.ActionDescriptor.GetMethodInfo().ReturnType))
            {
                return;
            }

            context.HttpContext.Response.StatusCode = GetStatusCode(context);

            context.Result = new ObjectResult(
                new AjaxResponse(
                    _errorInfoBuilder.BuildForException(context.Exception),
                    context.Exception is AbpAuthorizationException
                )
            );

            EventBus.Trigger(this, new AbpHandledExceptionData(context.Exception));

            context.Exception = null; //Handled!
        }

        private int GetStatusCode(ExceptionContext context)
        {
            if (context.Exception is AbpAuthorizationException)
            {
                return context.HttpContext.User.Identity.IsAuthenticated
                    ? (int)HttpStatusCode.Forbidden
                    : (int)HttpStatusCode.Unauthorized;
            }

            if (context.Exception is EntityNotFoundException)
            {
                return (int)HttpStatusCode.NotFound;
            }

            return (int)HttpStatusCode.InternalServerError;
        }
    }
}
