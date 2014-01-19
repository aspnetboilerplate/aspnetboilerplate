using Abp.Application.Services.Dto;

namespace Abp.Users.Dto
{
    public class SendPasswordResetLinkInput : IInputDto
    {
        public string EmailAddress { get; set; }
    }
}