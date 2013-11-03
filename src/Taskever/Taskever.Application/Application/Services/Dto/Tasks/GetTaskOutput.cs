using Abp.Application.Services.Dto;

namespace Taskever.Application.Services.Dto.Tasks
{
    public class GetTaskOutput : IOutputDto
    {
        public TaskDto Task { get; set; }
    }
}