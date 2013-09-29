namespace Abp.Modules.Core.Application.Services.Dto
{
    public class RegisterUserDto
    {
        public string Name { get; set; }
        
        public string Surname { get; set; }

        public string EmailAddress { get; set; }

        public string Password { get; set; }

        public string PasswordRepeat { get; set; }
    }
}