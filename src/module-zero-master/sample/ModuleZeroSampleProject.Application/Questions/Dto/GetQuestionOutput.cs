using Abp.Application.Services.Dto;

namespace ModuleZeroSampleProject.Questions.Dto
{
    public class GetQuestionOutput : IOutputDto
    {
        public QuestionWithAnswersDto Question { get; set; }
    }
}