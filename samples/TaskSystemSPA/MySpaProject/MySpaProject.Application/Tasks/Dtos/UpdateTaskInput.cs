using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;

namespace MySpaProject.Tasks.Dtos
{
    public class UpdateTaskInput : IInputDto, ICustomValidate
    {
        [Range(1, long.MaxValue)]
        public long TaskId { get; set; }

        public int? AssignedPersonId { get; set; }

        public TaskState? State { get; set; }

        public void AddValidationErrors(List<ValidationResult> results)
        {
            if (AssignedPersonId == null && State == null)
            {
                results.Add(new ValidationResult("Both of AssignedPersonId and State can not be null in order to update a Task!", new[] {"AssignedPersonId", "State"}));
            }
        }
    }
}