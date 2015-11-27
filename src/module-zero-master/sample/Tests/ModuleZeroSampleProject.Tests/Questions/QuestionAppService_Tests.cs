using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using ModuleZeroSampleProject.Questions;
using ModuleZeroSampleProject.Questions.Dto;
using Shouldly;
using Xunit;

namespace ModuleZeroSampleProject.Tests.Questions
{
    public class QuestionAppService_Tests : SampleProjectTestBase
    {
        private readonly IQuestionAppService _questionAppService;

        public QuestionAppService_Tests()
        {
            _questionAppService = LocalIocManager.Resolve<IQuestionAppService>();
        }

        [Fact]
        public void Should_Add_Question_With_Authorized_User()
        {
            AbpSession.UserId = UsingDbContext(context => context.Users.Single(u => u.UserName == "admin" && u.TenantId == AbpSession.TenantId.Value).Id);

            const string questionTitle = "Test question title 1";

            //Question does not exists now
            UsingDbContext(context => context.Questions.FirstOrDefault(q => q.Title == questionTitle).ShouldBe(null));

            //Create the question
            _questionAppService.CreateQuestion(
                new CreateQuestionInput
                {
                    Title = questionTitle,
                    Text = "A dummy question text..."
                });

            //Question is added now
            var question = UsingDbContext(context => context.Questions.FirstOrDefault(q => q.Title == questionTitle));
            question.ShouldNotBe(null);

            //Vote up the question
            var voteUpOutput = _questionAppService.VoteUp(new EntityRequestInput(question.Id));
            voteUpOutput.VoteCount.ShouldBe(1);

            //Vote down the question
            var voteDownOutput = _questionAppService.VoteDown(new EntityRequestInput(question.Id));
            voteDownOutput.VoteCount.ShouldBe(0);
        }

        [Fact]
        public async Task Should_Not_Add_Question_Without_Login()
        {
            AbpSession.UserId = null; //not logged in

            await Assert.ThrowsAsync<AbpAuthorizationException>(
                () => _questionAppService.CreateQuestion(
                    new CreateQuestionInput
                    {
                        Title = "A dummy title",
                        Text = "A dummy question text..."
                    }));
        }
    }
}
