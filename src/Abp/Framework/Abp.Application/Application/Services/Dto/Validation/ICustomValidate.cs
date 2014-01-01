using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Abp.Application.Services.Dto.Validation
{
    public interface ICustomValidate : IValidate
    {
        void AddValidationErrors(List<ValidationResult> results);
    }
}