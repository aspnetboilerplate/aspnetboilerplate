using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abp.Localization;
using Abp.Zero.SampleApp.MultiTenancy;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.Localization
{
    public class ApplicationLanguageManager_Tests : SampleAppTestBase
    {
        private readonly Tenant _defaultTenant;
        private readonly IApplicationLanguageManager _languageManager;

        public ApplicationLanguageManager_Tests()
        {
            _defaultTenant = GetDefaultTenant();
            _languageManager = Resolve<IApplicationLanguageManager>();
        }

        [Fact]
        public async Task Should_Get_All_Host_Languages()
        {
            var languages = await _languageManager.GetLanguagesAsync(null);
            languages.Count.ShouldBe(3);
        }

        [Fact]
        public async Task Should_Get_All_Tenant_Languages()
        {
            var languages = await _languageManager.GetLanguagesAsync(_defaultTenant.Id);
            languages.Count.ShouldBe(4);
        }

        [Fact]
        public async Task Should_Add_Language_To_Host()
        {
            await _languageManager.AddAsync(new ApplicationLanguage(null, "fr", "French"));

            var languages = await _languageManager.GetLanguagesAsync(null);
            languages.Count.ShouldBe(4);
            languages.FirstOrDefault(l => l.Name == "fr").ShouldNotBeNull();
        }

        [Fact]
        public async Task Should_Add_Language_To_Tenant()
        {
            await _languageManager.AddAsync(new ApplicationLanguage(_defaultTenant.Id, "fr", "French"));

            var languages = await _languageManager.GetLanguagesAsync(_defaultTenant.Id);
            languages.Count.ShouldBe(5);
            languages.FirstOrDefault(l => l.Name == "fr").ShouldNotBeNull();
        }

        [Fact]
        public async Task Remove_Language_From_Host()
        {
            await _languageManager.RemoveAsync(null, "tr");

            var languages = await _languageManager.GetLanguagesAsync(null);
            languages.Count.ShouldBe(2);
            languages.FirstOrDefault(l => l.Name == "tr").ShouldBeNull();
        }

        [Fact]
        public async Task Remove_Language_From_Tenant()
        {
            await _languageManager.RemoveAsync(null, "tr");

            var languages = await _languageManager.GetLanguagesAsync(null);
            languages.Count.ShouldBe(2);
            languages.FirstOrDefault(l => l.Name == "tr").ShouldBeNull();
        }
    }
}