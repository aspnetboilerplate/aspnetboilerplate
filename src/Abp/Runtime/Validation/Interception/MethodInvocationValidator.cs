using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Abp.Collections.Extensions;

namespace Abp.Runtime.Validation.Interception
{
    /// <summary>
    /// This class is used to validate a method call (invocation) for method arguments.
    /// </summary>
    internal class MethodInvocationValidator
    {
        private readonly object[] _parameterValues;
        private readonly ParameterInfo[] _parameters;
        private readonly List<ValidationResult> _validationErrors;

        /// <summary>
        /// Creates a new <see cref="MethodInvocationValidator"/> instance.
        /// </summary>
        /// <param name="method">Method to be validated</param>
        /// <param name="parameterValues">List of arguments those are used to call the <paramref name="method"/>.</param>
        public MethodInvocationValidator(MethodInfo method, object[] parameterValues)
        {
            _parameterValues = parameterValues;
            _parameters = method.GetParameters();
            _validationErrors = new List<ValidationResult>();
        }

        /// <summary>
        /// Validates the method invocation.
        /// </summary>
        public void Validate()
        {
            if (_parameters.IsNullOrEmpty())
            {
                //Object has no parameter, no need to validate.
                return;
            }

            if (_parameters.Length != _parameterValues.Length)
            {
                //This is not possible actually
                throw new Exception("Method parameter count does not match with argument count!");
            }

            for (var i = 0; i < _parameters.Length; i++)
            {
                ValidateMethodParameter(_parameters[i], _parameterValues[i]);
            }

            if (_validationErrors.Any())
            {
                throw new AbpValidationException("Method arguments are not valid! See ValidationErrors for details.") { ValidationErrors = _validationErrors };
            }

            foreach (var parameterValue in _parameterValues)
            {
                NormalizeParameter(parameterValue); //TODO@Halil: Why not normalize recursively as we did in validation.
            }
        }

        /// <summary>
        /// Validates given parameter for given value.
        /// </summary>
        /// <param name="parameter">Parameter of the method to validate</param>
        /// <param name="parameterValue">Value to validate</param>
        private void ValidateMethodParameter(ParameterInfo parameter, object parameterValue)
        {
            if (parameterValue == null)
            {
                //TODO@Halil: What if null value is acceptable?
                //TODO@Halil: What if has default value?

                if (!parameter.IsOptional && !parameter.IsOut)
                {
                    _validationErrors.Add(new ValidationResult(parameter.Name + " is null!", new[] { parameter.Name }));
                }

                return;
            }

            ValidateValueRecursively(parameterValue);
        }

        private void ValidateValueRecursively(object validatingValue)
        {
            if (!(validatingValue is IValidate))
            {
                return;
            }

            SetValidationAttributeErrors(validatingValue);

            if (validatingValue is ICustomValidate)
            {
                (validatingValue as ICustomValidate).AddValidationErrors(_validationErrors);
            }

            var properties = TypeDescriptor.GetProperties(validatingValue).Cast<PropertyDescriptor>();
            foreach (var property in properties)
            {
                var propertyValue = property.GetValue(validatingValue);
                ValidateValueRecursively(propertyValue);
            }
        }

        /// <summary>
        /// Checks all properties for DataAnnotations attributes.
        /// </summary>
        private void SetValidationAttributeErrors(object validatingValue)
        {
            var properties = TypeDescriptor.GetProperties(validatingValue).Cast<PropertyDescriptor>();
            foreach (var property in properties)
            {
                var validationAttributes = property.Attributes.OfType<ValidationAttribute>().ToArray();
                if (validationAttributes.IsNullOrEmpty())
                {
                    continue;
                }

                var validationContext = new ValidationContext(validatingValue) { DisplayName = property.Name };
                foreach (var attribute in validationAttributes)
                {
                    var result = attribute.GetValidationResult(property.GetValue(validatingValue), validationContext);
                    if (result != null)
                    {
                        _validationErrors.Add(result);
                    }
                }
            }
        }

        private static void NormalizeParameter(object parameterValue)
        {
            if (parameterValue is IShouldNormalize)
            {
                (parameterValue as IShouldNormalize).Normalize();
            }
        }
    }
}