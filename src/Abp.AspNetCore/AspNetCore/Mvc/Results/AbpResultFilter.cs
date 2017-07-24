using System;
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
        private readonly IAbpActionResultWrapperFactory _actionResultWrapperFactory;

        public AbpResultFilter(IAbpAspNetCoreConfiguration configuration, 
            IAbpActionResultWrapperFactory actionResultWrapper)
        {
            _configuration = configuration;
            _actionResultWrapperFactory = actionResultWrapper;
        }

        public virtual void OnResultExecuting(ResultExecutingContext context)
        {
            var methodInfo = context.ActionDescriptor.GetMethodInfo();

            var cacheResultAttribute =
            ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault(
                methodInfo,
                _configuration.DefaultCacheResultAttribute
            );

            if (!cacheResultAttribute.NoCache)
            {
                SetCache(context,
                    cacheResultAttribute.MustRevalidate,
                    cacheResultAttribute.PrivateOnly,
                    cacheResultAttribute.MaxAge);
            }
            else if (cacheResultAttribute.NoCache || 
                (_configuration.SetNoCacheForAjaxResponses && context.HttpContext.Request.IsAjaxRequest()))
            {
                SetNoCache(context);
            }

            var wrapResultAttribute =
                ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault(
                    methodInfo,
                    _configuration.DefaultWrapResultAttribute
                );

            if (!wrapResultAttribute.WrapOnSuccess)
            {
                return;
            }

            _actionResultWrapperFactory.CreateFor(context).Wrap(context);
        }

        private void SetCache(ResultExecutingContext context, bool mustRevalidate, bool privateOnly, int maxAge)
        {
            if (maxAge > 0)
            {
                context.HttpContext.Response.Headers["Cache-Control"] =
                    (privateOnly ? "private, " : "public, ") +
                    (mustRevalidate ? "must-revalidate, " : "") +
                    ("max-age=" + maxAge);
            }
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
