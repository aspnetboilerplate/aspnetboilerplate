using System;
using Abp.Entities;

namespace ExamCenter.Entities
{
    public class Question : Entity<int>
    {
        public virtual string QuestionText { get; set; }

        public virtual DateTime CreationDate { get; set; }

        public Question()
        {
            CreationDate = DateTime.Now;
        }
    }
}
