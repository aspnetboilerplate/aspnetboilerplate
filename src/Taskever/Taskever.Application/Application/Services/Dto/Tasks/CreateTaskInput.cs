using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Abp.Application.Services.Dto;

namespace Taskever.Application.Services.Dto.Tasks
{
    public class CreateTaskInput : IInputDto, ICustomValidate
    {
        public TaskDto Task { get; set; }
        
        public void AddValidationErrors(List<ValidationResult> results)
        {
            Task.Title = HttpUtility.HtmlEncode(Task.Title);
            Task.Description = HttpUtility.HtmlEncode(Task.Description);
        }
    }
}