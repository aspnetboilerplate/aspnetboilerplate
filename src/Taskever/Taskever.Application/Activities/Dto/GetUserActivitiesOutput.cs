using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace Taskever.Activities.Dto
{
    public class GetUserActivitiesOutput : IOutputDto
    {
        public IList<UserFollowedActivityDto> Activities { get; set; }
    }
}