using Abp.Services.Core.Dto;
using Abp.Services.Dto;
using ExamCenter.Entities;

namespace ExamCenter.Services.Dto
{
    public class QuestionDto : AuditedEntityDto
    {
        /// <summary>
        /// Question text.
        /// </summary>
        public virtual string QuestionText { get; set; }

        /// <summary>
        /// Answering type.
        /// </summary>
        public virtual AnsweringType AnsweringType { get; set; }

        /// <summary>
        /// Estimated answering type.
        /// </summary>
        public virtual int? EstimatedAnsweringTime { get; set; }

        /// <summary>
        /// Experince degree to be able to answer this question.
        /// </summary>
        public virtual ExperienceDegree? ExperienceDegree { get; set; }

        /// <summary>
        /// If <see cref="AnsweringType"/> is FreeText, this can be used to enter the right answer
        /// </summary>
        public virtual string RightAnswerText { get; set; }

        /// <summary>
        /// Creator user's name of this entity.
        /// </summary>
        public virtual string CreatorUserName { get; set; }
    }
}
