using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;

namespace Abp.Modules.Core.Application.Services.Dto.Users
{
    public class ChangePasswordInput : IInputDto
    {
        [StringLength(30, MinimumLength = 3)] //TODO: Avoid Magic numbers!
        public virtual string CurrentPassword { get; set; }

        [StringLength(30, MinimumLength = 3)]
        public virtual string NewPassword { get; set; }

        [StringLength(30, MinimumLength = 3)]
        [Compare("NewPassword", ErrorMessage = "Passwords must match!")]
        public string NewPasswordRepeat { get; set; }
    }
}