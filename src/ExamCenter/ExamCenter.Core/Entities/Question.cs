using System;
using Abp.Entities;

namespace ExamCenter.Entities
{
    /// <summary>
    /// An question in the system.
    /// </summary>
    public class Question : AuditedEntity<int>
    {
        /// <summary>
        /// Question text.
        /// </summary>
        public virtual string QuestionText { get; set; }

        public virtual AnsweringType AnsweringType { get; set; }

        public virtual int? EstimatedAnsweringTime { get; set; }

        public virtual ExperienceDegree? ExperienceDegree { get; set; }

        public virtual int TotalAskedCountInAllExams { get; set; }

        public virtual DateTime? LastAskedTime { get; set; }

        /// <summary>
        /// If <see cref="AnsweringType"/> is FreeText, this can be used to enter right answer
        /// </summary>
        public virtual string RightAnswerText { get; set; }
    }
}
