using System;
using System.Reflection;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Runtime.Validation.Interception;

namespace Abp.Web.Validation
{
    public abstract class ActionInvocationValidatorBase : MethodInvocationValidator
    {
        protected ActionInvocationValidatorBase(IValidationConfiguration configuration, IIocResolver iocResolver)
            : base(configuration, iocResolver)
        {
        }

        public void Initialize(MethodInfo method)
        {
            base.Initialize(
                method,
                GetParameterValues(method)
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

        protected override void ValidateMethodParameter(ParameterInfo parameterInfo, object parameterValue)
        {
            // If action parameter value is null then set only ModelState errors
            if (parameterValue == null)
            {
                SetDataAnnotationAttributeErrors();
            }
            else
            {
                base.ValidateMethodParameter(parameterInfo, parameterValue);
            }
        }

        protected virtual object[] GetParameterValues(MethodInfo method)
        {
            var parameters = method.GetParameters();
            var parameterValues = new object[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                parameterValues[i] = GetParameterValue(parameters[i].Name);
            }

            return parameterValues;
        }

        protected abstract void SetDataAnnotationAttributeErrors();

        protected abstract object GetParameterValue(string parameterName);
    }
}
