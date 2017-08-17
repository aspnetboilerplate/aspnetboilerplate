using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Web.Http.Controllers;
using Abp.Collections.Extensions;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Runtime.Validation.Interception;

namespace Abp.WebApi.Validation
{
    public class WebApiActionInvocationValidator : MethodInvocationValidator
    {
        protected HttpActionContext ActionContext { get; private set; }

        private bool _isValidatedBefore;

        public WebApiActionInvocationValidator(IValidationConfiguration configuration, IIocResolver iocResolver)
            : base(configuration, iocResolver)
        {

        }

        public void Initialize(HttpActionContext actionContext, MethodInfo methodInfo)
        {
            base.Initialize(
                methodInfo,
                GetParameterValues(actionContext, methodInfo)
            );

            ActionContext = actionContext;
        }

        protected override void SetDataAnnotationAttributeErrors(object validatingObject)
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

        protected virtual object[] GetParameterValues(HttpActionContext actionContext, MethodInfo methodInfo)
        {
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