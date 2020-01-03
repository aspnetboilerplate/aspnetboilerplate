using Abp.BackgroundJobs;
using Abp.WebHooks;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.WebHooks
{
    public class WebHookPublisher_Tests : WebHookTestBase
    {
        private IWebHookPublisher _webHookPublisher;
        private IBackgroundJobManager _backGroundJobSubstitute;

        public WebHookPublisher_Tests()
        {
            AbpSession.UserId = 1;
            AbpSession.TenantId = null;

            _backGroundJobSubstitute = RegisterFake<IBackgroundJobManager>();
            _webHookPublisher = Resolve<IWebHookPublisher>();
        }


        [Fact]
        public void Should_Not_Call_Un_Authorized_Users_WebHook()
        {
        }
    }
}
