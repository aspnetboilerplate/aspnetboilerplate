using System.Linq;
using Abp.Localization;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Shouldly;
using Xunit;

namespace Abp.Tests.Localization
{
    public class DefaultLanguageProvider_Test : TestBaseWithLocalIocManager
    {
        public DefaultLanguageProvider_Test()
        {
            LocalIocManager.Register<ILanguageProvider, DefaultLanguageProvider>();
            var bootstrapper = AbpBootstrapper.Create<DefaultLanguageProviderLangModule>(options =>
            {
                options.IocManager = LocalIocManager;
            });

            bootstrapper.Initialize();
        }

        [Fact]
        public void Should_Get_Languages()
        {
            var languageProvider = LocalIocManager.Resolve<ILanguageProvider>();

            var allLanguages = languageProvider.GetLanguages();
            allLanguages.Count.ShouldBe(2);
        }

        [Fact]
        public void Should_Get_Active_Languages()
        {
            var languageProvider = LocalIocManager.Resolve<ILanguageProvider>();

            var activeLanguages = languageProvider.GetActiveLanguages();
            activeLanguages.Count.ShouldBe(1);
            activeLanguages.Single().Name.ShouldBe("en");
        }
    }

    public class DefaultLanguageProviderLangModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Localization.Languages.Add(new LanguageInfo("en", "English", isDefault: true));
            Configuration.Localization.Languages.Add(new LanguageInfo("tr", "Türkçe", isDisabled: true));
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(DefaultLanguageProviderLangModule).GetAssembly());
        }
    }
}
