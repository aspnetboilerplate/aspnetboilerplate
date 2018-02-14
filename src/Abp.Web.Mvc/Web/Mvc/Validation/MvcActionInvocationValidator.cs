using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Web.Mvc;
using Abp.Collections.Extensions;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Extensions;
using Abp.Web.Validation;

namespace Abp.Web.Mvc.Validation
{
    public class MvcActionInvocationValidator : ActionInvocationValidatorBase
    {
        protected ActionExecutingContext ActionContext { get; private set; }

        public MvcActionInvocationValidator(IValidationConfiguration configuration, IIocResolver iocResolver) 
            : base(configuration, iocResolver)
        {
        }

        public void Initialize(ActionExecutingContext actionContext, MethodInfo methodInfo)
        {
            ActionContext = actionContext;

            base.Initialize(methodInfo);
        }

        protected override void SetDataAnnotationAttributeErrors()
        {
            var modelState = ActionContext.Controller.As<Controller>().ModelState;

            foreach (var state in modelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    ValidationErrors.Add(new ValidationResult(error.ErrorMessage, new[] { state.Key }));
                }
            }
        }

        protected override object GetParameterValue(string parameterName)
        {
            return ActionContext.ActionParameters.GetOrDefault(parameterName);
        }
    }
}