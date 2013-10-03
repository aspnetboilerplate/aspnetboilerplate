using Abp.Application.Services.Dto;

namespace Taskever.Application.Services.Dto.TaskService
{
    public class GetTasksOfUserInput : IInputDto
    {
        public int UserId { get; set; }
    }
}