using Abp.Application.Services.Dto;

namespace Abp.Users.Dto
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