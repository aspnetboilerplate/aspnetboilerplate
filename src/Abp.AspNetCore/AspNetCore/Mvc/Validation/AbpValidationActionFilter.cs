using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.Validation
{
    public class AbpValidationActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            //TODO: Use DI!
            new MvcActionInvocationValidator(
                context, 
                context.ActionArguments.Values.ToArray()
                ).Validate();
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }
    }
}
