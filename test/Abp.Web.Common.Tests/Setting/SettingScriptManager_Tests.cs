using System;
using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Runtime.Session;
using Abp.TestBase;
using Abp.Web.Http;
using Abp.Web.Settings;
using Shouldly;
using Xunit;

namespace Abp.Web.Common.Tests.Setting
{
    public class SettingScriptManager_Tests : AbpIntegratedTestBase<AbpWebCommonTestModule>
    {
        private readonly ISettingScriptManager _settingScriptManager;
        private readonly ISettingManager _settingManager;

        public SettingScriptManager_Tests()
        {
            _settingScriptManager = Resolve<ISettingScriptManager>();
            _settingManager = Resolve<ISettingManager>();
        }

        [Fact]
        public async Task SettingScriptManager_Setting_Which_RequiresAuthentication()
        {
            var scripts = await _settingScriptManager.GetScriptAsync();
            scripts.ShouldNotContain("AbpWebCommonTestModule.Test.Setting1");
        }

        [Fact]
        public async Task SettingScriptManager_Setting_Which_RequiresPermission()
        {
            var scripts = await _settingScriptManager.GetScriptAsync();
            scripts.ShouldNotContain("AbpWebCommonTestModule.Test.Setting2");
        }

        [Fact]
        public async Task SettingScriptManager_Setting_Which_RequiresPermission_For_Authanticated_User()
        {
            LoginAsDefaultTenantAdmin();

            var scripts = await _settingScriptManager.GetScriptAsync();
            scripts.ShouldContain("AbpWebCommonTestModule.Test.Setting1");
        }

        [Fact]
        public async Task SettingScriptManager_Setting_Which_RequiresPermission_For_Authorized_User()
        {
            LoginAsDefaultTenantAdmin();

            var scripts = await _settingScriptManager.GetScriptAsync();
            scripts.ShouldContain("AbpWebCommonTestModule.Test.Setting2");
        }

        [Fact]
        public async Task SettingScriptManager_Encode_Test()
        {
            LoginAsDefaultTenantAdmin();

            var scripts = await _settingScriptManager.GetScriptAsync();
            scripts.ShouldContain("AbpWebCommonTestModule.Test.Setting3");
            scripts.ShouldContain("Test \\u003e Value3");
        }

        private void LoginAsDefaultTenantAdmin()
        {
            AbpSession.UserId = 2;
            AbpSession.TenantId = 1;
        }
    }
}
