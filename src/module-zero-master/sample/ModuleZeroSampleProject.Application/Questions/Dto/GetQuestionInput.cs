using Abp.Application.Services.Dto;

namespace ModuleZeroSampleProject.Questions.Dto
{
    public class GetQuestionInput : EntityRequestInput
    {
        public bool IncrementViewCount { get; set; }
    }
}