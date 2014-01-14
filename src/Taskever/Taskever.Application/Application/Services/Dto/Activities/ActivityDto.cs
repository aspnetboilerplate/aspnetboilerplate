using System;
using Abp.Application.Services.Dto;
using Taskever.Activities;

namespace Taskever.Application.Services.Dto.Activities
{
    public class ActivityDto : EntityDto<long>
    {
        public ActivityType ActivityType { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
