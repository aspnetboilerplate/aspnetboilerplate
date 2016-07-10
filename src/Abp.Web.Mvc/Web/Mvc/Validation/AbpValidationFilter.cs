using System.Web.Mvc;
using Abp.Dependency;
using Abp.Web.Mvc.Extensions;

namespace Abp.Web.Mvc.Validation
{
    public class AbpValidationFilter : IActionFilter, ITransientDependency
    {
        private readonly IIocResolver _iocResolver;

        public AbpValidationFilter(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var methodInfo = filterContext.ActionDescriptor.GetMethodInfoOrNull();
            if (methodInfo == null)
            {
                return;
            }

            using (var validator = _iocResolver.ResolveAsDisposable<MvcActionInvocationValidator>())
            {
                validator.Object.Initialize(filterContext, methodInfo);
                validator.Object.Validate();
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            
        }
    }
}
