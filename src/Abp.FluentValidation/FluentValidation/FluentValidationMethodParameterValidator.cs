using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Abp.Runtime.Validation.Interception;
using FluentValidation;

namespace Abp.FluentValidation
{
    public class FluentValidationMethodParameterValidator : IMethodParameterValidator
    {
        private readonly IValidatorFactory _validatorFactory;

        public FluentValidationMethodParameterValidator(IValidatorFactory validatorFactory)
        {
            _validatorFactory = validatorFactory;
        }

        public IReadOnlyList<ValidationResult> Validate(object validatingObject)
        {
            var validationErrors = new List<ValidationResult>();

            var fvValidator = _validatorFactory.GetValidator(validatingObject.GetType());

            if (fvValidator != null)
            {
                var validationResult = fvValidator.Validate(validatingObject);
                var mappedValidationErrors = validationResult.Errors
                    .Select(e => new ValidationResult(e.ErrorMessage, new[] { e.PropertyName }))
                    .ToList();

                validationErrors.AddRange(mappedValidationErrors);
            }

            return validationErrors;
        }
    }
}
