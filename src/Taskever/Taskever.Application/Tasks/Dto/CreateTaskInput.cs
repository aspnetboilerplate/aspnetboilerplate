using System.Web;
using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;

namespace Taskever.Tasks.Dto
{
    public class CreateTaskInput : IInputDto, IShouldNormalize
    {
        public TaskDto Task { get; set; }

        public void Normalize()
        {
            Task.Title = HttpUtility.HtmlEncode(Task.Title);
            Task.Description = HttpUtility.HtmlEncode(Task.Description);
        }
    }
}