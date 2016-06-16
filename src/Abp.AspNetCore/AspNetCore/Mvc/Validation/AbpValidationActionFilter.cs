using System.Linq;
using Abp.Dependency;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.Validation
{
    public class AbpValidationActionFilter : IActionFilter
    {
        private readonly IIocResolver _iocResolver;

        public AbpValidationActionFilter(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            using (var validator = _iocResolver.ResolveAsDisposable<MvcActionInvocationValidator>())
            {
                validator.Object.Initialize(context, context.ActionArguments.Values.ToArray());
                validator.Object.Validate();
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }
    }
}
