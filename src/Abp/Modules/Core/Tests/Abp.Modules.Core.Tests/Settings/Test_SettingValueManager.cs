using System.Text;
using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Dependency;
using Castle.MicroKernel.Registration;
using NUnit.Framework;

namespace Abp.Modules.Core.Tests.Settings
{
    [TestFixture]
    public class Test_SettingValueManager
    {
        private MemorySettingValueRepository _repository;
        private SettingValueManager _settingValueManager;

        [TestFixtureSetUp]
        public void Initialize()
        {
            _repository = new MemorySettingValueRepository(new LongPrimaryKeyGenerator());
            _settingValueManager = new SettingValueManager(_repository, new SampleSettingDefinitionManager());
            FillTestData(_repository);
        }

        [Test]
        public void _01_Should_Get_Application_Setting_If_No_Current_User()
        {
            Assert.AreEqual("fr", _settingValueManager.GetSettingValue("Language"));
        }

        [Test]
        public void _02_Should_Get_User_Setting_If_There_Is_Current_User()
        {
            TestHelper.SetUserPrincipal(1);
            Assert.AreEqual("tr", _settingValueManager.GetSettingValue("Language"));
        }

        [Test]
        public void _03_Should_Get_Default_Value_If_No_Setting_Specified()
        {
            Assert.AreEqual("My Test Site", _settingValueManager.GetSettingValue("SiteTitle"));
        }

        [Test]
        public void _04_Should_Change_Application_Setting_With_Repository()
        {
            _settingValueManager.ChangeSettingForApplication("SiteTitle", "My Test Site v2");
            Assert.AreEqual("My Test Site v2", _settingValueManager.GetSettingValue("SiteTitle"));
            Assert.AreEqual("My Test Site v2", _repository.Single(sv => sv.Name == "SiteTitle").Value);
        }

        [Test]
        public void _05_Should_Remove_Setting_From_Repository_If_Set_To_Default_Value_For_Application()
        {
            _settingValueManager.ChangeSettingForApplication("SiteTitle", "My Test Site");
            Assert.AreEqual(null, _repository.FirstOrDefault(sv => sv.Name == "SiteTitle"));
        }

        [Test]
        public void _06_Should_Remove_Setting_From_Repository_If_Set_To_Default_Value_For_User()
        {
            _settingValueManager.ChangeSettingForUser(1, "Language", "tr");
            Assert.AreEqual("tr", _repository.FirstOrDefault(sv => sv.Name == "Language" && sv.UserId == 1).Value);

            _settingValueManager.ChangeSettingForUser(1, "Language", "fr");
            Assert.AreEqual(null, _repository.FirstOrDefault(sv => sv.Name == "Language" && sv.UserId == 1));

            _settingValueManager.ChangeSettingForUser(1, "Language", "en");
            Assert.AreEqual("en", _repository.FirstOrDefault(sv => sv.Name == "Language" && sv.UserId == 1).Value);
        }

        private static void FillTestData(MemorySettingValueRepository repository)
        {
            //Language setting for application
            repository.Insert(
                new SettingValue
                {
                    UserId = null,
                    Name = "Language",
                    Value = "fr"
                });

            //Language setting for UserId = 1
            repository.Insert(
                new SettingValue
                {
                    UserId = 1,
                    Name = "Language",
                    Value = "tr"
                });
        }
    }
}
