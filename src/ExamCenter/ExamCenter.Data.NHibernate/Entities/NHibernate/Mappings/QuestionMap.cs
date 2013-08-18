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
            Map(x => x.CreationDate);
        }
    }
}
