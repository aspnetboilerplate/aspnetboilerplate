using Abp.Modules.Core.Application.Services.Dto;

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
    }
}
