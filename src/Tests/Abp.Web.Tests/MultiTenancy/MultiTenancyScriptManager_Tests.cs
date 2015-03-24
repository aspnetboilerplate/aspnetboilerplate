using Abp.Configuration.Startup;
using Abp.Web.MultiTenancy;
using Shouldly;
using Xunit;

namespace Abp.Web.Tests.MultiTenancy
{
    public class MultiTenancyScriptManager_Tests
    {
        [Fact]
        public void Should_Get_Script()
        {
            var scriptManager = new MultiTenancyScriptManager(new MultiTenancyConfig {IsEnabled = true});
            var script = scriptManager.GetScript();
            script.ShouldNotBe(null);
        }
    }
}
