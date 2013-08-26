using Abp.Entities.NHibernate.Mappings;

namespace ExamCenter.Entities.NHibernate.Mappings
{
    public class QuestionMap : EntityMap<Question, int>
    {
        public QuestionMap()
            : base("Questions")
        {
            Map(x => x.QuestionText);
            Map(x => x.AnsweringType).CustomType<AnsweringType>();
            Map(x => x.EstimatedAnsweringTime);
            Map(x => x.ExperienceDegree).CustomType<ExperienceDegree>();
            Map(x => x.RightAnswerText);
        }
    }
}
