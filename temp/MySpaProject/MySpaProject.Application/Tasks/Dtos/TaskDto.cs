using System;
using Abp.Application.Services.Dto;

namespace MySpaProject.Tasks.Dtos
{
    public class TaskDto : EntityDto<long>
    {
        public int AssignedPersonId { get; set; }

        public string AssignedPersonName { get; set; }

        public string Description { get; set; }

        public DateTime CreationTime { get; set; }

        public byte State { get; set; }
    }
}