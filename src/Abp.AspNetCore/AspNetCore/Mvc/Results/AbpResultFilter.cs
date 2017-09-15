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
            if (!context.ActionDescriptor.IsControllerAction())
            {
                return;
            }

            var methodInfo = context.ActionDescriptor.GetMethodInfo();

            //var clientCacheAttribute = ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault(
            //    methodInfo,
            //    _configuration.DefaultClientCacheAttribute
            //);

            //clientCacheAttribute?.Apply(context);
            
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

        public virtual void OnResultExecuted(ResultExecutedContext context)
        {
            //no action
        }
    }
}
