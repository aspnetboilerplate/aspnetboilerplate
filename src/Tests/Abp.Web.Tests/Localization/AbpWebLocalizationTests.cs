using System.Globalization;
using Adorable.Collections;
using Adorable.Localization;
using Adorable.Modules;
using Adorable.TestBase;
using Adorable.Web.Localization;
using Shouldly;
using Xunit;

namespace Adorable.Web.Tests.Localization
{
    public class AbpWebLocalizationTests : AbpIntegratedTestBase
    {
        private readonly ILocalizationManager _localizationManager;

        public AbpWebLocalizationTests()
        {
            _localizationManager = Resolve<ILocalizationManager>();
        }

        protected override void AddModules(ITypeList<AbpModule> modules)
        {
            base.AddModules(modules);
            modules.Add(typeof(AbpWebModule));
        }

        [Fact]
        public void Should_Get_Localized_Strings()
        {
            var source = _localizationManager.GetSource(AbpWebLocalizedMessages.SourceName);
            source.GetString("Yes", new CultureInfo("en-US")).ShouldBe("Yes");
            source.GetString("Yes", new CultureInfo("tr-TR")).ShouldBe("Evet");
        }
    }
}
