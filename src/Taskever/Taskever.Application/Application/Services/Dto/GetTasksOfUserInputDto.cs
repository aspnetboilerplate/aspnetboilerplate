using Abp.Application.Services.Dto;

namespace Taskever.Application.Services.Dto
{
    public class GetTasksOfUserInputDto : IInputDto
    {
        public int UserId { get; set; }
    }
}