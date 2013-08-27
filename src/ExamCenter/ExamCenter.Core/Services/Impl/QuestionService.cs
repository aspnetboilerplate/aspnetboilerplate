using System.Collections.Generic;
using System.Linq;
using Abp.Data;
using Abp.Services.Core.Impl;
using ExamCenter.Data.Repositories;
using ExamCenter.Entities;
using ExamCenter.Services.Dto;

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
        public IList<QuestionDto> GetAllQuestions()
        {
            return _questionRepository.Query(q => q.ToList()).MapIList<Question, QuestionDto>();
        }
    }
}