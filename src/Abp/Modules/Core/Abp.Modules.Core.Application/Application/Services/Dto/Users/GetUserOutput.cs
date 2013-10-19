using Abp.Application.Services.Dto;

namespace Abp.Modules.Core.Application.Services.Dto.Users
{
    public class GetUserOutput : IOutputDto
    {
        public UserDto User { get; set; }

        public GetUserOutput(UserDto user)
        {
            User = user;
        }
    }
}