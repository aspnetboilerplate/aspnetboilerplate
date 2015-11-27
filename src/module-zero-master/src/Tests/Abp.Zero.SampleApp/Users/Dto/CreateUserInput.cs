using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;

namespace Abp.Zero.SampleApp.Users.Dto
{
    public class CreateUserInput : IInputDto
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        public virtual string Surname { get; set; }
        
        [Required]
        public virtual string UserName { get; set; }

        [Required]
        public virtual string EmailAddress { get; set; }
    }
}