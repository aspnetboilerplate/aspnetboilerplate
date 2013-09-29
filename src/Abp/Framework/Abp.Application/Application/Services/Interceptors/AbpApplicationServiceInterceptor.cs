using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Castle.DynamicProxy;

namespace Abp.Application.Services.Interceptors
{
    public class AbpApplicationServiceInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            foreach (var argument in invocation.Arguments)
            {
                if (argument is IValidate)
                {
                    if (!IsValid(argument))
                    {
                        throw new Exception("Argument " + argument.GetType().Name + " is not valid!!!");
                    }
                }
            }

            invocation.Proceed();
        }

        public bool IsValid(object item)
        {

            if (item == null)
                return false;

            List<ValidationResult> validationErrors = new List<ValidationResult>();

            //SetValidatableObjectErrors(item, validationErrors);
            SetValidationAttributeErrors(item, validationErrors);

            return !validationErrors.Any();
        }

        void SetValidationAttributeErrors(object item, List<ValidationResult> results)
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
