using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Runtime.Caching.Configuration;
using Abp.Runtime.Caching.Memory;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Abp.Tests.Configuration
{
    public class SettingManager_Tests : TestBaseWithLocalIocManager
    {
        private const string MyAppLevelSetting = "MyAppLevelSetting";
        private const string MyAllLevelsSetting = "MyAllLevelsSetting";
        private const string MyNotInheritedSetting = "MyNotInheritedSetting";

        private SettingManager CreateSettingManager()
        {
            return new SettingManager(
                CreateMockSettingDefinitionManager(),
                new AbpMemoryCacheManager(
                    LocalIocManager,
                    new CachingConfiguration(Substitute.For<IAbpStartupConfiguration>())
                    )
                );
        }

        [Fact]
        public async Task Should_Get_Default_Values_With_No_Store_And_No_Session()
        {
            var settingManager = CreateSettingManager();

            (await settingManager.GetSettingValueAsync<int>(MyAppLevelSetting)).ShouldBe(42);
            (await settingManager.GetSettingValueAsync(MyAllLevelsSetting)).ShouldBe("application level default value");
        }

        [Fact]
        public async Task Should_Get_Stored_Application_Value_With_No_Session()
        {
            var settingManager = CreateSettingManager();
            settingManager.SettingStore = new MemorySettingStore();

            (await settingManager.GetSettingValueAsync<int>(MyAppLevelSetting)).ShouldBe(48);
            (await settingManager.GetSettingValueAsync(MyAllLevelsSetting)).ShouldBe("application level stored value");
        }

        [Fact]
        public async Task Should_Get_Correct_Values()
        {
            var session = new MyChangableSession();

            var settingManager = CreateSettingManager();
            settingManager.SettingStore = new MemorySettingStore();
            settingManager.AbpSession = session;

            session.TenantId = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000001");

            //Inherited setting

            session.UserId = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000001");
            (await settingManager.GetSettingValueAsync(MyAllLevelsSetting)).ShouldBe("user 1 stored value");

            session.UserId = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000002");
            (await settingManager.GetSettingValueAsync(MyAllLevelsSetting)).ShouldBe("user 2 stored value");

            session.UserId = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000003");
            (await settingManager.GetSettingValueAsync(MyAllLevelsSetting)).ShouldBe("tenant 1 stored value"); //Because no user value in the store

            session.TenantId = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000003");
            session.UserId = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000003");
            (await settingManager.GetSettingValueAsync(MyAllLevelsSetting)).ShouldBe("application level stored value"); //Because no user and tenant value in the store

            //Not inherited setting

            session.TenantId = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000001");
            session.UserId = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000001");

            (await settingManager.GetSettingValueForApplicationAsync(MyNotInheritedSetting)).ShouldBe("application value");
            (await settingManager.GetSettingValueForTenantAsync(MyNotInheritedSetting, session.TenantId.Value)).ShouldBe("default-value");
            (await settingManager.GetSettingValueAsync(MyNotInheritedSetting)).ShouldBe("default-value");
        }

        [Fact]
        public async Task Should_Get_All_Values()
        {
            var settingManager = CreateSettingManager();
            settingManager.SettingStore = new MemorySettingStore();

            (await settingManager.GetAllSettingValuesAsync()).Count.ShouldBe(3);

            (await settingManager.GetAllSettingValuesForApplicationAsync()).Count.ShouldBe(3);

            (await settingManager.GetAllSettingValuesForTenantAsync(Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000001"))).Count.ShouldBe(1);
            (await settingManager.GetAllSettingValuesForTenantAsync(Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000002"))).Count.ShouldBe(0);
            (await settingManager.GetAllSettingValuesForTenantAsync(Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000003"))).Count.ShouldBe(0);

            (await settingManager.GetAllSettingValuesForUserAsync(Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000001"))).Count.ShouldBe(1);
            (await settingManager.GetAllSettingValuesForUserAsync(Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000002"))).Count.ShouldBe(1);
            (await settingManager.GetAllSettingValuesForUserAsync(Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000003"))).Count.ShouldBe(0);
        }

        [Fact]
        public async Task Should_Change_Setting_Values()
        {
            var session = new MyChangableSession();

            var settingManager = CreateSettingManager();
            settingManager.SettingStore = new MemorySettingStore();
            settingManager.AbpSession = session;

            //Application level changes

            await settingManager.ChangeSettingForApplicationAsync(MyAppLevelSetting, "53");
            await settingManager.ChangeSettingForApplicationAsync(MyAppLevelSetting, "54");
            await settingManager.ChangeSettingForApplicationAsync(MyAllLevelsSetting, "application level changed value");

            (await settingManager.SettingStore.GetSettingOrNullAsync(null, null, MyAppLevelSetting)).Value.ShouldBe("54");

            (await settingManager.GetSettingValueAsync<int>(MyAppLevelSetting)).ShouldBe(54);
            (await settingManager.GetSettingValueAsync(MyAllLevelsSetting)).ShouldBe("application level changed value");

            //Tenant level changes

            session.TenantId = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000001");
            await settingManager.ChangeSettingForTenantAsync(Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000001"), MyAllLevelsSetting, "tenant 1 changed value");
            (await settingManager.GetSettingValueAsync(MyAllLevelsSetting)).ShouldBe("tenant 1 changed value");

            //User level changes

            session.UserId = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000001");
            await settingManager.ChangeSettingForUserAsync(Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000001"), MyAllLevelsSetting, "user 1 changed value");
            (await settingManager.GetSettingValueAsync(MyAllLevelsSetting)).ShouldBe("user 1 changed value");
        }

        [Fact]
        public async Task Should_Delete_Setting_Values_On_Default_Value()
        {
            var session = new MyChangableSession();
            var store = new MemorySettingStore();

            var settingManager = CreateSettingManager();
            settingManager.SettingStore = store;
            settingManager.AbpSession = session;

            session.TenantId = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000001");
            session.UserId = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000001");

            //We can get user's personal stored value
            (await store.GetSettingOrNullAsync(null, Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000001"), MyAllLevelsSetting)).ShouldNotBe(null);
            (await settingManager.GetSettingValueAsync(MyAllLevelsSetting)).ShouldBe("user 1 stored value");

            //This will delete setting for the user since it's same as tenant's setting value
            await settingManager.ChangeSettingForUserAsync(Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000001"), MyAllLevelsSetting, "tenant 1 stored value");
            (await store.GetSettingOrNullAsync(null, Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000001"), MyAllLevelsSetting)).ShouldBe(null);

            //We can get tenant's setting value
            (await store.GetSettingOrNullAsync(Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000001"), null, MyAllLevelsSetting)).ShouldNotBe(null);
            (await settingManager.GetSettingValueAsync(MyAllLevelsSetting)).ShouldBe("tenant 1 stored value");

            //This will delete setting for tenant since it's same as application's setting value
            await settingManager.ChangeSettingForTenantAsync(Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000001"), MyAllLevelsSetting, "application level stored value");
            (await store.GetSettingOrNullAsync(null, Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000001"), MyAllLevelsSetting)).ShouldBe(null);

            //We can get application's value
            (await store.GetSettingOrNullAsync(null, null, MyAllLevelsSetting)).ShouldNotBe(null);
            (await settingManager.GetSettingValueAsync(MyAllLevelsSetting)).ShouldBe("application level stored value");

            //This will delete setting for application since it's same as the default value of the setting
            await settingManager.ChangeSettingForApplicationAsync(MyAllLevelsSetting, "application level default value");
            (await store.GetSettingOrNullAsync(null, null, MyAllLevelsSetting)).ShouldBe(null);

            //Now, there is no setting value, default value should return
            (await settingManager.GetSettingValueAsync(MyAllLevelsSetting)).ShouldBe("application level default value");
        }

        private static ISettingDefinitionManager CreateMockSettingDefinitionManager()
        {
            var settings = new Dictionary<string, SettingDefinition>
            {
                {MyAppLevelSetting, new SettingDefinition(MyAppLevelSetting, "42")},
                {MyAllLevelsSetting, new SettingDefinition(MyAllLevelsSetting, "application level default value", scopes: SettingScopes.Application | SettingScopes.Tenant | SettingScopes.User)},
                {MyNotInheritedSetting, new SettingDefinition(MyNotInheritedSetting, "default-value", scopes: SettingScopes.Application | SettingScopes.Tenant, isInherited: false)},
            };

            var definitionManager = Substitute.For<ISettingDefinitionManager>();

            //Implement methods
            definitionManager.GetSettingDefinition(Arg.Any<string>()).Returns(x => settings[x[0].ToString()]);
            definitionManager.GetAllSettingDefinitions().Returns(settings.Values.ToList());

            return definitionManager;
        }

        private class MemorySettingStore : ISettingStore
        {
            private readonly List<SettingInfo> _settings;

            public MemorySettingStore()
            {
                _settings = new List<SettingInfo>
                {
                    new SettingInfo(null, null, MyAppLevelSetting, "48"),
                    new SettingInfo(null, null, MyAllLevelsSetting, "application level stored value"),
                    new SettingInfo(Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000001"), null, MyAllLevelsSetting, "tenant 1 stored value"),
                    new SettingInfo(null, Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000001"), MyAllLevelsSetting, "user 1 stored value"),
                    new SettingInfo(null, Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000002"), MyAllLevelsSetting, "user 2 stored value"),
                    new SettingInfo(null, null, MyNotInheritedSetting, "application value"),
                };
            }

            public Task<SettingInfo> GetSettingOrNullAsync(Guid? tenantId, Guid? userId, string name)
            {
                return Task.FromResult(_settings.FirstOrDefault(s => s.TenantId == tenantId && s.UserId == userId && s.Name == name));
            }

            public async Task DeleteAsync(SettingInfo setting)
            {
                _settings.RemoveAll(s => s.TenantId == setting.TenantId && s.UserId == setting.UserId && s.Name == setting.Name);
            }

            public async Task CreateAsync(SettingInfo setting)
            {
                _settings.Add(setting);
            }

            public async Task UpdateAsync(SettingInfo setting)
            {
                var s = await GetSettingOrNullAsync(setting.TenantId, setting.UserId, setting.Name);
                if (s != null)
                {
                    s.Value = setting.Value;
                }
            }

            public Task<List<SettingInfo>> GetAllListAsync(Guid? tenantId, Guid? userId)
            {
                return Task.FromResult(_settings.Where(s => s.TenantId == tenantId && s.UserId == userId).ToList());
            }
        }
    }
}