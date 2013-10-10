using Abp.Application.Services.Dto;

namespace Taskever.Application.Services.Dto.Activities
{
    public class GetFallowedActivitiesInput : IInputDto
    {
        public int FallowerUserId { get; set; }
    }
}