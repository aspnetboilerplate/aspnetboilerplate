using Abp.Data.Repositories.NHibernate;
using ExamCenter.Entities;

namespace ExamCenter.Data.Repositories.NHibernate
{
    public class NhQuestionRepository : NhRepositoryBase<Question, int>, IQuestionRepository
    {

    }
}
