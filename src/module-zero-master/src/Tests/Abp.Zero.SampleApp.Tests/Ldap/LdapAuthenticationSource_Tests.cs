using System;
using System.Threading.Tasks;
using Abp.Authorization.Users;
using Abp.Collections;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Modules;
using Abp.Zero.Ldap;
using Abp.Zero.Ldap.Authentication;
using Abp.Zero.Ldap.Configuration;
using Abp.Zero.SampleApp.MultiTenancy;
using Abp.Zero.SampleApp.Users;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.Ldap
{
    public class LdapAuthenticationSource_Tests : SampleAppTestBase
    {
        private readonly UserManager _userManager;

        public LdapAuthenticationSource_Tests()
        {
            _userManager = Resolve<UserManager>();
        }

        protected override void AddModules(ITypeList<AbpModule> modules)
        {
            base.AddModules(modules);
            modules.Add<MyUserLoginTestModule>();
        }

        //[Fact]
        public async Task Should_Login_From_Ldap_Without_Any_Configuration()
        {
            var result = await _userManager.LoginAsync("-","-", Tenant.DefaultTenantName);
            result.Result.ShouldBe(AbpLoginResultType.Success);
        }

        //[Fact]
        public async Task Should_Not_Login_From_Ldap_If_Disabled()
        {
            var settingManager = Resolve<ISettingManager>();
            var defaultTenant = GetDefaultTenant();

            await settingManager.ChangeSettingForTenantAsync(defaultTenant.Id, LdapSettingNames.IsEnabled, "false");

            var result = await _userManager.LoginAsync("-", "-", Tenant.DefaultTenantName);
            result.Result.ShouldBe(AbpLoginResultType.InvalidUserNameOrEmailAddress);
        }

        //[Fact]
        public async Task Should_Login_From_Ldap_With_Properly_Configured()
        {
            var settingManager = Resolve<ISettingManager>();
            var defaultTenant = GetDefaultTenant();

            await settingManager.ChangeSettingForTenantAsync(defaultTenant.Id, LdapSettingNames.Domain, "-");
            await settingManager.ChangeSettingForTenantAsync(defaultTenant.Id, LdapSettingNames.UserName, "-");
            await settingManager.ChangeSettingForTenantAsync(defaultTenant.Id, LdapSettingNames.Password, "-");

            var result = await _userManager.LoginAsync("-", "-", Tenant.DefaultTenantName);
            result.Result.ShouldBe(AbpLoginResultType.Success);
        }

        //[Fact]
        public async Task Should_Not_Login_From_Ldap_With_Wrong_Configuration()
        {
            var settingManager = Resolve<ISettingManager>();
            var defaultTenant = GetDefaultTenant();

            await settingManager.ChangeSettingForTenantAsync(defaultTenant.Id, LdapSettingNames.Domain, "InvalidDomain");
            await settingManager.ChangeSettingForTenantAsync(defaultTenant.Id, LdapSettingNames.UserName, "NoUserName");
            await settingManager.ChangeSettingForTenantAsync(defaultTenant.Id, LdapSettingNames.Password, "123123123123");

            await Assert.ThrowsAnyAsync<Exception>(() => _userManager.LoginAsync("testuser", "testpass", Tenant.DefaultTenantName));
        }

        [DependsOn(typeof(AbpZeroLdapModule))]
        public class MyUserLoginTestModule : AbpModule
        {
            public override void PreInitialize()
            {
                Configuration.Modules.ZeroLdap().Enable(typeof (MyLdapAuthenticationSource));
            }

            public override void Initialize()
            {
                //This is needed just for this test, not for real apps
                IocManager.Register<MyLdapAuthenticationSource>(DependencyLifeStyle.Transient);
            }
        }

        public class MyLdapAuthenticationSource : LdapAuthenticationSource<Tenant, User>
        {
            public MyLdapAuthenticationSource(ILdapSettings settings, IAbpZeroLdapModuleConfig ldapModuleConfig)
                : base(settings, ldapModuleConfig)
            {

            }
        }
    }
}
