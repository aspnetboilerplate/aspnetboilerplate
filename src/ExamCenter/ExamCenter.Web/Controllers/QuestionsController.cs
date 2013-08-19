using System.Collections.Generic;
using Abp.Data;
using Abp.Localization;
using Abp.Web.Controllers;
using Castle.Core.Logging;
using ExamCenter.Entities;
using ExamCenter.Services;

namespace ExamCenter.Web.Controllers
{
    public class QuestionsController : AbpApiController
    {
        private readonly IQuestionService _questionService;

        public ILogger Logger { get; set; } 

        public QuestionsController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        [UnitOfWork]
        public virtual IEnumerable<Question> Get()
        {
            //validation
            //throwing appropriate messages
            //logging
            //exception handling

            Logger.Info(L("GetAllQuestions_Method_Is_Called"));
            return _questionService.GetAllQuestions();
        }
    }
}