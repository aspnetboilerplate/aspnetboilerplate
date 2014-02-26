using Abp.Application.Services.Dto;

namespace Abp.Users.Dto
{
    public class GetCurrentUserInfoOutput : IOutputDto
    {
        public UserDto User { get; set; }
    }
}