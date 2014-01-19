using System;
using Abp.Application.Services.Dto;

namespace Taskever.Activities.Dto
{
    public class ActivityDto : EntityDto<long>
    {
        public ActivityType ActivityType { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
