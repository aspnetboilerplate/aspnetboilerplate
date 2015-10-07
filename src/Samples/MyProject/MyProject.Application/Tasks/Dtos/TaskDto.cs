using System;
using Abp.Application.Services.Dto;

namespace MyProject.Tasks.Dtos
{
    /// <summary>
    /// A DTO class that can be used in various application service methods when needed to send/receive Task objects.
    /// </summary>
    public class TaskDto : EntityDto<long>
    {
        public int? AssignedPersonId { get; set; }

        public string AssignedPersonName { get; set; }

        public string Description { get; set; }

        public DateTime CreationTime { get; set; }

        public byte State { get; set; }
    }
}