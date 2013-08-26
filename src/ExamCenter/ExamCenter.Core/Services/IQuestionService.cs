using System.Collections.Generic;
using Abp.Services;
using ExamCenter.Entities;

namespace ExamCenter.Services
{
    public interface IQuestionService :IService
    {
        IList<Question> GetAllQuestions();
    }
}
