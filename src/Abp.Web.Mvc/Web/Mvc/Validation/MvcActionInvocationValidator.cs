using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Web.Mvc;
using Abp.Collections.Extensions;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Extensions;
using Abp.Runtime.Validation.Interception;

namespace Abp.Web.Mvc.Validation
{
    public class MvcActionInvocationValidator : MethodInvocationValidator
    {
        protected ActionExecutingContext ActionContext { get; private set; }

        private bool _isValidatedBefore;

        public MvcActionInvocationValidator(IValidationConfiguration configuration, IIocResolver iocResolver) 
            : base(configuration, iocResolver)
        {

        }

        public void Initialize(ActionExecutingContext actionContext, MethodInfo methodInfo)
        {
            base.Initialize(
                methodInfo,
                GetParameterValues(actionContext, methodInfo)
            );

            ActionContext = actionContext;
        }

        protected override void SetDataAnnotationAttributeErrors(object validatingObject)
        {
            if (_isValidatedBefore)
            {
                return;
            }

            _isValidatedBefore = true;

            var modelState = ActionContext.Controller.As<Controller>().ModelState;
            if (modelState.IsValid)
            {
                return;
            }

            foreach (var state in modelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    ValidationErrors.Add(new ValidationResult(error.ErrorMessage, new[] { state.Key }));
                }
            }
        }

        protected virtual object[] GetParameterValues(ActionExecutingContext actionContext, MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();
            var parameterValues = new object[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                parameterValues[i] = actionContext.ActionParameters.GetOrDefault(parameters[i].Name);
            }

            return parameterValues;
        }
    }
}