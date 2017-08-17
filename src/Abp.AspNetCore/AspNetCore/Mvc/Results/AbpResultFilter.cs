using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.Mvc.Extensions;
using Abp.AspNetCore.Mvc.Results.Wrapping;
using Abp.Dependency;
using Abp.Reflection;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.Results
{
    public class AbpResultFilter : IResultFilter, ITransientDependency
    {
        private readonly IAbpAspNetCoreConfiguration _configuration;

        public AbpResultFilter(IAbpAspNetCoreConfiguration configuration)
        {
            _configuration = configuration;
        }

        public virtual void OnResultExecuting(ResultExecutingContext context)
        {
            if (_configuration.SetNoCacheForAjaxResponses && context.HttpContext.Request.IsAjaxRequest())
            {
                SetNoCache(context);
            }

            var methodInfo = context.ActionDescriptor.GetMethodInfo();
            var wrapResultAttribute =
                ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault(
                    methodInfo,
                    _configuration.DefaultWrapResultAttribute
                );

            if (!wrapResultAttribute.WrapOnSuccess)
            {
                return;
            }

            AbpActionResultWrapperFactory
                .CreateFor(context)
                .Wrap(context);
        }

        public virtual void OnResultExecuted(ResultExecutedContext context)
        {
            //no action
        }
        
        protected virtual void SetNoCache(ResultExecutingContext context)
        {
            //Based on http://stackoverflow.com/questions/49547/making-sure-a-web-page-is-not-cached-across-all-browsers
            context.HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate, max-age=0";
            context.HttpContext.Response.Headers["Pragma"] = "no-cache";
            context.HttpContext.Response.Headers["Expires"] = "0";
        }
    }
}
