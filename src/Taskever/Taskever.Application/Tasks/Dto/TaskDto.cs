using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;

namespace Taskever.Tasks.Dto
{
    /// <summary>
    /// Task DTO.
    /// </summary>
    public class TaskDto : AuditedEntityDto
    {
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(2000)]
        public string Description { get; set; }

        public int AssignedUserId { get; set; }

        public byte Priority { get; set; }

        public byte State { get; set; }

        public byte Privacy { get; set; }
    }
}
