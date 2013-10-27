using Abp.Application.Services.Dto;
using Abp.Modules.Core.Application.Services.Dto.Users;
using Taskever.Domain.Entities;

namespace Taskever.Application.Services.Dto.Friendships
{
    public class FriendshipDto : EntityDto
    {
        public UserDto Friend { get; set; }

        public bool FallowActivities { get; set; }

        public bool CanAssignTask { get; set; }

        //? public virtual FriendshipStatus Status { get; set; }
    }
}