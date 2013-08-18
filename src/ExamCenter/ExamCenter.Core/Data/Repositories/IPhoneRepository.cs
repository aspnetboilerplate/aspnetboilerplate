using Abp.Data.Repositories;
using ExamCenter.Entities;

namespace ExamCenter.Data.Repositories
{
    public interface IQuestionRepository : IRepository<Question, int>
    {

    }
}
