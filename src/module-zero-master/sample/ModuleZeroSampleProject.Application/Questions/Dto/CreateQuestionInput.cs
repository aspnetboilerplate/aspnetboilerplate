using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;

namespace ModuleZeroSampleProject.Questions.Dto
{
    public class CreateQuestionInput : IInputDto
    {
        [Required]
        public string Title { get; set; }
        
        [Required]
        public string Text { get; set; }
    }
}