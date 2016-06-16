using System.ComponentModel.DataAnnotations;
using Abp.AspNetCore.Mvc.Extensions;
using Abp.Runtime.Validation.Interception;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.Validation
{
    internal class MvcActionInvocationValidator : MethodInvocationValidator
    {
        protected readonly ActionExecutingContext ActionContext;

        public MvcActionInvocationValidator(ActionExecutingContext actionContext, object[] parameterValues) 
            : base(actionContext.ActionDescriptor.GetMethodInfo(), parameterValues)
        {
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
                    _validationErrors.Add(new ValidationResult(error.ErrorMessage, new[] { state.Key }));
                }
            }
        }
    }
}