using System;
using Abp.Application.Services.Dto;
using Abp.Modules.Core.Application.Services.Dto.Users;
using Taskever.Application.Services.Dto.Tasks;
using Taskever.Domain.Entities.Activities;

namespace Taskever.Application.Services.Dto.Activities
{
    public class ActivityDto : EntityDto<long>, IOutputDto
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
}
