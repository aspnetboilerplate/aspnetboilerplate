using System;
using Abp.Application.Services.Dto;
using Abp.Modules.Core.Application.Services.Dto.Users;
using Taskever.Domain.Enums;

namespace Taskever.Application.Services.Dto.Friendships
{
    public class FriendshipDto : EntityDto
    {
        public UserDto Friend { get; set; }

        public bool FallowActivities { get; set; }

        public bool CanAssignTask { get; set; }

        public FriendshipStatus Status { get; set; }

        public DateTime CreationTime { get; set; }
    }
}