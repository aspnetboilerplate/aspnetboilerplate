using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Abp.Utils.Extensions.Collections;

namespace Abp.Application.Services.Dto.Validation
{
    public class MethodInvocationValidator
    {
        private readonly MethodInfo _method;
        private readonly object[] _arguments;
        
        private readonly ValidationContext _validationContext;
        private readonly List<ValidationResult> _validationErrors;

        public MethodInvocationValidator(object instance, MethodInfo method, object[] arguments)
        {
            _method = method;
            _arguments = arguments;

            _validationContext = new ValidationContext(instance);
            _validationErrors = new List<ValidationResult>();
        }

        public virtual void Validate()
        {
            if (_arguments.IsNullOrEmpty())
            {
                return;
            }

            var parameters = _method.GetParameters();
            if (parameters.Length != _arguments.Length)
            {
                throw new Exception("Method parameter count does not matche with argument count!");
            }
            
            for (var i = 0; i < parameters.Length; i++)
            {
                Validate(parameters[i], _arguments[i]);
            }

            if (_validationErrors.Any())
            {
                throw new ValidationException("There are validation errors!"); //TODO: What are thay?
            }
        }

        private void Validate(ParameterInfo parameter, object argument)
        {
            if (argument == null)
            {
                //TODO: Maybe method accept null values? How to handle it
                _validationErrors.Add(new ValidationResult(parameter.Name + " is null!"));
                return;
            }

            if (argument is IValidate)
            {
                SetValidationAttributeErrors(argument);
            }

            if (argument is ICustomValidate)
            {
                (argument as ICustomValidate).AddValidationErrors(_validationErrors);
            }
        }

        /// <summary>
        /// Checks all properties for DataAnnotations attributes.
        /// </summary>
        private void SetValidationAttributeErrors(object argument)
        {
            var properties = TypeDescriptor.GetProperties(argument).Cast<PropertyDescriptor>();
            foreach (var property in properties)
            {
                foreach (var attribute in property.Attributes.OfType<ValidationAttribute>())
                {
                    var result = attribute.GetValidationResult(property.GetValue(argument), _validationContext);
                    if (result != null)
                    {
                        _validationErrors.Add(result);
                    }
                }
            }
        }
    }
}