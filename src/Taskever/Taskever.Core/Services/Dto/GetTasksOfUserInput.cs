using Abp.Application.Services.Dto;

namespace Taskever.Services.Dto
{
    public class GetTasksOfUserInput : IInputDto
    {
        public int UserId { get; set; }
    }
}