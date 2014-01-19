using System;
using Abp.Application.Services.Dto;
using Abp.Users.Dto;

namespace Taskever.Activities.Dto
{
    public class UserFollowedActivityDto : EntityDto<long>
    {
        public UserDto User { get; set; }

        public ActivityDto Activity { get; set; }

        public bool IsActor { get; set; }

        public bool IsRelated { get; set; }

        public DateTime CreationTime { get; set; }
    }
}