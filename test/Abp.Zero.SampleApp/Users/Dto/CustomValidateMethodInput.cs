using System.ComponentModel.DataAnnotations;
using Abp.Runtime.Validation;

namespace Abp.Zero.SampleApp.Users.Dto
{
    public class CustomValidateMethodInput : ICustomValidate
    {
        public void AddValidationErrors(CustomValidationContext context)
        {
            var message = context.Localize(AbpZeroConsts.LocalizationSourceName, "Identity.UserNotInRole");
            context.Results.Add(new ValidationResult(message));
        }
    }
}