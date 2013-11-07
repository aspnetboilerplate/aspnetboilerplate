using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace Taskever.Application.Services.Dto.Activities
{
    public class GetUserActivitiesOutput : IOutputDto
    {
        public IList<ActivityDto> Activities { get; set; }
    }
}