using System.Linq;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.Configuration
{
    public class Settings_Tests : SampleAppTestBase
    {
        private readonly ISettingManager _settingManager;
        private readonly IRepository<Setting, long> _settingRepository;

        public Settings_Tests()
        {
            _settingManager = LocalIocManager.Resolve<ISettingManager>();
            _settingRepository = LocalIocManager.Resolve<IRepository<Setting, long>>();
        }

        [Fact]
        public void Should_Get_All_Settings()
        {
            var allValues = _settingManager.GetAllSettingValues();
            allValues.Any(v => v.Name == "Setting1").ShouldBe(true);
            allValues.Any(v => v.Name == "Setting2").ShouldBe(true);
        }

        [Fact]
        public void Should_Get_Default_Value_For_Setting1()
        {
            _settingManager.GetSettingValue<int>("Setting1").ShouldBe(1);
        }

        [Fact]
        public void Should_Change_Setting2()
        {
            //Check default value
            _settingManager.GetSettingValue("Setting2").ShouldBe("A");

            //Change it to a custom value
            _settingManager.ChangeSettingForApplication("Setting2", "B");

            //Check value from manager
            _settingManager.GetSettingValue("Setting2").ShouldBe("B");

            //Check value from repository
            var setting2 = _settingRepository.FirstOrDefault(s => s.TenantId == null && s.UserId == null && s.Name == "Setting2");
            setting2.ShouldNotBe(null);
            setting2.Value.ShouldBe("B");

            //Set it again to default value
            _settingManager.ChangeSettingForApplication("Setting2", "A");

            //Setting to default value cause the setting will be deleted from database
            _settingRepository.FirstOrDefault(s => s.TenantId == null && s.UserId == null && s.Name == "Setting2").ShouldBe(null);

            //Check value again from manager
            _settingManager.GetSettingValue("Setting2").ShouldBe("A");
        }
    }
}
