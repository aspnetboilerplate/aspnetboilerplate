using System.Collections.Generic;
using System.Linq;
using Abp.Data;
using ExamCenter.Data.Repositories;
using ExamCenter.Entities;

namespace ExamCenter.Services.Impl
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepository;

        public QuestionService(IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }

        [UnitOfWork]
        public IList<Question> GetAllQuestions()
        {
            return _questionRepository.GetAll().ToList();
        }
    }
}