using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace ModuleZeroSampleProject.Questions.Dto
{
    [AutoMapFrom(typeof(Question))]
    public class QuestionDto : CreationAuditedEntityDto
    {
        public string Title { get; set; }

        public string Text { get; set; }

        public int VoteCount { get; set; }

        public int AnswerCount { get; set; }

        public int ViewCount { get; set; }

        public string CreatorUserName { get; set; }
    }
}