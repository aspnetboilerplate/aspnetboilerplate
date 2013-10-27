using Abp.Application.Services.Dto;

namespace Taskever.Application.Services.Dto.Tasks
{
    public class CreateTaskInput :IInputDto
    {
        public TaskDto Task { get; set; }
    }
}