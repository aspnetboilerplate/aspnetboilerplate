using Abp.Application.Services.Dto;
using Abp.Modules.Core.Application.Services.Dto.Users;

namespace Taskever.Application.Services.Dto.TaskeverUsers
{
    public class GetUserProfileOutput :IOutputDto
    {
        public bool CanNotSeeTheProfile { get; set; }

        public bool SentFriendshipRequest { get; set; }

        public UserDto User { get; set; }
    }
}