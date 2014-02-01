using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Abp.Runtime.Validation
{
    /// <summary>
    /// Defines interface that must be implemented by classes those must be validated with custom rules.
    /// So, implementing class can define it's own validation logic.
    /// </summary>
    public interface ICustomValidate : IValidate
    {
        /// <summary>
        /// This method is used to validate the object.
        /// </summary>
        /// <param name="results">List of validation results (errors). Add validation errors to this list.</param>
        void AddValidationErrors(List<ValidationResult> results);
    }
}