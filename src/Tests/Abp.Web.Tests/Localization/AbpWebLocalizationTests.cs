using System.Globalization;
using Abp.Collections;
using Abp.Localization;
using Abp.Modules;
using Abp.TestBase;
using Abp.Web.Localization;
using Shouldly;
using Xunit;

namespace Abp.Web.Tests.Localization
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
