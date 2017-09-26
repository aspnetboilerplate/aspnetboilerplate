using Xunit;

namespace Abp.IdentityServer4
{
    public class DependencyInjection_Tests: AbpZeroIdentityServerTestBase
    {
        [Fact]
        public void Should_Inject_AbpPersistedGrantStore()
        {
            Resolve<AbpPersistedGrantStore>();
        }
    }
}
