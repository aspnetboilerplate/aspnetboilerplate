using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Abp.Collections.Extensions;
using Abp.Reflection;

namespace Abp.Runtime.Validation.Interception
{
    /// <summary>
    /// This class is used to validate a method call (invocation) for method arguments.
    /// </summary>
    internal class MethodInvocationValidator
    {
        protected readonly MethodInfo _method;
        protected readonly object[] _parameterValues;
        protected readonly ParameterInfo[] _parameters;
        protected readonly List<ValidationResult> _validationErrors;

        /// <summary>
        /// Creates a new <see cref="MethodInvocationValidator"/> instance.
        /// </summary>
        /// <param name="method">Method to be validated</param>
        /// <param name="parameterValues">List of arguments those are used to call the <paramref name="method"/>.</param>
        public MethodInvocationValidator(MethodInfo method, object[] parameterValues)
        {
            _method = method;
            _parameterValues = parameterValues;
            _parameters = method.GetParameters();
            _validationErrors = new List<ValidationResult>();
        }

        /// <summary>
        /// Validates the method invocation.
        /// </summary>
        public void Validate()
        {
            if (!_method.IsPublic)
            {
                return;
            }

            if (IsValidationDisabled(_method))
            {
                return;                
            }

            if (_parameters.IsNullOrEmpty())
            {
                return;
            }

            if (_parameters.Length != _parameterValues.Length)
            {
                throw new Exception("Method parameter count does not match with argument count!");
            }

            for (var i = 0; i < _parameters.Length; i++)
            {
                ValidateMethodParameter(_parameters[i], _parameterValues[i]);
            }

            if (_validationErrors.Any())
            {
                throw new AbpValidationException(
                    "Method arguments are not valid! See ValidationErrors for details.",
                    _validationErrors
                    );
            }

            foreach (var parameterValue in _parameterValues)
            {
                NormalizeParameter(parameterValue);
            }
        }

        protected virtual bool IsValidationDisabled(MethodInfo methodInfo)
        {
            if (methodInfo.IsDefined(typeof(EnableValidationAttribute), true))
            {
                return false;
            }

            return ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<DisableValidationAttribute>(methodInfo) != null;
        }

        /// <summary>
        /// Validates given parameter for given value.
        /// </summary>
        /// <param name="parameterInfo">Parameter of the method to validate</param>
        /// <param name="parameterValue">Value to validate</param>
        protected virtual void ValidateMethodParameter(ParameterInfo parameterInfo, object parameterValue)
        {
            if (parameterValue == null)
            {
                if (!parameterInfo.IsOptional && !parameterInfo.IsOut && !TypeHelper.IsPrimitiveExtendedIncludingNullable(parameterInfo.ParameterType))
                {
                    _validationErrors.Add(new ValidationResult(parameterInfo.Name + " is null!", new[] { parameterInfo.Name }));
                }

                return;
            }

            ValidateObjectRecursively(parameterValue);
        }

        protected virtual void ValidateObjectRecursively(object validatingObject)
        {
            if (validatingObject is IEnumerable && !(validatingObject is IQueryable))
            {
                foreach (var item in (validatingObject as IEnumerable))
                {
                    ValidateObjectRecursively(item);
                }
            }

            if (!(validatingObject is IValidate))
            {
                return;
            }

            SetDataAnnotationAttributeErrors(validatingObject);

            if (validatingObject is ICustomValidate)
            {
                (validatingObject as ICustomValidate).AddValidationErrors(_validationErrors);
            }

            var properties = TypeDescriptor.GetProperties(validatingObject).Cast<PropertyDescriptor>();
            foreach (var property in properties)
            {
                ValidateObjectRecursively(property.GetValue(validatingObject));
            }
        }

        /// <summary>
        /// Checks all properties for DataAnnotations attributes.
        /// </summary>
        protected virtual void SetDataAnnotationAttributeErrors(object validatingObject)
        {
            var properties = TypeDescriptor.GetProperties(validatingObject).Cast<PropertyDescriptor>();
            foreach (var property in properties)
            {
                var validationAttributes = property.Attributes.OfType<ValidationAttribute>().ToArray();
                if (validationAttributes.IsNullOrEmpty())
                {
                    continue;
                }

                var validationContext = new ValidationContext(validatingObject)
                {
                    DisplayName = property.Name,
                    MemberName = property.Name
                };

                foreach (var attribute in validationAttributes)
                {
                    var result = attribute.GetValidationResult(property.GetValue(validatingObject), validationContext);
                    if (result != null)
                    {
                        _validationErrors.Add(result);
                    }
                }
            }
        }

        protected virtual void NormalizeParameter(object parameterValue)
        {
            if (parameterValue is IShouldNormalize)
            {
                (parameterValue as IShouldNormalize).Normalize();
            }
        }
    }
}
