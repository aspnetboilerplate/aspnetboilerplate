using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;

namespace MyProject.Tasks.Dtos
{
    public class CreateTaskInput : IInputDto
    {
        public int? AssignedPersonId { get; set; }

        [Required]
        public string Description { get; set; }

        public override string ToString()
        {
            return string.Format("[CreateTaskInput > AssignedPersonId = {0}, Description = {1}]", AssignedPersonId, Description);
        }
    }
}