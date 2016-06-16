using Abp.Dependency;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.Validation
{
    public class AbpValidationActionFilter : IActionFilter, ITransientDependency
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
                validator.Object.Initialize(context);
                validator.Object.Validate();
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }
    }
}
