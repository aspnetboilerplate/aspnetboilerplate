using Abp.Modules.Core.Application.Services.Dto;

namespace Taskever.Application.Services.Dto.Tasks
{
    /// <summary>
    /// Task DTO.
    /// </summary>
    public class TaskDto : AuditedEntityDto
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public int AssignedUserId { get; set; }

        public byte Priority { get; set; }

        public byte State { get; set; }

        public byte Privacy { get; set; }
    }
}
