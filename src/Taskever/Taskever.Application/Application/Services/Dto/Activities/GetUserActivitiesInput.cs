using Abp.Application.Services.Dto;

namespace Taskever.Application.Services.Dto.Activities
{
    public class GetUserActivitiesInput : IInputDto
    {
        public int UserId { get; set; }
    }
}