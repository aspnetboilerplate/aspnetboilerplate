using Abp.Domain.Repositories;
using Abp.Domain.Services;

namespace ModuleZeroSampleProject.Questions
{
    public class QuestionDomainService : DomainService
    {
        private readonly IRepository<Answer> _answerRepository;

        public QuestionDomainService(IRepository<Answer> answerRepository)
        {
            _answerRepository = answerRepository;
        }

        public void AcceptAnswer(Answer answer)
        {
            var previousAcceptedAnswer = _answerRepository.FirstOrDefault(a => a.QuestionId == answer.QuestionId && a.IsAccepted);
            
            if (previousAcceptedAnswer != null)
            {
                previousAcceptedAnswer.IsAccepted = false;
            }

            answer.IsAccepted = true;
        }
    }
}
