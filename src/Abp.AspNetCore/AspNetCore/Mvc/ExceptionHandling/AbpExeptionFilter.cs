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
    public class AbpExeptionFilter : IExceptionFilter
    {
        private readonly ILogger _logger;

        public AbpExeptionFilter(ILogger logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var methodInfo = context.ActionDescriptor.GetMethodInfo();
            var wrapResultAttribute =
                ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrNull<WrapResultAttribute>(methodInfo) ??
                WrapResultAttribute.Default;

            if (wrapResultAttribute.LogError)
            {
                LogHelper.LogException(_logger, context.Exception);
            }
            
            if (!wrapResultAttribute.WrapOnError)
            {
                return;
            }

            var exception = context.Exception;

            context.Exception = null; //Handled!

            context.HttpContext.Response.Clear(); 
            context.HttpContext.Response.StatusCode = 500; //TODO: Get from a constant?
            context.Result = new ObjectResult(
                new AjaxResponse(
                    ErrorInfoBuilder.Instance.BuildForException(exception),
                    exception is AbpAuthorizationException
                )
            );

            //TODO: View results vs JSON results
        }
    }
}