using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Abp.Application.Services.Dto
{
    public interface ICustomValidate
    {
        void GetValidationResult(List<ValidationResult> results);
    }
}