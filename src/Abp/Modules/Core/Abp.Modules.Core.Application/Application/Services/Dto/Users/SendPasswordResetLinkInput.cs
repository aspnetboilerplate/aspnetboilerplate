using Abp.Application.Services.Dto;

namespace Abp.Modules.Core.Application.Services.Dto.Users
{
    public class SendPasswordResetLinkInput : IInputDto
    {
        public string EmailAddress { get; set; }
    }
}