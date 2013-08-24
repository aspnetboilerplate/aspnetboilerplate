using FluentNHibernate.Mapping;

namespace ExamCenter.Entities.NHibernate.Mappings
{
    public class QuestionMap : ClassMap<Question>
    {
        public QuestionMap()
        {
            Table("Questions");
            Id(x => x.Id);
            Map(x => x.QuestionText);
            Map(x => x.AnsweringType).CustomType<AnsweringType>();
            Map(x => x.EstimatedAnsweringTime);
            Map(x => x.ExperienceDegree).CustomType<ExperienceDegree>();
            Map(x => x.TotalAskedCountInAllExams);
            Map(x => x.LastAskedTime);
            Map(x => x.RightAnswerText);
            Map(x => x.CreationTime);
            Map(x => x.Creator);
            Map(x => x.LastModificationTime);
            Map(x => x.LastModifier);
        }
    }
}
