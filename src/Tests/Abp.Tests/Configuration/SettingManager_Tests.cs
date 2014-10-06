using System.Collections.Generic;
using System.Linq;
using Abp.Configuration;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Abp.Tests.Configuration
{
    public class SettingManager_Tests : TestBaseWithLocalIocManager
    {
        private const string MyAppLevelSetting = "MyAppLevelSetting";
        private const string MyAllLevelsSetting = "MyAllLevelsSetting";

        [Fact]
        public void Should_Get_Default_Values_With_No_Store_And_No_Session()
        {
            var settingManager = new SettingManager(CreateMockSettingDefinitionManager());

            settingManager.GetSettingValue<int>(MyAppLevelSetting).ShouldBe(42);
            settingManager.GetSettingValue(MyAllLevelsSetting).ShouldBe("application level default value");
        }

        [Fact]
        public void Should_Get_Stored_Application_Value_With_No_Session()
        {
            var settingManager = new SettingManager(CreateMockSettingDefinitionManager());
            settingManager.SettingStore = new MemorySettingStore();

            settingManager.GetSettingValue<int>(MyAppLevelSetting).ShouldBe(48);
            settingManager.GetSettingValue(MyAllLevelsSetting).ShouldBe("application level stored value");
        }

        [Fact]
        public void Should_Get_Correct_Values()
        {
            var session = new MyChangableSession();

            var settingManager = new SettingManager(CreateMockSettingDefinitionManager());
            settingManager.SettingStore = new MemorySettingStore();
            settingManager.Session = session;

            session.TenantId = 1;

            session.UserId = 1;
            settingManager.GetSettingValue(MyAllLevelsSetting).ShouldBe("user 1 stored value");

            session.UserId = 2;
            settingManager.GetSettingValue(MyAllLevelsSetting).ShouldBe("user 2 stored value");

            session.UserId = 3;
            settingManager.GetSettingValue(MyAllLevelsSetting).ShouldBe("tenant 1 stored value"); //Because no user value in the store

            session.TenantId = 3;
            session.UserId = 3;
            settingManager.GetSettingValue(MyAllLevelsSetting).ShouldBe("application level stored value"); //Because no user and tenant value in the store
        }

        [Fact]
        public void Should_Get_All_Values()
        {
            var settingManager = new SettingManager(CreateMockSettingDefinitionManager());
            settingManager.SettingStore = new MemorySettingStore();

            settingManager.GetAllSettingValues().Count.ShouldBe(2);

            settingManager.GetAllSettingValuesForApplication().Count.ShouldBe(2);

            settingManager.GetAllSettingValuesForTenant(1).Count.ShouldBe(1);
            settingManager.GetAllSettingValuesForTenant(2).Count.ShouldBe(0);
            settingManager.GetAllSettingValuesForTenant(3).Count.ShouldBe(0);

            settingManager.GetAllSettingValuesForUser(1).Count.ShouldBe(1);
            settingManager.GetAllSettingValuesForUser(2).Count.ShouldBe(1);
            settingManager.GetAllSettingValuesForUser(3).Count.ShouldBe(0);
        }

        [Fact]
        public void Should_Change_Setting_Values()
        {
            var session = new MyChangableSession();

            var settingManager = new SettingManager(CreateMockSettingDefinitionManager());
            settingManager.SettingStore = new MemorySettingStore();
            settingManager.Session = session;

            //Application level changes

            settingManager.ChangeSettingForApplication(MyAppLevelSetting, "53");
            settingManager.ChangeSettingForApplication(MyAllLevelsSetting, "application level changed value");

            settingManager.GetSettingValue<int>(MyAppLevelSetting).ShouldBe(53);
            settingManager.GetSettingValue(MyAllLevelsSetting).ShouldBe("application level changed value");

            //Tenant level changes

            session.TenantId = 1;
            settingManager.ChangeSettingForTenant(1, MyAllLevelsSetting, "tenant 1 changed value");
            settingManager.GetSettingValue(MyAllLevelsSetting).ShouldBe("tenant 1 changed value");

            //User level changes

            session.UserId = 1;
            settingManager.ChangeSettingForUser(1, MyAllLevelsSetting, "user 1 changed value");
            settingManager.GetSettingValue(MyAllLevelsSetting).ShouldBe("user 1 changed value");
        }

        [Fact]
        public void Should_Delete_Setting_Values_On_Default_Value()
        {
            var session = new MyChangableSession();
            var store = new MemorySettingStore();

            var settingManager = new SettingManager(CreateMockSettingDefinitionManager());
            settingManager.SettingStore = store;
            settingManager.Session = session;

            session.TenantId = 1;
            session.UserId = 1;

            //We can get user's personal stored value
            store.GetSettingOrNull(null, 1, MyAllLevelsSetting).ShouldNotBe(null);
            settingManager.GetSettingValue(MyAllLevelsSetting).ShouldBe("user 1 stored value");

            //This will delete setting for the user since it's same as tenant's setting value
            settingManager.ChangeSettingForUser(1, MyAllLevelsSetting, "tenant 1 stored value");
            store.GetSettingOrNull(null, 1, MyAllLevelsSetting).ShouldBe(null);

            //We can get tenant's setting value
            store.GetSettingOrNull(1, null, MyAllLevelsSetting).ShouldNotBe(null);
            settingManager.GetSettingValue(MyAllLevelsSetting).ShouldBe("tenant 1 stored value");

            //This will delete setting for tenant since it's same as application's setting value
            settingManager.ChangeSettingForTenant(1, MyAllLevelsSetting, "application level stored value");
            store.GetSettingOrNull(null, 1, MyAllLevelsSetting).ShouldBe(null);

            //We can get application's value
            store.GetSettingOrNull(null, null, MyAllLevelsSetting).ShouldNotBe(null);
            settingManager.GetSettingValue(MyAllLevelsSetting).ShouldBe("application level stored value");

            //This will delete setting for application since it's same as the default value of the setting
            settingManager.ChangeSettingForApplication(MyAllLevelsSetting, "application level default value");
            store.GetSettingOrNull(null, null, MyAllLevelsSetting).ShouldBe(null);

            //Now, there is no setting value, default value should return
            settingManager.GetSettingValue(MyAllLevelsSetting).ShouldBe("application level default value");
        }

        private static ISettingDefinitionManager CreateMockSettingDefinitionManager()
        {
            var settings = new Dictionary<string, SettingDefinition>
                           {
                               {MyAppLevelSetting, new SettingDefinition(MyAppLevelSetting, "42")},
                               {MyAllLevelsSetting, new SettingDefinition(MyAllLevelsSetting, "application level default value", scopes: SettingScopes.Application | SettingScopes.Tenant | SettingScopes.User)},
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
                                new SettingInfo(1, null, MyAllLevelsSetting, "tenant 1 stored value"),
                                new SettingInfo(null, 1, MyAllLevelsSetting, "user 1 stored value"),
                                new SettingInfo(null, 2, MyAllLevelsSetting, "user 2 stored value")
                            };
            }

            public SettingInfo GetSettingOrNull(int? tenantId, long? userId, string name)
            {
                return _settings.FirstOrDefault(s => s.TenantId == tenantId && s.UserId == userId && s.Name == name);
            }

            public void Delete(SettingInfo setting)
            {
                _settings.RemoveAll(s => s.TenantId == setting.TenantId && s.UserId == setting.UserId && s.Name == setting.Name);
            }

            public void Create(SettingInfo setting)
            {
                _settings.Add(setting);
            }

            public void Update(SettingInfo setting)
            {
                var s = GetSettingOrNull(setting.TenantId, setting.UserId, setting.Name);
                if (s != null)
                {
                    s.Value = setting.Value;
                }
            }

            public List<SettingInfo> GetAll(int? tenantId, long? userId)
            {
                return _settings.Where(s => s.TenantId == tenantId && s.UserId == userId).ToList();
            }
        }
    }
}