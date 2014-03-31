using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;

namespace Abp.Users.Dto
{
    public class RegisterUserInput : IInputDto
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

        [Required]
        [StringLength(30)]
        public string UserName { get; set; }
        
        [StringLength(30, MinimumLength = 3)]
        public string Password { get; set; }

        [StringLength(30, MinimumLength = 3)]
        [Compare("Password", ErrorMessage = "Passwords do no match!")]
        public string PasswordRepeat { get; set; }

        public string ProfileImage { get; set; }
    }
}