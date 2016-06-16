using System.ComponentModel.DataAnnotations;
using Abp.AspNetCore.Mvc.Extensions;
using Abp.Runtime.Validation.Interception;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.Validation
{
    public class MvcActionInvocationValidator : MethodInvocationValidator
    {
        protected ActionExecutingContext ActionContext { get; private set; }
        
        public void Initialize(ActionExecutingContext actionContext, object[] parameterValues)
        {
            base.Initialize(actionContext.ActionDescriptor.GetMethodInfo(), parameterValues);

            ActionContext = actionContext;
        }

        protected override void SetDataAnnotationAttributeErrors(object validatingObject)
        {
            if (ActionContext.ModelState.IsValid)
            {
                return;
            }

            foreach (var state in ActionContext.ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    ValidationErrors.Add(new ValidationResult(error.ErrorMessage, new[] { state.Key }));
                }
            }
        }
    }
}