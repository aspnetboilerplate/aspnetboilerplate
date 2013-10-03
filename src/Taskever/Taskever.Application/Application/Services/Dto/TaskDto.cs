using Abp.Modules.Core.Application.Services.Dto;
using Taskever.Domain.Entities;

namespace Taskever.Application.Services.Dto
{
    /// <summary>
    /// Task DTO.
    /// </summary>
    public class TaskDto : AuditedEntityDto
    {
        /// <summary>
        /// Task title.
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// Task description.
        /// </summary>
        public virtual string Description { get; set; }

        public virtual int AssignedUserId { get; set; }

        public virtual byte Priority { get; set; }
    }
}
