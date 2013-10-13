using Abp.Application.Services.Dto;

namespace Abp.Modules.Core.Application.Services.Dto.Users
{
    public class GetCurrentUserInfoOutput : IOutputDto
    {
        public UserDto User { get; set; }
    }
}