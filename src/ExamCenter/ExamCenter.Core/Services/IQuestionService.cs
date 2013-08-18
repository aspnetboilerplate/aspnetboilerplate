using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Services;
using ExamCenter.Entities;

namespace ExamCenter.Services
{
    public interface IQuestionService :IService
    {
        IList<Question> GetAllQuestions();
    }
}
