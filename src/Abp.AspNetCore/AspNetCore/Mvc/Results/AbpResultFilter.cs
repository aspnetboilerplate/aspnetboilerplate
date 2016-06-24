using Abp.AspNetCore.Mvc.Extensions;
using Abp.AspNetCore.Mvc.Results.Wrapping;
using Abp.Dependency;
using Abp.Reflection;
using Abp.Web.Models;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.Results
{
    public class AbpResultFilter : IResultFilter, ITransientDependency
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            var methodInfo = context.ActionDescriptor.GetMethodInfo();
            var wrapResultAttribute =
                ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<WrapResultAttribute>(
                    methodInfo,
                    WrapResultAttribute.Default
                );

            if (!wrapResultAttribute.WrapOnSuccess)
            {
                return;
            }

            AbpActionResultWrapperFactory
                .CreateFor(context.Result)
                .Wrap(context.Result);
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            //no action
        }
    }
}
