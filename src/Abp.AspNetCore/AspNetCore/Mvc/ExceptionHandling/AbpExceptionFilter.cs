using Abp.AspNetCore.Mvc.Extensions;
using Abp.Authorization;
using Abp.Logging;
using Abp.Reflection;
using Abp.Web.Models;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.ExceptionHandling
{
    public class AbpExceptionFilter : IExceptionFilter
    {
        private readonly ILogger _logger;

        public AbpExceptionFilter(ILoggerFactory loggerFactory)
        {
            //TODO: Constructor injection did not set logger Name correctly. So, we created logger from factory. Test it why.
            _logger = loggerFactory.Create(typeof(AbpExceptionFilter));
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
                LogHelper.LogException(_logger, context.Exception);
            }
            
            if (wrapResultAttribute.WrapOnError)
            {
                HandleAndWrapException(context);
            }
        }

        private static void HandleAndWrapException(ExceptionContext context)
        {
            context.HttpContext.Response.Clear();
            context.HttpContext.Response.StatusCode = 500; //TODO: Get from a constant?
            context.Result = new ObjectResult(
                new AjaxResponse(
                    ErrorInfoBuilder.Instance.BuildForException(context.Exception),
                    context.Exception is AbpAuthorizationException
                )
            );

            context.Exception = null; //Handled!

            //TODO: View results vs JSON results
        }
    }
}