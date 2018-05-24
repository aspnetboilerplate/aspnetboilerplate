using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.TestBase;

namespace Abp.Web.Common.Tests
{
    [DependsOn(typeof(AbpWebCommonModule), typeof(AbpTestBaseModule))]
    public class AbpWebCommonTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            base.PreInitialize();

            Configuration.Settings.Providers.Add<AbpWebCommonTestModuleSettingProvider>();
            Configuration.Authorization.Providers.Add<AbpWebCommonTestModuleAuthProvider>();
        }

        public override void Initialize()
        {
            base.Initialize();

            IocManager.RegisterAssemblyByConvention(typeof(AbpWebCommonTestModule).GetAssembly());
        }
    }
}
