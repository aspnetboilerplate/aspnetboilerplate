using System;
using Abp.Application.Services.Dto;
using Abp.Users.Dto;

namespace Taskever.Friendships.Dto
{
    public class FriendshipDto : EntityDto
    {
        public UserDto Friend { get; set; }

        public bool FollowActivities { get; set; }

        public bool CanAssignTask { get; set; }

        public FriendshipStatus Status { get; set; }

        public DateTime CreationTime { get; set; }
    }
}