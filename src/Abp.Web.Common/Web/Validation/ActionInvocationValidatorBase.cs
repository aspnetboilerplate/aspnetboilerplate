using System;
using System.Collections.Generic;
using System.Reflection;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Runtime.Validation.Interception;

namespace Abp.Web.Validation
{
    public abstract class ActionInvocationValidatorBase : MethodInvocationValidator
    {
        private bool IsSetDataAnnotationAttributeErrors { get; set; }

        protected IList<Type> ValidatorsToSkip => new List<Type>
        {
            typeof(DataAnnotationsValidator),
            typeof(ValidatableObjectValidator)
        };

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
            // Skip DataAnnotations and IValidatableObject validation because MVC does this automatically
            if (ValidatorsToSkip.Contains(validatorType))
            {
                return false;
            }

            return base.ShouldValidateUsingValidator(validatingObject, validatorType);
        }

        protected override void ValidateMethodParameter(ParameterInfo parameterInfo, object parameterValue)
        {
            // If action parameter value is null then set only ModelState errors
            if (parameterValue != null)
            {
                base.ValidateMethodParameter(parameterInfo, parameterValue);
            }

            if (!IsSetDataAnnotationAttributeErrors)
            {
                SetDataAnnotationAttributeErrors();
                IsSetDataAnnotationAttributeErrors = true;
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
