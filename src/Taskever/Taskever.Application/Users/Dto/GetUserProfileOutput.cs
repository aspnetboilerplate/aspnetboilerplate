using Abp.Application.Services.Dto;
using Abp.Users.Dto;

namespace Taskever.Users.Dto
{
    public class GetUserProfileOutput :IOutputDto
    {
        public bool CanNotSeeTheProfile { get; set; }

        public bool SentFriendshipRequest { get; set; }

        public UserDto User { get; set; }
    }
}