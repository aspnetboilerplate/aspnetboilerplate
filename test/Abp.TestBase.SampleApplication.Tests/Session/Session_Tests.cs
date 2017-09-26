using Abp.Configuration.Startup;
using Abp.Runtime.Session;
using Shouldly;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.Session
{
    public class Session_Tests : SampleApplicationTestBase
    {
        private readonly IAbpSession _session;

        public Session_Tests()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = true;
            _session = Resolve<IAbpSession>();
        }

        [Fact]
        public void Session_Override_Test()
        {
            _session.UserId.ShouldBeNull();
            _session.TenantId.ShouldBeNull();

            using (_session.Use(42, 571))
            {
                _session.TenantId.ShouldBe(42);
                _session.UserId.ShouldBe(571);

                using (_session.Use(null, 3))
                {
                    _session.TenantId.ShouldBeNull();
                    _session.UserId.ShouldBe(3);
                }

                _session.TenantId.ShouldBe(42);
                _session.UserId.ShouldBe(571);
            }

            _session.UserId.ShouldBeNull();
            _session.TenantId.ShouldBeNull();
        }
    }
}
