using System;
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
            ActionContext = actionContext;

            base.Initialize(
                methodInfo,
                GetParameterValues(actionContext, methodInfo)
            );
        }

        protected override bool ShouldValidateUsingValidator(object validatingObject, Type validatorType)
        {
            // Skip data annotations validation because MVC does this automatically
            if (validatorType == typeof(DataAnnotationsValidator))
            {
                return false;
            }

            return base.ShouldValidateUsingValidator(validatingObject, validatorType);
        }

        protected override void SetValidationErrors(object validatingObject)
        {
            base.SetValidationErrors(validatingObject);

            SetDataAnnotationAttributeErrors();
        }

        protected virtual void SetDataAnnotationAttributeErrors()
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
                    ValidationErrors.Add(new ValidationResult(error.ErrorMessage, new[] {state.Key}));
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