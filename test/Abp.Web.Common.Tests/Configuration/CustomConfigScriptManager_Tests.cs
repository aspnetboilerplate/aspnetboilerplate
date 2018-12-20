using System.Collections.Generic;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.TestBase;
using Abp.Web.Configuration;
using Shouldly;
using Xunit;

namespace Abp.Web.Common.Tests.Configuration
{
    public class CustomConfigScriptManager_Tests : AbpIntegratedTestBase<AbpWebCommonTestModule>
    {
        private readonly ICustomConfigScriptManager _customConfigScriptManager;
        private readonly IAbpStartupConfiguration _abpStartupConfiguration;

        public CustomConfigScriptManager_Tests()
        {
            _abpStartupConfiguration = Resolve<IAbpStartupConfiguration>();
            _customConfigScriptManager = Resolve<ICustomConfigScriptManager>();
        }

        [Fact]
        public void CustomConfigScriptManager_Should_Build_Custom_Configuration()
        {
            _abpStartupConfiguration.CustomConfigProviders.Add(new TestCustomConfigProvider());

            var script = _customConfigScriptManager.GetScript();
            script.ShouldNotBeNullOrEmpty();
            script.ShouldContain("EntityHistory");
        }

        [Fact]
        public void CustomConfigScriptManager_Should_Build_Empty_Custom_Configuration_When_CustomConfigProviders_Empty()
        {
            _abpStartupConfiguration.CustomConfigProviders.Clear();

            var script = _customConfigScriptManager.GetScript();
            script.ShouldNotBeNullOrEmpty();
        }
    }

    public class TestCustomConfigProvider : ICustomConfigProvider
    {
        public Dictionary<string, object> GetConfig(CustomConfigProviderContext customConfigProviderContext)
        {
            return new Dictionary<string, object>
            {
                {
                    "EntityHistory",
                    new {
                        IsEnabled = true
                    }
                }
            };
        }
    }
}
