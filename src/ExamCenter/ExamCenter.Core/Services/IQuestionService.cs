using System.Collections.Generic;
using Abp.Services;
using ExamCenter.Entities;
using ExamCenter.Services.Dto;

namespace ExamCenter.Services
{
    public interface IQuestionService :IService
    {
        IList<QuestionDto> GetAllQuestions();
    }
}
