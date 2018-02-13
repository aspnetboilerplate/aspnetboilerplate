using System;
using System.ComponentModel.DataAnnotations;
using Abp.AspNetCore.Mvc.Extensions;
using Abp.Collections.Extensions;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Runtime.Validation.Interception;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.Validation
{
    public class MvcActionInvocationValidator : MethodInvocationValidator
    {
        protected ActionExecutingContext ActionContext { get; private set; }

        private bool _isValidatedBefore;

        public MvcActionInvocationValidator(IValidationConfiguration configuration, IIocResolver iocResolver)
            : base(configuration, iocResolver)
        {

        }

        public void Initialize(ActionExecutingContext actionContext)
        {
            ActionContext = actionContext;

            base.Initialize(
                actionContext.ActionDescriptor.GetMethodInfo(),
                GetParameterValues(actionContext)
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
            if (_isValidatedBefore || ActionContext.ModelState.IsValid)
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

            _isValidatedBefore = true;
        }

        protected virtual object[] GetParameterValues(ActionExecutingContext actionContext)
        {
            var methodInfo = actionContext.ActionDescriptor.GetMethodInfo();

            var parameters = methodInfo.GetParameters();
            var parameterValues = new object[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                parameterValues[i] = actionContext.ActionArguments.GetOrDefault(parameters[i].Name);
            }

            return parameterValues;
        }
    }
}