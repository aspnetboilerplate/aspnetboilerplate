using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;

namespace Taskever.Application.Services.Dto.Tasks
{
    public class GetTasksOfUserInput : IInputDto, IPagedResultRequest, ICustomValidate
    {
        public int UserId { get; set; }

        public int SkipCount { get; set; }

        public int MaxResultCount { get; set; }

        public GetTasksOfUserInput()
        {
            MaxResultCount = int.MaxValue;
        }

        public void GetValidationResult(List<ValidationResult> results)
        {
            //TODO: For demonstration, do it declarative!
            if (UserId <= 0)
            {
                results.Add(new ValidationResult("UserId must be a positive value!", new[] { "UserId" }));
            }
        }
    }
}