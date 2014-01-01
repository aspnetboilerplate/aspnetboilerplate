using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Abp.Application.Services.Dto;
using Abp.Utils.Extensions;
using Abp.Utils.Extensions.Collections;

namespace Abp.Validation
{
    public class MethodInvocationValidator : IMethodInvocationValidator
    {
        public virtual void Validate(object targetObject, MethodInfo method, object[] arguments)
        {
            if (arguments.IsNullOrEmpty())
            {
                return;
            }

            foreach (var argument in arguments)
            {
                Validate(argument);
            }
        }

        protected virtual void Validate(object argument)
        {
            if (argument == null)
            {
                throw new ValidationException("Argument is null"); //TODO: Wich argument?
            }

            var validationErrors = new List<ValidationResult>();

            if (argument is IValidate)
            {
                SetValidationAttributeErrors(argument, validationErrors);
            }

            if (argument is ICustomValidate)
            {
                (argument as ICustomValidate).AddValidationResults(validationErrors);
            }
            
            if(validationErrors.Any())
            {
                throw new ValidationException("There are validation errors!"); //TODO: What are thay?
            }
        }

        protected virtual void SetValidationAttributeErrors(object item, List<ValidationResult> results)
        {
            var context = new ValidationContext(item);

            var properties = TypeDescriptor.GetProperties(item).Cast<PropertyDescriptor>();
            foreach (var property in properties)
            {
                foreach (var attribute in property.Attributes.OfType<ValidationAttribute>())
                {
                    var result = attribute.GetValidationResult(property.GetValue(item), context);
                    if (result != null)
                    {
                        results.Add(result);
                    }
                }
            }
        }
    }
}