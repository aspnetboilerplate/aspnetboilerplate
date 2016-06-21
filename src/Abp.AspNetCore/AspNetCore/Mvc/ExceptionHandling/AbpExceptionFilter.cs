using System.Net;
using Abp.AspNetCore.Mvc.Extensions;
using Abp.Authorization;
using Abp.Dependency;
using Abp.Logging;
using Abp.Reflection;
using Abp.Web.Models;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.ExceptionHandling
{
    public class AbpExceptionFilter : IExceptionFilter, ITransientDependency
    {
        public ILogger Logger { get; set; }

        public AbpExceptionFilter()
        {
            Logger = NullLogger.Instance;
        }

        public void OnException(ExceptionContext context)
        {
            var wrapResultAttribute =
                ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<WrapResultAttribute>(
                    context.ActionDescriptor.GetMethodInfo(),
                    WrapResultAttribute.Default
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

        private static void HandleAndWrapException(ExceptionContext context)
        {
            if (typeof(ActionResult).IsAssignableFrom(context.ActionDescriptor.GetMethodInfo().ReturnType))
            {
                return;
            }
            
            context.HttpContext.Response.Clear();
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Result = new ObjectResult(
                new AjaxResponse(
                    ErrorInfoBuilder.Instance.BuildForException(context.Exception),
                    context.Exception is AbpAuthorizationException
                )
            );

            context.Exception = null; //Handled!
        }
    }
}