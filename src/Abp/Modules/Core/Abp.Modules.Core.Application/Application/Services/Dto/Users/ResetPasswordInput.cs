using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;

namespace Abp.Modules.Core.Application.Services.Dto.Users
{
    public class ResetPasswordInput : IInputDto
    {
        [Range(1, int.MaxValue)]
        public int UserId { get; set; }

        [StringLength(30, MinimumLength = 3)]
        public string Password { get; set; }

        [StringLength(30, MinimumLength = 3)]
        [Compare("Password", ErrorMessage = "Passwords do no match!")]
        public string PasswordRepeat { get; set; }

        public string PasswordResetCode { get; set; }
    }
}