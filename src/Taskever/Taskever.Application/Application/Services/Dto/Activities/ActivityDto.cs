using System;
using Abp.Application.Services.Dto;
using Abp.Modules.Core.Application.Services.Dto.Users;
using Taskever.Application.Services.Dto.Tasks;
using Taskever.Domain.Entities.Activities;

namespace Taskever.Application.Services.Dto.Activities
{
    public class ActivityDto : EntityDto<long>
    {
        public ActivityType ActivityType { get; set; }

        public DateTime CreationTime { get; set; }
    }

    public class CreateTaskActivityDto : ActivityDto
    {
        public UserDto CreatorUser { get; set; }

        public UserDto AssignedUser { get; set; }

        public TaskDto Task { get; set; }
    }

    public class CompleteTaskActivityDto : ActivityDto
    {
        public UserDto AssignedUser { get; set; }

        public TaskDto Task { get; set; }
    }

    public class UserFollowedActivityDto : EntityDto<long>
    {
        public UserDto User { get; set; }

        public ActivityDto Activity { get; set; }

        public bool IsActor { get; set; }

        public bool IsRelated { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
