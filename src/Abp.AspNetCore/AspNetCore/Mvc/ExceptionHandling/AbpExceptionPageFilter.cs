using System.Net;
using System.Threading.Tasks;
using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.Mvc.Results;
using Abp.Authorization;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Events.Bus;
using Abp.Events.Bus.Exceptions;
using Abp.Logging;
using Abp.Reflection;
using Abp.Runtime;
using Abp.Runtime.Validation;
using Abp.Web.Models;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.ExceptionHandling
{
    public class AbpExceptionPageFilter : IAsyncPageFilter, ITransientDependency
    {
        public ILogger Logger { get; set; }

        public IEventBus EventBus { get; set; }

        private readonly IErrorInfoBuilder _errorInfoBuilder;
        private readonly IAbpAspNetCoreConfiguration _configuration;

        public AbpExceptionPageFilter(IErrorInfoBuilder errorInfoBuilder, IAbpAspNetCoreConfiguration configuration)
        {
            _errorInfoBuilder = errorInfoBuilder;
            _configuration = configuration;

            Logger = NullLogger.Instance;
            EventBus = NullEventBus.Instance;
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

            if (pageHandlerExecutedContext.Exception == null)
            {
                return;;
            }

            var wrapResultAttribute =
                ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault(
                    context.HandlerMethod.MethodInfo,
                    _configuration.DefaultWrapResultAttribute
                );

            if (wrapResultAttribute.LogError)
            {
                LogHelper.LogException(Logger, pageHandlerExecutedContext.Exception);
            }

            HandleAndWrapException(pageHandlerExecutedContext, wrapResultAttribute);
        }

        protected virtual void HandleAndWrapException(PageHandlerExecutedContext context, WrapResultAttribute wrapResultAttribute)
        {
            if (!ActionResultHelper.IsObjectResult(context.HandlerMethod.MethodInfo.ReturnType))
            {
                return;
            }

            context.HttpContext.Response.StatusCode = GetStatusCode(context, wrapResultAttribute.WrapOnError);

            if (!wrapResultAttribute.WrapOnError)
            {
                return;
            }

            context.Result = new ObjectResult(
                new AjaxResponse(
                    _errorInfoBuilder.BuildForException(context.Exception),
                    context.Exception is AbpAuthorizationException
                )
            );

            EventBus.Trigger(this, new AbpHandledExceptionData(context.Exception));

            context.Exception = null; //Handled!
        }

        protected virtual int GetStatusCode(PageHandlerExecutedContext context, bool wrapOnError)
        {
            if (context.Exception is AbpAuthorizationException)
            {
                return context.HttpContext.User.Identity.IsAuthenticated
                    ? (int)HttpStatusCode.Forbidden
                    : (int)HttpStatusCode.Unauthorized;
            }

            if (context.Exception is AbpValidationException)
            {
                return (int)HttpStatusCode.BadRequest;
            }

            if (context.Exception is EntityNotFoundException)
            {
                return (int)HttpStatusCode.NotFound;
            }

            if (wrapOnError)
            {
                return (int)HttpStatusCode.InternalServerError;
            }

            return context.HttpContext.Response.StatusCode;
        }
    }
}
