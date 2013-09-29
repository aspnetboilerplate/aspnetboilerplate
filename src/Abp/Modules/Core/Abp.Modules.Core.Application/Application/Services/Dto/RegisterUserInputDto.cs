using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;

namespace Abp.Modules.Core.Application.Services.Dto
{
    public class RegisterUserInputDto : IInputDto
    {
        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        [Required]
        [StringLength(30)]
        public string Surname { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string EmailAddress { get; set; }

        [StringLength(30, MinimumLength = 3)]
        public string Password { get; set; }

        [StringLength(30, MinimumLength = 3)]
        [Compare("Password", ErrorMessage = "Heyooo! þifreyi düzgün gir!")]
        public string PasswordRepeat { get; set; }
    }
}